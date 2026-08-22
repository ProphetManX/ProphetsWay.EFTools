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
	/// The keyless base for an entity that is <b>stamped as deleted</b> rather than removed, with — like
	/// <see cref="RootNonIdDao{TEntity}"/> — <b>no capability interface</b>.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <remarks>
	/// <para>
	/// <b>Why it exists.</b> Without it the only route to keyless soft delete ran through
	/// <see cref="BaseNonIdDao{TEntity}"/>, i.e. through <see cref="IBaseDao{T}"/> — so a join table that keeps
	/// its history rather than removing rows was forced to publish <c>Get</c> and <c>Update</c> it does not
	/// support.
	/// </para>
	/// <para>
	/// <b>Every difference from <see cref="RootNonIdDao{TEntity}"/> is an <c>override</c> and never a
	/// <c>new</c> member</b>, so a soft Data Access Object reached through a
	/// <see cref="RootNonIdDao{TEntity}"/>-typed reference still soft-deletes. A hidden <c>Delete</c> would
	/// hard-delete a soft entity through an upcast — silent data loss no consumer would suspect.
	/// </para>
	/// <para>
	/// <b>Soft deletion is the only exclusion rule.</b> <see cref="ApplyReadFilter"/> adds
	/// <c>DeletedDate == null</c> and nothing else, so <see cref="GetAll"/>, <see cref="GetPaged"/> and
	/// <c>GetCount</c> agree with one another, and <see cref="GetCore"/> — which is not filtered — still finds a
	/// deleted row.
	/// </para>
	/// <para>
	/// <b>The two timestamp hooks are one policy stated from two directions, and the two must never state
	/// disagreeing policies. Override both, or neither</b> — with one carve-out, which is part of the rule and
	/// not an exception to it: <b>overriding <see cref="GetCurrentTimestamp"/> alone is conforming provided the
	/// replacement clock still yields <see cref="DateTimeKind.Utc"/></b>, which the default normalizer already
	/// agrees with. This is the second of the rule's two declaration sites, and no compiler check spans them.
	/// The defaults come from one <c>internal static</c> helper so the two sites cannot drift.
	/// </para>
	/// <para>
	/// <b>Not thread-safe.</b> Every Data Access Object on a layer shares one <see cref="DbContext"/>.
	/// </para>
	/// </remarks>
	public abstract class RootSoftNonIdDao<TEntity> : RootNonIdDao<TEntity>
		where TEntity : class, IBaseSoftEntity
	{
		/// <inheritdoc />
		protected RootSoftNonIdDao(DbContext context) : base(context)
		{
		}

		/// <summary>Supplies the timestamp every stamping member writes.</summary>
		/// <returns>The current time. The default is <see cref="DateTime.UtcNow"/>.</returns>
		/// <remarks>
		/// Read <b>once per stamping operation</b>, and the one reading serves every object that operation
		/// writes it to — the stored row and the caller's instance alike. Bound to
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
		/// The default is <c>DateTime.SpecifyKind(value, DateTimeKind.Utc)</c>. It <b>relabels; it never
		/// shifts</b> — the returned value's <see cref="DateTime.Ticks"/> equal the input's. Applied to the three
		/// timestamps of every entity <see cref="GetCore"/>, <see cref="GetAll"/> and <see cref="GetPaged"/>
		/// materialize, after materialization and never inside a predicate, so it cannot affect translation.
		/// <b>A soft entity arriving as an <c>Include</c> on another Data Access Object's query is not
		/// reached</b> — that Data Access Object owns the query and cannot see this hook. Values written back by
		/// a write are not normalized either: they came from <see cref="GetCurrentTimestamp"/>, never went to the
		/// store, and never lost their kind.
		/// </remarks>
		protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value)
		{
			return SoftTimestamps.AsUtc(value);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// Stamps <c>CreatedDate</c> and forces <c>UpdatedDate</c> and <c>DeletedDate</c> to <c>null</c>,
		/// <b>whatever the caller assigned</b>. The copy carries all three, so the stored row does; the same
		/// three are assigned onto <paramref name="item"/>, from <b>one</b> reading of the clock. They are never
		/// read back off the store, which is what keeps a provider-stripped kind off a caller's instance.
		/// </para>
		/// <para>
		/// <b>The write-back onto <paramref name="item"/> happens only where a row was written.</b> A
		/// <c>SaveChanges</c> that throws leaves <paramref name="item"/> carrying the values it arrived with — no
		/// <c>CreatedDate</c>, and no <c>UpdatedDate</c>/<c>DeletedDate</c> nulled — because a caller told nothing
		/// was stored must not be holding an instance that says a row was.
		/// </para>
		/// <para>
		/// <b>No identifier is written back</b>, on this class as on its base — there is none.
		/// </para>
		/// </remarks>
		public override void Insert(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			InsertRoot(item, GetCurrentTimestamp());
		}

		/// <inheritdoc />
		/// <returns>
		/// <c>1</c> when a <b>live</b> row matched, <c>0</c> when the matched row was already deleted or none
		/// matched.
		/// </returns>
		/// <remarks>
		/// <b>Does not remove the row.</b> Stamps <c>DeletedDate</c> and writes that stamp back onto
		/// <paramref name="item"/>; <c>CreatedDate</c> and <c>UpdatedDate</c> are not touched, and the row stays
		/// retrievable through <see cref="GetCore"/>. A row that is already deleted, or absent, returns <c>0</c>
		/// and changes nothing — <b>an existing <c>DeletedDate</c> is never refreshed</b>, so the stamp always
		/// reports when the row was actually deleted and a second call is idempotent rather than an error. No
		/// write-back onto <paramref name="item"/> occurs when it returns <c>0</c>.
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
		/// Adds <c>DeletedDate == null</c> and nothing else. It reaches the retrieval trio only —
		/// <see cref="GetCore"/> and every locating fetch a write makes start from the raw
		/// <see cref="RootNonIdDao{TEntity}.Dataset"/>, or a soft Data Access Object could never reach the rows
		/// it had already deleted.
		/// </remarks>
		protected override IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query)
		{
			return base.ApplyReadFilter(query).Where(x => x.DeletedDate == null);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <b>Returns soft-deleted rows</b>, answering <c>null</c> only where no matching row was ever stored —
		/// inherited rather than added, since the base member never applies <see cref="ApplyReadFilter"/> and
		/// locates with <c>IgnoreQueryFilters()</c>. <b>This override exists to run
		/// <see cref="NormalizeRetrievedTimestamp"/> over what the base member found</b>, and for nothing else.
		/// </remarks>
		protected override TEntity? GetCore(TEntity item)
		{
			return SoftTimestamps.Normalize(base.GetCore(item), NormalizeRetrievedTimestamp);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// Stamps <c>UpdatedDate</c> and writes the entity's own data only: an incoming <c>CreatedDate</c>,
		/// <c>UpdatedDate</c> and <c>DeletedDate</c> are all ignored, and the stored <c>CreatedDate</c> and
		/// <c>DeletedDate</c> are preserved — so an update can neither rewrite history nor soft-delete a row
		/// behind <see cref="Delete"/>'s back, in either direction. <b>Updating a deleted row is allowed</b> and
		/// leaves it deleted.
		/// </para>
		/// <para>
		/// The three timestamps are restored from the tracked entry immediately after the <c>SetValues</c> copy,
		/// and <b>the entity's key properties are excluded from it</b>, the two sets composing by union. Only
		/// <c>UpdatedDate</c> travels back onto <paramref name="item"/>, and only where a row was written: a call
		/// returning <c>0</c>, or one whose <c>SaveChanges</c> throws, leaves the caller's <c>UpdatedDate</c> as
		/// it was found.
		/// </para>
		/// <para>
		/// <b>Not sealed</b>, and reached through a <see cref="RootNonIdDao{TEntity}"/>-typed reference by
		/// virtual dispatch, so a base-typed caller cannot get the hard body.
		/// </para>
		/// </remarks>
		protected override int UpdateCore(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			// The stamp is placed on item before the base member copies its values, so the stored row and the
			// caller's instance carry one reading of the clock. The other two never leave the tracked entry.
			var carried = item.UpdatedDate;
			var written = 0;

			item.UpdatedDate = GetCurrentTimestamp();

			try
			{
				written = base.UpdateCore(item);
			}
			finally
			{
				// No row matched, or the write threw. Either way the caller's instance is left as it was found.
				if (written == 0)
					item.UpdatedDate = carried;
			}

			return written;
		}

		/// <inheritdoc />
		/// <remarks>
		/// The two timestamps <see cref="UpdateCore"/> does not own are read off the tracked entry and put back
		/// immediately after the copy, so neither can arrive from <paramref name="item"/>. Restoring rather than
		/// excluding is safe here only because a timestamp is not a key property: the key-is-read-only exception
		/// is raised <i>during</i> the copy, so a key has no "after" to be restored in, while a timestamp does.
		/// </remarks>
		private protected override void ApplyUpdateValues(EntityEntry<TEntity> entry, TEntity item)
		{
			var created = entry.Entity.CreatedDate;
			var deleted = entry.Entity.DeletedDate;

			base.ApplyUpdateValues(entry, item);

			entry.Entity.CreatedDate = created;
			entry.Entity.DeletedDate = deleted;
		}
	}
}
