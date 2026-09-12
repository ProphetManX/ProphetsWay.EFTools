# Feature Requests & Deferred Decisions — ProphetsWay.EFTools

This is the record of things that were **considered**, together with the reasoning behind each decision.
Nothing here is a limitation, an apology, or a TODO list. Each entry exists so a future developer — or a
future AI agent — can find the decision, judge whether the tradeoff that produced it still holds, and
reopen it as a real feature request when it does not.

**If you are about to propose one of these, read its entry first.** The entry tells you what was already
weighed, so your proposal can start from the open questions rather than from the beginning.

**Numbering is per-repository and starts at 1.** It does not continue, mirror, or correspond to the indexes
in [ProphetsWay.BaseDataAccess/docs/feature-requests.md](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md)
or [ProphetsWay.Example/docs/feature-requests.md](../../ProphetsWay.Example/docs/feature-requests.md)
(**1–11** and **1–17** respectively as of 2026-08-23; this preamble previously said 1–9, then 1–13, then
1–14, and all three are stale). Those are
separate indexes.
This file follows their *format*; where an entry genuinely depends on one of theirs, it is cited by
repository and number.

The contracts themselves are **not** restated here. The binding rules live in the XML `<remarks>` on
`IBaseDataAccess` and `DataAccessConventionException` in `ProphetsWay.BaseDataAccess`, and on
`IExampleDataAccess` in `ProphetsWay.Example`. Those are the source of truth. This file links to them and
does not duplicate them, because duplicated rules drift.

The scope bar every entry below is judged against is in
[purpose-and-scope.md](purpose-and-scope.md#settled-one-sentence-purpose), and the owner decisions that set
the statuses below are recorded as **D1–D22** in
[purpose-and-scope.md § Owner Decisions](purpose-and-scope.md#owner-decisions--2026-08-15). **D1–D9 were
taken 2026-08-15, D10–D13 on 2026-08-16, D14–D18 on 2026-08-18, D19 on 2026-08-19 and D20–D22 on
2026-08-24**; the section heading
carries the earliest date only. **This preamble said D1–D10 until 2026-08-19, and that was stale by nine
decisions.**

**A second decision index binds the entries below, and it is not in that file.** `docs/api-contract.md`
carries owner decisions **S1–S13** (2026-08-15) and **OD-1–OD-11**, and several of them close questions this
file and `purpose-and-scope.md` still described as open — **S4** closes **Q3**, **S7** closes **Q2**. Read
all three documents before concluding that anything here awaits a decision.

**Stage 1 is closed as of 2026-08-15.** The two questions that needed the owner — **Q1** (the TFM
exception) and **Q4** (whether the certification scope is stated publicly) — were answered as **D7** and
**D8**. **Q2** and **Q3** remain open and are answerable by whoever implements v3.0.0.

**One owner question has since reopened, and has now been answered — 2026-08-16.** The sentence "no status in
this file is waiting on an owner decision" stopped being true when the `ProphetsWay.Example` submodule
pointer advanced:
[entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) was rescoped on that date
and carried a genuine fork — a local suite here, or a seam upstream — that this agent could not choose.
**The owner chose the seam (shape B) later the same day**, and the question is closed; see
[the resolution](#the-resolution--2026-08-16-shape-b-the-direction-only). **It is again true that no status
here waits on the owner.** What entry 6 now waits on is *another repository* —
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)
— which is a dependency, not an open question.

**The repository builds green — 2026-08-16, and this supersedes every "does not compile" claim below.**
`dotnet build ProphetsWay.EFTools.sln -c Debug` succeeded on SDK 10.0.400 with 7 warnings and every project
compiling. This is the **first verified-green state since the submodule pointer advanced**, and it is what made
this triage pass possible: entries 2, 3, 5 and 8 were previously asserted-but-unverified, and a compiling tree
is the evidence they were waiting on. Sentences elsewhere in this file describing the repository as
non-compiling were true when written and are **history**; they are marked where they appear rather than
deleted. ~~**A green build is not a passing suite** — this repository still contains no tests, by design, under
**D10**.~~ **That last sentence is dead as of 2026-08-23** — `ProphetsWay.EFTools.Tests/` holds 25 source files
and the suite discovers 270 cases. Do not restate it.

## Factual Result Refresh - 2026-09-08

The [canonical filtered-trial result](azure-sql-test-execution.md#filtered-trial-result-2026-09-08)
records **369 passed, 0 failed, 0 skipped** in the owner-run filtered trial, with prior retained local
physical **6/6** evidence accounted separately. [Entry 19](#19--certify-the-contract-suite-against-azure-sql)
records the evidence boundaries. **FR 19 remains `Scheduled` and RELEASE-BLOCKING; Gate 2 remains open
pending the owner's planned 2026-09-09 review.** No request status, number or owner decision changes.

The 2026-09-07 and 2026-09-06 passes below are historical snapshots, including their then-pending
inventory, trial and DACPAC statements. Current evidence supersedes that pending-work wording;
[D-035](decision-log.md#d-035-split-azure-certification-from-physical-database-lifecycle-checks) supersedes
the older all-cases-on-Azure requirement only as already approved. Neither the history nor retention
policy is rewritten by this factual refresh.

## Triage Pass — 2026-09-07 — Gate 2 inventory split approved, FR 19 still open

**Scope verdict: In scope.** The owner said **"approved as written"** to the exact scope recorded in
`.agent-runs/20260907-1550-azure-certification-split/run.md` under the project parent: keep physical database
create/drop tests mandatory locally but exclude them explicitly from Azure; retain all DAO, constraint,
isolation, disposal, and transaction contract coverage on Azure; use the fixed Example database plus the
minimum explicitly configured reusable scratch databases; serialize shared-store use while preserving
simultaneous isolation; make reset failures fail; and report exact exclusions with no silent passes or
unexpected skips.

The exact minimum scratch capacity must be derived and explained before any scratch-store provisioning.

The existing 14-case provider-comparison exception remains a separate selection exception. The concrete
physical-lifecycle exclusion inventory is still owed from `Test Designer` and the fixture implementation from
`Test Harness Engineer`; this pass creates no broad exemption and invents no counts.

**No status transition is applied. [Entry 19](#19--certify-the-contract-suite-against-azure-sql) remains
`Scheduled` and RELEASE-BLOCKING.** No Azure certification run under this approved inventory split exists.

## Triage Pass — 2026-09-06 — Azure SQL fixture deployed, FR 19 still open

**Scope verdict: In scope.** Recording the deployment milestone serves the settled purpose and changes no
package surface or certification bar.

[D-034](decision-log.md#d-034-record-the-owner-deployed-group-admin-fixture-and-defer-database-use)
records the owner-deployed `westus` fixture: a provisioned resource group, a ready SQL server with public
network access, the Online Basic 5 DTU / 2 GiB `ProphetsWay.Example` database, one firewall rule, and a dedicated
security-enabled, non-mail-enabled Microsoft Entra administrator Group. Exact live identifiers and address
values are deliberately not copied here. [D-033](decision-log.md#d-033-replace-the-stopped-agent-led-azure-sql-route-with-the-owners-manual-route)
remains the controlling owner-manual route.

**No status transition is applied. [Entry 19](#19--certify-the-contract-suite-against-azure-sql) remains
`Scheduled` and RELEASE-BLOCKING.** Deployment is a completed prerequisite, not certification: no DACPAC
application or Azure certification run under the approved inventory split is established, and protected
connection and authentication inputs remain deferred to the next workstream.

## Triage Pass — 2026-08-29 — Gate 1 satisfied, FR 11 still Scheduled

**Scope verdict: In scope.** Recording provider-certification evidence serves the settled purpose directly:
the library promises provider-neutral relational plumbing, and D20 makes a demonstrated local SQL Server run
the release bar for that claim. It changes neither the purpose sentence nor the package surface.

The local SQL Server half of [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
is now **satisfied**. The authoritative execution record is
`.agent-runs/20260829-1450-eftools-fr11-gate1/07-test-designer-certification-run.md` under the project parent:

| Gate 1 measure | Observed 2026-08-29 result |
| --- | ---: |
| External provider selection | `EFTOOLS_PROVIDER=SqlServer` |
| Unfiltered whole suite | **328 discovered / 328 passed / 0 failed / 0 skipped** |
| Provider-comparison spike | **14 selection-exempt cases, all executed and passed** |
| Provider-honouring cases | **314** |
| Provider-selection guards | **58 passed / 0 failed / 0 skipped** |
| Disposable `EFToolsTest_*` databases | **0 before / 0 after** |

Those numbers are a dated run result, not a permanent suite-size promise. They discharge the five Gate 1
criteria in `purpose-and-scope.md`: external selection, no-filter green execution, the single closed
selection exemption, a passing guard, and recorded provider/count evidence.

**No status transition is applied. FR 11 remains `Scheduled`.** Its whole-suite SQLite certification and
pipeline/`LocalTestsOnly` obligations remain open under D2/D4. They are not release blockers. Gate 1 being
satisfied also does not satisfy [entry 19](#19--certify-the-contract-suite-against-azure-sql): Azure SQL
remains the separate, unsatisfied Gate 2 and the remaining certification blocker on the 3.0.0 tag.

The Security Reviewer found one Low, test-only localhost transport issue. The full index contained no request
covering it, so it is captured once as new [entry 20](#20--decide-the-localhost-sql-server-certificate-validation-boundary),
`Proposed`. No remediation or owner decision is inferred.

## Triage Pass — 2026-08-24 — **the release gate**

> 🔴 **READ THIS BEFORE THE 2026-08-23 PASS BELOW. One entry moved from "not a blocker" to "the blocker,"
> and every sentence in this file that says otherwise is superseded.**

**The owner stated the release bar for the first time, and it lands squarely on
[entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).** Verbatim:

> *"if we can't say EFTools is cleared to work in SQL Server, then it's not worth pushing out."*

**`ProphetsWay.EFTools` 3.0.0 stays untagged and unpublished until a whole-suite SQL Server run exists.**
Recorded as [D20](purpose-and-scope.md#owner-decisions--2026-08-15); the environment and the target run are
[D21](purpose-and-scope.md#owner-decisions--2026-08-15). **No version bump, tag or publish is authorized by
this pass**, and none is an agent's to take in any case.

| Moved | Entry |
| --- | --- |
| **Not release-blocking → RELEASE-BLOCKING** | **11** — status token **unchanged at `Scheduled`**; what changed is its classification. No `Proposed`/`Deferred`/`Done` transition was taken or authorized |
| **Newly filed — `Scheduled`, RELEASE-BLOCKING** | **19** — Azure SQL certification, filed later the same day under [D22](purpose-and-scope.md#owner-decisions--2026-08-15). See [the second gate](#later-the-same-day--azure-sql-becomes-a-second-gate-d22) |
| Unchanged | 16, 17, 18 — still `Proposed`, still non-breaking, still not blockers |

**Three claims in the 2026-08-23 pass below are now false and are corrected in place rather than deleted:**
*"none of them is a blocker"*, *"Nothing needs to be built before 3.0.0 is published"*, and *"What remains
before the package is on nuget.org is release mechanics, not work items."* Each was **true when written** —
the judgement that certification was CI evidence rather than correctness was sound against the information
available, and it was the owner supplying a *purpose* for the library, not a defect being found, that
overturned it. That is why the pass is amended and not rewritten.

### What did **not** change, and must not be read as cancelled

**[D2](purpose-and-scope.md#owner-decisions--2026-08-15), [D4](purpose-and-scope.md#owner-decisions--2026-08-15)
and [D8](purpose-and-scope.md#owner-decisions--2026-08-15) all stand.** They certify **SQLite *and* SQL
Server** and put that claim on the package. D20 does not withdraw the SQLite half — it says which half is the
**gate**:

| | Certification on SQL Server, whole suite | Certification on SQLite, whole suite |
| --- | --- | --- |
| **Owner decision** | D20 — **release gate** | D2 / D4 / D8 — still owed, **not** the gate |
| **Blocks the 3.0.0 tag?** | **Yes** | **No** |
| **Why the split** | It is the provider the one real consumer deploys on — local MSSQL for development, Azure SQL in production | It is the fast CI leg D4 wants, and CI runs none of the suite today anyway ([`LocalTestsOnly`](#the-localtestsonly-half-is-still-pipeline-engineers)) |

**Nothing here authorizes weakening D8's public wording to "SQL Server only."** The certified tier is still
two providers; the SQLite evidence is *later*, not *withdrawn*. An agent that reads this pass and edits the
package `<Description>` down to one provider has misread it — and see [Q4](#q4--closed-the-certification-is-a-public-claim),
where the wording turns out not to be on the package at all yet.

### Still undecided, and not decidable here

> ✅ **ANSWERED later the same day — see [the second gate](#later-the-same-day--azure-sql-becomes-a-second-gate-d22).**
> The paragraph below was correct for a few hours on 2026-08-24 and is kept because its *reasoning* about
> `EnableRetryOnFailure` is unchanged and still governs the work. **Its conclusion — "no status is filed for
> it" — is dead: [entry 19](#19--certify-the-contract-suite-against-azure-sql) is filed and blocking.**

~~**Azure SQL as an additional certification leg is an open owner decision.**~~ The owner named two environments —
local MSSQL for development and debugging, **Azure SQL for production** — and certifying the first does not
certify the second. `EnableRetryOnFailure` is effectively mandatory against Azure SQL and an EF Core execution
strategy is incompatible with a user-initiated transaction unless it is wrapped in `ExecuteInTransaction`,
which lands **on this library's own transaction contract** rather than on the consumer's code. ~~**No status is
filed for it and none is invented**: it needs the owner's scope call first. Until then D20 is a **SQL Server**
gate and says nothing about Azure SQL.~~ **The scope call was made; the status exists.**

### Later the same day — Azure SQL becomes a second gate (D22)

**The owner answered the question above. Verbatim:**

> *"yes, option 1 please, mssql server and azure sql both need to be covered, i don't have an azure sql
> instance setup just yet, but if we get bicep, we shoudl be able to deploy an instance from cli and us that
> to test?"*

`option 1` is the offered choice **"Azure SQL is a certified target before 3.0.0"** and is interpreted as
nothing wider. Recorded as [D22](purpose-and-scope.md#owner-decisions--2026-08-15).

**At that point there were two certification gates on the 3.0.0 tag, and neither had been satisfied:**

| Gate | Provider | Entry | Evidence on 2026-08-24 |
| --- | --- | --- | --- |
| **1** | **Local Microsoft SQL Server** | [11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | Server reachable; `ProphetsWay.Example` schema has a **zero-drift DACPAC deploy report**; **the whole-suite run has not happened.** ~~the 270-case run~~ — the count moved to **287** on 2026-08-24 and 14 cases are exempt from the *selection*; see [the restatement](#the-close-condition-restated-2026-08-24--the-number-moved-the-bar-did-not) |
| **2** | **Azure SQL** | [19](#19--certify-the-contract-suite-against-azure-sql) | **At that date:** no subscription named, no resource deployed, no run attempted |

**Read the second row as the historical D22 snapshot, not current state.** At that date no Azure resource or
result existed. D-034 records the owner-deployed fixture; the
[2026-09-08 filtered trial](azure-sql-test-execution.md#filtered-trial-result-2026-09-08) supplies result
evidence and owner-reported DACPAC seed context for review, without closing Gate 2.

**Historical D22 authority boundary.** Infrastructure could be **authored as Bicep, built and
previewed**; `what-if` needs an available Azure context; **the deployment itself and the later teardown are
two irreversible steps, each requiring the owner's explicit approval at the moment it is taken.** D22 is not
that approval. **No version bump, tag, publish, `.yml` change, Azure resource creation or database mutation
was authorized by that pass.** D-033 later replaced this with the owner-manual route, and D-034 records the
owner's completed deployment.

**Gate 1 runs first, and the two bodies of work stay separate.** The immediate lap is local: provider-selectable
test wiring that the seven store-backed local classes actually consume, then a **whole-suite run** against the
local instance meeting the five criteria in
[entry 11's restatement](#the-close-condition-restated-2026-08-24--the-number-moved-the-bar-did-not). Bicep,
`what-if`, deployment, the Azure DACPAC apply and the Azure run are
[entry 19](#19--certify-the-contract-suite-against-azure-sql). At that date they started from an
infrastructure design the owner had not settled; D-034 now records the deployed fixture. **A Stage 3 lap
that reached for Azure was out of its own scope**, and debugging test
wiring and cloud infrastructure simultaneously against a billed resource with no local baseline is the
failure that ordering avoids.

**The SQLite whole-suite leg is still owed under D2/D4 and is still not a gate** — and it is **not a
substitute** for either SQL Server gate. Two gates and one deferred leg; do not collapse them into one item.

## Triage Pass — 2026-08-23

**Eleven entries closed, two narrowed, three filed.** Run by `Purpose Refiner` against the owner's stated goal:
*"i want to publish EFTools and know that all of these projects are 'finished' for now, with no outstanding
problems/bugs/features that we'll need to build out within a week."*

**Every closure below was verified by opening the artifact, on this date.** No status moved on an inherited
claim, and where the claim could not be checked without a terminal it is recorded as the owner's measurement
rather than as verified — see [entry 18](#18--committed-trx-files-under-testresults-are-read-as-current-and-are-not).

| Moved | Entries |
| --- | --- |
| `Scheduled` → **`Done`** | 1, 3, 4, 5, 6, 7, 9, 10, 13 |
| `Proposed` → **`Done`** | 14, 15 |
| **Narrowed, still open** | 11 (certification), 12 (the release note) |
| **Newly filed as `Proposed`** | 16 (Source Link and packaging), 17 (dead preprocessor guards), 18 (stale `.trx` artifacts) |

### Closing lap, later the same day — entry 12 closes, and with it the last release blocker *then known*

**`Changelog Author` discharged the D12 obligation between the pass above and this one.** `CHANGELOG.md` was
reopened on 2026-08-23 and every heading in its v3.0.0 entry re-read; **all four defects are now named as
`Fixed`, and the up-front 2.2.0 known-issues note is present with the `net10.0`-only caveat stated plainly.**
The field-by-field check is on
[The `Changelog Author` Obligation](#the-changelog-author-obligation--d12), and it is the *only* copy — the
four citing entries point at it rather than restating it.

| Moved | Entries |
| --- | --- |
| `Scheduled` (release note) → **`Done`** | **12** |
| Note corrected on an entry already `Done` | 3, 13, 14 — each carried *"release note still owed"*; that clause is now false and is struck where it appears |

> ⛔ **SUPERSEDED 2026-08-24 — the two bolded sentences below are false.**
> [Entry 11 is now a release blocker](#triage-pass--2026-08-24--the-release-gate) by owner decision
> **[D20](purpose-and-scope.md#owner-decisions--2026-08-15)**. They are kept because the *reasoning* that
> produced them — that nothing outstanding was breaking, and that certification was evidence rather than
> correctness — is still correct on its own terms and is what a reader needs in order to see why a statement
> of **purpose** rather than a discovered defect is what overturned it. **Read them as history.**

~~**Nothing in this repository now blocks publishing 3.0.0.** The section below is rewritten to say so.
**Entries 11, 16, 17 and 18 remain open and none of them is a blocker**~~ — that judgement is unchanged from
the pass above and was not re-litigated here. **16, 17 and 18 are still not blockers. 11 is.**

**Three claims elsewhere in this file are now not merely stale but *wrong*, and are struck where they appear:**

1. [Entry 7](#7--stop-forcing-a-database-provider-on-every-consumer) — *"All three are still present as of
   2026-08-16."* The two provider references are gone from the library `.csproj`.
2. [Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) fact 2 — *"The
   provider this package currently depends on is structurally unable to verify the contract."* The package
   depends on no provider.
3. The preamble sentence above — *"this repository still contains no tests."*

### The answer to the owner's question

> **Rewritten 2026-08-23, later the same day. The one item this section named has landed.** The paragraph it
> replaced said *"Exactly one item needs to be built before 3.0.0 is published, and it is not code"* and
> pointed at the unmet D12 release note. **That is no longer true**, and the strikethrough is not enough —
> the sentence would be read as current by anyone skimming for a blocker.

> ⛔ **SUPERSEDED 2026-08-24. This heading answers a question the owner asked on 2026-08-23 —
> *"is anything unfinished that we'll need within a week"* — and its answer was right for that question.**
> On 2026-08-24 the owner asked a **different** one and answered it themselves: *what is this library for, and
> what would make it fit to publish.* The answer is
> [the SQL Server gate](#triage-pass--2026-08-24--the-release-gate). **One thing does need to be built before
> 3.0.0 is published, and it is test wiring** — see [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).

~~**Nothing needs to be built before 3.0.0 is published.**~~ **One thing does: a whole-suite SQL Server run
(D20).** Everything in the paragraph below remains true about the *D12* obligation specifically.

The [D12 release-note obligation](#the-changelog-author-obligation--d12) — ~~the only item that ever blocked the
push~~ **the only item that blocked it as of 2026-08-23** — was discharged by `Changelog Author` on 2026-08-23 and **verified here by reopening `CHANGELOG.md`**,
not by accepting the report. All four shipped 2.2.0 defects are named as `Fixed`, each with the
silent-failure characteristic that made D12 an obligation rather than a formality, and the entry opens with a
2.2.0 known-issues section a reader finds without reading the rest of it. **The trade D12 made — four patches
for four release notes — has been paid on both sides.**

**Entries 16, 17 and 18 are comfortably deferrable** — in order: non-breaking packaging metadata; four dead
directive lines per file in an unshipped project; and a housekeeping decision about committed test artifacts.
**None of them is breaking, and none of them blocks a publish.** ~~Entry 11 is CI evidence for a claim not yet
printed on the package, and is equally deferrable.~~ **Struck 2026-08-24 — entry 11 is a gate ([D20](purpose-and-scope.md#owner-decisions--2026-08-15)).**
The "CI evidence" framing was the error: the gate is not that a *build agent* runs the suite, it is that
**anyone has run all 270 against SQL Server even once.**

~~**What remains before the package is on nuget.org is release mechanics, not work items** — the D11 ordering,
the tag, and the push.~~ ~~**Superseded 2026-08-24.** What remains is **one work item and then release
mechanics**~~ — **corrected later the same day: TWO work items and then release mechanics.**
[Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)'s whole-suite run
against **local SQL Server**, *and*
[entry 19](#19--certify-the-contract-suite-against-azure-sql)'s approved certification inventory against
**Azure SQL**
([D22](purpose-and-scope.md#owner-decisions--2026-08-15)); *then* the D11 ordering, the tag and the push.
The mechanics half of that sentence is unchanged and is still
not an agent's to do.

## Index

| # | Item | Status |
| --- | --- | --- |
| 1 | [Advance the `ProphetsWay.Example` submodule onto the 3.x contracts](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) | **Done** — 2026-08-23; all six implementation steps landed, both new entities are mapped, and both DAOs exist. The exact submodule pointer is mutable and belongs in runtime evidence, not this current-status row. **The D11 step 3 move onto a tag is release mechanics, not this entry** |
| 2 | [Move the `ProphetsWay.BaseDataAccess` reference from 2.5.0 to 3.1.0](#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310) | **Done** — 2026-08-16; both consuming projects now reference **3.2.0**, re-verified 2026-08-29 by reopening both project files |
| 3 | [Implement the 3.x disposal contract in `BaseEFDataAccess`](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | **Done** — 2026-08-23; `Dispose` is `sealed override` and **all ten** other members open with `ThrowIfDisposed()`. The **2.2.x patch stays `Rejected`** — [D12](purpose-and-scope.md#owner-decisions--2026-08-15) — and its [release-note obligation](#the-changelog-author-obligation--d12) is now **discharged**: `CHANGELOG.md` § *"Fixed: a Data Access Layer no longer leaks its context and its connection"*. **The "still owed" clause this row carried is dead** |
| 4 | [Make 3.x Entity Framework Core-only — retire EF6 and .NET Framework](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) | **Done** — 2026-08-23; `net10.0` alone, no EF6, and **zero preprocessor directives library-wide** |
| 5 | [Retarget to the house TFM standard](#5--retarget-to-the-house-tfm-standard) | **Done** — 2026-08-23; all three projects read `net10.0`, the destination **D7** ratifies |
| 6 | [Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory and `Scope` traits](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) | **Done** — 2026-08-23; shape B is built. `TestSeam` and 13 adapters use `ProphetsWay.Example` FR 13's seam; later provider-selection fixtures and guards belong to [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| 7 | [Stop forcing a database provider on every consumer](#7--stop-forcing-a-database-provider-on-every-consumer) | **Done** — 2026-08-23; **both halves.** The library references `Microsoft.EntityFrameworkCore` alone, `BaseEFContext` names no provider and its string constructor is gone, and `.InMemory` moved to the test project |
| 8 | [Remove `FluentAssertions` from `ProphetsWay.Example.DataAccess.EF`](#8--remove-fluentassertions-from-prophetswayexampledataaccessef) | **Done** — 2026-08-16; the reference is gone and the licence exposure is closed |
| 9 | [Delete the stray `[submodule "Submod"]` block from `.gitmodules`](#9--delete-the-stray-submodule-submod-block-from-gitmodules) | **Done** — 2026-08-23; `.gitmodules` declares one submodule and no `Submod`. **It stopped the warning; it did not give this package Source Link** — see [entry 16](#16--source-link-symbol-packages-and-the-empty-packaging-metadata-stubs) |
| 10 | [Collapse the `Guid`/`Int`/`Long` DAO triplication](#10--collapse-the-guidintlong-dao-triplication) | **Done** — 2026-08-23; the folder is flat and all 15 files declare `namespace ProphetsWay.EFTools`. Approved by **D3**, constrained by **D13** |
| 11 | [Certify the contract suite on SQLite in-memory and a SQL Server container](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | **Scheduled — local SQL Server Gate 1 satisfied 2026-08-29.** The externally selected, unfiltered run was **328 / 328 / 0 / 0**, with 14 selection-exempt cases executed, 314 provider-honouring cases, 58 passing guards, and 0 disposable databases before and after. The **SQLite** whole-suite leg and pipeline/`LocalTestsOnly` half remain owed under D2/D4 and are not release blockers. [Entry 19](#19--certify-the-contract-suite-against-azure-sql) remains separate, unsatisfied Gate 2 |
| 12 | [`RootNonIdDao.EnsureBeginTransaction` silently no-ops against a pre-existing transaction](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | **Done** — 2026-08-23, **both halves**. The code half was always moot (the member is gone); the release-note half landed and was verified by reopening `CHANGELOG.md` — § *"Fixed: commit and rollback no longer silently do nothing"*. **The 2.2.x patch stays `Rejected`**, which is decision history and does not reopen. ~~**This was the last release blocker in the repository**~~ — **struck 2026-08-24: it was the last blocker *then known*.** [Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) became one under [D20](purpose-and-scope.md#owner-decisions--2026-08-15). This entry is unaffected and stays `Done` |
| 13 | [The soft-delete and keyless DAO bases cannot serve the 3.x contracts by inheritance](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | **Done** — 2026-08-23; `RootBaseSoftDao` is deleted and the soft members are `override`s. **2.2.x patch stays `Rejected`**; its release note is **discharged** — `CHANGELOG.md` § *"Fixed: Update no longer un-deletes a soft-deleted row"*, which also names the second-`Delete` half |
| 14 | [The Entity Framework DAO bases adopt the caller's instance, violating the SNAPSHOT rule](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) | **Done** — 2026-08-23; carried inside entry 10 under **D14**. Its release note is **discharged** — `CHANGELOG.md` § *"Fixed: a read or a write no longer hands you the store's own object"*, **including the "what stops happening" paragraph D12 says this one uniquely owes**. The clause this row carried saying that sentence was absent is dead |
| 15 | [`ProphetsWay.EFTools.Guid` shadows `System.Guid` inside this assembly](#15--prophetswayeftoolsguid-shadows-systemguid-inside-this-assembly) | **Done** — 2026-08-23; closed for free inside entry 10, as predicted. The namespace does not exist, so the collision cannot occur |
| 16 | [Source Link, symbol packages, and the empty packaging metadata stubs](#16--source-link-symbol-packages-and-the-empty-packaging-metadata-stubs) | **Proposed.** The implementation facts are now satisfied by `e2f2120`, but no quoted owner decision authorizes a status transition; see the current-fact note in the entry |
| 17 | [Five proving-ground DAOs carry dead preprocessor guards](#17--five-proving-ground-daos-carry-dead-preprocessor-guards) | **Proposed** — filed 2026-08-23. Cosmetic, unshipped, and **misleading in the one project whose job is to be copied from** |
| 18 | [Committed `.trx` files under `TestResults/` are read as current and are not](#18--committed-trx-files-under-testresults-are-read-as-current-and-are-not) | **Proposed** — filed 2026-08-23. Seven committed run artifacts, **every one of them stale**, in a repository whose documents cite test counts as evidence |
| 19 | [Certify the contract suite against Azure SQL](#19--certify-the-contract-suite-against-azure-sql) | 🔴 **Scheduled — RELEASE-BLOCKING.** Filed 2026-08-24 under [D22](purpose-and-scope.md#owner-decisions--2026-08-15): *"mssql server and azure sql both need to be covered."* **Gate 2** on the 3.0.0 tag, beside [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)'s Gate 1. **D-034 records the owner-deployed Azure SQL fixture.** The [2026-09-08 filtered trial](azure-sql-test-execution.md#filtered-trial-result-2026-09-08) records 369 passed / 0 failed / 0 skipped; retained local physical 6/6 remains separate. DACPAC seed state and Azure targeting are owner-reported. Evidence awaits the owner's review; Gate 2 is not closed |
| 20 | [Decide the localhost SQL Server certificate-validation boundary](#20--decide-the-localhost-sql-server-certificate-validation-boundary) | **Proposed** — filed 2026-08-29 from the Security Reviewer Low finding. Test-only and localhost-only; no remediation or owner decision has been taken |

Numbers are permanent. Entries are never renumbered and never removed —
[purpose-and-scope.md](purpose-and-scope.md) cites entries by number, and a rejected entry is decision
history rather than dead weight.

## Release Ordering — D11

**Owner decision, 2026-08-16, recorded in full as
[D11](purpose-and-scope.md#owner-decisions--2026-08-15) and reasoned through in
[purpose-and-scope.md § Release Ordering](purpose-and-scope.md#release-ordering--settled-d11).** It is
summarized here because it governs *when* the entries below ship, and a reader planning the release from this
file would otherwise not meet it.

1. **All remaining 3.x work here is done against the live `ProphetsWay.Example` submodule working tree**, not
   against a tagged release of it.
2. **`ProphetsWay.Example` is tagged and released** once this repository's Entity Framework implementation is
   **green against it**.
3. **This repository's submodule pointer is advanced to that tag.**
4. **`ProphetsWay.EFTools` 3.0.0 is pull-requested, merged and published.**

> 🔴 **Step 4 gained two preconditions on 2026-08-24, and D11 itself is otherwise unchanged.** Both
> [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) (whole suite,
> **local SQL Server**, [D20](purpose-and-scope.md#owner-decisions--2026-08-15)) and
> [entry 19](#19--certify-the-contract-suite-against-azure-sql) (approved certification inventory,
> **Azure SQL**,
> [D22](purpose-and-scope.md#owner-decisions--2026-08-15)) must be green **before** step 4 runs.
> **Steps 1–3 are unaffected**, and step 2's *"green against it"* still means the ordinary suite result, not
> either certification.

`ProphetsWay.BaseDataAccess` **3.1.0 is already published and current** and needs no release in this sequence.

**The reason the order is this way round:** implementing against a contract is what exposes gaps *in* it.
`IDepartmentDao` rule 18 was **narrowed on 2026-08-16 as a direct result of Entity Framework design work
here**, changing `ProphetsWay.Example`'s interface. Tagging that repository first risks discovering a second
rule 18 and needing a second release of it — the double build / pull request / merge D11 exists to prevent.

**No entry below changed status because of D11.** It constrains sequence, not scope. In particular it does not
reopen [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 1, which has landed;
D11 governs the **next** pointer move, which is step 3 above.

### Clarification recorded 2026-08-16 — an interim pointer advance is **not** a D11 violation

D11 step 1 says the remaining 3.x work is done *against the live `ProphetsWay.Example` submodule working
tree*. **That premise silently assumed the submodule could see the live work. It cannot** — the submodule
tracks `main`, and the work is on `ProphetsWay.Example`'s `3.1.1-eftool-findings` branch. Concretely,
`TestDataAccessFactory.Use` — the seam that lets this repository run the upstream suite against Entity
Framework without editing a file it is forbidden to edit — is not in the code this repository compiles
against, so nothing downstream of it can move.

**Advancing the pointer onto in-progress upstream work is a different act from step 3.** Step 3 advances the
pointer onto a **tag**, after a green run. An interim advance onto a merged-but-untagged `main` is what makes
that green run *possible*. D11 defers **tagging and releasing** `ProphetsWay.Example`; it does not require
this repository to compile against a stale contract while doing the very work that proves the contract.

**No status changed. This is a note about the scope of an owner decision, not a change to it** — if the owner
reads it otherwise, D11 as written wins and this paragraph is what should be corrected.

## The `Changelog Author` Obligation — D12

**Not this agent's to execute, and it must not be lost.** It is stated once, here, and cited from
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess),
[entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) and
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) rather
than restated in each — three copies of an obligation drift, and the whole point of it is accuracy.

> ## DISCHARGED — 2026-08-23
>
> **Every requirement below is met.** `Changelog Author` wrote the sections; `Purpose Refiner` verified them
> by **reopening `CHANGELOG.md` and reading each heading and its body**, not by accepting the report. The
> requirements are left standing beneath this block rather than deleted, because the record of *what was
> owed* is what makes the discharge checkable by the next reader.
>
> **Requirement 1 — all four defects named as `Fixed`.** Met, four for four:
>
> | Entry | Required as `Fixed` | Section in `CHANGELOG.md` v3.0.0 | Silent-failure characteristic stated? |
> | --- | --- | --- | --- |
> | [3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | leaked context and connection per DAL instance | *"Fixed: a Data Access Layer no longer leaks its context and its connection"* | **Yes, and correctly inverted** — this is the one of the four that *does* announce itself, and the section says so: connection-pool exhaustion, *"findable once you suspect it"* |
> | [12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | commit and rollback silently no-op | *"Fixed: commit and rollback no longer silently do nothing"* | **Yes** — quotes the 2.2.0 `CurrentTransaction == null` guard, and states *"No exception, no return value indicating a skip, nothing in a log"* |
> | [13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | `Update` wipes the stored `DeletedDate`; a second `Delete` returns `1` | *"Fixed: Update no longer un-deletes a soft-deleted row"* | **Yes** — *"Nothing threw and no row count looked wrong."* Names the second-`Delete` half explicitly, **and a third half this obligation never asked for**: `Insert` carrying a stale `DeletedDate` into a new row |
> | [14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) | the caller's instance stayed tracked | *"Fixed: a read or a write no longer hands you the store's own object"* | **Yes** — *"It is silent, and unlike the leaked connection the damage lands in your data rather than in your process"* |
>
> **Entry 14's second, unique obligation is also met** — the one D12 says it owes and the other three do not.
> The section carries a dedicated bolded paragraph, *"What stops happening, which is the part to read even if
> none of the above sounds familiar"*, covering **both** halves: a stray edit is *"now silently dropped where
> it used to be silently applied"*, and `Update` against an absent row returns `0` where 2.2.0 threw out of
> `Single`, so **a `catch` a consumer wrote stops firing**. That is the hardest thing on this list to write
> and it is the one written most fully.
>
> **Requirement 2 — a 2.2.0 known-issues note naming all four, with the target-framework caveat.** Met, and
> placed better than the requirement asked. It is § *"If you are on 2.2.0: four defects you are exposed to,
> and three of them are silent"*, and it sits **at the top of the entry**, self-describing as being there *"so
> that a reader who is not upgrading does not have to read the rest of the entry to find them."* It states
> that never having noticed one is not evidence of not being affected; it records **"No 2.2.1 will be cut"**
> together with the near-empty-consumer-base premise **as a premise**, which is exactly the re-testable form
> this file asked for; and it carries the D7 caveat in the words the obligation demanded — *"That remedy is
> not available to everyone… if you are on `net48`, `net8.0` or `net9.0` you cannot take it at all."*
>
> **It goes past the obligation in one place, and the surplus is the right kind.** For the consumers who
> cannot upgrade, it supplies **in-their-own-code mitigations** — short-lived layers, driving transactions
> through `Context.Database`, never passing a soft-deleted entity to `Update`, and
> `QueryTrackingBehavior.NoTracking` with the honest note that it closes the `Get` half of the fourth defect
> **and not the `Insert` half**. D12 asked that the note not *imply* an unavailable remedy; this offers a real
> one instead, and qualifies it.
>
> **Nothing was found thinner than D12 requires.** Had any of the four been, it would be recorded here as
> still owed rather than closed — a false close on this obligation is worse than an open row, because it is
> the last thing standing between the package and the push.
>
> **Statuses moved on this finding:** entry 12 `Scheduled` → **`Done`**. Entries 3, 13 and 14 were already
> `Done` and keep that status; only their *"release note still owed"* clauses were struck.

**Owner decision [D12](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16: the three shipped 2.2.0
defects are documented, not patched.** No 2.2.1 will be cut. In exchange, the 3.0.0 release notes owe two
things:

> **A fourth defect joined the list on 2026-08-19, and the owner did not name it.**
> [Entry 14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) was
> filed 2026-08-18 and triaged 2026-08-19. D12 was taken against **three** defects because three were known;
> this triage reads it as governing *the shipped defects* rather than *those three*, and adds the fourth row
> below on that reading. **The reading is stated so it can be corrected** — nothing about the fourth defect
> distinguishes it from D12's own reasoning (silent, data-corrupting, near-empty consumer base), but the owner
> is entitled to say the scope was literal.

1. **All four defects appear as `Fixed` in `ProphetsWay.EFTools`' 3.0.0 `CHANGELOG.md` entry** — not
   `Changed`. A reader who sees only "soft-delete DAO bases rewritten" or "implements the new disposal
   contract" does not learn that the version they are running leaks connections, loses transactions, can
   resurrect a deleted row, and writes edits they never submitted.

   | Entry | What must be named as `Fixed` |
   |---|---|
   | [3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | `BaseEFDataAccess` never disposed the `DbContext` it constructed — a leaked context and connection per Data Access Layer instance |
   | [12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | `RootNonIdDao`'s commit and rollback **silently no-op** when a transaction was already open |
   | [13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | `RootBaseSoftDao.Update` **wipes the stored `DeletedDate`**, silently un-deleting a soft-deleted row; a second `Delete` refreshes the timestamp and returns `1` rather than `0` |
   | [14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) | **Added 2026-08-19.** `Insert` left the caller's own instance tracked, and `Get` returned the store's tracked instance under any context not explicitly set to `NoTracking` — so **an edit the caller never submitted was written on the next `SaveChanges`**. `Update` on an absent row threw rather than returning `0`. **This one changes behaviour a consumer may be relying on**, deliberately or by accident: after 3.0.0 those stray edits stop persisting, and a `catch` around `Update` stops firing |

2. **The release notes carry a known-issues note for 2.2.0** naming all four and recommending 3.0.0 — **with
   the target-framework caveat stated plainly**: [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes
   3.x **`net10.0`-only**, so a consumer on `net48`, `net8.0` or `net9.0` **cannot take 3.0.0 at all**.
   "Upgrade to 3.0.0" is not a remedy available to every 2.2.x consumer, and the note must not imply that it
   is.

**Three of the four fail silently** — no exception, just wrong data. That is why this is an obligation rather
than a formality: a consumer cannot discover any of them from a log, so the release note is the only
channel that reaches them. **Entry 14 is the only one of the four whose fix is itself a behaviour change a
consumer may be relying on**, so it owes a second sentence the other three do not: what stops happening.

**The premise this rests on is that the 2.2.x consumer base is near-empty**, and it is recorded so it can be
re-tested rather than assumed. If it ever turns out someone was on 2.2.x with a `net4x` or `net8.0` target,
D12 was taken without them in mind.

## Release Eligibility — the next release

The next release of this package is **v3.0.0**, a major. That is not a preference; it is forced. Entry 2
alone changes the transitive contract this package advertises, and entries 3, 4, 5 and 7 are each
independently breaking. ~~`app-variables.yml` currently reads `Major: '2' / Minor: '2' / Patch: '0'`~~ —
**corrected 2026-08-23: it reads `Major: '3' / Minor: '0' / Patch: '0'`, verified by opening it. The owner has
taken the bump; do not restate `2 / 2 / 0`.** **An agent must never change it** either way.

> **Re-read 2026-08-23 — the table below is now history for every row except 11 and 12.** Eleven of its
> fourteen rows describe work that has since landed, and the statuses in the [Index](#index) supersede this
> column. It is kept as the record of how the release was scoped rather than as a current view. **The final
> paragraph of this section is the part still worth reading**: this remains one indivisible release.
>
> 🔴 **Amended 2026-08-24 — two rows are missing from the table below and both block the tag.** Entry **11**
> (local SQL Server, [D20](purpose-and-scope.md#owner-decisions--2026-08-15)) is **Gate 1** and entry **19**
> (Azure SQL, [D22](purpose-and-scope.md#owner-decisions--2026-08-15)) is **Gate 2**. **Neither is satisfied,
> and 3.0.0 is not eligible for a tag or a publish until both are.** The table's row 11 predates the
> reclassification and reads as ordinary scheduled work; the [Index](#index) is the current view.

| # | Status | Eligible for v3.0.0? | Why |
| --- | --- | --- | --- |
| 1 | Scheduled | **Yes — and it is the gate; steps 1 and 3 of it have landed** | Nothing else can be verified until the proving ground compiles against the 3.x contracts. **It now does** — the 2026-08-16 build is green — but step 2 was satisfied by throwing stubs rather than an implementation, so the gate is passed for *compilation* and not for *conformance* |
| 2 | **Done** | **Landed 2026-08-16** | Both projects reference 3.1.0 and the solution compiles against it. This is what makes the release a 3.x |
| 3 | Scheduled | **Yes — rescoped** | `Dispose` landed 2026-08-16; what remains is the `ObjectDisposedException` guarding on every other member, which the parent's dispatcher cannot supply |
| 4 | Scheduled | **Yes — approved (D1)** | Only a major may drop targets, and this is the only major on the horizon |
| 5 | Scheduled | **Yes — strictly after 4** | The `#if` conditions go with 4; the destination is **`net10.0` alone**, a ratified exception to the house standard — see **D7** |
| 6 | Scheduled | **Yes — forced by 1** | The upstream base class it derives from no longer exists in that shape. **Rescoped 2026-08-16:** the deliverable is a suite that constructs the Entity Framework Data Access Layer itself, not a rebuilt set of adapters. **Resolved to shape B the same day** — so the deletion of the six adapters is in this release, while the *completion* of the entry additionally needs [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation), whose design is deferred until Lap 1 |
| 7 | Scheduled | **Yes — approved (D2), and only in a major** | Removing a transitive package reference is breaking. Postponing costs a second major |
| 8 | **Done** | **Landed 2026-08-16** | Trivial, isolated to a non-packaged project, and it did not wait for the rest — exactly as "eligible to land ahead" anticipated. **It was a licence item rather than hygiene**, and that exposure is now closed |
| 9 | Scheduled | **Yes** | Trivial, no build impact |
| 10 | Scheduled | **Yes — approved (D3)** | A breaking surface change is cheapest riding a major that is already breaking for four other reasons |
| 11 | Scheduled (test work) / Deferred (pipeline) | **Yes for the suite; the `LocalTestsOnly` removal is separately owned** | The contract cannot be *verified* without it; the CI plumbing is not this repository's decision alone |
| 12 | Scheduled (release note) / Rejected (2.2.x patch) | **Already, incidentally** | Nothing to schedule in code: the 3.x design removes the members that carry the defect. What **is** scheduled is the release-note obligation — the entry exists so the fix is *named* in the notes rather than shipping as an unannounced side effect. Triaged 2026-08-16 |
| 13 | Scheduled | **Yes — it is a precondition of 10, not a sibling of it** | The 3.x DAO families cannot be a re-wrap of `RootBaseSoftDao`; its `Update` and `Delete` semantics violate four of `IDepartmentDao`'s rules today. Filed 2026-08-16 |
| 14 | Scheduled | **Yes — and it cannot be deferred out** | Entry 10 deletes the types carrying the defect, so deferring it means writing six generic families **with** the adoption behaviour and breaking them again in 3.1.0. Triaged 2026-08-19. It is entry 13's counterpart on the **ordinary CRUD** path, and between them they specify every base in the package |

**The honest answer is that this is one indivisible release.** Entries 1–3 and 6 cannot be separated
without leaving the repository in a non-compiling state, and 4, 5, 7 and 10 are each cheap *now* and
expensive later because each needs a major version to land in. Splitting them across releases would buy two
more majors for no benefit.

**Nothing here is eligible for a patch or minor on the 2.2.x line.** That line's continuing job is to be
the EF6 answer, and it should receive no new work — see [entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework).

---

## 1 — Advance the `ProphetsWay.Example` submodule onto the 3.x contracts

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** All six steps of [The work](#the-work) are on disk.
> Verified by opening, on this date: `.git/modules/ProphetsWay.Example/HEAD`, which holds
> **`f93f0a41a76834647962ddf9e830e01e24e05f24`** — this repository's submodule now points at
> `ProphetsWay.Example`'s newest commit, *"Let an implementation declare what its store can structurally
> do"*, not at `d845863` and not at `61d9e7d`, **both of which are superseded and must not be restated**;
> `ProphetsWay.Example.DataAccess.EF/ExampleContext.cs`, which now declares **seven** `DbSet<>` properties
> including `Departments` and `CompanyResources`, calls `ToTable` for all seven, and carries
> `HasKey(x => new { x.CompanyId, x.ResourceId })` — the composite mapping step 2 argued was the only
> viable one; `ProphetsWay.Example.DataAccess.EF/Daos/`, which holds `DepartmentDao.cs` and
> `CompanyResourceDao.cs` alongside the original five; and `ProphetsWay.EFTools/BaseEFDataAccess.cs`, whose
> `Dispose` is `sealed override` and whose ten other members each open with `ThrowIfDisposed()`.
> **The eleven `NotWrittenYet` throwing stubs this entry was left waiting on are gone.**
>
> **One thing this closure does *not* cover, and it is release mechanics rather than this entry.** D11 step 3
> advances the pointer onto a **tag** of `ProphetsWay.Example`. The pointer is on an **untagged** commit, which
> is exactly the interim advance the clarification above permits. That tag move is still owed and belongs to
> the release sequence, not here.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Previously `Proposed`. Routed here from
[ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md), where it is recorded as the
highest-consequence open item in that repository. **The work is entirely in this one.**

**Partially landed — 2026-08-16.** Steps 1 and 3 of six are done, step 2 is done only to the extent of making
the compiler happy, and the entry stays `Scheduled` because the rest are not. It is deliberately **not**
`Done`: **the tree now compiles — `dotnet build ProphetsWay.EFTools.sln -c Debug` was green on this date — and
compiling is not conforming.** The sentence this paragraph replaced said the pointer move "costs the
repository its build"; that was true when written and is now history. Re-verified this date by opening
`.git/modules/ProphetsWay.Example/HEAD`, `ProphetsWay.EFTools.Tests/` (which now holds only `Constants.cs`
and its `.csproj`), `ProphetsWay.EFTools.Tests.csproj`, `ProphetsWay.Example.DataAccess.EF.csproj`,
`ProphetsWay.Example.DataAccess.EF/ExampleContext.cs`, `ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs`
and `ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`.

### The situation, verified rather than inherited

[.gitmodules](../.gitmodules) declares `path = ProphetsWay.Example`, `url = …/ProphetsWay.Example.git`,
`branch = main`. It is a **submodule, not a vendored copy** — `AGENTS.md` in this repository says otherwise
and is wrong. The two cannot drift; the pointer is simply **pinned**.

> **Correction, 2026-08-16 — the sentence above used to read "pinned pre-3.0.0", and that is now false.**
> The pointer was advanced to **`d845863`** — verified by reading
> `.git/modules/ProphetsWay.Example/HEAD`, which holds `d84586335a11d7c9efb7277b947015df0c15967e`, the tip
> of `ProphetsWay.Example`'s `main` and therefore its **3.1.0** tree. **Step 1 of [The work](#the-work)
> below has landed and steps 2–6 have not**, which is the whole of this repository's current build break.

> **Updated 2026-08-20 — the pointer moved again and `d845863` is no longer current.** It is now
> **`61d9e7d`** (`61d9e7dfb209c4a92b0c16d058aad1af08031fb5`), re-read from
> `.git/modules/ProphetsWay.Example/HEAD`. That is **one commit past the `3.1.0` tag** — `git submodule
> status` renders it `3.1.0-1-g61d9e7d` — being the 2026-08-18 merge of `ProphetsWay.Example` PR #21
> (`3.1.1-eftool-findings`), which opened the untagged **3.1.1** line and added `TestDataAccessFactory.Use`.
> **Do not restate `d845863`, and do not describe the pointer as sitting on a tagged release.** The
> paragraph above is left intact as the record of the first advance; only the SHA is superseded.
> The table immediately following describes the **pre-advance** pointer and is retained as the record of
> what the advance brought in; read its left-hand column as history, not as the checked-out tree.

The checked-out submodule was compared against the standalone repository directly. **This table describes
the pointer as it stood before 2026-08-16**; every "absent" in the left column is now present on disk.

| | Pinned copy under `ProphetsWay.Example/` (**pre-advance**) | Current `ProphetsWay.Example` |
|---|---|---|
| `ProphetsWay.Example.Tests/TestDataAccessFactory.cs` | **absent** | present — the single construction site |
| `ProphetsWay.Example.Tests/ConventionShowcase/` | **absent** | present |
| Disposal / transaction / snapshot test files | **absent** | `DataAccessDisposalTests`, `DataAccessTransactionTests`, `SnapshotDeepCopyTests` |
| `Department`, `CompanyResource` | **absent** | present, with 19 and 10 numbered contract rules |
| `BaseUnitTests<T>` | `protected abstract T GetIExampleDataAccess { get; }` | `TestDataAccessFactory.CreateAs<T>()`, and the class is `IDisposable` |
| `docs/` | **absent** | three documents |

That last row is the one with teeth. `ProphetsWay.EFTools.Tests` used to supply the implementation by
**overriding an abstract property**: six adapter classes — `EFBaseDataAccessTests`, `EFCompanyDaoTests`,
`EFJobDaoTests`, `EFResourceDaoTests`, `EFTransactionDaoTests` and `EFUserDaoTests` — each declaring
`protected override … GetIExampleDataAccess => Constants.GetExampleDataAccess;` against
[Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs) and containing no test logic. Upstream, that hook
was replaced by a static factory. **Advancing the pointer broke this repository's test project
structurally, not just semantically** — every test class here lost the member it overrode. That is entry 6,
and it is not optional. **All six files were deleted on 2026-08-16**, so they are named here rather than
linked; the deletion is recorded in
[entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits).

### The build break the advance created — two of three now closed

Recorded here rather than left to be rediscovered, because a future agent meeting a red build needs to know
it is this entry's mid-flight state and not a regression. Three independent breaks were found on
2026-08-16; **one remains at the end of that day.**

1. ~~**TFM mismatch.**~~ **Closed 2026-08-16.**
   [ProphetsWay.EFTools.Tests.csproj](../ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj)
   targeted `net472;net48;net80;net90` against a `ProphetsWay.Example.Tests` the 3.1.0 pointer had
   retargeted to `net48;net10.0`, leaving three of four legs with no compatible asset. All three EFTools
   projects are now `net10.0` alone, which binds. Delivered by
   [entry 5](#5--retarget-to-the-house-tfm-standard).
2. ~~**The missing hook**, above.~~ **Closed 2026-08-16** by deleting the six adapters —
   [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits). The project now
   contains no tests, which is the intended temporary state under **D10**, not a loss: the 35 upstream
   facts are parked awaiting the seam.
3. **`ExampleDataAccess` no longer satisfies `IExampleDataAccess`. — CLOSED 2026-08-16, and read the closure
   carefully.**
   At 3.1.0 that interface aggregates `IDepartmentDao` and `ICompanyResourceDao` and inherits `IDisposable`
   — verified by opening
   `ProphetsWay.Example/ProphetsWay.Example.DataAccess/IExampleDataAccess.cs`, whose declaration reads
   `: IBaseDataAccess, ICompanyDao, IJobDao, IUserDao, ITransactionDao, IResourceDao, IDepartmentDao, ICompanyResourceDao`.
   **`ExampleDataAccess.cs` now supplies all three**, but it supplies the two new DAO groups as **11 members
   that throw `NotImplementedException`** — verified by searching the project for `NotWrittenYet`, which
   matches 12 times in that one file: eight `IDepartmentDao` members, three `ICompanyResourceDao` members,
   and the helper itself. `Dispose` is inherited from `BaseEFDataAccess`. **The break is closed; step 2 below
   is not.**

### The work

0. **Nothing local blocks the pointer any more.** The submodule working tree carried an obsolete local
   modification against the pinned pre-3.0.0 commit; the owner **approved discarding it, and it has been
   discarded** — [D9](purpose-and-scope.md#owner-decisions--2026-08-15). The tree is clean, so the advance
   is a pointer move rather than a merge. Recorded because "the submodule had uncommitted changes" is
   exactly the kind of finding a later pass would otherwise re-report as an obstacle. **This step is now
   history rather than a precondition — see the note under [D9](purpose-and-scope.md#owner-decisions--2026-08-15).**
1. ~~Advance the submodule pointer to the published 3.1.0 commit.~~ **Done, 2026-08-16**, and advanced
   again on 2026-08-18 — the pointer is at **`61d9e7d`**, one commit past the `3.1.0` tag, on the open and
   untagged `3.1.1` line. **Earlier text here naming `d845863` is superseded.** **This is the only step of
   this entry that has landed**, and steps 2–6 not landing with it is
   what leaves the repository non-compiling.
2. Add `Department` and `CompanyResource` — entities, `I*Dao` implementations, EF mappings in
   `ExampleContext`, and the schema they need. **No longer a build break, and no longer close to done:** the
   11 throwing stubs satisfy the compiler and nothing else.

   **The mapping half has not been started at all, and it is sharper than it looks — verified 2026-08-16 by
   opening [ExampleContext.cs](../ProphetsWay.Example.DataAccess.EF/ExampleContext.cs).** That file declares
   five `DbSet<>` properties — `Company`, `User`, `Resource`, `Transaction`, `Job` — and `OnModelCreating`
   calls `ToTable` for the same five. **There is no `DbSet<Department>`, no `DbSet<CompanyResource>`, and no
   mapping for either.**

   Two consequences worth having written down before anyone starts:

   - **Model building will fail at runtime the moment anything materializes the model.** Nothing does yet,
     because every member that would touch those two entities throws first. The throwing stubs are therefore
     *hiding* this, not fixing it, and the first real implementation of any one of the 11 members surfaces it.
   - **`CompanyResource` is keyless, and EF Core forces a choice the contract has already made.** Per
     `ICompanyResourceDao` rule 1 a row is identified by the `CompanyId`/`ResourceId` pair. EF Core needs
     either an explicit composite `HasKey(x => new { x.CompanyId, x.ResourceId })` or `HasNoKey()` — and
     **`HasNoKey()` makes the entity read-only, which forbids the `Insert` and `Delete` that rules 3 and 4
     require.** The composite key is therefore the only viable mapping. "Keyless" in this contract means *no
     surrogate identifier property on the entity*, not *no primary key in the store*; the database project's
     `CompanyResources.sql` already carries the composite primary key. Do not read `IBaseEntity` as an
     instruction to reach for `HasNoKey`.

   **Owner decision [D13](purpose-and-scope.md#owner-decisions--2026-08-15), 2026-08-16 — the `Department`
   half of this step is approved as a hand-written concrete `DepartmentDao`.** Explicitly *not* by deriving
   from today's soft base, and explicitly *not* by waiting for
   [entry 10](#10--collapse-the-guidintlong-dao-triplication)'s generic families. The owner's reasoning:
   *"the point of the generic tests and classes was to just reduce all the duplicative code, but in this case
   there is a real need for it."* The Data Access Object written here becomes the **evidence** for what the
   generic soft-delete family must look like — see
   [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance), whose
   recommendation this ratifies, and [entry 10](#10--collapse-the-guidintlong-dao-triplication), which it
   constrains.

   **Verified 2026-08-16:** `ProphetsWay.Example.DataAccess.EF/Daos/` holds `CompanyDao.cs`, `JobDao.cs`,
   `ResourceDao.cs`, `TransactionDao.cs` and `UserDao.cs` — **no `DepartmentDao.cs`, no
   `CompanyResourceDao.cs`.** D13 approves work that has not started.
3. Implement `Dispose` and the three transaction members against the real `DbContext` — which is
   [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) in the library and its
   consequence here. **Landed 2026-08-16** at the library level: `BaseEFDataAccess` now carries `Dispose`,
   and the three transaction members were already forwarders. What entry 3 still owes is the
   `ObjectDisposedException` guarding, not the disposal itself.
4. Satisfy the **snapshot rule** — reads return deep snapshots, writes read their argument. EF Core's
   change tracker makes this the interesting one: the existing examples already set
   `QueryTrackingBehavior.NoTracking`, which is a start and not a proof.
5. Satisfy the **ordering rule** — an explicit `ORDER BY` on both `GetAll` and `GetPaged`.
   `RootDao.GetPaged` already orders by `Id`; **`RootDao.GetAll` does not** — it is
   `Dataset.ToList()`. [repo-profile.md](repo-profile.md) adds a third case this entry had missed:
   **soft-delete paging in `RootBaseSoftDao` is not explicitly ordered either.** All three are divergences
   the rule was written to catch, and all three are in the library, not the example. **`RootBaseSoftDao` has
   since been found to violate four of `IDepartmentDao`'s rules besides this one — see
   [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) before
   implementing the `Department` half of step 2.**
6. Run `dotnet test --filter "Scope=Contract"` and report the result.

### Why this is the gate

Every other entry is verified by this test suite. Until it runs, entries 2, 3, 4, 5 and 7 are changes
whose correctness is asserted rather than demonstrated. The repository's own `LocalTestsOnly: 'yes'` means
CI will not catch a mistake either.

### Open question — **answered**

> Should the EF implementation move to the `Microsoft.EntityFrameworkCore.Sqlite` in-memory mode, or a
> Testcontainers SQL Server, so that `LocalTestsOnly: 'yes'` can be lifted and CI actually runs the
> contract suite?

**Both**, per owner decision [D4](purpose-and-scope.md#owner-decisions--2026-08-15): SQLite in-memory as
the fast CI contract/query gate, and a SQL Server container for provider fidelity. The reasoning that made
this urgent stands — the `InMemory` provider cannot honour transactions, so the transaction-scope contract
tests **cannot pass against it**, which means the arrangement in place today is structurally unable to
verify a contract this package must now meet.

This has been promoted out of a footnote into its own entry,
[11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container), because it is now
approved work with its own scope rather than a question downstream of
[entry 7](#7--stop-forcing-a-database-provider-on-every-consumer).

---

## 2 — Move the `ProphetsWay.BaseDataAccess` reference from 2.5.0 to 3.1.0

**Status:** **Done — 2026-08-16.** Previously `Scheduled for v3.0.0` (2026-08-15, owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15)), and `Proposed` before that. A one-line edit with the
largest consequence in the file.

**Why `Done` rather than `Scheduled` until v3.0.0 ships.** The entry's deliverable is a reference version, and
the reference is at 3.1.0 in both projects. What made this triageable today rather than a week ago is that the
solution now **compiles against 3.1.0** — a version bump that does not build is a claim, not a change, and the
verified-green build of 2026-08-16 is the evidence this entry was waiting on. The release it rides in is a
separate fact, recorded in [Release Eligibility](#release-eligibility--the-next-release); an entry is not held
open for the act of shipping.

**Landed — 2026-08-16.**
[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) and
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
**now both reference `ProphetsWay.BaseDataAccess` 3.1.0** — re-verified by opening both files on 2026-08-16.
The sentence this paragraph replaced said they were on **2.5.0**, which was true when written.

**This is not a version-hygiene item.** A library whose stated purpose is "implements the
`ProphetsWay.BaseDataAccess` contracts" and which references a superseded major is not implementing the
contracts it advertises. The README's paradigm claim was, until this landed, a statement about 2.x — and it
remains one **for the published package**, which is still 2.2.0 carrying 2.5.0. Only the tree has moved.

What 3.0.0 changed that lands directly on this package:

- `IBaseDataAccess` now extends `IDisposable`
  ([IBaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/IBaseDataAccess.cs) line 164)
  and `BaseDataAccess` declares `public abstract void Dispose();`
  ([BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs) line 112).
  [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) overrode the three transaction members
  and nothing else, so it would not have compiled. **`Dispose` has since been implemented there — verified
  2026-08-16 by opening the file** — which is [entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess).
- Exceptions from derived DAL methods now propagate **unwrapped** — no `TargetInvocationException`. Any
  consumer catching the wrapper breaks.
- The identifier property must be **public**; an explicit interface implementation now throws
  `DataAccessConventionException` before dispatch.

**Breaking for consumers**, transitively: a `ProphetsWay.EFTools` 3.0.0 consumer is resolving
`ProphetsWay.BaseDataAccess` 3.1.0 whether they asked for it or not, and their own DAL must now supply
`Dispose`. This is the single strongest reason the next release must be a major.

**Do not split this from entry 3.** Landing it alone leaves the repository non-compiling. **In the event it
was not split** — both landed on 2026-08-16 and the tree compiles, which is the outcome this warning existed
to obtain. Retained as the reason, not as an outstanding instruction.

---

## 3 — Implement the 3.x disposal contract in `BaseEFDataAccess`

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** The rescoped remainder — `ObjectDisposedException`
> guarding on every member other than `Dispose` — has landed. Verified by opening
> `ProphetsWay.EFTools/BaseEFDataAccess.cs` on this date and reading every member declaration in it:
> `protected void ThrowIfDisposed()` is declared at line 104, `public sealed override void Dispose()` at 286,
> `protected virtual void DisposeCore()` at 327, and **all ten** non-`Dispose` members — `TransactionStart`,
> `TransactionCommit`, `TransactionRollBack`, `GetAll`, `GetPaged`, `GetCount`, `Get`, `Insert`, `Update`,
> `Delete` — open with a `ThrowIfDisposed()` call on their first statement line. `ThrowIfDisposed` is
> `protected` rather than `private`, so a derived Data Access Layer can guard its own forwarders, which is what
> `ProphetsWay.Example.DataAccess.EF` does.
>
> **The [D12 release-note obligation for this defect is discharged](#the-changelog-author-obligation--d12) —
> corrected 2026-08-23, later the same day.** This paragraph read *"is **not** discharged by this closure, and
> is now the single largest open item in this repository."* **That is false now and must not be restated.**
> `CHANGELOG.md` § *"Fixed: a Data Access Layer no longer leaks its context and its connection"* names the
> leak, quotes the 2.2.0 `Activator.CreateInstance` line, states that nothing ever disposed it and that a
> `using` around a Data Access Layer instance **could not compile**, and identifies this as the one of the four
> that eventually announces itself. **Nothing in this repository is open on account of D12.**

**Status:** ~~**Scheduled for v3.0.0, and rescoped 2026-08-16.**~~ **Done — 2026-08-23.** Scheduled 2026-08-15 by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15); previously `Proposed`.

### The rescope — 2026-08-16, and read this before reading the rest of the entry

**Half of this entry has landed and the other half has not, and the halves are not the ones the entry was
written around.** Verified by opening
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) on this date.

| Deliverable | State |
|---|---|
| `Dispose` exists, is idempotent, never throws, rolls back an open transaction, disposes the context it created | **Landed** — `public override void Dispose()` reads `_disposed`, returns early, rolls back `Context.Database.CurrentTransaction` inside a swallowing `try`/`catch`, then disposes the context in a second one |
| **Every member other than `Dispose` throws `ObjectDisposedException` once disposed** | **Outstanding — this is now the whole of the entry** |
| Ownership flag distinguishing a created context from an injected one | **Superseded** by `docs/api-contract.md`'s `ContextOwnership` enum (A9). Not this entry's to re-decide |

**The remaining half is not a detail, and it is not covered anywhere else.** `IBaseDataAccess` requires that
*every* member other than `Dispose` throw `ObjectDisposedException` once the instance is disposed.
`_disposed` is **set and never read outside `Dispose` itself** — so a disposed `BaseEFDataAccess` currently
fails the contract on **seven** members, not one:

- the three transaction members overridden here — `TransactionStart`, `TransactionCommit`,
  `TransactionRollBack` — which reach a disposed `Context.Database` and surface whatever EF Core throws;
- the four concrete dispatcher members inherited from `BaseDataAccess` — `GetAll<T>`, `GetPaged<T>`,
  `GetCount<T>` and `Get<T>` — verified by opening
  [BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs), where
  each is `public virtual` and reflects onto the derived class with no disposal state to consult.

**The parent cannot supply this guard and it is not an oversight there.** `BaseDataAccess` holds no
connection, context or disposal state — its own `<summary>` on `Dispose` says exactly that, which is why the
member is abstract. The guard has to live here. That answers open question 2 below on the evidence rather
than leaving it open, and the design question it asks — *guard here or in the parent* — has only one
answer available.

**A green build says nothing about this.** The 2026-08-16 build compiles; nothing in this repository executes
a disposed instance, because there is no test suite (entry 6). The gap is invisible until either the upstream
suite runs against this Data Access Layer or a consumer meets it.

**Q2 is superseded, not open** — see the note at the foot of this entry. The entry's remaining scope is one
sentence: **guard the seven members.**

### The mechanical part

`BaseDataAccess` declares `Dispose` abstract, so `BaseEFDataAccess` must supply it. The parent's contract —
the source of truth is the `<remarks>` on `IBaseDataAccess`, not this paragraph — requires that `Dispose`
be **idempotent**, **never throw**, **roll back an open transaction**, cause every *other* member to throw
`ObjectDisposedException` thereafter, and dispose **what the instance created and not what was handed to
it**.

### The part that is not mechanical

[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) constructs its own context:

```csharp
Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { connectionString });
```

Under "disposes what it created," that context **must** be disposed. Both current constructors create it,
so today the answer is unambiguous.

**It stops being unambiguous the moment a DI-hosted consumer asks for an injected context**, which is the
normal way EF Core is used in ASP.NET Core and which this library currently cannot accommodate at all. A
`BaseEFDataAccess(DbContext context)` overload would receive a context whose lifetime the container owns,
and disposing it would be a bug — the same bug in reverse.

The contract already answers this correctly ("what it created, not what was handed to it"), so the design
work is to make the *code* able to tell the difference: a flag captured at construction, set by whichever
constructor ran. That is small. What is not small is deciding whether to add the injecting constructor at
all in v3.0.0 — it widens the purpose sentence from "constructs your context for you" toward "participates
in your composition root."

### Recommendation

Implement `Dispose` with an ownership flag **now**, defaulting to owned, even if the injecting constructor
is not added in this release. Retrofitting ownership tracking after consumers depend on the disposal
behaviour is the expensive order.

### Open questions for the owner

1. **Superseded (Q2).** Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one?
   Owner decision [D2](purpose-and-scope.md#owner-decisions--2026-08-15) made this *more* pressing, not
   less: now that the consumer configures the provider themselves through `DbContextOptions`, the distance
   between "you pass options" and "you pass the context" is one step, and a DI-hosted consumer will ask for
   that step. **`docs/api-contract.md` settles it as the `ContextOwnership` enum (A9)** — a required
   constructor argument with no default, plus a sealed `Dispose`. Read that document; do not re-decide it
   here.
2. **Answered, 2026-08-16 — and it is not a question, it is the entry's remaining work.** Should
   `ObjectDisposedException` guarding be added here, or is the parent's dispatcher expected to guard? **Here.**
   The parent holds no state and cannot — confirmed by opening `BaseDataAccess.cs`, whose `Dispose` is
   abstract for precisely that reason. The scope is **seven members**, not the three transaction members this
   line originally named: the four concrete dispatcher members inherited from `BaseDataAccess` are equally
   bound by the contract and equally unguarded. See [The rescope](#the-rescope--2026-08-16-and-read-this-before-reading-the-rest-of-the-entry).
### Appended 2026-08-15 — this is a shipped defect, not only a forward-compatibility gap

The framing above reads as *"3.1.0 will require a `Dispose` we do not have yet."* That understates it.
At the time this was written, [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) contained
**no `Dispose`, no `IDisposable`, and no finalizer** — its only members were `Context`, two constructors,
and three transaction forwarders — and both constructors built the context with `Activator.CreateInstance`.
**Nothing ever disposed it.** That is a live resource leak in the **published 2.2.0 package**, independent
of any 3.x contract, and that half of this note remains true for as long as 2.2.0 is the published version.

> **Factual correction — 2026-08-16.** The description of the *file* above is now history. Opening
> `BaseEFDataAccess.cs` on this date shows `public override void Dispose()` present: it sets a `_disposed`
> flag and returns early when already set, rolls back `Context.Database.CurrentTransaction` inside a
> `try`/`catch` that swallows the failure, then disposes the context in a second guarded `try`/`catch`, with
> `<remarks>` stating that the context is disposed because both constructors construct it. **The published
> package is unchanged; the working tree is not.** `_disposed` is set but **never read outside `Dispose`**, so
> the second open question below is not merely still open — it is now the entirety of what this entry owes.
> **Status rescoped rather than closed**, 2026-08-16: see
> [The rescope](#the-rescope--2026-08-16-and-read-this-before-reading-the-rest-of-the-entry).

Two consequences:

- **`Changelog Author` must record this as `Fixed`, not `Changed`.** A reader of the 3.0.0 notes who sees only
  "implements the new disposal contract" will not learn that the version they are running leaks connections.
- The design question this entry calls "not mechanical" is **answered** — `docs/api-contract.md` settles it as
  the `ContextOwnership` enum (A9), a required constructor argument with no default so the branch can never be
  inferred, plus a sealed `Dispose` so a derived DAL cannot omit it. Open question Q2 above is superseded by
  that document; read it rather than re-deciding here.

The defect was found by `Repo Analyst` during a verification pass and confirmed independently by
`Contract Reviewer`; both opened the file. Recorded so it is not rediscovered as new.

### Triaged 2026-08-16 — the 2.2.x patch for the leaked context is **Rejected**

**This entry had left the question implicit rather than open**, which is worse: it said the leak "remains true
for as long as 2.2.0 is the published version" and stopped there, so a later reader could reasonably have
inferred a patch was still on the table. It is not.

**Owner decision [D12](purpose-and-scope.md#owner-decisions--2026-08-15):** *"I don't believe anyone is
currently using that library in any meaningful capacity, so it's document the bug and recommend to update to
v3.0.0."* No 2.2.1 will be cut. This **upholds [D1](purpose-and-scope.md#owner-decisions--2026-08-15)** rather
than carving an exception from it.

**Re-verified before this status was written**, not affirmed:
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) was opened on 2026-08-16. Both constructors
still read `Context = (DbContext)Activator.CreateInstance(...)`, so the *published* 2.2.0 shape of the defect
is real and unchanged; the working tree's `Dispose` is what the rescope above records.

**What is owed instead:** the release-note obligation, stated once in
[The `Changelog Author` obligation](#the-changelog-author-obligation--d12) — including the caveat that
[D7](purpose-and-scope.md#owner-decisions--2026-08-15) puts 3.0.0 out of reach of a `net48`/`net8.0`/`net9.0`
consumer, so "upgrade" is not a remedy available to all of them.

**The status line is unchanged.** The entry remains `Scheduled` for v3.0.0 on its live deliverable — the
`ObjectDisposedException` guarding on seven members — which D12 does not touch.

---

## 4 — Make 3.x Entity Framework Core-only — retire EF6 and .NET Framework

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** Verified by opening
> `ProphetsWay.EFTools/ProphetsWay.EFTools.csproj`, which reads `<TargetFrameworks>net10.0</TargetFrameworks>`
> and references `Microsoft.EntityFrameworkCore` 10.0.11 and `ProphetsWay.BaseDataAccess` 3.2.0 and nothing
> else; and by grepping all **15** `.cs` files in `ProphetsWay.EFTools/` for `#if`, `#else`, `#elif`,
> `#endif`, `#pragma`, `UseSqlServer` and `UseInMemoryDatabase` — **zero matches, library-wide.** No EF6
> reference, no `System.Data.Entity`, no conditionally compiled code, and no `net4x` leg survives.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15. **Approved by the owner as
[D1](purpose-and-scope.md#owner-decisions--2026-08-15).** Previously `Proposed — awaiting the owner's scope
decision`; this was the checkpoint, and it has been passed. The recommendation below was accepted as
written, and the argument is preserved rather than trimmed — it will be questioned again.

### The recommendation

**Make `ProphetsWay.EFTools` 3.x EF Core-only. Leave the published 2.2.x as the EF6 / .NET Framework
answer, and give that line no new work.**

The full argument, with the file-level evidence, is in
[purpose-and-scope.md § The EF6 Question](purpose-and-scope.md#the-ef6-question--settled).
The short form, because the reasoning belongs where it can be checked:

- **The divergence is semantic.** [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) `Update` is
  `Dataset.AddOrUpdate(item)` on EF6 — an **upsert that inserts a missing row** — and
  `Single(…)` + `SetValues` + `EntityState.Modified` on EF Core, which **throws** on a missing row. One
  method name, two behaviours, one package ID.
- **The public surface differs by target.** The `DbContextOptions` constructors on `BaseEFContext` and
  `BaseEFDataAccess` exist **only** on modern targets — and the README recommends that pattern as
  preferred, to readers who may not have it.
- **The preprocessor is already unreadable in one place.** In
  [ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  an `#if NET471 || NET48` block opens a constructor brace that an `#if NET8_0_OR_GREATER` block closes.
  No single compilation shows a reader the whole file.
- **It blocks the retarget.** The conditions are hardcoded TFM lists — `#if NET461 || NET471 || NET48` and
  `#if NET8_0_OR_GREATER`. `netstandard2.0` matches **neither**, so every affected file would compile to
  missing usings and empty bodies. EF6 retention is not neutral with respect to
  [entry 5](#5--retarget-to-the-house-tfm-standard); it stands in front of it.
- **Nothing verifies the EF6 leg.** `LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml)
  means CI does not run these tests at all. The .NET Framework leg is maintained and never executed.
- **The migration cost is close to zero.** No known live consumer requires .NET Framework or EF6, the owner
  has accepted a major bump, and **2.2.x remains published and installable**. Nobody is stranded; they
  simply stop receiving new work.

### The strongest argument against, stated at full strength

The EF6 branch **works today**, and deleting it is materially harder to reverse than keeping it. Those
`#if` blocks encode real knowledge about EF6's transaction types and upsert behaviour that a future port
would have to rediscover from scratch. If a .NET Framework consumer appears next year, today's cheap answer
("it already builds") becomes an expensive one.

The counter is that the knowledge is **tagged, not deleted** — it lives in git history and in a published,
installable 2.2.x package. But the owner should weigh it rather than be told it is settled.

### Explicitly not recommended

- **Not** a `ProphetsWay.EFTools.EF6` companion package. A second repository, pipeline, version line,
  changelog and support surface for an audience currently measured at **zero** is the maintenance tax this
  workspace's conventions exist to avoid. **The owner rejected this explicitly in D1. It is closed.**
- **Not** deprecating or unlisting 2.2.x. It should remain discoverable as the EF6 answer, and receive no
  new work.

### What follows from the approval

- The `<Description>` and the purpose sentence say **"Entity Framework Core"**, not "EntityFramework".
- The README's `DbModelBuilder` example goes. **`README Author`'s file, not this agent's.**
- `CHANGELOG.md` carries a breaking-change entry pointing EF6 consumers at 2.2.x. **`Changelog Author`'s
  file.**
- ~~The `EntityFramework` 6.5.1 `PackageReference` and the `$(TargetFramework.StartsWith('net4'))` item group
  go.~~ **Done 2026-08-16 by `Modernizer`** — verified by opening all three `.csproj` files, none of which
  now contains an `EntityFramework` reference or a `net4`-conditional `ItemGroup`. **EF6 is unreferenced by
  this repository.**
- Roughly 60 `#if NET461 || NET471 || NET48` / `#if NET8_0_OR_GREATER` blocks across 26 files collapse to
  unconditional EF Core code, which is what unblocks [entry 5](#5--retarget-to-the-house-tfm-standard).
  **Not done** — the blocks are still in the C# sources, where they are now dead under a `net10.0`-only
  build. `Implementer`'s work.

### The consequence the approval created — `netstandard2.0` is unreachable, and that is now ratified

Not foreseen when this entry was written, and it is a **conflict with a family-wide convention**, so it is
recorded here rather than left to be rediscovered during the retarget. **It has since been settled** by
owner decision [D7](purpose-and-scope.md#owner-decisions--2026-08-15), which closed **Q1**.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) pins a single
`Microsoft.EntityFrameworkCore` version across the package. **The version is deliberately not quoted here** —
`Modernizer` was bumping it on 2026-08-16 while this correction pass ran, and a number written from a
mid-flight file is a claim nobody can stand behind. Read it from the csproj. What matters to this entry is
unaffected by the number: EF Core has shipped no `netstandard2.0` asset since 3.1 — 5.0
onward are runtime-targeted, and **EF Core 10 exposes only `net10.0`**. So an EF Core-only
`ProphetsWay.EFTools` **cannot carry the house standard's permanent reach floor**; a `netstandard2.0`
target would be one that cannot restore its own primary dependency.

This is not an argument against D1 — the EF6 branch was the only thing that could ever have served
`netstandard2.0` here, and it was serving it via `net4x` rather than the floor anyway. It is the reason
this repository now holds a **ratified exception**: **3.x targets `net10.0` only**, and existing
`net4x`/`net8.0`/`net9.0` consumers stay on 2.2.x. The exception still needs a line in `AGENTS.md`, which
is **not this agent's file**. The reasoning is in
[purpose-and-scope.md § The `net10.0`-Only Exception](purpose-and-scope.md#the-net100-only-exception--settled).

### If the owner had declined

Retained because reversal remains possible until the code lands. Entry 5 would have needed re-planning: the
`#if` conditions become feature-shaped rather than TFM-shaped (`#if EF6` / `#else`, driven by a
`DefineConstants` in the conditional `ItemGroup`). That was the only way `netstandard2.0` could have been
added without breaking both branches — and, per the paragraph above, it is now moot for a different reason.

---

## 5 — Retarget to the house TFM standard

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** All three projects read
> `<TargetFrameworks>net10.0</TargetFrameworks>` — the destination **D7** ratifies — verified by opening
> `ProphetsWay.EFTools/ProphetsWay.EFTools.csproj` and
> `ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj` on this date. **This is the ratified exception
> to the house standard, not the house standard**; do not later "correct" it toward `netstandard2.0;net10.0`.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15. **Unblocked**, not merely sequenced, by the approval of
[entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework). The destination was the
last open part of this entry; it is **settled by [D7](purpose-and-scope.md#owner-decisions--2026-08-15)**,
which closed **Q1**.

**The retarget landed on 2026-08-16.** The status line above is untouched — triage is `Purpose Refiner`'s —
but the table below now records history in its left column. All three EFTools-owned projects read
`<TargetFrameworks>net10.0</TargetFrameworks>`, verified by opening each `.csproj`.

| Project | TFMs before 2026-08-16 | House standard | **Approved destination (D7)** | On disk now |
|---|---|---|---|---|
| `ProphetsWay.EFTools` | `net461;net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** | **`net10.0`** |
| `ProphetsWay.EFTools.Tests` | `net472;net48;net80;net90` | `net48;net10.0` | **`net10.0`** — the `net48` leg has nothing to bind | **`net10.0`** |
| `ProphetsWay.Example.DataAccess.EF` | `net471;net48;net80;net90` | `netstandard2.0;net10.0` | **`net10.0`** | **`net10.0`** |

Four separate problems, worth separating because they have different fixes — **all four are about the
left-hand column, and the retarget addressed them together**:

1. **`net461` and `net471` are end of life**, and `net472` appears only in the test project.
2. **`net80`/`net90` are non-canonical monikers** — the dotted form `net10.0` is the convention.
   Undotted parses; it is inconsistent across the family and reads as a typo.
3. **`netstandard2.0` is absent**, and after [D1](purpose-and-scope.md#owner-decisions--2026-08-15) it
   **cannot be added** — see below. What was recorded here as a gap is a permanent, deliberate and now
   **ratified** exception.
4. **.NET 8 and .NET 9 both reach end of life on 10 November 2026.** Retargeting to `net90` would be
   retargeting onto debt.

**The former blocker is gone.** Adding `netstandard2.0` while the `#if NET461 || NET471 || NET48` /
`#if NET8_0_OR_GREATER` pairs remained would have produced files with **no `using` directives and empty
class bodies**. [Entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)'s approval
deletes roughly 60 conditional blocks rather than auditing them.

**The destination is not the house standard, and that is now a decision rather than a question.**
No `Microsoft.EntityFrameworkCore` version this package could reference ships a `netstandard2.0` asset — EF
Core has been runtime-targeted since 5.0, and **EF Core 10 exposes only `net10.0`** — so an EF Core-only
library cannot carry the family's
reach floor at all. **[D7](purpose-and-scope.md#owner-decisions--2026-08-15) approves `net10.0` alone**, for
the library, the test project and `ProphetsWay.Example.DataAccess.EF` alike, and keeps existing
`net4x`/`net8.0`/`net9.0` consumers on the published 2.2.x line.

**Why not `net10.0` plus one LTS predecessor**, which this entry previously left as an option: an EF Core 10
dependency has no asset for an earlier runtime, so a second TFM would have to pin an older EF Core on that
leg — reintroducing the two-products-under-one-package-ID problem
[entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) exists to end.

The justification for the exception is that the reach floor buys nothing here: `netstandard2.0` exists to
make a **portable contract** consumable from .NET Framework upward, and this package's public surface is
generic base classes over `DbContext` — an **EF Core implementation, not a portable contract**. No consumer
the floor would reach could use what they found there. That exception belongs in `AGENTS.md`, which
**this agent may not write**.

The test project's `net48` leg goes with it for the same reason. In
`ProphetsWay.BaseDataAccess` and `ProphetsWay.Example` that leg exists to bind the `netstandard2.0` asset
and verify .NET Framework behaviour; here there will be no such asset to bind.

**Adding a TFM is a MINOR bump and removing one is MAJOR**, so this belongs in v3.0.0 or waits for v4.

**Not this agent's edit.** `Modernizer` owns csproj changes; this entry is the scope justification for them.

---

## 6 — Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory and `Scope` traits

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** Shape B is built, and the upstream dependency it was
> waiting on — `ProphetsWay.Example` FR 13 — is satisfied by the pointer this repository now carries. Verified
> by listing `ProphetsWay.EFTools.Tests/` and opening `TestSeam.cs` on this date. The project holds **25**
> source files: `TestSeam.cs`, whose `[ModuleInitializer]` calls
> `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess, StoreCapabilities.TransactionIsolation)`;
> **13** one-line adapters; **2** seam guards (`TestSeamTests.cs`, `AdapterCoverageTests.cs`); **8** classes
> written directly against this library's own surface; and `Constants.cs`.
>
> **The seam is no longer "landed and unverified"** — it is the mechanism the whole adapted upstream suite runs
> through, and `TestSeamTests` asserts by full type name that it is still pointing at
> `ProphetsWay.Example.DataAccess.EF` rather than the identically named NoDB type.
>
> **The second argument is new as of 2026-08-23** and is not this entry's original scope — see
> [ProphetsWay.Example FR 17](../../ProphetsWay.Example/docs/feature-requests.md), filed on that date for the
> mechanism itself.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15. Forced by
[entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts), not chosen.
**Rescoped 2026-08-16 — see [The rescope](#the-rescope--2026-08-16) below — and the fork that rescope opened
was resolved the same day; see [The resolution](#the-resolution--2026-08-16-shape-b-the-direction-only).**
The status does not change; the deliverable does. **The status was re-examined against the resolution and
deliberately left at `Scheduled`:** the deletion of the six adapters is real work in this release, and the
entry was already `Scheduled` before the fork existed. Choosing between two shapes never blocked the status,
only the estimate.

`ProphetsWay.EFTools.Tests` derived from the upstream test classes and supplied the implementation by
overriding `protected abstract T GetIExampleDataAccess { get; }` — six files, all following the same four-
to five-line shape. Upstream, that hook no longer exists: `BaseUnitTests<T>` now calls
`TestDataAccessFactory.CreateAs<T>()` and implements `IDisposable`.

**Advancing the pointer therefore broke every test class in this project at compile time.** The rebuild
is not cleanup; it is the cost of entry 1. **As of 2026-08-16 that is past tense** — the pointer has
advanced, and **the six adapters have since been deleted**; see
[What has landed](#what-has-landed--2026-08-16) below.

### The rescope — 2026-08-16

This entry was written as *"rebuild the adapters against the new base classes."* **That is no longer the
shape of the work, and the difference is not cosmetic.** Verified by opening
`ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs` in the standalone repository:

```csharp
public static IExampleDataAccess Create()
{
    //>>> The one line to change to point this suite at another implementation. <<<
    return new ExampleDataAccess();
}
```

`CreateAs<T>()` calls `Create()` and casts. **There is no hook to override, in either direction.** The
upstream suite has exactly one construction site, it is `static`, it names the **NoDB** implementation, and
it takes no parameter. So:

> **Superseded 2026-08-16 as a description of the current file.** The seam landed upstream that day and
> `Create()` no longer contains the construction — see
> [The seam landed upstream](#the-seam-landed-upstream--2026-08-16-and-nothing-has-been-run-through-it).
> The paragraph and snippet above are the state that **caused** this rescope, and are preserved as its
> reasoning rather than as a report of the tree.

- **The adapter concept is gone, not broken.** Six classes whose only content was an override have nothing
  left to override. They are to be **deleted**, not rewritten. Anything preserved from them is
  `Constants.cs`'s connection handling, and even that is provider wiring rather than a test hook.
- **An Entity Framework conformance run is a factory swap, not inheritance** — and the factory is not in
  this repository. It is in `ProphetsWay.Example`, consumed here as a **pinned submodule**, and
  [ProphetsWay.Example FR 5's](../../ProphetsWay.Example/docs/feature-requests.md) standing instruction is
  that files under `ProphetsWay.Example/` are never edited from this side.
- **Therefore this entry, as previously scoped, cannot be completed from within this repository.** That is
  the finding, and it is new. Inheriting the upstream classes and swapping the subject underneath them is
  not available, because the subject is chosen by a `static` method that neither accepts an argument nor
  consults one.

**What the deliverable becomes.** One of two shapes — **and the choice has been made; the table is kept
because the rejected half is the reasoning:**

| Shape | What it means here |
|---|---|
| ~~**A — a local suite**~~ | ~~This repository writes its own `Scope`-traited tests against `IExampleDataAccess`, constructing `ProphetsWay.Example.DataAccess.EF.ExampleDataAccess` itself. No dependency on `ProphetsWay.Example.Tests` at all; the `ProjectReference` to it goes.~~ **Declined by the owner, 2026-08-16.** It costs a permanent second copy of the assertions, which is exactly what the upstream suite exists to avoid |
| **B — an upstream seam** | **Chosen, 2026-08-16.** `ProphetsWay.Example` grows a way for a consuming repository to supply the implementation, and this repository provides it. Preserves the "same tests, different implementation" property that is the entire argument. **Requires a change in the other repository**, which is why it is filed there as [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation) |

**Shape B is the one that keeps the paradigm claim true**, and A is the one that could be done unilaterally.
A is also the one that quietly destroys the demonstration: two copies of the suite that must be kept in
step is the duplication problem the submodule arrangement exists to prevent, and the moment they diverge
the sentence "the tests do not change to accommodate it" stops being checkable. **That is the reasoning the
owner accepted.**

**Note the interaction with [ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md),
which is `Rejected`.** That entry declines reading the implementation choice from an environment variable
or `.runsettings`, on the grounds that one obvious line beats a lookup. **Shape B is not that proposal** —
it does not ask for configuration-driven selection, and the single obvious line can stay exactly where it
is as the default. It asks only that the line be *reachable* from a repository that cannot edit it. Do not
let FR 8's rejection be read as having already declined this; it was answering a different question.
**Choosing shape B does not reopen FR 8, and this distinction must survive any future summary of either.**

### The resolution — 2026-08-16: shape B, the direction only

**~~Open question for the owner: A or B?~~ Answered: B.** The open half of the rescope is closed, and what
follows is the whole of what the answer settles.

- **Shape A is foreclosed.** A duplicate local suite in this repository is declined, on the grounds stated
  above: it ends the demonstration `ProphetsWay.Example` exists to provide.
- **The six adapter classes were to be *deleted*, not rebuilt — and they have been.**
  `EFBaseDataAccessTests`, `EFCompanyDaoTests`, `EFJobDaoTests`, `EFResourceDaoTests`,
  `EFTransactionDaoTests` and `EFUserDaoTests` contained an override of a member that no longer exists
  upstream and nothing else. There was no repair; the concept is gone. Anything worth keeping is
  [Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs)'s connection handling, and that is provider
  wiring rather than a test hook. **Deleted 2026-08-16** — see
  [What has landed](#what-has-landed--2026-08-16).
- **The direction is committed; the seam's design is deliberately deferred.** Nobody has yet attempted to
  satisfy the 3.1.0 contracts in Entity Framework, so **the seam's requirements are unknown.** The decision
  forecloses shape A and **nothing else**. The seam is to be designed once **Lap 1** has shown what it must
  carry.
- **Do not read a committed direction as an approved design, and do not read the absence of a seam design as
  unfinished work.** There is no design owed by anyone at this date. The trigger is Lap 1.

**A mechanism was proposed for the seam and it cannot compile.** It is recorded in full — with the reason and
with the viable shape that was sketched against it as a *sketch only* — in
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation),
because every file it concerns lives there. The short form, so nobody re-derives it from this side: making
`TestDataAccessFactory.Create()` `protected` and overriding it is impossible — the type is a `public static
class`, a static class cannot declare a `protected` member, and a static method is never virtual.

### What has landed — 2026-08-16

**The deletion half of this entry is done, and it is re-verified rather than inherited. The status line is
untouched** — the entry cannot be `Done` while its second deliverable waits on another repository.

Re-verified 2026-08-16 by listing `ProphetsWay.EFTools.Tests/`: it contains **`Constants.cs`,
`ProphetsWay.EFTools.Tests.csproj`, and build output. The six adapters are gone.**

**The upstream dependency is now in flight rather than merely scheduled — 2026-08-16.**
[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)
is being implemented by an `Interface Architect` as this is written. **It is not done, and this entry must not
be advanced on the strength of it** — the seam is unverified, and one constraint that emerged during its
design lands directly here: **a per-class virtual hook is rejected**, on the grounds that it recreates the six
adapter classes deleted above. Whatever shape the seam takes, this repository does not get its adapters back.

Three things worth stating plainly, because a future reader will otherwise read "a test project with no
tests" as damage:

- **The project containing no tests is correct and temporary.** It is the intended resting state under
  **D10** until the upstream seam exists.
- **The 35 upstream tests are parked, not lost.** They live in `ProphetsWay.Example.Tests` and are
  unaffected by anything done here.
- **Nothing green went red.** `LocalTestsOnly: 'yes'` already skipped them in CI, and the project had not
  compiled since the submodule advance. The deletion removed something that was already not running.

### What this entry can and cannot deliver from inside this repository

**It cannot be *completed* here.** That was already the finding of the rescope, and the resolution confirms
rather than removes it. This entry now has an explicit cross-repository dependency:

| Deliverable | Where it lands | Blocked on |
|---|---|---|
| ~~Deleting the six adapters~~ | **Here** | **Done 2026-08-16** |
| Deleting the dead `#if` branches in `Constants.cs` | **Here** | Nothing — still outstanding; `Implementer`'s file |
| A run of the upstream suite against the Entity Framework Data Access Layer | **Here**, once the seam exists | **[ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation)** — `Scheduled`; **the seam landed 2026-08-16 and is unverified** |
| The seam itself | **`ProphetsWay.Example`** — never edited from this side | **Landed 2026-08-16, unverified** — see the note below |

### The seam landed upstream — 2026-08-16, and nothing has been run through it

**Verified by opening `ProphetsWay.Example/ProphetsWay.Example.Tests/TestDataAccessFactory.cs` in the
submodule working tree**, not inherited. The row above previously read *"in flight"*; it is no longer.

The seam is **`public static void Use(Func<IExampleDataAccess> implementation)`** on
`TestDataAccessFactory` — an assignment, not a hook. Four properties of it bear directly on this entry:

- **It is a single call, not a per-class override**, which is the constraint that emerged during its design
  and which this entry recorded: *"this repository does not get its adapters back."* It does not. One file in
  `ProphetsWay.EFTools.Tests` calling `Use` once points all 164 upstream tests at the Entity Framework Data
  Access Layer.
- **It takes a `Func<>`, not an instance**, so a connection string, provider or context factory can be closed
  over on this side — which matters, because `ExampleDataAccess` here does not have a parameterless
  constructor.
- **It is guarded against a late write.** `Create()` sets a flag under a lock; `Use` throws
  `InvalidOperationException` if it arrives after the first instance was handed out. The documented mechanism
  is a `[ModuleInitializer]` in the consuming test assembly — which is **this** assembly.
- **The default line stays visible and unconditional**, so
  [ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md#8--selecting-the-implementation-from-configuration-instead-of-a-code-edit)'s
  rejection is untouched. This is not configuration-driven selection.

**This entry does not move, and must not be advanced on the strength of it.** Its remaining deliverables are
unchanged: the dead `#if` branches in `Constants.cs`, and **a run** of the upstream suite against this
repository's Data Access Layer. Nothing has been run through the seam in either repository, and the Entity
Framework Data Access Layer still answers 11 members with `NotImplementedException`
([entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 2). **A seam that exists
is not a suite that passes.**

**What it does change is the risk profile of [D13](purpose-and-scope.md#owner-decisions--2026-08-15).** The
objection to hand-writing a `DepartmentDao` was that it meant writing code with no suite to verify it; the
seam is what answers that — on paper. The first green run is what answers it in fact.

What the rebuilt suite gains, and why it is worth having rather than merely unavoidable:

- **`dotnet test --filter "Scope=Contract"` becomes the conformance gate** — the subset any implementation
  must pass, separated from `Characterization` (choices the NoDB implementation made) and `Dispatcher`
  (reflection-convention tests belonging to `ProphetsWay.BaseDataAccess`, which no DAL is bound by).
- It is the closest thing that currently exists to
  [BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md)'s conformance kit, whose
  own revisit trigger is *"after `ProphetsWay.EFTools` has been built and updated onto 3.x."* Doing this
  well is the input that entry needs.

[Constants.cs](../ProphetsWay.EFTools.Tests/Constants.cs) also carries `#if` branches for `NET45`
through `NETCOREAPP3_1` and `NET5_0` — targets this project has not had in some time. Dead conditions, to
be removed. **Still present as of 2026-08-16**, and now more clearly dead than before: under the single
`net10.0` target only the `NET6_0_OR_GREATER` arm compiles, so the `Data Source=localhost` connection
string the other two arms reach is unreachable.

**Open question — answered.** Does the EF suite point at a real SQL Server (keeping
`LocalTestsOnly: 'yes'`), or at a provider CI can run? **Both**, per
[D4](purpose-and-scope.md#owner-decisions--2026-08-15) — SQLite in-memory for the CI gate and a SQL Server
container for provider fidelity. The `InMemory` provider is out either way: it **cannot honour
transactions**, so the transaction-scope contract tests cannot pass against it, meaning the current
arrangement is structurally unable to verify part of the contract this package now claims. The work is
[entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).

---

## 7 — Stop forcing a database provider on every consumer

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** Both halves have landed, and the sentence in the body
> below reading *"All three are still present as of 2026-08-16"* is **now false and must not be restated.**
>
> **The packaging half**, verified by opening `ProphetsWay.EFTools/ProphetsWay.EFTools.csproj` on this date:
> the runtime `ItemGroup` carries **`Microsoft.EntityFrameworkCore` 10.0.11 alone**, above a comment reading
> *"Provider-neutral by design: a consumer chooses their own EF provider. Never add one here."* There is no
> `Microsoft.EntityFrameworkCore.SqlServer` and no `Microsoft.EntityFrameworkCore.InMemory` line anywhere in
> the file. `Microsoft.EntityFrameworkCore.InMemory` and `.Sqlite` now sit in
> `ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj` under a comment naming them test-only and
> forbidding their return — which is the *"move `InMemory` to the test and example projects"* bullet, done.
>
> **The code half**, verified by grepping all 15 library `.cs` files for `UseSqlServer` and
> `UseInMemoryDatabase` — **no matches** — and by opening `ProphetsWay.EFTools/BaseEFContext.cs`, which
> declares one `protected BaseEFContext(DbContextOptions)` constructor and no other member. **The string
> constructor was deleted**, which is the option this entry called *"the honest option"*; the open
> implementation choice D2 left is therefore settled by the implementation.
>
> Not re-measured here, and recorded as the owner's evidence rather than mine:
> `dotnet list package --include-transitive` before and after the removal resolves to `EntityFrameworkCore`
> plus `ProphetsWay.BaseDataAccess` 3.2.0, with only `.Abstractions` and `.Analyzers` beneath — no SqlServer,
> InMemory, Relational, or `Microsoft.SqlServer.Server` anywhere in the closure.
>
> **`AGENTS.md` deviation 3 is the same fact as this entry and still records the packaging half as open.**
> `Repo Analyst` follows this pass and owns that file.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15. **Approved by the owner as
[D2](purpose-and-scope.md#owner-decisions--2026-08-15).** Previously `Proposed`. The clearest scope
violation in the package, and the one nobody had reported.

[ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) declares, in an `ItemGroup`
whose `net4`-excluding condition is now unconditionally true given the single `net10.0` target (versions
omitted — mid-flight, see [entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)):

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="..." />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="..." />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="..." />
```

**All three are still present as of 2026-08-16** — this entry is untouched by that day's changes — and [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) hardcodes the provider:

```csharp
protected BaseEFContext(string connectionString)
    : this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options) { }
```

### Why this is a scope violation rather than a default

The purpose sentence says this library supplies *plumbing*. Choosing the database is not plumbing; it is
the single most consumer-specific decision in a DAL. A consumer on PostgreSQL, SQLite, MySQL or Cosmos
installs `ProphetsWay.EFTools` and receives the **SQL Server** provider and the **InMemory** provider in
their dependency graph regardless, and cannot use the string constructor at all — the one the README
teaches first.

The `InMemory` reference is the sharper half. It is a **testing** provider that Microsoft documents as
unsuitable for production, and it is in the *runtime* package because the *examples* use it. A dependency
of the proving ground has become a dependency of the product. That inversion is exactly what the
equal-weighting of the two audiences in
[purpose-and-scope.md § Audience](purpose-and-scope.md#audience) is meant to prevent.

There is also a live consequence for [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits):
`InMemory` **cannot honour transactions**, so a suite built on it cannot verify the transaction contract
this package must now meet. The provider decision and the test-verification decision are the same decision.

### The shape of the fix

- Drop both provider references from the library. `Microsoft.EntityFrameworkCore` alone is the correct
  dependency for a package that never names a provider.
- Keep the `DbContextOptions` constructor as the primary path — the consumer supplies the provider, which
  is how every EF Core library does it.
- Either delete the string constructor, or keep it and require the derived context to configure the
  provider in `OnConfiguring`. **Deleting it is the honest option**; a "connection string" with no provider
  is not a meaningful parameter.
- Move `InMemory` to the test and example projects, where it is a legitimate test dependency.

**Breaking**, by design, and cheap to explain: consumers add one `PackageReference` for the provider they
already knew they were using.

### Counter-argument — **weighed and declined**

The string constructor is the first thing the README teaches and the shortest path from zero to a working
DAL, and SQL Server is very likely what every current consumer uses. Removing it makes the getting-started
experience longer for the majority in order to make it *possible* for a minority. The alternative offered
here was to **name** the SQL Server path as such — `UseSqlServerConnectionString`, or a separate
`ProphetsWay.EFTools.SqlServer` package.

**The owner declined both.** D2 makes provider neutrality part of the purpose sentence, and a named
provider reintroduced behind a friendlier name is the same coupling with better manners. Recorded, not
deleted: if onboarding friction turns out to be a real complaint from real consumers, this is the paragraph
to reopen.

### What D2 settled, and what it left to the implementer

**Settled — the scope of the promise is three tiers, not two:**

| Tier | Providers | Meaning |
|---|---|---|
| **Certified** | SQLite, SQL Server | This repository's tests run against them; a green suite is evidence for these two only |
| **In scope, uncertified** | PostgreSQL, MySQL/MariaDB, Oracle | Relational; nothing in the design excludes them, and a defect report against one is in scope |
| **Out of scope** | Cosmos, other non-relational | The DAO surface assumes `Skip`/`Take` over a stable order, server-side `GetCount`, and the parent's transaction semantics |

**Left open — an implementation choice, not a purpose question:** whether
`BaseEFContext(string connectionString)` is **deleted** or **retained** with the provider configured by the
derived context's `OnConfiguring`. Either satisfies neutrality. What it may **not** do is name a provider.

**Q4 — closed.** The certified/uncertified split is stated **on the package**, not only in these docs —
owner decision [D8](purpose-and-scope.md#owner-decisions--2026-08-15). Public wording says EFTools is
designed for **relational EF Core providers** and **certified and tested by this repository on SQLite and
SQL Server**, and must **not imply any other relational provider is certified**. The exact constraint is in
[purpose-and-scope.md § Public Wording](purpose-and-scope.md#public-wording--settled); the files that carry
it belong to `Modernizer` and `README Author`.

---

## 8 — Remove `FluentAssertions` from `ProphetsWay.Example.DataAccess.EF`

**Status:** **Done — 2026-08-16.** Previously `Scheduled for v3.0.0` (2026-08-15, owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15)). Trivial, isolated, and it did not wait — which is
exactly what the sequencing analysis below predicted.

**Re-verified before the status moved**, not affirmed:
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
was opened on 2026-08-16 and contains no `FluentAssertions` line. Its `PackageReference` entries are
`Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer` and
`ProphetsWay.BaseDataAccess` 3.1.0, and nothing else. **The licence exposure is closed.**

**Why `Done` and not held to the release.** The entry's own reasoning below establishes that there was nothing
to *ship* — the project is not packaged and reaches no consumer — so the only meaningful state was "is the
reference in the file." It is not. Holding it open until v3.0.0 would keep an entry alive to track a file that
no longer contains the thing it tracks.

[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
carried `<PackageReference Include="FluentAssertions" Version="8.2.0" />`. Two independent problems:

1. **It is a test-assertion library in a non-test project.** That project is a DAL implementation. Nothing
   in it should assert anything.
2. **FluentAssertions 8.x requires a paid commercial licence.** House convention names **Shouldly** as the
   assertion library and states plainly that FluentAssertions must not be added to any project.

The project is not packaged, so no consumer is affected and this is not breaking. It should still go: a
proving ground that models bad dependency hygiene teaches bad dependency hygiene.

**Check before deleting:** confirm nothing in `Daos/`, `ExampleContext.cs` or `ExampleDataAccess.cs`
actually uses it. If something does, that code is the real finding.

**Checked, 2026-08-16 — nothing uses it.** A search for `FluentAssertions` and `Should()` across all seven
`.cs` files in the project (`ExampleDataAccess.cs`, `ExampleContext.cs`, and the five under `Daos/`)
returns **no match**. Every other hit is the `PackageReference` itself or a `bin`/`obj` build artefact.
There is no "real finding" behind it; it is an unused reference and nothing more.

### Why it stayed Scheduled at the time — 2026-08-16, superseded later the same day

Retained because the sequencing argument is the durable part and it turned out to be right. The heading
previously read "Why it stays Scheduled rather than moving"; the entry is now `Done`, and a heading that
contradicts its own status line is the defect this pass exists to remove.

The question put was whether the paid-licence exposure on an unused reference should move this out of
v3.0.0 and into something sooner. **It should not have moved status *then*, but its sequencing claim needed
correcting.**

- **Status stayed `Scheduled` for v3.0.0.** Nothing about it had changed: the work is a one-line `.csproj`
  deletion owned by `Modernizer`, and v3.0.0 is the release it lands in. Moving it to a separate release
  would have meant cutting a 2.2.x patch to remove a reference from a project that is **not packaged and not
  published**, which reaches no consumer at all. There was nothing to ship.
- **Its "should not wait for anything" is currently false, and that is worth saying.** It *is* waiting —
  not on priority but on the absence of any green baseline, since the repository does not compile while
  [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) is mid-flight.
- **But the wait is not required by this entry's own risk.** The reference is statically verified unused,
  so its removal cannot break a compilation that entries 1–3 have not already broken. It is therefore
  **eligible to land ahead of the rest of v3.0.0**, as an isolated commit, the moment anyone is editing
  that csproj — which entries 2 and 5 both require anyway.
- **Frame it as a licence item, not hygiene.** The distinction changes who cares: hygiene waits for a
  convenient release, a licence obligation does not. The honest scope of the exposure is that the package
  is **restored and resolved** rather than *used* — `obj/project.assets.json` resolves 8.2.0 across four
  target legs — and that whether a licence is owed at all turns on whether the owner's use is commercial,
  which is not this agent's to determine. The house convention forbids it regardless of that answer, and
  that is sufficient reason on its own.

**Not this agent's edit.** `Modernizer` owns csproj changes.

### Landed — 2026-08-16

**The reference is gone.** Verified by opening
[ProphetsWay.Example.DataAccess.EF.csproj](../ProphetsWay.Example.DataAccess.EF/ProphetsWay.Example.DataAccess.EF.csproj)
on that date: it contains no `FluentAssertions` line, and its only `PackageReference` entries are the two EF
Core packages and `ProphetsWay.BaseDataAccess`. **The substance of this entry is satisfied and the licence
exposure is closed.**

**The status line has moved, and this is the one place in this file where that most needs saying.** The
entry above reasons at length about *when* this should land relative to v3.0.0 — and it landed early, exactly
as "eligible to land ahead of the rest" anticipated. **Triaged `Done` on 2026-08-16.**

---

## 9 — Delete the stray `[submodule "Submod"]` block from `.gitmodules`

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** Verified by opening `.gitmodules`, which is four lines
> long and declares exactly one submodule: `[submodule "ProphetsWay.Example"]` with
> `path = ProphetsWay.Example`, `url = https://github.com/ProphetManX/ProphetsWay.Example.git` and
> `branch = main`. **There is no `Submod` block.**
>
> **Removal stopped the Source Link *warning*; it did not give this package Source Link.** That is
> [entry 16](#16--source-link-symbol-packages-and-the-empty-packaging-metadata-stubs), filed on this date.
> Read the two together or you will conclude symbols work here.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15, by owner decision
[D6](purpose-and-scope.md#owner-decisions--2026-08-15). Trivial. Discovered from the other side, in
[ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md), and recorded here because
the file is here.

[.gitmodules](../.gitmodules) contains, after the legitimate `ProphetsWay.Example` block:

```ini
[submodule "Submod"]
  branch = main
```

No `path`, no `url`. Git tolerates it today because nothing resolves it, but it is a malformed submodule
declaration that will confuse `git submodule` operations — and, more immediately, anyone auditing this
repository's submodule story, which is already carrying a stale "vendored copy" claim in `AGENTS.md`.

Delete the two lines. No build impact, no consumer impact.

---

## 10 — Collapse the `Guid`/`Int`/`Long` DAO triplication

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** Verified by listing `ProphetsWay.EFTools/` and
> grepping the `namespace` declaration of every file in it on this date. The folder is **flat — no `Guid/`,
> `Int/` or `Long/` subfolder — and holds 15 `.cs` files, every one of which declares
> `namespace ProphetsWay.EFTools` and nothing else.** The 18 key-specific closures, the two `RootBase*`
> bridges with their `where TIdType : struct` constraint, the internal `RootDao<T,TIdType>` and the three
> short-lived `Legacy*` types are all gone; the open-key families `BaseDao<TEntity,TKey>`,
> `BaseGetAllDao`, `BasePagedDao`, their three soft counterparts and the four keyless types are what replaced
> them, alongside `BaseEFContext`, `BaseEFDataAccess<TContext>`, `ContextOwnership`, and the two internal
> statics `EntityGraph` and `SoftTimestamps`.
>
> `CHANGELOG.md`'s v3.0.0 entry states the same surface independently, including that **no `Legacy`-prefixed
> class is part of 3.0.0.**

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Scheduled 2026-08-15. **Approved by the owner as
[D3](purpose-and-scope.md#owner-decisions--2026-08-15), reversing this entry's recommendation.**
Previously `Proposed — and recommended against`.

**This is the one entry in the file where the agent recommendation was overturned, and the reversal was
correct.** One of the three arguments below was not a judgement call — it was a factual claim about the
source, and it was **wrong**. It is corrected in place rather than deleted, because a decision index that
quietly edits out its own errors cannot be trusted about anything else.

### The approved shape

The 18 key-typed classes are replaced by **six generic families in the root namespace** —
`BaseDao<TEntity, TKey>`, `BaseGetAllDao<TEntity, TKey>`, `BasePagedDao<TEntity, TKey>` and their three
soft-delete counterparts. The public breaking change is **accepted** and lands in the same major.
**No compatibility wrappers** — unless implementation evidence forces reconsideration, which the
[preservation requirements](#what-the-collapse-must-preserve) below define precisely.

### The observation

`ProphetsWay.EFTools` contains `Guid/`, `Int/` and `Long/` folders, each with the same six types —
`BaseDao`, `BaseGetAllDao`, `BasePagedDao`, `BaseSoftDao`, `BaseSoftGetAllDao`, `BaseSoftPagedDao` — 18
near-identical classes. Each is a thin closure over `RootBaseDao<T, TIdType>` or
`RootBaseSoftDao<T, TIdType>`. [Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs) is representative:
a constructor pass-through and one `Get` override. A change to one "nearly always must be made to all
three," per `AGENTS.md`.

That is real duplication, and an agent seeing it for the first time will propose collapsing it to a single
generic `BaseDao<T, TIdType>`. It has surfaced before, and this time it was approved.

### Why this entry argued against it — and which leg broke

Preserved in full. The middle one is the one that failed.

1. **"The duplication is the point."** These types exist so a consumer writes
   `using ProphetsWay.EFTools.Int;` and then `BaseDao<User>` with **one** type parameter. A collapsed
   `BaseDao<User, int>` forces the key type into every DAO declaration in every consumer's codebase, to
   save 18 files in one library. The cost lands on every consumer; the saving lands on one maintainer.
   — **Still true. The owner accepted the cost**, and it is ergonomic rather than a loss of capability.
2. ~~**"The `Get` override is not boilerplate."** It is the reason the split exists… `x => i.Id == item.Id`
   needs a comparison the C# generic constraint system cannot express over an open `TIdType` without
   `IEquatable`/`IComparable` gymnastics or `EqualityComparer<T>.Default` indirection that EF Core cannot
   translate to SQL.~~
   — **FALSE, and checkable in this repository.**
   [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already performs exactly this comparison generically on
   the EF Core branch: `Dataset.AsTracking().Single(x => x.Id.Equals(item.Id))` in `Update`, and
   `Dataset.OrderBy(x => x.Id)` in `GetPaged` — both over an open `TIdType`, both translated by EF Core
   today. [Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs) uses `i.Id == item.Id` because `int`
   permits the operator, **not** because the generic form is impossible. The claim was inherited from the
   README's "required so the default `Get` method can build a proper select by Id" and restated here
   without opening `RootDao.cs`. This was the load-bearing leg; with it gone the recommendation does not
   stand.
3. **"Binary-breaking for every consumer, for a change with no behavioural benefit."**
   — **Still true, and now free.** The same major already breaks on entries 2, 4, 5 and 7. A breaking
   change that must ride a major is cheapest when a major is already leaving.
4. **"The volume is overstated."** Eighteen files averaging roughly ten lines.
   — **Still true, and now beside the point.** The saving was never the argument for doing it; the argument
   is that a key type can now be added without adding a namespace, and that argument only became available
   once leg 2 fell.

### What the collapse must preserve

These are the conditions under which D3's "no compatibility wrappers" holds. If one of them cannot be met,
that is the implementation evidence that reopens the question — not a preference.

**Added 2026-08-16 — the collapse is not a re-wrap, and this is the most load-bearing constraint on it.**
`RootBaseSoftDao` violates four of `IDepartmentDao`'s 19 rules and `BaseNonIdDao<T>` structurally cannot
serve `ICompanyResourceDao`. The six generic families this entry creates must therefore **fix `Update` and
`Delete` semantics**, not close over the existing bases with a new type parameter list. The evidence is
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance); read it
before designing the families.

**Constrained further by owner decision [D13](purpose-and-scope.md#owner-decisions--2026-08-15) —
2026-08-16.** The soft-delete families are to be **derived from a hand-written concrete Entity Framework
`DepartmentDao`, proven against `IDepartmentDao`'s 19 rules, rather than designed ahead of one.** The owner's
reasoning: *"the point of the generic tests and classes was to just reduce all the duplicative code, but in
this case there is a real need for it."*

**This does not reopen [D3](purpose-and-scope.md#owner-decisions--2026-08-15) and does not change this entry's
status or shape.** Six generic families, no compatibility wrappers, in the same major — all unchanged. What
D13 settles is **order**: a concrete implementation first, generalized second.

Three consequences worth having written down:

- **Deduplication is the benefit of the collapse, not the evidence for it.** That distinction is the whole of
  D13. Entry 13 is the demonstration that a family designed from the existing bases ships four rule violations
  under new type names; a family generalized from a Data Access Object that already passes
  `DepartmentDaoTests` cannot.
- **The generalization is then a refactor with a green suite behind it**, which is a materially different
  activity from designing against a specification nobody has satisfied once.
- **It does not license a second permanent surface.** The concrete `DepartmentDao` lives in
  `ProphetsWay.Example.DataAccess.EF`, which is **not packaged** and never becomes part of the library. If it
  survives as anything, it survives as the proving ground's implementation — not as a shipped shim, which
  [D3](purpose-and-scope.md#owner-decisions--2026-08-15) rejects.

**The counter-argument, recorded because it was made:** writing the semantics concretely first means writing
them twice, and the surviving copy is the generic family. The answer is the second bullet above — but a
reader should see the cost rather than only the conclusion.

- **The `Get` predicate must remain translatable to SQL.** Use the `x.Id.Equals(item.Id)` form already
  proven in `RootDao.Update`, **not** `EqualityComparer<TKey>.Default`, which EF Core cannot translate.
  This is the single place where the collapse could genuinely fail.
- **Decide the fate of `where TIdType : struct`.** `RootDao<T, TIdType>` carries it today, which excludes
  `string` keys. Keeping it makes the collapse a pure refactor of the surface; relaxing it is a **reach**
  decision that would also close
  [Example FR 1](../../ProphetsWay.Example/docs/feature-requests.md)'s accepting-half gap. Carried as **Q3**
  in [purpose-and-scope.md](purpose-and-scope.md#unresolved-purpose-level-questions); D3 does not settle it.
- **Verify against the certified providers, not just SQLite.** A predicate that SQLite translates and SQL
  Server does not would be found by [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
  and by nothing else.

### The strongest argument against, recorded so it is weighed rather than forgotten

Six generic families in one namespace is a **less discoverable** surface than three namespaces of six. A
consumer who types `using ProphetsWay.EFTools.Int;` gets an IntelliSense list containing exactly the six
types that can apply to them. Afterwards they get one list of six and must supply the right `TKey`
themselves — the compiler catches a mistake, but later and less kindly than a namespace that could not
express it.

### What would reopen the "no wrappers" half of D3

- The `Get` predicate proving untranslatable on a **certified** provider. That is the escape hatch D3 names.
- A real consumer migration turning out to be larger than "add one type argument per DAO declaration."

**Fold this into the v3.0.0 modernization.** The earlier instruction here was the opposite — "do not fold
this in; it looks adjacent to the retarget and is not." That advice followed from the recommendation the
owner overturned. Riding the same major is now the cheapest possible way to land it.

### The "18 = 3 key types × 6 shapes" hypothesis — **counted 2026-08-19, and it survives as an inventory only**

Recorded here because it is the fact the `Interface Architect` shape pass is conditioned on, and because it
had been carried as *likely but unverified*. **It was verified by opening the files**, not by reading this
entry: a directory listing of `ProphetsWay.EFTools/`, a repository-wide grep for `public abstract class`
returning **24** declarations in 24 files, and the bodies of `Int/BaseDao.cs`, `Int/BaseGetAllDao.cs`,
`Int/BasePagedDao.cs`, `Int/BaseSoftDao.cs`, `RootBaseDao.cs`, `RootBaseSoftDao.cs` and `BaseNonIdDao.cs`.

**The count is exactly right.** `Guid/`, `Int/` and `Long/` each hold the same six file names, and the class
declarations differ only in the key type substituted into the base and the constraint. 3 × 6 = 18, and the
whole public surface is those 18 plus `BaseNonIdDao`, `BaseSoftNonIdDao`, `RootBaseDao`, `RootBaseSoftDao`,
`BaseEFContext` and `BaseEFDataAccess` — **24, which is the number `docs/api-contract.md` says it reduces
from.** That cross-check is now measured rather than asserted on both sides.

**What the hypothesis gets wrong is the word "shapes."** It invites the reading that six behaviours are being
generalized. They are not:

| Of the 18 | How many | What is in the body |
|---|---|---|
| `BaseGetAllDao`, `BasePagedDao`, `BaseSoftGetAllDao`, `BaseSoftPagedDao` | **12** | **Nothing.** A constructor pass-through and one added interface declaration. Zero members |
| `BaseDao`, `BaseSoftDao` | **6** | **One member each** — `public override T Get(T item)`, and all six bodies are the same expression, `Dataset.Where(i => i.Id == item.Id).SingleOrDefault()`, modulo the key type |

**So the total behaviour distributed across 18 classes is one method.** Everything else — CRUD, the retrieval
trio, the transaction helpers — lives in `RootBaseDao`, which already implements **all three** capability
interfaces on a single type; and the soft-delete semantics live entirely in `RootBaseSoftDao`, which is where
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)'s
violations are. The six names are **interface-declaration variants**, not capability variants, which is
exactly what `docs/api-contract.md` S2 calls the flat method surface and what it preserves.

**Three consequences for the shape pass, and they all point the same way — start.**

1. **The collapse is smaller and safer than the entry implies.** It deletes 12 empty classes, hoists one
   expression into `KeyEquals` / `MatchRow`, and keeps six names that api-contract **S1** already fixes. There
   is no behaviour to reconcile across the 18, because there is none in them.
2. **The one genuine risk is the one this entry already named** — the `Get` predicate's translatability. It is
   the *only* thing in the 18 that the collapse must reproduce, which is why it is the escape hatch
   [D3](purpose-and-scope.md#owner-decisions--2026-08-15) names.
3. **A gap in the family set exists and is already covered — do not "fix" it.** There is **no
   `BaseSoftGetAllPagedDao`**, and `IDepartmentDao` declares **both** `IBaseGetAllDao<Department>` **and**
   `IBasePagedDao<Department>` — verified by opening
   `ProphetsWay.Example.DataAccess/IDaos/IDepartmentDao.cs`, line 230. It compiles anyway, and only because of
   the flat surface: `BaseSoftPagedDao<Department, int>` carries a working public `GetAll`, which satisfies
   the second interface implicitly. **A seventh family must not be added to close the gap.** What is owed
   instead is one sentence of guidance — which base a DAO whose interface declares two capabilities should
   pick — because four of the six families are empty markers and the choice between them is otherwise
   arbitrary. That is a documentation obligation on the shape pass, not a design change.

---

## 11 — Certify the contract suite on SQLite in-memory and a SQL Server container

> **CURRENT RESULT — 2026-08-29: LOCAL SQL SERVER GATE 1 IS SATISFIED; FR 11 REMAINS `Scheduled`.**
>
> Report 07 for run `20260829-1450-eftools-fr11-gate1` records external
> `EFTOOLS_PROVIDER=SqlServer`, no test filter, and **328 discovered / 328 passed / 0 failed / 0 skipped**.
> All **14** provider-comparison cases executed and passed while remaining exempt only from provider
> selection, leaving **314 provider-honouring cases**. All **58** provider-selection guards passed, and the
> local SQL Server catalogue held **zero** disposable `EFToolsTest_*` databases both before and after.
>
> The exact evidence source is
> `.agent-runs/20260829-1450-eftools-fr11-gate1/07-test-designer-certification-run.md` under the project parent.
> The runtime total supersedes the 287-case planning snapshot below without rewriting it as history.
>
> **This does not close the entry.** Whole-suite SQLite certification and the separately owned
> pipeline/`LocalTestsOnly` work remain. It also does not reach Azure SQL;
> [entry 19](#19--certify-the-contract-suite-against-azure-sql) remains the distinct, unsatisfied Gate 2.

The dated 2026-08-24 re-triage below is preserved as the planning record that defined the gate.

> 🔴 **RE-TRIAGED 2026-08-24 — THIS ENTRY IS A RELEASE GATE. It is GATE 1 OF TWO.** Status token **unchanged
> at `Scheduled`**;
> its *classification* moved from "not release-blocking" to blocking. **Read this block before the 2026-08-23
> one below it, which reached the opposite conclusion honestly and on less information.**
>
> **The owner's words, which are the decision:**
>
> > *"if we can't say EFTools is cleared to work in SQL Server, then it's not worth pushing out."*
>
> Recorded as [D20](purpose-and-scope.md#owner-decisions--2026-08-15). **`ProphetsWay.EFTools` 3.0.0 stays
> untagged and unpublished until one complete whole-suite SQL Server run exists** — and, since
> [D22](purpose-and-scope.md#owner-decisions--2026-08-15) later the same day, until
> [entry 19](#19--certify-the-contract-suite-against-azure-sql)'s Azure SQL run exists as well.
> **Both gates, not either.** The reason the 2026-08-23
> pass got this wrong is worth keeping: it judged certification as *CI evidence for a correct package*, which
> it is. What it could not know was that the package's one real consumer deploys on SQL Server, which makes
> the same evidence a **fitness-for-purpose** claim instead. Nothing about the code changed; the bar did.
>
> **What the gate is, exactly — and what it is not:**
>
> | | Blocks the 3.0.0 tag? |
> | --- | --- |
> | **The whole suite executed against SQL Server, green** — ~~all 270 cases~~ **287 as of 2026-08-24, of which 14 are exempt from the *selection* and none from the *run*; see [the restatement](#the-close-condition-restated-2026-08-24--the-number-moved-the-bar-did-not)** | 🔴 **Yes** — this entry, D20 |
> | The **whole** suite on **SQLite**, green | **No** — still owed under D2/D4, still not the gate. **Not cancelled** |
> | `LocalTestsOnly: 'yes'` removed so CI runs any of it | **No** — `Pipeline Engineer`'s, still `Deferred`. See [below](#the-localtestsonly-half-is-still-pipeline-engineers) |
> | An **Azure SQL** leg | 🔴 ~~**Undecided** — no owner scope call yet. Not filed, not invented~~ **Yes, and it is a SEPARATE gate** — decided later the same day as [D22](purpose-and-scope.md#owner-decisions--2026-08-15) and filed as [entry 19](#19--certify-the-contract-suite-against-azure-sql). **This entry does not cover it, and satisfying this entry does not satisfy it** |
>
> **The environment exists. That is not the same as the evidence existing.** The owner has confirmed:
>
> > *"i have a local instance of mssql running on local host, if we need to deploy the schema from Example's
> > sqlproj dacpac, we can deploy it locally to then run our tests against it."*
>
> Recorded as [D21](purpose-and-scope.md#owner-decisions--2026-08-15), which also authorizes deploying
> `ProphetsWay.Example.Database`'s DACPAC locally if the run needs it — the `.sqlproj` is present in the
> submodule at `ProphetsWay.Example/ProphetsWay.Example.Database/ProphetsWay.Example.Database.sqlproj`,
> located by file search on 2026-08-24. **A running server and a deployable schema are *available evidence*.
> This entry closes when a run has happened, not when the means to run exists.**
>
> **The schema half is now stronger than "deployable" — and it is still not this entry's evidence.** The owner
> reports the local `ProphetsWay.Example` schema carrying a **zero-drift DACPAC deploy report**, i.e. the
> `.sqlproj` and the live database agree. That removes schema mismatch as a candidate explanation for any
> failure the run produces, which is worth having before the run rather than after it. **It is not a test
> result.** Zero cases have been executed against SQL Server outside the 13 adapted classes, and this entry
> asks for 270.
>
> ### The implementation scope below is INSUFFICIENT — re-derived 2026-08-24
>
> **The 2026-08-23 block says closing item 1 "means making `Constants.GetExampleDataAccess`
> provider-selectable." That is true and it is not enough**, and stating it as the plan sends the next agent
> down a route that cannot reach 270. Verified on 2026-08-24 by opening the files named, not carried:
>
> | Group | How it picks a provider **today** | Reached by a `Constants` change? |
> | --- | --- | --- |
> | **13 adapted upstream classes** | `TestSeam.cs` `[ModuleInitializer]` → `TestDataAccessFactory.Use(() => Constants.GetExampleDataAccess, StoreCapabilities.TransactionIsolation)` → `Constants.GetExampleDataAccess` → `new ExampleDataAccess(connectionString)` → `.UseSqlServer(...)` | ✅ **Yes** — and they are **already on SQL Server** |
> | **7 store-backed local classes** | Each builds its own `new SqliteConnection("Filename=:memory:")`, calls `.UseSqlite(connection)` and then `schema.Database.EnsureCreated()` — `KeylessDaoTests`, `KeylessSoftDaoTests`, `KeyPredicateOpenKeyTests`, `SoftDeleteTimestampHookTests`, `FailedInsertWriteBackTests`, `IdentifierResolutionTests`, and `AlternateKeyGuardSpikeTests`, which parameterizes `.UseSqlite` **and** `.UseInMemoryDatabase` per `[Theory]` | ❌ **No** — none of them calls `Constants`, so editing it moves none of them |
> | **1 store-free local class** | `CompanyResourceConversionTests` — reflection only, no context, no connection | **n/a** — passes on any provider or none |
>
> **So the gap is not a connection string. It is that seven classes have no seam at all**, and each stands up
> its own schema in-process against a throwaway in-memory connection — something SQL Server has no direct
> equivalent of. **Reaching an all-270 SQL Server run requires provider-selectable wiring those seven
> actually consume, plus a decision about how each gets an isolated schema.**
>
> **Three sub-questions Stage 3 must answer, and this entry deliberately does not:**
>
> 1. **What shape the wiring takes**, and whether the seven classes converge on one fixture or keep
>    per-class construction with a provider parameter.
> 2. **What happens to `AlternateKeyGuardSpikeTests`**, which parameterizes two providers *on purpose* — its
>    own `<remarks>` call it *"an empirical spike, not a specification."* Forcing it onto SQL Server would
>    destroy what it measures; leaving it pinned means "all 270 on SQL Server" has a footnote. **Either is
>    defensible and the choice is not this document's.**
> 3. **Schema provisioning** — `EnsureCreated()` per test against a real server is not free and is not
>    isolated the way `Filename=:memory:` is. This is where [D21](purpose-and-scope.md#owner-decisions--2026-08-15)'s
>    DACPAC authorization is likely to earn its place.
>
> **None of the three is a scope question**, which is why they are recorded rather than answered: they are
> test-design questions for the agent that writes the lap. What *is* settled here is the target — **one
> complete ~~270/270~~ whole-suite run against SQL Server** — and that no tag precedes it. **See the
> restatement below: the count moved and the criterion did not.**

### The close condition, restated 2026-08-24 — the number moved, the bar did not

**Sub-question 2 above has been answered by `Test Designer`, and the answer changes the arithmetic of this
entry rather than its target.** Recorded here because this entry's *"one complete 270/270 run"* is quoted
elsewhere and is now wrong in both directions.

| What moved | From | To | How it was established |
| --- | --- | --- | --- |
| Discovered cases | 270 | **287** | `Test Designer` added 17 provider-selection seam guards — `ProviderSelectionTests` declares 7 `[Fact]` plus 3 `[Theory]` carrying 10 `[InlineData]`; **counted 2026-08-24 by grepping every xUnit attribute in the file** |
| Cases that can honour a provider selection | — | **273** | 287 minus the spike |
| Cases structurally exempt from the selection | — | **14** | `AlternateKeyGuardSpikeTests`: 7 `[Theory]` × 2 `[InlineData]` (`InMemory`, `Sqlite`), **all `Scope=Characterization` / `Area=AlternateKeys`, verified by opening the file** |

**The spike stays pinned, and it stays at full strength.** Its subject is Entity Framework Core's own
alternate-key behaviour ***compared across* providers**, so the comparison **is** the measurement — forcing it
onto the selected provider deletes the case rather than certifying it. `Test Designer` chose to pin and guard
it rather than weaken or re-trait it; a new guard holds all its cases to `Scope=Characterization` so the
exemption cannot quietly widen. **Do not "fix" this by re-traiting the spike or by dropping one of its
`[InlineData]` legs.**

**The exemption is from the *selection*, not from the *run*.** All 287 cases execute in the certification
command and **all 287 must pass**. "Which store did it open" and "did it pass" are different questions and only
the first has an exemption.

**The five criteria that close this entry** are stated once, in
[purpose-and-scope.md § Gate 1's success criterion](purpose-and-scope.md#gate-1s-success-criterion-stated-so-it-does-not-go-stale),
and are not duplicated here. The short form a downstream agent needs: **an externally anchored SQL Server
selection, a green no-filter run, every store-touching case on SQL Server bar the spike, a guard that turns red
if anything else picks its own provider, and four recorded counts plus the resolved provider.** The counts
(**287 / 14 / 273 / 0** today) are a snapshot; the criteria are the bar.

### The SQLite half's scope — already settled by D2 and D4, and re-derived rather than re-decided

**`Test Auditor` raised this on 2026-08-24 as an open question:** the specified `TestStoreProvider.Sqlite`
member makes SQLite selectable for the **whole** suite, including the 13 adapted upstream classes whose
SQLite schema provisioning nothing currently specifies. Whole-suite leg, or fast local leg only?

**It is not an open question and no owner decision is needed.**
[D2](purpose-and-scope.md#owner-decisions--2026-08-15) certifies *"only SQLite and SQL Server … by this
repository's tests"*, [D4](purpose-and-scope.md#owner-decisions--2026-08-15) names SQLite the fast **CI
contract/query gate**, and this entry's own 2026-08-23 block asks for *"a certified run of the **whole** suite
on **both** providers."* **SQLite was decided as a whole-suite certification leg on 2026-08-15.** So the enum
member is correct and the missing provisioning is **this entry's known non-blocking debt**, not an
over-promise to be trimmed. Deleting the member or narrowing it to the local classes would silently descope a
provider [D8](purpose-and-scope.md#owner-decisions--2026-08-15) puts on the package.

**The constraint that follows, and it is the only new obligation in this block:** selecting SQLite must either
**run the whole suite** or **refuse by name**. A SQLite selection that runs 273 cases and fails 13 adapters on
a missing schema reads as a provider defect in this library and is the opposite of one. **Deferring the SQLite
provisioning is legitimate; deferring it silently is not.**

> **RE-TRIAGED 2026-08-23 — status **unchanged at `Scheduled`**, but the entry is **much narrower than it
> reads**, and two of its own "verified facts" are now wrong.**
>
> **What has landed, verified by opening the files named on this date.** Both providers are in real use.
> `ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj` references
> `Microsoft.EntityFrameworkCore.Sqlite` **10.0.11** and `Microsoft.EntityFrameworkCore.InMemory` **10.0.11**
> under a comment naming Sqlite *"the relational in-memory certification leg"*; seven of the eight locally
> written test classes build their own SQLite in-memory context, and `AlternateKeyGuardSpikeTests`
> parameterizes `InMemory` **and** `Sqlite` per `[Theory]`. `ProphetsWay.EFTools.Tests/Constants.cs` supplies
> the other leg — one SQL Server connection string, `Data Source=localhost`, which is what the 13 adapted
> upstream classes reach through `TestSeam`. **So the fast leg and the fidelity leg both exist.**
>
> **Fact 2 in *Why the current arrangement cannot verify what 3.x claims* is dead.** It reads *"The provider
> this package currently depends on is structurally unable to verify the contract it is about to advertise"* —
> the package depends on no provider at all as of [entry 7](#7--stop-forcing-a-database-provider-on-every-consumer),
> and the transaction contract is exercised against SQL Server, not `InMemory`. **Do not restate it.**
>
> **Fact 1 is still true and is now the whole of this entry.** `app-variables.yml`, opened on this date, still
> reads `LocalTestsOnly: 'yes'`, so **CI executes none of the suite** — "the pipeline is green" remains
> evidence of compilation only.
>
> **What is genuinely still owed, and it is two things rather than the four this entry describes:**
>
> 1. **A certified run of the *whole* suite on *both* providers.** Today the split is fixed by construction:
>    the 8 local classes are SQLite-or-`InMemory` and the 13 adapted classes are SQL Server, and **no single
>    provider runs all of it.** D2 certifies SQLite *and* SQL Server, and D8 puts that claim on the package,
>    so the claim currently outruns the evidence by exactly this gap. ~~Closing it means making
>    `Constants.GetExampleDataAccess` provider-selectable so the adapted classes can also run on SQLite.~~
>    **Struck 2026-08-24 as insufficient** — see the re-derived scope above. It reaches the 13 adapted
>    classes and none of the seven store-backed local ones.
> 2. **The `LocalTestsOnly` half**, which is `Deferred` to the pipeline owner and is **no longer justified by
>    its own stated reason.** That reason was a local SQL Server dependency; the majority of the local suite
>    now needs no database at all, and item 1 above is what would let the rest run without one. Recorded so
>    the decayed justification is not inherited as still sound. `Pipeline Engineer` owns the edit; this is not
>    an authorization to make it.
>
> ~~**Neither is release-blocking.** Both are about *proving* 3.0.0 in CI rather than about 3.0.0 being correct,
> and item 2 changes no code a consumer receives.~~ **STRUCK 2026-08-24.** Item 1, restricted to SQL Server,
> **is** release-blocking under [D20](purpose-and-scope.md#owner-decisions--2026-08-15). Item 2 is not, and the
> sentence remains true of it. The distinction the struck text missed: the gate is not that CI runs the suite,
> it is that **the suite has been run against SQL Server at all.**

### The `LocalTestsOnly` half is still `Pipeline Engineer`'s

**Unchanged by [D20](purpose-and-scope.md#owner-decisions--2026-08-15), and deliberately so.** `app-variables.yml`
still reads `LocalTestsOnly: 'yes'`, CI still executes none of the suite, and **nothing in the 2026-08-24 pass
authorizes changing that or any other `.yml`.** The gate D20 sets is satisfiable by a run on the owner's own
machine; it says nothing about a build agent. Keeping the two apart is what stops the release gate from
quietly acquiring a pipeline dependency it does not have.

**Status:** **Scheduled for v3.0.0; local SQL Server Gate 1 satisfied 2026-08-29.** D20's release-blocking
condition is discharged by the run above, but whole-suite SQLite certification and the pipeline half remain
open. [Entry 19](#19--certify-the-contract-suite-against-azure-sql) is Gate 2 under
[D22](purpose-and-scope.md#owner-decisions--2026-08-15), remains release-blocking, and is not closed by this
entry's local result.
Originally scheduled 2026-08-15, approved by the owner as
[D4](purpose-and-scope.md#owner-decisions--2026-08-15) and reinforced by
[D8](purpose-and-scope.md#owner-decisions--2026-08-15), which turns "certified on SQLite and SQL Server"
into a claim the **package** makes. **The pipeline half is `Deferred`** to whoever owns
`app-variables.yml` and the shared templates; the removal of blanket `LocalTestsOnly: 'yes'` is not this
repository's decision alone and is explicitly not this agent's edit.

**New entry, promoted from a footnote.** This was carried as an open question inside entries
[1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) and
[6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits) and treated as downstream of
[7](#7--stop-forcing-a-database-provider-on-every-consumer). D4 answered it, which makes it approved work
with its own scope rather than a question hanging off three other entries. Numbered 11 because numbers here
are monotonic and permanent.

### The approved strategy

| Leg | Provider | Job |
|---|---|---|
| **Fast** | `Microsoft.EntityFrameworkCore.Sqlite`, in-memory mode | The CI contract and query gate. Runs on every build, no external dependency, honours transactions |
| **Fidelity** | SQL Server in a container | Proves the generated SQL against the provider most consumers actually use |

### Why the current arrangement cannot verify what 3.x claims

Three verified facts that together make this non-optional rather than a nice-to-have:

1. **`LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml) means CI runs no tests here at
   all.** "The pipeline is green" is currently evidence of compilation, not of behaviour.
2. **The `InMemory` provider cannot honour transactions.** The 3.x transaction-scope contract tests
   therefore **cannot pass against it** — not "have not been written," *cannot pass*. The provider this
   package currently depends on is structurally unable to verify the contract it is about to advertise.
3. **[D2](purpose-and-scope.md#owner-decisions--2026-08-15) certifies exactly two providers**, and
   certification is a claim about test evidence. **[D8](purpose-and-scope.md#owner-decisions--2026-08-15)
   puts that claim on the package**, so without this entry "certified on SQLite and SQL Server" would be
   an assertion of the same kind this document objects to elsewhere — printed on nuget.org.

### Why two legs rather than one

SQLite alone is fast and free but is a **different SQL dialect**, so a predicate it translates may not be
what SQL Server produces — which matters most for exactly the change
[entry 10](#10--collapse-the-guidintlong-dao-triplication) makes, where a generic `x.Id.Equals(item.Id)`
replaces per-key-type operators. SQL Server alone is faithful but too slow and too dependency-heavy to gate
every commit. Neither leg alone certifies what D2 promises.

### Scope boundary

**In scope for this entry:** the test projects' provider wiring, fixtures, and the `Scope` trait filters
they expose. **Out of scope:** the YAML. `Pipeline Engineer` owns `.yml`; the `LocalTestsOnly` change and
any container service definition belong to that agent and to the shared templates in `prophets-pipelines`.

### Q4 — closed: the certification is a public claim

The question was whether "certified on SQLite and SQL Server" is stated on the **package** — `<Description>`,
README, `PackageTags` — or only in these documents. **Answered by
[D8](purpose-and-scope.md#owner-decisions--2026-08-15): on the package.** Public wording says EFTools is
designed for **relational EF Core providers** and **certified and tested by this repository on SQLite and
SQL Server**, and must **not imply any other relational provider is certified**; the uncertified tier
(PostgreSQL, MySQL/MariaDB, Oracle) exists to set expectations and sets none if it is invisible to the
person installing from nuget.org.

**This raises the stakes on this entry rather than settling it.** The certification is now something the
package *says*, so the two legs below are what makes it true rather than an internal quality bar. Shipping
the wording without the suite would be the "asserted rather than demonstrated" failure these documents
object to elsewhere. The wording itself belongs to `Modernizer` and `README Author`; the evidence behind it
is this entry.

#### The wording is **not on the package yet**, and as of 2026-08-24 that is a relief rather than a defect

**Verified 2026-08-24 by opening `ProphetsWay.EFTools/ProphetsWay.EFTools.csproj` at `e2f2120` — the
completed packaging commit — and grepping `README.md`.** The `<Description>` names EF and the decoupling
paradigm and **does not use the word "certified"**; `<PackageTags>` reads
`entity-framework efcore ef-core dal data-access data-access-layer dao repository abstraction decoupling`
and names **no provider**; the README's nearest sentence says a consumer builds the `DbContextOptions` and
*"you choose SQL Server, PostgreSQL, SQLite or in-memory"* — a statement about *configurability*, not a
certification claim. **So [D8](purpose-and-scope.md#owner-decisions--2026-08-15)'s public-wording obligation
is still unmet.**

**That is now load-bearing rather than an oversight to tidy.** Had the wording landed on 2026-08-23 it would
have been a published claim with a whole-suite run behind neither provider. **The SQL Server evidence was
earned on 2026-08-29; the remaining sequence is to earn the SQLite evidence, then write the wording.**
`Modernizer` and `README Author` still own the edit; **nothing here authorizes writing it early, and nothing
here authorizes narrowing D8's certified tier to SQL Server alone.**

---

## 12 — `RootNonIdDao.EnsureBeginTransaction` silently no-ops against a pre-existing transaction

> **CLOSED 2026-08-23 — both halves. This entry is `Done`, and it was the last release blocker in the
> repository ~~—~~ *then known*.**
>
> > **Amended 2026-08-24.** The claim was true on its date and is no longer a statement about the present:
> > [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) is now a
> > release blocker under [D20](purpose-and-scope.md#owner-decisions--2026-08-15). **Nothing about *this*
> > entry changed** — it is `Done`, both halves, and does not reopen.
>
> **The code half needs nothing.** `RootNonIdDao.cs` was rewritten wholesale in the 3.x collapse and the member
> carrying the defect does not survive into the shipped surface — which is what this entry always predicted:
> *"Nothing to schedule in code: the 3.x design removes the members that carry the defect."*
>
> **The release-note half is now met, and this is measured rather than reasoned.** `CHANGELOG.md` was reopened
> later on the same date and every heading in its **v3.0.0** entry re-read. It now carries **six** `Fixed:`
> headings where it carried two, and the four
> [the D12 obligation](#the-changelog-author-obligation--d12) requires are all among them. **This entry's own
> row is § *"Fixed: commit and rollback no longer silently do nothing"***, which quotes the 2.2.0
> `if (Context.Database.CurrentTransaction == null)` guard, explains that `_transaction` stayed `null` and that
> both members then reached it through `?.`, and states the characteristic that made this the most dangerous of
> the four: *"No exception, no return value indicating a skip, nothing in a log."* It then says what 3.0.0 does
> instead — transactions on the Data Access Layer, no transaction member on any Data Access Object,
> `CurrentTransaction` never consulted, every misuse throwing.
>
> **The 2.2.0 known-issues note is present too**, at the top of the entry, naming all four and carrying the
> `net10.0`-only caveat D12 requires. The full field-by-field check is recorded once, on
> [the obligation itself](#the-changelog-author-obligation--d12), and is deliberately **not** duplicated here.
>
> **The table this block used to carry — four rows reading "No. Not named anywhere" — is history and must not
> be restated.** It was accurate earlier the same day and describes a file that has since changed.
>
> **What does not move:** the **2.2.x patch stays `Rejected`**. That is
> [D12](purpose-and-scope.md#owner-decisions--2026-08-15) and it is decision history, not a pending item; the
> release note was the thing accepted *in exchange* for it, so the note landing is the trade completing rather
> than a reason to revisit the trade.

**Status:** **Done — 2026-08-23.** ~~Scheduled for v3.0.0 as a release-note obligation only~~ — the obligation
is discharged and verified against `CHANGELOG.md`. **The 2.2.x patch remains `Rejected`.** Previously
`Proposed — captured during a verification pass, not yet triaged by
Purpose Refiner`, then `Scheduled` from 2026-08-16. **No work is being requested on the 2.2.x line;** this
entry exists so a real defect is
named rather than disappearing into a redesign.

**The facts were re-verified on this date rather than affirmed** — [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs)
was opened, and lines 44–62 still read `if (Context.Database.CurrentTransaction == null) { _transaction =
Context.Database.BeginTransaction(); }` with `_transaction?.Commit()` and `_transaction?.Rollback()` in the
matching members. The defect is real and unchanged.

**What the triage decided, and what it did not.** The recommendation this entry already carried — no patch,
name it in the 3.0.0 notes as `Fixed` — is **accepted**, and it is accepted by *applying an existing owner
decision rather than making a new one*: [D1](purpose-and-scope.md#owner-decisions--2026-08-15) settles that
the 2.2.x line remains installable and receives no new work, and this entry is the exception that decision
invites someone to test. It does not survive the test. A patch would reopen a line the owner has closed, in
order to reach consumers who can equally be reached by release notes that tell them what they are exposed
to. **The owner may reasonably overrule this**, since it is the one place where "no new work on 2.2.x" meets
a data-correctness bug rather than a missing feature — see [The open question](#the-open-question) below,
which is retained rather than answered away.

> **The owner has since ruled, 2026-08-16, and upheld this.**
> [D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles it: document, do not patch. The invitation
> above is **answered, not withdrawn** — read
> [The owner has now ruled](#the-owner-has-now-ruled-and-the-question-is-closed--2026-08-16) before treating
> this paragraph as an open door.

### The defect

[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) guards its transaction start on whether one is
already open:

```csharp
if (Context.Database.CurrentTransaction == null)
    _transaction = Context.Database.BeginTransaction();
```

When a transaction **is** already open, `_transaction` stays `null` — and the matching commit and rollback
both reach it through `?.`, so **both quietly do nothing**. The caller receives no exception and no return
value indicating anything was skipped. A write intended to be committed by that path is left to whatever the
outer transaction decides, and a rollback intended to reverse it does not run.

No existing test would catch it: the 35 upstream tests exercise no nested or pre-existing transaction case,
and nothing in this repository runs them — `LocalTestsOnly: 'yes'` skipped them in CI even before the six
adapters that reached them were deleted on 2026-08-16.

### Why there is nothing to schedule in code

**The heading here previously read "Why it is `Proposed` rather than `Scheduled`", which contradicted this
entry's own status line. Corrected 2026-08-16.** The entry has not been `Proposed` since it was triaged
earlier that day; it is `Scheduled` as a release-note obligation with the 2.2.x patch `Rejected`. Nothing
about the reasoning below changed — only the heading, which had survived the triage that made it false.

**3.x designs the defect out rather than fixing it.** Under `docs/api-contract.md`, Data Access Objects carry
no transaction members at all — transactions live on the DAL root, every misuse throws rather than returning
quietly, and the library never consults `CurrentTransaction` to decide whether to begin. The one surviving
escape, a *borrowed* context that already carries a foreign transaction, surfaces EF Core's own
`InvalidOperationException` from `RelationalConnection.BeginTransaction` rather than a silent skip, and the
contract carries a regression obligation for it.

So there is nothing to schedule for v3.0.0 — the code is deleted. What needs a decision is narrower:

### The open question — decided, and retained because the owner may overrule

**Decided 2026-08-16: no patch.** The heading is kept because the question is a real one and the owner is
entitled to reopen it; it is no longer *open* in the sense of blocking anything, and the status line records
the decision rather than a pending choice.

**Does the 2.2.x line get a patch?** Entry 4 says that line's continuing job is to be the EF6 answer and it
should receive no new work. That is a reasonable rule and this is a reasonable exception to test it against —
silent transaction loss is a data-correctness bug, not a missing feature. The alternative is to leave 2.2.x as
it stands and let the release notes for 3.0.0 say plainly that this class of failure is gone.

**Recommendation:** no patch. Name it in the 3.0.0 notes as `Fixed`, alongside the leaked `DbContext` in
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess). A consumer holding 2.2.x who reads that
learns what they are exposed to, which is the outcome a patch would buy without the cost of reopening a line
this repository has decided to stop developing.

#### The owner has now ruled, and the question is closed — 2026-08-16

**The recommendation above was put to the owner and accepted.** It is
[D12](purpose-and-scope.md#owner-decisions--2026-08-15), and it settles the same question for
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess) and
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) at the
same time. The reasoning is the owner's, not this file's inference: *"I don't believe anyone is currently
using that library in any meaningful capacity, so it's document the bug and recommend to update to v3.0.0."*

**Three things this closure does, and one it does not.**

- **The invitation to overrule is answered, not withdrawn.** This entry filed itself as the exception D1
  invites someone to test; D12 is the owner testing it and upholding D1. The paragraphs above are kept exactly
  as they were so the argument that *lost* stays legible.
- **It supplies the missing premise.** The earlier reasoning turned on "release notes reach the same
  consumers a patch would." D12 makes the real premise explicit: **the exposure is judged near-zero because
  there is no meaningful consumer base.** That premise is re-testable and should be re-tested if one appears.
- **It attaches the caveat this entry never carried.**
  [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a 2.2.x consumer on
  `net48`, `net8.0` or `net9.0` **cannot take 3.0.0 at all.** "Upgrade to 3.0.0" is therefore not a remedy
  available to every consumer this defect reaches, and the release note must say so —
  [The `Changelog Author` obligation](#the-changelog-author-obligation--d12).
- **It does not change this entry's status.** `Scheduled` for the release-note obligation, 2.2.x patch
  `Rejected`, exactly as triaged earlier the same day. D12 confirms that triage rather than moving it.

**Facts re-verified on 2026-08-16 before this was written**, not carried forward:
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) was reopened. `EnsureBeginTransaction` still guards
on `Context.Database.CurrentTransaction == null`, and `EnsureTransactionCommit` / `EnsureTransactionRollback`
still reach `_transaction` through `?.` and then null it. The defect is unchanged.

### Provenance

Found by `Repo Analyst` reading the shipped source during a verification pass; confirmed independently by
`Contract Reviewer` against the same file while checking that the 3.x design could not reproduce it. Neither
agent inherited the claim — both opened `RootNonIdDao.cs`.

---

## 13 — The soft-delete and keyless DAO bases cannot serve the 3.x contracts by inheritance

> **STATUS CHANGED 2026-08-23 — `Scheduled` → `Done`.** The types carrying the four violations no longer
> exist. Verified by listing `ProphetsWay.EFTools/` on this date: there is no `RootBaseSoftDao.cs` and no
> `LegacyBaseSoftNonIdDao.cs`; what stands in their place is `BaseSoftDao.cs`, `BaseSoftGetAllDao.cs`,
> `BaseSoftPagedDao.cs`, `RootSoftNonIdDao.cs`, `BaseSoftNonIdDao.cs` and the internal `SoftTimestamps.cs`.
> The structural complaint at the heart of this entry — that the soft members were `new` rather than
> `override`, so an upcast stopped soft-deleting — is answered by the rewrite: the replacements declare them
> as `override`s. The proving ground consumes them, `DepartmentDao` deriving from
> `BaseSoftPagedDao<Department,int>`.
>
> **The [D12 release-note obligation for this defect is discharged](#the-changelog-author-obligation--d12) —
> corrected 2026-08-23, later the same day.** This paragraph read *"is still owed — `CHANGELOG.md`'s v3.0.0
> entry does **not** name the `DeletedDate` wipe."* **Both clauses are false now.** The entry carries
> § *"Fixed: Update no longer un-deletes a soft-deleted row"*, which names the whole-object
> `entry.CurrentValues.SetValues(item)` replacement, says the row *"came back to life"* in every read, and adds
> that *"Nothing threw and no row count looked wrong."* It also names the second-`Delete` half — the refreshed
> timestamp and the `1` where `0` belonged — **and a third half this entry never asked for**, `Insert` carrying
> a stale `DeletedDate` into a new row so that the row is invisible to every read the moment it is written.

**Status:** ~~**Scheduled for v3.0.0**~~ **Done — 2026-08-23.** Filed and triaged 2026-08-16. Scheduled rather than `Proposed` because
it requires no scope decision: [entry 10](#10--collapse-the-guidintlong-dao-triplication) is already approved
and already rewrites these types, and this entry is the specification of *what the rewrite must change*. It
is filed separately from entry 10 because entry 10 is a **surface** change (18 classes → 6 generic families)
and this is a **semantic** one, and conflating them is precisely the mistake it exists to prevent.

**"Just inherit the existing soft base" is a trap.** That is the whole entry in one sentence. Anyone
implementing `IDepartmentDao` or `ICompanyResourceDao` in `ProphetsWay.Example.DataAccess.EF` — which is step
2 of [entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) — will reach for
`Guid/Int/Long.BaseSoftDao` and `BaseNonIdDao<T>` because they are the types that exist and the names match.
They do not satisfy the contracts, and the ways they fail are quiet.

### How this was found

By checking `RootBaseSoftDao` against `IDepartmentDao`'s 19 numbered rules one at a time, during the lap that
produced the 2026-08-16 green build. **Nothing surfaced it before because nothing runs it** — there is no
test suite here (entry 4 of the deviation list; [entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits)
in this file), `LocalTestsOnly: 'yes'` skips CI, and the `Department` members in
`ProphetsWay.Example.DataAccess.EF` currently throw before reaching any base class. It is a reading finding,
which is the only kind available in this repository right now.

Verified by opening [RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs),
[RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs), [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs),
[BaseNonIdDao.cs](../ProphetsWay.EFTools/BaseNonIdDao.cs) and
`ProphetsWay.Example/ProphetsWay.Example.DataAccess/IDaos/IDepartmentDao.cs` and `ICompanyResourceDao.cs`.

### `RootBaseSoftDao` — four rule violations

`RootBaseSoftDao<T, TIdType>` is 40 lines. Three of them are the defect.

| Rule | What it requires | What the code does |
|---|---|---|
| **3** | `Update` writes the department's own data only; the stored `CreatedDate` and `DeletedDate` are **preserved** and values carried on the incoming instance for them are **ignored** | `Update` stamps `UpdatedDate` and calls `base.Update(item)`, which reaches `RootDao.Update`: `entry.CurrentValues.SetValues(item)` — **whole-object replacement.** A caller passing an instance fetched before the delete silently wipes the stored `DeletedDate` |
| **5** | `Delete` returns `1` when a **live** department with that identifier is stored | `Delete` stamps `DeletedDate` and returns `base.Update(item)`, which returns `Context.SaveChanges()`. It never asks whether the row was live |
| **6** | `Delete` on an already-deleted department returns `0`, changes nothing, and leaves the existing `DeletedDate` **not refreshed** — soft delete is **idempotent** | The stamp is unconditional, so a second delete **overwrites the original `DeletedDate` with a later one** and returns `1`. Both halves of the rule fail |
| **1** | After `Insert`, `UpdatedDate` and `DeletedDate` are `null` **whatever the caller had assigned to them** | `Insert` stamps `CreatedDate` and calls `base.Insert(item)`. Neither field is cleared, so a caller who reuses an instance carries stale timestamps into a new row |

**Rule 3 is the one `IDepartmentDao` itself predicts.** Its `WHY` section carries a paragraph headed
*"Rule 3 is the one that gets broken"*, naming whole-object replacement as the obvious implementation and
soft delete failing "with nothing to point at." This library is that prediction, in shipped code, in the
package the contract's own reference implementation is supposed to be demonstrated against.

**Rules 5 and 6 are the more dangerous pair in practice.** A refreshed `DeletedDate` is not an exception and
not a wrong row count that anyone checks — it is a timestamp that quietly moves. Any audit, retention window
or "deleted before" query built on it is wrong, and nothing anywhere reports it.

### `BaseNonIdDao<T>` — a structural mismatch, not a bug

`BaseNonIdDao<T> : IBaseDao<T>` declares `public abstract T Get(T item)` and `public abstract int Update(T item)`.

`ICompanyResourceDao` **deliberately does not inherit `IBaseDao<T>`** and its `<remarks>` spend two headed
paragraphs — *"No `Update`"* and *"No `Get`"* — explaining why: a `CompanyResource` is nothing but its two
foreign keys, so there is nothing to update *to*, and `IBaseDao<T>.Get(T)` is defined in terms of an
identifier field the entity does not have. Rule 8 goes further and states that
`Get<CompanyResource>(object id)` on the dispatcher **can never be made to work.**

So inheriting `BaseNonIdDao<T>` forces an implementer to write two members the contract declines to declare,
and the only honest bodies for them are throws. **That is not a defect in `BaseNonIdDao<T>`** — it was written
for keyless entities that still want CRUD, which is a real shape. It is the wrong base for this contract, and
the name will suggest otherwise to everyone who meets it.

### What this constrains

- **[Entry 10](#10--collapse-the-guidintlong-dao-triplication).** The six generic families must fix these
  semantics. A collapse that closes over `RootBaseSoftDao` unchanged ships the four violations under new type
  names and spends the major version doing it. The soft-delete families need a **read-modify-write `Update`**
  that preserves the three timestamps, and a **`Delete` that checks liveness first** and returns `0` without
  stamping when the row is already deleted or absent.
- **The keyless family must not implement `IBaseDao<T>`.** `ICompanyResourceDao`'s shape — `Insert`, `Delete`,
  `GetAll`, and nothing else — needs a base that offers exactly that. `IBaseDao<T>` is described by the
  contract as "a menu, not a mandate"; the library currently offers only the mandate.
- **[Entry 1](#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) step 2.** Do not implement
  the `Department` DAO by deriving from today's soft base. If step 2 is done before entry 10, the honest route
  is a hand-written DAO satisfying the 19 rules, which then becomes the evidence for what the generic family
  must look like.

  > **The owner approved exactly this on 2026-08-16 —
  > [D13](purpose-and-scope.md#owner-decisions--2026-08-15).** A concrete Entity Framework `DepartmentDao` is
  > to be hand-written in `ProphetsWay.Example.DataAccess.EF/Daos/`, explicitly rather than leaning on a
  > generic family first, in the owner's words because *"the point of the generic tests and classes was to
  > just reduce all the duplicative code, but in this case there is a real need for it."* **This paragraph is
  > no longer a recommendation; it is the approved route.** Verified 2026-08-16: that folder holds
  > `CompanyDao.cs`, `JobDao.cs`, `ResourceDao.cs`, `TransactionDao.cs` and `UserDao.cs` — **no
  > `DepartmentDao.cs`** — so the work has not started.
  >
  > **What makes it verifiable rather than speculative:** the shape B seam landed upstream the same day
  > (`TestDataAccessFactory.Use`), so `ProphetsWay.Example`'s 164 tests can be pointed at the Entity Framework
  > Data Access Layer, and `DepartmentDaoTests` — the largest class in that suite — is written directly
  > against these 19 rules. **The seam is unverified**; see
  > [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation).
- **[Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).** Rules 5 and
  6 are exactly the kind of defect a green suite catches and a reading pass nearly misses. The upstream
  `Contract` tests for `IDepartmentDao` already exist; they have simply never run against this library.

### Severity, stated honestly

**This is a shipped defect in the 2.2.0 package, not only a forward-compatibility gap** — the same shape as
[entry 3](#3--implement-the-3x-disposal-contract-in-baseefdataaccess)'s leaked `DbContext` and
[entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)'s
silent transaction skip. A 2.2.0 consumer using `BaseSoftDao` today has non-idempotent soft delete and an
`Update` that can resurrect a deleted row.

**It does not get a 2.2.x patch**, for the reason [entry 12](#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)
gives and on the same owner decision ([D1](purpose-and-scope.md#owner-decisions--2026-08-15)): that line
receives no new work. **`Changelog Author` must record this as `Fixed` in the 3.0.0 notes, not `Changed`** —
a consumer reading "soft-delete DAO bases rewritten" learns nothing about the rows they have already stamped.

**Confirmed by owner decision, 2026-08-16 — this is now ruled rather than inferred.**
[D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles all three shipped 2.2.0 defects as
**documented, not patched**, on the owner's judgement that the package has no meaningful consumer base. Two
things follow that this entry did not previously carry:

- **The changelog obligation is stated once**, in
  [The `Changelog Author` obligation](#the-changelog-author-obligation--d12), and cited from here rather than
  duplicated. The wording above is retained because it names *what* must be said; the consolidated section
  names *all three* defects and the caveat below.
- **"Upgrade to 3.0.0" is not available to every affected consumer.**
  [D7](purpose-and-scope.md#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a 2.2.x consumer on
  `net48`, `net8.0` or `net9.0` cannot take it. For them the release note is the whole remedy. This is the
  defect of the three where that matters most — **the failure is silent and it corrupts stored data**, so a
  consumer who cannot upgrade still needs to know to stop calling `Update` on a soft-deleted row.

**Re-verified 2026-08-16 before this was written**, not carried forward:
[RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs) was reopened. `Update` still stamps
`UpdatedDate` and delegates to `base.Update(item)`; `Delete` still stamps `DeletedDate` unconditionally and
returns `base.Update(item)`; `Insert` still clears neither `UpdatedDate` nor `DeletedDate`. All four rule
violations are unchanged.

### The strongest argument against filing this separately

It could have been three sentences appended to [entry 10](#10--collapse-the-guidintlong-dao-triplication),
which already owns these types and is already approved. A fourteen-entry index is harder to read than a
thirteen-entry one, and every entry that is really a constraint on another entry dilutes the index's job.

The counter, and the reason it is filed: entry 10 is a **surface** change with a stated recommendation-reversal
history, and someone reading it for the D3 decision will not read a semantics appendix buried in it. This
finding also outlives entry 10 — it is a statement about the published package that `Changelog Author` needs,
and it binds entry 1 step 2 whether or not the collapse happens first. A constraint that binds three entries
and the changelog is not an appendix to one of them.

### Strengthened 2026-08-18 — no longer a reading finding, and the inheritance claim is now structural

Two things this entry says about itself have stopped being true, and one claim in its title got stronger.
**No status changed** — that is `Purpose Refiner`'s to change.

**1. "It is a reading finding, which is the only kind available in this repository right now" is superseded.**
That sentence was accurate when written. This repository now has a working test harness: the upstream
`ProphetsWay.Example` suite is pointed at the Entity Framework Data Access Layer through
`TestDataAccessFactory.Use`, and `dotnet test` reports **151 tests, 123 passed, 28 failed**. The
`IDepartmentDao` rules are executed rather than read.

**2. "The work has not started" is superseded.** The hand-written `DepartmentDao.cs` that
[D13](purpose-and-scope.md#owner-decisions--2026-08-15) approved **landed 2026-08-18** in
`ProphetsWay.Example.DataAccess.EF/Daos/`, together with the `ExampleContext` mapping and the eight
forwarders. All 33 `DepartmentDaoTests` and 11 of 12 `DepartmentDataAccessTests` pass; the remaining one
fails inside `UserDao`, for the reason [entry 14](#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule)
describes. **This entry's prediction held**: the DAO derives from no EFTools base, and the reasons are the
four violations above.

**3. The inheritance claim in the title is structural, not a matter of degree.** The entry argued
`RootBaseSoftDao` *does not* satisfy the contracts. An independent `Code Reviewer` pass on 2026-08-18
confirmed the violations member by member — **eight of nine**, adding rules 11, 12, 14 and 17 to the four
already listed, with `GetCount` the only clean member — and found the reason it cannot be repaired by a
derived class:

> **Every member on `RootBaseSoftDao` is declared `new`, not `virtual`.**

They cannot be overridden, only hidden again. A derived Data Access Object that re-hides them and is then
used through a `RootBaseSoftDao`-typed reference **silently gets the base behaviour back** — the four
violations return, through a reference type the consumer chose reasonably. So the title is exact rather than
rhetorical: these bases cannot serve the 3.x contracts by inheritance **at all**, and a rewrite is the only
route. That sharpens the constraint on [entry 10](#10--collapse-the-guidintlong-dao-triplication) — a collapse
that preserves the `new` declarations preserves the trap.

**What was verified, and by whom.** The `new`-not-`virtual` finding and the eight-of-nine count come from a
`Code Reviewer` pass that opened `RootBaseSoftDao.cs`, `RootBaseDao.cs`, `RootDao.cs`, `RootNonIdDao.cs` and
`Int/BaseSoftDao.cs`. The test numbers were measured. Neither is carried forward from an earlier revision of
this file.

## 14 — The Entity Framework DAO bases adopt the caller's instance, violating the SNAPSHOT rule

> **STATUS CHANGED 2026-08-23 — `Proposed` → `Done`.** It carried inside [entry 10](#10--collapse-the-guidintlong-dao-triplication)'s
> collapse exactly as **D14** anticipated, and the types that adopted the caller's instance were deleted with
> the rest. Verified two ways on this date: `ProphetsWay.EFTools/` contains an internal
> `EntityGraph.cs`, the single copy of the navigation-graph mechanics the rewrite introduced; and
> `CHANGELOG.md`'s v3.0.0 entry carries **two `Fixed:` headings that are this entry** — *"a failed Insert no
> longer leaves timestamps or an identifier on your instance"* and *"TargetException on a committed write"* —
> the second describing the inverse-navigation clearing this entry identified as the mechanism.
>
> **The release-note obligation is now fully met — corrected 2026-08-23, later the same day.** This paragraph
> read *"This is the one of the four D12 defects whose release-note obligation is **partly** met"*, and said
> the entry *"still does not say… what stops happening."* **Both are false now and neither may be restated.**
> `CHANGELOG.md` gained a third and a fourth `Fixed:` heading, one of them § *"a read or a write no longer
> hands you the store's own object"*, which names the untracked-read half this entry is really about —
> `Dataset.Add(item); Context.SaveChanges();` leaving the caller's instance tracked, and `Get` returning the
> store's object with no `AsNoTracking`. **And it carries the sentence D12 says this defect uniquely owes**, as
> a dedicated bolded paragraph headed *"What stops happening, which is the part to read even if none of the
> above sounds familiar"*: a stray edit is *"now silently dropped where it used to be silently applied"*, and
> `Update` against an absent row returns `0` where 2.2.0 threw out of `Single`, **so a `catch` a consumer wrote
> stops firing.** Of the four, this is the one written most fully — which is right, because it is the only one
> whose fix takes something away.

**Status:** ~~**Proposed**~~ **Done — 2026-08-23.** Filed 2026-08-18 by an agent under the shared-capture
rule; only `Purpose Refiner` may change this status.

**Several tests in this repository are currently green *because* of this defect.** That is the entry in one
sentence, and it is why it is filed separately from [entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)
rather than appended to it. Entry 13 is about the **soft-delete and keyless** bases. This is about the
**ordinary CRUD** ones — `RootNonIdDao`, `RootDao`, `RootBaseDao` and the `Int`/`Guid`/`Long` `BaseDao`,
`BaseGetAllDao` and `BasePagedDao` that chain to them — i.e. the path every Data Access Object in the
published package takes.

### How this was found

By implementing a *correct* Data Access Object next to the incorrect ones. `DepartmentDao` landed on
2026-08-18 satisfying the SNAPSHOT rule — it copies on `Insert` rather than adding the caller's instance —
and nine previously-passing-or-stubbed tests began failing with:

```
Cannot insert explicit value for identity column in table 'Departments'
```

**The correct implementation is what broke them.** Confirmed by an independent `Code Reviewer` pass which
opened [CompanyDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/CompanyDao.cs),
[JobDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/JobDao.cs),
[UserDao.cs](../ProphetsWay.Example.DataAccess.EF/Daos/UserDao.cs),
[Int/BasePagedDao.cs](../ProphetsWay.EFTools/Int/BasePagedDao.cs),
[Int/BaseGetAllDao.cs](../ProphetsWay.EFTools/Int/BaseGetAllDao.cs),
[Int/BaseDao.cs](../ProphetsWay.EFTools/Int/BaseDao.cs),
[RootBaseDao.cs](../ProphetsWay.EFTools/RootBaseDao.cs),
[RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) and
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs).

### The mechanism

`RootNonIdDao.Insert` is `Dataset.Add(item); Context.SaveChanges();`. `SaveChanges` transitions the entity
`Added` → `Unchanged` and **keeps it in the change tracker**. The caller's instance is now the Data Access
Layer's instance — the definition of adopting an argument, which the SNAPSHOT rule on `IExampleDataAccess`
forbids.

The adoption is then load-bearing. `UserDao.Insert` is `Dataset.Add(user)`, and EF Core's `Add` walks the
graph and paints every **untracked** reachable entity `Added` regardless of whether its key is set.
`SnapshotDeepCopyTests.InsertUserWithNavigation` inserts a `Company`, a `Job` and a `Department`, hangs all
three off a `User`, and inserts the user:

- `co` and `job` are **still tracked** from their own inserts, so the graph walk skips them and only the
  `Users` row is written. **They work because they were adopted.**
- `dept` was correctly detached by the new Data Access Object, so the walk paints it `Added` with a non-zero
  `Id` and Entity Framework emits an `INSERT` carrying an explicit value into an `IDENTITY` column.

### Three findings that widen it

**1. It is not only `Insert`.** `Int/BaseDao.Get` is
`Dataset.Where(i => i.Id == item.Id).SingleOrDefault()` — no `AsNoTracking()`, no projection. It returns the
store's own tracked instance.

**2. It escapes being a violation only because of a setting on one constructor out of three.**
`ExampleDataAccess(string)` sets `QueryTrackingBehavior.NoTracking` context-wide.
`ExampleDataAccess(DbContextOptions<ExampleContext>)` and `ExampleDataAccess(ExampleContext)` take whatever
the caller configured, and Entity Framework Core's default is `TrackAll`. **A consumer using either of the
two dependency-injection-friendly constructors gets a Data Access Layer whose `Get` hands out the store's
tracked instances, so their next `Update` flushes edits nobody submitted.** Nothing detects it: the test seam
only ever uses the connection-string constructor.

**3. `RootDao.Update` compounds it.** It is `Dataset.AsTracking().Single(...)` then
`entry.CurrentValues.SetValues(item)` — whole-object replacement, and `Single` rather than `SingleOrDefault`,
so a missing row throws `InvalidOperationException` where the ROW COUNT rule requires `0`. Entry 13 records
the same shape reached through `RootBaseSoftDao`; this is the non-soft path to it.

### A test that is green for a prohibited reason

`SnapshotDeepCopyTests.ShouldNotStoreEditsMadeToAUsersNavigationAfterInsertReturned` passes today **by luck
of ordering.** It edits `co.Name` after the insert, then asserts through a second Data Access Layer instance
with its own context — and nothing calls `SaveChanges()` on the writer's context again before the assertion,
so the dirty tracked `co` is simply never flushed. **Insert one more entity through the same Data Access
Layer between the edit and the assertion and it fails.**

This is the finding with the longest reach, because it is not a defect in this repository. It is an upstream
test that does not currently test what it is named for, and it belongs to `ProphetsWay.Example` — see
[ProphetsWay.Example/docs/feature-requests.md](../../ProphetsWay.Example/docs/feature-requests.md). A
`Test Auditor` pass over that class is the right next step, and it must happen **in that repository**.

### Why it matters beyond the failing tests

**This is a shipped defect in the 2.2.0 package**, the same shape as entries 3, 12 and 13, and it very likely
falls under [D12](purpose-and-scope.md#owner-decisions--2026-08-15) — documented, not patched — with a
`Changelog Author` obligation attached. **That is a triage judgement and is deliberately not asserted here.**

It also bears directly on this repository's stated purpose. `ProphetsWay.Example` exists to demonstrate that
the same tests pass against two Data Access Layer implementations. A green suite that is green because one
implementation quietly aliases the caller's objects is the paradigm's central claim being *reported* rather
than *proven* — and the in-memory implementation, which does not alias, is the one telling the truth.

### What it constrains

- **[Entry 10](#10--collapse-the-guidintlong-dao-triplication).** The six generic families must copy on
  `Insert` and project on `Get`. A collapse that preserves `Dataset.Add(item)` ships this under new names.
- **[Entry 7](#7--stop-forcing-a-database-provider-on-every-consumer).** Finding 2 is a second reason the
  constructor surface needs attention: the three constructors do not agree on tracking behaviour, and only
  the least dependency-injection-friendly one is safe.
- **[Entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container).** Nine of the
  currently failing tests are this defect. Certification cannot be claimed until they pass **for the right
  reason** — a fix that restores adoption would turn them green and prove nothing.

### What has deliberately not been done

No fix has been attempted. `DepartmentDao` was implemented correctly and the failures were left standing
rather than papered over by making the new Data Access Object adopt its argument like its neighbours. **That
option was available and was declined** — taking it would have turned nine tests green while spreading the
defect to a tenth Data Access Object, and it is recorded here so it is not proposed later as an obvious
simplification.

### Triaged 2026-08-19 — **Scheduled** for v3.0.0, and it is a **fourth** shipped defect

**Status: `Proposed` → `Scheduled` for v3.0.0.** No new scope decision was needed to move it, and that is the
finding rather than a formality: **every behaviour this entry asks for is already specified**, in
`docs/api-contract.md` revision 8, written before this entry was filed. What this entry contributes is the
**evidence** that the specification was necessary, the **provenance** of nine red tests, and a **fourth** name
on the [D12](purpose-and-scope.md#owner-decisions--2026-08-15) release-note obligation.

#### It needs no design — the fix is already written down

Re-verified by opening `docs/api-contract.md` on 2026-08-19, section by section, rather than taken from this
entry's own summary. Each of its three findings has a term against it:

| This entry's finding | Where it is already settled |
|---|---|
| **1 — `Insert` adopts the argument.** `Dataset.Add(item)` leaves the caller's instance tracked `Unchanged` | **A24 + OD-4**: the root is added and everything reachable is set `Unchanged` **explicitly**; *"`Dataset.Add(item)` is wrong, and so is `Attach`"*, and **the walk is specified by state, not by API**. **A26 + OD-7**: the argument and its whole reachable graph are detached **in a `finally`, on success and on failure** |
| **2 — `Get` returns the store's tracked instance, saved only by a setting on one of three constructors** | **`Get`'s `Tracking` row**: `AsNoTracking()`, **explicitly, regardless of the context's configured `QueryTrackingBehavior`**. The constructor asymmetry stops being load-bearing because the library no longer depends on the setting. **S8/A9/A10** replace the three constructors with one taking a configured context and a `ContextOwnership` |
| **3 — `RootDao.Update` uses `Single`, so an absent row throws where ROW COUNT requires `0`** | **`Update`'s `Returns` and `The forced change` rows**: `1` when the row exists, `0` when it does not, *"never greater than `1`"*, and 2.2.x's throw-or-upsert behaviour named as **not surviving**. **A22** fixes the mechanism as a tracked fetch plus `SetValues` |

**So the correct disposition is not "schedule a fix" but "confirm the fix is inside the collapse."** It is:
[D14](purpose-and-scope.md#owner-decisions--2026-08-15) requires
[entry 10](#10--collapse-the-guidintlong-dao-triplication) to carry the corrected `Insert` / `Update` / `Get`
semantics rather than to collapse first and patch after. **This entry is now the third specification of what
that collapse must fix**, alongside
[entry 13](#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) — entry 13
covers the soft-delete and keyless bases, this one covers the ordinary CRUD path, and between them they cover
every base in the package.

#### In scope for 3.0.0? — **Yes, and it is not separable from it**

Judged against the [purpose sentence](purpose-and-scope.md#settled-one-sentence-purpose). The library's job is
to supply *"the CRUD … plumbing that is identical in every DAL."* A plumbing layer that aliases the caller's
objects has not supplied working plumbing; it has supplied plumbing that works only for callers who never look
at their arguments again. There is nothing to weigh here — the alternative to fixing it is shipping 3.0.0 with
the ordinary CRUD path violating a rule the release exists to satisfy.

**It is also not separable.** The collapse deletes the types that carry the defect. Scheduling this for a later
release would mean writing the six generic families **with** the adoption behaviour and then breaking them
again in 3.1.0 — the "doing the work twice" D14 declined, at a higher price because the second pass would be
breaking rather than internal.

#### Breaking? — **Yes, against the published 2.2.0 package, and in a way a consumer can be relying on**

**All three findings are behaviour changes a 2.2.0 consumer could have built on**, and that is worth stating
plainly rather than filed as an obvious win:

- A consumer who calls `Insert(entity)` and then edits `entity` **today gets those edits written** on the next
  `SaveChanges` through any Data Access Object on the layer. After the fix they do not. Code that relies on it
  — deliberately or not — silently stops persisting.
- A consumer who calls `Get`, edits the result, and calls `SaveChanges` through some other path **today gets
  the edit written without calling `Update`**. After the fix they do not.
- `Update` on an absent row **today throws**; after the fix it returns `0`. A `try`/`catch` around it stops
  firing.

The first two are the same shape as this entry's own headline — *"several tests are currently green because of
this defect"* — read from the consumer's side. A 2.2.0 consumer's *code* can be green for the same prohibited
reason. **`Changelog Author` must say so**; "corrected snapshot semantics" does not tell that consumer their
writes have stopped.

**Riding v3.0.0 is therefore correct and cheap**: the release is already breaking on entries 2, 4, 5, 7 and 10.

#### The 2.2.x patch is **Rejected**, on D12 and without carving an exception from it

**Same disposition as entries 3, 12 and 13, on the same owner decision.**
[D12](purpose-and-scope.md#owner-decisions--2026-08-15) settles that the 2.2.x line receives no new work
because the consumer base is judged near-empty, and nothing about this defect distinguishes it from the three
D12 already covers. It is **silent** and it **corrupts stored data** — a stray edit reaching the store through
an aliased instance — which puts it in the same class as entries 12 and 13 rather than in a new one.

**But D12 named three defects and this is a fourth, so the obligation it created has to grow.** That is a
triage judgement, and this entry deliberately declined to assert it; it is asserted here, and it is flagged for
the owner rather than buried: **D12's premise is re-testable and its scope is not.** If the owner meant "these
three," a fourth row on the changelog obligation needs their word. If they meant "the shipped defects," it is
already covered. **This triage assumes the latter** and records the assumption where it can be found — see
[The `Changelog Author` obligation](#the-changelog-author-obligation--d12), which now carries four rows.

#### The one thing that is genuinely blocked, and it is not in this repository

**The upstream test that is green for a prohibited reason** —
`SnapshotDeepCopyTests.ShouldNotStoreEditsMadeToAUsersNavigationAfterInsertReturned` — is a
`ProphetsWay.Example` defect and **this triage cannot move it.** That repository's index is separate, its
numbering is unrelated, and files under `ProphetsWay.Example/` are never edited from this side.

**What is owed there:** an entry recording that the assertion passes by ordering luck rather than by the
mechanism it is named for, and a `Test Auditor` pass over that class. **It is not blocking 3.0.0** — the test
will start passing *for the right reason* once the collapse lands, and the risk it carries is the opposite
one: it would have gone on passing against an implementation that never fixed anything. Recorded here so the
obligation is not lost when this entry is read as closed.

#### What this entry must **not** be read as authorising

**A fix applied to `RootNonIdDao`, `RootDao` or the 18 key-typed classes in place.** D14 forecloses that route
explicitly. These types are deleted by entry 10; correcting them first is work thrown away, and worse, it
would turn the nine `IDENTITY_INSERT` failures green **before** the families exist — removing the only signal
currently distinguishing a correct implementation from an adopting one.

**The nine red tests are the acceptance criterion for entry 10, not a regression to be cleared first.**


## 15 — `ProphetsWay.EFTools.Guid` shadows `System.Guid` inside this assembly

> **STATUS CHANGED 2026-08-23 — `Proposed` → `Done`.** It closed for free inside
> [entry 10](#10--collapse-the-guidintlong-dao-triplication), exactly as this entry predicted, and it closed
> some days before this triage recorded it — `AGENTS.md` had already noticed and correctly declined to change
> the status itself.
>
> **What would close it**, per this entry's own last section, was *"`ProphetsWay.EFTools.Guid` no longer
> existing as a namespace."* Verified on this date by grepping the `namespace` declaration of every `.cs` file
> in `ProphetsWay.EFTools/`: **all 15 declare `namespace ProphetsWay.EFTools`, and no file declares
> `ProphetsWay.EFTools.Guid`, `.Int` or `.Long`.** The namespace cannot shadow `System.Guid` because it does
> not exist. The narrower ask — that the sub-namespaces' disappearance be treated as a *required* outcome of
> the collapse rather than an incidental one — was met: `CHANGELOG.md` v3.0.0 gives it its own heading,
> *"The Guid, Int and Long namespaces have been removed."*
>
> The stated verification — `Guid.NewGuid()` compiling unqualified inside `ProphetsWay.EFTools.Tests` — was
> **not** re-run here; no test was compiled by this pass. The namespace's absence is the stronger and more
> direct check, and it is the one this entry named first.

**Status:** ~~**Proposed**~~ **Done — 2026-08-23.** Filed 2026-08-19 by an agent under the shared-capture rule. Nothing in this index covered the ground, so it is a new entry rather than an extension of
[entry 10](#10--collapse-the-guidintlong-dao-triplication) — but it is **evidence for** entry 10 and should be
triaged alongside it rather than on its own.

### What was found, and how

Writing `AlternateKeyGuardSpikeTests.cs` in `ProphetsWay.EFTools.Tests` on 2026-08-19, `Guid.NewGuid()` **did
not compile**. The key-type sub-namespace `ProphetsWay.EFTools.Guid` — the folder holding the six `Guid`-keyed
DAO bases — is a closer match than `System.Guid` from inside a namespace rooted at `ProphetsWay.EFTools`, so
the type name resolves to the namespace. The spike worked around it with `global::System.Guid`.

**This is a compile-time discovery, not a reading finding.** It surfaced the moment the first test file was
written in this assembly, which is also why it had never surfaced before: the project had no tests.

### Why it matters more than an inconvenience

- **It taxes exactly the work this release schedules.** [Entry 6](#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits)
  rebuilds this test project, and [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
  adds certification legs. Every `Guid` a future test needs pays `global::System.` or a using alias.
- **It reaches consumers, not just this repository.** Any consumer whose own namespace is rooted under
  `ProphetsWay.EFTools` — and a consumer deriving from these bases plausibly is not — would meet it too, but
  the sharp case is anyone writing `using ProphetsWay.EFTools;` alongside `Guid` in scope.
- **The workaround is ugly in a teaching context.** `global::System.Guid` in a sample is noise that has to be
  explained.

### What the request is — and what it is not

**Not "rename the namespace" as a standalone change.** [Entry 10](#10--collapse-the-guidintlong-dao-triplication)
already deletes `ProphetsWay.EFTools.Guid`, `.Int` and `.Long` by collapsing the 18 key-specific classes into
six generic families in the root namespace. **If that collapse lands as specified, this closes with it and
costs nothing.**

What this entry asks is narrower: that the shape pass **treat the disappearance of these three sub-namespaces
as a required outcome rather than an incidental one**, and that if any key-type sub-namespace survives the
collapse for another reason, the `Guid` collision be weighed explicitly before it does.

### What would close it

`ProphetsWay.EFTools.Guid` no longer existing as a namespace, verified by `Guid.NewGuid()` compiling
unqualified in a file whose namespace is `ProphetsWay.EFTools.Tests`.

---

## 16 — Source Link, symbol packages, and the empty packaging metadata stubs

**Status:** **Proposed** — filed 2026-08-23 by `Purpose Refiner` during triage. **Not breaking, so it does not
have to ride 3.0.0.**

> **CURRENT FACT REFRESH — 2026-08-29.** The implementation described by this entry is on disk in commit
> `e2f2120` (*Complete package metadata and enable Source Link*). Reopening
> `ProphetsWay.EFTools/ProphetsWay.EFTools.csproj` confirms valued `RepositoryType`, `PackageProjectUrl`,
> `PackageTags`, `PackageReleaseNotes`, `Copyright`, and `NeutralLanguage`; `PublishRepositoryUrl`,
> `EmbedUntrackedSources`, `IncludeSymbols`, and `SymbolPackageFormat`; a `TF_BUILD`-conditioned
> `ContinuousIntegrationBuild`; and `GenerateDocumentationFile`.
>
> **The status remains `Proposed`.** Completion is a factual candidate for `Proposed` → `Done`, not authority
> to apply the transition. No quoted owner decision in the current packet authorizes it, so the historical
> missing-field analysis below is preserved and the transition is left pending.

**Filed because `AGENTS.md` deviation 5 had no entry in this index at all**, and a deviation with no feature
request is a finding with nowhere to go. It is the same fact as that row; extend both together.

### What is missing, field by field

Verified by opening [ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) on this
date and reading every element in it.

| Group | State |
| --- | --- |
| Present with values | `PackageId`, `Description`, `Authors`, `Company`, `Product`, `RepositoryUrl`, `PackageIcon`, `PackageReadmeFile`, `PackageLicenseExpression`, `PackageRequireLicenseAcceptance`, and the `ItemGroup` packing `README.md`, `CHANGELOG.md` and `profile.png` |
| **Empty self-closing stubs** | `PackageProjectUrl`, `PackageTags`, `PackageReleaseNotes`, `Copyright`, `NeutralLanguage` — plus the pipeline-owned `Version`/`AssemblyVersion`/`FileVersion`/`InformationalVersion`, which are **correctly** empty |
| **Absent entirely** | `PublishRepositoryUrl`, `EmbedUntrackedSources`, `IncludeSymbols`, `SymbolPackageFormat`, `ContinuousIntegrationBuild` |
| Wrong value | `RepositoryType` is `GitHub`; the house convention is `git` |

**An empty self-closing element is not a value.** The nuget.org listing therefore ships with no homepage link,
no search tags, no release notes and no copyright.

### Why the Source Link half is cheaper than it looks, and is already proven next door

**This is not the open question it was in `ProphetsWay.BaseDataAccess`.** That repository's
[FR 8](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md#8--source-link-and-symbol-packages) asked
whether enabling Source Link costs a `PackageReference` and **answered it: no.**
`Microsoft.SourceLink.GitHub` ships inside SDK 10.0.400, so the whole csproj half is four properties plus one
CI-conditioned fifth. It shipped there in **3.2.0** and was verified from the extracted artifacts.

**The pipeline half is already done and needs nothing here.** `prophets-pipelines` pins `nuget.exe` to
**6.4.0**, which pushes a co-located `.snupkg` alongside the `.nupkg` on a single push — read from
`prophets-pipelines/stages/deploy-release.yml` on this date, where the pin and the co-located artifact
download both carry comments recording the measurement. **So a `.snupkg` produced by this repository would
publish with no template change whatsoever.**

### What it buys, specifically here

Two things this package needs more than most:

- **This library is abstract bases.** A consumer's own DAO derives from `BaseDao<TEntity,TKey>` and every
  interesting failure — a `DataAccessConventionException` from identifier resolution, an `ApplyStableOrder`
  that throws, a `MatchRow` that did not translate — surfaces *inside* a frame the consumer cannot step into.
- **`PackageTags` is empty**, so nothing about this package is discoverable by search on nuget.org. It is a
  new major on a package with a near-empty consumer base; discoverability is not a rounding error.

**The `PackageTags` and `Description` wording is also where [D8](purpose-and-scope.md#owner-decisions--2026-08-15)
lands** — the certified-on-SQLite-and-SQL-Server claim has to be *on the package*, and today the package says
nothing. Note the ordering constraint: **D8's wording should not be published ahead of
[entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)'s evidence**, which
is the one genuine reason to let this slip past 3.0.0 rather than the only one.

### Owner decision needed

**Does this ride 3.0.0 or follow it as 3.0.1 / 3.1.0?** Nothing here is breaking and nothing here blocks a
publish, so `Purpose Refiner` proposes **following it** — but the argument the other way is real: packaging
metadata is what a *new* major's listing is judged on, and 3.0.0 is the listing most consumers will meet
first. `Modernizer` owns the csproj either way.

---

## 17 — Five proving-ground DAOs carry dead preprocessor guards

**Status:** **Proposed** — filed 2026-08-23 by `Purpose Refiner` during triage. Cosmetic in effect, and filed
anyway for the reason in the last paragraph.

### What was found

Grepping `ProphetsWay.Example.DataAccess.EF/` for `#if`, `#else` and `#endif` on this date returns **20 matches
in 5 files** — `Daos/CompanyDao.cs`, `Daos/JobDao.cs`, `Daos/ResourceDao.cs`, `Daos/TransactionDao.cs` and
`Daos/UserDao.cs`. Each opens with the same four-directive preamble over its `using` block:

```csharp
#if NET8_0_OR_GREATER
...
#endif
#if NET471 || NET48
...
#endif
```

`ProphetsWay.Example.DataAccess.EF.csproj` reads `<TargetFrameworks>net10.0</TargetFrameworks>`, so
**`NET8_0_OR_GREATER` is unconditionally true and `NET471 || NET48` is unconditionally false.** Both arms are
decided at every build; neither can ever change while D7 stands. `DepartmentDao.cs` and
`CompanyResourceDao.cs` — the two written after the retarget — carry no directives at all, which is the shape
the other five should match.

### Why it is filed rather than shrugged off

**Nothing here ships.** This project is not packaged, the guards emit no warning, and a consumer never sees
them. On effect alone this is beneath the bar.

It is filed because of **where** it is. `ProphetsWay.Example.DataAccess.EF` exists to be read and copied by
someone writing their own EF Data Access Layer against these bases, and
[entry 4](#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) closed on the fact that the
**library** now has zero preprocessor directives. A reader who opens the reference implementation first meets
five files implying that multi-targeting across .NET Framework is still a thing this paradigm does. That is a
teaching cost, and it is the one kind of cost this family of repositories consistently agrees to pay for.

**Deliberately not proposed: deleting the files or rewriting the DAOs.** The change is the removal of four
directive lines per file and nothing else. `Modernizer` or `Refactorer` owns it; **it must not be bundled with
a behaviour change**, or the diff stops being reviewable at a glance, which is the whole reason it is cheap.

---

## 18 — Committed `.trx` files under `TestResults/` are read as current and are not

**Status:** **Proposed** — filed 2026-08-23 by `Purpose Refiner` during triage. **Filed against a live hazard
this pass hit directly**, not against a hypothetical one.

### What happened

`ProphetsWay.EFTools.Tests/TestResults/` holds **seven** committed run artifacts: `baseline.trx`,
`efseam.trx`, `efseam2.trx`, `efseam3.trx`, `eftools-verify-20260823.trx`, `final.trx` and `lap2.trx`.
Their names carry no ordering and only one carries a date.

The newest, `eftools-verify-20260823.trx`, was opened on this date. Its `ResultSummary` reads
**`total="270" executed="270" passed="259" failed="11"`**, and its results are stamped
`2026-08-23T16:09–16:10-04:00`. That looks authoritative and current: right date, right total, plausible
detail — ten `EFSnapshotDeepCopyTests` failures, all `NullReferenceException` at
`SnapshotDeepCopyTests.cs:line 504`, plus `EFDataAccessTransactionTests.ShouldExposeUncommittedWritesToAnotherInstance`
dying after **30.02 seconds** on a lock wait.

**It is stale.** The submodule pointer this repository now carries, `f93f0a4`, was authored at roughly
**18:53** the same day — nearly three hours *after* that run — and it is the commit that both rewrote
`SnapshotDeepCopyTests.cs` and introduced the `StoreCapabilities` declaration that removes the 30-second lock
wait. The artifact therefore records the state of a **superseded** submodule, and every one of its 11 failures
is against code no longer in the tree.

### Why this is worth an entry

**This index and `AGENTS.md` both cite test counts as evidence**, and a reader reconciling a document against
`TestResults/` will reconcile it against the wrong run. That is not a theoretical failure mode — the
2026-08-18 artifacts here (147/53/94, 147/15/132, 151/57/94) had already been mistaken for current once, which
is why `AGENTS.md` carries a sentence warning about them. A warning in one file about artifacts in another is
the weakest possible control.

### Three options, none of them chosen here

| Option | Effect | Cost |
| --- | --- | --- |
| **Gitignore `TestResults/`** | The hazard cannot recur | Loses the ability to point at a run in review, which is how several claims in this index were once evidenced |
| **Keep one, named for its commit** | Retains the evidence, makes staleness visible | Requires discipline at exactly the moment nobody has any — the end of a long session |
| **Keep them, add a `README.md` in the folder** | Cheapest; nothing is lost | A note beside stale data is still stale data with a note beside it |

**`Purpose Refiner` recommends the first and does not act on it.** The house rule is to *genericize rather than
delete* where a hygiene fix has a teaching cost — and here there is no teaching cost, because a `.trx` teaches
nothing a `CHANGELOG.md` or this index does not say better. The counter-argument, recorded so it is not
re-proposed as unfinished work: several claims in this file and in `AGENTS.md` were originally evidenced by
pointing at a `.trx`, and ignoring the folder removes that option permanently.

**Whatever is decided, the immediate correction is not optional: `eftools-verify-20260823.trx` must not be read
as the current state of this suite.** The owner reports the suite at **270 / 270 / 0** after `f93f0a4`;
**that figure was not re-measured by this pass** — no test was run — and it is recorded here as the owner's
measurement, corroborated only by the fact that `f93f0a4` touches precisely the two files carrying all 11
failures.

---

## 19 — Certify the contract suite against Azure SQL

**Status:** 🔴 **Scheduled for v3.0.0, and RELEASE-BLOCKING.** Filed 2026-08-24 by `Purpose Refiner` on the
owner's decision, recorded as [D22](purpose-and-scope.md#owner-decisions--2026-08-15). **Filed and scheduled
in the same pass** — an entry normally opens as `Proposed`, and this one does not, because the owner scheduled
it in the act of asking for it.

> **The decision, verbatim:**
>
> > *"yes, option 1 please, mssql server and azure sql both need to be covered, i don't have an azure sql
> > instance setup just yet, but if we get bicep, we shoudl be able to deploy an instance from cli and us that
> > to test?"*
>
> `option 1` is read **only** as the choice it answered — **"Azure SQL is a certified target before 3.0.0."**
> It is not read as approval of any infrastructure, any tool, any repository layout, or any spend.

### What this entry is

**One complete certification run against real Azure SQL, green under the approved 2026-09-07 inventory
split.** It retains every applicable DAO, constraint, isolation, disposal, and transaction contract case.
Physical database create/drop cases remain required by local Gate 1 but are explicitly excluded from Azure;
their exact inventory must be closed and reported separately, with no broad exemption, silent pass, or
unexpected skip. The fixed Example database and the minimum explicitly configured reusable scratch databases
are reset under controlled ownership; that exact minimum must be derived and explained before provisioning.
Shared stores are exclusive, simultaneously isolated stores remain available where required, and reset
failures fail the run.

> **The 2026-08-24 instruction that every discovered case execute on Azure under Gate 1's same five criteria
> is superseded only by the [D-035 physical-lifecycle split](decision-log.md#d-035-split-azure-certification-from-physical-database-lifecycle-checks).** The existing 14-case
> `AlternateKeyGuardSpikeTests` provider-comparison exception remains separately accounted as a provider
> selection exception; it is not part of the physical-lifecycle exclusion. The concrete exclusion inventory
> and its measured accounting are linked in the
> [execution record](azure-sql-test-execution.md#filtered-trial-result-2026-09-08), not invented here. **Nothing about
> [D22](purpose-and-scope.md#owner-decisions--2026-08-15)'s release gate changes.**

**`ProphetsWay.EFTools` 3.0.0 stays untagged and unpublished until both gates are met.** Neither implies the
other, and the [SQLite whole-suite leg](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
still owed under D2/D4 is **not a substitute for either**.

### Why it is a separate entry rather than a widening of entry 11

**Because the failure it can catch is one local SQL Server structurally cannot.** Azure SQL disconnects as a
matter of routine operation, so EF Core's `EnableRetryOnFailure` execution strategy is effectively mandatory
against it — and **an execution strategy is incompatible with a user-initiated transaction unless that
transaction is wrapped in `ExecuteInTransaction`**. `BaseEFDataAccess<TContext>` exposes `TransactionStart`,
`TransactionCommit` and `TransactionRollBack` as first-class members of the contract this package advertises,
so the incompatibility lands **on this library's public surface**, not on the consumer's code.

**If Gate 2 exposes that, it is a finding for the owner and not a contract change an agent takes.** The
possible answers — document the constraint, wrap internally, or change the transaction contract — differ in
severity from a doc note to a breaking change, and only the owner picks. Recording the possibility here is
what stops it being discovered at the moment of the run and patched in a hurry.

The secondary reason is practical: entry 11 needs a connection string and a schema. This entry needed a
dedicated cloud fixture in addition to those inputs. D-034 records that fixture as deployed. The
[filtered-trial record](azure-sql-test-execution.md#filtered-trial-result-2026-09-08) now supplies test
evidence and owner-reported DACPAC seed context; formal Gate 2 review remains open.

### Current state — filtered trial evidence awaits owner review

- [D-034](decision-log.md#d-034-record-the-owner-deployed-group-admin-fixture-and-defer-database-use)
  records an owner-deployed `westus` fixture: a provisioned resource group, a ready SQL server with public
  network access, the Online Basic 5 DTU / 2 GiB `ProphetsWay.Example` database, and exactly one firewall
  rule.
- The owner-authored `infra/example.solution.bicep` and `infra/group.bicep` use a dedicated
  security-enabled, non-mail-enabled Microsoft Entra Group as the Entra-only SQL administrator.
- Exact subscription, tenant, object, membership, and address values are not copied into this document.
- **2026-09-08 filtered trial:** the owner-run `net10.0` result is **369 passed / 0 failed / 0 skipped**.
  The parent verified retained TRX byte equality and all 369 unique linked executions/definitions passing;
  all 14 comparison cases are included, the six physical-lifecycle cases are absent, and the exact-six
  guard passed. Prior retained local physical **6/6** evidence is separate, not added to this count.
- The owner reported a **99.4-second** trial, successful build in **103.5 seconds**, Example DACPAC
  deployment with synthetic seed data, and use of the two-scratch-database route. Azure targeting and
  authentication are owner context, not independently inspected live state. No automatic DACPAC
  republication follows from this result.
- The [canonical execution section](azure-sql-test-execution.md#filtered-trial-result-2026-09-08) is the
  portable result reference. These dated observations are evidence for the owner's planned 2026-09-09
  review, not new count requirements or Gate 2 closure. **FR 19 remains Scheduled and release-blocking.**

The owner's earlier proposal in question form — *"if we get bicep, we shoudl be able to deploy an instance
from cli and us that to test?"* — is preserved as D22 history. D-033 later established the owner-manual
route, and D-034 records the completed deployment milestone without closing this request.

### Operational route and remaining work

| Step | Current state |
| --- | --- |
| **Author the Bicep** | **Complete.** The owner-authored templates live under `infra/` |
| **Deploy the infrastructure** | **Complete.** The owner manually deployed and the live fixture was verified read-only for D-034 |
| **Apply the DACPAC** | Owner-reported deployed with synthetic seed data in the [2026-09-08 result](azure-sql-test-execution.md#filtered-trial-result-2026-09-08); no automatic republication is pending |
| **Configure the connection and authentication** | Owner execution context for the filtered trial; endpoint and authentication are not independently established by the TRX |
| **Run the approved Azure SQL certification inventory** | Filtered-trial evidence is available for owner review; physical exclusions and prior local checks are separate. Formal Gate 2 closure remains open |
| **Tear the infrastructure down** | **Deferred.** The fixture is persistent for now; any future teardown remains an owner action under D-033 |
| Change any `.yml`, bump a version, tag, or publish | **Not part of this request.** No such operation is authorized here |

Deployment is no longer a pending gate. Any further DACPAC publication or future teardown remains a
separately governed, human-executed operation under D-033; no live cleanup is claimed or authorized here.

### Deployment choices resolved; certification review remains

[D-034](decision-log.md#d-034-record-the-owner-deployed-group-admin-fixture-and-defer-database-use)
records the selected deployment shape without repeating protected values:

| Area | Current state |
| --- | --- |
| **Placement and naming** | Deployed into a dedicated resource group in `westus`; the server and database names are selected. Exact subscription, tenant, and live identifiers are not repeated here |
| **Administrator** | Dedicated security-enabled, non-mail-enabled Microsoft Entra Group; Entra-only SQL administration |
| **Network** | Public network access with exactly one firewall rule; the address is not recorded here |
| **Database capacity** | `ProphetsWay.Example` is Online on the Basic tier at 5 DTU with a 2 GiB maximum size |
| **Infrastructure home and retention** | Owner-authored Bicep under `infra/`; the fixture remains persistent for now |
| **Still required** | Owner review of the [filtered-trial evidence](azure-sql-test-execution.md#filtered-trial-result-2026-09-08) and formal Gate 2 closure; unresolved private Bicep defaults' publication policy; any future teardown decision |

The earlier eleven-decision block is closed by the owner-selected deployment. The remaining blockers begin
at certification review and unresolved publication policy, not an unperformed filtered trial.

### Sequencing — Gate 1 first, and this is not a preference

| | Gate 1 — [entry 11](#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | Gate 2 — this entry |
| --- | --- | --- |
| **Blocked on** | **Satisfied 2026-08-29** | Owner review of the dated filtered-trial evidence and formal Gate 2 closure; the TRX does not independently establish endpoint or authentication |
| **The work** | **Complete:** provider-selectable test wiring and an externally selected, unfiltered local run at **328 / 14 exempt / 314 honouring / 0 failed** | **Deployment complete:** owner-authored Bicep → owner-manual deployment. **Remaining:** owner review of the [filtered trial](azure-sql-test-execution.md#filtered-trial-result-2026-09-08), with separate retained local physical checks and owner-reported DACPAC seed state |
| **Irreversible steps** | None | Deployment is complete; any further DACPAC publication and future teardown remain separately governed under D-033 |

**Its input is a suite already proven provider-selectable.** Attempting Gate 2 first means debugging test
wiring and cloud infrastructure simultaneously, against a billed resource, with no local baseline to attribute
a failure to. That is the argument; it is not a claim that Gate 2 matters less. **It matters more** — Azure
SQL is where the code actually runs in production.

### The counter-argument, recorded so it is not lost

**The historical counter-argument was that gating a tag on a fixture awaiting schema and test configuration
could delay 3.0.0 indefinitely**, while the package met the other measures then recorded in this index. A defensible
alternative was available: certify local
SQL Server, publish 3.0.0, and certify Azure SQL into a 3.0.1 or 3.1.0 with the wording updated then. **The
owner considered the choice and took the stricter one.** It is recorded here rather than argued, because the
bar is a fitness-for-purpose bar and the owner is the one person who can set it.

### Scope boundary

**In scope:** the explicit Azure inclusion/exclusion inventory, test wiring, reusable scratch-store
configuration, controlled reset behavior, and certification evidence needed to execute the approved Azure
SQL scope. Physical database create/drop behavior remains a local obligation and is not exercised against
Azure. Scoped builds, offline/in-memory checks, and focused localhost integration against test-owned
disposable resources are approved. **Out of scope:** production-library behavior changes, weakened
assertions, unrelated refactoring, live Azure operations, commits, releases, the `.yml` (`Pipeline Engineer`
owns it, `LocalTestsOnly` stays `Deferred`), the package wording (`Modernizer` and `README Author`, and it
must not precede the evidence), and any change to the transaction contract, which would be a fresh owner
decision rather than a consequence of this entry.

---

## 20 — Decide the localhost SQL Server certificate-validation boundary

**Status:** **Proposed** — filed 2026-08-29 from the Security Reviewer Low finding. The complete index was
searched before filing; no existing request covered certificate validation for the local test harness.
Anyone may append a `Proposed` request, and no owner decision or implementation is inferred here.

### Scope verdict — the local test harness accepting an unvalidated SQL Server certificate

| | |
| --- | --- |
| **Verdict** | **In scope for this repository's test infrastructure; out of scope for the shipped library** |
| **Purpose it's measured against** | The settled purpose includes certification evidence on relational providers; the harness that produces that evidence must state its trust boundary honestly |
| **Because** | `ProphetsWay.EFTools.Tests/TestStore.cs` configures only `localhost`, uses Windows Integrated Security, and sets `TrustServerCertificate = true`. That weakens endpoint authentication for local tests but does not enter the non-packable runtime package or choose a provider for consumers |
| **If it proceeds** | Change only the local test connection after the local SQL Server certificate configuration is known; do not import this setting into production, Azure SQL, package metadata, or consumer guidance |

### The finding, re-verified

`ProphetsWay.EFTools.Tests/TestStore.cs` builds the SQL Server connection with `DataSource = "localhost"`,
`IntegratedSecurity = true`, and `TrustServerCertificate = true`. The test project sets
`<IsPackable>false</IsPackable>`, and the published library has no reference to `TestStore`. No password,
token, connection secret, production endpoint, or shipped runtime path is involved.

The Security Reviewer therefore classified this **Low**: an actor able to interfere with the local SQL
Server transport can present a certificate the test connection does not validate, while Windows identity
authentication still applies. The finding did not block Gate 1 and does not invalidate its completed result.

### The unresolved decision

Two outcomes remain legitimate, and the local certificate configuration decides between them:

| Option | Condition | Result |
| --- | --- | --- |
| Require validation | The local SQL Server presents a certificate trusted by the machine running the suite | Set `TrustServerCertificate` to `false` and keep the normal platform trust check |
| Retain a bounded exception | Local certification cannot presently supply a trusted certificate | Keep the exception explicitly localhost-only, test-only, and forbidden for Azure SQL, shared servers, production, or copied consumer configuration; record the boundary and revisit trigger |

**No option is selected.** The owner has not decided whether the local-only exception should be retained or
removed, and the answer cannot be inferred without inspecting local certificate configuration. Route any
implementation to `Test Harness Engineer v2` only after that decision.
[Entry 19](#19--certify-the-contract-suite-against-azure-sql) has its own connection and infrastructure
decisions; this request neither widens nor satisfies Gate 2.
