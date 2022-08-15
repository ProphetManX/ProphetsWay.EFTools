#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;
#endif
#if NET45 || NET451 || NET452|| NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
#endif

namespace ProphetsWay.EFTools
{
	public abstract class BaseEFContext  : DbContext
	{
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
		protected BaseEFContext(string connectionString) : base(connectionString) { }
#endif

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
		protected BaseEFContext(string connectionString) : this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options) { }
		protected BaseEFContext(DbContextOptions builderOptions) : base(builderOptions) { }
#endif
	}
}
