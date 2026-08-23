using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	/// <summary>
	/// The Entity Framework implementation of <see cref="IDepartmentDao"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Eighteen of the nineteen numbered rules are the family's, and not one of them is overridden to obtain.
	/// <see cref="BaseSoftDao{TEntity, TKey}"/> supplies the soft <c>Insert</c>, <c>Update</c> and <c>Delete</c>
	/// semantics (rules 1-6) and the <c>DeletedDate == null</c> read filter (rules 9 and 15) — it adds that and
	/// nothing else, which is rule 15 in terms. <see cref="BaseDao{TEntity, TKey}"/> supplies <c>Get</c>, which
	/// applies no read filter and so finds a deleted department (rule 8); the retrieval trio and its type-selector
	/// parameter (rules 10, 13); an explicit stable <c>ORDER BY</c> (rule 11) and the paging boundaries (rule 12);
	/// the <see cref="ArgumentNullException"/> guards on the write members and their absence on the read members
	/// (rule 14); and the untracked reads and copy-on-write that make a retrieved instance a snapshot and an
	/// argument read rather than adopted (rules 17, 19). <c>NormalizeRetrievedTimestamp</c>'s default restores the
	/// <see cref="DateTimeKind"/> a relational provider does not persist (rule 18). Rule 16 arrives unchanged from
	/// the parent dispatcher's reflective setter.
	/// </para>
	/// <para>
	/// <b>The family is <see cref="BaseSoftPagedDao{TEntity, TKey}"/> because
	/// <see cref="IDepartmentDao"/> declares <see cref="IBasePagedDao{T}"/></b>, which wins the fixed precedence
	/// over <see cref="IBaseGetAllDao{T}"/>. The <see cref="IBaseGetAllDao{T}"/> it also declares is satisfied by
	/// the <c>GetAll</c> <see cref="BaseDao{TEntity, TKey}"/> already supplies — a base-class public method may
	/// implement an interface a derived class names — so no union family is needed and none exists.
	/// </para>
	/// <para>
	/// <b>Rule 7 is the exception, and the only member written here.</b> <see cref="Restore"/> belongs to
	/// <see cref="IDepartmentDao"/> and to no library contract. It cannot be expressed through <c>Update</c>,
	/// which deliberately preserves the stored <see cref="Department.DeletedDate"/>.
	/// </para>
	/// <para>
	/// <b>Rule 18's scope, restated because a reader will come here looking for it.</b> A value converter in
	/// <see cref="ExampleContext"/> would satisfy the rule too, and is the wrong scope twice over: rule 18 is a
	/// per-Data-Access-Object rule and a converter is model-wide, so it would also re-kind a department reached as
	/// <see cref="User.Department"/>, where <c>ProphetsWay.Example.DataAccess.NoDB</c> restores nothing. The two
	/// Data Access Layers would then observably differ on the navigation path, and "the same tests pass against
	/// both" is the claim this repository exists to prove.
	/// </para>
	/// </remarks>
	internal class DepartmentDao : BaseSoftPagedDao<Department, int>, IDepartmentDao
	{
		public DepartmentDao(DbContext context) : base(context)
		{
		}

		/// <inheritdoc />
		public int Restore(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			//TrackForWrite starts from the raw Dataset, which is what keeps ApplyReadFilter off this query. The
			//filter excludes exactly the rows Restore exists to reach, so a Restore composed on top of it would
			//return 0 for every department it was ever asked about. It also pre-detaches anything already tracked
			//for this row, because a tracking query resolves identity rather than re-reading - without that, the
			//guard below could decide from a sibling Data Access Object's in-memory value.
			var stored = TrackForWrite(item);

			if (stored == null)
				return 0;

			try
			{
				if (!stored.DeletedDate.HasValue)
					return 0;

				//a lifecycle change, not a modification - UpdatedDate is deliberately left alone
				stored.DeletedDate = null;

				Context.SaveChanges();

				item.DeletedDate = null;

				//rule 7 asks whether the department was deleted, which the guard above already answered.
				//SaveChanges answers "rows written", which is a different question.
				return 1;
			}
			finally
			{
				//stored is tracked from the moment TrackForWrite returns, so every exit owes this: the success path,
				//a throwing SaveChanges, and the early return that writes nothing. One left behind carries a pending
				//DeletedDate = null that the next write on the shared context would flush.
				Context.Entry(stored).State = EntityState.Detached;
			}
		}
	}
}
