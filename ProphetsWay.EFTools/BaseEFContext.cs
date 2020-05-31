#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;
#endif
#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
using System.Data.Entity;
#endif

namespace ProphetsWay.EFTools
{
	public abstract class BaseEFContext  : DbContext
	{
#if NETSTANDARD2_1 || NET45 || NET461 || NET471 || NET472
		protected BaseEFContext(string connectionString) : base(connectionString) { }
#endif

#if NETSTANDARD2_0

		private readonly string _connString;

		protected BaseEFContext(string connectionString)  {
			_connString = connectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connString);
		}
#endif
	}
}
