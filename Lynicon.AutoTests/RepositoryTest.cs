using System;
using System.Collections.Generic;
using Lynicon.Repositories;
using Lynicon.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Initialise database with test data
//  use ef directly, use appropriate schema for modules in use
// Attach event handlers to run at end of others, handlers store data for checking in class local vars
// 

namespace Lynicon.AutoTests
{
    [TestClass]
    public class RepositoryTest
    {
        [AssemblyInitialize]
        public static void GlobalInit(TestContext ctx)
        {
            LyniconConfig.Run();
        }

        [ClassInitialize]
        public static void Init(TestContext ctx)
        {

        }

        [TestMethod]
        public void Read()
        {
            var item = Repository.Instance.GetByPath(typeof(HeaderContent), new List<string> { "a" });
            Assert.IsNotNull(item);
        }
    }
}
