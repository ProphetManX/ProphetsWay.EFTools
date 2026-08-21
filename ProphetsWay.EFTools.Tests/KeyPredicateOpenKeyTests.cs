using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The key shapes S4 opens up and the existing adapted suite structurally cannot reach: a <c>string</c>
	/// identifier and a <see cref="Nullable{T}"/> value identifier, plus the null-key short-circuit A4 specifies
	/// and the <c>default(TKey)</c> contrast OD-3 draws against it.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>These are this library's own tests, not adapters.</b> The thirteen <c>EF*Tests.cs</c> files adapt
	/// <c>ProphetsWay.Example.Tests</c> through the seam; nothing in that upstream suite has a <c>string</c>-keyed
	/// or <c>int?</c>-keyed entity, so nothing there can exercise the open <c>TKey</c> that
	/// <c>docs/api-contract.md</c> calls the riskiest area of the redesign.
	/// </para>
	/// <para>
	/// <b>SQLite in-memory, never the InMemory provider.</b> The point of these tests is that the predicate
	/// <i>translates</i>; a non-relational provider would evaluate it in the client and report success for an
	/// implementation that never produced SQL at all. <c>Constants.cs</c> and its SQL Server connection string are
	/// deliberately untouched — nothing here needs a local server.
	/// </para>
	/// <para>
	/// Collation is deliberately not asserted anywhere in this file. Every seeded key differs from every other by
	/// more than case and padding, so each assertion holds identically on both certified legs. The collation split
	/// is OD-2 / G9 and is a two-leg concern of its own.
	/// </para>
	/// </remarks>
	public class KeyPredicateOpenKeyTests
	{
		#region fixture entities

		/// <summary>A reference-type identifier that is also the stored primary key.</summary>
		public class Country : IBaseIdEntity<string>
		{
			public string Id { get; set; }

			public string Name { get; set; }
		}

		/// <summary>
		/// A reference-type identifier that is <b>not</b> the primary key, so the column can hold null. A stored
		/// null key is unreachable when the string identifier is the primary key, and the null-stored-key
		/// obligation needs one.
		/// </summary>
		public class Alias : IBaseIdEntity<string>
		{
			public int Ordinal { get; set; }

			public string Id { get; set; }

			public string Name { get; set; }
		}

		/// <summary>A nullable value-type identifier — the key shape the old <c>where TIdType : struct</c> could not express.</summary>
		public class Reading : IBaseIdEntity<int?>
		{
			public int? Id { get; set; }

			public string Label { get; set; }
		}

		/// <summary>A non-nullable value-type identifier, present only to pin the OD-3 contrast.</summary>
		public class Marker : IBaseIdEntity<int>
		{
			public int Id { get; set; }

			public string Label { get; set; }
		}

		#endregion

		#region fixture context and data access objects

		public class KeyPredicateContext : DbContext
		{
			public KeyPredicateContext(DbContextOptions<KeyPredicateContext> options) : base(options)
			{
			}

			public DbSet<Country> Countries { get; set; }

			public DbSet<Alias> Aliases { get; set; }

			public DbSet<Reading> Readings { get; set; }

			public DbSet<Marker> Markers { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				var country = modelBuilder.Entity<Country>();
				country.HasKey(x => x.Id);

				var alias = modelBuilder.Entity<Alias>();
				alias.HasKey(x => x.Ordinal);
				alias.Property(x => x.Ordinal).ValueGeneratedNever();
				alias.Property(x => x.Id).IsRequired(false);

				var reading = modelBuilder.Entity<Reading>();
				reading.HasKey(x => x.Id);
				reading.Property(x => x.Id).IsRequired().ValueGeneratedNever();

				var marker = modelBuilder.Entity<Marker>();
				marker.HasKey(x => x.Id);
				marker.Property(x => x.Id).ValueGeneratedNever();
			}
		}

		public class CountryDao : BaseDao<Country, string>
		{
			public CountryDao(DbContext context) : base(context)
			{
			}
		}

		public class AliasDao : BaseDao<Alias, string>
		{
			public AliasDao(DbContext context) : base(context)
			{
			}
		}

		public class ReadingDao : BaseDao<Reading, int?>
		{
			public ReadingDao(DbContext context) : base(context)
			{
			}
		}

		public class MarkerDao : BaseDao<Marker, int>
		{
			public MarkerDao(DbContext context) : base(context)
			{
			}
		}

		#endregion

		#region apparatus

		/// <summary>One parameter, as it stood at the moment the command was sent.</summary>
		public sealed class CapturedParameter
		{
			public CapturedParameter(string name, object value)
			{
				Name = name;
				Value = value;
			}

			public string Name { get; }

			public object Value { get; }
		}

		/// <summary>The text and the parameters of one command, snapshotted together.</summary>
		public sealed class CapturedCommand
		{
			public CapturedCommand(string commandText, IReadOnlyList<CapturedParameter> parameters)
			{
				CommandText = commandText;
				Parameters = parameters;
			}

			public string CommandText { get; }

			public IReadOnlyList<CapturedParameter> Parameters { get; }

			public override string ToString()
			{
				return CommandText + " -- " + string.Join(", ", Parameters.Select(p => p.Name + "=" + Render(p.Value)));
			}
		}

		/// <summary>
		/// Records the command as sent to the provider. The interceptor route is the only instrument that can
		/// distinguish "answered without a query" from "queried and missed" — both return null, and it is the only
		/// one carrying a <see cref="DbParameterCollection"/> rather than redacted log text.
		/// </summary>
		public sealed class CommandRecorder : DbCommandInterceptor
		{
			private readonly List<CapturedCommand> _commands = new List<CapturedCommand>();

			public IReadOnlyList<CapturedCommand> Commands => _commands;

			public void Clear()
			{
				_commands.Clear();
			}

			public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
			{
				Capture(command);
				return base.ReaderExecuting(command, eventData, result);
			}

			public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
			{
				Capture(command);
				return base.NonQueryExecuting(command, eventData, result);
			}

			public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
			{
				Capture(command);
				return base.ScalarExecuting(command, eventData, result);
			}

			private void Capture(DbCommand command)
			{
				var parameters = new List<CapturedParameter>();

				// Snapshotted, not held: the provider reuses and clears the live collection once the command completes.
				foreach (DbParameter parameter in command.Parameters)
					parameters.Add(new CapturedParameter(parameter.ParameterName, parameter.Value));

				_commands.Add(new CapturedCommand(command.CommandText, parameters));
			}
		}

		/// <summary>Every <c>@name</c> placeholder, so a key value can be looked for in what is left of the text.</summary>
		private static readonly Regex ParameterPlaceholder = new Regex("@[A-Za-z0-9_]+", RegexOptions.Compiled);

		private static string Render(object value)
		{
			return value == null || value == DBNull.Value
				? "<null>"
				: Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Drops the provider's identifier quoting so one assertion reads on both certified legs.</summary>
		private static string Unquote(string sql)
		{
			return sql
				.Replace("\"", string.Empty)
				.Replace("[", string.Empty)
				.Replace("]", string.Empty)
				.Replace("`", string.Empty);
		}

		/// <summary>
		/// The predicate reached the store, over the identifier column, with the key travelling as a parameter.
		/// A client-evaluated implementation emits an unfiltered read and fails the first clause; one that builds
		/// the predicate over a constant emits the key inline and fails the last.
		/// </summary>
		private static void AssertKeyedLookup(CapturedCommand command, string identifierColumn, string renderedKey)
		{
			command.CommandText.ShouldContain("WHERE");

			var predicate = Unquote(command.CommandText.Substring(command.CommandText.IndexOf("WHERE", StringComparison.Ordinal)));
			predicate.ShouldContain(identifierColumn);

			command.Parameters.ShouldNotBeEmpty();
			command.Parameters.Select(p => Render(p.Value)).ShouldContain(renderedKey);

			ParameterPlaceholder.Replace(command.CommandText, string.Empty).ShouldNotContain(renderedKey);
		}

		private static void WithStore(Action<Func<KeyPredicateContext>, CommandRecorder> body)
		{
			var recorder = new CommandRecorder();

			using (var connection = new SqliteConnection("Filename=:memory:"))
			{
				// Held open for the whole body: closing it discards the in-memory database.
				connection.Open();

				var options = new DbContextOptionsBuilder<KeyPredicateContext>()
					.UseSqlite(connection)
					.AddInterceptors(recorder)
					.Options;

				Func<KeyPredicateContext> factory = () => new KeyPredicateContext(options);

				using (var schema = factory())
					schema.Database.EnsureCreated();

				body(factory, recorder);
			}
		}

		private static void Setup_ThreeCountries(Func<KeyPredicateContext> factory)
		{
			using (var context = factory())
			{
				context.Countries.Add(new Country { Id = "CA", Name = "Canada" });
				context.Countries.Add(new Country { Id = "US", Name = "United States" });
				context.Countries.Add(new Country { Id = "MX", Name = "Mexico" });
				context.SaveChanges();
			}
		}

		/// <summary>
		/// The empty string is a stored key here, not an absent one. A <c>default(TKey)</c> short-circuit makes
		/// this row unreachable through <c>Get</c> while <c>GetAll</c> still returns it — the OD-3 inconsistency.
		/// </summary>
		private static void Setup_CountriesIncludingOneStoredUnderAnEmptyKey(Func<KeyPredicateContext> factory)
		{
			using (var context = factory())
			{
				context.Countries.Add(new Country { Id = "", Name = "blank" });
				context.Countries.Add(new Country { Id = "CA", Name = "Canada" });
				context.Countries.Add(new Country { Id = "US", Name = "United States" });
				context.SaveChanges();
			}
		}

		private static void Setup_AliasesOneOfWhichHasANullKey(Func<KeyPredicateContext> factory)
		{
			using (var context = factory())
			{
				context.Aliases.Add(new Alias { Ordinal = 1, Id = null, Name = "unnamed" });
				context.Aliases.Add(new Alias { Ordinal = 2, Id = "beta", Name = "Beta" });
				context.SaveChanges();
			}
		}

		private static void Setup_ThreeReadings(Func<KeyPredicateContext> factory)
		{
			using (var context = factory())
			{
				context.Readings.Add(new Reading { Id = 10, Label = "ten" });
				context.Readings.Add(new Reading { Id = 20, Label = "twenty" });
				context.Readings.Add(new Reading { Id = 30, Label = "thirty" });
				context.SaveChanges();
			}
		}

		private static void Setup_MarkersWithNoRowUnderZero(Func<KeyPredicateContext> factory)
		{
			using (var context = factory())
			{
				context.Markers.Add(new Marker { Id = 1, Label = "one" });
				context.Markers.Add(new Marker { Id = 2, Label = "two" });
				context.SaveChanges();
			}
		}

		#endregion

		#region string identifier — S4 round trip

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldGetTheRowWhoseStringKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);

					//act
					var found = dao.Get(new Country { Id = "US" });

					//assert
					found.ShouldNotBeNull();
					found.Id.ShouldBe("US");
					found.Name.ShouldBe("United States");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldUpdateTheRowWhoseStringKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				int updated;

				using (var context = factory())
				{
					var dao = new CountryDao(context);

					//act
					updated = dao.Update(new Country { Id = "US", Name = "changed" });
				}

				//assert
				updated.ShouldBe(1);

				// Read back through the store rather than through Get: a 1 that wrote nothing is the failure.
				using (var check = factory())
				{
					check.Countries.Single(x => x.Id == "US").Name.ShouldBe("changed");
					check.Countries.Single(x => x.Id == "CA").Name.ShouldBe("Canada");
					check.Countries.Count().ShouldBe(3);
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldDeleteTheRowWhoseStringKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				int deleted;

				using (var context = factory())
				{
					var dao = new CountryDao(context);

					//act
					deleted = dao.Delete(new Country { Id = "MX" });
				}

				//assert
				deleted.ShouldBe(1);

				using (var check = factory())
				{
					check.Countries.Count().ShouldBe(2);
					check.Countries.Any(x => x.Id == "MX").ShouldBeFalse();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldEmitAWhereOverTheIdentifierColumnForAStringKey()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);
					recorder.Clear();

					//act
					var found = dao.Get(new Country { Id = "US" });

					//assert
					found.ShouldNotBeNull();
					AssertKeyedLookup(recorder.Commands.ShouldHaveSingleItem(), "Id", "US");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldNotThrowWhenAStoredRowCarriesANullStringKey()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_AliasesOneOfWhichHasANullKey(factory);

				using (var context = factory())
				{
					var dao = new AliasDao(context);
					IList<Alias> all = null;
					Alias found = null;
					Alias byNullKey = null;

					//act
					// GetAll is what forces the null-keyed row to be materialized; a Get that misses it never reads it.
					var thrownReadingEveryRow = Record.Exception(() => all = dao.GetAll(null));
					var thrownFetchingByKey = Record.Exception(() => found = dao.Get(new Alias { Id = "beta" }));
					var thrownFetchingByNullKey = Record.Exception(() => byNullKey = dao.Get(new Alias { Id = null }));

					//assert
					thrownReadingEveryRow.ShouldBeNull();
					thrownFetchingByKey.ShouldBeNull();
					thrownFetchingByNullKey.ShouldBeNull();

					all.ShouldNotBeNull();
					all.Count.ShouldBe(2);
					// Order is not asserted: Alias.Id is nullable, so A16's keyed default is not a total order here.
					all.Select(x => x.Name).OrderBy(x => x, StringComparer.Ordinal).ShouldBe(new[] { "Beta", "unnamed" });

					found.ShouldNotBeNull();
					found.Name.ShouldBe("Beta");

					// A null key never matches — least of all the row whose stored key is itself null.
					byNullKey.ShouldBeNull();
				}
			});
		}

		#endregion

		#region string identifier — A4 null key

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldReturnNullFromGetWhenTheStringKeyIsNull()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);
					var item = new Country { Id = null, Name = "not stored" };

					//act
					var found = dao.Get(item);

					//assert
					found.ShouldBeNull();
					item.Id.ShouldBeNull();
					item.Name.ShouldBe("not stored");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldReturnZeroFromUpdateAndDeleteWhenTheStringKeyIsNull()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				int updated;
				int deleted;

				using (var context = factory())
				{
					var dao = new CountryDao(context);

					//act
					updated = dao.Update(new Country { Id = null, Name = "not stored" });
					deleted = dao.Delete(new Country { Id = null, Name = "not stored" });
				}

				//assert
				updated.ShouldBe(0);
				deleted.ShouldBe(0);

				using (var check = factory())
					check.Countries.Count().ShouldBe(3);
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromGetWhenTheStringKeyIsNull()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);
					recorder.Clear();

					//act
					dao.Get(new Country { Id = null });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromUpdateWhenTheStringKeyIsNull()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);
					recorder.Clear();

					//act
					dao.Update(new Country { Id = null, Name = "not stored" });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromDeleteWhenTheStringKeyIsNull()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeCountries(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);
					recorder.Clear();

					//act
					dao.Delete(new Country { Id = null, Name = "not stored" });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		#endregion

		#region nullable value identifier — S4 round trip and A4 null key

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldGetTheRowWhoseNullableIntKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);

					//act
					var found = dao.Get(new Reading { Id = 20 });

					//assert
					found.ShouldNotBeNull();
					found.Id.ShouldBe(20);
					found.Label.ShouldBe("twenty");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldUpdateTheRowWhoseNullableIntKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				int updated;

				using (var context = factory())
				{
					var dao = new ReadingDao(context);

					//act
					updated = dao.Update(new Reading { Id = 20, Label = "changed" });
				}

				//assert
				updated.ShouldBe(1);

				using (var check = factory())
				{
					check.Readings.Single(x => x.Id == 20).Label.ShouldBe("changed");
					check.Readings.Single(x => x.Id == 10).Label.ShouldBe("ten");
					check.Readings.Count().ShouldBe(3);
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldDeleteTheRowWhoseNullableIntKeyMatches()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				int deleted;

				using (var context = factory())
				{
					var dao = new ReadingDao(context);

					//act
					deleted = dao.Delete(new Reading { Id = 30 });
				}

				//assert
				deleted.ShouldBe(1);

				using (var check = factory())
				{
					check.Readings.Count().ShouldBe(2);
					check.Readings.Any(x => x.Id == 30).ShouldBeFalse();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldEmitAWhereOverTheIdentifierColumnForANullableIntKey()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);
					recorder.Clear();

					//act
					var found = dao.Get(new Reading { Id = 20 });

					//assert
					found.ShouldNotBeNull();
					AssertKeyedLookup(recorder.Commands.ShouldHaveSingleItem(), "Id", "20");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldReturnNullFromGetWhenTheNullableIntKeyHasNoValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);
					var item = new Reading { Id = null, Label = "not stored" };

					//act
					var found = dao.Get(item);

					//assert
					found.ShouldBeNull();
					item.Id.ShouldBeNull();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldReturnZeroFromUpdateAndDeleteWhenTheNullableIntKeyHasNoValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				int updated;
				int deleted;

				using (var context = factory())
				{
					var dao = new ReadingDao(context);

					//act
					updated = dao.Update(new Reading { Id = null, Label = "not stored" });
					deleted = dao.Delete(new Reading { Id = null, Label = "not stored" });
				}

				//assert
				updated.ShouldBe(0);
				deleted.ShouldBe(0);

				using (var check = factory())
					check.Readings.Count().ShouldBe(3);
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromGetWhenTheNullableIntKeyHasNoValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				// Reading.Id is the primary key and NOT NULL, so an unguarded WHERE "Id" IS NULL also returns
				// nothing. Only the interceptor separates the short-circuit from the miss.
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);
					recorder.Clear();

					//act
					dao.Get(new Reading { Id = null });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromUpdateWhenTheNullableIntKeyHasNoValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);
					recorder.Clear();

					//act
					dao.Update(new Reading { Id = null, Label = "not stored" });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueNoCommandFromDeleteWhenTheNullableIntKeyHasNoValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_ThreeReadings(factory);

				using (var context = factory())
				{
					var dao = new ReadingDao(context);
					recorder.Clear();

					//act
					dao.Delete(new Reading { Id = null, Label = "not stored" });

					//assert
					recorder.Commands.ShouldBeEmpty();
				}
			});
		}

		#endregion

		#region default(TKey) is an ordinary key value — the OD-3 contrast

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldIssueACommandWhenANonNullableIntKeyCarriesItsDefaultValue()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_MarkersWithNoRowUnderZero(factory);

				using (var context = factory())
				{
					var dao = new MarkerDao(context);
					recorder.Clear();

					//act
					var found = dao.Get(new Marker { Id = 0 });

					//assert
					found.ShouldBeNull();
					// "A normal parameterized WHERE" is the whole of OD-3's contrast; a table scan is not it.
					AssertKeyedLookup(recorder.Commands.ShouldHaveSingleItem(), "Id", "0");
				}
			});
		}

		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "KeyPredicate")]
		public void ShouldGetTheRowStoredUnderAnEmptyStringKey()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_CountriesIncludingOneStoredUnderAnEmptyKey(factory);

				using (var context = factory())
				{
					var dao = new CountryDao(context);

					//act
					var found = dao.Get(new Country { Id = "" });

					//assert
					found.ShouldNotBeNull();
					found.Id.ShouldBe("");
					found.Name.ShouldBe("blank");
				}
			});
		}

		#endregion
	}
}
