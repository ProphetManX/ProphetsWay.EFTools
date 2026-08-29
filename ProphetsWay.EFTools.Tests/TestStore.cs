using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProphetsWay.EFTools.Tests
{
    internal enum TestStoreProvider
    {
        SqlServer,
        Sqlite
    }

    internal static class TestStore
    {
        internal const string ProviderVariable = "EFTOOLS_PROVIDER";

        private const string SqlServerDataSource = "localhost";
        private const string DisposableDatabasePrefix = "EFToolsTest_";
        private const int MaximumLabelLength = 48;

        private static readonly object LiveIdentitiesLock = new object();
        private static readonly HashSet<string> RegisteredIdentities = new HashSet<string>(StringComparer.Ordinal);

        internal static TestStoreProvider Provider =>
            ResolveProvider(Environment.GetEnvironmentVariable(ProviderVariable));

        internal static IReadOnlyCollection<string> LiveIdentities
        {
            get
            {
                lock (LiveIdentitiesLock)
                    return new List<string>(RegisteredIdentities);
            }
        }

        internal static bool IsDisposableIdentity(string identity)
        {
            return identity != null &&
                identity.Length > DisposableDatabasePrefix.Length &&
                identity.StartsWith(DisposableDatabasePrefix, StringComparison.Ordinal);
        }

        internal static TestStoreProvider ResolveProvider(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return TestStoreProvider.SqlServer;

            switch (raw.Trim().ToUpperInvariant())
            {
                case "SQLSERVER":
                    return TestStoreProvider.SqlServer;

                case "SQLITE":
                    return TestStoreProvider.Sqlite;

                default:
                    throw new ArgumentException(
                        $"Environment variable {ProviderVariable} must be either SqlServer or Sqlite; received '{raw}'.",
                        nameof(raw));
            }
        }

        internal static string ProviderNameOf(TestStoreProvider provider)
        {
            switch (provider)
            {
                case TestStoreProvider.SqlServer:
                    return "Microsoft.EntityFrameworkCore.SqlServer";

                case TestStoreProvider.Sqlite:
                    return "Microsoft.EntityFrameworkCore.Sqlite";

                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown test-store provider.");
            }
        }

        internal static Store OpenStore(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("A store label is required.", nameof(label));

            return new Store(Provider, NewIdentity(label));
        }

        internal static void ConfigureExisting(string identity, DbContextOptionsBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(identity))
                throw new ArgumentException("A store identity is required.", nameof(identity));

            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Configure(Provider, identity, builder);
        }

        internal static bool StoreExists(string identity)
        {
            var builder = new DbContextOptionsBuilder();

            ConfigureExisting(identity, builder);

            using (var context = new DbContext(builder.Options))
            {
                var creator = context.Database.GetService<IRelationalDatabaseCreator>();

                return creator.Exists() && creator.HasTables();
            }
        }

        private static void Configure(
            TestStoreProvider provider,
            string identity,
            DbContextOptionsBuilder builder)
        {
            switch (provider)
            {
                case TestStoreProvider.SqlServer:
                    builder.UseSqlServer(SqlServerConnectionString(identity));
                    break;

                case TestStoreProvider.Sqlite:
                    builder.UseSqlite(SqliteConnectionString(identity));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown test-store provider.");
            }
        }

        private static string NewIdentity(string label)
        {
            var safeLabel = new StringBuilder(Math.Min(label.Length, MaximumLabelLength));

            foreach (var character in label)
            {
                if (safeLabel.Length == MaximumLabelLength)
                    break;

                var isAsciiLetter = character >= 'A' && character <= 'Z' ||
                    character >= 'a' && character <= 'z';
                var isDigit = character >= '0' && character <= '9';

                safeLabel.Append(isAsciiLetter || isDigit ? character : '_');
            }

            return $"{DisposableDatabasePrefix}{safeLabel}_{Guid.NewGuid():N}";
        }

        private static string SqlServerConnectionString(string database)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = SqlServerDataSource,
                InitialCatalog = database,
                IntegratedSecurity = true,
                TrustServerCertificate = true
            }.ConnectionString;
        }

        private static string SqliteConnectionString(string identity)
        {
            return new SqliteConnectionStringBuilder
            {
                DataSource = identity,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared,
                ForeignKeys = true,
                Pooling = false
            }.ConnectionString;
        }

        private static void CreateSqlServerDatabase(string identity)
        {
            if (!IsDisposableIdentity(identity))
                return;

            using (var connection = new SqlConnection(SqlServerConnectionString("master")))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText =
                    $"IF DB_ID(@databaseName) IS NULL CREATE DATABASE {QuoteSqlServerIdentifier(identity)}";
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = identity;
                command.ExecuteNonQuery();
            }
        }

        private static void DropSqlServerDatabase(string identity)
        {
            if (!IsDisposableIdentity(identity))
                return;

            ClearSqlServerPool(identity);

            using (var connection = new SqlConnection(SqlServerConnectionString("master")))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                var quotedIdentity = QuoteSqlServerIdentifier(identity);

                command.CommandText =
                    $"IF DB_ID(@databaseName) IS NOT NULL BEGIN " +
                    $"ALTER DATABASE {quotedIdentity} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                    $"DROP DATABASE {quotedIdentity}; END";
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = identity;
                command.ExecuteNonQuery();
            }

            ClearSqlServerPool(identity);
        }

        private static string QuoteSqlServerIdentifier(string identity)
        {
            return "[" + identity.Replace("]", "]]") + "]";
        }

        private static void ClearSqlServerPool(string identity)
        {
            using (var connection = new SqlConnection(SqlServerConnectionString(identity)))
                SqlConnection.ClearPool(connection);
        }

        private static void RegisterIdentity(string identity)
        {
            lock (LiveIdentitiesLock)
                RegisteredIdentities.Add(identity);
        }

        private static void ForgetIdentity(string identity)
        {
            lock (LiveIdentitiesLock)
                RegisteredIdentities.Remove(identity);
        }

        internal sealed class Store : IDisposable
        {
            private readonly TestStoreProvider _provider;
            private readonly SqliteConnection _sqliteKeeper;
            private readonly object _disposeLock = new object();

            private bool _disposed;

            internal Store(TestStoreProvider provider, string identity)
            {
                RegisterIdentity(identity);

                try
                {
                    if (provider == TestStoreProvider.Sqlite)
                    {
                        _sqliteKeeper = new SqliteConnection(SqliteConnectionString(identity));
                        _sqliteKeeper.Open();
                    }
                    else if (provider == TestStoreProvider.SqlServer)
                    {
                        CreateSqlServerDatabase(identity);
                    }
                }
                catch
                {
                    _sqliteKeeper?.Dispose();
                    ForgetIdentity(identity);
                    throw;
                }
            }

            internal string Identity { get; }

            internal void Configure(DbContextOptionsBuilder builder)
            {
                if (builder == null)
                    throw new ArgumentNullException(nameof(builder));

                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                TestStore.Configure(_provider, Identity, builder);
            }

            public void Dispose()
            {
                DisposeReportingFailure();
            }

            internal Exception DisposeReportingFailure()
            {
                lock (_disposeLock)
                {
                    if (_disposed)
                        return null;

                    _disposed = true;

                    try
                    {
                        if (_provider == TestStoreProvider.Sqlite)
                            _sqliteKeeper.Dispose();
                        else
                            DropSqlServerDatabase(Identity);
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                    finally
                    {
                        ForgetIdentity(Identity);
                    }

                    return null;
                }
            }
        }
    }
}