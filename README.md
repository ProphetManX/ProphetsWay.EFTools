# ProphetsWay.EFTools
| Release   | Status |
|   ---     |  ---   |
| Latest Build: | [![Build status](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_apis/build/status/EFTools%20-%20CI)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_build/?definitionId=17)
| Alpha:    | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/9/21)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=9)  update still
| Beta:     | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/9/22)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=9)  update still
| Release:  | [![Build status](https://vsrm.dev.azure.com/ProphetsWay/_apis/public/Release/badge/dadb23ce-840b-4b7d-9783-dc5e9a2d9029/9/23)](https://dev.azure.com/ProphetsWay/ProphetsWay%20GitHub%20Projects/_release?definitionId=9)  update still


A small library that is useful when utilizing EntityFramework for a Data Access Layer (DAL) while 
adhering to Business Layer to DAL decoupling. This uses the paradigm explained in 
https://github.com/ProphetManX/ProphetsWay.BaseDataAccess, see its README for more information.


## How to use
You can reference EFTools from NuGet.  Once you have it in your project, you will be able to write custom implementations
for your DAL, however some functionality will be handled for you in base abstract classes that you can inherit from.  I will
try to explain each of the base classes in order of how I would probably code a new project from scratch.

For examples, please see the example projects included in the GitHub repository.

## Base Dao Classes
##### Special note
EFTools references interfaces that were established in another project, ProphetsWay.BaseDataAccess.  The point of mentioning
this is that you will need to follow the guidelines for the Data Access Object (DAO) interfaces defined within that project 
(but only to make use of the BaseDao classes).


### BaseDao and BaseDaoWith[type]Id
This Dao implements the IBaseDao interface, meaning it has all the Create, Read, Update and Delete (CRUD) calls established within.  
The following methods are implemented:

``` c#
T Get(T item);
void Insert(T item);
int Update(T item);
int Delete(T item);

void EnsureBeginTransaction();
void EnsureTransactionCommit();
void EnsureTransactionRollback();
```

An awkward thing to notice is how all the CRUD calls take an item an argument, this is used so all Entities
can use the same method names and there isn't any overlap.  The system knows which DAO to use because of the type
of the argument passed.  In the case of ```T Get(T item);``` and ```T Delete(T item);``` 
only the 'ID' property needs to be set of the passed object; however it is most likely you won't use 
```T Get(T item);``` directly due to the base class ```BaseDataAccess<TIdType>``` which I will 
get to further down.

Additionally there are the "EnsureTransaction" methods defined and made available to you.  These are for situations
within a specific DAO that you need to ensure the call is wrapped within a Transaction, but don't have the scope
to tell if a transaction was started higher within the call stack.  Because you can't begin a second transaction
while the first is begun, these methods will create one if none exists, and if one did exist, it won't commit/rollback
any changes either (leaving the commit/rollback to fall on the initial transaction creator).


Lastly all the BaseDao classes have a required constructor that you must pass in the ```DbContext``` that the
DAO will use to interface with the database.  I'll show an example of creating a DAL class that puts everything together
further down.


#### BaseDaoWith[type]Id
Because of how EntityFramework works, it was required to identify the 'type' of the identifier property
on your entities, so to optimize the queries.  For this reason there are three implementations of the ```BaseDao```
class:  ```BaseDaoWithIntId<T>```, ```BaseDaoWithLongId<T>```, and ```BaseDaoWithGuidId<T>```.  Basically 
you should use the one that corresponds to the type of the primary key you use in your database schema.  
**You will a similar trend with the next two DAOs covered.*

##### Example
In the following code block, you can see my implementation of the ```UserDao```, it implements the interface ```IUserDao```
which requires the CRUD calls, and it defines one additional method ```CustomUserFunctionality(User user)```.  Using
```BaseDaoWithIntId<User>``` tells the BaseDao that my entity is a "User" object, all the CRUD calls are handled for me
in that base class, and all I needed to implement manually is the additional method.

``` c#
internal class UserDao : BaseDaoWithIntId<User>, IUserDao
{
	public UserDao(DbContext context) : base(context) { }

	public void CustomUserFunctionality(User user)
	{
		user.Whatever = "custom functionality triggered";
		Update(user);
	}
}
```


### BaseGetAllDao
This Dao inherits from BaseDao and implements the IBaseGetAllDao interface.  The following method is implemented:
``` c#
IList<T> GetAll(T item);
```

##### Example
Below is an implementation of ```JobDao```.  Similar to above it implements the ```IJobDao``` 
interface which defines all the default methods needed, however in this case it does not define any additional methods
to implement.  All the CRUD and the GetAll methods are coded for me in the base classes.


``` c#
internal class JobDao : BaseGetAllDaoWithIntId<Job>, IJobDao
{
	public JobDao(DbContext context) : base(context) { }
}
```


### BasePagedDao
This Dao inherits from BaseDao and implements the IBasePagedDao interface.  The following methods are implemented:

``` c#
int GetCount(T item);
IList<T> GetPaged(T item, int skip, int take);
```

##### Example
Below is the last type of DAO created for ```CompanyDao``` using the interface ```ICompanyDao```.  This specifies 
default functionality, and an additional method ```GetCustomCompanyFunction(int id)```.  Just as before all the default
functionality is implemented in the base classes and you only need to code the additional methods you define.

``` c#
internal class CompanyDao : BasePagedDaoWithIntId<Company>, ICompanyDao
{
	public CompanyDao(DbContext context) : base(context) { }

	public Company GetCustomCompanyFunction(int id)
	{
		return Dataset.OrderBy(x=> x.Id).Skip(id % GetCount(null)).First();
	}
}
```


### BaseEFContext
This is a simple abstract class that is defined so that you always pass the connection string via its constructor.
It is used so that the next base class can properly instanciate your specific 'Typed' context.

##### Example
Here is an example of the ```DbContext``` that you'll create for your own application.  It inherits the ```BaseEFContext```
and thus its constructor, but everything else about the context is what you would expect from a manual EntityFramework
project.

``` c#
public class ExampleContext : BaseEFContext
{
	public ExampleContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

	public DbSet<Company> Companies { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Job> Jobs { get; set; }

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{		
		modelBuilder.Entity<User>().HasOptional(x => x.Company).WithMany().Map(m => m.MapKey("CompanyId"));
		modelBuilder.Entity<User>().HasOptional(x => x.Job).WithMany().Map(m => m.MapKey("JobId"));

		modelBuilder.Entity<Company>().ToTable("Companies");
		modelBuilder.Entity<User>().ToTable("Users");
		modelBuilder.Entity<Job>().ToTable("Jobs");
	}
}
```

### BaseEFDataAccess<TContextType, TIdType>
Now this is the part where it all comes together.  Your application's DAL will implement whatever ```IDataAccess```
interface you defined with all the ```IDao```'s specified within.  This base class will create your ```DbContext```
automatically when constructed as defined by ```TContextType``` and by passing it ```TIdType``` it will give you access
to a new way to "Get" entities by Id instead of the ```T Get(T item);```.  
Now you can just call ```yourDAL.Get<T>(idValue);``` to return an item of type T with an Id that you pass in.

Internally to this class you create each IDao, and then just "pass thru" each method call to the specific Dao they 
correspond to.  Please see the example below.

##### Example


``` c#
public class ExampleDataAccess : BaseEFDataAccess<ExampleContext, int>, IExampleDataAccess
{
	private readonly ICompanyDao _companyDao;
	private readonly IJobDao _jobDao;
	private readonly IUserDao _userDao;

	public ExampleDataAccess(string connectionString) : base(connectionString)
	{
		_companyDao = new CompanyDao(Context);
		_jobDao = new JobDao(Context);
		_userDao = new UserDao(Context);
	}

	#region CompanyDao

	public Company Get(Company item)
	{
		return _companyDao.Get(item);
	}

	public int GetCount(Company item)
	{
		return _companyDao.GetCount(item);
	}

	public Company GetCustomCompanyFunction(int id)
	{
		return _companyDao.GetCustomCompanyFunction(id);
	}

	public IList<Company> GetPaged(Company item, int skip, int take)
	{
		return _companyDao.GetPaged(item, skip, take);
	}

	public void Insert(Company item)
	{
		_companyDao.Insert(item);
	}

	public int Delete(Company item)
	{
		return _companyDao.Delete(item);
	}

	public int Update(Company item)
	{
		return _companyDao.Update(item);
	}

	#endregion

	//Region for JobDao

	//Region for UserDao

	}
}

```


For working examples of how all these parts work, please see the example projects within the GitHub repository.



## Running the tests

The library has 21 unit tests currently.  I only covered code that exercises the EFTools functionality.
They are created with an XUnit test project. Unfortunately I don't have the unit tests running in 
the build pipeline because they are actually hitting a local database to run.

If you look at the project ```ProphetsWay.EFTools.Tests``` you'll see that each set of tests is actually
just a new class inheriting from each original test class, but overriding a property that returns an instance 
of a particular interface.  It looks confusing, but this is so I can fully test ```IExampleDataAccess``` 
as defined within ```ProphetsWay.Example.DataAccess``` using the same tests the example project defined,
but with using my new custom implementation which utilizes EFTools.


## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/ProphetManX/ProphetsWay.EFTools/tags). 

## Authors

* **G. Gordon Nasseri** - *Initial work* - [ProphetManX](https://github.com/ProphetManX)

See also the list of [contributors](https://github.com/ProphetManX/ProphetsWay.EFTools/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

