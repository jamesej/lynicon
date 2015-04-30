using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Linq;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Search;
using Lynicon.Test.Models;
using Lynicon.Utility;
//using Lynicon.Workflow.Models;

namespace Lynicon.Test.Controllers
{
    public class StringInputOutput
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }

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

        //public ActionResult Layering(TestContent data)
        //{
        //    // Create HeaderContent at 'test' on layer 10
        //    var rd = new RouteData();
        //    rd.Values.Add("_0", "test");
        //    var hc = new HeaderContent();
        //    hc.Summary.Title = "Test Layer 10";
        //    var wfu = LyniconSecurityManager.Current.User as IWorkflowUser;
        //    LayerManager.Instance.SetUserLayer(wfu, 10);
        //    Collator.Instance.Set(rd, hc);

        //    // Verify doesn't exist at layer 5
        //    LayerManager.Instance.SetUserLayer(wfu, 5);
        //    var hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T1"] = (hcRead == null ? "Pass" : "Fail");

        //    // Verify does exist at layer 15
        //    LayerManager.Instance.SetUserLayer(wfu, 15);
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T2"] = (hcRead == null ? "Fail" : "Pass");

        //    // Edit at layer 15
        //    hcRead.Summary.Title = "Test Layer 15";
        //    Collator.Instance.Set(rd, hcRead);

        //    // Check edit
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T3"] = (hcRead.Summary.Title == "Test Layer 15" ? "Pass" : "Fail");

        //    // Verify OK at layer 10
        //    LayerManager.Instance.SetUserLayer(wfu, 10);
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T4"] = (hcRead.Summary.Title == "Test Layer 10" ? "Pass" : "Fail");

        //    // Delete at layer 20
        //    LayerManager.Instance.SetUserLayer(wfu, 20);
        //    Collator.Instance.Delete(hcRead);

        //    // Verify deleted at layer 20
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T5"] = (hcRead == null ? "Pass" : "Fail");

        //    // Verify deleted at layer 30
        //    LayerManager.Instance.SetUserLayer(wfu, 30);
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T6"] = (hcRead == null ? "Pass" : "Fail");

        //    // Verify OK at layer 10
        //    LayerManager.Instance.SetUserLayer(wfu, 10);
        //    hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    ViewData["T7"] = (hcRead != null && hcRead.Summary.Title == "Test Layer 10" ? "Pass" : "Fail");

        //    return View();
            
        //}

        //public ActionResult SearchTest()
        //{
        //    LayerManager.Instance.SetLiveLayer(10);

        //    var rd = new RouteData();
        //    rd.Values.Add("_0", "test");

        //    VersionManager.Instance.PushState(VersioningMode.All);
        //        var layerItems = Repository.Instance.GetByPath(typeof(HeaderContent),
        //            new List<string> { "test" });
        //        foreach (var item in layerItems)
        //            Repository.Instance.Delete(item);
        //    VersionManager.Instance.PopState();

        //    SearchManager.Instance.BuildIndex();

        //    var hc = new HeaderContent();
        //    hc.Summary.Title = "Test Layer 20";
        //    hc.HeaderBody = "<p>hippopotamus</p>";
        //    var wfu = LyniconSecurityManager.Current.User as IWorkflowUser;
        //    LayerManager.Instance.SetUserLayer(wfu, 20);
        //    Collator.Instance.Set(rd, hc);

        //    var spec = new SearchSpec { GeneralSearch = "hippopotamus" };
        //    int total;

        //    // Search (item in layer above)

        //    var res = SearchManager.Instance.Search<HeaderContent>(spec, 0, 1000, out total);
        //    var summs = Collator.Instance.Get<Summary>(res.Select(r => r.VersionedId).Cast<ItemId>()).ToList();
        //    ViewData["T1"] = (summs.Count == 0 ? "Pass" : "Fail");

        //    // Search (item in layer below)

        //    LayerManager.Instance.SetUserLayer(wfu, 0);
        //    hc.Summary.Title = "Test Layer 0";
        //    Collator.Instance.Set(rd, hc);

        //    res = SearchManager.Instance.Search<HeaderContent>(spec, 0, 1000, out total);
        //    summs = Collator.Instance.Get<Summary>(res.Select(r => r.VersionedId).Cast<ItemId>()).ToList();
        //    ViewData["T2"] = (summs.Count == 1 && summs[0].Title == "Test Layer 0" ? "Pass" : "Fail");

        //    // Search (item in same layer)

        //    LayerManager.Instance.SetUserLayer(wfu, 10);
        //    hc.Summary.Title = "Test Layer 10";
        //    Collator.Instance.Set(rd, hc);

        //    //SearchManager.Instance.FlushWriter();
        //    //Thread.Sleep(3000);

        //    res = SearchManager.Instance.Search<HeaderContent>(spec, 0, 1000, out total);
        //    summs = Collator.Instance.Get<Summary>(res.Select(r => r.VersionedId).Cast<ItemId>()).ToList();
        //    ViewData["T3"] = (summs.Count == 1 && summs[0].Title == "Test Layer 10" ? "Pass" : "Fail");
        //    ViewData["T3Detail"] = (summs.Count == 1 && summs[0].Title == "Test Layer 10" ? "" : (summs.Count == 0 ? "No summ" : "V: " + summs[0].Version["Layer"] + " T: " + summs[0].Title));

        //    Collator.Instance.Delete(rd, hc);

        //    //SearchManager.Instance.FlushWriter();
        //    //Thread.Sleep(3000);

        //    res = SearchManager.Instance.Search<HeaderContent>(spec, 0, 1000, out total);
        //    summs = Collator.Instance.Get<Summary>(res.Select(r => r.VersionedId).Cast<ItemId>()).ToList();
        //    ViewData["T4"] = (summs.Count == 0 ? "Pass" : "Fail");
        //    ViewData["T4Detail"] = (summs.Count == 0 ? "" : "V: " + summs[0].Version["Layer"] + " T: " + summs[0].Title);



        //    // Verify does exist at layer 15
        //    //LayerManager.Instance.SetUserLayer(wfu, 15);
        //    //hcRead = Collator.Instance.Get<HeaderContent>(rd);
        //    //ViewData["T2"] = (hcRead == null ? "Fail" : "Pass");

        //    // Edit at layer 15
        //    //hcRead.Summary.Title = "Test Layer 15";
        //    //Collator.Instance.Set(rd, hcRead);

        //    return View();
        //}

        public ActionResult Search(TestContent data, string search)
        {
            SearchSpec spec = new SearchSpec { GeneralSearch = search };
            int tot;
            var res = SearchManager.Instance.Search(spec, 0, 100, out tot);
            var x = Repository.Instance.Get<ContentItem>(typeof(HeaderContent), new Guid("DF1DF172-9BD6-4139-B86C-0EBA01EB6A58"));
            var summs = Collator.Instance.Get<Summary>(res.Select(sr => (ItemId)sr.VersionedId));

            return Json(summs.Select(summ => new { summ.Title, summ.Url, summ.Version }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSearchIndex()
        {
            //SearchManager.Instance.BuildIndex(;
            return Content("OK");
        }

        public ActionResult Resize()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HtmlNorm()
        {
            return View("HtmlNorm", new StringInputOutput());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult HtmlNorm(StringInputOutput sio)
        {
            sio.Output = HtmlX.MinimalHtml(sio.Input, true);
            return View("HtmlNorm", sio);
        }

    }
}
