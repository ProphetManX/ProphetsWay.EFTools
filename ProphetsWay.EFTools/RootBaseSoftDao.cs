#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ProphetsWay.EFTools
{
    [EditorBrowsable(EditorBrowsableState.Never)] // Hides from IntelliSense
    public abstract class RootBaseSoftDao<T, TIdType> : RootBaseDao<T, TIdType>, IBaseGetAllDao<T>, IBasePagedDao<T>, IBaseDao<T> where T : class, IBaseSoftIdEntity<TIdType> where TIdType : struct
    {
        protected RootBaseSoftDao(DbContext context, bool UseUtcTime = true) : base(context)
        {
            this.UseUtcTime = UseUtcTime;
        }

        protected bool UseUtcTime {get; private set;} 
        public new int Update(T item)
        {
            item.UpdatedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            return base.Update(item);
        }

        public new void Insert(T item)
        {
            item.CreatedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            base.Insert(item);
        }

        public new int Delete(T item)
        {
            item.DeletedDate = UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            return base.Update(item);
        }

        public new virtual IList<T> GetPaged(T item, int skip, int take)
        {
            var paged = Dataset.Where(x => x.DeletedDate == null).Skip(skip).Take(take);
            return paged.ToList();
        }

        public new int GetCount(T item)
        {
            return Dataset.Count(x => x.DeletedDate == null);
        }

        public new IList<T> GetAll(T item)
        {
            return Dataset.Where(x => x.DeletedDate == null).ToList();
        }
    }
}
