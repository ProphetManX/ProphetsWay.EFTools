# AGENTS.md — ProphetsWay.EFTools

<!-- ═══════════════════════════════════════════════════════════════════════
     BEGIN SHARED BLOCK
     Generated from prophets-pipelines/conventions/AGENTS.shared.md
     DO NOT EDIT BY HAND — run /sync-agents-md to regenerate.
     ═══════════════════════════════════════════════════════════════════════ -->

## About This Codebase

`ProphetsWay.*` is a family of small, focused .NET libraries by G. Gordon Nasseri, published to
NuGet under the `ProphetsWay.` prefix and hosted at `github.com/ProphetManX`. Each library lives
in its own repository with its own version line, changelog, and pipeline.

### The Two Families

| Family | Repos | Purpose |
|---|---|---|
| **Utility** | Utilities, Logger, Hasher | Standalone helpers with no dependency on each other |
| **Data Access** | BaseDataAccess, EFTools | A layered DAL-decoupling paradigm; EFTools implements BaseDataAccess |

`ProphetsWay.Example` is a reference implementation, not a published package.
`prophets-pipelines` holds shared Azure DevOps YAML templates and this conventions file.

## Naming

**Display vs. codified.** The organization name is written two ways, and the distinction matters:

- **Display name** — `Prophet's Way`, with the apostrophe. Used in `<Company>`, prose, README text, and anything a human reads.
- **Codified** — `ProphetsWay`, no apostrophe or space. Used in namespaces, package IDs, assembly names, repo names, and the Azure DevOps org.

| Thing | Rule | Example |
|---|---|---|
| Repository | `ProphetsWay.<Library>` | `ProphetsWay.Logger` |
| Package ID | matches repository | `ProphetsWay.Logger` |
| Assembly name | matches repository | `ProphetsWay.Logger` |
| Library project folder | matches repository | `ProphetsWay.Logger/` |
| Test project | `<Library>.Tests` — **plural** | `ProphetsWay.Logger.Tests` |
| Example project | `<Library>.Example` | `ProphetsWay.Logger.Example` |
| `<Company>` | display name | `Prophet's Way` |
| `<Authors>` | `G. Gordon Nasseri` | |
| `<Product>` | library name without prefix | `Logger` |

### Namespaces

The rule is **family-dependent**. Do not "correct" one family to match the other.

- **Utility family** shares one root namespace regardless of assembly name:
  `ProphetsWay.Utilities`, with sub-namespaces for areas (`ProphetsWay.Utilities.LoggerDestinations`).
  A consumer adds one `using ProphetsWay.Utilities;` and reaches every utility library.
  This is why `ProphetsWay.Logger.dll` declares `namespace ProphetsWay.Utilities` — intentional, not a bug.
- **Data Access family** uses per-library namespaces: `ProphetsWay.BaseDataAccess`, `ProphetsWay.EFTools`
  (plus key-type sub-namespaces `.Guid`, `.Int`, `.Long`). These are an architectural paradigm, not
  utilities, and are kept separately addressable.
- **Test projects always use their own namespace**, `<AssemblyName>.Tests` — never the shared root.

## Target Frameworks

```xml
<!-- default for a published library -->
<TargetFrameworks>netstandard2.0;net10.0</TargetFrameworks>

<!-- only when a framework-conditional dependency or API requires it -->
<TargetFrameworks>netstandard2.0;net48;net10.0</TargetFrameworks>

<!-- test projects — netstandard2.0 is not a valid test target -->
<TargetFrameworks>net48;net10.0</TargetFrameworks>
```

.NET ships every November: **even-numbered = LTS (3 years), odd-numbered = STS (18 months)**.
.NET 10 is the current LTS (Nov 2025 → ~Nov 2028). **.NET 8 and .NET 9 both go end of life on
10 November 2026** — `net8.0`/`net9.0` are now debt, as are `netcoreapp*`, `net5.0`–`net7.0`,
and anything below `net48`.

1. **LTS only.** Never target an STS release in a published library — an 18-month window means
   re-cutting the list every year and stranding someone each time. Never target a preview.
2. **`netstandard2.0` is permanent.** It is an API contract, not a runtime, so it cannot expire.
   It is the reach floor: consumable by .NET Framework 4.6.1+ (painless from 4.7.2 up) and by every
   .NET Core/5+ runtime. It is also the last .NET Standard version Framework supports —
   `netstandard2.1` deliberately excluded it.
3. **`net48` is conditional, not default.** `netstandard2.0` already reaches .NET Framework 4.8, so
   an explicit `net48` target earns its place only when the repo has a framework-conditional
   *dependency* or needs an API `netstandard2.0` does not expose. `ProphetsWay.EFTools` qualifies —
   its EF6 branch is keyed on `net4*`. Most repos do not. Justify it per repo.
   (.NET Framework 4.8 is the final Framework version; it ships as a Windows component and inherits
   the OS lifecycle, so it has no standalone EOL date.)
4. **Carry exactly one modern TFM** unless something concrete requires two. Every extra target
   multiplies build time.
5. **Test projects name runtimes directly**, since `netstandard2.0` cannot be a test target. A
   `net48` test target is how .NET Framework behavior is *verified*, which is distinct from a
   library merely *supporting* it: `Activator.CreateInstance<T>()` wraps a throwing constructor on
   .NET Framework and does not on .NET Core, so `ProphetsWay.Example.Tests` must keep `net48` or its
   exception-passthrough regression guard stops guarding anything.
6. **Canonical dotted monikers** — `net10.0`, never `net100`. The undotted form parses, but it is
   non-standard and inconsistent across the repos.
7. **`LangVersion`:** `netstandard2.0` defaults to C# 7.3, and that constraint applies to all shared
   code in a multi-targeted project. This is why nullable reference types do not work in these
   libraries regardless of what a csproj claims.
8. **Dropping `net8.0`/`net9.0` is not a breaking change** while `netstandard2.0` remains — those
   consumers still install and still resolve an asset.
9. **Adding a TFM is a MINOR bump, never a patch.** A new target silently repoints existing
   consumers to a *different assembly* — a .NET 10 consumer that resolved the `netstandard2.0` asset
   starts binding the `net10.0` one, a different compilation with different BCL bindings and no
   netstandard shims. A patch must be safe to take without reading the notes.

## Packaging Metadata

Required in every **published** library's `.csproj`. If a repo is not published to NuGet, these
are optional — but they become mandatory the moment publishing is on the table.

```xml
<PackageId>ProphetsWay.Thing</PackageId>
<Product>Thing</Product>
<Authors>G. Gordon Nasseri</Authors>
<Company>Prophet's Way</Company>
<Description>...</Description>
<RepositoryType>git</RepositoryType>
<RepositoryUrl>https://github.com/ProphetManX/ProphetsWay.Thing</RepositoryUrl>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
<PackageRequireLicenseAcceptance>true</PackageRequireLicenseAcceptance>
<PackageIcon>profile.png</PackageIcon>
<PackageReadmeFile>README.md</PackageReadmeFile>
<PackageTags>...</PackageTags>
```

Paired with the item group that actually packs those files — declaring `PackageIcon` or
`PackageReadmeFile` without the matching `<None Pack="true">` packs nothing and fails the build:

```xml
<ItemGroup>
  <None Include="..\CHANGELOG.md" Link="CHANGELOG.md" Pack="true" PackagePath="" />
  <None Include="..\README.md" Link="README.md" Pack="true" PackagePath="" />
  <Content Include="..\profile.png" Link="profile.png" Pack="true" PackagePath="" />
</ItemGroup>
```

**An empty self-closing element is not a value.** `<PackageId />` silently falls back to
`AssemblyName` and leaves the nuget.org listing without a license, readme, or source link.
Treat empty stubs as missing.

Versioning is owned by the pipeline. Leave `<Version />`, `<AssemblyVersion />`,
`<FileVersion />`, and `<InformationalVersion />` empty in the csproj — `app-variables.yml`
supplies them at build time.

## Testing

- **xUnit** — the test framework. Do not introduce NUnit or MSTest.
- **Shouldly** — assertion style. Prefer `result.ShouldBe(...)` over `Assert.Equal`.
  FluentAssertions 8.x requires a paid commercial license; do not add it to any project.
- **coverlet.collector** — coverage.
- **Moq** — only where a test genuinely needs a mock; most of these libraries do not.
- Test class names mirror the type under test: `HasherTests`, `FileDestinationTests`.
- Tests requiring a local database set `LocalTestsOnly: 'yes'` in `app-variables.yml` so CI skips them.

## Pipelines

Every repo consumes the shared templates in `prophets-pipelines` via two root files:

| File | Purpose |
|---|---|
| `app-variables.yml` | Per-repo values — `Major`/`Minor`/`Patch`, `TargetProject`, `Product`, `RepoName`, `PostTargetToNuGet`, `LocalTestsOnly` |
| `local-pipeline.yml` | Thin wrapper pulling `prophets-pipelines` stage templates |

`Major`/`Minor`/`Patch` are bumped **by hand** in `app-variables.yml` as work proceeds. The
pipeline appends build metadata to produce alpha/beta/release packages.

## Repo Layout

```
ProphetsWay.Thing/
├─ AGENTS.md                 ← this file
├─ README.md                 ← packed into the nupkg
├─ CHANGELOG.md              ← packed into the nupkg
├─ LICENSE                   ← MIT
├─ profile.png               ← NuGet icon
├─ app-variables.yml
├─ local-pipeline.yml
├─ ProphetsWay.Thing.sln
├─ ProphetsWay.Thing/        ← library
├─ ProphetsWay.Thing.Tests/  ← xUnit
└─ docs/                     ← agent-generated analysis
  ├─ repo-profile.md
  ├─ purpose-and-scope.md
  ├─ nuget-extraction-proposal.md
  └─ feature-requests.md    ← durable request and decision index
```

These artifacts are generated by agents and committed. `feature-requests.md` becomes applicable once
the first request is captured; an empty repo need not carry a placeholder.

**`docs/architecture.md` and per-project `docs/requirements.md` are `n/a` for a utility or reference
library** — its architecture lives in `AGENTS.md`, the README, and XML `<remarks>`. They apply to
multi-project **application** solutions only. Do not report either as missing from a library repo.

## Solution Layout

For multi-project application solutions, the rule is **base name = contracts, suffix = swappable
implementation**. Business logic has one implementation and needs no split; a DAL has many and does.

| Project | Contains |
|---|---|
| `<Solution>.Core` | Domain models, business logic interfaces, and their implementation |
| `<Solution>.DataAccess` | DAL contracts only — interfaces and entities |
| `<Solution>.DataAccess.<Provider>` | One DAL implementation: `.MSSQL`, `.PostgreSQL`, `.MySQL`, `.NoDB`, `.EF` |
| `<Solution>.Database` | The `.sqlproj` database project |
| `<Solution>.Api` | Service endpoints |
| `<Solution>.Web` | Web UI |
| `<Solution>.Win` | Desktop UI |
| `<Project>.Tests` | xUnit tests for that project — `<Solution>.Core.Tests` |

**The suffix list is open.** A new provider or UI technology gets a new suffix following the same
shape (`.DataAccess.Cosmos`, `.Mobile`, `.Cli`). Do not invent a new *pattern* — extend this one.

A contracts project must never reference an implementation project, and must never expose a type
from a specific technology (`DbContext`, `SqlConnection`, `HttpContext`) in its public surface.
That rule is what makes the DAL swappable, and it is the whole point of the paradigm.

### Database Projects

New `.sqlproj` projects use the **`Microsoft.Build.Sql`** SDK — SDK-style, cross-platform, and
buildable with `dotnet build`:

```xml
<Project Sdk="Microsoft.Build.Sql/<version>">
```

The legacy SSDT format (`ToolsVersion="4.0"`, the 2003 MSBuild namespace, `TargetFrameworkVersion`,
plus `.dbmdl`/`.jfm` sidecar files) requires Visual Studio on Windows and cannot be built by the
.NET CLI. Existing legacy projects are **debt to migrate** — the `.sql` files carry over unchanged;
the project header and sidecars are what change.

## Code Style

- Tabs for indentation in `.csproj` and `.cs`.
- Braces on their own line (Allman).
- Interfaces prefixed `I`. Abstract bases prefixed `Base` or `Root`.
- Public API surface gets XML doc comments; internals do not need them.
- No `.editorconfig` exists yet — style is convention, not enforced. Match surrounding code.

## Rules for Agents

- **Never edit `.cs`, `.csproj`, `.sln`, or `.yml`** unless the human explicitly asks in that turn.
  Propose changes as fenced snippets labeled `PROPOSED — not applied`.
- **Exception — the TDD agents.** `Interface Architect`, `Test Designer`, `Implementer`, and
  `Refactorer` exist to write code; invoking one *is* the explicit ask. Each is restricted to one
  kind of file, and those restrictions are load-bearing:

  | Agent | May write |
  |---|---|
  | `Interface Architect` | Interfaces and their supporting types — never tests, never implementations |
  | `API Designer` | HTTP contracts and `docs/api/` — never implementations |
  | `Test Designer` | `*Tests.cs` only |
  | `Implementer` | Implementation `.cs` only — **never** a test file |
  | `Refactorer` | Implementation `.cs` only, behavior-preserving — **never** a test file |
  | `Modernizer` | `.csproj` / `.sqlproj` build and packaging config — never versions, never namespaces |
  | `Pipeline Engineer` | `.yml` / `.yaml` only — never versions, secrets, project files, or Markdown |
  | `Changelog Author` | `CHANGELOG.md` only |
  | `Threat Modeler`, `Security Reviewer` | `docs/security/` only — read-only on source |

  If an agent edits a test to make it pass, the workflow has failed. Report it rather than
  accepting the green build.
- **Never bump a version** in `app-variables.yml`. That is a human decision.
- **Never invent an Azure DevOps `definitionId`.** Badge URLs must be copied from a file that
  already exists in the repo. If one is missing, ask.
- **Feature requests are shared-capture, single-owner triage.** The owner or any agent may append a
  `Proposed` entry to `docs/feature-requests.md`, but must read the index first and extend an existing
  entry instead of duplicating it. Only `Purpose Refiner` may change status. Never delete or renumber
  entries; rejected requests remain with their reasoning, and new numbers increase monotonically.
- **A namespace change is a binary-breaking change.** Never make one casually; it requires a major
  version bump and a CHANGELOG entry.
- **Affirming an inherited claim is not verifying it.** Before restating any existing claim in a
  README, `AGENTS.md`, or doc as still accurate, open the artifact it describes. A claim that has
  survived several passes has been *copied* several times, not *checked* several times. Say which
  file you opened.
- **When a hygiene fix has a teaching cost, genericize rather than delete** — replace the
  machine-specific value, keep the artifact — and record the declined option in the
  `docs/feature-requests.md` entry so it is not re-proposed later as unfinished work.
- Respect the family split above. `ProphetsWay.EFTools` living outside `ProphetsWay.Utilities`
  is correct, not drift.
- Deviations from these conventions are listed per-repo below. They are known, not overlooked —
  do not re-report them as discoveries.

<!-- ═══════════════════════════════════════════════════════════════════════
     END SHARED BLOCK
     ═══════════════════════════════════════════════════════════════════════ -->

---

## This Repo

**Family:** Data Access · **Published:** yes, as `ProphetsWay.EFTools`

The published 2.2.0 line implements `ProphetsWay.BaseDataAccess` 2.5.0 with abstract Entity Framework
DAO, context, and DAL bases. It ships two implementations selected by target framework: EF6 6.5.1
on .NET Framework and EF Core on .NET 8/9. The branches differ in API and behavior; notably,
EF6 `Update` is an upsert while EF Core requires an existing row.

**The working tree is no longer that package, and it is no longer transitional either.** All three
projects target `net10.0` alone and reference `ProphetsWay.BaseDataAccess` **3.2.0** — opened 2026-08-23:
`ProphetsWay.EFTools/ProphetsWay.EFTools.csproj` and
`ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj` both read
`<PackageReference Include="ProphetsWay.BaseDataAccess" Version="3.2.0" />`. **Every claim of 3.1.0
anywhere in this workspace is superseded; do not restate it.** No project references EF6,
`BaseEFDataAccess` implements the 3.x disposal contract, and **the library is now exactly the
twelve-class 3.0.0 surface plus one enum and two internal helpers — nothing else.** `CHANGELOG.md` opens
with a **v3.0.0** entry describing it, whose own heading reads *"ProphetsWay.BaseDataAccess moves from
2.5.0 to 3.2.0."*

**The version mismatch this section used to record as an open owner decision is CLOSED.**
`app-variables.yml` reads `Major: '3'` / `Minor: '0'` / `Patch: '0'` — verified by opening it on
2026-08-22. **Never change it; the bump is the owner's, and it has been taken.** What remains open is
narrower and is not a decision: **3.0.0 is set but not yet tagged or published, and nuget.org still serves
2.2.0.** So a consumer reading the listing still sees the EF6-and-`net4x` package described in the
paragraph above. Do not restate `2` / `2` / `0`, and do not describe the tree and the version file as
disagreeing.

### Azure SQL Certification Fixture — deployed 2026-09-06

`infra/` contains `bicepconfig.json`, `group.bicep`, `example.solution.bicep`, and `README.md`.
Under **D-033**, the owner authors and manually deploys the fixture while agents review and document it;
**D-034** records that owner deployment `deploy-sql-manually-ggn8` succeeded in `westus`. The live shape is
a dedicated resource group and logical server, the `ProphetsWay.Example` database online at Basic (5 DTUs,
2 GiB), one exact-address firewall rule, and a dedicated security-enabled, non-mail-enabled Microsoft Entra
administrator Group. Do not reproduce protected IDs or the client address. **Gate 2 remains open and
release-blocking:** DACPAC publication, connection and authentication configuration, and the Azure whole-suite
test run are still pending. Source-control policy for the current live-value defaults is also unresolved.

### 3.x Progress — the four implementation laps landed, and four more commits landed on 2026-08-23

The owner approved an EF Core-only, relational-provider-neutral 3.x redesign with generic
root-namespace DAO families and no compatibility wrappers (**D1**, **D2**, **D3**, **S3**). SQLite
in-memory will be the fast CI leg, with a SQL Server container for provider fidelity (**D4**).

**Landed before the laps — 2026-08-16.** The `ProphetsWay.Example` submodule was advanced onto the 3.x
contracts; all three projects were retargeted to `net10.0`; the `ProphetsWay.BaseDataAccess` reference
moved 2.5.0 → 3.1.0; the EF6 conditional `ItemGroup`s and the unused FluentAssertions reference were
removed; and `BaseEFDataAccess` gained its `Dispose`. **Do not restate the "six test adapters were
deleted" line that used to sit here** — thirteen new adapters replaced them the same night (`8e78b94`)
and the test project has grown continuously since.

**Landed in laps 1–4 — verified 2026-08-22 by listing `ProphetsWay.EFTools/` and grepping every type
declaration in it, not by inheriting a claim:**

| Lap | Commit | What landed |
| --- | --- | --- |
| 1 | `e52ee43` | Open-key `BaseDao<TEntity,TKey>` / `BaseGetAllDao<TEntity,TKey>` / `BasePagedDao<TEntity,TKey>` with an **unconstrained `TKey`**, so `string` and `int?` are legal keys; total default ordering |
| 2 | `52a2edf` | `BaseSoftDao` / `BaseSoftGetAllDao` / `BaseSoftPagedDao<TEntity,TKey>` plus `internal static SoftTimestamps`; `DepartmentDao` converted onto `BaseSoftPagedDao<Department,int>` |
| 3 | `10d93f0` → `fdc6e95` | `RootNonIdDao` / `BaseNonIdDao` / `RootSoftNonIdDao` / `BaseSoftNonIdDao`; `internal static EntityGraph` extracted from `BaseDao`'s privates; three 2.2.x keyless types renamed `Legacy*` to free their names; a real `CompanyResourceDao` on `RootNonIdDao<CompanyResource>`; the F10 post-save write-back fix |
| **4** | **`d00aad3`** | **The deletion lap — 24 files, 773 lines, and *no modification to any surviving file*.** Removed the 18 key-specific closures and with them the `ProphetsWay.EFTools.Guid` / `.Int` / `.Long` namespaces entirely; `RootBaseDao<T,TIdType>`; `RootBaseSoftDao<T,TIdType>`; the internal `RootDao<T,TIdType>`; and `LegacyRootNonIdDao<T>` / `LegacyBaseNonIdDao<T>` / `LegacyBaseSoftNonIdDao<T>` |

**`d00aad3` is no longer HEAD.** It was HEAD on 2026-08-22, with the subject *"Delete the 2.2.x DAO
surface superseded by the open-key families."* Its file-by-file stat was supplied by the owner and could
not be re-run here (no terminal); **its effect was verified directly against the tree**, which is the
stronger check. Since then a documentation pull (`b18052a`) and four further commits have landed.

#### Landed 2026-08-23 — four commits, all pushed. **HEAD is `a9e8199`**

Subjects, order and hashes read from `.git/logs/HEAD` on 2026-08-23. Each row's *effect* was re-verified
against the tree by opening or grepping the artifact named — not carried from a handoff.

| Commit | Subject | Effect, and how it was checked |
| --- | --- | --- |
| `0da679f` | *Take BaseDataAccess 3.2.0 and delete the CS8766 suppressions* | The dependency moved `3.1.0` → `3.2.0` in both consuming `.csproj`, and **all ten `CS8766` suppression pairs were deleted across eight library files.** **There are now zero `#pragma` directives in the library** — all 15 files grepped for `#pragma` / `#if` / `#else` / `#elif` / `#endif` / `#region` / `#define` on 2026-08-23, which returns **only** twelve `#nullable enable` lines. The suppressions were load-bearing rather than noise: 3.1.0 shipped **no** nullable metadata, so `IBaseDao<T>` was compiled null-oblivious against implementations declaring `where TEntity : class`; 3.2.0 annotates it, and the compiler now **verifies** what the pragmas had been silencing |
| `77cfe5c` | *Stop forcing EF providers on consumers* | `Microsoft.EntityFrameworkCore.SqlServer` and `.InMemory` removed from `ProphetsWay.EFTools.csproj`, which now references `Microsoft.EntityFrameworkCore` 10.0.11 and `ProphetsWay.BaseDataAccess` 3.2.0 and **nothing else** — opened 2026-08-23; the `ItemGroup` carries the comment *"Provider-neutral by design: a consumer chooses their own EF provider. Never add one here."* `.InMemory` moved into `ProphetsWay.EFTools.Tests.csproj` as a test-only reference beside `.Sqlite`. **Breaking, and deliberately landed before the 3.0.0 tag** — removing a transitive reference after publishing would have cost a 4.0.0. This closes **FR 7 / D2**, both halves |
| `3ac9615` | *Load navigations in the EF UserDao and TransactionDao* | `ApplyIncludes` overrides added to the proving ground's `UserDao` (Company, Job, Department — `UserDao.cs` line 25) and `TransactionDao` (Company, plus User with its own three through `ThenInclude` — `TransactionDao.cs` lines 39–45). **Cleared nine `Scope=Contract` failures that were all `NullReferenceException`**: the seam was never overridden, so `Get` returned null navigations and the tests died before asserting anything about snapshotting. **A proving-ground wiring gap, never a library defect** |
| `a9e8199` | *Declare the EF store's capabilities to the upstream suite* | Submodule pointer advanced to `f93f0a4`, and `TestSeam.cs` now passes a second argument — `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess, StoreCapabilities.TransactionIsolation)`, read from the file on 2026-08-23. **This is what took the suite to 270 / 270 / 0** and removed roughly thirty seconds of SQL Server lock-wait |

**Two of those hashes are easy to swap, and a task packet circulated on 2026-08-23 did swap them.** It
attributed the `ApplyIncludes` work to `77cfe5c`. `.git/logs/HEAD` says otherwise: `77cfe5c` is the
provider removal and `3ac9615` is the `ApplyIncludes` commit. **Cite the reflog, not the packet.**

#### The capability mechanism — describe it, not just the counts

Upstream added a `[Flags]` enum, `StoreCapabilities` — `None`, `DenormalizedNavigationWrites`,
`TransactionIsolation` — so an implementation **declares what its store can structurally do** and two
`Scope=Characterization` tests branch on the declaration instead of failing forever. Read from
`ProphetsWay.Example/ProphetsWay.Example.Tests/StoreCapabilities.cs` and `TestDataAccessFactory.cs`.

**The EF DAL declares `TransactionIsolation` only, and the *absence* of `DenormalizedNavigationWrites` is
itself a positive declaration** — a normalized relational store cannot hold a second, denormalized copy of
a navigation node. **The branch that absence selects is the *stronger* assertion**: it proves `Update` did
not cascade into the Company, Job and Department rows the caller never named. Getting this backwards and
"fixing" it by adding the flag would assert an in-memory outcome against a relational store and fail.

**Two tests fail permanently and correctly under any other arrangement, and nobody should try to fix
them.** Both are `Scope=Characterization`, both are outside the conformance gate by design, and neither
can pass for a relational store:

- `SnapshotDeepCopyTests.ShouldReadANavigationPropertyEditBackOnlyWhereTheStoreDenormalizesTheWrite` — its
  sibling asserts the **opposite** as `Contract`, so for a normalized store the two are mutually exclusive.
- `DataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstanceOnlyWhereTheStoreDoesNotIsolateThem`
  — passing would require `READ UNCOMMITTED`.

Under the capability mechanism **both now pass**, by asserting the correct outcome for a *declared*
relational store. Neither flag is a way out of a rule: no `Scope=Contract` assertion reads either of them.
**The sentence this file used to carry — that
`EFDataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstance` is an outstanding SQL Server
transaction-visibility timeout — is dead.**

**The target surface is complete and nothing else is in the library.** `docs/api-contract.md`
(revision 11) § *The Public Surface* names twelve public classes plus the `ContextOwnership` enum —
`BaseEFContext`, `BaseEFDataAccess<TContext>`, the three keyed families, the three soft keyed families,
and the four keyless types. Every one is in the tree, **and after lap 4 that list is now exhaustive**:
listing `ProphetsWay.EFTools/` gives **15 `.cs` files in one flat folder**, and grepping every type
declaration in them returns **exactly 15 top-level types — 12 public abstract classes, 1 public enum,
2 internal static classes**. The contract document's *"Types that disappear"* list has disappeared.
**Two sentences are dead and must not be restated: "none of the twelve-class redesign has landed", and
anything of the form "24 of the library's 39 source files are the 2.2.x shape lap 4 deletes."**

**There is no conditionally compiled code left in the library.** Grepping all 15 files for `#if`,
`#else`, `#elif`, `#endif`, `#region`, `#endregion` and `#define` returns **nothing**. The dead
`#if NET461 || NET471 || NET48` blocks went out with the 24 files that carried them, exactly as planned —
one removal, not two. `CHANGELOG.md`'s v3.0.0 entry states the same thing independently. **Do not report
them as surviving.**

**What has *not* landed — re-derived 2026-08-23. Three items this list used to carry are closed.**

- ~~**Provider neutrality (FR 7 / D2) — the packaging half.**~~ **CLOSED by `77cfe5c`.** Both halves are
  done; the library references no provider at all. See deviation 3.
- ~~**A green suite.**~~ **CLOSED.** 270 / 270 / 0. See deviation 4.
- **The SQLite / SQL Server certification legs (FR 11).** Both providers are in real use — SQLite
  in-memory by seven of the eight locally written classes, SQL Server by the thirteen adapted upstream
  ones — but no *certified* run of the **whole** suite on **either** provider alone exists.
  `Purpose Refiner` narrowed FR 11 on 2026-08-23 and recorded it **not release-blocking**.
- **Packaging metadata and Source Link (deviation 5, now also FR 16).** No SourceLink,
  `RepositoryType` is `GitHub` where the convention is `git`, and five metadata properties are empty
  stubs. **Not breaking**, so it can land after 3.0.0.
- **FR 17 — dead preprocessor guards in the proving ground.** Five DAOs in
  `ProphetsWay.Example.DataAccess.EF` open with `#if NET8_0_OR_GREATER` / `#if NET471 || NET48` blocks
  under a `net10.0`-only project — `CompanyDao`, `JobDao`, `ResourceDao`, `TransactionDao`, `UserDao`,
  grepped 2026-08-23. **This is scoped to the proving ground, not the library.** The library's
  zero-directives claim above is true and must not be contradicted by this row.
- **FR 18 — the committed `.trx` files under `ProphetsWay.EFTools.Tests/TestResults/` read authoritative
  and are stale.** `eftools-verify-20260823.trx` included: its run predates `a9e8199`. **Never cite a
  `.trx` in this repository as current.**
- **`LocalTestsOnly: 'yes'` — the justification has decayed.** CI still executes none of the 270. The
  recorded reason is a local SQL Server dependency, but the conformance gate now runs in about two
  seconds and most locally written classes use SQLite in-memory and need no server at all.
- **The tag and the publish.** 3.0.0 is set in `app-variables.yml` and not yet released; nuget.org still
  serves 2.2.0.

**FR 15 is closed by this lap, as a side effect.** `ProphetsWay.EFTools.Guid` shadowed `System.Guid` in
any file importing it; the namespace no longer exists, so the collision cannot occur.
[docs/feature-requests.md](docs/feature-requests.md) is **not** updated to say so — only `Purpose Refiner`
may change an entry's status. Recorded here so the next agent does not re-derive it.

**Build and test state — 2026-08-23, after all four commits. Reported by the owner; not re-run here.**
`dotnet build` **0 errors / 0 warnings**. Full `ProphetsWay.EFTools.Tests` **270 total / 270 passed /
0 failed**. Conformance gate `--filter "Scope=Contract|Guard=Seam"` **245 / 245 / 0 in about two
seconds**; the full run fell from roughly thirty-two seconds to about two.

**Both totals are independently corroborated by static count here, which is the strongest check available
without a terminal.** Counted 2026-08-23 by grepping every `[Fact]`, `[Theory]`, `[InlineData]` and
`[Trait]` in `ProphetsWay.EFTools.Tests/` and in the submodule's `ProphetsWay.Example.Tests/`: **126 cases
declared here** (88 `[Fact]` + 38 `[InlineData]`) plus **144 adapted upstream** (129 `[Fact]` + 15
`[InlineData]` across the thirteen adapted classes) = **270**. Of those, `Scope=Contract` is 238 and
`Guard=Seam` is 7 — **245**, exactly the gate figure — leaving `Scope=Characterization` 24 and
`Scope=Dispatcher` 1. The four numbers sum to 270, and that sum is the check.

**Do not restate any of these superseded figures:** *7 warnings*, *151 / 123 / 28*, *270 / 259 / 11*, or
the lap gate *68 / 68* as if it were the current gate — `--filter "Area=Keyless|Area=Insert"` was lap 4's
gate; the release gate is the 245.

### Ratified TFM Exception — this repo targets `net10.0` only

**This repo is exempt from the shared block's `netstandard2.0;net10.0` default.** Owner decision
**D7**, recorded in [docs/purpose-and-scope.md](docs/purpose-and-scope.md#owner-decisions--2026-08-15),
ratifies **`net10.0` alone** as the approved target state for the library,
`ProphetsWay.EFTools.Tests`, and `ProphetsWay.Example.DataAccess.EF` alike. The test project
therefore also loses its `net48` leg. Existing `net4x` / `net8.0` / `net9.0` consumers stay on the
published 2.2.x line.

Do not "correct" this repo toward the house default, and do not re-derive the reasoning — it is
settled in that document under **The `net10.0`-Only Exception**. **The tree now matches the approved
target state**: all three projects read `<TargetFrameworks>net10.0</TargetFrameworks>` as of
2026-08-16. Earlier text here called the tree's TFM list debt; that is no longer true.

### Layout

Counts re-taken 2026-08-22, **after lap 4**, by listing each directory. **The library figure changed again;
both the previous 27 and the previous 39 are superseded, and so is the 21 / 6 / 6 / 6 split — there are no
subfolders left to split across.**

| Project | Role |
| --- | --- |
| `ProphetsWay.EFTools/` | Published library; **15 source files, one flat folder, no subfolders.** `Guid/`, `Int/` and `Long/` are gone with `d00aad3`. One top-level type per file: **12 public abstract classes, 1 public enum (`ContextOwnership`), 2 internal static classes (`EntityGraph`, `SoftTimestamps`)** — i.e. the 3.x surface and nothing else. **Zero preprocessor directives library-wide** |
| `ProphetsWay.Example.DataAccess.EF/` | Non-packaged EF proving-ground implementation; **9 source files, 7 of them DAOs** — `CompanyResourceDao.cs` and `DepartmentDao.cs` both exist now |
| `ProphetsWay.EFTools.Tests/` | **25 source files**, re-listed 2026-08-23 — 13 one-line adapters, **8 test classes written directly against this library's own surface**, 2 seam guards, `TestSeam.cs` and `Constants.cs`. **The figure of 7 that this row and deviation 4 both once carried was wrong** — the eighth is `CompanyResourceConversionTests`. See deviation 4 |
| `ProphetsWay.Example/` | Git submodule tracking the standalone Example repo; never edit it here |

The submodule pointer is at **`f93f0a4`** — `f93f0a41a76834647962ddf9e830e01e24e05f24`, read from
`.git/modules/ProphetsWay.Example/HEAD` on 2026-08-23, advanced by `a9e8199` the same day. **It is not the
`3.1.0` tag** — `git submodule status` describes it as `3.1.0-4-gf93f0a4`, i.e. **four** commits past that
tag, on the open **3.1.1** line. Those commits brought in `TestDataAccessFactory.Use` and the
`StoreCapabilities` enum the harness now depends on. **Every earlier pointer this file has carried —
`967fd26`, `d845863`, `61d9e7d` — is superseded and must not be restated**, and neither may any
description of the pointer as sitting on a tagged release.

The solution also includes four projects from the submodule: DataAccess, DataAccess.NoDB, Tests, and
the Database project — which is **SDK-style `Microsoft.Build.Sql/2.2.0`**, not the legacy SSDT format
this file once named. The pipeline builds
`**/*.csproj`, not the solution;
with `HasSqlProj` commented out in `app-variables.yml`, it does not build the `.sqlproj`.
`LocalTestsOnly: 'yes'` skips every test in CI. All three pipeline facts re-verified against
`app-variables.yml`, `local-pipeline.yml`, and `prophets-pipelines/steps/restore-build-test.yml`.
The 2026-08-16 build also measured two things the pipeline never exercises: the SDK-style `.sqlproj`
**builds under the .NET CLI**, producing `ProphetsWay.Example.Database.dacpac`, and
`ProphetsWay.Example.Tests` compiled on **both** legs — which is what the 164 tests / 328 executions
figure rests on.

**The four upstream `xUnit1013` warnings this section used to describe are gone with the rest.** The
2026-08-16 build emitted 7 warnings, four of them `xUnit1013` on the submodule's `DepartmentDaoTests.cs`
and three of them the `Submod` Source Link warning. The 2026-08-22 build reports **0 warnings**, so both
sets are closed — the submodule pointer has not moved, so the upstream helpers were fixed on that side.
**Do not re-report either as outstanding.**

### Key Types

**One shape. There is no longer a second.** Lap 4 (`d00aad3`) removed the 2.2.x surface outright, so the
table below is the **whole** library — 12 public abstract classes, 1 public enum, 2 internal static helpers,
15 files, 15 top-level types. Everything below was read from the file named, on 2026-08-22, after lap 4.
**The section that used to sit here was split into "the 3.x surface" and "the 2.2.x surface — still present";
that framing is dead and must not be reinstated.**

#### The whole library — twelve public classes, one enum, two internal helpers

| Type | Kind | Role |
| --- | --- | --- |
| `BaseEFContext` | public abstract class | **Provider-free.** One `protected BaseEFContext(DbContextOptions)` constructor and nothing else — no members, no `UseSqlServer`, no string constructor. **The claim that "Core string construction hardcodes SQL Server" is dead**; deriving from it is now optional, because `BaseEFDataAccess<TContext>` constrains to `DbContext` |
| `BaseEFDataAccess<TContext>` | public abstract class | The DAL root. **One type parameter, not two** — `BaseEFDataAccess<TContextType,TIdType>` no longer exists. **The claim this row used to carry — that `README.md` still says it does — is FALSE and is struck.** `README.md` was grepped for `BaseEFDataAccess` on 2026-08-23: all six hits read `BaseEFDataAccess<TContext>` or the bare name, and none names a second type parameter. It was this file's own stale claim about another file. Takes a built context plus a `ContextOwnership` through one `protected` constructor; both public constructors are gone. `Dispose` is **`sealed override`**, idempotent, non-throwing, rolls back an open transaction and disposes the context only when `Owned`; `protected virtual DisposeCore()` is the derived hook. **`ObjectDisposedException` guarding is complete** — all **10** non-`Dispose` members (`TransactionStart`/`Commit`/`RollBack`, `GetAll`, `GetPaged`, `GetCount`, `Get`, `Insert`, `Update`, `Delete`) open with `ThrowIfDisposed()`, and `protected ThrowIfDisposed()` is exposed so a derived DAL guards its own forwarders. **The claim that guarding "is not yet in place" is dead** |
| `ContextOwnership` | public enum | `Borrowed` / `Owned`, no default — the caller states who disposes the context |
| `BaseDao<TEntity,TKey>` | public abstract class | Keyed CRUD. **`TKey` is unconstrained**, so `string` and `int?` are legal. Identifier resolved by name — `{TypeName}Id` then `Id` — validated in the constructor, and it must be a **public instance** property; an explicit `IBaseIdEntity<T>` implementation does not satisfy it |
| `BaseGetAllDao<TEntity,TKey>` / `BasePagedDao<TEntity,TKey>` | public abstract classes | Add `IBaseGetAllDao<T>` / `IBasePagedDao<T>` over `BaseDao` |
| `BaseSoftDao<TEntity,TKey>` | public abstract class | Soft-delete semantics as `override`s, never `new`, so an upcast still soft-deletes. `ApplyReadFilter` adds `DeletedDate == null` and nothing else |
| `BaseSoftGetAllDao` / `BaseSoftPagedDao<TEntity,TKey>` | public abstract classes | Soft plus the corresponding capability interface |
| `RootNonIdDao<TEntity>` | public abstract class | Keyless plumbing that **implements no capability interface**, so deriving from it commits to nothing. `MatchRow` is its one abstract member and **the only abstract member in the library** — re-verified after lap 4 by grepping every `protected`/`public` member declaration in all 15 files; `BaseDao.MatchRow` is `protected virtual`, `RootNonIdDao.MatchRow` is `protected abstract`, and nothing else is abstract. `ApplyStableOrder` throws until overridden. **`GetCore` and `UpdateCore` are declared here, not on `BaseDao`** — `RootNonIdDao.cs` lines 308 and 342, overridden in `RootSoftNonIdDao.cs` at 201 and 229. `BaseDao`'s own protected seams are `TrackForWrite`, `ApplyUpdateValues`, `GetKey`, `MatchRow`, `KeyEquals`, **`KeySelector`**, `ApplyReadFilter`, `ApplyIncludes` and `ApplyStableOrder` — and **no `GetCore` or `UpdateCore`**. **`KeySelector` was missing from this list until 2026-08-23 and its omission was this file's error**: it is `protected virtual Expression<Func<TEntity, TKey?>> KeySelector` at `BaseDao.cs` line 501, and `README.md` line 341 has listed it correctly all along |
| `BaseNonIdDao<TEntity>` | public abstract class | `RootNonIdDao` + `IBaseDao<TEntity>` |
| `RootSoftNonIdDao<TEntity>` / `BaseSoftNonIdDao<TEntity>` | public abstract classes | Keyless soft delete, without and with `IBaseDao<TEntity>` |
| `EntityGraph`, `SoftTimestamps` | internal static classes | The one copy of the navigation-graph mechanics and of the soft-timestamp policy. They exist because the keyed and keyless families are **unrelated inheritance branches** and cannot share a declaration — do not "simplify" either into a base class |

#### Removed by lap 4 (`d00aad3`) — recorded, not silently dropped

This table is **history**. Nothing in it is in the tree; **never write new code against any of it, and never
re-report any of it as present.** It is kept because a reader comparing this repository to the published
2.2.0 package, or to any alpha/beta cut taken mid-flight, will meet these names and needs to know where they
went.

| Type | Was | Fate |
| --- | --- | --- |
| `Guid.*`, `Int.*`, `Long.*` DAO bases | 18 public abstract classes — 6 per namespace | **Deleted.** The `ProphetsWay.EFTools.Guid`, `.Int` and `.Long` namespaces no longer exist at all. Replaced by the open-key families: `Int.BaseDao<T>` → `BaseDao<T,int>`, `Guid.BaseSoftPagedDao<T>` → `BaseSoftPagedDao<T,Guid>`, and so on through all eighteen |
| `RootBaseDao<T,TIdType>` / `RootBaseSoftDao<T,TIdType>` | public abstract bridges, `[EditorBrowsable(Never)]`, `where TIdType : struct` | **Deleted.** That `struct` constraint is the one the open-key families exist to drop |
| `RootDao<T,TIdType>`, `LegacyRootNonIdDao<T>` | internal classes — the 2.2.x engines | **Deleted.** `RootDao`'s body is absorbed by the bases it served |
| `LegacyBaseNonIdDao<T>` / `LegacyBaseSoftNonIdDao<T>` | public abstract classes | **Deleted.** They existed for three commits only, renamed in lap 3 purely to free `BaseNonIdDao` / `BaseSoftNonIdDao` for the new types. `CHANGELOG.md` v3.0.0 says in terms that **none of them is part of 3.0.0** and there is no `LegacyBaseNonIdDao<T>` to migrate onto |

All 24 of these files carried `using System.Data.Entity;` inside a dead `#if NET461 || NET471 || NET48`
block, and **they were the only files that did.** The `#if` blocks and the files went in one removal, as
planned. **There are now zero preprocessor directives in the library** — grepped across all 15 remaining
files, and stated independently by `CHANGELOG.md`.

**One name survives the deletion with a different meaning.** 2.2.x had an `internal RootNonIdDao<T>`
engine; the tree has a `public RootNonIdDao<TEntity>` extension point. `docs/api-contract.md` flags these
as **different types that happen to share a name** — the internal one did not survive, its body was
absorbed, and its visibility, role, members and `MatchRow` contract all changed. Describe it as new public
surface, never as a promotion.

**[docs/api-contract.md](docs/api-contract.md) and the tree now agree completely.** Read the contract
document as the specification and the table above as the tree; after lap 4 there is no remaining
divergence between them, where before there was the whole *"Types that disappear"* list. **The sentence
"the contract document's *Types that disappear* list is still entirely present" is dead.**

**In `ProphetsWay.Example.DataAccess.EF`, all seven DAOs are on the 3.x families** — `CompanyDao :
BasePagedDao<Company,int>`, `JobDao : BaseGetAllDao<Job,int>`, `ResourceDao : BaseGetAllDao<Resource,Guid>`,
`TransactionDao : BasePagedDao<Transaction,long>`, `UserDao : BaseDao<User,int>`, `DepartmentDao :
BaseSoftPagedDao<Department,int>`, `CompanyResourceDao : RootNonIdDao<CompanyResource>`. **Nothing in the
proving ground derives from a 2.2.x type any more**, which is what makes lap 4 a deletion rather than a
migration. Two sentences that used to sit in this file are dead: *"`DepartmentDao` derives from no EFTools
base at all"* and *"there is no `CompanyResourceDao`"*.

### Known Deviations

| # | Deviation | Severity / notes |
| --- | --- | --- |
| 1 | ~~Package and EF example reference `ProphetsWay.BaseDataAccess` 2.5.0~~ **CLOSED 2026-08-16, and moved again 2026-08-23** | **The reference is now 3.2.0 in both `ProphetsWay.EFTools.csproj` and `ProphetsWay.Example.DataAccess.EF.csproj`** — verified by opening both on 2026-08-23. It was 3.1.0 from 2026-08-16 until `0da679f`; **this row read 3.1.0 until 2026-08-23 and was wrong.** The row is kept rather than deleted because the *published* 2.2.0 package still carries 2.5.0, so a consumer reading nuget.org sees the old contract until 3.0.0 ships. Do not re-report the tree as being on 2.5.0 or on 3.1.0. |
| 2 | ~~Library TFMs are `net461;net471;net48;net80;net90`~~ **CLOSED 2026-08-16** | **All three projects now read `net10.0` alone** — the library, `ProphetsWay.EFTools.Tests` and `ProphetsWay.Example.DataAccess.EF` — verified by opening each `.csproj`. That is the approved target state under **D7**, a ratified exception to the house standard, **not drift**: see the section above and D7 in [docs/purpose-and-scope.md](docs/purpose-and-scope.md#owner-decisions--2026-08-15). Do not "fix" it toward `netstandard2.0;net10.0`, and do not re-report the old `net4*`/`net80`/`net90` lists as current. |
| 3 | ~~Runtime package forces SQL Server and InMemory providers~~ **CLOSED 2026-08-23 — both halves** | **Was Medium; now closed, and the row is kept only so the record survives.** The **code** half closed earlier: grepping every `.cs` in `ProphetsWay.EFTools/` for `UseSqlServer` / `UseInMemoryDatabase` returns **nothing**, `BaseEFContext` has one `protected BaseEFContext(DbContextOptions)` constructor and names no provider, and `BaseEFDataAccess<TContext>` takes a built context — the provider is named in the consumer's own file (`ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs`). The **packaging** half closed in `77cfe5c` on 2026-08-23: `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.InMemory` are **gone** from `ProphetsWay.EFTools.csproj`, verified by opening it — the library's entire reference list is now `Microsoft.EntityFrameworkCore` 10.0.11 and `ProphetsWay.BaseDataAccess` 3.2.0, so what a consumer restores transitively is those two plus `.Abstractions` and `.Analyzers` beneath them. `.InMemory` moved into `ProphetsWay.EFTools.Tests.csproj` as a test-only reference beside `.Sqlite`. **Removing a transitive reference is breaking**, which is why it landed **before** the 3.0.0 tag rather than after — after publishing it would have cost a 4.0.0. This deviation **is** [FR 7](docs/feature-requests.md) / **D2**, now `Done`. **Do not restate "the packaging half is not done."** |
| 4 | ~~**There is no test suite in this repository at all**~~ · ~~**19 files, 151 cases, nothing tests this library's own surface**~~ ~~**270 / 259 / 11**~~ **ALL THREE SUPERSEDED 2026-08-23 — 25 files, 270 cases, 126 of them targeting this library directly, and the suite is green** | **Low, and now only about CI and certification.** **Do not restate either struck heading, and do not restate 151 / 123 / 28.** Re-counted 2026-08-22 by listing `ProphetsWay.EFTools.Tests/` and grepping every `[Fact]`, `[Theory]`, `[InlineData]` and `[Trait]` in it and in `ProphetsWay.Example.Tests/`. **25 source files:** `TestSeam.cs`, an `internal static` class whose `[ModuleInitializer]` calls `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess)` and so points the **whole upstream `ProphetsWay.Example` suite** at the EF DAL for the assembly's run; **13 adapters**, each literally `public class EFXxxTests : XxxTests { }`; **2 seam guards** — `TestSeamTests.cs` (5 `[Fact]`) and `AdapterCoverageTests.cs` (2 `[Fact]`), both `[Trait("Guard","Seam")]`; **8 test classes written directly against this library's own public surface** — **the figure read `7` until 2026-08-22 and was wrong against its own list, which named eight** — `KeylessDaoTests` (31 cases), `KeylessSoftDaoTests` (29), `KeyPredicateOpenKeyTests` (21), `AlternateKeyGuardSpikeTests` (14), `SoftDeleteTimestampHookTests` (12), `FailedInsertWriteBackTests` (5), `IdentifierResolutionTests` (4), `CompanyResourceConversionTests` (3), each declaring its own entities, `DbContext` and DAOs on `BaseDao` / `BaseSoftDao` / `RootNonIdDao` / `RootSoftNonIdDao` / `BaseEFDataAccess`; and `Constants.cs`, one SQL Server connection string with `TrustServerCertificate=True` and **no `#if` branches**. **Discoverable cases: 270** — **144** from the 13 adapted upstream classes (the 20 `ConventionShowcase` cases are correctly *not* adapted, leaving 164 − 20) plus **126** declared here. Both halves were counted independently and they reconcile with the runner exactly, which is why this row states them without a terminal. **Trait split of the local 126:** `Scope=Contract` 99, `Characterization` 19, `Dispatcher` 1, `Guard=Seam` 7 — **every local case carries exactly one `Scope` or the `Guard` key.** By `Area`: `Keyless` 64, `KeyPredicate` 25, `AlternateKeys` 14, `SoftDelete` 12, `Insert` 4; `Keyless` + `Insert` = **68**, which is the lap gate the owner reported passing 68/68. **Last measured result: 2026-08-23, reported by the owner after all four of that day's commits — 270 total, 270 passed, 0 failed, and the conformance gate `--filter "Scope=Contract\|Guard=Seam"` 245 / 245 / 0 in about two seconds, the full run having fallen from roughly thirty-two seconds to about two.** Not re-run here, but **both totals reconcile exactly with the static count above**, which is the strongest check available without a terminal. The last eleven failures were cleared by two of that day's commits: `3ac9615` supplied the missing `ApplyIncludes` overrides in the proving ground, clearing nine `NullReferenceException`s inside `EFSnapshotDeepCopyTests`; `a9e8199` declared `StoreCapabilities.TransactionIsolation`, which lets the remaining two — `ShouldReadANavigationPropertyEditBackOnlyWhereTheStoreDenormalizesTheWrite` and `ShouldExposeUncommittedWritesToAnotherInstanceOnlyWhereTheStoreDoesNotIsolateThem` — assert the outcome correct for a *declared* relational store instead of failing forever. **Neither was ever a library defect, and neither may be "fixed"** — see the capability mechanism above. **Do not restate 270 / 259 / 11, 151 / 123 / 28, or "11 failures remain."** The `.trx` files in `TestResults/` are **all stale, the newest included** — `eftools-verify-20260823.trx` reads authoritative and its run predates `a9e8199`; the older six are laps from 2026-08-18 recording 147/53/94, 147/15/132 and 151/57/94. **Never cite a `.trx` in this repository as current.** Filed as [FR 18](docs/feature-requests.md). **What is now covered that this row said was not:** the open-key, soft-delete and keyless families are all exercised directly, and `CompanyResourceDao` **exists** — `ProphetsWay.Example.DataAccess.EF/Daos/CompanyResourceDao.cs` is a real `RootNonIdDao<CompanyResource>`, `ExampleContext` declares `DbSet<CompanyResource>` with `HasKey(x => new { x.CompanyId, x.ResourceId })` and `ToTable("CompanyResources")`, and the three `ICompanyResourceDao` forwarders call it behind `ThrowIfDisposed()`. **The claim that it "does not exist" and dominates the failures is dead.** **What is still not covered:** provider portability is not *certified* — but the sentence this row used to carry, *"the suite … runs on that provider alone, despite the `Microsoft.EntityFrameworkCore.Sqlite` reference in the test `.csproj`"*, **is false and is struck.** Grepping the whole test project for `UseSqlite` / `UseSqlServer` / `UseInMemoryDatabase` on 2026-08-22 shows **seven of the eight locally written classes build their own SQLite in-memory context** — `KeylessDaoTests`, `KeylessSoftDaoTests`, `KeyPredicateOpenKeyTests`, `SoftDeleteTimestampHookTests`, `FailedInsertWriteBackTests`, `IdentifierResolutionTests`, and `AlternateKeyGuardSpikeTests` (which parameterizes `InMemory` **and** `Sqlite` per `[Theory]`). The eighth, `CompanyResourceConversionTests`, is **reflection-only and needs no database at all** — its own `<remarks>` say "no store, no connection, no model." The Sqlite `PackageReference` is in **active use**, and the test `.csproj` comments it as *"Relational provider for the SQLite in-memory certification leg; must not move into the library."* What actually reaches SQL Server is the **13 adapted upstream classes**, through `Constants.GetExampleDataAccess`, which needs a **local SQL Server** carrying `ProphetsWay.Example`. So FR 11's remaining work is a *certified* run of the whole suite — and `Purpose Refiner` narrowed it on 2026-08-23 to *a run of the whole suite on **either** provider*, recording it **not release-blocking** ([FR 11](docs/feature-requests.md)). Also still true, and now weaker than its own justification: `LocalTestsOnly: 'yes'` skips the whole suite in CI, so none of the 270 runs on a build agent — including the SQLite ones, which need no server, and the 245-case gate, which now completes in about two seconds. **Closed by lap 4:** the sentence *"the 24 legacy files lap 4 deletes are exercised by nothing"* — those files no longer exist, so there is nothing untested about them. |
| 5 | Package homepage/tags and source/symbol/reproducibility settings are empty or missing — **still open; now also [FR 16](docs/feature-requests.md)** | **Medium, and explicitly not breaking**, so it may land after 3.0.0. Re-verified field by field by opening `ProphetsWay.EFTools.csproj` on **2026-08-23**. Present with values: `PackageId`, `Description`, `Authors`, `Company`, `Product`, `RepositoryUrl`, `RepositoryType` (`GitHub`, where the convention is `git`), `PackageIcon`, `PackageReadmeFile`, `PackageLicenseExpression`, `PackageRequireLicenseAcceptance`, and the `ItemGroup` packing README, CHANGELOG and `profile.png`. Empty self-closing stubs: `PackageProjectUrl`, `PackageTags`, `PackageReleaseNotes`, `Copyright`, `NeutralLanguage` (plus the pipeline-owned `Version`/`AssemblyVersion`/`FileVersion`/`InformationalVersion`, correctly empty, and the inert `ApplicationIcon`/`Win32Resource`). Absent entirely: SourceLink, `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat`, `ContinuousIntegrationBuild`, `GenerateDocumentationFile`. **Read this row together with deviation 7 or you will conclude Source Link works — it does not.** |
| 6 | ~~`ProphetsWay.Example.DataAccess.EF` references unused FluentAssertions 8.2.0~~ **CLOSED 2026-08-16** | **The reference has been removed** — verified by opening `ProphetsWay.Example.DataAccess.EF.csproj`, whose only `PackageReference` entries are now the two EF Core packages and `ProphetsWay.BaseDataAccess`. The paid-commercial-licence exposure is closed. Kept as a row so the reason it mattered survives: 8.x requires a paid licence and house convention names Shouldly. |
| 7 | ~~`.gitmodules` contains an incomplete `[submodule "Submod"]` block~~ **Severity raised, then closing — 2026-08-16** | **Medium, not cosmetic. This is one of the few entries here backed by a real build rather than reasoning.** `dotnet build ProphetsWay.EFTools.sln -c Debug`, run by the owner on **2026-08-16** (SDK 10.0.400), emitted `Microsoft.Build.Tasks.Git.targets(25,5): warning : The path of submodule 'Submod' is missing or invalid: ''. The source code won't be available via Source Link.` **three times** — once each for `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests`. **The measured cost is that it disables Source Link on a published package**: a consumer of `ProphetsWay.EFTools` cannot step into its source. The earlier **Low / cosmetic** reading was reasoned, not measured, and is superseded. **The block is absent from `.gitmodules` as read on 2026-08-16** — `Modernizer` owns that file and was removing it as this was written — so treat this as a deviation closing with its cost on record. **The warning is gone, and that is now measured rather than expected — 2026-08-22.** The owner's build on that date reports **0 warnings**, where the 2026-08-16 build emitted this one three times. **Removal stopped the warning; it did not give the library Source Link.** Deviation 5 stands unchanged — SourceLink is not referenced at all, and `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat` and `ContinuousIntegrationBuild` are all absent from `ProphetsWay.EFTools.csproj`, re-verified by opening it 2026-08-22. Read the two rows together or you will conclude Source Link works. |
| 8 | ~~**The repository is mid-flight on the 3.x redesign** — three independent breaks~~ **CLOSED 2026-08-16 by a verified-green build** | **All three enumerated breaks are closed, and this is measured rather than reasoned.** `dotnet build ProphetsWay.EFTools.sln -c Debug`, run by the owner on **2026-08-16**, **succeeded** — SDK 10.0.400, 7 warnings, 0 errors, every project compiling: `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests` on `net10.0`; the submodule's `DataAccess` and `DataAccess.NoDB` on `netstandard2.0` and `net10.0`; `ProphetsWay.Example.Tests` on **both** `net48` and `net10.0`; and `ProphetsWay.Example.Database` producing a `.dacpac` **under the .NET CLI**. The row is kept, not deleted, so the record of what broke survives: (a) ~~`ProphetsWay.EFTools.Tests` targeted `net472;net48;net80;net90` against a `ProphetsWay.Example.Tests` retargeted to `net48;net10.0`~~ — closed by the `net10.0` retarget; (b) ~~the same project's six adapters overrode a member that no longer exists upstream~~ — closed by deleting them; (c) ~~`ExampleDataAccess` did not satisfy the 3.1.0 `IExampleDataAccess`~~ — closed by `Implementer` adding the `IDepartmentDao` and `ICompanyResourceDao` member groups as deliberately-throwing "not written yet" stubs, with `Dispose` inherited from `BaseEFDataAccess`. **Compiling is not conforming** — those stubs threw, `ExampleContext` mapped neither new entity, and the verification gap deviation 4 described was untouched by this. **All three of those caveats have since been answered by laps 2 and 3** — both entities are mapped, both DAOs exist, and the suite reaches them; see deviation 4. **The lap-4 deletion of the 24 legacy files has since landed too (`d00aad3`), so the only 3.x work this row still points at is provider *packaging* (deviation 3).** Do not restate "and the lap-4 deletion of the 24 legacy files" as outstanding. |
| 9 | ~~**`AlternateKeyGuardSpikeTests.cs` is committed and carries no trait, so it is outside every gate**~~ **CLOSED 2026-08-22 — it is traited now** | **Low, factual.** The file still exists and still declares **7 `[Theory]` methods × 2 `[InlineData]` (`InMemory`, `Sqlite`) = 14 cases**, but the "zero `[Trait]` attributes" half is **false as of 2026-08-22**: every one of the seven now carries `[Trait("Scope","Characterization")]` and `[Trait("Area","AlternateKeys")]`, verified by grepping every attribute in the file. It is therefore inside `--filter "Scope=Characterization"` and outside `Scope=Contract`, which is the correct placement for something its own `<remarks>` call "an empirical spike, not a specification" — it declares its own `SpikeUser` / `SpikeUniqueUser` / `SpikeContext` and asserts about EF Core, not about this library. **Row kept, not deleted, so the reason the trait matters survives:** xUnit trait filters are allowlists, so an untraited test runs only on a bare `dotnet test`. Whether the file stays at all is still the owner's call. |

`docs/architecture.md`, per-project `docs/requirements.md`, and
`docs/nuget-extraction-proposal.md` are **n/a by owner decision**, not missing documentation.
