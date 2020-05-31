using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFCompanyDaoTests : CompanyDaoTests
	{
		protected override ICompanyDao GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
