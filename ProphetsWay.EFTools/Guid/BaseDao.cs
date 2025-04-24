#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;
using System.Linq;

namespace ProphetsWay.EFTools.Guid
{
    /// <summary>
    /// This is a base class for Data Access Objects (DAOs) that manage entities with a Guid ID.
    /// This class implements the basic CRUD operations, but you would likely prefer to use BaseGetAllDao or BaseGetPagedDao to either get all or get a paged list of entities.
    /// </summary>
    /// <typeparam name="T">The type of your entity this DAO will manage.</typeparam>
    public abstract class BaseDao<T> : RootBaseDao<T, System.Guid>, IBaseDao<T> where T : class, IBaseIdEntity<System.Guid>
    {
        protected BaseDao(DbContext context) : base(context) { }

        public override T Get(T item)
        {
            return Dao.Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
        }
    }
}
