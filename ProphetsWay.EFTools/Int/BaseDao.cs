#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System.Data;
using System.Linq;

namespace ProphetsWay.EFTools.Int
{
	public abstract class BaseDao<T> : IBaseDao<T> where T : class, IBaseIdEntity<int>
	{
		internal RootDao<T, int> Dao;

		protected BaseDao(DbContext context) {
			Dao = new RootDao<T, int>(context);
		}

		public DbContext Context => Dao.Context;
		public DbSet<T> Dataset => Dao.Dataset;

		public int Delete(T item)
		{
			return Dao.Delete(item);
		}

		public T Get(T item)
		{
			return Dao.Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}

		public void Insert(T item)
		{
			Dao.Insert(item);
		}

		public int Update(T item)
		{
			return Dao.Update(item);
		}

		public void EnsureBeginTransaction()
		{
			Dao.EnsureBeginTransaction();
		}

		public void EnsureTransactionCommit()
		{
			Dao.EnsureTransactionCommit();
		}

		public void EnsureTransactionRollback()
		{
			Dao.EnsureTransactionRollback();
		}
	}
}
