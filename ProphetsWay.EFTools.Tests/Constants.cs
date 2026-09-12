using System;

using Microsoft.EntityFrameworkCore;

using ProphetsWay.Example.DataAccess;
using ProphetsWay.Example.DataAccess.EF;

namespace ProphetsWay.EFTools.Tests
{
	public static class Constants
	{
		private static class ConnectionStrings
		{
			public const string ProphetsWayExample = "ProphetsWay.Example";
		}

		private static readonly Lazy<DbContextOptions<ExampleContext>> ExampleOptions =
			new Lazy<DbContextOptions<ExampleContext>>(CreateExampleOptions, true);

		// Root the keeper connection for the process lifetime; shared in-memory SQLite disappears when it closes.
		private static TestStore.Store _sqliteStore;

		public static IExampleDataAccess GetExampleDataAccess => new ExampleDataAccess(ExampleOptions.Value);

		private static DbContextOptions<ExampleContext> CreateExampleOptions()
		{
			var builder = new DbContextOptionsBuilder<ExampleContext>();

			if (TestStore.Provider == TestStoreProvider.Sqlite)
			{
				_sqliteStore = TestStore.OpenStore(ConnectionStrings.ProphetsWayExample);
				_sqliteStore.Configure(builder);

				using (var context = new ExampleContext(builder.Options))
					context.Database.EnsureCreated();
			}
			else
			{
				TestStore.ConfigureExisting(ConnectionStrings.ProphetsWayExample, builder);
			}

			builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

			return builder.Options;
		}
	}
}
