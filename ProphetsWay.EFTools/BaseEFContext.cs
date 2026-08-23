using Microsoft.EntityFrameworkCore;

namespace ProphetsWay.EFTools
{
	/// <summary>
	/// An optional base for a consumer's <see cref="DbContext"/>. It selects no provider and adds no
	/// behavior — it exists so a consumer's context has a family-shaped base and a single constructor
	/// convention.
	/// </summary>
	/// <remarks>
	/// Deriving from this type is <b>not</b> required. <see cref="BaseEFDataAccess{TContext}"/> constrains
	/// its context to <see cref="DbContext"/>, so any context works.
	/// </remarks>
	public abstract class BaseEFContext : DbContext
	{
		/// <summary>
		/// Initializes the context from options the consumer has already configured.
		/// </summary>
		/// <param name="builderOptions">
		/// The configured options. Whoever builds them names the database provider; this library never does.
		/// </param>
		protected BaseEFContext(DbContextOptions builderOptions) : base(builderOptions) { }
	}
}
