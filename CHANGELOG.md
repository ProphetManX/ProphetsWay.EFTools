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