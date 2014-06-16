using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Base;
using Lynicon.Base.Models;
using Lynicon.Base.Modules;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Map;
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
            LyniconModuleManager.Instance.RegisterModule(new CoreModule());

            LyniconModuleManager.Instance.RegisterModule(new SummaryCache());
            LyniconModuleManager.Instance.RegisterModule(new Auditing());
            LyniconModuleManager.Instance.RegisterModule(new Publishing());
            LyniconModuleManager.Instance.RegisterModule(new TasksModule());

            LyniconModuleManager.Instance.ValidateModules();
        }
        public static void Initialise()
        {
            // Set up data types here
            Collator.RegisterExtensionType(typeof(ExtendedUser));

            Collator.Instance.BuildRepository();
            LyniconModuleManager.Instance.Initialise();

            BuildTaskflows();

            DalTrack.Instance.Initialise();
            NavManager.Instance.RegisterNavManager(new UrlTreeNavManager<WikiContent>());
            SearchManager.Instance.Initialise(new List<Type> { typeof(HeaderContent) });
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

            LyniconModuleManager.Instance.GetModule<TasksModule>().RegisterTaskFlow(testFlow);
        }
    }
}