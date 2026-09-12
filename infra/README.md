# EFTools Azure SQL Test Fixture

This folder contains the owner-authored Bicep for a small, persistent Azure SQL fixture used for the EFTools SQL-backed tests. The owner deployed the initial fixture manually on 2026-09-06 and reported a passing filtered trial on 2026-09-08; deployment remains an owner action under D-033.

## Current Fixture

The recorded initial successful subscription deployment is `deploy-sql-manually-ggn8` in `westus`. These are historical reported observations, not a fresh live inventory:

- Resource group: `example-test-rg`
- SQL server: `example-test-sql`, `Ready`, with public network access enabled
- Database: `ProphetsWay.Example`, `Online`, Basic tier, 5 DTUs, 2 GiB maximum size
- SQL administrator: dedicated Entra security group `example-test-sql-admins`, mail disabled
- Firewall: one rule, limited to one exact client address

No administrator object ID, member identity, or client IP is documented here.

The database/module portion of [example.solution.bicep](example.solution.bicep) now declares **three databases on the same SQL server**: fixed `ProphetsWay.Example` plus the two dedicated reusable scratch databases `EFToolsScratch_1` and `EFToolsScratch_2`. All three declarations use Basic tier, 5 DTUs, and a 2 GiB maximum size. This is source-observed configuration; the current live inventory and deployed settings have not been independently verified.

Each persistent Basic database incurs ongoing charges while provisioned, including while empty or idle. The two scratch databases add paid capacity; current pricing, total spend, and the spending cap are unverified. No free-SKU or subscription-eligibility claim is made.

## Files

- `bicepconfig.json` enables the `microsoftGraphV1` Bicep extension used to create the Entra group.
- `group.bicep` creates the dedicated security-enabled, non-mail-enabled SQL administrator group and appends the configured user as a member. Its `groupName` output is the group's `uniqueName`, which the entry point supplies as the SQL administrator login. The current display and unique names coincide, but callers must not assume that is a display-name contract.
- `example.solution.bicep` is a subscription-scoped entry point. It creates the resource group, calls `group.bicep`, and uses pinned Azure Verified Modules to create the SQL server, three Basic databases, and exact-address firewall rule. The existing database loop uses `br/public:avm/res/sql/server/database:0.3.0`, read from source; this refresh did not restore, build, or independently resolve that module.

## Build And Deploy

From this directory, restore and compile the entry point:

```powershell
bicep restore .\example.solution.bicep
bicep build .\example.solution.bicep --stdout
```

After selecting the intended Azure subscription, the owner can manually deploy the current fixture shape:

```powershell
az deployment sub create --location westus --template-file .\example.solution.bicep --name deploy-sql-manually-<unique-suffix>
```

The template uses stable resource names, so an incremental redeployment targets the same resource group, server, named databases, and firewall rule. Its nested deployment names include a value generated with `utcNow()`, so they change on each deployment. Group membership uses append semantics: it adds the configured member but does not remove any existing members. Any owner-authorized preview or redeployment must consider the whole subscription-scoped template, including identity and firewall configuration, not just the two scratch additions. No preview or deployment was run for this documentation refresh.

## Caveats And Next Step

[example.solution.bicep](example.solution.bicep) declares `userObjectId` and `userFirewallIpAddress` as required owner-supplied strings with no defaults. The prior owner-specific defaults were removed; do not copy private values into documentation, parameter files, or command history.

The owner prefers removal of the prior values from history but accepts leaving prior history intact. That settles the narrow defaults/history question, not whole-history sanitization, blanket publication approval, or independent infrastructure clearance. Exact subscription and tenant identity, effective deployment parameters, current live inventory, and costs were not independently checked; this factual README update supplies no missing deployment identity or gate approval.

The owner reports that the fixed Example database already has the DACPAC and synthetic seed data; **no automatic DACPAC republication is pending**. The two scratch databases must be dedicated, initially empty reusable stores with no foreign data or user objects, and **do not receive the Example DACPAC**. Follow the existing [Azure SQL test execution contract](../docs/azure-sql-test-execution.md) for protected configuration and lifecycle boundaries: reusable tests clear all user foreign keys and tables in a leased scratch database before use and on release, then create the current test model. They do not create or drop the scratch databases. Fixed Example is never scratch-reset, although adapted tests may write synthetic rows there. Use synthetic test data only and **one test process per scratch pair**; the in-process lease pool does not coordinate separate processes.

The [2026-09-08 filtered trial](../docs/azure-sql-test-execution.md#filtered-trial-result-2026-09-08) used `Execution!=LocalPhysicalLifecycle`. The parent verified the retained TRX: **369 included executions, all passing, zero failed or skipped**, including all 14 alternate-key comparison cases; the six local physical-lifecycle cases are absent and the exact-six classification guard passed. Separate local 6/6 evidence is not added to that count. Azure targeting and Example preparation are owner-reported context; the TRX proves recorded outcomes and identities, not live inventory, authentication, or deployment state. **Gate 2, FR 19, and formal Azure SQL certification remain pending the owner's final review.** This update authorizes no resource, database, pipeline, or release operation.
