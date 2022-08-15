using ProphetsWay.Example.DataAccess.IDaos;
using ProphetsWay.Example.Tests;

namespace ProphetsWay.EFTools.Tests
{
	public class EFTransactionDaoTests : TransactionDaoTests
	{
		protected override ITransactionDao GetIExampleDataAccess => Constants.GetExampleDataAccess;
	}
}
