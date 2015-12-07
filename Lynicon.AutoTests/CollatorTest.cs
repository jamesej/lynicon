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

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestClass]
    public class CollatorTest
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            var db = new CoreDb();
            db.Database.ExecuteSqlCommand("DELETE FROM ContentItems WHERE DataType IN ('Lynicon.Test.Models.HeaderContent', 'Lynicon.Test.Models.Sub1TContent', 'Lynicon.Test.Models.Sub2TContent')");
            db.Database.ExecuteSqlCommand("DELETE FROM TestData");
        }

        [TestMethod]
        public void WriteRead()
        {
            var hc = Collator.Instance.GetNew<HeaderContent>(new Address(typeof(HeaderContent), "a"));

            hc.Summary.Title = "Header A";
            hc.Summary.Image.Url = "/abc.gif";
            hc.HeaderBody = "xyz";

            Collator.Instance.Set(hc);

            var item = Collator.Instance.Get<HeaderContent>(new Address(typeof(HeaderContent), "a"));
            Assert.IsNotNull(item, "Get by path");

            var itemId = new ItemId(item);
            var item2 = Collator.Instance.Get<HeaderContent>(itemId);
            Assert.IsNotNull(item2, "Get by Id");
            Assert.AreEqual(item.Summary.Title, item2.Summary.Title, "Get right item by Id");
        }

        [TestMethod]
        public void WriteReadBasic()
        {
            var td = Collator.Instance.GetNew<TestData>(new Address(typeof(TestData), "x"));
            td.Value1 = "nnn";
            td.Path = "x";
            td.Id = 1;
            Collator.Instance.Set(td, true);

            var item = Collator.Instance.Get<TestData>(new Address(typeof(TestData), "x"));
            Assert.IsNotNull(item, "GetByPath");

            var itemId = new ItemId(item);
            var item2 = Collator.Instance.Get<TestData>(itemId);
            Assert.IsNotNull(item2, "Get by Id");
            Assert.AreEqual(item2.Id, item.Id, "Get right item by Id");

            var item3 = Collator.Instance.Get<TestData>(new Address(typeof(TestData), "x"));
            Assert.IsNotNull(item3, "Get by Address");
            Assert.AreEqual(item3.Id, item.Id, "Get right item by Address");
        }

        [TestMethod]
        public void Polymorphic()
        {
            var s1 = Collator.Instance.GetNew<Sub1TContent>(new Address(typeof(Sub1TContent), "s1"));
            s1.Title = "Sub1";
            s1.SomeStuff = new Models.MinHtml("<b>this</b>");
            var s2 = Collator.Instance.GetNew<Sub2TContent>(new Address(typeof(Sub2TContent), "s2"));
            s2.Title = "Sub2";
            s2.Links.Add(new Models.Link { Content = "linky", Url = "/abc" });
        }
    }
}
