# EFTools Decision Log

This append-only log records owner decisions that govern the EFTools Azure SQL certification fixture.
Later decisions may supersede an entry by adding a new entry that cites it; existing entries are not
rewritten or reordered. Items under **Unresolved Boundaries** are explicitly not decisions.

The owner-authoritative source for entries D-001 through D-015 is the **Settled Owner Decisions**
section of the [2026-08-30 run record](../../.agent-runs/20260830-1228-azure-infra-authoring/run.md).
The prior [infrastructure advisory](../../.agent-runs/20260830-1205-azure-readonly-facts/04-azure-infrastructure-design-recovery.md),
[identity and retention threat analysis](../../.agent-runs/20260830-1205-azure-readonly-facts/03-threat-analysis-identity-retention.md),
and [certification requirements](azure-sql-certification-requirements.md) provide rationale and
dependency context, but do not override the current run's owner statements.

## D-001: Amend and independently review the requirements

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** A bounded amendment of the Azure SQL certification requirements and an independent
  re-review are authorized.
- **Rationale:** The prior advisory found that the documented resource-group-scoped entry point
  conflicted with the owner's selected subscription-scoped fixture boundary.
- **Implications:** The requirements must be reconciled before context-free infrastructure authoring
  proceeds, and the author cannot self-approve that reconciliation.
- **Unresolved boundary:** The review result is not predetermined. This decision authorizes neither a
  preview nor a deployment.
- **Authority:** Settled Owner Decisions item 1 in the 2026-08-30 run record.

## D-002: Use one subscription-scoped fixture entry point

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Root `infra/example.solution.bicep` is subscription-scoped, creates the dedicated
  `example-test-rg`, and invokes resource-group-scoped AVM modules from that same entry point. Root
  `infra/example.solution.bicepparam` is optional and exists only when useful.
- **Rationale:** One reviewed entry point keeps creation of the fixture boundary and its contained
  resources together while preserving each resource's proper deployment scope.
- **Implications:** The existing resource-group-scope requirement must be amended; no second root
  deployment entry point is implied.
- **Unresolved boundary:** Subscription context and live-operation approval remain outside this
  decision, and no parameter artifact may make the deployment actionable by itself.
- **Authority:** Settled Owner Decisions item 2 in the 2026-08-30 run record.

## D-003: Keep the fixture durable across independent waves

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Deploy the fixture once for multiple independent waves of test and other work. Remove
  it only through a separately approved final teardown; resource deployment is not repeated before
  every test run.
- **Rationale:** A durable fixture separates infrastructure lifetime from individual test-wave
  execution and avoids treating deployment as test setup.
- **Implications:** Later waves reuse the reviewed fixture only after their own state and execution
  gates pass.
- **Unresolved boundary:** Final expiry, monitoring ownership, teardown timing, and each live-operation
  approval remain unsettled.
- **Authority:** Settled Owner Decisions item 3 in the 2026-08-30 run record.

## D-004: Number top-level and nested deployment attempts together

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Top-level subscription deployments are named `example-test-deploy-<NNN>` and nested
  resource-group deployments are named `example-test-rg-deploy-<NNN>`. One attempt uses the same
  monotonically increasing three-digit number at both scopes, and a number is never reused, including
  after failure.
- **Rationale:** Unique paired names preserve deployment history and correlate the two scopes without
  overwriting evidence from an earlier attempt.
- **Implications:** Every retry or changed attempt allocates the next number; no timestamp, random
  value, or implicit default replaces it.
- **Unresolved boundary:** This naming rule does not allocate or approve an actual deployment attempt.
- **Authority:** Settled Owner Decisions item 4 in the 2026-08-30 run record.

## D-005: Fix the logical-server name and region

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The logical SQL server is exactly `example-test-sql`, and the region is exactly
  `eastus`.
- **Rationale:** The name is the shortest meaningful instance of the selected grammar, and `eastus`
  satisfies the owner's selected East-region placement without adding an unexplained token.
- **Implications:** Current name availability is evidence, not a reservation. Offer, quota, capacity,
  and name availability must be rechecked before preview, and no automatic fallback name is approved.
- **Unresolved boundary:** Pricing evidence, total cost, and lifecycle limits remain deployment gates.
- **Authority:** Settled Owner Decisions item 5 in the 2026-08-30 run record.

## D-006: Preserve the exact fixed database name as an exception

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The fixed Azure SQL database is exactly `ProphetsWay.Example`, matching the local SQL
  Server database. It is an explicit resource-specific exception to the lower-case hyphen naming
  grammar and must not be normalized.
- **Rationale:** Keeping the certification target's database identity aligned with the established
  local target removes an avoidable source of target mismatch.
- **Implications:** General resource-name normalization does not apply to this database token.
- **Unresolved boundary:** The exception settles no disposable-database name or readable fixture-value
  grammar.
- **Authority:** Settled Owner Decisions item 6 in the 2026-08-30 run record.

## D-007: Use Microsoft Entra only and reject UAMI for this fixture

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Authentication is Microsoft Entra-only, and the owner is the logical-server Entra
  administrator. A user-assigned managed identity (UAMI) is explicitly rejected for this fixture.
- **Rationale:** A UAMI is not a portable login for an unattached local workstation or a
  Microsoft-hosted agent, and a token-copy or secret workaround would defeat the selected identity
  boundary.
- **Implications:** The owner administrator is bootstrap authority, not an implicit runtime identity.
  No password, copied token, or stored secret may substitute for the eventual runtime flow.
- **Unresolved boundary:** The non-UAMI runtime principal, credential flow, and least-privilege SQL
  grants remain owner decisions.
- **Authority:** Settled Owner Decisions item 7 in the 2026-08-30 run record.

## D-008: Dedicate the resource group and server, and infrastructure-own the fixed database

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** `example-test-rg` and `example-test-sql` are dedicated solely to this certification
  fixture. The fixed database is infrastructure-owned, receives the DACPAC, persists across waves,
  and is never created, reset, or dropped by a test-store handle.
- **Rationale:** The dedicated boundary separates infrastructure ownership from test-store lifecycle
  authority and protects the fixed certification target from cleanup code.
- **Implications:** Fixed-database access and disposable-database lifecycle are distinct purposes with
  distinct authorization needs.
- **Unresolved boundary:** DACPAC authorization, runtime and lifecycle grants, and proof of a clean
  baseline remain gates before their respective operations.
- **Authority:** Settled Owner Decisions item 8 in the 2026-08-30 run record.

## D-009: Bound disposable-database creation and failed-state retention

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Direct-library tests may create ledger-bound, run-namespaced disposable databases on
  the dedicated server without redeploying infrastructure. Successful disposable databases are
  dropped immediately. At most one failed disposable database per run may be retained for at most
  24 hours, and only when manifest evidence is insufficient. Unresolved cleanup blocks new creates;
  expiry is not deletion authority.
- **Rationale:** This preserves the minimum exceptional failure evidence while bounding destructive
  authority and continuing cost.
- **Implications:** Creation must be registered before use, success has no retention window, and a
  failed cleanup stops further database multiplication.
- **Unresolved boundary:** Exact create ceilings, run lifetime, cost cap, pricing evidence, grants,
  and separately approved cleanup remain unsettled.
- **Authority:** Settled Owner Decisions item 9 in the 2026-08-30 run record.

## D-010: Quarantine failed fixed-database state between waves

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Failed-run state may remain in the fixed database for triage, but no later wave may
  run until an explicit baseline-reset or recovery operation restores and proves exact `Clean`
  content. After successful runs, table cleanup or baseline restoration may prepare the next wave
  without redeploying resources.
- **Rationale:** Preserving failed state supports diagnosis, while an exact clean proof prevents one
  wave's mutations from contaminating the next.
- **Implications:** Failure quarantines the fixed database; a new run identity or repeated DACPAC does
  not establish cleanliness.
- **Unresolved boundary:** The exact recovery operation, its approval, and fixture retention period
  remain outside this decision.
- **Authority:** Settled Owner Decisions item 10 in the 2026-08-30 run record.

## D-011: Default only stable reviewed values

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Stable fixed infrastructure values may default where reviewed. Identity, exact public
  client address, run and attempt identity, expiry, retention, lifecycle ceilings, and other
  approval-bound values remain explicit. The fixed authoring defaults are Basic, 5 DTUs, 2 GiB, and
  non-zone-redundant.
- **Rationale:** Stable non-secret values support repeatable authoring without silently selecting
  context, authority, privacy-sensitive, destructive, or financial inputs.
- **Implications:** A default is not deployment approval, and an optional parameter artifact may not
  fill approval-bound values.
- **Unresolved boundary:** Final expiry, short-term backup retention, database-create limits, run
  lifetime, cost cap, pricing evidence, and monitoring owner remain deployment gates.
- **Authority:** Settled Owner Decisions item 11 in the 2026-08-30 run record.

## D-012: Apply the approved fixture tags

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The approved fixed tags are `context=example`, `environment=test`, `managedBy=bicep`,
  and `purpose=eftools-azure-sql-certification`, together with the reviewed lifecycle and run tags.
- **Rationale:** The tags make fixture purpose, environment, management source, run binding, and
  lifecycle intent visible in resource evidence.
- **Implications:** Tags support ownership checks and review but never grant deletion authority or
  schedule teardown.
- **Unresolved boundary:** Approval-bound lifecycle and run tag values remain explicit inputs; the
  tags do not resolve expiry, retention, or cleanup authority.
- **Authority:** Settled Owner Decisions item 12 in the 2026-08-30 run record.

## D-013: Compose released AVM modules directly first

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Infrastructure is direct AVM-first, using explicit released versions and
  owner-confirmed compatible tags. A raw resource requires a documented, reviewed AVM capability-gap
  exception. Any future custom wrapper must compose pinned AVM modules.
- **Rationale:** Direct pinned modules keep the first fixture reproducible and reviewable without
  creating an unproven abstraction or silently falling back to raw resources.
- **Implications:** Convenience and parameter renaming are not capability gaps; exceptions must name
  the missing AVM capability.
- **Unresolved boundary:** This entry selects no module path or version and does not replace the later
  compatibility confirmation and infrastructure review.
- **Authority:** Settled Owner Decisions item 13 in the 2026-08-30 run record.

## D-014: Use the ProphetsWay naming grammar and root infrastructure layout

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** ProphetsWay resource names follow
  `{context}-{env}-{subContext?}-{type}-{subType?}-{subSubType?}`. Meaningless optional segments are
  omitted, child names represent their attachment paths, and no redundant `ProphetsWay` token is
  added. Repository-owned Bicep and support artifacts live under root `infra/`.
- **Rationale:** Meaningful segments make names readable without redundant branding, while one root
  directory makes infrastructure ownership explicit.
- **Implications:** Every fixture resource must show how its name instantiates the grammar, subject to
  D-006's fixed-database exception.
- **Unresolved boundary:** The readable upstream test-value grammar and publication of this resource
  grammar as a ProphetsWay-wide convention remain separate workstreams.
- **Authority:** Settled Owner Decisions item 14 in the 2026-08-30 run record.

## D-015: Author and validate Bicep only after requirements are ready

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** After the amended requirements receive a `Ready` verdict, context-free Bicep
  authoring, local module restore, build, lint, and parameter validation are authorized.
- **Rationale:** Local validation can prove artifact quality without accessing Azure or incurring
  cloud effects, but it must follow the independently reviewed requirements.
- **Implications:** The authorized work remains context-free and may proceed independently of live
  deployment inputs once the review gate passes.
- **Unresolved boundary:** Azure authentication, `what-if`, deployment, role assignment, DACPAC
  application, database access, testing, recovery, teardown, spend, and release remain unauthorized.
- **Authority:** Settled Owner Decisions item 15 in the 2026-08-30 run record.

## Unresolved Boundaries

The following are unresolved owner inputs, not defaults, recommendations, or decisions. They are
preserved from the **Known Unresolved Inputs** section of the 2026-08-30 run record.

### U-001: Runtime identity and credential flow

The non-UAMI runtime choice remains open among a local interactive owner or developer identity, a
separate Entra principal, and a future workload-federated pipeline identity. This blocks runnable
authentication and any live SQL connection, but not context-free authoring after D-015's review gate.

### U-002: Control-plane and SQL grants

Exact deployment, test, DACPAC, and lifecycle SQL grants remain unset. ARM RBAC and SQL data-plane
grants must remain separate. This blocks role assignment, DACPAC application, test execution, and
disposable-database lifecycle operations.

### U-003: Exact public client address

The current address is known but must not be committed or written to run artifacts. Its approved value
must be supplied through the later authorized path. This blocks firewall parameterization, preview,
deployment, and reachability evidence.

### U-004: Fixed-fixture lifetime and retention

Final fixture expiry, short-term backup retention, and the monitoring owner remain unset. These block
deployment approval and final teardown scheduling.

### U-005: Disposable lifecycle ceilings

Database-create limits and maximum run lifetime remain unset. These block any database-multiplying
test run, even though D-009 settles the retention behavior after an individual outcome.

### U-006: Cost and pricing evidence

The total cost cap and current pricing evidence remain unset. These block preview and deployment
approval and any disposable-database lifecycle.

### U-007: Readable upstream test-value grammar

The canonical readable-value grammar remains unset. This blocks its test-design and fixture-value
workstream, not the infrastructure decision and context-free authoring streams.

### U-008: Convention publication

Publication of the naming grammar as a ProphetsWay-wide convention remains a separate workstream. It
does not authorize changes outside this repository or block applying D-014 to this fixture.

## D-016: Authorize one fresh bounded requirements cycle

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Owner statement:** I authorize one fresh requirements cycle limited to `RR-SA-001`, `RR-SA-002`,
  and `RR-SA-004`, followed by independent re-review. `D-001` through `D-015` remain settled;
  `U-001` through `U-008` remain unresolved and unchanged; exact `ProphetsWay.Example` and durable
  multi-wave behavior must not be reopened. This is requirements-repair authority only and grants no
  repository implementation or live-operation authority.
- **Rationale:** The prior focused review cleared the other findings and left these three blocked on
  owner policy, so a bounded new cycle is required before their requirements can be repaired.
- **Implications:** The fresh cycle may repair only the three named findings and must end in an
  independent re-review. Every dependent threat-model, infrastructure, harness, and live-operation
  stream remains blocked until a new `Ready` verdict and its own later authority.
- **Authority:** The owner's 2026-08-30 message `confirm on all parts`, confirming item 1 in the
  copy-ready owner confirmations of the Product Discovery surviving-decisions report.

## D-017: Reserve unknown-cleanup exposure for every cumulative create attempt

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Owner statement:** For each reviewed wave lifecycle policy, reserve unknown-cleanup exposure once
  for every cumulative create attempt, using $C \times U$ rather than $K \times U$. If finite billing
  cessation cannot be evidenced for every possible attempt, the total is `Unbounded` and the policy
  is ineligible. This selects no cost value or budget and leaves `U-004` through `U-006` unresolved.
- **Rationale:** A concurrency reserve does not bound sequential unresolved survivors when cleanup can
  resolve and creation can later resume; cumulative attempts do.
- **Implications:** The conservative maximum is
  $M = F + N + (C \times O) + (R \times H) + (C \times U)$. A policy without finite cessation proof
  for every possible attempt must be rejected rather than accepted with an understated bound. No
  amount, currency, duration, pricing evidence, or ceiling is selected.
- **Authority:** The owner's 2026-08-30 message `confirm on all parts`, confirming item 2 in the
  copy-ready owner confirmations of the Product Discovery surviving-decisions report.

## D-018: Fail closed while the sole fixture-lifetime authority is unavailable

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Owner statement:** While the sole Fixture-Lifetime Control Authority is unavailable or
  unreconciled, only non-mutating inventory, evidence capture, and reconciliation may proceed. No
  recovery, disposable cleanup, or final teardown may be approved or executed until one valid active
  revision is restored or reconciled, a fresh attempt is durably preallocated there, and separate
  owner approval is recorded. Continuing cost or expiry grants no mutation or deletion authority.
- **Rationale:** Strict non-mutating behavior preserves a single consequential authority and prevents
  continuing exposure from silently becoming destructive authority.
- **Implications:** Authority outage or disagreement cannot be bypassed for recovery, cleanup, or
  teardown. Mutation resumes only after the active revision, durable preallocation, and separate owner
  approval conditions in the owner statement are all satisfied.
- **Authority:** The owner's 2026-08-30 message `confirm on all parts`, confirming item 3 in the
  copy-ready owner confirmations of the Product Discovery surviving-decisions report.

## D-019: Use a cause-preserving closed fixed-database taxonomy

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Owner statement:** Use a cause-preserving closed taxonomy with an explicit `Uninitialized` state.
  Admit `Initialization` only from `Uninitialized`: exact successful proof reaches `Clean`; a
  definitively pre-submission failure remains `Uninitialized`; any submitted non-success reaches
  failure-associated `Contaminated` on complete mismatch or failure-associated `Unknown` otherwise
  and permits only `Recovery` or `FinalTeardown`. `EvidenceProbe` never changes the retained cause or
  broadens mutation authority. A failed, partial, or uncertain teardown remains teardown-associated
  `Unknown` and permits only read-only inspection or a newly approved `FinalTeardown`. `Removed` is
  terminal and refuses every operation, including `Initialization` and repeat `FinalTeardown`.
- **Rationale:** A cause-preserving closed taxonomy gives every lifecycle condition one decidable
  authority boundary without allowing a non-mutating probe to create recovery authority.
- **Implications:** Initialization has one entry state; submitted initialization non-success cannot
  return directly to ordinary work; teardown-associated uncertainty cannot become recovery authority;
  and a removed fixture cannot be reused or operated on again.
- **Authority:** The owner's 2026-08-30 message `confirm on all parts`, confirming item 4 in the
  copy-ready owner confirmations of the Product Discovery surviving-decisions report.

## D-020: Pin the direct AVM composition and administrator principal type

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Use exactly `br/public:avm/res/resources/resource-group:0.4.4`,
  `br/public:avm/res/sql/server:0.22.0`, `br/public:avm/res/sql/server/database:0.3.0`, and
  `br/public:avm/res/sql/server/firewall-rule:0.1.0`. Compose these four modules directly: deploy the
  resource-group module at subscription scope, then deploy the SQL server, fixed database, and
  exact-address firewall-rule modules at the created resource-group scope. No raw-resource exception
  is approved or needed. The selected human Microsoft Entra administrator maps to
  `principalType: 'User'`.
- **Rationale:** The four released AVM modules cover the reviewed resource graph directly, and the
  selected administrator is a human Entra user. The owner confirmed every item in the consolidated
  confirmation after the prior correction established that a new response was required.
- **Implications:** This settles only the four AVM references, their direct composition, the
  raw-resource disposition, and the administrator principal-type mapping. It does not settle runtime
  identity, grants, public address, retention, ceilings, cost, readable values, conventions, or
  live-operation authority. `U-001` through `U-008` remain unresolved and unchanged; no live value or
  operation is authorized.
- **Authority:** The owner's 2026-08-30 message `confirmed on all counts`, recorded in the
  [current run](../../.agent-runs/20260830-1427-azure-bicep-authoring/run.md), directly confirming
  every item in the [Product Discovery AVM confirmation](../../.agent-runs/20260830-1358-azure-requirements-final/08-product-discovery-avm-confirmation.md).
  The [AVM tag proposal](../../.agent-runs/20260830-1358-azure-requirements-final/07-azure-avm-tag-proposal.md)
  supplies the module evidence, and the
  [Session Scribe correction](../../.agent-runs/20260830-1358-azure-requirements-final/10-session-scribe-correction.md)
  records why the owner's latest response was required.

## D-021: Separate root attempt numbers from nested module prefixes

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Owner statement:** Root subscription deployment names use `deploy-{context}-{env}-{NN}`; the
  first is `deploy-example-test-01`, and `{NN}` appears only at the top level and increments for each
  later root deployment attempt. Each attempt's nested prefix concatenates the first lowercase letter
  of `context`, the first lowercase letter of `env`, the first lowercase letter of `subContext` when
  present, and one explicit UTC `MMddHHmm` stamp. For Example test with no subcontext, its grammar is
  `etMMddHHmm`. The prefix is generated exactly once outside Bicep before preview, supplied explicitly,
  and reused byte-for-byte for deployment; Bicep never generates it with `utcNow()`. The prefix is an
  explicit attempt input, not a live Azure value or secret. It must be unused in the fixture-lifetime
  authority: if its minute-derived value is already reserved or used, allocation refuses until the
  operator supplies another unused UTC-minute prefix, without silently reusing, altering, or suffixing
  it. The four nested names are exactly `{prefix}-deploy-rg`, `{prefix}-deploy-sql`,
  `{prefix}-deploy-sql-db`, and `{prefix}-deploy-sql-fw-client`. One root attempt binds its root name,
  prefix, four nested names, and canonical deployment-input digest. Preview, failure, partial creation,
  conflict, changed input, or retry consumes that identity set; a retry uses the next root number and a
  new unused prefix.
- **Rationale:** The root sequence preserves readable, monotonic attempt history while one stable,
  externally allocated nested prefix gives D-020's four direct module deployments distinct names and
  keeps preview and deployment bound to the same identity set.
- **Implications:** Allocation and evidence must treat the root name, explicit prefix, four nested
  names, and canonical deployment-input digest as one atomic attempt identity. A collision or any
  consumed identity refuses reuse rather than repairing or varying part of the set.
- **Supersession scope:** This supersedes D-004 only for its exact top-level and nested name forms and
  its shared nested number suffix. D-004's monotonic, never-reused root attempt identity remains in
  force, and D-020's direct composition remains unchanged. D-001 through D-003 and D-005 through D-020
  remain settled and unchanged; U-001 through U-008 remain unresolved and unchanged.
- **Authority:** The complete Settled Owner Decision section in the
  [current run](../../.agent-runs/20260830-1449-module-prefix-authoring/run.md).

## D-022: Use one human Entra identity as a first-manual exception

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** For the first manual certification attempt only, the owner's existing human
  Microsoft Entra user may serve as deployment operator, logical-server administrator, DACPAC
  publisher, fixed-database test identity, and disposable-lifecycle operator. Every live operation
  still requires separate approval. Authentication uses an interactive Azure CLI sign-in and the
  supported local `Active Directory Default` credential chain. Copied tokens, SQL authentication,
  passwords, client secrets, embedded credentials, and UAMI remain prohibited. Existing subscription
  `Owner` authority governs the ARM control plane; `User Access Administrator` may be used only for an
  expressly approved role assignment. Logical-server Entra-administrator authority governs SQL
  bootstrap and the SQL data plane for this attempt. Neither plane grants the other, and no persistent
  additional owner SQL grant or standing runtime authority is approved. Later automation requires a
  newly approved requirements and threat-model cycle for separate workload-federated deployment,
  DACPAC, test, and lifecycle identities with reviewed least-privilege grants.
- **Rationale:** One explicitly bounded multi-purpose human identity is the smallest viable model for
  the first manual attempt without silently treating that exception as the future automation model.
- **Implications:** The first-manual identity and grant policy is settled, while identity reuse is
  limited to this one attempt and does not combine ARM control-plane authority with SQL data-plane
  authority.
- **Supersession scope:** This resolves the policy questions recorded at U-001 and U-002 for the first
  manual attempt by adding a decision; their historical text is not rewritten. It grants no live
  operation, standing automation authority, persistent additional SQL grant, or later-attempt policy.
- **Unresolved boundary:** Exact tenant, subscription, and principal metadata; sign-in state; any exact
  role assignment or SQL bootstrap action; operation identity and timestamps; and every operation
  approval remain protected live values or separately approved actions.
- **Authority:** The owner's 2026-08-30 message `i approve all 5`, confirming Confirmation 1 in the
  [Product Discovery live-input report](../../.agent-runs/20260830-1846-live-input-discovery/06-product-discovery-live-inputs.md),
  as recorded in the Settled Owner Decisions section of the
  [current run](../../.agent-runs/20260830-1919-managed-risk-cycle/run.md).

## D-023: Protect one just-in-time exact public address

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The fixture permits exactly one canonical global-unicast public IPv4 address, with
  identical firewall-rule start and end values. The literal address is observed and supplied only
  through the protected live-operation path and must not be requested or repeated in chat, source,
  run reports, routine evidence, logs, or Bicep outputs. The operator verifies the current egress
  address immediately before preview and again before every wave. Any change stops the operation and
  requires a new protected input set, preview, review, and deployment approval. No range, second
  address, automatic widening, or Azure-services-wide rule is permitted.
- **Rationale:** A single exact-address rule provides the required reachability without turning a
  privacy-sensitive and mutable operation-time value into durable product data or widening network
  exposure.
- **Implications:** Address equality and freshness are admission conditions for preview and every
  wave; a mismatch fails closed and starts a newly reviewed operation path.
- **Supersession scope:** This resolves the policy question recorded at U-003 by adding a decision;
  its historical text is not rewritten. It selects no literal address and authorizes no preview,
  deployment, firewall change, or wave.
- **Unresolved boundary:** The literal address, its exact observation and verification timestamps,
  the protected input set, and every preview, review, deployment, and wave approval remain protected
  operation-time values or separately approved actions.
- **Authority:** The owner's 2026-08-30 message `i approve all 5`, confirming Confirmation 2 in the
  [Product Discovery live-input report](../../.agent-runs/20260830-1846-live-input-discovery/06-product-discovery-live-inputs.md),
  as recorded in the Settled Owner Decisions section of the
  [current run](../../.agent-runs/20260830-1919-managed-risk-cycle/run.md).

## D-024: Bound fixture retention and require daily owner review

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The durable fixture lifetime is 14 calendar days from the approved deployment,
  short-term backup retention is exactly seven days, and the owner performs a daily review of
  control-authority state, expiry, failed holds, unresolved survivors, and spend. At most one failed
  disposable database may receive a diagnostic hold of no more than 24 hours, only after the owner
  records that the sanitized manifest is insufficient and separately approves that hold; no automatic
  extension is permitted. Every successful disposable database is dropped immediately. Failed,
  expired, second, or presence-unknown survivors remain creation-blocking and separately governed.
  Expiry and hold end are signals, not cleanup or teardown authority. The exact subscription offer and
  backup-cost treatment must be rechecked before approval, and any non-zero unmodeled charge stops the
  operation for cost reconciliation.
- **Rationale:** The selected lifetime, backup window, review cadence, and single exceptional hold
  preserve a narrow diagnostic opportunity while keeping ordinary success retention at zero and
  preventing time passage from becoming mutation authority.
- **Implications:** The owner is the monitoring owner. Daily review and creation-blocking survivor
  rules govern the full 14-day fixture period, while final teardown still requires fresh, separate
  owner approval and D-018 remains unchanged.
- **Supersession scope:** This resolves the lifecycle-policy questions recorded at U-004 by adding a
  decision; its historical text is not rewritten. It grants no cleanup, teardown, hold, deployment,
  or other live-operation approval.
- **Unresolved boundary:** The exact deployment and `expiresOnUtc` timestamps, daily-review records,
  any held database identity and hold timestamps, current subscription offer, backup-cost treatment,
  exact USD operating threshold, and every live-operation approval remain protected or separately
  unresolved.
- **Authority:** The owner's 2026-08-30 message `i approve all 5`, confirming Confirmation 3 in the
  [Product Discovery live-input report](../../.agent-runs/20260830-1846-live-input-discovery/06-product-discovery-live-inputs.md),
  as recorded in the Settled Owner Decisions section of the
  [current run](../../.agent-runs/20260830-1919-managed-risk-cycle/run.md).

## D-025: Apply first-wave disposable lifecycle ceilings

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** For the first one-process, unfiltered 328-case Azure SQL wave, the limits are eight
  concurrent disposable databases, 128 cumulative create attempts, 15 minutes per disposable
  database, and 180 minutes for the complete wave. Every submitted, failed, or outcome-unknown create
  consumes a cumulative attempt. Every unresolved survivor consumes concurrency and blocks new
  creates. A second process, changed test or collection topology, exceeded limit, or attempted increase
  is refused until source counts and concurrency are re-derived and the owner approves a replacement
  policy. The first authorized wave records actual create count, peak concurrency, and timings for
  later review.
- **Rationale:** The limits cover the source-derived 121-create surface and static eight-database
  concurrency bound with explicit cumulative headroom, while treating the unmeasured Azure timings as
  safety ceilings rather than performance claims.
- **Implications:** Admission accounting includes uncertain attempts and survivors, and the first wave
  must produce the measurements needed to assess any later policy without permitting an inferred
  increase.
- **Supersession scope:** This resolves the policy question recorded at U-005 for the first named wave
  by adding a decision; its historical text is not rewritten. It authorizes no database create or wave
  while U-006 and the protected live-operation gates remain unresolved.
- **Unresolved boundary:** The exact wave identity, start and end timestamps, live approval, observed
  create count, peak concurrency, timings, and any replacement policy after remeasurement remain
  operation-time evidence or later owner decisions.
- **Authority:** The owner's 2026-08-30 message `i approve all 5`, confirming Confirmation 4 in the
  [Product Discovery live-input report](../../.agent-runs/20260830-1846-live-input-discovery/06-product-discovery-live-inputs.md),
  as recorded in the Settled Owner Decisions section of the
  [current run](../../.agent-runs/20260830-1919-managed-risk-cycle/run.md).

## D-026: Authorize a reviewed first-manual managed-risk replacement

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** Option 5B is selected. A new requirements and threat-model cycle is authorized to
  replace D-017's hard finite-eligibility guarantee for this first manual certification with explicit
  acceptance of owner-managed residual unbounded-tail billing risk, while preserving D-018's
  non-mutating authority-outage rule. The cycle must define USD operational-budget and stop rules,
  including fresh official and subscription-specific pricing no more than 24 hours before every
  applicable approval, daily owner review, immediate refusal of new creates on the first failed or
  unknown cleanup, missed review, stale pricing, or approved operating-threshold breach, and explicit
  incident escalation and reapproval.
- **Rationale:** No provider-enforced database TTL supplies finite billing cessation for every possible
  create, and D-018 correctly forbids mutation while the sole authority is unavailable or unreconciled.
  The first-manual path therefore must state and govern the residual unbounded tail rather than present
  an operational process or reserve as a hard guarantee.
- **Implications:** `USD 3.1127` is recorded only as the conditional normal-path estimate. `USD 29.86`
  and `USD 184.42` are recorded only as one-day and seven-day illustrative reserves. None is a hard
  cap, selected budget, guarantee, cleanup authority, or deletion authority. No disposable operation
  is authorized until the replacement requirements and threat model are independently reviewed and
  owner-ratified.
- **Supersession scope:** Option 5B supersedes D-017's hard finite-eligibility requirement only for the
  reviewed first-manual certification path and only after independent review and owner ratification of
  the replacement requirements and threat model. Until that ratification, D-017 remains operative and
  the disposable path remains ineligible. D-018 remains settled and unchanged. This decision does not
  ratify the replacement model, govern later automation or another attempt, or authorize a disposable
  operation.
- **Unresolved boundary:** U-006 remains partially unresolved for the exact USD operating threshold and
  post-review owner ratification. Current subscription-specific pricing, backup-cost treatment,
  protected operation-time values, and every live-operation approval also remain unresolved or
  separately approved.
- **Authority:** The owner's 2026-08-30 message `i approve all 5`, selecting exactly Confirmation 5B in
  the [Product Discovery live-input report](../../.agent-runs/20260830-1846-live-input-discovery/06-product-discovery-live-inputs.md),
  as recorded in the Settled Owner Decisions section of the
  [current run](../../.agent-runs/20260830-1919-managed-risk-cycle/run.md).

## D-027: Accept the stated sub-USD-5 estimate as low enough

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** The owner accepts the described estimated Azure use of less than USD 5.00 as low
  enough to continue pursuing the intended Azure deployment.
- **Rationale:** The owner explicitly said, `the estimated use of less than $5 is low enough that I
  want you to go ahead with the deployment into azure`.
- **Implications:** This settles only the acceptability of that described estimate as planning
  context. It does not make USD 5.00 or the stated USD 150.00 monthly quota the immutable
  first-manual operating threshold, select terminal or successor behavior after threshold exhaustion,
  ratify current pricing or the reviewed managed-risk model, or approve an exact preview, deployment,
  firewall change, wave, Azure action, SQL action, or other live operation.
- **Unresolved boundary:** D-023's objective address-observation freshness predicate, D-026's exact
  positive USD operating threshold and post-threshold behavior, fresh authority for one repair and
  focused re-review of `RR-MR-001` through `RR-MR-008`, current pricing, post-review owner
  ratification, protected live values, and every attempt-bound operation approval remain unresolved.
- **Authority:** The owner's 2026-08-30 Azure message quoted in the
  [Product Discovery live-authorization report](../../.agent-runs/20260830-2016-azure-live-authorization/02-product-discovery-live-authorization.md).

## D-028: Set the first-manual stop and observation policy and authorize one bounded repair

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** USD 5.00 is the immutable first-manual operating stop threshold for the D-026
  managed-residual-risk path. Reaching or exceeding it permanently exhausts this ratification for
  every additional disposable create and every later wave. Separately authorized survivor inspection,
  cleanup, and final teardown remain governed by one valid reconciled Fixture-Lifetime Control
  Authority, a fresh attempt durably preallocated there, separate owner approval, and D-018 unchanged.
- **Address-observation freshness:** A protected public-address observation is accepted only when its
  certain age, measured on synchronized operator-host UTC, is from zero through exactly five minutes
  inclusive. Age above five minutes, negative or uncertain age, a backward clock discontinuity, or any
  change to the protected input set, authenticated egress context, network path or configuration,
  operation attempt, or target binding invalidates the observation. This rule governs observation
  freshness; it does not set or shorten firewall lifetime.
- **Repair and review authority:** Exactly one requirements repair limited to `RR-MR-001` through
  `RR-MR-008` is authorized, followed by exactly one focused independent requirements re-review. This
  entry performs neither action, does not predetermine the review verdict, and grants no second repair
  or re-review.
- **Rationale:** The owner explicitly stated, `Set the first-manual stop threshold to USD 5.00,
  terminal for additional creates and waves. Accept the five-minute IP observation rule. Authorize one
  RR-MR-001 through RR-MR-008 repair and one focused re-review. please proceed`, accepting the exact
  five-minute predicate previously proposed by Product Discovery.
- **Preservation scope:** D-001 through D-027 remain settled and unchanged. D-018's non-mutating
  authority-outage rule remains fully operative; D-021's externally allocated prefix rule remains
  unchanged; and D-027 remains planning-context acceptance only. This decision selects the policy
  threshold now but does not supply the separate post-review owner ratification of final reviewed
  requirements and threat-model digests.
- **Exclusions:** This entry records only the settled policy and bounded future repair/re-review
  authority. It does not itself edit or ratify requirements; change the threat model, Bicep, README,
  source, tests, pipeline, release, or any prior decision; disclose a protected literal address;
  select current pricing; approve a live operation; or authorize an Azure, SQL, or Git action. No
  authority beyond the owner's words is inferred.
- **Unresolved boundary:** Exact current subscription pricing and backup-cost treatment; post-review
  owner ratification of the final requirements and threat-model digests; the protected literal address
  and timestamps; exact tenant, subscription, and principal values; operation identities; exact
  preview, deployment, wave, cleanup, and teardown approvals; human execution of Azure mutation; and
  U-007 and U-008 remain unresolved or separately governed.
- **Authority:** The owner's 2026-08-30 authorization quoted above, recorded in the
  [current run](../../.agent-runs/20260830-2034-managed-risk-repair/run.md), confirming the exact policy
  proposed in the
  [Product Discovery live-authorization report](../../.agent-runs/20260830-2016-azure-live-authorization/02-product-discovery-live-authorization.md).

## D-029: Bound per-wave exposure, canonically hash lifecycle policy, and authorize one final repair

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Decision:** For this first-manual ratification, `coveredCumulativeAttempts` is the per-wave upper
  bound exactly `128`. D-025 wave ordinal 1 uses `maxCumulativeDatabases=128`. Every later-wave
  replacement policy under the same ratification still requires fresh source re-derivation and all
  separate approvals and may set `maxCumulativeDatabases` only to a positive integer from `1` through
  `128`, inclusive; each such value is covered by that upper bound. A value above `128` refuses pending
  a newly reviewed owner ratification. Ratification coverage is not later-wave authority.
- **Lifecycle-policy digest profile:** The distinct profile is exactly
  `EFTools.LifecyclePolicy.JCS.v1` and covers the full closed lifecycle-policy object. It first validates
  the complete exact schema, field set, field types, and field-specific constraints, including every
  already-specified canonical UTC and USD string; unknown, duplicate, missing, null, incorrectly typed,
  or noncanonical content refuses. It applies Unicode NFC where the field permits Unicode, preserves
  every stricter exact or ASCII field rule, and canonicalizes the complete object under RFC 8785. The
  digest input is the resulting UTF-8 byte sequence with no BOM and no trailing LF. SHA-256 is rendered
  as exactly 64 lowercase hexadecimal characters. Every producer must obtain byte-identical canonical
  payloads before a digest match is accepted. Acceptance evidence requires an independently reproducible
  positive vector and independently reproducible alternate-encoding refusal vectors.
- **Repair and review authority:** Exactly one fresh requirements repair limited to `RR-MR-002` and
  `RR-MR-008` and their touched consequences is authorized, followed by exactly one focused independent
  re-review of those two findings and touched consequences. This entry performs neither action, does not
  predetermine the review verdict, and grants no further repair or re-review.
- **Rationale:** The owner replied exactly, `Approve the 128 per-wave limit, JCS lifecycle-policy
  hashing, and one final two-finding repair/re-review.`, confirming the expanded proposal preserved by
  the prior managed-risk handoff.
- **Preservation scope:** D-001 through D-028 remain settled and unchanged. This entry adds only the
  same-ratification per-wave exposure relationship, the lifecycle-policy digest profile, and the final
  bounded repair/re-review authority. `DOC-MR-001` remains later non-blocking documentation work.
- **Exclusions:** This entry records only the decision and bounded repair/re-review authority. It makes
  no requirements, README, threat-model, Bicep, source, test, pipeline, release, or prior-decision edit;
  supplies no post-review ratification, current pricing, protected value, or live approval; approves no
  `what-if`, deployment, or other live operation; and authorizes no Azure, SQL, or Git mutation. No
  authority beyond the owner's words is inferred.
- **Unresolved boundary:** The re-review verdict is not predetermined. Current pricing and backup-cost
  treatment, post-review owner ratification, protected inputs, exact operation approvals, `what-if`,
  deployment, and human Azure execution remain later gates.
- **Authority:** The owner's 2026-08-30 authorization quoted above, recorded in the
  [current run](../../.agent-runs/20260830-2129-final-managed-risk-repair/run.md), confirms the exact
  proposal developed in the
  [surviving-questions report](../../.agent-runs/20260830-2034-managed-risk-repair/08-product-discovery-surviving-questions.md)
  and preserved by the
  [prior wrap-up](../../.agent-runs/20260830-2034-managed-risk-repair/09-session-scribe-wrapup.md).

## D-030: Use clause B inputs and authorize protected ratification-candidate assembly

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Pricing and backup decision:** For the GATE-05 candidate, clause B uses the public retail price
  of USD 0.161 per day regardless of any discount or credit. The stated USD 150 monthly quota or
  credit is context only, not an offer, operating threshold, spending cap, or billing guarantee. The
  public East US PITR `Data Stored - Free` meter at USD 0 per GB-month is accepted as the current
  seven-day backup-cost treatment. Any non-zero charge not already modeled stops work for
  reconciliation; it is not absorbed, ignored, or treated as approved spend.
- **Monitoring and notification decision:** G. Gordon Nasseri is the monitoring owner. Protected
  direct notification goes to the same Microsoft Entra account. The route address remains protected
  and must never be requested, printed, or stored in chat or routine evidence; only an opaque route
  reference may cross that boundary.
- **Identifier and assembly authority:** Deterministic non-secret allocation and reservation are
  authorized for `firstManualCertificationAttemptId`, `ratificationRecordId`, `pricingEvidenceId`, and
  `ownerApprovalEvidenceId`. Protected assembly of one canonical
  `EFTools.FirstManualRatification.JCS.v1` candidate is authorized after those identifiers and the
  required protected inputs are available. This entry allocates no identifier, assembles no candidate,
  and records no candidate digest.
- **Gate context:** `GATE-01` is `Ready`, `GATE-02` is `Accepted`, and `GATE-03` is `Pass`. This entry
  supplies only the settled candidate inputs and authority to prepare the protected candidate; it does
  not change or replace the evidence for those gates.
- **Rationale:** The owner's latest words are exactly, `Use pricing clause B. Accept the public
  zero-priced PITR meter, with any unmodeled charge stopping work. Set G. Gordon Nasseri as monitoring
  owner, use protected notification to the same Entra account, and authorize deterministic IDs plus
  protected ratification-candidate assembly.`
- **Preservation and reconciliation scope:** D-001 through D-029 remain settled and unchanged.
  `DOC-MR-001`, U-007, and U-008 remain separate and unchanged; none is resolved, absorbed, or
  authorized by this decision.
- **Exclusions:** This is not exact-digest owner ratification, `what-if` approval, deployment approval,
  Azure or SQL mutation authority, or authority for any operation. This capture makes no requirements,
  README, threat-model, Bicep, source, test, pipeline, release, or prior-decision edit; requests or
  collects no protected literal; creates no candidate; and performs no cloud or Git mutation. No
  authority beyond the owner's words is inferred.
- **Unresolved boundary:** The four allocated identifier values and candidate digest; later exact owner
  ratification of that digest; protected tenant, subscription, principal, route-destination, address,
  and timestamp values; exact `what-if` and deployment approvals; human Azure mutation; and
  `DOC-MR-001`, U-007, and U-008 remain unresolved, protected, or separately governed as applicable.
- **Authority:** The owner's 2026-08-30 statement quoted above, recorded in the
  [current run](../../.agent-runs/20260830-2218-gate05-candidate/run.md), with the pricing and gate
  evidence prepared in the
  [GATE-03 and GATE-05 report](../../.agent-runs/20260830-2129-final-managed-risk-repair/12-azure-ratification-preparation.md).

## D-031: Authorize protected sealing for later exact-digest ratification

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Sealing authority:** Protected sealing is authorized now. At sealing, `ratifiedAtUtc` must be set
  to the true current UTC at the sealing act, the reserved
  `owner-approval-evidence-3048d4d4dbf2b7cb5dd94635` must be bound, and the final
  `EFTools.FirstManualRatification.JCS.v1` RFC 8785 payload and digest must be generated for owner
  review.
- **Ratification and activation boundary:** This authorization is not ratification of the resulting
  digest because that digest was unknown when the authorization was issued. After generation, the
  record remains exactly `SealedAwaitingExactDigestRatification` and inactive. A later owner message
  must ratify the exact resulting digest before GATE-05 activation or any progression to GATE-06.
- **Rationale:** The owner's latest exact words are, `Authorize sealing now: set ratifiedAtUtc to
  current UTC, bind the reserved owner-approval evidence ID, and generate the final JCS payload and
  digest for my review.`
- **Preservation scope:** D-001 through D-030 remain settled and unchanged. This entry records only
  the sealing authority and the mandatory inactive-until-later-exact-digest-ratification boundary.
- **Exclusions:** This entry makes no package, requirements, README, threat-model, Bicep, source,
  test, pipeline, release, or prior-decision edit; collects no protected value; generates no payload
  or digest; and performs no Azure, SQL, Git, `what-if`, deployment, or other cloud action. It grants
  no operation approval, cloud-mutation authority, GATE-05 activation, or GATE-06 authority. No
  authority beyond the owner's words is inferred.
- **Unresolved boundary:** The true sealing timestamp and resulting digest, later owner ratification
  of that exact digest, GATE-05 activation, protected operation values, exact `what-if` and deployment
  approvals, and human Azure mutation remain unresolved, protected, or separately governed as
  applicable.
- **Authority:** The owner's 2026-08-30 statement quoted above, recorded in the
  [current sealing run](../../.agent-runs/20260830-2240-gate05-sealing/run.md), follows the protected
  candidate assembled and described in the
  [candidate report](../../.agent-runs/20260830-2218-gate05-candidate/04-azure-protected-candidate.md).

## D-032: Ratify the sealed digest and authorize bounded GATE-05 activation

**Date:** 2026-08-30 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Exact owner statement:** I ratify the exact digest 3262c83908db02684a54f030ac19953da4c170993b8e103581546bba79381b03 for the sealed first-manual record. This activates GATE-05 only for its bound identity and authorizes no what-if, deployment, or other operation.
- **Ratification and binding:** The owner ratifies exactly
  `3262c83908db02684a54f030ac19953da4c170993b8e103581546bba79381b03` and binds that
  ratification only to first-manual identity
  `first-manual-certification-attempt-e7a53eb114e26bb5bb12dc70`, ratification record
  `ratification-record-4dc1dfb59ca4b9b9b481a7e6`, and reserved owner evidence
  `owner-approval-evidence-3048d4d4dbf2b7cb5dd94635`. No other identity, record, evidence, or
  attempt is covered.
- **Bounded activation authority:** The protected aggregate transition from `Reserved` to `Active`
  and `GATE-05` transition from `Inactive` to `Active` are authorized only after successful
  revalidation of the sealed canonical payload's byte and digest binding, both reviewed-document
  bindings, current pricing evidence, and package ACL protection. The transitions must remain bound
  to the three identifiers and exact digest above. This decision records authority for that bounded
  protected transition; it does not perform package activation itself.
- **No-operation boundary:** This ratification grants no protected operation-input collection,
  `GATE-06` `what-if`, deployment, DACPAC, wave, cleanup, teardown, Azure or SQL mutation, or any
  other operation. Every later operation remains separately gated and requires its own owner
  authorization.
- **Preservation and exclusions:** D-001 through D-031 remain settled and unchanged. This capture
  makes no requirements, README, threat-model, Bicep, source, test, pipeline, release, or
  prior-decision edit; discloses no protected value; and performs no package, Git, cloud, Azure, or
  SQL mutation.
- **Unresolved boundary:** Execution and independent review of the protected activation transition;
  protected operation inputs; `GATE-06` authorization; deployment approval; human Azure mutation;
  and `DOC-MR-001`, U-007, and U-008 remain unresolved, protected, or separately governed as
  applicable.
- **Authority:** The owner's 2026-08-30 exact statement quoted above, recorded in the
  [current activation run](../../.agent-runs/20260830-2256-gate05-activation/run.md), following the
  completed independent
  [sealed-package re-review](../../.agent-runs/20260830-2240-gate05-sealing/05-azure-sealed-package-rereview.md).

## D-033: Replace the stopped agent-led Azure SQL route with the owner's manual route

**Date:** 2026-08-31 | **Owner:** G. Gordon Nasseri | **Status:** Settled

- **Exact owner intervention:** Stop the current agent-led Bicep, ratification, and deployment
  effort. The owner will create a new `solution.bicep`; agents will review that owner-authored file
  for syntax, Bicep restore/build/lint errors, parameter correctness, safety, and deployment risk
  without deploying it. The owner will deploy the Azure resources manually and keep a persistent
  Azure SQL instance for now. After the instance is set up, work resumes with DACPAC publish
  preparation and validation, followed by Azure test configuration analogous to the existing local
  SQL Server workflow.
- **Controlling operational route:** The sequence above is the controlling route for future
  sessions. Review may identify defects, risks, required corrections, and readiness; it grants no
  authority to deploy. Azure resource deployment remains an owner action. DACPAC publication and
  test execution begin only in later, separately authorized work after the deployed instance and
  required protected inputs are available.
- **Closed predecessor route:** The old `GATE-05` / `GATE-06`, protected-package, and successor-
  package correction or activation route is closed and carries no authority into this replacement
  path unless the owner explicitly reopens it. The prior decision entries, gate records, protected
  package, Bicep, reports, and other artifacts remain untouched historical evidence; they are not
  instructions or approvals for the new route.
- **Preservation boundary:** D-001 through D-032 remain verbatim as the historical record. D-033
  supersedes only their operational direction for continuing the Azure SQL certification effort; it
  does not rewrite, erase, or retroactively invalidate what those entries recorded.
- **Next-session inputs:** The owner must provide, or expose through protected means, the deployed
  server FQDN, database name, authentication context, firewall and reachability state, and desired
  DACPAC and test target. The new Bicep contents, deployed subscription, region, resource names,
  persistence duration, future teardown, DACPAC publish approval, and test configuration also remain
  unresolved until the owner supplies or decides them. Agents must not request secrets in chat or
  record secret values in routine reports or repository documents.
- **Human-execution boundary:** Under Vanguard, all mutating Azure commands and DACPAC publish
  commands remain human-executed unless the operating mode is explicitly changed. This decision does
  not authorize an agent to run either kind of command.
- **No-operation exclusions:** This capture makes no Bicep, source, README, requirements,
  threat-model, package, test, or prior-decision edit; performs no deployment, DACPAC
  publication or validation, test execution, Azure or SQL action, or Git action; and creates or
  changes no cloud resource. No authority beyond the owner's manual route is inferred.
- **Authority:** The owner's latest intervention recorded in the
  [current manual-handoff run](../../.agent-runs/20260830-2357-nightly-manual-handoff/run.md) and
  reconciled in its
  [Session Scribe resume](../../.agent-runs/20260830-2357-nightly-manual-handoff/01-session-scribe-resume.md).

## D-034: Record the owner-deployed Group-admin fixture and defer database use

**Date:** 2026-09-06 | **Owner:** G. Gordon Nasseri | **Status:** Settled milestone

- **Exact owner statement:** "i've gotten the bicep updated, and deployed in azure, please update all
  documentation accordingly so i can commit it all currently and then we can work on getting you
  whatever information you need to connect to the instance to run tests against it."
- **Selected route and authority boundary:** The owner-authored source creates the dedicated
  `example-test-sql-admins` Microsoft Entra Group as security-enabled and not mail-enabled, with the
  intended user membership, and assigns that Group as the Entra-only SQL administrator. D-033 remains
  controlling: the owner authors and manually deploys; agents review and document. This records the
  implemented route and completed deployment milestone without redesigning prior requirements or
  adding policy.
- **Live read-only verification:** Parent-supplied evidence records subscription deployment
  `deploy-sql-manually-ggn8` as succeeded in `westus` at `2026-09-07T00:38:42.022112+00:00`;
  resource group `example-test-rg` as successfully provisioned in `westus`; SQL server
  `example-test-sql` as `Ready` with public network access `Enabled`; database
  `ProphetsWay.Example` as `Online` on the `Basic` tier with capacity `5` and maximum size
  `2147483648` bytes; exactly one firewall rule; and the administrator Group as security-enabled and
  not mail-enabled. No object IDs, membership identities, or address values are recorded here.
- **Deferred next workstream:** No DACPAC application or Azure test run is established. Protected
  connection and authentication inputs have not been supplied. DACPAC publication preparation and
  authorization, connection configuration, and Azure test execution remain later work under D-033's
  human-execution boundary.
- **Preservation boundary:** D-001 through D-033 remain verbatim historical record. This entry records
  current state only and grants no Azure, SQL, DACPAC, test, release, or Git operation authority.
- **Authority:** The owner's statement above; the current owner-authored
  `infra/example.solution.bicep` and `infra/group.bicep`; and the parent-supplied live read-only
  evidence recorded for the 2026-09-06 documentation run.
