using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFJobDaoTests : JobDaoTests
	{
		protected override IJobDao GetIExampleDataAccess => Constants.GetExampleDataAccess;
	}
}
