using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Data.Entity;
using System.Linq;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class CompanyDao : BasePagedDaoWithIntId<Company>, ICompanyDao
	{
		public CompanyDao(DbContext context) : base(context) { }

		public Company GetCustomCompanyFunction(int id)
		{
			return Dataset.Skip(id % GetCount(null)).First();
		}
	}
}
