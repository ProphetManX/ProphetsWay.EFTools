#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools.Guid
{
    /// <summary>
    /// This is a base class for Data Access Objects (DAOs) that manage entities with an Guid ID and take advantage of Soft Deletes (set deleted date, but keep records in the database).
    /// This class implements the basic CRUD operations as well as GetCount/GetPaged methods, allowing for advanced UI paging control of the data.
    /// </summary>
    /// <typeparam name="TEntityType">The type of your entity this DAO will manage.</typeparam>
	public abstract class BaseSoftPagedDao<TEntityType> : BaseSoftDao<TEntityType>, IBasePagedDao<TEntityType> where TEntityType : class, IBaseSoftIdEntity<System.Guid>
    {
        protected BaseSoftPagedDao(DbContext context) : base(context) { }
    }
}
