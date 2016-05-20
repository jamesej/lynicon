using System;
using System.Threading;
using Lynicon.Base.Routing;
using Lynicon.Base.Search;
using Lynicon.Collation;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Routing;
//using Lynicon.Search;
using Lynicon.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestClass]
    public class SearchTest
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            var db = new CoreDb();
            db.Database.ExecuteSqlCommand("DELETE FROM ContentItems WHERE DataType = 'Lynicon.Test.Models.SearchContent'");

            var s1 = Collator.Instance.GetNew<SearchContent>(new Address(typeof(SearchContent), "s1"));
            s1.Body = new MinHtml("<p>Here is some text with plurals and a few accénts thrown in.  CAPITALISATION.</p>");
            s1.Date = new DateTime(2015, 1, 1);
            s1.Title = "Search Test S1";
            Collator.Instance.Set(s1);

            var s2 = Collator.Instance.GetNew<SearchContent>(new Address(typeof(SearchContent), "s2"));
            s2.Body = new MinHtml("<p>This is another <b>bit</b> of text to try.</p>");
            s2.Date = new DateTime(2016, 10, 21);
            s2.Title = "Search Test S2";
            Collator.Instance.Set(s2);

            var s3 = Collator.Instance.GetNew<SearchContent>(new Address(typeof(SearchContent), "s3"));
            s3.Body = new MinHtml("<p>This will have been deleted.</p>");
            s3.Date = new DateTime(2016, 10, 22);
            s3.Title = "Search Test S3";
            Collator.Instance.Set(s3);

            SearchManager.Instance.Initialise(true);

            var sPost = Collator.Instance.GetNew<SearchContent>(new Address(typeof(SearchContent), "sPost"));
            sPost.Body = new MinHtml("<p>Post creation update</p>");
            sPost.Date = new DateTime(2016, 9, 11);
            sPost.Title = "Search Test Post Update";
            Collator.Instance.Set(sPost);

            Collator.Instance.Delete(s3);

            var s4 = Collator.Instance.GetNew<SearchContent>(new Address(typeof(SearchContent), "s4"));
            s4.Body = new MinHtml("<p>This is added dynamically.</p>");
            s4.Date = new DateTime(2016, 10, 23);
            s4.Title = "Search Test S4";
            Collator.Instance.Set(s4);

            // ensure update has time to get into index
            Thread.Sleep(4000);
        }

        [TestMethod]
        public void StaticSearch()
        {
            var csr = ClientSearch.Instance.Search("plural");
            Assert.AreEqual(1, csr.Total, "Match count wrong single match singular search, match plural word");
            csr = ClientSearch.Instance.Search("bit");
            Assert.AreEqual(1, csr.Total, "Match count text in MinHtml in markup");
            csr = ClientSearch.Instance.Search("text");
            Assert.AreEqual(2, csr.Total, "Multiple match");
            csr = ClientSearch.Instance.Search("capitalisation");
            Assert.AreEqual(1, csr.Total, "Match count capitalisation");
            csr = ClientSearch.Instance.Search("accents");
            Assert.AreEqual(1, csr.Total, "Match count accents");
            csr = ClientSearch.Instance.Search("anither");
            Assert.AreEqual(0, csr.Total, "No match");
        }

        [TestMethod]
        public void DataSearch()
        {
            var ss = new SearchSpec();
            ss.AddFilterField("Date", new DateTime(2016, 10, 21).ToString());
            int total;
            var sris = SearchManager.Instance.Search(ss, 0, 100, out total);

        }

        [TestMethod]
        public void SearchUpdate()
        {
            var csr = ClientSearch.Instance.Search("Update");
            Assert.AreEqual(1, csr.Total, "Post update match");

            csr = ClientSearch.Instance.Search("deleted");
            Assert.AreEqual(0, csr.Total, "Post delete match");

            csr = ClientSearch.Instance.Search("dynamically");
            Assert.AreEqual(1, csr.Total, "Post add match");
        }

    }
}
