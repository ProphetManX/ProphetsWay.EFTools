#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.EFTools.Long;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class TransactionDao : BasePagedDao<Transaction>, ITransactionDao
	{
		public TransactionDao(DbContext context) : base(context) { }

		private Random random = new Random();

		public new void Insert(Transaction item)
        {
			if (item.Amount > int.MaxValue)
				item.Amount = random.Next();

			base.Insert(item);
        }

		public new int Update(Transaction item)
        {
			if (item.Amount > int.MaxValue)
				item.Amount = random.Next();

			return base.Update(item);
        }
	}
}
