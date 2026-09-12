# Azure SQL Test Execution Contract

**Authority:** [D-035](decision-log.md#d-035-split-azure-certification-from-physical-database-lifecycle-checks),
the revised [Gate 2 scope](purpose-and-scope.md#gate-2-scope-revision--2026-09-07), and
[FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql).

**Status:** Reviewed `Ready`, focused cycle 2; `RR-D035-001` cleared. This is a reviewed execution
contract, not Azure certification. It defines test execution only and authorizes no Azure, SQL, Git,
pipeline, release, production, project, or package operation.

## Filtered Trial Result 2026-09-08

The owner completed the `Execution!=LocalPhysicalLifecycle` trial. The parent's structured reconciliation
of the supplied TRX found **369 included executions, all passing, zero failed and zero skipped**, with
369 distinct execution identities and linked test definitions. All **14 provider-selection-exempt
alternate-key comparison cases** passed and are included in that count. The **six local physical-lifecycle
cases were excluded**: none of their named results is present, and the exact-six classification guard
passed once. Prior local physical **6/6** evidence is separate and is not added to this trial's count.
No new whole-assembly discovery was performed.

The owner reported **99.4 seconds** for the trial and a successful build in **103.5 seconds**. The owner
also reported the Example DACPAC deployed with synthetic seed data and selected the route using two empty
scratch databases. Azure targeting is owner execution context, not independent live inspection: the TRX
establishes test outcomes and identities, not endpoint, authentication, deployed state, or physical DDL.
No automatic DACPAC republication is pending. The raw TRX source location remains in the supplied result
record; durable retention is separate evidence work, and no verified copy receipt is claimed here.

**Gate 2, FR 19, and release remain pending the owner's final review.** This dated
observation changes no D-035 rule, acceptance criterion, test/count requirement, or historical decision,
and grants no operation authority.

## Execution Axes

Provider, connection, and physical lifecycle are independent axes.

| Axis | Contract |
| --- | --- |
| Provider | `TestStoreProvider` remains exactly `SqlServer` and `Sqlite`. `EFTOOLS_PROVIDER` keeps its current values, trimming/case behavior, `SqlServer` default, and fatal unknown-value behavior. Azure SQL is not a provider. |
| Connection | `EFTOOLS_SQLSERVER_CONNECTION_STRING` remains the optional SQL Server base-connection override. Presence, endpoint, or initial catalog never selects lifecycle. The helper continues to replace only the catalog after validating the requested database role. |
| SQL Server lifecycle | New `EFTOOLS_SQLSERVER_STORE_LIFECYCLE`: trimmed, case-insensitive `Disposable` or `Reusable`. Null, empty, or whitespace means `Disposable`. Any other value fails before a connection, lease, or database identity is created and names the variable and accepted values without echoing connection material. |
| Fixed store | Exact `ProphetsWay.Example` remains a configured-existing store for `Constants`. `TestStore` never creates, drops, or scratch-resets it. The inherited classes and `ProviderSelectionTests` remain serialized by `TestCollections.SharedStore`. |
| Scratch stores | `Disposable` keeps local per-call database creation/deletion. `Reusable` leases exactly two pre-existing scratch databases and resets their user schema/data without creating or deleting either database. |

The lifecycle variable is read only for `SqlServer`; SQLite keeps its existing per-store shared-memory
lifetime and does not read SQL Server lifecycle or scratch-name inputs. This keeps the reusable path
exercisable against localhost by changing lifecycle and scratch names without adding a provider or changing
the connection override.

## Reusable Configuration

`Reusable` requires both variables below; neither has a default:

- `EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1`
- `EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2`

Each value is trimmed, must match `^EFToolsScratch_[A-Za-z0-9_]{1,113}$`, and must be at most 128
characters. The two names must differ under ordinal-ignore-case comparison. Empty values, duplicates,
`EFToolsTest_` names, exact or case variants of `ProphetsWay.Example`, and the system names `master`,
`model`, `msdb`, `tempdb`, and `resource` are refused before network access. No arbitrary caller-supplied
database name may enter the reusable pool.

Missing reusable inputs produce one deterministic error naming every missing variable. A non-empty scratch
variable with `Disposable` is a configuration error; it does not select `Reusable`. SQL Server-specific
inputs are inert when SQLite is selected. No refusal includes a connection string or credential value.

The two configured databases must already exist, be dedicated to this test harness, and contain no foreign
data or user objects. The executing principal must already own enough database-level schema authority in
each scratch database to remove all user foreign keys and tables and to let EF create the current test model.
This contract selects no live role or grant and requires no server-level create/drop authority.

### Current-Route Configuration

For a separately authorized Azure run, set `EFTOOLS_PROVIDER=SqlServer` and
`EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable` in the test process. The owner supplies both
`EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1` and `EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2`, plus an explicit
`EFTOOLS_SQLSERVER_CONNECTION_STRING`, privately and process-locally; do not commit or log their values.
Both validated scratch names must identify already-existing, dedicated test databases. The connection
override changes only the catalog for the validated role, preserving the other owner-supplied settings.

Each scratch reset removes all user foreign keys and tables in the leased database before use and on
release, not just the current model's objects. Fixed `ProphetsWay.Example` is never scratch-reset, but the
adapted tests may write synthetic rows there. Validate its existing schema and access under separate
operation authority; do not automatically republish the DACPAC. Azure scratch provisioning, protected
configuration, and live execution require separate authority, not authority granted by these settings.

## Why Capacity Is Two

The six direct fixture classes can each hold one `OpenStore` lease while running in separate xUnit
collections. `ProviderSelectionTests`, serialized with the inherited fixed-store classes, is the only caller
that opens two stores simultaneously. One slot therefore deadlocks that isolation case; two permit it.
Additional one-slot callers may wait, so preserving current class parallelism does not require six or eight
physical databases.

Acquisition must not hold the pool lock while waiting. A one-slot caller never waits for another slot while
holding one; only the known two-store guard does so, and the second slot exists. Disposing either slot wakes
a waiter. A deterministic concurrency specification must hold both slots, observe a third acquisition
waiting, release one, and observe exactly one waiter acquire it; it must also complete the two-store isolation
case while the six one-store caller shapes contend. These checks disconfirm pool deadlock without making a
wall-clock performance promise.

## Lease Contract

In `Reusable`, `OpenStore(label)` chooses a free configured slot; the label is diagnostic only. Each
acquisition receives a never-reused lease identity and exposes its assigned database name separately for
test diagnostics. A database slot has at most one active lease, and two simultaneous leases have different
database names.

`LiveIdentities` means active lease identities, not physically existing databases. Physical existence is a
separate local-only probe. `ConfigureExisting` may resolve exact `ProphetsWay.Example`, an active lease
identity, and, only in local `Disposable` physical-lifecycle checks, `master`. It refuses a retired lease with
`ObjectDisposedException` and a never-issued, system, scratch-name, or foreign identity with
`ArgumentException`, before opening a connection.

`Store.Configure` succeeds only while that exact lease owns its slot. After first disposal it always throws
`ObjectDisposedException`; reacquiring the same physical slot never revives an old handle or lease identity.
Disposal is idempotent. Its first call resets the slot, retires the lease, and returns the slot to the pool.

An acquisition or release reset failure quarantines that slot, fails the current operation, wakes and fails
pool waiters, and prevents later acquisition in that process. The slot is never returned as clean.
`DisposeReportingFailure` reports that failure; ordinary `Dispose` must not swallow it. Existing fixture
cleanup may preserve an already-failing body as primary, but a reset failure can never coexist with a green
test run.

## Reset Contract

Reset runs under the slot's exclusive lease both before `OpenStore` returns and during first disposal. It
uses a short reset-owned transaction that commits or rolls back before test code receives the store. It does
not wrap a test, a DAO call, or a transaction-contract case in a global rollback transaction.

For the actual `EnsureCreated` callers, reset must drop every user foreign-key constraint and then every user
table across non-system schemas, using catalog metadata plus quoted identifiers, and verify that no user
table remains. Dropping tables removes their keys and indexes. The next caller's existing `EnsureCreated`
then creates its own model, whether the prior lease held the probe, keyless, soft-keyless, open-key,
identifier, timestamp, or failed-insert model. A partial reset is failure, never a clean slot.

The SQL Server connection pool for the assigned database may be cleared before reset, but the database may
not be forced single-user in `Reusable`. Every context must be disposed before its store. The fixed Example
database is outside this reset method and its DACPAC/baseline preparation remains a separate Gate 2 concern.

## Azure Database-DDL Refusal

`Reusable` contains no `CREATE DATABASE`, `DROP DATABASE`, or `ALTER DATABASE` path. Before `Disposable`
can issue database DDL, it must query `SERVERPROPERTY('EngineEdition')` read-only. Azure SQL Database
(`5`) is not the sole refusal value. The only engine-edition results that pass the capability classifier
are the closed set `{ 2, 3, 4 }`: `2` is the Standard engine family, `3` is the Enterprise engine family
(including Developer), and `4` is the Express engine family. This mapping follows Microsoft's
[`SERVERPROPERTY` `EngineEdition` reference](https://learn.microsoft.com/sql/t-sql/functions/serverproperty-transact-sql#engineedition).

| Classification result | Required outcome before database DDL |
| --- | --- |
| Successfully converted integer `2`, `3`, or `4` | Engine capability check passes; operation authority must still pass separately. |
| Any other numeric value, including any future value | Refuse. |
| Null, `DBNull`, absent scalar, fractional value, unconvertible value, or integer overflow | Refuse. |
| Connection, command, query, or conversion failure | Refuse. |

Every refusal occurs before `CREATE DATABASE` or `DROP DATABASE` and names
`EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable`. The checked result is bound to the store so disposal cannot
take an unclassified drop path. Passing `{ 2, 3, 4 }` establishes engine capability only: it neither proves
the endpoint is localhost or test-owned nor grants permission to mutate it. Database DDL additionally
requires the `Disposable` lifecycle resolved under Execution Axes and separate operation authority for the
approved focused localhost run against test-owned disposable resources. No live Azure or other remote SQL
operation is authorized by this classifier. Thus an Azure endpoint with lifecycle or scratch configuration
omitted may fail, but it cannot create or drop a database.

Every reset target must be selected from the validated two-name reusable allowlist and held by the current
lease. Exact Example, system, local-disposable, unconfigured, and inactive-slot names are never reset. The
connection override's initial catalog cannot widen that allowlist.

## Test Inventory And Classification

Only these existing specifications receive `[Trait("Execution", "LocalPhysicalLifecycle")]` and are
excluded from a reusable Azure run:

- `ShouldLeaveEntityFrameworkUnableToFindAStoreThatWasDisposed`
- `ShouldRemoveTheStoreItselfWhenItIsDisposed`
- `ShouldLeaveNoDisposableStoreBehindOnTheServer`
- `ShouldRegisterAStoreBeforeProvisioningItAndForgetItIfProvisioningFails`
- `ShouldDecideEveryDropWithTheOnePredicateThatGuardsIt`
- `ShouldNameEveryStoreInsideTheDroppableNamespace`

They remain discoverable and mandatory in the unfiltered local `Disposable` run. No whole class, all
`Guard=Seam` cases, cleanup cases generally, or failed assertion may be excluded. Azure selection uses an
explicit test filter, `Execution!=LocalPhysicalLifecycle`, never `Skip`, a conditional return, or a passing
no-op.

A reflection guard owns the exact method-name set above and fails if a listed method loses the trait, any
other method gains it, a method disappears, or a skipped test appears. The Azure evidence report derives
and records `discovered`, `included`, `excludedLocalPhysicalLifecycle`, `skipped`, and `failed` counts from
post-change discovery and lists exactly the six exclusions above. Discovered and included totals are run
observations, not fixed requirements. It must satisfy `included + excludedLocalPhysicalLifecycle = discovered`,
`excludedLocalPhysicalLifecycle = 6`, `skipped = 0`, and `failed = 0`.

The 14-case `AlternateKeyGuardSpikeTests` comparison remains executed and passing, reported separately as
provider-selection-exempt, and is included rather than physically excluded.

Environment-independent guards remain in both lifecycle modes, with honest terminology:

- live/existing checks use active lease state; only the local-only pair asserts physical absence after disposal;
- unopened-context disposal asserts quiet lease retirement and slot reuse, not database absence;
- cleanup reporting, ordinary disposal, six-fixture routing, and live identity checks cover reset/release;
- droppable-identity and foreign-open guards become the reusable reset allowlist and fixed/system/foreign refusal;
- fresh-store emptiness proves reset, while two-store identity/isolation, foreign keys, provider routing,
  null builders, stale handles, and adapted-suite provider checks retain their behavior.

All application, DAO, constraint, isolation, disposal, and transaction assertions remain unchanged. The six
direct fixture callers and `Constants` continue through `TestStore`; no assertion is weakened to fit reuse.

## Ownership And Focused Verification

`Test Designer v2` owns specification and trait changes in `ProviderSelectionTests.cs`, including the
closed-set drift guard and discovery-derived reporting assertions. `Test Harness Engineer v2` owns
`TestStore.cs`. `Constants.cs`, the six direct fixture files, and inherited `BaseUnitTests.cs` are
compatibility surfaces and should remain unchanged unless a red specification proves a helper-only edit is
necessary; their application/DAO assertions are never in harness scope.

Focused verification is: offline configuration/refusal and inventory guards; then a localhost reusable-mode
integration using two test-owned disposable databases created and removed by the outer validation fixture.
That integration must exercise two simultaneous leases, a waiting third lease, slot reuse across at least
two different `EnsureCreated` models, reset-failure propagation, stale-handle refusal, and unchanged physical
database existence after reusable disposal. It must not connect to or mutate the fixed
`ProphetsWay.Example` database. A focused local `Disposable` run executes the exact physical-lifecycle
inventory. No new package is required beyond the current runtime, SqlClient, and EF Core.

## Design Choices

| Selected | Rejected | Reason |
| --- | --- | --- |
| Explicit lifecycle axis | Infer lifecycle from endpoint or scratch variables | The same reusable path must run on localhost, and omitted Azure configuration must never select database DDL. |
| Two queued scratch slots | One slot or one database per test | One cannot satisfy simultaneous isolation; per-test creation is the behavior D-035 removes from Azure. |
| Lease identity separate from database name | Treat physical existence as store lifetime | Reusable disposal ends ownership and resets state while the pre-existing database correctly remains. |
| Schema reset before/after lease | `EnsureDeleted`, database drop, or whole-test rollback | Differing `EnsureCreated` models need an empty schema; physical DDL is forbidden on Azure; rollback would invalidate transaction tests. |
| Closed method-level trait | Exclude all seam/cleanup tests | Most guards remain applicable to leases, resets, provider selection, and safety. |
