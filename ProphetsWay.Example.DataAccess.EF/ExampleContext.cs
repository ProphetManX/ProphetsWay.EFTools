#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.Enums;

namespace ProphetsWay.Example.DataAccess.EF
{
	public class ExampleContext : BaseEFContext
	{
		public ExampleContext(string connectionString) : base(connectionString) { }

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
		public ExampleContext(DbContextOptions<ExampleContext> options) : base(options) { }
#endif

		public DbSet<Company> Companies { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Resource> Resources { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Job> Jobs { get; set; }


#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
			modelBuilder.Entity<User>().HasOne(x => x.Job).WithMany().HasForeignKey("JobId");
			modelBuilder.Entity<User>().Property(x => x.RoleStr).HasConversion(x=> x.ToString(), x=> (Roles)System.Enum.Parse(typeof(Roles), x));

			modelBuilder.Entity<Transaction>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
			modelBuilder.Entity<Transaction>().HasOne(x => x.User).WithMany().HasForeignKey("UserId");
#endif

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{		
			modelBuilder.Entity<User>().HasOptional(x => x.Company).WithMany().Map(m => m.MapKey("CompanyId"));
			modelBuilder.Entity<User>().HasOptional(x => x.Job).WithMany().Map(m => m.MapKey("JobId"));

			modelBuilder.Entity<Transaction>().HasOptional(x => x.Company).WithMany().Map(m => m.MapKey("CompanyId"));
			modelBuilder.Entity<Transaction>().HasOptional(x => x.User).WithMany().Map(m => m.MapKey("UserId"));
#endif

			modelBuilder.Entity<Company>().ToTable("Companies");
			modelBuilder.Entity<Job>().ToTable("Jobs");
			modelBuilder.Entity<Resource>().ToTable("Resources");
			modelBuilder.Entity<Transaction>().ToTable("Transactions");
			modelBuilder.Entity<User>().ToTable("Users");

		}


	}
}
