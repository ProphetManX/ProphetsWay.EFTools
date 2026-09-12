using System;

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// A8 identifier resolution and the A17 constructor contract, on this library's own <c>GetKey</c> /
	/// <c>KeyEquals</c> path rather than on the parent dispatcher's.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>{TypeName}Id</c>-then-<c>Id</c> order and the public-property requirement are already pinned upstream
	/// for <c>BaseDataAccess.Get&lt;T&gt;(object)</c>. They are pinned again here because A8 requires the two to
	/// agree, and a divergence would be invisible: <c>dal.Get&lt;T&gt;(id)</c> and <c>dao.Get(item)</c> would
	/// silently address different columns on an entity carrying both properties.
	/// </para>
	/// <para>
	/// <b>The failure-case entities are deliberately absent from the context's model.</b> A17 step 3 forbids the
	/// constructor from calling <c>Set&lt;TEntity&gt;()</c>, reading <c>Context.Model</c>, or initializing the
	/// model at all, so an unmapped entity is the arrangement that proves the exception came from the convention
	/// check rather than from EF Core. An implementation that resolves <c>Dataset</c> eagerly fails these with
	/// <c>InvalidOperationException</c>, which is the correct outcome for that implementation.
	/// </para>
	/// </remarks>
	public class IdentifierResolutionTests
	{
		#region fixture entities

		/// <summary>Carries both candidate properties. <c>WidgetId</c> must win — A8 step 1 before step 2.</summary>
		public class Widget : IBaseIdEntity<string>
		{
			public string WidgetId { get; set; }

			public string Id { get; set; }

			public string Name { get; set; }
		}

		/// <summary>
		/// Satisfies <see cref="IBaseIdEntity{T}"/> by an explicit implementation only, so the reflected surface
		/// carries no property named <c>Id</c> at all — the interface-qualified name is what is emitted.
		/// </summary>
		public class ExplicitKeyed : IBaseIdEntity<string>
		{
			string IBaseIdEntity<string>.Id { get; set; }

			public string Name { get; set; }
		}

		/// <summary>
		/// The other half of the same trap, and a different metadata shape: a property literally named <c>Id</c>
		/// exists and is found by name, but is not public. A resolver that binds non-public properties and forgets
		/// the visibility check passes <see cref="ExplicitKeyed"/> and fails here.
		/// </summary>
		public class HiddenKeyed : IBaseIdEntity<string>
		{
			internal string Id { get; set; }

			string IBaseIdEntity<string>.Id
			{
				get { return Id; }
				set { Id = value; }
			}

			public string Name { get; set; }
		}

		#endregion

		#region fixture context and data access objects

		public class IdentifierResolutionContext : DbContext
		{
			public IdentifierResolutionContext(DbContextOptions<IdentifierResolutionContext> options) : base(options)
			{
			}

			public DbSet<Widget> Widgets { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				var widget = modelBuilder.Entity<Widget>();
				widget.HasKey(x => x.WidgetId);
			}
		}

		public class WidgetDao : BaseDao<Widget, string>
		{
			public WidgetDao(DbContext context) : base(context)
			{
			}
		}

		public class ExplicitKeyedDao : BaseDao<ExplicitKeyed, string>
		{
			public ExplicitKeyedDao(DbContext context) : base(context)
			{
			}
		}

		public class HiddenKeyedDao : BaseDao<HiddenKeyed, string>
		{
			public HiddenKeyedDao(DbContext context) : base(context)
			{
			}
		}

		#endregion

		#region apparatus

		private static void WithStore(Action<Func<IdentifierResolutionContext>> body)
		{
			// Held for the whole body: disposing the store discards the database every context here reads.
			var store = TestStore.OpenStore(nameof(IdentifierResolutionTests));
			var completed = false;

			try
			{
				var builder = new DbContextOptionsBuilder<IdentifierResolutionContext>();

				store.Configure(builder);

				var options = builder.Options;

				Func<IdentifierResolutionContext> factory = () => new IdentifierResolutionContext(options);

				using (var schema = factory())
					schema.Database.EnsureCreated();

				body(factory);

				completed = true;
			}
			finally
			{
				// A store this run failed to drop is a database left on the server, so it is raised here - but only
				// over a body that otherwise passed, or cleanup replaces the finding with its consequence.
				TestStoreCleanup.DisposeReportingFailure(store, completed);
			}
		}

		/// <summary>
		/// A context that never opens a connection. The three constructor obligations must not need a database,
		/// and using one would hide an implementation that touched the store during construction.
		/// </summary>
		/// <remarks>
		/// The store is a parameter rather than a local because the caller has to outlive the context: nothing here
		/// materialises the database, but the seam still owns whatever naming one provisioned, and disposing it
		/// before the assertions run would leave the context pointed at a store that had already been taken away.
		/// </remarks>
		private static IdentifierResolutionContext UnopenedContext(TestStore.Store store)
		{
			var builder = new DbContextOptionsBuilder<IdentifierResolutionContext>();

			store.Configure(builder);

			return new IdentifierResolutionContext(builder.Options);
		}

		/// <summary>
		/// The two rows are arranged so the two candidate properties disagree about which row is addressed:
		/// resolving <c>WidgetId</c> returns the first, resolving <c>Id</c> returns the second.
		/// </summary>
		private static void Setup_TwoWidgetsWhoseCandidatePropertiesDisagree(Func<IdentifierResolutionContext> factory)
		{
			using (var context = factory())
			{
				context.Widgets.Add(new Widget { WidgetId = "A", Id = "Z", Name = "addressed by WidgetId" });
				context.Widgets.Add(new Widget { WidgetId = "Q", Id = "A", Name = "addressed by Id" });
				context.SaveChanges();
			}
		}

		#endregion

		#region A8 — resolution order

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldResolveTypeNameIdAheadOfId()
		{
			WithStore(factory =>
			{
				//setup
				Setup_TwoWidgetsWhoseCandidatePropertiesDisagree(factory);

				using (var context = factory())
				{
					var dao = new WidgetDao(context);

					//act
					var found = dao.Get(new Widget { WidgetId = "A", Id = "A" });

					//assert
					found.ShouldNotBeNull();
					found.Name.ShouldBe("addressed by WidgetId");
				}
			});
		}

		#endregion

		#region A8 — the identifier property must be public, and the failure lands in the constructor

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldThrowConventionExceptionFromTheConstructorWhenTheIdentifierIsAnExplicitInterfaceImplementation()
		{
			//setup
			var store = TestStore.OpenStore(nameof(IdentifierResolutionTests));
			var completed = false;

			try
			{
				using (var context = UnopenedContext(store))
				{
					//act
					var thrown = Record.Exception(() => new ExplicitKeyedDao(context));

					//assert
					thrown.ShouldBeOfType<DataAccessConventionException>();
				}

				completed = true;
			}
			finally
			{
				// Same rule as WithStore: a store this run failed to drop is raised here, but only over a body that
				// otherwise passed.
				TestStoreCleanup.DisposeReportingFailure(store, completed);
			}
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldThrowConventionExceptionFromTheConstructorWhenTheIdentifierPropertyIsNotPublic()
		{
			//setup
			var store = TestStore.OpenStore(nameof(IdentifierResolutionTests));
			var completed = false;

			try
			{
				using (var context = UnopenedContext(store))
				{
					//act
					var thrown = Record.Exception(() => new HiddenKeyedDao(context));

					//assert
					thrown.ShouldBeOfType<DataAccessConventionException>();
				}

				completed = true;
			}
			finally
			{
				TestStoreCleanup.DisposeReportingFailure(store, completed);
			}
		}

		#endregion

		#region A17 — the null-context check runs first

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldThrowArgumentNullExceptionRatherThanConventionExceptionWhenTheContextIsNull()
		{
			//setup
			// ExplicitKeyed is mis-wired, so an implementation that validated the convention first would throw
			// DataAccessConventionException here and tell the caller about a rule they did not break.

			//act
			var thrown = Record.Exception(() => new ExplicitKeyedDao(null));

			//assert
			thrown.ShouldBeOfType<ArgumentNullException>();
		}

		#endregion
	}
}
