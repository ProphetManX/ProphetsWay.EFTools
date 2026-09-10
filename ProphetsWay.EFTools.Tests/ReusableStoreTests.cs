using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using Shouldly;

using ProphetsWay.Example.Tests;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	[Trait("Guard", "Seam")]
	[Collection(TestCollections.SharedStore)]
	public class ReusableStoreTests
	{
		private const string SeamTypeName = "ProphetsWay.EFTools.Tests.TestStore";
		private const string LifecycleTypeName = "ProphetsWay.EFTools.Tests.TestStoreLifecycle";

		private static Type SeamType()
		{
			var type = typeof(ReusableStoreTests).Assembly.GetType(SeamTypeName, false);

			type.ShouldNotBeNull($"D-035 requires internal reusable-store behavior on {SeamTypeName}.");

			return type;
		}

		private static Type LifecycleType()
		{
			var type = typeof(ReusableStoreTests).Assembly.GetType(LifecycleTypeName, false);

			type.ShouldNotBeNull($"D-035 requires the closed lifecycle enum '{LifecycleTypeName}'.");
			type.IsEnum.ShouldBeTrue("The lifecycle selection must be a closed set, not an arbitrary string.");

			return type;
		}

		private static MethodInfo Method(string name, params Type[] parameterTypes)
		{
			var method = SeamType().GetMethod(
				name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
				null,
				parameterTypes,
				null);

			method.ShouldNotBeNull(
				$"{SeamTypeName} must expose {name}({string.Join(", ", parameterTypes.Select(t => t.Name))}) for the D-035 harness.");

			return method;
		}

		private static object Invoke(MethodInfo method, params object[] arguments)
		{
			try
			{
				return method.Invoke(null, arguments);
			}
			catch (TargetInvocationException exception)
			{
				ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
				throw;
			}
		}

		private static object Lifecycle(string name)
		{
			var type = LifecycleType();
			Enum.IsDefined(type, name).ShouldBeTrue($"{LifecycleTypeName} is missing '{name}'.");
			return Enum.Parse(type, name);
		}

		private static object ResolveLifecycle(string raw)
		{
			return Invoke(Method("ResolveSqlServerStoreLifecycle", typeof(string)), raw);
		}

		private static IReadOnlyList<string> ValidateScratchNames(string first, string second)
		{
			var value = Invoke(Method("ValidateReusableScratchDatabaseNames", typeof(string), typeof(string)), first, second);
			var names = value as IEnumerable;

			names.ShouldNotBeNull("The reusable scratch-name validator must return both validated names.");

			return names.Cast<object>().Select(name => name as string).ToArray();
		}

		private static IDisposable UsePool(string first, string second, Action<string> reset)
		{
			var scope = Invoke(Method("UseReusableSqlServerPool", typeof(string), typeof(string), typeof(Action<string>)), first, second, reset);
			scope.ShouldBeAssignableTo<IDisposable>();
			return (IDisposable)scope;
		}

		private static object OpenStore(string label)
		{
			var store = Invoke(Method("OpenStore", typeof(string)), label);
			store.ShouldBeAssignableTo<IDisposable>();
			return store;
		}

		private static int ReusableSqlServerWaitingCount()
		{
			var property = SeamType().GetProperty(
				"ReusableSqlServerWaitingCount",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			property.ShouldNotBeNull(
				$"{SeamTypeName} must expose a read-only ReusableSqlServerWaitingCount property for the D-035 pool-boundary specification.");
			property.PropertyType.ShouldBe(typeof(int));

			var value = property.GetValue(null);
			value.ShouldBeOfType<int>();
			return (int)value;
		}

		private static async Task WaitForReusableSqlServerWaitingCount(int expectedCount, TimeSpan timeout)
		{
			var deadline = DateTime.UtcNow + timeout;

			while (ReusableSqlServerWaitingCount() != expectedCount)
			{
				if (DateTime.UtcNow >= deadline)
					throw new TimeoutException($"The reusable pool did not report exactly {expectedCount} queued acquisition(s) before the bounded wait expired.");

				await Task.Delay(10);
			}
		}

		private static string Property(object store, string name)
		{
			var property = store.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			property.ShouldNotBeNull($"A reusable store must expose {name} for lease diagnostics.");
			var value = property.GetValue(store) as string;
			string.IsNullOrWhiteSpace(value).ShouldBeFalse($"{name} must name the current lease or configured slot.");
			return value;
		}

		private static void Configure(object store)
		{
			var builder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder();
			var method = store.GetType().GetMethod("Configure", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
				new[] { typeof(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder) }, null);
			method.ShouldNotBeNull("A lease must configure a context only while it owns its slot.");
			InvokeInstance(method, store, builder);
		}

		private static object InvokeInstance(MethodInfo method, object target, params object[] arguments)
		{
			try
			{
				return method.Invoke(target, arguments);
			}
			catch (TargetInvocationException exception)
			{
				ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
				throw;
			}
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldDeclareExactlyDisposableAndReusableLifecycles()
		{
			string.Join(", ", Enum.GetNames(LifecycleType()).OrderBy(name => name, StringComparer.Ordinal))
				.ShouldBe("Disposable, Reusable",
					"D-035 permits exactly the local disposable lifecycle and the pre-existing reusable lifecycle.");
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" \t ")]
		[InlineData("disposable")]
		[InlineData(" Disposable ")]
		public void ShouldResolveUnsetAndDisposableLifecycleToDisposable(string raw)
		{
			ResolveLifecycle(raw).ShouldBe(Lifecycle("Disposable"));
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData("reusable")]
		[InlineData(" ReUsAbLe ")]
		public void ShouldResolveReusableLifecycleIgnoringCaseAndWhitespace(string raw)
		{
			ResolveLifecycle(raw).ShouldBe(Lifecycle("Reusable"));
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData("Azure")]
		[InlineData("ReusableStore")]
		[InlineData("Sqlite")]
		public void ShouldRefuseAnUnknownSqlServerStoreLifecycle(string raw)
		{
			var exception = Should.Throw<ArgumentException>(() => ResolveLifecycle(raw));

			exception.Message.ShouldContain("EFTOOLS_SQLSERVER_STORE_LIFECYCLE");
			exception.Message.ShouldContain("Disposable");
			exception.Message.ShouldContain("Reusable");
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldAcceptTwoDistinctValidReusableScratchDatabaseNames()
		{
			var names = ValidateScratchNames(" EFToolsScratch_One ", "EFToolsScratch_Two");

			names.ShouldBe(new[] { "EFToolsScratch_One", "EFToolsScratch_Two" });
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData(null, null)]
		[InlineData("", "")]
		[InlineData("EFToolsScratch_One", "EFToolsScratch_One")]
		[InlineData("EFToolsScratch_One", "eftoolsscratch_one")]
		[InlineData("EFToolsTest_Disposable", "EFToolsScratch_Two")]
		[InlineData("ProphetsWay.Example", "EFToolsScratch_Two")]
		[InlineData("master", "EFToolsScratch_Two")]
		[InlineData("EFToolsScratch_Invalid-Name", "EFToolsScratch_Two")]
		public void ShouldRefuseMissingOrUnsafeReusableScratchDatabaseNames(string first, string second)
		{
			Should.Throw<ArgumentException>(() => ValidateScratchNames(first, second));
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData("master")]
		[InlineData("model")]
		[InlineData("msdb")]
		[InlineData("tempdb")]
		[InlineData("resource")]
		public void ShouldRefuseEverySystemDatabaseName(string name)
		{
			Should.Throw<ArgumentException>(() => ValidateScratchNames(name, "EFToolsScratch_Two"));
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldReportEveryMissingReusableScratchVariableWithoutOpeningAConnection()
		{
			var first = Should.Throw<ArgumentException>(() => ValidateScratchNames(null, "EFToolsScratch_Two"));
			var second = Should.Throw<ArgumentException>(() => ValidateScratchNames("EFToolsScratch_One", null));
			var both = Should.Throw<ArgumentException>(() => ValidateScratchNames(null, null));

			first.Message.ShouldContain("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1");
			second.Message.ShouldContain("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2");
			both.Message.ShouldContain("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1");
			both.Message.ShouldContain("EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2");
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData(null, "EFToolsScratch_Two")]
		[InlineData("EFToolsScratch_One", null)]
		[InlineData("EFToolsScratch_Invalid-Name", "EFToolsScratch_Two")]
		public void ShouldRefuseInvalidReusablePoolConfigurationBeforeTheConnectionAndResetBoundaries(string first, string second)
		{
			var resetCalls = 0;
			Action<string> reset = _ =>
			{
				resetCalls++;
				throw new InvalidOperationException("The reset boundary must not be reached for invalid reusable configuration.");
			};

			using (var observer = SqlConnectionOpenObserver.Enter())
			{
				Should.Throw<ArgumentException>(() => UsePool(first, second, reset));
				observer.OpenAttempts.ShouldBe(0,
					"Invalid reusable configuration must be refused before a SqlClient connection-open attempt.");
			}

			resetCalls.ShouldBe(0,
				"UseReusableSqlServerPool must validate configuration before its supplied reset callback can run.");
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldObserveAndBlockSqlClientConnectionOpenBeforeNetworkAccess()
		{
			using (var observer = SqlConnectionOpenObserver.Enter())
			using (var connection = new SqlConnection("Data Source=invalid;Initial Catalog=invalid;Integrated Security=True"))
			{
				Should.Throw<SqlConnectionOpenAttemptException>(() => connection.Open());
				observer.OpenAttempts.ShouldBe(1,
					"The diagnostic observer must see and interrupt SqlClient's connection-open-before event before transport work begins.");
			}
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldReachTheSuppliedResetBoundaryOnlyWhenOpenStoreUsesAValidReusablePool()
		{
			var resetCalls = 0;
			Action<string> reset = _ =>
			{
				resetCalls++;
				throw new InvalidOperationException("reset boundary reached");
			};

			using (UsePool("EFToolsScratch_One", "EFToolsScratch_Two", reset))
			{
				Should.Throw<InvalidOperationException>(() => OpenStore(nameof(ShouldReachTheSuppliedResetBoundaryOnlyWhenOpenStoreUsesAValidReusablePool)));
			}

			resetCalls.ShouldBe(1,
				"OpenStore must reach the supplied reset callback for a valid reusable pool, so invalid pool setup cannot hide connection or reset work behind another path.");
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldApplyTheScratchNameLengthBoundaryBeforePoolConstruction()
		{
			var valid = "EFToolsScratch_" + new string('a', 113);
			var invalid = valid + "a";

			ValidateScratchNames(valid, "EFToolsScratch_Two")[0].ShouldBe(valid);
			Should.Throw<ArgumentException>(() => ValidateScratchNames(invalid, "EFToolsScratch_Two"));
		}

		[Theory]
		[Trait("Area", "ReusableStore")]
		[InlineData(2, true)]
		[InlineData(3, true)]
		[InlineData(4, true)]
		[InlineData(5, false)]
		[InlineData(1, false)]
		[InlineData(null, false)]
		[InlineData("3", false)]
		[InlineData(3.5, false)]
		public void ShouldAcceptOnlyTheClosedSetOfLocalSqlServerEngineEditions(object edition, bool accepted)
		{
			var result = Invoke(Method("IsLocalSqlServerEngineEdition", typeof(object)), edition);

			result.ShouldBeOfType<bool>();
			((bool)result).ShouldBe(accepted,
				"Only ServerProperty EngineEdition 2, 3, and 4 may enable local disposable database DDL.");
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldRefuseDBNullOverflowAndUnconvertibleEngineEditions()
		{
			var classifier = Method("IsLocalSqlServerEngineEdition", typeof(object));

			foreach (var edition in new object[] { DBNull.Value, long.MaxValue, ulong.MaxValue, decimal.MaxValue, new object(), Type.Missing })
				Invoke(classifier, edition).ShouldBe(false);
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldLeaseDistinctConfiguredSlotsAndNeverReuseALeaseIdentity()
		{
			var resets = new List<string>();
			using (UsePool("EFToolsScratch_One", "EFToolsScratch_Two", name => resets.Add(name)))
			{
				using (var first = (IDisposable)OpenStore(nameof(ShouldLeaseDistinctConfiguredSlotsAndNeverReuseALeaseIdentity)))
				using (var second = (IDisposable)OpenStore(nameof(ShouldLeaseDistinctConfiguredSlotsAndNeverReuseALeaseIdentity)))
				{
					Property(first, "LeaseIdentity").ShouldNotBe(Property(second, "LeaseIdentity"));
					Property(first, "DatabaseName").ShouldNotBe(Property(second, "DatabaseName"));
				}

				using (var next = (IDisposable)OpenStore(nameof(ShouldLeaseDistinctConfiguredSlotsAndNeverReuseALeaseIdentity)))
				{
					Property(next, "LeaseIdentity").ShouldNotBeNull();
				}
			}

			resets.Count.ShouldBeGreaterThanOrEqualTo(6, "Both acquisition and first disposal must reset a reusable slot.");
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public void ShouldRetireAHandleBeforeItsPhysicalSlotIsReused()
		{
			using (UsePool("EFToolsScratch_One", "EFToolsScratch_Two", _ => { }))
			{
				var retired = OpenStore(nameof(ShouldRetireAHandleBeforeItsPhysicalSlotIsReused));
				var identity = Property(retired, "LeaseIdentity");
				((IDisposable)retired).Dispose();
				using (var current = (IDisposable)OpenStore(nameof(ShouldRetireAHandleBeforeItsPhysicalSlotIsReused)))
				{
					Property(current, "LeaseIdentity").ShouldNotBe(identity);
					Should.Throw<ObjectDisposedException>(() => Configure(retired));
				}
			}
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public async Task ShouldWakeOneWaiterWhenAReusableLeaseIsReleased()
		{
			var pool = UsePool("EFToolsScratch_One", "EFToolsScratch_Two", _ => { });
			using (var cancellation = new CancellationTokenSource())
			{
				IDisposable first = null;
				IDisposable second = null;
				IDisposable firstWaiterLease = null;
				IDisposable secondWaiterLease = null;
				Task<IDisposable> firstWaiter = null;
				Task<IDisposable> secondWaiter = null;

				try
				{
					first = (IDisposable)OpenStore("first");
					second = (IDisposable)OpenStore("second");
					firstWaiter = Task.Run(() => (IDisposable)OpenStore("third"), cancellation.Token);
					secondWaiter = Task.Run(() => (IDisposable)OpenStore("fourth"), cancellation.Token);

					var bothWaitersQueued = WaitForReusableSqlServerWaitingCount(2, TimeSpan.FromSeconds(1));
					var prematureAcquisition = Task.WhenAny(firstWaiter, secondWaiter);
					var initialOutcome = await Task.WhenAny(bothWaitersQueued, prematureAcquisition);
					initialOutcome.ShouldBe(bothWaitersQueued,
						"Both acquisitions must queue inside the reusable pool before either held slot is released.");
					await bothWaitersQueued;
					firstWaiter.IsCompleted.ShouldBeFalse("Neither queued acquisition may complete while both physical slots are held.");
					secondWaiter.IsCompleted.ShouldBeFalse("Neither queued acquisition may complete while both physical slots are held.");

					first.Dispose();
					first = null;
					var winner = await Task.WhenAny(firstWaiter, secondWaiter).WaitAsync(TimeSpan.FromSeconds(1));
					var loser = winner == firstWaiter ? secondWaiter : firstWaiter;
					var oneWaiterQueued = WaitForReusableSqlServerWaitingCount(1, TimeSpan.FromSeconds(1));
					var secondPrematureAcquisition = await Task.WhenAny(oneWaiterQueued, loser);
					secondPrematureAcquisition.ShouldBe(oneWaiterQueued,
						"Releasing one slot may permit exactly one queued acquisition while the other remains queued.");
					await oneWaiterQueued;
					loser.IsCompleted.ShouldBeFalse("One original waiter must remain queued while the second physical slot is held.");

					second.Dispose();
					second = null;
					await Task.WhenAll(firstWaiter, secondWaiter).WaitAsync(TimeSpan.FromSeconds(1));
					firstWaiterLease = await firstWaiter;
					secondWaiterLease = await secondWaiter;
				}
				finally
				{
					try
					{
						first?.Dispose();
						second?.Dispose();

						if (firstWaiter != null && secondWaiter != null)
						{
							await Task.WhenAll(firstWaiter, secondWaiter).WaitAsync(TimeSpan.FromSeconds(1));

							if (firstWaiter.Status == TaskStatus.RanToCompletion)
								firstWaiterLease = await firstWaiter;

							if (secondWaiter.Status == TaskStatus.RanToCompletion)
								secondWaiterLease = await secondWaiter;

							firstWaiterLease?.Dispose();
							secondWaiterLease?.Dispose();
						}
					}
					finally
					{
						pool.Dispose();
					}
				}
			}
		}

		private sealed class SqlConnectionOpenAttemptException : Exception
		{
		}

		private sealed class SqlConnectionOpenObserver : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>, IDisposable
		{
			private const string SqlClientDiagnosticListenerName = "SqlClientDiagnosticListener";
			private static readonly AsyncLocal<SqlConnectionOpenObserver> Current = new AsyncLocal<SqlConnectionOpenObserver>();
			private static readonly HashSet<string> ConnectionOpenBeforeEvents = new HashSet<string>(StringComparer.Ordinal)
			{
				"Microsoft.Data.SqlClient.WriteConnectionOpenBefore",
				"Microsoft.Data.SqlClient.WriteConnectionOpenBeforeAsync"
			};

			private readonly IDisposable listeners;
			private IDisposable sqlClientEvents;

			private SqlConnectionOpenObserver()
			{
				listeners = DiagnosticListener.AllListeners.Subscribe(this);
			}

			public int OpenAttempts { get; private set; }

			public static SqlConnectionOpenObserver Enter()
			{
				var observer = new SqlConnectionOpenObserver();
				Current.Value.ShouldBeNull("Connection-open observation must not leak between concurrently executing tests.");
				Current.Value = observer;
				return observer;
			}

			public void OnNext(DiagnosticListener listener)
			{
				if (listener.Name == SqlClientDiagnosticListenerName)
					sqlClientEvents = listener.Subscribe(this, ConnectionOpenBeforeEvents.Contains);
			}

			public void OnNext(KeyValuePair<string, object> diagnosticEvent)
			{
				if (ReferenceEquals(Current.Value, this) && ConnectionOpenBeforeEvents.Contains(diagnosticEvent.Key))
				{
					OpenAttempts++;
					throw new SqlConnectionOpenAttemptException();
				}
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
			}

			public void Dispose()
			{
				Current.Value.ShouldBe(this, "Connection-open observation must leave the flowed test context it entered.");
				Current.Value = null;
				sqlClientEvents?.Dispose();
				listeners.Dispose();
			}
		}

		[Fact]
		[Trait("Area", "ReusableStore")]
		public async Task ShouldQuarantineAResetFailureAndFailCurrentAndWaitingOperations()
		{
			var fail = false;
			using (UsePool("EFToolsScratch_One", "EFToolsScratch_Two", _ => { if (fail) throw new InvalidOperationException("reset failed"); }))
			{
				var first = (IDisposable)OpenStore("first");
				var second = (IDisposable)OpenStore("second");
				var waiter = Task.Run(() => OpenStore("waiter"));
				fail = true;

				Should.Throw<InvalidOperationException>(() => first.Dispose());
				await Should.ThrowAsync<InvalidOperationException>(async () => await waiter);
				Should.Throw<InvalidOperationException>(() => second.Dispose());
				Should.Throw<InvalidOperationException>(() => OpenStore("later"));
			}
		}
	}
}