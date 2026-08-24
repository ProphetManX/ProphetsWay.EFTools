# v3.0.0
Not yet published — nuget.org still serves 2.2.0 as the latest version of this package.  Everything below is a
change against that published 2.2.0 package.

This is a rewrite of the library on top of Entity Framework Core alone, and it breaks nearly every consumer.
Read the whole entry before upgrading.  There are no compatibility wrappers and none are planned; if the
migration is not worth your afternoon, 2.2.x remains installable and is unchanged — but read the next section
before you decide, because *unchanged* includes four defects this release fixes.

### If you are on 2.2.0: four defects you are exposed to, and three of them are silent
The published 2.2.0 package carries four defects.  **Three of the four fail silently** — no exception, no
failed build, no log line, just wrong data — so never having noticed one is not evidence you are unaffected.
Each has its own ```Fixed:``` section further down; this is the short list, so that a reader who is not
upgrading does not have to read the rest of the entry to find them.

- **A leaked context and a leaked connection per Data Access Layer instance.**  ```BaseEFDataAccess```
  constructed its own ```DbContext``` and never disposed it, and there was no ```Dispose``` for you to call
  either.  This is the one that is not silent forever: it surfaces as connection-pool exhaustion under load,
  and it is attributable once you go looking.
- **Commit and rollback that silently do nothing.**  The keyless Data Access Object base began a transaction
  only when none was already open, then reached the transaction it never stored through ```?.``` when
  committing and rolling back.  Both calls returned normally having done nothing at all.
- **```Update``` that un-deletes a soft-deleted row.**  Whole-object replacement wiped the stored
  ```DeletedDate```, so updating a soft-deleted row brought it back into every read.  A second ```Delete```
  also moved the original timestamp forward and returned ```1``` rather than ```0```.
- **Reads and writes that hand back the store's own tracked object.**  An edit you made and never submitted
  was written to the database on the next ```SaveChanges``` anywhere on that layer.

**No 2.2.1 will be cut.**  These are documented rather than patched — a deliberate decision taken because the
2.2.x consumer base is judged to be effectively empty, not an oversight — and the upgrade to 3.0.0 is the
remedy on offer.  **That remedy is not available to everyone.**  This release targets ```net10.0``` and
nothing else, so if you are on ```net48```, ```net8.0``` or ```net9.0``` you cannot take it at all, and what
follows is a description of what you are running rather than an upgrade path.  For that case the mitigations
live in your own code: there is no ```Dispose``` in 2.2.0 to call, so create as few Data Access Layer
instances as you can and keep them short-lived; drive transactions through ```Context.Database``` yourself
rather than through a Data Access Object's ```EnsureBeginTransaction```/```EnsureTransactionCommit```/
```EnsureTransactionRollback``` members; never pass a soft-deleted entity to ```Update```; and configure
```QueryTrackingBehavior.NoTracking``` on the context you hand in, which closes the ```Get``` half of the
fourth defect though not the ```Insert``` half.

### Entity Framework 6 support is gone
The 2.2.0 package shipped two implementations chosen by target framework — EntityFramework 6.5.1 on the .NET
Framework targets and EF Core on .NET 8 and 9 — and the two did not behave alike.  Most visibly, the EF6
```Update``` was an upsert that would store a row that did not exist, while the EF Core one requires the row to
be there already.  The EF6 branch has been removed outright.  If you were relying on the upsert, you now need an
explicit ```Insert```.

### Target frameworks collapse to net10.0
```net461```, ```net471```, ```net48```, ```net80``` and ```net90``` are all gone and ```net10.0``` is the only
target.  This is deliberate rather than incidental — this library is a thin layer over EF Core, and carrying
runtimes EF Core itself no longer serves was costing more than it returned.  If you are on .NET Framework, .NET 8
or .NET 9 you cannot restore this version and should remain on the 2.2.x line.  Entity Framework Core moves from
9.0.4 to 10.0.11 alongside it.

### ProphetsWay.BaseDataAccess moves from 2.5.0 to 3.2.0
Three changes in that dependency reach you through this one.  ```IBaseDataAccess``` now extends ```IDisposable```,
so every Data Access Layer has a ```Dispose``` and callers are expected to use one.  Exceptions thrown by your
own Data Access Layer methods are no longer wrapped in ```TargetInvocationException``` on their way out of the
dispatcher — if you were catching that wrapper and unwrapping ```InnerException```, catch the real exception type
instead.  And 3.2.0 annotates that package's contracts for nullable reference types, which changes what the
compiler can tell you rather than what runs.

The annotation that reaches an ordinary caller is on ```IBaseDao<T>```, whose ```Get``` every earlier version of
that package left null-oblivious:

```c#
	// 2.5.0 and 3.1.0 — null-oblivious
	T Get(T item);

	// 3.2.0
	T? Get(T item);
```
Nothing about the behavior is new — a ```Get``` for a row that is not there has always come back ```null``` — but
a call through your own ```ICompanyDao : IBaseDao<Company>``` now binds ```Company?``` instead of an oblivious
```Company```, so a project with nullable enabled may see a ```CS8602``` where it previously saw nothing.  That is
a new warning and not a break: no signature changed, nothing you have compiled needs rebuilding, and a project
without nullable enabled sees none of it.  If you implement ```IBaseDao<T>``` yourself rather than deriving from a
base in this library, read that package's own 3.2.0 entry as well — ```GetAll```, ```GetPaged``` and
```GetCount``` gained an annotated parameter, and an implementation declaring a plain ```T``` there is now
stricter than the interface allows and will report ```CS8767```.

Taking 3.2.0 is also what let the last of this library's warning suppressions go.  Ten
```#pragma warning disable CS8766``` pairs across eight files were holding down a diagnostic raised because the
bases here declared the nullable ```Get``` return their documentation described while the interface they
implemented could not say the same thing.  Both sides agree now, so the suppressions are deleted rather than
replaced and no ```#pragma``` of any kind is left in the library — the compiler verifies what it was previously
being asked to ignore.

### BaseNonIdDao and BaseSoftNonIdDao keep their names but are different types
This is the change most likely to be missed, because these two names do not disappear.  They still resolve, so
the upgrade reads like a small refactor rather than a replacement.  In 2.2.0 these were:

```c#
	BaseNonIdDao<T> : IBaseDao<T> where T: class, IBaseEntity
	BaseSoftNonIdDao<T> : BaseNonIdDao<T>, IBaseDao<T> where T : class, IBaseSoftEntity
```
They are now built on a separate plumbing class, and the members underneath you are not the members that were
there before:

```c#
	BaseNonIdDao<TEntity> : RootNonIdDao<TEntity>, IBaseDao<TEntity>
	BaseSoftNonIdDao<TEntity> : RootSoftNonIdDao<TEntity>, IBaseDao<TEntity>
```
```Get``` and ```Update``` were abstract and are now virtual with working defaults, so an existing override
keeps compiling while the code it overrides has changed underneath it.  ```Insert``` and ```Delete``` were
concrete and non-virtual and are now inherited virtual members with new implementations.  ```Context``` and
```Dataset``` were public and are now protected.  The public ```EnsureBeginTransaction```,
```EnsureTransactionCommit``` and ```EnsureTransactionRollback``` members are no longer on this surface.  In
their place ```RootNonIdDao``` declares one new abstract member, ```MatchRow```, which returns the predicate
identifying a single row — so your derived class will at least fail to compile until you supply it, which is the
one piece of this the compiler will catch for you.

The migration is to rewrite the class onto the new keyless family described below, supplying ```MatchRow``` and
dropping any overrides that no longer correspond to anything.  No wrapper preserving the old shape ships in this
release, so there is nothing to fall back to and no reason to delay the rewrite.

### BaseEFDataAccess takes one type parameter and is constructed differently
The published line documented ```BaseEFDataAccess<TContextType, TIdType>```.  That type no longer exists.  The
identifier type parameter was never used for anything the class did, and it is gone:

```c#
	BaseEFDataAccess<TContext> : BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContext : DbContext
```
The class is now abstract, and its context constraint has been relaxed from ```BaseEFContext``` to
```DbContext```, so any context you already have will do.  The protected ```Context``` property is typed
```TContext``` rather than ```DbContext```, which means your derived layer sees its own context type without a
cast.

Both public constructors are gone.  ```BaseEFDataAccess(string connectionString)``` and
```BaseEFDataAccess(DbContextOptions options)``` are replaced by a single protected constructor that takes a
built context and a statement of who owns it:

```c#
	protected BaseEFDataAccess(TContext context, ContextOwnership ownership)
```
A derived layer that called ```: base(connectionString)``` must now build the context itself — or accept one
from its caller — and say whether it is responsible for disposing it.

### BaseEFContext no longer takes a connection string, and no longer picks a provider
```BaseEFContext(string connectionString)``` has been removed; the ```DbContextOptions``` constructor is the
only one left.  On the EF Core side that removed constructor called ```UseSqlServer``` for you, which meant the
library chose your database provider.  It no longer does — whoever builds the options names the provider.  This
also removes the v2.0.0 behavior where omitting a connection string produced an in-memory context; build the
options with the provider you want, in-memory included — though as the next section describes, the in-memory
provider is no longer one this package hands you.  Deriving from ```BaseEFContext``` is now optional, since
```BaseEFDataAccess<TContext>``` accepts any ```DbContext```.

### The package no longer brings a database provider with it
Installing 2.2.0 also installed ```Microsoft.EntityFrameworkCore.SqlServer``` and
```Microsoft.EntityFrameworkCore.InMemory```, because both were ```PackageReference```s of the library itself.
A consumer on PostgreSQL, MySQL or SQLite restored two providers they were never going to call, and carried them
into every build and every deployment.  Both are gone from this package.
```Microsoft.EntityFrameworkCore``` 10.0.11 stays, because that is the dependency the library genuinely has.

Measured across the change, the dependency closure of a project taking this package goes from
```EntityFrameworkCore```, ```EntityFrameworkCore.SqlServer``` and ```EntityFrameworkCore.InMemory``` at the top
level — with ```EntityFrameworkCore.Relational``` and ```Microsoft.SqlServer.Server``` behind them — to
```EntityFrameworkCore``` alone, behind which sit only ```EntityFrameworkCore.Abstractions``` and
```EntityFrameworkCore.Analyzers```.  No provider of any kind, and no relational assembly, remains anywhere in
the closure.

**This is a breaking change if you were calling a provider you never referenced yourself.**  A line such as
```options.UseSqlServer(connectionString)``` compiled under 2.2.0 without
```Microsoft.EntityFrameworkCore.SqlServer``` ever appearing in your project file, because this package supplied
it for you.  That line will now fail to compile.  The remedy is one line in your own project file:

```xml
	<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.11" />
```
Name whichever provider you actually use — ```Npgsql.EntityFrameworkCore.PostgreSQL```,
```Pomelo.EntityFrameworkCore.MySql```, ```Microsoft.EntityFrameworkCore.Sqlite```, or
```Microsoft.EntityFrameworkCore.InMemory``` if what you wanted it for was tests.  Nothing else about the call
changes: no signature moved, no type was removed, and the code you already wrote keeps working the moment the
package it depends on is declared where it belongs.

This is the packaging catching up to the code rather than a change of behavior, and it is worth being precise
about which.  The library has named no provider in code since the ```BaseEFContext``` change described above —
there is no ```UseSqlServer``` or ```UseInMemoryDatabase``` call anywhere in it, and ```BaseEFContext``` exposes
a single ```protected BaseEFContext(DbContextOptions)``` constructor that names nothing.  What lagged behind was
the package manifest, which went on shipping two providers the code had already stopped using.

It lands in 3.0.0 because it cannot land anywhere later.  Removing a dependency changes what restores for
everyone who has already taken the package, so doing this after 3.0.0 shipped would have cost a 4.0.0 on its own.
Provider neutrality is the shape this library was meant to have, and this release is the last opportunity to
reach it without spending another major version to get there.

### The Guid, Int and Long namespaces have been removed
```ProphetsWay.EFTools.Guid```, ```ProphetsWay.EFTools.Int``` and ```ProphetsWay.EFTools.Long``` are gone, and
with them the 18 key-specific base classes they held — six in each namespace, being ```BaseDao```,
```BaseGetAllDao```, ```BasePagedDao``` and their three soft-delete counterparts.  This is likely to be the first
error you see, and it is a blunter one than the rest of this release: ```using ProphetsWay.EFTools.Int;``` no
longer resolves, so the file stops compiling at the top rather than at the class.

Every one of the eighteen has a direct replacement in the open-key families described below, formed by moving the
key type out of the namespace and into a second type argument:

```c#
	// 2.2.x
	using ProphetsWay.EFTools.Int;
	public class UserDao : BasePagedDao<User>, IUserDao { /* … */ }

	// 3.0.0
	using ProphetsWay.EFTools;
	public class UserDao : BasePagedDao<User, int>, IUserDao { /* … */ }
```
```Int.BaseDao<T>``` becomes ```BaseDao<T, int>```, ```Guid.BaseSoftPagedDao<T>``` becomes
```BaseSoftPagedDao<T, Guid>```, ```Long.BaseGetAllDao<T>``` becomes ```BaseGetAllDao<T, long>```, and so on
through all eighteen.  Constructor signatures are unchanged, so for most Data Access Objects the edit is the
```using``` line and one type argument.  The library now lives entirely in the single ```ProphetsWay.EFTools```
namespace.

One collision disappears with them.  ```ProphetsWay.EFTools.Guid``` shadowed ```System.Guid``` in any file that
imported it, which is why a ```Guid``` key sometimes had to be written out as ```System.Guid``` to compile at
all.  That no longer happens.

The two bridge classes underneath the closures, ```RootBaseDao<T, TIdType>``` and
```RootBaseSoftDao<T, TIdType>```, have been removed alongside them.  Both were marked
```[EditorBrowsable(Never)]``` and were not meant to be named directly; if you did name one, there is no 3.0.0
equivalent, and the replacement is to derive from whichever open-key family matches the capabilities your Data
Access Object interface publishes.

### New: keyed Data Access Object families with an open key type
The key-specific bases required a value-type key, through a ```where TIdType : struct``` constraint on the
bridge they closed over.  The new generic families drop it:

```c#
	BaseDao<TEntity, TKey> : IBaseDao<TEntity> where TEntity : class, IBaseIdEntity<TKey>
	BaseGetAllDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBaseGetAllDao<TEntity>
	BasePagedDao<TEntity, TKey> : BaseDao<TEntity, TKey>, IBasePagedDao<TEntity>
```
```TKey``` carries no constraint at all, so a ```string``` key or a ```Nullable<int>``` key is now legal where
before it was not expressible.  ```BaseSoftDao<TEntity, TKey>```, ```BaseSoftGetAllDao<TEntity, TKey>``` and
```BaseSoftPagedDao<TEntity, TKey>``` are the soft-delete counterparts and manage the created, updated and
deleted timestamps as the 2.x soft classes did.

### New: keyless Data Access Object families
An entity with no identifier — a join table, or one whose only key is a composite natural key — previously had
to be presented through ```IBaseDao<T>``` whether its contract promised that or not.  Four types replace that
arrangement, and they split the plumbing from what you publish:

```c#
	RootNonIdDao<TEntity> where TEntity : class, IBaseEntity
	BaseNonIdDao<TEntity> : RootNonIdDao<TEntity>, IBaseDao<TEntity>
	RootSoftNonIdDao<TEntity> : RootNonIdDao<TEntity> where TEntity : class, IBaseSoftEntity
	BaseSoftNonIdDao<TEntity> : RootSoftNonIdDao<TEntity>, IBaseDao<TEntity>
```
The ```Root``` classes declare no capability interface, so a Data Access Object deriving from one publishes only
the members it chooses.  The ```Base``` classes add ```IBaseDao<TEntity>``` and no members, for the case where
you do want the full interface.  Reads are filtered, ordered and paged through overridable seams —
```ApplyReadFilter```, ```ApplyStableOrder```, ```ApplyIncludes``` — and the row a write targets is identified by
your ```MatchRow``` predicate, which is translated to SQL rather than evaluated in memory.

### New: ContextOwnership, and a real disposal contract
```ContextOwnership``` is a two-value enum, ```Borrowed``` and ```Owned```, with **no default value you can fall
into** — the constructor makes you say which one applies.  A layer told ```Owned``` disposes the context along
with itself; one told ```Borrowed``` never touches it, because someone else will.

```c#
	public enum ContextOwnership { Borrowed = 0, Owned = 1 }
```
```BaseEFDataAccess``` implements the disposal contract that ```ProphetsWay.BaseDataAccess``` 3.x specifies.
```Dispose``` is sealed, idempotent and does not throw; it rolls back a transaction still open at the time; and
it disposes the context only when it owns it.  Every other member throws ```ObjectDisposedException``` once the
layer has been disposed.  Override ```DisposeCore``` to release anything your own layer created.

### Fixed: a Data Access Layer no longer leaks its context and its connection
In 2.2.0 both of ```BaseEFDataAccess```' constructors built the context themselves —
```Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { connectionString });```
— and **nothing ever disposed it.**  There was nowhere to: ```IBaseDataAccess``` did not extend
```IDisposable``` in that release, the class declared no ```Dispose```, no ```IDisposable``` and no finalizer,
and so a conscientious caller writing ```using``` around a Data Access Layer instance could not compile it.
Every instance you constructed left a ```DbContext```, and the database connection behind it, to be collected
whenever the garbage collector got to it.

This is the one of the four that eventually announces itself.  A long-running or high-throughput application
reaches connection-pool exhaustion and starts timing out on acquisition, and the cause is findable once you
suspect it — but a low-traffic application can run for a very long time without arriving there, which is why
it shipped.

In 3.0.0 the layer is disposable and the disposal is specified rather than incidental: ```Dispose``` is
```sealed```, idempotent and does not throw, it rolls back a transaction still open at the time, and it
disposes the context **only when it owns it** — which is now a thing the constructor makes you state, through
the ```ContextOwnership``` argument described above.  A layer handed a container-managed context leaves it
alone; a layer that was given one to own releases it.

### Fixed: commit and rollback no longer silently do nothing
This is the most dangerous of the four, because there is no observable difference between it working and it
not.  In 2.2.0 the keyless Data Access Object base began a transaction only when the context did not already
carry one:

```c#
	// 2.2.0
	if (Context.Database.CurrentTransaction == null)
		_transaction = Context.Database.BeginTransaction();
```
When a transaction **was** already open — an outer unit of work, or another Data Access Object on the same
shared context that got there first — ```_transaction``` stayed ```null```.  The matching commit and rollback
then both reached it through ```?.```, so **both returned normally having done nothing.**  No exception, no
return value indicating a skip, nothing in a log.  A Data Access Object that believed it had committed its
work had not committed anything, and a rollback written to reverse a failed write did not run — the write was
left to whatever the outer transaction decided later.

In 3.0.0 transactions belong to the Data Access Layer and not to a Data Access Object.  ```TransactionStart```,
```TransactionCommit``` and ```TransactionRollBack``` live on ```BaseEFDataAccess<TContext>```, no Data Access
Object base in the library carries a transaction member of any kind, and the library never consults
```CurrentTransaction``` to decide whether to begin one — so the check-and-skip that caused this cannot be
expressed.  Every misuse now throws instead of returning quietly: a second ```TransactionStart``` throws
```InvalidOperationException``` because transactions do not nest, and a commit or a rollback with nothing open
throws for the same reason.  If you hand the layer a ```Borrowed``` context whose owner has already begun a
transaction directly, Entity Framework Core's own exception propagates unwrapped and that outer transaction is
left in force — this layer will not silently enroll itself in someone else's transaction.

### Fixed: Update no longer un-deletes a soft-deleted row
In 2.2.0 the soft-delete base implemented ```Update``` as whole-object replacement —
```entry.CurrentValues.SetValues(item)``` over every mapped column, ```DeletedDate``` included.  An
```Update``` carrying an instance that was fetched before the delete, or built fresh from a form or a message,
wrote ```null``` over the stored ```DeletedDate``` and **the row came back to life**, reappearing in every
```GetAll```, every ```GetPaged``` and every count.  Nothing threw and no row count looked wrong.

Two more halves of the same defect sat beside it.  ```Delete``` stamped ```DeletedDate``` unconditionally
without asking whether the row was live, so a **second** delete of an already-deleted row overwrote the
original timestamp with a later one and returned ```1``` rather than ```0``` — meaning any retention window,
audit trail or "deleted before" query reading that column was answering from a timestamp that had quietly
moved.  And ```Insert``` stamped ```CreatedDate``` without clearing the other two, so a caller reusing an
instance carried stale timestamps into a brand-new row — including, in the worst case, a ```DeletedDate```,
storing a row that was invisible to every read the moment it was written.

In 3.0.0 all three are corrected together.  ```Update``` preserves the stored ```CreatedDate``` and
```DeletedDate``` and ignores whatever the incoming instance carries for them, so an update can neither
rewrite history nor soft-delete a row behind ```Delete```'s back; updating a deleted row is allowed and leaves
it deleted.  ```Delete``` checks liveness first — an already-deleted or absent row returns ```0```, changes
nothing, and **never refreshes an existing ```DeletedDate```**, so the stamp always reports when the row was
actually deleted and a second call is idempotent rather than an error.  ```Insert``` forces
```UpdatedDate``` and ```DeletedDate``` to ```null``` whatever the caller assigned.  All of them are declared
```override``` rather than ```new```, so a soft Data Access Object reached through a
```BaseDao<TEntity, TKey>```-typed reference still soft-deletes instead of quietly hard-deleting.

### Fixed: a read or a write no longer hands you the store's own object
In 2.2.0 ```Insert``` was ```Dataset.Add(item); Context.SaveChanges();```.  ```SaveChanges``` moves the entity
from ```Added``` to ```Unchanged``` and **leaves it in the change tracker**, so the instance you passed in
became the layer's instance.  ```Get``` was ```Dataset.Where(i => i.Id == item.Id).SingleOrDefault()``` with
no ```AsNoTracking``` and no projection, so it handed back the store's own tracked object rather than a copy
of it.  Entity Framework Core tracks by default and nothing in the library changed that, so unless you had
configured ```QueryTrackingBehavior.NoTracking``` on the context yourself, **an edit you made to a retrieved
entity — or to something hanging off its navigation properties — and never submitted was written to the
database on the next ```SaveChanges``` anywhere on that layer.**  It is silent, and unlike the leaked
connection the damage lands in your data rather than in your process.

In 3.0.0 ```Get```, ```GetAll``` and ```GetPaged``` read ```AsNoTracking``` and return fresh untracked
instances — never the argument you passed and never the store's object.  ```Insert``` gives the store a copy,
attaches everything reachable through your navigation properties as ```Unchanged``` so related rows are read
and never written, and writes back only the identifier the row ended up carrying, once the save has returned.
Every write detaches what it tracked before it returns.

**What stops happening, which is the part to read even if none of the above sounds familiar.**  This is the
only one of the four whose fix takes something away, and a 2.2.0 application can be depending on it without
anyone having decided to.  A stray edit made through an entity you retrieved **no longer reaches the
database** — if some part of your code has been relying on a change persisting without an explicit
```Update```, that write is now silently dropped where it used to be silently applied, and it is the one item
in this release that can change what your stored data looks like without changing a line of your own code.
And ```Update``` against a row that does not exist now returns ```0```, where 2.2.0 threw
```InvalidOperationException``` out of ```Single``` — so a ```catch``` you wrote around that call **stops
firing**, and a return value you may never have checked becomes the only signal that nothing was written.
Check it.

### Fixed: a failed Insert no longer leaves timestamps or an identifier on your instance
A soft entity's ```Insert``` stamped the created and updated timestamps onto the caller's instance before
attempting the write, so a ```SaveChanges``` that threw left you holding an instance that claimed to describe a
stored row when nothing had been stored.  The subtler half of the same defect survived a successful save: the
keyed insert assigns the identifier, clears inverse navigations and detaches, all after the row is committed, and
anything throwing in those steps left a real database identifier sitting beside a creation date naming no row.
Writes now stamp a copy and write back to your instance only once the save has returned, so the state you are
holding and the state of the database agree in both cases.

### Fixed: TargetException on a committed write
Clearing inverse navigations read a collection navigation as though it were a single principal and reflected onto
the collection object itself, raising ```TargetException``` one statement after the row had already been
committed.  The call appeared to fail while the write had in fact succeeded.  Collection and reference
navigations are now handled distinctly.

### The whole surface, and a note on prerelease builds
Nothing transitional ships.  The library is twelve public classes and one enum: ```BaseEFContext``` and
```BaseEFDataAccess<TContext>```; the keyed ```BaseDao```, ```BaseGetAllDao``` and ```BasePagedDao```; their
soft-delete counterparts ```BaseSoftDao```, ```BaseSoftGetAllDao``` and ```BaseSoftPagedDao```; the keyless
```RootNonIdDao```, ```BaseNonIdDao```, ```RootSoftNonIdDao``` and ```BaseSoftNonIdDao```; and
```ContextOwnership```.  All of them are in the ```ProphetsWay.EFTools``` namespace, and there is no longer any
conditionally compiled code in the library at all.

If you tried an alpha or beta package cut while this line was still in development, you may have seen three
```Legacy```-prefixed keyless classes, or the key-typed namespaces described above, sitting alongside the new
families.  Those were scaffolding internal to the development branch — the older classes were renamed for a
handful of commits so the new ones could take their names — and **none of them is part of 3.0.0**.  There is no
```LegacyBaseNonIdDao<T>``` to migrate onto.  Plan on the generic families.


# v2.2.0
Added support for non Id Based entities, in the BaseSoftDao class. This allows for the use of non Id based entities in the BaseSoftDao class. 
This is useful for entities that do not have a Id property, such as a composite key or a key that is not an integer.
The following classes were added:

```c#
	BaseNonIdDao<T> : IBaseDao<T> where T: class, IBaseEntity
	BaseSoftNonIdDao<T> : BaseNonIdDao<T>, IBaseDao<T> where T : class, IBaseSoftEntity
```
Similarly to the other Soft Dao classes, these classes will use the SoftDelete property to mark the entity as deleted, rather than actually deleting it from the database.
You won't have to manage the Created, Updated, or Deleted dates manually unless you override the methods in the BaseDao class.


# v2.1.1
Updated BaseDataAccess to use the new interface for IBaseSoftIdEntity and removed the interface from this project.
Fixed a bug where the BaseSoft Daos were not using the correct base class.

# v2.1.0
Updated libraries to current versions, removed target frameworks that are end of life, and added support for .Net 8.0 and 9.0.


# v2.0.0
Major updates to Entity Framework and removing obsolete methods
- added support for DbContextOptions at constructor for BaseDataAccess
- added support for non-connection string instances, will create an "in memory" instance/context (.Net 6 and above only)
- EF Tests exercise old and new .Net versions, but across both EF and EFCore as well



# v1.0.1
Updated [ProphetsWay.Example](https://github.com/ProphetManX/ProphetsWay.Example) submodule.
Updated readme link and Licence to MIT.



# v1.0.0
Initial Release!
Created a bunch of BaseDao classes to help cut down on the CRUD calls when using a proper DAL implementation with Entity Framework.