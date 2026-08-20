# Feature Requests & Deferred Decisions — ProphetsWay.EFTools

This is the record of things that were **considered**, together with the reasoning behind each decision.
Nothing here is a limitation, an apology, or a TODO list. Each entry exists so a future developer — or a
future AI agent — can find the decision, judge whether the tradeoff that produced it still holds, and
reopen it as a real feature request when it does not.

**If you are about to propose one of these, read its entry first.** The entry tells you what was already
weighed, so your proposal can start from the open questions rather than from the beginning.

**Numbering is per-repository and starts at 1.** It does not continue, mirror, or correspond to the indexes
in [ProphetsWay.BaseDataAccess/docs/feature-requests.md](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md)
or [ProphetsWay.Example/docs/feature-requests.md](../../ProphetsWay.Example/docs/feature-requests.md)
(**1–14** as of 2026-08-16; this preamble previously said 1–9, then 1–13, and both are stale). Those are
separate indexes.
This file follows their *format*; where an entry genuinely depends on one of theirs, it is cited by
repository and number.

The contracts themselves are **not** restated here. The binding rules live in the XML `<remarks>` on
`IBaseDataAccess` and `DataAccessConventionException` in `ProphetsWay.BaseDataAccess`, and on
`IExampleDataAccess` in `ProphetsWay.Example`. Those are the source of truth. This file links to them and
does not duplicate them, because duplicated rules drift.

The scope bar every entry below is judged against is in
[purpose-and-scope.md](purpose-and-scope.md#settled-one-sentence-purpose), and the owner decisions that set
the statuses below are recorded as **D1–D19** in
[purpose-and-scope.md § Owner Decisions](purpose-and-scope.md#owner-decisions--2026-08-15). **D1–D9 were
taken 2026-08-15, D10–D13 on 2026-08-16, D14–D18 on 2026-08-18 and D19 on 2026-08-19**; the section heading
carries the earliest date only. **This preamble said D1–D10 until 2026-08-19, and that was stale by nine
decisions.**

**A second decision index binds the entries below, and it is not in that file.** `docs/api-contract.md`
carries owner decisions **S1–S13** (2026-08-15) and **OD-1–OD-11**, and several of them close questions this
file and `purpose-and-scope.md` still described as open — **S4** closes **Q3**, **S7** closes **Q2**. Read
all three documents before concluding that anything here awaits a decision.

**Stage 1 is closed as of 2026-08-15.** The two questions that needed the owner — **Q1** (the TFM
exception) and **Q4** (whether the certification scope is stated publicly) — were answered as **D7** and
**D8**. **Q2** and **Q3** remain open and are answerable by whoever implements v3.0.0.

**One owner question has since reopened, and has now been answered — 2026-08-16.** The sentence "no status in
this file is waiting on an owner decision" stopped being true when the `ProphetsWay.Example` submodule
pointer advanced:
[entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) was rescoped on that date
and carried a genuine fork — a local suite here, or a seam upstream — that this agent could not choose.
**The owner chose the seam (shape B) later the same day**, and the question is closed; see
[the resolution](#the-resolution--2026-08-16-shape-b-the-direction-only). **It is again true that no status
here waits on the owner.** What entry 6 now waits on is *another repository* —
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)
— which is a dependency, not an open question.

**The repository builds green — 2026-08-16, and this supersedes every "does not compile" claim below.**
`dotnet build ProphetsWay.EFTools.sln -c Debug` succeeded on SDK 10.0.400 with 7 warnings and every project
compiling. This is the **first verified-green state since the submodule pointer advanced**, and it is what made
this triage pass possible: entries 2, 3, 5 and 8 were previously asserted-but-unverified, and a compiling tree
is the evidence they were waiting on. Sentences elsewhere in this file describing the repository as
non-compiling were true when written and are **history**; they are marked where they appear rather than
deleted. **A green build is not a passing suite** — this repository still contains no tests, by design, under
**D10**.

## Index

| # | Item | Status |
| --- | --- | --- |
| 1 | [Advance the `ProphetsWay.Example` submodule onto the 3.x contracts](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) | **Scheduled** — v3.0.0; **steps 1 and 3 landed 2026-08-16 and the tree now compiles**; steps 2, 4, 5 and 6 outstanding — step 2 is satisfied by throwing stubs, not by an implementation |
| 2 | [Move the `ProphetsWay.BaseDataAccess` reference from 2.5.0 to 3.1.0](#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310) | **Done** — 2026-08-16; both projects reference 3.1.0 and the solution compiles against it |
| 3 | [Implement the 3.x disposal contract in `BaseEFDataAccess`](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | **Scheduled** — v3.0.0; **rescoped 2026-08-16**: `Dispose` has landed, and the entry now carries only the `ObjectDisposedException` guarding. **Q2 is superseded**, not open. The **2.2.x patch for the leaked context is `Rejected`** — [D12](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16 — and the defect carries a [release-note obligation](#the-changelog-author-obligation--d12) instead |
| 4 | [Make 3.x Entity Framework Core-only — retire EF6 and .NET Framework](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) | **Scheduled** — v3.0.0; **approved by D1** |
| 5 | [Retarget to the house TFM standard](#5--retarget-to-the-house-tfm-standard) | **Scheduled** — v3.0.0; unblocked by 4; destination settled by **D7** as **`net10.0` only** |
| 6 | [Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory and `Scope` traits](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) | **Scheduled** — v3.0.0; forced by 1; rescoped 2026-08-16 and **resolved to shape B the same day**. The six adapters are **deleted** — verified 2026-08-16 — so this repository's half is done; the entry stays open on **ProphetsWay.Example FR 13**, whose seam **landed 2026-08-16 and is unverified** — nothing has been run through it |
| 7 | [Stop forcing a database provider on every consumer](#7--stop-forcing-a-database-provider-on-every-consumer) | **Scheduled** — v3.0.0; **approved by D2** |
| 8 | [Remove `FluentAssertions` from `ProphetsWay.Example.DataAccess.EF`](#8--remove-fluentassertions-from-prophetswayexampledataaccessef) | **Done** — 2026-08-16; the reference is gone and the licence exposure is closed |
| 9 | [Delete the stray `[submodule "Submod"]` block from `.gitmodules`](#9--delete-the-stray-submodule-submod-block-from-gitmodules) | **Scheduled** — v3.0.0; trivial |
| 10 | [Collapse the `Guid`/`Int`/`Long` DAO triplication](#10--collapse-the-guidintlong-dao-triplication) | **Scheduled** — v3.0.0; **approved by D3, reversing this file's recommendation**. **Constrained by [D13](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16:** the generic families are **derived from** a hand-written concrete `DepartmentDao`, not designed ahead of one |
| 11 | [Certify the contract suite on SQLite in-memory and a SQL Server container](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | **Scheduled** — v3.0.0 for the test work; **pipeline half Deferred** to its owner; **D8** makes the certification a public claim |
| 12 | [`RootNonIdDao.EnsureBeginTransaction` silently no-ops against a pre-existing transaction](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | **Scheduled** — v3.0.0, as a **release-note obligation only**; the 2.2.x patch is **Rejected** — triaged 2026-08-16, and its retained open question **closed by [D12](purpose-and-scope.md#owner-decisions--2026-08-15)** the same day |
| 13 | [The soft-delete and keyless DAO bases cannot serve the 3.x contracts by inheritance](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | **Scheduled** — v3.0.0; filed 2026-08-16. Four contract-rule violations in `RootBaseSoftDao` and a structural mismatch in `BaseNonIdDao<T>`; constrains [entry 10](#10--collapse-the-guidintlong-dao-triplication). **2.2.x patch `Rejected` ([D12](purpose-and-scope.md#owner-decisions--2026-08-15))**; the route to the fix is settled by **[D13](purpose-and-scope.md#owner-decisions--2026-08-15)**. **Strengthened 2026-08-18** — the members are `new`, not `virtual`, so the title's claim is structural rather than a judgement; and the entry is no longer a reading finding |
| 14 | [The Entity Framework DAO bases adopt the caller's instance, violating the SNAPSHOT rule](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) | **Scheduled** — v3.0.0, **triaged 2026-08-19**. A **fourth shipped 2.2.0 defect**, and the fix is **already specified** in `docs/api-contract.md` rev 8 — it needs no new design. It carries **inside** the [entry 10](#10--collapse-the-guidintlong-dao-triplication) collapse under [D14](purpose-and-scope.md#owner-decisions--2026-08-15), is **breaking** against 2.2.0, and the 2.2.x patch is **Rejected** on [D12](purpose-and-scope.md#owner-decisions--2026-08-15). See [the triage](#triaged-2026-08-19--scheduled-for-v300-and-it-is-a-fourth-shipped-defect) |

Numbers are permanent. Entries are never renumbered and never removed —
[purpose-and-scope.md](purpose-and-scope.md) cites entries by number, and a rejected entry is decision
history rather than dead weight.

## Release Ordering — D11

**Owner decision, 2026-08-16, recorded in full as
[D11](purpose-and-scope.md#owner-decisions--2026-08-15) and reasoned through in
[purpose-and-scope.md § Release Ordering](purpose-and-scope.md#release-ordering--settled-d11).** It is
summarized here because it governs *when* the entries below ship, and a reader planning the release from this
file would otherwise not meet it.

1. **All remaining 3.x work here is done against the live `ProphetsWay.Example` submodule working tree**, not
   against a tagged release of it.
2. **`ProphetsWay.Example` is tagged and released** once this repository's Entity Framework implementation is
   **green against it**.
3. **This repository's submodule pointer is advanced to that tag.**
4. **`ProphetsWay.EFTools` 3.0.0 is pull-requested, merged and published.**

`ProphetsWay.BaseDataAccess` **3.1.0 is already published and current** and needs no release in this sequence.

**The reason the order is this way round:** implementing against a contract is what exposes gaps *in* it.
`IDepartmentDao` rule 18 was **narrowed on 2026-08-16 as a direct result of Entity Framework design work
here**, changing `ProphetsWay.Example`'s interface. Tagging that repository first risks discovering a second
rule 18 and needing a second release of it — the double build / pull request / merge D11 exists to prevent.

**No entry below changed status because of D11.** It constrains sequence, not scope. In particular it does not
reopen [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 1, which has landed;
D11 governs the **next** pointer move, which is step 3 above.

### Clarification recorded 2026-08-16 — an interim pointer advance is **not** a D11 violation

D11 step 1 says the remaining 3.x work is done *against the live `ProphetsWay.Example` submodule working
tree*. **That premise silently assumed the submodule could see the live work. It cannot** — the submodule
tracks `main`, and the work is on `ProphetsWay.Example`'s `3.1.1-eftool-findings` branch. Concretely,
`TestDataAccessFactory.Use` — the seam that lets this repository run the upstream suite against Entity
Framework without editing a file it is forbidden to edit — is not in the code this repository compiles
against, so nothing downstream of it can move.

**Advancing the pointer onto in-progress upstream work is a different act from step 3.** Step 3 advances the
pointer onto a **tag**, after a green run. An interim advance onto a merged-but-untagged `main` is what makes
that green run *possible*. D11 defers **tagging and releasing** `ProphetsWay.Example`; it does not require
this repository to compile against a stale contract while doing the very work that proves the contract.

**No status changed. This is a note about the scope of an owner decision, not a change to it** — if the owner
reads it otherwise, D11 as written wins and this paragraph is what should be corrected.

## The `Changelog Author` Obligation — D12

**Not this agent's to execute, and it must not be lost.** It is stated once, here, and cited from
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess),
[entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) and
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) rather
than restated in each — three copies of an obligation drift, and the whole point of it is accuracy.

**Owner decision [D12](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16: the three shipped 2.2.0
defects are documented, not patched.** No 2.2.1 will be cut. In exchange, the 3.0.0 release notes owe two
things:

> **A fourth defect joined the list on 2026-08-19, and the owner did not name it.**
> [Entry 14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) was
> filed 2026-08-18 and triaged 2026-08-19. D12 was taken against **three** defects because three were known;
> this triage reads it as governing *the shipped defects* rather than *those three*, and adds the fourth row
> below on that reading. **The reading is stated so it can be corrected** — nothing about the fourth defect
> distinguishes it from D12's own reasoning (silent, data-corrupting, near-empty consumer base), but the owner
> is entitled to say the scope was literal.

1. **All four defects appear as `Fixed` in `ProphetsWay.EFTools`' 3.0.0 `CHANGELOG.md` entry** — not
   `Changed`. A reader who sees only "soft-delete DAO bases rewritten" or "implements the new disposal
   contract" does not learn that the version they are running leaks connections, loses transactions, can
   resurrect a deleted row, and writes edits they never submitted.

   | Entry | What must be named as `Fixed` |
   |---|---|
   | [3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | `BaseEFDataAccess` never disposed the `DbContext` it constructed — a leaked context and connection per Data Access Layer instance |
   | [12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | `RootNonIdDao`'s commit and rollback **silently no-op** when a transaction was already open |
   | [13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | `RootBaseSoftDao.Update` **wipes the stored `DeletedDate`**, silently un-deleting a soft-deleted row; a second `Delete` refreshes the timestamp and returns `1` rather than `0` |
   | [14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) | **Added 2026-08-19.** `Insert` left the caller's own instance tracked, and `Get` returned the store's tracked instance under any context not explicitly set to `NoTracking` — so **an edit the caller never submitted was written on the next `SaveChanges`**. `Update` on an absent row threw rather than returning `0`. **This one changes behaviour a consumer may be relying on**, deliberately or by accident: after 3.0.0 those stray edits stop persisting, and a `catch` around `Update` stops firing |

2. **The release notes carry a known-issues note for 2.2.0** naming all four and recommending 3.0.0 — **with
   the target-framework caveat stated plainly**: [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes
   3.x **`net10.0`-only**, so a consumer on `net48`, `net8.0` or `net9.0` **cannot take 3.0.0 at all**.
   "Upgrade to 3.0.0" is not a remedy available to every 2.2.x consumer, and the note must not imply that it
   is.

**Three of the four fail silently** — no exception, just wrong data. That is why this is an obligation rather
than a formality: a consumer cannot discover any of them from a log, so the release note is the only
channel that reaches them. **Entry 14 is the only one of the four whose fix is itself a behaviour change a
consumer may be relying on**, so it owes a second sentence the other three do not: what stops happening.

**The premise this rests on is that the 2.2.x consumer base is near-empty**, and it is recorded so it can be
re-tested rather than assumed. If it ever turns out someone was on 2.2.x with a `net4x` or `net8.0` target,
D12 was taken without them in mind.

## Release Eligibility — the next release

The next release of this package is **v3.0.0**, a major. That is not a preference; it is forced. Entry 2
alone changes the transitive contract this package advertises, and entries 3, 4, 5 and 7 are each
independently breaking. `app-variables.yml` currently reads `Major: '2' / Minor: '2' / Patch: '0'` —
**an agent must never change it**; the bump is the owner's.

| # | Status | Eligible for v3.0.0? | Why |
| --- | --- | --- | --- |
| 1 | Scheduled | **Yes — and it is the gate; steps 1 and 3 of it have landed** | Nothing else can be verified until the proving ground compiles against the 3.x contracts. **It now does** — the 2026-08-16 build is green — but step 2 was satisfied by throwing stubs rather than an implementation, so the gate is passed for *compilation* and not for *conformance* |
| 2 | **Done** | **Landed 2026-08-16** | Both projects reference 3.1.0 and the solution compiles against it. This is what makes the release a 3.x |
| 3 | Scheduled | **Yes — rescoped** | `Dispose` landed 2026-08-16; what remains is the `ObjectDisposedException` guarding on every other member, which the parent's dispatcher cannot supply |
| 4 | Scheduled | **Yes — approved (D1)** | Only a major may drop targets, and this is the only major on the horizon |
| 5 | Scheduled | **Yes — strictly after 4** | The `#if` conditions go with 4; the destination is **`net10.0` alone**, a ratified exception to the house standard — see **D7** |
| 6 | Scheduled | **Yes — forced by 1** | The upstream base class it derives from no longer exists in that shape. **Rescoped 2026-08-16:** the deliverable is a suite that constructs the Entity Framework Data Access Layer itself, not a rebuilt set of adapters. **Resolved to shape B the same day** — so the deletion of the six adapters is in this release, while the *completion* of the entry additionally needs [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation), whose design is deferred until Lap 1 |
| 7 | Scheduled | **Yes — approved (D2), and only in a major** | Removing a transitive package reference is breaking. Postponing costs a second major |
| 8 | **Done** | **Landed 2026-08-16** | Trivial, isolated to a non-packaged project, and it did not wait for the rest — exactly as "eligible to land ahead" anticipated. **It was a licence item rather than hygiene**, and that exposure is now closed |
| 9 | Scheduled | **Yes** | Trivial, no build impact |
| 10 | Scheduled | **Yes — approved (D3)** | A breaking surface change is cheapest riding a major that is already breaking for four other reasons |
| 11 | Scheduled (test work) / Deferred (pipeline) | **Yes for the suite; the `LocalTestsOnly` removal is separately owned** | The contract cannot be *verified* without it; the CI plumbing is not this repository's decision alone |
| 12 | Scheduled (release note) / Rejected (2.2.x patch) | **Already, incidentally** | Nothing to schedule in code: the 3.x design removes the members that carry the defect. What **is** scheduled is the release-note obligation — the entry exists so the fix is *named* in the notes rather than shipping as an unannounced side effect. Triaged 2026-08-16 |
| 13 | Scheduled | **Yes — it is a precondition of 10, not a sibling of it** | The 3.x DAO families cannot be a re-wrap of `RootBaseSoftDao`; its `Update` and `Delete` semantics violate four of `IDepartmentDao`'s rules today. Filed 2026-08-16 |
| 14 | Scheduled | **Yes — and it cannot be deferred out** | Entry 10 deletes the types carrying the defect, so deferring it means writing six generic families **with** the adoption behaviour and breaking them again in 3.1.0. Triaged 2026-08-19. It is entry 13's counterpart on the **ordinary CRUD** path, and between them they specify every base in the package |

**The honest answer is that this is one indivisible release.** Entries 1–3 and 6 cannot be separated
without leaving the repository in a non-compiling state, and 4, 5, 7 and 10 are each cheap *now* and
expensive later because each needs a major version to land in. Splitting them across releases would buy two
more majors for no benefit.

**Nothing here is eligible for a patch or minor on the 2.2.x line.** That line's continuing job is to be
the EF6 answer, and it should receive no new work — see [entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework).

---

## 1 — Advance the `ProphetsWay.Example` submodule onto the 3.x contracts

**Status:** **Scheduled for v3.0.0** — 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Previously `Proposed`. Routed here from
[ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md), where it is recorded as the
highest-consequence open item in that repository. **The work is entirely in this one.**

**Partially landed — 2026-08-16.** Steps 1 and 3 of six are done, step 2 is done only to the extent of making
the compiler happy, and the entry stays `Scheduled` because the rest are not. It is deliberately **not**
`Done`: **the tree now compiles — `dotnet build ProphetsWay.EFTools.sln -c Debug` was green on this date — and
compiling is not conforming.** The sentence this paragraph replaced said the pointer move "costs the
repository its build"; that was true when written and is now history. Re-verified this date by opening
`.git/modules/ProphetsWay.Example/HEAD`, `ProphetsWay.EFTools.Tests/` (which now holds only `Constants.cs`
and its `.csproj`), `ProphetsWay.EFTools.Tests.csproj`, `ProphetsWay.Example.DataAccess.EF.csproj`,
`ProphetsWay.Example.DataAccess.EF/ExampleContext.cs`, `ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs`
and `ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`.

### The situation, verified rather than inherited

[.gitmodules](../.gitmodules) declares `path = ProphetsWay.Example`, `url = …/ProphetsWay.Example.git`,
`branch = main`. It is a **submodule, not a vendored copy** — `AGENTS.md` in this repository says otherwise
and is wrong. The two cannot drift; the pointer is simply **pinned**.

> **Correction, 2026-08-16 — the sentence above used to read "pinned pre-3.0.0", and that is now false.**
> The pointer was advanced to **`d845863`** — verified by reading
> `.git/modules/ProphetsWay.Example/HEAD`, which holds `d84586335a11d7c9efb7277b947015df0c15967e`, the tip
> of `ProphetsWay.Example`'s `main` and therefore its **3.1.0** tree. **Step 1 of [The work](#the-work)
> below has landed and steps 2–6 have not**, which is the whole of this repository's current build break.
> The table immediately following describes the **pre-advance** pointer and is retained as the record of
> what the advance brought in; read its left-hand column as history, not as the checked-out tree.

The checked-out submodule was compared against the standalone repository directly. **This table describes
the pointer as it stood before 2026-08-16**; every "absent" in the left column is now present on disk.

| | Pinned copy under `ProphetsWay.Example/` (**pre-advance**) | Current `ProphetsWay.Example` |
|---|---|---|
| `ProphetsWay.Example.Tests/TestDataAccessFactory.cs` | **absent** | present — the single construction site |
| `ProphetsWay.Example.Tests/ConventionShowcase/` | **absent** | present |
| Disposal / transaction / snapshot test files | **absent** | `DataAccessDisposalTests`, `DataAccessTransactionTests`, `SnapshotDeepCopyTests` |
| `Department`, `CompanyResource` | **absent** | present, with 19 and 10 numbered contract rules |
| `BaseUnitTests<T>` | `protected abstract T GetIExampleDataAccess { get; }` | `TestDataAccessFactory.CreateAs<T>()`, and the class is `IDisposable` |
| `docs/` | **absent** | three documents |

That last row is the one with teeth. `ProphetsWay.EFTools.Tests` used to supply the implementation by
**overriding an abstract property**: six adapter classes — `EFBaseDataAccessTests`, `EFCompanyDaoTests`,
`EFJobDaoTests`, `EFResourceDaoTests`, `EFTransactionDaoTests` and `EFUserDaoTests` — each declaring
`protected override … GetIExampleDataAccess => Constants.GetExampleDataAccess;` against
[Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs) and containing no test logic. Upstream, that hook
was replaced by a static factory. **Advancing the pointer broke this repository's test project
structurally, not just semantically** — every test class here lost the member it overrode. That is entry 6,
and it is not optional. **All six files were deleted on 2026-08-16**, so they are named here rather than
linked; the deletion is recorded in
[entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits).

### The build break the advance created — two of three now closed

Recorded here rather than left to be rediscovered, because a future agent meeting a red build needs to know
it is this entry's mid-flight state and not a regression. Three independent breaks were found on
2026-08-16; **one remains at the end of that day.**

1. ~~**TFM mismatch.**~~ **Closed 2026-08-16.**
   [ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)
   targeted `net472;net48;net80;net90` against a `ProphetsWay.Example.Tests` the 3.1.0 pointer had
   retargeted to `net48;net10.0`, leaving three of four legs with no compatible asset. All three EFTools
   projects are now `net10.0` alone, which binds. Delivered by
   [entry 5](#5--retarget-to-the-house-tfm-standard).
2. ~~**The missing hook**, above.~~ **Closed 2026-08-16** by deleting the six adapters —
   [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits). The project now
   contains no tests, which is the intended temporary state under **D10**, not a loss: the 35 upstream
   facts are parked awaiting the seam.
3. **`ExampleDataAccess` no longer satisfies `IExampleDataAccess`. — CLOSED 2026-08-16, and read the closure
   carefully.**
   At 3.1.0 that interface aggregates `IDepartmentDao` and `ICompanyResourceDao` and inherits `IDisposable`
   — verified by opening
   `ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`, whose declaration reads
   `: IBaseDataAccess, ICompanyDao, IJobDao, IUserDao, ITransactionDao, IResourceDao, IDepartmentDao, ICompanyResourceDao`.
   **`ExampleDataAccess.cs` now supplies all three**, but it supplies the two new DAO groups as **11 members
   that throw `NotImplementedException`** — verified by searching the project for `NotWrittenYet`, which
   matches 12 times in that one file: eight `IDepartmentDao` members, three `ICompanyResourceDao` members,
   and the helper itself. `Dispose` is inherited from `BaseEFDataAccess`. **The break is closed; step 2 below
   is not.**

### The work

0. **Nothing local blocks the pointer any more.** The submodule working tree carried an obsolete local
   modification against the pinned pre-3.0.0 commit; the owner **approved discarding it, and it has been
   discarded** — [D9](purpose-and-scope.md#owner-decisions--2026-08-15). The tree is clean, so the advance
   is a pointer move rather than a merge. Recorded because "the submodule had uncommitted changes" is
   exactly the kind of finding a later pass would otherwise re-report as an obstacle. **This step is now
   history rather than a precondition — see the note under [D9](purpose-and-scope.md#owner-decisions--2026-08-15).**
1. ~~Advance the submodule pointer to the published 3.1.0 commit.~~ **Done, 2026-08-16** — the pointer is at
   `d845863`. **This is the only step of this entry that has landed**, and steps 2–6 not landing with it is
   what leaves the repository non-compiling.
2. Add `Department` and `CompanyResource` — entities, `I*Dao` implementations, EF mappings in
   `ExampleContext`, and the schema they need. **No longer a build break, and no longer close to done:** the
   11 throwing stubs satisfy the compiler and nothing else.

   **The mapping half has not been started at all, and it is sharper than it looks — verified 2026-08-16 by
   opening [ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs).** That file declares
   five `DbSet<>` properties — `Company`, `User`, `Resource`, `Transaction`, `Job` — and `OnModelCreating`
   calls `ToTable` for the same five. **There is no `DbSet<Department>`, no `DbSet<CompanyResource>`, and no
   mapping for either.**

   Two consequences worth having written down before anyone starts:

   - **Model building will fail at runtime the moment anything materializes the model.** Nothing does yet,
     because every member that would touch those two entities throws first. The throwing stubs are therefore
     *hiding* this, not fixing it, and the first real implementation of any one of the 11 members surfaces it.
   - **`CompanyResource` is keyless, and EF Core forces a choice the contract has already made.** Per
     `ICompanyResourceDao` rule 1 a row is identified by the `CompanyId`/`ResourceId` pair. EF Core needs
     either an explicit composite `HasKey(x => new { x.CompanyId, x.ResourceId })` or `HasNoKey()` — and
     **`HasNoKey()` makes the entity read-only, which forbids the `Insert` and `Delete` that rules 3 and 4
     require.** The composite key is therefore the only viable mapping. "Keyless" in this contract means *no
     surrogate identifier property on the entity*, not *no primary key in the store*; the database project's
     `CompanyResources.sql` already carries the composite primary key. Do not read `IBaseEntity` as an
     instruction to reach for `HasNoKey`.

   **Owner decision [D13](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16 — the `Department`
   half of this step is approved as a hand-written concrete `DepartmentDao`.** Explicitly *not* by deriving
   from today's soft base, and explicitly *not* by waiting for
   [entry 10](#10--collapse-the-guidintlong-dao-triplication)'s generic families. The owner's reasoning:
   *"the point of the generic tests and classes was to just reduce all the duplicative code, but in this case
   there is a real need for it."* The Data Access Object written here becomes the **evidence** for what the
   generic soft-delete family must look like — see
   [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance), whose
   recommendation this ratifies, and [entry 10](#10--collapse-the-guidintlong-dao-triplication), which it
   constrains.

   **Verified 2026-08-16:** `ProphetsWay.Example.DataAccess.EF/Daos/` holds `CompanyDao.cs`, `JobDao.cs`,
   `ResourceDao.cs`, `TransactionDao.cs` and `UserDao.cs` — **no `DepartmentDao.cs`, no
   `CompanyResourceDao.cs`.** D13 approves work that has not started.
3. Implement `Dispose` and the three transaction members against the real `DbContext` — which is
   [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) in the library and its
   consequence here. **Landed 2026-08-16** at the library level: `BaseEFDataAccess` now carries `Dispose`,
   and the three transaction members were already forwarders. What entry 3 still owes is the
   `ObjectDisposedException` guarding, not the disposal itself.
4. Satisfy the **snapshot rule** — reads return deep snapshots, writes read their argument. EF Core's
   change tracker makes this the interesting one: the existing examples already set
   `QueryTrackingBehavior.NoTracking`, which is a start and not a proof.
5. Satisfy the **ordering rule** — an explicit `ORDER BY` on both `GetAll` and `GetPaged`.
   `RootDao.GetPaged` already orders by `Id`; **`RootDao.GetAll` does not** — it is
   `Dataset.ToList()`. [repo-profile.md](repo-profile.md) adds a third case this entry had missed:
   **soft-delete paging in `RootBaseSoftDao` is not explicitly ordered either.** All three are divergences
   the rule was written to catch, and all three are in the library, not the example. **`RootBaseSoftDao` has
   since been found to violate four of `IDepartmentDao`'s rules besides this one — see
   [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) before
   implementing the `Department` half of step 2.**
6. Run `dotnet test --filter "Scope=Contract"` and report the result.

### Why this is the gate

Every other entry is verified by this test suite. Until it runs, entries 2, 3, 4, 5 and 7 are changes
whose correctness is asserted rather than demonstrated. The repository's own `LocalTestsOnly: 'yes'` means
CI will not catch a mistake either.

### Open question — **answered**

> Should the EF implementation move to the `Microsoft.EntityFrameworkCore.Sqlite` in-memory mode, or a
> Testcontainers SQL Server, so that `LocalTestsOnly: 'yes'` can be lifted and CI actually runs the
> contract suite?

**Both**, per owner decision [D4](purpose-and-scope.md#owner-decisions--2026-08-15): SQLite in-memory as
the fast CI contract/query gate, and a SQL Server container for provider fidelity. The reasoning that made
this urgent stands — the `InMemory` provider cannot honour transactions, so the transaction-scope contract
tests **cannot pass against it**, which means the arrangement in place today is structurally unable to
verify a contract this package must now meet.

This has been promoted out of a footnote into its own entry,
[11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container), because it is now
approved work with its own scope rather than a question downstream of
[entry 7](#7--stop-forcing-a-database-provider-on-every-consumer).

---

## 2 — Move the `ProphetsWay.BaseDataAccess` reference from 2.5.0 to 3.1.0

**Status:** **Done — 2026-08-16.** Previously `Scheduled for v3.0.0` (2026-08-15, owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15)), and `Proposed` before that. A one-line edit with the
largest consequence in the file.

**Why `Done` rather than `Scheduled` until v3.0.0 ships.** The entry's deliverable is a reference version, and
the reference is at 3.1.0 in both projects. What made this triageable today rather than a week ago is that the
solution now **compiles against 3.1.0** — a version bump that does not build is a claim, not a change, and the
verified-green build of 2026-08-16 is the evidence this entry was waiting on. The release it rides in is a
separate fact, recorded in [Release Eligibility](#release-eligibility--the-next-release); an entry is not held
open for the act of shipping.

**Landed — 2026-08-16.**
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) and
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
**now both reference `ProphetsWay.BaseDataAccess` 3.1.0** — re-verified by opening both files on 2026-08-16.
The sentence this paragraph replaced said they were on **2.5.0**, which was true when written.

**This is not a version-hygiene item.** A library whose stated purpose is "implements the
`ProphetsWay.BaseDataAccess` contracts" and which references a superseded major is not implementing the
contracts it advertises. The README's paradigm claim was, until this landed, a statement about 2.x — and it
remains one **for the published package**, which is still 2.2.0 carrying 2.5.0. Only the tree has moved.

What 3.0.0 changed that lands directly on this package:

- `IBaseDataAccess` now extends `IDisposable`
  ([IBaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/IBaseDataAccess.cs) line 164)
  and `BaseDataAccess` declares `public abstract void Dispose();`
  ([BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs) line 112).
  [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) overrode the three transaction members
  and nothing else, so it would not have compiled. **`Dispose` has since been implemented there — verified
  2026-08-16 by opening the file** — which is [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess).
- Exceptions from derived DAL methods now propagate **unwrapped** — no `TargetInvocationException`. Any
  consumer catching the wrapper breaks.
- The identifier property must be **public**; an explicit interface implementation now throws
  `DataAccessConventionException` before dispatch.

**Breaking for consumers**, transitively: a `ProphetsWay.EFTools` 3.0.0 consumer is resolving
`ProphetsWay.BaseDataAccess` 3.1.0 whether they asked for it or not, and their own DAL must now supply
`Dispose`. This is the single strongest reason the next release must be a major.

**Do not split this from entry 3.** Landing it alone leaves the repository non-compiling. **In the event it
was not split** — both landed on 2026-08-16 and the tree compiles, which is the outcome this warning existed
to obtain. Retained as the reason, not as an outstanding instruction.

---

## 3 — Implement the 3.x disposal contract in `BaseEFDataAccess`

**Status:** **Scheduled for v3.0.0, and rescoped 2026-08-16.** Scheduled 2026-08-15 by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15); previously `Proposed`.

### The rescope — 2026-08-16, and read this before reading the rest of the entry

**Half of this entry has landed and the other half has not, and the halves are not the ones the entry was
written around.** Verified by opening
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) on this date.

| Deliverable | State |
|---|---|
| `Dispose` exists, is idempotent, never throws, rolls back an open transaction, disposes the context it created | **Landed** — `public override void Dispose()` reads `_disposed`, returns early, rolls back `Context.Database.CurrentTransaction` inside a swallowing `try`/`catch`, then disposes the context in a second one |
| **Every member other than `Dispose` throws `ObjectDisposedException` once disposed** | **Outstanding — this is now the whole of the entry** |
| Ownership flag distinguishing a created context from an injected one | **Superseded** by `docs/api-contract.md`'s `ContextOwnership` enum (A9). Not this entry's to re-decide |

**The remaining half is not a detail, and it is not covered anywhere else.** `IBaseDataAccess` requires that
*every* member other than `Dispose` throw `ObjectDisposedException` once the instance is disposed.
`_disposed` is **set and never read outside `Dispose` itself** — so a disposed `BaseEFDataAccess` currently
fails the contract on **seven** members, not one:

- the three transaction members overridden here — `TransactionStart`, `TransactionCommit`,
  `TransactionRollBack` — which reach a disposed `Context.Database` and surface whatever EF Core throws;
- the four concrete dispatcher members inherited from `BaseDataAccess` — `GetAll<T>`, `GetPaged<T>`,
  `GetCount<T>` and `Get<T>` — verified by opening
  [BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs), where
  each is `public virtual` and reflects onto the derived class with no disposal state to consult.

**The parent cannot supply this guard and it is not an oversight there.** `BaseDataAccess` holds no
connection, context or disposal state — its own `<summary>` on `Dispose` says exactly that, which is why the
member is abstract. The guard has to live here. That answers open question 2 below on the evidence rather
than leaving it open, and the design question it asks — *guard here or in the parent* — has only one
answer available.

**A green build says nothing about this.** The 2026-08-16 build compiles; nothing in this repository executes
a disposed instance, because there is no test suite (entry 6). The gap is invisible until either the upstream
suite runs against this Data Access Layer or a consumer meets it.

**Q2 is superseded, not open** — see the note at the foot of this entry. The entry's remaining scope is one
sentence: **guard the seven members.**

### The mechanical part

`BaseDataAccess` declares `Dispose` abstract, so `BaseEFDataAccess` must supply it. The parent's contract —
the source of truth is the `<remarks>` on `IBaseDataAccess`, not this paragraph — requires that `Dispose`
be **idempotent**, **never throw**, **roll back an open transaction**, cause every *other* member to throw
`ObjectDisposedException` thereafter, and dispose **what the instance created and not what was handed to
it**.

### The part that is not mechanical

[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) constructs its own context:

```csharp
Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { connectionString });
```

Under "disposes what it created," that context **must** be disposed. Both current constructors create it,
so today the answer is unambiguous.

**It stops being unambiguous the moment a DI-hosted consumer asks for an injected context**, which is the
normal way EF Core is used in ASP.NET Core and which this library currently cannot accommodate at all. A
`BaseEFDataAccess(DbContext context)` overload would receive a context whose lifetime the container owns,
and disposing it would be a bug — the same bug in reverse.

The contract already answers this correctly ("what it created, not what was handed to it"), so the design
work is to make the *code* able to tell the difference: a flag captured at construction, set by whichever
constructor ran. That is small. What is not small is deciding whether to add the injecting constructor at
all in v3.0.0 — it widens the purpose sentence from "constructs your context for you" toward "participates
in your composition root."

### Recommendation

Implement `Dispose` with an ownership flag **now**, defaulting to owned, even if the injecting constructor
is not added in this release. Retrofitting ownership tracking after consumers depend on the disposal
behaviour is the expensive order.

### Open questions for the owner

1. **Superseded (Q2).** Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one?
   Owner decision [D2](purpose-and-scope.md#owner-decisions--2026-08-15) made this *more* pressing, not
   less: now that the consumer configures the provider themselves through `DbContextOptions`, the distance
   between "you pass options" and "you pass the context" is one step, and a DI-hosted consumer will ask for
   that step. **`docs/api-contract.md` settles it as the `ContextOwnership` enum (A9)** — a required
   constructor argument with no default, plus a sealed `Dispose`. Read that document; do not re-decide it
   here.
2. **Answered, 2026-08-16 — and it is not a question, it is the entry's remaining work.** Should
   `ObjectDisposedException` guarding be added here, or is the parent's dispatcher expected to guard? **Here.**
   The parent holds no state and cannot — confirmed by opening `BaseDataAccess.cs`, whose `Dispose` is
   abstract for precisely that reason. The scope is **seven members**, not the three transaction members this
   line originally named: the four concrete dispatcher members inherited from `BaseDataAccess` are equally
   bound by the contract and equally unguarded. See [The rescope](#the-rescope--2026-08-16-and-read-this-before-reading-the-rest-of-the-entry).
### Appended 2026-08-15 — this is a shipped defect, not only a forward-compatibility gap

The framing above reads as *"3.1.0 will require a `Dispose` we do not have yet."* That understates it.
At the time this was written, [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) contained
**no `Dispose`, no `IDisposable`, and no finalizer** — its only members were `Context`, two constructors,
and three transaction forwarders — and both constructors built the context with `Activator.CreateInstance`.
**Nothing ever disposed it.** That is a live resource leak in the **published 2.2.0 package**, independent
of any 3.x contract, and that half of this note remains true for as long as 2.2.0 is the published version.

> **Factual correction — 2026-08-16.** The description of the *file* above is now history. Opening
> `BaseEFDataAccess.cs` on this date shows `public override void Dispose()` present: it sets a `_disposed`
> flag and returns early when already set, rolls back `Context.Database.CurrentTransaction` inside a
> `try`/`catch` that swallows the failure, then disposes the context in a second guarded `try`/`catch`, with
> `<remarks>` stating that the context is disposed because both constructors construct it. **The published
> package is unchanged; the working tree is not.** `_disposed` is set but **never read outside `Dispose`**, so
> the second open question below is not merely still open — it is now the entirety of what this entry owes.
> **Status rescoped rather than closed**, 2026-08-16: see
> [The rescope](#the-rescope--2026-08-16-and-read-this-before-reading-the-rest-of-the-entry).

Two consequences:

- **`Changelog Author` must record this as `Fixed`, not `Changed`.** A reader of the 3.0.0 notes who sees only
  "implements the new disposal contract" will not learn that the version they are running leaks connections.
- The design question this entry calls "not mechanical" is **answered** — `docs/api-contract.md` settles it as
  the `ContextOwnership` enum (A9), a required constructor argument with no default so the branch can never be
  inferred, plus a sealed `Dispose` so a derived DAL cannot omit it. Open question Q2 above is superseded by
  that document; read it rather than re-deciding here.

The defect was found by `Repo Analyst` during a verification pass and confirmed independently by
`Contract Reviewer`; both opened the file. Recorded so it is not rediscovered as new.

### Triaged 2026-08-16 — the 2.2.x patch for the leaked context is **Rejected**

**This entry had left the question implicit rather than open**, which is worse: it said the leak "remains true
for as long as 2.2.0 is the published version" and stopped there, so a later reader could reasonably have
inferred a patch was still on the table. It is not.

**Owner decision [D12](purpose-and-scope.md#owner-decisions--2026-08-15):** *"I don't believe anyone is
currently using that library in any meaningful capacity, so it's document the bug and recommend to update to
v3.0.0."* No 2.2.1 will be cut. This **upholds [D1](purpose-and-scope.md#owner-decisions--2026-08-15)** rather
than carving an exception from it.

**Re-verified before this status was written**, not affirmed:
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) was opened on 2026-08-16. Both constructors
still read `Context = (DbContext)Activator.CreateInstance(...)`, so the *published* 2.2.0 shape of the defect
is real and unchanged; the working tree's `Dispose` is what the rescope above records.

**What is owed instead:** the release-note obligation, stated once in
[The `Changelog Author` obligation](#the-changelog-author-obligation--d12) — including the caveat that
[D7](purpose-and-scope.md#owner-decisions--2026-08-15) puts 3.0.0 out of reach of a `net48`/`net8.0`/`net9.0`
consumer, so "upgrade" is not a remedy available to all of them.

**The status line is unchanged.** The entry remains `Scheduled` for v3.0.0 on its live deliverable — the
`ObjectDisposedException` guarding on seven members — which D12 does not touch.

---

## 4 — Make 3.x Entity Framework Core-only — retire EF6 and .NET Framework

**Status:** **Scheduled for v3.0.0** — 2026-08-15. **Approved by the owner as
[D1](purpose-and-scope.md#owner-decisions--2026-08-15).** Previously `Proposed — awaiting the owner's scope
decision`; this was the checkpoint, and it has been passed. The recommendation below was accepted as
written, and the argument is preserved rather than trimmed — it will be questioned again.

### The recommendation

**Make `ProphetsWay.EFTools` 3.x EF Core-only. Leave the published 2.2.x as the EF6 / .NET Framework
answer, and give that line no new work.**

The full argument, with the file-level evidence, is in
[purpose-and-scope.md § The EF6 Question](purpose-and-scope.md#the-ef6-question--settled).
The short form, because the reasoning belongs where it can be checked:

- **The divergence is semantic.** [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) `Update` is
  `Dataset.AddOrUpdate(item)` on EF6 — an **upsert that inserts a missing row** — and
  `Single(…)` + `SetValues` + `EntityState.Modified` on EF Core, which **throws** on a missing row. One
  method name, two behaviours, one package ID.
- **The public surface differs by target.** The `DbContextOptions` constructors on `BaseEFContext` and
  `BaseEFDataAccess` exist **only** on modern targets — and the README recommends that pattern as
  preferred, to readers who may not have it.
- **The preprocessor is already unreadable in one place.** In
  [ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  an `#if NET471 || NET48` block opens a constructor brace that an `#if NET8_0_OR_GREATER` block closes.
  No single compilation shows a reader the whole file.
- **It blocks the retarget.** The conditions are hardcoded TFM lists — `#if NET461 || NET471 || NET48` and
  `#if NET8_0_OR_GREATER`. `netstandard2.0` matches **neither**, so every affected file would compile to
  missing usings and empty bodies. EF6 retention is not neutral with respect to
  [entry 5](#5--retarget-to-the-house-tfm-standard); it stands in front of it.
- **Nothing verifies the EF6 leg.** `LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml)
  means CI does not run these tests at all. The .NET Framework leg is maintained and never executed.
- **The migration cost is close to zero.** No known live consumer requires .NET Framework or EF6, the owner
  has accepted a major bump, and **2.2.x remains published and installable**. Nobody is stranded; they
  simply stop receiving new work.

### The strongest argument against, stated at full strength

The EF6 branch **works today**, and deleting it is materially harder to reverse than keeping it. Those
`#if` blocks encode real knowledge about EF6's transaction types and upsert behaviour that a future port
would have to rediscover from scratch. If a .NET Framework consumer appears next year, today's cheap answer
("it already builds") becomes an expensive one.

The counter is that the knowledge is **tagged, not deleted** — it lives in git history and in a published,
installable 2.2.x package. But the owner should weigh it rather than be told it is settled.

### Explicitly not recommended

- **Not** a `ProphetsWay.EFTools.EF6` companion package. A second repository, pipeline, version line,
  changelog and support surface for an audience currently measured at **zero** is the maintenance tax this
  workspace's conventions exist to avoid. **The owner rejected this explicitly in D1. It is closed.**
- **Not** deprecating or unlisting 2.2.x. It should remain discoverable as the EF6 answer, and receive no
  new work.

### What follows from the approval

- The `<Description>` and the purpose sentence say **"Entity Framework Core"**, not "EntityFramework".
- The README's `DbModelBuilder` example goes. **`README Author`'s file, not this agent's.**
- `CHANGELOG.md` carries a breaking-change entry pointing EF6 consumers at 2.2.x. **`Changelog Author`'s
  file.**
- ~~The `EntityFramework` 6.5.1 `PackageReference` and the `$(TargetFramework.StartsWith('net4'))` item group
  go.~~ **Done 2026-08-16 by `Modernizer`** — verified by opening all three `.csproj` files, none of which
  now contains an `EntityFramework` reference or a `net4`-conditional `ItemGroup`. **EF6 is unreferenced by
  this repository.**
- Roughly 60 `#if NET461 || NET471 || NET48` / `#if NET8_0_OR_GREATER` blocks across 26 files collapse to
  unconditional EF Core code, which is what unblocks [entry 5](#5--retarget-to-the-house-tfm-standard).
  **Not done** — the blocks are still in the C# sources, where they are now dead under a `net10.0`-only
  build. `Implementer`'s work.

### The consequence the approval created — `netstandard2.0` is unreachable, and that is now ratified

Not foreseen when this entry was written, and it is a **conflict with a family-wide convention**, so it is
recorded here rather than left to be rediscovered during the retarget. **It has since been settled** by
owner decision [D7](purpose-and-scope.md#owner-decisions--2026-08-15), which closed **Q1**.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) pins a single
`Microsoft.EntityFrameworkCore` version across the package. **The version is deliberately not quoted here** —
`Modernizer` was bumping it on 2026-08-16 while this correction pass ran, and a number written from a
mid-flight file is a claim nobody can stand behind. Read it from the csproj. What matters to this entry is
unaffected by the number: EF Core has shipped no `netstandard2.0` asset since 3.1 — 5.0
onward are runtime-targeted, and **EF Core 10 exposes only `net10.0`**. So an EF Core-only
`ProphetsWay.EFTools` **cannot carry the house standard's permanent reach floor**; a `netstandard2.0`
target would be one that cannot restore its own primary dependency.

This is not an argument against D1 — the EF6 branch was the only thing that could ever have served
`netstandard2.0` here, and it was serving it via `net4x` rather than the floor anyway. It is the reason
this repository now holds a **ratified exception**: **3.x targets `net10.0` only**, and existing
`net4x`/`net8.0`/`net9.0` consumers stay on 2.2.x. The exception still needs a line in `AGENTS.md`, which
is **not this agent's file**. The reasoning is in
[purpose-and-scope.md § The `net10.0`-Only Exception](purpose-and-scope.md#the-net100-only-exception--settled).

### If the owner had declined

Retained because reversal remains possible until the code lands. Entry 5 would have needed re-planning: the
`#if` conditions become feature-shaped rather than TFM-shaped (`#if EF6` / `#else`, driven by a
`DefineConstants` in the conditional `ItemGroup`). That was the only way `netstandard2.0` could have been
added without breaking both branches — and, per the paragraph above, it is now moot for a different reason.

---

## 5 — Retarget to the house TFM standard

**Status:** **Scheduled for v3.0.0** — 2026-08-15. **Unblocked**, not merely sequenced, by the approval of
[entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework). The destination was the
last open part of this entry; it is **settled by [D7](purpose-and-scope.md#owner-decisions--2026-08-15)**,
which closed **Q1**.

**The retarget landed on 2026-08-16.** The status line above is untouched — triage is `Purpose Refiner`'s —
but the table below now records history in its left column. All three EFTools-owned projects read
`<TargetFrameworks>net10.0</TargetFrameworks>`, verified by opening each `.csproj`.

| Project | TFMs before 2026-08-16 | House standard | **Approved destination (D7)** | On disk now |
|---|---|---|---|---|
| `ProphetsWay.EFTools` | `net461;net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** | **`net10.0`** |
| `ProphetsWay.EFTools.Tests` | `net472;net48;net80;net90` | `net48;net10.0` | **`net10.0`** — the `net48` leg has nothing to bind | **`net10.0`** |
| `ProphetsWay.Example.DataAccess.EF` | `net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** | **`net10.0`** |

Four separate problems, worth separating because they have different fixes — **all four are about the
left-hand column, and the retarget addressed them together**:

1. **`net461` and `net471` are end of life**, and `net472` appears only in the test project.
2. **`net80`/`net90` are non-canonical monikers** — the dotted form `net10.0` is the convention.
   Undotted parses; it is inconsistent across the family and reads as a typo.
3. **`netstandard2.0` is absent**, and after [D1](purpose-and-scope.md#owner-decisions--2026-08-15) it
   **cannot be added** — see below. What was recorded here as a gap is a permanent, deliberate and now
   **ratified** exception.
4. **.NET 8 and .NET 9 both reach end of life on 10 November 2026.** Retargeting to `net90` would be
   retargeting onto debt.

**The former blocker is gone.** Adding `netstandard2.0` while the `#if NET461 || NET471 || NET48` /
`#if NET8_0_OR_GREATER` pairs remained would have produced files with **no `using` directives and empty
class bodies**. [Entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)'s approval
deletes roughly 60 conditional blocks rather than auditing them.

**The destination is not the house standard, and that is now a decision rather than a question.**
No `Microsoft.EntityFrameworkCore` version this package could reference ships a `netstandard2.0` asset — EF
Core has been runtime-targeted since 5.0, and **EF Core 10 exposes only `net10.0`** — so an EF Core-only
library cannot carry the family's
reach floor at all. **[D7](purpose-and-scope.md#owner-decisions--2026-08-15) approves `net10.0` alone**, for
the library, the test project and `ProphetsWay.Example.DataAccess.EF` alike, and keeps existing
`net4x`/`net8.0`/`net9.0` consumers on the published 2.2.x line.

**Why not `net10.0` plus one LTS predecessor**, which this entry previously left as an option: an EF Core 10
dependency has no asset for an earlier runtime, so a second TFM would have to pin an older EF Core on that
leg — reintroducing the two-products-under-one-package-ID problem
[entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) exists to end.

The justification for the exception is that the reach floor buys nothing here: `netstandard2.0` exists to
make a **portable contract** consumable from .NET Framework upward, and this package's public surface is
generic base classes over `DbContext` — an **EF Core implementation, not a portable contract**. No consumer
the floor would reach could use what they found there. That exception belongs in `AGENTS.md`, which
**this agent may not write**.

The test project's `net48` leg goes with it for the same reason. In
`ProphetsWay.BaseDataAccess` and `ProphetsWay.Example` that leg exists to bind the `netstandard2.0` asset
and verify .NET Framework behaviour; here there will be no such asset to bind.

**Adding a TFM is a MINOR bump and removing one is MAJOR**, so this belongs in v3.0.0 or waits for v4.

**Not this agent's edit.** `Modernizer` owns csproj changes; this entry is the scope justification for them.

---

## 6 — Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory and `Scope` traits

**Status:** **Scheduled for v3.0.0** — 2026-08-15. Forced by
[entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts), not chosen.
**Rescoped 2026-08-16 — see [The rescope](#the-rescope--2026-08-16) below — and the fork that rescope opened
was resolved the same day; see [The resolution](#the-resolution--2026-08-16-shape-b-the-direction-only).**
The status does not change; the deliverable does. **The status was re-examined against the resolution and
deliberately left at `Scheduled`:** the deletion of the six adapters is real work in this release, and the
entry was already `Scheduled` before the fork existed. Choosing between two shapes never blocked the status,
only the estimate.

`ProphetsWay.EFTools.Tests` derived from the upstream test classes and supplied the implementation by
overriding `protected abstract T GetIExampleDataAccess { get; }` — six files, all following the same four-
to five-line shape. Upstream, that hook no longer exists: `BaseUnitTests<T>` now calls
`TestDataAccessFactory.CreateAs<T>()` and implements `IDisposable`.

**Advancing the pointer therefore broke every test class in this project at compile time.** The rebuild
is not cleanup; it is the cost of entry 1. **As of 2026-08-16 that is past tense** — the pointer has
advanced, and **the six adapters have since been deleted**; see
[What has landed](#what-has-landed--2026-08-16) below.

### The rescope — 2026-08-16

This entry was written as *"rebuild the adapters against the new base classes."* **That is no longer the
shape of the work, and the difference is not cosmetic.** Verified by opening
`ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs` in the standalone repository:

```csharp
public static IExampleDataAccess Create()
{
    //>>> The one line to change to point this suite at another implementation. <<<
    return new ExampleDataAccess();
}
```

`CreateAs<T>()` calls `Create()` and casts. **There is no hook to override, in either direction.** The
upstream suite has exactly one construction site, it is `static`, it names the **NoDB** implementation, and
it takes no parameter. So:

> **Superseded 2026-08-16 as a description of the current file.** The seam landed upstream that day and
> `Create()` no longer contains the construction — see
> [The seam landed upstream](#the-seam-landed-upstream--2026-08-16-and-nothing-has-been-run-through-it).
> The paragraph and snippet above are the state that **caused** this rescope, and are preserved as its
> reasoning rather than as a report of the tree.

- **The adapter concept is gone, not broken.** Six classes whose only content was an override have nothing
  left to override. They are to be **deleted**, not rewritten. Anything preserved from them is
  `Constants.cs`'s connection handling, and even that is provider wiring rather than a test hook.
- **An Entity Framework conformance run is a factory swap, not inheritance** — and the factory is not in
  this repository. It is in `ProphetsWay.Example`, consumed here as a **pinned submodule**, and
  [ProphetsWay.Example FR 5's](../../ProphetsWay.Example/docs/feature-requests.md) standing instruction is
  that files under `ProphetsWay.Example/` are never edited from this side.
- **Therefore this entry, as previously scoped, cannot be completed from within this repository.** That is
  the finding, and it is new. Inheriting the upstream classes and swapping the subject underneath them is
  not available, because the subject is chosen by a `static` method that neither accepts an argument nor
  consults one.

**What the deliverable becomes.** One of two shapes — **and the choice has been made; the table is kept
because the rejected half is the reasoning:**

| Shape | What it means here |
|---|---|
| ~~**A — a local suite**~~ | ~~This repository writes its own `Scope`-traited tests against `IExampleDataAccess`, constructing `ProphetsWay.Example.DataAccess.EF.ExampleDataAccess` itself. No dependency on `ProphetsWay.Example.Tests` at all; the `ProjectReference` to it goes.~~ **Declined by the owner, 2026-08-16.** It costs a permanent second copy of the assertions, which is exactly what the upstream suite exists to avoid |
| **B — an upstream seam** | **Chosen, 2026-08-16.** `ProphetsWay.Example` grows a way for a consuming repository to supply the implementation, and this repository provides it. Preserves the "same tests, different implementation" property that is the entire argument. **Requires a change in the other repository**, which is why it is filed there as [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation) |

**Shape B is the one that keeps the paradigm claim true**, and A is the one that could be done unilaterally.
A is also the one that quietly destroys the demonstration: two copies of the suite that must be kept in
step is the duplication problem the submodule arrangement exists to prevent, and the moment they diverge
the sentence "the tests do not change to accommodate it" stops being checkable. **That is the reasoning the
owner accepted.**

**Note the interaction with [ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md),
which is `Rejected`.** That entry declines reading the implementation choice from an environment variable
or `.runsettings`, on the grounds that one obvious line beats a lookup. **Shape B is not that proposal** —
it does not ask for configuration-driven selection, and the single obvious line can stay exactly where it
is as the default. It asks only that the line be *reachable* from a repository that cannot edit it. Do not
let FR 8's rejection be read as having already declined this; it was answering a different question.
**Choosing shape B does not reopen FR 8, and this distinction must survive any future summary of either.**

### The resolution — 2026-08-16: shape B, the direction only

**~~Open question for the owner: A or B?~~ Answered: B.** The open half of the rescope is closed, and what
follows is the whole of what the answer settles.

- **Shape A is foreclosed.** A duplicate local suite in this repository is declined, on the grounds stated
  above: it ends the demonstration `ProphetsWay.Example` exists to provide.
- **The six adapter classes were to be *deleted*, not rebuilt — and they have been.**
  `EFBaseDataAccessTests`, `EFCompanyDaoTests`, `EFJobDaoTests`, `EFResourceDaoTests`,
  `EFTransactionDaoTests` and `EFUserDaoTests` contained an override of a member that no longer exists
  upstream and nothing else. There was no repair; the concept is gone. Anything worth keeping is
  [Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs)'s connection handling, and that is provider
  wiring rather than a test hook. **Deleted 2026-08-16** — see
  [What has landed](#what-has-landed--2026-08-16).
- **The direction is committed; the seam's design is deliberately deferred.** Nobody has yet attempted to
  satisfy the 3.1.0 contracts in Entity Framework, so **the seam's requirements are unknown.** The decision
  forecloses shape A and **nothing else**. The seam is to be designed once **Lap 1** has shown what it must
  carry.
- **Do not read a committed direction as an approved design, and do not read the absence of a seam design as
  unfinished work.** There is no design owed by anyone at this date. The trigger is Lap 1.

**A mechanism was proposed for the seam and it cannot compile.** It is recorded in full — with the reason and
with the viable shape that was sketched against it as a *sketch only* — in
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation),
because every file it concerns lives there. The short form, so nobody re-derives it from this side: making
`TestDataAccessFactory.Create()` `protected` and overriding it is impossible — the type is a `public static
class`, a static class cannot declare a `protected` member, and a static method is never virtual.

### What has landed — 2026-08-16

**The deletion half of this entry is done, and it is re-verified rather than inherited. The status line is
untouched** — the entry cannot be `Done` while its second deliverable waits on another repository.

Re-verified 2026-08-16 by listing `ProphetsWay.EFTools.Tests/`: it contains **`Constants.cs`,
`ProphetsWay.EFTools.Tests.csproj`, and build output. The six adapters are gone.**

**The upstream dependency is now in flight rather than merely scheduled — 2026-08-16.**
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)
is being implemented by an `Interface Architect` as this is written. **It is not done, and this entry must not
be advanced on the strength of it** — the seam is unverified, and one constraint that emerged during its
design lands directly here: **a per-class virtual hook is rejected**, on the grounds that it recreates the six
adapter classes deleted above. Whatever shape the seam takes, this repository does not get its adapters back.

Three things worth stating plainly, because a future reader will otherwise read "a test project with no
tests" as damage:

- **The project containing no tests is correct and temporary.** It is the intended resting state under
  **D10** until the upstream seam exists.
- **The 35 upstream tests are parked, not lost.** They live in `ProphetsWay.Example.Tests` and are
  unaffected by anything done here.
- **Nothing green went red.** `LocalTestsOnly: 'yes'` already skipped them in CI, and the project had not
  compiled since the submodule advance. The deletion removed something that was already not running.

### What this entry can and cannot deliver from inside this repository

**It cannot be *completed* here.** That was already the finding of the rescope, and the resolution confirms
rather than removes it. This entry now has an explicit cross-repository dependency:

| Deliverable | Where it lands | Blocked on |
|---|---|---|
| ~~Deleting the six adapters~~ | **Here** | **Done 2026-08-16** |
| Deleting the dead `#if` branches in `Constants.cs` | **Here** | Nothing — still outstanding; `Implementer`'s file |
| A run of the upstream suite against the Entity Framework Data Access Layer | **Here**, once the seam exists | **[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)** — `Scheduled`; **the seam landed 2026-08-16 and is unverified** |
| The seam itself | **`ProphetsWay.Example`** — never edited from this side | **Landed 2026-08-16, unverified** — see the note below |

### The seam landed upstream — 2026-08-16, and nothing has been run through it

**Verified by opening `ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs` in the
submodule working tree**, not inherited. The row above previously read *"in flight"*; it is no longer.

The seam is **`public static void Use(Func<IExampleDataAccess> implementation)`** on
`TestDataAccessFactory` — an assignment, not a hook. Four properties of it bear directly on this entry:

- **It is a single call, not a per-class override**, which is the constraint that emerged during its design
  and which this entry recorded: *"this repository does not get its adapters back."* It does not. One file in
  `ProphetsWay.EFTools.Tests` calling `Use` once points all 164 upstream tests at the Entity Framework Data
  Access Layer.
- **It takes a `Func<>`, not an instance**, so a connection string, provider or context factory can be closed
  over on this side — which matters, because `ExampleDataAccess` here does not have a parameterless
  constructor.
- **It is guarded against a late write.** `Create()` sets a flag under a lock; `Use` throws
  `InvalidOperationException` if it arrives after the first instance was handed out. The documented mechanism
  is a `[ModuleInitializer]` in the consuming test assembly — which is **this** assembly.
- **The default line stays visible and unconditional**, so
  [ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md#8--selecting-the-implementation-from-configuration-instead-of-a-code-edit)'s
  rejection is untouched. This is not configuration-driven selection.

**This entry does not move, and must not be advanced on the strength of it.** Its remaining deliverables are
unchanged: the dead `#if` branches in `Constants.cs`, and **a run** of the upstream suite against this
repository's Data Access Layer. Nothing has been run through the seam in either repository, and the Entity
Framework Data Access Layer still answers 11 members with `NotImplementedException`
([entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 2). **A seam that exists
is not a suite that passes.**

**What it does change is the risk profile of [D13](purpose-and-scope.md#owner-decisions--2026-08-15).** The
objection to hand-writing a `DepartmentDao` was that it meant writing code with no suite to verify it; the
seam is what answers that — on paper. The first green run is what answers it in fact.

What the rebuilt suite gains, and why it is worth having rather than merely unavoidable:

- **`dotnet test --filter "Scope=Contract"` becomes the conformance gate** — the subset any implementation
  must pass, separated from `Characterization` (choices the NoDB implementation made) and `Dispatcher`
  (reflection-convention tests belonging to `ProphetsWay.BaseDataAccess`, which no DAL is bound by).
- It is the closest thing that currently exists to
  [BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md)'s conformance kit, whose
  own revisit trigger is *"after `ProphetsWay.EFTools` has been built and updated onto 3.x."* Doing this
  well is the input that entry needs.

[Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs) also carries `#if` branches for `NET45`
through `NETCOREAPP3_1` and `NET5_0` — targets this project has not had in some time. Dead conditions, to
be removed. **Still present as of 2026-08-16**, and now more clearly dead than before: under the single
`net10.0` target only the `NET6_0_OR_GREATER` arm compiles, so the `Data Source=localhost` connection
string the other two arms reach is unreachable.

**Open question — answered.** Does the EF suite point at a real SQL Server (keeping
`LocalTestsOnly: 'yes'`), or at a provider CI can run? **Both**, per
[D4](purpose-and-scope.md#owner-decisions--2026-08-15) — SQLite in-memory for the CI gate and a SQL Server
container for provider fidelity. The `InMemory` provider is out either way: it **cannot honour
transactions**, so the transaction-scope contract tests cannot pass against it, meaning the current
arrangement is structurally unable to verify part of the contract this package now claims. The work is
[entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).

---

## 7 — Stop forcing a database provider on every consumer

**Status:** **Scheduled for v3.0.0** — 2026-08-15. **Approved by the owner as
[D2](purpose-and-scope.md#owner-decisions--2026-08-15).** Previously `Proposed`. The clearest scope
violation in the package, and the one nobody had reported.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) declares, in an `ItemGroup`
whose `net4`-excluding condition is now unconditionally true given the single `net10.0` target (versions
omitted — mid-flight, see [entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)):

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="..." />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="..." />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="..." />
```

**All three are still present as of 2026-08-16** — this entry is untouched by that day's changes — and [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) hardcodes the provider:

```csharp
protected BaseEFContext(string connectionString)
    : this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options) { }
```

### Why this is a scope violation rather than a default

The purpose sentence says this library supplies *plumbing*. Choosing the database is not plumbing; it is
the single most consumer-specific decision in a DAL. A consumer on PostgreSQL, SQLite, MySQL or Cosmos
installs `ProphetsWay.EFTools` and receives the **SQL Server** provider and the **InMemory** provider in
their dependency graph regardless, and cannot use the string constructor at all — the one the README
teaches first.

The `InMemory` reference is the sharper half. It is a **testing** provider that Microsoft documents as
unsuitable for production, and it is in the *runtime* package because the *examples* use it. A dependency
of the proving ground has become a dependency of the product. That inversion is exactly what the
equal-weighting of the two audiences in
[purpose-and-scope.md § Audience](purpose-and-scope.md#audience) is meant to prevent.

There is also a live consequence for [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits):
`InMemory` **cannot honour transactions**, so a suite built on it cannot verify the transaction contract
this package must now meet. The provider decision and the test-verification decision are the same decision.

### The shape of the fix

- Drop both provider references from the library. `Microsoft.EntityFrameworkCore` alone is the correct
  dependency for a package that never names a provider.
- Keep the `DbContextOptions` constructor as the primary path — the consumer supplies the provider, which
  is how every EF Core library does it.
- Either delete the string constructor, or keep it and require the derived context to configure the
  provider in `OnConfiguring`. **Deleting it is the honest option**; a "connection string" with no provider
  is not a meaningful parameter.
- Move `InMemory` to the test and example projects, where it is a legitimate test dependency.

**Breaking**, by design, and cheap to explain: consumers add one `PackageReference` for the provider they
already knew they were using.

### Counter-argument — **weighed and declined**

The string constructor is the first thing the README teaches and the shortest path from zero to a working
DAL, and SQL Server is very likely what every current consumer uses. Removing it makes the getting-started
experience longer for the majority in order to make it *possible* for a minority. The alternative offered
here was to **name** the SQL Server path as such — `UseSqlServerConnectionString`, or a separate
`ProphetsWay.EFTools.SqlServer` package.

**The owner declined both.** D2 makes provider neutrality part of the purpose sentence, and a named
provider reintroduced behind a friendlier name is the same coupling with better manners. Recorded, not
deleted: if onboarding friction turns out to be a real complaint from real consumers, this is the paragraph
to reopen.

### What D2 settled, and what it left to the implementer

**Settled — the scope of the promise is three tiers, not two:**

| Tier | Providers | Meaning |
|---|---|---|
| **Certified** | SQLite, SQL Server | This repository's tests run against them; a green suite is evidence for these two only |
| **In scope, uncertified** | PostgreSQL, MySQL/MariaDB, Oracle | Relational; nothing in the design excludes them, and a defect report against one is in scope |
| **Out of scope** | Cosmos, other non-relational | The DAO surface assumes `Skip`/`Take` over a stable order, server-side `GetCount`, and the parent's transaction semantics |

**Left open — an implementation choice, not a purpose question:** whether
`BaseEFContext(string connectionString)` is **deleted** or **retained** with the provider configured by the
derived context's `OnConfiguring`. Either satisfies neutrality. What it may **not** do is name a provider.

**Q4 — closed.** The certified/uncertified split is stated **on the package**, not only in these docs —
owner decision [D8](purpose-and-scope.md#owner-decisions--2026-08-15). Public wording says EFTools is
designed for **relational EF Core providers** and **certified and tested by this repository on SQLite and
SQL Server**, and must **not imply any other relational provider is certified**. The exact constraint is in
[purpose-and-scope.md § Public Wording](purpose-and-scope.md#public-wording--settled); the files that carry
it belong to `Modernizer` and `README Author`.

---

## 8 — Remove `FluentAssertions` from `ProphetsWay.Example.DataAccess.EF`

**Status:** **Done — 2026-08-16.** Previously `Scheduled for v3.0.0` (2026-08-15, owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15)). Trivial, isolated, and it did not wait — which is
exactly what the sequencing analysis below predicted.

**Re-verified before the status moved**, not affirmed:
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
was opened on 2026-08-16 and contains no `FluentAssertions` line. Its `PackageReference` entries are
`Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer` and
`ProphetsWay.BaseDataAccess` 3.1.0, and nothing else. **The licence exposure is closed.**

**Why `Done` and not held to the release.** The entry's own reasoning below establishes that there was nothing
to *ship* — the project is not packaged and reaches no consumer — so the only meaningful state was "is the
reference in the file." It is not. Holding it open until v3.0.0 would keep an entry alive to track a file that
no longer contains the thing it tracks.

[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
carried `<PackageReference Include="FluentAssertions" Version="8.2.0" />`. Two independent problems:

1. **It is a test-assertion library in a non-test project.** That project is a DAL implementation. Nothing
   in it should assert anything.
2. **FluentAssertions 8.x requires a paid commercial licence.** House convention names **Shouldly** as the
   assertion library and states plainly that FluentAssertions must not be added to any project.

The project is not packaged, so no consumer is affected and this is not breaking. It should still go: a
proving ground that models bad dependency hygiene teaches bad dependency hygiene.

**Check before deleting:** confirm nothing in `Daos/`, `ExampleContext.cs` or `ExampleDataAccess.cs`
actually uses it. If something does, that code is the real finding.

**Checked, 2026-08-16 — nothing uses it.** A search for `FluentAssertions` and `Should()` across all seven
`.cs` files in the project (`ExampleDataAccess.cs`, `ExampleContext.cs`, and the five under `Daos/`)
returns **no match**. Every other hit is the `PackageReference` itself or a `bin`/`obj` build artefact.
There is no "real finding" behind it; it is an unused reference and nothing more.

### Why it stayed Scheduled at the time — 2026-08-16, superseded later the same day

Retained because the sequencing argument is the durable part and it turned out to be right. The heading
previously read "Why it stays Scheduled rather than moving"; the entry is now `Done`, and a heading that
contradicts its own status line is the defect this pass exists to remove.

The question put was whether the paid-licence exposure on an unused reference should move this out of
v3.0.0 and into something sooner. **It should not have moved status *then*, but its sequencing claim needed
correcting.**

- **Status stayed `Scheduled` for v3.0.0.** Nothing about it had changed: the work is a one-line `.csproj`
  deletion owned by `Modernizer`, and v3.0.0 is the release it lands in. Moving it to a separate release
  would have meant cutting a 2.2.x patch to remove a reference from a project that is **not packaged and not
  published**, which reaches no consumer at all. There was nothing to ship.
- **Its "should not wait for anything" is currently false, and that is worth saying.** It *is* waiting —
  not on priority but on the absence of any green baseline, since the repository does not compile while
  [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) is mid-flight.
- **But the wait is not required by this entry's own risk.** The reference is statically verified unused,
  so its removal cannot break a compilation that entries 1–3 have not already broken. It is therefore
  **eligible to land ahead of the rest of v3.0.0**, as an isolated commit, the moment anyone is editing
  that csproj — which entries 2 and 5 both require anyway.
- **Frame it as a licence item, not hygiene.** The distinction changes who cares: hygiene waits for a
  convenient release, a licence obligation does not. The honest scope of the exposure is that the package
  is **restored and resolved** rather than *used* — `obj/project.assets.json` resolves 8.2.0 across four
  target legs — and that whether a licence is owed at all turns on whether the owner's use is commercial,
  which is not this agent's to determine. The house convention forbids it regardless of that answer, and
  that is sufficient reason on its own.

**Not this agent's edit.** `Modernizer` owns csproj changes.

### Landed — 2026-08-16

**The reference is gone.** Verified by opening
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
on that date: it contains no `FluentAssertions` line, and its only `PackageReference` entries are the two EF
Core packages and `ProphetsWay.BaseDataAccess`. **The substance of this entry is satisfied and the licence
exposure is closed.**

**The status line has moved, and this is the one place in this file where that most needs saying.** The
entry above reasons at length about *when* this should land relative to v3.0.0 — and it landed early, exactly
as "eligible to land ahead of the rest" anticipated. **Triaged `Done` on 2026-08-16.**

---

## 9 — Delete the stray `[submodule "Submod"]` block from `.gitmodules`

**Status:** **Scheduled for v3.0.0** — 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Trivial. Discovered from the other side, in
[ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md), and recorded here because
the file is here.

[.gitmodules](../.gitmodules) contains, after the legitimate `ProphetsWay.Example` block:

```ini
[submodule "Submod"]
	branch = main
```

No `path`, no `url`. Git tolerates it today because nothing resolves it, but it is a malformed submodule
declaration that will confuse `git submodule` operations — and, more immediately, anyone auditing this
repository's submodule story, which is already carrying a stale "vendored copy" claim in `AGENTS.md`.

Delete the two lines. No build impact, no consumer impact.

---

## 10 — Collapse the `Guid`/`Int`/`Long` DAO triplication

**Status:** **Scheduled for v3.0.0** — 2026-08-15. **Approved by the owner as
[D3](purpose-and-scope.md#owner-decisions--2026-08-15), reversing this entry's recommendation.**
Previously `Proposed — and recommended against`.

**This is the one entry in the file where the agent recommendation was overturned, and the reversal was
correct.** One of the three arguments below was not a judgement call — it was a factual claim about the
source, and it was **wrong**. It is corrected in place rather than deleted, because a decision index that
quietly edits out its own errors cannot be trusted about anything else.

### The approved shape

The 18 key-typed classes are replaced by **six generic families in the root namespace** —
`BaseDao<TEntity, TKey>`, `BaseGetAllDao<TEntity, TKey>`, `BasePagedDao<TEntity, TKey>` and their three
soft-delete counterparts. The public breaking change is **accepted** and lands in the same major.
**No compatibility wrappers** — unless implementation evidence forces reconsideration, which the
[preservation requirements](#what-the-collapse-must-preserve) below define precisely.

### The observation

`ProphetsWay.EFTools` contains `Guid/`, `Int/` and `Long/` folders, each with the same six types —
`BaseDao`, `BaseGetAllDao`, `BasePagedDao`, `BaseSoftDao`, `BaseSoftGetAllDao`, `BaseSoftPagedDao` — 18
near-identical classes. Each is a thin closure over `RootBaseDao<T, TIdType>` or
`RootBaseSoftDao<T, TIdType>`. [Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs) is representative:
a constructor pass-through and one `Get` override. A change to one "nearly always must be made to all
three," per `AGENTS.md`.

That is real duplication, and an agent seeing it for the first time will propose collapsing it to a single
generic `BaseDao<T, TIdType>`. It has surfaced before, and this time it was approved.

### Why this entry argued against it — and which leg broke

Preserved in full. The middle one is the one that failed.

1. **"The duplication is the point."** These types exist so a consumer writes
   `using ProphetsWay.EFTools.Int;` and then `BaseDao<User>` with **one** type parameter. A collapsed
   `BaseDao<User, int>` forces the key type into every DAO declaration in every consumer's codebase, to
   save 18 files in one library. The cost lands on every consumer; the saving lands on one maintainer.
   — **Still true. The owner accepted the cost**, and it is ergonomic rather than a loss of capability.
2. ~~**"The `Get` override is not boilerplate."** It is the reason the split exists… `x => i.Id == item.Id`
   needs a comparison the C# generic constraint system cannot express over an open `TIdType` without
   `IEquatable`/`IComparable` gymnastics or `EqualityComparer<T>.Default` indirection that EF Core cannot
   translate to SQL.~~
   — **FALSE, and checkable in this repository.**
   [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already performs exactly this comparison generically on
   the EF Core branch: `Dataset.AsTracking().Single(x => x.Id.Equals(item.Id))` in `Update`, and
   `Dataset.OrderBy(x => x.Id)` in `GetPaged` — both over an open `TIdType`, both translated by EF Core
   today. [Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs) uses `i.Id == item.Id` because `int`
   permits the operator, **not** because the generic form is impossible. The claim was inherited from the
   README's "required so the default `Get` method can build a proper select by Id" and restated here
   without opening `RootDao.cs`. This was the load-bearing leg; with it gone the recommendation does not
   stand.
3. **"Binary-breaking for every consumer, for a change with no behavioural benefit."**
   — **Still true, and now free.** The same major already breaks on entries 2, 4, 5 and 7. A breaking
   change that must ride a major is cheapest when a major is already leaving.
4. **"The volume is overstated."** Eighteen files averaging roughly ten lines.
   — **Still true, and now beside the point.** The saving was never the argument for doing it; the argument
   is that a key type can now be added without adding a namespace, and that argument only became available
   once leg 2 fell.

### What the collapse must preserve

These are the conditions under which D3's "no compatibility wrappers" holds. If one of them cannot be met,
that is the implementation evidence that reopens the question — not a preference.

**Added 2026-08-16 — the collapse is not a re-wrap, and this is the most load-bearing constraint on it.**
`RootBaseSoftDao` violates four of `IDepartmentDao`'s 19 rules and `BaseNonIdDao<T>` structurally cannot
serve `ICompanyResourceDao`. The six generic families this entry creates must therefore **fix `Update` and
`Delete` semantics**, not close over the existing bases with a new type parameter list. The evidence is
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance); read it
before designing the families.

**Constrained further by owner decision [D13](purpose-and-scope.md#owner-decisions--2026-08-15) —
2026-08-16.** The soft-delete families are to be **derived from a hand-written concrete Entity Framework
`DepartmentDao`, proven against `IDepartmentDao`'s 19 rules, rather than designed ahead of one.** The owner's
reasoning: *"the point of the generic tests and classes was to just reduce all the duplicative code, but in
this case there is a real need for it."*

**This does not reopen [D3](purpose-and-scope.md#owner-decisions--2026-08-15) and does not change this entry's
status or shape.** Six generic families, no compatibility wrappers, in the same major — all unchanged. What
D13 settles is **order**: a concrete implementation first, generalized second.

Three consequences worth having written down:

- **Deduplication is the benefit of the collapse, not the evidence for it.** That distinction is the whole of
  D13. Entry 13 is the demonstration that a family designed from the existing bases ships four rule violations
  under new type names; a family generalized from a Data Access Object that already passes
  `DepartmentDaoTests` cannot.
- **The generalization is then a refactor with a green suite behind it**, which is a materially different
  activity from designing against a specification nobody has satisfied once.
- **It does not license a second permanent surface.** The concrete `DepartmentDao` lives in
  `ProphetsWay.Example.DataAccess.EF`, which is **not packaged** and never becomes part of the library. If it
  survives as anything, it survives as the proving ground's implementation — not as a shipped shim, which
  [D3](purpose-and-scope.md#owner-decisions--2026-08-15) rejects.

**The counter-argument, recorded because it was made:** writing the semantics concretely first means writing
them twice, and the surviving copy is the generic family. The answer is the second bullet above — but a
reader should see the cost rather than only the conclusion.

- **The `Get` predicate must remain translatable to SQL.** Use the `x.Id.Equals(item.Id)` form already
  proven in `RootDao.Update`, **not** `EqualityComparer<TKey>.Default`, which EF Core cannot translate.
  This is the single place where the collapse could genuinely fail.
- **Decide the fate of `where TIdType : struct`.** `RootDao<T, TIdType>` carries it today, which excludes
  `string` keys. Keeping it makes the collapse a pure refactor of the surface; relaxing it is a **reach**
  decision that would also close
  [Example FR 1](../../ProphetsWay.Example/docs/feature-requests.md)'s accepting-half gap. Carried as **Q3**
  in [purpose-and-scope.md](purpose-and-scope.md#unresolved-purpose-level-questions); D3 does not settle it.
- **Verify against the certified providers, not just SQLite.** A predicate that SQLite translates and SQL
  Server does not would be found by [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
  and by nothing else.

### The strongest argument against, recorded so it is weighed rather than forgotten

Six generic families in one namespace is a **less discoverable** surface than three namespaces of six. A
consumer who types `using ProphetsWay.EFTools.Int;` gets an IntelliSense list containing exactly the six
types that can apply to them. Afterwards they get one list of six and must supply the right `TKey`
themselves — the compiler catches a mistake, but later and less kindly than a namespace that could not
express it.

### What would reopen the "no wrappers" half of D3

- The `Get` predicate proving untranslatable on a **certified** provider. That is the escape hatch D3 names.
- A real consumer migration turning out to be larger than "add one type argument per DAO declaration."

**Fold this into the v3.0.0 modernization.** The earlier instruction here was the opposite — "do not fold
this in; it looks adjacent to the retarget and is not." That advice followed from the recommendation the
owner overturned. Riding the same major is now the cheapest possible way to land it.

### The "18 = 3 key types × 6 shapes" hypothesis — **counted 2026-08-19, and it survives as an inventory only**

Recorded here because it is the fact the `Interface Architect` shape pass is conditioned on, and because it
had been carried as *likely but unverified*. **It was verified by opening the files**, not by reading this
entry: a directory listing of `ProphetsWay.EFTools/`, a repository-wide grep for `public abstract class`
returning **24** declarations in 24 files, and the bodies of `Int/BaseDao.cs`, `Int/BaseGetAllDao.cs`,
`Int/BasePagedDao.cs`, `Int/BaseSoftDao.cs`, `RootBaseDao.cs`, `RootBaseSoftDao.cs` and `BaseNonIdDao.cs`.

**The count is exactly right.** `Guid/`, `Int/` and `Long/` each hold the same six file names, and the class
declarations differ only in the key type substituted into the base and the constraint. 3 × 6 = 18, and the
whole public surface is those 18 plus `BaseNonIdDao`, `BaseSoftNonIdDao`, `RootBaseDao`, `RootBaseSoftDao`,
`BaseEFContext` and `BaseEFDataAccess` — **24, which is the number `docs/api-contract.md` says it reduces
from.** That cross-check is now measured rather than asserted on both sides.

**What the hypothesis gets wrong is the word "shapes."** It invites the reading that six behaviours are being
generalized. They are not:

| Of the 18 | How many | What is in the body |
|---|---|---|
| `BaseGetAllDao`, `BasePagedDao`, `BaseSoftGetAllDao`, `BaseSoftPagedDao` | **12** | **Nothing.** A constructor pass-through and one added interface declaration. Zero members |
| `BaseDao`, `BaseSoftDao` | **6** | **One member each** — `public override T Get(T item)`, and all six bodies are the same expression, `Dataset.Where(i => i.Id == item.Id).SingleOrDefault()`, modulo the key type |

**So the total behaviour distributed across 18 classes is one method.** Everything else — CRUD, the retrieval
trio, the transaction helpers — lives in `RootBaseDao`, which already implements **all three** capability
interfaces on a single type; and the soft-delete semantics live entirely in `RootBaseSoftDao`, which is where
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)'s
violations are. The six names are **interface-declaration variants**, not capability variants, which is
exactly what `docs/api-contract.md` S2 calls the flat method surface and what it preserves.

**Three consequences for the shape pass, and they all point the same way — start.**

1. **The collapse is smaller and safer than the entry implies.** It deletes 12 empty classes, hoists one
   expression into `KeyEquals` / `MatchRow`, and keeps six names that api-contract **S1** already fixes. There
   is no behaviour to reconcile across the 18, because there is none in them.
2. **The one genuine risk is the one this entry already named** — the `Get` predicate's translatability. It is
   the *only* thing in the 18 that the collapse must reproduce, which is why it is the escape hatch
   [D3](purpose-and-scope.md#owner-decisions--2026-08-15) names.
3. **A gap in the family set exists and is already covered — do not "fix" it.** There is **no
   `BaseSoftGetAllPagedDao`**, and `IDepartmentDao` declares **both** `IBaseGetAllDao<Department>` **and**
   `IBasePagedDao<Department>` — verified by opening
   `ProphetsWay.Example.DataAccess/IDaos/IDepartmentDao.cs`, line 230. It compiles anyway, and only because of
   the flat surface: `BaseSoftPagedDao<Department, int>` carries a working public `GetAll`, which satisfies
   the second interface implicitly. **A seventh family must not be added to close the gap.** What is owed
   instead is one sentence of guidance — which base a DAO whose interface declares two capabilities should
   pick — because four of the six families are empty markers and the choice between them is otherwise
   arbitrary. That is a documentation obligation on the shape pass, not a design change.

---

## 11 — Certify the contract suite on SQLite in-memory and a SQL Server container

**Status:** **Scheduled for v3.0.0** for the test work — 2026-08-15, approved by the owner as
[D4](purpose-and-scope.md#owner-decisions--2026-08-15) and reinforced by
[D8](purpose-and-scope.md#owner-decisions--2026-08-15), which turns "certified on SQLite and SQL Server"
into a claim the **package** makes. **The pipeline half is `Deferred`** to whoever owns
`app-variables.yml` and the shared templates; the removal of blanket `LocalTestsOnly: 'yes'` is not this
repository's decision alone and is explicitly not this agent's edit.

**New entry, promoted from a footnote.** This was carried as an open question inside entries
[1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) and
[6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) and treated as downstream of
[7](#7--stop-forcing-a-database-provider-on-every-consumer). D4 answered it, which makes it approved work
with its own scope rather than a question hanging off three other entries. Numbered 11 because numbers here
are monotonic and permanent.

### The approved strategy

| Leg | Provider | Job |
|---|---|---|
| **Fast** | `Microsoft.EntityFrameworkCore.Sqlite`, in-memory mode | The CI contract and query gate. Runs on every build, no external dependency, honours transactions |
| **Fidelity** | SQL Server in a container | Proves the generated SQL against the provider most consumers actually use |

### Why the current arrangement cannot verify what 3.x claims

Three verified facts that together make this non-optional rather than a nice-to-have:

1. **`LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml) means CI runs no tests here at
   all.** "The pipeline is green" is currently evidence of compilation, not of behaviour.
2. **The `InMemory` provider cannot honour transactions.** The 3.x transaction-scope contract tests
   therefore **cannot pass against it** — not "have not been written," *cannot pass*. The provider this
   package currently depends on is structurally unable to verify the contract it is about to advertise.
3. **[D2](purpose-and-scope.md#owner-decisions--2026-08-15) certifies exactly two providers**, and
   certification is a claim about test evidence. **[D8](purpose-and-scope.md#owner-decisions--2026-08-15)
   puts that claim on the package**, so without this entry "certified on SQLite and SQL Server" would be
   an assertion of the same kind this document objects to elsewhere — printed on nuget.org.

### Why two legs rather than one

SQLite alone is fast and free but is a **different SQL dialect**, so a predicate it translates may not be
what SQL Server produces — which matters most for exactly the change
[entry 10](#10--collapse-the-guidintlong-dao-triplication) makes, where a generic `x.Id.Equals(item.Id)`
replaces per-key-type operators. SQL Server alone is faithful but too slow and too dependency-heavy to gate
every commit. Neither leg alone certifies what D2 promises.

### Scope boundary

**In scope for this entry:** the test projects' provider wiring, fixtures, and the `Scope` trait filters
they expose. **Out of scope:** the YAML. `Pipeline Engineer` owns `.yml`; the `LocalTestsOnly` change and
any container service definition belong to that agent and to the shared templates in `prophets-pipelines`.

### Q4 — closed: the certification is a public claim

The question was whether "certified on SQLite and SQL Server" is stated on the **package** — `<Description>`,
README, `PackageTags` — or only in these documents. **Answered by
[D8](purpose-and-scope.md#owner-decisions--2026-08-15): on the package.** Public wording says EFTools is
designed for **relational EF Core providers** and **certified and tested by this repository on SQLite and
SQL Server**, and must **not imply any other relational provider is certified**; the uncertified tier
(PostgreSQL, MySQL/MariaDB, Oracle) exists to set expectations and sets none if it is invisible to the
person installing from nuget.org.

**This raises the stakes on this entry rather than settling it.** The certification is now something the
package *says*, so the two legs below are what makes it true rather than an internal quality bar. Shipping
the wording without the suite would be the "asserted rather than demonstrated" failure these documents
object to elsewhere. The wording itself belongs to `Modernizer` and `README Author`; the evidence behind it
is this entry.

---

## 12 — `RootNonIdDao.EnsureBeginTransaction` silently no-ops against a pre-existing transaction

**Status:** **Scheduled for v3.0.0 as a release-note obligation only; the 2.2.x patch is Rejected** —
triaged 2026-08-16. Previously `Proposed — captured during a verification pass, not yet triaged by
Purpose Refiner`. **No work is being requested on the 2.2.x line;** this entry exists so a real defect is
named rather than disappearing into a redesign.

**The facts were re-verified on this date rather than affirmed** — [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)
was opened, and lines 44–62 still read `if (Context.Database.CurrentTransaction == null) { _transaction =
Context.Database.BeginTransaction(); }` with `_transaction?.Commit()` and `_transaction?.Rollback()` in the
matching members. The defect is real and unchanged.

**What the triage decided, and what it did not.** The recommendation this entry already carried — no patch,
name it in the 3.0.0 notes as `Fixed` — is **accepted**, and it is accepted by *applying an existing owner
decision rather than making a new one*: [D1](purpose-and-scope.md#owner-decisions--2026-08-15) settles that
the 2.2.x line remains installable and receives no new work, and this entry is the exception that decision
invites someone to test. It does not survive the test. A patch would reopen a line the owner has closed, in
order to reach consumers who can equally be reached by release notes that tell them what they are exposed
to. **The owner may reasonably overrule this**, since it is the one place where "no new work on 2.2.x" meets
a data-correctness bug rather than a missing feature — see [The open question](#the-open-question) below,
which is retained rather than answered away.

> **The owner has since ruled, 2026-08-16, and upheld this.**
> [D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles it: document, do not patch. The invitation
> above is **answered, not withdrawn** — read
> [The owner has now ruled](#the-owner-has-now-ruled-and-the-question-is-closed--2026-08-16) before treating
> this paragraph as an open door.

### The defect

[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) guards its transaction start on whether one is
already open:

```csharp
if (Context.Database.CurrentTransaction == null)
    _transaction = Context.Database.BeginTransaction();
```

When a transaction **is** already open, `_transaction` stays `null` — and the matching commit and rollback
both reach it through `?.`, so **both quietly do nothing**. The caller receives no exception and no return
value indicating anything was skipped. A write intended to be committed by that path is left to whatever the
outer transaction decides, and a rollback intended to reverse it does not run.

No existing test would catch it: the 35 upstream tests exercise no nested or pre-existing transaction case,
and nothing in this repository runs them — `LocalTestsOnly: 'yes'` skipped them in CI even before the six
adapters that reached them were deleted on 2026-08-16.

### Why there is nothing to schedule in code

**The heading here previously read "Why it is `Proposed` rather than `Scheduled`", which contradicted this
entry's own status line. Corrected 2026-08-16.** The entry has not been `Proposed` since it was triaged
earlier that day; it is `Scheduled` as a release-note obligation with the 2.2.x patch `Rejected`. Nothing
about the reasoning below changed — only the heading, which had survived the triage that made it false.

**3.x designs the defect out rather than fixing it.** Under `docs/api-contract.md`, Data Access Objects carry
no transaction members at all — transactions live on the DAL root, every misuse throws rather than returning
quietly, and the library never consults `CurrentTransaction` to decide whether to begin. The one surviving
escape, a *borrowed* context that already carries a foreign transaction, surfaces EF Core's own
`InvalidOperationException` from `RelationalConnection.BeginTransaction` rather than a silent skip, and the
contract carries a regression obligation for it.

So there is nothing to schedule for v3.0.0 — the code is deleted. What needs a decision is narrower:

### The open question — decided, and retained because the owner may overrule

**Decided 2026-08-16: no patch.** The heading is kept because the question is a real one and the owner is
entitled to reopen it; it is no longer *open* in the sense of blocking anything, and the status line records
the decision rather than a pending choice.

**Does the 2.2.x line get a patch?** Entry 4 says that line's continuing job is to be the EF6 answer and it
should receive no new work. That is a reasonable rule and this is a reasonable exception to test it against —
silent transaction loss is a data-correctness bug, not a missing feature. The alternative is to leave 2.2.x as
it stands and let the release notes for 3.0.0 say plainly that this class of failure is gone.

**Recommendation:** no patch. Name it in the 3.0.0 notes as `Fixed`, alongside the leaked `DbContext` in
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess). A consumer holding 2.2.x who reads that
learns what they are exposed to, which is the outcome a patch would buy without the cost of reopening a line
this repository has decided to stop developing.

#### The owner has now ruled, and the question is closed — 2026-08-16

**The recommendation above was put to the owner and accepted.** It is
[D12](purpose-and-scope.md#owner-decisions--2026-08-15), and it settles the same question for
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) and
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) at the
same time. The reasoning is the owner's, not this file's inference: *"I don't believe anyone is currently
using that library in any meaningful capacity, so it's document the bug and recommend to update to v3.0.0."*

**Three things this closure does, and one it does not.**

- **The invitation to overrule is answered, not withdrawn.** This entry filed itself as the exception D1
  invites someone to test; D12 is the owner testing it and upholding D1. The paragraphs above are kept exactly
  as they were so the argument that *lost* stays legible.
- **It supplies the missing premise.** The earlier reasoning turned on "release notes reach the same
  consumers a patch would." D12 makes the real premise explicit: **the exposure is judged near-zero because
  there is no meaningful consumer base.** That premise is re-testable and should be re-tested if one appears.
- **It attaches the caveat this entry never carried.**
  [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a 2.2.x consumer on
  `net48`, `net8.0` or `net9.0` **cannot take 3.0.0 at all.** "Upgrade to 3.0.0" is therefore not a remedy
  available to every consumer this defect reaches, and the release note must say so —
  [The `Changelog Author` obligation](#the-changelog-author-obligation--d12).
- **It does not change this entry's status.** `Scheduled` for the release-note obligation, 2.2.x patch
  `Rejected`, exactly as triaged earlier the same day. D12 confirms that triage rather than moving it.

**Facts re-verified on 2026-08-16 before this was written**, not carried forward:
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) was reopened. `EnsureBeginTransaction` still guards
on `Context.Database.CurrentTransaction == null`, and `EnsureTransactionCommit` / `EnsureTransactionRollback`
still reach `_transaction` through `?.` and then null it. The defect is unchanged.

### Provenance

Found by `Repo Analyst` reading the shipped source during a verification pass; confirmed independently by
`Contract Reviewer` against the same file while checking that the 3.x design could not reproduce it. Neither
agent inherited the claim — both opened `RootNonIdDao.cs`.

---

## 13 — The soft-delete and keyless DAO bases cannot serve the 3.x contracts by inheritance

**Status:** **Scheduled for v3.0.0** — filed and triaged 2026-08-16. Scheduled rather than `Proposed` because
it requires no scope decision: [entry 10](#10--collapse-the-guidintlong-dao-triplication) is already approved
and already rewrites these types, and this entry is the specification of *what the rewrite must change*. It
is filed separately from entry 10 because entry 10 is a **surface** change (18 classes → 6 generic families)
and this is a **semantic** one, and conflating them is precisely the mistake it exists to prevent.

**"Just inherit the existing soft base" is a trap.** That is the whole entry in one sentence. Anyone
implementing `IDepartmentDao` or `ICompanyResourceDao` in `ProphetsWay.Example.DataAccess.EF` — which is step
2 of [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) — will reach for
`Guid/Int/Long.BaseSoftDao` and `BaseNonIdDao<T>` because they are the types that exist and the names match.
They do not satisfy the contracts, and the ways they fail are quiet.

### How this was found

By checking `RootBaseSoftDao` against `IDepartmentDao`'s 19 numbered rules one at a time, during the lap that
produced the 2026-08-16 green build. **Nothing surfaced it before because nothing runs it** — there is no
test suite here (entry 4 of the deviation list; [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits)
in this file), `LocalTestsOnly: 'yes'` skips CI, and the `Department` members in
`ProphetsWay.Example.DataAccess.EF` currently throw before reaching any base class. It is a reading finding,
which is the only kind available in this repository right now.

Verified by opening [RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs),
[RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs), [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
[BaseNonIdDao.cs](../ProphetsWay.EFTools/BaseNonIdDao.cs) and
`ProphetsWay.Example/ProphetsWay.Example.DataAccess/IDaos/IDepartmentDao.cs` and `ICompanyResourceDao.cs`.

### `RootBaseSoftDao` — four rule violations

`RootBaseSoftDao<T, TIdType>` is 40 lines. Three of them are the defect.

| Rule | What it requires | What the code does |
|---|---|---|
| **3** | `Update` writes the department's own data only; the stored `CreatedDate` and `DeletedDate` are **preserved** and values carried on the incoming instance for them are **ignored** | `Update` stamps `UpdatedDate` and calls `base.Update(item)`, which reaches `RootDao.Update`: `entry.CurrentValues.SetValues(item)` — **whole-object replacement.** A caller passing an instance fetched before the delete silently wipes the stored `DeletedDate` |
| **5** | `Delete` returns `1` when a **live** department with that identifier is stored | `Delete` stamps `DeletedDate` and returns `base.Update(item)`, which returns `Context.SaveChanges()`. It never asks whether the row was live |
| **6** | `Delete` on an already-deleted department returns `0`, changes nothing, and leaves the existing `DeletedDate` **not refreshed** — soft delete is **idempotent** | The stamp is unconditional, so a second delete **overwrites the original `DeletedDate` with a later one** and returns `1`. Both halves of the rule fail |
| **1** | After `Insert`, `UpdatedDate` and `DeletedDate` are `null` **whatever the caller had assigned to them** | `Insert` stamps `CreatedDate` and calls `base.Insert(item)`. Neither field is cleared, so a caller who reuses an instance carries stale timestamps into a new row |

**Rule 3 is the one `IDepartmentDao` itself predicts.** Its `WHY` section carries a paragraph headed
*"Rule 3 is the one that gets broken"*, naming whole-object replacement as the obvious implementation and
soft delete failing "with nothing to point at." This library is that prediction, in shipped code, in the
package the contract's own reference implementation is supposed to be demonstrated against.

**Rules 5 and 6 are the more dangerous pair in practice.** A refreshed `DeletedDate` is not an exception and
not a wrong row count that anyone checks — it is a timestamp that quietly moves. Any audit, retention window
or "deleted before" query built on it is wrong, and nothing anywhere reports it.

### `BaseNonIdDao<T>` — a structural mismatch, not a bug

`BaseNonIdDao<T> : IBaseDao<T>` declares `public abstract T Get(T item)` and `public abstract int Update(T item)`.

`ICompanyResourceDao` **deliberately does not inherit `IBaseDao<T>`** and its `<remarks>` spend two headed
paragraphs — *"No `Update`"* and *"No `Get`"* — explaining why: a `CompanyResource` is nothing but its two
foreign keys, so there is nothing to update *to*, and `IBaseDao<T>.Get(T)` is defined in terms of an
identifier field the entity does not have. Rule 8 goes further and states that
`Get<CompanyResource>(object id)` on the dispatcher **can never be made to work.**

So inheriting `BaseNonIdDao<T>` forces an implementer to write two members the contract declines to declare,
and the only honest bodies for them are throws. **That is not a defect in `BaseNonIdDao<T>`** — it was written
for keyless entities that still want CRUD, which is a real shape. It is the wrong base for this contract, and
the name will suggest otherwise to everyone who meets it.

### What this constrains

- **[Entry 10](#10--collapse-the-guidintlong-dao-triplication).** The six generic families must fix these
  semantics. A collapse that closes over `RootBaseSoftDao` unchanged ships the four violations under new type
  names and spends the major version doing it. The soft-delete families need a **read-modify-write `Update`**
  that preserves the three timestamps, and a **`Delete` that checks liveness first** and returns `0` without
  stamping when the row is already deleted or absent.
- **The keyless family must not implement `IBaseDao<T>`.** `ICompanyResourceDao`'s shape — `Insert`, `Delete`,
  `GetAll`, and nothing else — needs a base that offers exactly that. `IBaseDao<T>` is described by the
  contract as "a menu, not a mandate"; the library currently offers only the mandate.
- **[Entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 2.** Do not implement
  the `Department` DAO by deriving from today's soft base. If step 2 is done before entry 10, the honest route
  is a hand-written DAO satisfying the 19 rules, which then becomes the evidence for what the generic family
  must look like.

  > **The owner approved exactly this on 2026-08-16 —
  > [D13](purpose-and-scope.md#owner-decisions--2026-08-15).** A concrete Entity Framework `DepartmentDao` is
  > to be hand-written in `ProphetsWay.Example.DataAccess.EF/Daos/`, explicitly rather than leaning on a
  > generic family first, in the owner's words because *"the point of the generic tests and classes was to
  > just reduce all the duplicative code, but in this case there is a real need for it."* **This paragraph is
  > no longer a recommendation; it is the approved route.** Verified 2026-08-16: that folder holds
  > `CompanyDao.cs`, `JobDao.cs`, `ResourceDao.cs`, `TransactionDao.cs` and `UserDao.cs` — **no
  > `DepartmentDao.cs`** — so the work has not started.
  >
  > **What makes it verifiable rather than speculative:** the shape B seam landed upstream the same day
  > (`TestDataAccessFactory.Use`), so `ProphetsWay.Example`'s 164 tests can be pointed at the Entity Framework
  > Data Access Layer, and `DepartmentDaoTests` — the largest class in that suite — is written directly
  > against these 19 rules. **The seam is unverified**; see
  > [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation).
- **[Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).** Rules 5 and
  6 are exactly the kind of defect a green suite catches and a reading pass nearly misses. The upstream
  `Contract` tests for `IDepartmentDao` already exist; they have simply never run against this library.

### Severity, stated honestly

**This is a shipped defect in the 2.2.0 package, not only a forward-compatibility gap** — the same shape as
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess)'s leaked `DbContext` and
[entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)'s
silent transaction skip. A 2.2.0 consumer using `BaseSoftDao` today has non-idempotent soft delete and an
`Update` that can resurrect a deleted row.

**It does not get a 2.2.x patch**, for the reason [entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)
gives and on the same owner decision ([D1](purpose-and-scope.md#owner-decisions--2026-08-15)): that line
receives no new work. **`Changelog Author` must record this as `Fixed` in the 3.0.0 notes, not `Changed`** —
a consumer reading "soft-delete DAO bases rewritten" learns nothing about the rows they have already stamped.

**Confirmed by owner decision, 2026-08-16 — this is now ruled rather than inferred.**
[D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles all three shipped 2.2.0 defects as
**documented, not patched**, on the owner's judgement that the package has no meaningful consumer base. Two
things follow that this entry did not previously carry:

- **The changelog obligation is stated once**, in
  [The `Changelog Author` obligation](#the-changelog-author-obligation--d12), and cited from here rather than
  duplicated. The wording above is retained because it names *what* must be said; the consolidated section
  names *all three* defects and the caveat below.
- **"Upgrade to 3.0.0" is not available to every affected consumer.**
  [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a 2.2.x consumer on
  `net48`, `net8.0` or `net9.0` cannot take it. For them the release note is the whole remedy. This is the
  defect of the three where that matters most — **the failure is silent and it corrupts stored data**, so a
  consumer who cannot upgrade still needs to know to stop calling `Update` on a soft-deleted row.

**Re-verified 2026-08-16 before this was written**, not carried forward:
[RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs) was reopened. `Update` still stamps
`UpdatedDate` and delegates to `base.Update(item)`; `Delete` still stamps `DeletedDate` unconditionally and
returns `base.Update(item)`; `Insert` still clears neither `UpdatedDate` nor `DeletedDate`. All four rule
violations are unchanged.

### The strongest argument against filing this separately

It could have been three sentences appended to [entry 10](#10--collapse-the-guidintlong-dao-triplication),
which already owns these types and is already approved. A fourteen-entry index is harder to read than a
thirteen-entry one, and every entry that is really a constraint on another entry dilutes the index's job.

The counter, and the reason it is filed: entry 10 is a **surface** change with a stated recommendation-reversal
history, and someone reading it for the D3 decision will not read a semantics appendix buried in it. This
finding also outlives entry 10 — it is a statement about the published package that `Changelog Author` needs,
and it binds entry 1 step 2 whether or not the collapse happens first. A constraint that binds three entries
and the changelog is not an appendix to one of them.

### Strengthened 2026-08-18 — no longer a reading finding, and the inheritance claim is now structural

Two things this entry says about itself have stopped being true, and one claim in its title got stronger.
**No status changed** — that is `Purpose Refiner`'s to change.

**1. "It is a reading finding, which is the only kind available in this repository right now" is superseded.**
That sentence was accurate when written. This repository now has a working test harness: the upstream
`ProphetsWay.Example` suite is pointed at the Entity Framework Data Access Layer through
`TestDataAccessFactory.Use`, and `dotnet test` reports **151 tests, 123 passed, 28 failed**. The
`IDepartmentDao` rules are executed rather than read.

**2. "The work has not started" is superseded.** The hand-written `DepartmentDao.cs` that
[D13](purpose-and-scope.md#owner-decisions--2026-08-15) approved **landed 2026-08-18** in
`ProphetsWay.Example.DataAccess.EF/Daos/`, together with the `ExampleContext` mapping and the eight
forwarders. All 33 `DepartmentDaoTests` and 11 of 12 `DepartmentDataAccessTests` pass; the remaining one
fails inside `UserDao`, for the reason [entry 14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule)
describes. **This entry's prediction held**: the DAO derives from no EFTools base, and the reasons are the
four violations above.

**3. The inheritance claim in the title is structural, not a matter of degree.** The entry argued
`RootBaseSoftDao` *does not* satisfy the contracts. An independent `Code Reviewer` pass on 2026-08-18
confirmed the violations member by member — **eight of nine**, adding rules 11, 12, 14 and 17 to the four
already listed, with `GetCount` the only clean member — and found the reason it cannot be repaired by a
derived class:

> **Every member on `RootBaseSoftDao` is declared `new`, not `virtual`.**

They cannot be overridden, only hidden again. A derived Data Access Object that re-hides them and is then
used through a `RootBaseSoftDao`-typed reference **silently gets the base behaviour back** — the four
violations return, through a reference type the consumer chose reasonably. So the title is exact rather than
rhetorical: these bases cannot serve the 3.x contracts by inheritance **at all**, and a rewrite is the only
route. That sharpens the constraint on [entry 10](#10--collapse-the-guidintlong-dao-triplication) — a collapse
that preserves the `new` declarations preserves the trap.

**What was verified, and by whom.** The `new`-not-`virtual` finding and the eight-of-nine count come from a
`Code Reviewer` pass that opened `RootBaseSoftDao.cs`, `RootBaseDao.cs`, `RootDao.cs`, `RootNonIdDao.cs` and
`Int/BaseSoftDao.cs`. The test numbers were measured. Neither is carried forward from an earlier revision of
this file.

## 14 — The Entity Framework DAO bases adopt the caller's instance, violating the SNAPSHOT rule

**Status:** **Proposed** — filed 2026-08-18, awaiting triage. Filed by an agent under the shared-capture
rule; only `Purpose Refiner` may change this status.

**Several tests in this repository are currently green *because* of this defect.** That is the entry in one
sentence, and it is why it is filed separately from [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)
rather than appended to it. Entry 13 is about the **soft-delete and keyless** bases. This is about the
**ordinary CRUD** ones — `RootNonIdDao`, `RootDao`, `RootBaseDao` and the `Int`/`Guid`/`Long` `BaseDao`,
`BaseGetAllDao` and `BasePagedDao` that chain to them — i.e. the path every Data Access Object in the
published package takes.

### How this was found

By implementing a *correct* Data Access Object next to the incorrect ones. `DepartmentDao` landed on
2026-08-18 satisfying the SNAPSHOT rule — it copies on `Insert` rather than adding the caller's instance —
and nine previously-passing-or-stubbed tests began failing with:

```
Cannot insert explicit value for identity column in table 'Departments'
```

**The correct implementation is what broke them.** Confirmed by an independent `Code Reviewer` pass which
opened [CompanyDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyDao.cs),
[JobDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/JobDao.cs),
[UserDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/UserDao.cs),
[Int/BasePagedDao.cs](../ProphetsWay.EFTools/Int/BasePagedDao.cs),
[Int/BaseGetAllDao.cs](../ProphetsWay.EFTools/Int/BaseGetAllDao.cs),
[Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs),
[RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs),
[RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) and
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs).

### The mechanism

`RootNonIdDao.Insert` is `Dataset.Add(item); Context.SaveChanges();`. `SaveChanges` transitions the entity
`Added` → `Unchanged` and **keeps it in the change tracker**. The caller's instance is now the Data Access
Layer's instance — the definition of adopting an argument, which the SNAPSHOT rule on `IExampleDataAccess`
forbids.

The adoption is then load-bearing. `UserDao.Insert` is `Dataset.Add(user)`, and EF Core's `Add` walks the
graph and paints every **untracked** reachable entity `Added` regardless of whether its key is set.
`SnapshotDeepCopyTests.InsertUserWithNavigation` inserts a `Company`, a `Job` and a `Department`, hangs all
three off a `User`, and inserts the user:

- `co` and `job` are **still tracked** from their own inserts, so the graph walk skips them and only the
  `Users` row is written. **They work because they were adopted.**
- `dept` was correctly detached by the new Data Access Object, so the walk paints it `Added` with a non-zero
  `Id` and Entity Framework emits an `INSERT` carrying an explicit value into an `IDENTITY` column.

### Three findings that widen it

**1. It is not only `Insert`.** `Int/BaseDao.Get` is
`Dataset.Where(i => i.Id == item.Id).SingleOrDefault()` — no `AsNoTracking()`, no projection. It returns the
store's own tracked instance.

**2. It escapes being a violation only because of a setting on one constructor out of three.**
`ExampleDataAccess(string)` sets `QueryTrackingBehavior.NoTracking` context-wide.
`ExampleDataAccess(DbContextOptions<ExampleContext>)` and `ExampleDataAccess(ExampleContext)` take whatever
the caller configured, and Entity Framework Core's default is `TrackAll`. **A consumer using either of the
two dependency-injection-friendly constructors gets a Data Access Layer whose `Get` hands out the store's
tracked instances, so their next `Update` flushes edits nobody submitted.** Nothing detects it: the test seam
only ever uses the connection-string constructor.

**3. `RootDao.Update` compounds it.** It is `Dataset.AsTracking().Single(...)` then
`entry.CurrentValues.SetValues(item)` — whole-object replacement, and `Single` rather than `SingleOrDefault`,
so a missing row throws `InvalidOperationException` where the ROW COUNT rule requires `0`. Entry 13 records
the same shape reached through `RootBaseSoftDao`; this is the non-soft path to it.

### A test that is green for a prohibited reason

`SnapshotDeepCopyTests.ShouldNotStoreEditsMadeToAUsersNavigationAfterInsertReturned` passes today **by luck
of ordering.** It edits `co.Name` after the insert, then asserts through a second Data Access Layer instance
with its own context — and nothing calls `SaveChanges()` on the writer's context again before the assertion,
so the dirty tracked `co` is simply never flushed. **Insert one more entity through the same Data Access
Layer between the edit and the assertion and it fails.**

This is the finding with the longest reach, because it is not a defect in this repository. It is an upstream
test that does not currently test what it is named for, and it belongs to `ProphetsWay.Example` — see
[ProphetsWay.Example/docs/feature-requests.md](../../ProphetsWay.Example/docs/feature-requests.md). A
`Test Auditor` pass over that class is the right next step, and it must happen **in that repository**.

### Why it matters beyond the failing tests

**This is a shipped defect in the 2.2.0 package**, the same shape as entries 3, 12 and 13, and it very likely
falls under [D12](purpose-and-scope.md#owner-decisions--2026-08-15) — documented, not patched — with a
`Changelog Author` obligation attached. **That is a triage judgement and is deliberately not asserted here.**

It also bears directly on this repository's stated purpose. `ProphetsWay.Example` exists to demonstrate that
the same tests pass against two Data Access Layer implementations. A green suite that is green because one
implementation quietly aliases the caller's objects is the paradigm's central claim being *reported* rather
than *proven* — and the in-memory implementation, which does not alias, is the one telling the truth.

### What it constrains

- **[Entry 10](#10--collapse-the-guidintlong-dao-triplication).** The six generic families must copy on
  `Insert` and project on `Get`. A collapse that preserves `Dataset.Add(item)` ships this under new names.
- **[Entry 7](#7--stop-forcing-a-database-provider-on-every-consumer).** Finding 2 is a second reason the
  constructor surface needs attention: the three constructors do not agree on tracking behaviour, and only
  the least dependency-injection-friendly one is safe.
- **[Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).** Nine of the
  currently failing tests are this defect. Certification cannot be claimed until they pass **for the right
  reason** — a fix that restores adoption would turn them green and prove nothing.

### What has deliberately not been done

No fix has been attempted. `DepartmentDao` was implemented correctly and the failures were left standing
rather than papered over by making the new Data Access Object adopt its argument like its neighbours. **That
option was available and was declined** — taking it would have turned nine tests green while spreading the
defect to a tenth Data Access Object, and it is recorded here so it is not proposed later as an obvious
simplification.

### Triaged 2026-08-19 — **Scheduled** for v3.0.0, and it is a **fourth** shipped defect

**Status: `Proposed` → `Scheduled` for v3.0.0.** No new scope decision was needed to move it, and that is the
finding rather than a formality: **every behaviour this entry asks for is already specified**, in
`docs/api-contract.md` revision 8, written before this entry was filed. What this entry contributes is the
**evidence** that the specification was necessary, the **provenance** of nine red tests, and a **fourth** name
on the [D12](purpose-and-scope.md#owner-decisions--2026-08-15) release-note obligation.

#### It needs no design — the fix is already written down

Re-verified by opening `docs/api-contract.md` on 2026-08-19, section by section, rather than taken from this
entry's own summary. Each of its three findings has a term against it:

| This entry's finding | Where it is already settled |
|---|---|
| **1 — `Insert` adopts the argument.** `Dataset.Add(item)` leaves the caller's instance tracked `Unchanged` | **A24 + OD-4**: the root is added and everything reachable is set `Unchanged` **explicitly**; *"`Dataset.Add(item)` is wrong, and so is `Attach`"*, and **the walk is specified by state, not by API**. **A26 + OD-7**: the argument and its whole reachable graph are detached **in a `finally`, on success and on failure** |
| **2 — `Get` returns the store's tracked instance, saved only by a setting on one of three constructors** | **`Get`'s `Tracking` row**: `AsNoTracking()`, **explicitly, regardless of the context's configured `QueryTrackingBehavior`**. The constructor asymmetry stops being load-bearing because the library no longer depends on the setting. **S8/A9/A10** replace the three constructors with one taking a configured context and a `ContextOwnership` |
| **3 — `RootDao.Update` uses `Single`, so an absent row throws where ROW COUNT requires `0`** | **`Update`'s `Returns` and `The forced change` rows**: `1` when the row exists, `0` when it does not, *"never greater than `1`"*, and 2.2.x's throw-or-upsert behaviour named as **not surviving**. **A22** fixes the mechanism as a tracked fetch plus `SetValues` |

**So the correct disposition is not "schedule a fix" but "confirm the fix is inside the collapse."** It is:
[D14](purpose-and-scope.md#owner-decisions--2026-08-15) requires
[entry 10](#10--collapse-the-guidintlong-dao-triplication) to carry the corrected `Insert` / `Update` / `Get`
semantics rather than to collapse first and patch after. **This entry is now the third specification of what
that collapse must fix**, alongside
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) — entry 13
covers the soft-delete and keyless bases, this one covers the ordinary CRUD path, and between them they cover
every base in the package.

#### In scope for 3.0.0? — **Yes, and it is not separable from it**

Judged against the [purpose sentence](purpose-and-scope.md#settled-one-sentence-purpose). The library's job is
to supply *"the CRUD … plumbing that is identical in every DAL."* A plumbing layer that aliases the caller's
objects has not supplied working plumbing; it has supplied plumbing that works only for callers who never look
at their arguments again. There is nothing to weigh here — the alternative to fixing it is shipping 3.0.0 with
the ordinary CRUD path violating a rule the release exists to satisfy.

**It is also not separable.** The collapse deletes the types that carry the defect. Scheduling this for a later
release would mean writing the six generic families **with** the adoption behaviour and then breaking them
again in 3.1.0 — the "doing the work twice" D14 declined, at a higher price because the second pass would be
breaking rather than internal.

#### Breaking? — **Yes, against the published 2.2.0 package, and in a way a consumer can be relying on**

**All three findings are behaviour changes a 2.2.0 consumer could have built on**, and that is worth stating
plainly rather than filed as an obvious win:

- A consumer who calls `Insert(entity)` and then edits `entity` **today gets those edits written** on the next
  `SaveChanges` through any Data Access Object on the layer. After the fix they do not. Code that relies on it
  — deliberately or not — silently stops persisting.
- A consumer who calls `Get`, edits the result, and calls `SaveChanges` through some other path **today gets
  the edit written without calling `Update`**. After the fix they do not.
- `Update` on an absent row **today throws**; after the fix it returns `0`. A `try`/`catch` around it stops
  firing.

The first two are the same shape as this entry's own headline — *"several tests are currently green because of
this defect"* — read from the consumer's side. A 2.2.0 consumer's *code* can be green for the same prohibited
reason. **`Changelog Author` must say so**; "corrected snapshot semantics" does not tell that consumer their
writes have stopped.

**Riding v3.0.0 is therefore correct and cheap**: the release is already breaking on entries 2, 4, 5, 7 and 10.

#### The 2.2.x patch is **Rejected**, on D12 and without carving an exception from it

**Same disposition as entries 3, 12 and 13, on the same owner decision.**
[D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles that the 2.2.x line receives no new work
because the consumer base is judged near-empty, and nothing about this defect distinguishes it from the three
D12 already covers. It is **silent** and it **corrupts stored data** — a stray edit reaching the store through
an aliased instance — which puts it in the same class as entries 12 and 13 rather than in a new one.

**But D12 named three defects and this is a fourth, so the obligation it created has to grow.** That is a
triage judgement, and this entry deliberately declined to assert it; it is asserted here, and it is flagged for
the owner rather than buried: **D12's premise is re-testable and its scope is not.** If the owner meant "these
three," a fourth row on the changelog obligation needs their word. If they meant "the shipped defects," it is
already covered. **This triage assumes the latter** and records the assumption where it can be found — see
[The `Changelog Author` obligation](#the-changelog-author-obligation--d12), which now carries four rows.

#### The one thing that is genuinely blocked, and it is not in this repository

**The upstream test that is green for a prohibited reason** —
`SnapshotDeepCopyTests.ShouldNotStoreEditsMadeToAUsersNavigationAfterInsertReturned` — is a
`ProphetsWay.Example` defect and **this triage cannot move it.** That repository's index is separate, its
numbering is unrelated, and files under `ProphetsWay.Example/` are never edited from this side.

**What is owed there:** an entry recording that the assertion passes by ordering luck rather than by the
mechanism it is named for, and a `Test Auditor` pass over that class. **It is not blocking 3.0.0** — the test
will start passing *for the right reason* once the collapse lands, and the risk it carries is the opposite
one: it would have gone on passing against an implementation that never fixed anything. Recorded here so the
obligation is not lost when this entry is read as closed.

#### What this entry must **not** be read as authorising

**A fix applied to `RootNonIdDao`, `RootDao` or the 18 key-typed classes in place.** D14 forecloses that route
explicitly. These types are deleted by entry 10; correcting them first is work thrown away, and worse, it
would turn the nine `IDENTITY_INSERT` failures green **before** the families exist — removing the only signal
currently distinguishing a correct implementation from an adopting one.

**The nine red tests are the acceptance criterion for entry 10, not a regression to be cleared first.**

