#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class JobDao : BaseGetAllDaoWithIntId<Job>, IJobDao
	{
		public JobDao(DbContext context) : base(context) { }
	}
}
