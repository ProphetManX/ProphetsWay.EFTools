#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0 || NETCOREAPP2_1 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET471 || NET472 || NET48
using System.Data.Entity;
using System.Data.Entity.Migrations;
#endif
using ProphetsWay.EFTools.Int;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class UserDao : BaseDao<User>, IUserDao
	{
		public UserDao(DbContext context) : base(context) { }

		public void CustomUserFunctionality(User user)
		{
			user.Whatever = "custom functionality triggered";
			Update(user);
		}
	}
}
