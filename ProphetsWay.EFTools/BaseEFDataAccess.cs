#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
#endif
#if NET45 || NET451 || NET452|| NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
# endif
using ProphetsWay.BaseDataAccess;
using System;

namespace ProphetsWay.EFTools
{
	public class BaseEFDataAccess<TContextType, TIdType> : BaseDataAccess<TIdType>, IBaseDataAccess<TIdType> where TContextType : BaseEFContext
	{
		protected DbContext Context { get; }

		public BaseEFDataAccess(string connectionString)
		{
			Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { connectionString });
		}

		public override void TransactionCommit()
		{
			Context.Database.CurrentTransaction.Commit();
		}

		public override void TransactionRollBack()
		{
			Context.Database.CurrentTransaction.Rollback();
		}

		public override void TransactionStart()
		{
			Context.Database.BeginTransaction();
		}
	}
}
