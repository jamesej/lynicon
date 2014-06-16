using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Base.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Routing;
using Lynicon.Tasks.Models;

namespace Lynicon.Base
{
    public class TasksModule : Module
    {
        public Dictionary<Type, TaskFlow> TaskFlows { get; set; }
        public TasksModule(params string[] dependentOn)
            : base("Tasks", dependentOn)
        {
            if (!VerifyDbState("LyniconTasks 0.0"))
            {
                this.Blocked = true;
                return;
            }

            TaskFlows = new Dictionary<Type, TaskFlow>();

            Collator.Instance.SetupType(typeof(ItemTask), new BasicCollator(), new BasicRepository());
        }

        public override bool Initialise()
        {
            var taskSelector = new FuncPanelButton { ViewName = "~/Areas/Lynicon.Tasks/Views/Shared/TaskSelector.ascx", RequiredRoles = "E" };
            LyniconUi.Instance.FuncPanelButtons.Add(taskSelector);

            return true;
        }

        public void RegisterTaskFlow(TaskFlow taskFlow)
        {
            foreach (Type contentType in ContentTypeHierarchy.AllContentTypes)
            {
                if (!TaskFlows.ContainsKey(contentType) && taskFlow.AppliesToType(contentType))
                    TaskFlows.Add(contentType, taskFlow);
            }
        }
    }
}
