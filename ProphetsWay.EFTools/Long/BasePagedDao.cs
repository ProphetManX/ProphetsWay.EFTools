#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools.Long
{
	public abstract class BasePagedDao<TEntityType> : BaseDao<TEntityType>, IBasePagedDao<TEntityType> where TEntityType : class, IBaseIdEntity<long>
	{
		protected BasePagedDao(DbContext context) : base(context) { }
	}
}
