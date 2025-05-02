#if NET8_0_OR_GREATER 
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ProphetsWay.EFTools
{
	internal class RootDao <T, TIdType> : RootNonIdDao<T> where T : class, IBaseIdEntity<TIdType> where TIdType : struct
	{
		internal RootDao(DbContext context) : base(context) { }

		public int Update(T item)
		{
#if NET461 || NET471 || NET48

			Dataset.AddOrUpdate(item);
#endif
#if NET8_0_OR_GREATER
            var orig = Dataset.AsTracking().Single(x => x.Id.Equals(item.Id));
			var entry = Context.Entry(orig);

			entry.CurrentValues.SetValues(item);
			entry.State = EntityState.Modified;
#endif
			return Context.SaveChanges();
		}

		public IList<T> GetAll(T item)
		{
			return Dataset.ToList();
		}

		public int GetCount(T item)
		{
			return Dataset.Count();
		}

		public IList<T> GetPaged(T item, int skip, int take)
		{
			return Dataset.OrderBy(x => x.Id).Skip(skip).Take(take).ToList();
		}
	}
}
