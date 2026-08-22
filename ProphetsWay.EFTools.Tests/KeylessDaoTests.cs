using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// The two <b>hard</b> keyless families — <see cref="RootNonIdDao{TEntity}"/> and
	/// <see cref="BaseNonIdDao{TEntity}"/> — over an entity whose identity is a pair of stored scalars and not a
	/// single identifier property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Why these cannot live upstream.</b> <c>ProphetsWay.Example</c> has exactly one keyless entity,
	/// <c>CompanyResource</c>, and its Data Access Object has no vocabulary for <c>MatchRow</c>,
	/// <c>ApplyStableOrder</c>, <c>GetCore</c> or <c>UpdateCore</c> — those four are this library's surface, and
	/// the routing table in <c>docs/api-contract.md</c> marks the <i>Hooks and overrides</i> group <b>new
	/// local</b> for exactly that reason. The upstream suite also has no keyless <i>read</i> Data Access Object
	/// at all, which is the separate reason the A15 obligations are local.
	/// </para>
	/// <para>
	/// <b>SQLite in-memory, never the InMemory provider.</b> Two obligations here turn on the store enforcing a
	/// primary key — the non-upsert guard and the key-property exclusion — and a non-relational provider enforces
	/// neither, so both would pass while asserting nothing. <c>Constants.cs</c> and its SQL Server connection
	/// string are deliberately untouched; nothing in this file needs a local server, which is what
	/// <c>Area=Keyless</c> is for.
	/// </para>
	/// <para>
	/// <b>Soft-delete keyless behavior is not here.</b> <see cref="RootSoftNonIdDao{TEntity}"/> and
	/// <see cref="BaseSoftNonIdDao{TEntity}"/> have their own file, because the timestamp pair rule and the
	/// virtual-dispatch guard are the whole subject of that half.
	/// </para>
	/// </remarks>
	public class KeylessDaoTests
	{
		#region fixture entities

		/// <summary>
		/// The worked keyless shape: a join whose natural key is the pair, mapped as the primary key, plus one
		/// writable column of its own so an update has something to write.
		/// </summary>
		public class Link : IBaseEntity
		{
			public int LeftId { get; set; }

			public int RightId { get; set; }

			public string Note { get; set; }
		}

		/// <summary>
		/// The A35 subject. Its primary key is <c>(TenantId, Code)</c> and its Data Access Object locates by
		/// <see cref="Code"/> alone, holding the tenant itself — so a caller's <see cref="TenantId"/> genuinely
		/// differs from the stored one and a copy that does not exclude key properties throws.
		/// </summary>
		public class Registration : IBaseEntity
		{
			public int TenantId { get; set; }

			public string Code { get; set; }

			public string Label { get; set; }
		}

		#endregion

		#region fixture context and data access objects

		public class KeylessContext : DbContext
		{
			public KeylessContext(DbContextOptions<KeylessContext> options) : base(options)
			{
			}

			public DbSet<Link> Links { get; set; }

			public DbSet<Registration> Registrations { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.Entity<Link>().HasKey(x => new { x.LeftId, x.RightId });
				modelBuilder.Entity<Registration>().HasKey(x => new { x.TenantId, x.Code });
			}
		}

		/// <summary>
		/// Overrides <c>MatchRow</c> and nothing else — the ordinary state of the base class (A15), and the
		/// subject of every obligation about an un-overridden <c>ApplyStableOrder</c>.
		/// </summary>
		public class LinkDao : RootNonIdDao<Link>
		{
			public LinkDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Link, bool>> MatchRow(Link item)
			{
				return x => x.LeftId == item.LeftId && x.RightId == item.RightId;
			}
		}

		/// <summary>
		/// The same un-overridden shape as <see cref="LinkDao"/>, <b>named so that its type name does not contain
		/// the entity's type name</b>.
		/// </summary>
		/// <remarks>
		/// It exists for one reason: on <see cref="LinkDao"/> the string <c>"Link"</c> is a substring of
		/// <c>"LinkDao"</c>, so an M7 assertion requiring the message to contain the entity name is satisfied by the
		/// Data Access Object name alone and a message omitting <c>{EntityTypeName}</c> entirely still passes. Here
		/// the two names share no substring, so the two assertions can fail independently.
		/// </remarks>
		public class UnorderedDao : RootNonIdDao<Link>
		{
			public UnorderedDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Link, bool>> MatchRow(Link item)
			{
				return x => x.LeftId == item.LeftId && x.RightId == item.RightId;
			}
		}

		/// <summary>
		/// The same Data Access Object with an ordering, plus public pass-throughs to the two <c>protected</c>
		/// cores.
		/// </summary>
		/// <remarks>
		/// The pass-throughs add no behavior and exist because a <c>Root</c> type publishes nothing: there is no
		/// other way to reach <c>GetCore</c> and <c>UpdateCore</c> at the declaration site where they are
		/// specified. Publishing them here is a test instrument, not the shape a consumer takes —
		/// <see cref="BaseNonIdDao{TEntity}"/> is that, and <see cref="PublishedLinkDao"/> is its subject.
		/// </remarks>
		public class OrderedLinkDao : RootNonIdDao<Link>
		{
			public OrderedLinkDao(DbContext context) : base(context)
			{
			}

			public int OrderingsApplied { get; private set; }

			public Link ReachGetCore(Link item)
			{
				return GetCore(item);
			}

			public int ReachUpdateCore(Link item)
			{
				return UpdateCore(item);
			}

			protected override Expression<Func<Link, bool>> MatchRow(Link item)
			{
				return x => x.LeftId == item.LeftId && x.RightId == item.RightId;
			}

			protected override IOrderedQueryable<Link> ApplyStableOrder(IQueryable<Link> query)
			{
				OrderingsApplied++;

				return query.OrderBy(x => x.LeftId).ThenBy(x => x.RightId);
			}
		}

		/// <summary>The opt-in shape: the same plumbing, publishing the <see cref="IBaseDao{T}"/> surface.</summary>
		public class PublishedLinkDao : BaseNonIdDao<Link>
		{
			public PublishedLinkDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Link, bool>> MatchRow(Link item)
			{
				return x => x.LeftId == item.LeftId && x.RightId == item.RightId;
			}

			protected override IOrderedQueryable<Link> ApplyStableOrder(IQueryable<Link> query)
			{
				return query.OrderBy(x => x.LeftId).ThenBy(x => x.RightId);
			}
		}

		/// <summary>
		/// <b>Deliberately under-specified.</b> Its <c>MatchRow</c> names one half of the pair, so it is the
		/// keyless equivalent of a non-unique key.
		/// </summary>
		public class AmbiguousLinkDao : RootNonIdDao<Link>
		{
			public AmbiguousLinkDao(DbContext context) : base(context)
			{
			}

			public Link ReachGetCore(Link item)
			{
				return GetCore(item);
			}

			protected override Expression<Func<Link, bool>> MatchRow(Link item)
			{
				return x => x.LeftId == item.LeftId;
			}

			protected override IOrderedQueryable<Link> ApplyStableOrder(IQueryable<Link> query)
			{
				return query.OrderBy(x => x.LeftId).ThenBy(x => x.RightId);
			}
		}

		/// <summary>Locates by <c>Code</c> alone; the tenant is the Data Access Object's, not the caller's.</summary>
		public class RegistrationDao : BaseNonIdDao<Registration>
		{
			public RegistrationDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Registration, bool>> MatchRow(Registration item)
			{
				return x => x.Code == item.Code;
			}

			protected override IOrderedQueryable<Registration> ApplyStableOrder(IQueryable<Registration> query)
			{
				return query.OrderBy(x => x.TenantId).ThenBy(x => x.Code);
			}
		}

		/// <summary>
		/// A purpose-built Data Access Layer forwarding <c>GetAll(Link?)</c> over an <see cref="UnorderedDao"/> — the
		/// R4-S6 subject, and the only route by which A15's refusal can be observed <i>through the dispatcher</i>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <b>It cannot be borrowed from the seam.</b> <c>CompanyResourceDao</c> is specified to override
		/// <c>ApplyStableOrder</c>, so the only keyless Data Access Object the upstream suite reaches never throws
		/// this — the obligation is discharged here or nowhere.
		/// </para>
		/// <para>
		/// <c>ContextOwnership.Borrowed</c>, because the test's own <c>using</c> owns the context. The forwarder
		/// calls <c>ThrowIfDisposed()</c> first, as every member a derived layer declares must.
		/// </para>
		/// </remarks>
		public class UnorderedLinkDataAccess : BaseEFDataAccess<KeylessContext>
		{
			private readonly UnorderedDao _linkDao;

			public UnorderedLinkDataAccess(KeylessContext context) : base(context, ContextOwnership.Borrowed)
			{
				_linkDao = new UnorderedDao(Context);
			}

			public IList<Link> GetAll(Link item)
			{
				ThrowIfDisposed();

				return _linkDao.GetAll(item);
			}
		}

		#endregion

		#region apparatus

		private static void WithStore(Action<Func<KeylessContext>> body)
		{
			WithStore((factory, recorder) => body(factory));
		}

		/// <summary>
		/// The same store, handing the body the interceptor that recorded every command sent to the provider.
		/// </summary>
		/// <remarks>
		/// <see cref="KeyPredicateOpenKeyTests.CommandRecorder"/> is reused rather than reimplemented — it already
		/// lives in this assembly and already captures <c>CommandText</c>. The recorder is wired on every store, not
		/// only the one test that reads it, so the two overloads cannot diverge; recording is the interceptor's only
		/// effect.
		/// </remarks>
		private static void WithStore(Action<Func<KeylessContext>, KeyPredicateOpenKeyTests.CommandRecorder> body)
		{
			var recorder = new KeyPredicateOpenKeyTests.CommandRecorder();

			using (var connection = new SqliteConnection("Filename=:memory:"))
			{
				// Held open for the whole body: closing it discards the in-memory database.
				connection.Open();

				var options = new DbContextOptionsBuilder<KeylessContext>()
					.UseSqlite(connection)
					.AddInterceptors(recorder)
					.Options;

				Func<KeylessContext> factory = () => new KeylessContext(options);

				using (var schema = factory())
					schema.Database.EnsureCreated();

				body(factory, recorder);
			}
		}

		private static Link NewLink(int left, int right, string note)
		{
			return new Link { LeftId = left, RightId = right, Note = note };
		}

		/// <summary>Six links, seeded through the context so no member under test is inside the arrangement.</summary>
		private static void Setup_SixLinks(Func<KeylessContext> factory)
		{
			using (var context = factory())
			{
				for (var left = 1; left <= 2; left++)
					for (var right = 1; right <= 3; right++)
						context.Links.Add(NewLink(left, right, $"link {left}-{right}"));

				context.SaveChanges();
			}
		}

		/// <summary>The retrieval trio, reached one at a time so a failure names which member refused.</summary>
		private static object Retrieve(RootNonIdDao<Link> dao, string member)
		{
			switch (member)
			{
				case "GetAll":
					return dao.GetAll(null);

				case "GetPaged":
					return dao.GetPaged(null, 0, 10);

				case "GetCount":
					return dao.GetCount(null);

				default:
					throw new ArgumentOutOfRangeException(nameof(member), member, "Not a retrieval member of this family.");
			}
		}

		#endregion

		#region the shape — what a Root type publishes, and what it does not

		/// <summary>
		/// <c>MatchRow</c> is abstract and <c>ApplyStableOrder</c> is virtual — the two hooks, one required at
		/// compile time and one at first use.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: <i>The two hooks — one required at compile time, one at first use</i>. Reversing
		/// either is a design change and not an implementation detail: an abstract <c>ApplyStableOrder</c> taxes
		/// the write-only join Data Access Object S5 was written for, and a virtual <c>MatchRow</c> would give a
		/// default predicate there is nothing to derive.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldDeclareMatchRowAbstractAndApplyStableOrderVirtual()
		{
			//setup
			const BindingFlags declared = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			//act
			var matchRow = typeof(RootNonIdDao<Link>).GetMethod("MatchRow", declared);
			var applyStableOrder = typeof(RootNonIdDao<Link>).GetMethod("ApplyStableOrder", declared);

			//assert
			matchRow.ShouldNotBeNull();
			matchRow.IsAbstract.ShouldBeTrue();

			applyStableOrder.ShouldNotBeNull();
			applyStableOrder.IsVirtual.ShouldBeTrue();
			applyStableOrder.IsAbstract.ShouldBeFalse();
		}

		/// <summary>
		/// A <c>Root</c> type implements no <c>ProphetsWay.BaseDataAccess</c> capability interface and publishes
		/// neither <c>Get</c> nor <c>Update</c>. That is the whole reason it exists.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: the <c>Base</c>/<c>Root</c> prefix convention and S5. <c>ICompanyResourceDao</c> rule
		/// 8 rests on this — a published <c>Get</c> on the base is what would put it at risk — so this is the
		/// structural guard behind a rule the upstream suite can only assert the outcome of.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPublishNeitherGetNorUpdateOnTheRootKeylessBase()
		{
			//setup
			var parent = typeof(IBaseEntity).Assembly;

			//act
			var capabilities = typeof(RootNonIdDao<Link>)
				.GetInterfaces()
				.Where(x => x.Assembly == parent)
				.Select(x => x.FullName)
				.ToList();

			//assert
			capabilities.ShouldBeEmpty();

			typeof(RootNonIdDao<Link>).GetMethod("Get", new[] { typeof(Link) }).ShouldBeNull();
			typeof(RootNonIdDao<Link>).GetMethod("Update", new[] { typeof(Link) }).ShouldBeNull();
		}

		/// <summary>
		/// A <c>Base</c> type declares <see cref="IBaseDao{T}"/> and publishes the two cores its root keeps
		/// <c>protected</c>. It adds no behavior beyond that.
		/// </summary>
		/// <remarks><c>Contract</c>: the public-surface table, rows 10 and 12, and the <c>Base</c>/<c>Root</c> convention.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPublishGetAndUpdateOnTheKeylessBaseThatDeclaresIBaseDao()
		{
			//act
			var declared = typeof(IBaseDao<Link>).IsAssignableFrom(typeof(BaseNonIdDao<Link>));

			//assert
			declared.ShouldBeTrue();

			typeof(BaseNonIdDao<Link>).GetMethod("Get", new[] { typeof(Link) }).ShouldNotBeNull();
			typeof(BaseNonIdDao<Link>).GetMethod("Update", new[] { typeof(Link) }).ShouldNotBeNull();
		}

		#endregion

		#region A15 — the ordering hook throws until it is overridden

		/// <summary>
		/// All three retrieval members refuse until <c>ApplyStableOrder</c> is supplied — <c>GetCount</c>
		/// included, even though counting needs no order, because the trio is contractually bound to agree.
		/// </summary>
		/// <remarks><c>Contract</c>: A15, and the member table in <i>The two hooks</i>.</remarks>
		[Theory]
		[InlineData("GetAll")]
		[InlineData("GetPaged")]
		[InlineData("GetCount")]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRefuseEveryRetrievalMemberWhenApplyStableOrderIsNotOverridden(string member)
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new LinkDao(context);

					//act
					var thrown = Record.Exception(() => Retrieve(dao, member));

					//assert
					thrown.ShouldBeOfType<NotSupportedException>();
				}
			});
		}

		/// <summary>
		/// The refusal names the Data Access Object type, the entity type and the hook that is missing. A message
		/// that merely said "not supported" would be strictly worse than the abstract method A15 declined.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A15's message template and M7. Asserted as the presence of the three names rather
		/// than as the whole sentence, so the wording can be improved without breaking this — the two
		/// placeholders are the contract term, the surrounding words are not.
		/// </para>
		/// <para>
		/// <b>The subject is <see cref="UnorderedDao"/> and not <see cref="LinkDao"/>, and that is the whole
		/// reason it exists.</b> <c>"Link"</c> is a substring of <c>"LinkDao"</c>, so on that Data Access Object
		/// the entity-name assertion is satisfied by the Data Access Object name alone and a message omitting
		/// <c>{EntityTypeName}</c> entirely still passes it. The two names here share no substring, so each of
		/// the three assertions can fail on its own.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldNameTheDaoTypeTheEntityTypeAndTheHookInTheRefusal()
		{
			WithStore(factory =>
			{
				//setup
				using (var context = factory())
				{
					var dao = new UnorderedDao(context);

					//act
					var thrown = Record.Exception(() => dao.GetAll(null));

					//assert
					thrown.ShouldBeOfType<NotSupportedException>();
					thrown.Message.ShouldContain(nameof(UnorderedDao));
					thrown.Message.ShouldContain(nameof(Link));
					thrown.Message.ShouldContain("ApplyStableOrder");
				}
			});
		}

		/// <summary>
		/// The same refusal surfaces from a <b>dispatcher entry point</b>: <c>dal.GetAll&lt;Link&gt;()</c> on a
		/// Data Access Layer forwarding <c>GetAll(Link?)</c> for an un-overridden Data Access Object throws
		/// <see cref="NotSupportedException"/> — not <see cref="DataAccessConventionException"/>, and not a
		/// wrapped form of either.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Dispatcher</c>, not <c>Contract</c>: the subject is the reflection convention in
		/// <c>ProphetsWay.BaseDataAccess</c> and what reaches a caller through it, which belongs to no single
		/// Data Access Object. It traces to <b>R4-S6</b> — <i>"the same exception surfaces from the dispatcher
		/// entry point … not <c>DataAccessConventionException</c>, and not a wrapped form of either"</i> — the
		/// one <c>[D]</c> obligation of this lap.
		/// </para>
		/// <para>
		/// <b>Here or nowhere.</b> <c>CompanyResourceDao</c> is specified to override <c>ApplyStableOrder</c>, so
		/// the only keyless Data Access Object the seam exposes never throws this and no adapted upstream test
		/// can reach it.
		/// </para>
		/// <para>
		/// <b>The two negative assertions are the ones that discriminate</b>, and they fail in opposite
		/// directions. <see cref="DataAccessConventionException"/> would mean the forwarder's signature did not
		/// match the convention — the dispatcher never reached the Data Access Object at all — which would
		/// falsify S5's <i>"conventional public methods a custom Data Access Object interface can bind to."</i>
		/// <see cref="TargetInvocationException"/> would mean the reflection layer stopped unwrapping, which is
		/// the 3.0.0 breaking change the parent made and which every exception term in this document rests on.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Dispatcher")]
		[Trait("Area", "Keyless")]
		public void ShouldSurfaceNotSupportedExceptionFromTheDispatcherEntryPoint()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				using (var dal = new UnorderedLinkDataAccess(context))
				{
					//act
					var thrown = Record.Exception(() => dal.GetAll<Link>());

					//assert
					thrown.ShouldNotBeNull();

					thrown.ShouldNotBeOfType<DataAccessConventionException>(
						"The dispatcher reported a convention failure instead of reaching the forwarder. " +
						"UnorderedLinkDataAccess declares a public IList<Link> GetAll(Link), which is the " +
						"signature the convention requires, so a DataAccessConventionException here means the " +
						"forwarder was never invoked and A15's refusal was never reached.");

					thrown.ShouldNotBeOfType<TargetInvocationException>(
						"The reflection layer re-wrapped the exception. ProphetsWay.BaseDataAccess 3.0.0 " +
						"propagates exceptions from derived Data Access Layer methods unwrapped, and every " +
						"exception term in this library rests on that.");

					thrown.ShouldBeOfType<NotSupportedException>();
				}
			});
		}

		/// <summary>
		/// A write-only join Data Access Object is fully functional with <c>MatchRow</c> alone: the members that
		/// order nothing are unaffected by the refusal above.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: A15's trade, stated on <see cref="RootNonIdDao{TEntity}"/>'s own remarks — <i>"a
		/// write-only join Data Access Object is fully functional with <c>MatchRow</c> alone."</i> Without this
		/// half the exception has no counterpart and an implementation could satisfy A15 by refusing everything.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldInsertAndDeleteWithNoApplyStableOrderOverride()
		{
			WithStore(factory =>
			{
				//setup
				var link = NewLink(9, 9, "write only");

				using (var context = factory())
				{
					var dao = new LinkDao(context);

					//act
					dao.Insert(link);
					var removed = dao.Delete(NewLink(9, 9, null));

					//assert
					removed.ShouldBe(1);
				}

				using (var context = factory())
					context.Links.Count().ShouldBe(0);
			});
		}

		/// <summary>
		/// Argument validation comes first: a negative window on an un-overridden Data Access Object yields
		/// <see cref="ArgumentOutOfRangeException"/>, not the refusal.
		/// </summary>
		/// <remarks><c>Contract</c>: the <c>GetPaged</c> declaration — <i>"thrown after the argument checks"</i>.</remarks>
		[Theory]
		[InlineData(-1, 10)]
		[InlineData(0, -1)]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldValidateThePagingArgumentsBeforeRefusingToOrder(int skip, int take)
		{
			WithStore(factory =>
			{
				//setup
				using (var context = factory())
				{
					var dao = new LinkDao(context);

					//act
					var thrown = Record.Exception(() => dao.GetPaged(null, skip, take));

					//assert
					thrown.ShouldBeOfType<ArgumentOutOfRangeException>();
				}
			});
		}

		#endregion

		#region reads, once the ordering is supplied

		/// <summary>
		/// With the hook in place the trio works and agrees: successive windows partition a full pass with no
		/// overlap and no omission, and the count matches.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: <i>A keyless Data Access Object paginates correctly through its
		/// <c>ApplyStableOrder</c> hook</i>, and <c>GetCount</c> equalling <c>GetAll().Count</c>. On this family
		/// there is no model-derived fallback, so totality is entirely the override's to deliver.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldPartitionAFullPassWithSuccessiveWindows()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var all = dao.GetAll(null);
					var count = dao.GetCount(null);

					var windows = new List<Link>();

					for (var skip = 0; skip < 6; skip += 2)
						windows.AddRange(dao.GetPaged(null, skip, 2));

					//assert
					all.Count.ShouldBe(6);
					count.ShouldBe(all.Count);

					windows.Count.ShouldBe(all.Count);
					windows.Select(x => $"{x.LeftId}-{x.RightId}")
						.ShouldBe(all.Select(x => $"{x.LeftId}-{x.RightId}"));
				}
			});
		}

		/// <summary>
		/// <c>GetCount</c> invokes the ordering hook and discards what it returns (A27), which is the mechanism
		/// by which an un-overridden hook makes counting refuse — and <b>no <c>ORDER BY</c> reaches the store</b>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A27, and <c>GetCount</c>'s own remarks — <i>"invoked and its result discarded … an
		/// override with a side effect runs exactly once per call"</i> and <i>"materializes no entity, so it
		/// includes nothing and emits no <c>ORDER BY</c>."</i>
		/// </para>
		/// <para>
		/// <b>The two halves are the whole assertion and neither is sufficient.</b> The counter alone cannot
		/// distinguish invoked-and-discarded from invoked-and-used; the emitted command alone cannot distinguish
		/// discarded from never-invoked, and never-invoked is what would make an un-overridden Data Access Object
		/// count happily while its two partners refused. The recorder is cleared immediately before the first
		/// call, so the schema and seed commands are out of scope.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldInvokeTheOrderingHookExactlyOncePerCount()
		{
			WithStore((factory, recorder) =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					recorder.Clear();

					//act
					dao.GetCount(null);

					//assert
					dao.OrderingsApplied.ShouldBe(1);

					recorder.Commands.ShouldNotBeEmpty();
					recorder.Commands
						.Where(x => x.CommandText.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) >= 0)
						.ShouldBeEmpty(
							"GetCount emitted an ORDER BY. A27 has the ordering hook invoked and its returned " +
							"query thrown away; a command carrying ORDER BY means the hook's output was counted " +
							"rather than discarded.");

					//act — a second call, so "once per call" is distinguished from "once ever"
					dao.GetCount(null);

					//assert
					dao.OrderingsApplied.ShouldBe(2);
				}
			});
		}

		/// <summary>
		/// The <c>item</c> parameter of the retrieval trio is a type selector only, and is <c>null</c> whenever
		/// the call arrives through the dispatcher — so none of the three may read it.
		/// </summary>
		/// <remarks><c>Contract</c>: S12, and <c>ICompanyResourceDao</c> rule 6.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldAcceptANullTypeSelectorOnEveryRetrievalMember()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var all = dao.GetAll(null);
					var page = dao.GetPaged(null, 0, 2);
					var count = dao.GetCount(null);

					//assert
					all.Count.ShouldBe(6);
					page.Count.ShouldBe(2);
					count.ShouldBe(6);
				}
			});
		}

		#endregion

		#region GetCore

		/// <summary>
		/// <c>GetCore</c> hands back a fresh untracked snapshot of the located row — never the argument, and
		/// never the store's own tracked object.
		/// </summary>
		/// <remarks><c>Contract</c>: the <c>GetCore</c> declaration, and <c>ICompanyResourceDao</c> rule 9.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReturnAFreshSnapshotFromGetCoreRatherThanTheArgument()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);
					var probe = NewLink(1, 2, null);

					//act
					var found = dao.ReachGetCore(probe);

					//assert
					found.ShouldNotBeNull();
					found.ShouldNotBeSameAs(probe);
					found.Note.ShouldBe("link 1-2");

					context.ChangeTracker.Entries<Link>().ShouldBeEmpty();
				}
			});
		}

		/// <summary>A row that was never stored answers <c>null</c> rather than throwing.</summary>
		/// <remarks><c>Contract</c>: the <c>GetCore</c> declaration — <i>"or <c>null</c> when no row matches"</i>.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReturnNullFromGetCoreWhenNoRowMatches()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var found = dao.ReachGetCore(NewLink(99, 99, null));

					//assert
					found.ShouldBeNull();
				}
			});
		}

		/// <summary>
		/// An under-specified <c>MatchRow</c> is the keyless equivalent of a non-unique key: <c>SingleOrDefault</c>
		/// throws rather than silently choosing a row.
		/// </summary>
		/// <remarks><c>Contract</c>: the <c>MatchRow</c> declaration, and the <i>Keyless member contracts</i> table.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldThrowWhenMatchRowMatchesMoreThanOneRow()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new AmbiguousLinkDao(context);

					//act
					var thrown = Record.Exception(() => dao.ReachGetCore(NewLink(1, 2, null)));

					//assert
					thrown.ShouldBeOfType<InvalidOperationException>();
				}
			});
		}

		#endregion

		#region the published surface — BaseNonIdDao.Get

		/// <summary>
		/// The published <c>Get</c> retrieves the stored row: the located values, a fresh instance rather than
		/// the probe, and <c>null</c> for a row that was never stored.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: the <see cref="BaseNonIdDao{TEntity}"/> declaration — <i>"<c>GetCore</c>, published.
		/// Contract unchanged"</i> — and through it the <c>GetCore</c> declaration's <i>"a fresh untracked
		/// snapshot, or <c>null</c> when no row matches."</i>
		/// </para>
		/// <para>
		/// <b>This is the only test that calls <c>Get</c> on a row that exists.</b> The shape test proves the
		/// member is declared and the null-argument theory proves it refuses <c>null</c>; between them an
		/// implementation returning a constant <c>null</c> passes both. Nothing upstream closes the gap either —
		/// <c>CompanyResourceDao</c> is specified onto <see cref="RootNonIdDao{TEntity}"/>, which publishes no
		/// <c>Get</c>, so the seam never reaches this type. A published member of a published type in a major
		/// release is otherwise entirely unexercised.
		/// </para>
		/// <para>
		/// The <c>ShouldNotBeSameAs</c> clause is the snapshot half: a published member that handed back its own
		/// argument, or the store's tracked object, would satisfy the value assertions and fail this one.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRetrieveAStoredRowThroughThePublishedGet()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new PublishedLinkDao(context);
					var probe = NewLink(1, 2, null);

					//act
					var found = dao.Get(probe);
					var absent = dao.Get(NewLink(99, 99, null));

					//assert
					found.ShouldNotBeNull();
					found.ShouldNotBeSameAs(probe);
					found.LeftId.ShouldBe(1);
					found.RightId.ShouldBe(2);
					found.Note.ShouldBe("link 1-2");

					context.ChangeTracker.Entries<Link>().ShouldBeEmpty();

					absent.ShouldBeNull();
				}
			});
		}

		#endregion

		#region Insert

		/// <summary>
		/// The store receives a <b>copy</b>; the caller's instance is never handed to the change tracker, during
		/// the write or after it.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A32, restated on <see cref="RootNonIdDao{TEntity}.Insert"/> — <i>"the caller's
		/// instance is never handed to the change tracker."</i>
		/// </para>
		/// <para>
		/// The tracker is inspected at the moment of the write rather than only afterwards, because detaching
		/// afterwards would satisfy the weaker reading while still having adopted the instance. <c>Entry(item)</c>
		/// is deliberately not the instrument: calling it on an untracked instance <i>creates</i> a
		/// <c>Detached</c> entry, which is exactly how a conforming implementation builds the copy.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldNeverTrackTheCallersInstanceOnInsert()
		{
			WithStore(factory =>
			{
				//setup
				var link = NewLink(4, 4, "a copy is stored");
				var trackedDuringTheWrite = true;

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					context.SavingChanges += (sender, args) =>
						trackedDuringTheWrite = context.ChangeTracker
							.Entries()
							.Any(entry => ReferenceEquals(entry.Entity, link));

					//act
					dao.Insert(link);

					//assert
					trackedDuringTheWrite.ShouldBeFalse();

					context.ChangeTracker
						.Entries()
						.Any(entry => ReferenceEquals(entry.Entity, link))
						.ShouldBeFalse();
				}
			});
		}

		/// <summary>
		/// <c>Insert</c> is neither idempotent nor an upsert: a pair the store already holds is a duplicate, the
		/// provider's violation propagates, and the stored row is <b>not</b> overwritten.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: <see cref="RootNonIdDao{TEntity}.Insert"/>'s remarks — <i>"not idempotent, and not an
		/// upsert … the provider's uniqueness or primary-key violation propagates unwrapped."</i>
		/// </para>
		/// <para>
		/// The unchanged-row assertion is the 2.2.x <c>AddOrUpdate</c> regression guard; the two negative type
		/// assertions are what "unwrapped" means from the caller's side. A silent no-op is a Data Access Object's
		/// own override to write — <c>ICompanyResourceDao</c> rule 3 — and is deliberately not offered here.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRefuseToOverwriteAStoredPairOnASecondInsert()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var thrown = Record.Exception(() => dao.Insert(NewLink(1, 2, "overwritten")));

					//assert
					thrown.ShouldNotBeNull();
					thrown.ShouldNotBeOfType<DbUpdateConcurrencyException>();
					thrown.ShouldNotBeOfType<TargetInvocationException>();
				}

				using (var context = factory())
				{
					context.Links.Count().ShouldBe(6);
					context.Links.Single(x => x.LeftId == 1 && x.RightId == 2).Note.ShouldBe("link 1-2");
				}
			});
		}

		#endregion

		#region Delete

		/// <summary>
		/// A hard delete: <c>1</c> when the row existed, <c>0</c> when it did not, and idempotent — a second call
		/// returns <c>0</c> and throws nothing.
		/// </summary>
		/// <remarks><c>Contract</c>: the <c>Delete</c> declaration, and <c>ICompanyResourceDao</c> rule 4.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReportOneThenZeroFromRepeatedDeletes()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var first = dao.Delete(NewLink(1, 2, null));
					var second = dao.Delete(NewLink(1, 2, null));
					var absent = dao.Delete(NewLink(99, 99, null));

					//assert
					first.ShouldBe(1);
					second.ShouldBe(0);
					absent.ShouldBe(0);
				}

				using (var context = factory())
					context.Links.Count().ShouldBe(5);
			});
		}

		#endregion

		#region UpdateCore

		/// <summary>
		/// <c>UpdateCore</c> reports whether a row <i>matched</i>, not whether a value <i>changed</i> — so an
		/// update carrying the stored values returns <c>1</c>, and one matching nothing returns <c>0</c>.
		/// </summary>
		/// <remarks><c>Contract</c>: the ROW COUNT RULE, restated on the <c>UpdateCore</c> declaration.</remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldReportAMatchRatherThanAChangeFromUpdateCore()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					//act
					var changed = dao.ReachUpdateCore(NewLink(1, 2, "a new note"));
					var identical = dao.ReachUpdateCore(NewLink(1, 3, "link 1-3"));
					var absent = dao.ReachUpdateCore(NewLink(99, 99, "nothing here"));

					//assert
					changed.ShouldBe(1);
					identical.ShouldBe(1);
					absent.ShouldBe(0);
				}

				using (var context = factory())
					context.Links.Single(x => x.LeftId == 1 && x.RightId == 2).Note.ShouldBe("a new note");
			});
		}

		/// <summary>
		/// <c>UpdateCore</c> writes through a row the shared context has already tracked with stale values: the
		/// write is computed against the <b>stored</b> row, not against the tracked instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A34 — <i>"every tracked fetch pre-detaches"</i> — on the keyless branch, where the
		/// pre-detach has no resolved key and must compile <c>MatchRow</c>. The obligation names <c>Update</c>
		/// and <c>Delete</c> both; this is its <c>UpdateCore</c> leg.
		/// </para>
		/// <para>
		/// <b>The arrangement is chosen so it can fail, and the failure is silent.</b> <c>Attach</c> marks the
		/// entry <c>Unchanged</c> and takes its <i>original</i> values from the attached instance, so Entity
		/// Framework now believes the row holds <c>"ghost"</c>. Call <c>UpdateCore</c> with an <c>item</c> also
		/// carrying <c>"ghost"</c>: <b>with</b> the pre-detach the stale entry is released, the fresh row is
		/// loaded carrying <c>"link 1-2"</c>, <c>SetValues</c> sees a difference and the row is written;
		/// <b>without</b> it the tracking query performs identity resolution rather than re-reading,
		/// <c>SetValues</c> writes a value the entry already holds, the entry stays <c>Unchanged</c> and
		/// <c>SaveChanges</c> emits nothing — <b>and the call still returns <c>1</c></b>. A lost update reported
		/// as a success.
		/// </para>
		/// <para>
		/// <b>The store assertion is therefore the whole test.</b> The returned <c>1</c> is asserted because the
		/// ROW COUNT RULE requires it, but it is identical under both implementations and discriminates nothing.
		/// Reachable in production under <c>ContextOwnership.Borrowed</c>, where the context's owner may have
		/// tracked entities this library never touched.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldWriteThroughAStaleTrackedRowOnUpdateCore()
		{
			WithStore(factory =>
			{
				//setup
				Setup_SixLinks(factory);

				using (var context = factory())
				{
					var dao = new OrderedLinkDao(context);

					// The stale instance: the store holds "link 1-2", and this tells the change tracker otherwise.
					context.Attach(NewLink(1, 2, "ghost"));

					//act
					var written = dao.ReachUpdateCore(NewLink(1, 2, "ghost"));

					//assert
					written.ShouldBe(1);
				}

				//assert — against the store, which is the half the tracked instance can hide
				using (var context = factory())
					context.Links.Single(x => x.LeftId == 1 && x.RightId == 2).Note.ShouldBe(
						"ghost",
						"UpdateCore returned 1 and wrote nothing. The stale tracked entry was not detached, so " +
						"the locating fetch resolved to it by identity rather than re-reading, SetValues wrote " +
						"a value the entry already held, and SaveChanges emitted no UPDATE.");
			});
		}

		/// <summary>
		/// The copy excludes the entity's key properties, so a Data Access Object that locates by a non-key column
		/// can update a row whose caller-supplied key values differ from the stored ones — without throwing.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: A35, refined by Revision 10, and the <c>UpdateCore</c> declaration — <i>"that
		/// exclusion is structural here, not a precaution."</i>
		/// </para>
		/// <para>
		/// <b>The arrangement is chosen so it can fail.</b> A predicate naming both key columns cannot:
		/// <c>SetValues</c> writes a property only where the value differs, and there the values <c>item</c>
		/// carries for the key columns are the values the row was located by. Here <c>TenantId</c> arrives at
		/// <c>0</c> against a stored <c>7</c>, so an implementation with no exclusion writes a key property and
		/// EF Core throws <see cref="InvalidOperationException"/>. The assertion is therefore as much <i>does not
		/// throw</i> as <i>writes the right column</i>.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldExcludeTheKeyPropertiesFromAKeylessUpdate()
		{
			WithStore(factory =>
			{
				//setup
				using (var context = factory())
				{
					context.Registrations.Add(new Registration { TenantId = 7, Code = "alpha", Label = "before" });
					context.SaveChanges();
				}

				int written;

				using (var context = factory())
				{
					var dao = new RegistrationDao(context);

					//act
					written = dao.Update(new Registration { TenantId = 0, Code = "alpha", Label = "after" });
				}

				//assert
				written.ShouldBe(1);

				using (var context = factory())
				{
					var stored = context.Registrations.Single();

					stored.TenantId.ShouldBe(7);
					stored.Code.ShouldBe("alpha");
					stored.Label.ShouldBe("after");
				}
			});
		}

		#endregion

		#region null arguments

		/// <summary>
		/// Every member that reads its argument refuses <c>null</c> with <see cref="ArgumentNullException"/> —
		/// including the two <c>protected</c> cores, reached at their declaration site.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: the <i>Null arguments</i> cross-cutting rule, the <c>Insert</c>/<c>Delete</c>/
		/// <c>GetCore</c>/<c>UpdateCore</c> declarations, and <c>ICompanyResourceDao</c> rule 7.
		/// </remarks>
		[Theory]
		[InlineData("Insert")]
		[InlineData("Delete")]
		[InlineData("GetCore")]
		[InlineData("UpdateCore")]
		[InlineData("Get")]
		[InlineData("Update")]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRefuseANullArgumentOnEveryMemberThatReadsIt(string member)
		{
			WithStore(factory =>
			{
				//setup
				using (var context = factory())
				{
					var root = new OrderedLinkDao(context);
					var published = new PublishedLinkDao(context);

					//act
					Exception thrown;

					switch (member)
					{
						case "Insert":
							thrown = Record.Exception(() => root.Insert(null));
							break;

						case "Delete":
							thrown = Record.Exception(() => root.Delete(null));
							break;

						case "GetCore":
							thrown = Record.Exception(() => root.ReachGetCore(null));
							break;

						case "UpdateCore":
							thrown = Record.Exception(() => root.ReachUpdateCore(null));
							break;

						case "Get":
							thrown = Record.Exception(() => published.Get(null));
							break;

						default:
							thrown = Record.Exception(() => published.Update(null));
							break;
					}

					//assert
					thrown.ShouldBeOfType<ArgumentNullException>();
				}
			});
		}

		/// <summary>
		/// The constructor has exactly one failure mode. There is no identifier to resolve on this family, so it
		/// can never raise <see cref="DataAccessConventionException"/> — A8, A17 step 2 and A33 are properties of
		/// the keyed half and do not run here.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: <see cref="RootNonIdDao{TEntity}"/>'s constructor declaration. The exception type is
		/// the whole assertion: <see cref="DataAccessConventionException"/> does not derive from
		/// <see cref="ArgumentNullException"/>, so requiring the one excludes the other and a second assertion
		/// saying so could not fail.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Keyless")]
		public void ShouldRefuseANullContextAndNeverRaiseAConventionFailure()
		{
			//act
			var thrown = Record.Exception(() => new LinkDao(null));

			//assert
			thrown.ShouldBeOfType<ArgumentNullException>();
		}

		#endregion
	}
}
