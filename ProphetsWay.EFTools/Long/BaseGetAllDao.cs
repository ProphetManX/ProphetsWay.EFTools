#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProphetsWay.EFTools.Long
{
	public abstract class BaseGetAllDao<TEntityType> : BaseDao<TEntityType>, IBaseGetAllDao<TEntityType> where TEntityType : class, IBaseIdEntity<long>
	{
		protected BaseGetAllDao(DbContext context) : base(context) { }

		public IList<TEntityType> GetAll(TEntityType item)
		{
			return Dataset.ToList();
		}
	}
}
