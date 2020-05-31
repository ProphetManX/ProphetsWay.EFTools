using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFBaseDataAccessTests : BaseDataAccessTests
	{
		protected override IExampleDataAccess GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
