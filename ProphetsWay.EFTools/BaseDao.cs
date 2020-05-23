using ProphetsWay.BaseDataAccess;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

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
		protected BaseDao(DbContext context) : base(context)
		{
			Dataset = Context.Set<TEntityType>();
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
			Dataset.AddOrUpdate(item);
			return Context.SaveChanges();
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
