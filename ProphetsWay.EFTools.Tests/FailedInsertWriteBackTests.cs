using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using ProphetsWay.BaseDataAccess;

using Shouldly;

using Xunit;

namespace ProphetsWay.EFTools.Tests
{
	/// <summary>
	/// Lap finding <b>F10</b> — <i>after an <c>Insert</c> that did not store a row, the caller's instance carries
	/// exactly the values it carried when the call was made.</i> No timestamp stamped, no timestamp nulled, no
	/// identifier assigned.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>The obligation binds every <c>Insert</c> on every family — hard and soft, keyed and keyless</b> — and
	/// this file runs it on the three arrangements <c>docs/api-contract.md</c> names in the <i>Soft delete</i>
	/// obligation group: a <see cref="BaseSoftDao{TEntity, TKey}"/> descendant, a
	/// <see cref="RootSoftNonIdDao{TEntity}"/> descendant (the R4-S2 both-branches reason), and a keyed hard
	/// <see cref="BaseDao{TEntity, TKey}"/> descendant where the assertion is that no identifier was assigned.
	/// </para>
	/// <para>
	/// <b>Only one of the three is a live defect.</b> The finding's own member-by-member table records
	/// <c>BaseSoftDao.Insert</c> as the single site — it assigns the three stamps onto <c>item</c> <i>before</i>
	/// calling <c>base.Insert(item)</c>, so a throwing <c>SaveChanges</c> hands control back with the caller's
	/// instance carrying a full set of stamps for a row that was never written. <c>BaseDao.Insert</c> is already
	/// clean, because its identifier write-back runs <i>after</i> <c>SaveChanges</c>. The clean sites are covered
	/// anyway: the obligation is family-wide, and a guard that only exists where the bug currently is stops being
	/// a guard the moment the bug is fixed.
	/// </para>
	/// <para>
	/// <b>How the write is made to fail.</b> A referential-integrity violation — the removable failure the
	/// obligation names. Every subject entity carries a required foreign key to <see cref="Holder"/>, and each
	/// arrangement inserts one naming a holder that was never stored, so the provider rejects the row and
	/// <c>SaveChanges</c> throws. The throw therefore lands <b>strictly after</b> the member has read
	/// <paramref name="item"/> and after any write-back an implementation may have hoisted, which is the whole
	/// point: an arrangement that failed earlier — a <c>null</c> argument, a convention breach — would pass
	/// against the defect and assert nothing. Foreign keys are switched on explicitly in the connection string
	/// rather than left to the provider's default, so the arrangement cannot silently stop failing.
	/// </para>
	/// <para>
	/// <b>The <c>DeletedDate</c>/<c>UpdatedDate</c> half is the half that discriminates.</b> A
	/// <c>CreatedDate</c> gets re-stamped on a retry and can look correct by accident; a nulled
	/// <c>DeletedDate</c> is destroyed caller data. Each soft arrangement therefore arrives carrying
	/// distinguishable non-default values in all three.
	/// </para>
	/// <para>
	/// <b>SQLite in-memory, never the InMemory provider.</b> A non-relational provider enforces no foreign key,
	/// so nothing would throw and every test here would pass while asserting nothing. <c>Constants.cs</c> and its
	/// SQL Server connection string are deliberately untouched — nothing here needs a local server.
	/// </para>
	/// <para>
	/// <b>The last two facts run F10's <i>other</i> failure shape</b> — the obligation binds on <i>"an exception
	/// out of <c>SaveChanges</c>, <b>and</b> an exception out of anything the member does after reading
	/// <c>item</c>"</i>, and the three above only ever exercise the first. A write whose <c>SaveChanges</c>
	/// <b>succeeded</b> and whose later steps then threw is the window neither the member nor the obligation's
	/// wording had been tested against, and the two shapes are not interchangeable: "the call threw" is a sound
	/// test for <i>an exception</i> and an unsound proxy for <i>nothing was stored</i>. The property asserted is
	/// therefore stated as agreement rather than as restoration — <b>the caller's instance and the stored row must
	/// say the same thing about whether the write happened</b> — because a row that is already committed cannot be
	/// un-stored by a <c>finally</c>, and an instance rolled back over a committed row carries a real identifier
	/// beside timestamps that identifier's row does not have.
	/// </para>
	/// <para>
	/// <b>How that window is opened, since <c>SaveChanges</c> must succeed.</b> Both post-save steps reach a
	/// <i>caller-owned collection instance</i> — <c>EntityGraph.RemoveFromInverseNavigations</c> invokes
	/// <c>Remove</c> on the principal's inverse collection, and <c>EntityGraph.ReachableFrom</c>, which every
	/// <c>DetachAfterWrite</c> runs, walks a collection navigation with a <c>foreach</c>. A collection type is
	/// therefore the one seam a fixture owns on either path, and <see cref="TrapCollection{T}"/> is that seam. It
	/// refuses exactly one operation and behaves normally otherwise, so nothing before the save is disturbed.
	/// </para>
	/// </remarks>
	public class FailedInsertWriteBackTests
	{
		#region fixture entities

		/// <summary>The principal every subject entity points at, and which the failing arrangements never store.</summary>
		public class Holder : IBaseIdEntity<int>
		{
			public int Id { get; set; }

			public string Name { get; set; }
		}

		/// <summary>Keyed, hard. The subject of the identifier half of the obligation.</summary>
		public class Ticket : IBaseIdEntity<int>
		{
			public int Id { get; set; }

			public string Subject { get; set; }

			public int HolderId { get; set; }
		}

		/// <summary>Keyed, soft. The subject of the one live defect site.</summary>
		public class Memo : IBaseSoftIdEntity<int>
		{
			public int Id { get; set; }

			public string Body { get; set; }

			public int HolderId { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		/// <summary>
		/// Keyless, soft. No identifier at all — the natural key is <see cref="HolderId"/> and <see cref="Code"/>
		/// together, which is what <c>MatchRow</c> is overridden to.
		/// </summary>
		public class Pairing : IBaseSoftEntity
		{
			public int HolderId { get; set; }

			public string Code { get; set; }

			public string Note { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		/// <summary>
		/// Keyless, soft, and the <b>principal</b> of the one relationship the post-save arrangements need. Its
		/// natural key is <see cref="Code"/> alone, and <see cref="Slips"/> is the inverse collection navigation
		/// both post-save steps reach through.
		/// </summary>
		public class Binder : IBaseSoftEntity
		{
			public string Code { get; set; }

			public string Label { get; set; }

			public ICollection<Slip> Slips { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		/// <summary>
		/// Keyed, soft, and the <b>dependent</b>. Its only navigation is the reference back to
		/// <see cref="Binder"/>, which is what makes <c>RemoveFromInverseNavigations</c> reach the principal's
		/// collection rather than skip it.
		/// </summary>
		public class Slip : IBaseSoftIdEntity<int>
		{
			public int Id { get; set; }

			public string Body { get; set; }

			public string BinderCode { get; set; }

			public Binder Binder { get; set; }

			public DateTime CreatedDate { get; set; }

			public DateTime? UpdatedDate { get; set; }

			public DateTime? DeletedDate { get; set; }
		}

		#endregion

		#region the post-save seam

		/// <summary>Marks a throw as this fixture's own, so a broken arrangement cannot be read as the defect.</summary>
		public class TrapException : Exception
		{
			public TrapException(string message) : base(message)
			{
			}
		}

		/// <summary>
		/// A collection navigation that refuses exactly one operation, so a step the library runs <b>after</b> a
		/// successful <c>SaveChanges</c> can be made to throw while the write itself stands.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="RejectRemove"/> is the seam on <c>Insert</c>: <c>EntityGraph.RemoveFromInverseNavigations</c>
		/// reflects out <c>ICollection&lt;T&gt;.Remove</c> and invokes it on the principal's collection, one
		/// statement after <c>BaseDao.Insert</c> has assigned the identifier onto the caller's instance. The
		/// exception arrives wrapped in <see cref="System.Reflection.TargetInvocationException"/>, which is why
		/// the assertions walk the chain rather than test the outermost type.
		/// </para>
		/// <para>
		/// <see cref="RejectEnumeration"/> is the seam on the write cores: <c>EntityGraph.ReachableFrom</c> walks
		/// a collection navigation with a plain <c>foreach</c>, and every <c>DetachAfterWrite</c> — which runs in
		/// the <c>finally</c> of <c>Update</c>, <c>Delete</c> and <c>UpdateCore</c> alike — walks the caller's own
		/// instance last. That call is unreflected, so the exception arrives unwrapped.
		/// </para>
		/// <para>
		/// Everything else behaves as a plain list. Refusing more than one operation would move the failure
		/// earlier than the window under test and assert nothing about it.
		/// </para>
		/// </remarks>
		public class TrapCollection<T> : ICollection<T>
		{
			private readonly List<T> _members = new List<T>();

			public bool RejectRemove { get; set; }

			public bool RejectEnumeration { get; set; }

			public int Count
			{
				get { return _members.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public void Add(T item)
			{
				_members.Add(item);
			}

			public void Clear()
			{
				_members.Clear();
			}

			public bool Contains(T item)
			{
				return _members.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_members.CopyTo(array, arrayIndex);
			}

			public bool Remove(T item)
			{
				if (RejectRemove)
					throw new TrapException("The inverse collection navigation refused Remove.");

				return _members.Remove(item);
			}

			public IEnumerator<T> GetEnumerator()
			{
				if (RejectEnumeration)
					throw new TrapException("The collection navigation refused enumeration.");

				return _members.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion

		#region fixture context and data access objects

		public class FailedInsertContext : DbContext
		{
			public FailedInsertContext(DbContextOptions<FailedInsertContext> options) : base(options)
			{
			}

			public DbSet<Holder> Holders { get; set; }

			public DbSet<Ticket> Tickets { get; set; }

			public DbSet<Memo> Memos { get; set; }

			public DbSet<Pairing> Pairings { get; set; }

			public DbSet<Binder> Binders { get; set; }

			public DbSet<Slip> Slips { get; set; }

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.Entity<Holder>().HasKey(x => x.Id);

				var ticket = modelBuilder.Entity<Ticket>();
				ticket.HasKey(x => x.Id);
				ticket.HasOne<Holder>().WithMany().HasForeignKey(x => x.HolderId);

				var memo = modelBuilder.Entity<Memo>();
				memo.HasKey(x => x.Id);
				memo.HasOne<Holder>().WithMany().HasForeignKey(x => x.HolderId);

				var pairing = modelBuilder.Entity<Pairing>();
				pairing.HasKey(x => new { x.HolderId, x.Code });
				pairing.HasOne<Holder>().WithMany().HasForeignKey(x => x.HolderId);

				modelBuilder.Entity<Binder>().HasKey(x => x.Code);

				// Both ends named, unlike the four above: the post-save steps under test only run where the model
				// knows the inverse navigation, so a one-sided relationship would skip them silently.
				var slip = modelBuilder.Entity<Slip>();
				slip.HasKey(x => x.Id);
				slip.HasOne(x => x.Binder).WithMany(x => x.Slips).HasForeignKey(x => x.BinderCode);
			}
		}

		public class TicketDao : BaseDao<Ticket, int>
		{
			public TicketDao(DbContext context) : base(context)
			{
			}
		}

		public class MemoDao : BaseSoftDao<Memo, int>
		{
			public MemoDao(DbContext context) : base(context)
			{
			}
		}

		public class PairingDao : RootSoftNonIdDao<Pairing>
		{
			public PairingDao(DbContext context) : base(context)
			{
			}

			protected override Expression<Func<Pairing, bool>> MatchRow(Pairing item)
			{
				return x => x.HolderId == item.HolderId && x.Code == item.Code;
			}
		}

		/// <summary>Keyed, soft — the subject of the post-save <c>Insert</c> window.</summary>
		public class SlipDao : BaseSoftDao<Slip, int>
		{
			public SlipDao(DbContext context) : base(context)
			{
			}
		}

		/// <summary>Keyless, soft — the subject of the post-save <c>UpdateCore</c> window.</summary>
		/// <remarks>
		/// <c>ReachUpdateCore</c> is a test instrument and adds no behavior, matching
		/// <see cref="KeylessSoftDaoTests"/>: a <c>Root</c> type publishes nothing, so there is no other way to
		/// reach the core at the declaration site where it is specified.
		/// </remarks>
		public class BinderDao : RootSoftNonIdDao<Binder>
		{
			public BinderDao(DbContext context) : base(context)
			{
			}

			public int ReachUpdateCore(Binder item)
			{
				return UpdateCore(item);
			}

			protected override Expression<Func<Binder, bool>> MatchRow(Binder item)
			{
				return x => x.Code == item.Code;
			}
		}

		#endregion

		#region apparatus

		/// <summary>An identifier no arrangement stores, so every foreign key naming it is dangling.</summary>
		private const int AbsentHolderId = 987654;

		private static readonly DateTime SentinelCreated = new DateTime(2001, 2, 3, 4, 5, 6, DateTimeKind.Utc);

		private static readonly DateTime SentinelUpdated = new DateTime(2002, 3, 4, 5, 6, 7, DateTimeKind.Utc);

		private static readonly DateTime SentinelDeleted = new DateTime(2003, 4, 5, 6, 7, 8, DateTimeKind.Utc);

		/// <summary>The one principal the post-save arrangements store, and whose collection carries the trap.</summary>
		private const string TrapBinderCode = "trap";

		private static void WithStore(Action<Func<FailedInsertContext>> body)
		{
			// Foreign Keys=True is stated rather than assumed: SQLite enforces no constraint without the pragma,
			// and an arrangement whose write silently stopped failing would leave every test here green and empty.
			using (var connection = new SqliteConnection("Filename=:memory:;Foreign Keys=True"))
			{
				connection.Open();

				var options = new DbContextOptionsBuilder<FailedInsertContext>()
					.UseSqlite(connection)
					.Options;

				Func<FailedInsertContext> factory = () => new FailedInsertContext(options);

				using (var schema = factory())
					schema.Database.EnsureCreated();

				body(factory);
			}
		}

		/// <summary>
		/// Whether the fixture's own refusal is anywhere in the chain, since one seam is reached by reflection and
		/// arrives wrapped while the other does not.
		/// </summary>
		private static bool CarriesTheTrap(Exception thrown)
		{
			for (var current = thrown; current != null; current = current.InnerException)
			{
				if (current is TrapException)
					return true;
			}

			return false;
		}

		/// <summary>Stores the principal through its own Data Access Object, so its stamps are the library's.</summary>
		private static void Setup_StoredBinder(Func<FailedInsertContext> factory, string label)
		{
			using (var context = factory())
				new BinderDao(context).Insert(new Binder { Code = TrapBinderCode, Label = label });
		}

		#endregion

		#region the keyed hard branch — no identifier is assigned

		/// <summary>
		/// A failed <c>Insert</c> on <see cref="BaseDao{TEntity, TKey}"/> leaves the caller's instance carrying no
		/// identifier and none of its other values disturbed.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: F10's obligation, plus A32's identifier write-back, whose ordering clause says
		/// <i>after <c>SaveChanges</c></i> and which F10 records as under-stated until the obligation was written.
		/// This site is already conforming, so this is a regression guard rather than a red test — an
		/// implementation that hoisted the assignment satisfies every sentence A32 contains and fails here.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Insert")]
		public void ShouldAssignNoIdentifierWhenAKeyedHardInsertStoresNoRow()
		{
			WithStore(factory =>
			{
				//setup
				var ticket = new Ticket { Subject = "names a holder that is not there", HolderId = AbsentHolderId };

				using (var context = factory())
				{
					var dao = new TicketDao(context);

					//act
					var thrown = Record.Exception(() => dao.Insert(ticket));

					//assert
					thrown.ShouldNotBeNull();

					ticket.Id.ShouldBe(0);
					ticket.Subject.ShouldBe("names a holder that is not there");
					ticket.HolderId.ShouldBe(AbsentHolderId);
				}
			});
		}

		#endregion

		#region the keyed soft branch — the one live defect site

		/// <summary>
		/// A failed <c>Insert</c> on <see cref="BaseSoftDao{TEntity, TKey}"/> leaves all three timestamps exactly
		/// as the caller supplied them, and assigns no identifier.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: F10's obligation, and the <c>Insert</c> member contract on
		/// <see cref="BaseSoftDao{TEntity, TKey}"/>, whose <c>&lt;remarks&gt;</c> now carry it in terms — <i>"an
		/// <c>Insert</c> that throws leaves the caller's instance carrying exactly the values it arrived with."</i>
		/// </para>
		/// <para>
		/// <b>This test fails against the lap 2 shipped implementation</b>, which stamps <c>item</c> before
		/// calling <c>base.Insert</c>. The <c>UpdatedDate</c> and <c>DeletedDate</c> assertions are the ones that
		/// cannot pass by accident: a re-stamped <c>CreatedDate</c> could coincide with a sentinel under some
		/// clock, a nulled <c>DeletedDate</c> cannot.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Insert")]
		public void ShouldLeaveEveryTimestampUntouchedWhenAKeyedSoftInsertStoresNoRow()
		{
			WithStore(factory =>
			{
				//setup
				var memo = new Memo
				{
					Body = "names a holder that is not there",
					HolderId = AbsentHolderId,
					CreatedDate = SentinelCreated,
					UpdatedDate = SentinelUpdated,
					DeletedDate = SentinelDeleted
				};

				using (var context = factory())
				{
					var dao = new MemoDao(context);

					//act
					var thrown = Record.Exception(() => dao.Insert(memo));

					//assert
					thrown.ShouldNotBeNull();

					memo.CreatedDate.ShouldBe(SentinelCreated);
					memo.UpdatedDate.ShouldBe(SentinelUpdated);
					memo.DeletedDate.ShouldBe(SentinelDeleted);
					memo.Id.ShouldBe(0);
				}
			});
		}

		#endregion

		#region the keyless soft branch — R4-S2's other half

		/// <summary>
		/// The same obligation on <see cref="RootSoftNonIdDao{TEntity}"/>, whose <c>Insert</c> is written in this
		/// lap from the same A24 steps by the same route and would otherwise reproduce the keyed defect.
		/// </summary>
		/// <remarks>
		/// <c>Contract</c>: F10's obligation and the <c>&lt;remarks&gt;</c> on
		/// <see cref="RootSoftNonIdDao{TEntity}.Insert"/>, which carry it at the declaration site — <i>"the
		/// write-back onto <c>item</c> happens only where a row was written."</i> No identifier clause here:
		/// there is no identifier to assign, on this class as on its base.
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Insert")]
		public void ShouldLeaveEveryTimestampUntouchedWhenAKeylessSoftInsertStoresNoRow()
		{
			WithStore(factory =>
			{
				//setup
				var pairing = new Pairing
				{
					HolderId = AbsentHolderId,
					Code = "dangling",
					Note = "names a holder that is not there",
					CreatedDate = SentinelCreated,
					UpdatedDate = SentinelUpdated,
					DeletedDate = SentinelDeleted
				};

				using (var context = factory())
				{
					var dao = new PairingDao(context);

					//act
					var thrown = Record.Exception(() => dao.Insert(pairing));

					//assert
					thrown.ShouldNotBeNull();

					pairing.CreatedDate.ShouldBe(SentinelCreated);
					pairing.UpdatedDate.ShouldBe(SentinelUpdated);
					pairing.DeletedDate.ShouldBe(SentinelDeleted);
					pairing.Note.ShouldBe("names a holder that is not there");
				}
			});
		}

		#endregion

		#region the post-save window — the write succeeded and a later step threw

		/// <summary>
		/// An <c>Insert</c> whose <c>SaveChanges</c> <b>succeeded</b> and which then threw from a later step
		/// leaves the caller's instance agreeing with the row it stored — the identifier and all three timestamps
		/// alike.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <c>Contract</c>: F10's obligation, which binds on <b>both</b> failure shapes in terms — <i>"an
		/// exception out of <c>SaveChanges</c>, and an exception out of anything the member does after reading
		/// <c>item</c>"</i> — and whose subject is the caller's instance rather than the exception. The three facts
		/// above run the first shape only; this is the second, and the two are not the same test. <b>"Did not
		/// store a row" is the obligation's antecedent, and an exception is only a proxy for it.</b> Here the
		/// proxy is false: the row is committed, so the reading that satisfies the obligation is the one the
		/// caller can act on — the instance must say what the row says.
		/// </para>
		/// <para>
		/// <b>Where it throws, and why that placement is the whole test.</b> <c>BaseDao.Insert</c> assigns the
		/// identifier onto <c>item</c> and <i>then</i> calls <c>RemoveFromInverseNavigations</c>, which invokes
		/// <c>Remove</c> on the principal's inverse collection. <see cref="TrapCollection{T}"/> refuses that one
		/// call, so control leaves <c>BaseDao.Insert</c> with the row stored and the real identifier already on
		/// <c>item</c>. <c>BaseSoftDao.Insert</c> then reads its own <c>stored</c> flag, which is set only after
		/// <c>base.Insert</c> returns normally, concludes nothing was written, and restores the three timestamps.
		/// </para>
		/// <para>
		/// <b>The identifier assertion is the one that passes</b>, and it is what makes the timestamp assertions
		/// mean something: an instance carrying a real database identifier beside a <c>CreatedDate</c> naming no
		/// row, and a <c>DeletedDate</c> the store does not have, is a state that says neither stored nor not
		/// stored. Either remedy F10 permits closes it — restoring nothing once the save has returned, or
		/// stamping only the copy — and this fact does not choose between them.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Contract")]
		[Trait("Area", "Insert")]
		public void ShouldLeaveTheCallersInstanceAgreeingWithTheStoredRowWhenAKeyedSoftInsertThrowsAfterSaving()
		{
			WithStore(factory =>
			{
				//setup
				Setup_StoredBinder(factory, "the principal");

				using (var context = factory())
				{
					var principal = context.Set<Binder>().AsNoTracking().Single();
					principal.Slips = new TrapCollection<Slip> { RejectRemove = true };

					var slip = new Slip
					{
						Body = "stored, and then the step after the save throws",
						BinderCode = TrapBinderCode,
						Binder = principal,
						CreatedDate = SentinelCreated,
						UpdatedDate = SentinelUpdated,
						DeletedDate = SentinelDeleted
					};

					var dao = new SlipDao(context);

					//act
					var thrown = Record.Exception(() => dao.Insert(slip));

					//assert
					thrown.ShouldNotBeNull();
					CarriesTheTrap(thrown).ShouldBeTrue("the arrangement must fail at the seam after SaveChanges, not before it");

					using (var verify = factory())
					{
						var row = verify.Set<Slip>().AsNoTracking().SingleOrDefault();

						// The save returned before the throw, so the write stands and the caller holds its key.
						row.ShouldNotBeNull();
						slip.Id.ShouldBe(row.Id);

						slip.CreatedDate.ShouldBe(row.CreatedDate);
						slip.UpdatedDate.ShouldBe(row.UpdatedDate);
						slip.DeletedDate.ShouldBe(row.DeletedDate);
					}
				}
			});
		}

		/// <summary>
		/// The same window on <see cref="RootSoftNonIdDao{TEntity}.UpdateCore"/>: a <c>SaveChanges</c> that
		/// succeeded and a <c>finally</c> that then threw must not leave the caller's <c>UpdatedDate</c>
		/// disagreeing with the stamp the row now carries.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <b><c>Characterization</c>, deliberately, and this is a gap in the specification rather than a choice
		/// of tag.</b> The member's own <c>&lt;remarks&gt;</c> state the restore for <i>"a call returning
		/// <c>0</c>, or one whose <c>SaveChanges</c> throws"</i> — and here <c>SaveChanges</c> did neither. F10's
		/// obligation names <c>Insert</c> and only <c>Insert</c>: <i>"this binds every <c>Insert</c> on every
		/// family."</i> No upstream rule reaches it either — <c>IExampleDataAccess</c>'s ROW COUNT RULE speaks to
		/// what a call <i>returns</i>, and this one returns nothing. So <b>no stated term covers a write whose
		/// save succeeded and whose later step threw</b>, and tagging this <c>Contract</c> would place an
		/// obligation on every future implementer in the name of a specification that does not make it.
		/// <b>The specification needs a term</b>, stated once for every write member rather than per member, and
		/// until it has one this fact records a defect it cannot yet cite.
		/// </para>
		/// <para>
		/// <b>Area is <c>Keyless</c> rather than a new <c>Update</c> value.</b> The subject is
		/// <see cref="RootSoftNonIdDao{TEntity}"/>, which is what <see cref="KeylessSoftDaoTests"/> is traited
		/// under, and <c>Area=Keyless|Area=Insert</c> is the gate this lap already runs — a new value would put
		/// the fact outside every existing filter, xUnit trait filters being allowlists.
		/// </para>
		/// <para>
		/// <b>The seam.</b> <c>base.UpdateCore</c> saves, computes <c>1</c>, and then runs
		/// <c>DetachAfterWrite(stored, item)</c> in its <c>finally</c>; that walks the caller's own instance with
		/// <c>ReachableFrom</c>, which <c>foreach</c>es a collection navigation.
		/// <see cref="TrapCollection{T}"/> refuses that enumeration, so the return value is abandoned,
		/// <c>written</c> is never assigned, and <c>RootSoftNonIdDao.UpdateCore</c>'s own <c>finally</c> restores
		/// an <c>UpdatedDate</c> the row no longer has. The refusal is on <c>item</c>'s collection and not on the
		/// located row's, so the walk that fails is strictly the last thing the member does.
		/// </para>
		/// </remarks>
		[Fact]
		[Trait("Scope", "Characterization")]
		[Trait("Area", "Keyless")]
		public void ShouldLeaveTheCallersInstanceAgreeingWithTheStoredRowWhenAKeylessSoftUpdateThrowsAfterSaving()
		{
			WithStore(factory =>
			{
				//setup
				Setup_StoredBinder(factory, "as stored");

				using (var context = factory())
				{
					var probe = new Binder
					{
						Code = TrapBinderCode,
						Label = "as edited",
						UpdatedDate = SentinelUpdated,
						Slips = new TrapCollection<Slip> { RejectEnumeration = true }
					};

					var dao = new BinderDao(context);

					//act
					var thrown = Record.Exception(() =>
					{
						dao.ReachUpdateCore(probe);
					});

					//assert
					thrown.ShouldNotBeNull();
					CarriesTheTrap(thrown).ShouldBeTrue("the arrangement must fail at the seam after SaveChanges, not before it");

					using (var verify = factory())
					{
						var row = verify.Set<Binder>().AsNoTracking().Single(x => x.Code == TrapBinderCode);

						// The save returned before the throw, so the edit stands.
						row.Label.ShouldBe("as edited");

						probe.UpdatedDate.ShouldBe(row.UpdatedDate);
					}
				}
			});
		}

		#endregion
	}
}
