using ProphetsWay.EFTools;
using ProphetsWay.Example.DataAccess.Entities;
using ProphetsWay.Example.DataAccess.IDaos;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace ProphetsWay.Example.DataAccess.EF.Daos
{
	internal class UserDao : BaseDaoWithIntId<User>, IUserDao
	{
		public UserDao(DbContext context) : base(context) { }

		public void CustomUserFunctionality(User user)
		{
			user.Whatever = "custom functionality triggered";
			Dataset.AddOrUpdate(user);
			Context.SaveChanges();
		}
	}
}
