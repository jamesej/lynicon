using System;
using System.Collections.Generic;
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
    public class EqualityTests
    {
        [Test]
        public void ItemId()
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
        public void ItemVersion()
        {
            Dictionary<string, object> vers = new Dictionary<string, object> { { "Existence", "Exists" }, { "Published", false } };
            var ii0 = new ItemVersion(vers);
            var ii1 = new ItemVersion(vers);
            vers["Published"] = true;
            var ii2 = new ItemVersion(vers);
            vers.Remove("Published");
            var ii3 = new ItemVersion(vers);
            vers["Partition"] = null;
            var ii4 = new ItemVersion(vers);
            var ii5 = new ItemVersion(vers);

            Assert.IsTrue(ii0.Equals(ii1), ".Equals true");
            Assert.IsTrue(ii0 == ii1, "== true");
            Assert.IsFalse(ii0.Equals(ii2), ".Equals false by different val");
            Assert.IsFalse(ii0 == ii2, "== false by different val");
            Assert.IsFalse(ii1.Equals(ii3), ".Equals false by missing key");
            Assert.IsFalse(ii1 == ii3, "== false by missing key");

            Assert.IsFalse(ii0.GetHashCode() == ii2.GetHashCode(), "hash code by val");
            Assert.IsFalse(ii1.GetHashCode() == ii3.GetHashCode(), "hash code by missing key");

            Assert.IsTrue(ii3 == ii4, "== ignore null value");
            Assert.IsTrue(ii3.GetHashCode() == ii4.GetHashCode(), "hash code ignore null value");

            Assert.IsTrue(ii5 == ii4, "== compare null value");
            Assert.IsTrue(ii5.GetHashCode() == ii4.GetHashCode(), "hash code compare null value");
        }

        [Test]
        public void Address()
        {
            ContentTypeHierarchy.RegisterType(typeof(HeaderContent));
            ContentTypeHierarchy.RegisterType(typeof(RestaurantContent));

            Dictionary<string, object> address = new Dictionary<string, object> { { "Existence", "Exists" }, { "Published", false } };
            var ii0 = new Address(typeof(HeaderContent), "abc&def");
            var ii1 = new Address(typeof(HeaderContent), "abc&def");
            var ii2 = new Address(typeof(RestaurantContent), "abc&def");
            var ii3 = new Address(typeof(HeaderContent), "abc");

            Assert.IsTrue(ii0.Equals(ii1), ".Equals true");
            Assert.IsTrue(ii0 == ii1, "== true");
            Assert.IsFalse(ii0.Equals(ii2), ".Equals false by type");
            Assert.IsFalse(ii0 == ii2, "== false by different val");
            Assert.IsFalse(ii1.Equals(ii3), ".Equals false by missing key");
            Assert.IsFalse(ii1 == ii3, "== false by missing key");

            Assert.IsFalse(ii0.GetHashCode() == ii2.GetHashCode(), "hash code by type");
            Assert.IsFalse(ii1.GetHashCode() == ii3.GetHashCode(), "hash code by missing key");
        }
    }
}
