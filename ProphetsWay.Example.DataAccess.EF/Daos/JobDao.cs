using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Data.Entity;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class JobDao : BaseGetAllDaoWithIntId<Job>, IJobDao
	{
		public JobDao(DbContext context) : base(context) { }
	}
}
