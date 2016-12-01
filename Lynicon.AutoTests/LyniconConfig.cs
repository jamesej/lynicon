using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.AutoTests.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Modules;
using Lynicon.Repositories;
using Lynicon.Routing;
using Lynicon.Test.Models;

namespace Lynicon.AutoTests
{
    public class LyniconConfig
    {
        public static void RegisterModules()
        {
            LyniconModuleManager.Instance.SkipDbStateCheck = true;

            LyniconModuleManager.Instance.RegisterModule(new CoreModule());
            LyniconModuleManager.Instance.RegisterModule(new ContentSchemaModule());

            //var searchModule = new SearchModule(t => t == typeof(HeaderContent));
            //searchModule.DontRebuild = true;
            //LyniconModuleManager.Instance.RegisterModule(searchModule);

            LyniconModuleManager.Instance.ValidateModules();
        }

        public static void InitialiseDataApi()
        {
            Collator.Instance.SetupTypeForBasic<TestData>();
            Collator.RegisterExtensionType(typeof(TestData));
            Collator.RegisterExtensionType(typeof(TestDataX));
            Collator.Instance.SetupTypeForBasic<PathAddressData>();
            Collator.Instance.SetupTypeForBasic<SplitAddressData>();

            Repository.Instance.Register(null, new ContentRepository(new MockDataSourceFactory()));
            Repository.Instance.Register(typeof(TestData), new BasicRepository(new MockDataSourceFactory()));
            Repository.Instance.Register(typeof(ContentItem), new ContentRepository(new MockDataSourceFactory()));
            Repository.Instance.Register(typeof(User), new UserRepository(new MockDataSourceFactory()));

            Collator.Instance.BuildRepository();
        }

        public static void Initialise()
        {
            // Set up data types here
            ContentTypeHierarchy.RegisterType(typeof(HeaderContent));
            ContentTypeHierarchy.RegisterType(typeof(HeaderContent2));
            ContentTypeHierarchy.RegisterType(typeof(TestData));
            ContentTypeHierarchy.RegisterType(typeof(Sub1TContent));
            ContentTypeHierarchy.RegisterType(typeof(Sub2TContent));
            ContentTypeHierarchy.RegisterType(typeof(RefContent));
            ContentTypeHierarchy.RegisterType(typeof(RefTargetContent));
            ContentTypeHierarchy.RegisterType(typeof(PathAddressData));
            ContentTypeHierarchy.RegisterType(typeof(SplitAddressData));
            ContentTypeHierarchy.RegisterType(typeof(RestaurantContent));

            LyniconModuleManager.Instance.Initialise();

        }

        public static void Shutdown()
        {
            LyniconModuleManager.Instance.Shutdown();
        }

        public static void MockRoutes()
        {
            RouteTable.Routes.RouteExistingFiles = true;
            RouteTable.Routes.AddDataRoute<HeaderContent>("header", "header/{_0}", new { controller = "mock", action = "mock" });
            RouteTable.Routes.AddDataRoute<TestData>("test-data", "testd/{_0}", new { controller = "mock", action = "mock" });
            RouteTable.Routes.MapRoute("static-route", "header/ut-x", new { controller = "mock", action = "mock" });
        }

        public static void Run()
        {
            RegisterModules();
            InitialiseDataApi();
            MockRoutes();
            Initialise();
        }
    }
}
