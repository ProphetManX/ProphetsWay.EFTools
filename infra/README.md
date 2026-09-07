# EFTools Azure SQL Test Fixture

This folder contains the owner-authored Bicep for a small, persistent Azure SQL fixture used to prepare and eventually run the EFTools SQL-backed tests. The owner deployed it manually on 2026-09-06; deployment remains an owner action under D-033.

## Current Fixture

The successful subscription deployment is `deploy-sql-manually-ggn8` in `westus`.

- Resource group: `example-test-rg`
- SQL server: `example-test-sql`, `Ready`, with public network access enabled
- Database: `ProphetsWay.Example`, `Online`, Basic tier, 5 DTUs, 2 GiB maximum size
- SQL administrator: dedicated Entra security group `example-test-sql-admins`, mail disabled
- Firewall: one rule, limited to one exact client address

No administrator object ID, member identity, or client IP is documented here.

## Files

- `bicepconfig.json` enables the `microsoftGraphV1` Bicep extension used to create the Entra group.
- `group.bicep` creates the dedicated security-enabled, non-mail-enabled SQL administrator group and appends the configured user as a member. Its `groupName` output is the group's `uniqueName`, which the entry point supplies as the SQL administrator login. The current display and unique names coincide, but callers must not assume that is a display-name contract.
- `example.solution.bicep` is a subscription-scoped entry point. It creates the resource group, calls `group.bicep`, and uses pinned Azure Verified Modules to create the SQL server, Basic database, and exact-address firewall rule.

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

The template uses stable resource names, so an incremental redeployment updates the same resource group, server, database, and firewall rule. Its nested deployment names include a value generated with `utcNow()`, so they change on each deployment. Group membership uses append semantics: it adds the configured member but does not remove any existing members.

## Caveats And Next Step

`example.solution.bicep` currently contains default values for a user object ID and a public IP address. They are suitable only for this manual fixture and must be parameterized before reuse or publication; do not copy those values into documentation, parameter files, or command history.

This deployment proves only that the Azure resources exist. It does not prove Gate 2, apply the DACPAC, configure a test connection or authentication, grant database access, or run Azure tests. The next work is to supply the protected connection and authentication inputs, prepare and manually authorize DACPAC publication, then configure and execute the SQL-backed test run.
