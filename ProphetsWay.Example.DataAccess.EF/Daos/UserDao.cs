#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET471 || NET48
using System.Data.Entity;
#endif
using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class UserDao : BaseDao<User, int>, IUserDao
	{
		public UserDao(DbContext context) : base(context) { }

		public void CustomUserFunctionality(User user)
		{
			user.Whatever = "custom functionality triggered";
			Update(user);
		}
	}
}
