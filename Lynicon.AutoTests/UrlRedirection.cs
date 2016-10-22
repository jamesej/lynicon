using System.Threading;
using Lynicon.Base.Models;
using Lynicon.Base.Modules;
using Lynicon.Base.Routing;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Routing;
using Lynicon.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestClass]
    public class UrlRedirection
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {

            //ContentTypeHierarchy.RegisterType(typeof(UrlRedirectContent));
        }

        [TestMethod]
        public void UrlRedirect301()
        {
            if (LyniconModuleManager.Instance.GetModule<UrlListModule>() == null)
                Assert.Inconclusive("No UrlList Module");

            var urc = Collator.Instance.GetNew<UrlRedirectContent>(new Address(typeof(UrlRedirectContent), "a"));
            urc.Title = "URC1";
            urc.AlternateUrls.Add(new AlternativeUrl("urc", UrlRedirectMode.Redirect301));
            Collator.Instance.Set(urc);

            var urlListRoute = new UrlListRoute();
            MockHttpContextBase mockcx = new MockHttpContextBase("~/urc");
            var rd = urlListRoute.GetRouteData(mockcx);

            Assert.IsInstanceOfType(rd.RouteHandler, typeof(RedirectRouteHandler));

            var rrh = rd.RouteHandler as RedirectRouteHandler;
            Assert.AreEqual(rrh.Url, "/aaa/a");
        }

        [TestMethod]
        public void UrlMain()
        {
            if (LyniconModuleManager.Instance.GetModule<UrlListModule>() == null)
                Assert.Inconclusive("No UrlList Module");

            var urc = Collator.Instance.GetNew<UrlRedirectContent>(new Address(typeof(UrlRedirectContent), "x"));
            urc.Title = "URC2";
            urc.AlternateUrls.Add(new AlternativeUrl("urc2", UrlRedirectMode.Main));
            Collator.Instance.Set(urc);

            var urlListRoute = new UrlListRoute();
            MockHttpContextBase mockcx = new MockHttpContextBase("~/urc2");
            Thread.CurrentPrincipal = new MockClaimsPrincipal("U");
            var rd = urlListRoute.GetRouteData(mockcx);

            Assert.IsNotNull(rd, "Failed to find redirect for urc2 via main url");
            Assert.AreEqual(rd.Values["controller"], "mock");
            Assert.AreEqual(rd.Values["action"], "mock");
            Assert.AreEqual(rd.Values["_0"], "x");
            Assert.IsInstanceOfType(rd.Values["data"], typeof(UrlRedirectContent));
            Assert.AreEqual(((UrlRedirectContent)rd.Values["data"]).Title, "URC2");
        }

        [TestMethod]
        public void UrlsWhenUrlChanged()
        {
            if (LyniconModuleManager.Instance.GetModule<UrlListModule>() == null)
                Assert.Inconclusive("No UrlList Module");

            var urc = Collator.Instance.GetNew<UrlRedirectContent>(new Address(typeof(UrlRedirectContent), "b"));
            urc.Title = "URC3";
            urc.AlternateUrls.Add(new AlternativeUrl("aaa/q", UrlRedirectMode.Main));
            urc.AlternateUrls.Add(new AlternativeUrl("urc3", UrlRedirectMode.Redirect301));
            Collator.Instance.Set(urc);

            Collator.Instance.MoveAddress(new ItemId(urc), new Address(typeof(UrlRedirectContent), "q"));

            var gotUrc = Collator.Instance.Get<UrlRedirectContent>(new Address(typeof(UrlRedirectContent), "q"));

            Assert.AreEqual(gotUrc.Title, "URC3");

            var urlListRoute = new UrlListRoute();
            MockHttpContextBase mockcx = new MockHttpContextBase("~/aaa/q");
            Thread.CurrentPrincipal = new MockClaimsPrincipal("U");
            var rd = urlListRoute.GetRouteData(mockcx);

            Assert.AreEqual(rd.Values["controller"], "mock");
            Assert.AreEqual(rd.Values["action"], "mock");
            Assert.AreEqual(rd.Values["_0"], "q");
            Assert.IsInstanceOfType(rd.Values["data"], typeof(UrlRedirectContent));
            Assert.AreEqual(((UrlRedirectContent)rd.Values["data"]).Title, "URC3");

            var mockcx2 = new MockHttpContextBase("~/urc3");
            rd = urlListRoute.GetRouteData(mockcx2);

            Assert.IsInstanceOfType(rd.RouteHandler, typeof(RedirectRouteHandler));

            var rrh = rd.RouteHandler as RedirectRouteHandler;
            Assert.AreEqual(rrh.Url, "/aaa/q");
        }
    }
}
