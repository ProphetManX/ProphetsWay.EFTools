#nullable enable

using System;
using System.Collections.Generic;

using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The one copy of the soft-delete timestamp policy — both hook defaults, and the post-materialization walk
	/// that applies a normalizer to the three <see cref="IBaseSoftEntity"/> timestamps.
	/// </summary>
	/// <remarks>
	/// The Timestamp Pair Rule declares <c>GetCurrentTimestamp</c> and <c>NormalizeRetrievedTimestamp</c> on two
	/// unrelated branches, because one is keyed and one is not and C# has no multiple inheritance. What is shared
	/// is the behavior, not the declaration: the duplication of the two <i>declarations</i> is structural and
	/// accepted, the duplication of the <i>logic</i> is not, and this class is why it does not exist. A change to
	/// a default cannot land on one branch and miss the other.
	/// </remarks>
	internal static class SoftTimestamps
	{
		/// <summary>The clock hook's default, whose <see cref="DateTime.Kind"/> is <see cref="DateTimeKind.Utc"/>.</summary>
		internal static DateTime Now()
		{
			return DateTime.UtcNow;
		}

		/// <summary>
		/// The normalization hook's default. A <b>relabel, not a conversion</b> — it changes no instant and adds
		/// no offset.
		/// </summary>
		internal static DateTime AsUtc(DateTime value)
		{
			return DateTime.SpecifyKind(value, DateTimeKind.Utc);
		}

		/// <summary>
		/// Writes an <c>Insert</c>'s three timestamps onto one object — the stored copy or the caller's instance.
		/// </summary>
		/// <remarks>
		/// Cast rather than constrained: the two callers are closed over <see cref="IBaseIdEntity{TKey}"/> and
		/// <see cref="IBaseEntity"/> respectively, and only their soft descendants ever supply a stamp.
		/// </remarks>
		internal static void StampForInsert(object target, DateTime stamp)
		{
			var soft = (IBaseSoftEntity)target;

			soft.CreatedDate = stamp;
			soft.UpdatedDate = null;
			soft.DeletedDate = null;
		}

		/// <summary>Writes an <c>Update</c>'s stamp onto one object — the tracked row or the caller's instance.</summary>
		internal static void StampForUpdate(object target, DateTime stamp)
		{
			((IBaseSoftEntity)target).UpdatedDate = stamp;
		}

		/// <summary>Runs <paramref name="normalize"/> over the three timestamps of a materialized soft entity.</summary>
		/// <returns><paramref name="entity"/> itself, normalized in place, or <c>null</c>.</returns>
		/// <remarks>
		/// <see cref="IBaseSoftEntity.CreatedDate"/> is a non-nullable <see cref="DateTime"/> and is always
		/// normalized. <see cref="IBaseSoftEntity.UpdatedDate"/> and <see cref="IBaseSoftEntity.DeletedDate"/> are
		/// <see cref="Nullable{T}"/> and reach the hook only when they hold a value, so a <c>null</c> stays
		/// <c>null</c> and is never normalized into one.
		/// </remarks>
		internal static TEntity? Normalize<TEntity>(TEntity? entity, Func<DateTime, DateTime> normalize)
			where TEntity : class, IBaseSoftEntity
		{
			if (entity == null)
				return null;

			entity.CreatedDate = normalize(entity.CreatedDate);

			if (entity.UpdatedDate.HasValue)
				entity.UpdatedDate = normalize(entity.UpdatedDate.Value);

			if (entity.DeletedDate.HasValue)
				entity.DeletedDate = normalize(entity.DeletedDate.Value);

			return entity;
		}

		/// <summary>Runs the walk above over a materialized list, in place.</summary>
		/// <returns><paramref name="entities"/> itself.</returns>
		internal static IList<TEntity> Normalize<TEntity>(IList<TEntity> entities, Func<DateTime, DateTime> normalize)
			where TEntity : class, IBaseSoftEntity
		{
			foreach (var entity in entities)
				Normalize(entity, normalize);

			return entities;
		}
	}
}
