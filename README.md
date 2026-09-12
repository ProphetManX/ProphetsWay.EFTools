# ProphetsWay.EFTools

**Write your Data Access Layer as declarations.** EFTools is a set of abstract Entity Framework Core base
classes that implement the CRUD, paging, soft-delete, ordering, transaction, ownership and disposal plumbing
behind a [`ProphetsWay.BaseDataAccess`](https://github.com/ProphetManX/ProphetsWay.BaseDataAccess) Data
Access Layer — so the only code you write is the code your own contract adds.

[![Build Status](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_apis/build/status/ProphetManX.ProphetsWay.EFTools?repoName=ProphetManX%2FProphetsWay.EFTools&branchName=main)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_build/latest?definitionId=22&repoName=ProphetManX%2FProphetsWay.EFTools&branchName=main)
[![NuGet](https://img.shields.io/nuget/v/ProphetsWay.EFTools)](https://www.nuget.org/packages/ProphetsWay.EFTools)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

> ### Which version this document describes
>
> **This README documents the 3.0.0 line, which is not released.** There is no 3.0.0 package on nuget.org
> and you cannot install it today. The published package is **2.2.0** — a different library, built on
> Entity Framework 6 *and* EF Core, targeting `net461` through `net90`, with a class surface 3.0.0 removes.
> Nothing below applies to it. If you are on 2.2.x, read [CHANGELOG.md](CHANGELOG.md) before you plan an
> upgrade; the migration is real work and there are no compatibility shims.
>
> The 2.2.x classes are **gone from the source**, not deprecated in place. See
> [Names removed in 3.0.0](#names-removed-in-300) for what they were and what took over.

---

## Why EFTools

Write an Entity Framework Data Access Layer by hand and you write the same forty lines per entity, seven
times. Read untracked; order stably or your paging silently overlaps; copy the argument so a rollback can
actually reverse an update; stamp created, updated and deleted dates; detach in a `finally` so a failed write
does not flush on the next `SaveChanges`; translate the identifier lookup into a *parameterized* predicate
rather than a SQL literal. Every one of those is easy to get subtly wrong, and none of them is your domain.

EFTools already wrote them. You pick the base class matching what your Data Access Object actually promises,
pass it the context, and stop.

Here is a complete Data Access Object from the reference implementation in this repository:

```c#
internal class JobDao : BaseGetAllDao<Job, int>, IJobDao
{
	public JobDao(DbContext context) : base(context) { }
}
```

That class has working `Get`, `Insert`, `Update`, `Delete` and `GetAll` — untracked reads, copy-on-write,
stable ordering, parameterized key lookup — and one line of body.

**Highlights**

- **A Data Access Object costs a constructor.** Of the seven in this repository's proving ground, one writes
  nothing beyond it, four write exactly one member of their own, and none writes more than three.
- **Soft delete is free, and it is not bypassable.** `BaseSoftDao` expresses every difference as an
  `override`, never a `new`, so a soft Data Access Object reached through a `BaseDao`-typed reference still
  soft-deletes instead of destroying the row.
- **Keys are not restricted to `int` / `long` / `Guid`.** `TKey` carries no constraint at all, so a `string`
  key or an `int?` key is legal.
- **Keyless entities have a real home.** Join tables and composite natural keys get a family whose `Root*`
  members implement no capability interface — deriving from one commits you to publishing nothing.
- **Disposal and transactions are specified, not improvised.** `Dispose` is sealed, idempotent, never
  throws, and rolls an open transaction *back*; every other member throws `ObjectDisposedException`
  afterward; the caller states who owns the context.
- **The library names no database provider.** Nothing in its C# calls `UseSqlServer`. You build the options;
  you pick the provider. One caveat, stated honestly:
  [provider neutrality](#provider-neutrality--done-in-code-not-yet-in-packaging).

---

## Install

**3.0.0 is unreleased.** To use the library described here, build it from source — see
[Building & Testing Locally](#building--testing-locally).

The package that exists on nuget.org today is the **2.2.x** line, which this document does *not* describe:

```
dotnet add package ProphetsWay.EFTools --version 2.2.0
```

```
Install-Package ProphetsWay.EFTools -Version 2.2.0
```

### What 3.0.0 will target

| | |
|---|---|
| **Target framework** | `net10.0` — and only `net10.0` |
| **Entity Framework** | Core **10.0.11**. Entity Framework 6 is not supported |
| **Contracts** | `ProphetsWay.BaseDataAccess` **3.2.0** |

The single target is deliberate rather than an oversight — see
[Architecture & Design Decisions](#architecture--design-decisions). If you are on .NET Framework, .NET 8 or
.NET 9, stay on the 2.2.x line.

---

## Quick Start

Three files: a context, a Data Access Object, and a Data Access Layer that hands them the context.

### 1 — Derive your context

`BaseEFContext` selects no provider and adds no members. Whoever builds the options names the database.

```c#
using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;

public class ExampleContext : BaseEFContext
{
	public ExampleContext(DbContextOptions<ExampleContext> options) : base(options) { }

	public DbSet<Company> Companies { get; set; }
	public DbSet<Job> Jobs { get; set; }
	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
		modelBuilder.Entity<User>().HasOne(x => x.Job).WithMany().HasForeignKey("JobId");

		modelBuilder.Entity<Company>().ToTable("Companies");
		modelBuilder.Entity<Job>().ToTable("Jobs");
		modelBuilder.Entity<User>().ToTable("Users");
	}
}
```

Deriving from `BaseEFContext` is **optional**. `BaseEFDataAccess<TContext>` constrains its context to
`DbContext`, so a context you already have — one registered in a dependency-injection container, for
instance — needs no re-parenting.

### 2 — Derive each Data Access Object from the family matching its capability

```c#
using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;

internal class UserDao : BaseDao<User, int>, IUserDao
{
	public UserDao(DbContext context) : base(context) { }

	public void CustomUserFunctionality(User user)
	{
		user.Whatever = "custom functionality triggered";
		Update(user);
	}
}
```

### 3 — Derive your Data Access Layer, and say who owns the context

```c#
using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;

public class ExampleDataAccess : BaseEFDataAccess<ExampleContext>, IExampleDataAccess
{
	private readonly IUserDao _userDao;

	//The provider is named here, in your own file. A PostgreSQL consumer writes UseNpgsql instead.
	public ExampleDataAccess(string connectionString)
		: this(new ExampleContext(new DbContextOptionsBuilder<ExampleContext>()
			.UseSqlServer(connectionString)
			.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
			.Options), ContextOwnership.Owned)
	{ }

	//A context someone else built and will dispose - a DI container, or a test with its own database.
	public ExampleDataAccess(ExampleContext context)
		: this(context, ContextOwnership.Borrowed)
	{ }

	private ExampleDataAccess(ExampleContext context, ContextOwnership ownership)
		: base(context, ownership)
	{
		_userDao = new UserDao(Context);
	}

	public User Get(User item)
	{
		ThrowIfDisposed();
		return _userDao.Get(item);
	}

	public void Insert(User item)
	{
		ThrowIfDisposed();
		_userDao.Insert(item);
	}

	public int Update(User item)
	{
		ThrowIfDisposed();
		return _userDao.Update(item);
	}

	public int Delete(User item)
	{
		ThrowIfDisposed();
		return _userDao.Delete(item);
	}
}
```

### What you get

Every CRUD call on `User` works. Reads come back as untracked snapshots. Writes read your argument rather
than adopting it. `Insert` assigns the store-generated identifier back onto your instance once the row has
committed. The layer disposes the context it built, and refuses every call once disposed.

```c#
using (var dal = new ExampleDataAccess(connectionString))
{
	dal.Insert(user);                    //user.Id now holds the stored identifier
	var fetched = dal.Get<User>(user.Id);
}
```

`Get<T>(id)` comes from `ProphetsWay.BaseDataAccess`'s reflection dispatcher, which `BaseEFDataAccess`
derives from and overrides. No type parameter on your layer confers it.

---

## Core Concepts

### The entity is the argument, and that is the point

Every CRUD method takes the entity as its argument — `Get(User item)`, not `GetUser(int id)`. It looks
redundant until you see what it buys: your Data Access Layer publishes **one method name per operation**, and
the *argument type* selects which Data Access Object handles it. `Insert(company)` and `Insert(user)` are the
same name on the same interface, and nothing in your business layer names a table, a Data Access Object or a
store.

For `Get` and `Delete`, only the identifier property of the instance you pass needs a value.

`Get` does **not** promise to return the instance you passed. Use the return value — it is a fresh untracked
snapshot, never your argument and never the store's own tracked object.

### `Base*` publishes; `Root*` does not

The prefix is load-bearing in this library, and the two are not interchangeable:

- **`Base*`** implements at least one `ProphetsWay.BaseDataAccess` capability interface. Deriving from it
  means your Data Access Object answers that interface, whether or not your own contract mentions it.
- **`Root*`** is plumbing that implements **no** capability interface. Deriving from it commits you to
  nothing; your own interface declares exactly the subset you support.

That distinction is why a join table's Data Access Object can refuse to publish `Get` and have the refusal
hold structurally rather than by convention.

### The key is resolved by name, and its type is open

`BaseDao<TEntity, TKey>` finds the identifier property by name — **`{TypeName}Id` first, then `Id`** — and
validates it in the constructor, so a mis-wired entity fails when you build the layer rather than on first
use. The property must be a **public instance** property of type `TKey` carrying a set accessor of any
visibility.

> An *explicit* `IBaseIdEntity<T>` implementation does **not** satisfy this. It compiles to a non-public,
> interface-qualified property that neither lookup finds, and the constructor throws
> `DataAccessConventionException`.

`TKey` carries no constraint, so `string` and `int?` are legal keys alongside `int`, `long` and `Guid`.

### The layer owns the context, the transaction and disposal

Data Access Objects have no transaction members in 3.0.0. `TransactionStart`, `TransactionCommit` and
`TransactionRollBack` live on `BaseEFDataAccess<TContext>`, because a transaction in EF Core lives on the
context and every Data Access Object on a layer shares one.

- One transaction per instance, and **they do not nest** — a second `TransactionStart` throws
  `InvalidOperationException`.
- A **failed commit leaves nothing open and discards its writes.** Do not follow one with
  `TransactionRollBack`; there is nothing left to roll back and that call throws.
- `TransactionStart` never inspects `Context.Database.CurrentTransaction`, so it can never check-and-skip.
  On a `Borrowed` context whose owner already began a transaction directly, EF Core's own exception
  propagates unwrapped and that outer transaction is left in force.

Disposal is a specified contract, not a convenience. `Dispose` is `sealed`, idempotent and never throws; it
rolls an open transaction **back**; it runs your `DisposeCore()` hook; and it disposes the context **only**
under `ContextOwnership.Owned`. All ten other members throw `ObjectDisposedException` once it has run.

> **One obligation is yours.** Nothing intercepts calls to members *your* layer declares. Call the
> `protected ThrowIfDisposed()` as the first statement of every per-entity forwarder and every custom method
> you write — as the Quick Start layer above does — or a disposed `Borrowed` layer will silently serve them
> from a context that is still alive.

### Ownership is stated, never defaulted

```c#
public enum ContextOwnership { Borrowed = 0, Owned = 1 }
```

There is no default to fall into, and the constructor rejects anything that is neither value with
`ArgumentOutOfRangeException`. A defaulted ownership is the shape that produces a silent double-dispose in a
dependency-injection host and a silent leak in a hand-built one, depending only on which way the default
happened to point.

---

## API Reference

**Thirteen public declarations — twelve abstract classes and one enum.** All in namespace
`ProphetsWay.EFTools`, with no sub-namespaces.

### The context and the layer

| Type | Member | Purpose |
|---|---|---|
| `BaseEFContext : DbContext` | `protected BaseEFContext(DbContextOptions)` | Its only member. Optional base for your context; selects no provider |
| `BaseEFDataAccess<TContext>` | `protected BaseEFDataAccess(TContext, ContextOwnership)` | Its only constructor. Captures a built context and who disposes it — no query, no connection, no transaction |
| | `protected TContext Context` | The context every Data Access Object on the layer shares, typed so you construct them without a cast |
| | `protected ContextOwnership Ownership` | What you told it |
| | `protected bool IsDisposed` | `true` from the first statement of `Dispose` onward |
| | `protected void ThrowIfDisposed()` | Call this first in every member you declare |
| | `public override void TransactionStart()` | Begins; throws if one is already open |
| | `public override void TransactionCommit()` | Commits and clears state; a failure discards the writes |
| | `public override void TransactionRollBack()` | Rolls back and clears state |
| | `public sealed override void Dispose()` | Idempotent, non-throwing; rolls back; disposes the context only when `Owned` |
| | `protected virtual void DisposeCore()` | Your hook. Must not throw |
| | `Get<T>(object id)`, `GetAll<T>()`, `GetPaged<T>(int, int)`, `GetCount<T>()`, `Insert<T>`, `Update<T>`, `Delete<T>` | Dispatcher overrides inherited from `ProphetsWay.BaseDataAccess`, each guarded |
| `ContextOwnership` *(enum)* | `Borrowed = 0`, `Owned = 1` | No default value |

`BaseEFDataAccess<TContext>` is declared
`: BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContext : DbContext`.

### The keyed families — `where TEntity : class, IBaseIdEntity<TKey>`

| Type | Base | Adds |
|---|---|---|
| `BaseDao<TEntity, TKey>` | — | `IBaseDao<TEntity>` — `Get`, `Insert`, `Update`, `Delete` |
| `BaseGetAllDao<TEntity, TKey>` | `BaseDao` | `IBaseGetAllDao<TEntity>` — publishes `GetAll` |
| `BasePagedDao<TEntity, TKey>` | `BaseDao` | `IBasePagedDao<TEntity>` — publishes `GetPaged` and `GetCount` |

Each takes `protected BaseXxxDao(DbContext context)`. `BaseDao<TEntity, TKey>` carries:

| Kind | Members |
|---|---|
| Public | `TEntity? Get(TEntity item)`, `void Insert(TEntity item)`, `int Update(TEntity item)`, `int Delete(TEntity item)`, `IList<TEntity> GetAll(TEntity? item)`, `IList<TEntity> GetPaged(TEntity? item, int skip, int take)`, `int GetCount(TEntity? item)` |
| Protected state | `DbContext Context`, `DbSet<TEntity> Dataset` |
| Protected seams | `TrackForWrite`, `ApplyUpdateValues`, `GetKey`, `MatchRow`, `KeyEquals`, `KeySelector`, `ApplyReadFilter`, `ApplyIncludes`, `ApplyStableOrder` |

> The `item` argument on `GetAll`, `GetPaged` and `GetCount` is a **type selector only**, and it is `null`
> when the call arrives through the dispatcher. Never read it.

> **`Get` is annotated `TEntity?`, and "not found" is what that expresses.** The whole library is compiled
> under `#nullable enable`, and `ProphetsWay.BaseDataAccess` 3.2.0 annotates `IBaseDao<T>` to match — so a
> call through the interface now binds `Company?` where it used to bind an unannotated `Company`. If you
> compile with nullable warnings on, expect a `CS8602` at any site that dereferences a `Get` result without
> checking it. It is a **new warning, not a break**: the change is source- and binary-compatible, and the
> warning is pointing at a null you could always have received.

### The keyed soft-delete families — `where TEntity : class, IBaseSoftIdEntity<TKey>`

| Type | Base | Adds |
|---|---|---|
| `BaseSoftDao<TEntity, TKey>` | `BaseDao<TEntity, TKey>` | Soft `Insert` / `Update` / `Delete`, the created, updated and deleted timestamps, and a `DeletedDate == null` read filter |
| `BaseSoftGetAllDao<TEntity, TKey>` | `BaseSoftDao` | `IBaseGetAllDao<TEntity>` |
| `BaseSoftPagedDao<TEntity, TKey>` | `BaseSoftDao` | `IBasePagedDao<TEntity>` |

Two timestamp hooks are overridable: `protected virtual DateTime GetCurrentTimestamp()` and
`protected virtual DateTime NormalizeRetrievedTimestamp(DateTime value)` — the latter restores the
`DateTimeKind` a relational provider does not persist.

`ApplyReadFilter` adds `DeletedDate == null` **and nothing else**, so `GetAll`, `GetPaged` and `GetCount`
agree with one another while the unfiltered `Get` still finds a deleted row.

### The keyless families

| Type | Base | Implements | For |
|---|---|---|---|
| `RootNonIdDao<TEntity>` | — | **nothing** | `TEntity : class, IBaseEntity`. Plumbing that commits you to no interface |
| `BaseNonIdDao<TEntity>` | `RootNonIdDao` | `IBaseDao<TEntity>` | The same, when you *do* want the full shape. Adds virtual `Get` and `Update` |
| `RootSoftNonIdDao<TEntity>` | `RootNonIdDao` | **nothing** | `TEntity : class, IBaseSoftEntity`. Keyless soft delete |
| `BaseSoftNonIdDao<TEntity>` | `RootSoftNonIdDao` | `IBaseDao<TEntity>` | Keyless soft delete with the full shape |

`RootNonIdDao<TEntity>` publishes `Insert`, `Delete`, `GetAll`, `GetPaged` and `GetCount`, and exposes
`GetCore`, `UpdateCore`, `TrackForWrite`, `ApplyStableOrder`, `ApplyReadFilter`, `ApplyIncludes` and
`ApplyUpdateValues` as protected seams. Two members deserve attention:

- **`protected abstract Expression<Func<TEntity, bool>> MatchRow(TEntity item)`** — the predicate
  identifying a single row. It replaces the entire identifier apparatus, and it is **the only abstract
  member in the library**. It is translated to SQL, not evaluated in memory.
- **`ApplyStableOrder` throws `NotSupportedException` until you override it.** `GetCore` and `UpdateCore`
  order nothing and are unaffected, so a write-only join Data Access Object is fully functional with
  `MatchRow` alone.

---

## Common Scenarios

Every sample below is taken from `ProphetsWay.Example.DataAccess.EF`, the working implementation in this
repository.

### A paged Data Access Object with one method of its own

`protected DbSet<TEntity> Dataset` is available on the keyed and keyless families alike, so a custom method
composes over it directly.

```c#
using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;
using System.Linq;

internal class CompanyDao : BasePagedDao<Company, int>, ICompanyDao
{
	public CompanyDao(DbContext context) : base(context) { }

	public Company GetCustomCompanyFunction(int id)
	{
		return Dataset.OrderBy(x => x.Id).Skip(id % GetCount(null)).First();
	}
}
```

### Soft delete, and reaching a row the read filter hides

`DepartmentDao` derives from `BaseSoftPagedDao<Department, int>` and satisfies eighteen of its nineteen
contract rules without overriding anything. The nineteenth — `Restore` — belongs to its own interface and
cannot be expressed through `Update`, which deliberately preserves the stored `DeletedDate`.

`TrackForWrite` is the seam that makes it possible: it starts from the raw `Dataset`, which is what keeps
`ApplyReadFilter` off the query. That filter excludes exactly the rows `Restore` exists to reach, so a
`Restore` composed on top of it would return `0` for every department it was ever asked about.

```c#
public int Restore(Department item)
{
	if (item == null)
		throw new ArgumentNullException(nameof(item));

	var stored = TrackForWrite(item);

	if (stored == null)
		return 0;

	try
	{
		if (!stored.DeletedDate.HasValue)
			return 0;

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

The `finally` is not optional. `stored` is tracked from the moment `TrackForWrite` returns, so every exit
owes the detach — the success path, a throwing `SaveChanges`, and the early return that writes nothing. One
left behind carries a pending change that the next write on the shared context would flush.

### A keyless join table

`CompanyResource` has no identifier at all; its identity is the `(CompanyId, ResourceId)` pair. Its interface
does not inherit `IBaseDao<T>`, so the Data Access Object derives from `RootNonIdDao<CompanyResource>`, which
publishes no `Get`. Deriving from `BaseNonIdDao<CompanyResource>` instead would publish one and remove that
guarantee silently, with every behavioral test still green.

```c#
using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;
using System;
using System.Linq;
using System.Linq.Expressions;

internal class CompanyResourceDao : RootNonIdDao<CompanyResource>, ICompanyResourceDao
{
	public CompanyResourceDao(DbContext context) : base(context) { }

	protected override Expression<Func<CompanyResource, bool>> MatchRow(CompanyResource item)
	{
		return x => x.CompanyId == item.CompanyId && x.ResourceId == item.ResourceId;
	}

	protected override IOrderedQueryable<CompanyResource> ApplyStableOrder(IQueryable<CompanyResource> query)
	{
		return query.OrderBy(x => x.CompanyId).ThenBy(x => x.ResourceId);
	}

	//This Data Access Object's own contract: a pair the store already holds is a silent no-op, not an error.
	public override void Insert(CompanyResource item)
	{
		if (item == null)
			throw new ArgumentNullException(nameof(item));

		if (Dataset.IgnoreQueryFilters().AsNoTracking().Any(MatchRow(item)))
			return;

		base.Insert(item);
	}
}
```

`IgnoreQueryFilters()` is required for the same reason every `MatchRow`-located path in the library calls it:
a consumer-declared global query filter would otherwise hide the stored row from the check, the guard would
fall through, and the base member would hit the primary key.

Map the composite key in your context:

```c#
modelBuilder.Entity<CompanyResource>().HasKey(x => new { x.CompanyId, x.ResourceId });
modelBuilder.Entity<CompanyResource>().ToTable("CompanyResources");
```

### Borrowing a context from a dependency-injection container

> **Illustrative** — not currently present in the repo.

```c#
services.AddDbContext<ExampleContext>(options => options.UseNpgsql(connectionString));
services.AddScoped<IExampleDataAccess>(sp => new ExampleDataAccess(sp.GetRequiredService<ExampleContext>()));
```

The `ExampleDataAccess(ExampleContext)` constructor this calls **is** real, and it passes
`ContextOwnership.Borrowed`, so the layer never disposes a context the container owns. The library itself
imposes no dependency-injection framework and references none.

---

## Architecture & Design Decisions

### Entity Framework Core only

2.2.0 shipped two implementations chosen by target framework — EF 6.5.1 on .NET Framework and EF Core on
.NET 8 and 9 — and the two did not behave alike. Most visibly, the EF6 `Update` was an upsert that would
store a row that did not exist, while the EF Core one requires the row to be there already. One package did
not expose one contract. The EF6 branch is removed outright; 2.2.x remains the answer for .NET Framework.

### One target framework

`net10.0`, alone. This library is a thin layer over EF Core, and carrying runtimes EF Core itself no longer
serves cost more than it returned. This is a deliberate, ratified exception to the wider `ProphetsWay`
convention of shipping `netstandard2.0` beside a current LTS — taken here because the dependency makes that
extra reach illusory.

### No compatibility wrappers

None exist and none are planned. The 3.0.0 break is real, and the migration is stated plainly in
[CHANGELOG.md](CHANGELOG.md). A shim that let 2.2.x source compile against 3.0.0 semantics would be worse
than the break, because the two lines differ behaviorally and not merely structurally.

### The key type is open, and the key value is parameterized

The 2.2.x design closed the key over three value types through a `where TIdType : struct` constraint, which
is why it needed three namespaces. Dropping the constraint collapses eighteen classes into six and makes a
`string` or `int?` key expressible for the first time.

The key value is carried into the expression tree by *reference*, through a private carrier field, so the
provider parameterizes it. `Expression.Constant` over the key itself would be emitted as a SQL literal and
give a distinct query plan per key value.

### Soft delete overrides; it never shadows

Every soft-delete difference is an `override`. Had any been `new`, a soft Data Access Object upcast to
`BaseDao<TEntity, TKey>` — which is what happens whenever a layer holds its Data Access Objects behind their
interfaces — would hard-delete. That is a data-loss bug no test written against the interface would catch.

### Keyed and keyless are unrelated inheritance branches

They share no base class, deliberately: a keyless entity has no identifier to make optional. What they do
share is behavior, and that lives in two `internal static` helpers — `EntityGraph` for the copy-for-store
walk, the inverse-navigation cleanup and the `finally` detach, and `SoftTimestamps` for the stamping policy.
Duplication of *declarations* across the two branches is structural and accepted; duplication of *logic* is
not.

### Provider neutrality — done in code, not yet in packaging

**The library's C# names no provider.** `UseSqlServer` and `UseInMemoryDatabase` appear nowhere in it. You
build the `DbContextOptions`, so you choose SQL Server, PostgreSQL, SQLite or in-memory — and 3.0.0 removed
the `BaseEFContext(string)` constructor that used to make that choice for you.

**The packaging does not match yet, and you should know before you plan a restore.**
`ProphetsWay.EFTools.csproj` still carries `PackageReference` entries for
`Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.InMemory`, so a consumer on
PostgreSQL still transitively restores two providers they will never use. Removing a transitive reference is
itself a breaking change, so it is scheduled to land inside 3.0.0 rather than after it. Tracked as FR 7 in
[docs/feature-requests.md](docs/feature-requests.md).

### Names removed in 3.0.0

The 2.2.x class surface was deleted outright rather than deprecated in place, so nothing below is in the
source, and nothing below has a shim:

- the eighteen key-specific closures in `ProphetsWay.EFTools.Guid`, `.Int` and `.Long` — **those three
  namespaces no longer exist**, so a `using` on one stops resolving and the file fails at the top rather
  than at the class
- the `RootBaseDao<T, TIdType>` and `RootBaseSoftDao<T, TIdType>` bridges beneath them, whose
  `where TIdType : struct` constraint is the one the open key exists to drop
- `LegacyBaseNonIdDao<T>`, `LegacyBaseSoftNonIdDao<T>` and `LegacyRootNonIdDao<T>` — development-branch
  scaffolding that lived for a handful of commits so the original names could be taken by the types that
  replace them. If an alpha or beta cut showed you one, it is not part of 3.0.0 and there is nothing to
  migrate onto

Each of the eighteen closures has a direct replacement, formed by moving the key type out of the namespace
and into a second type argument — `Int.BaseDao<T>` becomes `BaseDao<T, int>`, and so on through all of them.
The full mapping is in [CHANGELOG.md](CHANGELOG.md); it is not repeated here. The two bridges have no
equivalent — derive instead from whichever open-key family matches what your Data Access Object interface
publishes.

One name survives the deletion with a different meaning: 2.2.x had an **`internal`** `RootNonIdDao<T>`
engine, and 3.0.0 has a **`public`** `RootNonIdDao<TEntity>` extension point. They are different types that
happen to share a name — different visibility, different members, and a `MatchRow` contract the old one
never had. The internal one was not promoted; it was deleted and its body absorbed into the bases it served.

Nothing transitional ships. The library is the thirteen public declarations listed in the
[API Reference](#api-reference) plus two `internal static` helpers, in one flat namespace, and it contains
no conditionally compiled code at all.

### Further reading

| Document | Contents |
|---|---|
| [docs/api-contract.md](docs/api-contract.md) | The specification of the 3.0.0 surface, member by member |
| [docs/repo-profile.md](docs/repo-profile.md) | Evidence-based inventory — API surface, dependencies, packaging audit, test facts |
| [docs/purpose-and-scope.md](docs/purpose-and-scope.md) | What this library is for, and the owner decisions behind it |
| [docs/feature-requests.md](docs/feature-requests.md) | The durable record of what was considered and deliberately not built |

---

## Building & Testing Locally

The reference implementation lives partly in a git submodule, so clone recursively:

```
git clone --recurse-submodules https://github.com/ProphetManX/ProphetsWay.EFTools.git
cd ProphetsWay.EFTools
dotnet restore
dotnet build
```

If you already cloned without it, run `git submodule update --init --recursive`.

The solution contains seven projects — three owned here (`ProphetsWay.EFTools`,
`ProphetsWay.Example.DataAccess.EF`, `ProphetsWay.EFTools.Tests`) and four from the
[`ProphetsWay.Example`](https://github.com/ProphetManX/ProphetsWay.Example) submodule.

### Current SQL Server and Azure test runs

Follow [docs/azure-sql-test-execution.md](docs/azure-sql-test-execution.md). Local SQL Server defaults to
`Disposable`; Azure requires `EFTOOLS_SQLSERVER_STORE_LIFECYCLE=Reusable`.
Set `EFTOOLS_PROVIDER=SqlServer` explicitly and supply `EFTOOLS_SQLSERVER_CONNECTION_STRING`
securely as the base connection. Set `EFTOOLS_SQLSERVER_SCRATCH_DATABASE_1` and
`EFTOOLS_SQLSERVER_SCRATCH_DATABASE_2` to two distinct, pre-existing, dedicated scratch databases
matching `^EFToolsScratch_[A-Za-z0-9_]{1,113}$`. Those two configured names are the reset allowlist.
Keep settings private and session-local; do not commit or log connection values. Run only one test
process against a scratch pair at a time: the lease pool coordinates callers within that process.
Start with two empty scratch databases; do not publish the Example DACPAC to either. The fixtures
create their own models after reset.

**Destructive reset:** reusable leases drop every user foreign key and table before and after use,
destroying their contents. Never point scratch settings at a database containing anything you need.
Reusable mode does not create or drop databases, or run migrations.

The fixed `ProphetsWay.Example` database is separate and never scratch-reset. The owner reports its
Example DACPAC and synthetic seed data already applied. Before another authorized live run, validate
the existing schema, connection and authentication; do not automatically republish the DACPAC.
No commit is needed to run tests.

For a separately approved trial, configure the settings above and run from the repository root:

```powershell
dotnet test ProphetsWay.EFTools.Tests/ProphetsWay.EFTools.Tests.csproj -f net10.0 --filter "Execution!=LocalPhysicalLifecycle"
```

The filter excludes the six local physical-lifecycle checks, not all `Guard=Seam` checks.
Keep those physical checks in the unfiltered local `Disposable` run; their prior **6/6** result is
separate historical evidence, not part of the filtered trial total.

**Owner-run filtered trial, 2026-09-08:** the retained TRX reconciliation confirms **369 included
executions, all passing, zero failures and zero skips**, including all **14 provider-selection-exempt
comparison cases**. The owner reports **99.4 seconds** for the trial and a successful build in
**103.5 seconds**. See the [dated result](docs/azure-sql-test-execution.md#filtered-trial-result-2026-09-08)
for the evidence boundaries. Azure endpoint and authentication were not independently verified.
**Gate 2, FR 19, and release remain pending owner review**; this passing milestone is not
certification or release sign-off. The two owner-specific defaults were removed from the fixture
template. The owner prefers history removal but accepts leaving prior history intact and accepts
the fixture source as-is. Those decisions do not independently verify live infrastructure or clear
the infrastructure review gate.

### Running the tests

```
dotnet test
```

**SQL Server is the default test provider.**
[TestStore.cs](ProphetsWay.EFTools.Tests/TestStore.cs#L29) owns connection configuration: localhost,
integrated security and `TrustServerCertificate=True` by default, with an optional
`EFTOOLS_SQLSERVER_CONNECTION_STRING` override.
[Constants.cs](ProphetsWay.EFTools.Tests/Constants.cs#L10) names the fixed `ProphetsWay.Example` database
and delegates configuration to `TestStore`. Its schema comes from the submodule's
`ProphetsWay.Example.Database` project; the existing baseline is prepared separately and is not
scratch-reset. CI policy is unchanged: `app-variables.yml` sets `LocalTestsOnly: 'yes'` and the pipeline
skips the suite entirely.

Direct fixtures also use `TestStore` and follow its provider selection. Set `EFTOOLS_PROVIDER=Sqlite`
in the test process for **SQLite in-memory** checks without SQL Server; leaving it unset selects SQL Server.
The following trait filter selects the keyless and insert areas, not a provider:

```
dotnet test --filter "Area=Keyless|Area=Insert"
```

### How the conformance suite works

The interesting half of the suite is not written here. `ProphetsWay.Example.Tests` — in the submodule —
tests `IExampleDataAccess` without naming any implementation. This repository adapts those classes with
one-line subclasses (`public class EFUserDaoTests : UserDaoTests { }`) and repoints them at the Entity
Framework implementation through a single `[ModuleInitializer]` in `TestSeam.cs`.

The same test bodies also run against an in-memory implementation upstream. **That is the argument for the
whole paradigm**: swap the Data Access Layer, change nothing else, and the tests still pass.

> The `ProphetsWay.Example` submodule is a pinned pointer to another repository. Never edit files under
> `ProphetsWay.Example/` from here — change them upstream and advance the pointer.

---

## Contributing

Issues and pull requests are welcome at
[github.com/ProphetManX/ProphetsWay.EFTools](https://github.com/ProphetManX/ProphetsWay.EFTools).

Two things to know before you open one:

- **The 3.0.0 line is open and breaking.** The 2.2.x surface is already deleted; additions to the
  twelve-class surface that replaced it are weighed against
  [docs/api-contract.md](docs/api-contract.md) first.
- **Versions are set by hand in `app-variables.yml`, by the owner alone.** Do not bump them in a pull
  request.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the
[tags on this repository](https://github.com/ProphetManX/ProphetsWay.EFTools/tags).

## Changelog

See [CHANGELOG.md](CHANGELOG.md). The 3.0.0 entry is the migration guide — read it before upgrading from
2.2.x.

## Authors

* **G. Gordon Nasseri** - *Initial work* - [ProphetManX](https://github.com/ProphetManX)

See also the list of [contributors](https://github.com/ProphetManX/ProphetsWay.EFTools/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

