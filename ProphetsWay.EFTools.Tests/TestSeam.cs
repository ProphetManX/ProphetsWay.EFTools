using System.Runtime.CompilerServices;

using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// Points the upstream <c>ProphetsWay.Example</c> suite at the Entity Framework Data Access Layer for the
	/// whole of this assembly's run.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Without this file the adapter classes in this project discover the upstream suite and run it against the
	/// in-memory <c>ProphetsWay.Example.DataAccess.NoDB.ExampleDataAccess</c> — the upstream default — and a
	/// green run proves nothing whatsoever about Entity Framework. <see cref="TestSeamTests"/> is the assertion
	/// that this file is still doing its job; the upstream factory cannot write that assertion because it does
	/// not know this repository's type.
	/// </para>
	/// <para>
	/// A <c>[ModuleInitializer]</c> rather than a fixture because xUnit runs collections in parallel and a
	/// fixture is constructed after another collection may already have built a Data Access Layer. See the
	/// THREAD SAFETY remarks on <see cref="TestDataAccessFactory.Use"/>.
	/// </para>
	/// <para>
	/// <b>The second argument declares the shape of the store, and what it leaves out declares as much as what
	/// it names.</b> A normalized relational store takes transaction isolation from its provider, so
	/// <see cref="StoreCapabilities.TransactionIsolation"/> is stated. It cannot hold a second, denormalized
	/// copy of a navigation node — one Companies row, read back through a join, is the same data the entity's
	/// view of that company is — so <see cref="StoreCapabilities.DenormalizedNavigationWrites"/> is absent, and
	/// its absence is a positive declaration rather than an omission. Both upstream tests that read this branch
	/// on the one value, and the branch the missing flag selects is the stronger of the two: it asserts that a
	/// navigation property reads back with its <i>committed</i> name, which proves the Entity Framework
	/// <c>Update</c> did not cascade into the Company, Job and Department rows the caller never named. Adding
	/// the flag would assert the in-memory outcome against a relational store and fail.
	/// </para>
	/// <para>
	/// <b>Declaring isolation is also what removes this run's dominant cost.</b> Undeclared, the transaction
	/// test performs a read of an uncommitted row that blocks on a SQL Server lock until the writing
	/// transaction ends and then dies on the command timeout — thirty seconds of a thirty-two second run, spent
	/// failing an assertion no relational store was ever obliged to satisfy. The upstream branch is around the
	/// read rather than around the assertion, so it is the declaration and not a skip that avoids the wait.
	/// Neither flag is a way out of a rule: no <c>Scope=Contract</c> assertion reads either of them, and the
	/// limits on what a declaration here may ever excuse are on <see cref="StoreCapabilities"/>.
	/// </para>
	/// </remarks>
	internal static class TestSeam
	{
		[ModuleInitializer]
		internal static void PointTheSuiteAtEntityFramework()
		{
			// Constants owns the connection string; its ExampleDataAccess is ProphetsWay.Example.DataAccess.EF's,
			// not the identically named NoDB one. TestSeamTests pins that by full type name.
			TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess, StoreCapabilities.TransactionIsolation);
		}
	}
}
