# Security Review - Frozen EFTools Test Helper

_Reviewed 2026-09-08 against general security practice and the current D033/D035 execution contract. No threat-model.md or data-classification.md was present in docs/security._

## Verdict

**No blocking issues found in the inspected scope.** No confirmed Critical, High, Medium, Low or Informational security finding was identified. The explicitly no-restore, public-source dependency vulnerability query completed successfully with no reported vulnerabilities. This is a focused static review of the synthetic-data test harness, not a statement that the repository or deployed fixture is secure. It grants no permission to checkpoint, push, publish, merge, deploy, connect or certify; other required gates and owner decisions remain independent.

## Scope And Evidence

The acceptance target is the night-wrapup revision 1 security landing gate. Its parent records baseline HEAD `4e6008c9f85feb83a8706956481f6a1273ab21f0`. Reviewed the complete current [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L1), Constants control flow with string literals withheld, relevant fixture callers, the Example DAL options constructor, and the test dependency graph. No Git operation or independent HEAD-diff reconstruction was performed. Reviewing the complete current helper covers its present security behavior; the parent still owns exact final-diff verification and attribution to the frozen change.

Opened [D033](../decision-log.md#L791), [D035](../decision-log.md#L863), and the complete [execution contract](../azure-sql-test-execution.md#L1). D033 supersedes the former operational route; this review does not resurrect its stopped approval machinery. D035 expressly distinguishes engine capability from endpoint ownership and operation authority.

Consumed retained report 02 and its generated after-comparison: both measured comparisons contain 54 expected/actual specification-input paths with zero additions, removals or differences. Report 02 separately records the unchanged helper fingerprint. Prior local report 15 and focused Test Auditor rereview 12 were opened for provenance, not treated as security clearance. The owner's 369 passing results establish neither endpoint/authentication facts nor security. These are inherited preservation measurements, not fresh hashes calculated by this reviewer.

There was no existing security-review.md at entry. The existing azure-sql-certification.md was left untouched; its historical coverage and conclusions are not reissued here.

## Findings

None confirmed within the stated input trust model. Environment configuration and test-assembly callers are operator-controlled; there is no reviewed web/API entry point or real-user-data workload. Missing application authentication, tenant authorization or web middleware is not a defect in this test-helper slice.

## Security Evidence

| Concern | Location and evidence | Conclusion within scope |
| --- | --- | --- |
| Scratch-name admission | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L103): trims both inputs, bounds length to 128, uses anchored ASCII-only `EFToolsScratch_` grammar and rejects case-insensitive duplicates. | System, fixed Example, disposable and arbitrary SQL names cannot enter the real reusable pool through configuration. Validation precedes connection creation. |
| Caller identity admission | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L227): resolves active handles, rejects retired handles, and permits only exact fixed roles otherwise; `master` is refused in Reusable mode. | A physical scratch name or arbitrary caller identity cannot be substituted for an active lease. |
| Diagnostic labels | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L314): bounded ASCII label plus generated GUID; reusable labels produce lease identities, not reset destinations. | Caller labels do not become SQL commands or arbitrary database names. |
| Endpoint/catalog construction | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L333), [catalog replacement](../../ProphetsWay.EFTools.Tests/TestStore.cs#L423): typed SqlConnectionStringBuilder replaces only InitialCatalog. | An override's original catalog cannot widen the role allowlist. The endpoint and other connection settings remain trusted operator inputs, not independently validated ownership. |
| Lifecycle/configuration refusal | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L83), [configuration read](../../ProphetsWay.EFTools.Tests/TestStore.cs#L369): rejects unknown lifecycle, missing reusable names, scratch inputs in Disposable mode and changes to an established real reusable configuration. | Invalid configuration fails before SQL execution. SQLite does not consume the SQL lifecycle configuration. |
| Exclusive reset selection | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L431), [acquisition](../../ProphetsWay.EFTools.Tests/TestStore.cs#L674), [release](../../ProphetsWay.EFTools.Tests/TestStore.cs#L749): reset requires a configured reusable name; the slot remains reserved across reset. | Reset is confined to one of two selected databases and serialized within this process. Dedicated ownership outside this process is an execution precondition, not proved by the lock. |
| Reset SQL injection | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L451), [quoting](../../ProphetsWay.EFTools.Tests/TestStore.cs#L594): schema/table/constraint names come from catalog metadata; every dynamic identifier is bracket-quoted with closing brackets doubled. | No unquoted identifier or caller-supplied SQL body reaches the reset executor. Each component of a qualified name is quoted separately. |
| Reset scope and failure | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L443), [quarantine](../../ProphetsWay.EFTools.Tests/TestStore.cs#L782): transaction owns the reset; residual user tables cause failure; failures quarantine the pool. | Failure cannot advertise a clean reusable slot. Reset removes all user tables/foreign keys, deliberately not just test-model objects; foreign data is prohibited by the execution contract. No reset touches the fixed Example role. |
| Physical database SQL | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L517), [drop](../../ProphetsWay.EFTools.Tests/TestStore.cs#L537), [builders](../../ProphetsWay.EFTools.Tests/TestStore.cs#L580): lifecycle and checked edition gate database DDL; DB_ID uses a typed parameter and database identifiers are quoted. | Reusable has no reachable physical database create/drop path. The closed edition set refuses Azure SQL Database/unknown results; it does not prove localhost or authority. |
| Fixed-store and DAO route | [Constants.cs](../../ProphetsWay.EFTools.Tests/Constants.cs#L23), [SQL branch](../../ProphetsWay.EFTools.Tests/Constants.cs#L39), [TestSeam.cs](../../ProphetsWay.EFTools.Tests/TestSeam.cs#L53), [ExampleDataAccess.cs](../../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs#L40). | The adapted suite uses helper-configured options. The SQL fixed-store branch does not call EnsureCreated or reset; test DAO writes remain authorized synthetic fixture activity. |
| Direct fixture route | [KeylessDaoTests.cs](../../ProphetsWay.EFTools.Tests/KeylessDaoTests.cs#L287); targeted call-site search also located the other five direct fixture classes. | The inspected representative caller acquires a helper lease, configures its contexts, and then creates its model. The six classes pass diagnostic labels to OpenStore rather than supplying a reset destination. Lifecycle correctness remains the parallel Code Reviewer scope. |
| Credentials and output | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L343), [reset failure](../../ProphetsWay.EFTools.Tests/TestStore.cs#L496), [engine refusal](../../ProphetsWay.EFTools.Tests/TestStore.cs#L573). | Invalid connection parsing, real reset failures and engine refusals omit raw connection/SqlException messages and inner exceptions. No password/token literal, sensitive-data logging switch or logging sink was found in TestStore. Actual environment values, raw TRX and protected configuration were not read. |
| TLS/authentication defaults | [TestStore.cs](../../ProphetsWay.EFTools.Tests/TestStore.cs#L349): localhost fallback uses Integrated Security and TrustServerCertificate=true; explicit override is preserved. | Local certificate trust is not remote TLS validation. No Azure authentication mode, encryption setting or certificate validity is inferred from the code or passing tests. Remote TLS/authentication requires separately authorized protected verification. |

## Dependency Vulnerabilities

The successful command was run against the existing restored test graph, without restore, build, installation, update, private-source access or an authentication prompt:

```powershell
dotnet list 'c:\Projects\ProphetManX\ProphetsWay.EFTools\ProphetsWay.EFTools.Tests\ProphetsWay.EFTools.Tests.csproj' package --vulnerable --include-transitive --no-restore --source https://api.nuget.org/v3/index.json --format json
```

Final measured invocation: **exit 0, 1.57 seconds**, JSON output version 1, both vulnerability/transitive flags confirmed, exactly one source matching the public NuGet URL, one project, zero returned vulnerable-package entries, zero vulnerability records and zero non-null diagnostics. A vulnerability-filtered result omits packages without reported advisories; zero returned entries does not mean the project has zero dependencies.

| Package | Version | Advisory | Severity | Direct/Transitive | Fixed in |
| --- | --- | --- | --- | --- | --- |
| No known vulnerabilities reported by this query | n/a | None returned | n/a | Both queried | n/a |

Current files, rather than old repository notes, establish net10.0 for the EFTools test project, EF Core Sqlite/InMemory 10.0.11, and project references to the EF proving ground and inherited Example tests. The proving ground supplies SQL Server 10.0.11. The published library references EF Core 10.0.11 and BaseDataAccess 3.2.0, not test providers. Existing assets resolve SqlClient 6.1.6, Azure.Identity 1.17.1 and SQLitePCLRaw.lib.e_sqlite3 2.1.12. These identities were read without printing assets' source/configuration sections.

Sources opened: [test project](../../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj#L1), [proving-ground project](../../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj#L1), [library project](../../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj#L58), [inherited test project](../../ProphetsWay.Example/ProphetsWay.Example.Tests/ProphetsWay.Example.Tests.csproj#L1), and the relevant library identities in the existing test assets file.

The initial scan attempt rejected an extra `false` argument before scanning; removing that unsupported argument was a CLI syntax correction, not bypassing an access refusal. One successful query's result was local to its command scope, so the same permitted read-only query was repeated to capture the complete structured summary above. No refusal or package repair occurred. Advisory results depend on the existing assets and NuGet's available metadata/cache; they are not package-content auditing or a guarantee against unknown vulnerabilities. No deprecation/outdated scan was needed.

## Coverage

| Area | Reviewed | Notes |
| --- | --- | --- |
| Helper input and SQL sinks | Yes | Complete current TestStore; identifiers, role admission, reset ownership/quoting and configuration refusal traced above. |
| Constants and relevant callers | Yes, focused | Constants control flow, suite wiring, DAL options constructor, six fixture call sites and representative fixture body. Not a full production DAO audit. |
| Credentials, output, TLS/auth defaults | Yes, static | Code only; protected values and effective remote configuration excluded. |
| Direct/transitive dependencies | Yes | Successful no-restore public advisory query of the existing EFTools test graph; not independent scans of unrelated projects/TFMs. |
| Frozen-input provenance | Consumed | Generated 54-file comparison plus helper provenance and prior reports; no new hash baseline. |
| Exact HEAD/final diff and Git history | No | Git operations forbidden. Full current-helper review does not replace the parent's exact final-diff and publication checks. |
| Threat-model/classification obligations | No artifacts present | General-practice basis; no named threat-model obligations can be asserted discharged. Threat Modeler v2 owns any later model. |
| Live DB/cloud, effective identity, endpoint and TLS | No | Explicitly forbidden; synthetic-data-only execution context supplied by owner. |
| Bicep, pipeline, private-value publication policy | No | Separate reviewer/owner boundaries. |
| Lifecycle correctness and specification quality | No renewed audit | Code Reviewer v2 and prior Test Auditor v2 respectively. |
| Web/API security checklist | Not applicable | No endpoint/application change in this scoped test harness. |

This review is not exhaustive. No working exploit, source/configuration fix, runtime test, build, database operation or Azure operation was produced or executed.

## Worth Checking

- Before any separately authorized remote run, verify effective encryption, certificate validation and authentication privately. The helper preserves the override rather than enforcing these values; no insecure remote setting was observed because those inputs were excluded.
- Dedicated scratch ownership and absence of other users/processes remain external execution prerequisites. The two-slot reservation is process-local, and reset intentionally drops every user table. Do not interpret the namespace grammar, engine edition or a passing TRX as physical ownership evidence.
- Local Disposable admission is a prefix predicate, and its CREATE batch can find an already-existing name. Current OpenStore generates identities; do not infer authorization to adopt an arbitrary pre-existing prefixed database through the internal constructor. The parallel Code Reviewer v2 owns lifecycle/adoption correctness; this review establishes neither an externally reachable attack nor resource-ownership evidence.

## Handoff

Parent: retain this scoped result, verify the exact final diff against the frozen inputs, and combine the separately required reviews. No source repair is requested and no new approval is inferred. No secret value was reported; no literal credential was identified in the inspected helper. Effective credentials were deliberately uninspected.

Azure Deployment Reviewer v2: assess the already-known Bicep identity/firewall and protected-default publication boundary; those inputs were not inspected here and their policy remains the owner's decision.

Code Reviewer v2: retain ownership of lifecycle/adoption correctness and concurrency; Test Auditor v2's focused clearance remains separate and is not reissued by this review.
