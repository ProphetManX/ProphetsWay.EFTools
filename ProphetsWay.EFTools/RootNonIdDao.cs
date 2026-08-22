#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The Entity Framework Core base for a Data Access Object over an entity that has <b>no single stored
	/// identifier property</b> — a join table keyed by a pair, a composite key, or an identity that is
	/// computed rather than stored.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <remarks>
	/// <para>
	/// <b>It implements no <c>ProphetsWay.BaseDataAccess</c> capability interface, so deriving from it commits
	/// you to nothing.</b> Your own Data Access Object interface declares the subset you support, and the
	/// public members below satisfy those declarations implicitly because the signatures match. Deriving from
	/// <see cref="BaseNonIdDao{TEntity}"/> instead is how you opt in to <see cref="IBaseDao{T}"/>.
	/// </para>
	/// <para>
	/// <b><see cref="MatchRow"/> replaces the whole identifier apparatus, and it is the only abstract member
	/// in the library.</b> There is no <c>{TypeName}Id</c>/<c>Id</c> resolution, no <c>TKey</c>, no
	/// <c>GetKey</c>, no <c>KeyEquals</c> and no <c>KeySelector</c>: with no identifier there is nothing to
	/// resolve and nothing to derive a predicate from. A constructor on this family therefore <b>never throws
	/// <see cref="DataAccessConventionException"/></b>.
	/// </para>
	/// <para>
	/// <b>Publishing a read member costs one more override.</b> <see cref="ApplyStableOrder"/>'s default
	/// throws <see cref="NotSupportedException"/>, so <see cref="GetAll"/>, <see cref="GetPaged"/> and
	/// <see cref="GetCount"/> throw until it is supplied. <see cref="Insert"/>, <see cref="Delete"/>,
	/// <see cref="GetCore"/> and <see cref="UpdateCore"/> order nothing and are unaffected — a write-only join
	/// Data Access Object is fully functional with <see cref="MatchRow"/> alone.
	/// </para>
	/// <para>
	/// <b>Not thread-safe.</b> Every Data Access Object on a layer shares one <see cref="DbContext"/>.
	/// </para>
	/// </remarks>
	public abstract class RootNonIdDao<TEntity>
		where TEntity : class, IBaseEntity
	{
		private DbSet<TEntity>? _dataset;

		/// <summary>Captures the context every member reads and writes through.</summary>
		/// <param name="context">The context this Data Access Object's layer owns.</param>
		/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
		/// <remarks>
		/// Nothing else happens here — no <c>Set&lt;TEntity&gt;()</c>, no <see cref="DbContext.Model"/> access,
		/// no query and no connection. <b>And no convention validation</b>: unlike the keyed families there is no
		/// identifier to validate, so this constructor has exactly one failure mode.
		/// </remarks>
		protected RootNonIdDao(DbContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Context = context;
		}

		/// <summary>The context shared by every Data Access Object on the layer.</summary>
		protected DbContext Context { get; }

		/// <summary>
		/// The set this Data Access Object reads and writes. Resolved on first access rather than in the
		/// constructor, because <see cref="DbContext.Set{TEntity}()"/> forces the whole model to be built.
		/// </summary>
		protected DbSet<TEntity> Dataset => _dataset ??= Context.Set<TEntity>();

		/// <summary>Stores a new row carrying <paramref name="item"/>'s values.</summary>
		/// <param name="item">The entity to store. Read, never adopted.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// <para>
		/// The store receives a <b>copy</b> of <paramref name="item"/>, so the caller's instance is never handed
		/// to the change tracker. Everything reachable through <paramref name="item"/>'s navigation properties is
		/// attached <c>Unchanged</c> — related rows are read, never written. The copy is removed from any inverse
		/// navigation relationship fix-up wrote it into, and the copy and the whole reachable graph are detached
		/// in a <c>finally</c>, on success and on failure.
		/// </para>
		/// <para>
		/// <b>Nothing is written back onto <paramref name="item"/> on this class</b> — there is no identifier to
		/// assign. The two soft descendants write the three timestamps back and say so on their own
		/// <see cref="RootSoftNonIdDao{TEntity}.Insert"/>.
		/// </para>
		/// <para>
		/// <b>Not idempotent, and not an upsert.</b> Two calls store two rows unless a store constraint prevents
		/// it, and a row the store already holds is a duplicate: the provider's uniqueness or primary-key
		/// violation propagates <b>unwrapped</b>. A Data Access Object whose own contract requires a silent no-op
		/// overrides this member.
		/// </para>
		/// </remarks>
		public virtual void Insert(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			InsertRoot(item, null);
		}

		/// <summary>Removes the stored row <paramref name="item"/> names.</summary>
		/// <param name="item">The entity naming the row. Only the values <see cref="MatchRow"/> reads matter.</param>
		/// <returns><c>1</c> when the row existed, <c>0</c> when it did not. Never negative, never above <c>1</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <exception cref="DbUpdateConcurrencyException">
		/// Another connection removed the located row between the locating fetch and <c>SaveChanges</c>. A lost
		/// race is <b>not</b> converted to <c>0</c> — the two answer different questions.
		/// </exception>
		/// <remarks>
		/// <b>A hard delete on this class</b> — the row is genuinely removed — and <b>idempotent</b>: a second
		/// call returns <c>0</c> and throws nothing. The row is located through <see cref="MatchRow"/> with
		/// <c>IgnoreQueryFilters()</c> and the <i>located</i> instance is removed, never <paramref name="item"/>.
		/// Any entry already tracked for that row is detached first, by <c>MatchRow(item).Compile()</c> — there is
		/// no resolved key to match on instead, which is what puts the purity constraint on
		/// <see cref="MatchRow"/>.
		/// </remarks>
		public virtual int Delete(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = TrackForWrite(item);

			if (stored == null)
				return 0;

			try
			{
				Dataset.Remove(stored);
				Context.SaveChanges();

				return 1;
			}
			finally
			{
				DetachAfterWrite(stored, item);
			}
		}

		/// <summary>Every row <see cref="ApplyReadFilter"/> admits, in <see cref="ApplyStableOrder"/>'s order.</summary>
		/// <param name="item">
		/// A type selector only. It is never read, and is <c>null</c> whenever the call arrives through the
		/// dispatcher.
		/// </param>
		/// <returns>A fresh list of untracked snapshots, empty rather than <c>null</c> when nothing matches.</returns>
		/// <exception cref="NotSupportedException">
		/// <see cref="ApplyStableOrder"/> has not been overridden. <b>This is the ordinary state of the base
		/// class</b>, not an edge case.
		/// </exception>
		public virtual IList<TEntity> GetAll(TEntity? item)
		{
			return ApplyStableOrder(ApplyIncludes(ApplyReadFilter(Dataset)))
				.AsNoTracking()
				.ToList();
		}

		/// <summary>
		/// The <paramref name="skip"/>/<paramref name="take"/> window over the same filtered, ordered sequence
		/// <see cref="GetAll"/> returns, so successive windows partition a full pass with no overlap and no
		/// omission.
		/// </summary>
		/// <param name="item">A type selector only; never read.</param>
		/// <param name="skip">How many rows to pass over.</param>
		/// <param name="take">How many rows to return. Zero returns an empty list.</param>
		/// <returns>A fresh list, empty rather than <c>null</c> when the window falls beyond the data.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="skip"/> or <paramref name="take"/> is negative.</exception>
		/// <exception cref="NotSupportedException">
		/// <see cref="ApplyStableOrder"/> has not been overridden. <b>Thrown after the argument checks</b> — a
		/// negative <paramref name="skip"/> on an un-overridden Data Access Object yields
		/// <see cref="ArgumentOutOfRangeException"/>, not this.
		/// </exception>
		/// <remarks>
		/// <b>This is the member that obliges <see cref="ApplyStableOrder"/> to be a genuine total order.</b> On
		/// the keyless families there is no model-derived fallback to lean on, so totality is entirely the
		/// override's to deliver.
		/// </remarks>
		public virtual IList<TEntity> GetPaged(TEntity? item, int skip, int take)
		{
			if (skip < 0)
				throw new ArgumentOutOfRangeException(nameof(skip), skip, "A page offset cannot be negative.");

			if (take < 0)
				throw new ArgumentOutOfRangeException(nameof(take), take, "A page size cannot be negative.");

			return ApplyStableOrder(ApplyIncludes(ApplyReadFilter(Dataset)))
				.Skip(skip)
				.Take(take)
				.AsNoTracking()
				.ToList();
		}

		/// <summary>
		/// How many rows <see cref="ApplyReadFilter"/> admits — the same count <see cref="GetAll"/> returns,
		/// which is what makes a pager's last page correct.
		/// </summary>
		/// <param name="item">A type selector only; never read.</param>
		/// <returns>The number of admitted rows.</returns>
		/// <exception cref="NotSupportedException">
		/// <see cref="ApplyStableOrder"/> has not been overridden. <b>It throws even though counting needs no
		/// order</b>: the trio is contractually bound to agree, and a <see cref="GetCount"/> that worked while
		/// its two partners threw would invite a pager that cannot fetch a page.
		/// </exception>
		/// <remarks>
		/// Materializes no entity, so it includes nothing and emits no <c>ORDER BY</c>.
		/// <see cref="ApplyStableOrder"/> is nevertheless <b>invoked and its result discarded</b>, which is the
		/// mechanism by which the exception above is reached — and which means an override with a side effect
		/// runs exactly once per call.
		/// </remarks>
		public virtual int GetCount(TEntity? item)
		{
			var filtered = ApplyReadFilter(Dataset);

			ApplyStableOrder(filtered);

			return filtered.Count();
		}

		/// <summary>The predicate identifying the one stored row that corresponds to <paramref name="item"/>.</summary>
		/// <param name="item">The entity naming the row.</param>
		/// <returns>A predicate matching <b>at most one</b> stored row.</returns>
		/// <remarks>
		/// <para>
		/// <b>Abstract, and the only abstract member in the library.</b> With no identifier there is nothing to
		/// derive a predicate from — only the deriving Data Access Object knows that a <c>CompanyResource</c> is
		/// matched on <c>CompanyId &amp;&amp; ResourceId</c>. <see cref="Delete"/>, <see cref="GetCore"/> and
		/// <see cref="UpdateCore"/> are all built on it.
		/// </para>
		/// <para>
		/// <b>It must be evaluable in memory over the entity's own mapped scalars</b> — no navigation traversal,
		/// no <c>EF.Functions.*</c>, no store-only construct. <see cref="Delete"/> and <see cref="UpdateCore"/>
		/// compile it for the pre-detach, which runs against objects already in the change tracker.
		/// </para>
		/// <para>
		/// <b>An override matching several rows is the keyless equivalent of a non-unique key.</b> The public
		/// members validate <c>null</c> before reaching this member, so an override need not re-check
		/// <paramref name="item"/>.
		/// </para>
		/// </remarks>
		protected abstract Expression<Func<TEntity, bool>> MatchRow(TEntity item);

		/// <summary>A total ordering over the set, applied by <see cref="GetAll"/> and <see cref="GetPaged"/> alike.</summary>
		/// <param name="query">The filtered, included query.</param>
		/// <returns>The ordered query.</returns>
		/// <exception cref="NotSupportedException">
		/// <b>Always, on this class.</b> The message names the Data Access Object type, the entity type, and the
		/// hook that is missing.
		/// </exception>
		/// <remarks>
		/// <b>Virtual with a throwing default rather than abstract:</b> an abstract hook would tax the write-only
		/// join Data Access Object with inventing an ordering over a set it never enumerates, to buy a compile
		/// error for a member it does not publish. <b>The keyed half's model-derived default has no counterpart
		/// here</b> — there is no <c>KeySelector</c> to lead with, so a model-derived ordering would be the whole
		/// order rather than a tie-breaker on one, and these families exist to serve types the model may map with
		/// <c>HasNoKey()</c>.
		/// </remarks>
		protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query)
		{
			throw new NotSupportedException(
				$"{GetType().Name} publishes a retrieval member but does not override ApplyStableOrder. " +
				$"Override it to return a total ordering over {typeof(TEntity).Name}.");
		}

		/// <summary>Restricts which stored rows the retrieval members may see. Hides nothing on this class.</summary>
		/// <param name="query">The raw <see cref="Dataset"/> query.</param>
		/// <returns>The restricted query, which <see cref="ApplyIncludes"/> is then handed.</returns>
		/// <remarks>
		/// Reaches <see cref="GetAll"/>, <see cref="GetPaged"/> and <see cref="GetCount"/> only.
		/// <see cref="GetCore"/> and every locating fetch a write makes start from the raw
		/// <see cref="Dataset"/>.
		/// </remarks>
		protected virtual IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query)
		{
			return query;
		}

		/// <summary>Declares which navigation properties a read materializes. Loads none on this class.</summary>
		/// <param name="query">
		/// The row-restricted query for its path — <see cref="ApplyReadFilter"/>'s output on
		/// <see cref="GetAll"/>/<see cref="GetPaged"/>, the <see cref="MatchRow"/>-matched query on
		/// <see cref="GetCore"/>.
		/// </param>
		/// <returns>The query with whatever <c>Include</c>/<c>ThenInclude</c> this Data Access Object promises.</returns>
		/// <remarks>Not applied by <see cref="GetCount"/>, which materializes no entity.</remarks>
		protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
		{
			return query;
		}

		/// <summary>Retrieves the single row <see cref="MatchRow"/> names.</summary>
		/// <param name="item">The entity naming the row. Only the values <see cref="MatchRow"/> reads matter.</param>
		/// <returns>A fresh untracked snapshot, or <c>null</c> when no row matches.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <exception cref="InvalidOperationException"><see cref="MatchRow"/> matched more than one row.</exception>
		/// <remarks>
		/// <b>Protected because a <c>Root</c> type publishes nothing</b> — <see cref="BaseNonIdDao{TEntity}"/>
		/// publishes it as <c>Get</c>. Never returns <paramref name="item"/> and never the store's own tracked
		/// object: it applies <c>IgnoreQueryFilters()</c> before the predicate, then <see cref="ApplyIncludes"/>,
		/// then <c>AsNoTracking()</c>. <b>It does not apply <see cref="ApplyReadFilter"/></b>, which is what lets
		/// a soft descendant still find a row it has already deleted.
		/// </remarks>
		protected virtual TEntity? GetCore(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			return ApplyIncludes(Dataset.IgnoreQueryFilters().Where(MatchRow(item)))
				.AsNoTracking()
				.SingleOrDefault();
		}

		/// <summary>Writes <paramref name="item"/>'s values onto the stored row <see cref="MatchRow"/> names.</summary>
		/// <param name="item">The entity supplying the values.</param>
		/// <returns><c>1</c> when a row matched, <c>0</c> when none did. Never negative, never above <c>1</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <exception cref="InvalidOperationException"><see cref="MatchRow"/> matched more than one row.</exception>
		/// <exception cref="DbUpdateConcurrencyException">The located row was removed before <c>SaveChanges</c>.</exception>
		/// <remarks>
		/// <para>
		/// Reports whether a row <i>matched</i>, not whether a value <i>changed</i>. Pre-detaches by the compiled
		/// <see cref="MatchRow"/>, locates with <c>IgnoreQueryFilters()</c>, then copies every mapped scalar
		/// <b>less the entity's key properties</b>, read from <see cref="DbContext.Model"/> as
		/// <c>IProperty.IsKey()</c>.
		/// </para>
		/// <para>
		/// <b>That exclusion is structural here, not a precaution.</b> On a keyless entity the natural key is
		/// ordinarily the primary key, mapped as scalars, so a plain <c>SetValues</c> writes a key property the
		/// moment <paramref name="item"/> carries a different value for one, and EF Core throws
		/// <see cref="InvalidOperationException"/>. The exclusion must be <b>from</b> the copy: the exception is
		/// raised during the copy, so there is no "after" in which to restore.
		/// </para>
		/// <para>
		/// A navigation property with no foreign-key scalar on the entity cannot be repointed by this member.
		/// </para>
		/// </remarks>
		protected virtual int UpdateCore(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = TrackForWrite(item);

			if (stored == null)
				return 0;

			try
			{
				ApplyUpdateValues(Context.Entry(stored), item);

				Context.SaveChanges();

				return 1;
			}
			finally
			{
				DetachAfterWrite(stored, item);
			}
		}

		/// <summary>
		/// Locates the row <paramref name="item"/> names and tracks it, so a write can be made against it.
		/// </summary>
		/// <param name="item">The entity naming the row.</param>
		/// <returns>The tracked stored row, or <c>null</c> when no row matches.</returns>
		/// <remarks>
		/// It starts from the raw <see cref="Dataset"/> and adds <c>IgnoreQueryFilters()</c>, so neither
		/// <see cref="ApplyReadFilter"/> nor a consumer's global query filter can hide the row from a write.
		/// Anything already tracked for that row is released first: a tracking query performs identity resolution
		/// rather than re-reading, so without it a sibling Data Access Object's in-memory values are what the
		/// write would compute from.
		/// </remarks>
		private protected TEntity? TrackForWrite(TEntity item)
		{
			var match = MatchRow(item);

			DetachTrackedRowsMatching(match);

			return Dataset
				.AsTracking()
				.IgnoreQueryFilters()
				.Where(match)
				.SingleOrDefault();
		}

		/// <summary>
		/// Writes <paramref name="item"/>'s values onto the tracked row <see cref="UpdateCore"/> located,
		/// immediately before <c>SaveChanges</c>.
		/// </summary>
		/// <remarks>
		/// Every mapped scalar less the entity's key properties — primary and alternate alike. The soft family's
		/// override point for "this family owns a column and the caller does not".
		/// </remarks>
		private protected virtual void ApplyUpdateValues(EntityEntry<TEntity> entry, TEntity item)
		{
			// Copied into a detached buffer first: writing a key property on a tracked entry throws, and the
			// exception is raised during the copy, so there is no "after" in which to restore it.
			var incoming = entry.CurrentValues.Clone();
			incoming.SetValues(item);

			foreach (var property in entry.CurrentValues.Properties)
			{
				if (property.IsKey())
					continue;

				entry.CurrentValues[property] = incoming[property];
			}
		}

		/// <summary>
		/// The whole of <see cref="Insert"/>, with the soft families' timestamp steps folded in as the one
		/// reading of the clock they are given.
		/// </summary>
		/// <param name="item">The caller's instance. Read, never tracked.</param>
		/// <param name="stamp">
		/// <c>null</c> on the hard families. On the soft ones, the value stamped onto the copy before the write
		/// and onto <paramref name="item"/> only after it has succeeded — which is what leaves a caller's
		/// instance untouched when nothing was stored.
		/// </param>
		private protected void InsertRoot(TEntity item, DateTime? stamp)
		{
			var copy = EntityGraph.CopyForStore(Context, item);
			var related = EntityGraph.ReachableFrom(Context, item, includeRoot: false);

			try
			{
				foreach (var node in related)
					Context.Entry(node).State = EntityState.Unchanged;

				if (stamp.HasValue)
					StampForInsert(copy, stamp.Value);

				Context.Entry(copy).State = EntityState.Added;
				Context.SaveChanges();

				// After SaveChanges, never before: a write that threw must leave the caller's instance carrying
				// exactly the values it arrived with.
				if (stamp.HasValue)
					StampForInsert(item, stamp.Value);

				EntityGraph.RemoveFromInverseNavigations(Context, copy);
			}
			finally
			{
				EntityGraph.Detach(Context, copy);
				EntityGraph.Detach(Context, related);
			}
		}

		private static void StampForInsert(TEntity target, DateTime stamp)
		{
			var soft = (IBaseSoftEntity)target;

			soft.CreatedDate = stamp;
			soft.UpdatedDate = null;
			soft.DeletedDate = null;
		}

		/// <summary>
		/// Releases the located row, the caller's instance, and everything reachable from either — in the
		/// <c>finally</c> of every write, on success and on failure alike.
		/// </summary>
		private void DetachAfterWrite(TEntity stored, TEntity item)
		{
			EntityGraph.Detach(Context, EntityGraph.ReachableFrom(Context, stored, includeRoot: true));
			EntityGraph.Detach(Context, EntityGraph.ReachableFrom(Context, item, includeRoot: true));
		}

		/// <summary>
		/// Detaches every entry already tracked for the row the write is about to match, by compiling
		/// <see cref="MatchRow"/> and running it in memory.
		/// </summary>
		/// <remarks>
		/// There is no resolved key on this family to match on instead, which is what puts the in-memory purity
		/// constraint on <see cref="MatchRow"/>. It detaches what the write is about to touch and nothing else.
		/// </remarks>
		private void DetachTrackedRowsMatching(Expression<Func<TEntity, bool>> match)
		{
			var matches = match.Compile();

			var tracked = Context.ChangeTracker
				.Entries<TEntity>()
				.Where(entry => matches(entry.Entity))
				.ToList();

			foreach (var entry in tracked)
				entry.State = EntityState.Detached;
		}
	}
}
