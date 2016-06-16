using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Base.Modules;
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
            LyniconModuleManager.Instance.RegisterModule(new UrlListModule(t => t != typeof(User)));
            LyniconModuleManager.Instance.RegisterModule(new SummaryCache(t =>
                t != typeof(User) && t != typeof(TestData)
                ));
            LyniconModuleManager.Instance.RegisterModule(new Auditing(TimeSpan.FromDays(90)));
            LyniconModuleManager.Instance.RegisterModule(new Publishing());
            LyniconModuleManager.Instance.RegisterModule(new SoftDelete());
            var transfer = new Transfer(t => true, Publishing.PublishedVersion);
            LyniconModuleManager.Instance.RegisterModule(transfer);
            LyniconModuleManager.Instance.RegisterModule(new Sitemap());
            LyniconModuleManager.Instance.RegisterModule(new References());
            var search = new SearchModule();
            search.DontRebuild = true;
            LyniconModuleManager.Instance.RegisterModule(search);

            //var searchModule = new SearchModule(t => t == typeof(HeaderContent));
            //searchModule.DontRebuild = true;
            //LyniconModuleManager.Instance.RegisterModule(searchModule);

            LyniconModuleManager.Instance.ValidateModules();
        }

        public static void InitialiseDataApi()
        {
            Collator.Instance.SetupTypeForBasic<TestData>();
            Collator.RegisterExtensionType(typeof(TestData));

            Collator.Instance.BuildRepository();
        }

        public static void Initialise()
        {
            // Set up data types here
            ContentTypeHierarchy.RegisterType(typeof(HeaderContent));
            ContentTypeHierarchy.RegisterType(typeof(TestData));
            ContentTypeHierarchy.RegisterType(typeof(Sub1TContent));
            ContentTypeHierarchy.RegisterType(typeof(Sub2TContent));
            ContentTypeHierarchy.RegisterType(typeof(SearchContent));
            ContentTypeHierarchy.RegisterType(typeof(RefContent));
            ContentTypeHierarchy.RegisterType(typeof(RefTargetContent));

            LyniconModuleManager.Instance.Initialise();

        }

        public static void Shutdown()
        {
            LyniconModuleManager.Instance.Shutdown();
        }

        public static void MockRoutes()
        {
            RouteTable.Routes.RouteExistingFiles = true;
            RouteTable.Routes.AddDataRoute<UrlRedirectContent>("urlred", "aaa/{_0}", new { controller = "mock", action = "mock" });
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
