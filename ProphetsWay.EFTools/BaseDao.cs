#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The Entity Framework Core base for a Data Access Object over an entity whose identity is a single
	/// stored property of type <typeparamref name="TKey"/>.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">
	/// The declared type of the identifier property. Deliberately unconstrained, so <c>string</c> and
	/// <c>int?</c> are legal alongside <c>int</c>, <c>long</c> and <c>Guid</c>.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// The identifier property is resolved by name — <c>{TypeName}Id</c> first, then <c>Id</c> — and must be a
	/// <b>public instance</b> property declared as <typeparamref name="TKey"/> with a set accessor of any
	/// visibility. An explicit <see cref="IBaseIdEntity{T}"/> implementation does not satisfy that, because it
	/// compiles to a non-public, interface-qualified property that neither lookup finds. Resolution is
	/// validated in the constructor, so a mis-wired entity fails when the Data Access Layer is built rather
	/// than on first use.
	/// </para>
	/// <para>
	/// <b>Not thread-safe.</b> Every Data Access Object on a layer shares one <see cref="DbContext"/>.
	/// </para>
	/// </remarks>
	public abstract class BaseDao<TEntity, TKey> : IBaseDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		/// <summary>How the store and Entity Framework Core divide responsibility for the identifier value.</summary>
		private enum IdentifierGeneration
		{
			/// <summary>The store produces the value, so a pre-assigned one must be cleared before the insert.</summary>
			Store,

			/// <summary>Entity Framework Core produces it client-side, only when the property holds the CLR default.</summary>
			ClientSide,

			/// <summary>Nobody produces it. The caller owns the value.</summary>
			Never
		}

		/// <summary>
		/// Carries the key value into the expression tree by reference, which is what makes the provider
		/// parameterize it. <see cref="Expression.Constant(object)"/> over the key itself would be emitted as a
		/// SQL literal and give a distinct query plan per key value.
		/// </summary>
		private sealed class KeyCarrier
		{
			public readonly TKey? Value;

			public KeyCarrier(TKey? value)
			{
				Value = value;
			}
		}

		private static readonly FieldInfo CarrierValue = typeof(KeyCarrier).GetField(nameof(KeyCarrier.Value))!;

		private static readonly MethodInfo ThenByMethod = typeof(Queryable)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.First(method => method.Name == nameof(Queryable.ThenBy) && method.GetParameters().Length == 2);

		private static readonly MethodInfo EfProperty = typeof(EF)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.First(method => method.Name == nameof(EF.Property) && method.IsGenericMethodDefinition && method.GetParameters().Length == 2);

		private static PropertyInfo? _identifier;
		private static string? _identifierFailure;
		private static Expression<Func<TEntity, TKey?>>? _identifierSelector;
		private static Func<TEntity, TKey?>? _identifierReader;
		private static IdentifierGeneration? _identifierGeneration;

		private DbSet<TEntity>? _dataset;

		static BaseDao()
		{
			_identifierFailure = ResolveIdentifier(out var identifier);
			_identifier = identifier;

			if (identifier == null)
				return;

			var parameter = Expression.Parameter(typeof(TEntity), "x");
			_identifierSelector = Expression.Lambda<Func<TEntity, TKey?>>(Expression.Property(parameter, identifier), parameter);
			_identifierReader = _identifierSelector.Compile();
		}

		/// <summary>
		/// Captures the context every member reads and writes through, and validates that
		/// <typeparamref name="TEntity"/> carries a conventionally-resolvable identifier.
		/// </summary>
		/// <param name="context">The context this Data Access Object's layer owns.</param>
		/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
		/// <exception cref="DataAccessConventionException">
		/// <typeparamref name="TEntity"/> exposes no public instance <c>{TypeName}Id</c> or <c>Id</c> property
		/// declared as <typeparamref name="TKey"/> and carrying a set accessor.
		/// </exception>
		/// <remarks>
		/// The order is load-bearing: a caller who passed <c>null</c> is told that, rather than told about a
		/// convention they did not violate. Nothing else happens here — no <c>Set&lt;TEntity&gt;()</c>, no model
		/// access, no query and no connection.
		/// </remarks>
		protected BaseDao(DbContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (_identifierFailure != null)
				throw new DataAccessConventionException(_identifierFailure);

			Context = context;
		}

		/// <summary>The context shared by every Data Access Object on the layer.</summary>
		protected DbContext Context { get; }

		/// <summary>
		/// The set this Data Access Object reads and writes. Resolved on first access rather than in the
		/// constructor, because <see cref="DbContext.Set{TEntity}()"/> forces the whole model to be built.
		/// </summary>
		protected DbSet<TEntity> Dataset => _dataset ??= Context.Set<TEntity>();

		// CS8766: IBaseDao<T> is compiled null-oblivious, and T : IBaseEntity permits a struct, so the interface
		// cannot annotate the return its own documentation describes. Tracked for ProphetsWay.BaseDataAccess 3.2.0.
#pragma warning disable CS8766
		/// <inheritdoc />
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// Returns a fresh untracked instance, never the argument and never the store's own tracked object.
		/// A resolved key of <c>null</c> answers <c>null</c> without issuing a query; <c>default(TKey)</c> is an
		/// ordinary key value and is looked up normally.
		/// </remarks>
		public virtual TEntity? Get(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (IsNullKey(GetKey(item)))
				return null;

			return ApplyIncludes(Dataset.IgnoreQueryFilters().Where(MatchRow(item)))
				.AsNoTracking()
				.SingleOrDefault();
		}
#pragma warning restore CS8766

		/// <inheritdoc />
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// The store receives a <b>copy</b> of <paramref name="item"/>, so the caller's instance is read rather
		/// than adopted and is never handed to the change tracker. Everything reachable through
		/// <paramref name="item"/>'s navigation properties is attached <c>Unchanged</c> — related rows are read,
		/// never written. The identifier the row ended up carrying is the only value written back.
		/// </remarks>
		public virtual void Insert(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var generation = ResolveIdentifierGeneration();
			var copy = EntityGraph.CopyForStore(Context, item);
			var related = EntityGraph.ReachableFrom(Context, item, includeRoot: false);

			try
			{
				foreach (var node in related)
					Context.Entry(node).State = EntityState.Unchanged;

				if (generation == IdentifierGeneration.Store)
					_identifier!.SetValue(copy, default(TKey));

				Context.Entry(copy).State = EntityState.Added;
				Context.SaveChanges();

				_identifier!.SetValue(item, _identifier!.GetValue(copy));
				EntityGraph.RemoveFromInverseNavigations(Context, copy);
			}
			finally
			{
				EntityGraph.Detach(Context, copy);
				EntityGraph.Detach(Context, related);
			}
		}

		/// <inheritdoc />
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// Reports whether a row matched, not whether a value changed: <c>1</c> when a row exists, <c>0</c> when
		/// none does. Every mapped scalar less the entity's key properties is written; a navigation property with
		/// no foreign-key scalar on the entity cannot be repointed by this member.
		/// </remarks>
		public virtual int Update(TEntity item)
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

		/// <inheritdoc />
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// A hard delete — the row is genuinely removed. <c>1</c> when the row existed, <c>0</c> when it did not,
		/// so a second call is idempotent rather than an error.
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

		/// <summary>
		/// Every row <see cref="ApplyReadFilter"/> admits, in <see cref="ApplyStableOrder"/>'s order.
		/// </summary>
		/// <param name="item">
		/// A type selector only. It is never read, and is <c>null</c> whenever the call arrives through the
		/// dispatcher.
		/// </param>
		/// <returns>A fresh list, empty rather than <c>null</c> when nothing matches.</returns>
		/// <remarks>
		/// Ordered, but not obliged to be <i>totally</i> ordered. A full pass has no window to overlap or omit, so a
		/// Data Access Object that publishes only this member is bound by nothing in
		/// <see cref="ApplyStableOrder"/> beyond receiving the rows once each.
		/// </remarks>
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
		/// <remarks>
		/// <b>This is the member that obliges <see cref="ApplyStableOrder"/> to be total</b>, and publishing it —
		/// deriving from <see cref="BasePagedDao{TEntity, TKey}"/> — is what places that obligation on a Data Access
		/// Object. The default ordering meets it by appending the model's primary key to the identifier, so an
		/// override is needed only where the model declares no primary key to append.
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
		/// How many rows <see cref="ApplyReadFilter"/> admits — the same count <see cref="GetAll"/> returns, which
		/// is what makes a pager's last page correct.
		/// </summary>
		/// <param name="item">A type selector only; never read.</param>
		/// <returns>The number of admitted rows.</returns>
		/// <remarks>
		/// Materializes no entity, so it includes nothing and emits no <c>ORDER BY</c>.
		/// <see cref="ApplyStableOrder"/> is nevertheless invoked and its result discarded, so a family whose
		/// ordering default refuses to supply one refuses here too.
		/// </remarks>
		public virtual int GetCount(TEntity? item)
		{
			var filtered = ApplyReadFilter(Dataset);

			ApplyStableOrder(filtered);

			return filtered.Count();
		}

		/// <summary>
		/// Locates the row <paramref name="item"/> refers to and tracks it, so a write can be made against it.
		/// </summary>
		/// <param name="item">The entity naming the row.</param>
		/// <returns>The tracked stored row, or <c>null</c> when the key is absent or no row matches.</returns>
		/// <remarks>
		/// <para>
		/// The whole locating half of a write, in one place: the caller of this member owes only the write itself
		/// and the <c>finally</c> that detaches what it tracked. <c>Update</c>, <c>Delete</c>, the soft-delete
		/// families' own writes and a consumer's custom write all go through it, which is what makes one
		/// <see cref="MatchRow"/> override reach every one of them.
		/// </para>
		/// <para>
		/// It starts from the raw <see cref="Dataset"/> and adds <c>IgnoreQueryFilters()</c>, so neither
		/// <see cref="ApplyReadFilter"/> nor a consumer's global query filter can hide the row from a write —
		/// a soft-delete family could otherwise never reach the rows it has already deleted.
		/// </para>
		/// </remarks>
		protected TEntity? TrackForWrite(TEntity item)
		{
			var key = GetKey(item);

			if (IsNullKey(key))
				return null;

			DetachTrackedRowsCarrying(key);

			return Dataset
				.AsTracking()
				.IgnoreQueryFilters()
				.Where(MatchRow(item))
				.SingleOrDefault();
		}

		/// <summary>
		/// Writes <paramref name="item"/>'s values onto the tracked row <see cref="Update"/> located, immediately
		/// before <c>SaveChanges</c>.
		/// </summary>
		/// <param name="entry">The tracked entry for the stored row.</param>
		/// <param name="item">The entity supplying the values.</param>
		/// <remarks>
		/// Every mapped scalar less the entity's key properties — primary and alternate alike. The override point
		/// for "this family owns a column and the caller does not": a soft-delete family restores its timestamps
		/// from <paramref name="entry"/> here, so they cannot arrive from <paramref name="item"/>.
		/// </remarks>
		protected virtual void ApplyUpdateValues(EntityEntry<TEntity> entry, TEntity item)
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

		/// <summary>Reads the identifier value off <paramref name="item"/>.</summary>
		/// <param name="item">The entity to read.</param>
		/// <returns>The resolved identifier property's value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// Override to derive the value <b>from</b> the resolved property — trimming a <c>string</c> key,
		/// normalizing case. An override changes which value is compared, never which column is addressed.
		/// </remarks>
		protected virtual TKey? GetKey(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			return _identifierReader!(item);
		}

		/// <summary>The predicate locating the row <paramref name="item"/> refers to.</summary>
		/// <param name="item">The entity naming the row.</param>
		/// <returns><c>KeyEquals(GetKey(item))</c> by default.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
		/// <remarks>
		/// The override point for "the row is not identified by the key alone" — a tenant-scoped table where
		/// <c>(TenantId, Id)</c> is the real identity. <c>Get</c>, <c>Update</c> and <c>Delete</c> all locate
		/// through this member and through nothing else, so one override changes all three at once.
		/// </remarks>
		protected virtual Expression<Func<TEntity, bool>> MatchRow(TEntity item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			return KeyEquals(GetKey(item));
		}

		/// <summary>The predicate matching the row whose identifier equals <paramref name="key"/>.</summary>
		/// <param name="key">The identifier value to match.</param>
		/// <returns>An expression equivalent to <c>x =&gt; x.Id == key</c>.</returns>
		/// <remarks>
		/// Built as an expression tree because <c>==</c> is not available on an unconstrained type parameter;
		/// <see cref="Expression.Equal(Expression, Expression)"/> resolves the comparison the concrete
		/// <typeparamref name="TKey"/> actually has. The key travels in a carrier object so the provider
		/// parameterizes it rather than emitting a literal. Override only if a provider mistranslates the
		/// default for your key type.
		/// </remarks>
		protected virtual Expression<Func<TEntity, bool>> KeyEquals(TKey? key)
		{
			var parameter = Expression.Parameter(typeof(TEntity), "x");
			var identifier = Expression.Property(parameter, _identifier!);
			var carrier = Expression.Field(Expression.Constant(new KeyCarrier(key)), CarrierValue);

			return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(identifier, carrier), parameter);
		}

		/// <summary>The identifier selector, over the same resolved property <see cref="KeyEquals"/> compares.</summary>
		protected virtual Expression<Func<TEntity, TKey?>> KeySelector => _identifierSelector!;

		/// <summary>
		/// Restricts which stored rows the retrieval members may see. Hides nothing on this class.
		/// </summary>
		/// <param name="query">The raw <see cref="Dataset"/> query.</param>
		/// <returns>The restricted query, which <see cref="ApplyIncludes"/> is then handed.</returns>
		protected virtual IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query)
		{
			return query;
		}

		/// <summary>
		/// Declares which navigation properties a read materializes. Loads none on this class.
		/// </summary>
		/// <param name="query">
		/// The row-restricted query for its path — <see cref="ApplyReadFilter"/>'s output on the retrieval
		/// members, the key-matched query on <see cref="Get"/>.
		/// </param>
		/// <returns>The query with whatever <c>Include</c>/<c>ThenInclude</c> this Data Access Object's own contract promises.</returns>
		protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
		{
			return query;
		}

		/// <summary>A total ordering over the set, applied by <see cref="GetAll"/> and <see cref="GetPaged"/> alike.</summary>
		/// <param name="query">The filtered, included query.</param>
		/// <returns>
		/// <c>query.OrderBy(KeySelector)</c>, followed by a <c>ThenBy</c> over every primary-key property the model
		/// declares for <typeparamref name="TEntity"/> that the leading ordering does not already name.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The identifier alone is a total order only where it is unique and non-null, and the widened
		/// <typeparamref name="TKey"/> does not promise either — a <c>string</c> or <c>int?</c> identifier may be an
		/// ordinary stored column rather than the primary key. The primary key is therefore appended as a
		/// tie-breaker, in the model's declared order, so the ordering is total without the consumer having to know
		/// that. Where the identifier <i>is</i> the whole primary key the tie-breaker is redundant and is dropped,
		/// so the ordinary case emits exactly the <c>ORDER BY</c> it emitted before.
		/// </para>
		/// <para>
		/// <b>Only paging is bound by totality.</b> A <see cref="BasePagedDao{TEntity, TKey}"/> publishes
		/// <see cref="GetPaged"/>, whose windows partition a full pass only over a total order. A
		/// <see cref="BaseDao{TEntity, TKey}"/> or <see cref="BaseGetAllDao{TEntity, TKey}"/> hands back the whole
		/// set in one call, so a nullable or duplicate-capable identifier obliges its consumer to override nothing
		/// here. Overriding is for the case the tie-breaker cannot reach — a type the model maps without a primary
		/// key, whose Data Access Object publishes paging anyway.
		/// </para>
		/// <para>
		/// Reads <see cref="DbContext.Model"/>, at query time and never in the constructor. A type the model does
		/// not map, or maps with no primary key, falls back to the identifier alone rather than throwing, because
		/// <see cref="GetCount"/> invokes this member and discards the result — a throw here would break counting
		/// for an entity that never needed an ordering. For a <c>string</c> key the resulting sequence is the
		/// storage engine's collation, and the placement of nulls is likewise the engine's; both differ between
		/// providers, so the ordering is stable within one and not identical across two.
		/// </para>
		/// </remarks>
		protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query)
		{
			var selector = KeySelector;
			var ordered = query.OrderBy(selector);
			var keyProperties = Context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties;

			if (keyProperties == null)
				return ordered;

			// Named only when the selector is a plain member access, so an override that selects something else
			// keeps every key property rather than having one silently dropped out from under it.
			var leading = selector.Body is MemberExpression member && member.Expression == selector.Parameters[0]
				? member.Member.Name
				: null;

			foreach (var property in keyProperties)
			{
				if (property.Name == leading)
					continue;

				ordered = ThenBy(ordered, property);
			}

			return ordered;
		}

		/// <summary>Appends one <c>ThenBy</c> over <paramref name="property"/> to an already-ordered query.</summary>
		/// <remarks>
		/// Composed as an expression rather than written directly because the property's type is not known
		/// statically, and because a shadow key property has no CLR member for
		/// <see cref="Expression.Property(Expression, PropertyInfo)"/> to address —
		/// <see cref="EF.Property{TProperty}"/> is the only selector that reaches one.
		/// </remarks>
		private static IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> ordered, IProperty property)
		{
			var parameter = Expression.Parameter(typeof(TEntity), "x");
			var member = property.PropertyInfo;

			Expression body = member != null
				? Expression.Property(parameter, member)
				: Expression.Call(EfProperty.MakeGenericMethod(property.ClrType), parameter, Expression.Constant(property.Name));

			var call = Expression.Call(
				ThenByMethod.MakeGenericMethod(typeof(TEntity), body.Type),
				ordered.Expression,
				Expression.Quote(Expression.Lambda(body, parameter)));

			return (IOrderedQueryable<TEntity>)ordered.Provider.CreateQuery<TEntity>(call);
		}

		/// <summary>
		/// Whether the resolved key is absent, which is the one condition that answers without issuing a query.
		/// </summary>
		/// <remarks>
		/// Deliberately not a <c>default(TKey)</c> test. <c>0</c>, <c>Guid.Empty</c> and <c>""</c> are ordinary
		/// stored key values, and a Data Access Object that made them unreachable through <c>Get</c> while
		/// <c>GetAll</c> still returned them would disagree with itself. Boxing is what asks the right question
		/// on an unconstrained type parameter: a <see cref="Nullable{T}"/> holding no value boxes to <c>null</c>,
		/// while a non-nullable value type never does.
		/// </remarks>
		private static bool IsNullKey(TKey? key)
		{
			return (object?)key is null;
		}

		private static string? ResolveIdentifier(out PropertyInfo? identifier)
		{
			var entityType = typeof(TEntity);
			identifier = null;

			try
			{
				identifier = entityType.GetProperty($"{entityType.Name}Id", BindingFlags.Public | BindingFlags.Instance)
					?? entityType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
			}
			catch (AmbiguousMatchException)
			{
				return $"The entity type [{entityType.FullName}] declares more than one public instance identifier property, so which one carries the identifier is undecidable.";
			}

			if (identifier == null)
				return $"The entity type [{entityType.FullName}] exposes neither a public instance '{entityType.Name}Id' nor a public instance 'Id' property, so no identifier can be resolved. An explicit IBaseIdEntity<T> implementation does not satisfy this — declare the property as an ordinary public member.";

			if (!identifier.CanRead)
			{
				var name = identifier.Name;
				identifier = null;

				return $"The entity type [{entityType.FullName}] exposes an identifier property '{name}' with no get accessor, so the identifier cannot be read.";
			}

			if (!identifier.CanWrite)
			{
				var name = identifier.Name;
				identifier = null;

				return $"The entity type [{entityType.FullName}] exposes an identifier property '{name}' with no set accessor, so no identifier can be assigned to it.";
			}

			if (identifier.PropertyType != typeof(TKey))
			{
				var name = identifier.Name;
				var declared = identifier.PropertyType.FullName;
				identifier = null;

				return $"The entity type [{entityType.FullName}] resolves its identifier to '{name}', declared as [{declared}], which is not the key type [{typeof(TKey).FullName}] this Data Access Object was closed over.";
			}

			return null;
		}

		/// <summary>
		/// Reads from the model, on the first <c>Insert</c> rather than in the constructor, whether the store
		/// produces the identifier value or something else does.
		/// </summary>
		private IdentifierGeneration ResolveIdentifierGeneration()
		{
			if (_identifierGeneration.HasValue)
				return _identifierGeneration.Value;

			var generation = IdentifierGeneration.Never;
			var entityType = Context.Model.FindEntityType(typeof(TEntity));
			var property = entityType?.FindProperty(_identifier!.Name);

			if (property != null && property.ValueGenerated != ValueGenerated.Never)
			{
				// The provider's own selector answers, which is how a provider-neutral library gets a
				// provider-accurate answer. A temporary value is held until the store replies; a real one is sent.
				var generator = Context.GetService<IValueGeneratorSelector>().TrySelect(property, entityType!, out var selected)
					? selected
					: null;

				generation = generator == null || generator.GeneratesTemporaryValues
					? IdentifierGeneration.Store
					: IdentifierGeneration.ClientSide;
			}

			_identifierGeneration = generation;

			return generation;
		}

		/// <summary>
		/// Releases any entry already tracked for the row about to be fetched, because a tracking query performs
		/// identity resolution rather than re-reading — without this a sibling Data Access Object's in-memory
		/// values are what a later write would preserve.
		/// </summary>
		/// <remarks>
		/// Matched on the resolved key rather than on <see cref="MatchRow"/>: the predicate is a <i>query</i>
		/// expression, and compiling one that reads a navigation property or a store function would throw here or,
		/// worse, quietly match the wrong set.
		/// </remarks>
		private void DetachTrackedRowsCarrying(TKey? key)
		{
			var tracked = Context.ChangeTracker
				.Entries<TEntity>()
				.Where(entry => EqualityComparer<TKey?>.Default.Equals(GetKey(entry.Entity), key))
				.ToList();

			foreach (var entry in tracked)
				entry.State = EntityState.Detached;
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
	}
}
