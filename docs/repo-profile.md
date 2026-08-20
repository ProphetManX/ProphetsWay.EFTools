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
describes the earlier pass and is retained as history. ~~**No EF Core package version is stated anywhere in
this document — the bump was mid-flight when this pass ran, and an unverifiable number is worse than
none.**~~ **Superseded 2026-08-16: the bump has landed and the EF Core references are pinned at
`10.0.11`** — read from `ProphetsWay.EFTools.csproj` and
`ProphetsWay.Example.DataAccess.EF.csproj`, both opened that day; all five references agree. **No build has
been run since the bump**, so the versions below are stated as pinned in the project files and nothing
here claims the solution compiles against them — the green build recorded further down predates the
change._

_**Concurrency warning.** `Implementer` was editing `.cs` files and `Modernizer` was editing `.csproj`
files while this pass ran. Every statement below about C# source is a reading taken at that moment and
cited to the file; re-open the file rather than trusting the sentence. No build or test was run — this
agent has no command-execution tool — so **no compile outcome is claimed anywhere in this document**._

_**Superseded on 2026-08-16 by a real build.** The paragraph above is retained as history of how this
document was produced. **The owner ran `dotnet build ProphetsWay.EFTools.sln -c Debug` and it
succeeded** — SDK 10.0.400, **7 warnings, 0 errors**. That is the first measured compile outcome in
this repository since the submodule advance, and the statements it establishes are marked **observed
in a build on 2026-08-16** wherever they appear below. They are the only claims here not derived from
reading source. **No test was run** — there is nothing here to run — and a green compile is not
conformance. **That build predates the EF Core `10.0.11` bump**, so it is not evidence about the versions
now pinned in the project files._

_**Corrected again 2026-08-20** by a narrow factual pass — no re-profile. Three classes of claim in this
document were contradicted by the working tree and are fixed in place: (a) **the test project is not
empty**; thirteen adapters were re-added on 2026-08-16 at ~23:00 (`8e78b94`), `TestSeam.cs` /
`TestSeamTests.cs` in `08deda6` and `AdapterCoverageTests.cs` in `b9320fd` on 2026-08-18, and the suite has
been **run**; (b) **the submodule pointer is `61d9e7d`, not `d845863`** — one commit past the `3.1.0` tag;
(c) the library's own class inventory was understated by one file. Everything else below is left as its
author wrote it. Verified by listing `ProphetsWay.EFTools.Tests/` and `ProphetsWay.EFTools/`, opening all
19 test sources, and reading `.git/modules/ProphetsWay.Example/HEAD`._

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
| `ProphetsWay.EFTools` | Packable library | Published product; **27 source files** — 24 public abstract classes, one public enum (`ContextOwnership`), and the two internal `Root*Dao` engines. **Corrected 2026-08-20**: the earlier "26 source files, 24 public classes (23 abstract)" predates `ContextOwnership.cs` and counted `BaseEFDataAccess` as non-abstract; it is `public abstract class BaseEFDataAccess<TContext>` |
| `ProphetsWay.Example.DataAccess.EF` | Library/example | EF implementation of the Example contracts; 7 source files, 6 of them DAOs. **There is no `CompanyResourceDao`** |
| `ProphetsWay.EFTools.Tests` | xUnit project | **19 source files and a working harness — corrected 2026-08-20.** 13 one-line adapters, `TestSeam.cs`, `TestSeamTests.cs`, `AdapterCoverageTests.cs`, `AlternateKeyGuardSpikeTests.cs`, `Constants.cs`. The earlier "`Constants.cs` and the `.csproj`, and nothing else" was true for about two and a half hours on 2026-08-16 and is now false — see *Tests and Build Facts* |
| `ProphetsWay.Example.DataAccess` | Submodule library | **3.1.0** entities and DAL contracts, including `Department` and `CompanyResource` |
| `ProphetsWay.Example.DataAccess.NoDB` | Submodule library | In-memory implementation; the one `TestDataAccessFactory` returns |
| `ProphetsWay.Example.Tests` | Submodule xUnit project | 15 test classes holding **164 cases** (149 `[Fact]`, 6 `[Theory]` yielding 15). **13 of those classes are adapted into `ProphetsWay.EFTools.Tests` and do run against Entity Framework**; the 2 `ConventionShowcase` classes (20 facts) are correctly excluded. **Corrected 2026-08-20** — the earlier "35 `[Fact]` methods, parked, not lost" described the six pre-3.0.0 adapters and is superseded |
| `ProphetsWay.Example.Database` | Submodule database project | **SDK-style `Microsoft.Build.Sql/2.2.0`** as of the 3.1.0 pointer — not the legacy SSDT format this row previously named |

`ProphetsWay.Example` is a submodule at `path = ProphetsWay.Example`, tracking the standalone repository's
`main` branch. It is not vendored and must not be edited from this repository
([.gitmodules](../.gitmodules)). **The pointer is at `61d9e7d`** — read from
`.git/modules/ProphetsWay.Example/HEAD` on 2026-08-20, which holds
`61d9e7dfb209c4a92b0c16d058aad1af08031fb5`. **It is one commit past the `3.1.0` tag**, not the tag itself:
`git submodule status` renders it `3.1.0-1-g61d9e7d`, and that one commit is the merge of
`ProphetsWay.Example` PR #21 (`3.1.1-eftool-findings`) on 2026-08-18 — the commit that added
`TestDataAccessFactory.Use`, the seam this repository's test harness is built on. Corroborated by the
checked-out tree, whose `app-variables.yml` reads `3` / `1` / `1` and whose `CHANGELOG.md` opens with
"v3.1.1 — not yet released". **Earlier text in this document saying `d845863` — "3.1.0" — is superseded.**

The second, malformed `[submodule "Submod"]` declaration this section previously recorded — `branch =
main`, no `path`, no `url` — **cost more than the cosmetic nuisance it was filed as; see the Source
Link note under Packaging Audit.** It is **absent from `.gitmodules` as read on 2026-08-16**;
`Modernizer` owns that file and was removing it as this correction was written.

## Public API Surface

Internal `RootDao<T,TIdType>` and `RootNonIdDao<T>` perform the work but are not public API
([RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)).

**The `Tested?` column below is stale and is corrected 2026-08-20.** It was written when
`ProphetsWay.EFTools.Tests` held no tests. It now holds 151 discoverable cases, of which 144 are the
adapted upstream `ProphetsWay.Example` suite — but **every one of those is written against
`IExampleDataAccess`, not against a type in this table**. The types below are therefore reached only
indirectly, through the five `ProphetsWay.Example.DataAccess.EF` DAOs that derive from them, and
`DepartmentDao` derives from **none** of them. Read each cell as "exercised transitively, if at all";
nothing here has a test of its own.

| Type | Public/protected members | Purpose | Tested? |
| --- | --- | --- | --- |
| `BaseEFContext` | protected string constructor; EF Core-only protected `DbContextOptions` constructor | Common typed context base; string path chooses SQL Server on Core | Indirect construction only |
| `BaseEFDataAccess<TContext>` | context + `ContextOwnership` constructors; `TransactionStart`, `TransactionCommit`, `TransactionRollBack`; **`Dispose`**; protected `Context` | Holds a typed context, implements DAL transactions, and — **as of 2026-08-16** — the 3.x disposal contract. **One type parameter, not two — corrected 2026-08-20**; `BaseEFDataAccess<TContextType,TIdType>` no longer exists | Transitively, through `ExampleDataAccess` |
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
| `Microsoft.EntityFrameworkCore` | **10.0.11** | EF Core implementation |
| `Microsoft.EntityFrameworkCore.SqlServer` | **10.0.11** | Hardcoded string-constructor provider |
| `Microsoft.EntityFrameworkCore.InMemory` | **10.0.11** | Used by the proving-ground default constructor, not product source |
| `ProphetsWay.BaseDataAccess` | **3.1.0** | Parent DAL contracts — moved from 2.5.0 on 2026-08-16 |

The three EF Core references still sit inside a
`Condition="!$(TargetFramework.StartsWith('net4')) and $(TargetFramework.StartsWith('net'))"` `ItemGroup`,
which is now unconditionally true given the single `net10.0` target. **Their version is no longer
unstated** — the earlier text here withheld it because `Modernizer` was bumping it mid-pass. That bump
landed on 2026-08-16 and all three read `10.0.11`, verified by opening
`ProphetsWay.EFTools.csproj` rather than by inheriting the claim.

**There is no central version file to look for.** No `Directory.Packages.props` and no
`Directory.Build.props` exists anywhere in the workspace, and `ManagePackageVersionsCentrally` is not set
in any project — so the two `.csproj` files are the only place package versions live. Verified 2026-08-16
by a workspace-wide file search and a text search for the property.

**No EF6 reference survives anywhere.** Both conditional `net4*` `ItemGroup`s were removed and no project
in the repository references `EntityFramework` — verified 2026-08-16 by a workspace-wide search for the
package reference, which matches nothing.

**Release-relevant consequence:** pinning at 10.0.11 raises the **minimum EF Core version a consumer of the
eventual 3.0.0 package must resolve**. Recorded here as a fact about the project files; the release note
for it belongs to `Changelog Author`.

**No build has been run since the bump.** These are the versions the project files declare, not a
statement that the solution still compiles against them.

Evidence: [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj),
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj).

### Tests and proving ground

- `ProphetsWay.EFTools.Tests` uses `Microsoft.NET.Test.Sdk` 17.13.0, xUnit 2.9.3,
  `xunit.runner.visualstudio` 3.0.2, and coverlet 6.0.4 — re-verified 2026-08-16, unchanged by the adapter
  deletion. It still project-references `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.Example.Tests`
  even though it now defines nothing itself
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)).
- `ProphetsWay.Example.DataAccess.EF` references `Microsoft.EntityFrameworkCore` and
  `Microsoft.EntityFrameworkCore.SqlServer` (both **10.0.11**, as above) and `ProphetsWay.BaseDataAccess`
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

### Source Link is off, and a malformed `.gitmodules` block was the proximate cause — observed in a build on 2026-08-16

The owner's `dotnet build` emitted, **three times** — once each for `ProphetsWay.EFTools`,
`ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests`:

```
Microsoft.Build.Tasks.Git.targets(25,5): warning : The path of submodule 'Submod' is missing or invalid: ''.
  The source code won't be available via Source Link.
```

The cause is the malformed `[submodule "Submod"]` block — `branch = main`, no `path`, no `url`.
**The measured cost is that Source Link is disabled on a published package**: a consumer of
`ProphetsWay.EFTools` cannot step into its source. This is recorded in `AGENTS.md` as **Deviation 7**,
where it had been filed **Low / cosmetic** on reasoning; the build disproves that reading and the
severity has been raised to **Medium**. The block is **absent from `.gitmodules` as read on
2026-08-16** — `Modernizer` was removing it as this was written — but **no build has been run since,
so the warning is not claimed to be gone.**

**Removal is expected to stop the warning. It does not give the library Source Link.** The table
above is unchanged on that point and must be read alongside this note: SourceLink is not referenced
at all, and `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat`
and `ContinuousIntegrationBuild` are all absent — re-verified by opening
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) on 2026-08-16.
Source Link starts working when the proposed snippets below are applied, not when the block goes.

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

- **`ProphetsWay.EFTools.Tests` holds a working harness — corrected 2026-08-20, superseding the two
  bullets that stood here.** Those bullets said the project contained no tests and that 35 upstream facts
  were "parked"; both were accurate on 2026-08-16 at 20:24 and false by 23:00 the same day. What is there,
  read file by file:
  - **`TestSeam.cs`** — an `internal static` class whose `[ModuleInitializer]` calls
    `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess)`. That one call points the **entire**
    upstream suite at `ProphetsWay.Example.DataAccess.EF.ExampleDataAccess` for the assembly's whole run.
    A module initializer rather than a fixture because xUnit runs collections in parallel
    ([TestSeam.cs](../ProphetsWay.EFTools.Tests/TestSeam.cs),
    [TestDataAccessFactory.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs)).
  - **13 adapters** — `EFBaseDataAccessTests`, `EFCompanyDaoTests`, `EFCompanyResourceDaoTests`,
    `EFCompanyResourceDataAccessTests`, `EFDataAccessDisposalTests`, `EFDataAccessTransactionTests`,
    `EFDepartmentDaoTests`, `EFDepartmentDataAccessTests`, `EFJobDaoTests`, `EFResourceDaoTests`,
    `EFSnapshotDeepCopyTests`, `EFTransactionDaoTests`, `EFUserDaoTests`. Each is literally
    `public class EFXxxTests : XxxTests { }` with no test logic — inheritance is all xUnit needs to
    discover an upstream class in this assembly.
  - **`TestSeamTests.cs`** — 5 `[Fact]`, `[Trait("Guard","Seam")]`, deriving from
    `BaseUnitTests<ICompanyDao>` so it asserts on the same `_da` field every adapted test reads. It pins
    the concrete type, that the provider is **relational** (`Database.IsRelational()`), that the factory
    hands out a fresh instance per call, and that the store is reachable.
  - **`AdapterCoverageTests.cs`** — 2 `[Fact]`, same trait. Reflects over the upstream assembly and fails
    if any class deriving from `BaseUnitTests<>` or carrying `[Collection]` has no adapter here, and
    separately pins that the two `ConventionShowcase` classes stay excluded.
  - **`Constants.cs`** — now a single SQL Server connection string
    (`Data Source=localhost;Initial Catalog=ProphetsWay.Example;Integrated Security=True;TrustServerCertificate=True`)
    and one expression-bodied `GetExampleDataAccess` property.
- **Discoverable cases: 151**, or **165** counting the spike. 144 come from the 13 adapted classes — 129
  `[Fact]` plus 6 `[Theory]` yielding 15 `[InlineData]` cases — and 7 from the two guard classes. The 20
  `ConventionShowcase` facts are deliberately unadapted. Counted by grepping every `[Fact]`, `[Theory]`
  and `[InlineData]` in `ProphetsWay.Example.Tests/` and subtracting the showcase; the remainder, 144, plus
  the local 7, reconciles exactly with the 151 the runner reported.
- **Last measured result: 2026-08-18 — `dotnet test` = 151 tests, 123 passed, 28 failed**
  ([feature-requests.md](feature-requests.md) entry 13). The `.trx` files in
  `ProphetsWay.EFTools.Tests/TestResults/` are earlier laps from the same evening — `baseline`/`efseam2`/
  `efseam3` at 147/53/94, `lap2`/`final` at 151/57/94 — and `efseam.trx` at 147/15/132 is the run before
  the seam was fixed. **No build and no test run has happened since**, and
  `AlternateKeyGuardSpikeTests.cs` was committed in the interim, so 123/28 is the **last known** state and
  not a current one.
- **`AlternateKeyGuardSpikeTests.cs` is committed and carries no trait — recorded here as a fact, not a
  recommendation.** It entered version control in `2691526` (2026-08-19); the 2026-08-19 session handoff
  describes it as untracked, which is superseded. It declares **7 `[Theory]` methods, each with two
  `[InlineData]` values (`InMemory`, `Sqlite`) — 14 cases — and zero `[Trait]` attributes**, verified by
  grepping every attribute in the file. Because xUnit trait filters are allowlists, the repository's
  stated gate `--filter "Scope=Contract|Guard=Seam"` selects **none** of those 14: the file executes on a
  bare `dotnet test` and under no filtered invocation. It is also self-contained — its own `SpikeUser`,
  `SpikeUniqueUser` and `SpikeContext`, asserting about EF Core's read-only-key guard rather than about
  anything in this library, which its own `<remarks>` state ("an empirical spike, not a specification").
  **Which trait it should carry, and whether it stays, is an owner decision already queued in the
  2026-08-19 handoff and is not proposed here**
  ([AlternateKeyGuardSpikeTests.cs](../ProphetsWay.EFTools.Tests/AlternateKeyGuardSpikeTests.cs)).
- **What the harness still does not cover.** Every test in it is written against `IExampleDataAccess`, so
  **no test targets this library's own public surface directly**: the 18 key-specific DAO bases are
  reached only through the five EF DAOs that derive from them, and `DepartmentDao` derives from no EFTools
  base at all, leaving `RootBaseSoftDao` and both keyless bases exercised by nothing. The ~28 failures are
  dominated by `CompanyResourceDao`, which **does not exist** — no `CompanyResourceDao.cs`, no
  `DbSet<CompanyResource>`, no `ToTable`, and three `ICompanyResourceDao` forwarders that throw
  `NotWrittenYet`. The suite requires a **local SQL Server** carrying `ProphetsWay.Example` and runs on
  that provider alone, despite the `Microsoft.EntityFrameworkCore.Sqlite` reference sitting in the test
  `.csproj` for a certification leg that has not been built. Nothing tests the redesign in
  [api-contract.md](api-contract.md), which has not been implemented.
- **The restore mismatch is closed.** `ProphetsWay.EFTools.Tests` now targets `net10.0` alone and
  `ProphetsWay.Example.Tests` is `net48;net10.0`, so the single leg has a compatible asset
  ([ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj),
  [ProphetsWay.Example.Tests.csproj](../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj)).
- **`ProphetsWay.Example.DataAccess.EF` held the last recorded break, and it is closed.**
  `ExampleDataAccess` declares `: BaseEFDataAccess<ExampleContext>, IExampleDataAccess` — **one type
  argument, corrected 2026-08-20**; the `<ExampleContext, int>` this line used to name does not exist. At
  3.1.x that interface additionally aggregates `IDepartmentDao` and `ICompanyResourceDao` and inherits
  `IDisposable`. **The `IDepartmentDao` group is now written** — `DepartmentDao.cs` exists, `ExampleContext`
  declares `DbSet<Department>` and maps it, and the eight forwarders call it behind `ThrowIfDisposed()`.
  **Only the three `ICompanyResourceDao` forwarders still throw**: `NotWrittenYet` matches four times in
  the file, three throws plus the helper. The earlier "11 members that throw" is superseded. It compiles;
  it does not conform
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
- Soft-delete bases, keyless bases, the transaction helpers on the DAO bases, provider portability and
  stable `GetAll` ordering are still exercised by nothing — the harness reaches `IExampleDataAccess`, and
  the DAOs behind it do not derive from those types.
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
- **The solution builds — observed in a build on 2026-08-16.** `dotnet build ProphetsWay.EFTools.sln -c
  Debug`, run by the owner, **succeeded**: SDK 10.0.400, **7 warnings, 0 errors**. Every project
  compiled — `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests`
  on `net10.0`; the submodule's `ProphetsWay.Example.DataAccess` and `ProphetsWay.Example.DataAccess.NoDB`
  on `netstandard2.0` and `net10.0`; `ProphetsWay.Example.Tests` on **both** `net48` and `net10.0`; and
  `ProphetsWay.Example.Database` producing `ProphetsWay.Example.Database.dacpac`. This closes all three
  breaks recorded as `AGENTS.md` Deviation 8. **It is a compile outcome and nothing more** — no test was
  run, the new `ExampleDataAccess` DAO members are deliberately throwing stubs, and `ExampleContext`
  maps neither new entity.
- **Two facts the CI pipeline never exercises were measured by that build.** The SDK-style `.sqlproj`
  **builds under the .NET CLI** — `HasSqlProj` is commented out in [app-variables.yml](../app-variables.yml)
  and CI builds `**/*.csproj`, so CI has never proved this. And `ProphetsWay.Example.Tests` **compiled on
  both legs**, which is what the upstream **164 tests / 328 executions** figure rests on.
- **4 of the 7 warnings are upstream, not an EFTools defect.** Two per leg, both `xUnit1013` on the
  submodule's `DepartmentDaoTests.cs`: `public static void EditEveryFieldAfterTheCall` (line 294) and
  `public static void AssertEveryStampIsUtc` (line 1001) — both helpers carrying no test attribute, both
  confirmed present and public by opening the file. They belong to `ProphetsWay.Example`; a `Test
  Designer` fixes them **there**, never from this repository. **This verifies a claim that could not be
  verified before:** `ProphetsWay.Example/docs/repo-profile.md` asserts "two `xUnit1013` warnings" and
  annotates the count as taken from a `Modernizer` build and not re-measured. It is exactly two, and both
  are now identified
  ([DepartmentDaoTests.cs](../ProphetsWay.Example/ProphetsWay.Example.Tests/DepartmentDaoTests.cs)).
  The remaining 3 warnings are the Source Link warning — see the Packaging Audit.
- The rest of this document was produced with no command-execution tool and describes checked-in
  configuration and test inventory. Apart from the four bullets above and the Source Link note, nothing
  here is a measured result.

## Real Usage Examples Found

The proving ground demonstrates the actual assembly pattern:

1. Derive an application context from `BaseEFContext` and expose typed `DbSet` properties
   ([ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs)).
2. Derive each DAO from the key-specific base matching its capability, such as
   `Int.BasePagedDao<Company>`, `Guid.BaseGetAllDao<Resource>`, or `Long.BasePagedDao<Transaction>`
   ([CompanyDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyDao.cs),
   [ResourceDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/ResourceDao.cs),
   [TransactionDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/TransactionDao.cs)).
3. Derive the application DAL from `BaseEFDataAccess<ExampleContext>`, pass it a context plus a
   `ContextOwnership` value, construct each DAO with the shared protected context, and forward the
   application interfaces to those DAOs
   ([ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs)).

The string constructor configures SQL Server and takes ownership of the context it builds; the options
constructor accepts caller-built options and also owns; the context constructor borrows. **Corrected
2026-08-20** — there is no longer a parameterless EF Core InMemory default. These are current 2.x examples,
not the approved provider-neutral 3.x API.

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
| The library has 35 tests | **Stale, and stale in a new direction — corrected 2026-08-20** | The figure described six pre-3.0.0 adapters. `ProphetsWay.EFTools.Tests` now discovers **151** cases (165 with the untraited spike), of which 144 are the adapted upstream suite. Last measured 2026-08-18: 123 passed, 28 failed |
| Tests are absent from the build pipeline because they need a local database | **Accurate, and now literally so** | `LocalTestsOnly: 'yes'` skips them in CI, and the harness genuinely needs a local SQL Server carrying `ProphetsWay.Example` — see `Constants.cs` |
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
- ~~Advance the `ProphetsWay.Example` submodule to 3.1.0~~ **— done 2026-08-16 and advanced again
  2026-08-18; the pointer is at `61d9e7d`**, one commit past the `3.1.0` tag, on the open `3.1.1` line.
- ~~Retarget every project to `net10.0`~~ **— done 2026-08-16** for the library,
  `ProphetsWay.EFTools.Tests` and `ProphetsWay.Example.DataAccess.EF`.
- ~~Delete the six test adapters~~ **— done 2026-08-16, and superseded the same night.** Thirteen new
  adapters plus `TestSeam.cs` replaced them once `TestDataAccessFactory.Use` landed upstream; the Entity
  Framework conformance run they stood in for **now happens**, per owner decision **D10** shape B.

**The repository compiles — observed in a build on 2026-08-16**, after this document's reading passes.
The owner ran `dotnet build ProphetsWay.EFTools.sln -c Debug`; it succeeded with 7 warnings and 0
errors on SDK 10.0.400. That closes the last recorded break — `ExampleDataAccess` now satisfies
`IExampleDataAccess`, with `IDepartmentDao` and `ICompanyResourceDao` member groups present as
deliberately throwing "not written yet" stubs and `Dispose` inherited from `BaseEFDataAccess`.
**Read it as a compile outcome, not as progress on the redesign**: every item still marked *Not done*
or *Not started* above is unaffected by it.

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
5. **There is a test suite, and the gap it leaves is narrower and more specific than "none" — corrected
   2026-08-20.** The bullet that stood here said there was no test suite at all. `ProphetsWay.EFTools.Tests`
   runs 151 cases through `TestSeam.cs`, last measured at 123 passed / 28 failed on 2026-08-18. What remains
   uncovered: **this library's own public surface**, since every test targets `IExampleDataAccess`; the
   soft-delete and keyless bases, which nothing derives from; provider portability, since the suite runs on
   SQL Server alone; and the entire redesign in [api-contract.md](api-contract.md). `CompanyResourceDao`
   does not exist and accounts for most of the red.
6. Packaging is functional but lacks homepage/tags, SourceLink, symbol-package, and explicit CI build
   metadata. **Source Link is measurably off, not merely unconfigured** — the 2026-08-16 build emitted the
   `Submod` submodule-path warning three times, one per project, each ending "The source code won't be
   available via Source Link." Removing the malformed block stops the warning; the library still has no
   SourceLink package reference and none of the five related properties, so consumer source navigation
   remains absent either way. See the Packaging Audit.
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
