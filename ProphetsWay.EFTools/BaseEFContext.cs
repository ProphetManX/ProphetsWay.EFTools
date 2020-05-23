using System.Data.Entity;

namespace ProphetsWay.EFTools
{
	public abstract class BaseEFContext  : DbContext
	{
		protected BaseEFContext(string connectionString) : base(connectionString) { }
	}
}
