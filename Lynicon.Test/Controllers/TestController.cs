using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Lynicon.Linq;
using Lynicon.Test.Models;
using Lynicon.Utility;

namespace Lynicon.Test.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index(TestContent data)
        {
            //var parser = new ODataExpressionParser();
            //parser.Variables.Add("a", typeof(string));
            //var res = parser.Parse("'p' eq a");
            LinqTest.Test();
            return View(data);
        }

        public ActionResult ConstraintOrdering(TestContent data)
        {
            return View(data);
        }
    }
}
