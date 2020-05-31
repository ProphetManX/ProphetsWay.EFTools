#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#endif
#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;

namespace ProphetsWay.Example.DataAccess.EF
{
	public class ExampleContext : BaseEFContext
	{
		public ExampleContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

		public DbSet<Company> Companies { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Job> Jobs { get; set; }

#if NETSTANDARD2_0
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
			modelBuilder.Entity<User>().HasOne(x => x.Job).WithMany().HasForeignKey("JobId");
#endif

#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{		
			modelBuilder.Entity<User>().HasOptional(x => x.Company).WithMany().Map(m => m.MapKey("CompanyId"));
			modelBuilder.Entity<User>().HasOptional(x => x.Job).WithMany().Map(m => m.MapKey("JobId"));
#endif

			modelBuilder.Entity<Company>().ToTable("Companies");
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Job>().ToTable("Jobs");
		}


	}
}
