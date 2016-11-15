using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Map;
using Lynicon.Membership;
using Lynicon.Modules;
using Lynicon.Repositories;

namespace Lynicon.Site
{
    public class LyniconConfig
    {
        public static void RegisterModules()
        {
            LyniconModuleManager.Instance.RegisterModule(new CoreModule());
            
            // Register other modules here

            LyniconModuleManager.Instance.ValidateModules();
        }

        public static void InitialiseDataApi()
        {
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
    }
}