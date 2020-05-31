#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#endif
#if NETSTANDARD2_1
using System.Data.Entity;
using System.Data.Entity.Migrations;
# endif
using ProphetsWay.BaseDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProphetsWay.EFTools
{
	[Obsolete("You should be using one of the three new implementations:  BasePagedWithIntIdDao, BasePagedWithLongIdDao, or BasePagedWithGuidIdDao")]
	public abstract class BasePagedDao<TEntityType, TIdType> : BaseDao<TEntityType, TIdType>, IBasePagedDao<TEntityType> where TEntityType : class, IBaseIdEntity<TIdType>
	{
		protected BasePagedDao(DbContext context) : base(context) { }

		public int GetCount(TEntityType item)
		{
			return Dataset.Count();
		}

		public IList<TEntityType> GetPaged(TEntityType item, int skip, int take)
		{
			return Dataset.OrderBy(x=> x.Id).Skip(skip).Take(take).ToList();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BasePagedDaoWithIntId<T> : BasePagedDao<T, int>, IBasePagedDao<T> where T : class, IBaseIdEntity<int>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BasePagedDaoWithIntId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BasePagedDaoWithLongId<T> : BasePagedDao<T, long>, IBasePagedDao<T> where T : class, IBaseIdEntity<long>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BasePagedDaoWithLongId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BasePagedDaoWithGuidId<T> : BasePagedDao<T, Guid>, IBasePagedDao<T> where T : class, IBaseIdEntity<Guid>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BasePagedDaoWithGuidId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}
}
