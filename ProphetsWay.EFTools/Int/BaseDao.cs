#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;

using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Reflection;

namespace ProphetsWay.EFTools.Int
{
	public abstract class BaseDao<T> : IBaseDao<T> where T : class, IBaseIdEntity<int>
	{
		protected DbContext Context { get; }

		protected DbSet<T> Dataset { get; }

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
		private IDbContextTransaction _transaction;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		private DbContextTransaction _transaction;
#endif

		protected BaseDao(DbContext context)
		{
			Context = context;
			Dataset = Context.Set<T>();
			_transaction = null;
		}

		public T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
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

		public int Update(T item)
		{
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48

			Dataset.AddOrUpdate(item);
#endif
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
			var orig = Dataset.Single(x => x.Id.Equals(item.Id));
			var entry = Context.Entry(orig);

			entry.CurrentValues.SetValues(item);
			entry.State = EntityState.Modified;
#endif
			return Context.SaveChanges();
		}

		protected void EnsureBeginTransaction()
		{
			if (Context.Database.CurrentTransaction == null)
			{
				_transaction = Context.Database.BeginTransaction();
			}
		}

		protected void EnsureTransactionCommit()
		{
			_transaction?.Commit();
			_transaction = null;
		}

		protected void EnsureTransactionRollback()
		{
			_transaction?.Rollback();
			_transaction = null;
		}
	}
}
