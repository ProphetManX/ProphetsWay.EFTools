# Purpose & Scope — ProphetsWay.EFTools

This document settles **what this library is for** and **what does not belong in it**, so that the
modernization work queued behind it has a bar to be measured against. It is a purpose pass, not a
modernization plan. No `.cs`, `.csproj`, `.sln`, `.yml` or version file was changed to produce it.

Every factual claim below was verified by opening the artifact named. Where a claim is inherited from
`AGENTS.md`, the README, or a sibling repository's documents and turned out to be **stale**, it is
corrected here and flagged in [Stale Inherited Claims](#stale-inherited-claims) rather than repeated.

The companion decision index is [feature-requests.md](feature-requests.md). This document cites it by
number and does not duplicate its reasoning.

**Status: Stage 1 closed.** Every checkpoint question this document was written to pose has been answered
by the owner. Their answers are recorded verbatim in [Owner Decisions](#owner-decisions--2026-08-15) and
applied throughout. **Q1 and Q4 — the two that needed the owner because they change what the family and
the package *claim* — are now closed by [D7](#owner-decisions--2026-08-15) and
[D8](#owner-decisions--2026-08-15).** The two questions still open, Q2 and Q3, are answerable by whoever
implements v3.0.0 and do not block the release being planned.

> ## Currency refresh — 2026-08-23. **The plan this document scoped has been executed.**
>
> **Eleven feature requests closed on this date and one more later the same day, and this document was written
> while all of them were still ahead.** Read it accordingly: **the analysis is sound and the tense is wrong**
> in places. Every scope verdict below was tested against what shipped and **none of them failed** — which is
> the useful thing this refresh reports, and the reason the document is amended rather than rewritten.
>
> **What is now true, each verified on this date by opening the artifact named and not by inheriting it:**
>
> | Claim | Verified against | Value |
> | --- | --- | --- |
> | The submodule pointer | `.git/modules/ProphetsWay.Example/HEAD` | **`f93f0a4`** — `f93f0a41a76834647962ddf9e830e01e24e05f24`. **Every earlier SHA in this document — `d845863`, `61d9e7d` — is superseded** |
> | The contracts reference | both `.csproj` files | **`ProphetsWay.BaseDataAccess` 3.2.0**, not 3.1.0 and not 2.5.0 |
> | Target frameworks | all three `.csproj` files | **`net10.0`** alone — the D7 destination, reached |
> | Provider neutrality | `ProphetsWay.EFTools.csproj` | `Microsoft.EntityFrameworkCore` 10.0.11 and `ProphetsWay.BaseDataAccess` 3.2.0, **and nothing else.** `.SqlServer` and `.InMemory` are gone |
>
> **The one item that ever blocked publishing is discharged.** The
> [D12 release-note obligation](#the-obligation-this-creates-on-changelog-author) was met by `Changelog Author`
> and verified by reopening `CHANGELOG.md`; see that section, which is the only place the check is recorded.
>
> **The owner's near-term question, answered on this date:** *nothing needs to be built within a week.* The
> D12 release note was the sole time-critical item in the family and it has landed.
>
> > ⛔ **The rest of this paragraph was superseded on 2026-08-24 and is struck rather than deleted.** The
> > owner stated the library's *purpose* for the first time, and it turned FR 11 from deferrable evidence
> > into the release gate. See [The SQL Server Certification Gate](#the-sql-server-certification-gate--settled-d20--d21).
>
> ~~What stands between this
> package and nuget.org is **release mechanics — the D11 ordering, the tag, the push** — not work items.
> [FR 11](feature-requests.md) (provider certification), 16 (Source Link and packaging), 17 (dead directives in
> the proving ground) and 18 (stale `.trx` artifacts) are all open, all non-breaking, and **none blocks a
> publish.**~~
>
> **Corrected 2026-08-24:** what stands between this package and nuget.org is **one work item and then
> release mechanics**. [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
> is **release-blocking** under [D20](#owner-decisions--2026-08-15) — a whole-suite SQL Server run must exist
> before the 3.0.0 tag. **16, 17 and 18 remain open, non-breaking, and non-blocking**, exactly as this refresh
> said. The refresh's error was not a missed fact; it was that a correctness bar and a fitness-for-purpose bar
> are different bars, and only the owner could supply the second.
>
> > 🔴 **Corrected again, later on 2026-08-24: it is TWO work items, not one.** The owner closed
> > [Q5](#unresolved-purpose-level-questions) as [D22](#owner-decisions--2026-08-15) — **Azure SQL is a
> > certified leg of 3.x and a second release gate**, filed as
> > [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql). Read "one work item" above
> > as the state of the record for a few hours on 2026-08-24 and nothing more. **16, 17 and 18 are still not
> > blockers** — that half is unchanged.
>
> **The purpose sentence, the drift analysis, the cohesion map and the extraction verdict are unchanged.**
> Drifts 2, 3 and 4 — the EF6/EF Core fork, the forced provider, and the library not implementing the contract
> it advertises — are **closed by what shipped**, and they are the clearest evidence the scope calls were right:
> each was named here before the work started and each was fixed as named.

---

## Settled One-Sentence Purpose

> **`ProphetsWay.EFTools` supplies the Entity Framework Core half of a `ProphetsWay.BaseDataAccess` Data
> Access Layer — the CRUD, paging, soft-delete and transaction plumbing that is identical in every DAL —
> against whichever relational provider the consumer configures, so an implementer writes only the queries
> that are specific to their application.**

Five things that sentence does deliberately:

- **Names the parent contract.** This library is not independently meaningful. Every public type in it is
  either an implementation of a `ProphetsWay.BaseDataAccess` interface or a base class whose generic
  constraints are `BaseDataAccess` entity markers. A reader who does not already know that repository
  cannot use this one.
- **Names the boilerplate, not the technology.** The origin of the whole paradigm — carried over from the
  owner's earlier iBatisTools approach — is the observation that DAL CRUD is roughly 99% identical across
  entities. "Supply the 99% once, leave the 1% as custom DAO methods" is the *product*. Entity Framework
  is the mechanism.
- **Says EF Core, not "EntityFramework."** The shipped `<Description>` is ambiguous between EF6 and EF
  Core, and that ambiguity is not cosmetic — the two branches of this library have different public
  surfaces and different semantics. The owner has settled 3.x as **EF Core-only**; see
  [The EF6 Question](#the-ef6-question--settled).
- **Says "whichever relational provider the consumer configures."** Provider neutrality is now part of the
  purpose, not a nice-to-have. The library depends on `Microsoft.EntityFrameworkCore` and names no
  provider; the consumer supplies one through `DbContextOptions`. Neutrality is **bounded to relational
  providers**, and only **SQLite and SQL Server are certified** by this repository's tests — a distinction
  [D8](#owner-decisions--2026-08-15) requires the *public* wording to carry, not just this document. See
  [Provider Neutrality](#provider-neutrality--settled) and [Public Wording](#public-wording--settled).
- **Puts the boundary in the sentence.** "Only the queries that are specific to their application" is the
  out-of-scope test. If a proposed feature is something a consumer's own DAO could express as a custom
  method, it does not belong here.

### Audience

Two audiences, weighted equally, and that equality is itself a scope constraint:

| Audience | What it implies |
|---|---|
| The owner's own projects (`ProphetsWay.BPA` and others) | These are the **proving grounds**. A feature that has never been exercised by a real DAL has not earned publication. |
| General NuGet consumers | Nothing may assume the owner's conventions, schema, database, or provider. Anything shaped around one application's needs belongs in that application. |

The second audience is the one that most of the [Out of Scope](#out-of-scope-and-where-it-should-live-instead)
findings protect. A published package that quietly assumes SQL Server is not serving it.

---

## Owner Decisions — 2026-08-15

These are the owner's, not this agent's. They close the checkpoint questions this document was written to
pose. Every status change in [feature-requests.md](feature-requests.md) traces to one of them.

| # | Decision | Closes |
|---|---|---|
| **D1** | **3.x is Entity Framework Core-only.** The published **2.2.x** line remains available and installable as the legacy EF6 / .NET Framework answer. **No EF6 companion package will be built** — that option is rejected, not deferred. | [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) |
| **D2** | **3.x is relational-provider-neutral.** The consumer configures their provider through `DbContextOptions`. **SQL Server, PostgreSQL, MySQL/MariaDB, SQLite and Oracle are conceptually in scope** as relational providers. **Only SQLite and SQL Server will be certified** by this repository's tests. **Cosmos and other non-relational providers are out of scope.** | [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer) |
| **D3** | **Collapse the 18 key-specific public DAO classes into six generic root-namespace DAO families** — `BaseDao<TEntity, TKey>` and its five siblings. The public breaking change is **accepted** and lands in the same major. **No compatibility wrappers**, unless implementation evidence later forces reconsideration. | [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) — **this reverses the agent recommendation** |
| **D4** | **Test strategy is two-legged:** SQLite in-memory as the fast CI contract/query gate, and a SQL Server container for provider fidelity. Blanket `LocalTestsOnly: 'yes'` is to be retired eventually; **the pipeline work itself is separately owned.** | [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **D5** | **`docs/architecture.md` and per-project `docs/requirements.md` are `n/a`** for this library. This document plus `AGENTS.md` and the README are sufficient. | Confirms the house convention; ratifies the [Stale Inherited Claims](#stale-inherited-claims) row |
| **D6** | The **BaseDataAccess 3.1.0 upgrade, the `ProphetsWay.Example` submodule advance to 3.1.0, disposal/transaction compliance, provider decoupling, `FluentAssertions` removal and the malformed `.gitmodules` cleanup** are all intended for the 3.x cycle. | [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts), [2](feature-requests.md#2--move-the-prophetswaybasedataaccess-reference-from-250-to-310), [3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess), [7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), [8](feature-requests.md#8--remove-fluentassertions-from-prophetswayexampledataaccessef), [9](feature-requests.md#9--delete-the-stray-submodule-submod-block-from-gitmodules) |
| **D7** | **3.x targets `net10.0` only** — the library, `ProphetsWay.EFTools.Tests`, and `ProphetsWay.Example.DataAccess.EF` alike. This is a **ratified exception** to the family's published-library TFM rule, not drift: **EF Core 10 exposes only a `net10.0` asset**, and this package's public surface **is an EF Core implementation, not a portable contract**, so the reach floor buys nothing it can deliver. **Existing `net4x` / `net8.0` / `net9.0` consumers stay on `ProphetsWay.EFTools` 2.2.x.** | **Closes [Q1](#unresolved-purpose-level-questions)**; [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework), [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard) |
| **D8** | **The certification scope is stated publicly, not only in these docs.** Public wording says EFTools is **designed for relational EF Core providers** and **certified and tested by this repository on SQLite and SQL Server**. It **must not imply that any other relational provider is certified**. | **Closes [Q4](#unresolved-purpose-level-questions)**; [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **D9** | The **obsolete local modification inside the pinned `ProphetsWay.Example` submodule was approved for discard and has been discarded.** The submodule working tree is clean; nothing local stands between the repository and advancing the pointer. | [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) |
| **D10** *(2026-08-16)* | **The Entity Framework conformance suite is reached through an upstream seam — shape B — not through a duplicate local suite.** Shape A is **declined**: a second copy of the assertions inside this repository ends the demonstration `ProphetsWay.Example` exists to provide. **The commitment is to the direction only.** Nobody has yet attempted to satisfy the 3.1.0 contracts in Entity Framework, so the seam's requirements are unknown; **its design is deliberately deferred until Lap 1 has shown what it must carry.** The six adapter classes in `ProphetsWay.EFTools.Tests` are to be **deleted, not rebuilt.** | **Closes the fork opened by the 2026-08-16 rescope of** [FR 6](feature-requests.md#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits); depends on [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation) |
| **D11** *(2026-08-16)* | **Release ordering, so each repository is built, pull-requested, merged and published exactly once.** (1) All remaining EFTools 3.x work is done **against the live submodule working tree**, not against a tagged `ProphetsWay.Example`. (2) `ProphetsWay.Example` is **tagged and released only once EFTools' Entity Framework implementation is green against it.** (3) EFTools' submodule pointer is then advanced **to that tag.** (4) `ProphetsWay.EFTools` 3.0.0 is pull-requested, merged and published. `ProphetsWay.BaseDataAccess` **3.1.0 is already published and current** and needs no release in this sequence unless something new is found in it. | [Release Ordering](#release-ordering--settled-d11); binds [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts), [FR 6](feature-requests.md#6--rebuild-prophetswayeftoolstests-on-the-3x-factory-and-scope-traits), [ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md#5--advance-the-eftools-submodule-pointer-onto-the-3x-contracts) and [FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation) |
| **D12** *(2026-08-16)* | **The three shipped 2.2.0 defects are documented, not patched.** In the owner's words: *"I don't believe anyone is currently using that library in any meaningful capacity, so it's document the bug and recommend to update to v3.0.0."* **No 2.2.1 patch will be cut.** This **upholds [D1](#owner-decisions--2026-08-15)** rather than carving an exception from it, and puts the reasoning on record: the exposure is judged near-zero because the package has no meaningful consumer base. **The consequence is not softened** — [D7](#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a consumer on `net48`, `net8.0` or `net9.0` **cannot take 3.0.0 at all**, and "upgrade to 3.0.0" is therefore not available to every 2.2.x consumer. | [The Three Shipped 2.2.0 Defects](#the-three-shipped-220-defects--settled-d12); closes the 2.2.x-patch question in [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess), [FR 12](feature-requests.md#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) and [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) |
| **D13** *(2026-08-16)* | **A hand-written concrete Entity Framework `DepartmentDao` is approved** in `ProphetsWay.Example.DataAccess.EF/Daos/`, explicitly rather than leaning on a generic DAO family first. In the owner's words: *"the point of the generic tests and classes was to just reduce all the duplicative code, but in this case there is a real need for it."* **The generic families of [D3](#owner-decisions--2026-08-15) are to be derived from a concrete implementation proven against `IDepartmentDao`'s 19 rules, not designed ahead of one.** | [The Hand-Written `DepartmentDao`](#the-hand-written-departmentdao--settled-d13); constrains [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) and [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance); resolves step 2 of [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) |
| **D14** *(2026-08-18)* · **✅ Ratified 2026-08-19** | **Collapse first — do not fix-then-collapse.** [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication)'s Guid/Int/Long → six generic families collapse **proceeds now**, carrying the corrected `Insert` / `Update` / `Get` semantics **inside** it. The alternative — patch `RootNonIdDao.Insert`, `RootDao.Update` and `Int.BaseDao.Get` in place first and collapse afterwards — was considered and **declined as doing the work twice**. [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) already requires the collapse to carry these semantics, or it ships the same violations under new type names. | **Closes Blocking Q9** (the scope of the adoption fix) in favour of route (b); binds [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication), [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance), [FR 14](feature-requests.md#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) |
| **D15** *(2026-08-18)* · **✅ Ratified 2026-08-19** | **The six generic families are modelled on the hand-written `DepartmentDao`.** This is [D13](#owner-decisions--2026-08-15) executed rather than extended: the families are derived *from* the concrete DAO, not designed ahead of it. **The precondition D13 named was satisfied on 2026-08-18**, when `DepartmentDao` landed and passed 33 of 33 `DepartmentDaoTests`. | Executes [D13](#owner-decisions--2026-08-15); binds [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) |
| **D16** *(2026-08-18)* · **✅ Ratified 2026-08-19, with one clause added** | **`DepartmentDao` is converted onto the family once the family exists, and that conversion is the family's acceptance test.** `DepartmentDaoTests` is the largest class in the suite — **33 tests against 19 numbered rules**. If `DepartmentDao` reduces to a thin derivation and **all 33 still pass**, the family is proven against the strictest contract in the repository. The hand-written version survives in git history and in FR 13 / FR 14 as the record of why the family looks as it does. **Added clause — 33/33 is necessary and not sufficient:** the conversion must also not *lose* what the hand-written DAO proves. `DepartmentDao` today derives from no base at all and carries its own `Snapshot`, `Live`, `Track`, `Read`, `Save`, `Detach` and `AsUtc` helpers; a conversion that keeps a helper the family was supposed to absorb has moved the code, not generalized it. **State which helpers the family absorbed and which survived, and why**, or the acceptance test passes on a derivation that is thin in name only. | Binds [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication); consumes [D15](#owner-decisions--2026-08-15) |
| **D17** *(2026-08-18)* · **⚠️ Ratified 2026-08-19 with an amendment — the rule stands, one of its two worked examples does not** | **The line for hand-writing a DAO.** *A hand-written DAO is justified only when it is the source for a generic family that does not yet exist, or when the contract has no family to belong to.* `DepartmentDao` qualifies on the first; ~~`CompanyResourceDao` will qualify on the second~~ — **see the amendment below; it qualifies on the *first*, not the second.** **`CompanyDao`, `JobDao` and `UserDao` qualify on neither and stay on the bases.** The rejected alternative was hand-writing those three to route around the adoption defect: **declined**, because it would leave only 2 of 7 DAOs exercising EFTools — making `ProphetsWay.Example.DataAccess.EF` a demonstration of BaseDataAccess and the Example domain rather than of EFTools — while leaving the defect shipping in the published package. **This reversed an earlier agent recommendation; the owner caught it.** | Governs every DAO in `ProphetsWay.Example.DataAccess.EF`; binds [FR 14](feature-requests.md#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule). Amendment: [The `CompanyResourceDao` amendment to D17](#the-companyresourcedao-amendment-to-d17--2026-08-19) |
| **D18** *(2026-08-18)* · **✅ Ratified 2026-08-19** | **One test suite, never two.** A second Entity Framework suite — one exercising hand-written DAOs, one the bases — was considered and **rejected**: the seam's whole value is that **one suite has one construction site**, and a second would drift until it proved nothing. **The variable is which DAOs derive from the bases, never which suite runs.** | Upholds [D10](#owner-decisions--2026-08-15); guards `TestSeam.cs` and the `Guard=Seam` gate |
| **D19** *(2026-08-19)* · **Ratified by the owner directly** | **`Restore` belongs to `IDepartmentDao` and to no library contract.** In the owner's words: *"`Restore` is only for `IDepartmentDao`, to illustrate a custom method on a consumer's own DAO interface. It is **not** meant to be built into the `ProphetsWay.BaseDataAccess` interface contracts."* It is the "1%" the purpose sentence leaves to the consumer, and it stays there. **Neither the generic soft-delete families nor `ProphetsWay.BaseDataAccess` gains a `Restore` member**, and the option is **rejected rather than deferred**. | **Closes the question the 2026-08-18 session left open** — *does `Restore` belong on the soft-delete family or stay on `IDepartmentDao`?* Constrains the `Interface Architect` shape pass on [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication); reasoning and consequences in [The `Restore` Boundary](#the-restore-boundary--settled-d19) |
| **D20** *(2026-08-24)* · **Ratified by the owner directly** | 🔴 **Whole-suite SQL Server certification is a release gate on 3.0.0.** In the owner's words: *"if we can't say EFTools is cleared to work in SQL Server, then it's not worth pushing out."* **`ProphetsWay.EFTools` 3.0.0 stays untagged and unpublished until one complete run of all 270 cases against SQL Server is green.** *(Clarified 2026-08-24, without altering the decision: **"all 270" was the suite's size on the day, not the bar.** The suite is now **287** and **14 cases are structurally exempt from the provider selection** — see [the restatement](#what-all-270-on-sql-server-means-now--restated-2026-08-24). The exemption is from the selection, never from the run.)* This **narrows which certification gates the tag**; it does **not** withdraw [D2](#owner-decisions--2026-08-15)'s or [D4](#owner-decisions--2026-08-15)'s SQLite leg, which is still owed and is **not** a blocker. ~~**Azure SQL as an additional leg is a separate, undecided question** — the owner named it as the production target, and certifying local SQL Server does not certify it.~~ **⛔ That final clause was answered later the same day by [D22](#owner-decisions--2026-08-15): Azure SQL *is* a certified leg and a *second* release gate. Everything before it stands unchanged.** | Reclassifies [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) as release-blocking; constrains [D11](#owner-decisions--2026-08-15) step 4; reasoning in [The SQL Server Certification Gate](#the-sql-server-certification-gate--settled-d20--d21) |
| **D21** *(2026-08-24)* · **Ratified by the owner directly** | **The certification environment is the owner's local MSSQL instance, and deploying the `ProphetsWay.Example` DACPAC to it is authorized.** In the owner's words: *"i have a local instance of mssql running on local host, if we need to deploy the schema from Example's sqlproj dacpac, we can deploy it locally to then run our tests against it."* **The approved target is one complete 270/270 SQL Server run.** *(Same 2026-08-24 clarification as D20 — the count is a snapshot; the bar is [the five criteria](#gate-1s-success-criterion-stated-so-it-does-not-go-stale).)* A running server and a deployable schema are **available evidence, not completed certification** — [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) closes on a run, not on the means to run. **This authorizes no `.yml` change and no `LocalTestsOnly` edit**; those remain `Pipeline Engineer`'s and remain `Deferred`. | Supplies the environment D20 requires; unblocks the Stage 3 lap scoped in [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **D22** *(2026-08-24)* · **Ratified by the owner directly** | 🔴 **Azure SQL is a certified leg of 3.x, and a SECOND release gate on 3.0.0.** In the owner's words: *"yes, option 1 please, mssql server and azure sql both need to be covered, i don't have an azure sql instance setup just yet, but if we get bicep, we shoudl be able to deploy an instance from cli and us that to test?"* — `option 1` being the offered choice **"Azure SQL is a certified target before 3.0.0."** **Both local Microsoft SQL Server *and* Azure SQL must carry successful certification evidence before the 3.0.0 tag or publish.** This **extends** [D20](#owner-decisions--2026-08-15) rather than replacing it: D20's local SQL Server gate stands exactly as written, and Azure SQL is added beside it. **What this authorizes: nothing operational.** Infrastructure may be **authored as Bicep, built and previewed**; `what-if` requires an available Azure context; **actual deployment and later teardown each require the owner's explicit approval at their own irreversible gate.** **No Azure resource exists, no subscription is named, and no Azure run has happened** — see [Azure SQL is a separate scope question](#azure-sql-is-a-separate-scope-question-and-it-reaches-this-librarys-own-contract) for the open design decisions, none of which is an agent's to choose. | **Closes [Q5](#unresolved-purpose-level-questions).** Files [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql) as `Scheduled` and release-blocking; adds a second precondition to [D11](#owner-decisions--2026-08-15) step 4; supersedes D20's closing clause |

**⏳ D14–D18 were taken by the owner in conversation on 2026-08-18 and recorded here by an agent so they
would not be lost overnight. The substance is the owner's and is not to be re-litigated; the numbering and
the `Proposed`/ratified status are `Purpose Refiner`'s to confirm.** They are appended, not merged, and no
earlier decision's text has been altered. If `Purpose Refiner` reassigns a number, it does so once — numbers
here are permanent and monotonic thereafter.

### Ratification of D14–D18 — 2026-08-19

**Four ratified as written, one ratified with an amendment, none rejected.** The numbering is confirmed and
is now permanent. What each ratification actually checked, because "confirmed" without a check is the failure
mode these documents exist to prevent:

| # | Verdict | What was checked, and against what |
|---|---|---|
| **D14** | **Ratified as written** | Checked against the artefact that would falsify it — `docs/api-contract.md` revision 8, which **already specifies the corrected `Insert`, `Get` and `Update` semantics** (A24, A26, OD-4, OD-7, A22, and `Get`'s unconditional `AsNoTracking()`). So route (b) is not merely cheaper; **the collapse cannot avoid carrying the fix**, because the document the collapse is built from has the fix in it. Route (a) would have meant patching types the same document deletes. Consistent with the [FR 14 triage](feature-requests.md#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule) taken the same day |
| **D15** | **Ratified as written** | The precondition was verified rather than accepted: `ProphetsWay.Example.DataAccess.EF/Daos/DepartmentDao.cs` exists and declares `internal class DepartmentDao : IDepartmentDao` — **deriving from no EFTools base**, which is the property that makes it usable as a source rather than as a wrapper over the bases it was written to avoid |
| **D16** | **Ratified, with a clause added** | The 33/33 criterion is sound and is the strictest gate available. The clause was added because `DepartmentDao` carries **seven private helpers** — verified by opening it — and a conversion can keep all seven, pass 33/33, and have generalized nothing. The clause is an addition to the decision, not a qualification of it; if the owner disagrees, **D16 as they stated it wins** |
| **D17** | **Ratified with an amendment** — see below. The **rule** is right and is the sharpest thing in D14–D18. One of its two worked examples is not | |
| **D18** | **Ratified as written** | It is [D10](#owner-decisions--2026-08-15) applied one level down, and the reasoning is the same reasoning: two copies of a suite prove nothing once they diverge. Nothing found in this pass touches it |

**None of the five conflicts with the purpose sentence**, and D14 and D17 both actively defend it — D17 by
refusing to let `ProphetsWay.Example.DataAccess.EF` stop exercising the library it exists to exercise, which
is the *"proving ground"* clause of the [Audience](#audience) section doing real work.

#### The `CompanyResourceDao` amendment to D17 — 2026-08-19

**D17's second criterion — *"the contract has no family to belong to"* — is stated correctly and applied to
the wrong DAO.**

The reasoning D17 gives for `CompanyResourceDao` is that `ICompanyResourceDao` deliberately does not inherit
`IBaseDao<T>`. That is true — verified by opening
`ProphetsWay.Example.DataAccess/IDaos/ICompanyResourceDao.cs`, which declares a bare
`public interface ICompanyResourceDao` with `Insert`, `Delete` and the retrieval members, and no base
interface. And **against the library as it stands today the conclusion follows**: the only keyless base is
`BaseNonIdDao<T>`, which declares `public abstract T Get(T item)` and `public abstract int Update(T item)` and
so forces two members the contract declines to declare — the mismatch
[FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)
records.

**Against the library 3.0.0 is being built into, it does not.** `docs/api-contract.md` **S5** and **A1**
create the keyless family precisely for this shape: `RootNonIdDao<TEntity>` is public plumbing that
**implements no capability interface at all**, and the document names it *"the recommended base for the
Example's join-table DAO"* and carries a `CompanyResourceDao : RootNonIdDao<CompanyResource>` sample in two
places. **`ICompanyResourceDao` will have a family. It just does not have one yet.**

**The amendment, which narrows nothing and changes no work:** `CompanyResourceDao` qualifies for hand-writing
on D17's **first** criterion — *source for a family that does not yet exist* — and not the second. The
practical difference is what happens **after** the family lands: under the second criterion it would stay
hand-written permanently; under the first it is converted onto `RootNonIdDao<CompanyResource>` exactly as
`DepartmentDao` is converted onto `BaseSoftDao`, and its tests become that family's acceptance test the way
D16 makes `DepartmentDaoTests` the keyed family's.

**Why this matters rather than being pedantry.** A permanently hand-written `CompanyResourceDao` would leave
the keyless half of the library with **no consumer in the proving ground at all** — which is the exact failure
D17's own rejected alternative was rejected for, arrived at from the other direction. The keyless families
are 4 of the 12 public classes in the 3.0.0 surface; nothing would exercise them.

**If the owner meant the second criterion literally, this amendment is what should be corrected, not D17.**
The rule as they stated it is unchanged either way.

**D11–D13 were taken on 2026-08-16, in the same session, and are appended rather than merged into the rows
above.** No earlier decision's text has been altered. **The numbering is not the order they were stated in:**
the owner assigned **D11** to the release-ordering decision explicitly, so the other two took the next numbers
after it. Numbers here are permanent and monotonic; do not renumber to restore narrative order.

**D10 postdates this section's heading, which is left as it was.** The heading reads *2026-08-15* because
that is when D1–D9 were taken; D10 was taken on **2026-08-16** and is dated in its own row rather than by
rewriting the heading or any earlier decision's text.

**Two things D10 does *not* do, recorded because both are easy to over-read.** It does not approve a seam
*design* — only the direction, with the design deferred, so the absence of one is not unfinished work. And it
does not reopen
[ProphetsWay.Example FR 8](../../ProphetsWay.Example/docs/feature-requests.md#8--selecting-the-implementation-from-configuration-instead-of-a-code-edit),
which remains `Rejected`: that entry declined *configuration-driven* selection, while shape B asks only that
the construction line be **reachable** from a repository that cannot edit it.

**D9's precondition has since been met — factual note, 2026-08-16. The decision text above is the owner's
and is left exactly as written.** D9's closing clause, *"nothing local stands between the repository and
advancing the pointer,"* was a statement about a pending action. **That action has been taken:** the pointer
was advanced to `61d9e7d`, verified by reading `.git/modules/ProphetsWay.Example/HEAD` on 2026-08-20
(earlier text here said `d845863`, which was the first advance on 2026-08-16 and is superseded). Read the
clause as
the record of a condition that *was* satisfied and then *was* acted on — not as an outstanding invitation.
A future agent finding it and concluding there is still a pointer waiting to be moved would be reading a
closed decision as an open task. The remaining work is
[FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) steps 2–6,
which D9 never spoke to.

**D7 is the decision most likely to be misread as drift by a future agent**, because it puts this
repository outside a family-wide convention. The reasoning is in
[The `net10.0`-Only Exception](#the-net100-only-exception--settled), and the short form is that the
convention's own justification does not apply here: `netstandard2.0` exists to be a **portable contract**,
and this package's entire surface is generic base classes over `DbContext`. `AGENTS.md` needs a line saying
so — **that file is not this agent's to write.**

**D3 is the decision a future reader will most want the reasoning for**, because this document previously
argued against it. The argument that changed is recorded in
[The Key-Type Collapse](#the-key-type-collapse--settled-against-this-documents-earlier-recommendation) —
one of the three reasons given for the earlier recommendation turned out to be contradicted by the
library's own source.

---

## Current Purpose (as implied by README/csproj)

The `<Description>` in
[ProphetsWay.EFTools/ProphetsWay.EFTools.csproj](../ProphetsWay.EFTools/ProphetsWay.EFTools.csproj) and the
opening of [README.md](../README.md) say the same thing, near-verbatim:

> "A small library that is useful when utilizing EntityFramework for a Data Access Layer (DAL) while
> adhering to Business Layer to DAL decoupling. This uses the paradigm explained in
> …/ProphetsWay.BaseDataAccess, see its README for more information."

The README then spends its length on a walkthrough of the base classes in the order the owner would write
a new project — which is genuinely good teaching material and should survive any rewrite.

---

## The Drift

The drift here is **not** that the library grew features it should not have. The public surface is
disciplined: 26 source files, every one of them either a DAO base, a context base, or the DAL root, and
nothing resembling a utility, a helper, or a convenience API. That is unusual and worth saying plainly.

The drift is in four other places.

### 1. The purpose sentence has no subject

"A small library that is useful when…" describes a *feeling about* the library rather than its job. It does
not say what it removes, what it requires, or who finishes the work. A reader on nuget.org cannot tell
whether they need `ProphetsWay.BaseDataAccess` as well — they do, unconditionally — because the sentence
mentions it only as further reading.

### 2. "EntityFramework" hides an unresolved fork

The library currently ships **two different products under one package ID**, selected by target framework.
This is stated in `AGENTS.md` as a conditional package reference, which undersells it substantially. The
divergence reaches the public surface and the semantics:

| Where | `net461`/`net471`/`net48` (EF6 6.5.1) | `net80`/`net90` (EF Core 9.0.4) |
|---|---|---|
| [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) | `base(connectionString)` — EF6 resolves the provider from config | `UseSqlServer(connectionString)` — provider hardcoded |
| [BaseEFContext.cs](../ProphetsWay.EFTools/BaseEFContext.cs) | one constructor | **two** — a `DbContextOptions` overload exists only here |
| [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) | one constructor | **two** — `DbContextOptions` overload again |
| [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) `Update` | `Dataset.AddOrUpdate(item)` — an **upsert**; inserts when absent | `Single(…)` + `SetValues` + `EntityState.Modified` — **throws** when absent |
| [RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) | `DbContextTransaction` | `IDbContextTransaction` |

The `Update` row is the one that matters. **The same method on the same type has different behaviour for a
missing row depending on which asset the consumer resolved** — silent insert on one, exception on the
other. That is not an `#if` around a `using`; it is two contracts.

**Settled by [D1](#owner-decisions--2026-08-15).** 3.x keeps the EF Core column and deletes the other. The
EF6 column does not become undocumented — it remains the behaviour of the published 2.2.x package, which
is where an EF6 consumer is now pointed.

### 3. The library forces a database provider on every consumer

`ProphetsWay.EFTools.csproj` takes unconditional `PackageReference`s on
`Microsoft.EntityFrameworkCore.SqlServer` **and** `Microsoft.EntityFrameworkCore.InMemory` for every
non-`net4x` target, and `BaseEFContext`'s string constructor calls `.UseSqlServer(...)` directly.

A consumer on PostgreSQL, SQLite or Cosmos installs this package and receives the SQL Server provider and
the InMemory provider whether they want them or not, and cannot use the string constructor at all. A
library whose stated purpose is *decoupling* is coupling its consumers to a provider. Recorded as
[FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer).

`InMemory` is the sharper half: it is a **testing** provider, Microsoft documents it as unsuitable for
production, and it is here because the *examples* use it. A test dependency of the proving ground has
become a runtime dependency of the product.

**Settled by [D2](#owner-decisions--2026-08-15).** Both provider references leave the library. See
[Provider Neutrality](#provider-neutrality--settled) for what "neutral" was defined to mean, which is
narrower than "any provider EF Core has."

### 4. The library no longer implements the contract it advertises

`ProphetsWay.EFTools.csproj` references `ProphetsWay.BaseDataAccess` **2.5.0**. The published parent is
**3.1.0**, and 3.0.0 made `IBaseDataAccess` extend `IDisposable` with `BaseDataAccess` declaring
`public abstract void Dispose();`
([BaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/BaseDataAccess.cs) line 112,
[IBaseDataAccess.cs](../../ProphetsWay.BaseDataAccess/ProphetsWay.BaseDataAccess/IBaseDataAccess.cs) line 164).

[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) overrides `TransactionCommit`,
`TransactionRollBack` and `TransactionStart` and **nothing else**. It will not compile against 3.1.0 until
it supplies `Dispose` — and deciding what `Dispose` *means* for a class that constructs its own `DbContext`
via `Activator.CreateInstance` is a design decision, not a mechanical fix.

So the README's paradigm claim is currently a statement about version 2.x. This is the same shape of
staleness that [ProphetsWay.Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md) records from
the other side, and it is why that request was routed here as
[FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts).

---

## Cohesion Map

Every public type in the library, grouped. `→` means "depends on."

| Cluster | Types | Depends on | Depended on by | Extraction candidate? |
|---|---|---|---|---|
| **Context base** | `BaseEFContext` | EF `DbContext`; on modern targets, `Microsoft.EntityFrameworkCore.SqlServer` | `BaseEFDataAccess` (generic constraint `where TContextType : BaseEFContext`); every consumer context | **No** — 6 lines of body; meaningless without EF |
| **DAL root** | `BaseEFDataAccess<TContextType, TIdType>` | `ProphetsWay.BaseDataAccess.BaseDataAccess`, `BaseEFContext`, EF | Every consumer DAL | **No** — its entire body is EF transaction calls against the parent's abstract members |
| **Internal engine** | `RootNonIdDao<T>`, `RootDao<T, TIdType>` (both `internal`) | EF, `IBaseEntity` / `IBaseIdEntity<T>` | Public bridge cluster | **No** — `internal`; not a surface |
| **Public bridge** | `RootBaseDao<T, TIdType>`, `RootBaseSoftDao<T, TIdType>`, `BaseNonIdDao<T>`, `BaseSoftNonIdDao<T>` | Internal engine, `IBaseDao`/`IBaseGetAllDao`/`IBasePagedDao` | Key-typed DAO surface | **No** — all four are `[EditorBrowsable(Never)]` or plumbing for it |
| **Key-typed DAO surface** | 18 types: `{BaseDao, BaseGetAllDao, BasePagedDao, BaseSoftDao, BaseSoftGetAllDao, BaseSoftPagedDao}` × `{Guid, Int, Long}` | Public bridge | Consumer DAOs | **No** — each is a ~10-line closure over the bridge; separating them from it is meaningless |

**[D3](#owner-decisions--2026-08-15) rewrites the last row, not the verdict.** The 18 key-typed types
collapse into **six generic families in the root namespace**. That changes the shape of the public surface
and its `using` story; it changes nothing about the dependency graph, because the collapsed types depend on
exactly what the triplicated ones did. The extraction verdict below is unaffected.

### The extraction verdict — `docs/nuget-extraction-proposal.md` is `n/a`, not missing

**There is no viable extraction candidate in this repository, and that file has deliberately not been
created.** The dependency argument, which is the only argument my charter accepts:

1. **Every cluster has a non-optional inbound edge from another cluster in the same package.** Not one has
   zero inbound edges. The key-typed surface cannot compile without the bridge; the bridge cannot compile
   without the internal engine; the DAL root cannot compile without the context base.
2. **Every cluster depends on both Entity Framework *and* `ProphetsWay.BaseDataAccess`.** A split would
   produce packages with strictly more dependencies than callers, which is the opposite of the point.
3. **The independent-usefulness test fails at the first question.** Nobody wants `Int.BasePagedDao<T>`
   without `RootBaseDao<T,int>`, and nobody wants either without an `IBaseDataAccess` to hang them on.

The interesting split question here is the **reverse** one, and it also resolves against splitting:
`ProphetsWay.Example.DataAccess.EF` lives in this repository rather than in `ProphetsWay.Example`. Moving
it would invert the dependency edge and create a **cycle between repositories** — `ProphetsWay.Example`
would depend on the `ProphetsWay.EFTools` package while `ProphetsWay.EFTools` consumes
`ProphetsWay.Example` as a submodule. The current arrangement is acyclic and correct. Recorded here so it
is not re-proposed as tidiness.

The one thing that *should* eventually be a sibling package —
`ProphetsWay.BaseDataAccess.Conformance` — is already owned by the parent repository as
[BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md), whose revisit trigger is
literally "after `ProphetsWay.EFTools` has been built and updated onto 3.x." It is not this repository's
to build.

---

## The EF6 Question — Settled

> **Settled by the owner on 2026-08-15 as [D1](#owner-decisions--2026-08-15).** The recommendation below
> was accepted as written. [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework)
> is now `Scheduled` for v3.0.0. The reasoning is preserved in full — a decision without its argument is
> just an assertion, and this one will be questioned again.

### Scope Verdict — retaining EF6 support in 3.x

| | |
|---|---|
| **Verdict** | **Out of scope** for 3.x — EF6 belongs on the 2.2.x line, which is already published and stays available |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | Supporting both is not one conditional compile; it is two dependency graphs, two runtime matrices, two public surfaces, and — in `RootDao.Update` — two *semantics* under one method name. A package cannot honestly promise one contract while shipping two. |
| **Owner's decision** | **Approved.** 3.x is EF Core-only; 2.2.x remains the EF6 / .NET Framework answer and receives no new work; **no EF6 companion package** |
| **What follows from it** | The purpose sentence and `<Description>` say "Entity Framework Core"; the TFM list loses `net4x`; the `EntityFramework` 6.5.1 reference goes; roughly 60 `#if` blocks across 26 files collapse; the README's context examples stop showing `DbModelBuilder`; CHANGELOG carries a breaking-change entry pointing 2.2.x consumers at the existing package |

### The owner's stated instinct, and why the evidence points the other way

The instinct — *"the conditional code looks small, so keeping it looks cheap"* — is a reasonable reading of
the file listing. It is contradicted by what the files actually contain.

**Evidence that it is not small:**

- **The divergence is semantic, not syntactic.** `RootDao.Update` is an upsert on EF6 and a
  throw-if-missing update on EF Core (table in [The Drift](#2-entityframework-hides-an-unresolved-fork)).
  Any contract this library writes about `Update` is currently either false on one branch or so weak it
  says nothing.
- **The public surface differs by target.** The `DbContextOptions` constructors on `BaseEFContext` and
  `BaseEFDataAccess` exist only on modern targets. A `net48` consumer reading the README's preferred
  pattern — which the README explicitly recommends — finds it absent.
- **The preprocessor is already load-bearing in a way that is hard to read.** In
  [ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs](../ProphetsWay.Example.DataAccess.EF/ExampleDataAccess.cs),
  an `#if NET471 || NET48` block *opens a constructor brace* that a separate `#if NET8_0_OR_GREATER` block
  closes. Braces span preprocessor boundaries. Every future edit to that constructor must be reasoned
  about twice, and no single compilation shows a reader the whole file.
- **The `#if` conditions are hardcoded TFM lists, not feature tests.** They are
  `#if NET461 || NET471 || NET48` and `#if NET8_0_OR_GREATER`. **Adding `netstandard2.0` — the house
  standard's permanent reach floor — matches neither branch**, producing files with no `using` directives
  and empty bodies. So EF6 retention does not merely coexist with the TFM modernization; it **blocks** it
  until every condition is rewritten. That is [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard)'s
  hidden cost, and it was not visible from the csproj alone.
- **The test matrix doubles at the exact moment it must be rewritten anyway.** `LocalTestsOnly: 'yes'` in
  [app-variables.yml](../app-variables.yml) means CI does not run these tests. Keeping EF6 means
  maintaining a .NET Framework leg that **nothing automated ever executes**.
- **The stated migration cost is near zero.** No known live consumer requires .NET Framework or EF6, a
  major bump is acceptable, and **2.2.x remains published and installable**. Retiring EF6 strands nobody;
  it stops shipping them *new* work.

**The one honest argument for keeping it**, stated at full strength because the owner should weigh it:
the EF6 branch is *written and working today*, and once deleted it is materially harder to restore than to
have kept — the `#if` blocks encode real knowledge about EF6's transaction and upsert behaviour that a
future reconstruction would have to rediscover. If a .NET Framework consumer appears in the next year, the
cheap answer today ("it already builds") becomes an expensive one ("port it back").

The counter to that counter is the 2.2.x package: the knowledge is not deleted, it is *tagged*. Git history
and a published, installable artifact both retain it.

### What is explicitly **not** being recommended

- Not recommending a `ProphetsWay.EFTools.EF6` companion package. That is a second repository, pipeline,
  version line, changelog and support surface for an audience currently measured at zero.
  **The owner rejected it explicitly in [D1](#owner-decisions--2026-08-15) — this is closed, not open.**
- Not recommending deprecating or unlisting 2.2.x. It should remain the supported answer for EF6.
- ~~Not recommending this be decided by an agent. **This is the checkpoint question.**~~ **Decided by the
  owner. The checkpoint is passed.**

### The `net10.0`-Only Exception — Settled

> Settled as [D7](#owner-decisions--2026-08-15), which **closes Q1**. This surfaced only after D1 was
> taken, as a conflict with the house TFM standard; it is now a **ratified exception** rather than an open
> conflict. Tracked as [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard).

### Scope Verdict — taking a documented exception to the `netstandard2.0` reach floor

| | |
|---|---|
| **Verdict** | **In scope, and it does not widen the purpose sentence.** The sentence already says "Entity Framework Core"; a target list that only EF Core can restore is that sentence being honest about itself |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | The reach floor exists to make a **portable contract** consumable from .NET Framework 4.6.1 upward. This package's public surface is not a contract — it is generic base classes over `DbContext`, and every one of them is meaningless without an EF Core runtime. There is no consumer the floor would reach who could use what they found there |
| **Owner's decision** | **Approved.** 3.x targets **`net10.0` only** — library, tests, and `ProphetsWay.Example.DataAccess.EF` alike. Existing `net4x` / `net8.0` / `net9.0` consumers remain on the published **2.2.x** line |

**The mechanical fact behind it.** EF Core has shipped no `netstandard2.0` asset since 3.1 — 5.0 onward
are runtime-targeted — and **EF Core 10 exposes only `net10.0`**. Adding `netstandard2.0` to an EF
Core-only `ProphetsWay.EFTools` produces a target that cannot restore its own primary dependency. This is
not a preference the owner exercised over the convention; it is the only target list that builds.

**Why `net10.0` alone rather than `net10.0` plus an LTS predecessor.** This document previously floated the
two-target option. It does not survive the same fact: an EF Core 10 dependency has no asset for an earlier
runtime, so a second TFM would either pin an older EF Core on that leg — reintroducing the
two-products-under-one-package-ID problem [D1](#owner-decisions--2026-08-15) exists to end — or not
restore at all.

**Who this strands: nobody, and that is checkable.** 2.2.x is published and installable, and it is where
`net4x`, `net8.0` and `net9.0` consumers stay. They stop receiving new work; they lose nothing they have.

**The test project's `net48` leg goes with it.** In `ProphetsWay.BaseDataAccess` and `ProphetsWay.Example`
that leg exists to bind the `netstandard2.0` asset and verify .NET Framework behaviour. Here there will be
no such asset to bind, so the leg would be testing nothing.

**What still has to happen elsewhere:** `AGENTS.md` needs a line recording this exception, so a future
agent reading `net10.0` alone sees a decision rather than drift. **That file is not this agent's to
write** — it is reported here for its owner.

---

## Provider Neutrality — Settled

> Settled as [D2](#owner-decisions--2026-08-15). Tracked as
> [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer), now `Scheduled`.

### Scope Verdict — the library choosing a database provider

| | |
|---|---|
| **Verdict** | **Out of scope.** Choosing the database is the single most consumer-specific decision in a DAL |
| **Purpose it's measured against** | The settled sentence above, which now names provider neutrality explicitly |
| **Because** | A library whose stated purpose is *decoupling* cannot hand every consumer a SQL Server dependency they did not ask for, and cannot ship a *testing* provider in a runtime package |
| **Owner's decision** | **Approved.** The consumer configures their provider through `DbContextOptions` |

### What "neutral" was defined to mean — and the two lines it draws

Neutrality here is **bounded**, and the bound is the useful part of the decision. Three tiers, not two:

| Tier | Providers | What the library promises |
|---|---|---|
| **Certified** | **SQLite**, **SQL Server** | This repository's own tests run against them. A green suite is evidence for these two and only these two |
| **In scope, uncertified** | **PostgreSQL**, **MySQL/MariaDB**, **Oracle** | Relational, so nothing in the design excludes them. Untested here; a defect report against one is in scope to fix |
| **Out of scope** | **Cosmos** and other non-relational providers | Not a target. A defect that reproduces only on Cosmos is not a defect in this library |

The Cosmos exclusion is a genuine scope boundary rather than a lack of enthusiasm. The DAO surface assumes
things a relational store supplies and a document store does not — `Skip`/`Take` paging over a stably
ordered set, `GetCount` as a server-side aggregate, and transactions with the scope semantics the parent's
contract specifies. Promising those on Cosmos would mean either weakening the contract for everyone or
emulating them in the library, and emulation is the kind of surface this package exists to avoid.

**The certified/uncertified split is not a hedge.** It is the honest statement of what a test run proves.
Claiming five providers on the strength of two would be exactly the "asserted rather than demonstrated"
failure this document objects to elsewhere.

### Public Wording — Settled

> Settled as [D8](#owner-decisions--2026-08-15), which **closes Q4**. The question was whether the
> certified/uncertified split is stated on the **package** or only in these documents. It is stated on the
> package. Tracked as [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container)
> and [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer).

Public wording — `<Description>`, README, `PackageTags`, and anything else a reader meets before they meet
this repository — must say both halves and no more:

| Must say | Must not say |
|---|---|
| EFTools is **designed for relational Entity Framework Core providers** | That it supports Entity Framework 6, or any non-relational provider |
| It is **certified and tested by this repository on SQLite and SQL Server** | Anything implying **another relational provider is certified** — PostgreSQL, MySQL/MariaDB and Oracle are in scope and **uncertified**, and the wording must leave a reader in no doubt which they are getting |

The reason the second column matters is the whole point of the tier table above: the uncertified tier
exists to set expectations, and it sets none if it is invisible to the person installing from nuget.org. A
package that says "works with relational providers" unqualified has quietly promised five things it has
tested two of.

**Not this agent's files.** `<Description>` and `PackageTags` belong to `Modernizer`; the README belongs to
`README Author`. This entry is the wording constraint they implement, not the implementation.

### The one sub-question the decision does not answer

`BaseEFContext(string connectionString)` currently calls `.UseSqlServer(...)`. D2 removes the SQL Server
reference, so that body cannot survive as written. Whether the constructor is **deleted** or **retained**
with the provider configured by the derived context's `OnConfiguring` is now an implementation choice, not
a purpose question — either satisfies neutrality.

What D2 *does* settle is the option this document previously floated as a defensible alternative: a
SQL-Server-flavoured convenience entry point, or a separate `ProphetsWay.EFTools.SqlServer` package.
**Both are rejected.** A named provider in the package graph is the thing being removed; reintroducing it
behind a friendlier name reintroduces the coupling.

---

## The SQL Server Certification Gate — Settled (D20 · D21)

> Settled 2026-08-24 as [D20](#owner-decisions--2026-08-15) and [D21](#owner-decisions--2026-08-15). Tracked as
> [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container),
> which is now **release-blocking**.
>
> 🔴 **Extended later the same day by [D22](#owner-decisions--2026-08-15). There are now TWO certification
> gates on the 3.0.0 tag, not one.** The heading is left exactly as it was so existing links keep resolving;
> read it as *the certification gate section*, not as *the only gate*.
>
> | Gate | Provider | Decision | Tracked as | Evidence today |
> | --- | --- | --- | --- | --- |
> | **Gate 1** | **Local Microsoft SQL Server** | D20 · D21 | [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | Server reachable; `ProphetsWay.Example` schema has a zero-drift DACPAC deploy report; **the run has not happened** |
> | **Gate 2** | **Azure SQL** | **D22** | [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql) | **None.** No subscription named, no resource deployed, no run attempted |
>
> **Neither gate is satisfied, and neither may be inferred from the other** — see
> [Azure SQL is a separate scope question](#azure-sql-is-a-separate-scope-question-and-it-reaches-this-librarys-own-contract),
> which is now *answered* rather than open, and whose reasoning is why these are two gates rather than one
> gate with two providers.

### Scope Verdict — gating the 3.0.0 tag on a whole-suite SQL Server run

| | |
|---|---|
| **Verdict** | **In scope, and it sharpens the purpose rather than widening it** |
| **Purpose it's measured against** | The [settled sentence](#settled-one-sentence-purpose), which already promises *"whichever relational provider the consumer configures"* |
| **Because** | The purpose sentence makes a claim about provider behaviour. A claim of that shape is either demonstrated or asserted, and [D8](#owner-decisions--2026-08-15) already committed to printing it on the package. D20 does not add an obligation — it names the **minimum evidence** that discharges one already taken |
| **Owner's decision** | **Approved and stated as a bar:** *"if we can't say EFTools is cleared to work in SQL Server, then it's not worth pushing out."* |
| **If it proceeds** | Nothing in the purpose sentence, the cohesion map or the extraction verdict changes. What changes is the **release sequence**: [D11](#owner-decisions--2026-08-15) step 4 now has a precondition |

### Why this reverses a judgement that was correct when it was made

The 2026-08-23 pass classified FR 11 as CI evidence and therefore deferrable, and **on the information it had,
that was right.** Nothing about the library is less correct today. What arrived on 2026-08-24 was the
**Audience** section's first row getting a name and a deployment target: the owner's own applications —
`ProphetsWay.BPA`, and a previously unmentioned `HashDB` — build and debug against **local MSSQL** and deploy
to **Azure SQL**. *"The one real customer is me, and it needs to be ready."*

That converts the same test run from *quality evidence* into *fitness for the only purpose the package has*.
**A correctness bar and a fitness-for-purpose bar are different bars, and only the owner can set the second.**
This is the cleanest illustration in these documents of why a scope gate needs the owner rather than an agent:
no amount of reading the source would have produced D20.

### The distinction that must not collapse — the gate is narrower than the certification

**[D2](#owner-decisions--2026-08-15), [D4](#owner-decisions--2026-08-15) and [D8](#owner-decisions--2026-08-15)
are untouched.** The certified tier is still **SQLite and SQL Server**. D20 says which half gates the tag:

| Obligation | Source | Gates the 3.0.0 tag? | State |
|---|---|---|---|
| **The whole suite green against SQL Server** — ~~all 270 cases~~ **see [the restatement below](#what-all-270-on-sql-server-means-now--restated-2026-08-24); the count moved to 287 and 14 cases are structurally exempt from the *selection*, not from the *run*** | **D20** | 🔴 **Yes** | Owed. The environment exists (D21); the run does not |
| **The whole suite green against SQLite** — same restatement | D2, D4 | **No** | Owed. **Not cancelled, not descoped** — deferred behind the gate |
| `LocalTestsOnly: 'yes'` retired so CI runs any of it | D4 | **No** | `Deferred` to `Pipeline Engineer`. **No `.yml` change is authorized** |
| Certification wording on `<Description>`, `PackageTags`, README | D8 | **No**, but it must not *precede* the evidence | **Unmet** — verified 2026-08-24 against the csproj at `e2f2120` and the README. Fortunate: writing it on 2026-08-23 would have published a claim with a whole-suite run behind neither provider. **[D22](#owner-decisions--2026-08-15) adds Azure SQL to what that wording must eventually cover**; the phrasing belongs to `Modernizer` and `README Author`, and the "must not precede the evidence" rule now applies to two gates |
| **Azure SQL**, whole suite green | **D22** | 🔴 **Yes** | Owed, and **nothing exists yet** — no subscription, no resource, no run. Tracked as [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql) |

**An agent that reads D20 and narrows the certified tier to SQL Server alone has misread it.** The SQLite leg
is *later*, not *withdrawn* — it is the fast gate D4 wants, and it is what makes the eventual `LocalTestsOnly`
retirement worth anything.

### What "all 270 on SQL Server" means now — restated 2026-08-24

> **The criterion is unchanged. The *number* in it was a snapshot, and it has already moved.** `Test Designer`
> added 17 provider-selection seam guards on 2026-08-24 (270 → **287**), and `Test Auditor` established the
> same day that **14 cases cannot honour a provider selection by construction.** Restated here because
> [D20](#owner-decisions--2026-08-15) is quoted by four documents and its literal *"all 270 cases"* is now
> wrong in both directions at once. **Nothing the owner decided changes** — what changes is the arithmetic
> that discharges it.

**The 14 are `AlternateKeyGuardSpikeTests`** — 7 `[Theory]` methods × 2 `[InlineData]` (`InMemory`, `Sqlite`),
every one carrying `Scope=Characterization` and `Area=AlternateKeys`. **Verified 2026-08-24 by opening the file
and grepping every xUnit attribute in it**, not inherited. Its subject is Entity Framework Core's own
alternate-key behaviour ***compared across* providers**, so the comparison **is** the measurement: pinning it to
the selected provider does not turn it into a certification case, it deletes the case. `Test Designer` left it
pinned and added a guard holding it to `Scope=Characterization` rather than reclassifying it, and **that was the
right call** — the alternative is weakening a characterization test so an arithmetic sentence comes out round.

#### The two sentences that must never be collapsed

| Sentence | What it asserts | What evidences it |
|---|---|---|
| **"The whole-suite command was green."** | `dotnet test` with no filter: **0 failed, 0 skipped**, total equal to the discovered count | The runner's own summary |
| **"The whole suite executed against SQL Server."** | Every case that touches a store opened it on SQL Server | **Not** the runner's summary. It needs the selection anchored outside the process and the exemption list closed by a guard |

**A run can satisfy the first and not the second, and reporting the first as the second is the false green this
gate exists to prevent.** The evidence must state both; neither may stand in for the other.

#### Gate 1's success criterion, stated so it does not go stale

**Gate 1 ([D20](#owner-decisions--2026-08-15) · [D21](#owner-decisions--2026-08-15)) is satisfied when all five
hold in one run:**

1. **The selection resolved to SQL Server from outside the process**, and the evidence says so by naming the
   environment variable and the value it carried — not by quoting a default.
2. **The whole-suite command is green** — no filter, **0 failed, 0 skipped**.
3. **Every store-touching case executed on SQL Server**, with exactly **one** exemption: the
   provider-comparison spike.
4. **The exemption list is closed by a guard, not by a sentence.** Something must turn red if any class other
   than the spike selects its own provider — otherwise (3) is unfalsifiable and the two-store arrangement
   survives while being described as one.
5. **The evidence records four counts and the provider** — total discovered, exempt, provider-honouring,
   failed. At the time of writing that is **287 / 14 / 273 / 0**. **The counts are a snapshot; the criterion is
   the definition above**, and a later lap that adds tests changes the numbers and not the bar.

**Criteria 1 and 4 are ordinarily test-design detail and are named here deliberately**, because without them
criterion 3 is *self-reported*: a seam that answers its own questions can report `SqlServer` while every store it
opened was SQLite, and the run would be green, whole-suite, and false. That is exactly the
demonstrated-versus-asserted distinction [D8](#owner-decisions--2026-08-15) turns into a claim printed on the
package. **Every other finding in `Test Auditor`'s 2026-08-24 audit is test-design and stays test-design** —
cleanup identity and store existence, foreign-key enforcement, disposal without materialisation, whitespace
handling, the conditional third refusal assertion. They are not purpose requirements and must not be imported
here.

**What this does *not* relax.** The spike is exempt from the *provider selection*, not from the *run*. **All 287
cases execute in the certification command and all 287 must pass.** An exemption from *which store it opened* is
not an exemption from *whether it passed*.

#### The same restatement governs Gate 2

[FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql) is written as *"all 270 cases
against a real Azure SQL database."* **Read it against the same five criteria** — the number is the same
snapshot and the same 14 cases are exempt for the same structural reason. Nothing about
[D22](#owner-decisions--2026-08-15) changes.

### The SQLite leg's scope was already settled — D2 and D4, and it is not a new decision

> **Raised by `Test Auditor` on 2026-08-24 as an open question:** the specified `TestStoreProvider.Sqlite`
> member makes SQLite selectable for the **whole** suite — including the 13 adapted upstream classes, whose
> schema provisioning the tests do not specify. Is `Sqlite` meant to be whole-suite, or only the fast local leg?
> **It needs no owner decision. It was decided on 2026-08-15 and this document has carried the answer since.**

| Source | What it already says |
|---|---|
| [D2](#owner-decisions--2026-08-15) | *"**Only SQLite and SQL Server will be certified** by this repository's tests."* Certification is a claim about **the suite**, not about a subset of it |
| [D4](#owner-decisions--2026-08-15) | *"SQLite in-memory as the fast **CI contract/query gate**."* A gate that runs six classes gates nothing |
| The gate table above | **The whole suite green against SQLite** — *"Owed. **Not cancelled, not descoped**"* |
| [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) | *"A certified run of the **whole** suite on **both** providers"* |

**So `Sqlite` is correctly a whole-suite selection, and the absent provisioning is a known debt rather than an
over-promise in the enum.** Deleting the member, or narrowing it to the six local classes, would silently
descope a certified provider that [D8](#owner-decisions--2026-08-15) puts on the package — which the section
above already forbids in as many words.

**What follows is a constraint, not a design.** Selecting SQLite must either **run the whole suite** or
**refuse by name**. A selection that runs 273 cases and fails 13 adapters on a missing schema is the third door
to the same false report: a maintainer reads *"SQLite: 13 failures"* as a provider defect in this library, which
is exactly backwards. **The SQLite leg is release-relevant and not release-blocking**, so deferring the
provisioning is legitimate — deferring it *silently* is not.

### Azure SQL is a separate scope question, and it reaches this library's own contract

> ✅ **ANSWERED 2026-08-24 as [D22](#owner-decisions--2026-08-15) — Azure SQL is a certified leg and a second
> release gate.** The heading is kept verbatim so inbound links resolve; the question it names is closed.
> The reasoning below is *why the question was worth asking* and is unchanged — it is also the reason Azure
> SQL cannot be treated as "SQL Server again, remotely."

The owner named **two** environments. Certifying against a local MSSQL instance does not certify Azure SQL,
and the difference is not cosmetic for a DAL: Azure SQL disconnects routinely, so EF Core's
`EnableRetryOnFailure` execution strategy is effectively mandatory — and **an execution strategy is
incompatible with a user-initiated transaction unless that transaction is wrapped in `ExecuteInTransaction`.**
`BaseEFDataAccess<TContext>` exposes `TransactionStart` / `Commit` / `RollBack` as first-class contract
members, so this lands **on this library's public transaction contract**, not on the consumer's code.

**That is what makes it a purpose-level question rather than a test-matrix one**, and it is why the answer
mattered: Gate 2 can surface a contract defect that Gate 1 structurally cannot.

#### Scope Verdict — making Azure SQL a certified leg of 3.x

| | |
|---|---|
| **Verdict** | **In scope.** It sharpens the same claim [D20](#owner-decisions--2026-08-15) sharpened, one environment further out |
| **Purpose it's measured against** | The [settled sentence](#settled-one-sentence-purpose) — *"whichever relational provider the consumer configures"* |
| **Because** | The [Audience](#audience) table's first row is the owner's own applications, and their **production** target is Azure SQL. A package that is certified only where it is *developed* and not where it is *deployed* has not been certified for its one real consumer |
| **Owner's decision** | **Approved:** *"mssql server and azure sql both need to be covered."* Both are pre-tag gates |
| **If it proceeds** | The purpose sentence, cohesion map and extraction verdict are unchanged. What changes is that [D11](#owner-decisions--2026-08-15) step 4 now has **two** preconditions — and that a transaction-contract change becomes *possible*, because `EnableRetryOnFailure` may prove incompatible with the contract as written. **If it does, that is a finding for the owner, not a change an agent takes** |

#### What does NOT exist, stated plainly so no document later implies it does

- **No Azure subscription, tenant, resource group, SQL server or database has been named, created or
  deployed by any run to date.**
- **No Bicep template exists in this repository**, and none is authorized to be *deployed* by its existence.
- **No Azure SQL test run has been attempted**, so there is no result to cite — green, red, or partial.

The owner's own framing is a question, not a completed plan: *"if we get bicep, we shoudl be able to deploy
an instance from cli and us that to test?"* Treat it as the approved **direction**, exactly as
[D10](#owner-decisions--2026-08-15) was treated — direction settled, design deferred.

#### The two bodies of work are sequential and must not be merged

| | Gate 1 — local SQL Server | Gate 2 — Azure SQL |
|---|---|---|
| **Blocked on** | Nothing. The environment is ready today ([D21](#owner-decisions--2026-08-15)) | An infrastructure design the owner has not yet settled |
| **The work** | Provider-selectable test wiring the seven store-backed local classes actually consume, plus a schema-provisioning decision — then a **whole-suite run** meeting [the five criteria](#gate-1s-success-criterion-stated-so-it-does-not-go-stale) locally (**287 / 14 exempt / 273 honouring / 0 failed** at today's counts) | Bicep authored and built → `what-if` previewed against an available Azure context → **owner-approved** deployment → DACPAC applied → suite run → **owner-approved** teardown |
| **Irreversible steps** | None. Local database, local run | **Two** — the deployment and the teardown. **Each needs the owner's explicit approval at the moment it is taken**; neither is pre-authorized by D22 |
| **Order** | **First.** It is the cheaper gate and it de-risks the second: a suite that cannot reach 270 locally will not reach it remotely | **After.** Its input is a suite already proven provider-selectable |

**Doing Gate 2 first would mean debugging test wiring and cloud infrastructure at the same time**, against a
billed resource, with no local baseline to attribute a failure to. That is the argument for the ordering; it
is not a claim that Gate 2 is less important.

#### Open Azure design decisions — the owner's, and none of them is inferable

**Not one of these has a value. Do not choose one, and do not let a template imply one.**

| Decision | Why it cannot be inferred |
|---|---|
| **Subscription and tenant** | There is no default and no discoverable correct answer |
| **Region** | Cost, latency and data-residency all differ; the owner picks |
| **Resource group name and isolation** | Whether certification shares a group with anything else is a blast-radius decision |
| **SQL server and database naming** | Server names are globally unique; a guess collides or squats |
| **Microsoft Entra / admin model** | Entra-only vs. SQL authentication changes both the connection string and the test harness |
| **Network and firewall access** | Public endpoint with an allowlist, private endpoint, or something else — this decides whether a local run can reach it at all |
| **SKU and cost budget** | Serverless, Basic, and a provisioned tier differ by orders of magnitude in cost and in behaviour under a test suite |
| **Retention** | Whether the instance survives the run, and for how long |
| **Credential and secret handling** | Where the connection string lives. **No secret may be written to any file in any of these repositories** |
| **Lifetime and teardown** | Who tears it down, when, and on whose approval |
| **Where the infrastructure lives** | `ProphetsWay.EFTools`, `ProphetsWay.BPA`, or a separate infrastructure home. This is a repository-boundary question of exactly the kind this document exists to answer, and it has not been asked yet |

**The decision the owner still owes is no longer *whether* — it is *these eleven values*.** An agent that
invents any of them has manufactured a fact about someone's cloud account.

### What Stage 3 is contracted to deliver

The implementation scope is **re-derived from the current wiring** in
[FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) and
is not repeated here. The one-line summary a downstream agent needs: **the 13 adapted upstream classes already
run on SQL Server through `TestSeam` → `Constants`; the seven store-backed local classes each build their own
SQLite in-memory context and call `EnsureCreated()`, and reach `Constants` never.** Making `Constants`
provider-selectable is therefore **necessary and not sufficient**, and any plan that stops there cannot reach
270.

**Stage 3 is Gate 1 and nothing else.** It is local test wiring plus one whole-suite run against the owner's
local MSSQL instance, measured against
[the five criteria above](#gate-1s-success-criterion-stated-so-it-does-not-go-stale) rather than against a
literal case count. **No Bicep, no `what-if`, no Azure deployment, no Azure DACPAC apply and no Azure test run
is part of it** — that is [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql), and it
starts from an infrastructure design the owner has not settled. A Stage 3 lap that reaches for Azure is out of
its own scope.

---

## The Key-Type Collapse — Settled Against This Document's Earlier Recommendation

> Settled as [D3](#owner-decisions--2026-08-15). Tracked as
> [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication), moved from
> *Proposed — recommended against* to `Scheduled`.

### Scope Verdict — replacing the 18 key-specific DAO classes with six generic families

| | |
|---|---|
| **Verdict** | **In scope.** It changes the shape of the surface that delivers the purpose, not the purpose |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | Six generic families deliver the same plumbing as 18 key-typed closures. The owner has accepted the ergonomic cost at the call site and the breaking change, in a major that is already breaking for four other reasons |
| **Owner's decision** | **Approved**, with **no compatibility wrappers** unless implementation evidence forces reconsideration |

### Why this document argued the other way, and which argument failed

The earlier recommendation rested on three legs. **The load-bearing one is contradicted by the library's
own source**, which is worth stating plainly rather than quietly deleting.

| Earlier argument | Status now |
|---|---|
| **"The `Get` override buys a translatable predicate."** The key-type split was said to exist because a comparison over an open `TIdType` cannot be translated to SQL | **Wrong, and checkable.** [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already does exactly this on the EF Core branch: `Dataset.AsTracking().Single(x => x.Id.Equals(item.Id))` in `Update`, and `Dataset.OrderBy(x => x.Id)` in `GetPaged` — both generic over `TIdType`, both translated. `Int/BaseDao.Get` uses `i.Id == item.Id` only because `int` permits the operator, not because the generic form is impossible. The claim was inherited from the README and restated here without opening `RootDao.cs`. That is the exact failure mode the house rule about affirming inherited claims warns about |
| **"The cost lands on every consumer; the saving lands on one maintainer."** `BaseDao<User>` becomes `BaseDao<User, int>` | **Still true, and the owner accepted it.** It is an ergonomic cost, not a capability cost, and it buys the ability to add a key type without adding a namespace |
| **"Binary-breaking for no behavioural benefit."** | **Still true, and now free.** The same major already breaks on FR 2, 4, 5 and 7. A breaking change that must ride a major is cheapest when a major is already leaving |

### What the collapse must preserve

- **The `Get` predicate must remain translatable.** `x.Id.Equals(item.Id)` is proven in `RootDao.Update`;
  the collapsed `Get` should use the same form rather than `EqualityComparer<TKey>.Default`, which EF Core
  cannot translate. This is the one place where implementation evidence could force D3's escape hatch.
- **The `TKey : struct` constraint on `RootDao<T, TIdType>` is a decision, not an accident.** It excludes
  `string` keys today. The collapse is the moment to decide whether the generic families keep it — and
  relaxing it would also close
  [Example FR 1](../../ProphetsWay.Example/docs/feature-requests.md)'s accepting-half gap. Flagged as an
  [unresolved question](#unresolved-purpose-level-questions); it is not settled by D3.
- **The `using ProphetsWay.EFTools.Guid;` namespaces disappear.** `Guid` as a namespace segment shadows
  `System.Guid` inside those files, which is a small ongoing readability tax the collapse also removes.

### What the 18 classes actually contain — counted 2026-08-19

Recorded here because the shape of the collapse had been reasoned about from file *names*, and the `Interface
Architect` pass is conditioned on it. **The hypothesis "18 classes = 3 key types × 6 shapes" is true as an
inventory and misleading as a description.** Full working in
[FR 10](feature-requests.md#the-18--3-key-types--6-shapes-hypothesis--counted-2026-08-19); the purpose-level
consequence is this:

**Twelve of the eighteen have empty bodies.** `BaseGetAllDao`, `BasePagedDao`, `BaseSoftGetAllDao` and
`BaseSoftPagedDao`, in each of the three namespaces, are a constructor pass-through and one added interface
declaration — no members at all. The remaining six carry **one method each**, `override Get`, and all six
bodies are the same expression modulo the key type. **The total behaviour distributed across 18 public classes
is one method.** Everything else lives in `RootBaseDao`, which already implements all three capability
interfaces on a single type, and in `RootBaseSoftDao`.

**This strengthens D3 rather than qualifying it.** The collapse is not generalizing six behaviours into six
generics; it deletes twelve empty classes, hoists one expression into a `MatchRow` / `KeyEquals` hook, and
keeps six names that [api-contract.md](api-contract.md) **S1** already fixes. The one genuine risk — the `Get`
predicate's translatability — is the *only* thing in the eighteen that has to be reproduced, which is exactly
why D3 names it as the escape hatch.

**The count cross-checks a claim made from the other side.** `api-contract.md` says the 3.0.0 surface is
*"twelve public classes, down from twenty-four."* The twenty-four was verified independently here: 18 keyed,
plus `BaseNonIdDao`, `BaseSoftNonIdDao`, `RootBaseDao`, `RootBaseSoftDao`, `BaseEFContext` and
`BaseEFDataAccess`. Both halves of that reduction are now measured rather than asserted.

### The strongest argument against, recorded so it is weighed rather than forgotten

Six generic families in one namespace is a *less discoverable* surface than three namespaces of six. A
consumer who types `using ProphetsWay.EFTools.Int;` gets an IntelliSense list containing exactly the six
types that can possibly apply to them. After the collapse they get one list of six generic families and
must supply the right `TKey` themselves — the compiler catches a mistake, but later and less kindly than a
namespace that could not express it. If the DAO surface ever grows past six families, revisit this.

---

## Release Ordering — Settled (D11)

> Settled as [D11](#owner-decisions--2026-08-15) on **2026-08-16**. It constrains *when* things ship, not
> *what* they contain, so it changes no entry's scope and no entry's status. It binds two repositories.

### Scope Verdict — sequencing the 3.x release across repositories

| | |
|---|---|
| **Verdict** | **In scope, and it does not widen the purpose sentence.** Release mechanics are not a feature; this is a decision about the order of four existing, already-approved deliverables |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | The owner's goal, stated directly: *"I want to make sure that we deploy the latest changes in the correct order so we only have to build, PR, merge, publish once… whatever each library needs."* Nothing in the order adds work; a wrong order **doubles** it |
| **Owner's decision** | **Approved**, as the four steps below |

### The order

1. **Do all remaining EFTools 3.x work against the live `ProphetsWay.Example` submodule working tree**, not
   against a tagged release of it.
2. **Tag and release `ProphetsWay.Example`** — once EFTools' Entity Framework implementation is **green
   against it**.
3. **Advance this repository's submodule pointer to that tag.**
4. **Pull-request, merge and publish `ProphetsWay.EFTools` 3.0.0** to NuGet.

`ProphetsWay.BaseDataAccess` **3.1.0 is already published and current.** It needs no release in this sequence
unless something new is found in it.

### The reasoning, which is the load-bearing part

**Implementing against a contract is what exposes gaps *in* that contract.** That is not hypothetical here:
**`IDepartmentDao` rule 18 was narrowed on 2026-08-16 as a direct result of Entity Framework design work in
this repository** — a change to `ProphetsWay.Example`'s interface, driven from this side, recorded upstream as
[ProphetsWay.Example FR 14](../../ProphetsWay.Example/docs/feature-requests.md#14--restoring-datetimekind-on-a-department-reached-as-a-navigation-property).

Tagging `ProphetsWay.Example` before EFTools is green therefore risks discovering a **second rule 18** and
needing a second `ProphetsWay.Example` release — which is exactly the double build / pull request / merge the
decision exists to avoid.

### What this changes for a reader of this document

- **Step 1 is why the submodule pointer does not move again yet.** Anyone finding the pointer at `61d9e7d`
  while `ProphetsWay.Example` has moved on is looking at D11 working as intended, not at a stale pointer.
  (**SHA corrected 2026-08-20** — this line named `d845863`, the first advance; the pointer moved to
  `61d9e7d` on 2026-08-18 to pick up `TestDataAccessFactory.Use`.)
- **Nothing here reopens [D9](#owner-decisions--2026-08-15)**, which was about a discarded local modification
  inside the submodule, nor [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts)
  step 1, which has landed. D11 governs the **next** pointer move, which is step 3 above.
- **It does not schedule anything.** No entry moved status because of it.

### The strongest argument against, recorded so it is weighed

Working against a live, untagged submodule working tree means EFTools' 3.0.0 is developed against a moving
target: every upstream edit is immediately live here, with no pinned commit to reproduce a build from. If a
regression appears mid-flight, "which version of the contracts was that against" has no answer until step 3.

The counter is that steps 2 and 3 close exactly that window before anything is published, and that the
alternative — pinning first — buys reproducibility at the cost of a second release of `ProphetsWay.Example`
the moment implementation finds anything. The owner has weighed the pair and chosen the order above.

---

## The Three Shipped 2.2.0 Defects — Settled (D12)

> Settled as [D12](#owner-decisions--2026-08-15) on **2026-08-16**. It closes the 2.2.x-patch question that
> [FR 12](feature-requests.md#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)
> had deliberately left open for the owner, and answers the same question for
> [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) and
> [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance).

### Scope Verdict — cutting a 2.2.1 patch for three shipped defects

| | |
|---|---|
| **Verdict** | **Out of scope.** The 2.2.x line receives no new work; the defects are **documented**, and the remedy offered is 3.0.0 |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | The owner judges the exposure near-zero: *"I don't believe anyone is currently using that library in any meaningful capacity, so it's document the bug and recommend to update to v3.0.0."* This **upholds [D1](#owner-decisions--2026-08-15)** rather than carving an exception from it — FR 12 filed itself as the exception D1 invites someone to test, and it does not survive the test |
| **Owner's decision** | **Approved. No 2.2.1 patch will be cut** |

### The three defects, all live in the published 2.2.0 package

| Entry | Defect | Fails how? |
|---|---|---|
| [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | `BaseEFDataAccess` constructs its `DbContext` via `Activator.CreateInstance` and **never disposes it**. In 2.2.0 `IBaseDataAccess` did not extend `IDisposable`, so there was nowhere to. A leaked context and connection per Data Access Layer instance | Resource exhaustion — visible eventually, and attributable |
| [FR 12](feature-requests.md#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction) | `RootNonIdDao` begins a transaction only when none exists, leaving `_transaction` null otherwise — then `_transaction?.Commit()` / `?.Rollback()` **silently no-op**. The Data Access Object believes it managed a transaction and did not | **Silent.** No exception, wrong data |
| [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance) | `RootBaseSoftDao.Update` is whole-object replacement and **wipes the stored `DeletedDate`**, so `Update` on a soft-deleted row **silently un-deletes it**. A second `Delete` also refreshes the timestamp and returns `1` rather than `0` | **Silent.** No exception, wrong data |

**Two of the three fail silently — no exception, just wrong data.** That is recorded here because it is what
makes documenting them a real obligation rather than a formality: a consumer cannot discover either from a log.

> **A fourth defect was triaged into this set on 2026-08-19, and the owner did not name it.**
> [FR 14](feature-requests.md#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule)
> — the Entity Framework DAO bases **adopt the caller's instance**, so `Insert` leaves the argument tracked and
> `Get` hands back the store's own object under any context not explicitly `NoTracking`. **An edit the caller
> never submitted is written on the next `SaveChanges`.** It is silent and it corrupts stored data, which is
> the same class as FR 12 and FR 13, so **D12's reasoning covers it and its 2.2.x patch is `Rejected` on the
> same grounds.** Three of the four now fail silently.
>
> **What is assumed, and is the owner's to confirm:** that D12 governs *the shipped defects* rather than
> *those three specifically*. Nothing about the fourth distinguishes it from D12's own premise. If the owner
> meant the number literally, the fourth row on the changelog obligation needs their word — and the obligation
> is the only thing that changes either way, since no patch is being proposed under either reading.
>
> **It differs from the other three in one way that matters to `Changelog Author`:** its *fix* is itself a
> behaviour change a 2.2.0 consumer may be relying on. After 3.0.0 those stray edits stop persisting, and an
> `Update` on an absent row returns `0` where it used to throw. The other three take nothing away.

All three were re-verified in source on **2026-08-16** by opening
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs),
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs) and
[RootBaseSoftDao.cs](../ProphetsWay.EFTools/RootBaseSoftDao.cs).

### The consequence, recorded honestly and not softened

**[D7](#owner-decisions--2026-08-15) makes 3.x `net10.0`-only, so a consumer on `net48`, `net8.0` or `net9.0`
cannot take 3.0.0 at all.** "Upgrade to 3.0.0" is therefore **not available to every 2.2.x consumer**. For
those consumers the remedy on offer is the release note and nothing else.

The decision is defensible **because** the consumer base is judged near-empty — **and that premise is the
thing a future reader must be able to see and re-test.** If it ever turns out someone was on 2.2.x with a
`net4x` or `net8.0` target, **this decision was made without them in mind.** That sentence is the point of
this section; do not paraphrase it away.

### The obligation this creates on `Changelog Author`

Not this agent's to execute, and it must not be lost. It is stated in one place —
[feature-requests.md § The `Changelog Author` obligation](feature-requests.md#the-changelog-author-obligation--d12)
— and cited from the three entries rather than restated in each.

> **DISCHARGED — 2026-08-23.** `Changelog Author` wrote the sections and `Purpose Refiner` verified them by
> reopening `CHANGELOG.md`. **All four shipped defects are named as `Fixed`**, each with the silent-failure
> characteristic that made this an obligation rather than a formality, and the **2.2.0 known-issues note is
> present at the top of the v3.0.0 entry with the D7 `net10.0`-only caveat stated plainly** — *"if you are on
> `net48`, `net8.0` or `net9.0` you cannot take it at all"* — together with mitigations for the consumers that
> sentence strands. **The trade D12 made — four patches for four release notes — is paid on both sides.** The
> field-by-field check is recorded once, at the link above, and is deliberately not duplicated here.
>
> **The premise above is still the premise.** Discharging the obligation does not retire the sentence this
> section exists to protect: if a 2.2.x consumer on a `net4x` or `net8.0` target ever surfaces, D12 was taken
> without them in mind, and a written release note does not change that — it is what was offered *instead* of
> a remedy they could take.

---

## The Hand-Written `DepartmentDao` — Settled (D13)

> Settled as [D13](#owner-decisions--2026-08-15) on **2026-08-16**. It resolves the open recommendation in
> [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)
> and **constrains** [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication).

### Scope Verdict — writing a concrete Entity Framework `DepartmentDao` before the generic family

| | |
|---|---|
| **Verdict** | **In scope.** It is step 2 of [FR 1](feature-requests.md#1--advance-the-prophetswayexample-submodule-onto-the-3x-contracts) done in the order FR 13 already recommended, in the proving ground, which this document lists as in scope for the repository |
| **Purpose it's measured against** | The settled sentence above |
| **Because** | In the owner's words: *"the point of the generic tests and classes was to just reduce all the duplicative code, but in this case there is a real need for it."* Deduplication is the *benefit* of the generic families; it is not evidence that a generic family can satisfy `IDepartmentDao`'s 19 rules. FR 13 establishes that today's soft base violates four of them, so the family has to be **derived from something proven**, not designed ahead of one |
| **Owner's decision** | **Approved.** A concrete `DepartmentDao` in `ProphetsWay.Example.DataAccess.EF/Daos/` — verified against the 19 rules — becomes the evidence for what [D3](#owner-decisions--2026-08-15)'s generic soft-delete families must look like |

**Verified 2026-08-16:** `ProphetsWay.Example.DataAccess.EF/Daos/` contains `CompanyDao.cs`, `JobDao.cs`,
`ResourceDao.cs`, `TransactionDao.cs` and `UserDao.cs` — **no `DepartmentDao.cs` and no
`CompanyResourceDao.cs`.** The eight `IDepartmentDao` members and three `ICompanyResourceDao` members are the
throwing stubs on `ExampleDataAccess`. So D13 approves work that has not started.

### What changed to make this safe — the counter-argument has been answered

The objection on record was that this meant **writing throwaway code with no test suite to verify it**. That
objection is now largely answered rather than merely overruled:

**The shape B seam landed upstream on 2026-08-16** —
`ProphetsWay.Example.Tests.TestDataAccessFactory.Use(Func<IExampleDataAccess>)`, verified by opening that file
in the submodule working tree. EFTools can therefore point `ProphetsWay.Example`'s **164 tests** at its Entity
Framework Data Access Layer, and **`DepartmentDaoTests` is the largest class in that suite**, written directly
against the 19 rules. **The hand-written Data Access Object will be verified, not guessed.**

**The qualification, which is not optional:** the seam is **unverified** — nothing has been run through it,
in either repository. See [ProphetsWay.Example FR 13](../../ProphetsWay.Example/docs/feature-requests.md#13--a-seam-letting-another-repository-point-this-suite-at-its-own-implementation),
which is deliberately **not** `Done`. "Verified, not guessed" is the arrangement that now exists on paper; the
first run is what makes it true.

### The strongest argument against, recorded so it is weighed

A concrete `DepartmentDao` written now is code that the [D3](#owner-decisions--2026-08-15) collapse is
explicitly expected to generalize away, and the version of it that survives will be the generic family rather
than this class. Writing it first means writing the same semantics twice.

The counter is that the second writing is a **refactor with a green suite behind it**, and the alternative is
designing six generic families against a specification nobody has yet satisfied once — which is how
`RootBaseSoftDao` came to violate four rules quietly in the first place.

---

## The `Restore` Boundary — Settled (D19)

> Settled as [D19](#owner-decisions--2026-08-15) on **2026-08-19**, **ratified by the owner directly** rather
> than recommended by an agent. It closes a question the 2026-08-18 session left deliberately unanswered —
> *does `Restore` belong on the generic soft-delete family, or stay on `IDepartmentDao`?* — and it constrains
> the `Interface Architect` shape pass on [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication).

### Scope Verdict — adding `Restore` to the soft-delete DAO family

| | |
|---|---|
| **Verdict** | **Out of scope.** It belongs on the consumer's own DAO interface — `IDepartmentDao` — and stays there |
| **Purpose it's measured against** | The [settled sentence](#settled-one-sentence-purpose), and specifically its last clause: *"so an implementer writes only the queries that are specific to their application"* |
| **Because** | In the owner's words: *"`Restore` is only for `IDepartmentDao`, to illustrate a custom method on a consumer's own DAO interface. It is **not** meant to be built into the `ProphetsWay.BaseDataAccess` interface contracts."* This is the out-of-scope test in the purpose sentence returning the answer it was written to return: if a consumer's own DAO can express it, the base class must not |
| **Owner's decision** | **Approved, and the alternative is rejected rather than deferred.** Neither the six generic families nor `ProphetsWay.BaseDataAccess` gains a `Restore` member |

**It is already in the [Out of Scope](#out-of-scope-and-where-it-should-live-instead) table by category** —
*"Query specifications, filter builders, LINQ helpers"* → *"the consumer's **custom DAO methods** — the
deliberate 1%."* D19 names the one member everybody was going to argue about, which is worth more than the
category is.

**And a second reason, which is the owner's boundary rather than this document's:** `Restore` would have had
to land on `ProphetsWay.BaseDataAccess` to be a *contract*, and that repository is the root of the Data Access
family — a member added there is a member every future DAL must implement, including ones with no soft delete
at all. Putting it on the EFTools family alone would have been worse: a base-class capability that no
interface declares, discoverable only by whoever reads the base.

### The two consequences the coordinator surfaced — assessed

Both were put forward as following from D19. **The first is correct and is already satisfied. The second is
correct and is the more valuable of the two.**

#### 1 — the family must leave an accessible seam. **Correct, and already met by the design**

The claim: a concrete DAO must be able to add consumer-authored behaviour on top of the generic family, so the
family's shape must leave a seam — a **shape** constraint on the `Interface Architect` pass, not a semantics
one.

**Assessed and upheld, with one correction: it is not a new constraint.** `docs/api-contract.md` revision 8
already carries the seam, and carries a **worked `Restore` written against it** — read on 2026-08-19, in
[Writing a `Restore`](api-contract.md), not inherited. Four properties of the specified `BaseSoftDao<TEntity, TKey>`
are what make it writable, and each is already a stated decision:

| What `Restore` needs | Already specified as |
|---|---|
| Reach the store from inside a derived DAO | **S10** — `Context` and `Dataset` are `protected`, where 2.2.x had them `public`. The api-contract says in terms that this *"is exactly what makes it writable"* |
| See soft-deleted rows | Starting from the raw `Dataset` keeps `ApplyReadFilter`'s `DeletedDate == null` off the query. The hook composes onto the retrieval trio, not onto everything |
| Locate the row the same way the family does | **A12** — `MatchRow(item)` is `protected virtual`, so a custom member locates rows through the same override every CRUD member uses. The api-contract records an earlier draft that used `KeyEquals(item.Id)` instead and names the tenant-scoping bug it would have caused |
| Not poison the DAL instance | **A26 / OD-7** — detachment in a `finally`, which the sample demonstrates and which the api-contract calls *"the document's only worked custom write"* |

**So the shape pass inherits this constraint rather than being handed it.** What D19 adds is the instruction
**not to relax any of the four** — in particular, not to make `Dataset` or `MatchRow` private on the grounds
that nothing in the library calls them from outside. Something outside the library does: every custom method
the paradigm's 1% is made of.

**The one thing genuinely owed to the shape pass** is that the `Restore` sample must be re-read against the
family as actually written, not assumed to still compile. It is a sample in a design document; nothing has
compiled it.

#### 2 — `Restore` is inside D16's acceptance test. **Correct, and it upgrades D16**

The claim: the 33 `DepartmentDaoTests` that must stay green when `DepartmentDao` converts onto the family
include the `Restore` tests, so *"can a consumer still write `Restore` cleanly"* gets **proven rather than
asserted**.

**Assessed and upheld.** `DepartmentDao.cs` declares `public int Restore(Department item)` — verified by
opening it — and `IDepartmentDao` declares `int Restore(Department item)` at line 271, so the member is part
of the contract the 33 tests are written against. It follows that D16's gate already covers consequence 1:
**if the seam were inadequate, the conversion could not be completed and the 33 could not stay green.**

**This is the strongest thing in the whole D14–D19 set**, and it is worth naming why. Every other statement
about the seam — including this document's table above — is a reading of a design document. The `Restore`
tests are the only mechanism that can *fail*. A shape pass that produces a family on which `Restore` cannot be
written cleanly does not produce a warning; it produces red tests, in a class whose 33/33 is already the
acceptance criterion.

**Two cautions, so the guarantee is not over-read:**

- **It proves the seam is sufficient for `Restore`, not that it is sufficient in general.** `Restore` reads
  one row, writes one column and detaches. A custom method that loads a graph, or projects, or joins, exercises
  parts of the seam these 33 tests never reach.
- **`Restore` is the *only* custom write in the repository**, and the api-contract says so of its own sample
  as well. The evidence base for "a consumer can write custom methods on this family" is one method. That is
  one more than the library has ever had, and it is still one.

### What still blocks the `Interface Architect` shape pass — as of 2026-08-19

Recorded here rather than left in a conversation, because "is it safe to start" is the question every lap of
this cycle opens with and the answer keeps having to be re-derived.

**Blocking: nothing.** Every decision the shape pass consumes is taken.

| What the pass needs decided | Where it is decided |
|---|---|
| The six family names, their generic parameters and their namespace | [api-contract.md](api-contract.md) **S1**, **S3** |
| Whether `TKey` keeps `where TKey : struct` | **S4** — it does not. **This document said the question was open until 2026-08-19; it was not** |
| Whether each family publishes a narrowed method set | **S2** — flat surface, the consumer's DAO interface selects |
| Whether the soft families may use `new` | **A2** — `virtual`/`override`, never `new`. Closes [FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)'s structural finding |
| Whether the collapse carries the corrected write semantics or patches first | **[D14](#owner-decisions--2026-08-15)** — carries them |
| What the families are modelled on, and what proves them | **[D15](#owner-decisions--2026-08-15)**, **[D16](#owner-decisions--2026-08-15)** |
| Whether `Restore` joins the soft family | **[D19](#owner-decisions--2026-08-15)** — it does not |
| Whether the keyless half gets its own root | **S5**, **A1**, **A14** — four classes, two of which implement no capability interface |

**Not blocking, but the pass must not contradict them:** the four seam properties in the table above (S10,
A12, A26/OD-7), and the [counted shape of the 18](#what-the-18-classes-actually-contain--counted-2026-08-19).

**Not blocking, and not the shape pass's to settle — one item, and it belongs to the owner.**
`api-contract.md` **OD-11** is marked *"applied on the `Contract Reviewer`'s recommendation and open to
reversal by the owner."* It narrows the IDENTIFIER RULE's pre-assigned-key case — on a store-generated column,
the generated key replaces whatever the caller assigned. **It is a behaviour term on `Insert`, not a shape**,
so the pass proceeds either way; reversing it retags one test obligation and changes no signature.

---

## Unresolved Purpose-Level Questions

~~**Nothing in this table is open as of 2026-08-19.**~~ ~~**Q5 was opened on 2026-08-24 and is open now.**~~
**Q5 was opened and closed on 2026-08-24 — nothing in this table is open again.** The
original four are closed, and **Q2 and Q3 were closed on
2026-08-15 by decisions this document did not cite** — see the correction below the table. They are kept with
their answers rather than deleted, because all four were raised as blocking questions elsewhere and a reader
arriving from those links needs to find the answer here.

| # | Question | Status | Where it is tracked |
|---|---|---|---|
| **Q1** | **Does this repository take a documented exception to the `netstandard2.0` reach floor?** An EF Core-only library cannot ship that asset | **Closed 2026-08-15 by [D7](#owner-decisions--2026-08-15) — yes.** 3.x targets **`net10.0` only**, library and tests and EF example alike; 2.2.x remains the answer for `net4x`/`net8.0`/`net9.0`. Reasoning in [The `net10.0`-Only Exception](#the-net100-only-exception--settled). **Still owed elsewhere:** the `AGENTS.md` line recording it, which is not this agent's file | [FR 5](feature-requests.md#5--retarget-to-the-house-tfm-standard) |
| **Q2** | **Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one?** | **Closed 2026-08-15 by [api-contract.md](api-contract.md) S7 — yes, it takes the context.** `BaseEFDataAccess<TContext>` accepts a configured `TContext` directly; no `TIdType`, no `Activator.CreateInstance`, and the derived Data Access Layer builds the provider options. Ownership is explicit at construction (**S8**, **A9**). **This row read "Open" until 2026-08-19 and had been stale for four days** | [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) |
| **Q3** | **Do the collapsed generic DAO families keep `where TKey : struct`?** | **Closed 2026-08-15 by [api-contract.md](api-contract.md) S4 — no, the constraint is dropped.** *"Any key type — no `where TKey : struct`."* `string`, nullable-value and value keys are all supported through a provider-translatable equality predicate; **OD-2** settles string equality as the storage engine's collation and **OD-3** settles `default(TKey)` as an ordinary key value. **Also stale until 2026-08-19** | [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) |
| **Q4** | **Is "certified on SQLite and SQL Server" stated on the package, or only in this repository's docs?** | **Closed 2026-08-15 by [D8](#owner-decisions--2026-08-15) — on the package.** Public wording states relational EF Core providers **and** certification on SQLite and SQL Server only, and must not imply any other relational provider is certified. Constraint in [Public Wording](#public-wording--settled). **Note added 2026-08-24: the wording is still not written** — verified against the csproj at `e2f2120` and the README. That is now the correct order, not a lapse; see [the gate](#the-sql-server-certification-gate--settled-d20--d21) | [FR 11](feature-requests.md#11--certify-the-contract-suite-on-sqlite-in-memory-and-a-sql-server-container) |
| **Q5** | 🔴 **Is Azure SQL a certified leg of 3.x, an in-scope-but-uncertified relational provider, or out of scope for this release?** Opened 2026-08-24, when the owner named it as the **production** deployment target alongside local MSSQL for development | ✅ **CLOSED 2026-08-24 by [D22](#owner-decisions--2026-08-15) — a certified leg, and a second release gate.** In the owner's words: *"mssql server and azure sql both need to be covered."* **Both local Microsoft SQL Server and Azure SQL must be certified before 3.0.0 is tagged or published.** The reasoning that made this a purpose-level question rather than a test-matrix one is unchanged and still governs the work: `EnableRetryOnFailure` is effectively mandatory against Azure SQL, and an EF Core execution strategy is **incompatible with a user-initiated transaction** unless wrapped in `ExecuteInTransaction` — which lands on `BaseEFDataAccess<TContext>`'s `TransactionStart` / `Commit` / `RollBack`, i.e. **on this library's public contract**. If Gate 2 exposes that incompatibility, it is a **finding for the owner**, not a contract change an agent takes. Reasoning in [the gate section](#azure-sql-is-a-separate-scope-question-and-it-reaches-this-librarys-own-contract) | **Now filed** as [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql), `Scheduled` and release-blocking. It was deliberately unfiled while the scope call was open; the call has been made, so the work exists |

### Correction, 2026-08-19 — Q2 and Q3 were never open, and this document said they were for four days

**They were closed by owner decisions taken the same day as D1–D9, recorded in a document this one does not
cite as a decision source.** [api-contract.md](api-contract.md) carries **S1–S13**, described there as *"Owner
decisions taken 2026-08-15, numbered S1–S13 so they cannot be confused with D1–D9"* — and **S7 answers Q2 and
S4 answers Q3, each saying so in its own row.** This table went on reporting both as *"Open… answerable by
whoever implements"*, and the sentence beneath it — *"Q2 and Q3 are implementation decisions"* — was wrong
twice over: they are **owner** decisions, and they were **already made**.

**The mechanism of the error is worth more than the error.** This document and
[feature-requests.md](feature-requests.md) treat each other as the whole decision record. **There is a third
register.** An agent reading only these two files sees two open purpose-level questions and either re-asks
them or, worse, answers them itself. Both were live risks for the `Interface Architect` pass: **Q3 decides a
generic constraint on every one of the six families**, and a shape pass trusting this table would have kept
`where TKey : struct` and silently contradicted S4 — excluding `string` keys from a design that specifies
them.

**The standing correction, not just the fix:** `docs/api-contract.md` is a decision source. Its **S1–S13** are
owner decisions taken 2026-08-15, and its **OD-1–OD-11** are owner decisions taken during authoring. Anything
in this document or in `feature-requests.md` describing a question as open must be checked against it before
the description is repeated. **OD-11 is the one term there that is genuinely unsettled** — it is marked
*"applied on the `Contract Reviewer`'s recommendation and open to reversal by the owner"* — and it is the only
one. It concerns whether a store-generated key overwrites a caller's pre-assigned one; it is a term a consumer
reads, and **it is the owner's to confirm or reverse.**

**Nothing purpose-level is waiting on the owner** — with that single exception, which belongs to
`api-contract.md` rather than to this file.

> **Amended 2026-08-24.** That sentence is still true of *scope*: [Q5](#unresolved-purpose-level-questions) was
> opened and closed the same day, and no scope question is open. It is **not** true of *values*.
> [D22](#owner-decisions--2026-08-15) settles that Azure SQL is a certified leg and leaves **eleven
> infrastructure decisions unmade** — subscription, tenant, region, resource-group naming and isolation,
> server and database naming, the Entra/admin model, network access, SKU and budget, retention, credential
> handling, lifetime and teardown, and which repository the infrastructure lives in. They are listed in
> [the gate section](#open-azure-design-decisions--the-owners-and-none-of-them-is-inferable). **None is an
> agent's to choose**, and [FR 19](feature-requests.md#19--certify-the-contract-suite-against-azure-sql)
> cannot start until they are answered.

---

## In Scope

Judged against the proposed purpose sentence. These are what the library is *for*:

- **Abstract DAO bases implementing the `ProphetsWay.BaseDataAccess` capability interfaces** —
  `IBaseDao<T>`, `IBaseGetAllDao<T>`, `IBasePagedDao<T>`. As of [D3](#owner-decisions--2026-08-15) these are
  **six generic families keyed on an open `TKey`**, replacing the 18 `Guid`/`Int`/`Long` closures.
- **Soft-delete DAO bases** that maintain `CreatedDate` / `UpdatedDate` / `DeletedDate` and filter deleted
  rows out of `GetAll`, `GetPaged` and `GetCount`. The `UseUtcTime` constructor flag on `RootBaseSoftDao`
  is in scope: it is a one-bit policy choice with no application knowledge in it.
- **Keyless (`IBaseEntity`) DAO bases** — `BaseNonIdDao<T>`, `BaseSoftNonIdDao<T>` — for join and composite
  entities that have no identifier to `Get` by.
- **A `DbContext` base** that standardizes how a consumer's context is constructed, so the DAL root can
  instantiate a typed context generically — **without naming a provider**, per
  [D2](#owner-decisions--2026-08-15).
- **A DAL root** that wires the parent's reflection dispatch to a real `DbContext` and implements the
  parent's transaction and disposal contracts against it.
- **Whatever obligations `ProphetsWay.BaseDataAccess` 3.x places on an implementation** — `Dispose`,
  the disposal contract, the transaction contract, and the snapshot and ordering rules insofar as they bind
  an EF-backed DAL. Meeting a contract this library advertises is not scope creep; it is the minimum.
- **`ProphetsWay.Example.DataAccess.EF` as a proving ground.** It is not part of the package and must never
  become part of it, but it is in scope for this *repository*: it is how "genuinely reusable" is
  demonstrated rather than asserted.

## Out of Scope (and where it should live instead)

| Not in scope | Where it belongs | Why |
|---|---|---|
| **Choosing a database provider** — the hardcoded `UseSqlServer` and the unconditional `SqlServer` + `InMemory` package references | The **consumer's** `DbContextOptions`; providers as the consumer's own `PackageReference` | Coupling every consumer to SQL Server contradicts the decoupling purpose. Settled by [D2](#owner-decisions--2026-08-15); [FR 7](feature-requests.md#7--stop-forcing-a-database-provider-on-every-consumer) |
| **Cosmos and other non-relational EF Core providers** | Nowhere in this family | The DAO surface assumes `Skip`/`Take` over a stably ordered set, server-side `GetCount`, and the parent's transaction scope semantics. [D2](#owner-decisions--2026-08-15) |
| **A SQL-Server-flavoured convenience entry point, or a `ProphetsWay.EFTools.SqlServer` package** | Nowhere — the consumer's own one-line `UseSqlServer` in `DbContextOptions` | Reintroduces by another name the coupling D2 removes. Previously floated here as defensible; **rejected by [D2](#owner-decisions--2026-08-15)** |
| **EF6 / .NET Framework support in 3.x** | The already-published **2.2.x** package | Two semantics under one package ID. Settled by [D1](#owner-decisions--2026-08-15); [FR 4](feature-requests.md#4--make-3x-entity-framework-core-only--retire-ef6-and-net-framework) |
| **A `ProphetsWay.EFTools.EF6` companion package** | Nowhere — 2.2.x already is it | A second repo, pipeline, version line and changelog for an audience measured at zero. **Rejected by [D1](#owner-decisions--2026-08-15)** |
| **Compatibility wrappers preserving `ProphetsWay.EFTools.Int.BaseDao<T>` et al.** | Nowhere — the major is the migration | Keeping 18 shims to soften a break the owner accepted doubles the surface for the life of 3.x. **Rejected by [D3](#owner-decisions--2026-08-15)**, subject to implementation evidence |
| Migrations, schema generation, seeding, `EnsureCreated` | The consumer's own project, or a `.sqlproj` as in `ProphetsWay.Example` | Schema lifecycle is an application concern; this library reads and writes rows |
| Query specifications, filter builders, LINQ helpers, `Include` strategies | The consumer's **custom DAO methods** — the deliberate 1% | This is the exact boundary the purpose sentence draws. If a DAO can express it, the base class must not |
| **`Restore` — as a member of the soft-delete DAO family, or of `ProphetsWay.BaseDataAccess`** | `IDepartmentDao`, the consumer's own DAO interface, where it already is | The named instance of the row above, and the one most likely to be re-proposed — a soft-delete family that stamps `DeletedDate` looks like it owes an un-stamp. It does not. **Rejected, not deferred, by [D19](#owner-decisions--2026-08-15)**; reasoning in [The `Restore` Boundary](#the-restore-boundary--settled-d19) |
| Async members / `IAsyncDisposable` | `ProphetsWay.BaseDataAccess` first — [BaseDataAccess FR 4](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md) | An implementation cannot add async to a contract it does not own. EFTools **must not lead here** |
| Nested transactions / savepoints | `ProphetsWay.BaseDataAccess` — [BaseDataAccess FR 2](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md), deferred out of scope by decision | Same reason. `EnsureBeginTransaction` already covers the case that motivated it |
| A conformance test kit | `ProphetsWay.BaseDataAccess.Conformance` — [BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md) | Shipping a test framework in a runtime package is the coupling the family exists to prevent |
| Caching, retry/resilience, connection pooling, logging | The consumer, or EF Core's own `ExecutionStrategy` / interceptors / `Microsoft.Extensions.Logging` | Already solved by EF Core and the BCL. Re-solving them is unmaintainable surface |
| A generic repository / unit-of-work abstraction beyond the parent's | Nowhere — the parent's interfaces **are** that abstraction | Adding a second one competes with `IBaseDataAccess` |
| Entity base classes / attributes / a fluent mapping DSL | `ProphetsWay.BaseDataAccess` owns entity markers; mapping is EF Core's `OnModelCreating` | A mapping DSL is a second ORM |
| `FluentAssertions` as a dependency of a non-test project | Nowhere — it is a test-only library and 8.x requires a paid commercial licence | Present today in `ProphetsWay.Example.DataAccess.EF.csproj`. [FR 8](feature-requests.md#8--remove-fluentassertions-from-prophetswayexampledataaccessef) |

### Cannot tell yet — the boundaries that remain open

**None, as of 2026-08-19.** This subsection previously carried **Q2 — who owns the `DbContext`'s lifetime?** —
and described the library as *"not yet been designed to it."* **Both halves are stale.**
[api-contract.md](api-contract.md) **S7** has the Data Access Layer root accept a configured `TContext`
directly rather than build one with `Activator.CreateInstance`, and **S8 / A9** make ownership an explicit
`ContextOwnership` argument with no default — `Owned` is disposed, `Borrowed` never is, and the branch cannot
be inferred. `ContextOwnership.cs` and a `BaseEFDataAccess<TContext>` are **both on disk**, verified
2026-08-19 by listing `ProphetsWay.EFTools/`. The contract was clear, and the library **has** now been
designed to it.

The remaining work on [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) is
not a boundary question at all: it is `ObjectDisposedException` guarding on seven members, which **A19**
specifies and which nothing has yet run.

---

## Recommended Refinements

Numbered to match [feature-requests.md](feature-requests.md). Effort is relative, not calendar time.
"Breaking?" is judged against the **published 2.2.0 package**.

> **Status column rewritten 2026-08-23.** It read *"Every row is now `Scheduled` for v3.0.0"*, which was
> accurate when written and describes a plan rather than a tree. **Ten of the eleven rows have landed.**
> Statuses below are read from the [feature-requests.md index](feature-requests.md#index) as re-verified on
> that date; the *Rationale*, *Effort* and *Breaking?* columns are the original scope judgements and are left
> exactly as they were, because a scope call is only testable if it is still legible after the work.

| # | Change | Rationale | Effort | Breaking? | Status |
|---|---|---|---|---|---|
| 1 | Advance the `ProphetsWay.Example` submodule onto 3.x and bring the EF DAL with it | The paradigm claim is currently a statement about history. Routed here from [Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md) | **Large** | No (repo-internal), but gates everything | **Done** — 2026-08-23, all six steps. Pointer at **`f93f0a4`**, both new entities mapped, both DAOs written. **The "step 1 of 6", the `61d9e7d` SHA and "does not compile in the interim" are all superseded** |
| 2 | `ProphetsWay.BaseDataAccess` `2.5.0` → `3.1.0` | The library advertises a contract it does not reference | Small edit, **large** consequence | **Yes** — transitively | **Done** — and it went **past** this row's destination: both `.csproj` files read **3.2.0**, which is what let the ten `CS8766` suppressions be deleted rather than shipped |
| 3 | Implement the 3.x disposal contract in `BaseEFDataAccess` | Required by #2 to compile; the *design* is the real work | Medium | **Yes** — new abstract obligation on derived DALs | **Done** — 2026-08-23. `Dispose` is `sealed override`, idempotent, non-throwing, rolls back and disposes only when `Owned`; **all ten** other members open with `ThrowIfDisposed()`. **The "seven members (A19)" remainder is closed** |
| 4 | **Make 3.x EF Core-only; retire EF6/.NET Framework** | Two semantics under one package ID; blocks #5 | Medium (deletion) | **Yes** — by intent; 2.2.x remains | **Done** — no EF6 anywhere, and **zero preprocessor directives library-wide**. The dead `#if NET461 \|\| NET471 \|\| NET48` blocks went out with the files carrying them, in one removal rather than two |
| 5 | Retarget to **`net10.0` only** — off `net4x` and the undotted `net80`/`net90` monikers | `net461`/`net471` are EOL; `net80`/`net90` are non-canonical and EOL 10 Nov 2026; EF Core 10 ships only `net10.0` | Medium — the `#if` conditions go with #4 | **Yes** — TFM removal | **Done** — all three projects read `net10.0`, the destination **D7** ratifies. Verified by opening each `.csproj` |
| 6 | Rebuild `ProphetsWay.EFTools.Tests` on the 3.x factory + `Scope` traits | The inheritance hook it uses no longer exists upstream | Medium | No — `IsPackable=false` | **Done** — shape B as [D10](#owner-decisions--2026-08-15) chose it: `TestSeam` + 13 adapters + 2 seam guards + 8 classes written against this library directly. The upstream seam is in use and guarded by a test |
| 7 | Stop forcing `SqlServer` + `InMemory` on consumers | A decoupling library must not pick a provider | Small–medium | **Yes** — consumers add their own provider | **Done** — 2026-08-23, **both halves**, and deliberately **before** the 3.0.0 tag: removing a transitive reference after publishing would have cost a 4.0.0 |
| 8 | Remove `FluentAssertions` 8.2.0 from `ProphetsWay.Example.DataAccess.EF` | Paid licence; test library in a non-test project | Trivial | No — not packaged | **Done** — 2026-08-16; the licence exposure is closed |
| 9 | Delete the stray `[submodule "Submod"]` block in `.gitmodules` | Malformed; will confuse `git submodule` | Trivial | No | **Done** — 2026-08-23. **It cost more than "trivial" implied**: it was disabling Source Link on a published package, three warnings per build. **Removing it stopped the warning and did not give the package Source Link** — that is [FR 16](feature-requests.md), still open |
| 10 | Collapse the `Guid`/`Int`/`Long` triplication into six generic families | Owner-approved; the "untranslatable predicate" objection was **factually wrong** | Medium | **Yes** — accepted, no wrappers | **Done** — 2026-08-23, no wrappers as [D3](#owner-decisions--2026-08-15) required. The folder is flat and every file declares `namespace ProphetsWay.EFTools`; the three key-typed namespaces no longer exist |
| 11 | Certify on SQLite in-memory + a SQL Server container; retire blanket `LocalTestsOnly` | `InMemory` cannot honour transactions, so it cannot verify the contract this package now claims; **D8** makes the certification a public claim, so it must be earned | Medium | No | 🔴 **Scheduled — RELEASE-BLOCKING as of 2026-08-24 ([D20](#owner-decisions--2026-08-15)).** The **SQL Server** whole-suite run gates the 3.0.0 tag. ~~narrowed 2026-08-23, and explicitly not release-blocking~~ — **struck; do not restate.** The **SQLite** whole-suite leg and the `LocalTestsOnly` half are still owed and are **not** blockers. **Its "fact 2" is still false** — the package depends on no provider at all |

**These are not independent.** #2 forces #3; #4 unblocks #5; #1 forces #6; #7 forces #11. The realistic
unit of work is **one v3.0.0 release containing #1–#11**, with nothing deferred out of it except the
pipeline edits inside #11, which belong to another owner. **That prediction held**: ten of the eleven landed
together in one indivisible release, and the only row that did not is the one whose remainder is CI evidence
rather than surface.

**Row 12 is absent from this table and that is deliberate.**
[FR 12](feature-requests.md#12--rootnoniddaoensurebegintransaction-silently-no-ops-against-a-pre-existing-transaction)
requests no change to any file here — the 3.x design deletes the members carrying the defect, so its only
deliverable is a line in the release notes. It was triaged on 2026-08-16 to `Scheduled` for that obligation,
with the 2.2.x patch `Rejected`; there is nothing for a refinements table to hold.

**Row 13 is likewise absent, for a different reason.**
[FR 13](feature-requests.md#13--the-soft-delete-and-keyless-dao-bases-cannot-serve-the-3x-contracts-by-inheritance)
is a **constraint on row 10**, not a change of its own: it specifies what the six generic families must fix
rather than proposing a separate edit. It also carries a release-note obligation of its own, for the same
reason FR 12 does.

**Row 14 is absent for the same reason as row 13, and it is the more important absence of the two.**
[FR 14](feature-requests.md#14--the-entity-framework-dao-bases-adopt-the-callers-instance-violating-the-snapshot-rule)
specifies what row 10 must fix on the **ordinary CRUD** path, as row 13 does for the soft-delete and keyless
bases. Between them the two cover **every base in the package**, and neither proposes an edit of its own —
[D14](#owner-decisions--2026-08-15) forecloses fixing these types in place, because row 10 deletes them. It
also carries a release-note obligation, which is now the fourth on that list.

**Five 2026-08-18 decisions and one from 2026-08-19 land on this table without moving a row, and that is again
the correct outcome.** [D14](#owner-decisions--2026-08-15)–[D18](#owner-decisions--2026-08-15) and
[D19](#owner-decisions--2026-08-15) constrain **how row 10 is executed** — collapse rather than patch-then-
collapse, families derived from the hand-written `DepartmentDao`, that conversion as the acceptance test, the
line for hand-writing a DAO, one test suite, and `Restore` staying on the consumer's interface. None changes a
row's scope, effort or breaking-ness. Ratification and one amendment:
[Ratification of D14–D18](#ratification-of-d14d18--2026-08-19).

**Three 2026-08-16 decisions land on this table without moving a row either, for the same reason.**

- **[D11](#owner-decisions--2026-08-15)** sequences the release across repositories. It changes *when*, not
  *what*, so no row's scope, effort or breaking-ness moves. See [Release Ordering](#release-ordering--settled-d11).
- **[D12](#owner-decisions--2026-08-15)** settles the shipped 2.2.0 defects as **documented, not
  patched** — **three when it was taken, four since FR 14 was triaged on 2026-08-19**. Rows 3 and 10 both
  carry a defect that is live in the published package; neither gains work here, because the fix rides 3.x and
  the *record* rides the changelog. See
  [The Three Shipped 2.2.0 Defects](#the-three-shipped-220-defects--settled-d12), whose count is corrected in
  place rather than in its heading.
- **[D13](#owner-decisions--2026-08-15)** constrains **row 10** without changing its status: the six generic
  families are now to be **derived from a hand-written concrete `DepartmentDao`** proven against
  `IDepartmentDao`'s 19 rules, rather than designed ahead of one. That is a sequencing constraint inside row 1
  step 2 and row 10, not a new row. See [The Hand-Written `DepartmentDao`](#the-hand-written-departmentdao--settled-d13).

---

## Stale Inherited Claims

Corrected here because this document's charter is markdown under `docs/`. **`AGENTS.md` and `README.md` are
not this agent's files to edit** — these are reported for their owners.

> **Two rows below had themselves gone stale by 2026-08-23 and are corrected in place — a table of stale
> claims is the last place that should carry one.** They are marked rather than deleted, because the point of
> the table is the audit trail.

| Claim | Where | Status | Evidence |
|---|---|---|---|
| "`ProphetsWay.Example` is **vendored** here" | `AGENTS.md`, Known Deviations #1 | **False** | [.gitmodules](../.gitmodules) declares `path = ProphetsWay.Example`, `url = …/ProphetsWay.Example.git`, `branch = main`. It is a submodule. It cannot drift; it is *pinned*. `ProphetsWay.Example` corrected the same claim from its side |
| "Two copies … drift independently" | `AGENTS.md`, Known Deviations #1 | **False**, follows from the above | The problem is **coordination**, not duplication |
| "**This is the most modern repo in the family** … targets `net9.0` … When conventions conflict, prefer this repo's approach" | `AGENTS.md`, This Repo | ~~**Stale, and actively harmful as guidance**~~ — **this cell's own rebuttal is now stale too; corrected 2026-08-23** | The original claim is still wrong for its original reason — no repository should be preferred by default. **But the evidence this cell offered has expired.** It read *"This repo is at `net461;net471;net48;net80;net90` with **no `netstandard2.0`** and references the parent at 2.5.0. It is now the **least** modern of the three."* **All three clauses are false as of 2026-08-23**, verified by opening the `.csproj` files: TFMs are **`net10.0`** across all three projects, and both consuming projects reference `ProphetsWay.BaseDataAccess` **3.2.0**. The absence of `netstandard2.0` is now the **ratified D7 exception**, not a symptom of neglect. **Do not restate the old TFM list as current** |
| "EFTools carries an EF implementation of the very same `IExampleDataAccess`, and the tests do not change" | `ProphetsWay.Example/README.md` | ~~"Pending, not permanently false"~~ — **TRUE, 2026-08-23. The claim has landed and this cell's rebuttal is dead** | **Every fact this cell carried is superseded and none may be restated.** The pointer is **`f93f0a4`** — read from `.git/modules/ProphetsWay.Example/HEAD` on 2026-08-23, **not `61d9e7d` and not `d845863`**. Both `.csproj` files reference `ProphetsWay.BaseDataAccess` **3.2.0**, not 3.1.0. **`CompanyResourceDao` exists** — `ProphetsWay.Example.DataAccess.EF/Daos/CompanyResourceDao.cs` — so the three forwarders no longer throw. **The "roughly 28 of the 151 harness tests are red" figure is dead**; the suite is **270 / 270 / 0** with the conformance gate at **245 / 245 / 0**. [FR 1](feature-requests.md) and [Example FR 5](../../ProphetsWay.Example/docs/feature-requests.md) are both `Done` |
| "The pipeline is green" as evidence the tests ran | general | **Misleading** | `LocalTestsOnly: 'yes'` in [app-variables.yml](../app-variables.yml) — CI skips them. **Still true, and its justification has weakened**: the conformance gate now runs in about two seconds and most locally written classes use SQLite in-memory and need no server — see [FR 11](feature-requests.md) |
| `docs/architecture.md`, per-project `docs/requirements.md` | house convention | **`n/a`, not missing** — ratified by [D5](#owner-decisions--2026-08-15) | Library repo, not a multi-project application solution. The owner has confirmed this document plus `AGENTS.md` and the README are sufficient |
| `docs/nuget-extraction-proposal.md` | house convention | **`n/a`, not missing** | No candidate clears the dependency test. See [the extraction verdict](#the-extraction-verdict--docsnuget-extraction-proposalmd-is-na-not-missing) |
| `docs/repo-profile.md` | house convention | **Present** — corrected 2026-08-15, re-verified 2026-08-16 | It was absent when this document's first pass ran, which is why that pass read source directly. `Repo Analyst` has since produced it, dated 2026-08-15. Its findings **agree** with this document on every overlapping claim — the EF6/EF Core `Update` divergence, the hardcoded `UseSqlServer`, the InMemory reference and the malformed `[submodule "Submod"]` block. **Its submodule rows were corrected on 2026-08-16**, after the pointer advanced; see the note below |

### Factual note — the submodule pointer advanced on 2026-08-16

Recorded here because several statements in this document were written against the older pointer and a
reader needs the correction in the same place as the text. **No decision or status below has been changed
by this note; that is `Purpose Refiner`'s to do.**

> **The pointer has advanced twice more since, and is now `f93f0a4`** —
> `f93f0a41a76834647962ddf9e830e01e24e05f24`, read from `.git/modules/ProphetsWay.Example/HEAD` on
> **2026-08-23**. `git submodule status` describes it as `3.1.0-4-gf93f0a4`: **four** commits past that tag, on
> the open, untagged **3.1.1** line. It carries `TestDataAccessFactory.Use` and the `StoreCapabilities` enum
> this repository's test harness now depends on. **`d845863` and `61d9e7d` are both history and neither may be
> restated as current.** The three bullets below are kept as the record of what the *first* advance broke; all
> three are closed. **D11 step 3 wants the pointer on a tag before release — that is release mechanics, and it
> is the one part of this note still forward-looking.**

The `ProphetsWay.Example` submodule is now at **`61d9e7d`** — **SHA corrected 2026-08-20**; this line read
`d845863 — the 3.1.0 tree`, which was the first advance on 2026-08-16. The current pointer is **one commit
past the `3.1.0` tag** (`3.1.0-1-g61d9e7d`), the 2026-08-18 merge of `ProphetsWay.Example` PR #21 that
opened the untagged `3.1.1` line. Verified by reading
`.git/modules/ProphetsWay.Example/HEAD` and the checked-out working tree. Three consequences are facts
about the repository as it stands:

- **Step 1 of FR 1 has landed; steps 2–6 have not.** The recommended-refinements table below still reads
  `Scheduled` for that row, which is now a status that trails reality rather than a false statement of it.
- ~~**The repository does not compile.**~~ **Superseded 2026-08-16, corrected here 2026-08-19.** It was true
  when written — `ProphetsWay.EFTools.Tests` targeted `net472;net48;net80;net90` against a `net48;net10.0`
  project reference and overrode an upstream member that no longer existed, and `ExampleDataAccess` did not
  satisfy the 3.1.0 `IExampleDataAccess`. **All three breaks closed the same day**, and
  `dotnet build ProphetsWay.EFTools.sln -c Debug` was green on SDK 10.0.400. Left visible rather than deleted
  because two later paragraphs in this document reason from it.
- ~~**No EFTools-owned `.cs` or `.csproj` has been changed to match.**~~ **Also superseded, and only one
  clause of it survives.** The reference is at **3.1.0**, the TFMs are `net10.0`, `BaseEFDataAccess` has a
  `Dispose`, and the Data Access Layer root has been rebuilt as `BaseEFDataAccess<TContext>` with a
  `ContextOwnership` enum beside it — verified 2026-08-19 by listing `ProphetsWay.EFTools/`. ~~**What is still
  true: the EF6 `#if` branches are still in the C# sources, dead under a `net10.0`-only build, and the 18
  key-specific DAO classes are still there.**~~ **Both clauses are false as of 2026-08-23 and neither may be
  restated.** The 18 key-specific classes and the `#if` blocks went out together in the deletion lap; the
  library is **15 files in one flat folder with zero preprocessor directives**, and the reference is at
  **3.2.0**. Nothing in this bullet is outstanding.
| "The key-type namespaces are **required** so the default `Get` can build a proper select by Id" | `README.md`, and **restated by this document** in its first pass | **False** | [RootDao.cs](../ProphetsWay.EFTools/RootDao.cs) already compares generically on the EF Core branch — `Single(x => x.Id.Equals(item.Id))` in `Update`, `OrderBy(x => x.Id)` in `GetPaged`. `Int/BaseDao.Get` uses `==` because `int` allows it, not because a generic form is untranslatable. **This document inherited the claim from the README without opening `RootDao.cs`**, and it was the load-bearing argument in the recommendation the owner overturned as [D3](#owner-decisions--2026-08-15) |
| "Retarget to `netstandard2.0;net10.0`" as this repo's house-standard destination | this document, first pass; house convention | **Unachievable here — and now a ratified exception, not a violation** | `ProphetsWay.EFTools.csproj` pinned `Microsoft.EntityFrameworkCore` **9.0.4** when this was written and reads **10.0.11** as of 2026-08-23; neither ships a `netstandard2.0` asset — EF Core has been runtime-targeted since 5.0, and **EF Core 10 exposes only `net10.0`**. An EF Core-only library cannot carry the family's reach floor. Raised as **Q1**; **closed by [D7](#owner-decisions--2026-08-15)** — the destination is `net10.0` alone, **and the tree has reached it**. `AGENTS.md` now carries the line recording it |
