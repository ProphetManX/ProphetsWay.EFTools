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

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
#endif

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess(Constants.ConnectionStrings.ProphetsWayExample);
#endif

#if NET6_0_OR_GREATER
		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess();
#endif
	}
}
