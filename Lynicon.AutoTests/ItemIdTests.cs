using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Lynicon.Attributes;
using Lynicon.AutoTests.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Test.Models;
using NUnit.Framework;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestFixture]
    public class ItemIdTests
    {
        [Test]
        public void ItemIdEquality()
        {
            Guid id0 = Guid.NewGuid();
            var ii0 = new ItemId(typeof(HeaderContent), id0);
            var ii1 = new ItemId(typeof(HeaderContent), new Guid(id0.ToString()));
            var ii2 = new ItemId(typeof(RestaurantContent), id0);
            var ii3 = new ItemId(typeof(HeaderContent), Guid.NewGuid());

            Assert.IsTrue(ii0.Equals(ii1), ".Equals true");
            Assert.IsTrue(ii0 == ii1, "== true");
            Assert.IsFalse(ii0.Equals(ii2), ".Equals false by type");
            Assert.IsFalse(ii0 == ii2, "== false by type");
            Assert.IsFalse(ii1.Equals(ii3), ".Equals false by id");
            Assert.IsFalse(ii1 == ii3, "== false by id");

            Assert.IsFalse(ii0.GetHashCode() == ii2.GetHashCode(), "hash code by type");
            Assert.IsFalse(ii1.GetHashCode() == ii3.GetHashCode(), "hash code by id");
        }

        [Test]
        public void ItemIdConstructors()
        {
            Guid id1 = Guid.NewGuid();
            var ii1 = new ItemId(typeof(HeaderContent), id1);

        }

    }
}
