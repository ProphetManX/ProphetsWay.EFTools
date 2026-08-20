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

**The working tree is no longer that package.** As of 2026-08-16 all three projects target `net10.0`
alone and reference `ProphetsWay.BaseDataAccess` 3.1.0, nothing references EF6, and `BaseEFDataAccess`
implements the 3.x `Dispose`. `app-variables.yml` still reads `2` / `2` / `0` — **never change it** — so
the published package and the tree describe different things until the owner cuts 3.0.0.

### Planned 3.x Direction — Not Current State

The owner has approved an EF Core-only, relational-provider-neutral 3.x redesign with six generic
root-namespace DAO families and no compatibility wrappers. SQLite in-memory will be the fast CI leg,
with a SQL Server container for provider fidelity. The cycle also adopts `ProphetsWay.BaseDataAccess`
3.1.0. Do not describe those choices as current package behavior.

**Several steps of that cycle have now landed — 2026-08-16.** The `ProphetsWay.Example` submodule was
advanced onto the 3.x contracts; all three projects were retargeted to `net10.0`; the
`ProphetsWay.BaseDataAccess` reference moved 2.5.0 → 3.1.0; the EF6 conditional `ItemGroup`s and the
unused FluentAssertions reference were removed; the six test adapters were deleted; and
`BaseEFDataAccess` gained its `Dispose`. **Verified by opening `ProphetsWay.EFTools.csproj`,
`ProphetsWay.EFTools.Tests.csproj`, `ProphetsWay.Example.DataAccess.EF.csproj`, `BaseEFDataAccess.cs`
and the `ProphetsWay.EFTools.Tests/` directory listing.**

> **Correction, 2026-08-20 — the adapter deletion above is history, not current state.** Thirteen
> adapters were **re-added on 2026-08-16 at ~23:00 (`8e78b94`)**, roughly two and a half hours after
> this file was last committed (`6a328d3`, 2026-08-16T20:24), which is why the paragraph above and the
> Layout table below went stale together. `TestSeam.cs` / `TestSeamTests.cs` followed in `08deda6` and
> `AdapterCoverageTests.cs` in `b9320fd`, both 2026-08-18. **`ProphetsWay.EFTools.Tests` now holds 19
> source files and a working harness** — see the Layout table and deviation 4.

**What has not landed:** provider neutrality (the SQL Server and InMemory references are still in the
library), the collapse of the 18 key-specific DAO classes, the SQLite/SQL Server certification legs,
and the removal of the now-dead `#if NET461 || NET471 || NET48` blocks from the C# sources. Earlier
revisions of this file said the library "still references 2.5.0, still carries its EF6 `#if` branches,
still declares no `Dispose`" — **only the `#if` half of that is still true.** Do not restate the rest.

**The solution builds green as of 2026-08-16** — owner-run `dotnet build ProphetsWay.EFTools.sln -c
Debug`, SDK 10.0.400, 7 warnings, 0 errors. That is the **first verified-green state since the
submodule advance**. It closes deviation 8 and raises deviation 7. **It says the tree compiles, not
that the 3.x redesign is done** — the `ICompanyResourceDao` stubs still throw.

**The tail of that sentence — "and there is still no test suite" — is superseded.** A harness landed
later the same day and was run on 2026-08-18: `dotnet test` reported **151 tests, 123 passed, 28
failed**, recorded in [docs/feature-requests.md](docs/feature-requests.md) entry 13. Read deviation 4
for what that suite is and what it still does not cover.

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

| Project | Role |
| --- | --- |
| `ProphetsWay.EFTools/` | Published library; **27 source files** — 24 public abstract classes, one public enum (`ContextOwnership`), two internal classes |
| `ProphetsWay.Example.DataAccess.EF/` | Non-packaged EF proving-ground implementation; 7 source files, 6 of them DAOs |
| `ProphetsWay.EFTools.Tests/` | **19 source files and a working harness**, not an empty project — 13 one-line adapters, `TestSeam.cs`, `TestSeamTests.cs`, `AdapterCoverageTests.cs`, `AlternateKeyGuardSpikeTests.cs` and `Constants.cs`. See deviation 4 |
| `ProphetsWay.Example/` | Git submodule tracking the standalone Example repo; never edit it here |

The submodule pointer is at **`61d9e7d`**, verified by reading
`.git/modules/ProphetsWay.Example/HEAD` on 2026-08-20. **It is not the `3.1.0` tag** — `git submodule
status` describes it as `3.1.0-1-g61d9e7d`, i.e. one commit past that tag: the merge of
`ProphetsWay.Example` PR #21 (`3.1.1-eftool-findings`) on 2026-08-18. That commit opened the **3.1.1**
line — `ProphetsWay.Example/app-variables.yml` reads `3` / `1` / `1` and `CHANGELOG.md`'s top heading
is "v3.1.1 — not yet released" — and it is what brought in `TestDataAccessFactory.Use`, the seam this
repository's harness depends on. **Earlier text here saying `d845863` — "the 3.1.0 tree" — is
superseded and must not be restated**; so is any description of the pointer as sitting on a tagged
release. Corroborated by the checked-out tree (`IExampleDataAccess` aggregates `IDepartmentDao` and
`ICompanyResourceDao`; `TestDataAccessFactory` declares `public static void Use(Func<IExampleDataAccess>)`).

The solution also includes four projects from the submodule: DataAccess, DataAccess.NoDB, Tests, and
the Database project — which is **SDK-style `Microsoft.Build.Sql/2.2.0`**, re-verified 2026-08-20 at the
current pointer, not the legacy SSDT format this file previously named. The pipeline builds
`**/*.csproj`, not the solution;
with `HasSqlProj` commented out in `app-variables.yml`, it does not build the `.sqlproj`.
`LocalTestsOnly: 'yes'` skips every test in CI. All three pipeline facts re-verified against
`app-variables.yml`, `local-pipeline.yml`, and `prophets-pipelines/steps/restore-build-test.yml`.
The 2026-08-16 build also measured two things the pipeline never exercises: the SDK-style `.sqlproj`
**builds under the .NET CLI**, producing `ProphetsWay.Example.Database.dacpac`, and
`ProphetsWay.Example.Tests` compiled on **both** legs — which is what the 164 tests / 328 executions
figure rests on.

**Upstream observation, not an EFTools deviation.** 4 of that build's 7 warnings — two per leg — are
`xUnit1013` on the submodule's `ProphetsWay.Example.Tests/DepartmentDaoTests.cs`: `public void
EditEveryFieldAfterTheCall` (line 294) and `public void AssertEveryStampIsUtc` (line 1001), both
helpers rather than tests. They belong to `ProphetsWay.Example` and a `Test Designer` fixes them
**there**, never from this side. This **verifies a previously-unverifiable claim**:
`ProphetsWay.Example/docs/repo-profile.md` asserted "two `xUnit1013` warnings" and an analyst
annotated the count as build-derived and unchecked. It is exactly two, and both are now identified.

### Key Types

| Type | Kind | Role |
| --- | --- | --- |
| `BaseEFContext` | public abstract class | EF6/EF Core `DbContext` construction; Core string construction hardcodes SQL Server |
| `BaseEFDataAccess<TContext>` | public abstract class | Creates the context, forwards DAL transaction operations, and **implements the 3.x disposal contract as of 2026-08-16** — idempotent, non-throwing, rolls back an open transaction, disposes the context it created. `ObjectDisposedException` guarding on the transaction members is **not** yet in place. **One type parameter, not two** — earlier text here and in `README.md` says `BaseEFDataAccess<TContextType,TIdType>`, which no longer exists; verified by opening `BaseEFDataAccess.cs` |
| `ContextOwnership` | public enum | `Borrowed` / `Owned`, no default — the caller states who disposes the context. New since the published 2.2.0 line |
| `RootBaseDao<T,TIdType>` | public abstract bridge | CRUD, all/count/paged reads, datasets, and local transaction helpers |
| `RootBaseSoftDao<T,TIdType>` | public abstract bridge | Timestamped soft delete and filtering over the root DAO |
| `BaseNonIdDao<T>` / `BaseSoftNonIdDao<T>` | public abstract classes | Keyless/composite-key extension points |
| `Guid.*`, `Int.*`, `Long.*` DAO bases | 18 public abstract classes | Current key-specific CRUD, get-all, paged, and soft-delete consumer API |

**The library still carries the complete 2.2.x class shape.** None of the twelve-class redesign
specified in [docs/api-contract.md](docs/api-contract.md) has landed in `ProphetsWay.EFTools/` — the 18
key-specific closures, both `Root*` bridges, both internal `Root*Dao` engines and both keyless bases are
all present and unchanged, verified 2026-08-20 by listing the directory and grepping every type
declaration. `ContextOwnership` is the only structural addition. Do not read the api-contract document
as a description of the tree.

### Known Deviations

| # | Deviation | Severity / notes |
| --- | --- | --- |
| 1 | ~~Package and EF example reference `ProphetsWay.BaseDataAccess` 2.5.0~~ **CLOSED 2026-08-16** | **The reference is now 3.1.0 in both `ProphetsWay.EFTools.csproj` and `ProphetsWay.Example.DataAccess.EF.csproj`** — verified by opening both. The row is kept rather than deleted because the *published* 2.2.0 package still carries 2.5.0, so a consumer reading nuget.org sees the old contract until 3.0.0 ships. Do not re-report the tree as being on 2.5.0. |
| 2 | ~~Library TFMs are `net461;net471;net48;net80;net90`~~ **CLOSED 2026-08-16** | **All three projects now read `net10.0` alone** — the library, `ProphetsWay.EFTools.Tests` and `ProphetsWay.Example.DataAccess.EF` — verified by opening each `.csproj`. That is the approved target state under **D7**, a ratified exception to the house standard, **not drift**: see the section above and D7 in [docs/purpose-and-scope.md](docs/purpose-and-scope.md#owner-decisions--2026-08-15). Do not "fix" it toward `netstandard2.0;net10.0`, and do not re-report the old `net4*`/`net80`/`net90` lists as current. |
| 3 | Runtime package forces SQL Server and InMemory providers | **High, breaking to fix.** Provider packages and `UseSqlServer` live in the published library. |
| 4 | ~~**There is no test suite in this repository at all**~~ **SUPERSEDED 2026-08-18 — there is one, and it runs. The gap is now narrower and different** | **Medium.** The old text is wrong and must not be restated. **What is actually there, verified 2026-08-20 by opening all 19 files in `ProphetsWay.EFTools.Tests/`:** `TestSeam.cs`, an `internal static` class whose `[ModuleInitializer]` calls `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess)` and so points the **whole upstream `ProphetsWay.Example` suite** at the EF DAL for the assembly's entire run; **13 adapters**, each literally `public class EFXxxTests : XxxTests { }` with no test logic, which is all xUnit needs to discover the upstream classes here; `TestSeamTests.cs` (5 `[Fact]`) and `AdapterCoverageTests.cs` (2 `[Fact]`), both `[Trait("Guard","Seam")]`, guarding respectively that the seam is still wired to a *relational* EF layer and that no upstream test class has silently gone unadapted; `AlternateKeyGuardSpikeTests.cs`; and `Constants.cs`, now a plain SQL Server connection string with `TrustServerCertificate=True` and **no `#if` branches**. **Discoverable cases: 151** — 144 from the 13 adapted upstream classes (129 `[Fact]` plus 6 `[Theory]` yielding 15 cases; the 20 `ConventionShowcase` facts are correctly *not* adapted) plus those 7 guards — **and 165 counting the spike's 14.** **Last measured result: 2026-08-18, `dotnet test` = 151 tests, 123 passed, 28 failed** ([FR 13](docs/feature-requests.md)); the `.trx` files in `TestResults/` capture earlier laps the same day (147/53/94, then 151/57/94). **No build or test has been run since**, and `AlternateKeyGuardSpikeTests.cs` has been committed in the interim — treat 123/28 as the last known state, not as current. **What it still does not cover:** the ~28 red are dominated by `CompanyResourceDao`, which **does not exist** — there is no `CompanyResourceDao.cs`, no `DbSet<CompanyResource>` and no `ToTable` for it, and the three `ICompanyResourceDao` forwarders throw `NotWrittenYet`. Every test here is written against `IExampleDataAccess`, so **nothing tests this library's own public surface directly** — the 18 key-specific bases are reached only through the five EF DAOs that derive from them, and `DepartmentDao` derives from **no** EFTools base at all ([FR 13](docs/feature-requests.md)), so the soft-delete bases are exercised by nothing. The suite also requires a **local SQL Server** carrying `ProphetsWay.Example`, runs on that provider only despite the `Microsoft.EntityFrameworkCore.Sqlite` reference in the test `.csproj`, and is skipped wholesale in CI by `LocalTestsOnly: 'yes'`. Provider portability and the redesign in `docs/api-contract.md` remain entirely uncovered. |
| 5 | Package homepage/tags and source/symbol/reproducibility settings are empty or missing | **Medium.** Verified field by field in `ProphetsWay.EFTools.csproj`. Present with values: `PackageId`, `Description`, `Authors`, `Company`, `Product`, `RepositoryUrl`, `RepositoryType` (`GitHub`, where the convention is `git`), `PackageIcon`, `PackageReadmeFile`, `PackageLicenseExpression`, `PackageRequireLicenseAcceptance`, and the `ItemGroup` packing README, CHANGELOG and `profile.png`. Empty self-closing stubs: `PackageProjectUrl`, `PackageTags`, `PackageReleaseNotes`, `Copyright`, `NeutralLanguage` (plus the pipeline-owned `Version`/`AssemblyVersion`/`FileVersion`/`InformationalVersion`, correctly empty). Absent entirely: SourceLink, `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat`, `ContinuousIntegrationBuild`. |
| 6 | ~~`ProphetsWay.Example.DataAccess.EF` references unused FluentAssertions 8.2.0~~ **CLOSED 2026-08-16** | **The reference has been removed** — verified by opening `ProphetsWay.Example.DataAccess.EF.csproj`, whose only `PackageReference` entries are now the two EF Core packages and `ProphetsWay.BaseDataAccess`. The paid-commercial-licence exposure is closed. Kept as a row so the reason it mattered survives: 8.x requires a paid licence and house convention names Shouldly. |
| 7 | ~~`.gitmodules` contains an incomplete `[submodule "Submod"]` block~~ **Severity raised, then closing — 2026-08-16** | **Medium, not cosmetic. This is one of the few entries here backed by a real build rather than reasoning.** `dotnet build ProphetsWay.EFTools.sln -c Debug`, run by the owner on **2026-08-16** (SDK 10.0.400), emitted `Microsoft.Build.Tasks.Git.targets(25,5): warning : The path of submodule 'Submod' is missing or invalid: ''. The source code won't be available via Source Link.` **three times** — once each for `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests`. **The measured cost is that it disables Source Link on a published package**: a consumer of `ProphetsWay.EFTools` cannot step into its source. The earlier **Low / cosmetic** reading was reasoned, not measured, and is superseded. **The block is absent from `.gitmodules` as read on 2026-08-16** — `Modernizer` owns that file and was removing it as this was written — so treat this as a deviation closing with its cost on record. **The warning is not claimed to be gone: no build has been run since.** **Removal stops the warning; it does not give the library Source Link.** Deviation 5 stands unchanged — SourceLink is not referenced at all, and `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat` and `ContinuousIntegrationBuild` are all absent from `ProphetsWay.EFTools.csproj`, re-verified by opening it. Read the two rows together or you will conclude Source Link works. |
| 8 | ~~**The repository is mid-flight on the 3.x redesign** — three independent breaks~~ **CLOSED 2026-08-16 by a verified-green build** | **All three enumerated breaks are closed, and this is measured rather than reasoned.** `dotnet build ProphetsWay.EFTools.sln -c Debug`, run by the owner on **2026-08-16**, **succeeded** — SDK 10.0.400, 7 warnings, 0 errors, every project compiling: `ProphetsWay.EFTools`, `ProphetsWay.Example.DataAccess.EF` and `ProphetsWay.EFTools.Tests` on `net10.0`; the submodule's `DataAccess` and `DataAccess.NoDB` on `netstandard2.0` and `net10.0`; `ProphetsWay.Example.Tests` on **both** `net48` and `net10.0`; and `ProphetsWay.Example.Database` producing a `.dacpac` **under the .NET CLI**. The row is kept, not deleted, so the record of what broke survives: (a) ~~`ProphetsWay.EFTools.Tests` targeted `net472;net48;net80;net90` against a `ProphetsWay.Example.Tests` retargeted to `net48;net10.0`~~ — closed by the `net10.0` retarget; (b) ~~the same project's six adapters overrode a member that no longer exists upstream~~ — closed by deleting them; (c) ~~`ExampleDataAccess` did not satisfy the 3.1.0 `IExampleDataAccess`~~ — closed by `Implementer` adding the `IDepartmentDao` and `ICompanyResourceDao` member groups as deliberately-throwing "not written yet" stubs, with `Dispose` inherited from `BaseEFDataAccess`. **Compiling is not conforming** — those stubs throw, `ExampleContext` maps neither new entity, and the verification gap deviation 4 describes is untouched by this. The remaining 3.x work in [FR 1](docs/feature-requests.md) — provider neutrality, the DAO-family collapse, the dead `#if NET4*` blocks — is unaffected. |
| 9 | **`AlternateKeyGuardSpikeTests.cs` is committed and carries no trait, so it is outside every gate** | **Low, factual.** Committed in `2691526` (2026-08-19, "wrapping for the evening") — it is in version control, not untracked as the 2026-08-19 handoff recorded. It declares **7 `[Theory]` methods, each with 2 `[InlineData]` (`InMemory`, `Sqlite`) = 14 cases, and zero `[Trait]` attributes** — verified by opening the file and grepping every attribute in it. xUnit trait filters are allowlists, so the stated gate `--filter "Scope=Contract\|Guard=Seam"` selects **none** of the 14: the file runs on a bare `dotnet test` and on nothing else. Its own `<remarks>` call it "an empirical spike, not a specification" — it declares its own `SpikeUser` / `SpikeUniqueUser` / `SpikeContext` and asserts about EF Core, not about this library. **Recorded here as an observation only.** Which trait it should carry, and whether it stays at all, is an owner decision already queued in the 2026-08-19 handoff; no agent should assign one unasked. |

`docs/architecture.md`, per-project `docs/requirements.md`, and
`docs/nuget-extraction-proposal.md` are **n/a by owner decision**, not missing documentation.
