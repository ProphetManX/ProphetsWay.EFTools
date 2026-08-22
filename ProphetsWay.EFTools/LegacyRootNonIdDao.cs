#if NET8_0_OR_GREATER 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
	internal class LegacyRootNonIdDao <T> where T : class, IBaseEntity
	{
		public DbContext Context { get; }

		public DbSet<T> Dataset { get; }

#if NET8_0_OR_GREATER
		private IDbContextTransaction _transaction;
#endif
#if NET461 || NET471 || NET48
        private DbContextTransaction _transaction;
#endif

		internal LegacyRootNonIdDao(DbContext context)
		{
			Context = context;
			Dataset = Context.Set<T>();
			_transaction = null;
		}

		public int Delete(T item)
		{
			Dataset.Remove(item);
			return Context.SaveChanges();
		}

		public void Insert(T item)
		{
			Dataset.Add(item);
			Context.SaveChanges();
		}

		public void EnsureBeginTransaction()
		{
			if (Context.Database.CurrentTransaction == null)
			{
				_transaction = Context.Database.BeginTransaction();
			}
		}

		public void EnsureTransactionCommit()
		{
			_transaction?.Commit();
			_transaction = null;
		}

		public void EnsureTransactionRollback()
		{
			_transaction?.Rollback();
			_transaction = null;
		}
	}
}
