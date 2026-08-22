using Microsoft.EntityFrameworkCore;

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	/// <summary>
	/// The Entity Framework implementation of <see cref="ICompanyResourceDao"/> — a join table with no
	/// identifier at all, whose identity is the <c>(CompanyId, ResourceId)</c> pair.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>The family is <see cref="RootNonIdDao{TEntity}"/> and it must stay that way.</b>
	/// <see cref="ICompanyResourceDao"/> inherits <c>IBaseDao&lt;T&gt;</c> not at all, and rule 8 —
	/// <c>Get&lt;CompanyResource&gt;(id)</c> always throws <c>DataAccessConventionException</c> — rests on two
	/// independent structural facts, the first of which is that this Data Access Object publishes no
	/// <c>Get</c>. Deriving from <see cref="BaseNonIdDao{TEntity}"/> instead would publish one and remove it
	/// silently, with every behavioral test still green.
	/// </para>
	/// <para>
	/// Nine of the ten rules are the family's. <c>MatchRow</c> is rule 1; the base <c>Insert</c> assigns nothing
	/// back because there is nothing to assign (rule 2); the base <c>Delete</c> is the hard <c>1</c>/<c>0</c>
	/// of rule 4; <c>GetAll</c> is rules 5 and 6, and its <c>item</c> parameter is never read; the
	/// <see cref="ArgumentNullException"/> guards are rule 7; the untracked reads and the copy the store
	/// receives are rule 9; and rule 10 is the store's referential integrity, propagated unwrapped.
	/// </para>
	/// <para>
	/// <b>Two members are written here.</b> <c>ApplyStableOrder</c>, because the keyless default throws until it
	/// is overridden and rule 5's "no guaranteed order" does not excuse it; and <c>Insert</c>, because rule 3's
	/// silent no-op on a pair the store already holds is a Data Access Object's own contract and the library
	/// deliberately does not offer it.
	/// </para>
	/// </remarks>
	internal class CompanyResourceDao : RootNonIdDao<CompanyResource>, ICompanyResourceDao
	{
		public CompanyResourceDao(DbContext context) : base(context)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// Rule 3 — a pair the store already holds is a silent no-op rather than an error.
		/// <c>IgnoreQueryFilters()</c> is required, for the same reason every <c>MatchRow</c>-located path in the
		/// library calls it: a consumer-declared global query filter would otherwise hide the stored row from
		/// this check, the guard would fall through, and the base member would hit the primary key. It is a
		/// check-then-act, and the window between the two is closed by wrapping the pair in a transaction at the
		/// Data Access Layer, which is policy this Data Access Object does not own.
		/// </remarks>
		public override void Insert(CompanyResource item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (Dataset.IgnoreQueryFilters().AsNoTracking().Any(MatchRow(item)))
				return;

			base.Insert(item);
		}

		/// <inheritdoc />
		/// <remarks>Rule 1 — a row is the pair, so a join sharing only one side of it is a different row.</remarks>
		protected override Expression<Func<CompanyResource, bool>> MatchRow(CompanyResource item)
		{
			return x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId;
		}

		/// <inheritdoc />
		/// <remarks>
		/// The pair, in its declared order. Rule 5 guarantees no particular order to a caller, but the keyless
		/// default throws until this is supplied, so <c>GetAll</c> needs it regardless of what the contract
		/// promises.
		/// </remarks>
		protected override IOrderedQueryable<CompanyResource> ApplyStableOrder(IQueryable<CompanyResource> query)
		{
			return query.OrderBy(x => x.CompanyId).ThenBy(x => x.ResourceId);
		}
	}
}
