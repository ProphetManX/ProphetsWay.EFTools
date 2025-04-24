#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System.Collections.Generic;

namespace ProphetsWay.EFTools.Long
{
	public abstract class BasePagedDao<TEntityType> : BaseDao<TEntityType>, IBasePagedDao<TEntityType> where TEntityType : class, IBaseIdEntity<long>
	{
		protected BasePagedDao(DbContext context) : base(context) { }

		public int GetCount(TEntityType item)
		{
			return Dao.GetCount(item);
		}

		public IList<TEntityType> GetPaged(TEntityType item, int skip, int take)
		{
			return Dao.GetPaged(item, skip, take);
		}
	}
}
