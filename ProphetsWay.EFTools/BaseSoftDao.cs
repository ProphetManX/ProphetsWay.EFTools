#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The Entity Framework Core base for a Data Access Object over an entity that is <b>stamped as deleted</b>
	/// rather than removed.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">The declared type of the identifier property.</typeparam>
	/// <remarks>
	/// <para>
	/// Every difference from <see cref="BaseDao{TEntity, TKey}"/> is an <c>override</c> and never a <c>new</c>
	/// member, so a soft Data Access Object reached through a <see cref="BaseDao{TEntity, TKey}"/>-typed
	/// reference still soft-deletes. A hidden <c>Delete</c> would hard-delete a soft entity through an upcast —
	/// silent data loss no consumer would suspect.
	/// </para>
	/// <para>
	/// <b>Soft deletion is the only exclusion rule.</b> <see cref="ApplyReadFilter"/> adds
	/// <c>DeletedDate == null</c> and nothing else, so <see cref="GetAll"/>, <see cref="GetPaged"/> and
	/// <c>GetCount</c> agree with one another and <see cref="Get"/> — which is not filtered — still finds a
	/// deleted row.
	/// </para>
	/// <para>
	/// <b>The two timestamp hooks are one policy stated from two directions, and the two must never state
	/// disagreeing policies. Override both, or neither</b> — with one carve-out, which is part of the rule and
	/// not an exception to it: <b>overriding <see cref="GetCurrentTimestamp"/> alone is conforming provided the
	/// replacement clock still yields <see cref="DateTimeKind.Utc"/></b>, which the default normalizer already
	/// agrees with. Injecting a fixed-instant test clock is that case. Overriding the clock to a policy that
	/// does <i>not</i> yield UTC, or overriding <see cref="NormalizeRetrievedTimestamp"/> alone, leaves a stamp
	/// relabeled <see cref="DateTimeKind.Utc"/> on retrieval while holding a reading from some other clock — an
	/// instant wrong by that clock's offset, with nothing to indicate it. The library checks none of this: it
	/// never inspects what the clock returns and never compares one override against the other, so this is an
	/// obligation on the deriving Data Access Object rather than something enforced here.
	/// </para>
	/// <para>
	/// <b>Not thread-safe.</b> Every Data Access Object on a layer shares one <see cref="DbContext"/>.
	/// </para>
	/// </remarks>
	public abstract class BaseSoftDao<TEntity, TKey> : BaseDao<TEntity, TKey>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
		/// <inheritdoc />
		protected BaseSoftDao(DbContext context) : base(context)
		{
		}

		/// <summary>Supplies the timestamp every stamping member writes.</summary>
		/// <returns>The current time. The default is <see cref="DateTime.UtcNow"/>.</returns>
		/// <remarks>
		/// Read <b>once per stamping operation</b>, and the one reading serves every object that operation writes
		/// it to — the stored row and the caller's instance alike. Bound to
		/// <see cref="NormalizeRetrievedTimestamp"/> by the Timestamp Pair Rule: override both, or neither,
		/// <b>unless the replacement clock still yields <see cref="DateTimeKind.Utc"/></b>, which the default
		/// normalizer already agrees with.
		/// </remarks>
		protected virtual DateTime GetCurrentTimestamp()
		{
			return SoftTimestamps.Now();
		}

		/// <summary>
		/// Restores the <see cref="DateTimeKind"/> a relational store did not preserve, on a timestamp read back
		/// out of the store by <b>this</b> Data Access Object.
		/// </summary>
		/// <param name="value">A timestamp as the provider materialized it — typically <see cref="DateTimeKind.Unspecified"/>.</param>
		/// <returns>The same instant, carrying the kind this Data Access Object's clock produces.</returns>
		/// <remarks>
		/// <para>
		/// The default is <c>DateTime.SpecifyKind(value, DateTimeKind.Utc)</c>. It <b>relabels; it never shifts.</b>
		/// The returned value's <see cref="DateTime.Ticks"/> equal the input's — the same instant, differently
		/// labeled. Bound to <see cref="GetCurrentTimestamp"/> by the Timestamp Pair Rule: override both, or
		/// neither, unless the clock override still yields <see cref="DateTimeKind.Utc"/>.
		/// </para>
		/// <para>
		/// Applied to the three timestamps of every entity <see cref="Get"/>, <see cref="GetAll"/> and
		/// <see cref="GetPaged"/> materialize, after materialization and never inside a predicate, so it cannot
		/// affect translation. <b>A soft entity arriving as an <c>Include</c> on another Data Access Object's
		/// query is not reached</b> — that Data Access Object owns the query and cannot see this hook. Values
		/// written back by a write are not normalized either: they came from <see cref="GetCurrentTimestamp"/>,
		/// never went to the store, and never lost their kind.
		/// </para>
		/// </remarks>
		protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value)
		{
			return SoftTimestamps.AsUtc(value);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <b>Returns soft-deleted rows</b>, answering <c>null</c> only where no row with that identifier was ever
		/// stored. That is inherited rather than added: the base member never applies
		/// <see cref="ApplyReadFilter"/> and locates with <c>IgnoreQueryFilters()</c>. <b>This override exists to
		/// run <see cref="NormalizeRetrievedTimestamp"/> over what the base member found</b>, and for nothing
		/// else — delete it and the timestamps come back carrying whatever kind the provider supplied.
		/// </remarks>
		public override TEntity? Get(TEntity item)
		{
			return SoftTimestamps.Normalize(base.Get(item), NormalizeRetrievedTimestamp);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// Stamps <c>CreatedDate</c> and forces <c>UpdatedDate</c> and <c>DeletedDate</c> to <c>null</c>,
		/// <b>whatever the caller assigned</b>. All three are visible on <paramref name="item"/> when the call
		/// returns, alongside the identifier, and the stored row carries the same three — one reading of the clock
		/// written to both objects. They are never read back off the store, which is what keeps a
		/// provider-stripped kind off a caller's instance.
		/// </para>
		/// <para>
		/// <b>The write-back onto <paramref name="item"/> is owed only where a row was stored, and it is owed as
		/// soon as one is.</b> The copy is stamped before the write and <paramref name="item"/> only after
		/// <c>SaveChanges</c> has returned, so an <c>Insert</c> that stores nothing leaves the caller's instance
		/// carrying exactly the values it arrived with — no stamp, no nulled timestamp, no identifier — and an
		/// <c>Insert</c> that stores a row and then throws from a later step leaves the instance agreeing with
		/// that row. A caller told nothing was stored must not hold an instance saying a row was, and a caller
		/// holding a real identifier must not hold timestamps that identifier's row does not have. See finding
		/// F10 in <c>docs/api-contract.md</c>.
		/// </para>
		/// </remarks>
		public override void Insert(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			InsertRoot(item, GetCurrentTimestamp());
		}

		/// <inheritdoc />
		/// <remarks>
		/// Stamps <c>UpdatedDate</c> and writes the entity's own data only: an incoming <c>CreatedDate</c>,
		/// <c>UpdatedDate</c> and <c>DeletedDate</c> are all ignored, and the stored <c>CreatedDate</c> and
		/// <c>DeletedDate</c> are preserved — so an update can neither rewrite history nor soft-delete a row
		/// behind <see cref="Delete"/>'s back, in either direction. <b>Updating a deleted row is allowed</b> and
		/// leaves it deleted. Only <c>UpdatedDate</c> travels back onto <paramref name="item"/>, and only once the
		/// row carrying that same stamp has been written — the tracked row is stamped, never
		/// <paramref name="item"/> ahead of the write.
		/// </remarks>
		public override int Update(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			return UpdateRoot(item, GetCurrentTimestamp());
		}

		/// <inheritdoc />
		/// <remarks>
		/// <b>Does not remove the row.</b> Stamps <c>DeletedDate</c> and returns <c>1</c> where a <b>live</b> row
		/// with that identifier is stored, writing the stamp back onto <paramref name="item"/>; <c>CreatedDate</c>
		/// and <c>UpdatedDate</c> are not touched, and the row stays retrievable through <see cref="Get"/>. A row
		/// that is already deleted, or absent, returns <c>0</c> and changes nothing — <b>an existing
		/// <c>DeletedDate</c> is never refreshed</b>, so the stamp always reports when the row was actually
		/// deleted and a second call is idempotent rather than an error.
		/// </remarks>
		public override int Delete(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = TrackForWrite(item);

			if (stored == null)
				return 0;

			try
			{
				if (stored.DeletedDate.HasValue)
					return 0;

				var stamp = GetCurrentTimestamp();

				stored.DeletedDate = stamp;

				Context.SaveChanges();

				item.DeletedDate = stamp;

				return 1;
			}
			finally
			{
				// Every exit owes this, the two early returns included: the entry is tracked from the moment
				// TrackForWrite returns, and one left behind is flushed by the next write on the shared context.
				// The query declares no Include, so the one entity is the whole reachable graph.
				Context.Entry(stored).State = EntityState.Detached;
			}
		}

		/// <inheritdoc />
		/// <remarks>Omits soft-deleted rows, and normalizes the timestamps of the rows it returns.</remarks>
		public override IList<TEntity> GetAll(TEntity? item)
		{
			return SoftTimestamps.Normalize(base.GetAll(item), NormalizeRetrievedTimestamp);
		}

		/// <inheritdoc />
		/// <remarks>Omits soft-deleted rows, and normalizes the timestamps of the rows it returns.</remarks>
		public override IList<TEntity> GetPaged(TEntity? item, int skip, int take)
		{
			return SoftTimestamps.Normalize(base.GetPaged(item, skip, take), NormalizeRetrievedTimestamp);
		}

		/// <inheritdoc />
		/// <remarks>
		/// Adds <c>DeletedDate == null</c> and nothing else. It reaches the retrieval trio only — <c>Get</c> and
		/// every locating fetch a write makes start from the raw <see cref="BaseDao{TEntity, TKey}.Dataset"/>, or
		/// a soft Data Access Object could never reach the rows it had already deleted.
		/// </remarks>
		protected override IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query)
		{
			return base.ApplyReadFilter(query).Where(x => x.DeletedDate == null);
		}

		/// <inheritdoc />
		/// <remarks>
		/// The two timestamps <see cref="Update"/> does not own are read off the tracked entry and put back
		/// immediately after the copy, so neither can arrive from <paramref name="item"/>. Restoring rather than
		/// excluding is safe here only because a timestamp is not a key property: the key-is-read-only exception
		/// is raised <i>during</i> the copy, so a key has no "after" to be restored in, while a timestamp does.
		/// </remarks>
		protected override void ApplyUpdateValues(EntityEntry<TEntity> entry, TEntity item)
		{
			var created = entry.Entity.CreatedDate;
			var deleted = entry.Entity.DeletedDate;

			base.ApplyUpdateValues(entry, item);

			entry.Entity.CreatedDate = created;
			entry.Entity.DeletedDate = deleted;
		}
	}
}
