#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// A keyed Data Access Object that additionally publishes retrieval of the whole set.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">The declared type of the identifier property.</typeparam>
	/// <remarks>
	/// Adds no member of its own. It declares <see cref="IBaseGetAllDao{T}"/>, which the flat surface on
	/// <see cref="BaseDao{TEntity, TKey}"/> already satisfies.
	/// </remarks>
	public abstract class BaseGetAllDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBaseGetAllDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		/// <inheritdoc />
		protected BaseGetAllDao(DbContext context) : base(context)
		{
		}
	}
}
