#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif

namespace ProphetsWay.EFTools
{
	public abstract class BaseEFContext  : DbContext
	{
#if NET461 || NET471 || NET48
		protected BaseEFContext(string connectionString) : base(connectionString) { }
#endif

#if NET8_0_OR_GREATER
        protected BaseEFContext(string connectionString) : this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options) { }
		protected BaseEFContext(DbContextOptions builderOptions) : base(builderOptions) { }
#endif
	}
}
