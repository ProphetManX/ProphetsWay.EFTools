using System;
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

		#endregion

		#region apparatus

		/// <summary>An identifier no arrangement stores, so every foreign key naming it is dangling.</summary>
		private const int AbsentHolderId = 987654;

		private static readonly DateTime SentinelCreated = new DateTime(2001, 2, 3, 4, 5, 6, DateTimeKind.Utc);

		private static readonly DateTime SentinelUpdated = new DateTime(2002, 3, 4, 5, 6, 7, DateTimeKind.Utc);

		private static readonly DateTime SentinelDeleted = new DateTime(2003, 4, 5, 6, 7, 8, DateTimeKind.Utc);

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
	}
}
