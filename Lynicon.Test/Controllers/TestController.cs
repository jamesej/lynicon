using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Collation;
using Lynicon.Linq;
using Lynicon.Membership;
using Lynicon.Test.Models;
using Lynicon.Utility;
using Lynicon.Workflow.Models;

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

        public ActionResult VersionEquality(TestContent data)
        {
            return View(data);
        }

        public ActionResult Layering(TestContent data)
        {
            // Create HeaderContent at 'test' on layer 10
            var rd = new RouteData();
            rd.Values.Add("_0", "test");
            var hc = new HeaderContent();
            hc.Summary.Title = "Test Layer 10";
            var wfu = LyniconSecurityManager.Current.User as IWorkflowUser;
            LayerManager.Instance.SetUserLayer(wfu, 10);
            Collator.Instance.Set(rd, hc);

            // Verify doesn't exist at layer 5
            LayerManager.Instance.SetUserLayer(wfu, 5);
            var hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T1"] = (hcRead == null ? "Pass" : "Fail");

            // Verify does exist at layer 15
            LayerManager.Instance.SetUserLayer(wfu, 15);
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T2"] = (hcRead == null ? "Fail" : "Pass");

            // Edit at layer 15
            hcRead.Summary.Title = "Test Layer 15";
            Collator.Instance.Set(rd, hcRead);

            // Check edit
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T3"] = (hcRead.Summary.Title == "Test Layer 15" ? "Pass" : "Fail");

            // Verify OK at layer 10
            LayerManager.Instance.SetUserLayer(wfu, 10);
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T4"] = (hcRead.Summary.Title == "Test Layer 10" ? "Pass" : "Fail");

            // Delete at layer 20
            LayerManager.Instance.SetUserLayer(wfu, 20);
            Collator.Instance.Delete(hcRead);

            // Verify deleted at layer 20
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T5"] = (hcRead == null ? "Pass" : "Fail");

            // Verify deleted at layer 30
            LayerManager.Instance.SetUserLayer(wfu, 30);
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T6"] = (hcRead == null ? "Pass" : "Fail");

            // Verify OK at layer 10
            LayerManager.Instance.SetUserLayer(wfu, 10);
            hcRead = Collator.Instance.Get<HeaderContent>(rd);
            ViewData["T7"] = (hcRead != null && hcRead.Summary.Title == "Test Layer 10" ? "Pass" : "Fail");

            return View();
            
        }
    }
}
