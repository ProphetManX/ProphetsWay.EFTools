#nullable enable

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// A soft-delete Data Access Object that additionally publishes paged retrieval and a count.
	/// </summary>
	/// <typeparam name="TEntity">The entity this Data Access Object reads and writes.</typeparam>
	/// <typeparam name="TKey">The declared type of the identifier property.</typeparam>
	/// <remarks>
	/// <para>
	/// Derives from <see cref="BaseSoftDao{TEntity, TKey}"/> rather than from
	/// <see cref="BaseSoftGetAllDao{TEntity, TKey}"/>, because <see cref="IBasePagedDao{T}"/> inherits
	/// <see cref="IBaseDao{T}"/> and not <see cref="IBaseGetAllDao{T}"/>. Adds no member of its own.
	/// </para>
	/// <para>
	/// It is nevertheless the soft class that carries the ordering obligation. Publishing
	/// <see cref="BaseDao{TEntity, TKey}.GetPaged"/> is what makes
	/// <see cref="BaseDao{TEntity, TKey}.ApplyStableOrder"/>'s totality matter, because a window that is not cut
	/// from a total order can overlap or omit rows.
	/// </para>
	/// <para>
	/// This is also the family a Data Access Object interface declaring <b>both</b> capability interfaces derives
	/// from: paged wins the fixed precedence, and the remaining <see cref="IBaseGetAllDao{T}"/> is satisfied by
	/// the member <see cref="BaseDao{TEntity, TKey}"/> already supplies, because a base-class public method may
	/// implement an interface a derived class declares.
	/// </para>
	/// </remarks>
	public abstract class BaseSoftPagedDao<TEntity, TKey> : BaseSoftDao<TEntity, TKey>, IBasePagedDao<TEntity>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
		/// <inheritdoc />
		protected BaseSoftPagedDao(DbContext context) : base(context)
		{
		}
	}
}
