using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Repositories;

namespace Lynicon.Collation
{
    public class DataApi
    {
        static readonly DataApi instance = new DataApi();
        public static DataApi Instance { get { return instance; } }

        static DataApi() { }

        public Repository Repository { get; set; }

        public Collator Collator { get; set; }

        public EventHub EventHub { get; set; }

        public LyniconModuleManager ModuleManager { get; set; }


    }
}
