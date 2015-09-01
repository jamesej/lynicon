using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Base.Modules;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Modules;
using Lynicon.Repositories;

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
                t != typeof(User)
                ));
            LyniconModuleManager.Instance.RegisterModule(new Auditing());
            LyniconModuleManager.Instance.RegisterModule(new Publishing());
            LyniconModuleManager.Instance.RegisterModule(new SoftDelete());
            var transfer = new Transfer(t => true, Publishing.PublishedVersion);
            LyniconModuleManager.Instance.RegisterModule(transfer);
            LyniconModuleManager.Instance.RegisterModule(new Sitemap());

            //var searchModule = new SearchModule(t => t == typeof(HeaderContent));
            //searchModule.DontRebuild = true;
            //LyniconModuleManager.Instance.RegisterModule(searchModule);

            LyniconModuleManager.Instance.ValidateModules();
        }

        public static void InitialiseDataApi()
        {
            //Collator.RegisterExtensionType(typeof(ExtendedUser));

            Collator.Instance.BuildRepository();
        }

        public static void Initialise()
        {
            // Set up data types here

            LyniconModuleManager.Instance.Initialise();

        }

        public static void Shutdown()
        {
            LyniconModuleManager.Instance.Shutdown();
        }

        public static void Run()
        {
            RegisterModules();
            InitialiseDataApi();
            Initialise();
        }
    }
}
