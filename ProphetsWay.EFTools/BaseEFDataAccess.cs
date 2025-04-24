#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.BaseDataAccess;
using System;

namespace ProphetsWay.EFTools
{
	public class BaseEFDataAccess<TContextType, TIdType> : BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContextType : BaseEFContext
	{
		protected DbContext Context { get; }

		public BaseEFDataAccess(string connectionString)
		{
			Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { connectionString });
		}

#if NET8_0_OR_GREATER
        public BaseEFDataAccess(DbContextOptions options)
        {
			Context = (DbContext)Activator.CreateInstance(typeof(TContextType), new object[] { options });
		}
#endif
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
