#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System.Collections.Generic;

namespace ProphetsWay.EFTools.Int
{
	public abstract class BasePagedDao<TEntityType> : BaseDao<TEntityType>, IBasePagedDao<TEntityType> where TEntityType : class, IBaseIdEntity<int>
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
