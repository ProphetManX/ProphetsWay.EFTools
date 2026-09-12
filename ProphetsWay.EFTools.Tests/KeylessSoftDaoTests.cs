using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The two <b>soft</b> keyless families — <see cref="RootSoftNonIdDao{TEntity}"/> and
	/// <see cref="BaseSoftNonIdDao{TEntity}"/> — and the keyless half of the Timestamp Pair Rule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>This file discharges lap finding F8.</b> Routing rule <b>R4-S2</b> requires the timestamp-hook
	/// obligations to run <i>"once against a <c>BaseSoftDao</c> descendant and once against a
	/// <c>RootSoftNonIdDao</c> descendant"</i>, because the pair is declared on two unrelated branches with four
	/// override sites and <b>no compiler check spanning them</b>. Only the keyed half existed as of lap 2 —
	/// <see cref="SoftDeleteTimestampHookTests"/> — and the specification says the keyless half is discharged
	/// <i>here or nowhere</i>: <see cref="RootSoftNonIdDao{TEntity}"/> is the type being written in this lap, and
	/// an obligation whose subject never gets built is an obligation silently dropped.
	/// </para>
	/// <para>
	/// <b>The rule as amended, not as it read before.</b> Owner decision F2, Option C, 2026-08-22: overriding
	/// <c>GetCurrentTimestamp()</c> alone is <b>conforming provided the replacement clock still yields UTC</b>,
	/// because the default normalizer already agrees with such a clock. Every other single override remains a
	/// defect. <see cref="FrozenClockTagDao"/> is inside that carve-out and deliberately leaves the normalizer
	/// alone; <see cref="ClockOnlyTagDao"/> is outside it and is pinned as a hazard.
	/// </para>
	/// <para>
	/// <b>The normalizer's default is asserted directly against the hook</b>, per owner decision F3, Option C —
	/// no round trip and no provider. The superseded wording asserted a round trip, which made a <c>Contract</c>
	/// obligation turn on a certified-provider fact: against a provider that preserved <see cref="DateTimeKind"/>
	/// an identity normalizer passed it. The ticks clause is the half that discriminates a wrongly-applied
	/// <c>ToUniversalTime()</c> — and lap finding <b>F7</b> is that it discriminates it on any non-UTC machine
	/// and on no UTC one. F7 is not closed by this; what changed is that the old wording discriminated it on
	/// <i>no</i> machine.
	/// </para>
	/// <para>
	/// <b>SQLite in-memory, never the InMemory provider.</b> The retrieval obligations exist because a relational
	/// store does not persist <see cref="DateTimeKind"/>; a non-relational provider hands back the object it was
	/// given, so the loss these tests observe would never happen and a normalizer that did nothing would pass.
	/// <c>Constants.cs</c> and its SQL Server connection string are deliberately untouched.
	/// </para>
	/// </remarks>
	public class KeylessSoftDaoTests
	{
		#region fixture entities

		/// <summary>
		/// A keyless soft-delete join: the natural key is <see cref="OwnerId"/> and <see cref="Code"/> together,
		/// mapped as the primary key, plus one writable column of its own and the three lifecycle timestamps.
		/// </summary>
		public class Tag : IBaseSoftEntity
		{
			public int OwnerId { get; set; }

			public string Code { get; set; }

			public string Note { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		#endregion

		#region fixture context and data access objects

		public class KeylessSoftContext : DbContext
		{
			public KeylessSoftContext(DbContextOptions<KeylessSoftContext> options) : base(options)
			{
			}

			public DbSet<Tag> Tags { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.Entity<Tag>().HasKey(x => new { x.OwnerId, x.Code });
			}
		}

		/// <summary>
		/// Overrides the two required hooks and neither timestamp hook — the subject of every default-behavior
		/// obligation.
		/// </summary>
		/// <remarks>
		/// The <c>Reach*</c> members are test instruments and add no behavior. A <c>Root</c> type publishes
		/// nothing, so there is no other way to reach <c>GetCore</c>, <c>UpdateCore</c> or either timestamp hook
		/// at the declaration site where they are specified. They call the members rather than reimplementing
		/// them, so the default is what answers.
		/// </remarks>
		public class TagDao : RootSoftNonIdDao<Tag>
		{
			public TagDao(DbContext context) : base(context)
			{
			}

			public Tag ReachGetCore(Tag item)
			{
				return GetCore(item);
			}

			public int ReachUpdateCore(Tag item)
			{
				return UpdateCore(item);
			}

			public DateTime ReachGetCurrentTimestamp()
			{
				return GetCurrentTimestamp();
			}

			public DateTime ReachNormalizeRetrievedTimestamp(DateTime value)
			{
				return NormalizeRetrievedTimestamp(value);
			}

			protected override Expression<Func<Tag, bool>> MatchRow(Tag item)
			{
				return x => x.OwnerId == item.OwnerId && x.Code == item.Code;
			}

			protected override IOrderedQueryable<Tag> ApplyStableOrder(IQueryable<Tag> query)
			{
				return query.OrderBy(x => x.OwnerId).ThenBy(x => x.Code);
			}
		}

		/// <summary>The opt-in shape: the same soft plumbing, publishing the <see cref="IBaseDao{T}"/> surface.</summary>
		public class PublishedTagDao : BaseSoftNonIdDao<Tag>
		{
			public PublishedTagDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Tag, bool>> MatchRow(Tag item)
			{
				return x => x.OwnerId == item.OwnerId && x.Code == item.Code;
			}

			protected override IOrderedQueryable<Tag> ApplyStableOrder(IQueryable<Tag> query)
			{
				return query.OrderBy(x => x.OwnerId).ThenBy(x => x.Code);
			}
		}

		/// <summary>Counts clock reads. The only instrument that can observe "once per operation".</summary>
		public class CountingClockTagDao : TagDao
		{
			public CountingClockTagDao(DbContext context) : base(context)
			{
			}

			public int ClockReads { get; private set; }

			protected override DateTime GetCurrentTimestamp()
			{
				ClockReads++;

				return DateTime.UtcNow;
			}

			// The clock still yields UTC, so this override is inside the F2 Option C carve-out and owes no
			// normalizer override. It is left alone deliberately.
		}

		/// <summary>A clock the test drives, so each stamping member can be given a distinguishable instant.</summary>
		public class SettableClockTagDao : TagDao
		{
			public SettableClockTagDao(DbContext context) : base(context)
			{
			}

			public DateTime Now { get; set; }

			protected override DateTime GetCurrentTimestamp()
			{
				return Now;
			}
		}

		/// <summary>
		/// A conforming local-time policy — both halves, per the Timestamp Pair Rule.
		/// <see cref="DateTimeKind.Local"/> is the one kind no relational provider materializes, which makes it a
		/// sentinel for "this library's hook ran" rather than an assertion about a provider's storage behavior.
		/// </summary>
		public class LocalTimeTagDao : TagDao
		{
			public LocalTimeTagDao(DbContext context) : base(context)
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
		/// <b>Deliberately non-conforming.</b> A clock that does not yield UTC, with the normalizer left at its
		/// default — outside the carve-out, and a defect the rule names. It demonstrates the hazard; it does not
		/// model a supported policy.
		/// </summary>
		public class ClockOnlyTagDao : TagDao
		{
			public ClockOnlyTagDao(DbContext context) : base(context)
			{
			}

			protected override DateTime GetCurrentTimestamp()
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// The third conforming pairing the F2 Option C amendment added — a frozen clock that still yields UTC,
		/// with <c>NormalizeRetrievedTimestamp</c> deliberately <b>not</b> overridden.
		/// </summary>
		public class FrozenClockTagDao : TagDao
		{
			public static readonly DateTime Frozen = new DateTime(2026, 8, 22, 13, 45, 0, DateTimeKind.Utc);

			public FrozenClockTagDao(DbContext context) : base(context)
			{
			}

			protected override DateTime GetCurrentTimestamp()
			{
				return Frozen;
			}
		}

		#endregion

		#region apparatus

		private static void WithStore(Action<Func<KeylessSoftContext>> body)
		{
			// Held for the whole body: disposing the store discards the database every context here reads.
			var store = TestStore.OpenStore(nameof(KeylessSoftDaoTests));
			var completed = false;

			try
			{
				var builder = new DbContextOptionsBuilder<KeylessSoftContext>();

				store.Configure(builder);

				var options = builder.Options;

				Func<KeylessSoftContext> factory = () => new KeylessSoftContext(options);

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

		private static Tag NewTag(int owner, string code, string note)
		{
			return new Tag { OwnerId = owner, Code = code, Note = note };
		}

		/// <summary>Three live tags, stored through the Data Access Object so the stamps are the library's own.</summary>
		private static void Setup_ThreeLiveTags(Func<KeylessSoftContext> factory)
		{
			using (var context = factory())
			{
				var dao = new TagDao(context);

				dao.Insert(NewTag(1, "alpha", "first"));
				dao.Insert(NewTag(1, "beta", "second"));
				dao.Insert(NewTag(2, "gamma", "third"));
			}
		}

		/// <summary>The three retrieval paths of the soft root, reached one at a time so a failure names one.</summary>
		private static Tag Retrieve(TagDao dao, Tag probe, string member)
		{
			switch (member)
			{
				case "GetCore":
					return dao.ReachGetCore(probe);

				case "GetAll":
					return dao.GetAll(null).Single(x => x.OwnerId == probe.OwnerId && x.Code == probe.Code);

				case "GetPaged":
					return dao.GetPaged(null, 0, 50).Single(x => x.OwnerId == probe.OwnerId && x.Code == probe.Code);

				default:
					throw new ArgumentOutOfRangeException(nameof(member), member, "Not a retrieval member of this family.");
			}
		}

		private static MethodInfo Member(Type type, string name)
		{
			return type.GetMethod(
				name,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		}

		#endregion

		#region the shape — A14 and A2

		/// <summary>
		/// The soft keyless root commits to no capability interface and publishes neither <c>Get</c> nor
		/// <c>Update</c>; the <c>Base</c> type beneath it declares <see cref="IBaseDao{T}"/> and publishes both.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: A14. Without this type the only route to keyless soft delete ran through
		/// <see cref="IBaseDao{T}"/>, so a join table that keeps its history was forced to publish two members
		/// <c>ICompanyResourceDao</c> documents at length as meaningless — the coercion S5 exists to prevent,
		/// reintroduced one level down.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPublishNeitherGetNorUpdateOnTheKeylessSoftRoot()
		{
			//setup
			var parent = typeof(IBaseEntity).Assembly;

			//act
			var capabilities = typeof(RootSoftNonIdDao<Tag>)
				.GetInterfaces()
				.Where(x => x.Assembly == parent)
				.Select(x => x.FullName)
				.ToList();

			//assert
			capabilities.ShouldBeEmpty();

			typeof(RootSoftNonIdDao<Tag>).GetMethod("Get", new[] { typeof(Tag) }).ShouldBeNull();
			typeof(RootSoftNonIdDao<Tag>).GetMethod("Update", new[] { typeof(Tag) }).ShouldBeNull();

			typeof(IBaseDao<Tag>).IsAssignableFrom(typeof(BaseSoftNonIdDao<Tag>)).ShouldBeTrue();
			typeof(BaseSoftNonIdDao<Tag>).GetMethod("Get", new[] { typeof(Tag) }).ShouldNotBeNull();
			typeof(BaseSoftNonIdDao<Tag>).GetMethod("Update", new[] { typeof(Tag) }).ShouldNotBeNull();
		}

		/// <summary>
		/// Every difference from <see cref="RootNonIdDao{TEntity}"/> is an <c>override</c> and never a <c>new</c>
		/// member.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A2, stated on <see cref="RootSoftNonIdDao{TEntity}"/>'s own remarks — <i>"a hidden
		/// <c>Delete</c> would hard-delete a soft entity through an upcast — silent data loss no consumer would
		/// suspect."</i>
		/// </para>
		/// <para>
		/// <c>MethodInfo.GetBaseDefinition()</c> is the instrument that tells the two apart: for an
		/// <c>override</c> it returns the base declaration, and for a <c>new</c> member it returns the method
		/// itself. The two timestamp hooks are excluded because they are genuinely new on this type and have no
		/// base to point at.
		/// </para>
		/// </remarks>
		[Theory]
		[InlineData("Insert")]
		[InlineData("Delete")]
		[InlineData("GetAll")]
		[InlineData("GetPaged")]
		[InlineData("GetCore")]
		[InlineData("UpdateCore")]
		[InlineData("ApplyReadFilter")]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldOverrideRatherThanHideEverySoftDifference(string name)
		{
			//setup
			var declared = Member(typeof(RootSoftNonIdDao<Tag>), name);

			//act
			declared.ShouldNotBeNull();

			//assert
			declared.GetBaseDefinition().DeclaringType.ShouldBe(typeof(RootNonIdDao<Tag>));
		}

		/// <summary>
		/// A soft Data Access Object reached through a <see cref="RootNonIdDao{TEntity}"/>-typed reference still
		/// soft-deletes: the row is stamped, not removed, and stays retrievable.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: A2, the behavioral half of the guard above. This is the assertion a <c>new</c> member
		/// would fail while still compiling — the reflection test says the shape is right, this one says the shape
		/// is doing the work.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldStillSoftDeleteThroughARootNonIdDaoTypedReference()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				using (var context = factory())
				{
					var dao = new TagDao(context);
					RootNonIdDao<Tag> upcast = dao;

					//act
					var removed = upcast.Delete(NewTag(1, "alpha", null));

					//assert
					removed.ShouldBe(1);

					var stored = dao.ReachGetCore(NewTag(1, "alpha", null));

					stored.ShouldNotBeNull();
					stored.DeletedDate.ShouldNotBeNull();
				}

				using (var context = factory())
					context.Tags.Count().ShouldBe(3);
			});
		}

		#endregion

		#region soft-delete semantics

		/// <summary>
		/// <c>Insert</c> stamps <c>CreatedDate</c> and forces the other two to <c>null</c> whatever the caller
		/// assigned — on the caller's instance <b>and</b> on the stored row, from one reading of the clock.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: the <see cref="RootSoftNonIdDao{TEntity}.Insert"/> declaration, and obligation N3 —
		/// <i>"asserting one object cannot distinguish either failure."</i> An implementation that stamps only the
		/// copy passes the stored half and hands the caller a default <c>CreatedDate</c>; one that stamps only
		/// <c>item</c> passes the caller half and stores a default. The stored row is read through the context
		/// rather than a retrieval member, so the normalizer is not in the path.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldStampCreatedAndNullTheOtherTwoOnInsertEvenWhenTheCallerSetThem()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(3, "delta", "caller supplied all three");

				tag.CreatedDate = new DateTime(1999, 1, 1, 1, 1, 1, DateTimeKind.Utc);
				tag.UpdatedDate = new DateTime(1999, 2, 2, 2, 2, 2, DateTimeKind.Utc);
				tag.DeletedDate = new DateTime(1999, 3, 3, 3, 3, 3, DateTimeKind.Utc);

				using (var context = factory())
					new TagDao(context).Insert(tag);

				//assert — the caller's instance
				tag.CreatedDate.ShouldNotBe(new DateTime(1999, 1, 1, 1, 1, 1, DateTimeKind.Utc));
				tag.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
				tag.UpdatedDate.ShouldBeNull();
				tag.DeletedDate.ShouldBeNull();

				//assert — the stored row, carrying the same reading
				using (var context = factory())
				{
					var stored = context.Tags.Single(x => x.OwnerId == 3 && x.Code == "delta");

					stored.CreatedDate.ShouldBe(tag.CreatedDate);
					stored.UpdatedDate.ShouldBeNull();
					stored.DeletedDate.ShouldBeNull();
				}
			});
		}

		/// <summary>
		/// <c>Delete</c> stamps rather than removes, returns <c>1</c>, writes the stamp back onto the caller's
		/// instance, and leaves the row reachable through <c>GetCore</c> — which is not read-filtered.
		/// </summary>
		/// <remarks><c>Contract</c>: the <see cref="RootSoftNonIdDao{TEntity}.Delete"/> and <c>GetCore</c> declarations.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldStampDeletedDateAndLeaveTheRowRetrievable()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				using (var context = factory())
				{
					var dao = new TagDao(context);
					var probe = NewTag(1, "alpha", null);

					//act
					var removed = dao.Delete(probe);

					//assert
					removed.ShouldBe(1);
					probe.DeletedDate.ShouldNotBeNull();

					var stored = dao.ReachGetCore(NewTag(1, "alpha", null));

					stored.ShouldNotBeNull();
					stored.DeletedDate.ShouldNotBeNull();
					stored.DeletedDate.Value.ShouldBe(probe.DeletedDate.Value);
				}
			});
		}

		/// <summary>
		/// A second <c>Delete</c> returns <c>0</c>, does <b>not</b> refresh the existing <c>DeletedDate</c>, and
		/// writes nothing back — so the stamp always reports when the row was actually deleted.
		/// </summary>
		/// <remarks><c>Contract</c>: the <see cref="RootSoftNonIdDao{TEntity}.Delete"/> declaration.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReturnZeroAndNotRefreshAnExistingDeletedDate()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				DateTime firstStamp;

				using (var context = factory())
				{
					var dao = new TagDao(context);
					var probe = NewTag(1, "alpha", null);

					dao.Delete(probe);
					firstStamp = probe.DeletedDate.Value;
				}

				using (var context = factory())
				{
					var dao = new TagDao(context);
					var second = NewTag(1, "alpha", null);

					//act
					var removed = dao.Delete(second);
					var absent = dao.Delete(NewTag(9, "nowhere", null));

					//assert
					removed.ShouldBe(0);
					absent.ShouldBe(0);
					second.DeletedDate.ShouldBeNull();

					dao.ReachGetCore(NewTag(1, "alpha", null)).DeletedDate.Value.ShouldBe(firstStamp);
				}
			});
		}

		/// <summary>
		/// The retrieval trio omits deleted rows and agrees with itself, while <c>GetCore</c> still finds them.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: <c>ApplyReadFilter</c> adding <c>DeletedDate == null</c> and nothing else, which is
		/// what makes the trio agree, and <c>GetCore</c> not applying it, which is what lets a soft Data Access
		/// Object reach the rows it has already deleted.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldOmitDeletedRowsFromTheRetrievalTrioAndAgreeAcrossIt()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				using (var context = factory())
					new TagDao(context).Delete(NewTag(1, "alpha", null));

				using (var context = factory())
				{
					var dao = new TagDao(context);

					//act
					var all = dao.GetAll(null);
					var paged = dao.GetPaged(null, 0, 50);
					var count = dao.GetCount(null);

					//assert
					all.Count.ShouldBe(2);
					paged.Count.ShouldBe(2);
					count.ShouldBe(2);

					all.Any(x => x.Code == "alpha").ShouldBeFalse();
					dao.ReachGetCore(NewTag(1, "alpha", null)).ShouldNotBeNull();
				}
			});
		}

		/// <summary>
		/// <c>UpdateCore</c> stamps <c>UpdatedDate</c>, preserves the stored <c>CreatedDate</c> and
		/// <c>DeletedDate</c>, and ignores the incoming values of all three — so an update can neither rewrite
		/// history nor un-delete a row behind <c>Delete</c>'s back.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A30, the <c>UpdateCore</c> soft override declaration, and F6's <c>CreatedDate</c>
		/// clause. <b>Updating a deleted row is allowed</b> and leaves it deleted.
		/// </para>
		/// <para>
		/// The arrangement is the one that can fail: the caller's instance carries <c>DeletedDate = null</c> and
		/// a distinguishable wrong <c>CreatedDate</c>, so a <c>SetValues</c> that does not exclude or restore the
		/// three timestamps silently un-deletes the row and rewrites its creation time. An assertion made on an
		/// instance that happened to carry the stored values could not tell the difference.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPreserveTheStoredCreatedAndDeletedDatesThroughAnUpdate()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				DateTime storedCreated;
				DateTime storedDeleted;

				using (var context = factory())
				{
					var seed = new TagDao(context);
					var probe = NewTag(1, "alpha", null);

					seed.Delete(probe);

					var deleted = seed.ReachGetCore(probe);

					storedCreated = deleted.CreatedDate;
					storedDeleted = deleted.DeletedDate.Value;
				}

				using (var context = factory())
				{
					var dao = new TagDao(context);

					var edit = NewTag(1, "alpha", "edited while deleted");
					edit.CreatedDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					edit.UpdatedDate = new DateTime(1991, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					edit.DeletedDate = null;

					//act
					var written = dao.ReachUpdateCore(edit);

					//assert
					written.ShouldBe(1);
					edit.UpdatedDate.ShouldNotBe(new DateTime(1991, 1, 1, 0, 0, 0, DateTimeKind.Utc));

					var stored = dao.ReachGetCore(NewTag(1, "alpha", null));

					stored.Note.ShouldBe("edited while deleted");
					stored.CreatedDate.ShouldBe(storedCreated);
					stored.DeletedDate.ShouldNotBeNull();
					stored.DeletedDate.Value.ShouldBe(storedDeleted);

					dao.GetAll(null).Any(x => x.Code == "alpha").ShouldBeFalse();
				}
			});
		}

		/// <summary>
		/// An update that matched no row leaves the caller's <c>UpdatedDate</c> as it was found.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: the <c>UpdateCore</c> soft override declaration — <i>"only <c>UpdatedDate</c> travels
		/// back onto <c>item</c>, and only where a row was written: a call returning <c>0</c> … leaves the
		/// caller's <c>UpdatedDate</c> as it was found."</i> This is the same shape lap finding F10 requires of
		/// <c>Insert</c>.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldLeaveTheCallersUpdatedDateAloneWhenNoRowMatched()
		{
			WithStore(factory =>
			{
				//setup
				var carried = new DateTime(1995, 6, 7, 8, 9, 10, DateTimeKind.Utc);
				var absent = NewTag(9, "nowhere", "no such row");

				absent.UpdatedDate = carried;

				using (var context = factory())
				{
					var dao = new TagDao(context);

					//act
					var written = dao.ReachUpdateCore(absent);

					//assert
					written.ShouldBe(0);
					absent.UpdatedDate.ShouldBe(carried);
				}
			});
		}

		#endregion

		#region A34 — the pre-detach, on the branch whose mechanism differs

		/// <summary>
		/// A write computes its outcome from the <b>stored</b> row, not from an instance the shared context
		/// already has tracked.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A34. <b>The keyless leg is a separate obligation because the matching mechanism
		/// differs</b> — a keyed Data Access Object pre-detaches on <c>GetKey(item)</c> and never compiles
		/// <c>MatchRow</c>; a keyless one has no resolved key and must compile it. This is the arrangement the
		/// specification calls the sharpest form: a tracked instance carrying <c>DeletedDate = null</c> for a row
		/// that is already deleted. Without the pre-detach the tracking query performs identity resolution rather
		/// than re-reading, the implementation reads the tracked <c>null</c>, believes the row is live, and
		/// returns <c>1</c> while re-stamping a timestamp the contract forbids touching.
		/// </para>
		/// <para>
		/// Reachable in production under <c>ContextOwnership.Borrowed</c>, where the context's owner may have
		/// tracked entities this library never touched.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldComputeTheOutcomeFromTheStoredRowRatherThanATrackedOne()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				using (var context = factory())
					new TagDao(context).Delete(NewTag(1, "alpha", null));

				using (var context = factory())
				{
					var dao = new TagDao(context);

					// The stale instance: the row is deleted in the store, and this says otherwise.
					var stale = NewTag(1, "alpha", "first");
					stale.CreatedDate = DateTime.UtcNow;
					stale.DeletedDate = null;

					context.Attach(stale);

					//act
					var removed = dao.Delete(NewTag(1, "alpha", null));

					//assert
					removed.ShouldBe(0);
				}
			});
		}

		/// <summary>
		/// The same guard on <c>UpdateCore</c>: a stale tracked instance claiming the row is live must not be
		/// able to un-delete it.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A34, on the member the <c>Delete</c> leg above does not reach, and A30 —
		/// <i>"an update can neither rewrite history nor un-delete a row behind <c>Delete</c>'s back."</i> The
		/// two rules combine here: A30 has the soft <c>UpdateCore</c> restore the <b>stored</b>
		/// <c>DeletedDate</c>, and A34 is what makes "stored" mean the row rather than whatever the change
		/// tracker is holding. An implementation with A30 and without A34 reads the tracked <c>null</c>,
		/// restores <c>null</c>, and destroys the stamp.
		/// </para>
		/// <para>
		/// <b>The arrangement mutates an already-tracked instance rather than attaching a fabricated one, and
		/// that difference is load-bearing.</b> <c>Attach</c> takes an entry's <i>original</i> values from the
		/// instance handed to it, so a fabricated stale carrying <c>DeletedDate = null</c> has <c>null</c> as
		/// both original and current: the column is never modified, Entity Framework omits it from the
		/// <c>UPDATE</c>, and the stored stamp survives even with no pre-detach at all — the test would pass
		/// against the defect it was written for. Reading the row through the context gives the entry the
		/// stored values as its originals; mutating <c>DeletedDate</c> to <c>null</c> afterwards, without
		/// saving, makes the column genuinely modified, so an implementation that adopts the tracked value
		/// writes <c>NULL</c> to the store. This is the arrangement the obligation itself describes —
		/// <i>"an entity already tracked … and mutate the tracked instance without saving."</i>
		/// </para>
		/// <para>
		/// The returned <c>1</c> is asserted because the ROW COUNT RULE requires it, but it is identical under
		/// both implementations. <b>The store assertion is the discriminator.</b>
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldNotUnDeleteARowThroughAStaleTrackedInstanceOnUpdateCore()
		{
			WithStore(factory =>
			{
				//setup
				Setup_ThreeLiveTags(factory);

				DateTime storedDeleted;

				using (var context = factory())
				{
					var seed = new TagDao(context);
					var probe = NewTag(1, "alpha", null);

					seed.Delete(probe);
					storedDeleted = probe.DeletedDate.Value;
				}

				using (var context = factory())
				{
					// Tracked with the stored values as its originals, then told the row is live. Never saved.
					var tracked = context.Tags.Single(x => x.OwnerId == 1 && x.Code == "alpha");
					tracked.DeletedDate = null;

					var dao = new TagDao(context);
					var edit = NewTag(1, "alpha", "edited while deleted");
					edit.DeletedDate = null;

					//act
					var written = dao.ReachUpdateCore(edit);

					//assert
					written.ShouldBe(1);
				}

				//assert — against the store, which is the half the tracked instance can hide
				using (var context = factory())
				{
					var stored = context.Tags.Single(x => x.OwnerId == 1 && x.Code == "alpha");

					stored.DeletedDate.ShouldNotBeNull(
						"UpdateCore un-deleted the row. The stale tracked entry was not detached, so the " +
						"locating fetch resolved to it by identity rather than re-reading, and A30's " +
						"preserve-the-stored-DeletedDate step read the tracked null instead of the stamp.");

					stored.DeletedDate.Value.ShouldBe(storedDeleted);
					stored.Note.ShouldBe("edited while deleted");
				}
			});
		}

		#endregion

		#region the clock hook — F8's keyless half

		/// <summary>
		/// The clock's default is <see cref="DateTime.UtcNow"/> — asserted as a kind and a bracket, because an
		/// instant cannot be asserted exactly.
		/// </summary>
		/// <remarks><c>Contract</c>: the Timestamp Policy table, <i>GetCurrentTimestamp default</i>, on the keyless declaration site.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldStampWithUtcNowWhenTheClockIsNotOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(4, "epsilon", "default clock");

				using (var context = factory())
				{
					var dao = new TagDao(context);
					var before = DateTime.UtcNow;

					//act
					dao.Insert(tag);

					var after = DateTime.UtcNow;

					//assert
					tag.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					tag.CreatedDate.ShouldBeGreaterThanOrEqualTo(before);
					tag.CreatedDate.ShouldBeLessThanOrEqualTo(after);
				}
			});
		}

		/// <summary>
		/// The clock is read <b>once per stamping operation</b>, not once per value the operation writes.
		/// </summary>
		/// <remarks><c>Contract</c>: the Timestamp Policy table, <i>Called — once per stamping operation</i>.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReadTheClockOnceForEachStampingOperation()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(5, "zeta", "counted");

				using (var context = factory())
				{
					var dao = new CountingClockTagDao(context);

					//act & assert — one read per member, checked at each step so a failure names the member
					dao.Insert(tag);
					dao.ClockReads.ShouldBe(1);

					dao.ReachUpdateCore(tag);
					dao.ClockReads.ShouldBe(2);

					dao.Delete(tag);
					dao.ClockReads.ShouldBe(3);
				}
			});
		}

		/// <summary>
		/// An overridden clock is honored by all three stamping members, and each writes the instant that was
		/// current when it ran. Read back through the store rather than through a retrieval member, so the
		/// normalizer is not in the path.
		/// </summary>
		/// <remarks><c>Contract</c>: the Timestamp Policy table, and the four override sites A13 enumerates.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldStampWithTheValueAnOverriddenClockSupplies()
		{
			WithStore(factory =>
			{
				//setup
				var created = new DateTime(2020, 5, 17, 13, 45, 30, DateTimeKind.Utc);
				var updated = new DateTime(2021, 6, 18, 14, 46, 31, DateTimeKind.Utc);
				var deleted = new DateTime(2022, 7, 19, 15, 47, 32, DateTimeKind.Utc);
				var tag = NewTag(6, "eta", "fixed clock");

				using (var context = factory())
				{
					var dao = new SettableClockTagDao(context) { Now = created };

					//act
					dao.Insert(tag);

					dao.Now = updated;
					dao.ReachUpdateCore(tag);

					dao.Now = deleted;
					dao.Delete(tag);
				}

				//assert
				using (var context = factory())
				{
					var stored = context.Tags.Single(x => x.OwnerId == 6 && x.Code == "eta");

					stored.CreatedDate.ShouldBe(created);
					stored.UpdatedDate.ShouldBe(updated);
					stored.DeletedDate.ShouldBe(deleted);
				}
			});
		}

		#endregion

		#region the normalization hook — F8's keyless half, re-cut by F3

		/// <summary>
		/// The default normalizer <b>relabels; it never shifts</b>: a value arriving with
		/// <see cref="DateTimeKind.Unspecified"/> comes back carrying <see cref="DateTimeKind.Utc"/> and the same
		/// <see cref="DateTime.Ticks"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the Timestamp Policy table, <i>NormalizeRetrievedTimestamp default</i>. Asserted
		/// <b>directly against the hook</b>, per owner decision F3 Option C — no round trip, no provider. The
		/// superseded wording asserted a round trip and so turned a <c>Contract</c> obligation on a
		/// certified-provider fact: against a provider that preserved <c>Kind</c>, an identity normalizer passed
		/// it. This kills the identity normalizer with no store involved at all.
		/// </para>
		/// <para>
		/// <b>The ticks clause is the half that discriminates a wrongly-applied <c>ToUniversalTime()</c></b>, and
		/// lap finding <b>F7</b> is that it does so on any non-UTC machine and on no UTC one — a normal CI agent
		/// sits at UTC+00:00, where the two are extensionally identical. F7 is a property of the mechanism, not
		/// of this wording, and is not closed here. A non-zero time-of-day is used so a shift is visible at all.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRelabelWithoutShiftingWhenTheNormalizerDefaultIsCalledDirectly()
		{
			WithStore(factory =>
			{
				//setup
				var provided = new DateTime(2026, 8, 22, 13, 45, 30, DateTimeKind.Unspecified);

				using (var context = factory())
				{
					var dao = new TagDao(context);

					//act
					var normalized = dao.ReachNormalizeRetrievedTimestamp(provided);

					//assert
					normalized.Kind.ShouldBe(DateTimeKind.Utc);
					normalized.Ticks.ShouldBe(provided.Ticks);
				}
			});
		}

		/// <summary>
		/// The default clock and the default normalizer state one policy: the clock yields
		/// <see cref="DateTimeKind.Utc"/>, which is what the normalizer labels a retrieved value with.
		/// </summary>
		/// <remarks><c>Contract</c>: the Timestamp Policy table, both default rows, read together.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldYieldAUtcKindFromTheDefaultClock()
		{
			WithStore(factory =>
			{
				//setup
				using (var context = factory())
				{
					var dao = new TagDao(context);

					//act
					var now = dao.ReachGetCurrentTimestamp();

					//assert
					now.Kind.ShouldBe(DateTimeKind.Utc);
				}
			});
		}

		/// <summary>
		/// Every timestamp this Data Access Object's own reads materialize passes through the normalizer — on
		/// <c>GetCore</c>, <c>GetAll</c> and <c>GetPaged</c> alike.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the Timestamp Policy table, <i>Where it is applied</i>, and the F3 obligation's first
		/// clause — a Data Access Object whose normalizer yields a distinguishable sentinel sees that sentinel on
		/// all three retrieval members.
		/// </para>
		/// <para>
		/// <see cref="DateTimeKind.Local"/> is the sentinel because it is the one kind no relational provider
		/// materializes, so what is asserted is that the hook ran rather than what the store did. A <c>null</c>
		/// stays <c>null</c> and is never normalized into one.
		/// </para>
		/// </remarks>
		[Theory]
		[InlineData("GetCore")]
		[InlineData("GetAll")]
		[InlineData("GetPaged")]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRunTheNormalizerOnEveryTimestampTheRetrievalTrioMaterializes(string member)
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(7, "theta", "local policy");

				using (var context = factory())
				{
					var dao = new LocalTimeTagDao(context);

					dao.Insert(tag);
					dao.ReachUpdateCore(tag);
				}

				using (var context = factory())
				{
					var dao = new LocalTimeTagDao(context);

					//act
					var retrieved = Retrieve(dao, NewTag(7, "theta", null), member);

					//assert
					retrieved.ShouldNotBeNull();

					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);
					retrieved.CreatedDate.ShouldBe(tag.CreatedDate);

					retrieved.UpdatedDate.ShouldNotBeNull();
					retrieved.UpdatedDate.Value.Kind.ShouldBe(DateTimeKind.Local);

					retrieved.DeletedDate.ShouldBeNull();
				}
			});
		}

		#endregion

		#region the Timestamp Pair Rule — A13 as amended

		/// <summary>
		/// A custom policy that overrides both halves round-trips its own <see cref="DateTimeKind"/>: the value
		/// the stamping member wrote back carries the clock's kind, and the value the same Data Access Object
		/// retrieves carries it again.
		/// </summary>
		/// <remarks>
		/// <c>Characterization</c>. Nothing obliges an implementation to support a non-UTC policy — A13 states
		/// what a conforming <i>pairing</i> looks like, and the library checks neither half against the other.
		/// This pins the sanctioned shape on the keyless branch, not a requirement placed on a future
		/// implementation.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "Keyless")]
		public void ShouldRoundTripALocalTimePolicyWhenBothHooksAreOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(8, "iota", "local policy");

				using (var context = factory())
					new LocalTimeTagDao(context).Insert(tag);

				using (var context = factory())
				{
					var dao = new LocalTimeTagDao(context);

					//act
					var retrieved = dao.ReachGetCore(NewTag(8, "iota", null));

					//assert
					tag.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);
					retrieved.CreatedDate.ShouldBe(tag.CreatedDate);
				}
			});
		}

		/// <summary>
		/// The A13 hazard on the keyless branch: override the clock to a policy that does <b>not</b> yield UTC and
		/// leave the normalizer alone, and one stored instant answers with two different kinds — the write's
		/// instance says <see cref="DateTimeKind.Local"/>, the read's says <see cref="DateTimeKind.Utc"/>, with
		/// identical ticks. The retrieved value claims Coordinated Universal Time while holding a local
		/// wall-clock reading, and nothing signals it.
		/// </summary>
		/// <remarks>
		/// <c>Characterization</c>. This pins what a defect looks like, not behavior any implementation is obliged
		/// to reproduce. It is outside the F2 Option C carve-out precisely because the replacement clock does not
		/// yield UTC — the carve-out is keyed on what the clock yields, not on which member was overridden.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "Keyless")]
		public void ShouldRelabelALocalStampAsUtcWhenOnlyTheClockIsOverridden()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(9, "kappa", "half a policy");

				using (var context = factory())
					new ClockOnlyTagDao(context).Insert(tag);

				using (var context = factory())
				{
					var dao = new ClockOnlyTagDao(context);

					//act
					var retrieved = dao.ReachGetCore(NewTag(9, "kappa", null));

					//assert
					tag.CreatedDate.Kind.ShouldBe(DateTimeKind.Local);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.CreatedDate.Ticks.ShouldBe(tag.CreatedDate.Ticks);
				}
			});
		}

		/// <summary>
		/// <b>The F2 Option C carve-out.</b> A frozen clock that still yields UTC, with
		/// <c>NormalizeRetrievedTimestamp</c> left at its default, is <b>conforming</b>: the stamped instant and
		/// the retrieved one agree, and both carry <see cref="DateTimeKind.Utc"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the Timestamp Pair Rule as amended by owner decision F2 Option C, 2026-08-22 —
		/// <i>"overriding <c>GetCurrentTimestamp()</c> alone is conforming provided the replacement clock still
		/// yields UTC."</i> The obligation it discharges is the library-side half: an implementation whose default
		/// normalizer disagreed with a UTC clock would break the carve-out, and the ordinary way to inject a test
		/// clock with it.
		/// </para>
		/// <para>
		/// <b>It must not be pinned as a hazard.</b> A suite that treats every single override as a defect makes a
		/// fixed-instant test clock look non-conforming and pushes the next author into overriding a normalizer
		/// they have no reason to touch — which is the failure the amendment was made to prevent.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldConformWhenOnlyTheClockIsOverriddenAndItStillYieldsUtc()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(10, "lambda", "frozen clock");

				using (var context = factory())
					new FrozenClockTagDao(context).Insert(tag);

				using (var context = factory())
				{
					var dao = new FrozenClockTagDao(context);

					//act
					var retrieved = dao.ReachGetCore(NewTag(10, "lambda", null));

					//assert
					tag.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					tag.CreatedDate.Ticks.ShouldBe(FrozenClockTagDao.Frozen.Ticks);

					retrieved.ShouldNotBeNull();
					retrieved.CreatedDate.Kind.ShouldBe(DateTimeKind.Utc);
					retrieved.CreatedDate.Ticks.ShouldBe(FrozenClockTagDao.Frozen.Ticks);
				}
			});
		}

		#endregion

		#region BaseSoftNonIdDao — the published shape reaches the soft cores

		/// <summary>
		/// <c>BaseSoftNonIdDao.Update</c> cannot be made to bypass timestamp preservation through a hard-update
		/// core, and its <c>Get</c> returns a soft-deleted row.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: the <see cref="BaseSoftNonIdDao{TEntity}"/> declaration — <i>"the core is an
		/// <c>override</c>, so a caller holding a <see cref="RootNonIdDao{TEntity}"/>-typed reference still
		/// reaches the soft body."</i> The published members are thin publishers over the soft cores and add no
		/// behavior; this is the assertion that they publish the <i>soft</i> ones.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReachTheSoftCoresThroughThePublishedGetAndUpdate()
		{
			WithStore(factory =>
			{
				//setup
				var tag = NewTag(11, "mu", "published shape");

				using (var context = factory())
				{
					var dao = new PublishedTagDao(context);

					dao.Insert(tag);
					dao.Delete(tag);
				}

				using (var context = factory())
				{
					var dao = new PublishedTagDao(context);

					var edit = NewTag(11, "mu", "edited while deleted");
					edit.DeletedDate = null;
					edit.CreatedDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

					//act
					var written = dao.Update(edit);
					var retrieved = dao.Get(NewTag(11, "mu", null));

					//assert
					written.ShouldBe(1);

					retrieved.ShouldNotBeNull();
					retrieved.Note.ShouldBe("edited while deleted");
					retrieved.DeletedDate.ShouldNotBeNull();
					retrieved.DeletedDate.Value.ShouldBe(tag.DeletedDate.Value);
					retrieved.CreatedDate.ShouldBe(tag.CreatedDate);
				}
			});
		}

		#endregion
	}
}
