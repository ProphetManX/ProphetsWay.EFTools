# v2.0.0
Major updates to Entity Framework and removing obsolete methods
- added support for DbContextOptions at constructor for BaseDataAccess
- added support for non-connection string instances, will create an "in memory" instance/context (.Net 6 only)
- EF Tests exercise old and new .Net versions, but across both EF and EFCore as well



# v1.0.1
Updated [ProphetsWay.Example](https://github.com/ProphetManX/ProphetsWay.Example) submodule.
Updated readme link and Licence to MIT.



# v1.0.0
Initial Release!
Created a bunch of BaseDao classes to help cut down on the CRUD calls when using a proper DAL implementation with Entity Framework.