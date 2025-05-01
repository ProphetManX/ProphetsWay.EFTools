#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET471 || NET48
using System.Data.Entity;
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
