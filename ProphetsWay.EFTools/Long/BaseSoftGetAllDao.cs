#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools.Long
{
    /// <summary>
    /// This is a base class for Data Access Objects (DAOs) that manage entities with an long integer ID and take advantage of Soft Deletes (set deleted date, but keep records in the database).
    /// This class implements the basic CRUD operations as well as the GetAll method, returning all entities of the specified type (generally used for smaller tables or for lookup references).
    /// </summary>
    /// <typeparam name="TEntityType">The type of your entity this DAO will manage.</typeparam>
    public abstract class BaseSoftGetAllDao<TEntityType> : BaseDao<TEntityType>, IBaseGetAllDao<TEntityType> where TEntityType : class, IBaseSoftIdEntity<long>
    {
        protected BaseSoftGetAllDao(DbContext context) : base(context) { }
    }
}
