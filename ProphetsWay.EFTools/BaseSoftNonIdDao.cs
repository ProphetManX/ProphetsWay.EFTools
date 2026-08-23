#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// A keyless soft-delete Data Access Object that also publishes the <see cref="IBaseDao{T}"/> shape,
	/// identified by the <c>MatchRow</c> predicate rather than by an identifier.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <remarks>
	/// <b>Adds no behavior.</b> It declares <see cref="IBaseDao{T}"/> and publishes the two soft cores
	/// <see cref="RootSoftNonIdDao{TEntity}"/> keeps <c>protected</c>. Both timestamp hooks are inherited
	/// unchanged, and the Timestamp Pair Rule binds a Data Access Object deriving from <b>this</b> type exactly
	/// as it binds one deriving from <see cref="RootSoftNonIdDao{TEntity}"/> — the rule counts declaration
	/// sites, not derivation depth.
	/// </remarks>
	public abstract class BaseSoftNonIdDao<TEntity> : RootSoftNonIdDao<TEntity>, IBaseDao<TEntity>
		where TEntity : class, IBaseSoftEntity
	{
		/// <inheritdoc />
		protected BaseSoftNonIdDao(DbContext context) : base(context)
		{
		}

		/// <inheritdoc cref="RootSoftNonIdDao{TEntity}.GetCore" />
		/// <remarks>The soft <see cref="RootSoftNonIdDao{TEntity}.GetCore"/>, published — returns soft-deleted rows.</remarks>
		public virtual TEntity? Get(TEntity item)
		{
			return GetCore(item);
		}

		/// <inheritdoc cref="RootSoftNonIdDao{TEntity}.UpdateCore" />
		/// <remarks>
		/// The soft <see cref="RootSoftNonIdDao{TEntity}.UpdateCore"/>, published. <b>It cannot be made to
		/// bypass timestamp preservation through a hard-update core</b> — the core is an <c>override</c>, so a
		/// caller holding a <see cref="RootNonIdDao{TEntity}"/>-typed reference still reaches the soft body.
		/// </remarks>
		public virtual int Update(TEntity item)
		{
			return UpdateCore(item);
		}
	}
}
