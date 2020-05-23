using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using System.Data.Entity;

namespace ProphetsWay.Example.DataAccess.EF
{
	public class ExampleContext : BaseEFContext
	{
		public ExampleContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{

		}

		public DbSet<Company> Companies { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Job> Jobs { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Company>().ToTable("Companies");
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Job>().ToTable("Jobs");



		}


	}
}
