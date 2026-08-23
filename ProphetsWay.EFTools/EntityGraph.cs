#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The one copy of the navigation-graph mechanics every write shares — the copy the store receives (A32),
	/// the reachability walk (A24), the inverse-navigation cleanup (A37), and the detach A26 puts in a
	/// <c>finally</c>.
	/// </summary>
	/// <remarks>
	/// The keyed families and the keyless families are unrelated inheritance branches, so these members cannot
	/// be inherited from one place. What is shared is the behavior rather than the declaration: the duplication
	/// of the <i>logic</i> is what this class exists to prevent, on the same reasoning as
	/// <see cref="SoftTimestamps"/>.
	/// </remarks>
	internal static class EntityGraph
	{
		/// <summary>
		/// Builds the instance the store actually receives: every mapped scalar by value, every navigation by
		/// reference, so relationship fix-up has the same principals to work with that the caller's instance does.
		/// </summary>
		internal static TEntity CopyForStore<TEntity>(DbContext context, TEntity item)
			where TEntity : class
		{
			var source = context.Entry(item);
			var copy = (TEntity)source.CurrentValues.ToObject();

			// PropertyValues covers properties and not navigations, so the references are a separate move.
			foreach (var navigation in source.Navigations)
				navigation.Metadata.PropertyInfo?.SetValue(copy, navigation.CurrentValue);

			return copy;
		}

		/// <summary>
		/// Every entity reachable through navigation properties, visited by reference identity so a node reached
		/// by two paths is returned once.
		/// </summary>
		internal static IReadOnlyList<object> ReachableFrom(DbContext context, object root, bool includeRoot)
		{
			var model = context.Model;
			var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
			var pending = new Stack<object>();
			var reached = new List<object>();

			pending.Push(root);

			while (pending.Count > 0)
			{
				var current = pending.Pop();

				if (!seen.Add(current))
					continue;

				var entityType = model.FindEntityType(current.GetType());

				if (entityType == null)
					continue;

				if (includeRoot || !ReferenceEquals(current, root))
					reached.Add(current);

				foreach (var navigation in Navigations(entityType))
				{
					var value = navigation.PropertyInfo?.GetValue(current);

					if (value == null)
						continue;

					if (navigation.IsCollection)
					{
						foreach (var child in (IEnumerable)value)
						{
							if (child != null)
								pending.Push(child);
						}
					}
					else
					{
						pending.Push(value);
					}
				}
			}

			return reached;
		}

		/// <summary>
		/// Takes the newly inserted copy back out of any inverse navigation relationship fix-up wrote it into.
		/// </summary>
		/// <remarks>
		/// Those principals are the caller's own objects, and the copy is an object internal to this library that
		/// the caller cannot name. It has to happen before the detach: detaching does not run fix-up in reverse.
		/// </remarks>
		internal static void RemoveFromInverseNavigations<TEntity>(DbContext context, TEntity copy)
			where TEntity : class
		{
			var entityType = context.Model.FindEntityType(typeof(TEntity));

			if (entityType == null)
				return;

			foreach (var navigation in entityType.GetNavigations())
			{
				var inverse = navigation.Inverse;

				if (inverse?.PropertyInfo == null)
					continue;

				var value = navigation.PropertyInfo?.GetValue(copy);

				if (value == null)
					continue;

				// A collection navigation holds one partner per member and a reference navigation holds exactly
				// one. Both must be expanded: reflecting the inverse onto the collection object itself raises
				// TargetException, in the window after SaveChanges has already committed the row.
				var partners = navigation.IsCollection
					? ((IEnumerable)value).Cast<object>().Where(member => member != null).ToList()
					: new List<object> { value };

				foreach (var partner in partners)
					RemoveFromInverse(inverse, partner, copy, typeof(TEntity));
			}
		}

		/// <summary>Clears one partner's inverse navigation of <paramref name="copy"/>, reference or collection.</summary>
		private static void RemoveFromInverse(INavigation inverse, object partner, object copy, Type elementType)
		{
			var held = inverse.PropertyInfo!.GetValue(partner);

			if (held == null)
				return;

			if (!inverse.IsCollection)
			{
				if (ReferenceEquals(held, copy))
					inverse.PropertyInfo.SetValue(partner, null);

				return;
			}

			if (!((IEnumerable)held).Cast<object>().Any(member => ReferenceEquals(member, copy)))
				return;

			held.GetType()
				.GetMethod(nameof(ICollection<object>.Remove), new[] { elementType })
				?.Invoke(held, new[] { copy });
		}

		internal static void Detach(DbContext context, object entity)
		{
			context.Entry(entity).State = EntityState.Detached;
		}

		internal static void Detach(DbContext context, IReadOnlyList<object> entities)
		{
			foreach (var entity in entities)
				Detach(context, entity);
		}

		private static IEnumerable<INavigationBase> Navigations(IEntityType entityType)
		{
			foreach (var navigation in entityType.GetNavigations())
				yield return navigation;

			foreach (var navigation in entityType.GetSkipNavigations())
				yield return navigation;
		}
	}
}
