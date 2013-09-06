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

            var qt = new QueryTranslator<TestA>(items.AsQueryable());

            var res = qt.Where(x => x.B == "xyz").ToList();

            var db = new CoreDb("LyniconTest");

            //foreach (var o in db.CompositeSet<User>())
            //{
            //    var y = o;
            //}

            var uq = new QueryTranslator<IUser>(db.CompositeSet<User>());
            List<IUser> users = uq.Where(u => u.UserName == "jimmy").ToList();
            users[0].Roles = "XYZ";
            db.SaveChanges();
        }
    }
}
