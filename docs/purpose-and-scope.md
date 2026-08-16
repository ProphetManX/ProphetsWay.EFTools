# Purpose & Scope — ProphetsWay.EFTools

This document settles **what this library is for** and **what does not belong in it**, so that the
modernization work queued behind it has a bar to be measured against. It is a purpose pass, not a
modernization plan. No `.cs`, `.csproj`, `.sln`, `.yml` or version file was changed to produce it.

Every factual claim below was verified by opening the artifact named. Where a claim is inherited from
`AGENTS.md`, the README, or a sibling repository's documents and turned out to be **stale**, it is
corrected here and flagged in [Stale Inherited Claims](#stale-inherited-claims) rather than repeated.

The companion decision index is [feature-requests.md](feature-requests.md). This document cites it by
number and does not duplicate its reasoning.

**Status: Stage 1 closed.** Every checkpoint question this document was written to pose has been answered
by the owner. Their answers are recorded verbatim in [Owner Decisions](#owner-decisions--2026-08-15) and
applied throughout. **Q1 and Q4 — the two that needed the owner because they change what the family and
the package *claim* — are now closed by [D7](#owner-decisions--2026-08-15) and
[D8](#owner-decisions--2026-08-15).** The two questions still open, Q2 and Q3, are answerable by whoever
implements v3.0.0 and do not block the release being planned.

---

## Settled One-Sentence Purpose

> **`ProphetsWay.EFTools` supplies the Entity Framework Core half of a `ProphetsWay.BaseDataAccess` Data
> Access Layer — the CRUD, paging, soft-delete and transaction plumbing that is identical in every DAL —
> against whichever relational provider the consumer configures, so an implementer writes only the queries
> that are specific to their application.**

Five things that sentence does deliberately:

- **Names the parent contract.** This library is not independently meaningful. Every public type in it is
  either an implementation of a `ProphetsWay.BaseDataAccess` interface or a base class whose generic
  constraints are `BaseDataAccess` entity markers. A reader who does not already know that repository
  cannot use this one.
- **Names the boilerplate, not the technology.** The origin of the whole paradigm — carried over from the
  owner's earlier iBatisTools approach — is the observation that DAL CRUD is roughly 99% identical across
  entities. "Supply the 99% once, leave the 1% as custom DAO methods" is the *product*. Entity Framework
  is the mechanism.
- **Says EF Core, not "EntityFramework."** The shipped `<Description>` is ambiguous between EF6 and EF
  Core, and that ambiguity is not cosmetic — the two branches of this library have different public
  surfaces and different semantics. The owner has settled 3.x as **EF Core-only**; see
  [The EF6 Question](#the-ef6-question--settled).
- **Says "whichever relational provider the consumer configures."** Provider neutrality is now part of the
  purpose, not a nice-to-have. The library depends on `Microsoft.EntityFrameworkCore` and names no
  provider; the consumer supplies one through `DbContextOptions`. Neutrality is **bounded to relational
  providers**, and only **SQLite and SQL Server are certified** by this repository's tests — a distinction
  [D8](#owner-decisions--2026-08-15) requires the *public* wording to carry, not just this document. See
  [Provider Neutrality](#provider-neutrality--settled) and [Public Wording](#public-wording--settled).
- **Puts the boundary in the sentence.** "Only the queries that are specific to their application" is the
  out-of-scope test. If a proposed feature is something a consumer's own DAO could express as a custom
  method, it does not belong here.

### Audience

Two audiences, weighted equally, and that equality is itself a scope constraint:

| Audience | What it implies |
|---|---|
| The owner's own projects (`ProphetsWay.BPA` and others) | These are the **proving grounds**. A feature that has never been exercised by a real DAL has not earned publication. |
| General NuGet consumers | Nothing may assume the owner's conventions, schema, database, or provider. Anything shaped around one application's needs belongs in that application. |

The second audience is the one that most of the [Out of Scope](#out-of-scope-and-where-it-should-live-instead)
findings protect. A published package that quietly assumes SQL Server is not serving it.

---

## Owner Decisions — 2026-08-15

These are the owner's, not this agent's. They close the checkpoint questions this document was written to
pose. Every status change in [feature-requests.md](feature-requests.md) traces to one of them.

| # | Decision | Closes |
|---|---|---|
| **D1** | **3.x is Entity Framework Core-only.** The published **2.2.x** line remains available and installable as the legacy EF6 / .NET Framework answer. **No EF6 companion package will be built** \u2014 that option is rejected, not deferred. | [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) |
| **D2** | **3.x is relational-provider-neutral.** The consumer configures their provider through `DbContextOptions`. **SQL Server, PostgreSQL, MySQL/MariaDB, SQLite and Oracle are conceptually in scope** as relational providers. **Only SQLite and SQL Server will be certified** by this repository's tests. **Cosmos and other non-relational providers are out of scope.** | [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer) |
| **D3** | **Collapse the 18 key-specific public DAO classes into six generic root-namespace DAO families** \u2014 `BaseDao<TEntity, TKey>` and its five siblings. The public breaking change is **accepted** and lands in the same major. **No compatibility wrappers**, unless implementation evidence later forces reconsideration. | [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) \u2014 **this reverses the agent recommendation** |
| **D4** | **Test strategy is two-legged:** SQLite in-memory as the fast CI contract/query gate, and a SQL Server container for provider fidelity. Blanket `LocalTestsOnly: 'yes'` is to be retired eventually; **the pipeline work itself is separately owned.** | [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **D5** | **`docs/architecture.md` and per-project `docs/requirements.md` are `n/a`** for this library. This document plus `AGENTS.md` and the README are sufficient. | Confirms the house convention; ratifies the [Stale Inherited Claims](#stale-inherited-claims) row |
| **D6** | The **BaseDataAccess 3.1.0 upgrade, the `ProphetsWay.Example` submodule advance to 3.1.0, disposal/transaction compliance, provider decoupling, `FluentAssertions` removal and the malformed `.gitmodules` cleanup** are all intended for the 3.x cycle. | [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts), [2](feature-requests.md#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310), [3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess), [7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), [8](feature-requests.md#8--remove-fluentassertions-from-prophetswayexampledataaccessef), [9](feature-requests.md#9--delete-the-stray-submodule-submod-block-from-gitmodules) |
| **D7** | **3.x targets `net10.0` only** — the library, `ProphetsWay.EFTools.Tests`, and `ProphetsWay.Example.DataAccess.EF` alike. This is a **ratified exception** to the family's published-library TFM rule, not drift: **EF Core 10 exposes only a `net10.0` asset**, and this package's public surface **is an EF Core implementation, not a portable contract**, so the reach floor buys nothing it can deliver. **Existing `net4x` / `net8.0` / `net9.0` consumers stay on `ProphetsWay.EFTools` 2.2.x.** | **Closes [Q1](#unresolved-purpose-level-questions)**; [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework), [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard) |
| **D8** | **The certification scope is stated publicly, not only in these docs.** Public wording says EFTools is **designed for relational EF Core providers** and **certified and tested by this repository on SQLite and SQL Server**. It **must not imply that any other relational provider is certified**. | **Closes [Q4](#unresolved-purpose-level-questions)**; [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **D9** | The **obsolete local modification inside the pinned `ProphetsWay.Example` submodule was approved for discard and has been discarded.** The submodule working tree is clean; nothing local stands between the repository and advancing the pointer. | [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) |

**D7 is the decision most likely to be misread as drift by a future agent**, because it puts this
repository outside a family-wide convention. The reasoning is in
[The `net10.0`-Only Exception](#the-net100-only-exception--settled), and the short form is that the
convention's own justification does not apply here: `netstandard2.0` exists to be a **portable contract**,
and this package's entire surface is generic base classes over `DbContext`. `AGENTS.md` needs a line saying
so — **that file is not this agent's to write.**

**D3 is the decision a future reader will most want the reasoning for**, because this document previously
argued against it. The argument that changed is recorded in
[The Key-Type Collapse](#the-key-type-collapse--settled-against-this-documents-earlier-recommendation) —
one of the three reasons given for the earlier recommendation turned out to be contradicted by the
library's own source.

---

## Current Purpose (as implied by README/csproj)

The `<Description>` in
[ProphetsWay.EFTools/ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) and the
opening of [README.md](../README.md) say the same thing, near-verbatim:

> "A small library that is useful when utilizing EntityFramework for a Data Access Layer (DAL) while
> adhering to Business Layer to DAL decoupling. This uses the paradigm explained in
> …/ProphetsWay.BaseDataAccess, see its README for more information."

The README then spends its length on a walkthrough of the base classes in the order the owner would write
a new project — which is genuinely good teaching material and should survive any rewrite.

---

## The Drift

The drift here is **not** that the library grew features it should not have. The public surface is
disciplined: 26 source files, every one of them either a DAO base, a context base, or the DAL root, and
nothing resembling a utility, a helper, or a convenience API. That is unusual and worth saying plainly.

The drift is in four other places.

### 1. The purpose sentence has no subject

"A small library that is useful when…" describes a *feeling about* the library rather than its job. It does
not say what it removes, what it requires, or who finishes the work. A reader on nuget.org cannot tell
whether they need `ProphetsWay.BaseDataAccess` as well — they do, unconditionally — because the sentence
mentions it only as further reading.

### 2. "EntityFramework" hides an unresolved fork

The library currently ships **two different products under one package ID**, selected by target framework.
This is stated in `AGENTS.md` as a conditional package reference, which undersells it substantially. The
divergence reaches the public surface and the semantics:

| Where | `net461`/`net471`/`net48` (EF6 6.5.1) | `net80`/`net90` (EF Core 9.0.4) |
|---|---|---|
| [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) | `base(connectionString)` — EF6 resolves the provider from config | `UseSqlServer(connectionString)` — provider hardcoded |
| [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) | one constructor | **two** — a `DbContextOptions` overload exists only here |
| [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) | one constructor | **two** — `DbContextOptions` overload again |
| [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) `Update` | `Dataset.AddOrUpdate(item)` — an **upsert**; inserts when absent | `Single(…)` + `SetValues` + `EntityState.Modified` — **throws** when absent |
| [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) | `DbContextTransaction` | `IDbContextTransaction` |

The `Update` row is the one that matters. **The same method on the same type has different behaviour for a
missing row depending on which asset the consumer resolved** — silent insert on one, exception on the
other. That is not an `#if` around a `using`; it is two contracts.

**Settled by [D1](#owner-decisions--2026-08-15).** 3.x keeps the EF Core column and deletes the other. The
EF6 column does not become undocumented — it remains the behaviour of the published 2.2.x package, which
is where an EF6 consumer is now pointed.

### 3. The library forces a database provider on every consumer

`ProphetsWay.EFTools.csproj` takes unconditional `PackageReference`s on
`Microsoft.EntityFrameworkCore.SqlServer` **and** `Microsoft.EntityFrameworkCore.InMemory` for every
non-`net4x` target, and `BaseEFContext`'s string constructor calls `.UseSqlServer(...)` directly.

A consumer on PostgreSQL, SQLite or Cosmos installs this package and receives the SQL Server provider and
the InMemory provider whether they want them or not, and cannot use the string constructor at all. A
library whose stated purpose is *decoupling* is coupling its consumers to a provider. Recorded as
[FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer).

`InMemory` is the sharper half: it is a **testing** provider, Microsoft documents it as unsuitable for
production, and it is here because the *examples* use it. A test dependency of the proving ground has
become a runtime dependency of the product.

**Settled by [D2](#owner-decisions--2026-08-15).** Both provider references leave the library. See
[Provider Neutrality](#provider-neutrality--settled) for what "neutral" was defined to mean, which is
narrower than "any provider EF Core has."

### 4. The library no longer implements the contract it advertises

`ProphetsWay.EFTools.csproj` references `ProphetsWay.BaseDataAccess` **2.5.0**. The published parent is
**3.1.0**, and 3.0.0 made `IBaseDataAccess` extend `IDisposable` with `BaseDataAccess` declaring
`public abstract void Dispose();`
([BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs) line 112,
[IBaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/IBaseDataAccess.cs) line 164).

[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) overrides `TransactionCommit`,
`TransactionRollBack` and `TransactionStart` and **nothing else**. It will not compile against 3.1.0 until
it supplies `Dispose` — and deciding what `Dispose` *means* for a class that constructs its own `DbContext`
via `Activator.CreateInstance` is a design decision, not a mechanical fix.

So the README's paradigm claim is currently a statement about version 2.x. This is the same shape of
staleness that [ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md) records from
the other side, and it is why that request was routed here as
[FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts).

---

## Cohesion Map

Every public type in the library, grouped. `→` means "depends on."

| Cluster | Types | Depends on | Depended on by | Extraction candidate? |
|---|---|---|---|---|
| **Context base** | `BaseEFContext` | EF `DbContext`; on modern targets, `Microsoft.EntityFrameworkCore.SqlServer` | `BaseEFDataAccess` (generic constraint `where TContextType : BaseEFContext`); every consumer context | **No** — 6 lines of body; meaningless without EF |
| **DAL root** | `BaseEFDataAccess<TContextType, TIdType>` | `ProphetsWay.BaseDataAccess.BaseDataAccess`, `BaseEFContext`, EF | Every consumer DAL | **No** — its entire body is EF transaction calls against the parent's abstract members |
| **Internal engine** | `RootNonIdDao<T>`, `RootDao<T, TIdType>` (both `internal`) | EF, `IBaseEntity` / `IBaseIdEntity<T>` | Public bridge cluster | **No** — `internal`; not a surface |
| **Public bridge** | `RootBaseDao<T, TIdType>`, `RootBaseSoftDao<T, TIdType>`, `BaseNonIdDao<T>`, `BaseSoftNonIdDao<T>` | Internal engine, `IBaseDao`/`IBaseGetAllDao`/`IBasePagedDao` | Key-typed DAO surface | **No** — all four are `[EditorBrowsable(Never)]` or plumbing for it |
| **Key-typed DAO surface** | 18 types: `{BaseDao, BaseGetAllDao, BasePagedDao, BaseSoftDao, BaseSoftGetAllDao, BaseSoftPagedDao}` × `{Guid, Int, Long}` | Public bridge | Consumer DAOs | **No** — each is a ~10-line closure over the bridge; separating them from it is meaningless |

**[D3](#owner-decisions--2026-08-15) rewrites the last row, not the verdict.** The 18 key-typed types
collapse into **six generic families in the root namespace**. That changes the shape of the public surface
and its `using` story; it changes nothing about the dependency graph, because the collapsed types depend on
exactly what the triplicated ones did. The extraction verdict below is unaffected.

### The extraction verdict — `docs/nuget-extraction-proposal.md` is `n/a`, not missing

**There is no viable extraction candidate in this repository, and that file has deliberately not been
created.** The dependency argument, which is the only argument my charter accepts:

1. **Every cluster has a non-optional inbound edge from another cluster in the same package.** Not one has
   zero inbound edges. The key-typed surface cannot compile without the bridge; the bridge cannot compile
   without the internal engine; the DAL root cannot compile without the context base.
2. **Every cluster depends on both Entity Framework *and* `ProphetsWay.BaseDataAccess`.** A split would
   produce packages with strictly more dependencies than callers, which is the opposite of the point.
3. **The independent-usefulness test fails at the first question.** Nobody wants `Int.BasePagedDao<T>`
   without `RootBaseDao<T,int>`, and nobody wants either without an `IBaseDataAccess` to hang them on.

The interesting split question here is the **reverse** one, and it also resolves against splitting:
`ProphetsWay.Example.DataAccess.EF` lives in this repository rather than in `ProphetsWay.Example`. Moving
it would invert the dependency edge and create a **cycle between repositories** — `ProphetsWay.Example`
would depend on the `ProphetsWay.EFTools` package while `ProphetsWay.EFTools` consumes
`ProphetsWay.Example` as a submodule. The current arrangement is acyclic and correct. Recorded here so it
is not re-proposed as tidiness.

The one thing that *should* eventually be a sibling package —
`ProphetsWay.BaseDataAccess.Conformance` — is already owned by the parent repository as
[BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md), whose revisit trigger is
literally "after `ProphetsWay.EFTools` has been built and updated onto 3.x." It is not this repository's
to build.

---

## The EF6 Question — Settled

> **Settled by the owner on 2026-08-15 as [D1](#owner-decisions--2026-08-15).** The recommendation below
> was accepted as written. [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)
> is now `Scheduled` for v3.0.0. The reasoning is preserved in full — a decision without its argument is
> just an assertion, and this one will be questioned again.

### Scope Verdict — retaining EF6 support in 3.x

| | |
|---|---|
| **Verdict** | **Out of scope** for 3.x — EF6 belongs on the 2.2.x line, which is already published and stays available |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | Supporting both is not one conditional compile; it is two dependency graphs, two runtime matrices, two public surfaces, and — in `RootDao.Update` — two *semantics* under one method name. A package cannot honestly promise one contract while shipping two. |
| **Owner's decision** | **Approved.** 3.x is EF Core-only; 2.2.x remains the EF6 / .NET Framework answer and receives no new work; **no EF6 companion package** |
| **What follows from it** | The purpose sentence and `<Description>` say "Entity Framework Core"; the TFM list loses `net4x`; the `EntityFramework` 6.5.1 reference goes; roughly 60 `#if` blocks across 26 files collapse; the README's context examples stop showing `DbModelBuilder`; CHANGELOG carries a breaking-change entry pointing 2.2.x consumers at the existing package |

### The owner's stated instinct, and why the evidence points the other way

The instinct — *"the conditional code looks small, so keeping it looks cheap"* — is a reasonable reading of
the file listing. It is contradicted by what the files actually contain.

**Evidence that it is not small:**

- **The divergence is semantic, not syntactic.** `RootDao.Update` is an upsert on EF6 and a
  throw-if-missing update on EF Core (table in [The Drift](#2-entityframework-hides-an-unresolved-fork)).
  Any contract this library writes about `Update` is currently either false on one branch or so weak it
  says nothing.
- **The public surface differs by target.** The `DbContextOptions` constructors on `BaseEFContext` and
  `BaseEFDataAccess` exist only on modern targets. A `net48` consumer reading the README's preferred
  pattern — which the README explicitly recommends — finds it absent.
- **The preprocessor is already load-bearing in a way that is hard to read.** In
  [ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  an `#if NET471 || NET48` block *opens a constructor brace* that a separate `#if NET8_0_OR_GREATER` block
  closes. Braces span preprocessor boundaries. Every future edit to that constructor must be reasoned
  about twice, and no single compilation shows a reader the whole file.
- **The `#if` conditions are hardcoded TFM lists, not feature tests.** They are
  `#if NET461 || NET471 || NET48` and `#if NET8_0_OR_GREATER`. **Adding `netstandard2.0` — the house
  standard's permanent reach floor — matches neither branch**, producing files with no `using` directives
  and empty bodies. So EF6 retention does not merely coexist with the TFM modernization; it **blocks** it
  until every condition is rewritten. That is [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard)'s
  hidden cost, and it was not visible from the csproj alone.
- **The test matrix doubles at the exact moment it must be rewritten anyway.** `LocalTestsOnly: 'yes'` in
  [app-variables.yml](../app-variables.yml) means CI does not run these tests. Keeping EF6 means
  maintaining a .NET Framework leg that **nothing automated ever executes**.
- **The stated migration cost is near zero.** No known live consumer requires .NET Framework or EF6, a
  major bump is acceptable, and **2.2.x remains published and installable**. Retiring EF6 strands nobody;
  it stops shipping them *new* work.

**The one honest argument for keeping it**, stated at full strength because the owner should weigh it:
the EF6 branch is *written and working today*, and once deleted it is materially harder to restore than to
have kept — the `#if` blocks encode real knowledge about EF6's transaction and upsert behaviour that a
future reconstruction would have to rediscover. If a .NET Framework consumer appears in the next year, the
cheap answer today ("it already builds") becomes an expensive one ("port it back").

The counter to that counter is the 2.2.x package: the knowledge is not deleted, it is *tagged*. Git history
and a published, installable artifact both retain it.

### What is explicitly **not** being recommended

- Not recommending a `ProphetsWay.EFTools.EF6` companion package. That is a second repository, pipeline,
  version line, changelog and support surface for an audience currently measured at zero.
  **The owner rejected it explicitly in [D1](#owner-decisions--2026-08-15) — this is closed, not open.**
- Not recommending deprecating or unlisting 2.2.x. It should remain the supported answer for EF6.
- ~~Not recommending this be decided by an agent. **This is the checkpoint question.**~~ **Decided by the
  owner. The checkpoint is passed.**

### The `net10.0`-Only Exception — Settled

> Settled as [D7](#owner-decisions--2026-08-15), which **closes Q1**. This surfaced only after D1 was
> taken, as a conflict with the house TFM standard; it is now a **ratified exception** rather than an open
> conflict. Tracked as [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard).

### Scope Verdict — taking a documented exception to the `netstandard2.0` reach floor

| | |
|---|---|
| **Verdict** | **In scope, and it does not widen the purpose sentence.** The sentence already says "Entity Framework Core"; a target list that only EF Core can restore is that sentence being honest about itself |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | The reach floor exists to make a **portable contract** consumable from .NET Framework 4.6.1 upward. This package's public surface is not a contract — it is generic base classes over `DbContext`, and every one of them is meaningless without an EF Core runtime. There is no consumer the floor would reach who could use what they found there |
| **Owner's decision** | **Approved.** 3.x targets **`net10.0` only** — library, tests, and `ProphetsWay.Example.DataAccess.EF` alike. Existing `net4x` / `net8.0` / `net9.0` consumers remain on the published **2.2.x** line |

**The mechanical fact behind it.** EF Core has shipped no `netstandard2.0` asset since 3.1 — 5.0 onward
are runtime-targeted — and **EF Core 10 exposes only `net10.0`**. Adding `netstandard2.0` to an EF
Core-only `ProphetsWay.EFTools` produces a target that cannot restore its own primary dependency. This is
not a preference the owner exercised over the convention; it is the only target list that builds.

**Why `net10.0` alone rather than `net10.0` plus an LTS predecessor.** This document previously floated the
two-target option. It does not survive the same fact: an EF Core 10 dependency has no asset for an earlier
runtime, so a second TFM would either pin an older EF Core on that leg — reintroducing the
two-products-under-one-package-ID problem [D1](#owner-decisions--2026-08-15) exists to end — or not
restore at all.

**Who this strands: nobody, and that is checkable.** 2.2.x is published and installable, and it is where
`net4x`, `net8.0` and `net9.0` consumers stay. They stop receiving new work; they lose nothing they have.

**The test project's `net48` leg goes with it.** In `ProphetsWay.BaseDataAccess` and `ProphetsWay.Example`
that leg exists to bind the `netstandard2.0` asset and verify .NET Framework behaviour. Here there will be
no such asset to bind, so the leg would be testing nothing.

**What still has to happen elsewhere:** `AGENTS.md` needs a line recording this exception, so a future
agent reading `net10.0` alone sees a decision rather than drift. **That file is not this agent's to
write** — it is reported here for its owner.

---

## Provider Neutrality — Settled

> Settled as [D2](#owner-decisions--2026-08-15). Tracked as
> [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), now `Scheduled`.

### Scope Verdict — the library choosing a database provider

| | |
|---|---|
| **Verdict** | **Out of scope.** Choosing the database is the single most consumer-specific decision in a DAL |
| **Purpose it's measured against** | The settled sentence above, which now names provider neutrality explicitly |
| **Because** | A library whose stated purpose is *decoupling* cannot hand every consumer a SQL Server dependency they did not ask for, and cannot ship a *testing* provider in a runtime package |
| **Owner's decision** | **Approved.** The consumer configures their provider through `DbContextOptions` |

### What "neutral" was defined to mean — and the two lines it draws

Neutrality here is **bounded**, and the bound is the useful part of the decision. Three tiers, not two:

| Tier | Providers | What the library promises |
|---|---|---|
| **Certified** | **SQLite**, **SQL Server** | This repository's own tests run against them. A green suite is evidence for these two and only these two |
| **In scope, uncertified** | **PostgreSQL**, **MySQL/MariaDB**, **Oracle** | Relational, so nothing in the design excludes them. Untested here; a defect report against one is in scope to fix |
| **Out of scope** | **Cosmos** and other non-relational providers | Not a target. A defect that reproduces only on Cosmos is not a defect in this library |

The Cosmos exclusion is a genuine scope boundary rather than a lack of enthusiasm. The DAO surface assumes
things a relational store supplies and a document store does not — `Skip`/`Take` paging over a stably
ordered set, `GetCount` as a server-side aggregate, and transactions with the scope semantics the parent's
contract specifies. Promising those on Cosmos would mean either weakening the contract for everyone or
emulating them in the library, and emulation is the kind of surface this package exists to avoid.

**The certified/uncertified split is not a hedge.** It is the honest statement of what a test run proves.
Claiming five providers on the strength of two would be exactly the "asserted rather than demonstrated"
failure this document objects to elsewhere.

### Public Wording — Settled

> Settled as [D8](#owner-decisions--2026-08-15), which **closes Q4**. The question was whether the
> certified/uncertified split is stated on the **package** or only in these documents. It is stated on the
> package. Tracked as [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
> and [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer).

Public wording — `<Description>`, README, `PackageTags`, and anything else a reader meets before they meet
this repository — must say both halves and no more:

| Must say | Must not say |
|---|---|
| EFTools is **designed for relational Entity Framework Core providers** | That it supports Entity Framework 6, or any non-relational provider |
| It is **certified and tested by this repository on SQLite and SQL Server** | Anything implying **another relational provider is certified** — PostgreSQL, MySQL/MariaDB and Oracle are in scope and **uncertified**, and the wording must leave a reader in no doubt which they are getting |

The reason the second column matters is the whole point of the tier table above: the uncertified tier
exists to set expectations, and it sets none if it is invisible to the person installing from nuget.org. A
package that says "works with relational providers" unqualified has quietly promised five things it has
tested two of.

**Not this agent's files.** `<Description>` and `PackageTags` belong to `Modernizer`; the README belongs to
`README Author`. This entry is the wording constraint they implement, not the implementation.

### The one sub-question the decision does not answer

`BaseEFContext(string connectionString)` currently calls `.UseSqlServer(...)`. D2 removes the SQL Server
reference, so that body cannot survive as written. Whether the constructor is **deleted** or **retained**
with the provider configured by the derived context's `OnConfiguring` is now an implementation choice, not
a purpose question — either satisfies neutrality.

What D2 *does* settle is the option this document previously floated as a defensible alternative: a
SQL-Server-flavoured convenience entry point, or a separate `ProphetsWay.EFTools.SqlServer` package.
**Both are rejected.** A named provider in the package graph is the thing being removed; reintroducing it
behind a friendlier name reintroduces the coupling.

---

## The Key-Type Collapse — Settled Against This Document's Earlier Recommendation

> Settled as [D3](#owner-decisions--2026-08-15). Tracked as
> [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication), moved from
> *Proposed — recommended against* to `Scheduled`.

### Scope Verdict — replacing the 18 key-specific DAO classes with six generic families

| | |
|---|---|
| **Verdict** | **In scope.** It changes the shape of the surface that delivers the purpose, not the purpose |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | Six generic families deliver the same plumbing as 18 key-typed closures. The owner has accepted the ergonomic cost at the call site and the breaking change, in a major that is already breaking for four other reasons |
| **Owner's decision** | **Approved**, with **no compatibility wrappers** unless implementation evidence forces reconsideration |

### Why this document argued the other way, and which argument failed

The earlier recommendation rested on three legs. **The load-bearing one is contradicted by the library's
own source**, which is worth stating plainly rather than quietly deleting.

| Earlier argument | Status now |
|---|---|
| **"The `Get` override buys a translatable predicate."** The key-type split was said to exist because a comparison over an open `TIdType` cannot be translated to SQL | **Wrong, and checkable.** [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already does exactly this on the EF Core branch: `Dataset.AsTracking().Single(x => x.Id.Equals(item.Id))` in `Update`, and `Dataset.OrderBy(x => x.Id)` in `GetPaged` — both generic over `TIdType`, both translated. `Int/BaseDao.Get` uses `i.Id == item.Id` only because `int` permits the operator, not because the generic form is impossible. The claim was inherited from the README and restated here without opening `RootDao.cs`. That is the exact failure mode the house rule about affirming inherited claims warns about |
| **"The cost lands on every consumer; the saving lands on one maintainer."** `BaseDao<User>` becomes `BaseDao<User, int>` | **Still true, and the owner accepted it.** It is an ergonomic cost, not a capability cost, and it buys the ability to add a key type without adding a namespace |
| **"Binary-breaking for no behavioural benefit."** | **Still true, and now free.** The same major already breaks on FR 2, 4, 5 and 7. A breaking change that must ride a major is cheapest when a major is already leaving |

### What the collapse must preserve

- **The `Get` predicate must remain translatable.** `x.Id.Equals(item.Id)` is proven in `RootDao.Update`;
  the collapsed `Get` should use the same form rather than `EqualityComparer<TKey>.Default`, which EF Core
  cannot translate. This is the one place where implementation evidence could force D3's escape hatch.
- **The `TKey : struct` constraint on `RootDao<T, TIdType>` is a decision, not an accident.** It excludes
  `string` keys today. The collapse is the moment to decide whether the generic families keep it — and
  relaxing it would also close
  [Example FR 1](../../ProphetsWay.Example/docs/feature-requests.md)'s accepting-half gap. Flagged as an
  [unresolved question](#unresolved-purpose-level-questions); it is not settled by D3.
- **The `using ProphetsWay.EFTools.Guid;` namespaces disappear.** `Guid` as a namespace segment shadows
  `System.Guid` inside those files, which is a small ongoing readability tax the collapse also removes.

### The strongest argument against, recorded so it is weighed rather than forgotten

Six generic families in one namespace is a *less discoverable* surface than three namespaces of six. A
consumer who types `using ProphetsWay.EFTools.Int;` gets an IntelliSense list containing exactly the six
types that can possibly apply to them. After the collapse they get one list of six generic families and
must supply the right `TKey` themselves — the compiler catches a mistake, but later and less kindly than a
namespace that could not express it. If the DAO surface ever grows past six families, revisit this.

---

## Unresolved Purpose-Level Questions

Everything else in this document is settled. **Two of the original four are now closed** — they are kept in
the table with their answers rather than deleted, because both were raised as blocking questions elsewhere
and a reader arriving from those links needs to find the answer here.

| # | Question | Status | Where it is tracked |
|---|---|---|---|
| **Q1** | **Does this repository take a documented exception to the `netstandard2.0` reach floor?** An EF Core-only library cannot ship that asset | **Closed 2026-08-15 by [D7](#owner-decisions--2026-08-15) — yes.** 3.x targets **`net10.0` only**, library and tests and EF example alike; 2.2.x remains the answer for `net4x`/`net8.0`/`net9.0`. Reasoning in [The `net10.0`-Only Exception](#the-net100-only-exception--settled). **Still owed elsewhere:** the `AGENTS.md` line recording it, which is not this agent's file | [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard) |
| **Q2** | **Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one?** | **Open.** It widens the sentence from "constructs your context for you" toward "participates in your composition root" — and D2 makes it more likely a consumer wants it, since they now configure the provider anyway. Answerable by whoever implements | [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) |
| **Q3** | **Do the collapsed generic DAO families keep `where TKey : struct`?** | **Open.** Relaxing it admits `string` keys, which is a reach decision about who can use the library at all, not a refactor detail. Answerable by whoever implements | [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) |
| **Q4** | **Is "certified on SQLite and SQL Server" stated on the package, or only in this repository's docs?** | **Closed 2026-08-15 by [D8](#owner-decisions--2026-08-15) — on the package.** Public wording states relational EF Core providers **and** certification on SQLite and SQL Server only, and must not imply any other relational provider is certified. Constraint in [Public Wording](#public-wording--settled) | [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |

**Nothing purpose-level is now waiting on the owner.** Q2 and Q3 are implementation decisions with
purpose-level consequences — whoever builds v3.0.0 answers them and records the answer here.

---

## In Scope

Judged against the proposed purpose sentence. These are what the library is *for*:

- **Abstract DAO bases implementing the `ProphetsWay.BaseDataAccess` capability interfaces** —
  `IBaseDao<T>`, `IBaseGetAllDao<T>`, `IBasePagedDao<T>`. As of [D3](#owner-decisions--2026-08-15) these are
  **six generic families keyed on an open `TKey`**, replacing the 18 `Guid`/`Int`/`Long` closures.
- **Soft-delete DAO bases** that maintain `CreatedDate` / `UpdatedDate` / `DeletedDate` and filter deleted
  rows out of `GetAll`, `GetPaged` and `GetCount`. The `UseUtcTime` constructor flag on `RootBaseSoftDao`
  is in scope: it is a one-bit policy choice with no application knowledge in it.
- **Keyless (`IBaseEntity`) DAO bases** — `BaseNonIdDao<T>`, `BaseSoftNonIdDao<T>` — for join and composite
  entities that have no identifier to `Get` by.
- **A `DbContext` base** that standardizes how a consumer's context is constructed, so the DAL root can
  instantiate a typed context generically — **without naming a provider**, per
  [D2](#owner-decisions--2026-08-15).
- **A DAL root** that wires the parent's reflection dispatch to a real `DbContext` and implements the
  parent's transaction and disposal contracts against it.
- **Whatever obligations `ProphetsWay.BaseDataAccess` 3.x places on an implementation** — `Dispose`,
  the disposal contract, the transaction contract, and the snapshot and ordering rules insofar as they bind
  an EF-backed DAL. Meeting a contract this library advertises is not scope creep; it is the minimum.
- **`ProphetsWay.Example.DataAccess.EF` as a proving ground.** It is not part of the package and must never
  become part of it, but it is in scope for this *repository*: it is how "genuinely reusable" is
  demonstrated rather than asserted.

## Out of Scope (and where it should live instead)

| Not in scope | Where it belongs | Why |
|---|---|---|
| **Choosing a database provider** — the hardcoded `UseSqlServer` and the unconditional `SqlServer` + `InMemory` package references | The **consumer's** `DbContextOptions`; providers as the consumer's own `PackageReference` | Coupling every consumer to SQL Server contradicts the decoupling purpose. Settled by [D2](#owner-decisions--2026-08-15); [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer) |
| **Cosmos and other non-relational EF Core providers** | Nowhere in this family | The DAO surface assumes `Skip`/`Take` over a stably ordered set, server-side `GetCount`, and the parent's transaction scope semantics. [D2](#owner-decisions--2026-08-15) |
| **A SQL-Server-flavoured convenience entry point, or a `ProphetsWay.EFTools.SqlServer` package** | Nowhere — the consumer's own one-line `UseSqlServer` in `DbContextOptions` | Reintroduces by another name the coupling D2 removes. Previously floated here as defensible; **rejected by [D2](#owner-decisions--2026-08-15)** |
| **EF6 / .NET Framework support in 3.x** | The already-published **2.2.x** package | Two semantics under one package ID. Settled by [D1](#owner-decisions--2026-08-15); [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) |
| **A `ProphetsWay.EFTools.EF6` companion package** | Nowhere — 2.2.x already is it | A second repo, pipeline, version line and changelog for an audience measured at zero. **Rejected by [D1](#owner-decisions--2026-08-15)** |
| **Compatibility wrappers preserving `ProphetsWay.EFTools.Int.BaseDao<T>` et al.** | Nowhere — the major is the migration | Keeping 18 shims to soften a break the owner accepted doubles the surface for the life of 3.x. **Rejected by [D3](#owner-decisions--2026-08-15)**, subject to implementation evidence |
| Migrations, schema generation, seeding, `EnsureCreated` | The consumer's own project, or a `.sqlproj` as in `ProphetsWay.Example` | Schema lifecycle is an application concern; this library reads and writes rows |
| Query specifications, filter builders, LINQ helpers, `Include` strategies | The consumer's **custom DAO methods** — the deliberate 1% | This is the exact boundary the purpose sentence draws. If a DAO can express it, the base class must not |
| Async members / `IAsyncDisposable` | `ProphetsWay.BaseDataAccess` first — [BaseDataAccess FR 4](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md) | An implementation cannot add async to a contract it does not own. EFTools **must not lead here** |
| Nested transactions / savepoints | `ProphetsWay.BaseDataAccess` — [BaseDataAccess FR 2](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md), deferred out of scope by decision | Same reason. `EnsureBeginTransaction` already covers the case that motivated it |
| A conformance test kit | `ProphetsWay.BaseDataAccess.Conformance` — [BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md) | Shipping a test framework in a runtime package is the coupling the family exists to prevent |
| Caching, retry/resilience, connection pooling, logging | The consumer, or EF Core's own `ExecutionStrategy` / interceptors / `Microsoft.Extensions.Logging` | Already solved by EF Core and the BCL. Re-solving them is unmaintainable surface |
| A generic repository / unit-of-work abstraction beyond the parent's | Nowhere — the parent's interfaces **are** that abstraction | Adding a second one competes with `IBaseDataAccess` |
| Entity base classes / attributes / a fluent mapping DSL | `ProphetsWay.BaseDataAccess` owns entity markers; mapping is EF Core's `OnModelCreating` | A mapping DSL is a second ORM |
| `FluentAssertions` as a dependency of a non-test project | Nowhere — it is a test-only library and 8.x requires a paid commercial licence | Present today in `ProphetsWay.Example.DataAccess.EF.csproj`. [FR 8](feature-requests.md#8--remove-fluentassertions-from-prophetswayexampledataaccessef) |

### Cannot tell yet — the boundaries that remain open

Two, consolidated into [Unresolved Purpose-Level Questions](#unresolved-purpose-level-questions) above so
they are in one place. The older of them is **Q2 — who owns the `DbContext`'s lifetime?**
`BaseEFDataAccess` constructs its own context via `Activator.CreateInstance`, which under the parent's
disposal contract ("a DAL disposes what it created and not what was handed to it") means it must dispose
it. But if a future overload accepts an injected context — which is what a DI-hosted consumer will ask for,
and which [D2](#owner-decisions--2026-08-15) makes *more* likely now that the consumer configures the
provider themselves — that one must **not** be disposed. The contract is clear; the library has not yet
been designed to it. This is [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess).

---

## Recommended Refinements

Numbered to match [feature-requests.md](feature-requests.md). Effort is relative, not calendar time.
"Breaking?" is judged against the **published 2.2.0 package**. **Every row is now `Scheduled` for v3.0.0**
following the owner decisions above — including row 10, which this document had recommended against.

| # | Change | Rationale | Effort | Breaking? | Status |
|---|---|---|---|---|---|
| 1 | Advance the `ProphetsWay.Example` submodule onto 3.x and bring the EF DAL with it | The paradigm claim is currently a statement about history. Routed here from [Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md) | **Large** | No (repo-internal), but gates everything | **Scheduled** (D6) |
| 2 | `ProphetsWay.BaseDataAccess` `2.5.0` → `3.1.0` | The library advertises a contract it does not reference | Small edit, **large** consequence | **Yes** — transitively | **Scheduled** (D6) |
| 3 | Implement the 3.x disposal contract in `BaseEFDataAccess` | Required by #2 to compile; the *design* is the real work | Medium | **Yes** — new abstract obligation on derived DALs | **Scheduled** (D6); carries **Q2** |
| 4 | **Make 3.x EF Core-only; retire EF6/.NET Framework** | Two semantics under one package ID; blocks #5 | Medium (deletion) | **Yes** — by intent; 2.2.x remains | **Scheduled** (D1) |
| 5 | Retarget to **`net10.0` only** — off `net4x` and the undotted `net80`/`net90` monikers | `net461`/`net471` are EOL; `net80`/`net90` are non-canonical and EOL 10 Nov 2026; EF Core 10 ships only `net10.0` | Medium — the `#if` conditions go with #4 | **Yes** — TFM removal | **Scheduled** (D1 entails it; destination settled by **D7**) |
| 6 | Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory + `Scope` traits | The inheritance hook it uses no longer exists upstream | Medium | No — `IsPackable=false` | **Scheduled** (forced by 1) |
| 7 | Stop forcing `SqlServer` + `InMemory` on consumers | A decoupling library must not pick a provider | Small–medium | **Yes** — consumers add their own provider | **Scheduled** (D2) |
| 8 | Remove `FluentAssertions` 8.2.0 from `ProphetsWay.Example.DataAccess.EF` | Paid licence; test library in a non-test project | Trivial | No — not packaged | **Scheduled** (D6) |
| 9 | Delete the stray `[submodule "Submod"]` block in `.gitmodules` | Malformed; will confuse `git submodule` | Trivial | No | **Scheduled** (D6) |
| 10 | Collapse the `Guid`/`Int`/`Long` triplication into six generic families | Owner-approved; the "untranslatable predicate" objection was **factually wrong** | Medium | **Yes** — accepted, no wrappers | **Scheduled** (D3) |
| 11 | Certify on SQLite in-memory + a SQL Server container; retire blanket `LocalTestsOnly` | `InMemory` cannot honour transactions, so it cannot verify the contract this package now claims; **D8** makes the certification a public claim, so it must be earned | Medium | No | **Scheduled** (D4, D8); pipeline half **Deferred** |

**These are not independent.** #2 forces #3; #4 unblocks #5; #1 forces #6; #7 forces #11. The realistic
unit of work is **one v3.0.0 release containing #1–#11**, with nothing deferred out of it except the
pipeline edits inside #11, which belong to another owner.

---

## Stale Inherited Claims

Corrected here because this document's charter is markdown under `docs/`. **`AGENTS.md` and `README.md` are
not this agent's files to edit** — these are reported for their owners.

| Claim | Where | Status | Evidence |
|---|---|---|---|
| "`ProphetsWay.Example` is **vendored** here" | `AGENTS.md`, Known Deviations #1 | **False** | [.gitmodules](../.gitmodules) declares `path = ProphetsWay.Example`, `url = …/ProphetsWay.Example.git`, `branch = main`. It is a submodule. It cannot drift; it is *pinned*. `ProphetsWay.Example` corrected the same claim from its side |
| "Two copies … drift independently" | `AGENTS.md`, Known Deviations #1 | **False**, follows from the above | The problem is **coordination**, not duplication |
| "**This is the most modern repo in the family** … targets `net9.0` … When conventions conflict, prefer this repo's approach" | `AGENTS.md`, This Repo | **Stale, and actively harmful as guidance** | `ProphetsWay.BaseDataAccess` and `ProphetsWay.Example` are both at `netstandard2.0;net10.0` as of their 3.1.0 releases. This repo is at `net461;net471;net48;net80;net90` with **no `netstandard2.0`** and references the parent at 2.5.0. It is now the **least** modern of the three. An agent following that line will copy the wrong pattern |
| "EFTools carries an EF implementation of the very same `IExampleDataAccess`, and the tests do not change" | `ProphetsWay.Example/README.md` | **Pending, not permanently false** | **The reason moved on 2026-08-16.** The submodule pointer is no longer behind — it is at `d845863`, the 3.1.0 tree. What has not happened is this repository's own adoption: `ProphetsWay.Example.DataAccess.EF.csproj` and `ProphetsWay.EFTools.csproj` still reference `ProphetsWay.BaseDataAccess` 2.5.0, and `ExampleDataAccess` supplies neither `Dispose` nor the two Data Access Objects `IExampleDataAccess` now aggregates. FR 1's remaining steps are what make the claim true again |
| "The pipeline is green" as evidence the tests ran | general | **Misleading** | `LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml) — CI skips them |
| `docs/architecture.md`, per-project `docs/requirements.md` | house convention | **`n/a`, not missing** — ratified by [D5](#owner-decisions--2026-08-15) | Library repo, not a multi-project application solution. The owner has confirmed this document plus `AGENTS.md` and the README are sufficient |
| `docs/nuget-extraction-proposal.md` | house convention | **`n/a`, not missing** | No candidate clears the dependency test. See [the extraction verdict](#the-extraction-verdict--docsnuget-extraction-proposalmd-is-na-not-missing) |
| `docs/repo-profile.md` | house convention | **Present** — corrected 2026-08-15, re-verified 2026-08-16 | It was absent when this document's first pass ran, which is why that pass read source directly. `Repo Analyst` has since produced it, dated 2026-08-15. Its findings **agree** with this document on every overlapping claim — the EF6/EF Core `Update` divergence, the hardcoded `UseSqlServer`, the InMemory reference and the malformed `[submodule "Submod"]` block. **Its submodule rows were corrected on 2026-08-16**, after the pointer advanced; see the note below |

### Factual note — the submodule pointer advanced on 2026-08-16

Recorded here because several statements in this document were written against the older pointer and a
reader needs the correction in the same place as the text. **No decision or status below has been changed
by this note; that is `Purpose Refiner`'s to do.**

The `ProphetsWay.Example` submodule is now at **`d845863` — the 3.1.0 tree**, verified by reading
`.git/modules/ProphetsWay.Example/HEAD` and the checked-out working tree. Three consequences are facts
about the repository as it stands:

- **Step 1 of FR 1 has landed; steps 2–6 have not.** The recommended-refinements table below still reads
  `Scheduled` for that row, which is now a status that trails reality rather than a false statement of it.
- **The repository does not compile.** `ProphetsWay.EFTools.Tests` targets `net472;net48;net80;net90`
  against a `net48;net10.0` project reference and overrides an upstream member that no longer exists;
  `ExampleDataAccess` does not satisfy the 3.1.0 `IExampleDataAccess`. This is the mid-flight state FR 1
  and FR 6 both predicted, not a new regression.
- **No EFTools-owned `.cs` or `.csproj` has been changed to match.** The library still references
  `ProphetsWay.BaseDataAccess` 2.5.0, still carries the EF6 `#if` branches, still declares no `Dispose`,
  and still exposes the 18 key-specific DAO classes — checked file by file, not inferred.
| "The key-type namespaces are **required** so the default `Get` can build a proper select by Id" | `README.md`, and **restated by this document** in its first pass | **False** | [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already compares generically on the EF Core branch — `Single(x => x.Id.Equals(item.Id))` in `Update`, `OrderBy(x => x.Id)` in `GetPaged`. `Int/BaseDao.Get` uses `==` because `int` allows it, not because a generic form is untranslatable. **This document inherited the claim from the README without opening `RootDao.cs`**, and it was the load-bearing argument in the recommendation the owner overturned as [D3](#owner-decisions--2026-08-15) |
| "Retarget to `netstandard2.0;net10.0`" as this repo's house-standard destination | this document, first pass; house convention | **Unachievable here — and now a ratified exception, not a violation** | `ProphetsWay.EFTools.csproj` pins `Microsoft.EntityFrameworkCore` **9.0.4**, which ships no `netstandard2.0` asset — EF Core has been runtime-targeted since 5.0, and **EF Core 10 exposes only `net10.0`**. An EF Core-only library cannot carry the family's reach floor. Raised as **Q1**; **closed by [D7](#owner-decisions--2026-08-15)** — the destination is `net10.0` alone. `AGENTS.md` still needs the line recording it |
