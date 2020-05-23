using ProphetsWay.BaseDataAccess;
using System;
using System.Data.Entity;

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
