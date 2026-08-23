#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

using System;
using System.Linq;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class TransactionDao : BasePagedDao<Transaction, long>, ITransactionDao
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

		//Include(x => x.User) is restated per ThenInclude because each chain resumes from the Transaction root.
		protected override IQueryable<Transaction> ApplyIncludes(IQueryable<Transaction> query)
		{
			return query
				.Include(x => x.Company)
				.Include(x => x.User).ThenInclude(u => u.Company)
				.Include(x => x.User).ThenInclude(u => u.Job)
				.Include(x => x.User).ThenInclude(u => u.Department);
		}
	}
}
