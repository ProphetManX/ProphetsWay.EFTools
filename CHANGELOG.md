# v3.0.0 — not yet released
This line is open.  The owner has not tagged it, and further work will land under this same number, so the entry
below describes the state of the line rather than the contents of a release.  Everything here is a change against
the published 2.2.0 package.

This is a rewrite of the library on top of Entity Framework Core alone, and it breaks nearly every consumer.
Read the whole entry before upgrading.  There are no compatibility wrappers and none are planned; if the
migration is not worth your afternoon, stay on 2.2.x, which continues to work exactly as it did.

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

### ProphetsWay.BaseDataAccess moves from 2.5.0 to 3.1.0
Two changes in that dependency reach you through this one.  ```IBaseDataAccess``` now extends ```IDisposable```,
so every Data Access Layer has a ```Dispose``` and callers are expected to use one.  And exceptions thrown by
your own Data Access Layer methods are no longer wrapped in ```TargetInvocationException``` on their way out of
the dispatcher — if you were catching that wrapper and unwrapping ```InnerException```, catch the real exception
type instead.

### BaseNonIdDao and BaseSoftNonIdDao keep their names but are different types
This is the change most likely to be missed, because nothing disappears.  The names still resolve, so the
upgrade reads like a small refactor rather than a replacement.  In 2.2.0 these were:

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
dropping any overrides that no longer correspond to anything.  You will see three ```Legacy```-prefixed types in
this build holding the old shapes; **do not migrate onto them** — see the closing note.

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
options with the provider you want, in-memory included.  Deriving from ```BaseEFContext``` is now optional, since
```BaseEFDataAccess<TContext>``` accepts any ```DbContext```.

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

### A note on the Legacy types, so they are not mistaken for API
```LegacyBaseNonIdDao<T>```, ```LegacyBaseSoftNonIdDao<T>``` and ```LegacyRootNonIdDao<T>``` are the 2.2.x
keyless classes carried under new names purely so the original names could be reused by the types that replace
them.  They are scheduled for deletion before 3.0.0 is tagged and are not a supported migration target.  The
same removal is planned for the 18 key-specific closures in the ```ProphetsWay.EFTools.Guid```,
```ProphetsWay.EFTools.Int``` and ```ProphetsWay.EFTools.Long``` namespaces and the two ```RootBase``` bridges
beneath them, all of which are still present in this build and all of which are superseded by the open-key
families above.  Plan on the generic families; do not write new code against anything named in this paragraph.


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