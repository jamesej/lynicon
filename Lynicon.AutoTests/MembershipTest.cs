using System;
using System.Linq;
using System.Collections.Generic;
using Lynicon.Collation;
using Lynicon.Repositories;
using Lynicon.Test.Models;
using Lynicon.Extensibility;
using Lynicon.Relations;
using Lynicon.Membership;
using NUnit.Framework;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestFixture]
    public class MembershipTest
    {
        [Test]
        public void TestLyniconSecurityManager()
        {
            //var db = new PreloadDb();
            //db.Database.ExecuteSqlCommand("DELETE FROM Users WHERE UserName = 'TestUser'");

            var ur = new UserRepository(new MockDataSourceFactory());
            var newUser = new User { UserName = "TestUser", Email = "test@user.com", Roles = "UE"};
            ur.Set(new List<object> { newUser }, new Dictionary<string, object>());

            //var user = sm.LoginUser("TestUser", password);
            //Assert.IsNotNull(user, "log in");

            //bool inRoleU = sm.CurrentUserInRole("U");
            //Assert.IsTrue(inRoleU, "in U role");
        }
    }
}
