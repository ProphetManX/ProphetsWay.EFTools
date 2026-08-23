# Repo Profile — ProphetsWay.EFTools

_Generated 2026-08-15. Evidence-based; every claim cites a source file._

---

## Reading Note — Rewritten 2026-08-22, then corrected the same day for lap 4

**This document was re-profiled against the working tree on 2026-08-22, after implementation laps 1, 2 and
3 — and then corrected again hours later, after lap 4 (`d00aad3`) landed and made a third of it false.** Six
passes of dated corrections had accreted on top of a 2026-08-15 original, and the result no longer
told a reader what was true. The correction history is compressed here; the sections below describe the
**tree as it stands at `d00aad3`**, not the tree as any earlier pass found it.

**Lap 4 is a pure deletion — 24 files, 773 lines, and no modification to any surviving file.** It removed
the 18 key-specific closures and with them the `ProphetsWay.EFTools.Guid` / `.Int` / `.Long` namespaces
entirely, the `RootBaseDao<T,TIdType>` and `RootBaseSoftDao<T,TIdType>` bridges, the internal
`RootDao<T,TIdType>`, and `LegacyRootNonIdDao<T>` / `LegacyBaseNonIdDao<T>` / `LegacyBaseSoftNonIdDao<T>`.
The file-by-file stat came from the owner and **could not be re-run here — this agent has no terminal.**
What *was* done here is stronger: the **effect** was verified directly against the tree by listing the
library folder and grepping every type declaration and every preprocessor directive in it.

**What earlier passes said that is now false, so it is not re-derived below:**

| Superseded claim | Where it stood | What the tree says |
| --- | --- | --- |
| Library has **27 source files**, then **39** — 34 public abstract classes, 1 enum, 4 internal classes | Projects in the Solution, Public API Surface | **15 files, one flat folder, no subfolders** — 12 public abstract classes, 1 enum, 2 internal static classes. **Both 27 and 39 are superseded** |
| **24 of the 39 files are the 2.2.x shape lap 4 deletes**, and every variant of that sentence | Throughout | **Lap 4 has landed.** Those 24 files do not exist |
| The dead `#if NET461 \|\| NET471 \|\| NET48` blocks **survive in 24 files** | What It Actually Does, Planned 3.x Direction, Gaps 2 | **Zero preprocessor directives of any kind remain library-wide** — grepped all 15 files for `#if`, `#else`, `#elif`, `#endif`, `#region`, `#endregion`, `#define` |
| `ProphetsWay.EFTools.Tests` has **19 source files / 151 cases** | Projects in the Solution, Tests and Build Facts | **25 files / 270 cases** |
| **Nothing tests this library's own public surface** | Tests and Build Facts, Gaps 5 | **126 of the 270 cases do**, in **8** locally written classes. **The figure of 7 that three earlier sentences carried was wrong against their own table, which listed eight** |
| `GetCore` / `UpdateCore` are among **`BaseDao`'s** protected seams | Public API Surface | They are declared on **`RootNonIdDao`** only — `RootNonIdDao.cs` lines 308 and 342 — and overridden in `RootSoftNonIdDao.cs` at 201 and 229. `BaseDao`'s seams are `TrackForWrite`, `ApplyUpdateValues`, `GetKey`, `MatchRow`, `KeyEquals`, `ApplyReadFilter`, `ApplyIncludes`, `ApplyStableOrder` |
| `CompanyResourceDao` **does not exist** and dominates the failures | Tests and Build Facts, Gaps 5 | It exists, on `RootNonIdDao<CompanyResource>`; the 11 remaining failures are elsewhere |
| `DepartmentDao` derives from **no EFTools base** | Public API Surface | `BaseSoftPagedDao<Department, int>` |
| `BaseEFContext`'s string constructor **hardcodes `UseSqlServer`** | What It Actually Does, Public API Surface | That constructor is gone; the class names no provider |
| `ObjectDisposedException` guarding **is not in place** | Gaps 1, Planned 3.x Direction | All 10 non-`Dispose` members call `ThrowIfDisposed()` |
| Six generic DAO families are **"Not started"** | Planned 3.x Direction | Twelve classes landed across laps 1–3; lap 4 removed everything they replaced |
| **7 warnings** on the last build; `Submod` Source Link warning present | Packaging Audit, Tests and Build Facts | **0 warnings** on 2026-08-22 |
| `AlternateKeyGuardSpikeTests.cs` carries **zero `[Trait]`** | Tests and Build Facts | All 7 theories now carry `Scope` and `Area` traits |
| `app-variables.yml` reads **`2` / `2` / `0`**, and the mismatch is **an open owner decision** | Reading Note, Gaps 10, Open Questions | **CLOSED.** It reads `Major: '3'` / `Minor: '0'` / `Patch: '0'` — opened 2026-08-22. The owner has taken the bump |

**What this pass could and could not measure.** Every structural claim below was taken by opening the file
named or by grepping the directory named on **2026-08-22**; the file is cited inline. **No build and no test
run was performed by this pass** — this agent has no command-execution tool, and the full suite needs a local
SQL Server. Build and test *results* are attributed to the owner's 2026-08-22 run and labelled as reported.
The case counts are the exception: they were derived by static count and **reconcile with the reported runner
totals exactly**, which is stated where it matters.

**Unchanged and re-verified rather than inherited:** the submodule pointer
(`.git/modules/ProphetsWay.Example/HEAD`, still `61d9e7dfb209c4a92b0c16d058aad1af08031fb5`) and the packaging
metadata block ([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj), which lap 4
did not touch). **`app-variables.yml` is *not* in that list any more** — it changed, and the change closes
the question this document had been recording. It now reads `3` / `0` / `0`. **What is still open is not a
decision:** 3.0.0 is set and **not yet tagged or published**, and nuget.org still serves 2.2.0.

---


## One-Line Purpose

Entity Framework Core base classes that implement the repetitive CRUD, paging, soft-delete, context,
ownership and transaction plumbing behind a `ProphetsWay.BaseDataAccess` Data Access Layer, so a consumer
writes only the members their own contract adds.

**Two versions of that sentence are true at once, and the difference matters.** The **published 2.2.0**
package supplies EF6 *and* EF Core bases against `ProphetsWay.BaseDataAccess` 2.5.0. **The working tree is
an EF Core-only library against 3.1.0**, targeting `net10.0` alone, and since lap 4 it carries the
twelve-class 3.0.0 surface **and nothing else** — the 2.2.x surface it replaces was deleted in `d00aad3`
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[CHANGELOG.md](../CHANGELOG.md), [docs/api-contract.md](api-contract.md)).

**The two are not yet the same artifact on nuget.org.** `app-variables.yml` reads `3` / `0` / `0`, but
3.0.0 is **not tagged and not published**, so a reader of the listing still meets the first sentence.

## What It Actually Does

Everything below was read from the file cited, on 2026-08-22.

- **EF Core only, and there is no longer any residue of the alternative.** No project references
  `EntityFramework` 6.x. **There is also no conditionally compiled code left at all** — grepping all 15
  library files for `#if`, `#else`, `#elif`, `#endif`, `#region`, `#endregion` and `#define` on 2026-08-22
  returns **nothing**. The dead `#if NET461 || NET471 || NET48` blocks carrying `using System.Data.Entity;`
  lived in exactly the 24 files lap 4 deleted, so they left with them — one removal, not two.
  `CHANGELOG.md`'s v3.0.0 entry says the same thing independently: *"there is no longer any conditionally
  compiled code in the library at all"* ([CHANGELOG.md](../CHANGELOG.md)).
- **The library selects no database provider in code.** Grepping every `.cs` under `ProphetsWay.EFTools/`
  for `UseSqlServer` and `UseInMemoryDatabase` returns **nothing**. `BaseEFContext` is a bare abstract
  `DbContext` with one `protected BaseEFContext(DbContextOptions)` constructor and no members;
  `BaseEFDataAccess<TContext>` takes a context the caller has already built. The provider is named in the
  consumer's own file ([BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs),
  [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs),
  [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
  **The `.csproj` is a different question** — see the Packaging Audit and Dependencies.
- **The DAL root owns transactions, ownership and disposal.** `BaseEFDataAccess<TContext>` holds one
  `IDbContextTransaction`, refuses to nest, clears its transaction state *before* attempting a commit so a
  failed commit leaves nothing open, and never consults `Context.Database.CurrentTransaction`. `Dispose` is
  `sealed override`, idempotent, non-throwing, rolls back an open transaction, runs a `protected virtual
  DisposeCore()` hook, and disposes the context **only** under `ContextOwnership.Owned`. **All 10
  non-`Dispose` members open with `ThrowIfDisposed()`** — the three transaction members plus `GetAll`,
  `GetPaged`, `GetCount`, `Get`, `Insert`, `Update`, `Delete` — and `ThrowIfDisposed` is `protected` so a
  derived layer guards its own forwarders ([BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs)).
- **Keyed DAOs take an open key.** `BaseDao<TEntity, TKey>` places **no constraint on `TKey`**, so `string`
  and `int?` are legal alongside `int`, `long` and `Guid`. The identifier property is resolved by name —
  `{TypeName}Id`, then `Id` — must be a **public instance** property, and is validated in the constructor,
  so a mis-wired entity fails when the layer is built rather than on first use. The key value is carried
  into the expression tree through a private `KeyCarrier` field reference specifically so the provider
  parameterizes it instead of emitting a SQL literal
  ([BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs)).
- **Soft delete is expressed as `override`, never `new`.** `BaseSoftDao<TEntity, TKey>` differs from
  `BaseDao` only by overrides, so a soft DAO reached through a `BaseDao`-typed reference still soft-deletes
  rather than silently hard-deleting. `ApplyReadFilter` adds `DeletedDate == null` **and nothing else**, so
  `GetAll`/`GetPaged`/`GetCount` agree with one another while the unfiltered `Get` still finds a deleted row
  ([BaseSoftDao.cs](../ProphetsWay.EFTools/BaseSoftDao.cs)).
- **Keyless entities get a family that commits to nothing.** `RootNonIdDao<TEntity>` implements **no**
  capability interface, so a join-table DAO publishes only what its own interface declares. Its one abstract
  member `MatchRow` replaces the whole identifier apparatus and is **the only abstract member in the
  library**; `ApplyStableOrder` throws `NotSupportedException` until overridden, which is what makes a
  write-only join DAO fully functional with `MatchRow` alone
  ([RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).
- **The keyed and keyless halves are unrelated inheritance branches**, so shared behaviour lives in two
  `internal static` classes rather than a common base: `EntityGraph` (copy-for-store, the reachability walk,
  inverse-navigation cleanup, the `finally` detach) and `SoftTimestamps` (both hook defaults and the
  post-materialization walk). Both say in their own `<remarks>` that the duplication of *declarations* is
  structural and accepted while duplication of *logic* is not
  ([EntityGraph.cs](../ProphetsWay.EFTools/EntityGraph.cs),
  [SoftTimestamps.cs](../ProphetsWay.EFTools/SoftTimestamps.cs)).
- **The 2.2.x surface is gone, deleted in `d00aad3`.** What left: 18 key-specific closures and with them the
  `ProphetsWay.EFTools.Guid` / `.Int` / `.Long` namespaces **entirely**; the `RootBaseDao<T, TIdType>` and
  `RootBaseSoftDao<T, TIdType>` bridges (both `where TIdType : struct` — the constraint the open-key
  families exist to drop); the internal `RootDao<T, TIdType>`; and the three `Legacy*` types renamed in
  lap 3 purely to free `BaseNonIdDao` / `BaseSoftNonIdDao` / `RootNonIdDao` for the new ones. Those three
  existed for a handful of commits and **never shipped**: `CHANGELOG.md`'s v3.0.0 entry states that none of
  them is part of 3.0.0 and that there is no `LegacyBaseNonIdDao<T>` to migrate onto. **Never write new
  code against any of these names, and do not re-report them as present**
  ([CHANGELOG.md](../CHANGELOG.md), [docs/api-contract.md](api-contract.md) § *The Public Surface*).


## Projects in the Solution

`ProphetsWay.EFTools.sln` declares **seven build projects and two solution folders** — `ProphetsWay.Example
submodule` and `Solution Items`. Three projects are owned by this repository; four come from the
`ProphetsWay.Example` git submodule ([ProphetsWay.EFTools.sln](../ProphetsWay.EFTools.sln),
[.gitmodules](../.gitmodules)). Counts re-taken 2026-08-22 by listing each directory.

| Project | Type | Role |
| --- | --- | --- |
| `ProphetsWay.EFTools` | Packable library | The published product. **15 source files, one flat folder, no subfolders** — `Guid/`, `Int/` and `Long/` left with `d00aad3`. One top-level type per file: **12 public abstract classes, 1 public enum, 2 internal static classes**. That is the 3.x surface and nothing else. **Zero preprocessor directives library-wide** |
| `ProphetsWay.Example.DataAccess.EF` | Library / proving ground | EF implementation of the Example contracts. **9 source files — `ExampleContext.cs`, `ExampleDataAccess.cs` and 7 DAOs.** All seven derive from a 3.x family |
| `ProphetsWay.EFTools.Tests` | xUnit project | **25 source files.** 13 one-line adapters, **8** classes written directly against this library's surface, 2 seam guards, `TestSeam.cs`, `Constants.cs`. **The figure of 7 carried by earlier passes was wrong against their own table** |
| `ProphetsWay.Example.DataAccess` | Submodule library | Entities and DAL contracts, including `Department` and `CompanyResource` |
| `ProphetsWay.Example.DataAccess.NoDB` | Submodule library | In-memory implementation; the upstream default |
| `ProphetsWay.Example.Tests` | Submodule xUnit project | 15 test classes holding **164 cases**. **13 are adapted here and do run against Entity Framework**; the 2 `ConventionShowcase` classes (20 cases) are correctly excluded |
| `ProphetsWay.Example.Database` | Submodule database project | **SDK-style `Microsoft.Build.Sql/2.2.0`** |

### The submodule pointer

`.git/modules/ProphetsWay.Example/HEAD` holds **`61d9e7dfb209c4a92b0c16d058aad1af08031fb5`** — read
2026-08-22, **unchanged since 2026-08-18**. `.gitmodules` declares one block: `path = ProphetsWay.Example`,
`url = https://github.com/ProphetManX/ProphetsWay.Example.git`, `branch = main`.

**It is one commit past the `3.1.0` tag, not the tag itself.** `git submodule status` renders it
`3.1.0-1-g61d9e7d`; that one commit is the merge of `ProphetsWay.Example` PR #21
(`3.1.1-eftool-findings`) on 2026-08-18, which opened the untagged **3.1.1** line and added
`TestDataAccessFactory.Use` — the seam this repository's harness is built on. **Any text describing the
pointer as `d845863`, as "the 3.1.0 tree", or as sitting on a tagged release is superseded.**

**The malformed `[submodule "Submod"]` block is gone.** `.gitmodules` was re-read on 2026-08-22 and contains
only the block above. See the Source Link note under Packaging Audit for what its presence cost while it
lasted, and what its removal did **not** buy.

## Public API Surface

**13 public declarations: 12 abstract classes and one enum.** Two further types — `EntityGraph` and
`SoftTimestamps` — are `internal static` and are not API. **Everything is in namespace
`ProphetsWay.EFTools`; there are no sub-namespaces.** Established 2026-08-22, after lap 4, by grepping every
type declaration in `ProphetsWay.EFTools/`: the grep returns **exactly 15 matches in 15 files**, one per
file, and they are the 15 rows below.

**There is one table now, not two, and the deletion is no longer pending.** `docs/api-contract.md`
revision 11 § *The Public Surface* specifies **twelve public classes plus one enum** as the 3.0.0 target.
All thirteen are in the tree, and after `d00aad3` **nothing else is** — that document's *"Types that
disappear"* list has disappeared. **The sentence "all of it is still present" is dead.** What that list held
is recorded below the table so a reader meeting the names in the published 2.2.0 package knows where they
went.

### The public surface — the whole library

| Type | Members that matter | Purpose | Tested? |
| --- | --- | --- | --- |
| `BaseEFContext` | one `protected BaseEFContext(DbContextOptions)` and **nothing else** | Optional provider-free context base. Deriving from it is not required — `BaseEFDataAccess<TContext>` constrains to `DbContext` | Indirectly, via `ExampleContext` |
| `BaseEFDataAccess<TContext>` | `protected BaseEFDataAccess(TContext, ContextOwnership)`; `TransactionStart` / `TransactionCommit` / `TransactionRollBack`; the seven dispatcher overrides; `sealed override Dispose`; `protected virtual DisposeCore`; `protected ThrowIfDisposed`; `protected TContext Context`; `protected ContextOwnership Ownership`; `protected bool IsDisposed` | The DAL root — transactions, ownership, disposal. **One type parameter.** Both public constructors from 2.2.x are gone | **Yes** — `KeylessDaoTests` builds one directly, plus the whole adapted upstream suite |
| `ContextOwnership` *(enum)* | `Borrowed`, `Owned` | No default value to fall into; the constructor rejects anything else with `ArgumentOutOfRangeException` | Yes |
| `BaseDao<TEntity, TKey>` | `Get`, `Insert`, `Update`, `Delete`, `GetAll`, `GetPaged`, `GetCount`; `protected DbSet<TEntity> Dataset`; `protected` seams `TrackForWrite`, `ApplyUpdateValues`, `GetKey`, `MatchRow`, `KeyEquals`, `ApplyReadFilter`, `ApplyIncludes`, `ApplyStableOrder`. **`GetCore` and `UpdateCore` are *not* here** — earlier passes of this document listed them among these seams and were wrong; see `RootNonIdDao` below | Keyed CRUD, **`TKey` unconstrained** | **Yes** — `KeyPredicateOpenKeyTests` (21), `IdentifierResolutionTests` (4) |
| `BaseGetAllDao<TEntity, TKey>` | adds `IBaseGetAllDao<TEntity>` over `BaseDao` | `+ IBaseGetAllDao<TEntity>` | Transitively — `JobDao`, `ResourceDao` |
| `BasePagedDao<TEntity, TKey>` | adds `IBasePagedDao<TEntity>` over `BaseDao` | `+ IBasePagedDao<TEntity>` | Transitively — `CompanyDao`, `TransactionDao` |
| `BaseSoftDao<TEntity, TKey>` | overrides — never `new` — plus `GetCurrentTimestamp`, `NormalizeRetrievedTimestamp`, and an `ApplyReadFilter` override adding `DeletedDate == null` | Keyed CRUD with soft-delete semantics | **Yes** — `SoftDeleteTimestampHookTests` (12) |
| `BaseSoftGetAllDao` / `BaseSoftPagedDao<TEntity, TKey>` | as above plus the capability member | Soft + `GetAll` / `GetPaged` + `GetCount` | **Yes** — plus `DepartmentDao` and its 40 upstream cases |
| `RootNonIdDao<TEntity>` | **`protected abstract MatchRow`** — the library's only abstract member; `Insert`, `Delete`, `GetAll`, `GetPaged`, `GetCount`; **`protected virtual GetCore` (line 308) and `UpdateCore` (line 342), declared here and nowhere else**; `ApplyStableOrder`, `ApplyReadFilter`, `ApplyIncludes`, `Dataset` | Keyless plumbing that implements **no** capability interface | **Yes** — `KeylessDaoTests` (31), `CompanyResourceConversionTests` (3) |
| `BaseNonIdDao<TEntity>` | adds nothing | `RootNonIdDao` + `IBaseDao<TEntity>` | Yes — `PublishedLinkDao`, `RegistrationDao` |
| `RootSoftNonIdDao<TEntity>` | soft overrides + the two timestamp hooks; **overrides `GetCore` (201) and `UpdateCore` (229)** | Keyless soft delete, committing to no interface | **Yes** — `KeylessSoftDaoTests` (29) |
| `BaseSoftNonIdDao<TEntity>` | adds nothing | `RootSoftNonIdDao` + `IBaseDao<TEntity>` | Yes — `PublishedTagDao` |
| `EntityGraph`, `SoftTimestamps` | *(internal, not API)* | The single copy of the navigation-graph mechanics and of the soft-timestamp policy. They exist because the keyed and keyless families are unrelated inheritance branches and cannot share a declaration | Exercised through the families |

**Where the protected seams actually live — re-derived 2026-08-22 by grepping every `protected` / `public`
member declaration in all 15 files, because this document had it wrong.** `MatchRow` exists on both
branches with different modifiers: `protected virtual` on `BaseDao` (line 477) and `protected abstract` on
`RootNonIdDao` (line 246). **`RootNonIdDao.MatchRow` is the only abstract member in the library.** `GetCore`
and `UpdateCore` exist **only** on the keyless branch. `ApplyStableOrder` is total by default on `BaseDao`
and throws `NotSupportedException` on `RootNonIdDao` until overridden
([BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs), [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs),
[RootSoftNonIdDao.cs](../ProphetsWay.EFTools/RootSoftNonIdDao.cs)).

### Removed by lap 4 (`d00aad3`) — recorded, not silently dropped

**Nothing in this table is in the tree.** It is kept because a reader comparing this repository to the
published 2.2.0 package — or to an alpha/beta cut taken mid-flight — will meet these names.

| Type | Was | Fate |
| --- | --- | --- |
| `Guid.*`, `Int.*`, `Long.*` DAO bases | 18 public abstract classes, 6 per namespace | **Deleted**, and the three namespaces with them. Replaced by the open-key families: `Int.BaseDao<T>` → `BaseDao<T, int>`, `Guid.BaseSoftPagedDao<T>` → `BaseSoftPagedDao<T, Guid>`, and so on through all eighteen |
| `RootBaseDao<T, TIdType>` / `RootBaseSoftDao<T, TIdType>` | 2 public abstract bridges, `[EditorBrowsable(Never)]`, `where TIdType : struct`. Both carried the three `Ensure*Transaction` members | **Deleted.** That `struct` constraint is the one the open-key families exist to drop. **The `Ensure*Transaction` trio now matches nowhere in the library** — grepped 2026-08-22 |
| `LegacyBaseNonIdDao<T>` / `LegacyBaseSoftNonIdDao<T>` | 2 public abstract classes | **Deleted.** They existed for three commits, renamed in lap 3 purely to free the new names. `CHANGELOG.md` says in terms that none of them is part of 3.0.0 |
| `RootDao<T, TIdType>` / `LegacyRootNonIdDao<T>` | 2 internal classes — the 2.2.x engines | **Deleted.** `RootDao`'s body is absorbed by the bases it served |

**`RootNonIdDao` is one name across two different types, and the older one did not survive.** 2.2.x had an
`internal RootNonIdDao<T>` engine; the tree has a `public abstract RootNonIdDao<TEntity>` extension point
with a different body, different visibility, different members and a `MatchRow` contract. The 2.2.x engine
was carried through lap 3 under the name `LegacyRootNonIdDao<T>` and **deleted in lap 4**. **A reader
comparing the two trees must not conclude the type was merely made public** — `docs/api-contract.md` says
the same thing and asks that it be described as new public surface
([api-contract.md](api-contract.md), [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).


## Dependencies

### Published library

Read from [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) on 2026-08-22.
**No EF6 row appears because no EF6 reference exists** — a repository-wide search for a `PackageReference`
to `EntityFramework` matches nothing.

| Dependency | Version | Why it is present |
| --- | --- | --- |
| `Microsoft.EntityFrameworkCore` | **10.0.11** | The implementation |
| `Microsoft.EntityFrameworkCore.SqlServer` | **10.0.11** | **Nothing in the library's C# source needs it.** A leftover; FR 7 / D2 removes it |
| `Microsoft.EntityFrameworkCore.InMemory` | **10.0.11** | Same — no library source references it |
| `ProphetsWay.BaseDataAccess` | **3.1.0** | Parent DAL contracts |

The three EF Core references sit inside
`Condition="!$(TargetFramework.StartsWith('net4')) and $(TargetFramework.StartsWith('net'))"`, which is
unconditionally true given the single `net10.0` target — the condition is a vestige of the EF6 split and
now gates nothing.

**The provider problem is now packaging-only, and that is a narrowing.** Earlier passes of this document
recorded `UseSqlServer` in `BaseEFContext`. It is gone: a grep of every `.cs` under `ProphetsWay.EFTools/`
for `UseSqlServer` and `UseInMemoryDatabase` returns nothing, and the provider is named in
[ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs), the consumer's own file.
What remains is that a PostgreSQL consumer of the eventual package still transitively restores two
providers they will never use. Removing a transitive reference is breaking, so it lands in 3.0.0 —
[FR 7](feature-requests.md), **D2**.

**There is no central version file.** No `Directory.Packages.props`, no `Directory.Build.props`, and
`ManagePackageVersionsCentrally` is set nowhere — so the three `.csproj` files are the only place package
versions live.

**Release-relevant consequence:** pinning at 10.0.11 sets the **minimum EF Core version a consumer of the
eventual 3.0.0 package must resolve**. Recorded here as a project-file fact; the release note is
`Changelog Author`'s.


### Tests and proving ground

- `ProphetsWay.EFTools.Tests` uses `Microsoft.NET.Test.Sdk` 17.13.0, xUnit 2.9.3,
  `xunit.runner.visualstudio` 3.0.2, coverlet 6.0.4, and **`Microsoft.EntityFrameworkCore.Sqlite`
  10.0.11** in its own `ItemGroup`, commented *"Relational provider for the SQLite in-memory certification
  leg; must not move into the library."* It project-references `ProphetsWay.Example.DataAccess.EF` and
  `ProphetsWay.Example.Tests`. **It carries no Shouldly reference of its own** — the assertion library
  arrives transitively from `ProphetsWay.Example.Tests`
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).
- `ProphetsWay.Example.DataAccess.EF` references `Microsoft.EntityFrameworkCore` and
  `Microsoft.EntityFrameworkCore.SqlServer` (both 10.0.11) and `ProphetsWay.BaseDataAccess` **3.1.0**, and
  nothing else. **It carries no `InMemory` provider** — unlike the library. The FluentAssertions 8.2.0
  reference earlier passes recorded here is **gone**, closing the paid-commercial-licence exposure
  ([ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)).
- The submodule's Example tests use **Shouldly 4.3.0**, xUnit 2.9.3, `xunit.runner.visualstudio` 3.0.2,
  `Microsoft.NET.Test.Sdk` 17.13.0 and coverlet 6.0.4
  ([ProphetsWay.Example.Tests.csproj](../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj)).

## Target Frameworks

| Project | Current TFMs | Review |
| --- | --- | --- |
| `ProphetsWay.EFTools` | **`net10.0`** | **Corrected 2026-08-16 — retargeted.** This is the approved destination under **D7**, a ratified exception to the house `netstandard2.0;net10.0` standard, not drift. The `net461;net471;net48;net80;net90` list previously recorded here is history |
| `ProphetsWay.Example.DataAccess.EF` | **`net10.0`** | **Corrected 2026-08-16 — retargeted**, same decision |
| `ProphetsWay.EFTools.Tests` | **`net10.0`** | **Corrected 2026-08-16 — retargeted**, and the restore mismatch this row previously described is **closed**: the single `net10.0` leg binds `ProphetsWay.Example.Tests`'s `net10.0` asset. The `net48` leg is deliberately absent — there is no `netstandard2.0` asset for it to bind |
| submodule Example DataAccess / NoDB | `netstandard2.0;net10.0` | **Corrected 2026-08-16.** The 3.1.0 pointer put both on the house standard; the `net461;…;net90` list previously recorded here belonged to the old pointer |
| submodule Example Tests | `net48;net10.0` | **Corrected 2026-08-16**, same cause |

No EFTools-owned project sets `LangVersion`, so each target uses its SDK default
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj),
[ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).

**Both halves of the 3.x TFM/provider direction are now partly or wholly done.** `net10.0` alone is what
the approved EF Core-only design requires, and the retarget delivered it. Provider neutrality is **done in
code and not in packaging** — the library still carries the SQL Server and InMemory package references. See
Dependencies.

## Packaging Audit

**PACKAGING: ACTION REQUIRED.** Publication intent is explicit: `PackageId` is non-empty,
`PostTargetToNuGet: 'yes'`, and the shared pipeline packs alpha, beta, and release artifacts
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[app-variables.yml](../app-variables.yml),
[package-artifacts.yml](../../prophets-pipelines/steps/package-artifacts.yml)).

| Field | State | Evidence / impact |
| --- | --- | --- |
| `PackageId` | Present: `ProphetsWay.EFTools` | Correct |
| `Description` | Present | Current wording says ambiguous “EntityFramework” and describes 2.x |
| `PackageLicenseExpression` | Present: `MIT` | Correct; LICENSE is MIT |
| `PackageReadmeFile` + packed README | Both present | Correct |
| `PackageIcon` + packed icon | Both present | Correct |
| `RepositoryUrl` / `RepositoryType` | URL present; type is `GitHub` | URL correct; standard repository type is `git` |
| `PackageProjectUrl` | Empty | NuGet homepage link absent |
| `PackageTags` | Empty | Discoverability metadata absent |
| `PackageReleaseNotes` or CHANGELOG | Empty in source; CHANGELOG packed and pipeline populates release notes | Upgrade guidance is available in pipeline-built packages |
| `PublishRepositoryUrl` + `EmbedUntrackedSources` + SourceLink | Missing | Consumer source navigation/debugging metadata absent |
| `IncludeSymbols` + `SymbolPackageFormat=snupkg` | Missing | No symbol package configuration |
| `Deterministic` / `ContinuousIntegrationBuild` | Not explicit / missing | SDK builds are deterministic by default; CI-specific normalization is absent |

All source states above come from
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj). The README, changelog,
and icon packing entries are correctly paired.

### Source Link is off — and the malformed `.gitmodules` block that used to mask the question is gone

**The `Submod` warning is closed, measured on both ends.** The owner's `dotnet build` on **2026-08-16**
emitted, **three times** — once each for `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and
`ProphetsWay.EFTools.Tests`:

```
Microsoft.Build.Tasks.Git.targets(25,5): warning : The path of submodule 'Submod' is missing or invalid: ''.
  The source code won't be available via Source Link.
```

The cause was a malformed `[submodule "Submod"]` block — `branch = main`, no `path`, no `url`. **It is
absent from `.gitmodules` as read on 2026-08-22**, and the owner's build on that date reports **0
warnings**. That is what raised this from *Low / cosmetic* — a reasoned reading — to a **measured** cost:
while it stood it disabled Source Link on a published package, so a consumer could not step into its
source.

**Removing it stopped the warning. It did not give the library Source Link.** Re-verified by opening
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) on 2026-08-22: SourceLink
is not referenced at all, and `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`,
`SymbolPackageFormat` and `ContinuousIntegrationBuild` are all absent. **Read this note and the table above
together, or you will conclude Source Link works.** It starts working when the snippets below are applied.

**PROPOSED — not applied:** add or replace these values in the existing main `PropertyGroup`:

```xml
<RepositoryType>git</RepositoryType>
<PackageProjectUrl>https://github.com/ProphetManX/ProphetsWay.EFTools</PackageProjectUrl>
<PackageTags>entity-framework;data-access;dal;crud</PackageTags>
<PublishRepositoryUrl>true</PublishRepositoryUrl>
<EmbedUntrackedSources>true</EmbedUntrackedSources>
<IncludeSymbols>true</IncludeSymbols>
<SymbolPackageFormat>snupkg</SymbolPackageFormat>
<Deterministic>true</Deterministic>
<ContinuousIntegrationBuild Condition="'$(TF_BUILD)' == 'True'">true</ContinuousIntegrationBuild>
```

**PROPOSED — not applied:** add this to the unconditional package-reference `ItemGroup`:

```xml
<PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
```

The 3.x `<Description>` and provider-specific tags should be finalized only after the approved redesign
is implemented; writing future behavior into the current package metadata would be inaccurate.

## Tests and Build Facts

### What is in `ProphetsWay.EFTools.Tests`

**25 source files**, listed and read 2026-08-22. **Do not restate "19 files", "151 cases", "165 counting
the spike", or "nothing tests this library's own public surface" — all four are false.**

- **`TestSeam.cs`** — an `internal static` class whose `[ModuleInitializer]` calls
  `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess)`. That one call points the **entire**
  upstream suite at `ProphetsWay.Example.DataAccess.EF.ExampleDataAccess` for the assembly's whole run. A
  module initializer rather than a fixture, because xUnit runs collections in parallel and a fixture is
  constructed after another collection may already have built a DAL
  ([TestSeam.cs](../ProphetsWay.EFTools.Tests/TestSeam.cs)).
- **13 adapters** — `EFBaseDataAccessTests`, `EFCompanyDaoTests`, `EFCompanyResourceDaoTests`,
  `EFCompanyResourceDataAccessTests`, `EFDataAccessDisposalTests`, `EFDataAccessTransactionTests`,
  `EFDepartmentDaoTests`, `EFDepartmentDataAccessTests`, `EFJobDaoTests`, `EFResourceDaoTests`,
  `EFSnapshotDeepCopyTests`, `EFTransactionDaoTests`, `EFUserDaoTests`. Each is literally
  `public class EFXxxTests : XxxTests { }` — inheritance is all xUnit needs to discover an upstream class
  in this assembly.
- **2 seam guards, both `[Trait("Guard","Seam")]`.** `TestSeamTests.cs` (5 `[Fact]`, deriving from
  `BaseUnitTests<ICompanyDao>` so it asserts on the same `_da` field every adapted test reads) pins the
  concrete type by full name, that the provider is **relational**, that the factory hands out a fresh
  instance per call, and that the store is reachable. `AdapterCoverageTests.cs` (2 `[Fact]`) reflects over
  the upstream assembly and fails if any class deriving from `BaseUnitTests<>` or carrying `[Collection]`
  has no adapter here, and separately pins that the two `ConventionShowcase` classes stay **excluded** —
  those build their own deliberately mis-wired DALs and are the subject of their tests rather than the
  implementation under test ([AdapterCoverageTests.cs](../ProphetsWay.EFTools.Tests/AdapterCoverageTests.cs)).
- **8 classes written directly against this library's own public surface.** **Earlier passes of this
  document said "7" in three places while listing eight rows below and doing the arithmetic at eight; the
  count was simply wrong and is corrected here.** Each declares its own entities, `DbContext` and DAOs, so
  it exercises `ProphetsWay.EFTools` rather than `IExampleDataAccess`:

  | File | Cases | What it targets |
  | --- | --- | --- |
  | `KeylessDaoTests.cs` | 31 | `RootNonIdDao` / `BaseNonIdDao`, plus a `BaseEFDataAccess<KeylessContext>` built in-test |
  | `KeylessSoftDaoTests.cs` | 29 | `RootSoftNonIdDao` / `BaseSoftNonIdDao` and the timestamp hooks |
  | `KeyPredicateOpenKeyTests.cs` | 21 | `BaseDao<TEntity, TKey>` with `string`, `int?` and `int` keys; a `DbCommandInterceptor` records the emitted SQL to prove parameterization |
  | `AlternateKeyGuardSpikeTests.cs` | 14 | EF Core's read-only-key guard, on `InMemory` and `Sqlite`. Its own `<remarks>` call it *"an empirical spike, not a specification"* |
  | `SoftDeleteTimestampHookTests.cs` | 12 | `BaseSoftDao` / `BaseSoftGetAllDao` / `BaseSoftPagedDao` |
  | `FailedInsertWriteBackTests.cs` | 5 | The lap-3 post-save write-back window, across keyed, soft and keyless families |
  | `IdentifierResolutionTests.cs` | 4 | `{TypeName}Id` → `Id` resolution, including the explicit-implementation failure |
  | `CompanyResourceConversionTests.cs` | 3 | That `CompanyResourceDao` is on `RootNonIdDao` and publishes no `Get` |

- **`Constants.cs`** — one nested `ConnectionStrings` class holding a single SQL Server connection string
  (`Data Source=localhost;Initial Catalog=ProphetsWay.Example;Integrated Security=True;TrustServerCertificate=True`)
  and one expression-bodied `GetExampleDataAccess`. **No `#if` branches, and the string is live**
  ([Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs)).

### The case count, and why it can be stated without a terminal

**270 discoverable cases**, counted 2026-08-22 by grepping every `[Fact]`, `[Theory]` and `[InlineData]` in
`ProphetsWay.EFTools.Tests/` and in `ProphetsWay.Example.Tests/`. The two halves were counted independently
and reconcile with the runner totals the owner reported, which is the strongest available check short of
running it:

| Half | Cases | Derivation |
| --- | --- | --- |
| Adapted upstream | **144** | The upstream suite is 164; `ConventionShowcaseTests` (11) and `ExceptionPassthroughShowcaseTests` (9) are deliberately unadapted, leaving 164 − 20 |
| Declared here | **126** | 31 + 29 + 21 + 14 + 12 + 5 + 4 + 3, plus the 7 guard cases |
| **Total** | **270** | |

**Trait partition of the local 126 — every case carries exactly one `Scope`, or the `Guard` key:**
`Scope=Contract` 99, `Scope=Characterization` 19, `Scope=Dispatcher` 1, `Guard=Seam` 7. By `Area`:
`Keyless` 64, `KeyPredicate` 25, `AlternateKeys` 14, `SoftDelete` 12, `Insert` 4 — and
`Keyless` + `Insert` = **68**, which is exactly the lap gate `--filter "Area=Keyless|Area=Insert"` the owner
reported passing 68/68.

**Last measured result: 2026-08-22, reported by the owner *after* lap 4 — 270 total, 259 passed, 11 failed.**
The 11 are **10 × `EFSnapshotDeepCopyTests`** — that upstream class in its entirety, since
`SnapshotDeepCopyTests` declares exactly 10 `[Fact]` — plus
`EFDataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstance`, a SQL Server
transaction-visibility timeout. **Not re-run by this pass.** **Lap 4 moved none of these figures**, which is
the expected result of deleting 24 files that nothing referenced: it fixed nothing and broke nothing.

**The `.trx` files in `ProphetsWay.EFTools.Tests/TestResults/` are stale and must not be read as current.**
`baseline`, `efseam2` and `efseam3` all record 147/53/94; `efseam` records 147/15/132; `lap2` and `final`
record 151/57/94. All six are laps from 2026-08-18, before three implementation laps and 119 new cases.

**`AlternateKeyGuardSpikeTests.cs` is now inside a gate.** Earlier passes recorded it as carrying **zero**
`[Trait]` attributes and therefore selected by no filtered invocation. That is **false as of 2026-08-22**:
all 7 theories carry `[Trait("Scope","Characterization")]` and `[Trait("Area","AlternateKeys")]`, verified
by grepping every attribute in the file. It is inside `Scope=Characterization` and correctly outside
`Scope=Contract`. Whether the file stays at all remains the owner's call
([AlternateKeyGuardSpikeTests.cs](../ProphetsWay.EFTools.Tests/AlternateKeyGuardSpikeTests.cs)).

- **What the harness does not cover — re-derived 2026-08-22, because every clause of the bullet that stood
  here is now false.** It claimed no test targets this library's own public surface, that `DepartmentDao`
  derives from no EFTools base, that `RootBaseSoftDao` and both keyless bases are exercised by nothing, that
  `CompanyResourceDao` does not exist, and that the redesign is unimplemented. **All five are dead.** The
  locally written classes tabulated above target the library directly;
  [DepartmentDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/DepartmentDao.cs) declares
  `BaseSoftPagedDao<Department, int>` and
  [CompanyResourceDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyResourceDao.cs) declares
  `RootNonIdDao<CompanyResource>` — both opened on 2026-08-22, not inherited. **What is genuinely still
  uncovered:**
  - ~~**The 24 legacy files.**~~ **CLOSED by lap 4.** This sub-item said the 18 closures, both `Root*`
    bridges, `RootDao` and the three `Legacy*` types were derived from by nothing and exercised by nothing,
    and that this was harmless because lap 4 would delete them. **It did.** There is nothing untested about
    a file that does not exist; **do not re-report this as a coverage gap.**
  - **Provider portability of the *conformance* suite.** Which provider a case runs on splits cleanly along
    the same line the case counts do, and it is not the split this document previously implied. The **144
    adapted upstream cases** reach the store through `Constants.GetExampleDataAccess`, which is
    `new ExampleDataAccess(connectionString)` against a **local SQL Server** carrying `ProphetsWay.Example`
    — one provider, one machine, and the reason `LocalTestsOnly: 'yes'` exists. The **locally written
    classes already run on SQLite in-memory**: `UseSqlite` appears in `KeylessDaoTests`,
    `KeylessSoftDaoTests`, `KeyPredicateOpenKeyTests`, `SoftDeleteTimestampHookTests`,
    `FailedInsertWriteBackTests`, `IdentifierResolutionTests` and `AlternateKeyGuardSpikeTests`, which also
    parameterizes over `UseInMemoryDatabase`; `CompanyResourceConversionTests` and `AdapterCoverageTests`
    are reflection-only and touch no store at all. **So the `Microsoft.EntityFrameworkCore.Sqlite`
    reference is in use, not merely staged** — verified 2026-08-22 by grepping every provider call in the
    test project. What [FR 11](feature-requests.md) / **D4** still owe is running the **contract** suite —
    the 144 — on both legs; today no assertion about `IExampleDataAccess` has ever executed against
    anything but SQL Server.
  - **CI.** `LocalTestsOnly: 'yes'` skips the whole suite, so none of the 270 cases runs on a build agent —
    including the SQLite ones, which need no local database and could run there today.
- **The restore mismatch is closed.** `ProphetsWay.EFTools.Tests` now targets `net10.0` alone and
  `ProphetsWay.Example.Tests` is `net48;net10.0`, so the single leg has a compatible asset
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj),
  [ProphetsWay.Example.Tests.csproj](../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj)).
- **`ProphetsWay.Example.DataAccess.EF` held the last recorded break. It is closed, and so is the caveat
  that outlived it — corrected 2026-08-22.** `ExampleDataAccess` declares
  `: BaseEFDataAccess<ExampleContext>, IExampleDataAccess` — **one type argument**; the
  `<ExampleContext, int>` this line once named does not exist. At 3.1.x that interface additionally
  aggregates `IDepartmentDao` and `ICompanyResourceDao` and inherits `IDisposable`, and **both groups are
  now written**. `NotWrittenYet` and `NotImplementedException` match **nowhere** in the project — grepped
  across every `.cs` under `ProphetsWay.Example.DataAccess.EF/` on 2026-08-22, zero hits. All seven DAO
  fields are constructed in the private `(ExampleContext, ContextOwnership)` constructor,
  `_departmentDao` and `_companyResourceDao` included; `ExampleContext` declares `DbSet<Department>` and
  `DbSet<CompanyResource>`, gives the latter `HasKey(x => new { x.CompanyId, x.ResourceId })`, and maps
  both with `ToTable`. **The sentence "it compiles; it does not conform" is superseded** — laps 2 and 3
  answered it
  ([ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  [ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs),
  [IExampleDataAccess.cs](../ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs)).
- **`Constants.cs` no longer carries any `#if` branch — corrected 2026-08-20.** The bullet that stood here
  described `NET45`–`NETCOREAPP3_1`, `NET5_0` and `NET6_0_OR_GREATER` arms and an unreachable
  `Data Source=localhost` string. The file was rewritten for the seam: it is now one nested
  `ConnectionStrings` class holding a single SQL Server connection string — `Data Source=localhost`,
  `Initial Catalog=ProphetsWay.Example`, Integrated Security, `TrustServerCertificate=True` — and one
  `GetExampleDataAccess` property returning `new ExampleDataAccess(...)` from
  `ProphetsWay.Example.DataAccess.EF`. **The connection string is live, not dead**, and the InMemory path
  is gone
  ([Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs),
  [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
- **Three of the five things this list used to call untested are now tested, and the last of the remainder
  left with lap 4.** Soft-delete bases are covered by `SoftDeleteTimestampHookTests` (12) and
  `KeylessSoftDaoTests` (29); keyless bases by `KeylessDaoTests` (31) and `CompanyResourceConversionTests`
  (3); stable `GetAll` ordering is now a property of the families themselves rather than of a test —
  `GetAll`, `GetPaged` and `GetCount` all compose through `ApplyStableOrder`, which `BaseDao` makes total
  and `RootNonIdDao` refuses to default. **The "transaction helpers on the 2.2.x DAO bridges" entry is
  closed rather than outstanding**: the `Ensure*Transaction` trio existed only on `RootBaseDao`,
  `RootBaseSoftDao`, `LegacyRootNonIdDao` and `LegacyBaseNonIdDao`, all four deleted in `d00aad3`, and a
  grep of all 15 remaining files for those three names on 2026-08-22 returns **nothing**. What is still
  untested is **the conformance suite on a second provider**, and that alone
  ([BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs), [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).
- `LocalTestsOnly: 'yes'` causes the shared pipeline to omit its `dotnet test` task entirely
  ([app-variables.yml](../app-variables.yml),
  [restore-build-test.yml](../../prophets-pipelines/steps/restore-build-test.yml)).
- CI builds `**/*.csproj` in Release, not the solution. `HasSqlProj` is commented out in
  [app-variables.yml](../app-variables.yml), so the submodule's `.sqlproj` — now SDK-style
  `Microsoft.Build.Sql/2.2.0`, not the legacy SSDT project this line previously named — is not restored or
  built. With `PostTargetToNuGet: 'yes'`, the target library is packed as alpha, beta, and release
  artifacts
  ([restore-build-test.yml](../../prophets-pipelines/steps/restore-build-test.yml),
  [ci-build.yml](../../prophets-pipelines/stages/ci-build.yml)).
- **The solution builds clean — reported by the owner on 2026-08-22, not re-measured here.**
  `dotnet build` succeeds with **0 warnings and 0 errors**. **That supersedes the 7-warning figure from the
  2026-08-16 build**, which this document carried for six days; do not restate it as current. The
  2026-08-16 run — `dotnet build ProphetsWay.EFTools.sln -c Debug`, SDK 10.0.400 — is retained here only
  because it is what closed the three breaks recorded as `AGENTS.md` Deviation 8: every project compiled,
  `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests` on `net10.0`;
  the submodule's `ProphetsWay.Example.DataAccess` and `ProphetsWay.Example.DataAccess.NoDB` on
  `netstandard2.0` and `net10.0`; `ProphetsWay.Example.Tests` on **both** `net48` and `net10.0`; and
  `ProphetsWay.Example.Database` producing `ProphetsWay.Example.Database.dacpac`. **The caveat attached to
  it then — "the new `ExampleDataAccess` DAO members are deliberately throwing stubs, and `ExampleContext`
  maps neither new entity" — is itself now superseded** by laps 2 and 3; see the `ExampleDataAccess` bullet
  above.
- **Two facts the CI pipeline never exercises were measured by that 2026-08-16 build.** The SDK-style
  `.sqlproj` **builds under the .NET CLI** — `HasSqlProj` is commented out in
  [app-variables.yml](../app-variables.yml) and CI builds `**/*.csproj`, so CI has never proved this. And
  `ProphetsWay.Example.Tests` **compiled on both legs**, which is what the upstream **164 tests / 328
  executions** figure rests on.
- **The four upstream `xUnit1013` warnings are closed — with the other three, and not by this repository.**
  The 2026-08-16 build emitted two per leg on the submodule's `DepartmentDaoTests.cs`:
  `public static void EditEveryFieldAfterTheCall` and `public static void AssertEveryStampIsUtc`, helpers
  carrying no test attribute. They belonged to `ProphetsWay.Example`, and a `Test Designer` fixes such a
  thing **there**, never from here. The owner's 2026-08-22 build reports **0 warnings** while the submodule
  pointer has not moved since 2026-08-18 — so they were fixed upstream at or before that pointer. **Do not
  re-report either these or the three `Submod` Source Link warnings as outstanding.** The record is kept
  because it settled something: `ProphetsWay.Example/docs/repo-profile.md` asserted "two `xUnit1013`
  warnings" and annotated the count as taken from a `Modernizer` build and not re-measured; it was exactly
  two, per leg, and both were identified by name
  ([DepartmentDaoTests.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/DepartmentDaoTests.cs)).
- Everything in this document other than the build and test *results* was produced with no
  command-execution tool, from checked-in source, project files and test inventory. The results are
  attributed to the owner's runs and dated where they appear.

## Real Usage Examples Found

**These are 3.x examples, not 2.x ones — corrected 2026-08-22.** The bullet that stood here named
`Int.BasePagedDao<Company>`, `Guid.BaseGetAllDao<Resource>` and `Long.BasePagedDao<Transaction>` and closed
by calling the set "current 2.x examples, not the approved provider-neutral 3.x API." **Every one of those
type names is wrong for the tree**, and the closing sentence is backwards: the proving ground was moved onto
the open-key families in laps 1–3 and is now the best available worked example of the approved surface.
Every derivation below was read from the file cited on 2026-08-22.

1. **Derive the application context from `BaseEFContext` and expose typed `DbSet` properties.**
   `ExampleContext` has **one constructor**, `ExampleContext(DbContextOptions<ExampleContext> options)`, and
   names no provider — its `<remarks>` say so explicitly, which is what lets a test point it at a database
   it controls. `OnModelCreating` takes EF Core's `ModelBuilder`, gives the keyless `CompanyResource` a
   composite `HasKey(x => new { x.CompanyId, x.ResourceId })`, and maps all seven entities with `ToTable`
   ([ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs)).
2. **Derive each DAO from the open-key family matching its capability**, passing the key type as a second
   type argument. All seven, verbatim from the tree:

   | DAO | Declaration |
   | --- | --- |
   | `CompanyDao` | `BasePagedDao<Company, int>, ICompanyDao` |
   | `JobDao` | `BaseGetAllDao<Job, int>, IJobDao` |
   | `ResourceDao` | `BaseGetAllDao<Resource, Guid>, IResourceDao` |
   | `TransactionDao` | `BasePagedDao<Transaction, long>, ITransactionDao` |
   | `UserDao` | `BaseDao<User, int>, IUserDao` |
   | `DepartmentDao` | `BaseSoftPagedDao<Department, int>, IDepartmentDao` |
   | `CompanyResourceDao` | `RootNonIdDao<CompanyResource>, ICompanyResourceDao` |

   `protected DbSet<TEntity> Dataset` survives on both `BaseDao` and `RootNonIdDao`, so a custom method
   composes over it directly — `CompanyDao.GetCustomCompanyFunction` is
   `Dataset.OrderBy(x => x.Id).Skip(id % GetCount(null)).First()`
   ([CompanyDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyDao.cs),
   [BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs),
   [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).
3. **The two keyless overrides, which are the clearest thing in the proving ground.** `CompanyResourceDao`
   writes exactly two members and inherits the rest: `MatchRow` — `x => x.CompanyId == item.CompanyId &&
   x.ResourceId == item.ResourceId` — because it is the library's only abstract member; and
   `ApplyStableOrder`, because the keyless default throws until it is overridden and `GetAll` needs it. Its
   `Insert` override is a third, and its `<remarks>` explain why it is the DAO's contract rather than the
   library's: rule 3's silent no-op on a pair the store already holds
   ([CompanyResourceDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyResourceDao.cs)).
4. **Derive the DAL from `BaseEFDataAccess<ExampleContext>` — one type argument — and state ownership.**
   `ExampleDataAccess` funnels three public constructors into one private
   `(ExampleContext, ContextOwnership)`: a connection-string one that calls `UseSqlServer` **in the
   consumer's own file** and takes `Owned`; an options one that takes `Owned`; and a context one that takes
   `Borrowed` for a container or a test that built its own. Each DAO is constructed with the shared
   `protected Context`, and each interface member forwards to it
   ([ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
5. **A worked custom soft-delete member.** `DepartmentDao.Restore` is the clearest use of a `protected` seam
   in the tree: it starts from `TrackForWrite(item)` rather than a read, precisely so `ApplyReadFilter` —
   which excludes exactly the deleted rows `Restore` exists to reach — stays off the query, and it detaches
   in a `finally` on all three exits ([DepartmentDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/DepartmentDao.cs)).

**What the proving ground does *not* show, and a README must not copy from it:** there is no
`BaseEFContext(string)`, no `BaseEFDataAccess(string)` or `BaseEFDataAccess(DbContextOptions)`, no
parameterless in-memory default, and no `ProphetsWay.EFTools.Int` / `.Guid` / `.Long` using-directive
anywhere in it. Five of its seven DAO files do still open with dead `#if NET8_0_OR_GREATER` /
`#if NET471 || NET48` blocks — `CompanyDao`, `JobDao`, `ResourceDao`, `TransactionDao`, `UserDao` — which
are inert under `net10.0` and are noise a reader should not copy.

## README Accuracy Check

**Rewritten 2026-08-22 against the file itself.** [README.md](../README.md) was opened end to end for this
pass, not inherited from an earlier one. It predates **2.2.0** — it teaches nothing that release added — and
it documents the **pre-3.x** surface throughout. **It is not a stale README that needs freshening: its two
central code samples name types that do not exist, so a reader who follows it does not get a working
project.** The two are `BaseEFDataAccess<TContextType, TIdType>` and `BaseEFContext(string)`.

`README Author` should treat everything marked **Does not exist** below as a rewrite obligation rather than
an edit, and should note that the *published* 2.2.0 package still matches most of the *rest* of this README —
so the rewrite must say which line it describes.

| Existing claim | Verdict | Evidence |
| --- | --- | --- |
| The package reduces repetitive DAO CRUD under the BaseDataAccess paradigm | **Accurate**, and still the right lead | The product and the proving ground both implement exactly that |
| “Please see the example projects included in the GitHub repository” | **Misleading** | They are not included together: four projects come from the `ProphetsWay.Example` **git submodule** and one, `ProphetsWay.Example.DataAccess.EF`, is owned by this repository. A fresh clone without `--recurse-submodules` has none of the four |
| “Three value types you can use for a Primary Key: int, long, and Guid… three separate namespaces in EFTools… This is required so the default Get method can build a proper select by Id” | **Wrong for the tree, and the *reason* given is wrong too** | `BaseDao<TEntity, TKey>` places **no constraint on `TKey`**, so `string` and `int?` are legal, and the identifier is resolved by name — `{TypeName}Id`, then `Id`. **The three namespaces no longer exist at all** — deleted in `d00aad3` ([BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs), [CHANGELOG.md](../CHANGELOG.md)) |
| `BaseDao` publishes `T Get(T item)`, `void Insert(T item)`, `int Update(T item)`, `int Delete(T item)` | **Accurate** | All four are on `BaseDao<TEntity, TKey>` |
| Prose says “In the case of `T Get(T item);` and `T Delete(T item);`” | **Wrong** | `Delete` returns `int`. The README's own code block two paragraphs earlier says `int Delete(T item);`, so it contradicts itself |
| `BaseDao` publishes `EnsureBeginTransaction` / `EnsureTransactionCommit` / `EnsureTransactionRollback` | **Does not exist anywhere in the library** | Grepped all 15 `.cs` files on 2026-08-22, after lap 4: the trio matches **nothing**. It lived only on `RootBaseDao`, `RootBaseSoftDao`, `LegacyRootNonIdDao` and `LegacyBaseNonIdDao`, all deleted in `d00aad3`. Transactions are the DAL root's, not the DAO's. Also carries the [FR 12](feature-requests.md) defect |
| `using ProphetsWay.EFTools.Int;` then `UserDao : BaseDao<User>`, `JobDao : BaseGetAllDao<Job>`, `CompanyDao : BasePagedDao<Company>` | **All three samples are wrong** | The tree writes `BaseDao<User, int>`, `BaseGetAllDao<Job, int>`, `BasePagedDao<Company, int>` from the **root** namespace, with no key-specific `using`. See *Real Usage Examples Found* |
| `CompanyDao.GetCustomCompanyFunction` composing over `Dataset` | **Accurate**, and worth keeping | `protected DbSet<TEntity> Dataset` survives on both `BaseDao` and `RootNonIdDao`, and the proving ground's method is byte-identical to the README's |
| **`BaseEFContext` “is defined so that you always pass the connection string via its constructor”**, sampled as `ExampleContext(string nameOrConnectionString) : base(nameOrConnectionString)` | **Does not exist — the sample cannot compile** | [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) is 24 lines with **one** member: `protected BaseEFContext(DbContextOptions builderOptions)`. The string constructor was removed, and with it the `UseSqlServer` call it made on the consumer's behalf ([CHANGELOG.md](../CHANGELOG.md) § *BaseEFContext no longer takes a connection string*) |
| “It is used so that the next base class can properly instantiate your specific 'Typed' context” | **Wrong** | Nothing in the library instantiates a context. `BaseEFDataAccess<TContext>` receives one already built |
| Deriving from `BaseEFContext` is required | **Wrong** | It is optional. `BaseEFDataAccess<TContext>` constrains to `DbContext`, which its `<typeparam>` says is deliberate so an existing DI-registered context need not be re-parented |
| Context sample overrides `OnModelCreating(DbModelBuilder modelBuilder)` | **Wrong — will not compile** | `DbModelBuilder` is EF6; the sample's body uses EF Core's `HasOne`/`WithMany`/`HasConversion`. `ExampleContext` correctly takes `ModelBuilder` |
| **`BaseEFDataAccess<TContextType, TIdType>`** — the README's own section heading | **Does not exist** | The type is `BaseEFDataAccess<TContext>`, **one** parameter. `CHANGELOG.md` states the identifier parameter “was never used for anything the class did” ([BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs)) |
| “This base class will create your `DbContext` automatically when constructed as defined by `TContextType`” | **Wrong, and it is the load-bearing error** | The one constructor is `protected BaseEFDataAccess(TContext context, ContextOwnership ownership)` and its `<remarks>` say it “has no side effect beyond capturing the two values — no query, no connection, no transaction.” Both public constructors are gone |
| “By passing it `TIdType` it will give you access to `yourDAL.Get<T>(idValue)`” | **Half right, wrong cause** | `Get<T>(id)` still works, but it is inherited from `ProphetsWay.BaseDataAccess`'s reflection dispatcher, which `BaseEFDataAccess<TContext>` derives from — no type parameter confers it |
| DAL sample: parameterless constructor calling `UseInMemoryDatabase`; `ExampleDataAccess(string)` calling `UseSqlServer`; `ExampleDataAccess(DbContextOptions options) : base(options)` | **First and third are wrong; the second is right for the wrong reason** | There is no in-memory default — `CHANGELOG.md` records its removal. `base(options)` does not exist. The connection-string constructor survives **in the consumer's own file**, which is the point the README should be making and does not: the library names no provider |
| “The library has 35 unit tests” | **Stale by an order of magnitude** | **270** discoverable cases; last reported 259 passed / 11 failed on 2026-08-22. See *Tests and Build Facts* |
| “Each set of tests is a new class inheriting from each original test class, but overriding a property that returns an instance of a particular interface” | **The first half is right, the mechanism is wrong** | The 13 adapters are literally `public class EFXxxTests : XxxTests { }` and **override nothing**. The redirect is `TestSeam.cs` — a `[ModuleInitializer]` calling `TestDataAccessFactory.Use` once for the whole assembly |
| “I don't have the unit tests running in the build pipeline because they are actually hitting a local database” | **Accurate, and literally so** | `LocalTestsOnly: 'yes'` removes the `dotnet test` task, and `Constants.cs` holds a live `Data Source=localhost` SQL Server string |
| Soft delete, keyless DAOs, `ContextOwnership`, disposal, DAL transactions, provider neutrality | **Absent** | Ten DAO base classes — six keyed families and four keyless ones — one enum, and the entire disposal and transaction contract are undocumented. `Dispose` is `sealed`, idempotent, non-throwing, rolls back an open transaction and disposes the context only when `Owned`; all 10 non-`Dispose` members throw `ObjectDisposedException` once disposed. The README teaches none of it |
| Versioning / Authors / License sections | **Accurate** | SemVer, MIT, and the tag link are all correct |

**One thing the README gets right that the rewrite must not lose:** its explanation of *why* every CRUD
method takes the entity as its argument — so one DAL can publish one method name per operation and let the
argument type select the DAO. That paragraph is the clearest statement of the paradigm anywhere in the repo.


## Planned 3.x Direction — Approved, and Where Each Item Now Stands

**This section's heading used to end "Approved, Not Implemented", and that is no longer true — rewritten
2026-08-22, then corrected the same day for lap 4.** The approved direction has now landed across
implementation laps 1–4. What is left is **provider *packaging*, the certification legs, a green suite, and
the tag** — lap 4 is no longer on that list. Each row's state was taken from the file named, on 2026-08-22,
after `d00aad3`.

| Approved item | State | Evidence |
| --- | --- | --- |
| **D1 — EF Core only**; 2.2.x remains the EF6 / .NET Framework answer, and no EF6 companion package is built | **Done, and the residue is gone too.** No `PackageReference` to `EntityFramework` exists anywhere, and **there is no conditionally compiled code left in the library at all** — the dead `#if NET461 \|\| NET471 \|\| NET48` blocks carrying `using System.Data.Entity;` lived only in the 24 files `d00aad3` deleted. **Do not restate "dead but not deleted" or "still appears in 24 library files"** | Grepped all 15 files for every preprocessor directive 2026-08-22 — zero hits; [CHANGELOG.md](../CHANGELOG.md) |
| **D2 / [FR 7](feature-requests.md) — relational-provider-neutral** | **Half done, and the halves are worth separating.** **Code: done.** `UseSqlServer` and `UseInMemoryDatabase` match nothing under `ProphetsWay.EFTools/`; `BaseEFContext` names no provider and `BaseEFDataAccess<TContext>` takes a built context. **Packaging: not done.** `Microsoft.EntityFrameworkCore.SqlServer` and `.InMemory` 10.0.11 are still `PackageReference`s of the library, so a PostgreSQL consumer restores two providers they will never use | [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs), [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) |
| **D3 / [FR 10](feature-requests.md) — six generic root-namespace DAO families replacing the 18 closures, no compatibility wrappers** | **Done, both halves.** **Do not restate this as "Not started", and do not restate "the 18 have not been deleted."** `BaseDao` / `BaseGetAllDao` / `BasePagedDao` and the three `BaseSoft*` counterparts are all in the tree with an **unconstrained `TKey`**, and four keyless types were added beyond the approved six. Lap 4 (`d00aad3`) then removed the 18 closures, both `Root*` bridges, `RootDao` and the three `Legacy*` types. **S3 held — nothing transitional shipped** | [api-contract.md](api-contract.md) § *The Public Surface*; the *Public API Surface* tables above |
| **D4 / [FR 11](feature-requests.md) — SQLite in-memory as the fast CI leg, SQL Server container for provider fidelity** | **Started, and further along than "not started" — corrected 2026-08-22.** Seven of the eight locally written test classes already stand up **SQLite in-memory** contexts, so the library's own surface is exercised on a second relational provider today. **The conformance suite is not**: all 144 adapted upstream cases run against a local SQL Server through `Constants.GetExampleDataAccess`, and `LocalTestsOnly: 'yes'` keeps every leg out of CI. What is owed is the *contract* suite on both providers and a container to host the SQL Server one | Grepped every `UseSqlite` / `UseInMemoryDatabase` / `UseSqlServer` in the test project 2026-08-22; [Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs), [app-variables.yml](../app-variables.yml) |
| **D7 — `net10.0` only** for the library, the tests and the EF proving ground | **Done 2026-08-16.** A **ratified exception** to the house `netstandard2.0;net10.0` standard, not drift | All three `.csproj` files; [purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15) |
| **[FR 2](feature-requests.md) — move to `ProphetsWay.BaseDataAccess` 3.1.0** | **Done 2026-08-16** in both consuming projects | [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) |
| **[FR 3](feature-requests.md) — implement the 3.x disposal contract** | **Done, including the half this document twice recorded as open.** `Dispose` is `sealed override`, idempotent, non-throwing, rolls back an open transaction and disposes the context only under `Owned`, with `protected virtual DisposeCore()` as the derived hook. **All 10 non-`Dispose` members open with `ThrowIfDisposed()`** — the three transaction members and the seven dispatcher overrides — and `ThrowIfDisposed` is `protected` so a derived layer guards its own forwarders. **The sentence "`ObjectDisposedException` guarding on the transaction members has not [been implemented]" is dead** | [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), verified by grep — the guard is declared once and called at ten sites |
| **[FR 1](feature-requests.md) — advance the `ProphetsWay.Example` submodule** | **Done 2026-08-16, advanced again 2026-08-18.** The pointer is `61d9e7d`, one commit past the `3.1.0` tag, on the open `3.1.1` line | `.git/modules/ProphetsWay.Example/HEAD` |
| **[FR 6](feature-requests.md) — rebuild the test project on the 3.x seam** | **Done, in a shape the original request did not anticipate.** The six old adapters were deleted 2026-08-16 and **superseded the same night**: thirteen new adapters plus `TestSeam.cs` replaced them once `TestDataAccessFactory.Use` landed upstream, per owner decision **D10** shape B. The Entity Framework conformance run they stood in for **now happens** | [TestSeam.cs](../ProphetsWay.EFTools.Tests/TestSeam.cs) |
| **[FR 8](feature-requests.md) — remove FluentAssertions**, **[FR 9](feature-requests.md) — delete the stray `[submodule "Submod"]` block** | **Both are done in the tree as of 2026-08-16** — the reference is gone from the EF proving ground's `.csproj`, and `.gitmodules` holds one well-formed block. **Their entries still read `Scheduled`; only `Purpose Refiner` may close a status**, so this row states the tree, not the triage | [ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj), [.gitmodules](../.gitmodules) |

**What remains before 3.0.0 can be tagged**, stated as a list rather than left implicit. **Lap 4 was item 1
and is struck; the version decision was item 5 and is struck. Neither is outstanding.**

1. ~~**Lap 4 — delete the 24 legacy files** and the dead `#if` blocks that live in them.~~ **DONE —
   `d00aad3`.** 24 files, 773 lines, no modification to any surviving file. The `#if` blocks left with the
   files that held them.
2. **Remove the two provider `PackageReference`s** — the packaging half of D2 / FR 7. Still open; lap 4 did
   not touch [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj), re-opened
   2026-08-22 to confirm.
3. **Build the two certification legs** — D4 / FR 11. What is owed is the *contract* suite on both
   providers; SQLite is already in real use by the local classes.
4. **Get the suite green** — 11 failures remain at the last reported run, unchanged by lap 4.
5. ~~**The version decision.**~~ **CLOSED by the owner.** `app-variables.yml` reads `3` / `0` / `0` — opened
   2026-08-22. What remains is mechanical rather than a decision: **the tag and the publish have not
   happened, and nuget.org still serves 2.2.0.**

**FR 15 is closed as a side effect of lap 4.** `ProphetsWay.EFTools.Guid` shadowed `System.Guid` in any file
importing it; the namespace no longer exists, so the collision cannot occur.
[docs/feature-requests.md](feature-requests.md) is **not** amended to say so — only `Purpose Refiner` may
change an entry's status. Recorded here so it is not re-derived.

The direction was supplied by the owner on 2026-08-15 as D1–D6 and extended by D7–D19; see
[purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15). Current-state contrasts are
verified in [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), [.gitmodules](../.gitmodules), and
[ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj).

## Gaps & Observations

**Rewritten 2026-08-22. The numbering is preserved because the Reading Note above cites items by number —
findings are marked closed with their evidence rather than deleted.**

1. **CLOSED — the 3.x disposal contract is fully implemented.** The reference moved to 3.1.0 on 2026-08-16
   and `Dispose` landed with it: `sealed override`, idempotent, swallowing a failed rollback, disposing the
   context **only** under `ContextOwnership.Owned`, with `protected virtual DisposeCore()` for a derived
   layer. **The open half this item recorded twice — "`_disposed` is set but never consulted outside
   `Dispose` itself" — is dead.** `ThrowIfDisposed()` is declared once and called at **ten** sites, one per
   non-`Dispose` member: `TransactionStart`, `TransactionCommit`, `TransactionRollBack`, `GetAll`,
   `GetPaged`, `GetCount`, `Get`, `Insert`, `Update`, `Delete`. Verified by grepping
   [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) on 2026-08-22, not inherited.
   [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) has nothing left in
   this repository.
2. **Historical, and about the published package only.** The shipped 2.2.0 EF6 and EF Core assets differ
   semantically on missing-row update and structurally on available constructors, so one package does not
   expose one uniform contract. **No build in the tree produces an EF6 asset**, and since `d00aad3` the
   branches do not survive anywhere — the files that carried them are deleted and **zero preprocessor
   directives remain library-wide**. It matters to a consumer reading nuget.org today, which still serves
   2.2.0, and to nobody reading this tree.
3. **Narrowed to packaging — the code half closed.** `UseSqlServer` and `UseInMemoryDatabase` match nothing
   under `ProphetsWay.EFTools/`. What leaks is two `PackageReference` entries. **Read this together with
   the *Dependencies* section, or you will conclude the library still picks your provider** — it does not;
   it merely makes you restore two you did not ask for. [FR 7](feature-requests.md), **D2**.
4. **CLOSED.** This item read "`GetPaged` orders by ID, but `GetAll` does not; soft paging has no explicit
   ordering either." That described the internal `RootDao<T, TIdType>`, where `GetAll` was
   `Dataset.ToList()` and `GetPaged` was `Dataset.OrderBy(x => x.Id).Skip(skip).Take(take)`. **That file was
   deleted in `d00aad3`, so the item now describes nothing in the tree.** In the surviving families **all
   three reads compose through the same `ApplyStableOrder`**: `BaseDao` defaults it to the key followed by a
   `ThenBy` over every primary-key property in the model, and `RootNonIdDao` throws `NotSupportedException`
   until a keyless DAO supplies one — which is why `CompanyResourceDao` writes it even though its own rule 5
   promises no order
   ([BaseDao.cs](../ProphetsWay.EFTools/BaseDao.cs), [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).
5. **NARROWED — there is a suite, it targets this library directly, and the specific gaps are named.** This
   item has now been wrong twice in opposite directions: it first said there was no test suite at all, then
   that 151 cases existed but "nothing tests this library's own public surface" and `CompanyResourceDao`
   "does not exist and accounts for most of the red." **All of that is superseded.** 270 cases; **126 of
   them are declared in this repository against `BaseDao`, `BaseSoftDao`, `RootNonIdDao`,
   `RootSoftNonIdDao` and `BaseEFDataAccess` directly**, in **8** locally written classes — not 7, as three
   earlier sentences said against their own eight-row table; `CompanyResourceDao` exists on
   `RootNonIdDao<CompanyResource>`. What is genuinely uncovered: **the conformance suite on a second
   provider** — the 144 adapted cases run against a local SQL Server alone, even though the local classes
   already run on SQLite in-memory ([FR 11](feature-requests.md)) — and **CI** (`LocalTestsOnly: 'yes'`
   skips all 270). **The third item this list used to name — the 24 legacy files — is gone with `d00aad3`
   and is not a gap.** **11 failures remain** at the last reported run, unchanged by lap 4 — 10 ×
   `EFSnapshotDeepCopyTests` plus
   `EFDataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstance`.
6. **Packaging lacks homepage, tags, SourceLink, symbols and CI build metadata.** **Unchanged, and the
   Source Link half needs both of its facts or it reads wrong.** The malformed `[submodule "Submod"]` block
   is gone and its warning with it — the owner's 2026-08-22 build reports 0 warnings where the 2026-08-16
   one emitted it three times. **That stopped the warning; it did not give the library Source Link.** No
   SourceLink package reference exists and none of `PublishRepositoryUrl`, `EmbedUntrackedSources`,
   `IncludeSymbols`, `SymbolPackageFormat` or `ContinuousIntegrationBuild` is present. See the Packaging
   Audit for the exact snippets. `RepositoryType` is also `GitHub` where the house standard is `git`.
7. **NARROWED further by lap 4 — the documentation gap has inverted, and only the build-level one is left.**
   This item said XML documentation exists on the 18 key-specific leaves "but not on the principal context,
   DAL, root bridge, or keyless public API." **Three of those four are now among the best-documented files
   in the workspace** — `BaseEFContext`, `BaseEFDataAccess<TContext>` and `RootNonIdDao` all carry
   `<summary>`, `<typeparam>`, `<exception>` and multi-paragraph `<remarks>` that state contract rather than
   restating signatures. **The fourth — the undocumented 2.2.x residue, `RootBaseDao` and `RootBaseSoftDao`
   carrying only `[EditorBrowsable(EditorBrowsableState.Never)]` — was deleted in `d00aad3`, so it is not a
   gap either.** **The one live gap is that none of the documentation is shipped** —
   `GenerateDocumentationFile` is set nowhere in
   [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj), re-opened 2026-08-22, so
   no `.xml` file is produced or packed and a consumer gets no IntelliSense from any of it.
8. `docs/architecture.md`, per-project `docs/requirements.md`, and `docs/nuget-extraction-proposal.md` are
   **n/a by owner decision** (**D5**). They are not documentation gaps.
9. **NEW 2026-08-22 — the proving ground guards only its two newest forwarder groups.** In
   [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs), `ThrowIfDisposed()`
   appears at eleven sites, all of them in the `IDepartmentDao` (8) and `ICompanyResourceDao` (3) groups
   added in laps 2 and 3. The five original groups — Company, Job, User, Resource, Transaction — forward
   without it. `BaseEFDataAccess.ThrowIfDisposed`'s own `<remarks>` say a derived layer **must** call it as
   the first statement of every member it declares, and describe exactly this masking: the test harness
   constructs through `new ExampleDataAccess(connectionString)`, which is `ContextOwnership.Owned`, so the
   disposed context throws of its own accord and the omission is invisible. Under `Borrowed` those five
   groups would silently succeed after disposal. **This is not currently causing a failure** — it is an
   inconsistency inside one file, in the artifact whose job is to be copied.
10. ~~**NEW 2026-08-22 — recorded, not resolved.** `app-variables.yml` reads `Major` `2` / `Minor` `2` /
    `Patch` `0` against a tree carrying the 3.0.0 surface.~~ **CLOSED THE SAME DAY — the owner took the
    bump.** [app-variables.yml](../app-variables.yml) was re-opened on 2026-08-22 and reads `Major: '3'` /
    `Minor: '0'` / `Patch: '0'`. It now agrees with a `CHANGELOG.md` whose top entry is `v3.0.0` and with an
    `api-contract.md` specifying 3.0.0. **The item is kept rather than deleted so the record of the
    mismatch survives; do not restate `2` / `2` / `0`, and do not re-file this as an open decision.** What
    is still true and is **not** a decision: **3.0.0 is set but not yet tagged or published, so nuget.org
    still serves 2.2.0** and a consumer reading the listing meets the EF6-and-`net4x` package. **No agent
    may change `Major`/`Minor`/`Patch` under any circumstances.**
11. **NEW 2026-08-22, after lap 4 — a vestigial condition on the EF Core `ItemGroup`.** The three EF Core
    `PackageReference`s in
    [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) sit inside
    `Condition="!$(TargetFramework.StartsWith('net4')) and $(TargetFramework.StartsWith('net'))"`, which is
    unconditionally true given the single `net10.0` target. It is a leftover of the EF6/EF Core split and
    now gates nothing. **Harmless, and not urgent** — but it is the last structural trace of the EF6 branch
    outside the `<Description>`, and whoever removes the two provider references for D2 / FR 7 will be
    editing that exact `ItemGroup` anyway. Recorded so it is done once rather than twice. `Modernizer`'s
    territory, not this document's.

## Open Questions for the Owner

**The three questions this section carried have all been answered — kept with their answers rather than
deleted, so they are not re-asked.**

| # | Question | Status |
| --- | --- | --- |
| 1 | Should EF Core-only 3.x target `net10.0` alone, given that current EF Core cannot supply the usual `netstandard2.0` reach-floor asset? | **Answered — D7.** `net10.0` alone, ratified as an exception to the house standard. All three projects match it as of 2026-08-16 |
| 2 | Will 3.x accept caller-owned contexts as well as creating them, and therefore track disposal ownership? | **Answered, and implemented.** `ContextOwnership` is a two-value enum with **no default to fall into**, and the constructor rejects anything else with `ArgumentOutOfRangeException`. `Dispose` disposes the context only under `Owned` |
| 3 | Should package metadata claim "relational-provider-neutral, certified on SQLite and SQL Server", or keep certification detail in repository documentation? | **Answered — D8.** The certification scope is a public claim. It **must not imply any other relational provider is certified**, and it cannot honestly be made until [FR 11](feature-requests.md) builds the two legs |

**What is actually open:**

1. ~~**The version line.**~~ **CLOSED — the owner set `app-variables.yml` to `3` / `0` / `0`**, opened
   2026-08-22. See Gaps 10. **What replaces it is not a question but a pending action: 3.0.0 is not yet
   tagged and not yet published**, so nuget.org still serves 2.2.0.
2. ~~**Lap 4's trigger.**~~ **CLOSED — lap 4 has landed** as `d00aad3`, "Delete the 2.2.x DAO surface
   superseded by the open-key families," which is HEAD. It was simply next; nothing gated it.
3. **Does `AlternateKeyGuardSpikeTests.cs` stay?** It is correctly traited now and sits inside
   `Scope=Characterization`, but its own `<remarks>` call it "an empirical spike, not a specification," and
   it asserts about EF Core rather than about this library. Keeping it is defensible; it is a judgement
   this document should not make.
4. **The 11 remaining failures.** 10 are `EFSnapshotDeepCopyTests` in its entirety, which points at
   [FR 14](feature-requests.md) — the SNAPSHOT-rule finding — rather than at ten separate defects. The
   eleventh is `EFDataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstance`. Is closing
   FR 14 a 3.0.0 blocker, or does 3.0.0 ship with the deviation documented?
5. **When is 3.0.0 tagged and published?** Everything the tag was waiting on except the suite is done: the
   surface is complete, the deletion has landed, the version is set. Outstanding before or alongside it are
   the provider `PackageReference` removal (D2 / FR 7), the certification legs (D4 / FR 11), and the 11
   failures. Which of those are release blockers is the owner's call, not this document's.
