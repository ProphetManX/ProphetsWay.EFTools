#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#endif
#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProphetsWay.EFTools
{
	[Obsolete("You should be using one of the three new implementations:  BaseGetAllWithIntIdDao, BaseGetAllWithLongIdDao, or BaseGetAllWithGuidIdDao")]
	public abstract class BaseGetAllDao<TEntityType, TIdType> : BaseDao<TEntityType, TIdType>, IBaseGetAllDao<TEntityType> where TEntityType : class, IBaseIdEntity<TIdType>
	{
		protected BaseGetAllDao(DbContext context) : base(context) { }

		public IList<TEntityType> GetAll(TEntityType item)
		{
			return Dataset.ToList();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseGetAllDaoWithIntId<T> : BaseGetAllDao<T, int>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<int>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllDaoWithIntId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseGetAllDaoWithLongId<T> : BaseGetAllDao<T, long>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<long>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllDaoWithLongId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseGetAllDaoWithGuidId<T> : BaseGetAllDao<T, Guid>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<Guid>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllDaoWithGuidId(DbContext context) : base(context) { }

		public override T Get(T item)
		{
			return Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
		}
	}
}
