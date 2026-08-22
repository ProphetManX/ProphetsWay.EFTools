#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>A keyless Data Access Object that does publish the <see cref="IBaseDao{T}"/> shape.</summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <remarks>
	/// <b>Adds no behavior.</b> It declares <see cref="IBaseDao{T}"/> and publishes the two cores
	/// <see cref="RootNonIdDao{TEntity}"/> keeps <c>protected</c>. Take it when your keyless entity really does
	/// support single-row retrieval and in-place update through <see cref="RootNonIdDao{TEntity}.MatchRow"/>;
	/// take <see cref="RootNonIdDao{TEntity}"/> when it does not, and do not take this one merely to reach a
	/// member — <c>Get</c> and <c>Update</c> published on an entity they are meaningless for is the coercion the
	/// keyless families exist to prevent.
	/// </remarks>
	// CS8766: IBaseDao<T> is compiled null-oblivious, and T : IBaseEntity permits a struct, so the interface
	// cannot annotate the return its own documentation describes. Tracked for ProphetsWay.BaseDataAccess 3.2.0.
#pragma warning disable CS8766
	public abstract class BaseNonIdDao<TEntity> : RootNonIdDao<TEntity>, IBaseDao<TEntity>
		where TEntity : class, IBaseEntity
	{
#pragma warning restore CS8766
		/// <inheritdoc />
		protected BaseNonIdDao(DbContext context) : base(context)
		{
		}

		/// <inheritdoc cref="RootNonIdDao{TEntity}.GetCore" />
		/// <remarks><see cref="RootNonIdDao{TEntity}.GetCore"/>, published. Contract unchanged.</remarks>
#pragma warning disable CS8766
		public virtual TEntity? Get(TEntity item)
#pragma warning restore CS8766
		{
			return GetCore(item);
		}

		/// <inheritdoc cref="RootNonIdDao{TEntity}.UpdateCore" />
		/// <remarks><see cref="RootNonIdDao{TEntity}.UpdateCore"/>, published. Contract unchanged.</remarks>
		public virtual int Update(TEntity item)
		{
			return UpdateCore(item);
		}
	}
}
