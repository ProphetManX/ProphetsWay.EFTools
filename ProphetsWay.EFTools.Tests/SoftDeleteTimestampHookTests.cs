using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The two timestamp hooks the soft-delete families add — <c>GetCurrentTimestamp</c> and
	/// <c>NormalizeRetrievedTimestamp</c> — and the Timestamp Pair Rule (A13) that binds them.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Only the hooks.</b> Soft-delete filtering, the stamping members' own semantics, the row counts,
	/// idempotent <c>Delete</c>, <c>Get</c> returning a deleted row and the identifier write-back are
	/// <i>already discharged upstream</i> — <c>DepartmentDaoTests</c> is 33 tests against 19 numbered rules and
	/// reaches this library through the seam. The routing table in <c>docs/api-contract.md</c> marks this group
	/// <i>mostly already discharged upstream</i> and names exactly the obligations that are <b>new local</b>:
	/// the three timestamp-hook obligations and the custom-timezone pairing. A second local copy of the rest is
	/// declined by D10. Nothing here re-authors any of it.
	/// </para>
	/// <para>
	/// <b>Why they cannot live upstream.</b> Both hooks are <c>protected virtual</c> members of
	/// <c>BaseSoftDao</c>. <c>ProphetsWay.Example.DataAccess.NoDB</c> cannot override what it does not declare,
	/// and <c>IDepartmentDao</c> rule 18 asserts the <i>outcome</i> of a UTC policy rather than that a hook
	/// produced it. There is no upstream vocabulary for either member.
	/// </para>
	/// <para>
	/// <b>SQLite in-memory, never the InMemory provider.</b> The retrieval obligations exist because a
	/// relational store does not persist <see cref="DateTimeKind"/>; a non-relational provider hands back the
	/// object it was given, so the loss these tests are written to observe would never happen and every one of
	/// them would pass against a normalizer that does nothing. <c>Constants.cs</c> and its SQL Server
	/// connection string are deliberately untouched — nothing here needs a local server.
	/// </para>
	/// <para>
	/// <b>The Timestamp Pair Rule as amended, not as it read before.</b> Owner decision <b>F2, Option C</b>,
	/// 2026-08-22: overriding <c>GetCurrentTimestamp()</c> alone is <b>conforming provided the replacement clock
	/// still yields UTC</b>, because the default normalizer already agrees with such a clock. Every other single
	/// override remains a defect. <see cref="CountingClockLabelDao"/>, <see cref="SettableClockLabelDao"/> and
	/// <see cref="FrozenClockLabelDao"/> are all inside that carve-out and deliberately leave the normalizer
	/// alone; <see cref="LocalTimeLabelDao"/> overrides both because its clock does not yield UTC, and
	/// <see cref="ClockOnlyLabelDao"/> is outside the carve-out and is pinned as a hazard rather than as a
	/// conforming policy. <b>A suite that treats every single override as a defect makes a fixed-instant test
	/// clock look non-conforming</b>, which is the failure the amendment was made to prevent.
	/// </para>
	/// <para>
	/// <b>The normalizer's default is asserted directly against the hook</b>, per owner decision <b>F3, Option
	/// C</b> — no round trip and no provider. The superseded wording asserted a round trip, which made a
	/// <c>Contract</c> obligation turn on a certified-provider fact: against a provider that preserved
	/// <see cref="DateTimeKind"/> an identity normalizer passed it.
	/// </para>
	/// </remarks>
	public class SoftDeleteTimestampHookTests
	{
		#region fixture entities

		/// <summary>
		/// The soft entity these tests are written against. Purpose-built rather than borrowed:
		/// <c>IDepartmentDao</c> rule 18 pins both halves of <c>Department</c>'s timestamp policy, so there is
		/// no legitimate way to perturb a <c>Department</c> Data Access Object's hooks.
		/// </summary>
		public class Label : IBaseSoftIdEntity<int>
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		/// <summary>
		/// A <b>hard</b> entity carrying a navigation to a soft one. It has no timestamps of its own and its
		/// Data Access Object has no hooks — which is the whole point of the include obligation.
		/// </summary>
		public class Article : IBaseIdEntity<int>
		{
			public int Id { get; set; }

			public string Title { get; set; }

			public int LabelId { get; set; }

			public Label Label { get; set; }
		}

		#endregion

		#region fixture context and data access objects

		public class TimestampHookContext : DbContext
		{
			public TimestampHookContext(DbContextOptions<TimestampHookContext> options) : base(options)
			{
			}

			public DbSet<Label> Labels { get; set; }

			public DbSet<Article> Articles { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				var label = modelBuilder.Entity<Label>();
				label.HasKey(x => x.Id);

				var article = modelBuilder.Entity<Article>();
				article.HasKey(x => x.Id);
				article.HasOne(x => x.Label).WithMany().HasForeignKey(x => x.LabelId);
			}
		}

		/// <summary>Overrides nothing. The subject of every default-behavior obligation.</summary>
		/// <remarks>
		/// <see cref="ReachNormalizeRetrievedTimestamp"/> is a test instrument and not an override: it calls the
		/// hook rather than reimplementing it, so what answers is the default. There is no other way to reach a
		/// <c>protected</c> member at the keyed declaration site, which is what owner decision F3 Option C requires
		/// — no round trip, no provider.
		/// </remarks>
		public class LabelDao : BaseSoftPagedDao<Label, int>
		{
			public LabelDao(DbContext context) : base(context)
			{
			}

			public DateTime ReachNormalizeRetrievedTimestamp(DateTime value)
			{
				return NormalizeRetrievedTimestamp(value);
			}
		}

		/// <summary>Counts clock reads. The only instrument that can observe "once per operation".</summary>
		public class CountingClockLabelDao : BaseSoftDao<Label, int>
		{
			public CountingClockLabelDao(DbContext context) : base(context)
			{
			}

			public int ClockReads { get; private set; }

			// The clock still yields UTC, so this override is inside the F2 Option C carve-out and owes no
			// normalizer override. It is left alone deliberately.
			protected override DateTime GetCurrentTimestamp()
			{
				ClockReads++;

				return DateTime.UtcNow;
			}
		}

		/// <summary>A clock the test drives, so each stamping member can be given a distinguishable instant.</summary>
		/// <remarks>
		/// Every instant the test sets carries <see cref="DateTimeKind.Utc"/>, so this is inside the F2 Option C
		/// carve-out and owes no normalizer override.
		/// </remarks>
		public class SettableClockLabelDao : BaseSoftGetAllDao<Label, int>
		{
			public SettableClockLabelDao(DbContext context) : base(context)
			{
			}

			public DateTime Now { get; set; }

			protected override DateTime GetCurrentTimestamp()
			{
				return Now;
			}
		}

		/// <summary>
		/// A conforming local-time policy — both halves, per A13. <see cref="DateTimeKind.Local"/> is the one
		/// kind no relational provider materializes, which makes it a sentinel for "this library's hook ran"
		/// rather than an assertion about a certified provider's storage behavior.
		/// </summary>
		public class LocalTimeLabelDao : BaseSoftDao<Label, int>
		{
			public LocalTimeLabelDao(DbContext context) : base(context)
			{
			}

			protected override DateTime GetCurrentTimestamp()
			{
				return DateTime.Now;
			}

			protected override DateTime NormalizeRetrievedTimestamp(DateTime value)
			{
				return DateTime.SpecifyKind(value, DateTimeKind.Local);
			}
		}

		/// <summary>
		/// <b>Deliberately non-conforming.</b> A clock that does <b>not</b> yield UTC, with the normalizer left at
		/// its default — outside the F2 Option C carve-out, and a defect the Timestamp Pair Rule names. It exists to
		/// demonstrate the hazard, not to model a supported policy. The carve-out is keyed on what the replacement
		/// clock yields, not on which member was overridden, which is the only thing separating this from
		/// <see cref="FrozenClockLabelDao"/>.
		/// </summary>
		public class ClockOnlyLabelDao : BaseSoftDao<Label, int>
		{
			public ClockOnlyLabelDao(DbContext context) : base(context)
			{
			}

			protected override DateTime GetCurrentTimestamp()
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// The third conforming pairing the F2 Option C amendment added on the <b>keyed</b> branch — a frozen clock
		/// that still yields UTC, with <c>NormalizeRetrievedTimestamp</c> deliberately <b>not</b> overridden.
		/// </summary>
		public class FrozenClockLabelDao : BaseSoftDao<Label, int>
		{
			public static readonly DateTime Frozen = new DateTime(2026, 8, 22, 13, 45, 0, DateTimeKind.Utc);

			public FrozenClockLabelDao(DbContext context) : base(context)
			{
			}

			protected override DateTime GetCurrentTimestamp()
			{
				return Frozen;
			}
		}

		/// <summary>A hard Data Access Object that materializes a soft entity as an include.</summary>
		public class ArticleDao : BaseDao<Article, int>
		{
			public ArticleDao(DbContext context) : base(context)
			{
			}

			protected override IQueryable<Article> ApplyIncludes(IQueryable<Article> query)
			{
				return query.Include(x => x.Label);
			}
		}

		#endregion

		#region apparatus

		private static void WithStore(Action<Func<TimestampHookContext>> body)
		{
			// Held for the whole body: disposing the store discards the database every context here reads.
			var store = TestStore.OpenStore(nameof(SoftDeleteTimestampHookTests));
			var completed = false;

			try
			{
				var builder = new DbContextOptionsBuilder<TimestampHookContext>();

				store.Configure(builder);

				var options = builder.Options;

				Func<TimestampHookContext> factory = () => new TimestampHookContext(options);

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
		/// One label inserted and then updated through the defaulting Data Access Object, so the stored row
		/// carries a <c>CreatedDate</c> and an <c>UpdatedDate</c> and no <c>DeletedDate</c>. Returns the
		/// caller's instance, which carries the un-normalized values the clock produced.
		/// </summary>
		private static Label Setup_InsertedThenUpdatedLabel(Func<TimestampHookContext> factory)
		{
			var label = new Label { Name = "inserted then updated" };

			using (var context = factory())
			{
				var dao = new LabelDao(context);

				dao.Insert(label);
				dao.Update(label);
			}

			return label;
		}

		/// <summary>The three retrieval members, reached one at a time so a failure names which one.</summary>
		private static Label Retrieve(LabelDao dao, int id, string member)
		{
			switch (member)
			{
				case "Get":
					return dao.Get(new Label { Id = id });

				case "GetAll":
					return dao.GetAll(default(Label)).Single(x => x.Id == id);

				case "GetPaged":
					return dao.GetPaged(default(Label), 0, 10).Single(x => x.Id == id);

				default:
					throw new ArgumentOutOfRangeException(nameof(member), member, "Not a retrieval member of this family.");
			}
		}

		/// <summary>
		/// An article over a label whose timestamps were stamped by the local-time policy. The article is added
		/// through the context directly: the subject is <see cref="ArticleDao"/>'s <i>read</i>, and routing the
		/// seed through a write member would put this library's insert semantics inside the arrangement.
		/// </summary>
		private static (int ArticleId, int LabelId) Setup_ArticleOverALocalTimeStampedLabel(Func<TimestampHookContext> factory)
		{
			var label = new Label { Name = "sentinel" };

			using (var context = factory())
				new LocalTimeLabelDao(context).Insert(label);

			var article = new Article { Title = "an article", LabelId = label.Id };

			using (var context = factory())
			{
				context.Articles.Add(article);
				context.SaveChanges();
			}

			return (article.Id, label.Id);
		}

		#endregion

		#region GetCurrentTimestamp

		/// <summary>
		/// The clock's default is <see cref="DateTime.UtcNow"/> — asserted as a kind and a bracket, because an
		/// instant cannot be asserted exactly. Timestamp Policy: <i>GetCurrentTimestamp default</i>.
		/// </summary>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldStampWithUtcNowWhenTheClockIsNotOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "default clock" };

				using (var context = factory())
				{
					var dao = new LabelDao(context);
					var before = DateTime.UtcNow;

					//act
					dao.Insert(label);

					var after = DateTime.UtcNow;

					//assert
					label.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					label.CreatedDate.ShouldBeGreaterThanOrEqualTo(before);
					label.CreatedDate.ShouldBeLessThanOrEqualTo(after);
				}
			});
		}

		/// <summary>
		/// The clock is read <b>once per stamping operation</b>, not once per value the operation writes.
		/// Timestamp Policy: <i>Called — once per stamping operation</i>.
		/// </summary>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldReadTheClockOnceForEachStampingOperation()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "counted" };

				using (var context = factory())
				{
					var dao = new CountingClockLabelDao(context);

					//act & assert — one read per member, checked at each step so a failure names the member
					dao.Insert(label);
					dao.ClockReads.ShouldBe(1);

					dao.Update(label);
					dao.ClockReads.ShouldBe(2);

					dao.Delete(label);
					dao.ClockReads.ShouldBe(3);
				}
			});
		}

		/// <summary>
		/// An overridden clock is honored by all three stamping members, and each writes the instant that was
		/// current when it ran. Read back through the store rather than through a retrieval member, so the
		/// normalizer is not in the path.
		/// </summary>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldStampWithTheValueAnOverriddenClockSupplies()
		{
			WithStore(factory =>
			{
				//setup
				var created = new DateTime(2020, 5, 17, 13, 45, 30, DateTimeKind.Utc);
				var updated = new DateTime(2021, 6, 18, 14, 46, 31, DateTimeKind.Utc);
				var deleted = new DateTime(2022, 7, 19, 15, 47, 32, DateTimeKind.Utc);
				var label = new Label { Name = "fixed clock" };

				using (var context = factory())
				{
					var dao = new SettableClockLabelDao(context) { Now = created };

					//act
					dao.Insert(label);

					dao.Now = updated;
					dao.Update(label);

					dao.Now = deleted;
					dao.Delete(label);
				}

				//assert
				using (var check = factory())
				{
					var stored = check.Labels.Single(x => x.Id == label.Id);

					stored.CreatedDate.ShouldBe(created);
					stored.UpdatedDate.ShouldBe(updated);
					stored.DeletedDate.ShouldBe(deleted);
				}
			});
		}

		#endregion

		#region NormalizeRetrievedTimestamp

		/// <summary>
		/// The default normalizer <b>relabels; it never shifts</b>: a value arriving with
		/// <see cref="DateTimeKind.Unspecified"/> comes back carrying <see cref="DateTimeKind.Utc"/> and the same
		/// <see cref="DateTime.Ticks"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the Timestamp Policy table, <i>NormalizeRetrievedTimestamp default</i>, second clause.
		/// Asserted <b>directly against the hook</b>, per owner decision F3 Option C, 2026-08-22 — no round trip,
		/// no provider. This is the keyed half of that obligation; the keyless half is
		/// <c>KeylessSoftDaoTests.ShouldRelabelWithoutShiftingWhenTheNormalizerDefaultIsCalledDirectly</c>, and
		/// R4-S2 requires both because the pair is declared on two unrelated branches with no compiler check
		/// spanning them.
		/// </para>
		/// <para>
		/// <b>The superseded wording is named so it is not restored.</b> <i>"The default restores
		/// <see cref="DateTimeKind.Utc"/> on SQLite and SQL Server"</i> asserted a round trip, which made a
		/// <c>Contract</c> obligation turn on a certified-provider fact: against a provider that preserved
		/// <c>Kind</c>, an identity normalizer (<c>value =&gt; value</c>) passed it. This kills the identity
		/// normalizer with no store in the path at all.
		/// </para>
		/// <para>
		/// <b>The ticks clause is the half that discriminates a wrongly-applied <c>ToUniversalTime()</c></b>, and
		/// lap finding <b>F7</b> is that it does so on any non-UTC machine and on no UTC one. F7 is a property of
		/// the mechanism rather than of this wording and is not closed here; what changed is that the old wording
		/// discriminated it on <i>no</i> machine. A non-zero time-of-day is used so a shift is visible at all.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldRelabelWithoutShiftingWhenTheNormalizerDefaultIsCalledDirectly()
		{
			WithStore(factory =>
			{
				//setup
				var provided = new DateTime(2026, 8, 22, 13, 45, 30, DateTimeKind.Unspecified);

				using (var context = factory())
				{
					var dao = new LabelDao(context);

					//act
					var normalized = dao.ReachNormalizeRetrievedTimestamp(provided);

					//assert
					normalized.Kind.ShouldBe(DateTimeKind.Utc);
					normalized.Ticks.ShouldBe(provided.Ticks);
				}
			});
		}

		/// <summary>
		/// Every timestamp this Data Access Object's own reads materialize passes through the normalizer, whose
		/// default restores <see cref="DateTimeKind.Utc"/> — and a <c>null</c> stays <c>null</c> rather than
		/// being normalized into one. The instant is unchanged, because the default relabels and never shifts.
		/// </summary>
		[Theory]
		[InlineData("Get")]
		[InlineData("GetAll")]
		[InlineData("GetPaged")]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldRestoreTheUtcKindOnTimestampsTheRetrievalTrioMaterializes(string member)
		{
			WithStore(factory =>
			{
				//setup
				var label = Setup_InsertedThenUpdatedLabel(factory);

				using (var context = factory())
				{
					var dao = new LabelDao(context);

					//act
					var retrieved = Retrieve(dao, label.Id, member);

					//assert
					retrieved.ShouldNotBeNull();

					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.CreatedDate.ShouldBe(label.CreatedDate);

					retrieved.UpdatedDate.ShouldNotBeNull();
					retrieved.UpdatedDate.Value.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.UpdatedDate.Value.ShouldBe(label.UpdatedDate.Value);

					retrieved.DeletedDate.ShouldBeNull();
				}
			});
		}

		/// <summary>
		/// The third timestamp is normalized too. <c>Get</c> is the only member that can reach it — the read
		/// filter restricts <c>GetAll</c> and <c>GetPaged</c> to rows whose <c>DeletedDate</c> is <c>null</c>.
		/// </summary>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldRestoreTheUtcKindOnARetrievedDeletedDate()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "deleted" };

				using (var context = factory())
				{
					var dao = new LabelDao(context);

					dao.Insert(label);
					dao.Delete(label);
				}

				using (var context = factory())
				{
					var dao = new LabelDao(context);

					//act
					var retrieved = dao.Get(new Label { Id = label.Id });

					//assert
					retrieved.ShouldNotBeNull();
					retrieved.DeletedDate.ShouldNotBeNull();
					retrieved.DeletedDate.Value.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.DeletedDate.Value.ShouldBe(label.DeletedDate.Value);
				}
			});
		}

		#endregion

		#region the Timestamp Pair Rule — A13

		/// <summary>
		/// A custom policy that overrides both halves round-trips its own <see cref="DateTimeKind"/>: the value
		/// written back by the stamping member carries the clock's kind, and the value the same Data Access
		/// Object retrieves carries it again.
		/// </summary>
		/// <remarks>
		/// <c>Characterization</c>. Nothing obliges an implementation to support a non-UTC policy — A13 states
		/// what a conforming <i>pairing</i> looks like, and the library cannot check that one half agrees with
		/// the other. This pins the sanctioned shape, not a requirement placed on a future implementation.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "SoftDelete")]
		public void ShouldRoundTripALocalTimePolicyWhenBothHooksAreOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "local policy" };

				using (var context = factory())
					new LocalTimeLabelDao(context).Insert(label);

				using (var context = factory())
				{
					var dao = new LocalTimeLabelDao(context);

					//act
					var retrieved = dao.Get(new Label { Id = label.Id });

					//assert
					label.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);
					retrieved.CreatedDate.ShouldBe(label.CreatedDate);
				}
			});
		}

		/// <summary>
		/// The A13 hazard, demonstrated rather than assumed away: override the clock alone and one stored
		/// instant answers with two different kinds — <see cref="DateTimeKind.Local"/> on the instance the write
		/// handed back, <see cref="DateTimeKind.Utc"/> on the instance the read handed back, with identical
		/// ticks. The retrieved value therefore claims to be Coordinated Universal Time while holding a local
		/// wall-clock reading, and nothing signals it.
		/// </summary>
		/// <remarks>
		/// <c>Characterization</c>. This pins what a defect looks like, not behavior any implementation is
		/// obliged to reproduce.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "SoftDelete")]
		public void ShouldRelabelALocalStampAsUtcWhenOnlyTheClockIsOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "half a policy" };

				using (var context = factory())
					new ClockOnlyLabelDao(context).Insert(label);

				using (var context = factory())
				{
					var dao = new ClockOnlyLabelDao(context);

					//act
					var retrieved = dao.Get(new Label { Id = label.Id });

					//assert
					label.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.CreatedDate.Ticks.ShouldBe(label.CreatedDate.Ticks);
				}
			});
		}

		/// <summary>
		/// <b>The F2 Option C carve-out, on the keyed branch.</b> A frozen clock that still yields UTC, with
		/// <c>NormalizeRetrievedTimestamp</c> left at its default, is <b>conforming</b>: the stamped instant and
		/// the retrieved one agree, and both carry <see cref="DateTimeKind.Utc"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the Timestamp Pair Rule as amended by owner decision F2 Option C, 2026-08-22 —
		/// <i>"overriding <c>GetCurrentTimestamp()</c> alone is conforming provided the replacement clock still
		/// yields UTC"</i> — stated on <see cref="BaseSoftDao{TEntity, TKey}"/>'s own remarks. The obligation it
		/// discharges is the library-side half: an implementation whose default normalizer disagreed with a UTC
		/// clock would break the carve-out, and with it the ordinary way to inject a fixed test clock.
		/// </para>
		/// <para>
		/// <b>It must not be pinned as a hazard.</b> A suite that treats every single override as a defect makes a
		/// fixed-instant test clock look non-conforming and pushes the next author into overriding a normalizer
		/// they have no reason to touch — which is the failure the amendment was made to prevent.
		/// <see cref="ShouldRelabelALocalStampAsUtcWhenOnlyTheClockIsOverridden"/> is the hazard, and the two
		/// differ only in what the replacement clock yields: the carve-out is keyed on that, not on which member
		/// was overridden.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldConformWhenOnlyTheClockIsOverriddenAndItStillYieldsUtc()
		{
			WithStore(factory =>
			{
				//setup
				var label = new Label { Name = "frozen clock" };

				using (var context = factory())
					new FrozenClockLabelDao(context).Insert(label);

				using (var context = factory())
				{
					var dao = new FrozenClockLabelDao(context);

					//act
					var retrieved = dao.Get(new Label { Id = label.Id });

					//assert
					label.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					label.CreatedDate.Ticks.ShouldBe(FrozenClockLabelDao.Frozen.Ticks);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.CreatedDate.Ticks.ShouldBe(FrozenClockLabelDao.Frozen.Ticks);
				}
			});
		}

		#endregion

		#region the include bypass — G12

		/// <summary>
		/// The normalizer reaches the timestamps <b>this</b> Data Access Object's own reads materialize, and not
		/// one materialized as an include on another Data Access Object's query. Both halves are required: the
		/// divergence is the assertion.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>. Rule 18's retrieval clause was narrowed by owner decision to <c>IDepartmentDao</c>'s
		/// own reads, so this is the specified boundary rather than a shortfall from one, and this library
		/// generalizes that boundary to every soft family.
		/// </para>
		/// <para>
		/// The included instance is asserted <b>not</b> to carry <see cref="DateTimeKind.Local"/> rather than to
		/// carry <see cref="DateTimeKind.Unspecified"/>. What is being asserted is that the hook did not run;
		/// asserting the provider's own storage behavior would make a <c>Contract</c> obligation depend on a
		/// certified-provider fact.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "SoftDelete")]
		public void ShouldNotNormalizeASoftEntityMaterializedAsAnIncludeOnAnotherDaosQuery()
		{
			WithStore(factory =>
			{
				//setup
				var seeded = Setup_ArticleOverALocalTimeStampedLabel(factory);

				Article article;
				Label label;

				//act
				// Two contexts, so no identity map can hand the same Label instance to both reads and make the
				// divergence unobservable against an implementation that reads with tracking.
				using (var context = factory())
					article = new ArticleDao(context).Get(new Article { Id = seeded.ArticleId });

				using (var context = factory())
					label = new LocalTimeLabelDao(context).Get(new Label { Id = seeded.LabelId });

				//assert
				article.ShouldNotBeNull();
				article.Label.ShouldNotBeNull();
				article.Label.CreatedDate.Kind.ShouldNotBe(DateTimeKind.Local);

				label.ShouldNotBeNull();
				label.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);
			});
		}

		#endregion
	}
}
