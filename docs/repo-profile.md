# Repo Profile — ProphetsWay.EFTools

_Generated 2026-08-15. Evidence-based; every claim cites a source file._

_Re-verified 2026-08-16 by `Repo Analyst` against the tree. **The `ProphetsWay.Example` submodule pointer
was advanced to `d845863` — 3.1.0 — between the two passes**, which invalidated every claim in this
document that described the submodule as pinned pre-3.0.0. Those rows are corrected below and marked. No
EFTools-owned `.cs` or `.csproj` changed between the passes._

_**Corrected again later on 2026-08-16**, against a tree in which the six `ProphetsWay.EFTools.Tests`
adapter classes have been **deleted**, and all three EFTools-owned `.csproj` files have been retargeted to
**`net10.0` alone**, moved to `ProphetsWay.BaseDataAccess` **3.1.0**, and stripped of their **EF6** and
**FluentAssertions** references. The paragraph above saying no EFTools-owned `.cs` or `.csproj` had changed
describes the earlier pass and is retained as history. **No EF Core package version is stated anywhere in
this document — the bump was mid-flight when this pass ran, and an unverifiable number is worse than
none.**_

_**Concurrency warning.** `Implementer` was editing `.cs` files and `Modernizer` was editing `.csproj`
files while this pass ran. Every statement below about C# source is a reading taken at that moment and
cited to the file; re-open the file rather than trusting the sentence. No build or test was run — this
agent has no command-execution tool — so **no compile outcome is claimed anywhere in this document**._

## One-Line Purpose

The **published 2.2.0** package supplies EF6 and EF Core base classes that implement the repetitive CRUD,
paging, soft-delete, context, and transaction plumbing behind a `ProphetsWay.BaseDataAccess` 2.5.0 DAL
([RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs),
[RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs)). **The working tree no longer matches that
sentence in three respects** — every project now references `ProphetsWay.BaseDataAccess` **3.1.0** and
targets **`net10.0`** alone, nothing in the repository references EF6, and `BaseEFDataAccess` now
implements the 3.x `Dispose`
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs)).

## What It Actually Does

- **EF Core only, as of the 2026-08-16 retarget.** The EF6 conditional `ItemGroup` is gone from every
  `.csproj`, so **no project in this repository references `EntityFramework` 6.5.1 any more**. The
  `#if NET461 || NET471 || NET48` branches still stand in the C# sources and are dead code under a
  `net10.0`-only build; removing them belongs to `Implementer`
  ([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
  [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
  [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs)).
  The EF6-versus-EF Core behavioural split described below is therefore a statement about the **published
  2.2.0 package and the surviving source branches**, not about what a `net10.0` build compiles.
- Exposes one `DbContext`, DAO CRUD methods, `GetAll`, `GetCount`, ordered `GetPaged`, and three
  transaction helpers. Every write immediately calls `SaveChanges`
  ([RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs),
  [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs)).
- Supplies `Guid`, `int`, and `long` public DAO families. Each family has CRUD, get-all, paged,
  soft-delete CRUD, soft-delete get-all, and soft-delete paged bases
  ([Guid/BaseDao.cs](../ProphetsWay.EFTools/Guid/BaseDao.cs),
  [Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs),
  [Long/BaseDao.cs](../ProphetsWay.EFTools/Long/BaseDao.cs)).
- Supports entities without a simple ID through keyless abstract bases where the consumer supplies
  `Get` and `Update`; the EF Core soft variant additionally requires a tracked lookup
  ([BaseNonIdDao.cs](../ProphetsWay.EFTools/BaseNonIdDao.cs),
  [BaseSoftNonIdDao.cs](../ProphetsWay.EFTools/BaseSoftNonIdDao.cs)).
- Implements soft delete by assigning created/updated/deleted timestamps and filtering null
  `DeletedDate` values. Soft paging is not explicitly ordered
  ([RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs)).
- Ships materially different implementations under one package: EF6 `Update` uses `AddOrUpdate`
  (upsert), while EF Core loads an existing row with `Single`, copies values, and marks it modified
  ([RootDao.cs](../ProphetsWay.EFTools/RootDao.cs)).
- On EF Core, the context string constructor hardcodes `UseSqlServer`; the runtime package also
  carries the InMemory provider
  ([BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs),
  [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj)).

## Current Inventory

The solution contains seven build projects plus two solution folders. Three projects are owned by this
repository and four come from the `ProphetsWay.Example` git submodule
([ProphetsWay.EFTools.sln](../ProphetsWay.EFTools.sln), [.gitmodules](../.gitmodules)).

| Project | Type | Role |
| --- | --- | --- |
| `ProphetsWay.EFTools` | Packable library | Published product; 26 source files, 24 public classes (23 abstract) |
| `ProphetsWay.Example.DataAccess.EF` | Library/example | EF implementation of the Example contracts; 7 source files. Being brought onto the 3.1.0 contracts by `Implementer` as this pass ran |
| `ProphetsWay.EFTools.Tests` | xUnit project | **`Constants.cs` and the `.csproj`, and nothing else.** The six adapter classes were **deleted 2026-08-16**; the project now contains **no tests**, which is correct and temporary — see below |
| `ProphetsWay.Example.DataAccess` | Submodule library | **3.1.0** entities and DAL contracts, including `Department` and `CompanyResource` |
| `ProphetsWay.Example.DataAccess.NoDB` | Submodule library | In-memory implementation; the one `TestDataAccessFactory` returns |
| `ProphetsWay.Example.Tests` | Submodule xUnit project | The 3.1.0 suite. The six classes the deleted adapters used to derive from hold **35** `[Fact]` methods between them — **parked, not lost** |
| `ProphetsWay.Example.Database` | Submodule database project | **SDK-style `Microsoft.Build.Sql/2.2.0`** as of the 3.1.0 pointer — not the legacy SSDT format this row previously named |

`ProphetsWay.Example` is a submodule at `path = ProphetsWay.Example`, tracking the standalone repository's
`main` branch. It is not vendored and must not be edited from this repository
([.gitmodules](../.gitmodules)). The extra `[submodule "Submod"]` declaration has no path or URL.
**The pointer is at `d845863`** — read from `.git/modules/ProphetsWay.Example/HEAD` and corroborated by the
checked-out tree, whose `app-variables.yml` reads `3` / `1` / `0`.

## Public API Surface

Internal `RootDao<T,TIdType>` and `RootNonIdDao<T>` perform the work but are not public API
([RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).

**The `Tested?` column below describes the *parked upstream* suite, not tests presently in this
repository.** Since the six adapters were deleted on 2026-08-16, `ProphetsWay.EFTools.Tests` contains no
tests at all, so the honest current answer for **every** row is "not exercised here." The column is kept
because it records which behaviours the 35 parked tests would cover once the shape B seam exists, and
losing that would lose the coverage analysis the redesign needs.

| Type | Public/protected members | Purpose | Tested? |
| --- | --- | --- | --- |
| `BaseEFContext` | protected string constructor; EF Core-only protected `DbContextOptions` constructor | Common typed context base; string path chooses SQL Server on Core | Indirect construction only |
| `BaseEFDataAccess<TContextType,TIdType>` | string constructor; Core-only options constructor; `TransactionStart`, `TransactionCommit`, `TransactionRollBack`; **`Dispose`**; protected `Context` | Instantiates a typed context, implements DAL transactions, and — **as of 2026-08-16** — the 3.x disposal contract | Not exercised here |
| `RootBaseDao<T,TIdType>` | `Context`, `Dataset`, `Get`, `Insert`, `Update`, `Delete`, `GetAll`, `GetCount`, `GetPaged`, three `Ensure*Transaction` methods | Public bridge over the internal DAO engine; hidden from IntelliSense | CRUD/read paths yes; helpers no |
| `RootBaseSoftDao<T,TIdType>` | timestamped/filtering replacements for insert/update/delete/all/count/paged; protected `UseUtcTime` | Soft-delete bridge | No |
| `BaseNonIdDao<T>` | `Context`, `Dataset`, abstract `Get`/`Update`, insert/delete, three transaction helpers | Extension point for entities without a simple ID | No |
| `BaseSoftNonIdDao<T>` | soft insert/update/delete; Core-only `Get(item, asTracking)` | Soft-delete extension for composite/keyless entities | No |
| `Guid.BaseDao<T>`, `Int.BaseDao<T>`, `Long.BaseDao<T>` | protected constructor; `Get` override; inherited CRUD/read/helper members | Key-specific simple DAO | Yes for all three key types |
| Three `BaseGetAllDao<T>` types | protected constructor; inherited `GetAll` | Key-specific full-list DAO | `int` and `Guid` only |
| Three `BasePagedDao<T>` types | protected constructor; inherited count/paging | Key-specific paged DAO | `int` and `long` only |
| Three `BaseSoftDao<T>` types | protected constructor; `Get` override; inherited soft CRUD | Key-specific soft-delete DAO | No |
| Three `BaseSoftGetAllDao<T>` types | protected constructor; inherited soft `GetAll` | Key-specific soft full-list DAO | No |
| Three `BaseSoftPagedDao<T>` types | protected constructor; inherited soft count/paging | Key-specific soft paged DAO | No |

The `TIdType` parameter on `BaseEFDataAccess<TContextType,TIdType>` is not referenced in that class; the
actual current key API is the three namespace-specific DAO families
([BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs),
[Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs)).

## Dependencies

### Published library

**All EF6 rows are gone from this table because the reference is gone from the file** — the
`$(TargetFramework.StartsWith('net4'))` `ItemGroup` was removed on 2026-08-16 and `EntityFramework` 6.5.1
is no longer referenced anywhere in the repository.

| Dependency | Version | Why it is present |
| --- | --- | --- |
| `Microsoft.EntityFrameworkCore` | *mid-flight — see note* | EF Core implementation |
| `Microsoft.EntityFrameworkCore.SqlServer` | *mid-flight* | Hardcoded string-constructor provider |
| `Microsoft.EntityFrameworkCore.InMemory` | *mid-flight* | Used by the proving-ground default constructor, not product source |
| `ProphetsWay.BaseDataAccess` | **3.1.0** | Parent DAL contracts — moved from 2.5.0 on 2026-08-16 |

The three EF Core references still sit inside a
`Condition="!$(TargetFramework.StartsWith('net4')) and $(TargetFramework.StartsWith('net'))"` `ItemGroup`,
which is now unconditionally true given the single `net10.0` target. **Their version is deliberately
unstated**: `Modernizer` was bumping it while this pass ran, so any number written here would be a claim
this agent could not stand behind. Read it from the file.

Evidence: [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj).

### Tests and proving ground

- `ProphetsWay.EFTools.Tests` uses `Microsoft.NET.Test.Sdk` 17.13.0, xUnit 2.9.3,
  `xunit.runner.visualstudio` 3.0.2, and coverlet 6.0.4 — re-verified 2026-08-16, unchanged by the adapter
  deletion. It still project-references `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.Example.Tests`
  even though it now defines nothing itself
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).
- `ProphetsWay.Example.DataAccess.EF` references `Microsoft.EntityFrameworkCore` and
  `Microsoft.EntityFrameworkCore.SqlServer` (versions mid-flight, as above) and `ProphetsWay.BaseDataAccess`
  **3.1.0**. **The FluentAssertions 8.2.0 reference this document previously recorded here has been
  removed** — verified by opening the csproj on 2026-08-16, which now carries no `FluentAssertions` line at
  all. The paid-commercial-licence exposure it described is closed
  ([ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)).
- The submodule's Example tests now use **Shouldly 4.3.0**, xUnit 2.9.3, `xunit.runner.visualstudio` 3.0.2,
  `Microsoft.NET.Test.Sdk` 17.13.0 and coverlet 6.0.4. **The FluentAssertions 8.2.0, Newtonsoft.Json 13.0.3
  and Microsoft.VSSDK.BuildTools 17.13.2126 references this document previously listed there are gone** —
  they belonged to the pre-3.0.0 pointer
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

**The TFM half of the 3.x direction has landed; the provider half has not.** `net10.0` alone is what the
approved EF Core-only design requires, and the retarget delivered it. The library still carries the
SQL Server and InMemory provider references, so the provider-neutrality work remains open.

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

- **`ProphetsWay.EFTools.Tests` now contains no tests.** The six adapter classes were **deleted on
  2026-08-16**; `Constants.cs` and the `.csproj` are all that survive. Each adapter was four to five lines
  holding a `GetIExampleDataAccess` override and no test logic, and the hook they overrode no longer exists
  upstream — `BaseUnitTests<T>` obtains its subject from `TestDataAccessFactory.CreateAs<T>()`, a `static`
  method with no parameter and nothing to override. The concept was gone, not merely broken, so the classes
  were removed rather than rewritten
  ([BaseUnitTests.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/BaseUnitTests.cs),
  [TestDataAccessFactory.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs)).
- **The 35 upstream `[Fact]` methods are parked, not lost.** They still live in the six submodule classes —
  `BaseDataAccessTests` (7), `CompanyDaoTests` (7), `JobDaoTests` (5), `ResourceDaoTests` (5),
  `TransactionDaoTests` (6) and `UserDaoTests` (5) — and they will be reachable against the Entity Framework
  implementation once the shape B seam exists (owner decision **D10**). **Nothing green went red:** they
  were already not running, because `LocalTestsOnly: 'yes'` skips them in CI and this project had not
  compiled since the submodule advance. Even had the adapters compiled, `CreateAs<T>()` returns the **NoDB**
  implementation, so they would not have exercised EF.
- **The restore mismatch is closed.** `ProphetsWay.EFTools.Tests` now targets `net10.0` alone and
  `ProphetsWay.Example.Tests` is `net48;net10.0`, so the single leg has a compatible asset
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj),
  [ProphetsWay.Example.Tests.csproj](../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj)).
- **`ProphetsWay.Example.DataAccess.EF` is where the last recorded break was, and it is being closed as
  this pass ran.** `ExampleDataAccess` declares `: BaseEFDataAccess<ExampleContext, int>,
  IExampleDataAccess`, and at 3.1.0 that interface additionally aggregates `IDepartmentDao` and
  `ICompanyResourceDao` and inherits `IDisposable`. Opening the file on 2026-08-16 shows member groups for
  both DAOs present as **deliberately throwing "not written yet" stubs**, and `Dispose` inherited from
  `BaseEFDataAccess`. `Implementer` owns this file and was editing it concurrently, so **no compile outcome
  is claimed here**
  ([ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  [IExampleDataAccess.cs](../ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs)).
- `Constants.cs` survives and still carries `#if` branches for `NET45`–`NETCOREAPP3_1`, `NET5_0` and
  `NET6_0_OR_GREATER`. Under the `net10.0`-only target only the last arm compiles, which builds
  `new ExampleDataAccess()` — the EF Core InMemory path. **The `Data Source=localhost` connection string is
  now unreachable**, because the Framework legs that used it no longer exist; the dead branches remain to be
  removed by `Implementer`
  ([Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs),
  [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
- No current test exercises soft-delete classes, keyless classes, transaction methods/helpers, disposal,
  rollback, provider neutrality, stable `GetAll` ordering, or update of a missing entity (all EFTools and
  inherited Example test sources were checked).
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
- No build or test command was run during this read-only grounding pass because this agent session has no
  command-execution tool. The facts above describe checked-in configuration and test inventory, not a
  claimed green local run.

## Real Usage Examples Found

The proving ground demonstrates the actual assembly pattern:

1. Derive an application context from `BaseEFContext` and expose typed `DbSet` properties
   ([ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs)).
2. Derive each DAO from the key-specific base matching its capability, such as
   `Int.BasePagedDao<Company>`, `Guid.BaseGetAllDao<Resource>`, or `Long.BasePagedDao<Transaction>`
   ([CompanyDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyDao.cs),
   [ResourceDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/ResourceDao.cs),
   [TransactionDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/TransactionDao.cs)).
3. Derive the application DAL from `BaseEFDataAccess<ExampleContext,int>`, construct each DAO with the
   shared protected context, and forward the application interfaces to those DAOs
   ([ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).

The default modern constructor uses EF Core InMemory; the string constructor configures SQL Server; and
the options constructor accepts caller-built options. These are current 2.x examples, not the approved
provider-neutral 3.x API.

## README Accuracy Check

| Existing claim | Verdict | Evidence |
| --- | --- | --- |
| The package reduces repetitive DAO CRUD under the BaseDataAccess paradigm | Accurate for 2.x | Product and example source implement that pattern |
| Example projects are “included” | Misleading | They are split between a git submodule and a root-owned EF project, not vendored together |
| `BaseDao` exposes CRUD and `EnsureTransaction` helpers | Mostly accurate | Current methods exist; prose incorrectly says `Delete` returns `T` in one paragraph, while code returns `int` |
| Primary-key API is split into `int`, `long`, and `Guid` namespaces | Accurate current state | 18 key-specific public classes exist |
| `DbContextOptions` is the preferred context path | Accurate for the tree; incomplete for the published package | It exists only on EF Core targets, which is now the only kind of target the tree has; consumers of the published 2.2.x .NET Framework assets do not have it |
| Context sample is usable as shown | Incorrect | The sample combines EF Core relationship APIs with a `DbModelBuilder` signature |
| Default modern construction provides an in-memory test database | Accurate current state | `ExampleDataAccess()` calls `UseInMemoryDatabase` |
| String construction points to SQL Server | Accurate but under-disclosed | The product context itself hardcodes `UseSqlServer`; the package is not provider-neutral |
| The library has 35 tests | **Stale.** The count survives upstream; the mechanism and the local project do not | The six submodule classes still hold 35 facts, but the adapters that reached them were deleted 2026-08-16 and `ProphetsWay.EFTools.Tests` now defines nothing |
| Tests are absent from the build pipeline because they need a local database | Accurate only as history | CI skips everything via `LocalTestsOnly: 'yes'`, and there is now nothing here to skip |
| README describes soft-delete and non-ID APIs added by 2.2.0 | Missing | Public classes and changelog exist; README does not teach them |
| README describes disposal / BaseDataAccess 3.x compatibility | **Now understated rather than wrong** | The reference is **3.1.0** and `BaseEFDataAccess` now carries an idempotent, non-throwing `Dispose` that rolls back an open transaction and disposes the context it created — verified by opening [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) on 2026-08-16. The README teaches none of it |

Evidence: [README.md](../README.md), [CHANGELOG.md](../CHANGELOG.md),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), and the files linked in each row's
supporting sections.

## Planned 3.x Direction — Approved, Not Implemented

The owner has approved the following direction. These statements must remain labeled as planned until
source, projects, and tests prove them:

- ~~EF Core-only; EF6 and .NET Framework remain historical 2.x behavior.~~ **Package references removed
  2026-08-16 — nothing in the repository references EF6.** The `#if NET4*` blocks in the C# sources are dead
  but not yet deleted.
- Relational-provider-neutral runtime package; no SQL Server or test provider forced on consumers.
  **Not done** — both provider references are still in the library.
- Six generic root-namespace DAO families replacing the 18 key-specific classes, with no compatibility
  wrappers unless implementation evidence forces reconsideration. **Not started.**
- SQLite in-memory as the fast CI contract/query leg and a SQL Server container for provider fidelity.
  **Not started.**
- ~~`ProphetsWay.BaseDataAccess` 3.1.0 adoption~~ **— the reference moved 2026-08-16**, and
  `BaseEFDataAccess.Dispose` has been implemented against it. `ObjectDisposedException` guarding on the
  transaction members has not.
- ~~Advance the `ProphetsWay.Example` submodule to 3.1.0~~ **— done 2026-08-16, pointer at `d845863`.**
- ~~Retarget every project to `net10.0`~~ **— done 2026-08-16** for the library,
  `ProphetsWay.EFTools.Tests` and `ProphetsWay.Example.DataAccess.EF`.
- ~~Delete the six test adapters~~ **— done 2026-08-16.** The Entity Framework conformance run they used to
  stand in for now waits on the upstream seam, per owner decision **D10**.

**Whether the repository compiles was not verified by this pass** — this agent has no build tool, and both
`Implementer` and `Modernizer` were editing concurrently. What was verified by opening the files: the break
recorded earlier as `ExampleDataAccess` not satisfying `IExampleDataAccess` is being closed as this was
written — the class now carries `IDepartmentDao` and `ICompanyResourceDao` member groups (deliberately
throwing "not written yet" stubs) and inherits `Dispose` from `BaseEFDataAccess`. **Do not read that as a
report of a green build.**

This direction was supplied by the owner on 2026-08-15 and recorded as D1-D6 in
[purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15). Current-state contrasts are verified in
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), [.gitmodules](../.gitmodules), and
[ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj).

## Gaps & Observations

1. **The reference moved to 3.1.0 on 2026-08-16, and the disposal obligation it carries has been met in the
   library.** `BaseEFDataAccess` now declares `public override void Dispose()` — idempotent, swallowing a
   failed rollback, disposing the context it constructed, and documented in `<remarks>` as disposing only
   what it created. **Verified by opening [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs),
   not inherited from an earlier pass of this document, which said the opposite and was correct when
   written.** What is *not* yet done is `ObjectDisposedException` guarding on the three transaction members
   — `_disposed` is set but never consulted outside `Dispose` itself. That is the open half of
   [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess).
2. Current EF6 and EF Core assets differ semantically on missing-row update and structurally on available
   constructors; one package does not expose one uniform contract. **This is now a statement about the
   published 2.2.0 package and the surviving `#if` branches** — no build produces an EF6 asset any more.
3. Provider selection leaks into the runtime package through both dependencies and code. **Unchanged.**
4. `GetPaged` orders by ID, but `GetAll` does not; soft paging has no explicit ordering either.
5. **There is no test suite in this repository at all.** The six adapters were deleted 2026-08-16 and the 35
   upstream tests are parked behind a seam that does not exist yet. Nothing regressed — `LocalTestsOnly:
   'yes'` already skipped them and the project already did not compile — but the verification gap is now
   total and explicit rather than nominal.
6. Packaging is functional but lacks homepage/tags, SourceLink, symbol-package, and explicit CI build
   metadata.
7. XML documentation exists on the 18 key-specific leaf classes but not on the principal context, DAL,
   root bridge, or keyless public API.
8. `docs/architecture.md`, per-project `docs/requirements.md`, and
   `docs/nuget-extraction-proposal.md` are **n/a by owner decision**. They are not documentation gaps.

## Open Questions for the Owner

1. **Target framework:** confirm that EF Core-only 3.x should target `net10.0` alone; current EF Core cannot
  supply the usual `netstandard2.0` reach-floor asset.
2. **Context ownership:** will 3.x accept caller-owned/injected contexts as well as creating contexts, and
   therefore track whether disposal ownership belongs to the DAL?
3. **Package promise:** should package-facing metadata say “relational-provider-neutral, certified on
  SQLite and SQL Server,” or keep certification detail in repository documentation only?
