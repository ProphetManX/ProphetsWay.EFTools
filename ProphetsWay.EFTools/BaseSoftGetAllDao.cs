#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// A soft-delete Data Access Object that additionally publishes retrieval of the whole live set.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">The declared type of the identifier property.</typeparam>
	/// <remarks>
	/// Adds no member of its own. It declares <see cref="IBaseGetAllDao{T}"/>, which the flat surface on
	/// <see cref="BaseDao{TEntity, TKey}"/> already satisfies, and inherits both timestamp hooks from
	/// <see cref="BaseSoftDao{TEntity, TKey}"/> unchanged.
	/// </remarks>
	// CS8766: declaring a second interface re-runs the implicit-implementation check against the inherited Get,
	// which IBaseDao<T>, compiled null-oblivious, cannot annotate. Tracked for ProphetsWay.BaseDataAccess 3.2.0.
#pragma warning disable CS8766
	public abstract class BaseSoftGetAllDao<TEntity, TKey> : BaseSoftDao<TEntity, TKey>, IBaseGetAllDao<TEntity>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
#pragma warning restore CS8766
		/// <inheritdoc />
		protected BaseSoftGetAllDao(DbContext context) : base(context)
		{
		}
	}
}
