using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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

    internal enum TestStoreLifecycle
    {
        Disposable,
        Reusable
    }

    internal static class TestStore
    {
        internal const string ProviderVariable = "EFTOOLS_PROVIDER";

        private const string SqlServerDataSource = "localhost";
        private const string DisposableDatabasePrefix = "EFToolsTest_";
        private const string ReusableLeasePrefix = "EFToolsLease_";
        private const int MaximumLabelLength = 48;

        private static readonly object LiveIdentitiesLock = new object();
        private static readonly HashSet<string> RegisteredIdentities = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Store> ActiveReusableStores = new Dictionary<string, Store>(StringComparer.Ordinal);
        private static readonly HashSet<string> RetiredReusableIdentities = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Store> ActiveDisposableStores = new Dictionary<string, Store>(StringComparer.Ordinal);
        private static readonly HashSet<string> RetiredDisposableIdentities = new HashSet<string>(StringComparer.Ordinal);
        private static readonly AsyncLocal<ReusableSqlServerPool> ReusablePoolOverride = new AsyncLocal<ReusableSqlServerPool>();
        private static readonly object RealReusablePoolLock = new object();

        private static SqlServerConfiguration RealReusableConfiguration;
        private static ReusableSqlServerPool RealReusablePool;

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

        internal static int ReusableSqlServerWaitingCount
        {
            get
            {
                var pool = ReusablePoolOverride.Value;

                if (pool != null)
                    return pool.WaitingCount;

                lock (RealReusablePoolLock)
                    return RealReusablePool?.WaitingCount ?? 0;
            }
        }

        internal static bool IsDisposableIdentity(string identity)
        {
            return identity != null &&
                identity.Length > DisposableDatabasePrefix.Length &&
                identity.StartsWith(DisposableDatabasePrefix, StringComparison.Ordinal);
        }

        internal static TestStoreLifecycle ResolveSqlServerStoreLifecycle(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return TestStoreLifecycle.Disposable;

            switch (raw.Trim().ToUpperInvariant())
            {
                case "DISPOSABLE":
                    return TestStoreLifecycle.Disposable;

                case "REUSABLE":
                    return TestStoreLifecycle.Reusable;

                default:
                    throw new ArgumentException(
                        "Environment variable EFTOOLS_SQLSERVER_STORE_LIFECYCLE must be either Disposable or Reusable.",
                        nameof(raw));
            }
        }

        internal static IReadOnlyList<string> ValidateReusableScratchDatabaseNames(string first, string second)
        {
            var names = new[] { first?.Trim(), second?.Trim() };
            var variables = new[]
            {
                "EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1",
                "EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2"
            };
            var missingVariables = new List<string>();

            for (var index = 0; index < names.Length; index++)
            {
                if (string.IsNullOrEmpty(names[index]))
                    missingVariables.Add(variables[index]);
            }

            if (missingVariables.Count > 0)
                throw new ArgumentException(
                    $"Environment variables {string.Join(", ", missingVariables)} are required for Reusable stores.");

            for (var index = 0; index < names.Length; index++)
            {
                if (names[index].Length > 128 ||
                    !Regex.IsMatch(names[index], @"\AEFToolsScratch_[A-Za-z0-9_]{1,113}\z"))
                    throw new ArgumentException(
                        $"Environment variable {variables[index]} must match ^EFToolsScratch_[A-Za-z0-9_]{{1,113}}$ and be at most 128 characters.");
            }

            if (string.Equals(names[0], names[1], StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    $"Environment variables {variables[0]} and {variables[1]} must be distinct under ordinal-ignore-case comparison.");

            return names;
        }

        internal static bool IsLocalSqlServerEngineEdition(object edition = null)
        {
            return edition switch
            {
                byte value => value is 2 or 3 or 4,
                sbyte value => value is 2 or 3 or 4,
                short value => value is 2 or 3 or 4,
                ushort value => value is 2 or 3 or 4,
                int value => value is 2 or 3 or 4,
                uint value => value is 2 or 3 or 4,
                long value => value is 2 or 3 or 4,
                ulong value => value is 2 or 3 or 4,
                float value => value is 2 or 3 or 4,
                double value => value is 2 or 3 or 4,
                decimal value => value is 2 or 3 or 4,
                _ => false
            };
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

        internal static IDisposable UseReusableSqlServerPool(string first, string second, Action<string> reset)
        {
            var names = ValidateReusableScratchDatabaseNames(first, second);

            if (reset == null)
                throw new ArgumentNullException(nameof(reset));

            var pool = new ReusableSqlServerPool(names, reset, ReusablePoolOverride.Value);
            ReusablePoolOverride.Value = pool;
            return pool;
        }

        internal static Store OpenStore(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("A store label is required.", nameof(label));

            var pool = ReusablePoolOverride.Value;

            if (pool != null)
                return pool.Acquire(label);

            var provider = Provider;

            if (provider == TestStoreProvider.Sqlite)
                return new Store(provider, NewIdentity(label));

            var configuration = ReadSqlServerConfiguration();

            if (configuration.Lifecycle == TestStoreLifecycle.Reusable)
                return GetRealReusablePool(configuration).Acquire(label);

            return new Store(provider, NewIdentity(label));
        }

        internal static void ConfigureExisting(string identity, DbContextOptionsBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(identity))
                throw new ArgumentException("A store identity is required.", nameof(identity));

            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Store store;

            lock (LiveIdentitiesLock)
            {
                if (RetiredReusableIdentities.Contains(identity) || RetiredDisposableIdentities.Contains(identity))
                    throw new ObjectDisposedException(typeof(Store).FullName);

                if (!ActiveReusableStores.TryGetValue(identity, out store))
                    ActiveDisposableStores.TryGetValue(identity, out store);
            }

            if (store != null)
            {
                store.Configure(builder);
                return;
            }

            if (identity != "ProphetsWay.Example" && identity != "master")
                throw new ArgumentException("The identity is not an active test lease or an approved existing-store role.", nameof(identity));

            var provider = Provider;

            if (provider == TestStoreProvider.SqlServer)
            {
                var configuration = ReadSqlServerConfiguration();

                if (identity == "master" && configuration.Lifecycle != TestStoreLifecycle.Disposable)
                    throw new ArgumentException("The master role is available only for local Disposable lifecycle checks.", nameof(identity));

                builder.UseSqlServer(WithSqlServerDatabase(configuration.ConnectionString, identity));
                return;
            }

            if (identity != "ProphetsWay.Example")
                throw new ArgumentException("The identity is not an active test lease or an approved existing-store role.", nameof(identity));

            Configure(provider, identity, builder);
        }

        internal static bool StoreExists(string identity)
        {
            lock (LiveIdentitiesLock)
            {
                if (RetiredReusableIdentities.Contains(identity) || RetiredDisposableIdentities.Contains(identity))
                    return false;
            }

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
            var configuredConnectionString = Environment.GetEnvironmentVariable("EFTOOLS_SQLSERVER_CONNECTION_STRING");

            if (!string.IsNullOrWhiteSpace(configuredConnectionString))
            {
                try
                {
                    return WithSqlServerDatabase(configuredConnectionString, database);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Environment variable EFTOOLS_SQLSERVER_CONNECTION_STRING is not a valid SQL Server connection configuration.");
                }
            }

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

        private static SqlServerConfiguration ReadSqlServerConfiguration()
        {
            var lifecycle = ResolveSqlServerStoreLifecycle(
                Environment.GetEnvironmentVariable("EFTOOLS_SQLSERVER_STORE_LIFECYCLE"));
            var first = Environment.GetEnvironmentVariable("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1");
            var second = Environment.GetEnvironmentVariable("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2");
            IReadOnlyList<string> names = null;

            if (lifecycle == TestStoreLifecycle.Reusable)
                names = ValidateReusableScratchDatabaseNames(first, second);
            else if (!string.IsNullOrWhiteSpace(first) || !string.IsNullOrWhiteSpace(second))
                throw new ArgumentException(
                    "EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1 and EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2 require EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable.");

            var configuration = new SqlServerConfiguration(lifecycle, SqlServerConnectionString("master"), names);

            lock (RealReusablePoolLock)
                RequireMatchingReusableConfiguration(configuration);

            return configuration;
        }

        private static void RequireMatchingReusableConfiguration(SqlServerConfiguration configuration)
        {
            if (RealReusableConfiguration != null &&
                (configuration.Lifecycle != TestStoreLifecycle.Reusable ||
                configuration.ConnectionString != RealReusableConfiguration.ConnectionString ||
                configuration.DatabaseNames[0] != RealReusableConfiguration.DatabaseNames[0] ||
                configuration.DatabaseNames[1] != RealReusableConfiguration.DatabaseNames[1]))
                throw new InvalidOperationException(
                    "The reusable SQL Server configuration is fixed for this process; changing it cannot replace or recover the pool.");
        }

        private static ReusableSqlServerPool GetRealReusablePool(SqlServerConfiguration configuration)
        {
            lock (RealReusablePoolLock)
            {
                RequireMatchingReusableConfiguration(configuration);

                if (RealReusablePool == null)
                {
                    RealReusableConfiguration = configuration;
                    RealReusablePool = new ReusableSqlServerPool(
                        configuration.DatabaseNames,
                        database => ResetReusableSqlServerDatabase(configuration, database),
                        null,
                        configuration.ConnectionString);
                }

                return RealReusablePool;
            }
        }

        private static string WithSqlServerDatabase(string connectionString, string database)
        {
            return new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = database
            }.ConnectionString;
        }

        private static void ResetReusableSqlServerDatabase(SqlServerConfiguration configuration, string database)
        {
            if (configuration.Lifecycle != TestStoreLifecycle.Reusable ||
                (database != configuration.DatabaseNames[0] && database != configuration.DatabaseNames[1]))
                throw new ArgumentException("A reset requires an exclusively reserved configured scratch database.", nameof(database));

            try
            {
                using (var connection = new SqlConnection(WithSqlServerDatabase(configuration.ConnectionString, database)))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        try
                        {
                            command.CommandText =
                                "SELECT schemas.name, tables.name, foreignKeys.name " +
                                "FROM sys.foreign_keys AS foreignKeys " +
                                "JOIN sys.tables AS tables ON tables.object_id = foreignKeys.parent_object_id " +
                                "JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id " +
                                "WHERE tables.is_ms_shipped = 0; " +
                                "SELECT schemas.name, tables.name FROM sys.tables AS tables " +
                                "JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id " +
                                "WHERE tables.is_ms_shipped = 0;";
                            var statements = new List<string>();

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                    statements.Add(
                                        $"ALTER TABLE {QuoteSqlServerIdentifier(reader.GetString(0))}.{QuoteSqlServerIdentifier(reader.GetString(1))} " +
                                        $"DROP CONSTRAINT {QuoteSqlServerIdentifier(reader.GetString(2))}");

                                reader.NextResult();

                                while (reader.Read())
                                    statements.Add(
                                        $"DROP TABLE {QuoteSqlServerIdentifier(reader.GetString(0))}.{QuoteSqlServerIdentifier(reader.GetString(1))}");
                            }

                            foreach (var statement in statements)
                            {
                                command.CommandText = statement;
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped = 0";

                            if (Convert.ToInt32(command.ExecuteScalar()) != 0)
                                throw new InvalidOperationException("The scratch database still contains user tables after reset.");

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception failure)
            {
                var detail = failure is SqlException sqlFailure ? $" SQL error {sqlFailure.Number}." : string.Empty;
                throw new InvalidOperationException($"Reusable SQL Server schema reset failed ({failure.GetType().Name}).{detail}");
            }
        }

        private sealed class SqlServerConfiguration
        {
            internal SqlServerConfiguration(TestStoreLifecycle lifecycle, string connectionString, IReadOnlyList<string> databaseNames)
            {
                Lifecycle = lifecycle;
                ConnectionString = connectionString;
                DatabaseNames = databaseNames;
            }

            internal TestStoreLifecycle Lifecycle { get; }
            internal string ConnectionString { get; }
            internal IReadOnlyList<string> DatabaseNames { get; }
        }

        private static object CreateSqlServerDatabase(string identity, SqlServerConfiguration configuration)
        {
            if (!IsDisposableIdentity(identity))
                throw new ArgumentException("Only a disposable test identity may be created.", nameof(identity));

            if (configuration.Lifecycle != TestStoreLifecycle.Disposable)
                throw new InvalidOperationException("Database creation is unavailable with EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable.");

            using (var connection = new SqlConnection(configuration.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                var edition = OpenCheckedLocalSqlServerConnection(connection, command);
                command.CommandText = LocalSqlServerDatabaseCreateCommandText(identity);
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = identity;

                command.ExecuteNonQuery();
                return edition;
            }
        }

        private static void DropSqlServerDatabase(string identity, SqlServerConfiguration configuration, object checkedEdition)
        {
            if (!IsDisposableIdentity(identity))
                return;

            if (configuration == null || configuration.Lifecycle != TestStoreLifecycle.Disposable ||
                !IsLocalSqlServerEngineEdition(checkedEdition))
                throw new InvalidOperationException("Database deletion requires a checked local Disposable store; use EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable otherwise.");

            ClearSqlServerPool(configuration.ConnectionString, identity);

            using (var connection = new SqlConnection(configuration.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                OpenCheckedLocalSqlServerConnection(connection, command);
                command.CommandText = LocalSqlServerDatabaseDropCommandText(identity);
                command.Parameters.Add("@databaseName", SqlDbType.NVarChar, 128).Value = identity;
                command.ExecuteNonQuery();
            }

            ClearSqlServerPool(configuration.ConnectionString, identity);
        }

        private static object OpenCheckedLocalSqlServerConnection(SqlConnection connection, SqlCommand command)
        {
            try
            {
                connection.Open();
                command.CommandText = "SELECT SERVERPROPERTY('EngineEdition')";
                var edition = command.ExecuteScalar();

                if (!IsLocalSqlServerEngineEdition(edition))
                    throw new InvalidOperationException();

                return edition;
            }
            catch (Exception failure)
            {
                throw new InvalidOperationException(
                    $"Disposable SQL Server engine verification failed ({failure.GetType().Name}); use EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable with pre-existing scratch databases.");
            }
        }

        private static string LocalSqlServerDatabaseCreateCommandText(string identity)
        {
            return $"IF DB_ID(@databaseName) IS NULL CREATE DATABASE {QuoteSqlServerIdentifier(identity)}";
        }

        private static string LocalSqlServerDatabaseDropCommandText(string identity)
        {
            var quotedIdentity = QuoteSqlServerIdentifier(identity);

            return $"IF DB_ID(@databaseName) IS NOT NULL BEGIN " +
                $"ALTER DATABASE {quotedIdentity} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                $"DROP DATABASE {quotedIdentity}; END";
        }

        private static string QuoteSqlServerIdentifier(string identity)
        {
            return "[" + identity.Replace("]", "]]") + "]";
        }

        private static void ClearSqlServerPool(string connectionString, string identity)
        {
            using (var connection = new SqlConnection(WithSqlServerDatabase(connectionString, identity)))
                SqlConnection.ClearPool(connection);
        }

        private static void RegisterIdentity(string identity, Store store)
        {
            lock (LiveIdentitiesLock)
            {
                RegisteredIdentities.Add(identity);
                ActiveDisposableStores.Add(identity, store);
            }
        }

        private static void ForgetIdentity(string identity)
        {
            lock (LiveIdentitiesLock)
            {
                RegisteredIdentities.Remove(identity);
                ActiveDisposableStores.Remove(identity);
            }
        }

        private static void RetireDisposableIdentity(string identity)
        {
            lock (LiveIdentitiesLock)
            {
                ActiveDisposableStores.Remove(identity);
                RetiredDisposableIdentities.Add(identity);
            }
        }

        private static void RetireReusableIdentity(string identity)
        {
            lock (LiveIdentitiesLock)
            {
                ActiveReusableStores.Remove(identity);
                RegisteredIdentities.Remove(identity);
                RetiredReusableIdentities.Add(identity);
            }
        }

        internal sealed class ReusableSqlServerPool : IDisposable
        {
            private readonly Slot[] _slots;
            private readonly Action<string> _reset;
            private readonly ReusableSqlServerPool _previous;
            private readonly string _connectionString;
            private readonly object _gate = new object();

            private Exception _failure;
            private bool _closed;
            private int _waitingCount;

            internal ReusableSqlServerPool(
                IReadOnlyList<string> databaseNames,
                Action<string> reset,
                ReusableSqlServerPool previous,
                string connectionString = null)
            {
                _slots = new[] { new Slot(databaseNames[0]), new Slot(databaseNames[1]) };
                _reset = reset;
                _previous = previous;
                _connectionString = connectionString;
            }

            internal int WaitingCount
            {
                get
                {
                    lock (_gate)
                        return _waitingCount;
                }
            }

            internal Store Acquire(string label)
            {
                Slot slot;

                lock (_gate)
                {
                    while (true)
                    {
                        ThrowIfUnavailable();
                        slot = Array.Find(_slots, candidate => !candidate.Reserved && !candidate.Quarantined);

                        if (slot != null)
                        {
                            slot.Reserved = true;
                            break;
                        }

                        _waitingCount++;

                        try
                        {
                            Monitor.Wait(_gate);
                        }
                        finally
                        {
                            _waitingCount--;
                        }
                    }
                }

                try
                {
                    _reset(slot.DatabaseName);
                }
                catch (Exception failure)
                {
                    Quarantine(slot, failure);
                    throw;
                }

                lock (_gate)
                {
                    try
                    {
                        ThrowIfUnavailable();
                        slot.Lease = new Store(this, label, slot.DatabaseName);
                        return slot.Lease;
                    }
                    catch
                    {
                        slot.Reserved = false;
                        Monitor.PulseAll(_gate);
                        throw;
                    }
                }
            }

            internal void Configure(Store store, DbContextOptionsBuilder builder)
            {
                lock (_gate)
                {
                    ThrowIfUnavailable();
                    var slot = Array.Find(_slots, candidate => ReferenceEquals(candidate.Lease, store));

                    if (slot == null || !slot.Reserved)
                        throw new ObjectDisposedException(typeof(Store).FullName);

                    if (_connectionString == null)
                        TestStore.Configure(TestStoreProvider.SqlServer, slot.DatabaseName, builder);
                    else
                        builder.UseSqlServer(WithSqlServerDatabase(_connectionString, slot.DatabaseName));
                }
            }

            internal void Release(Store store)
            {
                Slot slot;

                lock (_gate)
                {
                    slot = Array.Find(_slots, candidate => ReferenceEquals(candidate.Lease, store));

                    if (slot == null)
                        throw new ObjectDisposedException(typeof(Store).FullName);

                    slot.Lease = null;
                }

                try
                {
                    _reset(slot.DatabaseName);
                }
                catch (Exception failure)
                {
                    Quarantine(slot, failure);
                    throw;
                }

                lock (_gate)
                {
                    slot.Reserved = false;
                    Monitor.PulseAll(_gate);
                    ThrowIfFaulted();
                }
            }

            private void Quarantine(Slot slot, Exception failure)
            {
                lock (_gate)
                {
                    slot.Quarantined = true;
                    slot.Reserved = false;
                    _failure ??= failure;
                    Monitor.PulseAll(_gate);
                }
            }

            private void ThrowIfFaulted()
            {
                if (_failure != null)
                    throw new InvalidOperationException("The reusable SQL Server pool is quarantined after a reset failure.", _failure);
            }

            private void ThrowIfUnavailable()
            {
                ThrowIfFaulted();

                if (_closed)
                    throw new ObjectDisposedException(GetType().FullName);
            }

            public void Dispose()
            {
                try
                {
                    var stores = new List<Store>();

                    lock (_gate)
                    {
                        if (_closed)
                            return;

                        _closed = true;

                        foreach (var slot in _slots)
                        {
                            if (slot.Lease != null)
                                stores.Add(slot.Lease);
                        }

                        Monitor.PulseAll(_gate);
                    }

                    var failures = new List<Exception>();

                    foreach (var store in stores)
                    {
                        var failure = store.DisposeReportingFailure();

                        if (failure != null)
                            failures.Add(failure);
                    }

                    if (failures.Count == 1)
                        ExceptionDispatchInfo.Capture(failures[0]).Throw();

                    if (failures.Count > 1)
                        throw new AggregateException("Reusable SQL Server pool cleanup failed.", failures);
                }
                finally
                {
                    if (ReferenceEquals(ReusablePoolOverride.Value, this))
                        ReusablePoolOverride.Value = _previous;
                }
            }

            private sealed class Slot
            {
                internal Slot(string databaseName)
                {
                    DatabaseName = databaseName;
                }

                internal string DatabaseName { get; }
                internal bool Reserved { get; set; }
                internal bool Quarantined { get; set; }
                internal Store Lease { get; set; }
            }
        }

        internal sealed class Store : IDisposable
        {
            private readonly TestStoreProvider _provider;
            private readonly SqliteConnection _sqliteKeeper;
            private readonly ReusableSqlServerPool _reusablePool;
            private readonly SqlServerConfiguration _sqlServerConfiguration;
            private readonly object _checkedEngineEdition;
            private readonly object _disposeLock = new object();

            private bool _disposed;

            internal Store(TestStoreProvider provider, string identity)
            {
                if (!IsDisposableIdentity(identity))
                    throw new ArgumentException($"Store identity '{identity}' is not disposable.", nameof(identity));

                if (provider == TestStoreProvider.SqlServer)
                {
                    _sqlServerConfiguration = ReadSqlServerConfiguration();

                    if (_sqlServerConfiguration.Lifecycle != TestStoreLifecycle.Disposable)
                        throw new ArgumentException("Reusable stores must be acquired through OpenStore, not constructed from database identities.", nameof(identity));
                }

                _provider = provider;
                Identity = identity;
                DatabaseName = identity;
                RegisterIdentity(identity, this);

                try
                {
                    if (provider == TestStoreProvider.Sqlite)
                    {
                        _sqliteKeeper = new SqliteConnection(SqliteConnectionString(identity));
                        _sqliteKeeper.Open();
                    }
                    else if (provider == TestStoreProvider.SqlServer)
                    {
                        _checkedEngineEdition = CreateSqlServerDatabase(identity, _sqlServerConfiguration);
                    }
                }
                catch
                {
                    _sqliteKeeper?.Dispose();
                    ForgetIdentity(identity);
                    throw;
                }
            }

            internal Store(ReusableSqlServerPool pool, string label, string databaseName)
            {
                _provider = TestStoreProvider.SqlServer;
                _reusablePool = pool;
                DatabaseName = databaseName;

                lock (LiveIdentitiesLock)
                {
                    string identity;

                    do
                    {
                        identity = ReusableLeasePrefix + NewIdentity(label).Substring(DisposableDatabasePrefix.Length);
                    }
                    while (ActiveReusableStores.ContainsKey(identity) || RetiredReusableIdentities.Contains(identity));

                    Identity = identity;
                    ActiveReusableStores.Add(identity, this);
                    RegisteredIdentities.Add(identity);
                }
            }

            internal string Identity { get; }
            internal string LeaseIdentity => Identity;
            internal string DatabaseName { get; }

            internal void Configure(DbContextOptionsBuilder builder)
            {
                if (builder == null)
                    throw new ArgumentNullException(nameof(builder));

                lock (_disposeLock)
                {
                    if (_disposed)
                        throw new ObjectDisposedException(GetType().FullName);

                    if (_reusablePool != null)
                        _reusablePool.Configure(this, builder);
                    else if (_provider == TestStoreProvider.SqlServer)
                        builder.UseSqlServer(WithSqlServerDatabase(_sqlServerConfiguration.ConnectionString, Identity));
                    else
                        TestStore.Configure(_provider, Identity, builder);
                }
            }

            public void Dispose()
            {
                var failure = DisposeReportingFailure();

                if (failure != null)
                    ExceptionDispatchInfo.Capture(failure).Throw();
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
                        if (_reusablePool != null)
                        {
                            RetireReusableIdentity(Identity);
                            _reusablePool.Release(this);
                        }
                        else
                        {
                            RetireDisposableIdentity(Identity);

                            if (_provider == TestStoreProvider.Sqlite)
                                _sqliteKeeper.Dispose();
                            else
                                DropSqlServerDatabase(Identity, _sqlServerConfiguration, _checkedEngineEdition);
                        }
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