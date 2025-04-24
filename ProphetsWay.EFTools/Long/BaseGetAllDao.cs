#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools.Long
{
	public abstract class BaseGetAllDao<TEntityType> : BaseDao<TEntityType>, IBaseGetAllDao<TEntityType> where TEntityType : class, IBaseIdEntity<long>
	{
		protected BaseGetAllDao(DbContext context) : base(context) { }
	}
}
