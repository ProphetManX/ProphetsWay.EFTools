namespace ProphetsWay.EFTools
{
	/// <summary>
	/// Who is responsible for disposing the context a Data Access Layer was handed.
	/// </summary>
	/// <remarks>
	/// There is deliberately no default. A defaulted ownership is the shape that produces a silent
	/// double-dispose in a dependency-injection host and a silent leak in a manually constructed one,
	/// depending only on which way the default happened to point. The caller states it.
	/// </remarks>
	public enum ContextOwnership
	{
		/// <summary>Someone else created the context and will dispose it. This layer never does.</summary>
		Borrowed = 0,

		/// <summary>This layer created the context and disposes it along with itself.</summary>
		Owned = 1
	}
}
