using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Base;
using Lynicon.Base.Models;
using Lynicon.Base.Modules;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Extensions;
using Lynicon.Map;
using Lynicon.Membership;
using Lynicon.Modules;
using Lynicon.Repositories;
using Lynicon.Search;
using Lynicon.Tasks.Models;
using Lynicon.Test.Models;

namespace Lynicon.Test
{
    public class LyniconConfig
    {
        public static void RegisterModules()
        {
            LyniconModuleManager.Instance.SkipDbStateCheck = true;

            var fullCache = new FullCache();
            Repository.Instance.EFProxyCreationEnabled = false;
            //Repository.Instance.ReadWriteDisabled = true;
            //fullCache.PersistToFile = false;

            LyniconModuleManager.Instance.RegisterModule(new CoreModule());
            LyniconModuleManager.Instance.RegisterModule(new ContentSchemaModule());
            LyniconModuleManager.Instance.RegisterModule(new UrlListModule(t => t != typeof(User)));
            LyniconModuleManager.Instance.RegisterModule(new FullCache(t => t != Repository.Instance.ContainerType(typeof(User))));
            LyniconModuleManager.Instance.RegisterModule(new Auditing());
            LyniconModuleManager.Instance.RegisterModule(new Publishing());
            LyniconModuleManager.Instance.RegisterModule(new TasksModule());
            LyniconModuleManager.Instance.RegisterModule(new SoftDelete());
            var transfer = new Transfer(t => true, Publishing.PublishedVersion);
            transfer.AutoTransferType = t => t == typeof(TagContent);
            LyniconModuleManager.Instance.RegisterModule(transfer);
            LyniconModuleManager.Instance.RegisterModule(new Sitemap());

            var searchModule = new SearchModule(t => t == typeof(HeaderContent));
            searchModule.DontRebuild = true;
            LyniconModuleManager.Instance.RegisterModule(searchModule);

            LyniconModuleManager.Instance.RegisterModule(new DomainPartition(
                new DomainPartitionInfo[] {
                    new DomainPartitionInfo { PartitionId = 1, Domain = "localhost", Code = "LH", Name = "Localhost" },
                    new DomainPartitionInfo { PartitionId = 2, Domain = "www.gic.com", Code = "GIC", Name = "GIC" }
                }));

            LyniconModuleManager.Instance.ValidateModules();
        }

        public static void InitialiseDataApi()
        {
            Collator.RegisterExtensionType(typeof(ExtendedUser));

            Collator.Instance.BuildRepository();
        }

        public static void Initialise()
        {
            // Set up data types here

            LyniconModuleManager.Instance.Initialise();
            ViewEngines.Engines.Insert(0, new DynamicViewEngine());

            BuildTaskflows();

            //DalTrack.Instance.Initialise();
        }

        public static void Shutdown()
        {
            LyniconModuleManager.Instance.Shutdown();
        }
        public static void BuildTaskflows()
        {
            var testFlow = new TaskFlow();
            testFlow.AppliesToType = t => t == typeof(ChefContent);
            var testCreate = new TaskSpec
            {
                Name = "Create",
                IsCreateTask = true,
                RestrictFields = new List<string> { "FirstName", "LastName" },
                CompleteFields = new List<string> { "FirstName", "LastName" },
                StartsTasks = new List<string> { "Edit" }
            };
            var testEdit = new TaskSpec
            {
                Name = "Edit",
                StartsTasks = new List<string> { "Publish" }
            };
            var testPublish = new TaskSpec
            {
                Name = "Publish",
                IsPublishableIfActive = true
            };
            testFlow.TaskSpecs.Add("Create", testCreate);
            testFlow.TaskSpecs.Add("Edit", testEdit);
            testFlow.TaskSpecs.Add("Publish", testPublish);

            TaskManager.Instance.RegisterTaskFlow(testFlow);
        }
    }
}