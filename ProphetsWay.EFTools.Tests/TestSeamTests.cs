using System.Reflection;

using Microsoft.EntityFrameworkCore;

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.EF;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

using Shouldly;

using Xunit;

using EFExampleDataAccess = ProphetsWay.Example.DataAccess.EF.ExampleDataAccess;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// Guards <see cref="TestSeam"/>: if the module initializer is ever removed, broken, or silently reverted,
	/// these fail rather than letting the whole upstream suite pass against the in-memory Data Access Layer.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>This class derives from <see cref="BaseUnitTests{T}"/> on purpose.</b> A guard that called
	/// <see cref="TestDataAccessFactory"/> directly would interrogate the factory while every adapted test
	/// reads <c>_da</c>, and nothing would bind the two. Upstream's own remarks record an overridable
	/// <c>CreateDataAccess</c> hook on <see cref="BaseUnitTests{T}"/> as considered-and-rejected, which means
	/// revisitable; were it reintroduced, <see cref="BaseUnitTests{T}"/> would stop consulting the factory, a
	/// factory-interrogating guard would stay green, and the whole suite would revert to the in-memory
	/// implementation in silence. Reading <c>_da</c> - the identical field, filled by the identical constructor
	/// - is the only assertion that cannot come apart from what the suite actually runs against.
	/// </para>
	/// <para>
	/// <b><c>[Trait("Guard", "Seam")]</c> rather than upstream's <c>Scope</c> key.</b> xUnit trait filters are
	/// allowlists, so an untraited class is excluded from <c>--filter "Scope=Contract"</c> - the documented
	/// conformance gate - and this guard would never run on the one path a human actually invokes. It must not
	/// take a <c>Scope</c> value either: <c>Scope</c> partitions upstream's specification, a downstream
	/// assembly has no business extending it, and none of <c>Contract</c>, <c>Characterization</c> or
	/// <c>Dispatcher</c> describes a wiring guard. An orthogonal key leaves that partition untouched and makes
	/// the gate <c>--filter "Scope=Contract|Guard=Seam"</c>. <see cref="BaseUnitTests{T}"/> carries no trait of
	/// its own, so deriving from it pulls nothing into a <c>Scope</c> value by the back door.
	/// </para>
	/// <para>
	/// <b><c>[Collection]</c> matching the adapters.</b> Two of these tests reach the store, so without it they
	/// would run in parallel with the thirteen adapters and race the whole-set count assertions in
	/// <c>DataAccessTransactionTests</c>.
	/// </para>
	/// </remarks>
	[Collection(TestCollections.SharedStore)]
	[Trait("Guard", "Seam")]
	public class TestSeamTests : BaseUnitTests<ICompanyDao>
	{
		/// <summary>
		/// Why a red result here is worth more than the large green mass that deleting <c>TestSeam.cs</c> would
		/// produce. Carried on every assertion in this class, because a maintainer meeting a handful of confusing
		/// failures beside a hundred passes will otherwise reach the wrong conclusion about which to delete.
		/// </summary>
		private const string WhatThisFailureMeans =
			"\n\nWHAT THIS MEANS: the upstream ProphetsWay.Example suite is not running against the Entity Framework " +
			"Data Access Layer. TestDataAccessFactory's default is the in-memory NoDB implementation, so the likely " +
			"cause is that TestSeam.cs has been deleted, its [ModuleInitializer] attribute removed, or " +
			"Constants.GetExampleDataAccess changed to hand back something else. " +
			"EVERY GREEN RESULT IN THIS RUN IS THEREFORE MEANINGLESS: the thirteen EF*Tests adapters carry no test " +
			"logic of their own and exist solely to run the upstream suite against Entity Framework, so they will " +
			"pass in bulk against an implementation that has nothing to do with this repository. " +
			"Fix the seam. Deleting this test is how the false green gets back in.";

		/// <summary>
		/// <c>BaseEFDataAccess&lt;TContext&gt;.Context</c> is <c>protected</c>, and deliberately so - the context
		/// is not part of the Data Access Layer's public surface. Reflection is therefore the only way to reach
		/// the provider from outside the class, and the provider is the one fact that separates a real relational
		/// store from an in-memory imitation of one.
		/// </summary>
		private static DbContext ContextOf(object dataAccess)
		{
			//not the assertion, just enough of a check that a non-EF layer fails here with a sentence rather than
			//with a reflection exception naming no cause
			var layer = dataAccess as BaseEFDataAccess<ExampleContext>;

			layer.ShouldNotBeNull(
				$"The Data Access Layer is a {dataAccess.GetType().FullName}, which is not an Entity Framework layer " +
				"at all, so it has no provider to inspect." + WhatThisFailureMeans);

			var property = typeof(BaseEFDataAccess<ExampleContext>)
				.GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic);

			property.ShouldNotBeNull(
				"BaseEFDataAccess<TContext>.Context could not be found by reflection, so this guard can no longer " +
				"tell a relational provider from an in-memory one. The member was renamed or removed; re-point this " +
				"helper at whatever replaced it rather than deleting the assertion." + WhatThisFailureMeans);

			return (DbContext)property.GetValue(layer);
		}

		[Fact]
		public void ShouldHandOutTheEntityFrameworkDataAccessLayer()
		{
			//setup
			//act - _da is filled by BaseUnitTests<T>'s constructor, the identical path every adapted test takes

			//assert
			_da.ShouldBeOfType<EFExampleDataAccess>(
				"The Data Access Layer handed to a BaseUnitTests<T> derived class is not " +
				"ProphetsWay.Example.DataAccess.EF.ExampleDataAccess. Note that the in-memory implementation carries " +
				"the same short type name, so read the namespace in the failure above before concluding otherwise." +
				WhatThisFailureMeans);
		}

		[Fact]
		public void ShouldHandOutADataAccessLayerOnARelationalProvider()
		{
			//setup
			//act
			var context = ContextOf(_da);

			//assert - Entity Framework backing is not the claim; a relational store is
			context.Database.IsRelational().ShouldBeTrue(
				$"The Entity Framework provider in use is '{context.Database.ProviderName}', which is not relational. " +
				"Microsoft.EntityFrameworkCore.InMemory is on this project's compile path transitively through the " +
				"library, and one edit to Constants.cs pointing ExampleContext at it would leave every type check in " +
				"this class green while proving nothing about SQL translation, foreign keys, identity generation or " +
				"transactions - none of which that provider implements. That is the original false green reproduced " +
				"one abstraction up, which is why this assertion is on the provider rather than on a type." +
				WhatThisFailureMeans);
		}

		[Fact]
		public void ShouldHandOutTheEntityFrameworkDataAccessLayerThroughCreate()
		{
			//setup
			//act - the aggregate path, taken by the upstream tests that build a second reader instance
			using (var da = TestDataAccessFactory.Create())
			{
				//assert
				da.ShouldBeOfType<EFExampleDataAccess>(
					"TestDataAccessFactory.Create() - the path upstream tests take when they need a second reader " +
					"instance alongside _da - did not return the Entity Framework Data Access Layer." +
					WhatThisFailureMeans);
			}
		}

		[Fact]
		public void ShouldHandOutAFreshInstanceForEveryRequest()
		{
			//setup
			const string message =
				"TestDataAccessFactory handed out the same Data Access Layer instance twice. Its contract states the " +
				"delegate is 'called once per instance rather than once in total'. The likely cause is one character " +
				"in Constants.cs: GetExampleDataAccess is an expression-bodied property ('=>'), and changing it to a " +
				"field initialiser ('=') makes every caller share one instance. That is not a slow leak - " +
				"BaseUnitTests<T> disposes _da after every test, so the second test onward would receive an already " +
				"disposed layer while a guard made only of type checks reported everything was fine." +
				WhatThisFailureMeans;

			//act
			using (var first = TestDataAccessFactory.Create())
			using (var second = TestDataAccessFactory.Create())
			{
				//assert
				first.ShouldNotBeSameAs(second, message);
				first.ShouldNotBeSameAs(_da, message);
				second.ShouldNotBeSameAs(_da, message);
			}
		}

		[Fact]
		public void ShouldHandOutAnInstanceThatReachesTheStore()
		{
			//setup - a read, so this guard adds no row the adapters would then have to count around
			//act
			var count = _da.GetCount(new Company());

			//assert - the assertion that matters is that the call came back at all
			count.ShouldBeGreaterThanOrEqualTo(0,
				"A count against the store did not come back. If this threw ObjectDisposedException the factory is " +
				"handing out one shared, already disposed instance - see " +
				nameof(ShouldHandOutAFreshInstanceForEveryRequest) + ". If it threw a provider or login exception, " +
				"the Entity Framework leg has no database to talk to: this suite needs a local SQL Server carrying " +
				"ProphetsWay.Example, published from ProphetsWay.Example.Database. Either way every result below it " +
				"is reporting on nothing." + WhatThisFailureMeans);
		}
	}
}
