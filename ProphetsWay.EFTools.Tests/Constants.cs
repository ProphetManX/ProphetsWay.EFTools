using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;

namespace ProphetsWay.EFTools.Tests
{
	public static class Constants
	{
		private static class ConnectionStrings
		{
			public const string ProphetsWayExample = "Data Source=localhost;Initial Catalog=ProphetsWay.Example;Integrated Security=True";
		}

		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess(ConnectionStrings.ProphetsWayExample);
	}
}
