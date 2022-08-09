using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;
using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	[Collection("pipeline-skip")]
	public class EFCompanyDaoTests : CompanyDaoTests
	{
		protected override ICompanyDao GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
