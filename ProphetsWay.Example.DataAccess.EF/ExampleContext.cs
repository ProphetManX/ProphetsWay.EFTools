using Microsoft.EntityFrameworkCore;
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.Enums;

namespace ProphetsWay.Example.DataAccess.EF
{
	/// <summary>
	/// The Entity Framework Core context behind <see cref="ExampleDataAccess"/>.
	/// </summary>
	/// <remarks>
	/// It names no database provider. Whoever builds the <see cref="DbContextOptions{TContext}"/> chooses one,
	/// which is what lets a test point this context at an in-memory SQLite database it controls.
	/// </remarks>
	public class ExampleContext : BaseEFContext
	{
		/// <summary>
		/// Initializes the context from options the caller has already configured, provider included.
		/// </summary>
		public ExampleContext(DbContextOptions<ExampleContext> options) : base(options) { }

		public DbSet<Company> Companies { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Resource> Resources { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Job> Jobs { get; set; }
		public DbSet<Department> Departments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
			modelBuilder.Entity<User>().HasOne(x => x.Job).WithMany().HasForeignKey("JobId");
			modelBuilder.Entity<User>().HasOne(x => x.Department).WithMany().HasForeignKey("DepartmentId");
			modelBuilder.Entity<User>().Property(x => x.RoleStr).HasConversion(x => x.ToString(), x => (Roles)System.Enum.Parse(typeof(Roles), x));

			modelBuilder.Entity<Transaction>().HasOne(x => x.Company).WithMany().HasForeignKey("CompanyId");
			modelBuilder.Entity<Transaction>().HasOne(x => x.User).WithMany().HasForeignKey("UserId");

			modelBuilder.Entity<Company>().ToTable("Companies");
			modelBuilder.Entity<Department>().ToTable("Departments");
			modelBuilder.Entity<Job>().ToTable("Jobs");
			modelBuilder.Entity<Resource>().ToTable("Resources");
			modelBuilder.Entity<Transaction>().ToTable("Transactions");
			modelBuilder.Entity<User>().ToTable("Users");
		}
	}
}
