using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;

namespace ProphetsWay.EFTools.Tests
{
	public static class Constants
	{
		private static class ConnectionStrings
		{
			// TrustServerCertificate because Microsoft.Data.SqlClient defaults Encrypt=true and a local developer
			// instance presents a self-signed certificate; without it every test fails at login, not at the query.
			public const string ProphetsWayExample = "Data Source=localhost;Initial Catalog=ProphetsWay.Example;Integrated Security=True;TrustServerCertificate=True";
		}

		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess(ConnectionStrings.ProphetsWayExample);
	}
}
