#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET461 || NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.BaseDataAccess;
using System;

namespace ProphetsWay.EFTools
{
	public class BaseEFDataAccess<TContextType, TIdType> : BaseDataAccess.BaseDataAccess, IBaseDataAccess where TContextType : BaseEFContext
	{
		private bool _disposed;

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

		/// <summary>
		/// Rolls back any transaction still open and disposes the <see cref="DbContext"/> this instance created.
		/// </summary>
		/// <remarks>
		/// Idempotent, and never throws: a failed rollback is swallowed rather than propagated, as
		/// <see cref="IBaseDataAccess"/> requires. The context is disposed because both constructors construct it
		/// here — a context handed in by a caller would belong to the caller and would not be disposed.
		/// </remarks>
		public override void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;

			try
			{
				var transaction = Context.Database.CurrentTransaction;

				if (transaction != null)
					transaction.Rollback();
			}
			catch
			{
				// An abandoned transaction is rolled back by the database once the connection drops.
			}

			try
			{
				Context.Dispose();
			}
			catch
			{
				// Disposal never throws.
			}
		}
	}
}
