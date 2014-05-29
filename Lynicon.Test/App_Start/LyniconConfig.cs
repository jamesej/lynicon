using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Base.Models;
using Lynicon.Base.Modules;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Map;
using Lynicon.Repositories;
using Lynicon.Search;
using Lynicon.Test.Models;

namespace Lynicon.Test
{
    public class LyniconConfig
    {
        public static void Initialise()
        {
            // Set up data types here

            // Initialise caches
            LyniconModuleManager.Instance.RegisterModule(new SummaryCache(null));
            LyniconModuleManager.Instance.RegisterModule(new Auditing(null));
            LyniconModuleManager.Instance.RegisterModule(new Publishing(null));

            Collator.RegisterExtensionType(typeof(ExtendedUser));

            Collator.Instance.BuildRepository();
            LyniconModuleManager.Instance.Initialise();

            DalTrack.Instance.Initialise();
            NavManager.Instance.RegisterNavManager(new UrlTreeNavManager<WikiContent>());
            SearchManager.Instance.Initialise(new List<Type> { typeof(HeaderContent) });
        }
    }
}