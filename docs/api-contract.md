# API Contract — ProphetsWay.EFTools 3.0.0

**Status: Stage 2 — Revision 8, *under review*. Revision 7 passed a `Contract Reviewer` review *with
findings*, and Revision 8 closes those findings. No pass has run against *this* text. Nothing here may be
described as "closed" or "passed" on Revision 8's own account until one does.**

> **The status line on Revision 3 said "Stage 2 closed; passed `Contract Reviewer`" and that claim was
> false.** It was recorded before the revision it described was written. An independent review of the text
> as it actually stood returned **BLOCK** with five blocking, eight significant and seven minor findings.
> The status line is therefore a statement of *where this document is in the workflow*, not a statement of
> its quality, and it is not the authoring agent's to advance.

**Revision 8** answers a `Contract Reviewer` review of Revision 7, which returned **PASS WITH FINDINGS** —
two blocking, three significant and two minor, keyed **H1–H9**, plus two findings carried forward from
earlier passes, **G11** and **G12**. Two of them were not the author's to close and went to the owner:
[**OD-8**](#owner-decisions-taken-during-revision-8) **retracts A26's fetched-graph clause** — `Update`'s
locating fetch applies neither `ApplyReadFilter` nor `ApplyIncludes` and never tracks `item`, so every
navigation on the row it fetched is `null` and there was no graph for the clause to describe — and
[**OD-9**](#owner-decisions-taken-during-revision-8) **replaces the `CompanyResource` counter-example with a
purpose-built entity**, because `CompanyResource` has two mapped scalars, both of them in its `MatchRow`
predicate, and no navigation property at all. The rest are author fixes. Nothing is restructured.

**The change of shape is that every test obligation now carries a `Scope` tag.** The obligations preamble had
claimed the groups matched the `Contract` / `Characterization` / `Dispatcher` partition; they are subject-area
groups, and about 125 of 142 obligations had no assignable scope at all. Scope is now stated per obligation,
assigned by `ProphetsWay.Example`'s own traceability rule, and the per-scope counts are published so a
translated suite can be checked against them the way that repository checks its own.

**Revision 7** answers a focused `Contract Reviewer` delta review of Revision 6, which returned **PASS WITH
FINDINGS**: the OD-7 sweep found eight of nine sites, the `R4-S*` renumbering left no dangling citation, the
observation seam is a real buildable mechanism, A30 and A31 are sound, and **141 of 142** obligations were
cleared for authoring with two gating the rest. Eight findings (**F1–F8**) are closed here. Nothing is
restructured — six revisions in, the risk is churn rather than under-specification.

**The largest of them is a fact about another repository that moved under this document: the
`ProphetsWay.Example` retrait has LANDED.** Revision 6 was written as though it were still pending, and one
of the sentences that assumed so sat *inside a test obligation*, telling its author to expect a red Example
test — the exact condition under which a genuine regression is waved through.
`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` now carries no class-level `Scope` trait, declares them
per method, and the cascade assertion is a `Characterization` fact named
`ShouldReadANavigationPropertyEditBackInsideTheTransactionThatSubmittedIt`. A second retrait has since landed
in `UserDaoTests.cs`. **Counted directly from the standalone `ProphetsWay.Example` tree rather than taken
from a report: 164 tests — Contract 139, Characterization 5, Dispatcher 20 — over `net10.0` and `net48`,
328 executions.** *"The Example suite is green"* **is** now the right gate.

**Revision 6** answers the third `Contract Reviewer` pass, which returned **PASS WITH FINDINGS** against
Revision 5: all twelve Revision 4 findings verified closed against source, OD-4 and OD-5 judged implemented
with mechanism, and obligations up from 106 to 135 with **zero blocked groups**, down from three. The verdict
on readiness was *"Yes — start now,"* and the instruction with it was explicit — *"none requires a design
change, and none requires the owner. A fourth full revision cycle is not warranted. Fix in place."* This is
therefore a **correction pass**. Nothing that was working has been restructured.

Two owner decisions were taken during it, neither of them raised by that review.
[**OD-6**](#owner-decisions-taken-during-revision-6) closes the `Update` cascade question **in favor of the
current design** — `Update` writes the root only, and the conflicting `Scope=Contract` assertion was a
`ProphetsWay.Example` defect. The owner authorized its retrait **in that repository**, and as of Revision 7
that retrait **has landed**: the assertion is `Characterization`, and the suite is green.
[**OD-7**](#owner-decisions-taken-during-revision-6) **reverses** A26's failure clause: detachment now happens
in a `finally`, on success **and** on failure, so a failed write cannot poison the Data Access Layer instance.

The remaining eleven changes are localized. The largest by value: **`ToQueryString()` was prescribed on nine
obligations whose members return no queryable** — no public member of this library returns an `IQueryable` —
and is replaced by one stated observation seam in
[Observing the generated SQL](#observing-the-generated-sql--the-seam).

**Revision 5** answers the review of Revision 4, which returned **BLOCK, narrowly**: eighteen of the twenty
prior findings confirmed closed, both refusals accepted as reasoned, both 2.2.0 defect retirements verified
rather than taken on assertion, and eight of eleven test groups authorable. Three groups remained blocked —
hooks and overrides, navigation loading, snapshot and tracking — on twelve findings the reviewer summarized as
*"everything blocking is a localized statement, not a design flaw."*

The changes with teeth: **the write half of the deep-snapshot rule is specified for the first time**
([OD-4](#owner-decisions-taken-during-revision-5), A24–A26) — `Insert` writes the root and nothing reachable
from it, `SetValues` is **declared** unable to repoint a relationship, and "detaches what it touched" is
defined as the whole reachable graph; **global query filters get a named rule**
([OD-5](#owner-decisions-taken-during-revision-5), A28) — `IgnoreQueryFilters()` on every `MatchRow`-located
path, composed with on the retrieval trio, and documented as a **conflicting** mechanism rather than a
sanctioned alternative to `ApplyReadFilter`; the **per-hook input contract** (A23) replaces the "every hook
receives the raw `Dataset`" sentence Revision 4 left standing in three places while writing its opposite in
three others; `GetCount`'s relationship to `ApplyStableOrder` gains a **stated mechanism** (A27); the
`AsNoTracking()` identity-resolution trap is named rather than dismissed; and the split-query position is
recorded (A29).

**Revision 5 opened one question with the owner. It is now closed.** Reading
`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` against the write half turned up a `Scope=Contract`
assertion that the design as written cannot satisfy; **[OD-6](#owner-decisions-taken-during-revision-6)
resolved it in favor of the current design** — see
[The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6). The write-side group is
unblocked, and no group in this document is blocked.

**Revision 4** closed the review before it. The changes with teeth there: **navigation loading becomes a first-class
cross-cutting rule** with an opt-in `ApplyIncludes` hook (OD-1, A18) — Revision 3 mandated `AsNoTracking()`
on every read while loading no navigation property anywhere, so a conforming implementation would have
thrown `NullReferenceException` against the Example's deep-snapshot suite on both certified providers;
`ThrowIfDisposed()` moves onto the **seven inherited dispatcher members** instead of being delegated to the
consumer's forwarders (A19); `ApplyReadFilter` gains a stated default and a **fixed** filter → include →
order composition (A20); string-key equality is settled as **collation-defined** (OD-2); `default(TKey)` is
settled as an **ordinary key value with no short-circuit** (OD-3); `RootSoftNonIdDao.UpdateCore` is
**unsealed** (A21); and `SetValues` is fixed as the `Update` mechanism because `ExecuteUpdate` cannot
satisfy the row-existed rule on both certified legs (A22).

Decisions [A9–A31](#design-decisions-made-here) carry the detail. No settled owner decision (S1–S13,
D1–D9) was reopened; nine new owner decisions ([OD-1–OD-3](#owner-decisions-taken-during-revision-4),
[OD-4–OD-5](#owner-decisions-taken-during-revision-5),
[OD-6–OD-7](#owner-decisions-taken-during-revision-6),
[OD-8–OD-9](#owner-decisions-taken-during-revision-8)) were taken to close findings this document could not
close itself. **OD-7 and OD-8 each reverse something an earlier revision stated** — a rule and a clause
respectively — and they are the only two places in this document where a later revision contradicts an
earlier one on purpose; in both the superseded wording is retracted rather than left standing.

### Revision Log

Keyed to the review's finding identifiers so the next reviewer can check this revision against the list
rather than re-derive it.

**On the identifiers.** Revision 4's log below uses `R4-S1`–`R4-S8` for that review's *significant* findings.
They were written as bare `S1`–`S8` and collided with the **settled owner decisions** `S1`–`S13`, so five
obligations cited a finding and resolved to an unrelated decision. The prefix is the fix (Revision 6,
Finding 5). A bare `S`*n* anywhere in this document now means a **settled decision** and nothing else.

#### Revision 8

A `Contract Reviewer` review of Revision 7 returned **PASS WITH FINDINGS** — two blocking, three significant
and two minor, keyed **H1–H9** — together with two findings carried forward from earlier passes, **G11** and
**G12**. Two of the nine could not be closed by an agent and were put to the owner as
[OD-8 and OD-9](#owner-decisions-taken-during-revision-8). Every other row below is an author fix.

| Finding | What Revision 8 did |
|---|---|
| **H1** | **Blocking, and closed by [OD-9](#owner-decisions-taken-during-revision-8).** The `CompanyResource.CompanyId` obligation was **unauthorable for three independent reasons**: `CompanyResourceDao` derives from `RootNonIdDao<CompanyResource>`, which publishes neither `Get` nor `Update`; `ICompanyResourceDao` declares no `Update` at all; and — the structural one, which survives even a purpose-built `BaseNonIdDao<CompanyResource>` — **both of the entity's two mapped scalars sit inside its `MatchRow` predicate**, so changing either makes the locating query search for the new value, find nothing and return `0`. **There is no column an `Update` could write.** A25's counter-example and the obligation are rewritten against a **purpose-built entity, `Assignment`** — a navigation, its foreign key declared explicitly, and a third writable non-key column — stated as purpose-built and **not present in `ProphetsWay.Example`**, matching the device the split-query obligation already uses |
| **H2** | **Blocking, and closed by [OD-8](#owner-decisions-taken-during-revision-8).** *"Mutate the graph hanging off the entity `Update` fetched"* had no subject: the locating fetch applies **neither `ApplyReadFilter` nor `ApplyIncludes`** and `item` is **never tracked**, so no fix-up partner exists and **every navigation on the fetched row is `null`**. A26's *"plus any fetched row's whole reachable graph"* clause is **retracted**, on OD-7's precedent — stated as withdrawn, with the reason, rather than quietly edited. The obligation is rewritten against **`AutoInclude`**, which per H7 is the one route that genuinely populates a fetched graph, and asserts through the change tracker because the fetched row is never handed to the caller. The `Detachment` rows on `Update` and `Delete` and the `What` row on A26 are made consistent with the retraction |
| **H3** | **The `ProphetsWay.Example` sweep moved the counts and not the rules.** `IExampleDataAccess` carries **four** DAL-wide rules; SNAPSHOT RULE was cited three times and **IDENTIFIER RULE and ROW COUNT RULE zero times**. Both are now cited by name where the behavior was already specified — `Insert`'s write-back row and its CRUD obligation, `Update`'s and `Delete`'s `Returns` rows, `Update`'s no-op subtlety, A22's justification (which is the ROW COUNT RULE's own reasoning nearly verbatim), and the soft-delete deltas, where `IDepartmentDao` rules 2/4/5/6 are that rule applied to a narrower notion of a match. Both are marked as **elected in `ProphetsWay.Example`**, not inherited from `ProphetsWay.BaseDataAccess`, which documents the write-back as *"a convention left to the implementation"* and `Update`/`Delete` as returning *"typically 1"*. **One substantive under-statement fell out and is fixed:** *"never greater than `1`"* was stated for `Delete` and not for `Update` |
| **H4** | **Corruption residue in a region reported closed.** Within the F6 soft-delete obligation, *"This is the guard on the mechanism: `SetValues` copies every mapped scalar by name…"* appeared **twice**, as did *"can pass on an instance that happened to carry the stored value"*. Second occurrence of each deleted, `and rewrites its creation time` folded onto the surviving sentence, and the `CreatedDate` instruction kept intact. The surrounding obligations were swept for other repeated sentences; none found |
| **H5** | The recovered keyless **Migration** sample gains `public CompanyResourceDao(DbContext context) : base(context) { }`. `RootNonIdDao<TEntity>` declares `protected RootNonIdDao(DbContext context)` and no parameterless constructor, so the sample did not compile; every other Migration sample carries one, as does the same class in the [`ICompanyResourceDao`](#icompanyresourcedao--the-shape-this-exists-to-serve) section |
| **H6** | **The obligations preamble claimed a grouping the document does not have.** The groups are **subject-area** groups — key predicate, hooks, navigation loading, CRUD, soft delete — and only one names a scope. Corrected, **and every obligation now carries a tag**: `[C]` / `[X]` / `[D]`, assigned by `ProphetsWay.Example`'s traceability rule (a `Contract` assertion must trace to a stated rule; otherwise it is `Characterization`). The per-scope counts are published in the preamble, because that repository's integrity check is that the three sum to the total and **there is no analyzer behind it** |
| **H7** | **`AutoInclude` reaches the write path, and the document said the opposite by omission.** It is applied at query compilation and reaches *"every query against that entity, including the ones this library builds"* — and `Update` and `Delete` build a tracked fetch against that entity. A row is added to the `AutoInclude` table stating so, with the consequence named plainly: a write silently tracks and detaches a larger graph, **per consumer, on a model this library cannot see**. The cross-cutting *"`Update` attaches nothing"* claim is qualified to *"attaches nothing of its own"*, and the `Who applies it` row, `Update`'s `Mechanism` row and `Delete`'s implementation note follow it. This is what H2's replacement obligation is written against |
| **H8** | The `Restore` sample never said that **starting from `Dataset` is what keeps `ApplyReadFilter` off the query** — the one property it cannot do without, since `BaseSoftDao.ApplyReadFilter` excludes exactly the soft-deleted rows `Restore` exists to reach. Stated at the sample. The note's conflation of `item` and `stored` is corrected: `item` is in A26's scope but is **never tracked** by this method, so `stored` is the whole of what the `finally` owes |
| **H9** | **Closed with H1.** A25 concluded *"a relationship expressed as a foreign-key property is repointable through the ordinary path"* from an entity that **declares no navigation property at all** — verified by opening `ProphetsWay.Example.DataAccess/Entities/CompanyResource.cs`, which carries `public int CompanyId` and `public Guid ResourceId` and nothing else. It demonstrated that a scalar is writable and nothing about repointing. `Assignment` carries the navigation, so the conclusion now follows from the example that states it |
| **G11** | **Carried forward, still open, now closed.** The obligation pinning the OD-7 discard turns on the provider raising a referential-integrity exception. `Microsoft.Data.Sqlite` enables `PRAGMA foreign_keys` on connections it opens, but the obligation did not say so and the SQLite-limitations table never mentioned foreign-key enforcement — build the context without it and the `Insert` succeeds, the exception never fires, and the obligation silently stops testing OD-7. A clause requiring enforcement to be **asserted in the arrangement** is added, and a row to the SQLite table |
| **G12** | **Carried forward, still open, now closed.** `NormalizeRetrievedTimestamp` is declared on `BaseSoftDao` and `RootSoftNonIdDao` only, so a `Department` materialized through **`UserDao`**'s includes — a hard DAO with no such hook — returns its timestamps as the provider gave them, `Unspecified`, while `IDepartmentDao` rule 18 requires `Kind == Utc`. **The same shape as N10**, and stated where N10 is stated: an included soft entity bypasses **both** the including DAO's inaccessible `ApplyReadFilter` **and** `NormalizeRetrievedTimestamp`. The overclaim that normalization reaches *"every entity a read materializes"* is corrected to **that Data Access Object's own reads**, and an obligation is added beside N10's — `[X]`, deliberately, because pinning it `Contract` would oblige every future implementation to reproduce a gap |
| **Obligation count** | **142 → 143.** H1 and H2 **rewrote** obligations rather than adding them, G11 **extended** one with a clause, H4 deleted duplicated sentences from inside one, and **G12 added the only new checkbox**. Recounted item by item rather than carried forward: **`Contract` 124, `Characterization` 11, `Dispatcher` 8 — 143**. None is blocked |

#### Revision 7

A focused `Contract Reviewer` delta review of Revision 6 returned **PASS WITH FINDINGS** — eight items, two
of them gating a test obligation. Keyed **F1–F8**. No section was added or restructured; the two owner
decisions and every design decision stand unchanged.

| Finding | What Revision 7 did |
|---|---|
| **F1** | **Blocking. The `ProphetsWay.Example` retrait has landed, and Revision 6 was written as though it were pending.** Four false statements struck: `SnapshotDeepCopyTests` being traited `Contract` **at class level**; a conforming implementation *"will fail that one `Scope=Contract` test"*; *"expect that one Example test to fail until the retrait lands"*; and the same claim in the `Purpose Refiner` block. Five stale-tense sites swept with them — *"a separate `Test Designer` pass is handling it"*, *"is being retraited"*, and three in the header and this log. **The author's note inside the unblocked OD-6 obligation is deleted outright rather than softened** — a note telling a test author to expect a red Example test is the condition under which a genuine regression is waved through. The collision is now stated in the past tense: it *was* `Contract`, it *is* `Characterization`, the retrait landed, a second landed in `UserDaoTests.cs`, and **"the Example suite is green" is the right gate**. Counts measured directly against the standalone repository: **164 / 328 / Contract 139 / Characterization 5 / Dispatcher 20**. The relational-store analysis is **kept** — it is the durable part and the justification for the retrait |
| **F2** | **Blocking. The OD-7 obligation's failure mode made its own assertion unobservable.** A unique-constraint violation is not removable: the entity retained as `Added` violates the same constraint on the next `SaveChanges`, so step two throws and *"the failed row appears"* is never evaluated. Changed to a **removable** failure — `Insert` a graph whose related entity names no stored row, take the referential-integrity exception this document already specifies, then insert the missing principal through the other Data Access Object and let `SaveChanges` run. Under the retracted rule the pending dependent goes in with it and the assertion is **directly assertable**. The paired retry obligation is corrected: it **passes vacuously** on a broken implementation, because the still-tracked `Added` entity is the object the caller fixed, so it is no longer credited with discriminating power |
| **F3** | **The ninth OD-7 site — the `Restore` sample**, and the document's only worked custom write. It detached on the **success path only**, and its early guard returned having tracked `stored` via `AsTracking()` without detaching at all; the note below it conditioned the `finally` on *"a custom method that loads a graph"*, which the sample is not. Detachment moved into a `try`/`finally`, the guard split so the `return 0` path detaches too, and the note generalized: **the `finally` is owed by every custom write**, and the graph clause is about *what* to detach, not *when* |
| **F4** | **A26's Failure row overstated the mechanism.** *"The tracker is returned to the state it was in before the call"* contradicts its own second half — an entity tracked before the call that the write's walk reaches is **detached, not restored**, and the `Restore` sample is exactly the shape that produces one. First clause deleted; the rule kept and the pre-tracked case named |
| **F5** | **The observation seam lumped two routes of unequal fidelity.** `DbCommandInterceptor` and `DbContextOptionsBuilder.LogTo` shared one row claiming a parameter collection; `LogTo` yields formatted text and redacts values without `EnableSensitiveDataLogging()`. Row split in two, and the parameterization obligation now names **`DbCommandInterceptor` specifically**. The other eight interceptor-route obligations are satisfiable by either |
| **F6** | **A30 named `CreatedDate` and then guarded only `DeletedDate`.** The sharpened soft-`Update` obligation gains a clause: set `CreatedDate` to a distinguishable wrong value on the same instance and require the stored one preserved. The general form A30 calls able to *"pass on an instance that happened to carry the stored value"* was all that stood behind it |
| **F7** | **A31's forward clause was too loose for this library's own terms.** A25's *"`SetValues` cannot reach a shadow foreign key"* and A22's `ExecuteUpdate` rejection are **terms of this package**, not consumer code. The disclaimer is narrowed to consumer-authored code, and the document's **behavioral terms are stated as guaranteed against EF Core 10 and re-verified per major** |
| **F8** | **Factual error in A25.** `CompanyResource` declares `public int CompanyId` and `public Guid ResourceId` — confirmed by opening `ProphetsWay.Example.DataAccess/Entities/CompanyResource.cs`, not inferred. A25's point survives intact; both are ordinary mapped scalars, which is the property that matters. The type is corrected |
| **Obligation count** | **142, unchanged, and none is blocked.** F2 rewrote an obligation pair rather than adding one, F6 extended an existing obligation with a clause, and F1 deleted a note rather than a checkbox |

#### Revision 6

The third `Contract Reviewer` pass returned **PASS WITH FINDINGS**, with the instruction to fix in place
rather than open a fourth revision cycle. Nothing below is a design change except the two owner decisions,
and one of those — OD-6 — ratifies the design that was already written.

| Finding | What Revision 6 did |
|---|---|
| **OD-6** | [The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6) rewritten as **resolved**. `Update` writes the root only, as A22 and A25 already specified; the conflicting `Scope=Contract` assertion was a **`ProphetsWay.Example` defect** and the owner authorized its retrait **in that repository** — Revision 7 records that the retrait has since landed. The analysis of why a normalized relational store cannot reproduce the `NoDB` shape is **kept**, because it is the reasoning that justified the retrait. The cascade-dependent obligation is **unblocked** and authorable against root-only behavior |
| **OD-7** | **A26's failure clause reversed.** Detachment happens in a **`finally`**, on success and on failure. Stated explicitly: detaching an `Added` entity after a failed `SaveChanges` **discards the pending insert, and that is the intent**; the caller may fix their argument and retry on the same instance. New obligation — a failed `Insert` followed by a successful `SaveChanges` through a **different DAO on the same context** must not surface the failed row. Every surviving statement that assumed post-success-only detachment swept and retracted |
| **1** | **`ToQueryString()` removed from all nine obligations that prescribed it.** No public member of this library returns an `IQueryable` — `Get` returns `TEntity?`, the trio returns `IList<TEntity>`/`int`, and `Dataset` is `protected` (S10) — so the call could not be made on any of them, and two of the nine offered no alternative assertion at all. The seam is now stated **once**, in [Observing the generated SQL](#observing-the-generated-sql--the-seam), with the second route's limit named honestly. The two prose sites (A27's mechanism, the SQLite limitations table) follow it |
| **2** | **`Attach` named as the second trap in A24**, alongside `Dataset.Add(item)`: `DbContext.Attach` / `DbSet.Attach` mark a **default-keyed** entity `Added` by their own heuristic, which reintroduces the duplicate insert OD-4 forbids against exactly the rows OD-3 exists to protect. This is why the walk is specified **by state**, not by API. Pinned in [Implementer question 7](#implementer-only-questions) as a **constraint, not a choice**, and given a test obligation |
| **4** | The cross-cutting [What a write touches](#what-a-write-touches) bullet **scoped to `Insert`**, which is the only write that attaches anything. `Update` gets its own clause naming its actual mechanism — tracked fetch plus `SetValues`, `item` never tracked — and the A25 limitation, and points at OD-6 rather than at an open question |
| **5** | Revision 4's findings renumbered **`R4-S1`–`R4-S8`** in the log, and the five obligations citing the old bare form updated: `DbUpdateConcurrencyException` → **R4-S7**, the dispatcher `NotSupportedException` and its heading → **R4-S6**, the timestamp both-branches obligation → **R4-S2**, the rollback generated-key obligation → **R4-S4**. Swept for others: implementer question 6 carried the same `(S2)` collision and is fixed |
| **6** | The keyless member-contract table's `GetCore` and `UpdateCore` rows gain the **`IgnoreQueryFilters()`** clause the three keyed tables already carry. Same shape as N1 — a rule stated in full in one place and partially in another |
| **7** | **The soft `Update` mechanism stated** (**A30**). `SetValues` copies every mapped scalar by name, including the three timestamps, so a naive soft `Update` wipes `DeletedDate` — the property `IDepartmentDao`'s own WHY paragraph calls *"the one that gets broken."* One row in the soft-delta table; **which** of the two mechanisms is used is the implementer's |
| **8** | **Minimum EF Core major pinned** (**A31**) in the Framework and language bullet, and a line added to [Global Query Filters](#global-query-filters) on whether EF Core 10's named filters and selective ignoring change the A28 rule |
| **9** | The pipeline code block gains its **third line**. It was labelled "the retrieval trio" and described two members; `GetCount` neither includes nor takes, and discards its ordered query, which is A27's whole point |
| **10** | **A hard family's `Delete` will hard-delete a globally-filtered-out row** — stated for the first time in the conflicting-mechanism subsection, and the hard-entity obligation extended from `Get` to `Delete`. The rule was there; this consequence was not, and a consumer who declares `HasQueryFilter(c => c.IsActive)` will expect the opposite |
| **11** | Stale anchor `#how-the-read-hooks-compose--a20` corrected to `#how-the-read-hooks-compose--a20-a23`. Swept for others introduced by heading renames across Revisions 4 and 5 |
| **A28's justification** | **A12 promoted to the primary argument.** `Get`, `Update` and `Delete` all locate through `MatchRow`, so defending one and not the others produces exactly the *"a `Get` that finds a row its `Update` cannot"* failure A12 exists to prevent — the widening beyond OD-5's wording is **forced, not chosen**. The `IDepartmentDao` rules 4 and 6 citations are accurate and are kept as corroboration |
| **Shadow-FK note** | Two obligations read a **shadow** foreign key. Opening `Entities/User.cs` and `Entities/Transaction.cs` confirms `User` declares no `CompanyId` and `Transaction` declares neither, so both need a route stating. Each obligation now names the one it intends |
| **Obligation count** | **135 → 142, and none is blocked.** Seven added — the hard-family `Delete` under a global filter (10), named filters not changing A28 (A31), the soft `Update` timestamp guard (A30), three for OD-7's `finally`, and the `Attach` default-key guard (2) — and the one 🚫 obligation Revision 5 carried is **unblocked** by OD-6 rather than removed |

#### Revision 5

| Finding | What Revision 5 did |
|---|---|
| **OD-4** | Recorded as an owner decision; implemented as [Writes and the Navigation Graph](#writes-and-the-navigation-graph), **A24** and **A26**, with a `Mechanism` row on [`Insert`](#inserttentity-item) |
| **OD-5** | Recorded; implemented as [Global Query Filters](#global-query-filters) and **A28**, with the placement stated against the existing `Get`-versus-trio filter split |
| **N1** | The "an override of any hook receives the raw `Dataset`" sentence **deleted** from all three places it survived — the A20 decision row, the A20 prose bullet, and its Hooks-group test obligation — and replaced by the **per-hook input contract** (**A23**), stated once as a table covering all three read paths and given three replacement obligations |
| **N2** | `Insert`'s mechanism stated (**A24**), with `Dataset.Add(item)` named as wrong and why; `SetValues`'s inability to repoint a navigation-only relationship **declared** as an accepted limitation (**A25**); "detaches the instances it touched" defined as **the whole reachable graph** (**A26**); new test-obligation group [Writes with a populated navigation graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) |
| **N3** | `GetCount` **invokes `ApplyStableOrder` and discards the returned query** (**A27**). The composition table, the composition bullet and *The two hooks* all restated to match, and an obligation added pinning that an ordering hook with a side effect **does** run during a count |
| **N4** | Closed by OD-5. `HasQueryFilter` / `IgnoreQueryFilters` went from zero mentions to a named section, a cross-cutting bullet, an entry in the rejected table and five test obligations |
| **N5** | "Interaction with `AsNoTracking()`: None" **replaced**. `AsNoTracking()` suppresses identity resolution, the design depends on that suppression, and `AsNoTrackingWithIdentityResolution()` is named as the single change that breaks it. **The review's supporting citation was checked and is wrong** — see the note in [Navigation Loading](#navigation-loading); the conclusion stands, the reasoning is corrected, and the obligation that would actually catch it is new |
| **N6** | `HasColumnType(...)` **deleted** from the two supported consumer routes — `UseCollation` is the API. The collation obligation now names `NOCASE` as the SQLite counterpart and asserts the mirror result. Trailing-blank equality re-attributed to **ANSI padding semantics on `=`**, not to collation |
| **N7** | The `Restore` sample uses **`MatchRow(item)`**, not `SingleOrDefault(KeyEquals(item.Id))` |
| **N8** | `UserDao` gains the `CustomUserFunctionality` member `IUserDao` declares; `TransactionDao` derives from **`BasePagedDao<Transaction, long>`**, agreeing with the migration table and with `ITransactionDao : IBasePagedDao<Transaction>` |
| **N9** | The local-time migration sample is renamed off `Department` — it was teaching a clock `IDepartmentDao` rule 18 forbids — and the sample now overrides **both** timestamp hooks, which it previously did not |
| **N10** | One line, plus an obligation: including a soft entity **bypasses that entity's `ApplyReadFilter`**, and a deleted `Department` therefore arrives populated on `User.Department` |
| **N11** | The orphaned *"They describe one policy from two directions"* sentence **deleted**; "three members, all transaction forwarders" corrected to three **methods** plus two constructors and a `Context` property, verified by opening [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) |
| **N12** | Split-query position recorded (**A29**): the library calls neither `AsSplitQuery()` nor `AsSingleQuery()`; the choice belongs to the `ApplyIncludes` override |
| **"large enough"** | Replaced with a number **and** a mechanism: 10,000 rows, plus a `ToQueryString()` assertion that an explicit `ORDER BY` is emitted — the half that cannot pass by luck at any row count. *(The number survives; the `ToQueryString()` half was unexecutable and was replaced in Revision 6, Finding 1.)* |
| **Editing residue** | Seven literal `\u2014` escapes, left by an earlier pass in the null-argument table and the collation section, replaced with em-dashes. Same family of defect as N11 |
| **New, found by this pass** | [The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6) — a `Scope=Contract` assertion in `SnapshotDeepCopyTests` that no relational implementation of this design can satisfy. Was **open with the owner** when Revision 5 was written; **closed by [OD-6](#owner-decisions-taken-during-revision-6)** in Revision 6, in favor of the design |

#### Revision 4

| Finding | What Revision 4 did |
|---|---|
| **OD-1** | Recorded as an owner decision; implemented as [Navigation Loading](#navigation-loading) and **A18** |
| **OD-2** | Recorded; implemented as the collation term in [String keys and collation](#string-keys-and-collation--od-2) |
| **OD-3** | Recorded; implemented as the `default(TKey)` rows in [Null resolved keys](#null-resolved-keys--the-precise-rule) and [Null key semantics](#null-key-semantics--a4) |
| **B1** | New [Navigation Loading](#navigation-loading) section at the weight of Stable Ordering; `ApplyIncludes` added to every base's signature block; the "a **loaded** navigation property" weasel in the snapshot table replaced; "Include strategies" narrowed to *ad-hoc filtered* includes in the out-of-scope table; obligation placed on `ProphetsWay.Example.DataAccess.EF`; six test obligations added including one pinning the **non**-overriding default |
| **B2** | The seven inherited dispatcher members are now overridden in `BaseEFDataAccess<TContext>` with a `ThrowIfDisposed()` guard delegating to `base` (**A19**). A5 narrowed to *custom* DAL members. Test obligation distinguishing a dispatcher-entry call from a direct forwarder call added |
| **B3** | Provider-fidelity bullet replaced with a per-provider characterization obligation carrying a stated expectation on each leg; implementer question 7 **deleted** |
| **B4** | `default(TKey)` row and paragraph added to the A4 tables, stated as distinct from the null-resolved-key rule |
| **B5** | `ApplyReadFilter` default stated per family; composition fixed as **filter → include → order**, not overridable; `ApplyIncludes` folded into that ordering; implementer question 5 **deleted** (**A20**) |
| **R4-S1** | `RootSoftNonIdDao.UpdateCore` **unsealed** (**A21**); the invariant the seal was believed to protect is named, and shown to be carried by `override` rather than by `sealed` |
| **R4-S2** | The timestamp pair is stated once as the **Timestamp Pair Rule**, cited from both branches; the two defaults are supplied by one `internal static` helper so they cannot drift; the four override sites are enumerated and given a test obligation; the `GetCore`/`UpdateCore` asymmetry is gone with R4-S1 |
| **R4-S3** | Test obligations added for `MatchRow`, `GetKey`, `KeyEquals` and `ApplyReadFilter` overrides, including the A12 all-three-agree claim and a match-nothing override |
| **R4-S4** | Test obligation added for "a rollback does not un-assign a generated key" |
| **R4-S5** | [SQLite leg limitations](#sqlite-leg-limitations) subsection added, symmetrical with the SQL Server-only list |
| **R4-S6** | A15's `NotSupportedException` **accepted** and stated explicitly as a documented escape from the parent's exception vocabulary, with a dispatcher-entry test obligation. `abstract` not reinstated |
| **R4-S7** | `DbUpdateConcurrencyException` specified for `Update` on the same terms as `Delete`, with the fetch-then-`SaveChanges` window named and a test obligation |
| **R4-S8** | Implementer question 3 converted to the stated constraint **A22**; `ExecuteUpdate` rejected with the rows-modified/rows-matched reason; the perf note kept |
| **M1** | "Nothing is abstract" restated as "**no abstract members** on the keyed families" |
| **M2** | `Restore` sample no longer assigns an unread `affected` |
| **M3** | The `RootNonIdDao<T>` name collision between the retired internal engine and the new public type is stated in both places |
| **M4** | Timestamp normalization wording corrected — `CreatedDate` is non-nullable |
| **M5** | Null-argument table split by family; `RootNonIdDao`/`RootSoftNonIdDao` publish neither `Get` nor `Update` |
| **M6** | Constructing `BaseEFDataAccess` with an already-disposed context specified |
| **M7** | Test obligation added pinning the A15 exception **message** |
| **Source defects** | [Two 2.2.0 defects this design retires](#two-220-defects-this-design-retires) added, confirming where each is handled and flagging the CHANGELOG framing |

This is a *design* document, not an implementation. No `.cs`, `.csproj`, `.sln` or `.yml` file was created
or changed to produce it. Every signature below is **proposed**; nothing here has been applied to source.

It exists because the target API of this library is **abstract base classes over `DbContext`**, not
interfaces. An `Interface Architect` would normally hand `Contract Reviewer` and `Test Designer` a set of
interface files whose XML `<remarks>` are the specification. That is not possible here — the contract lives
on classes that cannot be written without also writing their bodies — so the contract is written here
instead, at the same level of detail an XML `<remarks>` block would carry.

**How to read it as a downstream agent:**

| Agent | What this document is for you |
|---|---|
| `Contract Reviewer` | The artifact under review. Every member has null, empty/default, failure, idempotency, ordering, side-effect, concurrency, async and disposal behavior stated. Gaps are defects |
| `Test Designer` | The specification. [Test Obligations](#test-obligations) enumerates what must be pinned; the per-member sections are where the edge cases live |
| `Implementer` | The target. [Implementer-Only Questions](#implementer-only-questions) lists what is deliberately left to you |
| `Modernizer` / `README Author` | Not your file, but the public wording constraint in [Owner Decisions D8](purpose-and-scope.md#owner-decisions--2026-08-15) still binds you |

**Ownership.** `docs/purpose-and-scope.md` and `docs/feature-requests.md` belong to `Purpose Refiner`. This
document **records** how the Stage 2 decisions answer questions those files carry open; it does **not**
change a status in either. Where a question is answered here, the answer is marked *"answered here; status
change is `Purpose Refiner`'s to make."*

**Companion documents:** [purpose-and-scope.md](purpose-and-scope.md) settles what this library is *for*.
[feature-requests.md](feature-requests.md) is the decision index. This document settles what its API *is*.
It cites both and duplicates neither.

---

## Contents

- [Stage 2 Settled Decisions](#stage-2-settled-decisions)
- [The Public Surface](#the-public-surface)
- [Cross-Cutting Rules](#cross-cutting-rules)
- [The DAL Root — `BaseEFDataAccess<TContext>`](#the-dal-root--baseefdataaccesstcontext)
- [The Context Base — `BaseEFContext`](#the-context-base--baseefcontext)
- [The Keyed DAO Families](#the-keyed-dao-families)
- [The Keyless DAO Families](#the-keyless-dao-families)
- [The Key Equality Predicate](#the-key-equality-predicate)
- [Stable Ordering](#stable-ordering)
- [Navigation Loading](#navigation-loading)
- [Global Query Filters](#global-query-filters)
- [Writes and the Navigation Graph](#writes-and-the-navigation-graph)
- [Snapshot and Tracking](#snapshot-and-tracking)
- [Forced Behavior Changes](#forced-behavior-changes)
- [Two 2.2.0 Defects This Design Retires](#two-220-defects-this-design-retires)
- [Migration](#migration)
- [Test Obligations](#test-obligations)
- [Reclassified, Deferred and Rejected](#reclassified-deferred-and-rejected)
- [Implementer-Only Questions](#implementer-only-questions)
- [Questions This Document Closes](#questions-this-document-closes)

---

## Stage 2 Settled Decisions

Owner decisions taken 2026-08-15, numbered **S1–S13** so they cannot be confused with **D1–D9** in
[purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15). Every design choice in this
document traces to one of them, or to a gap-filling decision recorded in
[Design Decisions Made Here](#design-decisions-made-here).

| # | Decision | Consequence |
|---|---|---|
| **S1** | The six keyed generic families **retain their current names** — `BaseDao`, `BaseGetAllDao`, `BasePagedDao`, `BaseSoftDao`, `BaseSoftGetAllDao`, `BaseSoftPagedDao` — now `<TEntity, TKey>` in the **root namespace** | Closes the shape half of [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) |
| **S2** | **Flat method surface.** Every keyed base exposes the whole method set; the consumer's **DAO interface** selects which capabilities are published | Preserves today's behavior — `RootBaseDao` already implements all three capability interfaces |
| **S3** | **Delete the `Guid`/`Int`/`Long` namespaces. No compatibility wrappers** | 18 types removed. `using ProphetsWay.EFTools.Guid;` no longer shadows `System.Guid` |
| **S4** | **Any key type — no `where TKey : struct`.** `string`, nullable value and value keys are all supported, through a **provider-translatable** equality predicate | Answers **Q3**. See [The Key Equality Predicate](#the-key-equality-predicate) |
| **S5** | **Keyless support is kept and extended with ordered paging**, and keyless DAOs are **not** forced to inherit `IBasePagedDao` (which drags in `IBaseDao`). Paging is offered as **conventional public methods** a custom DAO interface can bind to, plus a **protected stable-ordering hook that every keyless DAO publishing a read member must override** (A15) | See [The Keyless DAO Families](#the-keyless-dao-families) |
| **S6** | **Nullable reference annotations are enabled** for 3.x | `<Nullable>enable</Nullable>`; `Get` returns `TEntity?` |
| **S7** | **`BaseEFDataAccess<TContext>` accepts a configured `TContext` directly.** No `TIdType`, no `Activator.CreateInstance`. The derived DAL builds the connection string / provider options | Answers **Q2** — yes, v3.0.0 takes the context |
| **S8** | **Explicit ownership at construction**, expressed as `ContextOwnership.Owned` — created by the derived DAL, disposed with it — or `ContextOwnership.Borrowed` — injected, never disposed (A9). Only the DAL root disposes; **all DAOs share one context**, and that context backs **one live DAL instance only** (A10) | Implements the parent's "disposes what it created" rule |
| **S9** | **The DAO `Ensure*` transaction helpers are removed.** DAL-level `TransactionStart` / `TransactionCommit` / `TransactionRollBack` are the **sole** transaction authority and must match the `ProphetsWay.BaseDataAccess` 3.1.0 contract in full | See [Transactions](#transactions) |
| **S10** | **DAO `Context` and `Dataset` become `protected`** | They were public. Custom queries move inside the DAO, which is where the paradigm always intended them |
| **S11** | **Soft-delete timestamps come from `protected virtual DateTime GetCurrentTimestamp()`, defaulting to `DateTime.UtcNow`.** The `UseUtcTime` flag is removed. The clock is paired with `protected virtual DateTime NormalizeRetrievedTimestamp(DateTime)` so a retrieved timestamp carries a `Kind` again (A13) | See [Timestamp Policy](#timestamp-policy) |
| **S12** | **Forced behavior changes:** `Update` on an absent row returns `0`; `GetAll`/`GetPaged` carry **stable explicit ordering**; the `item` selector is **never read**; snapshot/tracking behavior must satisfy the `ProphetsWay.Example` 3.1 contract; **no async and no `IAsyncDisposable`** in this release | See [Forced Behavior Changes](#forced-behavior-changes) |
| **S13** | **Relational-provider-neutral.** Public EF types appear only where a consumer must supply one — the configured context and its options. **Certified on SQLite and SQL Server** | Extends [D2](purpose-and-scope.md#owner-decisions--2026-08-15) into the signature list |

### Owner Decisions taken during Revision 4

Three questions the contract review raised could not be closed by an agent — each fixes a term a consumer
reads rather than an implementation detail. They were put to the owner and answered. Numbered **OD-1–OD-3**
so they are not confused with **S1–S13** above or **D1–D9** in
[purpose-and-scope.md](purpose-and-scope.md#owner-decisions--2026-08-15).

| # | Decision | Where it lands |
|---|---|---|
| **OD-1** | **Navigation loading is an opt-in hook.** The library adds `protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)`, **defaulting to identity** — the base members load **no** navigation properties unless a Data Access Object says so. In the owner's words: developers know when they want a graph, and it definitely will not be always. **EF Core's model-level `AutoInclude` remains a fully sanctioned alternative**, not a second-class one | [Navigation Loading](#navigation-loading), A18 |
| **OD-2** | **Key equality for string identifiers is the storage engine's collation.** This library neither imposes nor promises one; a consumer needing ordinal matching configures it on the column in their model. Injecting `EF.Functions.Collate(...)` or any forced collation into the key predicate is **rejected** | [String keys and collation](#string-keys-and-collation--od-2) |
| **OD-3** | **`default(TKey)` is an ordinary key value.** No short-circuit. `Get`/`Update`/`Delete` on an entity whose key is `0`, `Guid.Empty` or `""` issue the ordinary query and miss normally when no such row exists | [Null resolved keys](#null-resolved-keys--the-precise-rule), [Null key semantics](#null-key-semantics--a4) |

### Owner Decisions taken during Revision 5

Two more, on the same terms: each fixes a term a consumer reads rather than an implementation detail, and
neither was an agent's to settle. Numbered **OD-4–OD-5**.

| # | Decision | Where it lands |
|---|---|---|
| **OD-4** | **`Insert` writes the root only.** Entities reachable through the argument's navigation properties are **read, never inserted**. Anything already carrying a key is attached `Unchanged`, and EF Core's relationship fix-up then writes the foreign key onto the new row. Nothing about a related entity is written back to the store. The caller's own instance is untouched except for the generated identifier, per the existing snapshot rule. The owner asked, and the answer confirmed, that after `Insert(user)` where `user.Company` names stored company `7` the association is preserved and the foreign key is `7` — **the point of the decision is to get that association without the duplicate insert** | [Writes and the Navigation Graph](#writes-and-the-navigation-graph), A24, A26 |
| **OD-5** | **Global query filters are DEFENDED on `Get`.** The library calls `IgnoreQueryFilters()` on the key-lookup path so a consumer-declared `HasQueryFilter` cannot hide a row from `Get`. In the owner's words: *the intention is for `Get` to work regardless of soft-delete status — if the record is there, `Get` returns it.* Global query filters are documented as a **conflicting mechanism**, not a sanctioned alternative to `ApplyReadFilter` | [Global Query Filters](#global-query-filters), A28 |

### Owner Decisions taken during Revision 6

Two more, numbered **OD-6–OD-7**. Neither came from the `Contract Reviewer` pass this revision answers — that
pass explicitly stated that **none** of its findings required the owner. These were taken alongside it.

**OD-7 is the only decision in this document that reverses an earlier one**, and it is recorded that way
deliberately: Revision 5's A26 said detachment was *"a post-success step, not a `finally`,"* and that sentence
is now wrong. It is retracted where it stood rather than left for a reader to reconcile.

| # | Decision | Where it lands |
|---|---|---|
| **OD-6** | **The `Update` cascade question is resolved in favor of the current design.** `Update` writes the root only, as A22 and A25 specify. The conflicting assertion in `SnapshotDeepCopyTests.Setup_UpdateNavigationInsideTransaction_TestRollBackRestoresIt` was **a `ProphetsWay.Example` defect** — it encodes a `NoDB` denormalization detail (the user row storing a deep copy of the company) that no normalized relational store can reproduce, and it was mis-scoped `Contract` rather than `Characterization`. The owner authorized the retrait **in the `ProphetsWay.Example` repository**, and **it has landed**: the assertion is now a `Characterization` fact and the suite is green. **This document still changes nothing in that repository** | [The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6); the previously-blocked obligation in [Writes with a populated navigation graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) |
| **OD-7** | **Detachment happens in a `finally`** — the whole reachable graph is detached on success **and** on failure. In the owner's reasoning: a failed write must not poison the Data Access Layer instance, and forcing a caller to dispose and rebuild an entire DAL because one insert violated a unique constraint is too harsh a contract. Detaching an `Added` entity after a failed `SaveChanges` **discards the pending insert, and that is the intent**; the caller may fix their argument and retry on the same instance | [Detachment spans the whole reachable graph](#detachment-spans-the-whole-reachable-graph--a26-od-7), A26 |

### Owner Decisions taken during Revision 8

Two more, numbered **OD-8–OD-9**. Both close a **blocking** finding this document could not close itself, and
both are about a statement that could not be written rather than about a behavior anyone wanted changed.
Neither reopens a settled decision, and no design behavior moves.

**OD-8 is the second decision in this document that reverses an earlier one.** It is recorded the way
[OD-7](#owner-decisions-taken-during-revision-6) was: the superseded wording is **retracted where it stood**,
so a reader who met the earlier text can see that its removal was deliberate rather than accidental.

| # | Decision | Where it lands |
|---|---|---|
| **OD-8** | **A26's *"plus any fetched row's whole reachable graph"* clause is retracted.** `Update` and `Delete` locate a row with a tracked fetch that applies **neither `ApplyReadFilter` nor `ApplyIncludes`**, and `item` is **never tracked**, so no relationship fix-up partner exists either — **every navigation on the fetched row is `null`**. The clause described a graph this design cannot produce. Two options were put: widen the fetch so the clause becomes true, or retract it. **The owner chose retraction** — widening would make every write pay for a graph no write uses, and would contradict the opt-in default OD-1 settled. What survives is the fetched **row**, detached in the same `finally` as before. The one route that *does* populate a fetched graph is a consumer-declared model-level `AutoInclude` (H7), and that case is now stated explicitly instead of being reached by accident | [Detachment spans the whole reachable graph](#detachment-spans-the-whole-reachable-graph--a26-od-7), and the replacement obligation in [Writes with a populated navigation graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) |
| **OD-9** | **The `CompanyResource` counter-example in A25 is replaced by a purpose-built entity, `Assignment`.** `CompanyResource` cannot carry the point three times over: its DAO derives from `RootNonIdDao<CompanyResource>`, which publishes neither `Get` nor `Update`; `ICompanyResourceDao` declares no `Update`; and **both of its two mapped scalars sit in its `MatchRow` predicate**, so an `Update` that changed either would locate nothing and return `0`. It also declares **no navigation property at all**, so it could never have demonstrated that a *relationship* is repointable — only that a scalar is writable. The owner approved specifying an entity for the purpose: one navigation, its foreign key declared explicitly, and a third writable non-key column. It is named as purpose-built and **not present in `ProphetsWay.Example`**, on the same footing as `Country` in the collation obligations | [`Update` writes scalars, and cannot repoint a relationship](#update-writes-scalars-and-cannot-repoint-a-relationship--a25), and the rewritten obligation in [Writes with a populated navigation graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) |

### Design Decisions Made Here

Gaps the S-list does not cover, decided in this document rather than referred back to the owner. Each is a
**mechanical consequence** of a settled decision plus a contract this library already advertises — none of
them widens the purpose sentence, and none is a preference exercised over an owner instruction.

| # | Decision | Forced by |
|---|---|---|
| **A1** | The keyless set is **four** classes — `RootNonIdDao<TEntity>`, `BaseNonIdDao<TEntity>`, `RootSoftNonIdDao<TEntity>`, `BaseSoftNonIdDao<TEntity>` — retaining the `NonId` vocabulary | S5, and the naming convention in `AGENTS.md`. The soft root is A14 |
| **A2** | Soft-delete method overriding uses `virtual`/`override`, **never `new`** | Today's `new` hiding silently hard-deletes through a base-typed reference. That cannot satisfy the Example contract |
| **A3** | One **row-matching predicate hook** and one **ordering hook** serve both keyed and keyless families; the keyed families supply a `MatchRow` default composed from `GetKey` + `KeyEquals` (A12) and an ordering default that is valid only under the conditions in A16 | S5 requires the ordering hook; making it the same shape in both halves is the smaller surface |
| **A4** | A null **resolved key** — the value `GetKey(item)` returns — **never matches**, and is answered **without issuing a query** | `= NULL` is never true in SQL; leaving it to the provider makes SQLite and SQL Server disagree |
| **A5** | `BaseEFDataAccess` supplies `protected void ThrowIfDisposed()`. The library applies it to the seven inherited dispatcher members itself (A19); the derived DAL **must** call it from its own **custom** members and forwarders | The parent holds no state and cannot guard. **Narrowed by A19** — Revision 3 made the guard a consumer obligation on all of them, which left `IBaseDataAccess` members unguarded. Answers [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) open question 2 |
| **A6** | `BaseEFContext` is **retained**, provider-free, with only the `DbContextOptions` constructor | D2 kills its `UseSqlServer` body; deleting the type as well would break every consumer context declaration for no gain |
| **A7** | `BaseEFDataAccess<TContext>` becomes **`abstract`**, and its `Dispose()` is a **sealed override** delegating to `protected virtual void DisposeCore()` (A11) | A DAL with no DAOs is meaningless; an overridable `Dispose()` lets a derived DAL break the idempotent/never-throws rule |
| **A8** | Identifier resolution follows the family rule — **`{TypeName}Id` first, then `Id`**, public instance property required — validated at **DAO construction**, reported as `DataAccessConventionException` | Makes `dal.Get<Company>(5)` and `companyDao.Get(item)` address the same column. Diverging would be a defect nobody could see |

#### Revision 2 additions

Seventeen rows rather than eight, for the same reason as the first eight: each is forced by a settled
decision meeting a contract this family already publishes.

| # | Decision | Forced by |
|---|---|---|
| **A9** | Context ownership is the **public enum `ContextOwnership { Borrowed, Owned }`**, not a `bool` | `base(context, true)` at a call site says nothing. `ContextOwnership.Owned` says which half of S8 you chose, and a mistake is visible in review rather than in a double-dispose |
| **A10** | **A `TContext` — borrowed or owned — may back exactly one live `BaseEFDataAccess` instance.** Sharing one context across two DALs is unsupported | Transaction scope is *context*-level in EF Core, so two DALs over one context would silently share a transaction. That contradicts the parent's "scope is the instance, not the connection" rule outright, and no amount of care inside this library can restore it |
| **A11** | The derived cleanup hook is **`protected virtual void DisposeCore()`**, not `Dispose(bool)`. `IsDisposed` is set **before** any teardown step, and each step is **swallowed and the next one still runs** | There is no finalizer in this class and no unmanaged handle, so the `disposing` parameter would always be `true` and would only invite a wrong `false` branch. Setting `IsDisposed` first is what makes disposal safe to re-enter from a failing teardown step |
| **A12** | **`protected TKey? GetKey(TEntity item)`** resolves the identifier value, and `MatchRow(item)` is defined as `KeyEquals(GetKey(item))`. **Every keyed CRUD member calls `MatchRow` and nothing else** | Two locating paths — one for `Get`, one for `Update`/`Delete` — is how the two drift. One path means a `MatchRow` override changes every member at once, which is what an override of it is for |
| **A13** | The soft bases add **`protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value)`**, defaulting to `DateTime.SpecifyKind(value, DateTimeKind.Utc)`, applied to every timestamp on every soft entity **that Data Access Object's own reads** materialize | `IDepartmentDao` rule 18 requires `Kind == Utc` on a **retrieved** instance, and relational providers do not store `Kind`. Without this hook the default derivation cannot satisfy the Example contract — the clock hook alone only covers the write half. **The scope qualifier is load-bearing**: a soft entity materialized as an *include* on some other Data Access Object's query is not reached, because the hook belongs to the DAO that owns the query. See [Including a soft-delete entity bypasses its `ApplyReadFilter`](#including-a-soft-delete-entity-bypasses-its-applyreadfilter--and-that-is-correct) |
| **A14** | **`RootSoftNonIdDao<TEntity>`** is added: soft-delete semantics with **no** capability interface. `BaseSoftNonIdDao<TEntity>` derives from it and adds `IBaseDao<TEntity>` | Without it a keyless soft DAO had to inherit `BaseNonIdDao`, i.e. publish `Get` and `Update` it does not support — the exact coercion S5 exists to prevent, reintroduced one level down |
| **A15** | `ApplyStableOrder` is **`virtual` on every base**. Its keyless default **throws `NotSupportedException`**, and `GetAll`, `GetPaged` and `GetCount` therefore throw until it is overridden | An `abstract` hook taxes the write-only join DAO — the one shape S5 was written for — to buy a compile error for a member it never publishes. A `NotSupportedException` names the missing override precisely and costs the write-only DAO nothing |
| **A16** | The **keyed** ordering default is `OrderBy(key)`, and it is a **total** order only when the resolved key is **unique and non-null**. A model whose key is nullable or duplicate-capable **must** override `ApplyStableOrder` with a deterministic tie-breaker | An identifier that is a real primary key satisfies both conditions, which is the normal case. S4's widened reach — `string`, `int?`, alternate-key identifiers — is exactly where it stops holding, and null ordering differs between providers |
| **A17** | A DAO constructor validates **`context` null first** (`ArgumentNullException`), **then** the identifier (`DataAccessConventionException`). `Dataset` is resolved **lazily on first access** and cached for the DAO's lifetime | Argument validation precedes convention validation everywhere else in the family. `Context.Set<TEntity>()` forces EF model initialization, which must not be a side effect of constructing a Data Access Layer |

#### Revision 4 additions

Five more, each forced by a finding the contract review raised against Revision 3 meeting an owner decision
taken in response.

| # | Decision | Forced by |
|---|---|---|
| **A18** | Every base gains **`protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)`, defaulting to identity**, applied by `Get`/`GetCore`, `GetAll` and `GetPaged` and **not** by `GetCount`. Depth is the Data Access Object's decision | OD-1. Revision 3 mandated `AsNoTracking()` on every read and loaded nothing, so the Example's deep-snapshot suite — which reads `stored.User.Company.Name` two levels down — would have thrown `NullReferenceException` against a conforming implementation on both certified providers |
| **A19** | `BaseEFDataAccess<TContext>` **overrides all seven inherited dispatcher members** — `GetAll<T>`, `GetPaged<T>`, `GetCount<T>`, `Get<T>`, `Insert<T>`, `Update<T>`, `Delete<T>` — with a `ThrowIfDisposed()` guard delegating to `base`. A5's obligation survives only for **custom** DAL members | Those seven are `IBaseDataAccess` members and the parent's DISPOSAL rule binds them. Revision 3 left them inherited unguarded, so a disposed instance answered a dispatcher call with `DataAccessConventionException` instead of `ObjectDisposedException` — and that contradicted A7, which seals `Dispose()` on exactly the reasoning that contract terms a derived DAL can break by accident must not be left to it |
| **A20** | `ApplyReadFilter` **defaults to identity** on the hard families and to `DeletedDate == null` on the soft ones. Composition is **`Dataset` → `ApplyReadFilter` → `ApplyIncludes` → `ApplyStableOrder` → `Skip`/`Take`**, **fixed and not overridable**. What each hook is handed is stated per hook by **A23** | The signatures already forced it and Revision 3 deferred it anyway: `ApplyStableOrder` returns `IOrderedQueryable<TEntity>` while `ApplyReadFilter` takes and returns `IQueryable<TEntity>`, so order-then-filter would discard the ordered-ness. This was never an open question, only an unwritten one |
| **A21** | `RootSoftNonIdDao.UpdateCore` is **`protected override`, not `protected sealed override`** | The seal bought nothing — a consumer overrides the public `BaseSoftNonIdDao.Update` and does as they like — while forbidding the customization "every public member is `virtual`" promises. The invariant that actually matters, *a soft Data Access Object never hard-updates through a base-typed reference*, is carried by `override` (A2), not by `sealed`. Unsealing also removes the unexplained asymmetry with `GetCore`, which was overridden and not sealed on the same class |
| **A22** | **`Update` is implemented with a tracked fetch plus `SetValues`. `ExecuteUpdate` is rejected**, not deferred | `ExecuteUpdate` returns rows-**modified** on SQLite and rows-**matched** on SQL Server, so it cannot deliver the "row existed → `1`, even when the values are identical" rule on both certified legs. The question was foreclosed, not open |

#### Revision 5 additions

Seven more. Three of them (A24–A26) are the write half of the snapshot rule, which no earlier revision stated
at all; the rest are localized statements the review found missing or self-contradictory.

| # | Decision | Forced by |
|---|---|---|
| **A23** | **The per-hook input contract.** `ApplyReadFilter` receives the raw `Dataset` query; `ApplyIncludes` receives the **row-restricted** query for its path; `ApplyStableOrder` receives the filtered, included query. Each returns what the next is handed, and **the wiring is fixed and not overridable**. Stated once as a table in [How the read hooks compose](#how-the-read-hooks-compose--a20-a23) | N1. Revision 4 added "`ApplyIncludes` receives the filtered query" in three places without retracting Revision 3's "an override of any hook receives the raw `Dataset` query" in three others, leaving **two mutually exclusive test obligations**. The old sentence was also incoherent alone: if `ApplyStableOrder` received the raw `Dataset`, "the base wires them together in the order above" would produce an ordered **unfiltered** query |
| **A24** | **`Insert` adds the root and attaches everything reachable from it as `Unchanged`.** A naive `Dataset.Add(item)` is **wrong**, not merely suboptimal, and so is a naive `Attach` — **the walk is specified by *state*, not by API** | OD-4. `DbContext.Add` tracks the given entity **and every reachable untracked entity** as `Added`, so already-stored related rows are re-inserted and the caller's key is overwritten with the duplicate's. `DbContext.Attach` fails differently and just as badly: it applies its own heuristic and marks any entity carrying a **default key value** `Added` rather than `Unchanged`. See [Writes and the Navigation Graph](#writes-and-the-navigation-graph) |
| **A25** | **`SetValues` cannot change a relationship, and that limitation is declared.** `Update` can never repoint a navigation property that has no foreign-key scalar on the entity. A Data Access Object needing it writes a custom method | OD-4 plus A22. `Entry(stored).CurrentValues.SetValues(item)` reads properties **by name off the CLR type**, and a shadow foreign key has no CLR property to read — so `Update(transaction)` can never repoint `Transaction.User`. Accepted, but it must not be silent |
| **A26** | **"Every write detaches the instances it touched" means the whole reachable graph** — the argument and everything reachable from it, plus any row a tracked fetch loaded and everything reachable from that. **In a `finally`, on success and on failure** — [OD-7](#owner-decisions-taken-during-revision-6), which reverses the failure clause Revision 5 wrote | The existing obligation requires that mutating an argument after a write returns cannot reach the store *"including after an unrelated later `SaveChanges` triggered through a different DAO on the same context."* With one shared context (S8) and a graph left tracked, that obligation fails. "The instances it touched" was undefined and could be read as the root alone |
| **A27** | **`GetCount` invokes `ApplyStableOrder` and discards the returned query.** The counted query is `ApplyReadFilter`'s output, and no `ORDER BY` is emitted | N3. *The two hooks* requires `GetCount` to throw `NotSupportedException` when the keyless default is not overridden "because the trio moves together," and the only way to reach that exception is to invoke the hook. The composition table said `GetCount` "orders nothing," which denied it. Both are true once the mechanism is named: the hook **runs**, its result is **thrown away** |
| **A28** | **`IgnoreQueryFilters()` is applied on every `MatchRow`-located path** — `Get`, `GetCore`, and the tracked fetches inside `Update`, `UpdateCore` and `Delete` — and on **no** retrieval-trio path | **A12, primarily.** OD-5 states the rule for `Get`; **A12 forces the rest, and forces it by construction.** `Get`, `Update` and `Delete` locate through `MatchRow` and through nothing else, so defending one path and not the others makes the three disagree about which rows exist — which is exactly the *"a `Get` that finds a row its `Update` cannot"* failure A12 was written to prevent. The widening beyond OD-5's wording is therefore **forced, not chosen**: a version of A28 scoped to `Get` alone would be a defect against A12 on its own terms, before any consumer contract is consulted. **Corroborated** by `IDepartmentDao`: rule 4 requires `Update` on a soft-deleted row to work and rule 6 requires `Delete` to *see* an already-deleted row to return `0`, so a consumer-declared `HasQueryFilter(d => d.DeletedDate == null)` would break two of the three rules. That corroboration is real but weaker — it depends on one consumer's contract, where A12 does not |
| **A29** | **The library calls neither `AsSplitQuery()` nor `AsSingleQuery()`.** Splitting is the Data Access Object's decision, expressed inside its own `ApplyIncludes` override, or the consumer's on the context options | N12. The library cannot know whether a consumer's include set is cheap enough to keep in one query, and EF Core's `MultipleCollectionIncludeWarning` fires the first time a DAO includes two collection navigations — a shape the Example has none of and a consumer will hit |

#### Revision 6 additions

Two, both filling a gap the third review named. Neither changes a behavior this document already specified;
each states a **mechanism** for one, which is what the rest of the document does everywhere else.

| # | Decision | Forced by |
|---|---|---|
| **A30** | **A soft `Update` excludes `CreatedDate`, `UpdatedDate` and `DeletedDate` from the `SetValues` copy** — or restores all three from the tracked entry immediately after it. **Which of the two, is the implementer's**; that the timestamps do not arrive from `item` is not | Finding 7. `Entry(stored).CurrentValues.SetValues(item)` copies **every mapped scalar by name**, the three timestamps included, so a soft `Update` written the naive way overwrites the stored `DeletedDate` with the caller's — usually `null` — and **silently un-deletes the row**. `IDepartmentDao`'s own WHY paragraph names `DeletedDate` as *"the one that gets broken."* The soft-delta table already specified the *behavior*; the document names the mechanism for `Insert` (A24), `Update` (A22), `Delete` and `GetCount` (A27), and this was the one write left without one |
| **A31** | **EF Core 10 is the minimum**, stated in the Framework and language bullet rather than implied by `net10.0` | Finding 8. Several claims here are version-sensitive — `IgnoreQueryFilters()` granularity, `SetValues` against shadow foreign keys, `MultipleCollectionIncludeWarning` — and *"`Microsoft.EntityFrameworkCore` only"* pinned nothing. [D7](purpose-and-scope.md#owner-decisions--2026-08-15)'s `net10.0`-only target makes EF Core 10 the aligned major; saying so is what lets a reader check a version-sensitive claim instead of assuming one |

---

## The Public Surface

**Twelve public classes, down from twenty-four**, plus **one public enum** — thirteen public declarations in
total. All in namespace `ProphetsWay.EFTools`. No sub-namespaces.

| # | Type | Base | Implements | Role |
|---|---|---|---|---|
| 1 | `BaseEFContext` | `DbContext` | — | Optional provider-free context base |
| 2 | `BaseEFDataAccess<TContext>` | `BaseDataAccess` | `IBaseDataAccess` | The DAL root — transactions, disposal, context ownership |
| 3 | `BaseDao<TEntity, TKey>` | — | `IBaseDao<TEntity>` | Keyed CRUD |
| 4 | `BaseGetAllDao<TEntity, TKey>` | `BaseDao` | `+ IBaseGetAllDao<TEntity>` | Publishes `GetAll` |
| 5 | `BasePagedDao<TEntity, TKey>` | `BaseDao` | `+ IBasePagedDao<TEntity>` | Publishes `GetPaged` + `GetCount` |
| 6 | `BaseSoftDao<TEntity, TKey>` | `BaseDao` | `IBaseDao<TEntity>` | Keyed CRUD, soft-delete semantics |
| 7 | `BaseSoftGetAllDao<TEntity, TKey>` | `BaseSoftDao` | `+ IBaseGetAllDao<TEntity>` | Soft + `GetAll` |
| 8 | `BaseSoftPagedDao<TEntity, TKey>` | `BaseSoftDao` | `+ IBasePagedDao<TEntity>` | Soft + `GetPaged` + `GetCount` |
| 9 | `RootNonIdDao<TEntity>` | — | **none** | Keyless plumbing that commits to no capability interface |
| 10 | `BaseNonIdDao<TEntity>` | `RootNonIdDao` | `IBaseDao<TEntity>` | Keyless DAO that does publish the `IBaseDao` shape |
| 11 | `RootSoftNonIdDao<TEntity>` | `RootNonIdDao` | **none** | Keyless **soft delete** that commits to no capability interface (A14) |
| 12 | `BaseSoftNonIdDao<TEntity>` | `RootSoftNonIdDao` | `IBaseDao<TEntity>` | Keyless + soft delete + the `IBaseDao` shape, matched by predicate |
| — | `ContextOwnership` *(enum)* | — | — | `Borrowed` / `Owned` — the constructor argument of type 2 (A9) |

**Types that disappear:** the 18 `Guid`/`Int`/`Long` closures (S3), and the `RootBaseDao<T, TIdType>` /
`RootBaseSoftDao<T, TIdType>` bridges — both `[EditorBrowsable(Never)]` today, both made redundant by the
flat surface. `RootDao<T, TIdType>` is folded into the bases it served.

**One name is reused rather than retired, and the reuse is deliberate.** 2.2.x has an **`internal`**
`RootNonIdDao<T>` engine class; row 9 above introduces a **`public`** `RootNonIdDao<TEntity>` extension
point. **They are different types with the same name**, and the internal one does not survive — its body is
absorbed by the public one. Nothing breaks for a consumer, because nobody could reference an `internal`
type; but a reader comparing the two trees will see one name in both and must not conclude the type was
merely made public. Its visibility, its role, its members and its `MatchRow` contract all changed.
`README Author` and `Changelog Author` should describe it as **new public surface**, not as a promotion.

### The `Base` / `Root` prefix convention

`AGENTS.md` permits both. Within this library they are now **load-bearing and not interchangeable**:

- **`Base*`** — implements at least one `ProphetsWay.BaseDataAccess` DAO interface. Deriving from it means
  your DAO answers that interface.
- **`Root*`** — plumbing that implements **no** capability interface. Deriving from it commits you to
  nothing; your own DAO interface declares whatever subset you support.

`RootNonIdDao<TEntity>` and `RootSoftNonIdDao<TEntity>` exist because of that distinction, and they are the
whole answer to S5.

**The `Root` prefix is being re-purposed, and a 2.2.x reader must be told so.** In 2.2.x, `Root*` meant
*editor-hidden bridge*: `RootBaseDao<T, TIdType>` and `RootBaseSoftDao<T, TIdType>` were `[EditorBrowsable(Never)]`
plumbing nobody was expected to derive from, and `RootNonIdDao<T>` was an internal engine. In 3.x, `Root*`
means *implements no capability interface*, and a `Root` type is a **first-class, documented, browsable
extension point** — `RootNonIdDao<TEntity>` is the recommended base for the Example's join-table DAO. The name
is reused; the meaning is not. `README Author` and `Changelog Author` must not describe a 3.x `Root` type as
internal, and the migration table below spells out that a 2.2.x `RootBaseDao` reference has **no** 3.x
successor — the flat surface absorbed it. **The `RootNonIdDao<T>` name is reused across the visibility
boundary as well** — see [Types that disappear](#the-public-surface) above; the 2.2.x type of that name is
`internal`, the 3.x one is public, and they are not the same type.

### Flat method surface — what S2 does and does not mean

Every keyed base carries **the whole method set** — `Get`, `Insert`, `Update`, `Delete`, `GetAll`, `GetPaged`,
`GetCount` — regardless of which capability interface it declares. `BaseDao<TEntity, TKey>` implements
`IBaseDao<TEntity>` only, and still has a working `GetPaged` on it.

That is deliberate, it is what 2.2.x already does, and it has three consequences worth stating before anyone
reads it as sloppiness:

1. **Publication is the consumer's choice, made in their DAO interface.** `IUserDao : IBaseDao<User>` publishes
   four members; the other three exist on the base and are simply not part of that DAO's advertised contract.
2. **Dispatcher reachability is a separate choice, made on the Data Access Layer** — see
   [How the dispatcher reaches a Data Access Object](#how-the-dispatcher-reaches-a-data-access-object--it-does-not).
   A present-but-unpublished member becomes dispatcher-reachable the moment the DAL forwards it, and not before.
3. **The alternative is worse.** Splitting the method set across the hierarchy means `BasePagedDao` cannot
   inherit `BaseDao` — `IBasePagedDao<T>` inherits `IBaseDao<T>` but not `IBaseGetAllDao<T>` — which forces
   either duplicated bodies or a second bridge layer, the `RootBaseDao` shape this release exists to delete.

The cost is honest and small: a consumer who casts to a base type can call a member their own interface does
not advertise. They could already in 2.2.x, and the member behaves correctly when they do.

---

## Cross-Cutting Rules

Binding on every type above unless a member states otherwise. Stated once here rather than repeated twelve
times.

### Framework and language

- **`net10.0` only** — [D7](purpose-and-scope.md#owner-decisions--2026-08-15). No `netstandard2.0`,
  no `net4x`, no `#if`.
- **EF Core only.** No `System.Data.Entity`, no EF6 branch — [D1](purpose-and-scope.md#owner-decisions--2026-08-15).
- **`<Nullable>enable</Nullable>`** (S6). `ProphetsWay.BaseDataAccess` 3.1.0 is compiled null-oblivious, so
  declaring `TEntity? Get(TEntity item)` against `T Get(T item)` produces no warning.
- **No provider package reference.** `Microsoft.EntityFrameworkCore` only.
- **Minimum EF Core major: 10** (A31). [D7](purpose-and-scope.md#owner-decisions--2026-08-15) fixes the
  target framework at `net10.0`, and EF Core 10 is the major aligned to it — but *"`net10.0` only"* is a
  statement about the TFM, not about the dependency, and several rules here are version-sensitive:
  `IgnoreQueryFilters()` granularity (A28), `SetValues` against a shadow foreign key (A25), and
  `MultipleCollectionIncludeWarning` (A29). **Read every EF-behavior claim in this document as a claim about
  EF Core 10.** A consumer may reference a later 10.x patch or a subsequent major; this library does not test
  against one, and a behavior change reaching **consumer-authored code** is the consumer's to discover.
  **That disclaimer does not extend to this document's own behavioral terms.** A25's *"`SetValues` cannot
  reach a shadow foreign key"* and A22's rejection of `ExecuteUpdate` are **terms of this package**, not
  consumer code — if a subsequent major changed either, `Update`'s documented limitation would change with it
  and a consumer would have no statement of which terms survived. **The behavioral terms stated here are
  guaranteed against EF Core 10 and are re-verified per EF Core major**; a major that has not been re-verified
  carries no such guarantee, and the re-verification is what publishes which terms held.

### Null arguments

| Member group | Null argument behavior |
|---|---|
| `Insert`, `Delete` — **every** family, keyed and keyless | **`ArgumentNullException`**, thrown before any query is issued |
| `Get`, `Update` — the six **keyed** families and the two keyless `Base*` families | **`ArgumentNullException`**, thrown before any query is issued |
| `GetCore`, `UpdateCore` — the two keyless `Root*` families | **`ArgumentNullException`**. These are the `protected` cores; **`RootNonIdDao<TEntity>` and `RootSoftNonIdDao<TEntity>` publish neither `Get` nor `Update`**, so there is no public member to state the rule on. A derived Data Access Object that publishes one of them inherits the check from the core |
| `GetAll`, `GetPaged`, `GetCount` (all families) | **Accepted and ignored.** The parameter is a type selector; `null` is the normal value when the call arrives through the dispatcher |
| `BaseEFDataAccess` constructor `context` | **`ArgumentNullException`** |
| Every DAO constructor `context` | **`ArgumentNullException`**, thrown **before** identifier validation (A17) |

### The `item` type selector

`GetAll(TEntity? item)`, `GetPaged(TEntity? item, int skip, int take)` and `GetCount(TEntity? item)` **must
never read `item`** (S12). It is annotated `TEntity?` precisely so the compiler shows that. It is `null`
whenever the call arrives through `BaseDataAccess.GetAll<T>()`, `GetPaged<T>()` or `GetCount<T>()`. Any
implementation that dereferences it compiles cleanly and throws `NullReferenceException` the first time the
dispatcher is used.

### How the dispatcher reaches a Data Access Object — it does not

**`ProphetsWay.BaseDataAccess.BaseDataAccess` dispatches onto the concrete Data Access Layer, never onto a
DAO.** `Get<T>`, `GetAll<T>`, `GetPaged<T>`, `GetCount<T>`, `Insert<T>`, `Update<T>` and `Delete<T>` each
locate a **public instance method on `instance.GetType()`** — the derived DAL — verify its declared return
type, and invoke it. The dispatcher has no knowledge that DAOs exist.

That fact binds every sample in this document, and it splits the responsibility three ways:

| Layer | What it must declare |
|---|---|
| The consumer's DAO interface — `IUserDao`, `ICompanyResourceDao` | Whichever subset of the conventional signatures that DAO supports: by inheriting a `ProphetsWay.BaseDataAccess` capability interface, **or by declaring the conventional methods itself** |
| The concrete Data Access Layer — `ExampleDataAccess` | **A public forwarding method per entity per capability** — `IList<CompanyResource> GetAll(CompanyResource? item)` — each calling `ThrowIfDisposed()` first and then the DAO. **Without it the dispatcher throws `DataAccessConventionException`** however complete the DAO is |
| The DAO base in this library | The reusable implementation the forwarder calls |

So "the base supplies `GetPaged`, therefore `dal.GetPaged<CompanyResource>(0, 10)` works" is **false on its
own**. It works when, and only when, the derived DAL publishes `GetPaged(CompanyResource?, int, int)` and
forwards to the DAO. A capability that a DAO base implements and the DAL does not forward is reachable
through the DAO reference and **invisible to the dispatcher** — a legitimate choice, made by the DAL author,
not by this library.

### Paging boundaries

Binding on every `GetPaged` in this library, keyed and keyless — this is `IDepartmentDao` rule 12
generalized:

| Input | Result |
|---|---|
| `skip < 0` or `take < 0` | **`ArgumentOutOfRangeException`**, thrown before any query is issued |
| `take == 0` | Empty list |
| `skip` beyond the available count | Empty list |
| `take` larger than the remainder | The remainder, neither padded nor thrown |

### Return values

- `GetAll` and `GetPaged` return **`IList<TEntity>`, never `null`** — an empty list when nothing matches.
- The returned list is a fresh `List<TEntity>` per call; mutating it changes nothing stored.
- `Get` returns `TEntity?` — `null` means no matching row, which is an ordinary outcome, not an error.
- `Insert` returns `void`; `Update` and `Delete` return `int`.

### Concurrency

**No member of this library is thread-safe.** All DAOs on a DAL share one `DbContext` (S8), and `DbContext`
is documented by Microsoft as unsafe for concurrent use. A DAL instance with a transaction open additionally
carries mutable transaction state. **One DAL instance per logical unit of work, one thread at a time.**
Concurrency is achieved with more instances, not shared ones.

### Async

**None in this release** (S12). No `Task`-returning member, no `CancellationToken`, no `IAsyncDisposable`.
Every EF call is the synchronous overload — `SaveChanges`, `ToList`, `SingleOrDefault`, `Count`. This library
**must not lead** the family here: `ProphetsWay.BaseDataAccess` owns that decision and defers it in the
`DELIBERATE OMISSIONS` section of `IBaseDataAccess`. An implementation cannot add async to a contract it
does not own.

### Ordering

Every `GetAll` and `GetPaged` applies an **explicit** ordering, and within one DAO the two order
**identically** — the `ProphetsWay.Example` ordering rule. See [Stable Ordering](#stable-ordering).

### Navigation loading

**The base members load no navigation property.** Every navigation on a returned instance is `null` unless
the Data Access Object overrides `ApplyIncludes`, or the consumer's model declares `AutoInclude`. This is an
owner decision ([OD-1](#owner-decisions-taken-during-revision-4)), not a limitation, and it is stated in
[Navigation Loading](#navigation-loading) with the obligations it places on a Data Access Layer whose own
contract promises a graph.

### Global query filters

**A consumer-declared `HasQueryFilter` cannot hide a row from `Get`.** Every path that locates a single row
through `MatchRow` calls `IgnoreQueryFilters()` first; the retrieval trio does not, so a declared filter
composes there with `ApplyReadFilter`. This is an owner decision
([OD-5](#owner-decisions-taken-during-revision-5)), and a global filter is a **conflicting** mechanism rather
than a sanctioned alternative to `ApplyReadFilter` — see [Global Query Filters](#global-query-filters).

### What a write touches

**Stated per member, because the two writes do not use the same mechanism** and a single sentence covering
both was wrong about `Update`.

- **`Insert` reads its argument's navigation graph and writes only the root.** Related entities are attached
  `Unchanged` — not inserted, not updated (OD-4, A24).
- **`Update` attaches nothing of its own.** Its mechanism is a tracked fetch through `MatchRow` plus
  `SetValues(item)` (A22), and **`item` is never tracked at all** — it is read for its scalar values and
  discarded. It writes the root's mapped scalars and nothing else, and it **cannot repoint a navigation-only
  relationship** (A25). It does not cascade into the graph, and that is settled rather than open —
  [OD-6](#owner-decisions-taken-during-revision-6). **The one qualification is `AutoInclude`:** where a
  consumer's model declares it on the fetched entity, the locating fetch materializes and tracks that graph
  as well, and the `finally` detaches it. *"Attaches nothing"* is true only where no `AutoInclude` is
  declared — see the `AutoInclude` table in
  [Navigation Loading](#model-level-autoinclude-is-an-equally-valid-path).
- **`Delete` locates and removes one row**, and touches no related entity.
- **Every write detaches the whole reachable graph in a `finally`** — on success and on failure alike
  (A26, OD-7).

See [Writes and the Navigation Graph](#writes-and-the-navigation-graph).

---

## The DAL Root — `BaseEFDataAccess<TContext>`

```csharp
namespace ProphetsWay.EFTools
{
	/// <summary>
	/// The Entity Framework Core root of a <see cref="IBaseDataAccess"/> Data Access Layer: it holds the
	/// context every Data Access Object on the layer shares, owns the transaction, and owns disposal.
	/// </summary>
	/// <typeparam name="TContext">
	/// The consumer's context type. Constrained to <see cref="DbContext"/> rather than to
	/// <see cref="BaseEFContext"/>, so a consumer with an existing context — one registered in a
	/// dependency-injection container, for instance — is not required to re-parent it.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// <b>This class selects no database provider and constructs no context.</b> The derived Data Access
	/// Layer builds a configured <typeparamref name="TContext"/> and passes it in, which is what makes the
	/// library relational-provider-neutral.
	/// </para>
	/// <para>
	/// It still derives from <c>ProphetsWay.BaseDataAccess.BaseDataAccess</c>, so the parent's
	/// convention-based dispatch continues to work unchanged — <c>Get&lt;T&gt;</c>, <c>GetAll&lt;T&gt;</c> and
	/// the rest resolve onto <b>the public methods of the derived Data Access Layer</b>, which is where the
	/// forwarding members live. The dispatcher never resolves a method on a Data Access Object. The
	/// reflection removed in 3.0.0 is this class's own <c>Activator.CreateInstance</c> of the context, not
	/// the parent's dispatcher.
	/// </para>
	/// <para>
	/// <b>Not thread-safe</b>, and <b>not shareable</b>: one instance, one unit of work, one thread, and one
	/// <typeparamref name="TContext"/> that backs no other live instance of this class.
	/// </para>
	/// </remarks>
	public abstract class BaseEFDataAccess<TContext> : ProphetsWay.BaseDataAccess.BaseDataAccess, IBaseDataAccess
		where TContext : DbContext
	{
		protected BaseEFDataAccess(TContext context, ContextOwnership ownership);

		protected TContext Context { get; }

		protected ContextOwnership Ownership { get; }

		protected bool IsDisposed { get; }

		protected void ThrowIfDisposed();

		public override void TransactionStart();

		public override void TransactionCommit();

		public override void TransactionRollBack();

		// --- the seven inherited dispatcher members, guarded here and delegated to base (A19) ---
		// Each body is exactly: ThrowIfDisposed(); return base.X<T>(...);
		// Without these overrides they are inherited unguarded, and a call on a disposed instance
		// reaches the parent's reflection and reports DataAccessConventionException instead of
		// ObjectDisposedException.
		public override IList<T> GetAll<T>();
		public override IList<T> GetPaged<T>(int skip, int take);
		public override int GetCount<T>();
		public override T Get<T>(object id);
		public override void Insert<T>(T item);
		public override int Update<T>(T item);
		public override int Delete<T>(T item);

		public sealed override void Dispose();

		/// <summary>
		/// Releases whatever the derived Data Access Layer created for itself. Called once, from
		/// <see cref="Dispose"/>, after <c>IsDisposed</c> is already <c>true</c>. Must not throw; anything it
		/// does throw is swallowed and disposal continues.
		/// </summary>
		protected virtual void DisposeCore();
	}

	/// <summary>Who is responsible for disposing the context a Data Access Layer was handed.</summary>
	public enum ContextOwnership
	{
		/// <summary>Someone else created the context and will dispose it. This layer never does.</summary>
		Borrowed = 0,

		/// <summary>This layer created the context and disposes it along with itself.</summary>
		Owned = 1
	}
}
```

### `BaseEFDataAccess(TContext context, ContextOwnership ownership)`

| Aspect | Contract |
|---|---|
| **Nulls** | `context` null → `ArgumentNullException` |
| **`ContextOwnership.Owned`** | The context was created by the derived Data Access Layer. `Dispose` disposes it |
| **`ContextOwnership.Borrowed`** | The context was injected. `Dispose` **never** disposes it — whoever supplied it still owns it and will dispose it on its own schedule |
| **Undefined enum values** | An `ownership` value that is neither member — `(ContextOwnership)7`, reachable by a cast — → `ArgumentOutOfRangeException`. There is no "treat anything unknown as borrowed" fallback; a silently misread ownership is the bug this parameter exists to prevent |
| **An already-disposed `context`** | **Accepted; the constructor does not detect it and must not try.** EF Core exposes no supported predicate for "this context is disposed," and probing for one would mean touching the context in a constructor that A17 and this table both promise touches nothing. The instance constructs normally and `IsDisposed` is `false`; the first member that reaches the store fails with EF Core's own `ObjectDisposedException`, **naming the context type, not this Data Access Layer** — so the message points at the real mistake. `Dispose` on such an instance still returns cleanly, because every teardown step is swallowed (A11). Handing a disposed context to `BaseEFDataAccess` is a composition-root error with an unspecified moment of failure, which is why it is stated rather than tolerated silently |
| **Default** | **There is none, deliberately.** Both parameters are required. A defaulted ownership is the shape that produces double-dispose bugs in a dependency-injection host and leaks in a manual one, silently, in whichever direction the default happened to point |
| **Side effects** | None beyond capturing the two values. No query, no connection, no transaction, no EF model initialization |
| **Ordering** | Nothing else on the instance may be called before it |

**Why an enum and not a `bool`** (A9): `base(context, true)` communicates nothing at a call site, and the two
failure modes it guards — a double dispose and a leaked context — are both silent and both discovered late.
`ContextOwnership.Owned` names the decision where it is made, and a reviewer reading a constructor can see
which half of S8 the author chose without navigating to the base class.

**Both hosting styles are first-class** and neither requires anything of the other:

```csharp
// Manual construction — this Data Access Layer created the context, so it owns it.
public ExampleDataAccess(string connectionString)
	: this(new ExampleContext(new DbContextOptionsBuilder<ExampleContext>()
		.UseSqlServer(connectionString)
		.Options), ContextOwnership.Owned)
{ }

// Dependency injection — the container created the context and will dispose it.
public ExampleDataAccess(ExampleContext context)
	: this(context, ContextOwnership.Borrowed)
{ }
```

### One context, one live Data Access Layer

**A `TContext` — owned or borrowed — may back exactly one live `BaseEFDataAccess` instance at a time.**
Sharing one context between two DAL instances is **unsupported**, and this library neither detects it nor
works around it (A10).

The reason is structural rather than stylistic. In EF Core a transaction is begun on
`Context.Database` and lives on the **context**, not on the object that started it. Two DALs over one context
would therefore share one transaction: `dalA.TransactionStart()` would silently enroll `dalB`'s writes,
`dalB.TransactionStart()` would throw or clobber depending on order, and `dalA.Dispose()` would roll back
work `dalB` believed committed. The parent contract's **"scope is the instance, not the connection"** rule
cannot be honored on a shared context by any amount of care inside this library — the state being shared is
not ours.

Practical consequences for a consumer:

- **Dependency injection:** register the context and the DAL with the **same lifetime** — typically scoped.
  A singleton DAL over a scoped context, or two scoped DALs resolving one singleton context, both violate
  this rule.
- **Borrowed does not mean shareable.** `ContextOwnership.Borrowed` says only that someone else disposes
  it; it does not license a second concurrent DAL over the same instance.
- **Sequential reuse is permitted.** Handing a borrowed context to a second DAL *after* the first has been
  disposed is fine, provided the first left no transaction open — and it cannot have, because disposal rolls
  one back.

### `Context`, `Ownership`, `IsDisposed`, `ThrowIfDisposed()`

- **`Context`** is `protected`, typed `TContext` rather than `DbContext` so a derived DAL constructs its
  DAOs without a cast. It is the **one** context every DAO on the layer receives (S8).
- **`Ownership`** is `protected` and exposed for a derived `DisposeCore()` override that needs to know.
- **`IsDisposed`** is `protected`; `false` until `Dispose()` runs, `true` from the **first statement** of
  `Dispose()` onward (A11).
- **`ThrowIfDisposed()`** throws `ObjectDisposedException` when `IsDisposed`, otherwise returns. **The
  library applies it to the seven inherited dispatcher members itself** (A19), and **a derived Data Access
  Layer must call it as the first statement of every member it declares** — `Get(Company)`,
  `Insert(Company)`, the custom methods, all of them.

#### The seven inherited members are guarded here, not by the consumer — A19

`ProphetsWay.BaseDataAccess.BaseDataAccess` declares `GetAll<T>()`, `GetPaged<T>()`, `GetCount<T>()`,
`Get<T>(object)`, `Insert<T>`, `Update<T>` and `Delete<T>` as **`public virtual`**. They are
`IBaseDataAccess` members, so the parent's DISPOSAL rule — *every member other than `Dispose` throws
`ObjectDisposedException` after disposal* — binds them directly.

**`BaseEFDataAccess<TContext>` therefore overrides all seven**, and each body is exactly
`ThrowIfDisposed();` followed by a delegation to `base`. Nothing else changes; the parent's reflection still
does the resolution.

**Revision 3 got this wrong and the error was not cosmetic.** It overrode only the three transaction members
and `Dispose`, leaving the seven inherited unguarded, and resolved the gap by making the guard an obligation
on the consumer's *forwarders*. But a forwarder is what the dispatcher resolves **onto**, not what it is
called **through**: on a disposed instance with a forwarder that is missing or misnamed, the parent's
reflection fails first and the caller receives **`DataAccessConventionException`** — a wiring error — for what
is actually a use-after-dispose. It also contradicted [A7](#design-decisions-made-here), which seals
`Dispose()` precisely because idempotency and never-throwing "are contract terms a derived Data Access Layer
must not be able to break by accident." The same sentence applies to the disposal guard on the same
interface's other members.

**They are overridden, not sealed**, and that is a decision rather than an omission. The parent declares them
`virtual` so a Data Access Layer can bypass reflection on a hot path or wrap dispatch in logging; sealing
them here would withdraw a capability the parent grants. **An override of any of the seven must call
`ThrowIfDisposed()` first** — and unlike the inherited-unguarded case, an override that forgets is visible in
the consumer's own file rather than invisible in an inherited member nobody wrote.

**Why the obligation on custom members cannot be removed** (A5, as narrowed): a custom member —
`GetCreatedSince(DateTime)`, `Restore(Department)`, a per-entity forwarder — is declared on the *derived*
Data Access Layer, and this library dispatches nothing and intercepts nothing on that path.
`ProphetsWay.BaseDataAccess` holds no state and cannot guard either. With `ContextOwnership.Owned` a
disposed context throws `ObjectDisposedException` of its own accord, which masks a missing guard — and then
the same code silently stops throwing the moment someone switches to a borrowed context.
**`ThrowIfDisposed()` is what makes the behavior identical in both modes.** `Contract Reviewer` should treat
a derived DAL member without it as a defect; `Test Designer` must cover the `ContextOwnership.Borrowed` path
specifically, because that is the one where a missing guard is visible.

### Transactions

Sole authority (S9). The `Ensure*` helpers on the DAOs are gone; there is exactly one place a transaction
can be started, and it is the DAL. Behavior below **is** the `IBaseDataAccess` 3.1.0 contract — that
`<remarks>` block is the source of truth, and this table is its EF Core mapping.

| Member | Behavior |
|---|---|
| `TransactionStart()` | `ThrowIfDisposed()`. **Throws `InvalidOperationException` when a transaction is already open on this instance** — transactions do not nest. Otherwise begins one on `Context.Database` and retains it |
| `TransactionCommit()` | `ThrowIfDisposed()`. **Throws `InvalidOperationException` when no transaction is open** — including a second call after a commit or a rollback. Otherwise commits and clears the instance's transaction state |
| `TransactionRollBack()` | `ThrowIfDisposed()`. **Throws `InvalidOperationException` when no transaction is open** — including after a commit, after a rollback, and after a *failed* commit. Otherwise rolls back and clears state |

Additional binding rules:

- **A failed commit leaves no transaction open, and its writes are discarded.** If the provider throws from
  `Commit()`, the implementation clears its transaction state and disposes the EF transaction — which rolls
  the batch back at the store — and then **rethrows the provider's exception unwrapped**. A caller must not
  follow a failed commit with `TransactionRollBack()`; that call throws `InvalidOperationException`, because
  there is nothing left to roll back.
- **Scope is the instance, not the connection.** Two `BaseEFDataAccess` instances over the same database do
  not share a transaction. **This holds because a context backs one instance** — see
  [One context, one live Data Access Layer](#one-context-one-live-data-access-layer). Two instances over the
  *same context* would share one, which is exactly why that arrangement is unsupported.
- **Outside a transaction every write auto-commits.** Each DAO write calls `SaveChanges()`, which commits on
  its own. Inside a transaction, those same `SaveChanges()` calls enroll in it and persist only on commit.
- **Ambient `TransactionScope` is untouched.** This library never creates one and never suppresses one.
  Whatever the provider does in the presence of an ambient scope — SQL Server enlisting, SQLite not — happens
  exactly as it would without this library. That includes any exception the provider raises from
  `BeginTransaction()` inside an ambient scope: it propagates unwrapped, and this library adds no behavior
  to it. **Assert this provider-neutrally:** capture `System.Transactions.Transaction.Current` before and
  after a DAL call and require the same reference (or `null` both times) — that is a statement about *this
  library* and holds on both certified legs. Do **not** assert that writes enlist, or that they do not; that
  is the provider's behavior and it differs between SQL Server and SQLite by design.
- **A rollback does not un-assign a generated key.** `Insert` writes the store-generated identifier onto its
  argument as soon as `SaveChanges()` returns, and `TransactionRollBack()` — or a rollback performed by
  `Dispose` — removes the row but **leaves that value on the caller's instance**, where it now names nothing.
  Re-inserting the same instance after a rollback is a caller decision with caller consequences: on a
  store-generated column the new insert overwrites it with a fresh value, and on a client-generated key
  (`Guid`, `string`) the original value is reused. This library does not track, restore, or clear it. Most
  relational sequences also do not recycle a rolled-back value, so the second insert usually lands on a
  different identifier — that is the provider's business, not a promise made here.
- **Not reentrant, not thread-safe.** An instance with a transaction open must not be touched from a second
  thread.

### Disposal

```csharp
public sealed override void Dispose();   // idempotent, never throws
protected virtual void DisposeCore();    // derived cleanup hook; must not throw
```

**The sequence is fixed, and every step is best-effort** (A11):

1. **If `IsDisposed` is already `true`, return immediately.**
2. **Set `IsDisposed = true` — first, before any teardown.**
3. Roll back and dispose the open transaction, if there is one. **Failure is swallowed; step 4 still runs.**
4. Call `DisposeCore()`. **Failure is swallowed; step 5 still runs.**
5. Dispose `Context`, **only when `Ownership` is `Owned`**. Failure is swallowed.
6. Return. `Dispose` throws nothing, ever.

| Rule | Behavior |
|---|---|
| **Idempotent** | The second and every subsequent call returns at step 1 without touching anything |
| **Never throws** | A failed rollback, a throwing `DisposeCore()`, and a throwing `Context.Dispose()` are each caught and discarded |
| **Never abandons a step** | A failure in one step does **not** skip the next. A `DisposeCore()` that throws must not leave an owned context undisposed |
| **Rolls back, never commits** | A transaction still open when the instance is disposed is rolled back and disposed |
| **Ownership** | Disposes `Context` **only when `Ownership` is `ContextOwnership.Owned`** |
| **After disposal** | Every other member throws `ObjectDisposedException` — see [`ThrowIfDisposed`](#context-ownership-isdisposed-throwifdisposed) |

**`IsDisposed` is set first, and that ordering is load-bearing.** It has two consequences that are the whole
point:

- **The rollback in step 3 must not go through `TransactionRollBack()`.** That member calls
  `ThrowIfDisposed()`, and `IsDisposed` is already `true`, so it would throw `ObjectDisposedException` from
  inside `Dispose`. It would also throw `InvalidOperationException` whenever no transaction is open, which is
  the ordinary case. **Disposal uses a private rollback path that talks to the retained EF transaction
  directly**, checks for its presence rather than throwing, and never consults the public members. A design
  that calls the public method and catches the resulting exception is *not* equivalent: it makes the normal
  disposal path exception-driven, and it hides a genuine rollback failure among two expected ones.
- **A `DisposeCore()` that calls back onto the instance gets `ObjectDisposedException`, not a half-torn-down
  object.** Derived cleanup that tries to write "one last audit row" fails loudly and is then swallowed,
  rather than issuing a query against a context that is about to be disposed.

**`Dispose()` is a sealed override** (A7). Idempotency and never-throwing are contract terms a derived Data
Access Layer must not be able to break by accident; a derived class with something of its own to release
overrides `DisposeCore()` and inherits both guarantees.

**Why `DisposeCore()` and not `Dispose(bool disposing)`** (A11): the `bool` pattern exists to tell a
finalizer-bearing type which cleanup is legal during finalization. This class has **no finalizer** and holds
**no unmanaged handle** — a `DbContext` is a managed object with its own disposal — so `disposing` would
always be `true`, and its only live effect would be to invite a derived class to write a `false` branch that
never executes. `DisposeCore()` cannot be called wrongly because it takes nothing.

Note the deliberate asymmetry with the transaction members one screen up: **`Dispose` twice is fine,
`TransactionCommit` twice throws.** Disposal is cleanup that has to be safe from a `finally` block; a
repeated transaction call means the caller has lost track of its own control flow.

`ObjectDisposedException` derives from `InvalidOperationException`, so a consumer's
`catch (InvalidOperationException)` around transaction handling catches use-after-dispose too. That is
inherited from the parent contract, not invented here.

### Data Access Object lifetime

DAOs are **not** disposable and do not own anything (N8). A DAO holds the reference to the DAL's `Context`
and nothing else; it opens no connection, retains no transaction, and caches nothing across calls beyond its
resolved `Dataset` and identifier metadata.

| Rule | Behavior |
|---|---|
| **No `IDisposable` on any DAO** | There is nothing for it to release. A DAO does not get a `Dispose` in 3.x, and adding one to a derived DAO does not make the DAL call it |
| **A DAO must not outlive its Data Access Layer** | Its context is disposed (owned) or handed back (borrowed) when the DAL is disposed. A DAO used afterwards fails: `ObjectDisposedException` from EF Core with an owned context, and whatever the borrowed context's owner has since done to it otherwise — **an unspecified failure mode, which is why the boundary is stated rather than tolerated** |
| **A DAO must not be handed to a second Data Access Layer** | It carries the first one's context, so using it through a second DAL puts two DALs on one context — the arrangement A10 forbids |
| **A DAO must not escape the layer** | Returning one from a public DAL member re-exposes what S10 made `protected`. Publish the *operation* on the DAL's interface, not the DAO |

The derived DAL constructs its DAOs in its own constructor, holds them in private fields, and lets them be
collected with itself. That is the entire lifetime story, and it is the reason DAOs need no disposal
contract of their own.

---

## The Context Base — `BaseEFContext`

```csharp
namespace ProphetsWay.EFTools
{
	/// <summary>
	/// An optional base for a consumer's <see cref="DbContext"/>. It selects no provider and adds no
	/// behavior — it exists so a consumer's context has a family-shaped base and a single constructor
	/// convention.
	/// </summary>
	/// <remarks>
	/// Deriving from this type is <b>not</b> required. <see cref="BaseEFDataAccess{TContext}"/> constrains
	/// its context to <see cref="DbContext"/>, so any context works.
	/// </remarks>
	public abstract class BaseEFContext : DbContext
	{
		protected BaseEFContext(DbContextOptions options) : base(options) { }
	}
}
```

**The `string connectionString` constructor is removed.** Its body was
`new DbContextOptionsBuilder().UseSqlServer(connectionString)`, which is exactly the coupling
[D2](purpose-and-scope.md#owner-decisions--2026-08-15) exists to end. A consumer who wants a
connection-string constructor writes one on their own context, one line, naming their own provider.

**Why the type survives at all** (A6): its remaining constructor is a passthrough, and a passthrough type is
usually surface not worth keeping. It is kept because every existing consumer context already declares
`: BaseEFContext`, the README teaches that shape, and retaining it means the context half of the migration
is *delete one constructor* rather than *re-parent every context*. It is a candidate for removal in 4.x, and
that is recorded rather than acted on.

**It is a convenience, not a conformance requirement, and it is not a behavior seam.** Three statements bind
downstream agents:

1. **`BaseEFDataAccess<TContext>` constrains `TContext` to `DbContext`, never to `BaseEFContext`.** A
   consumer whose context derives from `DbContext` directly — or from some other base of their own — is
   fully conforming and gives up nothing. No sample in the README or the CHANGELOG may imply otherwise.
2. **Nothing in this library reads or requires this type.** No DAO, no DAL member, and no test obligation
   mentions it. Deleting it from a consumer's solution changes no behavior described anywhere in this
   document.
3. **It performs no timestamp conversion, and must not grow one.** A global
   `DateTimeKind`-restoring value converter registered here would be invisible to a consumer who does not
   derive from it, so it would make conformance depend on which base a context happened to pick. The soft
   DAOs solve that problem where every consumer gets it — see
   [Timestamp Policy](#timestamp-policy) — and the two mechanisms must not both exist, because a value
   converter and a normalization hook applied to the same value is precisely how a double conversion ships.

---

## The Keyed DAO Families

Six classes (S1), root namespace (S3), open `TKey` (S4).

```csharp
namespace ProphetsWay.EFTools
{
	public abstract class BaseDao<TEntity, TKey> : IBaseDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		protected BaseDao(DbContext context);

		protected DbContext Context { get; }
		protected DbSet<TEntity> Dataset { get; }

		// --- IBaseDao<TEntity> ---
		public virtual TEntity? Get(TEntity item);
		public virtual void Insert(TEntity item);
		public virtual int Update(TEntity item);
		public virtual int Delete(TEntity item);

		// --- conventional, published only by the derived family that declares the interface ---
		public virtual IList<TEntity> GetAll(TEntity? item);
		public virtual IList<TEntity> GetPaged(TEntity? item, int skip, int take);
		public virtual int GetCount(TEntity? item);

		// --- hooks ---
		/// <summary>Reads the identifier value off <paramref name="item"/>.</summary>
		protected virtual TKey? GetKey(TEntity item);

		/// <summary>The predicate locating the row <paramref name="item"/> refers to. Defaults to KeyEquals(GetKey(item)).</summary>
		protected virtual Expression<Func<TEntity, bool>> MatchRow(TEntity item);

		/// <summary>The predicate matching the row whose identifier equals <paramref name="key"/>.</summary>
		protected virtual Expression<Func<TEntity, bool>> KeyEquals(TKey? key);

		/// <summary>The identifier selector, over the same resolved property KeyEquals compares.</summary>
		protected virtual Expression<Func<TEntity, TKey?>> KeySelector { get; }

		/// <summary>
		/// Restricts which stored rows the retrieval members may see. <b>Defaults to identity</b> on this class
		/// — no row is hidden. Receives the raw Dataset query and runs first (A20).
		/// </summary>
		protected virtual IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query);

		/// <summary>
		/// Declares which navigation properties a read materializes. <b>Defaults to identity</b> — the base
		/// members load none (A18). Override with Include/ThenInclude to the depth this Data Access Object's
		/// own contract promises. Receives the row-restricted query for its path — ApplyReadFilter's output on
		/// GetAll/GetPaged, the MatchRow-matched query on Get/GetCore (A23).
		/// </summary>
		protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query);

		/// <summary>A total ordering over the set, applied by GetAll and GetPaged alike. Defaults to OrderBy(KeySelector).</summary>
		protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query);
	}

	public abstract class BaseGetAllDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBaseGetAllDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		protected BaseGetAllDao(DbContext context) : base(context) { }
	}

	public abstract class BasePagedDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBasePagedDao<TEntity>
		where TEntity : class, IBaseIdEntity<TKey>
	{
		protected BasePagedDao(DbContext context) : base(context) { }
	}

	public abstract class BaseSoftDao<TEntity, TKey> : BaseDao<TEntity, TKey>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
		protected BaseSoftDao(DbContext context) : base(context) { }

		/// <summary>The clock every stamping member reads. Defaults to DateTime.UtcNow.</summary>
		protected virtual DateTime GetCurrentTimestamp();

		/// <summary>
		/// Restores the DateTimeKind a relational store did not preserve. Defaults to
		/// DateTime.SpecifyKind(value, DateTimeKind.Utc), and is applied to every timestamp on every entity a
		/// read materializes. Bound to GetCurrentTimestamp by the Timestamp Pair Rule (A13) — override both or
		/// neither.
		/// </summary>
		protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value);

		public override TEntity? Get(TEntity item);
		public override void Insert(TEntity item);
		public override int Update(TEntity item);
		public override int Delete(TEntity item);
		public override IList<TEntity> GetAll(TEntity? item);
		public override IList<TEntity> GetPaged(TEntity? item, int skip, int take);
		protected override IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query); // DeletedDate == null
	}

	public abstract class BaseSoftGetAllDao<TEntity, TKey> : BaseSoftDao<TEntity, TKey>, IBaseGetAllDao<TEntity>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
		protected BaseSoftGetAllDao(DbContext context) : base(context) { }
	}

	public abstract class BaseSoftPagedDao<TEntity, TKey> : BaseSoftDao<TEntity, TKey>, IBasePagedDao<TEntity>
		where TEntity : class, IBaseSoftIdEntity<TKey>
	{
		protected BaseSoftPagedDao(DbContext context) : base(context) { }
	}
}
```

**The five subclasses add no members.** Each adds one interface declaration and a constructor — exactly the
shape the `Int`/`Guid`/`Long` classes have today, which is what S2's flat surface preserves. `BasePagedDao`
derives from `BaseDao` rather than `BaseGetAllDao` because `IBasePagedDao<T>` inherits `IBaseDao<T>` and not
`IBaseGetAllDao<T>`; that mirrors today's inheritance exactly.

**The six keyed families declare no abstract members.** The classes themselves are `abstract` — all six of
them, as the declarations above show — because a base nobody instantiates directly should say so. What is
absent is an abstract *member*: nothing here forces a deriving Data Access Object to implement anything. A
consumer's DAO is
`class UserDao : BasePagedDao<User, int> { public UserDao(DbContext c) : base(c) { } }` and it works. The
`Get` override that every one of the 18 old classes had to write is gone —
[The Key Equality Predicate](#the-key-equality-predicate) is what replaces it.

**This is a property of the keyed half only.** `RootNonIdDao<TEntity>.MatchRow` **is** abstract, deliberately
and for a stated reason — see [The two hooks](#the-two-hooks--one-required-at-compile-time-one-at-first-use).
Earlier revisions wrote "nothing is abstract" unqualified, directly beneath six `abstract class`
declarations, which was wrong twice over.

**Every public member is `virtual`** so a DAO with a genuine special case overrides one method rather than
abandoning the base. **Every hook is `virtual` too, and none is `sealed`** — the one `sealed` override
Revision 3 carried was removed in Revision 4 (A21).

#### How the read hooks compose — A20, A23

**The composition is fixed. It is not a hook, not overridable, and not an implementer's choice.**

```text
// GetAll and GetPaged
Dataset → ApplyReadFilter → ApplyIncludes → ApplyStableOrder → Skip/Take → AsNoTracking → ToList

// GetCount — includes nothing, takes nothing, and throws the ordered query away (A27)
Dataset → ApplyReadFilter → [ApplyStableOrder, discarded] → Count

// the key-lookup path — Get and GetCore
Dataset → IgnoreQueryFilters → Where(MatchRow(item)) → ApplyIncludes → AsNoTracking → SingleOrDefault
```

**The second line is the one a reader skips.** An earlier revision labelled the first line "the retrieval
trio" and then described two of the three members; `GetCount` differs from its partners in every segment
except the filter, and A27's whole point is that its ordered query is built and discarded.

| Hook | Default — hard families | Default — soft families | Applied by |
|---|---|---|---|
| `ApplyReadFilter` | **identity** — hides nothing | **`DeletedDate == null`** | `GetAll`, `GetPaged`, `GetCount` |
| `ApplyIncludes` | **identity** — loads no navigation property | identity | `Get` / `GetCore`, `GetAll`, `GetPaged` |
| `ApplyStableOrder` | `OrderBy(KeySelector)` — keyed; **throws `NotSupportedException`** — keyless | as the hard families | `GetAll`, `GetPaged` — and **invoked and discarded** by `GetCount` (A27) |

##### What each hook is handed — A23

**Stated per hook, because it is not the same for all three.** Revision 3 said "an override of any hook
receives the raw `Dataset` query"; Revision 4 wrote the opposite for `ApplyIncludes` without retracting it.
The sentence is gone, and this table replaces it.

| Hook | On `GetAll` / `GetPaged` it receives | On `GetCount` it receives | On `Get` / `GetCore` it receives |
|---|---|---|---|
| `ApplyReadFilter` | the **raw `Dataset`** query | the **raw `Dataset`** query | not invoked |
| `ApplyIncludes` | the **filtered** query — `ApplyReadFilter`'s return value | not invoked | the **key-matched** query — `Dataset.IgnoreQueryFilters().Where(MatchRow(item))` |
| `ApplyStableOrder` | the **filtered, included** query | the **filtered** query, whose return value is **discarded** (A27) | not invoked |

**The one-sentence form: each hook receives the row-restricted query for its path, and returns what the next
one is handed.** `ApplyIncludes` therefore never sees a set larger than the one its path will return, on
either path — the filtered set on the trio, the single matched row on the key lookup. The wiring is fixed;
no override changes it.

- **`GetCount` applies the filter, invokes `ApplyStableOrder` and throws the result away, and includes
  nothing.** It materializes no entity, so there is nothing to include, and the counted query is
  `ApplyReadFilter`'s output, so **no `ORDER BY` is emitted**. The hook is invoked for its *contract* effect
  rather than its query effect: it is what makes the keyless default's `NotSupportedException` reach
  `GetCount`, which is the trio-moves-together rule in [The two hooks](#the-two-hooks--one-required-at-compile-time-one-at-first-use).
  **A consequence `Test Designer` must be able to predict: an ordering override with a side effect runs
  during a count.** Counting the *ordered* query instead was rejected — it would make the emitted SQL depend
  on the provider's translator stripping an `ORDER BY` it cannot legally keep, which is exactly the kind of
  per-provider divergence [D1](purpose-and-scope.md#owner-decisions--2026-08-15) exists to end.
- **`GetCount` must still agree with `GetAll().Count`**, which is why the filter is common to both.
- **`Get` and `GetCore` apply `ApplyIncludes` and neither of the other two.** Not the filter, because
  `IDepartmentDao` rule 8 requires `Get` to return a soft-deleted row; not the order, because
  `SingleOrDefault` over a key predicate has nothing to order. **They additionally call
  `IgnoreQueryFilters()`** so a mechanism this library does not own cannot reinstate the exclusion rule 8
  keeps off them — see [Global Query Filters](#global-query-filters).
- **Filter before order is forced by the signatures**, not chosen: `ApplyStableOrder` returns
  `IOrderedQueryable<TEntity>` while `ApplyReadFilter` takes and returns `IQueryable<TEntity>`, so ordering
  first and filtering second would discard the ordered-ness the return type exists to guarantee. Revision 3
  deferred this to an implementer question; it was never open, only unwritten.
- **Includes sit between them** because EF Core composes `Include` additively at any point before execution,
  and placing it after the filter keeps the filter's predicate over the root entity, where it is easiest both
  to read and to translate.
- **A soft family's `ApplyReadFilter` override is the `DeletedDate == null` filter**, and
  `IDepartmentDao` rule 15 binds it: **soft deletion is the only exclusion rule.** A derived DAO that
  overrides the hook to add a further restriction has left the family contract and owns the consequences —
  its `GetAll`, `GetPaged` and `GetCount` will still agree with each other, which is the property the
  composition guarantees, but they will no longer agree with any other conforming implementation.


### The DAO constructor — `BaseDao(DbContext context)`

Every keyed and keyless base shares this constructor contract (A17). **The order of the two checks is
specified**, because a caller passing `null` should be told that, not told about a convention they did not
violate.

| Step | Behavior |
|---|---|
| 1 | `context` is `null` → **`ArgumentNullException`**. Nothing else runs |
| 2 | Resolve the identifier property per [Identifier resolution](#identifier-resolution--a8) and check its declared type against `TKey` → **`DataAccessConventionException`** on failure, naming the entity type, the key type and which of the two lookups failed |
| 3 | Capture `context`. **No `Set<TEntity>()`, no query, no connection, no model initialization** |

**Keyless bases run step 1 and step 3 only.** `RootNonIdDao<TEntity>` and its descendants have no identifier
to validate, so there is no `DataAccessConventionException` path in their construction at all.

**`Dataset` is lazy** — `Context.Set<TEntity>()` is evaluated on **first access** and cached for the DAO's
lifetime:

| Aspect | Contract |
|---|---|
| **When it is resolved** | On first read of the property, not in the constructor |
| **Caching** | Once, per DAO instance. Repeated reads return the same `DbSet<TEntity>` |
| **Unmapped entity** | `Context.Set<TEntity>()` throws `InvalidOperationException` from EF Core. That exception **propagates unwrapped**, from the first member that touches the store — not from the constructor |
| **Disposed context** | `ObjectDisposedException` from EF Core, unwrapped. The DAL's `ThrowIfDisposed()` is the guard that should have fired first; this is the backstop, not the contract |
| **Thread safety** | None. The DAO shares the concurrency rules of everything else here |

**Why lazy and not eager:** `Set<TEntity>()` forces EF Core to build the model, which reads every
configuration the context declares. Constructing a Data Access Layer — which constructs every DAO — must not
be the thing that pays that cost, and a dependency-injection host that resolves a DAL per request would pay
it at a moment of its own choosing rather than ours. Identifier validation stays in the constructor because
it is pure reflection over the entity type, costs nothing, and touches no EF machinery. **The two validations
are deliberately at different times, and that difference is a decision, not an oversight:** a mis-wired
entity is a wiring error the author can fix immediately, an unmapped entity is a *model* error that only the
configured context can report.

### Locating a row — `GetKey`, `KeyEquals`, `MatchRow`

**Three hooks, one path.** Every keyed CRUD member locates its row through `MatchRow` and through nothing
else (A12).

```csharp
protected virtual TKey? GetKey(TEntity item);                              // read the identifier off the entity
protected virtual Expression<Func<TEntity, bool>> KeyEquals(TKey? key);    // key value  → predicate
protected virtual Expression<Func<TEntity, bool>> MatchRow(TEntity item);  // entity     → predicate
```

| Hook | Default | Override when |
|---|---|---|
| `GetKey(item)` | Reads the resolved identifier property off `item` and returns it as `TKey?` | The identifier is computed rather than stored — rare, and usually a sign the entity wants a different key type |
| `KeyEquals(key)` | The expression-built `x => x.Id == key` — see [The Key Equality Predicate](#the-key-equality-predicate) | A provider mistranslates the default for your key type |
| `MatchRow(item)` | **`KeyEquals(GetKey(item))`** | The row is identified by more than the key — a tenant-scoped table where `(TenantId, Id)` is the real identity |

**The composition is the contract, and it is what makes one override sufficient.** `Get`, `Update` and
`Delete` each call `MatchRow(item)`; none of them calls `KeyEquals` or `GetKey` directly. A DAO that overrides
`MatchRow` therefore changes all three at once and cannot end up with a `Get` that finds a row its `Update`
cannot. `KeySelector` — used by the ordering default — is built over the **same resolved property**
`KeyEquals` compares, so ordering and matching can never address different columns.

#### `GetKey` and the identifier's declared type

`GetKey` resolves the property by **name**, exactly as the parent's dispatcher does — `{TypeName}Id` first,
then `Id` — and then **validates the property's declared type against `TKey`**:

| Situation | Result |
|---|---|
| Property found, declared type is `TKey` | Resolved. This is the normal case |
| Property found, declared type is **not** `TKey` — `long Id` on a `BaseDao<Order, int>` | **`DataAccessConventionException`**, from the **DAO constructor** (A17 step 2), naming the entity, the property, its declared type and `TKey` |
| Property found but **not public**, or an explicit `IBaseIdEntity<TKey>` implementation | **`DataAccessConventionException`** from the constructor. An explicit implementation compiles to a non-public interface-qualified property that neither lookup finds, **and** that EF Core could not translate a member access through in any case |
| Property has no set accessor | **`DataAccessConventionException`** from the constructor. A set accessor of any visibility is accepted because `PropertyInfo.CanWrite` is true and the parent dispatcher can invoke it through reflection |
| Neither name found | **`DataAccessConventionException`** from the constructor |

The type check is not pedantry. `IBaseIdEntity<TKey>` guarantees a member named `Id` of type `TKey` exists,
but it guarantees nothing about a property named `{TypeName}Id`, which wins the lookup. An entity carrying
`int Id` **and** `long OrderId` would resolve to `OrderId`, and without this check the mismatch would surface
as an expression-tree failure from deep inside the predicate builder, at first use, with no useful message.

#### Null resolved keys — the precise rule

**"The key is null" means `GetKey(item) is null`.** Nothing else counts, and the check happens **before**
`KeyEquals` is called and therefore before any predicate is built or any query is issued (A4).

| `TKey` | Can `GetKey` return null? | Consequence |
|---|---|---|
| `int`, `long`, `Guid`, any non-nullable value type | **No.** `TKey?` on an unconstrained type parameter is a nullability *annotation*; for a value type it is `TKey` itself | The short-circuit is unreachable. `dal.Get<T>(null)` is rejected earlier still, by the parent's `ArgumentException` |
| `int?`, `Guid?`, any `Nullable<T>` | **Yes**, when the property holds no value | Short-circuit applies |
| `string`, or any reference-type key | **Yes**, when the property is null | Short-circuit applies |

| Member | Result when `GetKey(item)` is null |
|---|---|
| `Get` | `null` |
| `Update` | `0` |
| `Delete` | `0` |

In all three cases: **no query is issued, no `SaveChanges` is called, `item` is not mutated, and nothing is
thrown.** A null resolved key is "no such row," which is an ordinary outcome — the caller error case is
`item` itself being null, and that throws.

#### `default(TKey)` is an ordinary key value — OD-3

**There is no short-circuit for `default(TKey)`, and there must not be one.**

| `GetKey(item)` returns | Behavior |
|---|---|
| `null` | Short-circuit above. **No query issued** |
| `0`, `Guid.Empty`, `""`, or any other `default(TKey)` | **Ordinary key value.** The predicate is built, the query is issued, and the call **misses normally** if no such row is stored |

An entity whose identifier has never been assigned therefore carries `0` or `Guid.Empty`, and `Get` returns
`null` / `Update` returns `0` / `Delete` returns `0` **by ordinary miss, not by special-casing**. The
observable result usually looks the same; the mechanism is not, and the difference is testable — the null
case issues no SQL and the `default(TKey)` case issues a normal parameterized `WHERE`.

**This is distinct from the null-resolved-key rule above and must not be folded into it.** That rule exists
because `WHERE x = NULL` is never true in SQL and providers rewrite null semantics differently, so answering
in the Data Access Object is the only way SQLite and SQL Server agree. Nothing analogous is true of `0`:
`WHERE Id = 0` is a perfectly ordinary predicate that every relational provider translates identically.

**And a short-circuit would be wrong, not merely unnecessary.** `0` and `Guid.Empty` are legal stored
values. A table with an identity seed of `0`, a `Guid` column carrying a deliberately-zero sentinel row, or
a natural `string` key of `""` are all things a consumer may have; a library that decided `default(TKey)`
means "unassigned" would make those rows permanently unreachable through `Get` while remaining visible
through `GetAll` — an inconsistency between two members of the same Data Access Object, invisible until
somebody's data happened to contain the value.

**The three key shapes must each be exercised by the test suite**, because they take different paths through
this logic: a value key never short-circuits, a nullable value key short-circuits on `HasValue == false`, and
a reference key short-circuits on a null reference and additionally has to survive a *stored* row whose key
column is null. See [Test Obligations](#test-obligations).

### `Get(TEntity item)`

| Aspect | Contract |
|---|---|
| **Summary** | Loads the row `MatchRow(item)` identifies — by default, the one whose identifier equals the one `item` carries |
| **Nulls** | `item` null → `ArgumentNullException`. `GetKey(item)` null → returns `null` **without issuing a query** (A4) |
| **Returns** | The matching entity as a snapshot, or `null` when no row matches. `null` is an ordinary outcome |
| **Identity** | Never the store's own tracked object. The returned snapshot **may or may not be the instance passed in**; callers must use the return value and must not assert on argument identity |
| **Filtering** | Does **not** apply `ApplyReadFilter`. A soft-deleted row is still returned by `Get`, per `IDepartmentDao` rule 8 |
| **Global query filters** | **Ignored.** The query calls `IgnoreQueryFilters()` before the `MatchRow` predicate, so a consumer-declared `HasQueryFilter` cannot hide a row from `Get` (OD-5, A28). The call is unconditional and is a no-op where no filter is declared |
| **Navigation properties** | **Only those `ApplyIncludes` declares, which by default is none** — every navigation property on the returned instance is `null` unless this DAO overrides the hook. See [Navigation Loading](#navigation-loading) |
| **Tracking** | `AsNoTracking()`, explicitly, regardless of the context's configured `QueryTrackingBehavior` |
| **Multiple matches** | Uses `SingleOrDefault`, so more than one matching row throws **`InvalidOperationException` — raised by LINQ, not by the provider** — and it propagates unwrapped. See below |
| **Idempotency** | Read-only; repeatable |
| **Side effects** | None. `item` is not mutated |

**On the duplicate-match exception.** `SingleOrDefault` materializes and counts; the exception it throws is
`System.InvalidOperationException` with a message about a sequence containing more than one element, and it
is **the same on every provider** because it comes from LINQ rather than from the store. Earlier drafts
described it as "the provider's exception," which was wrong and would have sent `Test Designer` looking for
a provider-specific type.

Two consequences worth stating:

- It is **catchable as `InvalidOperationException`**, and so is `ObjectDisposedException`. A consumer
  distinguishing them must check the type, not the base type.
- **`SingleOrDefault` is deliberate, and `FirstOrDefault` is not an acceptable substitute.** A duplicate
  identifier means the key is not a key — a schema defect, a `MatchRow` override that under-specifies the
  row, or an alternate-key identifier without a unique index. `FirstOrDefault` would hide it behind an
  arbitrary row, and which row that is could change with the query plan. Throwing is the whole point.

### `Insert(TEntity item)`

| Aspect | Contract |
|---|---|
| **Nulls** | `item` null → `ArgumentNullException` |
| **Returns** | `void` |
| **Side effects** | **The store-generated identifier is written back onto `item`.** This is the **IDENTIFIER RULE** on `IExampleDataAccess` — a convention **elected in `ProphetsWay.Example`**, not one `ProphetsWay.BaseDataAccess` promises, which documents the write-back as *"a convention left to the implementation — this library neither performs it nor verifies that it happened."* Callers rely on it. Any key the caller pre-assigned is replaced by the generated one where the store generates keys; a client-generated key (`Guid`, `string`) is used as supplied — which the rule leaves deliberately unspecified, so a caller must depend on neither |
| **Mechanism** | **The root is added; everything reachable through `item`'s navigation properties is set `Unchanged` explicitly** (OD-4, A24). Related rows are read, never written. **`Dataset.Add(item)` is wrong, and so is `Attach`** — see [Writes and the Navigation Graph](#writes-and-the-navigation-graph) |
| **Related entities** | Not inserted, not updated, and their keys are not reassigned. EF Core's relationship fix-up writes the foreign key onto the new row, so an association to a stored row is preserved |
| **Detachment** | **In a `finally`, on success and on failure**, `item` and its whole reachable graph are detached from the context (A26, OD-7). Mutating any of them afterwards must not reach the store on a later `SaveChanges` — that is the "read rather than adopted" half of the SNAPSHOT RULE. **A failed `Insert` therefore leaves nothing pending**, and the caller may fix `item` and call again on the same instance |
| **Idempotency** | **Not idempotent.** Two calls insert two rows unless a store constraint prevents it |
| **Failure** | Constraint violations surface as the provider's exception, unwrapped |
| **Transactions** | Auto-commits outside a transaction; enrolls inside one |

### `Update(TEntity item)`

| Aspect | Contract |
|---|---|
| **Nulls** | `item` null → `ArgumentNullException`. `GetKey(item)` null → returns `0` without querying (A4) |
| **Returns** | **`1` when a row with that identifier is stored, `0` when none is. Never a negative number, and never greater than `1`** — the **ROW COUNT RULE** on `IExampleDataAccess`, a convention **elected in `ProphetsWay.Example`** rather than inherited from `ProphetsWay.BaseDataAccess`, which says only *"typically 1"*. A write reached through this contract addresses a single row |
| **The forced change** | 2.2.x threw on an absent row (EF Core branch) or silently inserted one (EF6 branch). **Neither survives.** See [Forced Behavior Changes](#forced-behavior-changes) |
| **The no-op subtlety** | EF's `SaveChanges()` returns `0` when the incoming values are identical to the stored ones. **That must not leak.** The return value reports whether the row *existed*, not whether EF detected a change: row found → `1`, row absent → `0`. This is provider-independent and testable, and it is the **ROW COUNT RULE**'s most easily lost clause, stated there in terms — *"`Update` reports that a row matched, not that a value changed"* |
| **Which values are written** | Every mapped scalar on `item`. `BaseSoftDao` narrows this — see below |
| **Mechanism** | **A tracked fetch through `MatchRow`, then `Entry(stored).CurrentValues.SetValues(item)`, then `SaveChanges()`** (A22). The fetch calls `IgnoreQueryFilters()` (A28) and applies neither `ApplyReadFilter` nor `ApplyIncludes` — **but a consumer's model-level `AutoInclude` still reaches it**, because that is applied at query compilation and this library has no override for it, so the fetch then materializes and tracks the auto-included graph (H7; see [Navigation Loading](#model-level-autoinclude-is-an-equally-valid-path)). `ExecuteUpdate` is rejected — see below |
| **What it cannot do** | **Repoint a navigation property that has no foreign-key scalar on the entity** (A25). `SetValues` reads properties by name off the CLR type, and a shadow foreign key has none — so `Update(transaction)` can never change `Transaction.User`. The call still returns `1` and nothing reports the dropped change. See [Writes and the Navigation Graph](#writes-and-the-navigation-graph) |
| **Concurrent deletion** | **`DbUpdateConcurrencyException`, propagated unwrapped.** The fetch and the `SaveChanges` are two round trips, and outside a transaction there is a window between them. If another connection removes the row inside that window, EF Core finds zero rows affected and throws. **It is not converted to `0`** |
| **Side effects** | None on `item` for `BaseDao`. **The fetched row, `item`, and everything reachable from either are detached in a `finally`** — after `SaveChanges()` returns or throws (A26, OD-7). On the fetched side that is **the row itself, plus whatever a consumer's `AutoInclude` materialized with it, and nothing else**: the locating fetch loads no navigation of its own ([OD-8](#owner-decisions-taken-during-revision-8)) |
| **Idempotency** | Idempotent in effect — a second identical call rewrites the same values and returns `1` again |

**The `0` return and `DbUpdateConcurrencyException` answer two different questions, and conflating them is
the defect this row exists to prevent.** `0` means *the locating query found no row*. The exception means
*a row was found and had vanished by the time the write was issued*. A caller that treats the exception as
"absent, same as `0`" has decided a lost update is normal.

**This is specified symmetrically with [`Delete`](#deletetentity-item)** — same tracked-fetch-then-
`SaveChanges` shape, same window, same exception, same refusal to swallow it. Revision 3 specified the race
for `Delete` and left it open for `Update`, which invited an implementation that swallowed one and not the
other.

**Closing the window is the caller's move, not the library's.** Wrapping the pair in
`TransactionStart`/`TransactionCommit` puts both round trips inside one transaction, where the provider's
isolation level governs. This library adds no retry, no optimistic-concurrency token and no re-read: those
are policy, and policy belongs to the consumer.

**Why `SetValues` and not `ExecuteUpdate`** (A22). `ExecuteUpdate` is attractive — one round trip, no tracked
fetch, no change tracker — and it **cannot satisfy this contract on both certified legs**. It returns a
provider row count, and the two providers count different things: **SQLite reports rows *modified***, so an
update whose values already match the stored row reports `0`, while **SQL Server reports rows *matched***,
so the same call reports `1`. The "row existed → `1`, even when the values are identical" rule would then be
true on one certified provider and false on the other, which is precisely the split-semantics failure
[D1](purpose-and-scope.md#owner-decisions--2026-08-15) exists to end. **That rule is the ROW COUNT RULE on
`IExampleDataAccess`, and its justification there is this paragraph nearly verbatim** — an implementation
reporting rows-*modified* returns `0` where one reporting rows-*matched* returns `1`, *"two conforming Data
Access Layers disagreeing, which is the exact failure this repository exists to disprove."* Reaching the same
conclusion independently is corroboration; citing it is what lets a reader check that this library is
implementing a stated rule rather than inventing one. It also bypasses the change tracker,
so the detachment guarantee above would have to be re-established by other means. **The question is closed,
not deferred.** The performance observation survives as an observation: on a large batch the tracked fetch
is the dominant cost, and a consumer who needs that cost gone writes a custom DAO method with
`ExecuteUpdate` in it, where they own the row-count semantics they get.

### `Delete(TEntity item)`

| Aspect | Contract |
|---|---|
| **Nulls** | `item` null → `ArgumentNullException`. `GetKey(item)` null → returns `0` without querying |
| **Semantics** | **Hard delete** on `BaseDao`. The row is genuinely removed |
| **Returns** | `1` when the row existed, `0` when it did not. Never a negative number and never greater than `1` — the **ROW COUNT RULE**, as for [`Update`](#updatetentity-item) |
| **Implementation note** | The row is located with `MatchRow`, and the *located* instance is removed. `Dataset.Remove(item)` on a detached instance attaches it as `Deleted` and throws `DbUpdateConcurrencyException` when the row is absent — which would break the `0` contract. The locating fetch calls `IgnoreQueryFilters()` (A28), applies no includes of its own, and is nevertheless reached by a consumer's model-level `AutoInclude` (H7) |
| **Detachment** | **`item`, the located row, and everything reachable from either are detached in a `finally`** — after `SaveChanges()` returns or throws (A26, OD-7). As for `Update`, the located row carries a graph only where an `AutoInclude` put one there ([OD-8](#owner-decisions-taken-during-revision-8)) |
| **Concurrent deletion** | **`DbUpdateConcurrencyException`, propagated unwrapped** — the same window as [`Update`](#updatetentity-item). A row that the locating query found and another connection removed before `SaveChanges` is a lost race, not an absent row, and it is **not** converted to `0` |
| **Idempotency** | **Idempotent.** A second call returns `0` and throws nothing |
| **Failure** | Referential-integrity violations surface as the provider's exception, unwrapped |

### `GetAll`, `GetPaged`, `GetCount`

| Aspect | Contract |
|---|---|
| **`item`** | Type selector, never read (S12), `null` through the dispatcher, annotated `TEntity?` |
| **`GetAll` returns** | Every row admitted by `ApplyReadFilter`, ordered by `ApplyStableOrder`, as a fresh non-null `IList<TEntity>`; empty when there are none |
| **`GetPaged` returns** | The window `skip`/`take` over that same filtered, ordered sequence. Boundaries per [Paging boundaries](#paging-boundaries) |
| **`GetCount` returns** | The count of that same filtered sequence — **it must agree with `GetAll().Count`**, which is what makes a pager's last page correct |
| **Ordering** | `GetAll` and `GetPaged` order **identically**, so successive windows partition a full pass with no overlap and no omission |
| **Navigation properties** | `GetAll` and `GetPaged` apply `ApplyIncludes` — by default, none. **`GetCount` does not**, because it materializes no entity |
| **Tracking** | `AsNoTracking()` on all three |
| **Nulls** | Never returns `null` |
| **Idempotency** | Read-only; stable across calls while the stored data is unchanged |

### `BaseSoftDao<TEntity, TKey>` — the soft-delete deltas

The soft classes **override** `virtual` members (A2). They must never use `new`. Today's `new int Delete(...)`
means `((BaseDao<Department,int>)departmentDao).Delete(d)` **hard-deletes a soft-delete entity** — a silent
data-loss bug that no consumer would suspect, and one the Example contract cannot tolerate.

Behavior below is the generalization of `IDepartmentDao` rules 1–9 and 15, which are the family's worked
statement of what soft delete means.

| Member | Contract |
|---|---|
| **`Insert`** | Stamps `CreatedDate = GetCurrentTimestamp()` and forces `UpdatedDate = null` and `DeletedDate = null`, **whatever the caller assigned**, then inserts. All three values, and the generated key, are visible on `item` when the call returns |
| **`Update`** | Stamps `UpdatedDate = GetCurrentTimestamp()`. Writes the entity's own data only: **incoming `CreatedDate`, `UpdatedDate` and `DeletedDate` are ignored and the stored `CreatedDate` and `DeletedDate` are preserved.** Returns `1` when a row with that identifier is stored, `0` otherwise. **Only `UpdatedDate` travels back onto `item`** |
| **`Update` — mechanism** (A30) | The tracked fetch plus `SetValues` of [A22](#revision-4-additions), with **the three timestamp properties excluded from the copy** — or restored from the tracked entry immediately after it. Which of the two, is the implementer's; that they do not arrive from `item`, is not. **`SetValues` copies every mapped scalar by name**, timestamps included, so a soft `Update` written the plain way overwrites the stored `DeletedDate` with whatever the caller's instance holds — normally `null` — and **silently un-deletes the row**. `IDepartmentDao`'s own WHY paragraph names `DeletedDate` as *"the one that gets broken"*; this row is why it does not get broken here. `CreatedDate` fails the same way, less visibly |
| **`Update` on a deleted row** | **Allowed**, and behaves exactly as above. The row stays deleted |
| **`Delete`** | **Does not remove the row.** Stamps `DeletedDate = GetCurrentTimestamp()` when a **live** row with that identifier is stored, and returns `1`; the stamped value is written back onto `item`. `CreatedDate` and `UpdatedDate` are not touched |
| **`Delete` when already deleted or absent** | Returns `0`, changes nothing, and **does not refresh an existing `DeletedDate`** — so `Delete` is idempotent. No write-back onto `item` occurs when it returns `0` |
| **`Get`** | **Returns soft-deleted rows.** `null` only when no row with that identifier was ever stored |
| **`GetAll` / `GetPaged` / `GetCount`** | **Omit soft-deleted rows** and must agree with one another. With every row deleted: two empty lists and `0` |
| **Exclusion** | **Soft deletion is the only exclusion rule.** `ApplyReadFilter` on this class adds `DeletedDate == null` and nothing else |
| **Retrieved timestamps** | `Get`, `GetAll` and `GetPaged` pass every timestamp a read materializes through `NormalizeRetrievedTimestamp`, **after materialization**, so a retrieved instance carries a meaningful `DateTime.Kind`. `CreatedDate` is a non-nullable `DateTime` on `IBaseSoftEntity` and is **always** normalized; `UpdatedDate` and `DeletedDate` are `DateTime?` and are normalized **only when they hold a value** — a `null` stays `null` and is never normalized into one |
| **Restore** | **Not supplied.** Clearing `DeletedDate` is a consumer method — see [Writing a `Restore`](#writing-a-restore) |

**Soft delete narrows what counts as a match; it does not change the counts.** Every `1` and `0` above is the
**ROW COUNT RULE** on `IExampleDataAccess` applied to a narrower notion of a row the operation applied to,
which that rule states in terms: a live department returns `1`, one already deleted returns `0` *even though
its row is still stored*. `IDepartmentDao` rules 2, 4, 5 and 6 are that application, not exceptions to it, and
`Update`'s "never a negative number, and never greater than `1`" binds here unchanged. The write-back of the
stamped `DeletedDate` and `UpdatedDate` onto `item` is the one extension of the **IDENTIFIER RULE**'s
write-back that a Data Access Object may state for itself — rule 1 there does exactly that. Both rules are
**elected in `ProphetsWay.Example`** rather than promised by `ProphetsWay.BaseDataAccess`.

#### Timestamp Policy

**Two hooks, and they are a pair.** S11 keeps the clock configurable; A13 adds the second half that makes a
default derivation actually satisfy the Example contract.

```csharp
/// <summary>Supplies the timestamp written to CreatedDate, UpdatedDate and DeletedDate.</summary>
/// <returns>The current time. The default is <see cref="DateTime.UtcNow"/>.</returns>
protected virtual DateTime GetCurrentTimestamp();

/// <summary>
/// Restores the <see cref="DateTimeKind"/> a relational store did not preserve, on a timestamp read back
/// out of the store.
/// </summary>
/// <param name="value">A timestamp as the provider materialized it — typically <c>Unspecified</c>.</param>
/// <returns>The same instant, carrying the Kind this DAO's clock produces.</returns>
/// <remarks>The default is <c>DateTime.SpecifyKind(value, DateTimeKind.Utc)</c>. It relabels; it never shifts.</remarks>
protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value);
```

The `UseUtcTime` constructor flag is **removed** — a `bool` could express two clocks and nothing else, and it
could not express a test clock at all.

| Aspect | Contract |
|---|---|
| **`GetCurrentTimestamp` default** | `DateTime.UtcNow`, whose `Kind` is `DateTimeKind.Utc` |
| **Called** | **Once per stamping operation.** The same value is used for every field that operation stamps |
| **`NormalizeRetrievedTimestamp` default** | `DateTime.SpecifyKind(value, DateTimeKind.Utc)` — a **relabel, not a conversion.** It changes no instant and adds no offset |
| **Where it is applied** | To `CreatedDate`, `UpdatedDate` and `DeletedDate` on every entity **this Data Access Object's** `Get`, `GetAll` and `GetPaged` materialize, after materialization and before the instance is handed back. Never inside a predicate, so it cannot affect translation. **A soft entity materialized as an *include* on some other Data Access Object's query is not reached** — that DAO owns the query and cannot see this hook (G12); see [Including a soft-delete entity bypasses its `ApplyReadFilter`](#including-a-soft-delete-entity-bypasses-its-applyreadfilter--and-that-is-correct) |
| **Nulls** | **`CreatedDate` is a non-nullable `DateTime`** on `IBaseSoftEntity`, so it is always present and always normalized. `UpdatedDate` and `DeletedDate` are `DateTime?`; the hook is called only when they hold a value, and a `null` is left `null` |
| **Values written back by a write** | Come from `GetCurrentTimestamp` directly and are **not** normalized — they never went to the store and never lost their `Kind` |

**Why the second hook exists.** `IDepartmentDao` rule 18 requires every stamped timestamp to carry
`Kind == Utc` **both** on the instance written back to the caller **and** on an instance later retrieved by
`Get`, `GetAll` or `GetPaged`. Relational providers do not store `Kind`: SQL Server `datetime2` and SQLite's
text/numeric date storage both round-trip a `DateTime` as `Unspecified`. So with the clock hook alone, a
default-derived `BaseSoftDao` satisfies the write half of rule 18 and **fails the read half on both certified
providers** — the Example suite would not pass against a conforming implementation of this library, which is
not an acceptable state for the package whose job is to implement that contract.

##### The Timestamp Pair Rule — A13

> **`GetCurrentTimestamp()` and `NormalizeRetrievedTimestamp(DateTime)` are one policy stated from two
> directions. Override both, or neither. Overriding one is a defect.**

That is the rule in full, and it is stated **once, here**, so the two places that declare the pair can cite
it rather than restate it. The library cannot check that an override of one agrees with an override of the
other, so this is an obligation on the deriving Data Access Object.

**The pair is declared on two unrelated branches, and there is no shared base to hang it on.** `BaseSoftDao`
derives from `BaseDao`; `RootSoftNonIdDao` derives from `RootNonIdDao`; the two roots have nothing in common
because one is keyed and one is not, and C# has no multiple inheritance. **Four override sites therefore
exist**, and no compiler check spans them:

| Declaration site | Override sites beneath it |
|---|---|
| `BaseSoftDao<TEntity, TKey>` | `GetCurrentTimestamp`, `NormalizeRetrievedTimestamp` — inherited unchanged by `BaseSoftGetAllDao` and `BaseSoftPagedDao` |
| `RootSoftNonIdDao<TEntity>` | `GetCurrentTimestamp`, `NormalizeRetrievedTimestamp` — inherited unchanged by `BaseSoftNonIdDao` |

**What is shared is the behavior, not the declaration.** Both defaults — `DateTime.UtcNow` and
`DateTime.SpecifyKind(value, DateTimeKind.Utc)` — and the post-materialization walk that applies
`NormalizeRetrievedTimestamp` to the three properties of an `IBaseSoftEntity` are supplied by **one
`internal static` helper**, called from both branches. There is exactly one copy of the logic; the two
branches differ only in declaring the `protected virtual` members that expose it. **A change to the default
therefore cannot land on one branch and miss the other.**

**An interface was considered and does not work.** `protected virtual` members cannot be declared on an
interface, and promoting them to `public` to make one possible would put a clock and a `DateTime`
relabeler on the public surface of every soft Data Access Object — API a consumer would be able to call and
had no reason to see. A shared abstract base does not work either, for the inheritance reason above. **The
duplication of the two *declarations* is structural and accepted; the duplication of the *logic* is not, and
does not exist.**

**`Test Designer` must exercise the rule on both branches**, not on the keyed one alone: the same
consistent-override test and the same inconsistent-override hazard test, once against a `BaseSoftDao`
descendant and once against a `RootSoftNonIdDao` descendant. A test that covers only the keyed branch leaves
half the override sites unguarded, which is exactly the risk having four of them creates.

**Two conforming pairings, for reference:**

```csharp
// A DAO that keeps local time. Both halves, or neither.
protected override DateTime GetCurrentTimestamp()
	=> DateTime.Now;                                                    // Kind = Local

protected override DateTime NormalizeRetrievedTimestamp(DateTime value)
	=> DateTime.SpecifyKind(value, DateTimeKind.Local);                 // relabel to match
```

```csharp
// A DAO storing an explicitly chosen zone. Note that Kind is Unspecified in both directions,
// which is the honest answer — the zone is knowledge held by the DAO, not by the value.
private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");

protected override DateTime GetCurrentTimestamp()
	=> TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);          // Kind = Unspecified

protected override DateTime NormalizeRetrievedTimestamp(DateTime value)
	=> value;                                                           // already Unspecified; leave it
```

**Overriding one and not the other is a defect**, and it is a silent one: override the clock to `DateTime.Now`
and leave normalization at the default, and a stamped value comes back relabeled `Utc` while holding a local
wall-clock reading — an instant wrong by the machine's offset, with nothing to indicate it. `Test Designer`
should pin an inconsistent pairing as a demonstrated hazard rather than assume nobody will write one.

**`DateTimeKind` and DST — read before overriding.** `DateTime` carries a `Kind` in memory that most
relational providers **do not store**. A value written as `Local` or as an arbitrary zone comes back as
`Unspecified`, so the offset that made it meaningful is gone and no later reader can recover it — the
normalization hook can only *relabel* what comes back, and relabeling a value the store already stripped is
a claim about the value, not a recovery of it. Local time is also **not monotonic**: during a DST fall-back
the same wall-clock hour occurs twice, so `CreatedDate` ordering can invert, a range query can double-count
or miss rows, and two rows stamped an hour apart can compare equal. During the spring-forward gap an hour
does not exist at all. UTC has none of these problems, which is why it is the default for both hooks and why
`IDepartmentDao` rule 18 specifies `DateTime.UtcNow` outright.

**This library does not prescribe arbitrary-zone storage and offers no conversion helper.** An override
returning something other than UTC is the deriving consumer's decision, made with the consequences above
understood. A consumer who needs a zone preserved should store the offset in a column of their own, or map
the column as `DateTimeOffset` — which this library neither requires nor obstructs, because the entity
contract in `ProphetsWay.BaseDataAccess` is what fixes those properties as `DateTime`.

**The normalization hook is a DAO-level mechanism on purpose.** The alternative — a global
`DateTimeKind`-restoring value converter registered on `BaseEFContext` — was rejected: it would apply only to
consumers who happened to derive from that optional type, making conformance depend on a base-class choice
that is explicitly free (see [The Context Base](#the-context-base--baseefcontext)), and it would apply to
*every* `DateTime` column in the model rather than the three this contract governs. The two mechanisms must
not both exist — a value converter plus a normalization hook applied to the same value is how a double
conversion ships.

#### Writing a `Restore`

`Restore` is the Example's custom method, not a library capability — it is the "1%" the paradigm leaves to
the consumer. It cannot be expressed through `Update`, because `Update` deliberately preserves the stored
`DeletedDate`. `Context` and `Dataset` being `protected` rather than private (S10) is exactly what makes it
writable:

```csharp
public int Restore(Department item)
{
	if (item is null) throw new ArgumentNullException(nameof(item));

	var stored = Dataset.AsTracking().SingleOrDefault(MatchRow(item));
	if (stored is null) return 0;

	try
	{
		if (stored.DeletedDate is null) return 0;

		stored.DeletedDate = null;
		Context.SaveChanges();
		item.DeletedDate = null;
		return 1;
	}
	finally
	{
		Context.Entry(stored).State = EntityState.Detached;
	}
}
```

**Read the `return 1` before copying this.** It is deliberate, and it is the same rule
[`Update`](#updatetentity-item) states: the return value reports **whether the row existed**, not what
`SaveChanges()` counted. `SaveChanges()` returns `0` when the stored `DeletedDate` was already `null` — but
the guard at the top of the `try` has already returned `0` for that case, so by the time `SaveChanges()` runs
the row both exists and is changing. Returning its count instead would be correct here by coincidence and wrong
the moment the method grew a second write. The count is not captured because nothing reads it; an earlier
revision assigned it to a variable it then ignored, which taught the opposite of the rule this sample is
meant to teach.

**And read the `MatchRow(item)`.** An earlier revision located the row with
`SingleOrDefault(KeyEquals(item.Id))`, which teaches the pattern [A12](#revision-2-additions) exists to
prevent: on a Data Access Object carrying a tenant-scoped `MatchRow` override, that `Restore` restores across
tenants while `Get`, `Update` and `Delete` on the same DAO do not. **Every member that locates a row goes
through `MatchRow`, custom members included** — that is what makes one override sufficient.

**Read the `try`/`finally` too — it is the reason the guard was split in two.** `AsTracking()` means `stored`
is tracked from the moment the query returns, so **every** exit after that owes a detach: the success path, a
throwing `SaveChanges()`, and the early `return 0` that never writes anything. An earlier revision detached on
the success path only and folded `stored.DeletedDate is null` into the null check, which left a tracked entity
behind on two of the three exits — and on the throwing one it left `stored` carrying `DeletedDate = null`
**pending**, so the next successful `SaveChanges` on the shared context (S8), through any Data Access Object on
the layer, un-deleted the row. **That is the exact shape [OD-7](#owner-decisions-taken-during-revision-6)
retracted, and this is the document's only worked custom write** — what is written here is what gets copied.

**And read where the query starts.** `Dataset` is the **raw** set, and starting there is what keeps
`ApplyReadFilter` off this query — the one property `Restore` cannot do without. `BaseSoftDao.ApplyReadFilter`
adds `DeletedDate == null`, which excludes **exactly** the rows `Restore` exists to reach, so a `Restore`
composed on top of the trio's filtered query would find nothing, always, and would return `0` for every row it
was asked about. That is derivable from [A23](#revision-5-additions) — the hook receives the raw `Dataset`,
and a custom method composes from wherever it chooses — but it was never said here, and it is the first thing
a reader copying this sample needs to know.

**The `finally` is owed by every custom write, not only one that loads a graph** (A26, OD-7). The graph clause
is about *what* to detach, not *when*: this query declares no `Include`, so the one entity **is** the whole
reachable graph; a custom method that loads a graph detaches the rest of it in the same `finally`. **`item` is
not part of it.** A26's scope names the argument's graph alongside the fetched row's, but this method never
tracks `item` — it reads its identifier through `MatchRow` and writes `DeletedDate` onto it after the fact —
so there is nothing on the argument side to detach, and `stored` is the whole of what the `finally` owes. And
a consumer who has declared a global query filter on `Department` must add `.IgnoreQueryFilters()` to this
query, or the filter hides those same rows a second time — the library applies it to its own locating paths
(A28), not to a method it never sees.

Note also what this sample does **not** do: it declares no `ThrowIfDisposed()`, because it is a method on a
**Data Access Object**, and a DAO holds no disposal state (see
[Data Access Object lifetime](#data-access-object-lifetime)). The guard belongs on the Data Access Layer's
`Restore(Department)` forwarder, which is a member the consumer declares.

---

## The Keyless DAO Families

**Four classes** (A1, A14). This is the S5 half of the design, and the layering is the whole point.

### Why the layering is inverted relative to the keyed families

`IBasePagedDao<T> : IBaseDao<T>`. So a keyless DAO that wanted paging by declaring `IBasePagedDao<T>` would
be forced to also declare `Get(T)` and `Update(T)` — the two members `ICompanyResourceDao` documents at
length as **meaningless** for an entity whose identity is a pair of foreign keys. S5 forbids that.

The resolution is to put the plumbing in classes that **implement no capability interface at all**, and to
expose every operation on them as **conventional public methods**. The three layers then divide as follows,
and the middle one is the one earlier drafts got wrong:

1. **The consumer's DAO interface declares the conventional methods it supports** — `Insert`, `Delete`,
   `GetAll` for a join table — inheriting nothing from `ProphetsWay.BaseDataAccess`. The base class satisfies
   those declarations implicitly, because the signatures match.
2. **The concrete Data Access Layer must publish a public forwarding method** for anything the generic
   dispatcher is expected to reach. `dal.GetPaged<CompanyResource>(0, 10)` resolves
   `GetPaged(CompanyResource, int, int)` **on `ExampleDataAccess`** — see
   [How the dispatcher reaches a Data Access Object](#how-the-dispatcher-reaches-a-data-access-object--it-does-not).
   A capability sitting unforwarded on the DAO base is **not** dispatcher-reachable, and an earlier draft's
   claim that it "works with no interface involved anywhere" was false: no *interface* is involved, but a
   **forwarding method** absolutely is.
3. **The DAO base supplies the reusable implementation** the forwarder ultimately calls, plus `protected`
	cores (`GetCore`, `UpdateCore`) for the shapes a `Root` type deliberately does not publish.

```csharp
namespace ProphetsWay.EFTools
{
	/// <summary>
	/// Plumbing for a Data Access Object over an entity with no single identifier. Implements no
	/// <c>ProphetsWay.BaseDataAccess</c> capability interface, so deriving from it commits you to nothing:
	/// your own Data Access Object interface declares the subset you support.
	/// </summary>
	public abstract class RootNonIdDao<TEntity>
		where TEntity : class, IBaseEntity
	{
		protected RootNonIdDao(DbContext context);

		protected DbContext Context { get; }
		protected DbSet<TEntity> Dataset { get; }

		public virtual void Insert(TEntity item);
		public virtual int Delete(TEntity item);
		public virtual IList<TEntity> GetAll(TEntity? item);
		public virtual IList<TEntity> GetPaged(TEntity? item, int skip, int take);
		public virtual int GetCount(TEntity? item);

		/// <summary>The predicate identifying the one stored row that corresponds to <c>item</c>.</summary>
		protected abstract Expression<Func<TEntity, bool>> MatchRow(TEntity item);

		/// <summary>
		/// A total ordering over the set, applied by GetAll and GetPaged alike. <b>The default throws
		/// NotSupportedException</b> — a Data Access Object publishing any read member must override it.
		/// </summary>
		protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query);

		/// <summary>
		/// Restricts which stored rows the retrieval members may see. <b>Defaults to identity</b> on this class
		/// — no row is hidden. Receives the raw Dataset query and runs first (A20).
		/// </summary>
		protected virtual IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query);

		/// <summary>
		/// Declares which navigation properties a read materializes. <b>Defaults to identity</b> — the base
		/// members load none (A18). Receives the row-restricted query for its path (A23).
		/// </summary>
		protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query);

		// --- reusable cores, for a Data Access Object that chooses to publish these shapes ---
		protected virtual TEntity? GetCore(TEntity item);
		protected virtual int UpdateCore(TEntity item);
	}

	/// <summary>A keyless Data Access Object that does publish the <see cref="IBaseDao{T}"/> shape.</summary>
	public abstract class BaseNonIdDao<TEntity> : RootNonIdDao<TEntity>, IBaseDao<TEntity>
		where TEntity : class, IBaseEntity
	{
		protected BaseNonIdDao(DbContext context) : base(context) { }

		public virtual TEntity? Get(TEntity item);   // GetCore(item)
		public virtual int Update(TEntity item);     // UpdateCore(item)
	}

	/// <summary>
	/// Keyless plumbing with soft-delete semantics and — like <see cref="RootNonIdDao{TEntity}"/> — no
	/// capability interface. This is the base for a genuinely keyless soft-delete Data Access Object.
	/// </summary>
	public abstract class RootSoftNonIdDao<TEntity> : RootNonIdDao<TEntity>
		where TEntity : class, IBaseSoftEntity
	{
		protected RootSoftNonIdDao(DbContext context) : base(context) { }

		// The Timestamp Pair Rule (A13) binds these two here exactly as it does on BaseSoftDao.
		// Same names, same defaults, same must-be-overridden-together obligation; the defaults are
		// supplied by one internal helper so the two declaration sites cannot drift.
		protected virtual DateTime GetCurrentTimestamp();
		protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value);

		public override void Insert(TEntity item);
		public override int Delete(TEntity item);
		public override IList<TEntity> GetAll(TEntity? item);
		public override IList<TEntity> GetPaged(TEntity? item, int skip, int take);
		protected override IQueryable<TEntity> ApplyReadFilter(IQueryable<TEntity> query); // DeletedDate == null
		protected override TEntity? GetCore(TEntity item); // normalizes timestamps after materialization

		/// <summary>Soft Update: stamps UpdatedDate, preserves the stored CreatedDate and DeletedDate.</summary>
		protected override int UpdateCore(TEntity item);
	}

	/// <summary>
	/// A keyless soft-delete Data Access Object that also publishes the <see cref="IBaseDao{T}"/> shape,
	/// keyed by the <c>MatchRow</c> predicate rather than by an identifier.
	/// </summary>
	public abstract class BaseSoftNonIdDao<TEntity> : RootSoftNonIdDao<TEntity>, IBaseDao<TEntity>
		where TEntity : class, IBaseSoftEntity
	{
		protected BaseSoftNonIdDao(DbContext context) : base(context) { }

		public virtual TEntity? Get(TEntity item);   // GetCore(item) — returns soft-deleted rows
		public virtual int Update(TEntity item);     // UpdateCore(item) — soft semantics from RootSoftNonIdDao
	}
}
```

### Why `RootSoftNonIdDao` had to exist — A14

In the previous draft `BaseSoftNonIdDao<TEntity>` derived from `BaseNonIdDao<TEntity>`, so **the only route
to keyless soft delete ran through `IBaseDao<TEntity>`**. A join table that soft-deletes — an
`ICompanyResourceDao` that keeps its history rather than removing rows — would have been forced to publish
`Get(T)` and `Update(T)`, the exact two members S5 and `ICompanyResourceDao` say are meaningless for it. The
escape available was to derive `RootNonIdDao` and hand-write the soft-delete stamping, which is the
duplication this library exists to remove.

Inserting `RootSoftNonIdDao<TEntity>` between them costs one type and closes the hole:

| You need | Derive from |
|---|---|
| Keyless, hard delete, no `IBaseDao` | `RootNonIdDao<TEntity>` |
| Keyless, hard delete, with `IBaseDao` | `BaseNonIdDao<TEntity>` |
| Keyless, **soft** delete, **no** `IBaseDao` | **`RootSoftNonIdDao<TEntity>`** |
| Keyless, soft delete, with `IBaseDao` | `BaseSoftNonIdDao<TEntity>` |

The `Base`/`Root` distinction now holds in both halves of the keyless set, which is the property that made
the rule worth having. `Get` and `Update` are implemented **once each** — as `GetCore` and `UpdateCore` on
`RootNonIdDao`, both overridden to soft semantics on `RootSoftNonIdDao` — and the two `Base` types are thin
publishers over them, so the extra class adds no duplicated logic. `BaseSoftNonIdDao<TEntity>` is now
explicitly the **opt-in keyed-by-predicate shape**: you take it when your keyless entity really does support
single-row retrieval and in-place update through `MatchRow`, and you take `RootSoftNonIdDao<TEntity>` when it
does not.

#### `UpdateCore` is not sealed — A21

Revision 3 declared `RootSoftNonIdDao.UpdateCore` as **`protected sealed override`** while leaving
`BaseSoftNonIdDao.Update` `public virtual`. **That seal is removed.**

It bought nothing. A consumer who wants hard-update semantics on a soft keyless Data Access Object overrides
the **public** `Update` and writes whatever they like; the sealed `protected` core underneath it is not in
their way. So the seal blocked no misuse — it only blocked the *supported* customization this document
promises two screens earlier, where "every public member is `virtual` so a DAO with a genuine special case
overrides one method rather than abandoning the base" is stated as a design principle.

**Name the invariant it was believed to protect, and it turns out to be carried by something else.** The
invariant is A2: *a soft-delete Data Access Object never hard-updates or hard-deletes through a base-typed
reference.* That is a statement about `new` versus `override`, and it holds because `RootSoftNonIdDao`
**overrides** `UpdateCore` rather than hiding it — virtual dispatch reaches the soft body through a
`RootNonIdDao<TEntity>`-typed reference whether or not the override is sealed. `sealed` prevents a *further*
override one level down; it does nothing about the base-typed reference A2 is about.

Unsealing also removes an asymmetry Revision 3 left unexplained: `GetCore` was overridden and **not** sealed
on the same class, for no stated reason. Both are now plain overrides, and the rule is uniform — **no member
of any type in this library is `sealed` except `BaseEFDataAccess.Dispose()`**, which is sealed for the reason
A7 gives and which has a `DisposeCore()` hook precisely so the customization is not lost.

The previous draft recorded this type as *Deferred* in
[Reclassified, Deferred and Rejected](#reclassified-deferred-and-rejected) on the
reasoning that soft delete needs the same locate-and-update capability `IBaseDao` expresses. That reasoning
conflated **needing a locating predicate** with **publishing `Get` and `Update`**: the stamping is internal,
it uses `MatchRow` directly, and nothing about it requires the DAO to expose either member to a caller. The
entry is reclassified below.

### The two hooks — one required at compile time, one at first use

| Hook | Shape | Why |
|---|---|---|
| `MatchRow(TEntity item)` | **`abstract`** on `RootNonIdDao` | With no identifier there is nothing to derive a predicate from. Only the deriving DAO knows a `CompanyResource` is matched on `CompanyId && ResourceId`. **`Delete`, `GetCore` and `UpdateCore` are all built on it**, and `Delete` is published by *every* keyless base — so the hook is needed by every keyless DAO that exists, and a compile-time demand costs nobody anything |
| `ApplyStableOrder(IQueryable<TEntity>)` | **`virtual`**, default throws `NotSupportedException` (A15) | This is the ordered-keyless-paging requirement in S5, and it is needed **only** by the read trio. A write-only join DAO that publishes `Insert` and `Delete` should not be taxed with naming a total order over a set it never reads |

**`ApplyStableOrder` on the keyless bases throws until it is overridden**, and the three read members throw
with it:

| Member | Behavior with no `ApplyStableOrder` override |
|---|---|
| `GetAll` | **`NotSupportedException`** |
| `GetPaged` | **`NotSupportedException`**, thrown *after* the `ArgumentOutOfRangeException` check on `skip`/`take` — argument validation still comes first |
| `GetCount` | **`NotSupportedException`** |
| `Insert`, `Delete`, `GetCore`, `UpdateCore` | Unaffected. They never order anything |

The exception message must name the type and the override: *"`CompanyResourceDao` publishes a retrieval
member but does not override `ApplyStableOrder`. Override it to return a total ordering over
`CompanyResource`."* A message that merely says "not supported" wastes the one advantage this shape has over
an abstract method.

**Why `GetCount` throws too, when counting needs no order — and by what mechanism.** The three read members
are contractually bound to agree — `GetCount` must equal `GetAll().Count`, which is what makes a pager's last
page correct. A `GetCount` that worked while its two partners threw would publish one third of a read surface
and invite a consumer to build a pager on it that cannot fetch a page. The trio moves together.

**It reaches the exception by invoking the hook and discarding what it returns** (A27). That is the whole
mechanism, and it is stated because the alternative reading — that `GetCount` inspects whether the hook was
overridden — is not something a base class can do. Two consequences `Test Designer` must be able to predict:

- **An `ApplyStableOrder` override with a side effect runs during a count.** It is invoked exactly once per
  `GetCount` call, on `ApplyReadFilter`'s output.
- **No `ORDER BY` reaches the store.** The counted query is the filtered one; the ordered query the hook
  returned is dropped on the floor. Verify against the **emitted command**, not against the returned number —
  see [Observing the generated SQL](#observing-the-generated-sql--the-seam).

**The trade this reverses.** The previous draft made `ApplyStableOrder` `abstract`, buying a compile-time
error. That is the better failure mode in the abstract, but it charged the cost to the wrong DAO: the
write-only join table is the exact shape S5 was written to serve, and it would have had to invent an ordering
for a set it never enumerates. `NotSupportedException` moves the failure to first use of a member the DAO
chose to publish, where the author is already looking, and it keeps the hook overridable by the DAOs that do
publish reads. **`MatchRow` keeps its `abstract` shape** for the opposite reason — there is no keyless DAO
that does not need it.

**`Test Designer` must pin both halves of A15:** a keyless DAO with no `ApplyStableOrder` override throws
`NotSupportedException` from `GetAll`, `GetPaged` and `GetCount` and **succeeds** at `Insert` and `Delete`;
and the same DAO with the override in place passes the full ordering and paging obligations. Without the
first test the default silently becomes a broken read; without the second the exception has no counterpart.

#### `NotSupportedException` is reachable through the dispatcher, and that is accepted — R4-S6

**Stated plainly because it is an escape from the parent's exception vocabulary.** A keyless Data Access
Object that does not override `ApplyStableOrder`, whose Data Access Layer nevertheless forwards
`GetAll(TEntity?)`, surfaces `NotSupportedException` from **`dal.GetAll<T>()`** — a dispatcher entry point.
That type appears in neither `IBaseDataAccess`'s documented exception set nor `IBaseDao`'s.

**It is accepted rather than removed, and `abstract` is not reinstated.** The trade A15 records still holds:
making the hook `abstract` taxes the write-only join Data Access Object — the exact shape S5 was written for
— with inventing an ordering over a set it never enumerates, to buy a compile error for a member it does not
publish. Reversing that to tidy an exception list is the wrong trade.

Three things make the escape tolerable, and all three must be true:

1. **It is a wiring error, not a runtime condition.** It fires on the first call, every call, deterministically,
   in a build the author has just wired up. It cannot appear later in production against data that changed.
2. **It cannot reach a consumer of a conforming Data Access Layer.** The exception exists to be seen by the
   person writing the DAL, whose own test run is the first thing to hit it.
3. **Its message names the type and the missing override** — see the paragraph above. A `NotSupportedException`
   that merely said "not supported" would be strictly worse than the abstract method it replaced, which is
   why the message is a contract term with a test obligation of its own (M7).

**What this does *not* license.** `NotSupportedException` is the only addition to the parent's exception
vocabulary this library makes. Every other failure maps to a type the parent already documents —
`ArgumentNullException`, `ArgumentOutOfRangeException`, `ArgumentException`, `InvalidOperationException`,
`ObjectDisposedException`, `DataAccessConventionException` — or is a provider exception propagated unwrapped.
An implementer who finds themselves reaching for a second novel exception type should treat that as a design
signal, not a precedent.

**`Purpose Refiner` should note this for the parent repository.** If `ProphetsWay.BaseDataAccess` ever grows
a conformance suite ([BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md)), the
question of whether an implementation may widen the exception vocabulary is one that suite will have to
answer. This document records the first instance of it; it does not settle it for the family.

### Keyless member contracts

Identical to the keyed contracts above, with the identifier lookup replaced by `MatchRow`. The differences
worth stating:

| Member | Keyless contract |
|---|---|
| `Insert` | `ArgumentNullException` on null. **Assigns nothing back onto `item`** — there is no generated identifier. `item` **and its whole reachable graph** are detached in a `finally`, on success and on failure (A26, OD-7), and the graph is set `Unchanged` rather than inserted (OD-4, A24) |
| `Delete` | Hard delete on `RootNonIdDao`/`BaseNonIdDao`, soft on the two soft types. Locates via `MatchRow`; `1` when the row existed, `0` when not; idempotent |
| `GetCore` *(protected, all four; virtual)* | Locates via `MatchRow`, **calls `IgnoreQueryFilters()` before the predicate** (A28), applies `ApplyIncludes` — by default none — then `AsNoTracking`, and returns a snapshot or `null`. **Does not apply `ApplyReadFilter`**, so a soft-deleted row is still returned. The soft root overrides it to normalize retrieved timestamps (A13) |
| `UpdateCore` *(protected virtual, hard types)* | Locates via `MatchRow`, with **`IgnoreQueryFilters()` on the locating fetch** (A28); `1` when found, `0` when absent. Writes every mapped scalar |
| `UpdateCore` *(protected override, soft types)* | Stamps `UpdatedDate`, preserves the stored `CreatedDate` and `DeletedDate` — by the A30 mechanism, since `SetValues` would otherwise copy all three off `item`. Reached through a `RootNonIdDao<TEntity>`-typed reference by virtual dispatch, so a base-typed caller cannot get the hard body (A2). **Not sealed** — A21 |
| `Get` / `Update` *(public, `BaseNonIdDao`)* | `GetCore` and `UpdateCore`, published |
| `Get` / `Update` *(public, `BaseSoftNonIdDao`)* | The soft overrides of `GetCore` and `UpdateCore`, published |
| `GetAll` / `GetPaged` / `GetCount` | As the keyed families — filtered by `ApplyReadFilter`, then `ApplyIncludes` (`GetAll` and `GetPaged` only), then ordered by `ApplyStableOrder`, which throws until overridden (A15). The composition order is fixed (A20) |
| `MatchRow` returning a predicate that matches several rows | The same rule as a duplicate key: `GetCore` uses `SingleOrDefault` and throws `InvalidOperationException` from LINQ. An under-specified `MatchRow` is the keyless equivalent of a non-unique key |
| Soft types | The same soft-delete deltas as [`BaseSoftDao`](#basesoftdaotentity-tkey--the-soft-delete-deltas) — including timestamp normalization on retrieval (A13) — with `MatchRow` doing the locating |

### `ICompanyResourceDao` — the shape this exists to serve

The Example's join-table DAO declares `Insert`, `Delete` and `GetAll` and inherits `IBaseDao<T>` **not at
all**. Under this design it is:

```csharp
public class CompanyResourceDao : RootNonIdDao<CompanyResource>, ICompanyResourceDao
{
	public CompanyResourceDao(DbContext context) : base(context) { }

	protected override Expression<Func<CompanyResource, bool>> MatchRow(CompanyResource item)
		=> x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId;

	protected override IOrderedQueryable<CompanyResource> ApplyStableOrder(IQueryable<CompanyResource> query)
		=> query.OrderBy(x => x.CompanyId).ThenBy(x => x.ResourceId);

	// Rule 3: inserting a pair that is already stored is a no-op.
	public override void Insert(CompanyResource item)
	{
		if (item is null) throw new ArgumentNullException(nameof(item));
		if (Dataset.AsNoTracking().Any(MatchRow(item))) return;
		base.Insert(item);
	}
}
```

`ICompanyResourceDao` declares no `GetPaged` and no `GetCount`, so neither is part of that DAO's published
contract — but both are present and correct on the base, so a later decision to publish them costs one line
in the interface. **That is what "conventional methods, not interface inheritance" buys.**

**It does not, on its own, buy dispatcher reach.** `dal.GetAll<CompanyResource>()` resolves a public
`GetAll(CompanyResource)` **on `ExampleDataAccess`**, so the Data Access Layer must forward:

```csharp
public class ExampleDataAccess : BaseEFDataAccess<ExampleContext>, IExampleDataAccess
{
	private readonly ICompanyResourceDao _companyResourceDao;

	// One forwarding member per capability the dispatcher must reach. Without these,
	// dal.GetAll<CompanyResource>() throws DataAccessConventionException however complete the DAO is.
	public void Insert(CompanyResource item)
	{
		ThrowIfDisposed();
		_companyResourceDao.Insert(item);
	}

	public int Delete(CompanyResource item)
	{
		ThrowIfDisposed();
		return _companyResourceDao.Delete(item);
	}

	public IList<CompanyResource> GetAll(CompanyResource? item)
	{
		ThrowIfDisposed();
		return _companyResourceDao.GetAll(item);
	}

	// No Get(CompanyResource) is declared, and none can usefully be:
	// Get<CompanyResource>(id) throws DataAccessConventionException by design.
}
```

And `ICompanyResourceDao` **rule 8** is binding on the test suite: `Get<CompanyResource>(object id)` on the
dispatcher always throws `DataAccessConventionException` and can never be made to work — the entity exposes
no identifier property *and* neither the DAO nor the DAL declares a `Get`. **Which of the two reasons is
reported is unspecified, so a test must assert on the exception type only**, never on its message.

---

## The Key Equality Predicate

The single place where S4 — no `struct` constraint — could fail, and the escape hatch
[D3](purpose-and-scope.md#owner-decisions--2026-08-15) names for reopening "no compatibility wrappers".
`Contract Reviewer` should read this section hardest.

### What must be true

1. It must translate to SQL on **both certified providers**, SQLite and SQL Server.
2. It must work for `int`, `long`, `Guid`, `string`, and nullable value types such as `int?`.
3. It must be **parameterized**, not embedded as a literal, so the provider's plan cache is not polluted by
   one entry per distinct key value.
4. It must not depend on the entity exposing an operator, since `TKey` is unconstrained and `==` is not
   available on an open type parameter.

### What does not work, and why it is recorded rather than left to be rediscovered

| Candidate | Verdict |
|---|---|
| `x.Id == key` written directly in C# | **Does not compile.** `==` is unavailable on an unconstrained `TKey` |
| `EqualityComparer<TKey>.Default.Equals(x.Id, key)` | Compiles; **EF Core cannot translate it**. Named explicitly in [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) as the thing not to do |
| `x.Id.Equals(key)` | Translates today for value keys and is the form proven in 2.2.x `RootDao.Update`. **But it is an instance call on `x.Id`**, which is a null-reference hazard the moment `TKey` is `string` and a stored row has a null key — the exact reach S4 opens up |
| `Dataset.Find(key)` | Would work, but **`Find` returns a tracked entity** and consults the change tracker before the store. Both break [Snapshot and Tracking](#snapshot-and-tracking) |

### The design

**Build the predicate as an expression tree** so `Expression.Equal` resolves the correct comparison for the
concrete `TKey` at runtime — the string `==` operator, lifted equality for `int?`, primitive equality for
`int`, `long` and `Guid` — each of which EF Core translates.

```csharp
/// <summary>
/// The predicate matching the row whose identifier equals <paramref name="key"/>.
/// </summary>
/// <remarks>
/// Equivalent to <c>x =&gt; x.Id == key</c> written against the concrete key type, built as an expression
/// tree because <c>==</c> is not available on an unconstrained type parameter. The key value is carried in
/// a closure so the provider parameterizes it rather than embedding a literal.
/// Override only if a provider mistranslates the default for your key type.
/// </remarks>
protected virtual Expression<Func<TEntity, bool>> KeyEquals(TKey? key);
```

Sketch — the shape, not the final code:

```csharp
var parameter  = Expression.Parameter(typeof(TEntity), "x");
var idAccess   = Expression.Property(parameter, ResolvedIdProperty);          // see A8
var keyCarrier = Expression.Property(Expression.Constant(new Box(key)), "Value"); // parameterized
return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(idAccess, keyCarrier), parameter);
```

Three properties of that sketch are **binding**, not stylistic:

- **The property access is on `TEntity` directly**, never on a cast to `IBaseIdEntity<TKey>`. EF Core cannot
  translate a member access through an interface conversion. This is also why A8 requires the identifier
  property to be **public and declared on the entity** — an explicit interface implementation compiles to a
  non-public, interface-qualified property and is unreachable.
- **The key travels in a closure carrier, not `Expression.Constant(key)`.** A bare constant is emitted as a
  SQL literal, giving a distinct query plan per key value.
- **`Expression.Equal` does the operator resolution**, which is what makes one code path serve every key
  type.

### String keys and collation — OD-2

**Key equality for a `string` identifier is the storage engine's collation. This library neither imposes nor
promises one.** A consumer needing ordinal matching configures it on the column in their model; a consumer
who configures nothing gets whatever their database does by default, which is the same answer every other
query against that column already gives them.

The predicate this library builds is `x.Id == key`. In C# that is ordinal comparison; translated to SQL it
is `WHERE Id = @p`, and **the server decides what `=` means**. The two certified legs disagree, and the
disagreement is real rather than theoretical:

| Leg | Default collation | `"acme"` vs `"ACME"` — **collation** | `"acme"` vs `"acme "` — **padding**, not collation |
|---|---|---|---|
| **SQL Server** | `SQL_Latin1_General_CP1_CI_AS` | **Equal** — case-insensitive | **Equal** — `=` ignores trailing blanks |
| **SQLite** | `BINARY` | **Not equal** — byte-exact | **Not equal** — byte-exact |

**The two columns are two different mechanisms, and the second one is not collation at all.** SQL Server
follows ANSI/ISO padding semantics on `=`: trailing blanks are ignored when comparing character data. That is
a property of the comparison, not of the collation the column carries, so a consumer who switches to a
case-**sensitive** collation changes the third column and should not assume they have changed the fourth. The
outcome in the table is right either way; the attribution matters because a test that explains the trailing
space as collation will be written to change the wrong knob. Whether a **binary** collation additionally
changes the padding behavior is a box question, not a design one — see
[Implementer-Only Questions](#implementer-only-questions).

So `dao.Get(new Country { Id = "US" })` can find a row stored as `"us"` on SQL Server and not on SQLite.
**That is stated as a term of the contract, not hidden as an implementation detail**, because a consumer
migrating between providers needs to know it is the schema's answer they are moving, not the library's.

**Forcing a collation in the predicate is rejected.** The obvious "fix" is to emit
`EF.Functions.Collate(x.Id, "...")`, or to lower-case both sides, so every provider agrees. Three reasons it
does not happen here:

1. **It is provider-specific SQL in a package whose stated purpose is provider neutrality.** Collation names
   are not portable — `Latin1_General_BIN2` is SQL Server's, `BINARY` and `NOCASE` are SQLite's, `C` is
   PostgreSQL's — so "neutral" would be implemented by naming three providers in the library. That is
   [D2](purpose-and-scope.md#owner-decisions--2026-08-15) inverted.
2. **It defeats index seeks.** A collation or a function applied to the column makes the predicate
   non-sargable on every relational engine, so the primary-key lookup this library exists to make trivial
   becomes a scan. A `Get` by key that scans the table is a performance defect shipped to every consumer to
   satisfy a preference none of them expressed.
3. **It overrides a decision the consumer already made.** Collation is declared in the schema. A consumer
   who chose a case-insensitive column chose case-insensitive matching, and a library that silently
   re-decides it makes the Data Access Layer disagree with every report, index and constraint over the same
   column.

**A consumer who wants provider-independent string-key matching has two supported routes**, both in their
own code: **declare a collation on the column** in `OnModelCreating` with `.UseCollation("...")` — which is
also available model-wide as `modelBuilder.UseCollation("...")` — or normalize the value before it reaches
the key, storing and querying a canonical form. This library neither requires nor obstructs either.

**`HasColumnType(...)` is not one of them, and an earlier revision listed it.** It sets the *store type* —
`varchar(50)`, `nvarchar(max)` — and sets no collation. The two are configured separately, and a consumer
who follows the old advice changes the column's type and none of its comparison behavior.

**Nothing above applies to non-`string` keys.** `int`, `long`, `Guid` and their nullable forms have one
equality on every relational provider.

### Null key semantics — A4

**A null key never matches any row, and the answer is produced without issuing a query.**

| Member | Null key result |
|---|---|
| `Get` | `null` |
| `Update` | `0` |
| `Delete` | `0` |

The short-circuit happens **before** the predicate is built. This is a decision, not provider behavior:
`WHERE x = NULL` is never true in SQL, EF Core's null-semantics rewriting differs by provider and by whether
the value is known at translation time, and "the DAO's answer depends on which provider you configured" is
the exact failure the family exists to prevent. Answering in the DAO makes SQLite and SQL Server agree by
construction.

**`default(TKey)` is not covered by this rule** (OD-3). `0`, `Guid.Empty` and `""` are ordinary key values:
the predicate is built, the query is issued, and the call misses normally if no such row exists. See
[`default(TKey)` is an ordinary key value](#defaulttkey-is-an-ordinary-key-value--od-3).

Note the layering: `IBaseDataAccess.Get<T>(object id)` throws `ArgumentException` for a `null` id where the
identifier property is a **non-nullable value type**, and that is the parent's dispatcher doing caller-error
validation before it ever reaches a DAO. The rule here governs the case the dispatcher lets through — a
`string` or `int?` key that is legitimately null.

### Identifier resolution — A8

Resolved **once per closed generic type** and **validated in the DAO constructor**, so a mis-wired entity
fails when the Data Access Layer is constructed rather than on first use.

1. Resolve a public instance property named **`{TypeName}Id`** — `CompanyId` for `Company` — by name only.
2. Failing that, resolve a public instance property named **`Id`**, again by name only.
3. Failing both, throw **`DataAccessConventionException`**, naming the entity type and the key type.
4. Validate that the resolved property is public, readable, has a set accessor of any visibility, and is
	declared as `TKey`. A mismatched or get-only property throws `DataAccessConventionException` during DAO
	construction. This mirrors the parent dispatcher's `PropertyInfo.CanWrite` check: a private setter is valid
	and reflection can invoke it, while a property with no setter cannot receive the selector identifier.

This is the same order `BaseDataAccess.Get<T>(object)` uses. **Diverging would be a defect nobody could
see:** `dal.Get<Company>(5)` and `companyDao.Get(item)` would silently address different columns on an entity
carrying both properties. `DataAccessConventionException` is the right type because this is a *wiring* error
— the family reserves `ArgumentException` for caller error.

---

## Stable Ordering

The `ProphetsWay.Example` ordering rule, binding on every retrieval in this library: order is **unspecified
but stable** while the data is unchanged, so successive `GetPaged` windows partition a full pass with no
overlap and no omission, and `GetAll` and `GetPaged` order **identically**.

```csharp
protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query); // keyed: defaults to the key
protected virtual IOrderedQueryable<TEntity> ApplyStableOrder(IQueryable<TEntity> query); // keyless: default throws
```

- **Keyed default:** `query.OrderBy(KeySelector)`, where `KeySelector` is `x => x.Id` built by the same
  expression machinery as `KeyEquals` and against the same resolved property.
- **Keyed total-order precondition (A16):** the resolved identifier must be unique and non-null. A model whose
	key permits nulls or duplicates must override `ApplyStableOrder` and add a deterministic tie-breaker.
- **Keyless default (A15):** throws `NotSupportedException`. Write-only keyless DAOs therefore need no
	ordering stub, while any DAO publishing `GetAll`, `GetPaged` or `GetCount` must override the hook.
- **`GetAll` applies it too**, not only `GetPaged`. 2.2.x ordered inside `GetPaged` alone, so a `GetAll`
  page-equivalence test passed by luck.
- **The return type is `IOrderedQueryable<TEntity>`** so the compiler rejects an unordered query. It cannot
  reject a *non-total* order — ordering by a non-unique column leaves ties broken arbitrarily, which the
  provider is free to resolve differently between two executions of the same query.

**Why this is stated so insistently:** SQL Server guarantees no order without an explicit `ORDER BY`, and the
plan it picks for an unordered scan **changes as a table grows**. A DAL that omits it passes every test today
and starts failing intermittently at some future row count, with nothing to point at. An in-memory store
satisfies the rule incidentally, through dictionary insertion order — which is precisely why SQLite and SQL
Server, not `InMemory`, are the certification legs.

---

## Navigation Loading

**A cross-cutting rule at the same weight as [Stable Ordering](#stable-ordering).** It was absent from
Revision 3 entirely, and its absence was the single largest defect in that revision.

### The rule

```csharp
/// <summary>
/// Declares which navigation properties a read materializes.
/// </summary>
/// <param name="query">
/// The row-restricted query for the calling path — <c>ApplyReadFilter</c>'s output on <c>GetAll</c> and
/// <c>GetPaged</c>, the <c>MatchRow</c>-matched query on <c>Get</c> and <c>GetCore</c> (A23). Never null.
/// </param>
/// <returns>
/// The query with whatever <c>Include</c> / <c>ThenInclude</c> calls this Data Access Object needs.
/// <b>The default returns <paramref name="query"/> unchanged.</b>
/// </returns>
/// <remarks>
/// The base members load <b>no</b> navigation property. A Data Access Object whose own contract promises a
/// populated graph must declare it here; one that promises scalars only overrides nothing. Depth is this
/// Data Access Object's decision, not the library's. On <c>GetAll</c> and <c>GetPaged</c> it is composed
/// after <c>ApplyReadFilter</c> and before <c>ApplyStableOrder</c> (A20, A23); on <c>Get</c>/<c>GetCore</c>
/// it is composed after the <c>MatchRow</c> predicate; it is <b>not</b> applied by <c>GetCount</c>.
/// </remarks>
protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query);
```

Present on **every** base in the library, keyed and keyless alike, with the same signature and the same
identity default.

| Statement | Binding |
|---|---|
| **Default is identity** | The base members load nothing. Every navigation property on a returned instance is `null` unless this DAO says otherwise (OD-1) |
| **Where it composes** | `Dataset` → `ApplyReadFilter` → **`ApplyIncludes`** → `ApplyStableOrder` → `Skip`/`Take` on the trio; `Dataset` → `IgnoreQueryFilters` → `Where(MatchRow)` → **`ApplyIncludes`** on `Get`/`GetCore` (A20, A23, A28). Fixed, not overridable |
| **Who applies it** | `Get`, `GetCore`, `GetAll`, `GetPaged`. **Not `GetCount`** — a count materializes no entity — and **not the `Update`/`Delete` locating fetches**, which load no navigation and hand back a row whose every navigation is `null` ([OD-8](#owner-decisions-taken-during-revision-8)). **A model-level `AutoInclude` is not bound by this row**: it is applied at query compilation and reaches those fetches too (H7) |
| **Depth** | The Data Access Object's decision. The library imposes no limit and offers no depth policy |
| **Interaction with `AsNoTracking()`** | It does **not** suppress loading — an included navigation is materialized either way, and materialized **fresh**, which is what makes an included graph a deep snapshot. It **does** suppress identity resolution, and the design depends on that. **See the trap below** |
| **Interaction with `AsSplitQuery()`** | None, in both directions. The library calls neither `AsSplitQuery()` nor `AsSingleQuery()` (A29); an override that calls one is honored |
| **Override receives** | The **row-restricted query for its path** (A23) — `ApplyReadFilter`'s output on `GetAll`/`GetPaged`, so a soft family's override sees the not-deleted set; the `MatchRow`-matched query on `Get`/`GetCore` |
| **Including a soft entity** | **Bypasses that entity's `ApplyReadFilter`.** The including Data Access Object materializes the navigation itself, so a *soft-deleted* related row arrives **populated** — see below |

### Why the default is opt-in and not eager

**The owner's decision** (OD-1): developers know when they want a graph, and it will definitely not be
always. Three supporting facts, recorded so the decision is not reopened casually:

- **Eager-by-default is unbounded.** "Include every navigation property" over an arbitrary consumer model is
  a cartesian explosion waiting for the first entity with two collection navigations, and the library has no
  way to know which of a consumer's relationships are cheap.
- **A `Get` by key that drags a graph is a performance defect shipped to everyone**, and the consumer who
  wanted scalars has no way to opt out of a library default short of writing a custom method.
- **The Data Access Object is where the knowledge lives.** `IUserDao`'s contract is what says whether a
  `User` comes back with its `Company`; the library cannot know that and should not guess.

### Model-level `AutoInclude` is an equally valid path

**Stated explicitly so nobody reads the hook as the only route.** A consumer may declare the same intent in
their `OnModelCreating` instead:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<User>().Navigation(u => u.Company).AutoInclude();
	modelBuilder.Entity<User>().Navigation(u => u.Job).AutoInclude();
	modelBuilder.Entity<User>().Navigation(u => u.Department).AutoInclude();
}
```

| Fact | Consequence |
|---|---|
| `AutoInclude` is applied at **query compilation** | It reaches every query against that entity, including the ones this library builds, with no override anywhere in the Data Access Object |
| It is **unaffected by `AsNoTracking()`** | The mandated `AsNoTracking()` on every read does not suppress it |
| **The two compose** | `Include` is additive. A DAO that overrides `ApplyIncludes` *and* a model that declares `AutoInclude` produce the union, not a conflict |
| **`IgnoreAutoIncludes()` is the per-query escape** | A custom DAO method that wants the scalars only calls it on its own query. This library never calls it |
| **It reaches the `Update` and `Delete` locating fetches too** | Those are queries against the entity, and this library builds them (A22, and `Delete`'s implementation note) — so the row above admits no exception for them. For a consumer who declares `AutoInclude`, **a write's locating fetch materializes and tracks the whole auto-included graph**, which the `finally` then detaches (A26, OD-7). Stated plainly because the consequence is invisible from here: it **silently enlarges what a write tracks and detaches, per consumer, on a model this library cannot see**. It is also the *only* route by which a fetched row arrives carrying a graph, which is why [OD-8](#owner-decisions-taken-during-revision-8)'s replacement obligation is written against `AutoInclude` rather than against the plain fetch |

**Neither path is preferred by this document.** They differ in where the decision is written — the hook puts
it in the Data Access Object next to the contract it satisfies, `AutoInclude` puts it in the model next to
the relationship it describes — and both are supported for the whole life of 3.x. What is *not* supported is
declaring the same navigation in both places and then being surprised that changing one had no effect;
`Include` being additive means the union wins, and the union is what you get.

### The `AsNoTrackingWithIdentityResolution()` trap

**`AsNoTracking()` is load-bearing for a second reason, and "it only governs change tracking" is the wrong
way to say it.** It also suppresses **identity resolution**: within one query, a row reached twice
materializes twice, as two independent instances. That suppression is what
[Snapshot and Tracking](#snapshot-and-tracking) means by *"Plain `AsNoTracking()`, **not**
`AsNoTrackingWithIdentityResolution()`"*, and `IExampleDataAccess`'s SNAPSHOT RULE demands it in terms:
*"a retrieval materializes fresh objects, it does not hand out a shared identity map."*

**Where it bites is within a single query**, because EF Core's identity resolution is scoped to one query
execution and discarded afterwards. Two shapes reach one row twice in one query:

- **`GetAll` over two rows naming a third.** Two `User`s of one `Company`, one query, one `Include`.
- **Two include paths meeting.** `Get(Transaction)` with the `ApplyIncludes` override below reaches
  `Company` through `t.Company` **and** through `t.User.Company`;
  `SnapshotDeepCopyTests.Setup_CreateTransaction_TestInsertDoesNotAdoptItsSecondLevel` constructs exactly
  that, with both naming the same stored company.

Under plain `AsNoTracking()` those produce independent instances and the rule holds. Under
`AsNoTrackingWithIdentityResolution()` they produce one shared instance and it does not — **and that is the
single change most likely to be made by an implementer acting in good faith**, because duplicate instances
under a reference include look like a bug worth fixing. It is not a bug; it is the contract.

**One correction to the review that raised this, so the next reader does not inherit it.**
`Setup_RetrieveTheSameUserTwice_TestTheNavigationInstancesAreIndependent` was cited as passing "only because
there is no identity map." Opening
`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` shows it calls `da.Get(...)` **twice** — two queries, two
identity maps — so it passes under either setting, as does
`Setup_RetrieveTwoUsersNamingOneCompany_TestTheirCompaniesAreIndependent`, which also uses two `Get` calls.
The conclusion is unaffected and the row above is still wrong to say "None"; what follows is that **no
existing Example test would catch the substitution**, so the guard has to be written here. See the
single-query obligation in [Navigation loading](#navigation-loading--od-1-a18).

### Including a soft-delete entity bypasses its `ApplyReadFilter` — and that is correct

`UserDao.ApplyIncludes` includes `User.Department`; `Department` is a soft-delete entity; `DepartmentDao` is
not consulted, because `UserDao`'s query is what materializes the navigation. **A soft-deleted department
therefore arrives populated on `User.Department`.** A reader will assume the opposite, which is why it is
stated: `User.cs` says of that property *"Because a department is soft-deleted rather than removed, this
reference never dangles"* — a filtered include is precisely what would make it dangle.

**It bypasses `NormalizeRetrievedTimestamp` as well — and *that* half nobody wants** (G12). The two hooks fail
the same way for the same reason, and the document previously named only one of them.
`NormalizeRetrievedTimestamp` is declared on `BaseSoftDao` and `RootSoftNonIdDao` and applied by **those**
Data Access Objects' own reads (A13). **`UserDao` is a *hard* Data Access Object and has no such hook**, so a
`Department` materialized through `UserDao.ApplyIncludes` carries its three timestamps exactly as the provider
handed them back — `DateTimeKind.Unspecified` on both certified providers, since neither stores `Kind` — while
`IDepartmentDao` rule 18 requires `Kind == Utc` on a retrieved instance. **The same stored row satisfies rule
18 when read through `DepartmentDao` and fails it when read through `User.Department`.**

**The earlier claim that normalization reaches *"every timestamp on every entity a read materializes"* is
corrected**, here and in [Timestamp Policy](#timestamp-policy) and A13: it reaches every timestamp on every
soft entity **that Data Access Object's own reads** materialize. The mechanism cannot do more. An including
DAO owns the query and has no access to the included type's hooks — neither its filter nor its normalizer —
and giving it one would mean a hard Data Access Object knowing which of its included types are soft entities,
knowledge it does not have and that this library will not synthesize.

**The two halves differ only in verdict, and the difference is worth keeping straight.** The filter bypass is
**wanted**: `User.cs` promises the reference never dangles, and filtering the include is what would break that
promise. The timestamp bypass is **wanted by nobody**; it is a limitation, recorded rather than fixed. A
consumer who needs rule 18 to hold through an include has three ordinary routes and no library support:
normalize inside the including DAO's own custom method, map the column as `DateTimeOffset`, or read the
department through `DepartmentDao`. It is pinned by an obligation beside N10's, scoped `Characterization`
because the contract asks for the opposite of what the mechanism delivers.

### Split queries — A29

**The library calls neither `AsSplitQuery()` nor `AsSingleQuery()`, ever.** Whether an include set is cheaper
as one join or as several round trips depends on the consumer's model and data, which this library cannot
see. A Data Access Object that wants splitting calls it inside its own `ApplyIncludes` override; a consumer
who wants it everywhere sets `UseQuerySplittingBehavior` on the context options. Both compose with this
library and neither is preferred here.

This is not exercised by `ProphetsWay.Example` — none of its seven entities has a collection navigation — and
it is recorded because the first consumer whose `ApplyIncludes` override carries two collection includes
meets EF Core's `MultipleCollectionIncludeWarning` and needs to know whose decision the fix is. It is theirs.

### What this means for a Data Access Layer that promises a graph

**A Data Access Object whose own contract promises a populated navigation property must declare the includes
that deliver it.** The library will not do it, and a contract that says "a snapshot is deep" is not satisfied
by a graph that was never loaded — a `null` navigation is not a deep copy of anything.

**This is now an obligation on `ProphetsWay.Example.DataAccess.EF`**, and it is the concrete reason this
section exists. `IExampleDataAccess`'s SNAPSHOT RULE names five navigation properties —
`User.Company`, `User.Job`, `User.Department`, `Transaction.User`, `Transaction.Company` — and
`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` asserts on `stored.User.Company.Name`, **two levels
down**, through a second Data Access Layer instance. Under Revision 3, where every read was `AsNoTracking()`
and nothing anywhere issued an `Include`, those assertions would have thrown `NullReferenceException` on both
certified providers. The EF example must therefore carry:

```csharp
// IUserDao is IBaseDao<User> plus one custom member, so the DAO must declare that member;
// the base supplies nothing for it. Deriving from BasePagedDao rather than BaseDao publishes
// more than IUserDao advertises, which is the flat surface (S2) working as intended.
public class UserDao : BasePagedDao<User, int>, IUserDao
{
	public UserDao(DbContext context) : base(context) { }

	public void CustomUserFunctionality(User user) { /* the consumer's own */ }

	protected override IQueryable<User> ApplyIncludes(IQueryable<User> query)
		=> query
			.Include(u => u.Company)
			.Include(u => u.Job)
			.Include(u => u.Department);
}

// BasePagedDao, not BaseDao: ITransactionDao is IBasePagedDao<Transaction>, and the migration
// table maps a paged Data Access Object to BasePagedDao<T, TKey>.
public class TransactionDao : BasePagedDao<Transaction, long>, ITransactionDao
{
	public TransactionDao(DbContext context) : base(context) { }

	// Two levels, because Transaction is the deepest graph in the Example and
	// SnapshotDeepCopyTests reads stored.User.Company.Name.
	protected override IQueryable<Transaction> ApplyIncludes(IQueryable<Transaction> query)
		=> query
			.Include(t => t.Company)
			.Include(t => t.User).ThenInclude(u => u.Company)
			.Include(t => t.User).ThenInclude(u => u.Job)
			.Include(t => t.User).ThenInclude(u => u.Department);
}
```

The five remaining Example DAOs — `Company`, `Job`, `Resource`, `Department`, `CompanyResource` — carry
scalars only and override nothing. **That asymmetry is the point of an opt-in hook**: two DAOs pay for the
graph, five do not.

`ProphetsWay.Example.DataAccess.NoDB` satisfies the same rule by deep-copying on read, which is why the
in-memory implementation needs no equivalent of this hook. Two implementations, one contract, different
mechanisms — which is the whole argument the Example repository makes.

### What remains out of scope

**Ad-hoc filtered includes** — `Include(u => u.Orders.Where(o => o.Total > 100))`, specification objects,
runtime include-path builders, projection DSLs. Those are the "1%" the purpose sentence leaves to consumer
Data Access Object methods, and they are unchanged from Revision 3's position. What changed is that
*declarative, per-DAO include strategy* is now **in** scope and has a hook; only the ad-hoc, per-call,
filtered variety is out.

---

## Global Query Filters

EF Core's `HasQueryFilter` declares a model-level predicate the provider appends to **every** query against
an entity type — typically the soft-delete shape
`modelBuilder.Entity<Department>().HasQueryFilter(d => d.DeletedDate == null)`. It is a mechanism this
library does not own and cannot prevent a consumer from declaring, and Revision 4 mentioned it **zero**
times.

### The rule — OD-5, A28

| Path | Global query filters |
|---|---|
| `Get`, `GetCore`, and the tracked locating fetches inside `Update`, `UpdateCore` and `Delete` — every path that finds one row through `MatchRow` | **Defended.** The library calls `IgnoreQueryFilters()` on the query before the predicate. A declared filter cannot hide a row from any of them |
| `GetAll`, `GetPaged`, `GetCount` — the retrieval trio | **Allowed to compose.** The library never calls `IgnoreQueryFilters()` here. A declared filter narrows the set on top of `ApplyReadFilter`, identically for all three |

The owner's reasoning, in his words: *the intention is for `Get` to work regardless of soft-delete status —
if the record is there, `Get` returns it.*

**`IgnoreQueryFilters()` is a no-op where no filter is declared**, so the call is unconditional and a
consumer who declares none pays nothing for it.

**EF Core 10's named filters do not change this rule** (A31). EF Core 10 allows an entity to carry more than
one named filter and allows `IgnoreQueryFilters([...])` to lift a named subset, which raises the obvious
question of whether A28 should lift only the soft-delete one. **It should not, and the granularity is
deliberately not used.** A28's defense is not "suppress the soft-delete predicate" — it is *"a `MatchRow`
lookup sees every stored row"*, which is what makes `Get`, `Update` and `Delete` agree about what exists
(A12). Lifting a subset would make that agreement depend on how a consumer happened to name and partition
their filters, and this library cannot know which of a consumer's filters is the row-hiding one. **The
parameterless overload is the contract; every declared filter is lifted on those paths, named or not.**

### How this sits with the existing filter split — read this before changing either

The split it has to be stated against is already there, and OD-5 does not disturb it:

| | `ApplyReadFilter` | Global query filter |
|---|---|---|
| `Get` / `GetCore` | **not applied** — `IDepartmentDao` rule 8 requires `Get` to return a soft-deleted row | **ignored** — OD-5 |
| `Update` / `Delete` locating fetch | **not applied** | **ignored** — A28 |
| `GetAll` / `GetPaged` / `GetCount` | **applied** — `DeletedDate == null` on the soft families | **honored**, composed as a conjunction |

**`IgnoreQueryFilters()` is what makes the first row hold against a mechanism the library does not own.**
Without it, a consumer's `HasQueryFilter` silently reinstates on `Get` exactly the exclusion `ApplyReadFilter`
was deliberately kept off it, and `IDepartmentDao` rule 8 fails with nothing in this library to point at.

**The write members force the same treatment, which is why A28 reaches further than OD-5's wording.**
`IDepartmentDao` rule 4 requires `Update` on a soft-deleted department to succeed and return `1`, and rule 6
requires `Delete` on an already-deleted one to return `0` — which it can only know by **seeing** the row. A
filter left in force on those locating fetches turns both into `0`-returning misses. Defending `Get` alone
would leave two of the three rules broken.

**The trio is left alone deliberately.** Those three already hide rows by contract, the filter composes
identically across all three so `GetCount == GetAll().Count` still holds, and ignoring it there would
*override* a restriction the consumer wrote on purpose — the same objection
[OD-2](#owner-decisions-taken-during-revision-4) makes to forcing a collation.

### It is a conflicting mechanism, not a sanctioned alternative

**The contrast with `AutoInclude` is deliberate and should not be smoothed over.** `AutoInclude` and
`ApplyIncludes` are *additive*: declare a navigation in either place and the union is what you get, and no
member of the Data Access Object behaves differently from any other. `HasQueryFilter` and `ApplyReadFilter`
are not additive — the same predicate is honored on the retrieval trio and defeated wherever a row is located
through `MatchRow`, by design — so
which rows a consumer's filter hides depends on which member they call. On a soft family it also states
`DeletedDate == null` twice, in two places, one of which `IDepartmentDao` rule 15 (*soft deletion is the only
exclusion rule*) does not know about.

**The sharpest instance is an include.** `IgnoreQueryFilters()` lifts filters on **included** navigation types
too, so with a filter declared on `Department`:

- `userDao.Get(user)` populates `User.Department` even when that department is soft-deleted — which is what
  `User.cs` promises when it says the reference *"never dangles."*
- `userDao.GetAll(null)` returns users whose `Department` is **`null`** for deleted departments — the
  reference dangling after all.

Two members of one Data Access Object disagreeing about the same navigation property is the cost of declaring
the rule twice. **`ApplyReadFilter` is the seam; a global filter is a second one the library has to defend
against.** A consumer who declares both owns the divergence.

**And the sharpest consequence is on a *hard* family's `Delete`.** A28 lifts filters on every `MatchRow`-located
path, and `Delete` is one of them — so **a consumer who declares `HasQueryFilter(c => c.IsActive)` on a
hard-delete entity will find that `Delete` hard-deletes an inactive row.** The filter hides that row from
`GetAll`, `GetPaged` and `GetCount`, and does not protect it from removal.

A reader will expect the opposite, which is why it is written down rather than left to be discovered: a
global filter *reads* like a visibility rule, and on the trio it is one. On the locating paths it is lifted by
design, for the reason A12 gives — a `Delete` that could not see a row `Get` returns would be the divergence
A28 exists to prevent, and "protect it from deletion" and "return `0` because it is not there" are
indistinguishable to a caller. **A consumer who wants a row protected from deletion expresses that in
`ApplyReadFilter` or in their own DAO method, not in a global query filter**, and the obligation on this is
extended from `Get` to `Delete` in [Test Obligations](#global-query-filters--od-5-a28).

---

## Writes and the Navigation Graph

The **write half** of the deep-snapshot rule. `IExampleDataAccess`'s SNAPSHOT RULE binds both directions —
*"an entity reached through a navigation property on an argument is likewise read rather than adopted"* — and
`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` exercises a populated graph across `Insert` and `Update`
three times. Revision 4 specified `Delete`'s mechanism and `Update`'s (A22), and left `Insert`'s unstated
altogether.

### `Insert` writes the root only — OD-4, A24

| Step | What happens |
|---|---|
| 1 | `item` null → `ArgumentNullException`, before anything is tracked |
| 2 | Walk the graph reachable from `item` through its navigation properties, **by reference identity**, so a node reached by two paths is visited once |
| 3 | Set every entity in that walk **other than `item`** to **`EntityState.Unchanged`, explicitly** — not by calling `Attach`, which infers a state from the key value. This happens **before** the root is added — order matters, see below |
| 4 | Mark `item` `Added` |
| 5 | `SaveChanges()`. EF Core's relationship fix-up writes the foreign keys onto the new row from the attached principals |
| 6 | The store-generated identifier is written onto `item`, and onto **nothing else** |
| 7 | `item` and the whole walked graph are **detached** — in a `finally`, so steps 5 and 6 failing does not skip it (A26, OD-7) |

**The worked case the owner confirmed.** After `Insert(user)` where `user.Company` names stored company `7`:
the new `User` row's foreign key is `7`, the association is preserved, `user.Id` carries the generated value,
and `user.Company` is the same object the caller passed in with the same `Id`. **The point of the decision is
to get the association without the duplicate insert.**

#### Why `Dataset.Add(item)` is wrong, not merely suboptimal

`DbContext.Add` tracks the given entity **and every reachable untracked entity** as `Added`. Against a graph
whose related entities are already stored, that means:

- **already-stored related rows are re-inserted** as duplicates, and
- **the caller's related instance has its key overwritten** with the duplicate's, which violates the snapshot
  rule's "the generated identifier is the only value that travels back" in the most confusing way available —
  onto an object the caller did not pass as the argument.

Attaching before adding is what prevents it: with the related entities already tracked as `Unchanged`, neither
`Add` nor the `DetectChanges` that precedes `SaveChanges` has an untracked reachable entity left to promote.

#### `Attach` is the second trap, and it fails on exactly the rows OD-3 protects

**`DbContext.Attach` / `DbSet.Attach` is not the safe alternative to `Add`.** It applies a heuristic of its
own: an entity whose key holds a **default value** is marked **`Added`**, not `Unchanged`. Against a related
entity legitimately stored under `0` or `Guid.Empty` — the rows [OD-3](#owner-decisions-taken-during-revision-4)
exists to keep legal — that reintroduces the exact duplicate-insert bug OD-4 forbids, and it does so on the
narrow slice of data least likely to appear in a developer's test fixture.

**So the state is set explicitly** — `entry.State = EntityState.Unchanged` — rather than inferred from any
API's idea of what a key value means. **This is why the walk in step 3 is specified by *state* and not by
method name**, and it is a constraint on the implementer rather than a choice among equivalents: see
[Implementer question 7](#implementer-only-questions).

**The Example suite currently passes this by luck, and the luck is identifiable.**
`Setup_CreateUserWithNavigation_TestInsertDoesNotAdoptItsNavigation` and
`Setup_CreateTransaction_TestInsertDoesNotAdoptItsSecondLevel` assert **by value**, and they re-read through
`reader.Get(new Company { Id = co.Id })` — evaluating `co.Id` *after* a naive `Add` would have overwritten it,
so the assertion follows the duplicate row, which carries the same name and passes. That stops the moment a
key is **client-generated** — `Resource` is `Guid`-keyed, and a second insert under the same primary key is a
constraint violation rather than a silent duplicate — or a column is unique.

#### Key values are not inspected, and that is forced by OD-3

**Every reachable entity is attached `Unchanged` whatever its key holds.** The library does not read
`default(TKey)` as "unsaved, insert this one," because [OD-3](#owner-decisions-taken-during-revision-4) makes
`0`, `Guid.Empty` and `""` **legal stored values**; a rule that treated them as unsaved would re-introduce the
duplicate insert for precisely the rows OD-3 protects.

So a related entity whose key names no stored row fails at `SaveChanges()` with **the provider's
referential-integrity exception, propagated unwrapped** — the same treatment [`Insert`](#inserttentity-item)
already states for a constraint violation. Inserting a genuinely new graph in one call is **not supported**:
insert the principals first, then the dependent. That is what the Example does
(`InsertUserWithNavigation` inserts the company, job and department before the user), and it is now a rule
rather than a coincidence.

#### Two distinct instances naming one row

The walk visits by reference identity, so **one object reached by two paths is attached once** — the shape
`SnapshotDeepCopyTests` builds when it assigns the same `Company` object to both `trans.Company` and
`trans.User.Company`. **Two *different* instances carrying the same key** are a different matter: EF Core
refuses to track both and throws `InvalidOperationException` — *"another instance with the same key is
already being tracked"* — which **propagates unwrapped**. The library neither de-duplicates by key nor picks a
winner; doing either would mean silently discarding one of two objects the caller believed in.

### `Update` writes scalars, and cannot repoint a relationship — A25

`Entry(stored).CurrentValues.SetValues(item)` copies **values, by property name, off `item`'s CLR type**. It
copies no navigation property, and it cannot reach a **shadow** foreign key, because there is no property of
that name on `item` to read.

**`Transaction` is the worked case.** It declares `User` and `Company` navigations and **no** `UserId` or
`CompanyId` scalar, so EF Core creates both foreign keys as shadow properties. **`Update(transaction)` can
therefore never repoint `Transaction.User`.** The stored row keeps the `UserId` it had, the call still returns
`1` because the row existed, and nothing anywhere reports that the change was dropped.

**An entity that declares its foreign key as an ordinary mapped scalar is unaffected**, and the worked case
for it is **purpose-built — it is not present in `ProphetsWay.Example`** ([OD-9](#owner-decisions-taken-during-revision-8)).
Declare it in the test model, as the collation obligations declare `Country`:

```csharp
// Assignment — purpose-built for this rule and for the obligation that pins it.
// Three properties, and each one is load-bearing.
public class Assignment : IBaseIdEntity<int>
{
	public int Id { get; set; }             // the identifier, and the only thing MatchRow locates by
	public int CompanyId { get; set; }      // the foreign key, declared explicitly as a mapped scalar
	public Company Company { get; set; }    // the navigation that scalar backs
	public string Note { get; set; }        // a third column: writable, and part of no key
}
```

`Update(assignment)` locates the row by `Id` **alone**, so both other columns are free to move. `SetValues`
copies `Note` like any other scalar, and it copies `CompanyId` like any other scalar too — **which repoints
`Company`**, because where the foreign key is a property of the entity, the foreign key *is* the relationship.
**The limitation is specific to navigation-only relationships**, and a consumer who wants `Update` to move an
association removes the limitation from their own model by declaring the foreign key, exactly as this entity
does. (`Company` is named here only because the surrounding prose already uses it; nothing about the rule
depends on which principal the navigation points at.)

**`CompanyResource` was the counter-example through Revision 7, and it could not carry the point.** Recorded
so it is not reached for again — it is the obvious candidate, and it fails three ways:

- **It declares no navigation property at all.** Opening
  `ProphetsWay.Example.DataAccess/Entities/CompanyResource.cs` shows `public int CompanyId` and
  `public Guid ResourceId` and nothing else. It could demonstrate that a *scalar* is writable; it could
  demonstrate nothing about repointing a *relationship*, which is the claim being made.
- **Both of its mapped scalars are in its own locating predicate.** `CompanyResourceDao.MatchRow` is
  `x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId`, and `Update` locates through
  `MatchRow(item)` — so changing either value makes the locating query search for the **new** value, find
  nothing, and return `0`. **There is no column an `Update` could write.** This is structural: it holds
  against a purpose-built `BaseNonIdDao<CompanyResource>` just as it holds against the real DAO.
- **Neither the interface nor the base publishes `Update` in the first place.** `ICompanyResourceDao` declares
  none, and `CompanyResourceDao` derives from `RootNonIdDao<CompanyResource>`, which publishes neither `Get`
  nor `Update` — `UpdateCore` is `protected`.

That is the shape [`ICompanyResourceDao`](#icompanyresourcedao--the-shape-this-exists-to-serve) exists to
serve, and it is a *keyless join* rather than a *dependent carrying a foreign key*. Those are different
subjects, and A25 needs the second one.

**It is declared, not fixed.** Two ways out were considered:

| Candidate | Verdict |
|---|---|
| Attach the incoming graph as `Modified` | **Rejected.** It writes related rows — the thing OD-4 forbids on `Insert`, and more damaging on `Update`, where the caller usually goes on holding the instance |
| Read the shadow foreign key off the related entity and set it via `Entry(stored).Property("UserId")` | **Rejected.** The library would have to guess the shadow property's name from a convention EF Core owns, and decide which of two candidate instances wins |

A Data Access Object that needs to repoint a navigation-only relationship **writes a custom method**.
`Context` and `Dataset` are `protected` for exactly this (S10), and it is the same "1%"
[`Restore`](#writing-a-restore) occupies.

### Detachment spans the whole reachable graph — A26, OD-7

> **"Every write detaches the instances it touched" means the argument, everything reachable from it, and the
> row any tracked fetch loaded.** Not the root alone. **And it happens in a `finally` — on success and on
> failure alike.** A fetched row brings a graph of its own only where the consumer's model declares
> `AutoInclude` on it; the clause that assumed otherwise is retracted
> ([OD-8](#owner-decisions-taken-during-revision-8)).

The looser reading does not survive the obligation it exists to serve. [Snapshot and
Tracking](#snapshot-and-tracking) requires that mutating an argument after a write returns cannot reach the
store *"including after an unrelated later `SaveChanges` triggered through a different DAO on the same
context"* — and every Data Access Object on a layer shares one context (S8). A graph left tracked means a
later `SaveChanges` from any other DAO persists edits made to it after the call returned, which is the exact
leak [Forced Behavior Change 5](#forced-behavior-changes) exists to close.

| Aspect | Contract |
|---|---|
| **When** | In a **`finally`**, after `SaveChanges()` returns **or throws**, on every write member — `Insert`, `Update`, `Delete`, and the keyless cores |
| **What** | The argument's whole reachable graph, plus **the row any tracked fetch loaded** — and, where the consumer's model declares `AutoInclude` on that entity, the auto-included graph the fetch materialized with it (H7). Reachability is the same walk `Insert` uses. **A fetched row has no graph of its own otherwise** — see below |
| **Inside a transaction** | Unchanged. `SaveChanges()` has already issued the SQL, so detaching the entities neither commits nor reverses anything; a later `TransactionRollBack()` removes the rows and, as [Transactions](#transactions) states, **leaves the generated identifier on the caller's instance** |
| **Failure** | The exception from `SaveChanges()` propagates **unwrapped**, and the detachment still runs (OD-7). Nothing the write touched is left tracked — including an entity that was tracked **before** the call and that the write's walk reached, which is **detached, not restored**. See [Writing a `Restore`](#writing-a-restore) for the shape that produces one |

#### The fetched-graph clause is retracted — OD-8

**Revision 5 wrote, and Revisions 6 and 7 carried, *"plus any fetched row's whole reachable graph."* That
clause is withdrawn.** It described something this design does not produce.

`Update` and `Delete` locate a row with a tracked fetch that applies **neither `ApplyReadFilter` nor
`ApplyIncludes`** — stated on [`Update`](#updatetentity-item)'s `Mechanism` row, and by the *Who applies it*
row in [Navigation Loading](#the-rule), which lists `Get`, `GetCore`, `GetAll` and `GetPaged` and nothing
else. `item` is **never tracked**, so there is no relationship fix-up partner to populate anything either.
**Every navigation on the fetched row is `null`.** There was no graph, and the obligation written against one
could not have been authored.

It is retracted rather than quietly corrected for the reason
[OD-7](#owner-decisions-taken-during-revision-6) was recorded the same way: a reader who met the earlier
wording should be able to see that its removal was a decision. **The behavior does not change** — the fetched
row was always detached and still is. What changes is the claim about what hangs off it.

**One route does populate a fetched graph.** A consumer's model-level `AutoInclude` is applied at query
compilation and reaches *every* query against that entity, this library's locating fetches included (H7, and
the `AutoInclude` table in
[Navigation Loading](#model-level-autoinclude-is-an-equally-valid-path)). Where one is declared, the fetch
materializes and tracks the auto-included graph and the `finally` detaches it — which is real, is invisible
from this library's side, and is what the replacement obligation is written against.

#### Why `finally`, and what it costs — OD-7

**Revision 5 said the opposite**, in terms: *"If `SaveChanges()` throws… the tracker is left as EF Core left
it. Detachment is a post-success step, not a `finally`."* **That is retracted.** The owner reversed it, and
the reasoning is a contract judgment rather than a mechanical one:

- **A failed write must not poison the Data Access Layer instance.** Leaving a failed `Insert`'s graph tracked
  as `Added` means the *next* successful `SaveChanges` on that context — through any DAO, since they share one
  (S8) — carries the failed row along with it. The caller sees a row appear from an operation that threw.
- **Forcing a rebuild is too harsh a contract.** Under the old rule, recovering from one unique-constraint
  violation meant disposing the DAL and reconstructing it with every DAO on it. That is a heavy price for a
  caller error the caller can fix in one line.

**The cost is stated plainly: detaching an `Added` entity after a failed `SaveChanges` discards the pending
insert, and that is the intent.** The write did not happen, and after the call nothing about it remains
tracked. A caller who wants the insert may **fix their argument and call `Insert` again on the same
instance** — no dispose, no rebuild, no leftover state from the attempt. That retry is the behavior OD-7 buys,
and it is the reason the discard is not a loss.

**It applies to the fetch-based writes too.** `Update` and `Delete` locate a row with a tracked fetch; if
`SaveChanges()` throws, that row and its graph are detached along with the argument's. A failed `Update`
leaves the context holding nothing, exactly as a successful one does.

### The `Update` cascade question — resolved by OD-6

**Revision 5 opened this as the one thing waiting on a decision. It is now closed, in favor of the design as
written: `Update` writes the root only, exactly as A22 and A25 specify. Nothing in this document changed to
accommodate it.**

The analysis is kept, because it is the reasoning that justifies the fix — and the fix lands in a different
repository.

#### What the collision was

`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` **was** annotated `[Trait("Scope", "Contract")]` at
**class level**, so every assertion in it bound any conforming Data Access Layer.
`Setup_UpdateNavigationInsideTransaction_TestRollBackRestoresIt` does this:

```csharp
var edit = da.Get(new User { Id = seed.User.Id });
edit.Company.Name = EditedName;          // an edit made through a navigation property
edit.Whatever  = EditedName;
da.Update(edit);

var uncommitted = da.Get(new User { Id = seed.User.Id });
// ...
uncommitted.Company.Name.ShouldBe(EditedName);   // the navigation edit is required to have landed
```

**`ProphetsWay.Example.DataAccess.NoDB` satisfies that by storing the user's copy of the company *inside the
user row*** — `UserDao.Update` writes `Copy(item)` into the `Users` table, and that copy is deep. The
`Companies` table is untouched, which is why the post-rollback assertion
`reader.Get(new Company { Id = seed.Company.Id }).Name.ShouldBe(companyName)` also holds — the same test
requires the company row to be unchanged and the user's *view* of the company to carry the edit, which is
only possible when the two are the same storage.

**No normalized relational store can reproduce that shape, and the reason is structural rather than a matter
of effort.** `User` has a foreign key to `Company`; the name lives in exactly one place; `Get(user)` reads it
back through an `Include`. There is no arrangement of `SetValues`, cascade or tracking under which a company
name is simultaneously edited (as read through the user) and unedited (as read directly). The assertion is
not hard to satisfy on a relational provider — it is **unsatisfiable**, and it is unsatisfiable for every
relational DAL, not only this one.

#### The decision — OD-6

**The assertion was a `ProphetsWay.Example` defect.** It encoded a `NoDB` denormalization detail — the user row
holding a deep copy of the company — as though it were a rule of the paradigm, and it was mis-scoped
`Contract` when what it actually pins is a **characterization** of the in-memory implementation.

The owner authorized the **retrait to `Characterization`, in the `ProphetsWay.Example` repository**, and
**that retrait has landed.** `SnapshotDeepCopyTests` no longer carries a class-level `Scope` trait — it
declares one per method, because xUnit accumulates traits rather than letting a method override a class — and
the assertion above is now a `Characterization` fact named
`ShouldReadANavigationPropertyEditBackInsideTheTransactionThatSubmittedIt`. A second retrait has since landed
in `UserDaoTests.cs` on the same reasoning.

| Option | Disposition |
|---|---|
| **`Update` writes the root only**, as A22 and A25 specify | **Chosen.** The design is coherent and unchanged; the conformance bar moves, because the bar was wrong |
| **`Update` cascades into the reachable graph**, writing related rows | **Rejected.** `da.Update(user)` would silently rewrite `Company`, `Job` and `Department` rows a caller never named, and it contradicts OD-4's shape for `Insert` — leaving the two write members with opposite policies toward the same graph |

**The dependency this section used to state is discharged.** A conforming EF Core implementation of this
design is no longer required to fail anything in the Example suite: the assertion that bound it is
`Characterization`, and **"the Example suite is green" is the right gate**, without qualification. Counted
directly from the standalone repository rather than taken from a report: **164 tests — Contract 139,
Characterization 5, Dispatcher 20 — green on `net10.0` and `net48`, 328 executions.** **This document changed
nothing in `ProphetsWay.Example` and proposed nothing there** — it recorded the decision; the fix landed in
that repository on its own terms.

**What was never in doubt:** the other three `SnapshotDeepCopyTests` write-side setups —
`Setup_CreateUserWithNavigation_TestInsertDoesNotAdoptItsNavigation`,
`Setup_InsertUserWithNavigation_TestUpdateDoesNotAdoptItsNavigation` and
`Setup_CreateTransaction_TestInsertDoesNotAdoptItsSecondLevel` — all assert about edits made **after** the
call returned, and OD-4 plus A26 satisfy every one of them. The conflict was confined to this single
assertion, and to the single word *cascade*.

**The [Writes with a populated navigation
graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) obligations are all authorable**, the
cascade-dependent one included — it is written against root-only behavior, which is now settled.

---

## Snapshot and Tracking

The `ProphetsWay.Example` 3.1 snapshot rule is binding: **reads return snapshots; writes read their argument
rather than adopting it.** EF Core's default change tracking violates both halves, so satisfying the rule is
an explicit design obligation, not a default.

| Rule | How the design meets it |
|---|---|
| A returned instance is not the store's object | Every read issues `.AsNoTracking()` **explicitly**, so behavior does not depend on the consumer's `QueryTrackingBehavior` |
| Two separately retrieved instances are independent | Plain `AsNoTracking()`, **not** `AsNoTrackingWithIdentityResolution()`. Identity resolution hands out a shared instance within a query, which the rule denies. **The substitution is the single most likely good-faith way to break this document** — see [The `AsNoTrackingWithIdentityResolution()` trap](#the-asnotrackingwithidentityresolution-trap) |
| A snapshot is **deep** | **Only the navigation properties `ApplyIncludes` declares are loaded at all** — the rest are `null`, which is vacuously deep. Those that *are* loaded are materialized fresh per query under `AsNoTracking()`, so the graph is a genuine deep copy rather than a shared one. **A Data Access Object whose contract promises a populated graph must override `ApplyIncludes`** — see [Navigation Loading](#navigation-loading). A custom DAO method that `Include`s must also be `AsNoTracking` |
| Mutating an argument after a write must not reach the store | Every write detaches **the whole reachable graph** — the argument, everything reachable from it, and any row a tracked fetch loaded — in a **`finally`**, on success and on failure alike (A26, OD-7). Without that, `Insert(x)` leaves `x` tracked and a later mutation plus any later `SaveChanges` — including one from a *different* DAO on the same shared context — silently persists it |
| An argument's navigation graph is read, not adopted | `Insert` attaches everything reachable as `Unchanged` and writes the root only (OD-4, A24); `Update` writes scalars off the root and cannot repoint a navigation-only relationship (A25). See [Writes and the Navigation Graph](#writes-and-the-navigation-graph) |
| A rollback must reverse an `Update`, not preserve it | Follows from the two rules above: the pre-update state stays in the store until `Update` runs |

**Writes fetch with `.AsTracking()` explicitly**, for the same reason reads specify `AsNoTracking()` — the
consumer may have configured the context either way, and the DAO's behavior must not depend on it.

**No `DbContext`-level configuration is required of the consumer.** 2.2.x's Example DAL configured
`QueryTrackingBehavior.NoTracking` on the options builder; the library now does it per query, so a consumer
who configures nothing still conforms.

---

## Forced Behavior Changes

Against the published **2.2.x** package. Every row is breaking by intent (S12), and every row rides the same
major that is already breaking for the reasons in
[feature-requests.md](feature-requests.md#release-eligibility--the-next-release).

| # | 2.2.x | 3.0.0 | Why it cannot stay |
|---|---|---|---|
| 1 | `Update` on an absent row **throws** (EF Core) or **silently inserts** (EF6 `AddOrUpdate`) | **Returns `0`** | `IBaseDao<T>.Update` documents "0 when the identifier matches no stored row." One branch violated it loudly, the other silently — and a package cannot ship two semantics under one method name |
| 2 | `GetPaged` ordered by key; `GetAll` unordered | **Both ordered, identically** | The Example ordering rule. An unordered `GetAll` cannot be page-equivalent |
| 3 | Soft classes hide base methods with `new` | **`virtual` / `override`** (A2) | `new` means a base-typed reference **hard-deletes a soft-delete entity**. Silent data loss |
| 4 | Reads honor whatever tracking the context was configured with | **`AsNoTracking()` per query** | The snapshot rule cannot be delegated to consumer configuration |
| 5 | Written instances stay tracked | **The whole reachable graph is detached in a `finally`** (A26, OD-7) | "Read rather than adopted." A shared context makes the leak reach across DAOs, a root-only detachment leaves the graph reachable, and a detachment skipped on failure leaves a failed write's rows to be carried by the next successful `SaveChanges` |
| 6 | `Dao.EnsureBeginTransaction()` / `EnsureTransactionCommit()` / `EnsureTransactionRollback()` | **Removed** (S9) | Two transaction authorities cannot both honor "one transaction per instance." `Ensure*` also silently no-ops when a transaction is already open, which is the opposite of the parent's throw-on-misuse rule |
| 7 | `public DbContext Context` / `public DbSet<T> Dataset` on every DAO | **`protected`** (S10) | A public `Dataset` lets business logic bypass the DAL entirely — it is a hole straight through the decoupling this library exists to provide |
| 8 | `BaseEFDataAccess<TContextType, TIdType>`, context built by `Activator.CreateInstance` | **`BaseEFDataAccess<TContext>`, context passed in** (S7) | `TIdType` was unused by anything the class did. Reflective construction cannot accommodate dependency injection or an options-configured context |
| 9 | No `Dispose` | **`Dispose` with explicit ownership** (S8) | `IBaseDataAccess` extends `IDisposable` as of 3.0.0 — the library does not compile against 3.1.0 without it |
| 10 | `UseUtcTime` constructor flag | **`GetCurrentTimestamp()` override** (S11) | A `bool` cannot express a test clock or a chosen zone |
| 11 | `BaseEFContext(string)` calls `UseSqlServer` | **Removed**; options-only | [D2](purpose-and-scope.md#owner-decisions--2026-08-15) |
| 12 | 18 key-typed classes in three namespaces | **6 generic families, root namespace** (S1–S3) | [D3](purpose-and-scope.md#owner-decisions--2026-08-15) |
| 13 | `BaseNonIdDao<T>` demands abstract `Get` and `Update`; `BaseSoftNonIdDao<T>` additionally demands `Get(T, bool)` | **One `MatchRow` hook**, plus `ApplyStableOrder` | Three abstract methods to express one idea — "how do I find this row" — and the `bool AsTracking` parameter leaked an EF implementation detail into the consumer's override |
| 14 | `BaseEFDataAccess` is a concrete class | **`abstract`**, `Dispose()` sealed (A7) | A DAL with no DAOs is not a usable object, and an overridable `Dispose()` lets a derived class break idempotency |

**Not changing, stated so it is not "fixed":** `Insert` still assigns the generated identifier back onto its
argument; `Get` still does not promise to return the instance passed in; exceptions from a derived DAL still
propagate **unwrapped**, with no `TargetInvocationException`.

---

## Two 2.2.0 Defects This Design Retires

Both were found in the shipped **2.2.0** source while Revision 4 was being written, by reading
[BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs) and
[RootNonIdDao.cs](../ProphetsWay.EFTools/RootNonIdDao.cs). Neither is a forward-compatibility gap — both are
**live defects in the published package**. They are recorded here, rather than in the table above, because
the table is a list of *behavior changes* and these are *fixes*.

**`Changelog Author` owns the framing.** Each belongs in the 3.0.0 entry under **Fixed**, not under
**Changed** or **Breaking**, and each should name the 2.2.x behavior plainly so a consumer can recognize it
in their own logs.

### 1 — `BaseEFDataAccess` leaks every `DbContext` it constructs

**The defect.** `BaseEFDataAccess<TContextType, TIdType>` builds its context with
`Activator.CreateInstance` in both constructors, holds it in `protected DbContext Context { get; }`, and
**declares no `Dispose` at all**. Opening [BaseEFDataAccess.cs](../ProphetsWay.EFTools/BaseEFDataAccess.cs)
confirms the whole class: **three methods, all transaction forwarders** — `TransactionCommit`,
`TransactionRollBack`, `TransactionStart` — plus **two constructors** and the `Context` property. (An earlier
revision said "three members," which undercounted by three.) Every Data Access
Layer instance therefore constructs a `DbContext`, which opens and pools a connection, and nothing ever
releases it. Under a per-request Data Access Layer this leaks a context per request until the finalizer or
the pool reclaims it.

**Where the design handles it.** [`ContextOwnership`](#the-dal-root--baseefdataaccesstcontext) (S8, A9) plus
the [six-step disposal sequence](#disposal) (A11). Step 5 disposes `Context` when `Ownership` is `Owned`, and
`Owned` is exactly the case this defect describes — a context the Data Access Layer created for itself.
`ContextOwnership.Borrowed` is the other half, which is what stops the fix from becoming a double-dispose in
a dependency-injection host.

**Confirmed: the design does not reproduce it.** Three properties make that a guarantee rather than a hope:

- `Dispose()` is a **sealed override** (A7), so a derived Data Access Layer cannot replace it with one that
  forgets.
- `Ownership` is a **required constructor argument with no default** (A9), so "which half applies" is never
  inferred.
- Step 5 runs even when steps 3 and 4 threw (A11), so **a throwing `DisposeCore()` cannot leave an owned
  context undisposed** — which is the shape a naive fix would have.

**One consequence worth stating for the CHANGELOG.** 2.2.x consumers do not call `Dispose` today, because
there is nothing to call. In 3.x `IBaseDataAccess` extends `IDisposable`, a container will dispose the Data
Access Layer at the end of its scope, and a consumer who was relying on the leak to keep a context alive past
its scope will see `ObjectDisposedException`. That is the fix working, not a regression.

### 2 — `RootNonIdDao.EnsureBeginTransaction` silently no-ops

**The defect.** `EnsureBeginTransaction` begins a transaction **only if `Context.Database.CurrentTransaction`
is `null`**. When one is already open it does nothing and leaves the private `_transaction` field `null` — and
`EnsureTransactionCommit` and `EnsureTransactionRollback` then both go through `_transaction?.`, so they
quietly do nothing too. The caller's `Ensure…Commit()` returns normally having committed nothing, and the
outer transaction it silently joined decides the outcome.

**Where the design handles it.** [S9](#stage-2-settled-decisions) removes the three `Ensure*` helpers
outright, and the DAL-level [transaction members](#transactions) replace them. **The 3.x design cannot
express this failure**, for three independent reasons:

1. **There is one transaction authority.** Data Access Objects have no transaction members at all, so there
   is no second place that could begin, skip, or discard one.
2. **Every misuse throws rather than no-ops.** `TransactionStart` on an instance that already has one open
   throws `InvalidOperationException`; `TransactionCommit` and `TransactionRollBack` with none open throw the
   same. There is no `?.` anywhere in the transaction path and no branch that returns having done nothing.
3. **The library never checks-and-skips.** It does not consult `Context.Database.CurrentTransaction` to
   decide whether to begin one. It tracks *its own* transaction reference and asks EF Core to begin one; if
   the context already carries a transaction it did not start, **EF Core's own
   `InvalidOperationException` propagates unwrapped**. That case is reachable only with
   `ContextOwnership.Borrowed` and an owner who started a transaction on the context directly, which is
   already outside what [A10](#one-context-one-live-data-access-layer) supports — and it now fails loudly
   instead of enrolling silently.

**A test obligation is warranted and has been added** — see the **Transactions** group in
[Test Obligations](#test-obligations). It is the regression guard for this defect: begin a transaction
directly on a borrowed context, then call `dal.TransactionStart()`, and require an exception rather than a
silent join.

---

## Migration

Every example is 2.2.x on the left, 3.0.0 on the right. There are **no compatibility wrappers** (S3), so each
is a required edit.

### Key-typed DAO declarations

```csharp
// 2.2.x
using ProphetsWay.EFTools.Int;
public class UserDao : BasePagedDao<User>, IUserDao
{
	public UserDao(DbContext context) : base(context) { }
}

// 3.0.0
using ProphetsWay.EFTools;
public class UserDao : BasePagedDao<User, int>, IUserDao
{
	public UserDao(DbContext context) : base(context) { }
}
```

```csharp
// 2.2.x — note the namespace shadowing System.Guid
using ProphetsWay.EFTools.Guid;
public class ResourceDao : BaseDao<Resource>, IResourceDao { /* … */ }

// 3.0.0
using ProphetsWay.EFTools;
public class ResourceDao : BaseDao<Resource, Guid>, IResourceDao { /* … */ }
```

| 2.2.x | 3.0.0 |
|---|---|
| `Int.BaseDao<T>` | `BaseDao<T, int>` |
| `Int.BaseGetAllDao<T>` | `BaseGetAllDao<T, int>` |
| `Int.BasePagedDao<T>` | `BasePagedDao<T, int>` |
| `Int.BaseSoftDao<T>` | `BaseSoftDao<T, int>` |
| `Int.BaseSoftGetAllDao<T>` | `BaseSoftGetAllDao<T, int>` |
| `Int.BaseSoftPagedDao<T>` | `BaseSoftPagedDao<T, int>` |
| `Guid.*<T>` | the same six with `, Guid` |
| `Long.*<T>` | the same six with `, long` |
| *(not expressible)* | `BaseDao<Country, string>` — **new reach, per S4** |

**The mechanical migration is: delete the sub-namespace `using`, add one type argument.** If a real migration
turns out to be larger than that, [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication)
names it as the evidence that reopens the no-wrappers half of D3.

### The `Get` override every old DAO carried

```csharp
// 2.2.x — every one of the 18 closures required this
public override T Get(T item)
{
	return Dao.Dataset.Where(i => i.Id == item.Id).SingleOrDefault();
}

// 3.0.0 — delete it. The base supplies it for any key type.
```

### The DAL root, string and options constructors

```csharp
// 2.2.x
public class ExampleDataAccess : BaseEFDataAccess<ExampleContext, int>, IExampleDataAccess
{
	public ExampleDataAccess(string connectionString) : base(connectionString) { }
	public ExampleDataAccess(DbContextOptions options) : base(options) { }
	// context built inside the library by Activator.CreateInstance
}

// 3.0.0
public class ExampleDataAccess : BaseEFDataAccess<ExampleContext>, IExampleDataAccess
{
	private readonly ICompanyDao _companyDao;
	// …

	// The consumer names the provider. The library never does.
	public ExampleDataAccess(string connectionString)
		: this(new ExampleContext(new DbContextOptionsBuilder<ExampleContext>()
			.UseSqlServer(connectionString)
			.Options), ContextOwnership.Owned)
	{ }

	public ExampleDataAccess(DbContextOptions<ExampleContext> options)
		: this(new ExampleContext(options), ContextOwnership.Owned)
	{ }

	// Dependency injection — the container owns the context's lifetime.
	public ExampleDataAccess(ExampleContext context)
		: this(context, ContextOwnership.Borrowed)
	{ }

	private ExampleDataAccess(ExampleContext context, ContextOwnership ownership)
		: base(context, ownership)
	{
		_companyDao = new CompanyDao(Context);
		// …
	}

	public Company? Get(Company item)
	{
		ThrowIfDisposed();          // required — see A5
		return _companyDao.Get(item);
	}
}
```

Note what moved: **`UseSqlServer` is now in the consumer's file**, which is the whole of
[D2](purpose-and-scope.md#owner-decisions--2026-08-15) expressed in one line. A PostgreSQL consumer writes
`UseNpgsql`; a SQLite one writes `UseSqlite`; the library is unaware of all three.

### Transactions

```csharp
// 2.2.x — from a DAO
_userDao.EnsureBeginTransaction();
_userDao.Insert(user);
_userDao.EnsureTransactionCommit();

// 3.0.0 — from the DAL, which is the sole authority
dal.TransactionStart();
dal.Insert(user);
dal.TransactionCommit();
```

Behavior differences to expect: `EnsureBeginTransaction` **silently did nothing** when a transaction was
already open, and `EnsureTransactionCommit` silently did nothing when none was. `TransactionStart` and
`TransactionCommit` **throw `InvalidOperationException`** in those cases. Code that relied on the silent
no-op is relying on a bug, and the exception is the parent contract surfacing it.

### Soft-delete clock

```csharp
// 2.2.x
public class LocalClockDao : Int.BaseSoftPagedDao<LegacyRecord>
{
	public LocalClockDao(DbContext context) : base(context, UseUtcTime: false) { }
}

// 3.0.0 — and note that the pair moves together (A13): a local clock needs a local relabel.
public class LocalClockDao : BaseSoftPagedDao<LegacyRecord, int>
{
	public LocalClockDao(DbContext context) : base(context) { }

	protected override DateTime GetCurrentTimestamp() => DateTime.Now;

	protected override DateTime NormalizeRetrievedTimestamp(DateTime value)
		=> DateTime.SpecifyKind(value, DateTimeKind.Local);
}

// …and the shape the flag could never express. The injected clock is expected to return UTC,
// so the default normalization still pairs with it; a clock returning anything else owes an
// override of NormalizeRetrievedTimestamp too (A13).
public class TestDepartmentDao : BaseSoftPagedDao<Department, int>
{
	private readonly Func<DateTime> _clock;
	public TestDepartmentDao(DbContext context, Func<DateTime> clock) : base(context) => _clock = clock;
	protected override DateTime GetCurrentTimestamp() => _clock();
}
```

**The local-time sample is deliberately not a `DepartmentDao`.** An earlier revision wrote it as one, which
was faithful to 2.2.x's `UseUtcTime: false` and directly contrary to `IDepartmentDao` **rule 18** — *local time
is never used* — so anyone carrying the snippet into `ProphetsWay.Example.DataAccess.EF` would have broken the
Example's contract by copying this document. `LegacyRecord` is a stand-in for whatever entity a migrating
consumer really has.

### `Context` and `Dataset` going `protected`

```csharp
// 2.2.x — business logic reaching through the DAL, which the paradigm exists to prevent
var recent = dal.UserDao.Dataset.Where(u => u.CreatedDate > cutoff).ToList();

// 3.0.0 — the query lives inside the DAO and is published on the DAO's own interface
public IList<User> GetCreatedSince(DateTime cutoff)
	=> Dataset.AsNoTracking().Where(u => u.CreatedDate > cutoff).ToList();
```

### Keyless DAOs

```csharp
// 2.2.x — two abstract methods, plus a third on the soft variant, all expressing "find this row"
public class CompanyResourceDao : BaseNonIdDao<CompanyResource>
{
	public override CompanyResource Get(CompanyResource item)
		=> Dataset.Single(x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId);

	public override int Update(CompanyResource item) { /* … */ }
}

// 3.0.0 — one predicate hook, one ordering hook, and no IBaseDao unless you want it
public class CompanyResourceDao : RootNonIdDao<CompanyResource>, ICompanyResourceDao
{
	public CompanyResourceDao(DbContext context) : base(context) { }

	protected override Expression<Func<CompanyResource, bool>> MatchRow(CompanyResource item)
		=> x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId;

	protected override IOrderedQueryable<CompanyResource> ApplyStableOrder(IQueryable<CompanyResource> query)
		=> query.OrderBy(x => x.CompanyId).ThenBy(x => x.ResourceId);
}
```

---

## Test Obligations

For `Test Designer`. **The groups below are subject-area groups** — key predicate, hooks, navigation loading,
CRUD, soft delete, and the rest. **They are not the `Scope` trait partition**, and Revisions 3 through 7 said
they were: *"Grouped to match the `Scope` trait partition in `ProphetsWay.Example.Tests`."* They are not, only
one of them names a scope, and about 125 of the obligations had no assignable scope at all. **Scope is stated
per obligation instead**, because almost every group below mixes.

**Scope notation.** Every obligation carries exactly one leading tag:

| Tag | `Scope` | Assigned when |
|---|---|---|
| **`[C]`** | `Contract` | Any conforming implementation must pass it, **and the behavior traces to a stated rule** — a decision in [Stage 2 Settled Decisions](#stage-2-settled-decisions) or [Design Decisions Made Here](#design-decisions-made-here), a member-contract table above, or a rule stated on an interface in `ProphetsWay.Example` or `ProphetsWay.BaseDataAccess` |
| **`[X]`** | `Characterization` | It pins a choice **this library** or **this provider** made that no stated rule requires — a declared limitation, a hazard demonstration, or a per-leg result |
| **`[D]`** | `Dispatcher` | Its subject is a call through `ProphetsWay.BaseDataAccess`'s reflection dispatcher rather than a Data Access Object member |

**The traceability half of `[C]` is the half that does work.** If no interface, XML `<remarks>`, numbered DAO
rule or decision in this document states the behavior being asserted, the obligation is `[X]` and not `[C]`.
A `Contract` assertion is an obligation placed on every future implementation of this design, and making one
in the name of a specification that does not make it is the failure `ProphetsWay.Example`'s own traceability
convention exists to prevent. Two obligations below are `[X]` precisely because the stated contract asks for
the *opposite* of what this mechanism delivers — the global-filter include divergence, and the
`NormalizeRetrievedTimestamp` bypass (G12). Marking either `[C]` would oblige every future implementer to
reproduce a gap.

**And the counts have to sum.** `ProphetsWay.Example` enforces that every test carries exactly one `Scope`
trait and that the three counts **sum to the suite total** — that sum *is* the integrity check, and **there is
no analyzer behind it**: no `.editorconfig` rule, no CI check on test metadata, nothing but the author being
told. This document's own tally is therefore published, so a translated suite can be checked the same way:

| Scope | Obligations |
|---|---|
| `Contract` | 124 |
| `Characterization` | 11 |
| `Dispatcher` | 8 |
| **Total** | **143** |

A suite whose traits do not sum to 143 has dropped or doubled one. **These are this document's obligations,
not `ProphetsWay.Example`'s suite**, whose own partition stands separately at 164 tests — Contract 139,
Characterization 5, Dispatcher 20 — over two legs, 328 executions. The two must not be added together.

Per [D4](purpose-and-scope.md#owner-decisions--2026-08-15), everything below runs on **both** certified legs
— SQLite in-memory and a SQL Server container. Several of these **cannot fail on SQLite and can on SQL
Server**, which is the reason for the second leg.

### Observing the generated SQL — the seam

**A number of obligations below assert on the *shape of the emitted SQL* rather than on a returned value.
Stated once, here, because the obvious mechanism does not exist on this API.**

`ToQueryString()` is an extension on `IQueryable`, and **no public member of this library returns one.**
`Get` returns `TEntity?`, `GetAll` and `GetPaged` return `IList<TEntity>`, `GetCount` returns `int`, and
`Dataset` is `protected` (S10) — so a test holding a Data Access Object reference has nothing to call it on.
Revision 5 prescribed it on nine obligations regardless; the prescription was unexecutable and is withdrawn.

**Three routes exist. All are legitimate; they observe different things — and the first two are not of equal
fidelity, which an earlier revision obscured by listing them as one.**

| Route | What it observes | Limit |
|---|---|---|
| **A `DbCommandInterceptor`** | The **command as sent to the provider** — final SQL text **and its `DbParameterCollection`**, as an object. Registered on the options the test's context is built from, so the Data Access Object under test is the real one | Captures a command, not a query object. An assertion is a string or parameter-collection assertion, and it must be written to tolerate provider formatting differences between the two legs |
| **`DbContextOptionsBuilder.LogTo`** | The **formatted log text** of that same command | **Text only.** There is no `DbParameterCollection` to inspect, and parameter *values* are **redacted** unless `EnableSensitiveDataLogging()` is enabled on the options. Sufficient wherever the assertion is over SQL text — an `ORDER BY`, a join, a `COLLATE` clause, whether a command was issued at all — and **not** sufficient for the parameterization obligation |
| **A test-only Data Access Object whose hook override captures the query it was handed and calls `ToQueryString()` on it** | The query **as composed up to that hook** — `ApplyReadFilter`, `ApplyIncludes` or `ApplyStableOrder` receives an `IQueryable<TEntity>`, and that is a real one | **Two limits, both real.** It observes the pipeline *only up to that hook*, so anything the member does afterwards — `Skip`/`Take`, `AsNoTracking`, the discard in `GetCount` — is invisible to it. And **it changes the Data Access Object under test**: the subject is now a DAO carrying an override, not the plain one |

**Nine obligations below cite the "interceptor route." Eight are satisfiable by either of the first two;
the parameterization obligation in [Key predicate](#key-predicate--the-riskiest-area-s4) names
`DbCommandInterceptor` specifically, because it reads the parameter collection rather than the text.**

**Which to use is the obligation's to state, and each one below now does.** Where the assertion is about
what reached the store — an `ORDER BY`, a join, a `COLLATE` clause, a parameter versus a literal, whether a
query was issued at all — the **interceptor** is the honest instrument, because it is the only one that sees
the finished command. The hook route is the cheaper one and is sufficient where the assertion is about
composition *before* the hook, and where a DAO already carries an override for other reasons.

**Neither route is a `ToQueryString()` call on a member's return value, and no obligation below asks for
one.**

### Key predicate — the riskiest area (S4)

- [ ] **[C]** `Get` / `Update` / `Delete` translate and round-trip for `int`, `long`, `Guid`, `string`, `int?`.
- [ ] **[C]** **The query is translated, not client-evaluated.** Assert on translation, not just on the result — a
	client-evaluated predicate returns the right answer while reading the whole table. EF Core's translation
	failure guards execution; confirm the predicate reached the store through the **interceptor route** in
	[Observing the generated SQL](#observing-the-generated-sql--the-seam) — the emitted command carries a
	`WHERE` over the identifier column.
- [ ] **[C]** The key value is **parameterized**, not emitted as a literal. **Interceptor route**: the captured
	command's parameter collection is non-empty and the key value appears in it rather than inline in the SQL
	text. This is the one assertion the hook route cannot make — parameterization is decided when the command
	is built, after every hook has run.
- [ ] **[C]** Null key: `Get` → `null`, `Update` → `0`, `Delete` → `0`, **and no query is issued** for any of them.
- [ ] **[C]** A stored row with a **null `string` key** does not cause a `NullReferenceException` in any member.
- [ ] **[C]** An entity whose identifier property is **non-public**, or an **explicit** `IBaseIdEntity<T>`
      implementation, throws `DataAccessConventionException` **from the DAO constructor**, not on first use.
- [ ] **[C]** An identifier property with **no set accessor** throws `DataAccessConventionException` from the DAO
	constructor; a non-public setter is accepted, matching the parent dispatcher's `CanWrite` rule.
- [ ] **[C]** Constructor validation checks a null context first: a mis-wired entity constructed with a null context
	throws `ArgumentNullException`, not `DataAccessConventionException`.
- [ ] **[C]** DAO construction does not resolve `Dataset`; an unmapped entity throws EF Core's
	`InvalidOperationException` on first store access rather than during construction.
- [ ] **[C]** An entity exposing both `{TypeName}Id` and `Id` resolves to `{TypeName}Id`, and
      `dal.Get<T>(id)` and `dao.Get(item)` address the **same** property.
- [ ] **[C]** **`default(TKey)` is an ordinary key value** (OD-3): `Get`/`Update`/`Delete` on an entity carrying `0`,
	`Guid.Empty` or `""` **issue a query** — **interceptor route**, since the assertion is that a command was
	sent at all — and miss normally when no such row is stored. Contrast with the null-key cases above, where
	the interceptor must record **no command**.
- [ ] **[C]** A row genuinely stored under `0` / `Guid.Empty` / `""` **is retrievable by `Get`** and is not hidden by
	any short-circuit.

### Hooks and overrides — A12, A18, A20

A12's justification is that a `MatchRow` override "changes all three at once and cannot end up with a `Get`
that finds a row its `Update` cannot." **That claim had no test in Revision 3.** These pin it, and pin the
other three overridable hooks with it.

- [ ] **[C]** A keyed DAO overriding `MatchRow` with a **composite** predicate — `x => x.TenantId == item.TenantId &&
	x.Id == item.Id` — has **`Get`, `Update` and `Delete` all honor it**: a row matching the key but not the
	tenant is invisible to all three, and a row matching both is found by all three.
- [ ] **[C]** A `MatchRow` override that **matches nothing** (`x => false`) makes all three miss **consistently** —
	`Get` → `null`, `Update` → `0`, `Delete` → `0` — for a row that is demonstrably stored.
- [ ] **[C]** A `GetKey` override returning a value other than the stored property's is honored by all three, and by
	nothing else — `GetAll` ordering still uses `KeySelector` over the resolved property.
- [ ] **[C]** A `KeyEquals` override is reached by the default `MatchRow`, and **is not reached** when `MatchRow` is
	itself overridden — the single-path claim (A12) fails if both are consulted.
- [ ] **[C]** An `ApplyReadFilter` override is honored by `GetAll`, `GetPaged` **and** `GetCount`, which continue to
	agree with one another, and is **not** applied by `Get` — a row the filter excludes is still retrievable
	by key.
- [ ] **[C]** **Composition order is filter → include → order** (A20): a soft DAO's `GetPaged` window over the
	not-deleted set skips no live row, i.e. the filter is applied before `Skip`/`Take`, not after.
- [ ] **[C]** **`ApplyReadFilter` receives the raw `Dataset` query** (A23) — assertable by having the override record
	what it was handed and comparing its element count to the full table.
- [ ] **[C]** **`ApplyIncludes` receives the row-restricted query for its path** (A23): on `GetAll`/`GetPaged` it is
	handed `ApplyReadFilter`'s output, so a soft DAO recording what it received sees the not-deleted set; on
	`Get`/`GetCore` it is handed the `MatchRow`-matched query. **These two are one obligation with two halves
	and must both be written** — Revision 4 carried a Hooks-group obligation asserting the opposite of its own
	Navigation-group one.
- [ ] **[C]** **`ApplyStableOrder` receives the filtered, included query** (A23): a soft DAO overriding all three has
	its ordering hook handed a query that already excludes deleted rows and already carries the includes.
- [ ] **[C]** **`GetCount` invokes `ApplyStableOrder` exactly once and discards the result** (A27): an override that
	increments a counter is called once per `GetCount`, **and** the command that same call emits carries no
	`ORDER BY`. The second half is the **interceptor route** — the hook route cannot see it, because the
	discard happens after the hook returns and the ordered query the hook produced is precisely the one that
	never reaches the store. Both halves are required: the count alone does not distinguish
	invoked-and-discarded from never-invoked.
- [ ] **[C]** `GetCount` on a keyless DAO with **no** `ApplyStableOrder` override throws `NotSupportedException`,
	which is the observable consequence of the same mechanism.

### Navigation loading — OD-1, A18

The group that would have caught the Revision 3 defect. **Both halves are required**: the default must be
pinned as firmly as the override, or a later change to the default goes unnoticed.

- [ ] **[C]** **A Data Access Object that does *not* override `ApplyIncludes` returns `null` navigation properties**
	from `Get`, `GetAll` and `GetPaged`, on both certified providers. This is the default under test, not an
	incidental observation.
- [ ] **[C]** A DAO that **does** override `ApplyIncludes` returns those navigation properties populated from `Get`,
	`GetAll` and `GetPaged` alike — all three, not `Get` alone.
- [ ] **[C]** **Two levels deep**: a DAO overriding with `Include(...).ThenInclude(...)` returns a populated
	second-level navigation. This is the `Transaction` → `User` → `Company` shape
	`ProphetsWay.Example.Tests/SnapshotDeepCopyTests.cs` asserts on.
- [ ] **[C]** **An included navigation is itself a snapshot**: editing it changes neither the store nor a second
	retrieval's copy of the same row, with `AsNoTracking()` in force.
- [ ] **[C]** `GetCount` **issues no join** for a DAO that overrides `ApplyIncludes` — **interceptor route**, asserting
	on the emitted command, not on the returned number.
- [ ] **[C]** A DAO overriding `ApplyIncludes` and a model declaring `AutoInclude` on a *different* navigation of the
	same entity produce the **union** of both, not one or the other.
- [ ] **[C]** **`AutoInclude` alone populates a navigation** through a DAO that overrides nothing, with the library's
	mandated `AsNoTracking()` in force — the sanctioned-alternative claim in
	[Navigation Loading](#navigation-loading) is otherwise untested.
- [ ] **[C]** The filter runs before the includes: a soft DAO overriding both has its included graph materialized
	only for rows the filter admitted.
- [ ] **[C]** **Identity resolution stays suppressed, asserted *within one query*** — the guard on the
	`AsNoTrackingWithIdentityResolution()` substitution (N5). Two shapes, either of which is sufficient:
	`GetAll` over two `User` rows naming one `Company` returns
	`a.Company.ShouldNotBeSameAs(b.Company)`; or `Get(Transaction)` with the two-level override, where
	`t.Company` and `t.User.Company` name the same stored row, returns two instances. **No existing Example
	test covers this** — its independence assertions all span two queries or two Data Access Layer instances,
	where both settings agree — so this obligation is new here and cannot be inherited.
- [ ] **[C]** **Including a soft-delete entity bypasses that entity's `ApplyReadFilter`** (N10): with
	`UserDao.ApplyIncludes` including `User.Department` and that department soft-deleted, `Get`, `GetAll` and
	`GetPaged` all return the user with `Department` **populated**. This is the stated behavior, not a defect;
	pin it so nobody "fixes" it into a dangling reference.
- [ ] **[X]** **…and it bypasses `NormalizeRetrievedTimestamp` with it** (G12). Same arrangement: soft-delete
	nothing, read a `User` through a Data Access Object that includes `Department`, and require the **included**
	department's `CreatedDate` to carry `DateTimeKind.Unspecified`, while the **same stored row** read through
	`DepartmentDao.Get` carries `DateTimeKind.Utc`. The hook is declared on `BaseSoftDao` and
	`RootSoftNonIdDao` and applied by those Data Access Objects' own reads (A13); `UserDao` is a **hard** DAO
	and has none, so the including query hands back whatever the provider stored — and neither certified
	provider stores `Kind`. **`Characterization`, deliberately, and this is the one place in the group where
	the tag carries an argument**: `IDepartmentDao` rule 18 requires `Kind == Utc` on a retrieved instance, so
	this pins a **divergence from the stated contract**, not conformance to it. Tagging it `[C]` would oblige
	every future implementation to reproduce a gap. Pin it so the divergence is visible in a test run rather
	than discovered in a consumer's logs — see
	[Including a soft-delete entity bypasses its `ApplyReadFilter`](#including-a-soft-delete-entity-bypasses-its-applyreadfilter--and-that-is-correct).
- [ ] **[C]** **The library emits neither split nor forced-single queries** (A29): a DAO that overrides `ApplyIncludes`
	without calling either produces **one** command; the same DAO with `AsSplitQuery()` in its override produces
	several. **Interceptor route**, counting commands per call — which is also the cleanest form the assertion
	has, since "split" is observable as a command count and not as a substring. Not exercisable against
	`ProphetsWay.Example`, which has no collection navigation — write it against a purpose-built entity.

### Global query filters — OD-5, A28

The group that pins the defense. Every obligation runs on **both** legs, and each needs a model declaring a
`HasQueryFilter` — which means a context built for these tests, not the Example's.

- [ ] **[C]** **A consumer-declared soft-delete global filter does not stop `Get` returning a soft-deleted row.**
	Declare `HasQueryFilter(d => d.DeletedDate == null)` on `Department`, soft-delete one, and require
	`Get` to return it — `IDepartmentDao` rule 8 holding against a mechanism this library does not own. This is
	the headline obligation of OD-5.
- [ ] **[C]** The same filter on a **hard** entity with a non-soft-delete predicate — `c => c.IsActive` — does not stop
	`Get` returning an inactive row.
- [ ] **[C]** **…and does not stop `Delete` hard-deleting it** (Finding 10). With `HasQueryFilter(c => c.IsActive)`
	declared and a row stored inactive, `Delete` returns **`1`** and the row is **genuinely gone**. A consumer
	will expect a global filter to protect that row; A28 lifts it on every `MatchRow`-located path and `Delete`
	is one. **This is `Contract`, not `Characterization`** — it follows from A12, and an implementation that
	"protected" the row would make `Delete` disagree with the `Get` above about what exists. Pin it so nobody
	softens A28 into "lift filters on reads only."
- [ ] **[C]** **Named filters do not change the rule** (A31, EF Core 10). Declare **two** named filters on one entity
	and require a `MatchRow` lookup to lift **both** — `Get` returns a row excluded by either. A28 uses the
	parameterless `IgnoreQueryFilters()` deliberately; an implementation that lifts a named subset makes the
	defense depend on how a consumer partitioned their filters, which this library cannot know.
- [ ] **[C]** **`Update` and `Delete` still find a filtered-out row** (A28): `Update` on a soft-deleted department
	returns `1` and stamps `UpdatedDate` (rule 4), and `Delete` on an already-deleted one returns `0` rather
	than missing (rule 6), with the filter declared. Without the `IgnoreQueryFilters()` on the locating fetch
	both return `0`, and the second failure is invisible because `0` is also the correct answer for a different
	reason.
- [ ] **[C]** **The trio still honors the filter, and the three still agree**: `GetAll`, `GetPaged` and `GetCount`
	omit the filtered rows and `GetCount == GetAll().Count`.
- [ ] **[C]** **The emitted SQL shows the difference**: the filter's predicate is **absent** from `Get`'s command and
	**present** in `GetAll`'s. **Interceptor route** — this obligation had no executable mechanism before
	Revision 6, and it is the sharpest of the group, because the behavioral assertions above can also be
	satisfied by an implementation that never declared the filter correctly in the first place.
- [ ] **[X]** **The include divergence is real and is pinned as such**: with a filter on `Department` and
	`UserDao.ApplyIncludes` including it, `Get(user)` populates `User.Department` for a deleted department while
	`GetAll(null)` returns that user with `Department` **`null`**. Characterization, not contract — it is the
	cost of declaring the exclusion rule twice, and it is why a global filter is documented as a conflicting
	mechanism.

### CRUD

- [ ] **[C]** `Update` on an absent row returns `0` and throws nothing — **on both providers**.
- [ ] **[C]** `Update` with **values identical to the stored row** returns `1`, not `0` (the EF no-op case) — the
	**ROW COUNT RULE**'s most easily lost clause, and the reason A22 rejects `ExecuteUpdate`.
- [ ] **[C]** `Delete` on an absent row returns `0`; a second `Delete` returns `0`; neither throws.
- [ ] **[C]** `Insert` writes the generated key back onto the argument — the **IDENTIFIER RULE**, elected in
	`ProphetsWay.Example` rather than promised by `ProphetsWay.BaseDataAccess`.
- [ ] **[C]** `Insert` with a caller-assigned key on a store-generated column: the generated key wins.
- [ ] **[C]** `Get` on a missing row returns `null`.
- [ ] **[C]** A duplicate keyed match and an under-specified keyless `MatchRow` each make `SingleOrDefault` throw
	`InvalidOperationException`; neither path silently chooses a row.
- [ ] **[C]** `Get`, `Insert`, `Update`, `Delete` throw `ArgumentNullException` on `null`; `GetAll`, `GetPaged`,
      `GetCount` **do not**.
- [ ] **[C]** **`Update` on a row deleted by another connection between the fetch and the `SaveChanges` throws
	`DbUpdateConcurrencyException`, unwrapped, and does not return `0`** (R4-S7). **SQL Server leg only** — see
	[SQLite leg limitations](#sqlite-leg-limitations).
- [ ] **[C]** The same for `Delete`, on the same terms — the two must not diverge.
- [ ] **[C]** `Update` detaches the row it fetched: mutating that entity afterwards, then triggering a `SaveChanges`
	through another Data Access Object on the same context, does not reach the store.

### Writes with a populated navigation graph — OD-4, A24–A26

**There was no such group anywhere in Revision 4**, and `SnapshotDeepCopyTests` covers this path three times.
Every obligation here needs a graph that is genuinely populated, so the setup is the Example's: insert the
company, job and department first, then hang them off the user.

- [ ] **[C]** **`Insert` writes one row.** `Insert(user)` where `user.Company` names stored company `7` produces
	**one** new `User` row and **no** new `Company` row — assert the company table's count is unchanged, not
	just that the name reads correctly. A by-value assertion passes against the duplicate-insert bug; a count
	does not.
- [ ] **[C]** **The association survives**: that row's foreign key is `7`, and a fresh `Get(user)` through a DAO that
	includes `Company` returns `Company.Id == 7`. **`User` declares no `CompanyId`** — opening
	`ProphetsWay.Example.DataAccess/Entities/User.cs` confirms it carries a `Company` navigation and no scalar,
	so the foreign key is a **shadow** property. **Use the `Get`-with-`Include` half**, which this obligation
	already supplies and which needs no access to the shadow value; reading it directly would require a
	test-owned context and `Entry(x).Property("CompanyId").CurrentValue`, and buys nothing here.
- [ ] **[C]** **The caller's related instance is untouched**: `user.Company.Id` is still `7` after the call, and
	`user.Id` carries the generated value. **This is the assertion the Example suite does not make** — it
	re-reads through `Get(new Company { Id = co.Id })`, evaluating `co.Id` after a naive `Add` would have
	overwritten it, so it follows the duplicate and passes.
- [ ] **[C]** **The client-generated case, which is where luck runs out**: `Insert` a graph whose related entity is a
	stored `Resource` (`Guid` key). It succeeds. Under `Dataset.Add(item)` it fails on a duplicate primary key.
	This is the regression guard for A24 and it is the one that cannot pass by accident.
- [ ] **[C]** **A related entity whose key names no stored row fails at the provider**, with the referential-integrity
	exception propagated unwrapped — not silently inserted. Inserting a whole new graph in one call is
	unsupported by design.
- [ ] **[C]** **`default(TKey)` on a related entity is not special-cased** (OD-3 × OD-4): a related entity genuinely
	stored under `0` or `Guid.Empty` is attached and its foreign key written, exactly as for any other value.
- [ ] **[C]** **One object reached twice is attached once**: assign the same `Company` instance to both
	`trans.Company` and `trans.User.Company` and `Insert(trans)` succeeds — the shape
	`Setup_CreateTransaction_TestInsertDoesNotAdoptItsSecondLevel` builds.
- [ ] **[C]** **Two distinct instances carrying one key throw**, unwrapped, from EF Core's duplicate-tracking check.
	The library neither de-duplicates nor picks a winner.
- [ ] **[X]** **`Update` cannot repoint a navigation-only relationship** (A25): change `transaction.User` to a
	different stored user, call `Update`, and require **`1`** returned and the stored foreign key **unchanged**.
	This is a `Characterization` test of a declared limitation, not a bug report — assert it so a later
	implementation cannot quietly acquire the behavior without a decision.
	**`Transaction` declares neither `UserId` nor `CompanyId`** — confirmed by opening
	`ProphetsWay.Example.DataAccess/Entities/Transaction.cs`, which carries `User` and `Company` navigations and
	no foreign-key scalars — so there is no property to read and the `Get`-with-`Include` route is not available
	either, the point being precisely that the *stored* value did not move. **This one intends the test-owned
	context**: `Entry(stored).Property("UserId").CurrentValue`, read on a context the test owns rather than the
	DAL's. Do not stall looking for a `UserId` property; there is not one, and that absence is the subject under
	test.
- [ ] **[C]** **An entity with an explicit foreign-key scalar is not subject to that limitation** (A25,
	[OD-9](#owner-decisions-taken-during-revision-8)). Store two companies; `Insert` an `Assignment` naming the
	first; then set `assignment.CompanyId` to the **second** company's identifier and `assignment.Note` to a new
	value, and call `Update`. Require **`1`** returned, the stored `Note` **written**, and the stored foreign key
	**moved** — re-read through a Data Access Object that includes `Company` and require `Company.Id` to be the
	second. That is the positive half of A25: where the foreign key is a mapped scalar, `SetValues` repoints the
	relationship like any other column. **`Assignment` is purpose-built and is not present in
	`ProphetsWay.Example`** — declare it in the test model, as the collation obligations declare `Country`:
	`int Id`, `int CompanyId`, `Company Company`, `string Note`. **`CompanyResource` will not serve**, and the
	reason is worth carrying into the test file so nobody substitutes it: both of its mapped scalars sit in its
	`MatchRow` predicate, so an `Update` changing either would locate nothing and return `0`, and it declares no
	navigation property to repoint. See
	[`Update` writes scalars](#update-writes-scalars-and-cannot-repoint-a-relationship--a25).
- [ ] **[C]** **Detachment spans the whole reachable graph** (A26): after `Insert(user)` **and** after
	`Update(user)`, mutate `user.Company.Name`, then trigger a `SaveChanges` through a **different** Data Access
	Object on the same context, and require the `Company` row unchanged. Run it for both write members — they
	track different things and a fix to one does not carry.
- [ ] **[C]** **The graph a locating fetch materialized is detached too — and `AutoInclude` is the only thing that
	produces one** (A26, OD-7, [OD-8](#owner-decisions-taken-during-revision-8), H7). Declare `AutoInclude` on a
	navigation of the updated entity in the test model, call `Update(item)`, and require that afterwards
	**nothing is tracked**: neither the fetched row nor any auto-included entity hanging off it, so a later
	`SaveChanges` through a different Data Access Object on the same context writes none of them. **Assert
	through the change tracker** — a Data Access Object subclass exposing its `protected` `Context`, or the
	context the test itself built and handed to the Data Access Layer — because the fetched row is never handed
	to the caller and there is no other way to reach it. **Without `AutoInclude` this obligation has no
	subject**, and that is why the wording it replaces was unauthorable: `Update`'s locating fetch applies
	neither `ApplyReadFilter` nor `ApplyIncludes` and `item` is never tracked, so **every navigation on the
	fetched row is `null`**. A26's *"plus any fetched row's whole reachable graph"* clause was retracted on that
	ground by [OD-8](#owner-decisions-taken-during-revision-8).
- [ ] **[C]** **`Insert` inside a transaction, then roll back**: the detachment does not interfere — the row is gone,
	the generated identifier is still on the caller's instance, and the related rows were never written and are
	still there.
- [ ] **[C]** **A failed write leaves nothing tracked** (OD-7). **The failure has to be a *removable* one, or the
	assertion cannot be reached.** `Insert` a graph whose related entity names **no stored row** and let the
	provider's referential-integrity exception propagate — the path the obligation four above already specifies.
	Then insert the missing principal **through a different Data Access Object on the same context** and let that
	`SaveChanges` run. Require that the failed dependent row **does not appear** in the store afterwards. Under
	the Revision 5 rule the dependent stayed tracked as `Added`, the row it named now exists, and the second
	`SaveChanges` carries it in — so *"the failed row appears"* is **directly assertable**, which is what makes
	this obligation an observation rather than a hope. **A unique-constraint violation will not serve**: the
	retained `Added` entity violates the *same* constraint on the next `SaveChanges`, so step two throws, the
	stated assertion is never evaluated, and the test reports the wrong failure.
	**And the referential integrity has to be *asserted*, not assumed** (G11). This obligation turns entirely on
	the provider raising that exception. `Microsoft.Data.Sqlite` issues `PRAGMA foreign_keys = 1` on a connection
	it opens itself, but a context built over a connection the **test** opened, or one whose connection string
	carries `Foreign Keys=False`, enforces nothing — the offending `Insert` then **succeeds**, no exception
	fires, the arrange step never reaches the act, and the obligation passes while testing nothing about OD-7.
	Read `PRAGMA foreign_keys` back in the arrangement and require `1` before proceeding. This is exactly the
	"passes on the fast leg while asserting nothing" trap [SQLite leg limitations](#sqlite-leg-limitations)
	exists to catch, and it is listed there.
- [ ] **[C]** **And the instance is still usable** (OD-7): after that failed `Insert`, fix the argument — point it at a
	principal that exists — and call `Insert` again **on the same Data Access Layer instance**. It succeeds. No
	dispose, no rebuild. **This one discriminates nothing on its own and must not be credited with doing so**: on
	a broken implementation the still-tracked `Added` entity is the same object the caller just fixed, so the
	retry succeeds either way. It pins that the discard is not a punishment; the obligation above is the only one
	of the pair that pins that the discard happened.
- [ ] **[C]** **A failed `Update` detaches its fetched row too** (OD-7): provoke a failure from `SaveChanges` on an
	`Update`, then mutate the entity that `Update` fetched and trigger a `SaveChanges` through another Data
	Access Object on the same context. Nothing reaches the store.
- [ ] **[C]** **A related entity carrying a default key is set `Unchanged`, not `Added`** (A24, Finding 2). Store a
	`Company` under key `0` — legal per OD-3 — hang it off a new `User`, and `Insert(user)`. Require **one** new
	`User` row and **no** new `Company` row. This is the guard on the `Attach` trap: an implementation that walks
	the graph with `DbContext.Attach` instead of assigning `EntityState.Unchanged` marks that company `Added` by
	EF Core's default-key heuristic and inserts a duplicate. **It is a distinct test from the `Dataset.Add`
	guard above** — an implementation can be immune to one and not the other, and this one fails only on the
	default-keyed slice of data.
- [ ] **[C]** **`Update` writes the root and does not cascade into the graph** (OD-6). Retrieve a `User` through a DAO
	that includes `Company`, edit `user.Company.Name`, edit a scalar on the user itself, call `Update(user)`, and
	require the **user's scalar written** and the **`Company` row unchanged** — re-read the company directly, not
	through the user. This is `Scope=Contract`. It was blocked in Revision 5 and is **unblocked by
	[OD-6](#owner-decisions-taken-during-revision-6)**, which settled the question in favor of root-only. See
	[The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6).

### Snapshot and tracking

- [ ] **[C]** Mutating an instance returned by `Get` / `GetAll` / `GetPaged` does not change stored data.
- [ ] **[C]** Mutating an argument **after** `Insert` / `Update` / `Delete` returns does not reach the store — including
      after an unrelated later `SaveChanges` triggered through a **different DAO on the same context**.
- [ ] **[C]** **The same holds for anything reachable from that argument** (A26), which is where a root-only
      detachment fails. See [Writes with a populated navigation
      graph](#writes-with-a-populated-navigation-graph--od-4-a24a26) for the full group.
- [ ] **[C]** Two separate retrievals of the same row return **independent** instances (`ReferenceEquals` is false).
- [ ] **[C]** A navigation property that `ApplyIncludes` **loaded** is itself a snapshot; editing `userA.Company`
      changes neither `userB.Company` nor the store. A DAO that loads nothing has nothing to assert here —
      see [Navigation loading](#navigation-loading--od-1-a18) for the default-is-null obligation.
- [ ] **[C]** Conformance holds with the context configured as `TrackAll` **and** as `NoTracking` — the DAO must not
      depend on it.

### Ordering and paging

- [ ] **[C]** `GetAll` and `GetPaged` return the same entities in the same order.
- [ ] **[C]** Successive `GetPaged` windows partition the `GetAll` set with **no overlap and no omission**.
- [ ] **[C]** Ordering is stable across two calls with no writes between them.
- [ ] **[C]** A nullable or duplicate-capable key overrides `ApplyStableOrder` with a deterministic tie-breaker;
	successive windows remain stable when null or duplicate ordering values exist.
- [ ] **[C]** `GetCount` equals `GetAll().Count`.
- [ ] **[C]** `skip` beyond the count → empty; `take == 0` → empty; `take` past the remainder → the remainder.
- [ ] **[C]** Negative `skip` or `take` → `ArgumentOutOfRangeException`.
- [ ] **[C]** A keyless DAO paginates correctly through its `ApplyStableOrder` hook.
- [ ] **[C]** A keyless DAO that does not override `ApplyStableOrder` may still `Insert` and `Delete`, while
	`GetAll`, `GetPaged` and `GetCount` each throw `NotSupportedException`.
- [ ] **[C]** **That `NotSupportedException`'s message names both the Data Access Object type and
	`ApplyStableOrder`** (M7). A15 demands a specific message and the demand is worthless unpinned; assert on
	the presence of the two names, not on the whole sentence, so the wording can be improved without breaking
	the test.
- [ ] **[D]** **The same exception surfaces from the dispatcher entry point** (R4-S6): with the Data Access Layer
	forwarding `GetAll(T?)` for such a DAO, `dal.GetAll<T>()` throws `NotSupportedException` — not
	`DataAccessConventionException`, and not a wrapped form of either.
- [ ] **[C]** `GetPaged` on such a DAO throws `ArgumentOutOfRangeException` for a negative `skip` **before** it throws
	`NotSupportedException` — argument validation comes first.
- [ ] **[C]** **Ordering holds at scale, pinned by a number and a mechanism.** Seed **10,000 rows** and require
      `GetAll` and successive `GetPaged` windows to agree across two executions with no writes between them.
      **And, in the same test, assert that the emitted command carries an explicit `ORDER BY`** —
      **interceptor route**, per [Observing the generated SQL](#observing-the-generated-sql--the-seam). The row
      count is the smoke test; the `ORDER BY` assertion is the guard, because it cannot pass by luck at any row
      count, on any plan, on either leg. "A row count large enough for SQL Server to change scan plans" was the
      earlier wording and is not authorable — nobody can look up that number, and it is not a constant. This is
      the test that would have caught the 2.2.x unordered `GetAll`.

### Soft delete

- [ ] **[C]** `Insert` stamps `CreatedDate` and nulls `UpdatedDate` / `DeletedDate` **even when the caller set them**.
- [ ] **[C]** `Update` stamps `UpdatedDate`, preserves stored `CreatedDate` and `DeletedDate`, and **ignores** the
      incoming values of all three.
- [ ] **[C]** **And specifically: `Update` on a soft-deleted row with `item.DeletedDate == null` leaves the row
	deleted** (A30). Soft-delete a row, `Get` it, set `DeletedDate = null` on that instance, call `Update`, and
	require the row **still deleted** — absent from `GetAll` and still carrying its original `DeletedDate`. This
	is the guard on the mechanism: `SetValues` copies every mapped scalar by name, so an implementation that
	does not exclude or restore the three timestamps silently un-deletes the row and rewrites its creation time.
	The obligation above can pass on an instance that happened to carry the stored value; this one cannot.
	**On the same instance, also set `CreatedDate` to a distinguishable wrong value and require the stored one
	preserved** (F6). A30 names `CreatedDate` as failing the same way and less visibly, and it is the timestamp
	an instance is most likely to be carrying correctly by accident.
- [ ] **[C]** `Update` on a soft-deleted row succeeds, returns `1`, and leaves it deleted.
- [ ] **[C]** `Delete` stamps `DeletedDate`, returns `1`, and leaves the row retrievable by `Get`.
- [ ] **[C]** `Delete` on an already-deleted row returns `0` and **does not refresh** the existing `DeletedDate`.
- [ ] **[C]** `Delete` on an absent row returns `0`.
- [ ] **[C]** `GetAll` / `GetPaged` / `GetCount` omit deleted rows and agree with one another; all-deleted →
      two empty lists and `0`.
- [ ] **[C]** `Get` **returns** a soft-deleted row.
- [ ] **[C]** Stamped values are visible on the caller's instance after the call.
- [ ] **[C]** `GetCurrentTimestamp` default is UTC; an override is honored by all three stamping members; it is called
      **once** per operation.
- [ ] **[C]** Timestamps retrieved through `Get`, `GetAll` and `GetPaged` pass through
	`NormalizeRetrievedTimestamp`; the default restores `DateTimeKind.Utc` on SQLite and SQL Server.
- [ ] **[X]** A custom timezone policy overriding both timestamp hooks round-trips its documented `DateTimeKind`;
	overriding only one hook is covered as a characterization hazard rather than a conforming policy.
- [ ] **[C]** **Both of the above run twice — once against a `BaseSoftDao` descendant and once against a
	`RootSoftNonIdDao` descendant** (R4-S2). The Timestamp Pair Rule has four override sites on two unrelated
	branches and no compiler check spanning them, so a suite that covers the keyed branch alone leaves half of
	them unguarded.
- [ ] **[C]** `RootSoftNonIdDao` performs soft insert/delete/filtering without publishing `Get` or `Update`, and
	`BaseSoftNonIdDao.Update` cannot bypass timestamp preservation through a hard-update core.
- [ ] **[C]** **A soft DAO reached through a `BaseDao<TEntity, TKey>`-typed reference still soft-deletes** — the A2
      regression guard. This test fails against 2.2.x.

### Transactions

- [ ] **[C]** Writes inside a transaction persist on `TransactionCommit` and vanish on `TransactionRollBack`.
- [ ] **[C]** `TransactionStart` twice → `InvalidOperationException`.
- [ ] **[C]** `TransactionCommit` or `TransactionRollBack` with none open → `InvalidOperationException`.
- [ ] **[C]** `TransactionRollBack` after a commit → `InvalidOperationException`.
- [ ] **[C]** After a **failed** commit: no transaction is open, the writes are gone, and a following
      `TransactionRollBack` throws `InvalidOperationException`.
- [ ] **[C]** Writes made outside a transaction auto-commit.
- [ ] **[C]** Two DAL instances over the same database do not share a transaction. **SQL Server leg only** — see
	[SQLite leg limitations](#sqlite-leg-limitations).
- [ ] **[C]** A rollback reverses an `Update` to the **pre-update** state — the snapshot rule's transaction corollary.
- [ ] **[C]** **A rollback does not un-assign a generated key** (R4-S4): `Insert` inside a transaction, roll back, and
	the caller's instance still carries the identifier `SaveChanges` wrote onto it. The row is gone; the value
	is not. Assert on the instance, and separately assert `Get` on that identifier now returns `null`.
- [ ] **[X]** Re-inserting that same instance after the rollback succeeds, and on a store-generated column receives a
	**different** identifier — stated as characterization, not contract, because sequence recycling is the
	provider's business.
- [ ] **[C]** **`TransactionStart` on a borrowed context that already carries a transaction its owner began does not
	silently join it** — it throws. This is the regression guard for the 2.2.x `EnsureBeginTransaction`
	no-op; see [Two 2.2.0 defects](#two-220-defects-this-design-retires). Assert that an exception is thrown
	and that the outer transaction is still the one in force; do **not** assert the exception's type beyond
	`InvalidOperationException`, since it originates in EF Core.
- [ ] **[C]** An ambient `TransactionScope` is neither created nor suppressed by this library.

### Disposal

- [ ] **[C]** Constructing `BaseEFDataAccess` with a null context throws `ArgumentNullException`; an undefined
	`ContextOwnership` value throws `ArgumentOutOfRangeException`.
- [ ] **[C]** `Dispose` twice does not throw.
- [ ] **[C]** Every other member throws `ObjectDisposedException` after disposal — **including with
	`ContextOwnership.Borrowed`**, where the context is still alive.
- [ ] **[D]** **The seven inherited dispatcher members throw `ObjectDisposedException` after disposal** (A19):
	`dal.GetAll<T>()`, `dal.GetPaged<T>(0, 10)`, `dal.GetCount<T>()`, `dal.Get<T>(id)`, `dal.Insert<T>(item)`,
	`dal.Update<T>(item)`, `dal.Delete<T>(item)` — all seven, on a disposed instance.
- [ ] **[D]** **A dispatcher-entry call and a direct forwarder call on the same disposed instance report the same
	thing.** Take an entity whose Data Access Layer forwarder exists: `dal.GetAll<Company>()` and
	`dal.GetAll(default(Company))` both throw `ObjectDisposedException`.
- [ ] **[D]** **And they still agree when the forwarder is missing.** For an entity the Data Access Layer does *not*
	forward, `dal.GetAll<T>()` on a **disposed** instance throws `ObjectDisposedException`, **not**
	`DataAccessConventionException` — the guard runs before the parent's reflection. On a **live** instance the
	same call throws `DataAccessConventionException`. This pair is the A19 regression guard and fails against
	Revision 3's design.
- [ ] **[X]** A derived Data Access Layer that **overrides** one of the seven and forgets `ThrowIfDisposed()` is
	demonstrated as a hazard, not as conforming behavior — the members are overridable by design and the cost
	of that is worth pinning once.
- [ ] **[C]** `Dispose` with a transaction open **rolls back**; the writes are gone.
- [ ] **[C]** `Dispose` does not throw when the rollback itself fails.
- [ ] **[C]** `ContextOwnership.Owned` → the context is disposed. `ContextOwnership.Borrowed` → **it is not**, and
	remains usable by its owner afterwards.
- [ ] **[C]** A derived `DisposeCore()` that throws does not propagate.
- [ ] **[C]** `ObjectDisposedException` is catchable as `InvalidOperationException`.

### Dispatcher (`Scope=Dispatcher`)

- [ ] **[D]** `GetAll<T>()`, `GetPaged<T>(skip, take)`, `GetCount<T>()` resolve public forwarding methods on the
	concrete DAL; those methods pass `item == null` to the DAO and succeed.
- [ ] **[D]** `Get<T>(object id)` resolves the same identifier property the DAO uses.
- [ ] **[D]** Exceptions from a derived DAL member propagate **unwrapped** —
      `ShouldNotBeOfType<TargetInvocationException>()`.
- [ ] **[D]** `Get<CompanyResource>(id)` throws `DataAccessConventionException`; assert the exception type only,
	  because the missing identifier and missing concrete-DAL `Get` method are both valid failure reasons.

### Provider fidelity (SQL Server leg only)

- [ ] **[C]** Every key-predicate case translates on SQL Server, not only SQLite.
- [ ] **[X]** `DateTime` precision differences between the two do not make a timestamp assertion pass on one and fail
      on the other.
- [ ] **[C]** **`Update` and `Delete` against a row removed by a second connection mid-operation throw
	`DbUpdateConcurrencyException`** (R4-S7). Not authorable on SQLite — see below.
- [ ] **[C]** **Two Data Access Layer instances over the same database do not share a transaction.** Not authorable
	on SQLite — see below.

### String-key collation — a per-provider characterization, with a stated expectation on each leg

**These are `Scope=Characterization` tests, and they assert opposite results on the two legs.** That is the
point: [OD-2](#owner-decisions-taken-during-revision-4) makes key equality the storage engine's collation, so
a test that expected the same answer everywhere would be asserting a promise this library refuses to make.
Revision 3 said only "document the finding whichever way it lands," which is not a test.

Seed a `BaseDao<Country, string>` with a row whose key is `"acme"`, then:

| Assertion | **SQL Server leg** — `SQL_Latin1_General_CP1_CI_AS` | **SQLite leg** — `BINARY` |
|---|---|---|
| `Get(new Country { Id = "ACME" })` | **finds the row** — case-insensitive | **returns `null`** — byte-exact |
| `Get(new Country { Id = "acme " })` (trailing space) | **finds the row** — trailing-space-insensitive | **returns `null`** |
| `Get(new Country { Id = "acme" })` | finds the row | finds the row |
| `Update` / `Delete` on each of the above | **agree with `Get` on the same leg** | **agree with `Get` on the same leg** |

- [ ] **[X]** All four rows, both legs, with the expectation stated per leg rather than shared.
- [ ] **[X]** **A column configured with an explicit collation overrides the default, and the assertion is per leg**
	because the vocabularies do not overlap. On **SQL Server** declare `Latin1_General_BIN2` with
	`.UseCollation(...)` and confirm `"ACME"` **no longer matches**. On **SQLite** declare **`NOCASE`** — the
	only case-folding collation SQLite ships, alongside `BINARY` and `RTRIM` — and confirm `"ACME"` **now
	does**. The two assert **opposite** results, which is the point: each leg is moved off its own default.
	`Latin1_General_BIN2` is meaningless to SQLite and an obligation naming it "on both legs" is unauthorable.
	This is the supported consumer route in
	[String keys and collation](#string-keys-and-collation--od-2), and it is untested otherwise.
- [ ] **[X]** **The trailing-space row is padding, not collation**, so it is asserted separately: on SQL Server
	`"acme "` matches `"acme"` under the **case-sensitive** `Latin1_General_BIN2` too, if the box confirms
	ANSI padding is unaffected by the collation change — see
	[Implementer-Only Questions](#implementer-only-questions). Record what the box says; do not assume it.
- [ ] **[X]** **No `COLLATE` clause appears in the emitted command** for the default predicate — **interceptor route**,
	per [Observing the generated SQL](#observing-the-generated-sql--the-seam). This is the guard on the OD-2
	rejection — an implementer who "fixes" the leg divergence by injecting a collation breaks provider
	neutrality and index seeks at once, and nothing else here would catch it.

### SQLite leg limitations

[D4](purpose-and-scope.md#owner-decisions--2026-08-15) makes SQLite in-memory the fast CI gate, and it cannot
carry every obligation. **This list is the symmetrical counterpart of the SQL Server-only list above**, and
without it a suite quietly passes on the fast leg while asserting nothing.

| Obligation | Why SQLite cannot carry it |
|---|---|
| **"Two Data Access Layer instances over the same database do not share a transaction"** | Not authorable. Two separate `:memory:` connections are **different databases**, so the test passes vacuously — it proves isolation between two stores, not transaction scope. A shared-cache `:memory:` connection string makes them one database, and then SQLite's **single-writer** model yields `SQLITE_BUSY` on the second writer rather than the independent-transaction behavior under test. Neither configuration tests the rule |
| **`DbUpdateConcurrencyException` from a mid-operation delete** (R4-S7) | Needs a second connection writing to the same database while the first holds a read. Same single-writer wall |
| **Any concurrent-writer scenario at all** | SQLite serializes writers. Concurrency obligations belong on the SQL Server leg by construction |
| **Scan-plan-dependent ordering at scale** | The 10,000-row half of that obligation is about SQL Server's optimizer, and SQLite will pass it whatever the implementation does. **The `ORDER BY` half is not a SQLite gap** — an absent `ORDER BY` in the emitted command fails on both legs, which is exactly why the obligation was given a mechanism as well as a number |
| **Collation — the SQL Server half** | `Latin1_General_BIN2` is SQL Server's vocabulary. SQLite's counterpart is `NOCASE`, asserting the opposite result; neither substitutes for the other |
| **Collation defaults** | SQLite's `BINARY` default is one of the two answers under test above, not a substitute for the other |
| **Foreign-key enforcement, unless the arrangement asserts it** (G11) | Not an engine limit — a **configuration** one, and the more dangerous for it. `Microsoft.Data.Sqlite` issues `PRAGMA foreign_keys = 1` on a connection it opens itself, but a context built over a connection the **test** opened, or one whose connection string carries `Foreign Keys=False`, enforces nothing. The OD-7 discard obligation turns entirely on a referential-integrity exception being raised: without enforcement the offending `Insert` **succeeds**, the exception never fires, and the obligation passes while asserting nothing about OD-7. **Read `PRAGMA foreign_keys` back and require `1` in the arrangement.** SQL Server enforces unconditionally, so this is a one-leg trap and exactly the shape the rule below describes |

**Rule for `Test Designer`:** an obligation that cannot fail on SQLite must be **marked**, not merely run
there. A test that executes on both legs and can only fail on one is fine; a test that *appears* to cover the
rule on the fast leg while asserting nothing is not, and "two DAL instances do not share a transaction" is
exactly that trap.

**And the inverse.** A handful of obligations are *only* meaningful on SQLite — chiefly the collation table
above, where `BINARY` is the byte-exact half. Those are not SQL Server gaps; they are the second leg earning
its place.

---

## Reclassified, Deferred and Rejected

`RootSoftNonIdDao<TEntity>` was initially deferred and was reclassified as **Included** by A14. The remaining
items were considered and deliberately left out, with the reason, so they are weighed rather than re-proposed
as unfinished work.

| Item | Verdict |
|---|---|
| **A soft keyless base with no `IBaseDao`** — `RootSoftNonIdDao<TEntity>` | **Included** (A14). It preserves soft-delete reuse without forcing meaningless `Get` and `Update` members onto a genuinely keyless DAO |
| **`public TEntity? Get(TKey id)` convenience overload** | **Deferred.** Not asked for. `IBaseDao<T>.Get(T item)` and `IBaseDataAccess.Get<T>(object id)` already cover both call styles, and a third would be speculative surface |
| **Deleting `BaseEFContext`** | **Rejected** (A6). Its provider-selecting body goes; the type stays so consumers do not re-parent every context for a passthrough |
| **A `bool useUtc` constructor parameter alongside `GetCurrentTimestamp()`** | **Rejected.** Two ways to set one policy, and the flag is the one that cannot express a test clock |
| **`AsNoTrackingWithIdentityResolution()` on reads** | **Rejected.** It hands out a shared instance within a query, which the Example snapshot rule explicitly denies. See [the trap](#the-asnotrackingwithidentityresolution-trap) — it is the substitution most likely to be made in good faith, and no existing Example test would catch it |
| **Cascading `Insert` into the argument's navigation graph** | **Rejected by [OD-4](#owner-decisions-taken-during-revision-5).** Related entities are read, never inserted: `DbContext.Add` would re-insert already-stored rows and overwrite the caller's keys with the duplicates'. See [Writes and the Navigation Graph](#writes-and-the-navigation-graph) |
| **Cascading `Update` into the argument's navigation graph** | **Rejected by [OD-6](#owner-decisions-taken-during-revision-6).** `Update` writes the root's mapped scalars and nothing else. The one `Scope=Contract` assertion that required otherwise encodes a `NoDB` denormalization no normalized relational store can reproduce, and **has been retraited `Characterization` in `ProphetsWay.Example`** |
| **`DbContext.Attach` / `DbSet.Attach` for the reachable-graph walk** | **Rejected** (A24). It marks a **default-keyed** entity `Added` by its own heuristic, which re-introduces the duplicate insert OD-4 forbids against exactly the rows [OD-3](#owner-decisions-taken-during-revision-4) makes legal. The state is assigned explicitly instead, which is why the walk is specified by **state** and not by API |
| **Leaving the change tracker as EF Core left it when `SaveChanges` throws** | **Rejected by [OD-7](#owner-decisions-taken-during-revision-6)**, reversing the rule Revision 5 wrote. A failed write's graph left tracked as `Added` is carried into the next successful `SaveChanges` on the shared context, and recovering would mean disposing and rebuilding the whole Data Access Layer. Detachment runs in a `finally`; the pending insert is discarded, and that is the intent |
| **Repointing a navigation-only relationship through `Update`** | **Not supported** (A25). `SetValues` reads properties by name off the CLR type and a shadow foreign key has none. A Data Access Object that needs it writes a custom method; a consumer who declares the foreign key as a scalar removes the limitation from their own model, which is what the purpose-built `Assignment` in [A25](#update-writes-scalars-and-cannot-repoint-a-relationship--a25) demonstrates ([OD-9](#owner-decisions-taken-during-revision-8)) |
| **Widening the `Update`/`Delete` locating fetch so it loads a graph** | **Rejected by [OD-8](#owner-decisions-taken-during-revision-8)**, which retracted A26's *"plus any fetched row's whole reachable graph"* clause rather than making it true. The fetch applies neither `ApplyReadFilter` nor `ApplyIncludes` and never tracks `item`, so every navigation on the fetched row is `null`; widening it would make every write pay for a graph no write uses, and would contradict the opt-in default [OD-1](#owner-decisions-taken-during-revision-4) settled. A consumer's `AutoInclude` is the one thing that populates it, and that is the consumer's decision |
| **`CompanyResource` as A25's foreign-key counter-example** | **Replaced by [OD-9](#owner-decisions-taken-during-revision-8).** It declares no navigation property, so it could show a scalar being written and nothing about repointing a relationship; both of its mapped scalars sit in its `MatchRow` predicate, so an `Update` changing either locates nothing and returns `0`; and neither `ICompanyResourceDao` nor `RootNonIdDao<TEntity>` publishes `Update` at all. The counter-example is a purpose-built `Assignment` instead — stated as purpose-built, and not present in `ProphetsWay.Example` |
| **Ignoring global query filters on `GetAll`/`GetPaged`/`GetCount`** | **Rejected by [OD-5](#owner-decisions-taken-during-revision-5).** The filter is defeated only where the library locates one row through `MatchRow`. On the trio it composes, because ignoring it there would override a restriction the consumer wrote on purpose — the same objection OD-2 makes to forcing a collation |
| **A split-query policy** | **Out of scope** (A29). The library calls neither `AsSplitQuery()` nor `AsSingleQuery()`; the choice belongs to the `ApplyIncludes` override or to the consumer's context options |
| **Counting the *ordered* query in `GetCount`** | **Rejected** (A27). It would make the emitted SQL depend on the provider's translator stripping an `ORDER BY` it cannot legally keep. The hook is invoked and its result discarded instead |
| **`Dataset.Find(key)` for `Get`** | **Rejected.** Returns a tracked entity and consults the change tracker before the store |
| **A default value for `ownership`** | **Rejected.** Whichever way it pointed, the wrong half of the audience would get a silent double-dispose or a silent leak |
| **Compatibility wrappers for `Int`/`Guid`/`Long`** | **Rejected by [D3](purpose-and-scope.md#owner-decisions--2026-08-15)** (S3). The escape hatch is named in [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) and is not open on present evidence |
| **Async members / `IAsyncDisposable`** | **Out of scope** (S12). `ProphetsWay.BaseDataAccess` owns that decision; this library must not lead |
| **Nested transactions / savepoints** | **Out of scope.** Same reason. `IBaseDataAccess` defers it explicitly |
| **Declarative per-Data-Access-Object include strategy** | **In scope as of Revision 4** — the `ApplyIncludes` hook (OD-1, A18). Formerly listed here as out of scope, which was the defect [Navigation Loading](#navigation-loading) exists to correct |
| **Ad-hoc filtered includes, specifications, runtime include-path builders, projection DSLs** | **Out of scope.** `Include(u => u.Orders.Where(...))` and anything that composes an include path at call time is the "1%" the purpose sentence leaves to consumer DAO methods |
| **Forcing a collation into the key predicate** | **Rejected by [OD-2](#owner-decisions-taken-during-revision-4).** Provider-specific SQL in a provider-neutral package, non-sargable, and it overrides a decision the consumer made in their schema. See [String keys and collation](#string-keys-and-collation--od-2) |
| **`ExecuteUpdate` as the `Update` mechanism** | **Rejected** (A22). It returns rows-modified on SQLite and rows-matched on SQL Server, so it cannot satisfy the "row existed → `1`" rule on both certified legs |
| **A `default(TKey)` short-circuit** | **Rejected by [OD-3](#owner-decisions-taken-during-revision-4).** `0`, `Guid.Empty` and `""` are legal stored values; special-casing them would make such rows unreachable through `Get` while visible through `GetAll` |
| **Emitting SQL `ORDER BY` guidance per provider** | **Out of scope.** `ApplyStableOrder` is the seam; provider tuning is the consumer's |

---

## Implementer-Only Questions

None of these needs the owner. Each is answerable with a keyboard and a test run, and each is small enough
that guessing wrong is cheap to correct. Record the answers here when they are settled.

1. **Closure carrier for the key parameter.** A one-field private class, a captured local in a helper method,
   or `Expression.Convert` over a boxed constant — whichever produces a parameterized query on **both**
   certified providers. Verify by inspecting generated SQL, not by inspecting the expression tree.
2. **Where the resolved identifier `PropertyInfo` is cached.** A `static readonly` on the closed generic
   type is the obvious answer; confirm it does not resurrect a startup cost per DAO instance.
3. **How `Update` distinguishes "row absent" from "row present, nothing changed."** The mechanism is fixed —
   tracked fetch plus `SetValues` (A22) — but whether the answer comes from the fetch's own result, from
   `ChangeTracker` state, or from a separate existence check is open. The observable contract is not: row
   found → `1`, row absent → `0`, regardless of what `SaveChanges()` returns.
4. **SQLite `DateTime` storage precision** versus SQL Server `datetime2`, and whether a timestamp equality
   assertion needs a tolerance on one leg. If it does, the tolerance belongs in the test, not in the library.
5. **Whether `RootNonIdDao` should be `[EditorBrowsable(Never)]`.** It should not — it is now a first-class
   extension point — but confirm that showing it does not confuse IntelliSense next to `BaseNonIdDao`.
6. **Where the shared timestamp helper lives.** The Timestamp Pair Rule requires one `internal static` copy
   of both defaults and the post-materialization walk, called from both soft branches (R4-S2). Its name and file
   are the implementer's; its singleness is not.
7. **How the reachable-graph walk is written** (A24, A26). Whether it uses `ChangeTracker.TrackGraph`, a
   hand-rolled traversal over `Context.Model`'s navigations, or `Entry(item).Navigations`, is open. What is
   not: it visits by **reference identity**, it attaches every non-root node `Unchanged`, it runs **before**
   the root is marked `Added`, it runs in a **`finally`** on the detachment side (A26, OD-7), and the same
   walk defines what detachment covers.
   **One constraint, not a choice:** the `Unchanged` state is **assigned explicitly** —
   `entry.State = EntityState.Unchanged`. Neither `Attach` nor `TrackGraph`'s default callback may be relied
   on to produce it, because both mark a **default-keyed** entity `Added`, and OD-3 makes `0`, `Guid.Empty`
   and `""` legal stored key values. Whichever traversal is chosen, the state is set by this library and not
   inferred by EF Core.
8. **Whether a binary collation changes SQL Server's trailing-blank behavior on `=`.** The document
   attributes trailing-blank equality to ANSI padding rather than to collation, and that attribution is what
   the collation obligation is written against. Confirm on the box which of `Latin1_General_BIN2` and the
   default agree about `'acme' = 'acme '`, and record the answer here — the *rejection* of forced collations
   does not depend on it, but the test's expectation does.

### Questions removed in Revision 4, and why

Recorded rather than deleted silently, so nobody re-opens them as unfinished work.

| Former question | Disposition |
|---|---|
| **`SetValues` versus `ExecuteUpdate`** | **Closed as A22**, not answered by an implementer. It was never open: `ExecuteUpdate` returns rows-modified on SQLite and rows-matched on SQL Server, so it cannot deliver the "row existed → `1`" rule on both certified legs. See [`Update`](#updatetentity-item). The performance observation survives as the perf note there |
| **Whether `ApplyReadFilter` and `ApplyStableOrder` compose in a fixed order** | **Closed as A20.** The signatures already forced it — `ApplyStableOrder` returns `IOrderedQueryable<TEntity>`, `ApplyReadFilter` takes and returns `IQueryable<TEntity>`, so order-then-filter discards the ordered-ness. The question was unwritten, not open. See [How the read hooks compose](#how-the-read-hooks-compose--a20-a23) |
| **`string` key collation** | **Closed as [OD-2](#owner-decisions-taken-during-revision-4)** by the owner, because it is a term a consumer reads rather than an implementation detail. Key equality is the storage engine's collation; forcing one is rejected. The test suite now asserts a **stated expectation per leg** rather than "documenting the finding." See [String keys and collation](#string-keys-and-collation--od-2) |

---

## Questions This Document Closes

Recorded for `Purpose Refiner`. **No status in [purpose-and-scope.md](purpose-and-scope.md) or
[feature-requests.md](feature-requests.md) was changed by this document** — those files are that agent's, and
the entries below are reported, not edited.

| Question | Where it is open | Answer |
|---|---|---|
| **Q2** — Does v3.0.0 add a `DbContext`-accepting constructor, or only prepare for one? | [purpose-and-scope.md](purpose-and-scope.md#unresolved-purpose-level-questions), [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | **Yes, and it is now the *only* way in.** `BaseEFDataAccess<TContext>` takes a configured context plus explicit `ContextOwnership` (S7, S8). The purpose sentence widens from "constructs your context for you" to "participates in your composition root" — which is what D2 made inevitable once the consumer configures the provider |
| **FR 3, question 2** — Should `ObjectDisposedException` guarding live here, or in the parent's dispatcher? | [FR 3](feature-requests.md#3--implement-the-3x-disposal-contract-in-baseefdataaccess) | **Here — and for the seven `IBaseDataAccess` members, in the library rather than in the consumer.** The parent holds no state and cannot guard. `BaseEFDataAccess<TContext>` overrides all seven inherited dispatcher members with a `ThrowIfDisposed()` guard (A19); `ThrowIfDisposed()` remains `protected` and the derived Data Access Layer is contractually required to call it from its own custom members and forwarders (A5, as narrowed) |
| **Q3** — Do the collapsed generic families keep `where TKey : struct`? | [purpose-and-scope.md](purpose-and-scope.md#unresolved-purpose-level-questions), [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) | **No — the constraint is dropped** (S4). `string` and nullable-value keys are supported through the expression-built predicate. This also closes the accepting-half gap [Example FR 1](../../ProphetsWay.Example/docs/feature-requests.md) records |
| **FR 10** — "Decide the fate of `where TIdType : struct`" | [FR 10](feature-requests.md#10--collapse-the-guidintlong-dao-triplication) | Same as Q3. The "must preserve" condition about a translatable `Get` predicate is answered by [The Key Equality Predicate](#the-key-equality-predicate), which does **not** use `EqualityComparer<TKey>.Default` |
| **[D2](purpose-and-scope.md#owner-decisions--2026-08-15)'s sub-question** — is `BaseEFContext(string)` deleted or retained with provider config moved to `OnConfiguring`? | [purpose-and-scope.md](purpose-and-scope.md#the-one-sub-question-the-decision-does-not-answer) | **Deleted.** `BaseEFContext` keeps only its `DbContextOptions` constructor (A6). The consumer names the provider where they build the options |

**No question in this design is waiting on the owner.** Nine were put to the owner and answered — navigation
loading, string-key collation and `default(TKey)` during Revision 4
([OD-1–OD-3](#owner-decisions-taken-during-revision-4)), the `Insert` graph policy and global query filters
during Revision 5 ([OD-4–OD-5](#owner-decisions-taken-during-revision-5)), the `Update` cascade question
plus the detachment-on-failure rule during Revision 6
([OD-6–OD-7](#owner-decisions-taken-during-revision-6)), and the fetched-graph retraction plus the
replacement of A25's counter-example during Revision 8
([OD-8–OD-9](#owner-decisions-taken-during-revision-8)). **All nine are implemented above.** The sixth —
[does `Update` cascade into the navigation graph?](#the-update-cascade-question--resolved-by-od-6) — was open
when Revision 5 was written and is now closed in favor of the design; the fix it implies lands in
`ProphetsWay.Example`, and **nothing in that repository was edited or proposed by this document.**

**That is not the same as this document being finished.** It is Revision 8 and its status is *under review*.
Revision 7 passed `Contract Reviewer` **with findings**, and Revision 8 closes them — but **no pass has run
against this text**, and Revision 8 claims nothing on its own account. Revision 3 asserted its own passage
and was wrong to. Whoever reads this next should check the [Revision Log](#revision-log) against the findings
it claims to close — they are keyed **H1–H9**, **G11** and **G12** for exactly that purpose — rather than
taking the claim.

### For `Purpose Refiner`

Three items in this revision are reported for triage and **were not written into**
[feature-requests.md](feature-requests.md), which is that agent's file:

1. **The two 2.2.0 defects** in [Two 2.2.0 Defects This Design Retires](#two-220-defects-this-design-retires)
   are live in the published package. They are fixed by work already scheduled, so they may not need entries
   of their own — but the leaked `DbContext` in particular is a defect a consumer could be experiencing
   today, and whether 2.2.x gets a patch is a decision this document cannot make.
2. **Whether an implementation may widen the parent's exception vocabulary** — raised by A15's
   `NotSupportedException` reaching a dispatcher entry point (R4-S6). It is accepted here for this library; it is
   a family-level question that belongs with
   [BaseDataAccess FR 1](../../ProphetsWay.BaseDataAccess/docs/feature-requests.md), the conformance-suite
   request.
3. **A `Scope=Contract` assertion in `ProphetsWay.Example` that no relational Data Access Layer can satisfy —
   decided, and now fixed.**
   `SnapshotDeepCopyTests.Setup_UpdateNavigationInsideTransaction_TestRollBackRestoresIt` required an edit made
   through `user.Company` and submitted via `Update(user)` to be readable back — which
   `ProphetsWay.Example.DataAccess.NoDB` delivers by storing the user's copy of the company **inside the user
   row**, a shape a foreign key cannot reproduce. **[OD-6](#owner-decisions-taken-during-revision-6) resolved
   it**: the assertion belongs to the in-memory implementation rather than to the contract, and the owner
   authorized its retrait to `Characterization` **in `ProphetsWay.Example`**. **That retrait has landed**, along
   with a second one in `UserDaoTests.cs`; the suite is **164 tests — Contract 139, Characterization 5,
   Dispatcher 20 — green on both legs**. One consequence remains for this agent: `ProphetsWay.Example`'s claim
   that the same suite passes against both Data Access Layers is **materially affected** and may want an entry.
   **Nothing in that repository was edited or proposed by this document.** See
   [The `Update` cascade question](#the-update-cascade-question--resolved-by-od-6).
