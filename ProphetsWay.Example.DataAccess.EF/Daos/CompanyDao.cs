#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Linq;
using ProphetsWay.EFTools.Int;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class CompanyDao : BasePagedDao<Company>, ICompanyDao
	{
		public CompanyDao(DbContext context) : base(context) { }

		public Company GetCustomCompanyFunction(int id)
		{
			return Dataset.OrderBy(x=> x.Id).Skip(id % GetCount(null)).First();
		}
	}
}
