using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFResourceDaoTests : ResourceDaoTests
	{
		protected override IResourceDao GetIExampleDataAccess => Constants.GetExampleDataAccess;
	}
}
