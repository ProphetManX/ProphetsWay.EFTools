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
	/// </remarks>
	internal static class TestSeam
	{
		[ModuleInitializer]
		internal static void PointTheSuiteAtEntityFramework()
		{
			// Constants owns the connection string; its ExampleDataAccess is ProphetsWay.Example.DataAccess.EF's,
			// not the identically named NoDB one. TestSeamTests pins that by full type name.
			TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess);
		}
	}
}
