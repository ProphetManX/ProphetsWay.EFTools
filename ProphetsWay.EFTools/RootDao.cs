#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
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

namespace ProphetsWay.EFTools
{
	internal class RootDao <T, TIdType> where T : class, IBaseIdEntity<TIdType> where TIdType : struct
	{
		public DbContext Context { get; }

		public DbSet<T> Dataset { get; }

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
		private IDbContextTransaction _transaction;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		private DbContextTransaction _transaction;
#endif

		internal RootDao(DbContext context)
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

		public int Update(T item)
		{
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48

			Dataset.AddOrUpdate(item);
#endif
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
			var orig = Dataset.AsTracking().Single(x => x.Id.Equals(item.Id));
			var entry = Context.Entry(orig);

			entry.CurrentValues.SetValues(item);
			entry.State = EntityState.Modified;
#endif
			return Context.SaveChanges();
		}

		public IList<T> GetAll(T item)
		{
			return Dataset.ToList();
		}

		public int GetCount(T item)
		{
			return Dataset.Count();
		}

		public IList<T> GetPaged(T item, int skip, int take)
		{
			return Dataset.OrderBy(x => x.Id).Skip(skip).Take(take).ToList();
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
