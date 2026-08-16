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
(**1–13** as of 2026-08-16; this preamble previously said 1–9 and was stale). Those are separate indexes.
This file follows their *format*; where an entry genuinely depends on one of theirs, it is cited by
repository and number.

The contracts themselves are **not** restated here. The binding rules live in the XML `<remarks>` on
`IBaseDataAccess` and `DataAccessConventionException` in `ProphetsWay.BaseDataAccess`, and on
`IExampleDataAccess` in `ProphetsWay.Example`. Those are the source of truth. This file links to them and
does not duplicate them, because duplicated rules drift.

The scope bar every entry below is judged against is in
[purpose-and-scope.md](purpose-and-scope.md#settled-one-sentence-purpose), and the owner decisions that set
the statuses below are recorded as **D1–D9** in
[purpose-and-scope.md § Owner Decisions](purpose-and-scope.md#owner-decisions--2026-08-15).

**Stage 1 is closed as of 2026-08-15.** The two questions that needed the owner — **Q1** (the TFM
exception) and **Q4** (whether the certification scope is stated publicly) — were answered as **D7** and
**D8**. **Q2** and **Q3** remain open and are answerable by whoever implements v3.0.0.

**One owner question has since reopened — 2026-08-16.** The sentence "no status in this file is waiting on
an owner decision" stood until the `ProphetsWay.Example` submodule pointer advanced. It no longer does:
[entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) was rescoped on that date
and now carries a genuine fork — a local suite here, or a seam upstream — that this agent may not choose.
**No status is blocked on it**; the entry stays `Scheduled` either way. Its *deliverable* is what waits.

## Index

| # | Item | Status |
| --- | --- | --- |
| 1 | [Advance the `ProphetsWay.Example` submodule onto the 3.x contracts](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) | **Scheduled** — v3.0.0; **step 1 landed 2026-08-16, steps 2–6 outstanding**; the repository does not currently compile as a result |
| 2 | [Move the `ProphetsWay.BaseDataAccess` reference from 2.5.0 to 3.1.0](#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310) | **Scheduled** — v3.0.0 |
| 3 | [Implement the 3.x disposal contract in `BaseEFDataAccess`](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | **Scheduled** — v3.0.0; carries open question **Q2** |
| 4 | [Make 3.x Entity Framework Core-only — retire EF6 and .NET Framework](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) | **Scheduled** — v3.0.0; **approved by D1** |
| 5 | [Retarget to the house TFM standard](#5--retarget-to-the-house-tfm-standard) | **Scheduled** — v3.0.0; unblocked by 4; destination settled by **D7** as **`net10.0` only** |
| 6 | [Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory and `Scope` traits](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) | **Scheduled** — v3.0.0; forced by 1; **rescoped 2026-08-16** — there is no adapter to rebuild, and the seam it needs is upstream |
| 7 | [Stop forcing a database provider on every consumer](#7--stop-forcing-a-database-provider-on-every-consumer) | **Scheduled** — v3.0.0; **approved by D2** |
| 8 | [Remove `FluentAssertions` from `ProphetsWay.Example.DataAccess.EF`](#8--remove-fluentassertions-from-prophetswayexampledataaccessef) | **Scheduled** — v3.0.0; trivial, and **eligible to land ahead of the rest** — 2026-08-16 |
| 9 | [Delete the stray `[submodule "Submod"]` block from `.gitmodules`](#9--delete-the-stray-submodule-submod-block-from-gitmodules) | **Scheduled** — v3.0.0; trivial |
| 10 | [Collapse the `Guid`/`Int`/`Long` DAO triplication](#10--collapse-the-guidintlong-dao-triplication) | **Scheduled** — v3.0.0; **approved by D3, reversing this file's recommendation** |
| 11 | [Certify the contract suite on SQLite in-memory and a SQL Server container](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | **Scheduled** — v3.0.0 for the test work; **pipeline half Deferred** to its owner; **D8** makes the certification a public claim |
| 12 | [`RootNonIdDao.EnsureBeginTransaction` silently no-ops against a pre-existing transaction](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | **Scheduled** — v3.0.0, as a **release-note obligation only**; the 2.2.x patch is **Rejected** — triaged 2026-08-16 |

Numbers are permanent. Entries are never renumbered and never removed —
[purpose-and-scope.md](purpose-and-scope.md) cites entries by number, and a rejected entry is decision
history rather than dead weight.

## Release Eligibility — the next release

The next release of this package is **v3.0.0**, a major. That is not a preference; it is forced. Entry 2
alone changes the transitive contract this package advertises, and entries 3, 4, 5 and 7 are each
independently breaking. `app-variables.yml` currently reads `Major: '2' / Minor: '2' / Patch: '0'` —
**an agent must never change it**; the bump is the owner's.

| # | Status | Eligible for v3.0.0? | Why |
| --- | --- | --- | --- |
| 1 | Scheduled | **Yes — and it is the gate; step 1 of it has landed** | Nothing else can be verified until the proving ground compiles against the 3.x contracts. **The pointer advanced on 2026-08-16 and the remaining steps did not, so the repository is currently non-compiling** — that is the gate held open, not a regression |
| 2 | Scheduled | **Yes — required** | Without it the package advertises a contract it does not reference. This is what makes the release a 3.x |
| 3 | Scheduled | **Yes — forced by 2** | The code does not compile against 3.1.0 without it |
| 4 | Scheduled | **Yes — approved (D1)** | Only a major may drop targets, and this is the only major on the horizon |
| 5 | Scheduled | **Yes — strictly after 4** | The `#if` conditions go with 4; the destination is **`net10.0` alone**, a ratified exception to the house standard — see **D7** |
| 6 | Scheduled | **Yes — forced by 1** | The upstream base class it derives from no longer exists in that shape. **Rescoped 2026-08-16:** the deliverable is a suite that constructs the Entity Framework Data Access Layer itself, not a rebuilt set of adapters |
| 7 | Scheduled | **Yes — approved (D2), and only in a major** | Removing a transitive package reference is breaking. Postponing costs a second major |
| 8 | Scheduled | **Yes — and it need not wait for the rest** | Trivial, isolated to a non-packaged project, no reason to wait. **It is a licence item rather than hygiene**, and the reference is statically verified unused, so removing it cannot break a build that the other entries have not already broken |
| 9 | Scheduled | **Yes** | Trivial, no build impact |
| 10 | Scheduled | **Yes — approved (D3)** | A breaking surface change is cheapest riding a major that is already breaking for four other reasons |
| 11 | Scheduled (test work) / Deferred (pipeline) | **Yes for the suite; the `LocalTestsOnly` removal is separately owned** | The contract cannot be *verified* without it; the CI plumbing is not this repository's decision alone |
| 12 | Scheduled (release note) / Rejected (2.2.x patch) | **Already, incidentally** | Nothing to schedule in code: the 3.x design removes the members that carry the defect. What **is** scheduled is the release-note obligation — the entry exists so the fix is *named* in the notes rather than shipping as an unannounced side effect. Triaged 2026-08-16 |

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

**Partially landed — 2026-08-16.** Step 1 of six is done and the entry stays `Scheduled` because the other
five are not. It is deliberately **not** `Done`: the pointer move on its own delivers none of this entry's
value and costs the repository its build. Re-verified this date by opening
`.git/modules/ProphetsWay.Example/HEAD`, all six files in `ProphetsWay.EFTools.Tests/`,
`ProphetsWay.EFTools.Tests.csproj`, `ProphetsWay.Example.DataAccess.EF.csproj` and
`ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`.

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

That last row is the one with teeth. `ProphetsWay.EFTools.Tests` supplies the implementation by
**overriding an abstract property** — verified 2026-08-16 by opening all six adapters
([EFBaseDataAccessTests.cs](../ProphetsWay.EFTools.Tests/EFBaseDataAccessTests.cs),
`EFCompanyDaoTests`, `EFJobDaoTests`, `EFResourceDaoTests`, `EFTransactionDaoTests`, `EFUserDaoTests`) and
[Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs); each still declares
`protected override … GetIExampleDataAccess => Constants.GetExampleDataAccess;`. Upstream, that hook was
replaced by a static factory. **Advancing the pointer broke this repository's test project structurally,
not just semantically** — every test class here has lost the member it overrides. That is entry 6, and it
is not optional.

### The build break the advance created — verified 2026-08-16

Recorded here rather than left to be rediscovered, because a future agent meeting a red build needs to know
it is this entry's mid-flight state and not a regression. Three independent breaks:

1. **TFM mismatch.** [ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)
   targets `net472;net48;net80;net90` and references `ProphetsWay.Example.Tests`, which the 3.1.0 pointer
   retargeted to `net48;net10.0`. Three of the four legs have no compatible asset. Resolved by
   [entry 5](#5--retarget-to-the-house-tfm-standard).
2. **The missing hook**, above. Resolved by [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits).
3. **`ExampleDataAccess` no longer satisfies `IExampleDataAccess`.** At 3.1.0 that interface aggregates
   `IDepartmentDao` and `ICompanyResourceDao` and inherits `IDisposable` — verified by opening
   `ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`, whose declaration reads
   `: IBaseDataAccess, ICompanyDao, IJobDao, IUserDao, ITransactionDao, IResourceDao, IDepartmentDao, ICompanyResourceDao`.
   `ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs` supplies none of the three. Resolved by steps 2
   and 3 below.

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
   `ExampleContext`, and the schema they need. **Now a build break rather than a gap:** `ExampleDataAccess`
   does not satisfy the interface it declares.
3. Implement `Dispose` and the three transaction members against the real `DbContext` — which is
   [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) in the library and its
   consequence here. **Also now a build break**, for the same reason.
4. Satisfy the **snapshot rule** — reads return deep snapshots, writes read their argument. EF Core's
   change tracker makes this the interesting one: the existing examples already set
   `QueryTrackingBehavior.NoTracking`, which is a start and not a proof.
5. Satisfy the **ordering rule** — an explicit `ORDER BY` on both `GetAll` and `GetPaged`.
   `RootDao.GetPaged` already orders by `Id`; **`RootDao.GetAll` does not** — it is
   `Dataset.ToList()`. [repo-profile.md](repo-profile.md) adds a third case this entry had missed:
   **soft-delete paging in `RootBaseSoftDao` is not explicitly ordered either.** All three are divergences
   the rule was written to catch, and all three are in the library, not the example.
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

**Status:** **Scheduled for v3.0.0** — 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Previously `Proposed`. A one-line edit with the
largest consequence in the file.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) and
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
both reference `ProphetsWay.BaseDataAccess` **2.5.0**. The published parent is **3.1.0**.

**This is not a version-hygiene item.** A library whose stated purpose is "implements the
`ProphetsWay.BaseDataAccess` contracts" and which references a superseded major is not implementing the
contracts it advertises. The README's paradigm claim is currently a statement about 2.x.

What 3.0.0 changed that lands directly on this package:

- `IBaseDataAccess` now extends `IDisposable`
  ([IBaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/IBaseDataAccess.cs) line 164)
  and `BaseDataAccess` declares `public abstract void Dispose();`
  ([BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs) line 112).
  [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) overrides the three transaction members
  and nothing else, so **it will not compile.** That is [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess).
- Exceptions from derived DAL methods now propagate **unwrapped** — no `TargetInvocationException`. Any
  consumer catching the wrapper breaks.
- The identifier property must be **public**; an explicit interface implementation now throws
  `DataAccessConventionException` before dispatch.

**Breaking for consumers**, transitively: a `ProphetsWay.EFTools` 3.0.0 consumer is resolving
`ProphetsWay.BaseDataAccess` 3.1.0 whether they asked for it or not, and their own DAL must now supply
`Dispose`. This is the single strongest reason the next release must be a major.

**Do not split this from entry 3.** Landing it alone leaves the repository non-compiling.

---

## 3 — Implement the 3.x disposal contract in `BaseEFDataAccess`

**Status:** **Scheduled for v3.0.0** — 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Previously `Proposed`. Mechanically forced by
[entry 2](#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310); the **design** is a genuine
open question and the reason this is its own entry rather than a line in that one. Scheduling the entry
does not answer the design question — see [the open questions](#open-questions-for-the-owner) below.

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

1. **Still open (Q2).** Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one?
   Owner decision [D2](purpose-and-scope.md#owner-decisions--2026-08-15) makes this *more* pressing, not
   less: now that the consumer configures the provider themselves through `DbContextOptions`, the distance
   between "you pass options" and "you pass the context" is one step, and a DI-hosted consumer will ask for
   that step.
2. **Still open.** Should `ObjectDisposedException` guarding be added to the three transaction members, or
   is the parent's dispatcher expected to guard? The parent holds no state and cannot; the guard has to be
   here.
### Appended 2026-08-15 — this is a shipped defect, not only a forward-compatibility gap

The framing above reads as *"3.1.0 will require a `Dispose` we do not have yet."* That understates it.
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) contains **no `Dispose`, no `IDisposable`,
and no finalizer** — its only members are `Context`, two constructors, and three transaction forwarders — and
both constructors build the context with `Activator.CreateInstance`. **Nothing ever disposes it.** That is a
live resource leak in the *published* 2.2.0 package, independent of any 3.x contract.

Two consequences:

- **`Changelog Author` must record this as `Fixed`, not `Changed`.** A reader of the 3.0.0 notes who sees only
  "implements the new disposal contract" will not learn that the version they are running leaks connections.
- The design question this entry calls "not mechanical" is **answered** — `docs/api-contract.md` settles it as
  the `ContextOwnership` enum (A9), a required constructor argument with no default so the branch can never be
  inferred, plus a sealed `Dispose` so a derived DAL cannot omit it. Open question Q2 above is superseded by
  that document; read it rather than re-deciding here.

The defect was found by `Repo Analyst` during a verification pass and confirmed independently by
`Contract Reviewer`; both opened the file. Recorded so it is not rediscovered as new.
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
- The `EntityFramework` 6.5.1 `PackageReference` and the `$(TargetFramework.StartsWith('net4'))` item group
  go. **`Modernizer`'s file.**
- Roughly 60 `#if NET461 || NET471 || NET48` / `#if NET8_0_OR_GREATER` blocks across 26 files collapse to
  unconditional EF Core code, which is what unblocks [entry 5](#5--retarget-to-the-house-tfm-standard).

### The consequence the approval created — `netstandard2.0` is unreachable, and that is now ratified

Not foreseen when this entry was written, and it is a **conflict with a family-wide convention**, so it is
recorded here rather than left to be rediscovered during the retarget. **It has since been settled** by
owner decision [D7](purpose-and-scope.md#owner-decisions--2026-08-15), which closed **Q1**.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) pins
`Microsoft.EntityFrameworkCore` **9.0.4**. EF Core has shipped no `netstandard2.0` asset since 3.1 — 5.0
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

Current, from the two csproj files:

| Project | TFMs today | House standard | **Approved destination (D7)** |
|---|---|---|---|
| `ProphetsWay.EFTools` | `net461;net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** |
| `ProphetsWay.EFTools.Tests` | `net472;net48;net80;net90` | `net48;net10.0` | **`net10.0`** — the `net48` leg has nothing to bind |
| `ProphetsWay.Example.DataAccess.EF` | `net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** |

Four separate problems, worth separating because they have different fixes:

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
`Microsoft.EntityFrameworkCore` 9.0.4 ships no `netstandard2.0` asset — EF Core has been runtime-targeted
since 5.0, and **EF Core 10 exposes only `net10.0`** — so an EF Core-only library cannot carry the family's
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
**Rescoped 2026-08-16 — see [The rescope](#the-rescope--2026-08-16) below.** The status does not change;
the deliverable does.

`ProphetsWay.EFTools.Tests` derives from the upstream test classes and supplies the implementation by
overriding `protected abstract T GetIExampleDataAccess { get; }` — six files, all following
[EFBaseDataAccessTests.cs](../ProphetsWay.EFTools.Tests/EFBaseDataAccessTests.cs). Upstream, that hook no
longer exists: `BaseUnitTests<T>` now calls `TestDataAccessFactory.CreateAs<T>()` and implements
`IDisposable`.

**Advancing the pointer therefore breaks every test class in this project at compile time.** The rebuild
is not cleanup; it is the cost of entry 1. **As of 2026-08-16 that is past tense** — the pointer has
advanced and the project no longer compiles.

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

**What the deliverable becomes.** One of two shapes, and the choice is the open question below:

| Shape | What it means here |
|---|---|
| **A — a local suite** | This repository writes its own `Scope`-traited tests against `IExampleDataAccess`, constructing `ProphetsWay.Example.DataAccess.EF.ExampleDataAccess` itself. No dependency on `ProphetsWay.Example.Tests` at all; the `ProjectReference` to it goes. Costs a permanent second copy of the assertions, which is exactly what the upstream suite exists to avoid |
| **B — an upstream seam** | `ProphetsWay.Example` grows a way for a consuming repository to supply the implementation, and this repository provides it. Preserves the "same tests, different implementation" property that is the entire argument. **Requires a change in the other repository**, which is why it is filed there as [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md) |

**Shape B is the one that keeps the paradigm claim true**, and A is the one that can be done unilaterally.
A is also the one that quietly destroys the demonstration: two copies of the suite that must be kept in
step is the duplication problem the submodule arrangement exists to prevent, and the moment they diverge
the sentence "the tests do not change to accommodate it" stops being checkable.

**Note the interaction with [ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md),
which is `Rejected`.** That entry declines reading the implementation choice from an environment variable
or `.runsettings`, on the grounds that one obvious line beats a lookup. **Shape B is not that proposal** —
it does not ask for configuration-driven selection, and the single obvious line can stay exactly where it
is as the default. It asks only that the line be *reachable* from a repository that cannot edit it. Do not
let FR 8's rejection be read as having already declined this; it was answering a different question.

**Open question for the owner:** A or B? Nothing else in entry 6 can be estimated until it is answered.

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
be removed with the rest.

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

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) declares, unconditionally
for every non-`net4x` target:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.4" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.4" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.4" />
```

and [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) hardcodes the provider:

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

**Status:** **Scheduled for v3.0.0** — 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Trivial, isolated, and should not wait for
anything. **Re-triaged 2026-08-16 — status unchanged, sequencing sharpened; see
[Why it stays Scheduled](#why-it-stays-scheduled-rather-than-moving--2026-08-16).**

[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
carries `<PackageReference Include="FluentAssertions" Version="8.2.0" />`. Two independent problems:

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

### Why it stays Scheduled rather than moving — 2026-08-16

The question put was whether the paid-licence exposure on an unused reference should move this out of
v3.0.0 and into something sooner. **It should not move status, but its sequencing claim needs correcting.**

- **Status stays `Scheduled` for v3.0.0.** Nothing about it has changed: the work is a one-line `.csproj`
  deletion owned by `Modernizer`, and v3.0.0 is the release it lands in. Moving it to a separate release
  would mean cutting a 2.2.x patch to remove a reference from a project that is **not packaged and not
  published**, which reaches no consumer at all. There is nothing to ship.
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

No existing test would catch it: the 35 inherited tests exercise no nested or pre-existing transaction case,
and `LocalTestsOnly: 'yes'` skips all of them in CI regardless.

### Why it is `Proposed` rather than `Scheduled`

**3.x designs the defect out rather than fixing it.** Under `docs/api-contract.md`, Data Access Objects carry
no transaction members at all — transactions live on the DAL root, every misuse throws rather than returning
quietly, and the library never consults `CurrentTransaction` to decide whether to begin. The one surviving
escape, a *borrowed* context that already carries a foreign transaction, surfaces EF Core's own
`InvalidOperationException` from `RelationalConnection.BeginTransaction` rather than a silent skip, and the
contract carries a regression obligation for it.

So there is nothing to schedule for v3.0.0 — the code is deleted. What needs a decision is narrower:

### The open question

**Does the 2.2.x line get a patch?** Entry 4 says that line's continuing job is to be the EF6 answer and it
should receive no new work. That is a reasonable rule and this is a reasonable exception to test it against —
silent transaction loss is a data-correctness bug, not a missing feature. The alternative is to leave 2.2.x as
it stands and let the release notes for 3.0.0 say plainly that this class of failure is gone.

**Recommendation:** no patch. Name it in the 3.0.0 notes as `Fixed`, alongside the leaked `DbContext` in
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess). A consumer holding 2.2.x who reads that
learns what they are exposed to, which is the outcome a patch would buy without the cost of reopening a line
this repository has decided to stop developing.

### Provenance

Found by `Repo Analyst` reading the shipped source during a verification pass; confirmed independently by
`Contract Reviewer` against the same file while checking that the 3.x design could not reproduce it. Neither
agent inherited the claim — both opened `RootNonIdDao.cs`.
