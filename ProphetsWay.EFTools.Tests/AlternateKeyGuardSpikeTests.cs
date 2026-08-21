using System;
using System.Linq;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// An empirical spike, not a specification. It measures whether Entity Framework Core's read-only-key guard
	/// fires for ALTERNATE keys or only for PRIMARY keys, on both a relational and a non-relational provider,
	/// through both a direct property assignment and <c>Entry(x).CurrentValues.SetValues(y)</c>.
	/// </summary>
	/// <remarks>
	/// Decision A35 in <c>docs/api-contract.md</c> excludes "primary or alternate" key properties from the copy
	/// an Update performs, reading the exclusion set from <c>IProperty.IsKey()</c>. Whether the alternate half of
	/// that exclusion prevents a real exception or merely drops a legitimate write is the question here. The
	/// assertions state the hypothesis that the guard covers alternate keys; a failure is a result, not a defect.
	/// </remarks>
	public class AlternateKeyGuardSpikeTests
	{
		private const string InMemory = "InMemory";
		private const string Sqlite = "Sqlite";

		private const string OriginalEmail = "original@example.com";
		private const string ChangedEmail = "changed@example.com";

		private readonly ITestOutputHelper _output;

		public AlternateKeyGuardSpikeTests(ITestOutputHelper output)
		{
			_output = output;
		}

		public class SpikeUser
		{
			public int Id { get; set; }

			public string Email { get; set; }

			public string Name { get; set; }
		}

		/// <summary>
		/// Same shape, but <c>Email</c> carries only a unique index - it is not a declared key.
		/// </summary>
		public class SpikeUniqueUser
		{
			public int Id { get; set; }

			public string Email { get; set; }

			public string Name { get; set; }
		}

		public class SpikeContext : DbContext
		{
			public SpikeContext(DbContextOptions<SpikeContext> options) : base(options)
			{
			}

			public DbSet<SpikeUser> Users { get; set; }

			public DbSet<SpikeUniqueUser> UniqueUsers { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				var user = modelBuilder.Entity<SpikeUser>();
				user.HasKey(u => u.Id);
				user.Property(u => u.Email).IsRequired();
				user.HasAlternateKey(u => u.Email);

				// The control for "declared alternate key" - uniqueness without keyhood.
				var unique = modelBuilder.Entity<SpikeUniqueUser>();
				unique.HasKey(u => u.Id);
				unique.Property(u => u.Email).IsRequired();
				unique.HasIndex(u => u.Email).IsUnique();
			}
		}

		private sealed class Observation
		{
			public Exception AtAssignment { get; set; }

			public Exception AtDetectChanges { get; set; }

			public string StateAfter { get; set; }

			public string IdAfter { get; set; }

			public string EmailAfter { get; set; }

			public Exception Thrown => AtAssignment ?? AtDetectChanges;
		}

		private static void WithProvider(string provider, Action<Func<SpikeContext>> body)
		{
			if (provider == Sqlite)
			{
				using (var connection = new SqliteConnection("Filename=:memory:"))
				{
					connection.Open();

					var options = new DbContextOptionsBuilder<SpikeContext>()
						.UseSqlite(connection)
						.Options;

					Func<SpikeContext> factory = () => new SpikeContext(options);

					using (var schema = factory())
						schema.Database.EnsureCreated();

					body(factory);
				}
			}
			else
			{
				// global:: because ProphetsWay.EFTools.Guid shadows System.Guid from inside this namespace.
				var options = new DbContextOptionsBuilder<SpikeContext>()
					.UseInMemoryDatabase(global::System.Guid.NewGuid().ToString())
					.Options;

				body(() => new SpikeContext(options));
			}
		}

		private static int Seed(Func<SpikeContext> factory)
		{
			using (var context = factory())
			{
				var user = new SpikeUser { Email = OriginalEmail, Name = "Original" };
				context.Users.Add(user);
				context.SaveChanges();
				return user.Id;
			}
		}

		private static Observation Observe(Func<SpikeContext> factory, int id, Action<SpikeContext, SpikeUser> act)
		{
			var observation = new Observation();

			using (var context = factory())
			{
				var stored = context.Users.Single(u => u.Id == id);
				context.Entry(stored).State.ShouldBe(EntityState.Unchanged, "the spike needs a tracked, Unchanged entity before it mutates anything");

				observation.AtAssignment = Record.Exception(() => act(context, stored));

				if (observation.AtAssignment == null)
					observation.AtDetectChanges = Record.Exception(() => context.ChangeTracker.DetectChanges());

				observation.StateAfter = Describe(() => context.Entry(stored).State.ToString());
				observation.IdAfter = Describe(() => Convert.ToString(context.Entry(stored).CurrentValues[nameof(SpikeUser.Id)]));
				observation.EmailAfter = Describe(() => Convert.ToString(context.Entry(stored).CurrentValues[nameof(SpikeUser.Email)]));
			}

			return observation;
		}

		private static string Describe(Func<string> read)
		{
			try
			{
				return read();
			}
			catch (Exception ex)
			{
				return $"<unreadable: {ex.GetType().Name}: {ex.Message}>";
			}
		}

		private void Report(string provider, string route, Observation observation)
		{
			_output.WriteLine($"provider          : {provider}");
			_output.WriteLine($"route             : {route}");
			_output.WriteLine($"threw at mutation : {Format(observation.AtAssignment)}");
			_output.WriteLine($"threw at detect   : {Format(observation.AtDetectChanges)}");
			_output.WriteLine($"state after       : {observation.StateAfter}");
			_output.WriteLine($"Id current value  : {observation.IdAfter}");
			_output.WriteLine($"Email current val : {observation.EmailAfter}");
			_output.WriteLine(string.Empty);
		}

		private static string Format(Exception ex)
			=> ex == null ? "(nothing)" : $"{ex.GetType().FullName}: {ex.Message}";

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldThrowWhenPrimaryKeyIsChangedByDirectAssignment(string provider)
		{
			//setup
			Observation observation = null;

			WithProvider(provider, factory =>
			{
				var id = Seed(factory);

				//act
				observation = Observe(factory, id, (context, stored) => stored.Id = stored.Id + 1000);
			});

			//assert
			Report(provider, "direct assignment to the primary key", observation);
			observation.Thrown.ShouldBeOfType<InvalidOperationException>(
				"This is the CONTROL. Entity Framework Core is expected to refuse a primary key change on a tracked, " +
				"Unchanged entity. If nothing was thrown, the whole framing of this spike is wrong.");
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldThrowWhenAlternateKeyIsChangedByDirectAssignment(string provider)
		{
			//setup
			Observation observation = null;

			WithProvider(provider, factory =>
			{
				var id = Seed(factory);

				//act
				observation = Observe(factory, id, (context, stored) => stored.Email = ChangedEmail);
			});

			//assert
			Report(provider, "direct assignment to the alternate key", observation);
			observation.Thrown.ShouldBeOfType<InvalidOperationException>(
				"This is the UNKNOWN, and the assertion states the hypothesis that the read-only-key guard covers " +
				"alternate keys. A failure here means the guard is primary-key-only, and A35's alternate-key " +
				"exclusion prevents no exception.");
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldThrowWhenPrimaryKeyIsChangedBySetValues(string provider)
		{
			//setup
			Observation observation = null;

			WithProvider(provider, factory =>
			{
				var id = Seed(factory);

				//act
				observation = Observe(factory, id, (context, stored) =>
				{
					var incoming = new SpikeUser { Id = id + 1000, Email = OriginalEmail, Name = "Original" };

					// SetValues writes only where the incoming value differs; without this the test proves nothing.
					incoming.Id.ShouldNotBe(stored.Id);
					incoming.Email.ShouldBe(stored.Email);

					context.Entry(stored).CurrentValues.SetValues(incoming);
				});
			});

			//assert
			Report(provider, "SetValues carrying a differing primary key", observation);
			observation.Thrown.ShouldBeOfType<InvalidOperationException>(
				"SetValues is the mechanism A35 actually specifies. If the direct-assignment control throws and this " +
				"does not, the two routes differ and that difference is the finding.");
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldThrowWhenAlternateKeyIsChangedBySetValues(string provider)
		{
			//setup
			Observation observation = null;

			WithProvider(provider, factory =>
			{
				var id = Seed(factory);

				//act
				observation = Observe(factory, id, (context, stored) =>
				{
					var incoming = new SpikeUser { Id = id, Email = ChangedEmail, Name = "Original" };

					// SetValues writes only where the incoming value differs; without this the test proves nothing.
					incoming.Id.ShouldBe(stored.Id);
					incoming.Email.ShouldNotBe(stored.Email);

					context.Entry(stored).CurrentValues.SetValues(incoming);
				});
			});

			//assert
			Report(provider, "SetValues carrying a differing alternate key", observation);
			observation.Thrown.ShouldBeOfType<InvalidOperationException>(
				"This is the case A35 turns on: an Update copying an incoming entity whose natural-key locator " +
				"column has changed. A failure here means excluding the alternate key from the copy costs a silent " +
				"data loss and prevents no exception.");
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldReportTheAlternateKeyPropertyAsAKeyButNotAPrimaryKey(string provider)
		{
			//setup
			bool emailIsKey = false, emailIsPrimaryKey = false, idIsKey = false, idIsPrimaryKey = false;

			WithProvider(provider, factory =>
			{
				using (var context = factory())
				{
					//act
					var entity = context.Model.FindEntityType(typeof(SpikeUser));
					var email = entity.FindProperty(nameof(SpikeUser.Email));
					var identifier = entity.FindProperty(nameof(SpikeUser.Id));

					emailIsKey = email.IsKey();
					emailIsPrimaryKey = email.IsPrimaryKey();
					idIsKey = identifier.IsKey();
					idIsPrimaryKey = identifier.IsPrimaryKey();

					_output.WriteLine($"provider              : {provider}");
					_output.WriteLine($"Email.IsKey()         : {emailIsKey}");
					_output.WriteLine($"Email.IsPrimaryKey()  : {emailIsPrimaryKey}");
					_output.WriteLine($"Id.IsKey()            : {idIsKey}");
					_output.WriteLine($"Id.IsPrimaryKey()     : {idIsPrimaryKey}");
					_output.WriteLine($"declared keys         : {string.Join(" | ", entity.GetKeys().Select(k => (k.IsPrimaryKey() ? "PK " : "AK ") + string.Join(",", k.Properties.Select(p => p.Name))))}");
				}
			});

			//assert - A35 reads its exclusion set from IsKey(), so whether that predicate is true here is the crux
			emailIsKey.ShouldBeTrue("IProperty.IsKey() is expected to be true for a property in any declared key, alternate ones included.");
			emailIsPrimaryKey.ShouldBeFalse("Email is an alternate key, not the primary key.");
			idIsKey.ShouldBeTrue();
			idIsPrimaryKey.ShouldBeTrue();
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldNotThrowWhenAUniqueIndexedNonKeyColumnIsChangedBySetValues(string provider)
		{
			//setup
			var observation = new Observation();
			var uniqueEmailIsKey = true;

			WithProvider(provider, factory =>
			{
				int id;

				using (var context = factory())
				{
					var seeded = new SpikeUniqueUser { Email = OriginalEmail, Name = "Original" };
					context.UniqueUsers.Add(seeded);
					context.SaveChanges();
					id = seeded.Id;

					uniqueEmailIsKey = context.Model
						.FindEntityType(typeof(SpikeUniqueUser))
						.FindProperty(nameof(SpikeUniqueUser.Email))
						.IsKey();
				}

				//act
				using (var context = factory())
				{
					var stored = context.UniqueUsers.Single(u => u.Id == id);
					context.Entry(stored).State.ShouldBe(EntityState.Unchanged);

					var incoming = new SpikeUniqueUser { Id = id, Email = ChangedEmail, Name = "Original" };
					incoming.Email.ShouldNotBe(stored.Email);

					observation.AtAssignment = Record.Exception(() => context.Entry(stored).CurrentValues.SetValues(incoming));

					if (observation.AtAssignment == null)
						observation.AtDetectChanges = Record.Exception(() => context.ChangeTracker.DetectChanges());

					observation.StateAfter = Describe(() => context.Entry(stored).State.ToString());
					observation.EmailAfter = Describe(() => Convert.ToString(context.Entry(stored).CurrentValues[nameof(SpikeUniqueUser.Email)]));
					observation.IdAfter = Describe(() => Convert.ToString(context.Entry(stored).CurrentValues[nameof(SpikeUniqueUser.Id)]));
				}
			});

			//assert - uniqueness alone is not keyhood, so neither IsKey() nor the guard should engage
			Report(provider, "SetValues onto a unique-indexed but non-key column", observation);
			uniqueEmailIsKey.ShouldBeFalse("A unique index is not a key, so IProperty.IsKey() should be false and A35's exclusion set should not contain this property.");
			observation.Thrown.ShouldBeNull();
			observation.EmailAfter.ShouldBe(ChangedEmail);
		}

		[Theory]
		[InlineData(InMemory)]
		[InlineData(Sqlite)]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "AlternateKeys")]
		public void ShouldNotThrowWhenSetValuesCarriesAnUnchangedAlternateKey(string provider)
		{
			//setup
			Observation observation = null;

			WithProvider(provider, factory =>
			{
				var id = Seed(factory);

				//act
				observation = Observe(factory, id, (context, stored) =>
				{
					var incoming = new SpikeUser { Id = id, Email = OriginalEmail, Name = "Renamed" };

					// Every key value matches; only the non-key Name differs, which is the ordinary Update.
					incoming.Id.ShouldBe(stored.Id);
					incoming.Email.ShouldBe(stored.Email);
					incoming.Name.ShouldNotBe(stored.Name);

					context.Entry(stored).CurrentValues.SetValues(incoming);
				});
			});

			//assert - SetValues writes only where values differ, so an unchanged key should never reach the guard
			Report(provider, "SetValues carrying identical key values and a differing non-key value", observation);
			observation.Thrown.ShouldBeNull(
				"If this throws, A35's exclusion is required on every Update rather than only on one that changes a key.");
			observation.StateAfter.ShouldBe(EntityState.Modified.ToString());
		}
	}
}
