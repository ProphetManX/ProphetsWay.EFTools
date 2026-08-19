using Microsoft.EntityFrameworkCore;

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.EF.Daos;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System;
using System.Collections.Generic;

namespace ProphetsWay.Example.DataAccess.EF
{
	public class ExampleDataAccess : BaseEFDataAccess<ExampleContext>, IExampleDataAccess
	{
		private readonly ICompanyDao _companyDao;
		private readonly IJobDao _jobDao;
		private readonly IUserDao _userDao;
		private readonly IResourceDao _resourceDao;
		private readonly ITransactionDao _transactionDao;
		private readonly IDepartmentDao _departmentDao;

		/// <summary>
		/// Builds a SQL Server-backed context from a connection string and owns it.
		/// </summary>
		/// <remarks>
		/// The provider is named here, in the consumer's own file — the library names none. A PostgreSQL
		/// consumer writes <c>UseNpgsql</c>, a SQLite one <c>UseSqlite</c>, and neither needs anything from
		/// <c>ProphetsWay.EFTools</c> to do it.
		/// </remarks>
		public ExampleDataAccess(string connectionString)
			: this(new ExampleContext(new DbContextOptionsBuilder<ExampleContext>()
				.UseSqlServer(connectionString)
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
				.Options), ContextOwnership.Owned)
		{ }

		/// <summary>
		/// Builds a context from options the caller has already configured — provider included — and owns it.
		/// </summary>
		public ExampleDataAccess(DbContextOptions<ExampleContext> options)
			: this(new ExampleContext(options), ContextOwnership.Owned)
		{ }

		/// <summary>
		/// Takes a context someone else created and will dispose — a dependency-injection container, or a test
		/// that configured its own in-memory database.
		/// </summary>
		public ExampleDataAccess(ExampleContext context)
			: this(context, ContextOwnership.Borrowed)
		{ }

		private ExampleDataAccess(ExampleContext context, ContextOwnership ownership)
			: base(context, ownership)
		{
			_companyDao = new CompanyDao(Context);
			_jobDao = new JobDao(Context);
			_userDao = new UserDao(Context);
			_resourceDao = new ResourceDao(Context);
			_transactionDao = new TransactionDao(Context);
			_departmentDao = new DepartmentDao(Context);
		}


#region CompanyDao

		public Company Get(Company item)
		{
			return _companyDao.Get(item);
		}

		public int GetCount(Company item)
		{
			return _companyDao.GetCount(item);
		}

		public Company GetCustomCompanyFunction(int id)
		{
			return _companyDao.GetCustomCompanyFunction(id);
		}

		public IList<Company> GetPaged(Company item, int skip, int take)
		{
			return _companyDao.GetPaged(item, skip, take);
		}

		public void Insert(Company item)
		{
			_companyDao.Insert(item);
		}

		public int Delete(Company item)
		{
			return _companyDao.Delete(item);
		}

		public int Update(Company item)
		{
			return _companyDao.Update(item);
		}

#endregion

#region JobDao

		public int Delete(Job item)
		{
			return _jobDao.Delete(item);
		}

		public Job Get(Job item)
		{
			return _jobDao.Get(item);
		}

		public IList<Job> GetAll(Job item)
		{
			return _jobDao.GetAll(item);
		}

		public void Insert(Job item)
		{
			_jobDao.Insert(item);
		}

		public int Update(Job item)
		{
			return _jobDao.Update(item);
		}

#endregion

#region UserDao

		public int Delete(User item)
		{
			return _userDao.Delete(item);
		}

		public void CustomUserFunctionality(User user)
		{
			_userDao.CustomUserFunctionality(user);
		}

		public User Get(User item)
		{
			return _userDao.Get(item);
		}

		public void Insert(User item)
		{
			_userDao.Insert(item);
		}

		public int Update(User item)
		{
			return _userDao.Update(item);
		}

#endregion

#region TransactionDao

        public IList<Transaction> GetPaged(Transaction item, int skip, int take)
        {
            return _transactionDao.GetPaged(item, skip, take);
        }

        public int GetCount(Transaction item)
        {
			return _transactionDao.GetCount(item);
        }

        public Transaction Get(Transaction item)
        {
			return _transactionDao.Get(item);
        }

        public void Insert(Transaction item)
        {
			_transactionDao.Insert(item);
        }

        public int Update(Transaction item)
        {
			return _transactionDao.Update(item);
        }

        public int Delete(Transaction item)
        {
			return _transactionDao.Delete(item);
        }

#endregion
		
#region ResourceDao

		public IList<Resource> GetAll(Resource item)
        {
			return _resourceDao.GetAll(item);
        }

        public Resource Get(Resource item)
        {
			return _resourceDao.Get(item);
        }

        public void Insert(Resource item)
        {
			_resourceDao.Insert(item);
        }

        public int Update(Resource item)
        {
            return _resourceDao.Update(item);
        }

        public int Delete(Resource item)
        {
			return _resourceDao.Delete(item);
        }

#endregion

#region DepartmentDao

		//BaseEFDataAccess asks every member a derived Data Access Layer declares to open with this. The Borrowed
		//constructor is why it is not redundant: the context outlives this instance there, so without the guard a
		//disposed Data Access Layer would keep answering against a live context.
		public Department Get(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.Get(item);
		}

		public void Insert(Department item)
		{
			ThrowIfDisposed();
			_departmentDao.Insert(item);
		}

		public int Update(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.Update(item);
		}

		public int Delete(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.Delete(item);
		}

		public IList<Department> GetAll(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.GetAll(item);
		}

		public IList<Department> GetPaged(Department item, int skip, int take)
		{
			ThrowIfDisposed();
			return _departmentDao.GetPaged(item, skip, take);
		}

		public int GetCount(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.GetCount(item);
		}

		public int Restore(Department item)
		{
			ThrowIfDisposed();
			return _departmentDao.Restore(item);
		}

#endregion

#region CompanyResourceDao - NOT IMPLEMENTED

		public void Insert(CompanyResource item)
		{
			throw NotWrittenYet("ICompanyResourceDao.Insert(CompanyResource)");
		}

		public int Delete(CompanyResource item)
		{
			throw NotWrittenYet("ICompanyResourceDao.Delete(CompanyResource)");
		}

		public IList<CompanyResource> GetAll(CompanyResource item)
		{
			throw NotWrittenYet("ICompanyResourceDao.GetAll(CompanyResource)");
		}

#endregion

		private static NotImplementedException NotWrittenYet(string member)
		{
			return new NotImplementedException(
				member + " has NOT been implemented. The ProphetsWay.EFTools 3.x lap 1 was scoped to making this " +
				"repository compile against ProphetsWay.Example 3.1.0 and nothing else. Department (soft-delete) and " +
				"CompanyResource (keyless) have no Entity Framework Data Access Object, and ExampleContext maps " +
				"neither entity. See docs/api-contract.md for the specification this member must satisfy.");
		}
	}
}
