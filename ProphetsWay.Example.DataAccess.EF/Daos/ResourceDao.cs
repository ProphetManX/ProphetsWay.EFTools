#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.EFTools.Guid;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class ResourceDao : BaseGetAllDao<Resource>, IResourceDao
	{
		public ResourceDao(DbContext context) : base(context) { }

		public new void Insert(Resource item)
        {
			if(item.Id == default(Guid))
				item.Id = Guid.NewGuid();

			base.Insert(item);
        }
	}
}
