#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#endif
#if NETSTANDARD2_1
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
