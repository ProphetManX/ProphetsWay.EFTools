using ProphetsWay.BaseDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
	public abstract class BaseGetAllWithIntIdDao<T> : BaseGetAllDao<T, int>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<int>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllWithIntIdDao(DbContext context) : base(context) { }
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseGetAllWithLongIdDao<T> : BaseGetAllDao<T, long>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<long>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllWithLongIdDao(DbContext context) : base(context) { }
	}

#pragma warning disable CS0618 // Type or member is obsolete
	public abstract class BaseGetAllWithGuidIdDao<T> : BaseGetAllDao<T, Guid>, IBaseGetAllDao<T> where T : class, IBaseIdEntity<Guid>
#pragma warning restore CS0618 // Type or member is obsolete
	{
		protected BaseGetAllWithGuidIdDao(DbContext context) : base(context) { }
	}
}
