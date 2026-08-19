using Microsoft.EntityFrameworkCore;

using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	/// <summary>
	/// The Entity Framework implementation of <see cref="IDepartmentDao"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It descends from no base in <c>ProphetsWay.EFTools</c>. <c>RootBaseSoftDao</c> is the nearest fit and
	/// satisfies none of rules 2, 3, 6, 7, 11, 12, 14 or 19: its <c>Update</c> is a whole-object replacement
	/// that wipes <see cref="Department.DeletedDate"/>, its <c>Delete</c> re-stamps an already-deleted row, its
	/// <c>GetPaged</c> carries no <c>ORDER BY</c>, and it has no <c>Restore</c> at all. Every member below is
	/// therefore written against the context directly.
	/// </para>
	/// <para>
	/// <b>Rule 19 against Entity Framework change tracking.</b> Reads project into fresh instances and writes
	/// mutate a tracked instance this class fetched itself, never the caller's. The caller's object is never
	/// handed to the change tracker, so an edit made to it after a call returns has nothing to travel along.
	/// Each write detaches what it tracked — on the failure path as well as the success one — so the context
	/// does not accumulate entities a later <c>SaveChanges</c> on a sibling Data Access Object would flush.
	/// </para>
	/// <para>
	/// <b>Rule 18 against a relational provider.</b> <c>datetime2</c> stores no
	/// <see cref="DateTimeKind"/>, so every stamp comes back <see cref="DateTimeKind.Unspecified"/> and this
	/// class restores it on read. A <c>ValueConverter</c> in <see cref="ExampleContext"/> would satisfy the
	/// rule too — with one installed the provider effectively supplies <see cref="DateTimeKind.Utc"/>, which
	/// rule 18 permits — but it is the wrong scope twice over. Rule 18 is a per-Data-Access-Object rule and a
	/// converter is model-wide, so it would also re-kind a department reached as
	/// <see cref="User.Department"/>, where <c>ProphetsWay.Example.DataAccess.NoDB</c> restores nothing. The
	/// two Data Access Layers would then observably differ on the navigation path, and "the same tests pass
	/// against both" is the claim this repository exists to prove.
	/// </para>
	/// </remarks>
	internal class DepartmentDao : IDepartmentDao
	{
		private readonly DbContext _context;

		public DepartmentDao(DbContext context)
		{
			_context = context;
		}

		private DbSet<Department> Dataset => _context.Set<Department>();

		/// <inheritdoc />
		public void Insert(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stamp = DateTime.UtcNow;

			//rule 19 - the store receives a copy, so the caller's instance is read and not adopted
			var stored = new Department
			{
				Name = item.Name,
				Description = item.Description,
				CreatedDate = stamp,
				UpdatedDate = null,
				DeletedDate = null
			};

			Dataset.Add(stored);
			Save(stored);

			//rule 1 - the generated identifier and the stamps travel back onto the caller's instance
			item.Id = stored.Id;
			item.CreatedDate = stamp;
			item.UpdatedDate = null;
			item.DeletedDate = null;
		}

		/// <inheritdoc />
		public Department Get(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			//rule 8 - a soft-deleted department is found here just like a live one
			return Snapshot(Read(item.Id));
		}

		/// <inheritdoc />
		public int Update(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = Track(item.Id);

			//rule 4 - a soft-deleted department is updated like any other, so DeletedDate is not consulted here
			if (stored == null)
				return 0;

			var stamp = DateTime.UtcNow;

			//rule 3 - the department's own data, and nothing else. CreatedDate and DeletedDate are left as the
			//store holds them, so an Update can neither rewrite history nor soft-delete behind Delete's back.
			stored.Name = item.Name;
			stored.Description = item.Description;

			//rule 2 - Update owns UpdatedDate
			stored.UpdatedDate = stamp;

			Save(stored);

			item.UpdatedDate = stamp;

			//rule 2 states the return as an existence question - 1 when a department with that Id is stored - which
			//Track already answered. SaveChanges answers "rows written", which is a different question, and it masks
			//nothing: a row another process removed between the Track and the Save raises DbUpdateConcurrencyException
			//rather than reporting zero. The literal returns in Delete and Restore are the same reading of rules 5 and 7.
			return 1;
		}

		/// <inheritdoc />
		public int Delete(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = Track(item.Id);

			if (stored == null)
				return 0;

			//rule 6 - already deleted, so the existing stamp stands and still reports when it happened
			if (stored.DeletedDate.HasValue)
			{
				Detach(stored);
				return 0;
			}

			var stamp = DateTime.UtcNow;

			//rule 5 - a soft delete: the row stays, only the stamp is written
			stored.DeletedDate = stamp;

			Save(stored);

			item.DeletedDate = stamp;

			return 1;
		}

		/// <inheritdoc />
		public int Restore(Department item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var stored = Track(item.Id);

			if (stored == null)
				return 0;

			if (!stored.DeletedDate.HasValue)
			{
				Detach(stored);
				return 0;
			}

			//a lifecycle change, not a modification - UpdatedDate is deliberately left alone
			stored.DeletedDate = null;

			Save(stored);

			item.DeletedDate = null;

			return 1;
		}

		/// <inheritdoc />
		public IList<Department> GetAll(Department item)
		{
			//rule 13 - item is a type selector and is null when the call arrives through the dispatcher

			//the first ToList is not redundant: it ends the query so Snapshot runs on the client, as Entity Framework
			//cannot translate a method group into SQL
			return Live().ToList().Select(Snapshot).ToList();
		}

		/// <inheritdoc />
		public IList<Department> GetPaged(Department item, int skip, int take)
		{
			//rule 12 - the bounds are rejected before any data is read
			if (skip < 0)
				throw new ArgumentOutOfRangeException(nameof(skip), skip, "A page cannot skip a negative number of departments.");

			if (take < 0)
				throw new ArgumentOutOfRangeException(nameof(take), take, "A page cannot take a negative number of departments.");

			//the first ToList is not redundant - see GetAll
			return Live().Skip(skip).Take(take).ToList().Select(Snapshot).ToList();
		}

		/// <inheritdoc />
		public int GetCount(Department item)
		{
			return Dataset.AsNoTracking().Count(x => x.DeletedDate == null);
		}

		/// <summary>
		/// The live set, ordered explicitly. Rule 11 asks for an ordering that is stable across calls, which a
		/// relational store supplies only when the query says so.
		/// </summary>
		private IQueryable<Department> Live()
		{
			return Dataset.AsNoTracking().Where(x => x.DeletedDate == null).OrderBy(x => x.Id);
		}

		private Department Read(int id)
		{
			return Dataset.AsNoTracking().SingleOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// The stored row for <paramref name="id"/>, tracked so that a write can be made against it.
		/// </summary>
		/// <remarks>
		/// A tracking query performs identity resolution rather than throwing, so an instance a sibling Data Access
		/// Object left tracked for this identifier would come back carrying that sibling's in-memory values instead
		/// of the store's — rules 3 and 6 would then compute from the wrong numbers, silently. Releasing the one
		/// entity this call is about to write is enough; clearing the whole change tracker belongs to the Data
		/// Access Layer, not to a Data Access Object.
		/// </remarks>
		private Department Track(int id)
		{
			var tracked = _context.ChangeTracker.Entries<Department>().Where(x => x.Entity.Id == id).ToList();

			foreach (var entry in tracked)
				entry.State = EntityState.Detached;

			return Dataset.AsTracking().SingleOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// Flushes the pending write and releases <paramref name="tracked"/> whether or not it succeeded.
		/// </summary>
		/// <remarks>
		/// The context is shared by every Data Access Object on the Data Access Layer, so an entity left <c>Added</c>
		/// or <c>Modified</c> by a failed <c>SaveChanges</c> is flushed by the next sibling's write — a phantom row,
		/// or a rule 19 violation reached through an exception path.
		/// </remarks>
		private void Save(Department tracked)
		{
			try
			{
				_context.SaveChanges();
			}
			finally
			{
				Detach(tracked);
			}
		}

		private void Detach(Department entity)
		{
			_context.Entry(entity).State = EntityState.Detached;
		}

		/// <summary>
		/// A field-for-field copy carrying rule 18's <see cref="DateTimeKind"/> back onto the three stamps.
		/// </summary>
		private static Department Snapshot(Department source)
		{
			if (source == null)
				return null;

			return new Department
			{
				Id = source.Id,
				Name = source.Name,
				Description = source.Description,
				CreatedDate = AsUtc(source.CreatedDate),
				UpdatedDate = AsUtc(source.UpdatedDate),
				DeletedDate = AsUtc(source.DeletedDate)
			};
		}

		private static DateTime AsUtc(DateTime value)
		{
			return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
		}

		private static DateTime? AsUtc(DateTime? value)
		{
			return value.HasValue ? AsUtc(value.Value) : (DateTime?)null;
		}
	}
}
