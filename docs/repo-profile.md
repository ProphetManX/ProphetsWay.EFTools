# Repo Profile — ProphetsWay.EFTools

_Generated 2026-08-15. Evidence-based; every claim cites a source file._

## One-Line Purpose

The current package supplies EF6 and EF Core base classes that implement the repetitive CRUD, paging,
soft-delete, context, and transaction plumbing behind a `ProphetsWay.BaseDataAccess` 2.5.0 DAL
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs),
[RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs)).

## What It Actually Does

- Chooses EF6 6.5.1 for `net4*` targets and EF Core 9.0.4 for modern targets through conditional
  package references and preprocessor branches
  ([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
  [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs)).
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
| `ProphetsWay.EFTools` | Packable library | Published product; 26 source files |
| `ProphetsWay.Example.DataAccess.EF` | Library/example | EF implementation of the pinned Example 2.x contracts |
| `ProphetsWay.EFTools.Tests` | xUnit adapter | Re-runs the pinned Example tests against the EF implementation |
| `ProphetsWay.Example.DataAccess` | Submodule library | Pinned 2.x entities and DAL contracts |
| `ProphetsWay.Example.DataAccess.NoDB` | Submodule library | Pinned in-memory implementation used by the source tests |
| `ProphetsWay.Example.Tests` | Submodule xUnit project | Owns the 35 inherited test bodies |
| `ProphetsWay.Example.Database` | Submodule legacy SSDT project | SQL Server schema used by .NET Framework test legs |

`ProphetsWay.Example` is a submodule at `path = ProphetsWay.Example`, tracking the standalone repository's
`main` branch. It is not vendored and must not be edited from this repository
([.gitmodules](../.gitmodules)). The extra `[submodule "Submod"]` declaration has no path or URL.

## Public API Surface

Internal `RootDao<T,TIdType>` and `RootNonIdDao<T>` perform the work but are not public API
([RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).

| Type | Public/protected members | Purpose | Tested? |
| --- | --- | --- | --- |
| `BaseEFContext` | protected string constructor; EF Core-only protected `DbContextOptions` constructor | Common typed context base; string path chooses SQL Server on Core | Indirect construction only |
| `BaseEFDataAccess<TContextType,TIdType>` | string constructor; Core-only options constructor; `TransactionStart`, `TransactionCommit`, `TransactionRollBack`; protected `Context` | Instantiates a typed context and implements 2.x DAL transactions | Construction yes; transactions no |
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

| Target condition | Dependency | Version | Why it is present |
| --- | --- | --- | --- |
| `net4*` | `EntityFramework` | 6.5.1 | EF6 implementation |
| non-`net4*` `net*` | `Microsoft.EntityFrameworkCore` | 9.0.4 | EF Core implementation |
| non-`net4*` `net*` | `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.4 | Hardcoded string-constructor provider |
| non-`net4*` `net*` | `Microsoft.EntityFrameworkCore.InMemory` | 9.0.4 | Used by the proving-ground default constructor, not product source |
| all | `ProphetsWay.BaseDataAccess` | 2.5.0 | Parent DAL contracts |

Evidence: [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj).

### Tests and proving ground

- `ProphetsWay.EFTools.Tests` uses `Microsoft.NET.Test.Sdk` 17.13.0, xUnit 2.9.3,
  `xunit.runner.visualstudio` 3.0.2, and coverlet 6.0.4
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).
- `ProphetsWay.Example.DataAccess.EF` repeats EF6/EF Core/SQL Server and BaseDataAccess 2.5.0. It also
  references FluentAssertions 8.2.0, although none of its seven source files uses it
  ([ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj),
  [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
- The pinned Example tests use FluentAssertions 8.2.0, Newtonsoft.Json 13.0.3, xUnit 2.9.3, and
  Microsoft.VSSDK.BuildTools 17.13.2126
  ([ProphetsWay.Example.Tests.csproj](../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj)).

## Target Frameworks

| Project | Current TFMs | Review |
| --- | --- | --- |
| `ProphetsWay.EFTools` | `net461;net471;net48;net80;net90` | Missing a current LTS target; `net461`/`net471` are EOL; `net80`/`net90` are non-canonical and both reach EOL 2026-11-10 |
| `ProphetsWay.Example.DataAccess.EF` | `net471;net48;net80;net90` | Same old/undotted split; no reach-floor target |
| `ProphetsWay.EFTools.Tests` | `net472;net48;net80;net90` | Four test legs, including EOL Framework and soon-EOL modern legs |
| pinned Example DataAccess / NoDB | `net461;net471;net48;net50;net60;net70;net80;net90` | Submodule debt; not this repo's files to edit |
| pinned Example Tests | `net472;net48;net60;net80;net90` | Submodule debt; not this repo's files to edit |

No EFTools-owned project sets `LangVersion`, so each target uses its SDK default
([ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj),
[ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).

**Recommended current-state correction:** the approved 3.x direction is EF Core-only. Modern EF Core does
not support `netstandard2.0`, so the compatible slim library target is `net10.0`, with SQLite and SQL Server
test legs also on `net10.0`. This is planned work, not the current TFM/provider matrix. Removing existing
TFMs and EF6 is breaking and belongs in the approved major release.

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

- The adapter project defines no `[Fact]` methods. Its six classes inherit 35 facts from the pinned
  Example tests and override one construction property to return the EF DAL
  ([EFBaseDataAccessTests.cs](../ProphetsWay.EFTools.Tests/EFBaseDataAccessTests.cs),
  [BaseDataAccessTests.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/BaseDataAccessTests.cs)).
- The 35 methods cover direct and generic CRUD, `int`/`Guid`/`long` IDs, `GetAll`, count/paging, and two
  custom DAO methods. Across four configured test TFMs that is up to 140 test executions, not 35,
  when every target can run
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).
- Modern test legs create a named EF Core InMemory database with no tracking. Framework legs connect to
  `Data Source=localhost;Initial Catalog=ProphetsWay.Example;Integrated Security=True`
  ([Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs),
  [ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).
- No current test exercises soft-delete classes, keyless classes, transaction methods/helpers, disposal,
  rollback, provider neutrality, stable `GetAll` ordering, or update of a missing entity (all EFTools and
  inherited Example test sources were checked).
- `LocalTestsOnly: 'yes'` causes the shared pipeline to omit its `dotnet test` task entirely
  ([app-variables.yml](../app-variables.yml),
  [restore-build-test.yml](../../prophets-pipelines/steps/restore-build-test.yml)).
- CI builds `**/*.csproj` in Release, not the solution. `HasSqlProj` is unset, so the pinned legacy SSDT
  project is not restored or built. With `PostTargetToNuGet: 'yes'`, the target library is packed as
  alpha, beta, and release artifacts
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
| `DbContextOptions` is the preferred context path | Incomplete | It exists only on EF Core targets; Framework consumers do not have it |
| Context sample is usable as shown | Incorrect | The sample combines EF Core relationship APIs with a `DbModelBuilder` signature |
| Default modern construction provides an in-memory test database | Accurate current state | `ExampleDataAccess()` calls `UseInMemoryDatabase` |
| String construction points to SQL Server | Accurate but under-disclosed | The product context itself hardcodes `UseSqlServer`; the package is not provider-neutral |
| The library has 35 tests | Accurate as test-method count | Six adapter classes inherit 35 facts; four target frameworks can produce 140 executions |
| Tests are absent from the build pipeline because they need a local database | Accurate only for Framework legs | CI skips all legs, although modern legs use InMemory and do not require local SQL Server |
| README describes soft-delete and non-ID APIs added by 2.2.0 | Missing | Public classes and changelog exist; README does not teach them |
| README describes disposal / BaseDataAccess 3.x compatibility | Not current | Package still references BaseDataAccess 2.5.0 and has no `Dispose` implementation |

Evidence: [README.md](../README.md), [CHANGELOG.md](../CHANGELOG.md),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), and the files linked in each row's
supporting sections.

## Planned 3.x Direction — Approved, Not Implemented

The owner has approved the following direction. These statements must remain labeled as planned until
source, projects, and tests prove them:

- EF Core-only; EF6 and .NET Framework remain historical 2.x behavior.
- Relational-provider-neutral runtime package; no SQL Server or test provider forced on consumers.
- Six generic root-namespace DAO families replacing the 18 key-specific classes, with no compatibility
  wrappers unless implementation evidence forces reconsideration.
- SQLite in-memory as the fast CI contract/query leg and a SQL Server container for provider fidelity.
- `ProphetsWay.BaseDataAccess` 3.1.0 adoption, including its disposal/transaction obligations.
- Advance the `ProphetsWay.Example` submodule to 3.1.0 and rebuild the EF conformance adapter around it.

This direction was supplied by the owner on 2026-08-15 and recorded as D1-D6 in
[purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15). Current-state contrasts are verified in
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs), [.gitmodules](../.gitmodules), and
[ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj).

## Gaps & Observations

1. The published package implements BaseDataAccess 2.5.0, not the current 3.1.0 contract. Updating the
   reference is breaking because `BaseEFDataAccess` must acquire disposal behavior.
2. Current EF6 and EF Core assets differ semantically on missing-row update and structurally on available
   constructors; one package does not expose one uniform contract.
3. Provider selection leaks into the runtime package through both dependencies and code.
4. `GetPaged` orders by ID, but `GetAll` does not; soft paging has no explicit ordering either.
5. The entire current test suite is skipped in CI and cannot verify transaction/disposal behavior.
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
