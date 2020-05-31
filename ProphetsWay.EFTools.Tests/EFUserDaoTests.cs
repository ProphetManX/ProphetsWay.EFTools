using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFUserDaoTests : UserDaoTests
	{
		protected override IUserDao GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
