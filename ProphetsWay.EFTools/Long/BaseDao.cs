#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;
using System.Linq;

namespace ProphetsWay.EFTools.Long
{
    public abstract class BaseDao<T> : RootBaseDao<T, long>, IBaseDao<T> where T : class, IBaseIdEntity<long>
    {
        protected BaseDao(DbContext context) : base(context) { }

        public override T Get(T item)
        {
            return Dao.Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
        }
    }
}
