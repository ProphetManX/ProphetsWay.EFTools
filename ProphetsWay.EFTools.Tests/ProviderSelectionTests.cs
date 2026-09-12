using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.Tests;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The specification for the one thing this assembly does not have: a <b>shared provider selection</b> that
	/// every store-backed test in it obeys, so that a whole-suite run can be certified against a single relational
	/// provider.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>What is wrong today, and why no smaller change fixes it.</b> The suite reaches two providers by
	/// construction and cannot be pointed at one. The thirteen <c>EF*Tests</c> adapters reach SQL Server through
	/// <c>TestSeam</c> → <c>Constants.GetExampleDataAccess</c>; <b>six</b> store-backed local classes each open
	/// their own <c>SqliteConnection("Filename=:memory:")</c> and call <c>EnsureCreated()</c>, and reach
	/// <c>Constants</c> never; a seventh, <see cref="AlternateKeyGuardSpikeTests"/>, parameterizes two providers
	/// per <c>[Theory]</c> on purpose and is exempt for the reason given below. Editing <c>Constants</c> therefore
	/// moves thirteen classes and no others, which is why the release gate — one complete run in which <i>every</i>
	/// store-touching case outside the fourteen-case spike reached the selected provider — is unreachable from here.
	/// </para>
	/// <para>
	/// <b>Why a seam that merely answers questions about itself is not enough.</b> Several of the obligations
	/// below exist because an otherwise conforming seam can satisfy the rest without a single statement ever
	/// reaching SQL Server. <see cref="ShouldApplyTheResolverToTheEnvironmentItReads"/> and
	/// <see cref="ShouldBindTheSelectionToTheEnvironmentVariableItNames"/> anchor the ambient selection to the
	/// variable the seam itself names, so <c>Provider</c> cannot be a constant;
	/// <see cref="ShouldNameTheEntityFrameworkProviderExactly"/> pins the two provider names to literals, so the
	/// store assertions stop comparing the seam's answer against the seam's other answer; and
	/// <see cref="ShouldRouteEveryProviderSelectionInThisAssemblyThroughTheSeam"/> reads the compiled assembly, so
	/// a class that keeps — or later re-grows — a provider call of its own is red rather than quietly certified
	/// alongside everything else.
	/// </para>
	/// <para>
	/// <b>Cleanup is the one obligation the seam must not be allowed to answer.</b>
	/// <see cref="ShouldRemoveTheStoreItselfWhenItIsDisposed"/> asks <c>StoreExists</c>, and a seam that keeps a
	/// <c>HashSet</c> its own <c>Dispose</c> removes from passes it with no <c>DROP DATABASE</c> anywhere — the
	/// same closed loop the literal provider names exist to break, arriving through the one door that decides
	/// whether a certification run leaves a database per store behind on the owner's instance. So
	/// <see cref="ShouldLetEntityFrameworkFindALiveStoreByItsIdentity"/> and
	/// <see cref="ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed"/> hand the identity back through
	/// <c>ConfigureExisting</c>, open a <i>second</i> context on it and ask Entity Framework's own
	/// <see cref="IRelationalDatabaseCreator"/> whether that store exists and carries tables — then require
	/// <c>StoreExists</c> to agree, in both directions. The seam does not author that answer, so bookkeeping
	/// cannot produce it.
	/// </para>
	/// <para>
	/// <b>The seam this class specifies.</b> A single <c>internal static class TestStore</c> alongside an
	/// <c>internal enum TestStoreProvider { SqlServer, Sqlite }</c>, exposing the selected provider, a pure
	/// resolver over the raw environment value, the Entity Framework provider name for a selection,
	/// <c>OpenStore(string)</c> — a disposable, isolated store that configures any
	/// <see cref="DbContextOptionsBuilder"/> handed to it — and
	/// <c>ConfigureExisting(string identity, DbContextOptionsBuilder)</c>, which points a builder at a store
	/// <i>named</i> rather than owned, and which must never bring one into being. It is deliberately
	/// <b>non-generic</b>: every local class builds a <c>DbContextOptionsBuilder&lt;TContext&gt;</c>, which derives
	/// from the non-generic builder, so one <c>Configure</c> serves all seven without a type parameter and without
	/// a provider branch in any of them.
	/// </para>
	/// <para>
	/// <b>Why the default is SQL Server rather than SQLite.</b> The suite already cannot run without a local SQL
	/// Server — thirteen of the fifteen discovering classes require one today — so defaulting to it costs nothing
	/// that is not already required, and it makes the release-gating run the ordinary one rather than an opt-in a
	/// release can forget to take. SQLite stays available as the opt-in fast leg, which is the leg a build agent
	/// would select once it selects anything at all.
	/// </para>
	/// <para>
	/// <b>An unrecognised selection must be fatal, and that is the point of
	/// <see cref="ShouldRefuseAnUnrecognisedProviderNameRatherThanFallingBack"/>.</b> A resolver that silently
	/// falls back to a default turns one mistyped environment variable into a green run that is then reported as
	/// SQL Server certification. That is the same false green <c>TestSeamTests</c> and <c>AdapterCoverageTests</c>
	/// exist to prevent, arriving through a third door. <c>InMemory</c> is refused by name for a second reason: it
	/// honours no transaction, so the transaction contract cannot be certified against it at all.
	/// </para>
	/// <para>
	/// <b>The provider-comparison spike is outside provider certification, deliberately.</b>
	/// <see cref="AlternateKeyGuardSpikeTests"/> parameterizes SQLite and InMemory per <c>[Theory]</c> because its
	/// subject is Entity Framework Core's own alternate-key behaviour <i>compared across</i> providers — its own
	/// remarks call it "an empirical spike, not a specification." Forcing it onto the selected provider destroys
	/// the comparison, which is the measurement. It is therefore left pinned, and
	/// <see cref="ShouldKeepTheProviderComparisonSpikeOutsideProviderCertification"/> holds it to
	/// <c>Scope=Characterization</c> so it stays outside the conformance gate and so the footnote on "all 270 on
	/// SQL Server" cannot quietly become untrue.
	/// </para>
	/// <para>
	/// <b>Trait key.</b> <c>Guard</c>/<c>Seam</c>, for the reasons set out on <c>TestSeamTests</c>: upstream's
	/// <c>Scope</c> key partitions upstream's specification and a downstream assembly has no business extending
	/// it, and an untraited class is excluded from the documented gate <c>--filter "Scope=Contract|Guard=Seam"</c>
	/// — which is precisely the run this seam has to survive.
	/// </para>
	/// <para>
	/// <b>Collection.</b> Almost every store opened here is this class's own — but
	/// <see cref="ShouldRunTheAdaptedUpstreamSuiteOnTheSelectedProvider"/> calls
	/// <c>TestDataAccessFactory.Create()</c> and so reaches the one shared store the thirteen adapters count rows
	/// in. One case is enough: this class therefore joins <c>TestCollections.SharedStore</c>, exactly as
	/// <c>TestSeamTests</c> did for the same reason. The read is harmless today and becomes a race the moment
	/// <c>Constants</c> starts provisioning a store of its own, which is the very next lap.
	/// </para>
	/// <para>
	/// <b>These assertions are made by reflection on purpose.</b> Naming <c>TestStore</c> in code would stop the
	/// assembly compiling and produce no counts, no trait accounting and no way to tell a missing seam from a
	/// database that was not reachable. Reflection keeps the red observable and keeps every other test in the
	/// suite runnable while the seam is written.
	/// </para>
	/// </remarks>
	[Trait("Guard", "Seam")]
	[Collection(TestCollections.SharedStore)]
	public class ProviderSelectionTests
	{
		private const string SeamTypeName = "ProphetsWay.EFTools.Tests.TestStore";

		private const string ProviderTypeName = "ProphetsWay.EFTools.Tests.TestStoreProvider";

		/// <summary>
		/// Carried on every assertion in this class. A maintainer meeting seventeen failures beside two hundred and
		/// seventy passes needs to be told, at the failure, that the passes are the ones that are misleading.
		/// </summary>
		private const string WhatThisFailureMeans =
			"\n\nWHAT THIS MEANS: this assembly has no shared provider selection, so no whole-suite run can be " +
			"certified against one relational provider. The thirteen EF*Tests adapters run on SQL Server through " +
			"TestSeam -> Constants; six store-backed local classes each open their own SQLite in-memory " +
			"connection and never consult Constants; a seventh parameterizes two providers deliberately and is " +
			"exempt. A green run of this suite is therefore a green run of two different stores, and reporting it " +
			"as a SQL Server result would be false. " +
			"The fix is to ADD the seam described in this class's remarks - " + SeamTypeName + " and " +
			ProviderTypeName + " - and then to migrate the six store-backed classes and Constants onto it. " +
			"Relaxing an assertion here is how the two-store arrangement survives while being described as one.";

		#region the probe store

		/// <summary>
		/// One table, two columns, no relationships — enough to seed a row and count it, and nothing that could
		/// make a failure here mean anything other than "the store did not behave".
		/// </summary>
		public class Probe
		{
			public int Id { get; set; }

			public string Note { get; set; }
		}

		/// <summary>
		/// Exists only to carry a foreign key. SQL Server enforces one unconditionally; SQLite enforces one only
		/// when the connection asks it to, which is why the enforcement is specified rather than assumed.
		/// </summary>
		public class ProbeChild
		{
			public int Id { get; set; }

			public int ParentId { get; set; }

			public Probe Parent { get; set; }
		}

		public class ProbeContext : DbContext
		{
			public ProbeContext(DbContextOptions<ProbeContext> options) : base(options)
			{
			}

			public DbSet<Probe> Probes { get; set; }

			public DbSet<ProbeChild> ProbeChildren { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.Entity<ProbeChild>()
					.HasOne(c => c.Parent)
					.WithMany()
					.HasForeignKey(c => c.ParentId)
					.OnDelete(DeleteBehavior.Restrict);
			}
		}

		public class AlternateProbeContext : DbContext
		{
			public AlternateProbeContext(DbContextOptions<AlternateProbeContext> options) : base(options)
			{
			}

			public DbSet<AlternateProbe> AlternateProbes { get; set; }
		}

		public class AlternateProbe
		{
			public int Id { get; set; }

			public string Note { get; set; }
		}

		#endregion

		#region reaching the seam

		private static Type SeamType()
		{
			var type = typeof(ProviderSelectionTests).Assembly.GetType(SeamTypeName, false);

			type.ShouldNotBeNull(
				$"There is no type named '{SeamTypeName}' in this assembly. It is the seam every store-backed test " +
				"is meant to obtain its store from, and nothing in this suite can select a provider without it." +
				WhatThisFailureMeans);

			return type;
		}

		private static Type ProviderEnum()
		{
			var type = typeof(ProviderSelectionTests).Assembly.GetType(ProviderTypeName, false);

			type.ShouldNotBeNull(
				$"There is no type named '{ProviderTypeName}' in this assembly. It is the closed set of providers " +
				"this repository certifies against, and a selection has nothing to resolve to without it." +
				WhatThisFailureMeans);

			type.IsEnum.ShouldBeTrue(
				$"'{ProviderTypeName}' exists but is not an enum. A closed set is the point: a string would let a " +
				"typo name a provider that was never certified." + WhatThisFailureMeans);

			return type;
		}

		private static MethodInfo Method(Type declaring, string name, params Type[] parameters)
		{
			var method = declaring.GetMethod(
				name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
				null,
				parameters,
				null);

			method.ShouldNotBeNull(
				$"{declaring.FullName} declares no method '{name}({string.Join(", ", parameters.Select(p => p.Name))})'." +
				WhatThisFailureMeans);

			return method;
		}

		private static object Invoke(MethodInfo method, object target, params object[] arguments)
		{
			try
			{
				return method.Invoke(target, arguments);
			}
			catch (TargetInvocationException ex)
			{
				//rethrow what the seam actually threw, so an exception assertion reads the seam's type and message
				//rather than reflection's wrapper
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

				throw;
			}
		}

		private static object Resolve(string raw)
		{
			return Invoke(Method(SeamType(), "ResolveProvider", typeof(string)), null, raw);
		}

		private static object Selection(string name)
		{
			var providers = ProviderEnum();

			Enum.IsDefined(providers, name).ShouldBeTrue(
				$"'{ProviderTypeName}' declares no member named '{name}'. The certified tier is SQL Server and " +
				"SQLite, and both must be nameable." + WhatThisFailureMeans);

			return Enum.Parse(providers, name);
		}

		private static object AmbientSelection()
		{
			var property = SeamType().GetProperty(
				"Provider",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			property.ShouldNotBeNull(
				$"{SeamTypeName} exposes no static 'Provider' property. It is the one value the whole run agrees " +
				"on, and without it each class is free to pick its own store again." + WhatThisFailureMeans);

			return property.GetValue(null);
		}

		/// <summary>
		/// The name of the environment variable the seam reads. Returned rather than tested for null at each call
		/// site, so that a missing constant fails here instead of quietly removing an assertion from a caller.
		/// </summary>
		private static string ProviderVariableName()
		{
			var variable = SeamType().GetField(
				"ProviderVariable",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			variable.ShouldNotBeNull(
				$"{SeamTypeName} declares no 'ProviderVariable' constant naming the environment variable that " +
				"selects the provider." + WhatThisFailureMeans);

			var name = variable.GetValue(null) as string;

			string.IsNullOrWhiteSpace(name).ShouldBeFalse(
				$"{SeamTypeName}.ProviderVariable is empty or is not a string, so nothing can select a provider " +
				"from outside the process." + WhatThisFailureMeans);

			return name;
		}

		private static string ProviderNameOf(object selection)
		{
			return (string)Invoke(Method(SeamType(), "ProviderNameOf", ProviderEnum()), null, selection);
		}

		/// <summary>
		/// The identity of one live store — the database name on SQL Server, whatever names the connection on
		/// SQLite. It exists so that <see cref="StoreExists"/> can be asked about a store <i>after</i> the object
		/// that owned it has been disposed, which is the only way to tell a real drop from an empty
		/// <c>Dispose</c>.
		/// </summary>
		private static string IdentityOf(object store)
		{
			var property = store.GetType().GetProperty(
				"Identity",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			property.ShouldNotBeNull(
				$"{store.GetType().FullName} exposes no 'Identity' property. Without one, nothing outside the store " +
				"can name it, so nothing can ask whether it survived disposal - and an empty Dispose passes every " +
				"other assertion in this class while leaving a database behind on every run." + WhatThisFailureMeans);

			var identity = property.GetValue(store) as string;

			string.IsNullOrWhiteSpace(identity).ShouldBeFalse(
				$"{store.GetType().FullName}.Identity is empty or is not a string, so the store cannot be named in " +
				"a cleanup check or in a failure message." + WhatThisFailureMeans);

			return identity;
		}

		private static string StoreProperty(object store, string name)
		{
			var property = store.GetType().GetProperty(
				name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			property.ShouldNotBeNull($"{store.GetType().FullName} exposes no '{name}' property for reusable-slot diagnostics.");

			var value = property.GetValue(store) as string;
			string.IsNullOrWhiteSpace(value).ShouldBeFalse($"{store.GetType().FullName}.{name} must be a non-empty name.");

			return value;
		}

		/// <summary>
		/// Whether the seam can still find the named store. This must be answered by asking the server — or the
		/// live connection registry — not by consulting a list the seam edits in <c>Dispose</c>, or the answer is
		/// the same closed loop the literal provider names exist to break. That is not left to good faith:
		/// <see cref="ShouldLetEntityFrameworkFindALiveStoreByItsIdentity"/> and
		/// <see cref="ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed"/> require this method to agree
		/// with <see cref="EntityFrameworkFinds"/> about the same identity, and Entity Framework consults the store.
		/// </summary>
		private static bool StoreExists(string identity)
		{
			return (bool)Invoke(Method(SeamType(), "StoreExists", typeof(string)), null, identity);
		}

		private static IDisposable OpenStore(string label)
		{
			var store = Invoke(Method(SeamType(), "OpenStore", typeof(string)), null, label);

			store.ShouldNotBeNull(
				$"{SeamTypeName}.OpenStore(\"{label}\") returned null. It owns the lifetime of one isolated store, " +
				"so a caller has nothing to dispose and nothing to configure a context from." + WhatThisFailureMeans);

			store.ShouldBeAssignableTo<IDisposable>(
				$"{SeamTypeName}.OpenStore returned a {store.GetType().FullName}, which is not IDisposable. On SQL " +
				"Server a store is a real database on a real server; if disposing it is not how it goes away, every " +
				"run leaves databases behind." + WhatThisFailureMeans);

			return (IDisposable)store;
		}

		private static ProbeContext ContextFor(object store)
		{
			var builder = new DbContextOptionsBuilder<ProbeContext>();

			//the non-generic parameter is the whole reason one Configure can serve seven differently typed contexts
			Invoke(Method(store.GetType(), "Configure", typeof(DbContextOptionsBuilder)), store, builder);

			return new ProbeContext(builder.Options);
		}

		/// <summary>
		/// A context pointed at a store the caller does not hold — named, not owned. It exists so that a store can
		/// be interrogated by identity alone, after the object that opened it is gone, without the seam being the
		/// one who answers.
		/// </summary>
		/// <remarks>
		/// This must not bring a store into being. A <c>ConfigureExisting</c> that provisions on demand makes
		/// <see cref="ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed"/> unpassable, and rightly so:
		/// naming a store is not the same act as opening one.
		/// </remarks>
		private static ProbeContext ContextForExistingStore(string identity)
		{
			var builder = new DbContextOptionsBuilder<ProbeContext>();

			Invoke(
				Method(SeamType(), "ConfigureExisting", typeof(string), typeof(DbContextOptionsBuilder)),
				null,
				identity,
				builder);

			return new ProbeContext(builder.Options);
		}

		private static AlternateProbeContext AlternateContextFor(object store)
		{
			var builder = new DbContextOptionsBuilder<AlternateProbeContext>();

			Invoke(Method(store.GetType(), "Configure", typeof(DbContextOptionsBuilder)), store, builder);

			return new AlternateProbeContext(builder.Options);
		}

		/// <summary>
		/// Entity Framework's own verdict on a named store: it is there, and it has a schema in it. This is the one
		/// answer in this class the seam does not author, which is why the two cleanup cases are written against it
		/// rather than against <see cref="StoreExists"/> alone.
		/// </summary>
		/// <remarks>
		/// Both halves are load-bearing, and the conjunction is what makes one probe serve both providers. A dropped
		/// SQL Server database answers <c>Exists()</c> false outright. A released SQLite in-memory store answers
		/// <c>Exists()</c> <i>true</i> — connecting to a memory database brings an empty one into being — and answers
		/// <c>HasTables()</c> false, which is the difference between a store and its ghost. The call order matters:
		/// <c>HasTables()</c> against a database that is not there throws rather than answering, so the conjunction
		/// short-circuits deliberately.
		/// </remarks>
		private static bool EntityFrameworkFinds(string identity)
		{
			using (var context = ContextForExistingStore(identity))
			{
				var creator = context.Database.GetService<IRelationalDatabaseCreator>();

				return creator.Exists() && creator.HasTables();
			}
		}

		/// <summary>
		/// <c>BaseEFDataAccess&lt;TContext&gt;.Context</c> is <c>protected</c>, so reflection is the only way to
		/// read the provider the adapted suite is actually running on. Same technique, and same reason, as
		/// <c>TestSeamTests.ContextOf</c>.
		/// </summary>
		private static DbContext ContextOf(object dataAccess)
		{
			var layer = dataAccess as BaseEFDataAccess<ExampleContext>;

			layer.ShouldNotBeNull(
				$"TestDataAccessFactory handed out a {dataAccess.GetType().FullName}, which is not an Entity " +
				"Framework layer, so it names no provider. See TestSeamTests, which owns that failure." +
				WhatThisFailureMeans);

			var property = typeof(BaseEFDataAccess<ExampleContext>)
				.GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic);

			property.ShouldNotBeNull(
				"BaseEFDataAccess<TContext>.Context could not be found by reflection. Re-point this helper at " +
				"whatever replaced it rather than deleting the assertion." + WhatThisFailureMeans);

			return (DbContext)property.GetValue(layer);
		}

		#endregion

		#region finding provider selections the seam does not own

		/// <summary>
		/// The Entity Framework calls that choose a provider. A new provider added to
		/// <c>TestStoreProvider</c> adds its <c>Use*</c> method here, or the guard stops seeing the site.
		/// </summary>
		private static readonly string[] ProviderSelectors =
		{
			"UseSqlServer",
			"UseSqlite",
			"UseInMemoryDatabase"
		};

		/// <summary>
		/// The only two types allowed to name a provider. <c>TestStore</c> because that is its job, and the
		/// provider-comparison spike because its measurement <i>is</i> the comparison. Named by string so this
		/// class still compiles while the seam does not exist.
		/// </summary>
		private static readonly string[] SelectionExemptRoots =
		{
			SeamTypeName,
			"ProphetsWay.EFTools.Tests.AlternateKeyGuardSpikeTests"
		};

		private static readonly string[] LocalPhysicalLifecycleMethodNames =
		{
			nameof(ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed),
			nameof(ShouldRemoveTheStoreItselfWhenItIsDisposed),
			nameof(ShouldLeaveNoDisposableStoreBehindOnTheServer),
			nameof(ShouldRegisterAStoreBeforeProvisioningItAndForgetItIfProvisioningFails),
			nameof(ShouldDecideEveryDropWithTheOnePredicateThatGuardsIt),
			nameof(ShouldNameEveryStoreInsideTheDroppableNamespace)
		};

		private static bool ReusableSqlServerIsSelected()
		{
			if (!SqlServerIsSelected())
				return false;

			var lifecycle = Invoke(
				Method(SeamType(), "ResolveSqlServerStoreLifecycle", typeof(string)),
				null,
				Environment.GetEnvironmentVariable("EFTOOLS_SQLSERVER_STORE_LIFECYCLE"));

			return string.Equals(lifecycle.ToString(), "Reusable", StringComparison.Ordinal);
		}

		/// <summary>
		/// A provider selection made one call away. <c>new ExampleDataAccess(string)</c> reads
		/// <c>UseSqlServer(connectionString)</c> in its own body, in the proving-ground assembly — so a scan that
		/// looks only for provider calls written in <i>this</i> assembly sees nothing at all while every one of the
		/// thirteen adapters is pinned to SQL Server through <c>Constants</c>. The constructor itself is not the
		/// offender: naming a provider in the consumer's own file is the arrangement this library exists to make
		/// possible. Choosing it from inside a suite that claims to select its provider elsewhere is.
		/// </summary>
		private static bool IsSelectionByProxy(MethodBase callee)
		{
			if (!(callee is ConstructorInfo) || callee.DeclaringType != typeof(ExampleDataAccess))
				return false;

			var parameters = callee.GetParameters();

			return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
		}

		private static readonly int[] SingleByteOperandSizes = BuildSingleByteOperandSizes();

		private static readonly int[] TwoByteOperandSizes = BuildTwoByteOperandSizes();

		private static int[] BuildSingleByteOperandSizes()
		{
			var sizes = new int[256];

			foreach (var code in new byte[]
			{
				0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x1F, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
				0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0xDE
			})
				sizes[code] = 1;

			foreach (var code in new byte[]
			{
				0x20, 0x22, 0x27, 0x28, 0x29, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
				0x40, 0x41, 0x42, 0x43, 0x44, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x79,
				0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x80, 0x81, 0x8C, 0x8D, 0x8F, 0xA3, 0xA4, 0xA5, 0xC2,
				0xC6, 0xD0, 0xDD
			})
				sizes[code] = 4;

			sizes[0x21] = 8;
			sizes[0x23] = 8;
			sizes[0x45] = -1;

			return sizes;
		}

		private static int[] BuildTwoByteOperandSizes()
		{
			var sizes = new int[256];

			foreach (var code in new byte[] { 0x06, 0x07, 0x15, 0x16, 0x1C })
				sizes[code] = 4;

			foreach (var code in new byte[] { 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E })
				sizes[code] = 2;

			foreach (var code in new byte[] { 0x12, 0x19 })
				sizes[code] = 1;

			return sizes;
		}

		/// <summary>
		/// Walks the instruction stream properly rather than searching it for byte patterns: an operand can
		/// contain any byte, so a scan that does not know each opcode's length reports calls that are not there
		/// and misses ones that are. <c>ldftn</c>/<c>ldvirtftn</c> are treated as calls because that is how a
		/// method group or a lambda names its target.
		/// </summary>
		/// <remarks>
		/// Every way this walk can lose its place calls <paramref name="onAbandoned"/> before stopping. A walker
		/// that mis-sizes one instruction reads the following operand bytes as opcodes and is desynchronised from
		/// there on — it does not throw, it simply stops finding things, which is the failure mode a guard like this
		/// one cannot afford to have silently.
		/// </remarks>
		private static IEnumerable<int> CallTokens(byte[] il, Action onAbandoned, Action<int> onStringToken = null,
			Action onUnsupportedFlow = null)
		{
			var offset = 0;
			var returns = 0;

			while (offset < il.Length)
			{
				var code = il[offset];
				int operandAt;
				int size;
				bool isCall;

				if (code == 0xFE)
				{
					if (offset + 1 >= il.Length)
					{
						onAbandoned();

						yield break;
					}

					var second = il[offset + 1];

					operandAt = offset + 2;
					size = TwoByteOperandSizes[second];
					isCall = second == 0x06 || second == 0x07;
				}
				else
				{
					operandAt = offset + 1;
					size = SingleByteOperandSizes[code];
					isCall = code == 0x28 || code == 0x6F || code == 0x73;
				}

				if (size < 0)
				{
					//switch: a four-byte branch count followed by that many four-byte targets
					if (operandAt + 4 > il.Length)
					{
						onAbandoned();

						yield break;
					}

					var branches = BitConverter.ToInt32(il, operandAt);

					if (branches < 0 || branches > (il.Length - operandAt - 4) / 4)
					{
						onAbandoned();

						yield break;
					}

					onUnsupportedFlow?.Invoke();
					offset = operandAt + 4 + (branches * 4);

					continue;
				}

				if (operandAt + size > il.Length)
				{
					onAbandoned();

					yield break;
				}

				if ((code >= 0x2C && code <= 0x37) || (code >= 0x39 && code <= 0x44) ||
					code == 0x27 || code == 0x29 || code == 0x7A || code == 0xDD || code == 0xDE ||
					(code == 0x2B && il[operandAt] != 0) ||
					(code == 0x38 && BitConverter.ToInt32(il, operandAt) != 0) ||
					(code == 0x2A && ++returns > 1))
					onUnsupportedFlow?.Invoke();

				if (isCall)
					yield return BitConverter.ToInt32(il, operandAt);

				if (code == 0x72)
					onStringToken?.Invoke(BitConverter.ToInt32(il, operandAt));

				offset = operandAt + size;
			}
		}

		/// <summary>
		/// Every place this scan can fail to look is reported into the same list as a real offender, under this
		/// prefix. A guard whose degradation mode is silence goes green by not looking, which is indistinguishable
		/// in a run log from going green because there was nothing to find.
		/// </summary>
		private const string BlindSpot = "GUARD BLIND SPOT: ";

		private static string Describe(MethodBase method)
		{
			return $"{method.DeclaringType?.FullName}.{method.Name}";
		}

		private static IEnumerable<Type> TypesOf(Assembly assembly, ISet<string> found)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				found.Add(
					$"{BlindSpot}{ex.Types.Count(t => t == null)} type(s) in {assembly.GetName().Name} could not be " +
					"loaded, so they were not scanned at all and may hold a provider selection");

				return ex.Types.Where(t => t != null);
			}
		}

		private static IEnumerable<MethodBase> DeclaredMethods(Type type, ISet<string> found)
		{
			const BindingFlags everything = BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			MethodBase[] methods;

			try
			{
				methods = type.GetMethods(everything)
					.Cast<MethodBase>()
					.Concat(type.GetConstructors(everything))
					.ToArray();
			}
			catch (Exception ex)
			{
				found.Add(
					$"{BlindSpot}the methods of {type.FullName} could not be enumerated ({ex.GetType().Name}), so " +
					"none of them was scanned");

				methods = new MethodBase[0];
			}

			return methods;
		}

		private static IEnumerable<MethodBase> CalleesOf(MethodBase method, ISet<string> found, Action<string> onString = null,
			Action onUnsupportedFlow = null)
		{
			MethodBody body = null;

			try
			{
				body = method.GetMethodBody();
			}
			catch
			{
				//an abstract, extern or otherwise bodiless method calls nothing
			}

			var il = body?.GetILAsByteArray();

			if (il == null || il.Length == 0)
				yield break;

			if (body.ExceptionHandlingClauses.Count > 0)
				onUnsupportedFlow?.Invoke();

			var declaring = method.DeclaringType;

			var typeArguments = declaring != null && declaring.IsGenericType
				? declaring.GetGenericArguments()
				: null;

			var methodArguments = method.IsGenericMethodDefinition
				? method.GetGenericArguments()
				: null;

			var abandoned = false;

			foreach (var token in CallTokens(il, () => abandoned = true, stringToken =>
			{
				if (onString == null)
					return;

				try
				{
					onString(method.Module.ResolveString(stringToken));
				}
				catch (Exception ex)
				{
					found.Add($"{BlindSpot}a string token in {Describe(method)} could not be resolved ({ex.GetType().Name})");
				}
			}, onUnsupportedFlow))
			{
				MethodBase callee = null;

				try
				{
					callee = method.Module.ResolveMethod(token, typeArguments, methodArguments);
				}
				catch (Exception ex)
				{
					found.Add(
						$"{BlindSpot}a call token in {Describe(method)} could not be resolved " +
						$"({ex.GetType().Name}), so whatever it calls went unchecked");
				}

				if (callee != null)
					yield return callee;
			}

			if (abandoned)
				found.Add(
					$"{BlindSpot}the instruction walk of {Describe(method)} stopped on an instruction it could not " +
					"size, so the rest of that method went unchecked");
		}

		/// <summary>
		/// The outermost type, so a call made from a lambda's display class or an iterator state machine is
		/// attributed to the test class a maintainer can actually open.
		/// </summary>
		private static Type RootOf(Type type)
		{
			var current = type;

			while (current?.DeclaringType != null)
				current = current.DeclaringType;

			return current;
		}

		private static IReadOnlyList<string> ProviderSelectionSitesOutsideTheSeam()
		{
			return ProviderSelectionSites(SelectionExemptRoots);
		}

		/// <summary>
		/// The exemptions are a parameter rather than a constant so the scanner can be run against a type it is
		/// normally told to ignore — see <see cref="ShouldStillFindAProviderSelectionWhenNothingIsExempt"/>.
		/// </summary>
		private static IReadOnlyList<string> ProviderSelectionSites(IReadOnlyCollection<string> exemptRoots)
		{
			var found = new SortedSet<string>(StringComparer.Ordinal);

			//direct provider calls, and the proxy, in the assembly being certified
			ScanForSelections(typeof(ProviderSelectionTests).Assembly, exemptRoots, true, found);

			//the proving ground is scanned for the proxy only: ExampleDataAccess(string) naming SQL Server in the
			//consumer's own file is the arrangement, not the defect - see IsSelectionByProxy
			ScanForSelections(typeof(ExampleDataAccess).Assembly, exemptRoots, false, found);

			return found.ToList();
		}

		private static void ScanForSelections(
			Assembly assembly,
			IReadOnlyCollection<string> exemptRoots,
			bool includeDirectSelectors,
			ISet<string> found)
		{
			foreach (var type in TypesOf(assembly, found))
			{
				var root = RootOf(type);

				if (root == null || exemptRoots.Contains(root.FullName, StringComparer.Ordinal))
					continue;

				foreach (var method in DeclaredMethods(type, found))
					foreach (var callee in CalleesOf(method, found))
					{
						var direct = includeDirectSelectors &&
							ProviderSelectors.Contains(callee.Name, StringComparer.Ordinal);

						if (!direct && !IsSelectionByProxy(callee))
							continue;

						var where = type == root
							? $"{root.FullName}.{method.Name}"
							: $"{root.FullName} -> {type.FullName}.{method.Name} (compiler-generated)";

						found.Add(direct
							? $"{where} calls {callee.DeclaringType?.Name}.{callee.Name}"
							: $"{where} constructs {callee.DeclaringType?.Name} from a connection string, which " +
								"selects a provider by proxy");
					}
			}
		}

		/// <summary>
		/// Every callee named anywhere inside a type and its nested types, which is how a structural claim about
		/// the seam is made without naming the seam in code.
		/// </summary>
		private static IReadOnlyCollection<string> CalleeNamesWithin(Type root)
		{
			var names = new SortedSet<string>(StringComparer.Ordinal);
			var ignored = new SortedSet<string>(StringComparer.Ordinal);

			foreach (var type in TypesOf(root.Assembly, ignored))
			{
				if (RootOf(type) != root)
					continue;

				foreach (var method in DeclaredMethods(type, ignored))
					foreach (var callee in CalleesOf(method, ignored))
						names.Add(callee.Name);
			}

			return names;
		}

		#endregion

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldDeclareOneSharedStoreSeam()
		{
			//setup
			//act
			var seam = SeamType();

			//assert
			seam.IsAbstract.ShouldBeTrue(
				$"{SeamTypeName} is not a static class. Every store-backed test in this assembly has to reach the " +
				"same selection, and an instance seam is one each class can hold its own copy of." +
				WhatThisFailureMeans);

			seam.IsSealed.ShouldBeTrue(
				$"{SeamTypeName} is not a static class." + WhatThisFailureMeans);

			ProviderVariableName();
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldDeclareTheTwoCertifiedProviders()
		{
			//setup
			//act
			var declared = string.Join(", ", Enum.GetNames(ProviderEnum()).OrderBy(n => n, StringComparer.Ordinal));

			//assert
			//ordinal order puts SqlServer first: 'S' (0x53) precedes 'i' (0x69) at the fourth character
			declared.ShouldBe("SqlServer, Sqlite",
				$"'{ProviderTypeName}' declares [{declared}]. The certified tier is exactly SQL " +
				"Server and SQLite. InMemory in particular must not be a member: it honours no transaction, so the " +
				"transaction contract cannot be certified against it, and a run that selected it would report a " +
				"green transaction suite that had tested nothing." + WhatThisFailureMeans);
		}

		[Theory]
		[Trait("Area", "ProviderSelection")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void ShouldDefaultToSqlServerWhenNothingSelectsAProvider(string unset)
		{
			//setup
			var expected = Selection("SqlServer");

			//act
			var resolved = Resolve(unset);

			//assert
			resolved.ShouldBe(expected,
				$"An unset selection ({(unset == null ? "null" : $"'{unset}'")}) resolved to {resolved}. It must " +
				"resolve to SqlServer: the suite already cannot run without a local SQL Server, so the release " +
				"gating run is the one that must happen by default rather than the one someone has to remember to " +
				"ask for." + WhatThisFailureMeans);
		}

		[Theory]
		[Trait("Area", "ProviderSelection")]
		[InlineData("sqlserver", "SqlServer")]
		[InlineData("SqlServer", "SqlServer")]
		[InlineData("sqlite", "Sqlite")]
		[InlineData("SQLITE", "Sqlite")]
		public void ShouldSelectTheNamedProviderIgnoringCase(string raw, string expectedName)
		{
			//setup
			var expected = Selection(expectedName);

			//act
			var resolved = Resolve(raw);

			//assert
			resolved.ShouldBe(expected,
				$"'{raw}' resolved to {resolved} rather than {expectedName}. Selection arrives from a shell, a " +
				"launch profile or a build agent, none of which agree on casing." + WhatThisFailureMeans);
		}

		/// <summary>
		/// Trimming is not a new decision — it is the one already made by
		/// <see cref="ShouldDefaultToSqlServerWhenNothingSelectsAProvider"/>, which requires an all-whitespace
		/// value to reach the default rather than be refused. A resolver that treats <c>"   "</c> as absent and
		/// <c>" sqlite "</c> as unrecognised is inconsistent with itself, and the second form is what a YAML
		/// block scalar or a copied shell line actually produces.
		/// </summary>
		[Theory]
		[Trait("Area", "ProviderSelection")]
		[InlineData(" sqlite ", "Sqlite")]
		[InlineData("\tSqlServer\r\n", "SqlServer")]
		[InlineData("  SQLITE  ", "Sqlite")]
		public void ShouldIgnoreWhitespaceAroundASelection(string raw, string expectedName)
		{
			//setup
			var expected = Selection(expectedName);

			//act
			var resolved = Resolve(raw);

			//assert
			resolved.ShouldBe(expected,
				$"'{raw}' resolved to {resolved} rather than {expectedName}. An all-whitespace value already has " +
				"to reach the default rather than be refused, so whitespace is already being treated as absent; " +
				"treating it as significant once it surrounds a real name is the same resolver disagreeing with " +
				"itself, and it turns a stray space in a build definition into a refusal nobody can see." +
				WhatThisFailureMeans);
		}

		/// <summary>
		/// The literals are the whole point. Every store assertion in this class compares the provider a real
		/// context reports against <c>ProviderNameOf(Provider)</c>; if that method is never held to anything
		/// outside the seam, the seam can answer <c>SqlServer</c> while opening SQLite and the comparison agrees
		/// with itself. These two strings are Entity Framework Core's own, so they are the one fact in the loop
		/// the seam does not get to choose.
		/// </summary>
		[Theory]
		[Trait("Area", "ProviderSelection")]
		[InlineData("SqlServer", "Microsoft.EntityFrameworkCore.SqlServer")]
		[InlineData("Sqlite", "Microsoft.EntityFrameworkCore.Sqlite")]
		public void ShouldNameTheEntityFrameworkProviderExactly(string selectionName, string providerName)
		{
			//setup
			var selection = Selection(selectionName);

			//act
			var actual = ProviderNameOf(selection);

			//assert
			actual.ShouldBe(providerName,
				$"{SeamTypeName}.ProviderNameOf({selectionName}) answered '{actual}' rather than " +
				$"'{providerName}', which is what Entity Framework Core's own provider reports through " +
				"DbContext.Database.ProviderName. Every store assertion here compares a real context's provider " +
				"against this method's answer, so an answer that is merely self-consistent lets the whole suite " +
				"run on one provider while being certified as the other." + WhatThisFailureMeans);
		}

		/// <summary>
		/// Without this, <c>Provider</c> is free to be a constant: the resolver is pinned on ten literal inputs
		/// and then wired to nothing, and <c>Provider =&gt; TestStoreProvider.Sqlite</c> satisfies every other
		/// case in this class.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldBindTheSelectionToTheEnvironmentVariableItNames()
		{
			//setup
			var variable = ProviderVariableName();
			var raw = Environment.GetEnvironmentVariable(variable);

			//act
			var expected = Resolve(raw);

			//assert
			AmbientSelection().ShouldBe(expected,
				$"{SeamTypeName}.Provider is {AmbientSelection()}, but {variable} currently reads " +
				$"{(raw == null ? "(unset)" : $"'{raw}'")}, which resolves to {expected}. The ambient selection " +
				"has to BE the resolver applied to that variable. If it is anything else - a constant, a second " +
				"copy of the defaulting rule, a different variable - then the ten resolver cases above pin a " +
				"method nothing calls, and the provider the suite actually runs on is chosen somewhere no test " +
				"looks." + WhatThisFailureMeans);
		}

		/// <summary>
		/// The structural half of the same obligation, and the half a default run can actually enforce.
		/// <see cref="ShouldBindTheSelectionToTheEnvironmentVariableItNames"/> compares <c>Provider</c> against the
		/// resolver applied to the variable — but in the ordinary run that variable is unset, so both sides read
		/// SqlServer and <c>Provider =&gt; TestStoreProvider.SqlServer</c> satisfies it with the resolver wired to
		/// nothing and the entire SQLite mode fictional. This asks the compiled seam instead: somewhere inside
		/// <c>TestStore</c>, the resolver must be applied to something read out of the environment.
		/// </summary>
		/// <remarks>
		/// The alternative — setting the variable and re-reading <c>Provider</c> — is a process-wide mutation, and
		/// xUnit runs this class in parallel with others that read the selection. That is a race introduced into a
		/// suite whose whole purpose here is to make a run trustworthy, so it is refused. A real round trip through
		/// the environment belongs to the SQLite leg, where the variable is set for the run rather than during it.
		/// </remarks>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldApplyTheResolverToTheEnvironmentItReads()
		{
			//setup
			var seam = SeamType();

			//act
			var callees = CalleeNamesWithin(seam);

			//assert
			callees.Contains("ResolveProvider").ShouldBeTrue(
				$"Nothing inside {SeamTypeName} calls its own ResolveProvider. The resolver is pinned by ten " +
				"literal cases above, and if nothing calls it then those ten cases specify a method the run does " +
				"not use, while the provider the suite actually opens is chosen by whatever Provider happens to " +
				"return." + WhatThisFailureMeans);

			callees.Contains("GetEnvironmentVariable").ShouldBeTrue(
				$"Nothing inside {SeamTypeName} reads an environment variable, so the selection cannot come from " +
				"outside the process and ProviderVariable names a variable nothing consults. A default run cannot " +
				"tell that apart from a working seam - both answer SqlServer - which is why this is asserted " +
				"against the compiled seam rather than by setting the variable and looking." + WhatThisFailureMeans);
		}

		[Theory]
		[Trait("Area", "ProviderSelection")]
		[InlineData("inmemory")]
		[InlineData("mssql")]
		[InlineData("sqlserver2022")]
		public void ShouldRefuseAnUnrecognisedProviderNameRatherThanFallingBack(string raw)
		{
			//setup
			var variable = ProviderVariableName();

			//act
			var thrown = Should.Throw<ArgumentException>(() => Resolve(raw),
				$"'{raw}' was accepted rather than refused. A resolver that falls back to a default turns one " +
				"mistyped environment variable into a green run that is then reported as SQL Server certification - " +
				"the run picked a different store and nothing said so. Refusing loudly is the only behaviour that " +
				"cannot produce that." + WhatThisFailureMeans);

			//assert
			thrown.Message.Contains("SqlServer").ShouldBeTrue(
				"The refusal does not name the accepted values, so the person who mistyped it has to read the " +
				$"source to find out what to type instead. The message was: {thrown.Message}" + WhatThisFailureMeans);

			thrown.Message.Contains("Sqlite").ShouldBeTrue(
				"The refusal does not name the accepted values. The message was: " + thrown.Message +
				WhatThisFailureMeans);

			thrown.Message.Contains(variable).ShouldBeTrue(
				"The refusal does not name the environment variable it read, so the message cannot be searched " +
				$"for in a build definition or a launch profile. The message was: {thrown.Message}" +
				WhatThisFailureMeans);
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldOpenEveryStoreOnTheSelectedRelationalProvider()
		{
			//setup
			var expected = ProviderNameOf(AmbientSelection());

			//act
			using (var store = OpenStore(nameof(ShouldOpenEveryStoreOnTheSelectedRelationalProvider)))
			using (var context = ContextFor(store))
			{
				context.Database.EnsureCreated();

				//assert
				context.Database.ProviderName.ShouldBe(expected,
					$"A store opened through the seam is on '{context.Database.ProviderName}' while the selection " +
					$"names '{expected}'. If the seam does not decide the provider then the seven store-backed " +
					"classes have gained a dependency and kept their own store, which is worse than the arrangement " +
					"it replaced." + WhatThisFailureMeans);

				context.Database.IsRelational().ShouldBeTrue(
					$"A store opened through the seam is not relational. Two obligations the local classes carry - " +
					"the non-upsert guard and the key-property exclusion - turn on the store enforcing a primary " +
					"key, and a non-relational provider enforces neither, so both would pass while asserting " +
					"nothing." + WhatThisFailureMeans);
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldEnforceForeignKeysOnTheSelectedProvider()
		{
			//setup
			using (var store = OpenStore(nameof(ShouldEnforceForeignKeysOnTheSelectedProvider)))
			using (var context = ContextFor(store))
			{
				context.Database.EnsureCreated();

				context.ProbeChildren.Add(new ProbeChild { ParentId = 987654321 });

				//act
				//assert
				var thrown = Should.Throw<DbUpdateException>(() => context.SaveChanges(),
					"A row referencing a parent that does not exist was accepted. SQL Server enforces a foreign " +
					"key unconditionally; SQLite enforces one only when the connection asks it to, so a seam that " +
					"omits that on the SQLite leg silently turns every trap test in this suite green-and-empty - " +
					"FailedInsertWriteBackTests exists to watch a constrained insert fail, and against an " +
					"unconstrained store it watches nothing." + WhatThisFailureMeans);

				thrown.ShouldNotBeOfType<DbUpdateConcurrencyException>(
					"The save failed as a concurrency conflict rather than as a rejected row, so nothing here says " +
					"the store enforces the key." + WhatThisFailureMeans);

				//the store rejected it, rather than Entity Framework declining to send it: every relational
				//provider surfaces that as a DbException, and none of them agrees with another on the message
				thrown.InnerException.ShouldBeAssignableTo<DbException>(
					$"The save threw {thrown.GetType().Name} with an inner " +
					$"{thrown.InnerException?.GetType().Name ?? "(none)"}, so the row was refused by something " +
					"other than the database. Only a rejection from the store is evidence the constraint is in the " +
					"schema; a DbUpdateException raised for any other reason reads identically in a run log and " +
					"would let an unconstrained store pass this case. The inner type is asserted rather than the " +
					"message deliberately - the message and the error number differ per provider, and pinning " +
					"either would make this case provider-specific." + WhatThisFailureMeans);
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldGiveEveryStoreItsOwnIdentity()
		{
			//setup
			using (var first = OpenStore(nameof(ShouldGiveEveryStoreItsOwnIdentity)))
			using (var second = OpenStore(nameof(ShouldGiveEveryStoreItsOwnIdentity)))
			{
				//act
				var one = IdentityOf(first);
				var other = IdentityOf(second);

				//assert
				other.ShouldNotBe(one,
					$"Two stores opened under the same label are both named '{one}'. The label is a hint for a " +
					"human reading a server, not the identity - two classes that happen to pass the same label, " +
					"or one class opening a store per test, would otherwise share a database and race each " +
					"other's whole-set counts." + WhatThisFailureMeans);
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldIsolateEveryStoreFromEveryOther()
		{
			//setup
			using (var first = OpenStore(nameof(ShouldIsolateEveryStoreFromEveryOther)))
			using (var second = OpenStore(nameof(ShouldIsolateEveryStoreFromEveryOther)))
			{
				using (var writer = ContextFor(first))
				{
					writer.Database.EnsureCreated();
					writer.Probes.Add(new Probe { Note = "written to the first store" });
					writer.SaveChanges();
				}

				//act
				using (var reader = ContextFor(second))
				{
					reader.Database.EnsureCreated();

					var seen = reader.Probes.Count();

					//assert
					seen.ShouldBe(0,
						$"A second store opened under the same label saw {seen} row(s) written to the first. On " +
						"SQLite 'Filename=:memory:' isolation was free; on SQL Server it is a decision, and two " +
						"classes sharing one database will interleave writes and race each other's whole-set " +
						"counts. Every OpenStore call has to yield a store of its own." + WhatThisFailureMeans);
				}
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldReportALiveStoreAsExisting()
		{
			//setup
			using (var store = OpenStore(nameof(ShouldReportALiveStoreAsExisting)))
			using (var context = ContextFor(store))
			{
				context.Database.EnsureCreated();
				context.Probes.Add(new Probe { Note = "written to a live store" });
				context.SaveChanges();

				//act
				var identity = IdentityOf(store);

				//assert
				StoreExists(identity).ShouldBeTrue(
					$"The seam says store '{identity}' does not exist, while a context opened on it has just " +
					"created a schema and committed a row. StoreExists has to answer from the store itself - the " +
					"server's catalogue, or the live connection - because the only other test that can tell a real " +
					"drop from an empty Dispose is the one below, and it reads this same method. An implementation " +
					"that answers from a list it maintains itself makes both of them vacuous." + WhatThisFailureMeans);
			}
		}

		/// <summary>
		/// The first half of the pair that makes seam bookkeeping insufficient. <see cref="StoreExists"/> alone
		/// is satisfied by a <c>HashSet</c> the seam adds to in <c>OpenStore</c> and removes from in
		/// <c>Dispose</c> — every cleanup case here passes, and not one <c>DROP DATABASE</c> is issued. So the
		/// identity is handed back through <c>ConfigureExisting</c>, a second context is opened on it, and Entity
		/// Framework is asked whether that store is there and has a schema. The seam does not get to write that
		/// answer down in advance, and it must agree with the one it does write down.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldLetEntityFrameworkFindALiveStoreByItsIdentity()
		{
			//setup
			using (var store = OpenStore(nameof(ShouldLetEntityFrameworkFindALiveStoreByItsIdentity)))
			{
				var identity = IdentityOf(store);

				using (var context = ContextFor(store))
				{
					context.Database.EnsureCreated();
					context.Probes.Add(new Probe { Note = "written to a live store" });
					context.SaveChanges();
				}

				//act
				var entityFrameworkFindsIt = EntityFrameworkFinds(identity);

				//assert
				entityFrameworkFindsIt.ShouldBeTrue(
					$"A context configured for existing store '{identity}' cannot find it, while a context the seam " +
					"configured has just created a schema in it and committed a row. Either ConfigureExisting points " +
					"somewhere other than where Configure did - in which case Identity does not identify anything, " +
					"and the cleanup case below can never be more than the seam agreeing with itself - or the store " +
					"was gone before its owner was disposed." + WhatThisFailureMeans);

				StoreExists(identity).ShouldBe(entityFrameworkFindsIt,
					$"The seam says store '{identity}' " +
					$"{(StoreExists(identity) ? "exists" : "does not exist")} while Entity Framework, asked about " +
					"the same identity through ConfigureExisting, says the opposite. StoreExists has to be a report " +
					"of the store rather than of the seam's own records - a set that OpenStore adds to and Dispose " +
					"removes from satisfies every other cleanup assertion in this class while dropping nothing, and " +
					"this case and its partner exist to make that arrangement fail." + WhatThisFailureMeans);
			}
		}

		/// <summary>
		/// The second half, and the one that costs a real database if it is wrong. Same identity, same
		/// independent options captured through <c>ConfigureExisting</c> while active, queried after disposal.
		/// </summary>
		/// <remarks>
		/// <c>Exists() &amp;&amp; HasTables()</c> rather than <c>Exists()</c> alone because a released SQLite
		/// in-memory store answers <c>Exists()</c> true to the next connection that names it: connecting brings an
		/// empty database into being. The schema is what does not come back, on either provider.
		/// </remarks>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed()
		{
			//setup
			string identity;
			DbContextOptions<ProbeContext> probeOptions;

			using (var store = OpenStore(nameof(ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed)))
			{
				identity = IdentityOf(store);

				using (var context = ContextFor(store))
				{
					context.Database.EnsureCreated();
					context.Probes.Add(new Probe { Note = "written before disposal" });
					context.SaveChanges();
				}

				var builder = new DbContextOptionsBuilder<ProbeContext>();
				ConfigureExistingOnto(identity, builder);
				probeOptions = builder.Options;

				using (var probe = new ProbeContext(probeOptions))
				{
					probe.Probes.Select(row => row.Note).ToArray().ShouldBe(new[] { "written before disposal" },
						"The independent probe must read the committed row from this store before observing its disposal.");
				}
			}

			//act
			bool entityFrameworkFindsIt;
			using (var probe = new ProbeContext(probeOptions))
			{
				var creator = probe.Database.GetService<IRelationalDatabaseCreator>();
				entityFrameworkFindsIt = creator.Exists() && creator.HasTables();
			}

			//assert
			entityFrameworkFindsIt.ShouldBeFalse(
				$"Entity Framework, asked about store '{identity}' after the object that owned it was disposed, " +
				"found it and found tables in it. The store was not removed - whatever the seam reports. This is " +
				"the assertion a HashSet cannot pass: the answer comes from the provider, so bookkeeping that " +
				"forgets a database is not the same act as dropping one, and a certification run that ends here " +
				"green has left one database per store on the instance that carries ProphetsWay.Example." +
				WhatThisFailureMeans);

			StoreExists(identity).ShouldBe(entityFrameworkFindsIt,
				$"The seam says store '{identity}' " +
				$"{(StoreExists(identity) ? "still exists" : "is gone")} while Entity Framework says the opposite. " +
				"The two have to agree in both directions or StoreExists is not a measurement of anything: read " +
				"together with its partner case, this is what forces disposal to remove the store rather than the " +
				"record of it." + WhatThisFailureMeans);
		}

		/// <summary>
		/// This is the assertion an empty <c>Dispose</c> cannot pass, and it is not entailed by
		/// <see cref="ShouldIsolateEveryStoreFromEveryOther"/>: once every <c>OpenStore</c> yields a store of its
		/// own, a later store sees no rows whether or not the earlier one was ever removed. Without it, a
		/// certification run leaves a database on the owner's instance for every store it opened.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldRemoveTheStoreItselfWhenItIsDisposed()
		{
			//setup
			string identity;

			using (var store = OpenStore(nameof(ShouldRemoveTheStoreItselfWhenItIsDisposed)))
			{
				identity = IdentityOf(store);

				using (var context = ContextFor(store))
				{
					context.Database.EnsureCreated();
					context.Probes.Add(new Probe { Note = "written before disposal" });
					context.SaveChanges();
				}
			}

			//act
			var survived = StoreExists(identity);

			//assert
			survived.ShouldBeFalse(
				$"Store '{identity}' still exists after the object that owned it was disposed. Disposal is where a " +
				"SQL Server store has to be dropped from the server, and nothing else in this class can detect its " +
				"absence: unique stores per call already make a fresh store read zero rows, so row counts prove " +
				"nothing about cleanup. Left unfixed, one certification run leaves roughly one database per test " +
				"behind, and the instance that carries ProphetsWay.Example fills up with them." + WhatThisFailureMeans);
		}

		/// <summary>
		/// <c>IdentifierResolutionTests.UnopenedContext()</c> builds a context it never opens, so the seam has to
		/// tolerate a store nothing ever materialised. Disposal of one must be quiet, not a second failure mode
		/// layered over whatever the test was actually asserting.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldDisposeAStoreThatNoContextEverMaterialised()
		{
			//setup
			var store = OpenStore(nameof(ShouldDisposeAStoreThatNoContextEverMaterialised));

			var identity = IdentityOf(store);

			//act
			Should.NotThrow(() => store.Dispose(),
				$"Disposing store '{identity}', which no context ever opened, threw. A caller that builds options " +
				"and never queries - IdentifierResolutionTests does exactly this - would then see the seam's " +
				"failure instead of its own assertion." + WhatThisFailureMeans);

			//assert
			LiveIdentities().ShouldNotContain(identity,
				$"Lease '{identity}' remained live after its unopened store was disposed. Reusable stores retain their " +
				"physical scratch database, but the lease must retire immediately so an old handle cannot configure a " +
				"new owner's slot." + WhatThisFailureMeans);

			Should.Throw<ObjectDisposedException>(() => ContextForExistingStore(identity),
				$"Retired lease '{identity}' was accepted as an existing store. A later lease may reuse its physical " +
				"slot, but it must never revive an old identity or handle." + WhatThisFailureMeans);
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldClassifyExactlyTheApprovedLocalPhysicalLifecycleMethods()
		{
			var methods = typeof(ProviderSelectionTests).Assembly.GetTypes()
				.Where(type => type.IsClass && !type.IsAbstract)
				.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(IsTestMethod)
					.Select(method => new { Type = type, Method = method }))
				.ToArray();

			var excluded = methods
				.Where(test => TraitsOf(test.Method, test.Type).Contains("Execution=LocalPhysicalLifecycle"))
				.SelectMany(test => DiscoveredCases(test.Type, test.Method))
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToArray();

			var expectedExcluded = LocalPhysicalLifecycleMethodNames
				.Select(name => typeof(ProviderSelectionTests).FullName + "." + name)
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToArray();

			excluded.ShouldBe(expectedExcluded,
				"Only D-035's six physical database lifecycle specifications may be excluded from a reusable Azure run.");

			methods.Where(test => IsSkipped(test.Method, test.Type))
				.Select(test => test.Type.FullName + "." + test.Method.Name)
				.ShouldBeEmpty("D-035 uses a filter, never a skipped test, for lifecycle selection.");

			var included = methods
				.Where(test => !TraitsOf(test.Method, test.Type).Contains("Execution=LocalPhysicalLifecycle"))
				.SelectMany(test => DiscoveredCases(test.Type, test.Method))
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToArray();
			var discovered = methods
				.SelectMany(test => DiscoveredCases(test.Type, test.Method))
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToArray();

			included.Intersect(excluded, StringComparer.Ordinal).ShouldBeEmpty(
				"A discovered test case cannot be both Azure-included and local-physical-lifecycle-excluded.");
			included.Concat(excluded).OrderBy(name => name, StringComparer.Ordinal).ShouldBe(discovered);
		}

		private static bool IsTestMethod(MethodInfo method)
		{
			return method.GetCustomAttributes(typeof(FactAttribute), true).Any() ||
				method.GetCustomAttributes(typeof(TheoryAttribute), true).Any();
		}

		private static bool IsSkipped(MethodInfo method, Type concreteType)
		{
			return FactAttributesOf(method, concreteType).Any(attribute => !string.IsNullOrWhiteSpace(attribute.Skip));
		}

		private static IEnumerable<FactAttribute> FactAttributesOf(MethodInfo method, Type concreteType)
		{
			return method.GetCustomAttributes(typeof(FactAttribute), true).OfType<FactAttribute>()
				.Concat(concreteType.GetCustomAttributes(typeof(FactAttribute), true).OfType<FactAttribute>());
		}

		private static IReadOnlyCollection<string> TraitsOf(MethodInfo method, Type concreteType)
		{
			return CustomAttributeData.GetCustomAttributes(method)
				.Concat(TraitsOfTypeAndBases(concreteType))
				.Where(attribute => attribute.AttributeType == typeof(TraitAttribute) && attribute.ConstructorArguments.Count == 2)
				.Select(attribute => attribute.ConstructorArguments[0].Value + "=" + attribute.ConstructorArguments[1].Value)
				.ToArray();
		}

		private static IEnumerable<CustomAttributeData> TraitsOfTypeAndBases(Type type)
		{
			for (var current = type; current != null; current = current.BaseType)
				foreach (var attribute in CustomAttributeData.GetCustomAttributes(current))
					yield return attribute;
		}

		private static IEnumerable<string> DiscoveredCases(Type concreteType, MethodInfo method)
		{
			var name = concreteType.FullName + "." + method.Name;
			var count = InvocationCount(method);

			for (var index = 0; index < count; index++)
				yield return count == 1 ? name : name + "[" + index + "]";
		}

		private static int InvocationCount(MethodInfo method)
		{
			return method.GetCustomAttributes(typeof(TheoryAttribute), true).Any()
				? method.GetCustomAttributes(typeof(InlineDataAttribute), true).Length
				: 1;
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldResetAReusedStoreBeforeAnotherModelCreatesItsSchema()
		{
			IDisposable first = null;
			IDisposable heldOtherSlot = null;
			IDisposable reacquired = null;

			try
			{
				first = OpenStore(nameof(ShouldResetAReusedStoreBeforeAnotherModelCreatesItsSchema));
				heldOtherSlot = OpenStore(nameof(ShouldResetAReusedStoreBeforeAnotherModelCreatesItsSchema));
				var firstDatabaseName = StoreProperty(first, "DatabaseName");

				using (var context = ContextFor(first))
				{
					context.Database.EnsureCreated();
					context.Probes.Add(new Probe { Note = "first model" });
					context.SaveChanges();
				}

				first.Dispose();
				first = null;
				reacquired = OpenStore(nameof(ShouldResetAReusedStoreBeforeAnotherModelCreatesItsSchema));

				if (ReusableSqlServerIsSelected())
					StoreProperty(reacquired, "DatabaseName").ShouldBe(firstDatabaseName,
						"With the other reusable slot held, the alternate model must receive the exact physical slot that the first model released.");

				using (var context = AlternateContextFor(reacquired))
				{
					context.Database.EnsureCreated();
					context.AlternateProbes.Count().ShouldBe(0,
						"The alternate model must receive a schema-empty store after the first model releases it.");
				}
			}
			finally
			{
				reacquired?.Dispose();
				first?.Dispose();
				heldOtherSlot?.Dispose();
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldLeaveNothingBehindWhenAStoreIsDisposed()
		{
			//setup
			using (var store = OpenStore(nameof(ShouldLeaveNothingBehindWhenAStoreIsDisposed)))
			using (var writer = ContextFor(store))
			{
				writer.Database.EnsureCreated();
				writer.Probes.Add(new Probe { Note = "written before disposal" });
				writer.SaveChanges();
			}

			//act
			using (var store = OpenStore(nameof(ShouldLeaveNothingBehindWhenAStoreIsDisposed)))
			using (var reader = ContextFor(store))
			{
				reader.Database.EnsureCreated();

				var survived = reader.Probes.Count();

				//assert
				survived.ShouldBe(0,
					$"{survived} row(s) written by a disposed store were still readable. This says only that a new " +
					"store starts empty - whether the old one was removed is asserted by " +
					nameof(ShouldRemoveTheStoreItselfWhenItIsDisposed) + ", and fixing this one without fixing " +
					"that one leaves every run accumulating databases." + WhatThisFailureMeans);
			}
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldRunTheAdaptedUpstreamSuiteOnTheSelectedProvider()
		{
			//setup
			var expected = ProviderNameOf(AmbientSelection());

			//act - the same path the thirteen adapters take
			using (var dataAccess = TestDataAccessFactory.Create())
			{
				var context = ContextOf(dataAccess);
				var actual = context.Database.ProviderName;

				//assert
				actual.ShouldBe(expected,
					$"The adapted upstream suite runs on '{actual}' while the selection names '{expected}'. This is " +
					"the whole of the release gate: 270 cases green is not a SQL Server result unless every case " +
					"that reaches a store reached the same one. Constants.GetExampleDataAccess hardcodes a SQL " +
					"Server connection string, so it has to read the selection too - ExampleDataAccess already " +
					"takes a DbContextOptions<ExampleContext>, which is the constructor that makes this a small " +
					"change rather than a new implementation." + WhatThisFailureMeans);

				context.Database.GetService<IRelationalDatabaseCreator>().HasTables().ShouldBeTrue(
					$"The adapted upstream suite reached a store on '{actual}' that has no tables in it. A selection " +
					"the suite cannot provision must refuse by name at selection time, not half-run: thirteen adapter " +
					"classes failing on missing schema are indistinguishable, in a run log, from thirteen classes " +
					"finding real defects in this library." + WhatThisFailureMeans);
			}
		}

		/// <summary>
		/// The provider-selection counterpart of <c>AdapterCoverageTests</c>. Without it a partly migrated suite is
		/// green: the seam exists, every assertion above passes, and six classes quietly go on opening their own
		/// SQLite connection while the run is reported as SQL Server. It reads compiled IL rather than source, so a
		/// call made from a lambda, a local function, an iterator or an async state machine is found too.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldRouteEveryProviderSelectionInThisAssemblyThroughTheSeam()
		{
			//setup
			//act
			var sites = ProviderSelectionSitesOutsideTheSeam();

			//assert
			sites.ShouldBeEmpty(
				"These methods choose an Entity Framework provider without going through the seam:\n  " +
				string.Join("\n  ", sites) +
				"\n\nOnly two types may name a provider: " + string.Join(" and ", SelectionExemptRoots) + " - the " +
				"seam because that is its job, and the provider-comparison spike because its measurement IS the " +
				"comparison across two providers. Every other site is a class that cannot be pointed at the " +
				"selection, so a whole-suite run that includes it is a run against more than one store no matter " +
				"what the seam reports. An entry beginning '" + BlindSpot + "' is not a call site: it is a place " +
				"this scan could not look, which is the same thing as an undetected one." + WhatThisFailureMeans);
		}

		/// <summary>
		/// The positive control for <see cref="ShouldRouteEveryProviderSelectionInThisAssemblyThroughTheSeam"/>.
		/// That guard proves itself while it is red — seven named sites is a scanner visibly working. The moment the
		/// migration lands it goes green and stays green, and from then on a scanner that has quietly stopped
		/// detecting anything is indistinguishable from a suite with nothing left to detect. So the same scan is run
		/// once with nothing exempt, over a class that is known to select two providers on purpose, and required to
		/// come back with both.
		/// </summary>
		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldStillFindAProviderSelectionWhenNothingIsExempt()
		{
			//setup
			var spike = typeof(AlternateKeyGuardSpikeTests).FullName;

			//act
			var sites = ProviderSelectionSites(new string[0])
				.Where(s => s.StartsWith(spike, StringComparison.Ordinal))
				.ToList();

			//assert
			sites.Any(s => s.EndsWith(".UseSqlite", StringComparison.Ordinal)).ShouldBeTrue(
				$"Run with no exemptions, the scanner did not find the UseSqlite call in {spike}, which is written " +
				"there on purpose and is not going anywhere. It found: " +
				(sites.Count == 0 ? "nothing at all" : string.Join("; ", sites)) +
				". The scanner is therefore not detecting provider selections, and the guard that reports none " +
				"left is reporting that it looked, not that there are none. Fix the scan - do not relax this.");

			sites.Any(s => s.EndsWith(".UseInMemoryDatabase", StringComparison.Ordinal)).ShouldBeTrue(
				$"Run with no exemptions, the scanner did not find the UseInMemoryDatabase call in {spike}. It " +
				"found: " + (sites.Count == 0 ? "nothing at all" : string.Join("; ", sites)) +
				". Both providers are named in one private helper there, so finding one and not the other means " +
				"the instruction walk is losing its place partway through a method.");
		}

		[Fact]
		[Trait("Area", "ProviderSelection")]
		public void ShouldKeepTheProviderComparisonSpikeOutsideProviderCertification()
		{
			//setup
			var spike = typeof(AlternateKeyGuardSpikeTests);

			var cases = spike
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Where(m => m.GetCustomAttributes(typeof(FactAttribute), true).Length > 0)
				.ToList();

			cases.ShouldNotBeEmpty(
				$"{spike.Name} declares no test methods, so this guard is measuring nothing. It was renamed, " +
				"emptied or replaced - re-point this at whatever carries the provider comparison now.");

			var invocations = cases.Sum(m =>
			{
				var inline = m.GetCustomAttributes(typeof(InlineDataAttribute), true).Length;

				return inline == 0 ? 1 : inline;
			});

			cases.Count.ShouldBe(7,
				$"{spike.Name} declares {cases.Count} test methods rather than 7. The residual-selection guard " +
				"exempts this class by name, so its size is the size of the hole in provider certification. Adding " +
				"a method here widens that hole silently, and the settled footnote on a certified run is 'every " +
				"store-touching case except these fourteen'. If the spike really has to grow, the count changes " +
				"here in the same change set, so the footnote and the exemption move together.");

			invocations.ShouldBe(14,
				$"{spike.Name} declares {invocations} cases rather than 14. That figure is the exemption, quoted " +
				"as such wherever this suite is reported as certified against one provider; it is not a count " +
				"anybody re-derives when reading a run log.");

			//act
			var misScoped = cases
				.Where(m => !ScopesOf(m).Contains("Characterization") || ScopesOf(m).Contains("Contract"))
				.Select(m => m.Name)
				.OrderBy(n => n, StringComparer.Ordinal)
				.ToList();

			//assert
			misScoped.ShouldBeEmpty(
				"These cases in the provider-comparison spike are not Scope=Characterization:\n  " +
				string.Join("\n  ", misScoped) +
				"\n\nThe spike parameterizes SQLite and InMemory because its subject is Entity Framework Core's own " +
				"alternate-key behaviour compared ACROSS providers - the comparison is the measurement, so forcing " +
				"it onto the selected provider destroys it. It is therefore deliberately outside provider " +
				"certification, and 'all 270 on SQL Server' carries that as a stated footnote. Scope=Characterization " +
				"is what keeps it out of the conformance gate and keeps the footnote honest. If the spike is ever " +
				"meant to be certified, it needs its own provider leg rather than a re-trait.");
		}

		/// <summary>
		/// <c>TraitAttribute</c> exposes neither name nor value as a property - xUnit reads them off the metadata at
		/// discovery - so the constructor arguments are the only way to see a trait from inside a test.
		/// </summary>
		private static IReadOnlyCollection<string> ScopesOf(MethodInfo method)
		{
			return CustomAttributeData.GetCustomAttributes(method)
				.Concat(CustomAttributeData.GetCustomAttributes(method.DeclaringType))
				.Where(a => a.AttributeType == typeof(TraitAttribute) && a.ConstructorArguments.Count == 2)
				.Where(a => (string)a.ConstructorArguments[0].Value == "Scope")
				.Select(a => (string)a.ConstructorArguments[1].Value)
				.ToList();
		}

		#region cleanup that can be seen, and identities the seam may not drop

		/// <summary>
		/// Carried on every assertion in the region below. The cases above certify that <i>one</i> store was
		/// dropped; these certify that a run notices when the others are not.
		/// </summary>
		private const string WhatACleanupFailureMeans =
			"\n\nWHAT THIS MEANS: store cleanup is unobservable. Store.Dispose catches every failure and reports " +
			"nothing, and no case above watches any drop but its own - so a drop path that fails for every store " +
			"except the handful this class disposes by hand is a green run that leaves a database per test on the " +
			"instance carrying ProphetsWay.Example, and the zero-orphan figure in the Gate 1 record is a number " +
			"somebody typed at a server once rather than a property this suite holds. Three surfaces close that: " +
			"Store.DisposeReportingFailure(), which disposes and hands back whatever cleanup failed with instead of " +
			"swallowing it; TestStore.LiveIdentities, so a sweep can tell an orphan from a store another test is " +
			"still using; and TestStore.IsDisposableIdentity(string), the one predicate that decides whether an " +
			"identity may be dropped at all. Relaxing an assertion here is how a suite goes on reporting a clean " +
			"server it has stopped looking at.";

		/// <summary>
		/// The store type itself, rather than the type of an instance, so its constructor can be reached before
		/// any store is opened - which is where the refusal to name a database the seam may not drop has to live.
		/// </summary>
		private static Type StoreType()
		{
			var type = SeamType().GetNestedType("Store", BindingFlags.Public | BindingFlags.NonPublic);

			type.ShouldNotBeNull(
				$"{SeamTypeName} declares no nested 'Store' type, so nothing owns the lifetime of one store and " +
				"there is no constructor to refuse a foreign identity in." + WhatACleanupFailureMeans);

			return type;
		}

		private static ConstructorInfo StoreConstructor()
		{
			var constructor = StoreType().GetConstructor(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				new[] { ProviderEnum(), typeof(string) },
				null);

			constructor.ShouldNotBeNull(
				$"{StoreType().FullName} declares no ({ProviderTypeName}, string) constructor, so the boundary a " +
				"foreign identity would have to cross cannot be reached without opening a store first - which is " +
				"the one thing a refusal has to happen before." + WhatACleanupFailureMeans);

			return constructor;
		}

		private static object Construct(ConstructorInfo constructor, params object[] arguments)
		{
			try
			{
				return constructor.Invoke(arguments);
			}
			catch (TargetInvocationException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

				throw;
			}
		}

		private static bool IsDisposableIdentity(string identity)
		{
			return (bool)Invoke(Method(SeamType(), "IsDisposableIdentity", typeof(string)), null, identity);
		}

		/// <summary>
		/// The identities of every store that has been opened and not yet disposed. It exists so a sweep of the
		/// server can subtract the stores other tests are still using, rather than reporting them as orphans.
		/// </summary>
		private static IReadOnlyCollection<string> LiveIdentities()
		{
			var property = SeamType().GetProperty(
				"LiveIdentities",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			property.ShouldNotBeNull(
				$"{SeamTypeName} exposes no static 'LiveIdentities'. Without it a sweep of the server cannot tell " +
				"a database that was left behind from one a concurrently running class is still writing to, so the " +
				"sweep is either blind or flaky and there is no third option." + WhatACleanupFailureMeans);

			var live = property.GetValue(null) as IEnumerable<string>;

			live.ShouldNotBeNull(
				$"{SeamTypeName}.LiveIdentities is not a sequence of identities, so nothing can be subtracted from " +
				"a sweep of the server." + WhatACleanupFailureMeans);

			return live.ToList();
		}

		private static void ConfigureOnto(object store, DbContextOptionsBuilder builder)
		{
			Invoke(Method(store.GetType(), "Configure", typeof(DbContextOptionsBuilder)), store, builder);
		}

		private static void ConfigureExistingOnto(string identity, DbContextOptionsBuilder builder)
		{
			Invoke(
				Method(SeamType(), "ConfigureExisting", typeof(string), typeof(DbContextOptionsBuilder)),
				null,
				identity,
				builder);
		}

		private static bool SqlServerIsSelected()
		{
			return Equals(AmbientSelection(), Selection("SqlServer"));
		}

		/// <summary>
		/// Every database on the selected SQL Server instance that the seam's own predicate says is one of its
		/// disposable stores.
		/// </summary>
		/// <remarks>
		/// The catalogue is read whole and then filtered through <c>IsDisposableIdentity</c> rather than through a
		/// <c>LIKE</c> this class writes for itself. That is deliberate: the sweep and the drop then agree by
		/// construction, so a predicate that stops recognising its own stores cannot leave the sweep looking for a
		/// pattern nothing is named with any more. It reaches the instance through <c>ConfigureExisting</c>, so no
		/// provider is selected here and the residual-selection guard stays true.
		/// </remarks>
		private static IReadOnlyCollection<string> DisposableStoresOnTheServer()
		{
			var builder = new DbContextOptionsBuilder();

			ConfigureExistingOnto("master", builder);

			var found = new List<string>();

			using (var context = new DbContext(builder.Options))
			{
				var connection = context.Database.GetDbConnection();

				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT name FROM sys.databases";

					using (var reader = command.ExecuteReader())
						while (reader.Read())
						{
							var name = reader.GetString(0);

							if (IsDisposableIdentity(name))
								found.Add(name);
						}
				}
			}

			return found;
		}

		private static IEnumerable<MethodBase> MethodsWithin(Type root)
		{
			var ignored = new SortedSet<string>(StringComparer.Ordinal);

			foreach (var type in TypesOf(root.Assembly, ignored))
			{
				if (RootOf(type) != root)
					continue;

				foreach (var method in DeclaredMethods(type, ignored))
					yield return method;
			}
		}

		private static IReadOnlyCollection<string> CalleeNamesOf(MethodBase method)
		{
			var ignored = new SortedSet<string>(StringComparer.Ordinal);

			return CalleesOf(method, ignored)
				.Select(c => c.Name)
				.ToList();
		}

		/// <summary>
		/// The callees of one method in the order the instruction stream names them — which, for straight-line code
		/// and the branches around it, is the order the source was written in. It is what lets a claim about
		/// <i>sequence</i> be made structurally rather than by reading a file.
		/// </summary>
		private static IReadOnlyList<MethodBase> OrderedCalleesOf(MethodBase method)
		{
			var ignored = new SortedSet<string>(StringComparer.Ordinal);

			return CalleesOf(method, ignored).ToList();
		}

		private static int IndexOfFirst(IReadOnlyList<MethodBase> callees, Func<MethodBase, bool> matches)
		{
			for (var index = 0; index < callees.Count; index++)
				if (matches(callees[index]))
					return index;

			return -1;
		}

		/// <summary>
		/// Whether a call adds to — or removes from — the collection of identities <c>LiveIdentities</c> hands out,
		/// either directly or through one seam method that does.
		/// </summary>
		/// <remarks>
		/// Recognised by what the call does to a collection <i>of identities</i>, not by the name of the seam method
		/// wrapping it, so renaming <c>RegisterIdentity</c> does not blind the guard. The element type is what keeps
		/// this honest: <c>ExecuteNonQuery</c>'s parameter collection also has a method called <c>Add</c>, and it is
		/// not a collection of strings.
		/// </remarks>
		private static bool MutatesTheLiveRegistry(MethodBase callee, string mutator)
		{
			if (IsRegistryMutation(callee, mutator))
				return true;

			return RootOf(callee.DeclaringType) == SeamType() &&
				OrderedCalleesOf(callee).Any(inner => IsRegistryMutation(inner, mutator));
		}

		private static bool IsRegistryMutation(MethodBase callee, string mutator)
		{
			return string.Equals(callee.Name, mutator, StringComparison.Ordinal) &&
				callee.DeclaringType != null &&
				typeof(ICollection<string>).IsAssignableFrom(callee.DeclaringType);
		}

		/// <summary>
		/// Whether a call is the act of bringing the store into existence — on SQLite the keeper connection
		/// <i>is</i> the store, and on SQL Server the store arrives as a command sent to the instance.
		/// </summary>
		private static bool BringsAStoreIntoBeing(MethodBase callee)
		{
			if (callee.DeclaringType == null)
				return false;

			if (typeof(DbConnection).IsAssignableFrom(callee.DeclaringType))
				return true;

			return RootOf(callee.DeclaringType) == SeamType() &&
				OrderedCalleesOf(callee).Any(inner => string.Equals(inner.Name, "ExecuteNonQuery", StringComparison.Ordinal));
		}

		/// <summary>
		/// The cleanup surface itself. A <c>Dispose</c> that catches everything is correct — a failed drop must not
		/// replace the assertion failure that caused it — but correct and silent are not the same thing, and the
		/// silence is what makes 303 green compatible with 87 databases left on the server.
		/// </summary>
		/// <remarks>
		/// The success case is the only one this can drive: nothing here can make a real <c>DROP DATABASE</c> fail
		/// without editing the seam. So it pins the shape and the quiet path — the method exists, returns an
		/// <see cref="Exception"/>, answers null when the store really was removed, stays idempotent, and the store
		/// really is gone afterwards. What turns that shape into a standing property is
		/// <see cref="ShouldRouteEveryFixtureStoreThroughTheReportingCleanup"/>, which requires all six store-backed
		/// fixtures to read the answer rather than discard it.
		/// </remarks>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldReportACleanupFailureRatherThanSwallowingIt()
		{
			//setup
			var reporting = Method(StoreType(), "DisposeReportingFailure");

			typeof(Exception).IsAssignableFrom(reporting.ReturnType).ShouldBeTrue(
				$"{StoreType().FullName}.DisposeReportingFailure returns {reporting.ReturnType.Name} rather than an " +
				"Exception. A caller cannot fail its own test on a cleanup failure it is only told about as a " +
				"boolean, because it has nothing to put in the failure message and no stack to look at." +
				WhatACleanupFailureMeans);

			var store = OpenStore(nameof(ShouldReportACleanupFailureRatherThanSwallowingIt));

			var identity = IdentityOf(store);

			//act
			var first = Invoke(reporting, store);
			var second = Invoke(reporting, store);

			//assert
			first.ShouldBeNull(
				$"Disposing store '{identity}' reported a cleanup failure ({first}) where the drop was expected to " +
				"succeed. Either the drop is broken - which is exactly what this surface exists to make visible - " +
				"or the method reports something other than a failure, in which case every caller that gates on it " +
				"will fail tests that passed." + WhatACleanupFailureMeans);

			second.ShouldBeNull(
				$"A second DisposeReportingFailure on store '{identity}' reported {second}. Disposal is idempotent " +
				"and the second call has nothing left to clean up, so it has nothing to report; a caller in a " +
				"finally block cannot know whether it is the first." + WhatACleanupFailureMeans);

			StoreExists(identity).ShouldBeFalse(
				$"Store '{identity}' survived a DisposeReportingFailure that reported no failure. A cleanup that " +
				"answers 'nothing went wrong' without removing the store is worse than the silent catch it " +
				"replaced: it is a positive statement that the server is clean." + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// One cleanup path, not two. If <c>Dispose</c> keeps a second copy of the drop with its own <c>catch</c>,
		/// then the reporting surface is a method the fixtures call and the runtime does not, and every store
		/// closed by a <c>using</c> anywhere else in the assembly goes on failing silently.
		/// </summary>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldMakeOrdinaryDisposalGoThroughTheReportingCleanup()
		{
			//setup
			var dispose = Method(StoreType(), "Dispose");

			//act
			var callees = CalleeNamesOf(dispose);

			//assert
			callees.ShouldContain("DisposeReportingFailure",
				$"{StoreType().FullName}.Dispose does not call DisposeReportingFailure - it calls: " +
				(callees.Count == 0 ? "nothing at all" : string.Join(", ", callees.Distinct().OrderBy(n => n, StringComparer.Ordinal))) +
				". Two cleanup paths means the one nobody watches is the one most stores take." +
				WhatACleanupFailureMeans);
		}

		/// <summary>
		/// The six store-backed fixtures own roughly ninety of the ninety-three stores a run opens. Wherever one of
		/// them opens a store, that call site has to read the cleanup answer — otherwise the reporting surface is a
		/// method with three callers, all of them in this class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Asserted on the compiled method rather than on a run, because a cleanup failure cannot be induced from
		/// here: what is being specified is that the call site exists and takes the answer, and IL is where that is
		/// visible. A site must gate on it only when the body succeeded — a cleanup failure raised over a failing
		/// assertion replaces the finding with its consequence, which is the behaviour the silent catch was right
		/// about.
		/// </para>
		/// <para>
		/// <b>Every method that opens a store, not every method called <c>WithStore</c>.</b> Watching the helper by
		/// name leaves a fixture free to open a store anywhere else in the same class under a plain <c>using</c> and
		/// stay green — which is exactly what two cases in <c>IdentifierResolutionTests</c> did while this guard
		/// claimed to cover every store-backed fixture in the assembly. The subject is the call to <c>OpenStore</c>,
		/// so a new site is covered the moment it is written rather than the moment somebody remembers to name it
		/// after the helper.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldRouteEveryFixtureStoreThroughTheReportingCleanup()
		{
			//setup
			var fixtures = new[]
			{
				"ProphetsWay.EFTools.Tests.FailedInsertWriteBackTests",
				"ProphetsWay.EFTools.Tests.IdentifierResolutionTests",
				"ProphetsWay.EFTools.Tests.KeyPredicateOpenKeyTests",
				"ProphetsWay.EFTools.Tests.KeylessDaoTests",
				"ProphetsWay.EFTools.Tests.KeylessSoftDaoTests",
				"ProphetsWay.EFTools.Tests.SoftDeleteTimestampHookTests"
			};

			var assembly = typeof(ProviderSelectionTests).Assembly;

			//act
			var offenders = new List<string>();

			foreach (var name in fixtures)
			{
				var fixture = assembly.GetType(name, false);

				if (fixture == null)
				{
					offenders.Add($"{name} is not in this assembly at all, so this guard has stopped watching it");

					continue;
				}

				var opening = MethodsWithin(fixture)
					.Select(m => new { Method = m, Callees = CalleeNamesOf(m) })
					.Where(m => m.Callees.Contains("OpenStore", StringComparer.Ordinal))
					.ToList();

				if (opening.Count == 0)
				{
					offenders.Add(
						$"{name} opens no store anywhere, so this guard cannot see how it cleans up - it was emptied, " +
						"or it now reaches a store by some route this guard does not watch");

					continue;
				}

				offenders.AddRange(opening
					.Where(m => !m.Callees.Contains("DisposeReportingFailure", StringComparer.Ordinal))
					.Select(m => $"{Describe(m.Method)} opens a store and discards the cleanup answer")
					.OrderBy(n => n, StringComparer.Ordinal));
			}

			//assert
			offenders.ShouldBeEmpty(
				"These fixtures open a store per test and cannot tell you when one failed to go away:\n  " +
				string.Join("\n  ", offenders) +
				"\n\nEvery method that opens a store has to hold it in a try/finally with a completed flag, dispose " +
				"through the reporting cleanup, and re-raise the cleanup failure only when the body itself succeeded. " +
				"A using block cannot do that: it discards the answer, which is how ninety silent failures fit " +
				"inside a green run." + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// The registry the orphan sweep subtracts. A store is listed from the moment it exists until the moment
		/// its cleanup has run — anything narrower makes the sweep flaky, anything wider makes it blind.
		/// </summary>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldListEveryLiveStoreAndForgetEachDisposedOne()
		{
			//setup
			var store = OpenStore(nameof(ShouldListEveryLiveStoreAndForgetEachDisposedOne));

			var identity = IdentityOf(store);

			//act
			var whileLive = LiveIdentities();

			store.Dispose();

			var afterDisposal = LiveIdentities();

			//assert
			whileLive.ShouldContain(identity,
				$"Store '{identity}' was open and is not in LiveIdentities. A sweep that subtracts this list would " +
				"then report a store another class is still writing to as an orphan, and the sweep would be " +
				"switched off within a week for being flaky." + WhatACleanupFailureMeans);

			afterDisposal.ShouldNotContain(identity,
				$"Store '{identity}' is still in LiveIdentities after it was disposed. A list nothing is removed " +
				"from subtracts every store ever opened, so the sweep can never find an orphan and reports a clean " +
				"server unconditionally." + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// The standing form of the zero-orphan figure in the Gate 1 record. Every disposable store on the
		/// instance, less every store any test currently holds — twice, because a class running in another
		/// collection may open one between the snapshot and the query.
		/// </summary>
		/// <remarks>
		/// A suspect is re-checked rather than reported on one look. A store opened after the first snapshot and
		/// dropped before the second appears in neither, so a single pass could name it; a second query with a
		/// third snapshot subtracted removes that window without weakening what is asserted — an orphan is a
		/// database that is still there, still disposable, and held by nobody.
		/// </remarks>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldLeaveNoDisposableStoreBehindOnTheServer()
		{
			//setup
			var store = OpenStore(nameof(ShouldLeaveNoDisposableStoreBehindOnTheServer));

			var identity = IdentityOf(store);

			if (!SqlServerIsSelected())
			{
				//the SQLite leg has no catalogue to sweep: an in-memory store ends with the connection that keeps
				//it, so the equivalent standing property is that the registry drains and the store stops answering
				try
				{
					LiveIdentities().ShouldContain(identity,
						$"SQLite store '{identity}' is open and unlisted." + WhatACleanupFailureMeans);

					store.Dispose();

					LiveIdentities().ShouldNotContain(identity,
						$"SQLite store '{identity}' is still listed after disposal, so on the SQL Server leg the " +
						"sweep below would subtract it forever and never see an orphan." + WhatACleanupFailureMeans);

					StoreExists(identity).ShouldBeFalse(
						$"SQLite store '{identity}' still answers after disposal." + WhatACleanupFailureMeans);
				}
				finally
				{
					store.Dispose();
				}

				return;
			}

			//act
			try
			{
				var beforeQuery = LiveIdentities();
				var onServer = DisposableStoresOnTheServer();
				var afterQuery = LiveIdentities();

				var suspects = onServer
					.Except(beforeQuery, StringComparer.Ordinal)
					.Except(afterQuery, StringComparer.Ordinal)
					.ToList();

				var orphans = new List<string>();

				if (suspects.Count > 0)
				{
					var stillHeld = LiveIdentities();
					var stillThere = DisposableStoresOnTheServer();

					orphans = suspects
						.Intersect(stillThere, StringComparer.Ordinal)
						.Except(stillHeld, StringComparer.Ordinal)
						.OrderBy(n => n, StringComparer.Ordinal)
						.ToList();
				}

				//assert
				onServer.ShouldContain(identity,
					$"The sweep of the server did not find store '{identity}', which this test is holding open. " +
					"Either the catalogue is not being read, or IsDisposableIdentity does not recognise the seam's " +
					"own naming - and a sweep that cannot see a store it is looking straight at reports zero " +
					"orphans on an instance full of them." + WhatACleanupFailureMeans);

				orphans.ShouldBeEmpty(
					$"{orphans.Count} disposable store(s) are on the server and held by no test:\n  " +
					string.Join("\n  ", orphans) +
					"\n\nEach one is a database a previous run created and failed to drop. They were confirmed on a " +
					"second query with a third live-store snapshot subtracted, so a class opening a store in " +
					"parallel is not what this is. Drop them, then find the cleanup that failed silently - " +
					nameof(ShouldReportACleanupFailureRatherThanSwallowingIt) + " and " +
					nameof(ShouldRouteEveryFixtureStoreThroughTheReportingCleanup) + " exist so the next one is " +
					"reported at the test that caused it rather than found here a week later." +
					WhatACleanupFailureMeans);
			}
			finally
			{
				store.Dispose();
			}
		}

		/// <summary>
		/// The order of the two acts in <c>Store</c>'s constructor: a store must be listed <i>before</i> it is
		/// brought into being, and unlisted again if bringing it into being fails.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Provisioning first opens a window — however short — in which a real database is on the server and in
		/// neither of the snapshots <see cref="ShouldLeaveNoDisposableStoreBehindOnTheServer"/> subtracts. The six
		/// store-backed fixtures are outside this class's collection and so run alongside that sweep, which makes
		/// the window reachable rather than theoretical, and what it produces is a <i>false</i> orphan: a database
		/// another test is in the middle of creating, reported as one a previous run left behind. That is the
		/// failure that gets a sweep switched off for being flaky, and the zero-orphan property goes with it.
		/// Registering first cannot cause the opposite error, because the registry is only ever <i>subtracted</i>
		/// from a sweep — an identity listed a moment early hides nothing that was not this run's to hide.
		/// </para>
		/// <para>
		/// <b>Structural on purpose, and it could not be otherwise.</b> The two orders are behaviourally
		/// indistinguishable from outside: on the success path both end with the identity listed, and on a
		/// provisioning failure both end with it unlisted — the broken order because it was never added, the
		/// correct one because it was rolled back. Only the instruction stream separates them, and making a real
		/// <c>CREATE DATABASE</c> fail on demand would mean editing the seam from here. Nothing below names a
		/// private method or a line number: registration is found as the call that adds an identity to a
		/// collection of them, provisioning as the call that opens a connection or reaches the server, so both
		/// survive a rename.
		/// </para>
		/// <para>
		/// The rollback is pinned by shape for the same reason — the constructor has to carry an exception handler,
		/// and the call that unlists the identity has to sit after the provisioning it protects, since a handler is
		/// emitted after its protected region. An <i>unconditional</i> forget would satisfy that and then fail
		/// <see cref="ShouldListEveryLiveStoreAndForgetEachDisposedOne"/> and the sweep instead, so the three
		/// guards close the case between them.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldRegisterAStoreBeforeProvisioningItAndForgetItIfProvisioningFails()
		{
			//setup
			var constructor = StoreConstructor();

			var body = constructor.GetMethodBody();

			body.ShouldNotBeNull(
				$"{StoreType().FullName}'s constructor has no readable body, so nothing here can tell whether it " +
				"lists a store before it creates one. Re-point this guard rather than deleting it." +
				WhatACleanupFailureMeans);

			var callees = OrderedCalleesOf(constructor);

			//act
			var registration = IndexOfFirst(callees, c => MutatesTheLiveRegistry(c, "Add"));
			var provisioning = IndexOfFirst(callees, BringsAStoreIntoBeing);
			var rollback = IndexOfFirst(callees, c => MutatesTheLiveRegistry(c, "Remove"));

			var order = callees.Count == 0
				? "nothing at all"
				: string.Join(", ", callees.Select(Describe));

			//assert
			registration.ShouldNotBe(-1,
				$"{StoreType().FullName}'s constructor never adds the identity to the live registry. Either it has " +
				"stopped registering - in which case every store it opens is invisible to the sweep, which then " +
				"reports each one as an orphan - or the registry is no longer a collection of identities and this " +
				$"guard is watching the wrong call. The constructor calls: {order}" + WhatACleanupFailureMeans);

			provisioning.ShouldNotBe(-1,
				$"{StoreType().FullName}'s constructor opens no connection and sends no command, so this guard " +
				"cannot see the store being brought into being and has nothing to order the registration against. " +
				$"The constructor calls: {order}" + WhatACleanupFailureMeans);

			registration.ShouldBeLessThan(provisioning,
				$"{StoreType().FullName}'s constructor brings the store into being " +
				$"({Describe(callees[provisioning])}) before it registers the identity " +
				$"({Describe(callees[registration])}). Between those two calls a real database is on the server and " +
				"in no LiveIdentities snapshot, so " + nameof(ShouldLeaveNoDisposableStoreBehindOnTheServer) +
				" - which runs alongside six fixtures that open a store per test - can read the catalogue inside " +
				"that window and report a store another test is still creating as an orphan. Register immediately " +
				"after the identity check instead: the registry is only ever subtracted from a sweep, so listing an " +
				"identity a moment early hides nothing." + WhatACleanupFailureMeans);

			rollback.ShouldNotBe(-1,
				$"{StoreType().FullName}'s constructor never removes the identity from the live registry. Once " +
				"registration comes first, a provisioning failure leaves a name listed for a store that was never " +
				"created and never will be - and the sweep subtracts that name for the rest of the run, which is " +
				$"the blind half of the same property. The constructor calls: {order}" + WhatACleanupFailureMeans);

			body.ExceptionHandlingClauses.Count.ShouldBeGreaterThan(0,
				$"{StoreType().FullName}'s constructor declares no exception handler, so whatever unlists the " +
				"identity cannot be on the failure path - it runs on every construction or on none. Provisioning " +
				"has to be guarded, the identity forgotten, and the failure re-raised so the caller still learns " +
				"the store could not be created." + WhatACleanupFailureMeans);

			rollback.ShouldBeGreaterThan(provisioning,
				$"{StoreType().FullName}'s constructor unlists the identity ({Describe(callees[rollback])}) before " +
				"it provisions the store, so that call is not a rollback: a handler is emitted after the region it " +
				"protects, and a forget that runs first leaves every live store unlisted for the sweep to report as " +
				"an orphan." + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// The single predicate that decides whether an identity may be dropped, held to the two things it exists
		/// for: it recognises the seam's own stores, and it refuses everything else — the shared
		/// <c>ProphetsWay.Example</c> database above all, which the thirteen adapted upstream classes count rows in
		/// and which no test may take away.
		/// </summary>
		[Theory]
		[Trait("Area", "StoreCleanup")]
		[InlineData("EFToolsTest_KeylessDaoTests_0123456789abcdef0123456789abcdef", true)]
		[InlineData("EFToolsTest_x", true)]
		[InlineData("ProphetsWay.Example", false)]
		[InlineData("master", false)]
		[InlineData("tempdb", false)]
		[InlineData("EFToolsTest", false)]
		[InlineData("eftoolstest_x", false)]
		[InlineData("XEFToolsTest_x", false)]
		[InlineData(" EFToolsTest_x", false)]
		[InlineData("", false)]
		[InlineData("   ", false)]
		[InlineData(null, false)]
		public void ShouldTreatOnlyItsOwnStoresAsDroppable(string identity, bool droppable)
		{
			//setup
			//act
			var answer = IsDisposableIdentity(identity);

			//assert
			answer.ShouldBe(droppable,
				$"IsDisposableIdentity({(identity == null ? "null" : $"'{identity}'")}) answered {answer}. " +
				(droppable
					? "That is the seam's own naming, so a sweep filtered through this predicate would stop seeing " +
						"the stores it is meant to sweep and a drop would stop dropping them."
					: "That is not a database this suite created. This predicate is the whole of the protection " +
						"standing between a test run and the shared ProphetsWay.Example database on the same " +
						"instance, and a predicate that widens by one character takes it away.") +
				WhatACleanupFailureMeans);
		}

		/// <summary>
		/// The refusal has to land in the constructor, before anything is opened. A store that can be built around
		/// a database it may not drop is one <c>Dispose</c> away from dropping it, and the guard that stops it is
		/// then the last line of defence rather than the first.
		/// </summary>
		/// <remarks>
		/// <c>SqlServer</c> is named explicitly rather than taken from the ambient selection so this case opens no
		/// connection on either leg, and the constructed store is deliberately <b>not</b> disposed if the
		/// constructor lets it through — disposing it is precisely the drop this case exists to prevent.
		/// </remarks>
		[Theory]
		[Trait("Area", "StoreCleanup")]
		[InlineData("ProphetsWay.Example")]
		[InlineData("master")]
		[InlineData("EFToolsTest")]
		public void ShouldRefuseToOpenAStoreOnAnIdentityItMayNotDrop(string foreign)
		{
			//setup
			var constructor = StoreConstructor();

			object opened = null;

			//act
			var thrown = Record.Exception(() => opened = Construct(constructor, Selection("SqlServer"), foreign));

			//assert
			opened.ShouldBeNull(
				$"A store was opened on '{foreign}', which IsDisposableIdentity refuses. Nothing else in this class " +
				"can catch that: the object is now indistinguishable from a real store, and the next Dispose - a " +
				"using block exiting, a finally, a failing test unwinding - runs the drop against a database this " +
				"suite did not create. It has deliberately not been disposed here." + WhatACleanupFailureMeans);

			thrown.ShouldBeAssignableTo<ArgumentException>(
				$"Opening a store on '{foreign}' threw {thrown?.GetType().Name ?? "nothing"}. It has to be refused " +
				"as a bad argument, at construction, before a connection is opened or a name is recorded." +
				WhatACleanupFailureMeans);

			thrown.Message.Contains(foreign).ShouldBeTrue(
				"The refusal does not name the identity it refused, so a maintainer meeting it has to guess which " +
				$"database was being asked for. The message was: {thrown.Message}" + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// One predicate, consulted by every database-DDL execution owner. SQL literals and reachable builders
		/// distinguish database lifecycle commands from reusable table reset, never a private method's name.
		/// </summary>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldDecideEveryDropWithTheOnePredicateThatGuardsIt()
		{
			//setup
			var blindSpots = new SortedSet<string>(StringComparer.Ordinal);
			var controls = typeof(DatabaseDdlControls);

			SqlCommandsOf(Method(controls, "Reset", typeof(DbCommand)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Database });
			SqlCommandsOf(Method(controls, "ResetTables", typeof(DbCommand)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Schema });
			SqlCommandsOf(Method(controls, "FromBuilder", typeof(DbCommand)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Database });
			SqlCommandsOf(Method(controls, "Read", typeof(DbCommand)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Read });
			SqlCommandsOf(Method(controls, "Unknown", typeof(DbCommand), typeof(string)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Unknown });
			SqlCommandsOf(Method(controls, "Mixed", typeof(DbCommand), typeof(string)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Schema, SqlCommandKind.Unknown });
			SqlCommandsOf(Method(controls, "Alternative", typeof(DbCommand), typeof(string), typeof(bool)), blindSpots)
				.ShouldBe(new[] { SqlCommandKind.Unknown });

			var unresolvedControls = new[]
			{
				Method(controls, "FromBranchingBuilder", typeof(DbCommand), typeof(string), typeof(bool)),
				Method(controls, "FromNestedBuilder", typeof(DbCommand), typeof(string), typeof(bool)),
				Method(controls, "FromUnresolvedBuilder", typeof(DbCommand), typeof(string)),
				Method(controls, "FromCyclicBuilder", typeof(DbCommand)),
				Method(controls, "ReadWithDynamicExecution", typeof(DbCommand)),
				Method(controls, "SchemaWithDynamicExecution", typeof(DbCommand))
			};
			unresolvedControls
				.Select(method => $"{method.Name}: {string.Join(", ", SqlCommandsOf(method, blindSpots))}")
				.ShouldBe(new[]
				{
					"FromBranchingBuilder: Unknown",
					"FromNestedBuilder: Unknown",
					"FromUnresolvedBuilder: Unknown",
					"FromCyclicBuilder: Unknown",
					"ReadWithDynamicExecution: Unknown, Unknown",
					"SchemaWithDynamicExecution: Unknown, Unknown"
				});

			var predicate = Method(SeamType(), "IsDisposableIdentity", typeof(string));
			var executing = TypesOf(SeamType().Assembly, blindSpots)
				.Where(type => RootOf(type) == SeamType())
				.SelectMany(type => DeclaredMethods(type, blindSpots))
				.Select(method => new { Method = method, Callees = CalleesOf(method, blindSpots).ToArray() })
				.Where(method => method.Callees.Any(ExecutesSql))
				.Select(method => new { method.Method, method.Callees, Commands = SqlCommandsOf(method.Method, blindSpots) })
				.ToList();

			executing.ShouldNotBeEmpty(
				$"Nothing inside {SeamTypeName} sends a command to the server, so either the drop is gone - and " +
				"every store a run opens now survives it - or this guard is looking for the wrong call and is " +
				"reporting that it looked rather than that it found nothing." + WhatACleanupFailureMeans);

			//act
			var databaseDdl = executing.Where(method => method.Commands.Contains(SqlCommandKind.Database)).ToList();
			var unclassified = executing
				.Where(method => method.Commands.Count == 0 || method.Commands.Contains(SqlCommandKind.Unknown))
				.Select(method => Describe(method.Method))
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToList();
			var unguarded = databaseDdl
				.Where(method => !method.Callees.Contains(predicate))
				.Select(m => Describe(m.Method))
				.OrderBy(n => n, StringComparer.Ordinal)
				.ToList();

			//assert
			unclassified.ShouldBeEmpty(
				"Every SQL execution site must be classified; unknown SQL cannot be exempted from database-DDL guards.");
			databaseDdl.ShouldNotBeEmpty(
				"No real CREATE, DROP or ALTER DATABASE executor was found; table reset alone cannot satisfy physical lifecycle.");
			unguarded.ShouldBeEmpty(
				"These methods issue database DDL without consulting IsDisposableIdentity:\n  " +
				string.Join("\n  ", unguarded) +
				"\n\nThe drop is a DROP DATABASE against an instance that also carries ProphetsWay.Example. Whatever " +
				"decides which names are droppable must be the same predicate " +
				nameof(ShouldTreatOnlyItsOwnStoresAsDroppable) + " pins and " +
				nameof(ShouldLeaveNoDisposableStoreBehindOnTheServer) + " sweeps with, or the three can disagree " +
				"and the only one that matters is the one nothing tests." + WhatACleanupFailureMeans);

			CalleesOf(StoreConstructor(), blindSpots).ShouldContain(predicate,
				$"{StoreType().FullName}'s constructor does not consult IsDisposableIdentity, so a foreign identity " +
				"is refused - if at all - only once something tries to drop it." + WhatACleanupFailureMeans);
			blindSpots.ShouldBeEmpty("An incomplete IL scan cannot establish database-DDL ownership.");
		}

		private enum SqlCommandKind
		{
			Unknown,
			Read,
			Schema,
			Database
		}

		private static bool ExecutesSql(MethodBase method)
		{
			return new[] { "ExecuteNonQuery", "ExecuteReader", "ExecuteScalar", "ExecuteDbDataReader", "ExecuteSql" }
				.Any(name => method.Name.StartsWith(name, StringComparison.Ordinal));
		}

		private static IReadOnlyList<SqlCommandKind> SqlCommandsOf(MethodBase method, ISet<string> blindSpots)
		{
			var commands = new List<SqlCommandKind>();
			var assignments = new List<SqlCommandKind>();
			var literals = new List<string>();
			var unknownBuilder = false;

			foreach (var callee in CalleesOf(method, blindSpots, literals.Add))
			{
				if (ExecutesSql(callee))
				{
					if (assignments.Count == 0)
						assignments.Add(unknownBuilder ? SqlCommandKind.Unknown : ClassifySql(literals));

					commands.Add(assignments.Contains(SqlCommandKind.Unknown) ? SqlCommandKind.Unknown : assignments.Max());
					assignments.Clear();
					literals.Clear();
					unknownBuilder = false;
				}
				else if (callee.Name == "set_CommandText" && typeof(DbCommand).IsAssignableFrom(callee.DeclaringType))
				{
					assignments.Add(unknownBuilder ? SqlCommandKind.Unknown : ClassifySql(literals));
					literals.Clear();
					unknownBuilder = false;
				}
				else if (callee is ConstructorInfo && typeof(DbConnection).IsAssignableFrom(callee.DeclaringType))
				{
					literals.Clear();
					unknownBuilder = false;
				}
				else if (IsSqlBuilder(callee, method))
				{
					var builderLiterals = SqlBuilderLiterals(callee, new HashSet<MethodBase>(), blindSpots);
					unknownBuilder |= builderLiterals == null;

					if (builderLiterals != null)
						literals.AddRange(builderLiterals);
				}
			}

			return commands;
		}

		private static bool IsSqlBuilder(MethodBase callee, MethodBase owner)
		{
			return callee is MethodInfo method && method.ReturnType == typeof(string) &&
				callee.Module.Assembly == owner.Module.Assembly;
		}

		private static IReadOnlyList<string> SqlBuilderLiterals(MethodBase method, ISet<MethodBase> visited, ISet<string> blindSpots)
		{
			if (!visited.Add(method))
				return null;

			try
			{
				var literals = new List<string>();
				var unsupportedFlow = false;

				foreach (var callee in CalleesOf(method, blindSpots, literals.Add, () => unsupportedFlow = true))
				{
					if (!IsSqlBuilder(callee, method))
						continue;

					var nestedLiterals = SqlBuilderLiterals(callee, visited, blindSpots);

					if (nestedLiterals == null)
						return null;

					literals.AddRange(nestedLiterals);
				}

				return unsupportedFlow || literals.Count == 0 ? null : literals;
			}
			finally
			{
				visited.Remove(method);
			}
		}

		private static SqlCommandKind ClassifySql(IEnumerable<string> literals)
		{
			var sql = string.Join(" ", literals);
			const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

			if (Regex.IsMatch(sql, @"\bEXEC(?:UTE)?\b", options))
				return SqlCommandKind.Unknown;

			var operations = Regex.Matches(sql, @"\b(CREATE|ALTER|DROP|EXEC(?:UTE)?|INSERT|UPDATE|DELETE|TRUNCATE)\s+([A-Z]+)\b", options)
				.Cast<Match>().ToArray();

			if (operations.Any(operation =>
				!Regex.IsMatch(operation.Groups[1].Value, @"^(CREATE|ALTER|DROP)$", options) ||
				!Regex.IsMatch(operation.Groups[2].Value, @"^(DATABASE|TABLE|SCHEMA|CONSTRAINT)$", options)))
				return SqlCommandKind.Unknown;

			if (operations.Any(operation => string.Equals(operation.Groups[2].Value, "DATABASE", StringComparison.OrdinalIgnoreCase)))
				return SqlCommandKind.Database;

			if (operations.Length > 0)
				return SqlCommandKind.Schema;

			return Regex.IsMatch(sql, @"\bSELECT\b", options) ? SqlCommandKind.Read : SqlCommandKind.Unknown;
		}

		private static class DatabaseDdlControls
		{
			internal static void Reset(DbCommand command)
			{
				command.CommandText = "DROP DATABASE [control]";
				command.ExecuteNonQuery();
			}

			internal static void ResetTables(DbCommand command)
			{
				command.CommandText = "ALTER TABLE [control] DROP CONSTRAINT [fk]; DROP TABLE [control]";
				command.ExecuteNonQuery();
			}

			internal static void FromBuilder(DbCommand command)
			{
				command.CommandText = DatabaseCommand();
				command.ExecuteNonQuery();
			}

			private static string DatabaseCommand()
			{
				return "CREATE DATABASE [control]; ALTER DATABASE [control] SET MULTI_USER";
			}

			internal static void Read(DbCommand command)
			{
				command.CommandText = "SELECT 1";
				command.ExecuteScalar();
			}

			internal static void Unknown(DbCommand command, string statement)
			{
				command.CommandText = statement;
				command.ExecuteNonQuery();
			}

			internal static void Mixed(DbCommand command, string statement)
			{
				command.CommandText = "DROP TABLE [control]";
				command.ExecuteNonQuery();
				command.CommandText = statement;
				command.ExecuteNonQuery();
			}

			internal static void Alternative(DbCommand command, string statement, bool known)
			{
				if (known)
					command.CommandText = "DROP TABLE [control]";
				else
					command.CommandText = statement;

				command.ExecuteNonQuery();
			}

			internal static void FromBranchingBuilder(DbCommand command, string statement, bool known)
			{
				command.CommandText = BranchingCommand(statement, known);
				command.ExecuteNonQuery();
			}

			private static string BranchingCommand(string statement, bool known)
			{
				if (known)
					return "DROP TABLE [control]";

				return statement;
			}

			internal static void FromNestedBuilder(DbCommand command, string statement, bool known)
			{
				command.CommandText = NestedCommand(statement, known);
				command.ExecuteNonQuery();
			}

			private static string NestedCommand(string statement, bool known)
			{
				return "DROP TABLE [outer]; " + BranchingCommand(statement, known);
			}

			internal static void FromUnresolvedBuilder(DbCommand command, string statement)
			{
				command.CommandText = "DROP TABLE [control]; " + UnresolvedCommand(statement);
				command.ExecuteNonQuery();
			}

			private static string UnresolvedCommand(string statement)
			{
				return statement;
			}

			internal static void FromCyclicBuilder(DbCommand command)
			{
				command.CommandText = CyclicCommand();
				command.ExecuteNonQuery();
			}

			private static string CyclicCommand()
			{
				return "DROP TABLE [control]; " + CyclicCommand();
			}

			internal static void ReadWithDynamicExecution(DbCommand command)
			{
				command.CommandText = "SELECT 1; EXEC(@statement)";
				command.ExecuteScalar();
				command.CommandText = "SELECT 1; execute @statement";
				command.ExecuteScalar();
			}

			internal static void SchemaWithDynamicExecution(DbCommand command)
			{
				command.CommandText = "DROP TABLE [control]; eXeC(@statement)";
				command.ExecuteNonQuery();
				command.CommandText = "DROP TABLE [control]; EXEC @statement";
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Both guards on <c>Store.Configure</c>. Neither is exotic, and both were deletable without a single case
		/// in this suite failing - which is the only reason they are written down.
		/// </summary>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldRefuseToConfigureAStoreThatWasDisposed()
		{
			//setup
			var store = OpenStore(nameof(ShouldRefuseToConfigureAStoreThatWasDisposed));

			var identity = IdentityOf(store);

			store.Dispose();

			//act
			//assert
			Should.Throw<ObjectDisposedException>(
				() => ConfigureOnto(store, new DbContextOptionsBuilder<ProbeContext>()),
				$"A builder was configured against store '{identity}' after it was disposed. On SQL Server the " +
				"database is gone, so the context that follows fails somewhere far from the mistake - and on SQLite " +
				"naming a released in-memory store silently brings an empty one back, which is worse: the test goes " +
				"green having read nothing." + WhatACleanupFailureMeans);
		}

		[Fact]
		[Trait("Area", "StoreCleanup")]
		public void ShouldRefuseANullBuilder()
		{
			//setup
			using (var store = OpenStore(nameof(ShouldRefuseANullBuilder)))
			{
				//act
				//assert
				Should.Throw<ArgumentNullException>(() => ConfigureOnto(store, null),
					"Store.Configure accepted a null builder. It would throw anyway, one frame further in and " +
					"without naming the parameter." + WhatACleanupFailureMeans);
			}

			Should.Throw<ArgumentNullException>(() => ConfigureExistingOnto("EFToolsTest_nothing", null),
				$"{SeamTypeName}.ConfigureExisting accepted a null builder." + WhatACleanupFailureMeans);
		}

		/// <summary>
		/// What an identity has to be for the rest of this region to hold: inside the droppable namespace, made of
		/// characters that cannot mean anything to the server, and short enough to be a name on it.
		/// </summary>
		/// <remarks>
		/// The label is hostile on purpose. It arrives from <c>nameof</c> at every real call site, so nothing today
		/// carries a quote or a bracket — which is exactly why the sanitisation is untested and why deleting it
		/// would be invisible. The length cap matters for a plainer reason: SQL Server refuses a database name over
		/// 128 characters outright, so an uncapped label turns one long <c>nameof</c> into a store that cannot be
		/// created and a failure nowhere near the seam.
		/// </remarks>
		[Fact]
		[Trait("Area", "StoreCleanup")]
		[Trait("Execution", "LocalPhysicalLifecycle")]
		public void ShouldNameEveryStoreInsideTheDroppableNamespace()
		{
			//setup
			var hostile = new string('x', 200) + "; DROP DATABASE [ProphetsWay.Example]--";

			//act
			using (var store = OpenStore(hostile))
			{
				var identity = IdentityOf(store);

				//assert
				IsDisposableIdentity(identity).ShouldBeTrue(
					$"OpenStore produced identity '{identity}', which its own drop predicate refuses. The store " +
					"would then be created and never removed, and the sweep would never see it either - the two " +
					"failures cancel into a green run and a growing instance." + WhatACleanupFailureMeans);

				identity.Length.ShouldBeLessThanOrEqualTo(128,
					$"Identity '{identity}' is {identity.Length} characters. SQL Server refuses a database name " +
					"longer than 128, so the label has to be capped rather than trusted." + WhatACleanupFailureMeans);

				var unsafeCharacters = new string(identity
					.Where(c => c > 127 || !(char.IsLetterOrDigit(c) || c == '_'))
					.Distinct()
					.ToArray());

				unsafeCharacters.ShouldBeEmpty(
					$"Identity '{identity}' contains [{unsafeCharacters}]. The label reaching OpenStore is a " +
					"caller's string, and the identity built from it is interpolated into a DROP DATABASE " +
					"statement; restricting it to letters, digits and underscore is what makes that safe " +
					"regardless of what a future call site passes." + WhatACleanupFailureMeans);
			}
		}

		#endregion
	}

	/// <summary>
	/// The one cleanup call every store-backed fixture in this assembly makes, and the reason it is not simply a
	/// <c>using</c>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <c>using</c> discards the outcome of disposal, and <c>TestStore.Store.Dispose</c> catches everything —
	/// rightly, because a failed drop must not replace the assertion failure that caused it to run. The two
	/// together mean a run in which every store failed to go away is a run of green tests, which is the finding
	/// this type exists to close: cleanup is re-raised, but <b>only</b> where the body itself succeeded.
	/// </para>
	/// <para>
	/// It reaches the seam by reflection, for the same reason every assertion in
	/// <see cref="ProviderSelectionTests"/> does: naming a member that does not exist yet stops the assembly
	/// compiling and takes three hundred unrelated cases down with it. While
	/// <c>DisposeReportingFailure</c> is missing this degrades to a plain <c>Dispose</c> and reports nothing —
	/// <see cref="ProviderSelectionTests.ShouldReportACleanupFailureRatherThanSwallowingIt"/> is the assertion that
	/// says so.
	/// </para>
	/// </remarks>
	internal static class TestStoreCleanup
	{
		/// <summary>
		/// Disposes <paramref name="store"/> and hands back whatever its cleanup failed with, or <c>null</c>.
		/// </summary>
		internal static Exception DisposeReportingFailure(IDisposable store)
		{
			if (store == null)
				return null;

			var reporting = store.GetType().GetMethod(
				"DisposeReportingFailure",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				Type.EmptyTypes,
				null);

			if (reporting == null)
			{
				store.Dispose();

				return null;
			}

			try
			{
				return reporting.Invoke(store, null) as Exception;
			}
			catch (TargetInvocationException ex)
			{
				return ex.InnerException;
			}
		}

		/// <summary>
		/// The <c>finally</c> half of a <c>WithStore</c> helper. <paramref name="bodySucceeded"/> is the flag set
		/// on the last line of the <c>try</c>: false means an assertion or the store itself already failed, and
		/// that failure is the finding — a cleanup failure raised over it would replace a defect with its
		/// consequence.
		/// </summary>
		internal static void DisposeReportingFailure(IDisposable store, bool bodySucceeded)
		{
			var failure = DisposeReportingFailure(store);

			if (failure == null || !bodySucceeded)
				return;

			throw new InvalidOperationException(
				"The test body succeeded but its store could not be cleaned up, so this run has left a database " +
				"behind on the server. The test itself is not the defect - the cleanup is, and it is reported here " +
				"rather than swallowed so it is attributed to the test that caused it instead of being found later " +
				"by a sweep. See ProviderSelectionTests.ShouldLeaveNoDisposableStoreBehindOnTheServer.",
				failure);
		}
	}
}
