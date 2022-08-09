using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;
using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	[Collection("pipeline-skip")]
	public class EFJobDaoTests : JobDaoTests
	{
		protected override IJobDao GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
