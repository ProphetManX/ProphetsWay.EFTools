using System;

using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.EF;
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
	public class TestSeamTests
	{
		[Fact]
		public void ShouldHandOutTheEntityFrameworkDataAccessLayer()
		{
			//setup
			//act
			using (var da = TestDataAccessFactory.Create())
			{
				//assert
				da.ShouldBeOfType<EFExampleDataAccess>();
			}
		}

		[Fact]
		public void ShouldHandOutADataAccessLayerBackedByEntityFrameworkCore()
		{
			//setup
			//act
			using (var da = TestDataAccessFactory.Create())
			{
				//assert - the in-memory implementation satisfies IExampleDataAccess but not this
				da.ShouldBeAssignableTo<BaseEFDataAccess<ExampleContext>>();
			}
		}

		[Fact]
		public void ShouldHandOutTheSameImplementationThroughCreateAs()
		{
			//setup
			//act - the path every BaseUnitTests<T> derived class takes
			var dao = TestDataAccessFactory.CreateAs<ICompanyDao>();

			//assert
			using ((IDisposable)dao)
				dao.ShouldBeOfType<EFExampleDataAccess>();
		}
	}
}
