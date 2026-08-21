#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Linq;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class CompanyDao : BasePagedDao<Company, int>, ICompanyDao
	{
		public CompanyDao(DbContext context) : base(context) { }

		public Company GetCustomCompanyFunction(int id)
		{
			return Dataset.OrderBy(x=> x.Id).Skip(id % GetCount(null)).First();
		}
	}
}
