#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NETSTANDARD2_1
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
	[Obsolete]
	public abstract class BaseDao
	{
		protected DbContext Context { get; }

		protected BaseDao(DbContext context)
		{
			Context = context;
		}
	}

	[Obsolete("You should be using one of the three new implementations:  BaseDaoWithIntId, BaseDaoWithLongId, or BaseDaoWithGuidId")]
	public abstract class BaseDao<TEntityType, TIdType> : BaseDao, IBaseDao<TEntityType> where TEntityType : class, IBaseIdEntity<TIdType>
	{
		protected DbSet<TEntityType> Dataset { get; }

#if NETSTANDARD2_0
		private IDbContextTransaction _transaction;
#endif
#if NETSTANDARD2_1
		private DbContextTransaction _transaction;
#endif


		protected BaseDao(DbContext context) : base(context)
		{
			Dataset = Context.Set<TEntityType>();
			_transaction = null;
		}

		public int Delete(TEntityType item)
		{
			Dataset.Remove(item);
			return Context.SaveChanges();
		}

		public abstract TEntityType Get(TEntityType item);

		public void Insert(TEntityType item)
		{
			Dataset.Add(item);
			Context.SaveChanges();
		}

		public int Update(TEntityType item)
		{
#if NETSTANDARD2_1

			Dataset.AddOrUpdate(item);
#endif
#if NETSTANDARD2_0
			var orig = Dataset.Single(x => x.Id.Equals(item.Id));
			var entry = Context.Entry(orig);

			entry.CurrentValues.SetValues(item);
			entry.State = EntityState.Modified;
#endif
			return Context.SaveChanges();
		}
		protected void EnsureBeginTransaction()
		{
			if(Context.Database.CurrentTransaction == null)
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

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseDaoWithIntId<T> : BaseDao<T, int>, IBaseDao<T> where T : class, IBaseIdEntity<int>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseDaoWithIntId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseDaoWithLongId<T> : BaseDao<T, long>, IBaseDao<T> where T : class, IBaseIdEntity<long>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseDaoWithLongId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseDaoWithGuidId<T> : BaseDao<T, Guid>, IBaseDao<T> where T : class, IBaseIdEntity<Guid>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseDaoWithGuidId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}
}
