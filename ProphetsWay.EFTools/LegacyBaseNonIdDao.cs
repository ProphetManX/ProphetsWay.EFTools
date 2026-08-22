#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

using System.Linq;

namespace ProphetsWay.EFTools
{
    public abstract class LegacyBaseNonIdDao<T> : IBaseDao<T> where T: class, IBaseEntity
    {
        internal LegacyRootNonIdDao<T> Dao;

        protected LegacyBaseNonIdDao(DbContext context)
        {
            Dao = new LegacyRootNonIdDao<T>(context);
        }

        public DbContext Context => Dao.Context;
        public DbSet<T> Dataset => Dao.Dataset;

        public int Delete(T item)
        {
            return Dao.Delete(item);
        }

        public abstract T Get(T item);

        public void Insert(T item)
        {
            Dao.Insert(item);
        }

        public abstract int Update(T item);

        public void EnsureBeginTransaction()
        {
            Dao.EnsureBeginTransaction();
        }

        public void EnsureTransactionCommit()
        {
            Dao.EnsureTransactionCommit();
        }

        public void EnsureTransactionRollback()
        {
            Dao.EnsureTransactionRollback();
        }
    }
}
