#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// A keyed Data Access Object that additionally publishes paged retrieval and a count.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">The declared type of the identifier property.</typeparam>
	/// <remarks>
	/// Derives from <see cref="BaseDao{TEntity, TKey}"/> rather than from
	/// <see cref="BaseGetAllDao{TEntity, TKey}"/>, because <see cref="IBasePagedDao{T}"/> inherits
	/// <see cref="IBaseDao{T}"/> and not <see cref="IBaseGetAllDao{T}"/>. Adds no member of its own.
	/// </remarks>
	public abstract class BasePagedDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBasePagedDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		/// <inheritdoc />
		protected BasePagedDao(DbContext context) : base(context)
		{
		}
	}
}
