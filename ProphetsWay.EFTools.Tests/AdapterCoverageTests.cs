using System;
using System.Collections.Generic;
using System.Linq;

using ProphetsWay.Example.Tests;
using ProphetsWay.Example.Tests.ConventionShowcase;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// Guards the thirteen <c>EF*Tests</c> adapters as a <i>set</i>: an upstream test class that gains no adapter
	/// here never runs against Entity Framework, and nothing else in this repository would turn red to say so.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The mapping is hand-maintained and the submodule pointer moves. When it advances onto an upstream revision
	/// carrying a new test class, the compiler is content, the run is green, and that class silently never
	/// exercises this repository's implementation - the same false green <see cref="TestSeamTests"/> exists to
	/// prevent, arriving through the front door instead.
	/// </para>
	/// <para>
	/// <b>The exclusion is the hard part, not the comparison.</b> The classes under
	/// <c>ProphetsWay.Example.Tests.ConventionShowcase</c> correctly have no adapter and must never gain one:
	/// each builds its own deliberately mis-wired Data Access Layer to demonstrate a convention failure, so it is
	/// the subject of its test rather than the implementation under test, and
	/// <see cref="TestDataAccessFactory"/>'s own remarks say those layers "do not come from here, and must not".
	/// Adapting one would run this repository's Entity Framework layer through a test that was never about an
	/// implementation at all. <see cref="ShouldExcludeTheConventionShowcaseFromAdaptation"/> pins that exclusion
	/// so a later loosening of the predicate below fails here rather than quietly recruiting them.
	/// </para>
	/// <para>
	/// <b>Trait key.</b> <c>Guard</c>/<c>Seam</c> for the same reasons set out on <see cref="TestSeamTests"/>,
	/// and the same value so one gate expression - <c>--filter "Scope=Contract|Guard=Seam"</c> - covers both.
	/// No <c>[Collection]</c>: nothing here constructs a Data Access Layer or touches the store.
	/// </para>
	/// </remarks>
	[Trait("Guard", "Seam")]
	public class AdapterCoverageTests
	{
		/// <summary>
		/// The upstream classes that run against whatever <see cref="TestDataAccessFactory"/> hands out, and
		/// therefore the ones this assembly has to adapt.
		/// </summary>
		/// <remarks>
		/// Two criteria, unioned, because either one alone has a blind spot. Deriving from
		/// <see cref="BaseUnitTests{T}"/> is the direct statement that a class consumes the factory, but a future
		/// upstream class could call <see cref="TestDataAccessFactory.Create"/> itself without deriving from
		/// anything. <c>[Collection]</c> is upstream's own marker for "this class writes to the one shared store",
		/// which cannot be true of a class that does not obtain a Data Access Layer from the factory. Both
		/// criteria are documented design rather than incidental shape, and the showcase classes fail both.
		/// </remarks>
		private static IEnumerable<Type> AdaptableUpstreamTestClasses()
		{
			return typeof(BaseUnitTests<>).Assembly
				.GetExportedTypes()
				.Where(t => t.IsClass && !t.IsAbstract)
				.Where(t => DerivesFromBaseUnitTests(t) || t.GetCustomAttributes(typeof(CollectionAttribute), false).Length > 0);
		}

		private static bool DerivesFromBaseUnitTests(Type type)
		{
			for (var current = type.BaseType; current != null; current = current.BaseType)
			{
				if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(BaseUnitTests<>))
					return true;
			}

			return false;
		}

		[Fact]
		public void ShouldAdaptEveryUpstreamTestClassThatUsesTheFactory()
		{
			//setup
			var adapted = typeof(TestSeam).Assembly
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract)
				.Select(t => t.BaseType)
				.Where(t => t != null)
				.ToList();

			//act
			var unadapted = AdaptableUpstreamTestClasses()
				.Where(t => !adapted.Contains(t))
				.Select(t => t.FullName)
				.OrderBy(name => name, StringComparer.Ordinal)
				.ToList();

			//assert
			unadapted.ShouldBeEmpty(
				"These upstream test classes have no EF*Tests adapter in this assembly, so xUnit never discovers " +
				"them here and they never run against this repository's Entity Framework Data Access Layer:\n  " +
				string.Join("\n  ", unadapted) +
				"\n\nThis is what happens when the ProphetsWay.Example submodule pointer advances onto a revision " +
				"carrying a new test class: nothing fails to compile and nothing turns red, so the suite reports a " +
				"green run over a specification it is no longer covering in full. Add one adapter per class named " +
				"above - 'public class EFXxxTests : XxxTests { }' and nothing else - or, if a class genuinely must " +
				"not be adapted, say why here rather than widening the exclusion silently.");
		}

		[Fact]
		public void ShouldExcludeTheConventionShowcaseFromAdaptation()
		{
			//setup
			const string message =
				"A ConventionShowcase class was classified as needing an adapter. It must not be adapted: those " +
				"classes construct their own deliberately mis-wired Data Access Layers to demonstrate convention " +
				"failures, take nothing from TestDataAccessFactory, and reach no store. Adapting one would run this " +
				"repository's Entity Framework layer through a test that was never about an implementation. The " +
				"predicate in " + nameof(AdaptableUpstreamTestClasses) + " has been loosened - narrow it again " +
				"rather than adding the adapter it is asking for.";

			//act
			var adaptable = AdaptableUpstreamTestClasses().ToList();

			//assert
			adaptable.ShouldNotContain(typeof(ConventionShowcaseTests), message);
			adaptable.ShouldNotContain(typeof(ExceptionPassthroughShowcaseTests), message);
		}
	}
}
