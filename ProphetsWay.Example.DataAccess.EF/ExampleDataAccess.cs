#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
#endif

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.EF.Daos;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Collections.Generic;

namespace ProphetsWay.Example.DataAccess.EF
{
	public class ExampleDataAccess : BaseEFDataAccess<ExampleContext, int>, IExampleDataAccess
	{
		private readonly ICompanyDao _companyDao;
		private readonly IJobDao _jobDao;
		private readonly IUserDao _userDao;
		private readonly IResourceDao _resourceDao;
		private readonly ITransactionDao _transactionDao;



#if NET6_0_OR_GREATER
		public ExampleDataAccess() : this(new DbContextOptionsBuilder<ExampleContext>().UseInMemoryDatabase(typeof(ExampleContext).Name).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options) { }
#endif

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
public ExampleDataAccess(string connectionString) : base(connectionString) {
#endif

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
		
		public ExampleDataAccess(string connectionString) : this(new DbContextOptionsBuilder<ExampleContext>().UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options) { }

		public ExampleDataAccess(DbContextOptions options) : base(options)
		{
#endif
			_companyDao = new CompanyDao(Context);
			_jobDao = new JobDao(Context);
			_userDao = new UserDao(Context);
			_resourceDao = new ResourceDao(Context);
			_transactionDao = new TransactionDao(Context);
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
    }
}
