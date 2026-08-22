#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System;

namespace ProphetsWay.EFTools
{
    public abstract class LegacyBaseSoftNonIdDao<T> : LegacyBaseNonIdDao<T>, IBaseDao<T> where T : class, IBaseSoftEntity
    {
        protected LegacyBaseSoftNonIdDao(DbContext context, bool UseUtcTime = true) : base(context)
        {
            this.UseUtcTime = UseUtcTime;
        }

        protected bool UseUtcTime { get; private set; }

#if NET461 || NET471 || NET48
        public override int Update(T item)
        {
            item.UpdatedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            return UpdateEntity(item);
        }
#endif
#if NET8_0_OR_GREATER
        public override int Update(T item)
        {
            item.UpdatedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            return UpdateEntity(item);
        }

        public override T Get(T item)
        {
            return Get(item, false);
        }

        /// <summary>
        /// Need to retrieve the entity by its ID(s) so we can update it.
        /// when AsTracking is true, must use AsTracking() to get the entity
        ///     return Dataset.AsTracking().Single(x=> x.Key1 == item.Key1 && x.Key2 == item.Key2);
        /// else
        ///     return Dataset.Single(x=> x.Key1 == item.Key1 && x.Key2 == item.Key2);
        /// </summary>
        public abstract T Get(T item, bool AsTracking);
#endif

        private int UpdateEntity(T item)
        {
#if NET461 || NET471 || NET48
            Dao.Dataset.AddOrUpdate(item);
#endif
#if NET8_0_OR_GREATER
            var entity = Get(item, true);
            var entityEntry = Dao.Context.Entry(entity);
            entityEntry.CurrentValues.SetValues(item);
            entityEntry.State = EntityState.Modified;
#endif
            return Dao.Context.SaveChanges();
        }

        public new int Delete(T item)
        {
            item.DeletedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            return UpdateEntity(item);
        }

        public new void Insert(T item)
        {
            item.CreatedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            Dao.Insert(item);
        }


    }
}
