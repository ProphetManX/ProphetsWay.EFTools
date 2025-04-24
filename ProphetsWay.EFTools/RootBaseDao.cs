#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProphetsWay.EFTools
{
    [EditorBrowsable(EditorBrowsableState.Never)] // Hides from IntelliSense
    public abstract class RootBaseDao<T, TIdType> : IBaseGetAllDao<T>, IBasePagedDao<T>, IBaseDao<T> where T : class, IBaseIdEntity<TIdType> where TIdType : struct
    {
        internal RootDao<T, TIdType> Dao;

        protected RootBaseDao(DbContext context)
        {
            Dao = new RootDao<T, TIdType>(context);
        }

        public DbContext Context => Dao.Context;
        public DbSet<T> Dataset => Dao.Dataset;

        public int Delete(T item)
        {
            return Dao.Delete(item);
        }

        public abstract T Get(T item);

        public IList<T> GetAll(T item)
        {
            return Dao.GetAll(item);
        }

        public int GetCount(T item)
        {
            return Dao.GetCount(item);
        }

        public IList<T> GetPaged(T item, int skip, int take)
        {
            return Dao.GetPaged(item, skip, take);
        }

        public void Insert(T item)
        {
            Dao.Insert(item);
        }

        public int Update(T item)
        {
            return Dao.Update(item);
        }

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
