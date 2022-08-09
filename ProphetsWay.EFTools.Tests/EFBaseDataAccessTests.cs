using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.Tests;
using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	[Collection("pipeline-skip")]
	public class EFBaseDataAccessTests : BaseDataAccessTests
	{
		protected override IExampleDataAccess GetIExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
	}
}
