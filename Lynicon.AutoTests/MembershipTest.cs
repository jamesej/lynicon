using System;
using System.Linq;
using System.Collections.Generic;
using Lynicon.Collation;
using Lynicon.Repositories;
using Lynicon.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lynicon.Extensibility;
using Lynicon.Base.Modules;
using Lynicon.Base.Models;
using Lynicon.Relations;
using Lynicon.Membership;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestClass]
    public class MembershipTest
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {

        }

        [TestMethod]
        public void TestLyniconSecurityManager()
        {
            var db = new PreloadDb();
            db.Database.ExecuteSqlCommand("DELETE FROM Users WHERE UserName = 'TestUser'");

            var ur = new UserRepository();
            var newUser = new User { UserName = "TestUser", Email = "test@user.com", Roles = "UE"};
            ur.Set(new List<object> { newUser }, new Dictionary<string, object>());

            //string password = "abcdefg";
            //var sm = new LyniconSecurityManager();
            //var succeed = sm.SetPassword(newUser.IdAsString, password);
            //Assert.IsTrue(succeed, "set password");

            //var user = sm.LoginUser("TestUser", password);
            //Assert.IsNotNull(user, "log in");

            //bool inRoleU = sm.CurrentUserInRole("U");
            //Assert.IsTrue(inRoleU, "in U role");
        }
    }
}
