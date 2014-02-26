using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Membership;
using Lynicon.Repositories;
using Lynicon.Linq;

namespace Lynicon.Test.Models
{
    public class TestA
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    public class TestB
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
    }

    public class LinqTest
    {
        public static void Test()
        {
            List<TestB> items = new List<TestB> {
                new TestB { A = "hello", B = "xyz", C = "ccc" },
                new TestB { A = "goodbye", B = "xyz", C = "ddd" },
                new TestB { A = "lll", B = "bbb", C = "ccc" }
            };

            var q = items.AsQueryable()
                .Where(x => x.B == "xyz")
                .Select(x => x.A);

            var exp = q.Expression;

            var qt = new FacadeTypeQueryable<TestA>(items.AsQueryable());

            var res = qt.Where(x => x.B == "xyz").ToList();

            var db = new CoreDb("LyniconTest");

            foreach (var o in db.CompositeSet<ContentItem>())
            {
                var y = o;
            }

            var qry = db.CompositeSet<User>()
                .AsFacade<User>()
                .RestrictFields(new List<string> { "Id" });
                

            //var uq = db.CompositeSet<User>();
            //var users = uq.AsFacade<User>()
            //    .Where(u => u.UserName == "jimmy")
            //    .AsFacade<ExtendedUser>()
            //    .Where(u => u.ExtData != null)
            //    .ToList();
            //users[0].Roles = "XYZ";
            //db.SaveChanges();

            var cr = new BasicRepository();
            var ci = cr.Get<User>(typeof(User), new List<object> { new Guid("040C7CFD-4107-4EF6-AD2C-A9D7779227E1") }).FirstOrDefault();
            ci.Roles = "PDQ";
        }
    }
}
