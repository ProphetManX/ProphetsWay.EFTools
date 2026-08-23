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
	/// <para>
	/// Derives from <see cref="BaseDao{TEntity, TKey}"/> rather than from
	/// <see cref="BaseGetAllDao{TEntity, TKey}"/>, because <see cref="IBasePagedDao{T}"/> inherits
	/// <see cref="IBaseDao{T}"/> and not <see cref="IBaseGetAllDao{T}"/>. Adds no member of its own.
	/// </para>
	/// <para>
	/// It is nevertheless the class that carries the ordering obligation. Publishing
	/// <see cref="BaseDao{TEntity, TKey}.GetPaged"/> is what makes
	/// <see cref="BaseDao{TEntity, TKey}.ApplyStableOrder"/>'s totality matter, because a window that is not cut
	/// from a total order can overlap or omit rows.
	/// </para>
	/// </remarks>
	// CS8766: declaring a second interface re-runs the implicit-implementation check against the inherited Get,
	// which IBaseDao<T>, compiled null-oblivious, cannot annotate. Tracked for ProphetsWay.BaseDataAccess 3.2.0.
#pragma warning disable CS8766
	public abstract class BasePagedDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBasePagedDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
#pragma warning restore CS8766
		/// <inheritdoc />
		protected BasePagedDao(DbContext context) : base(context)
		{
		}
	}
}
