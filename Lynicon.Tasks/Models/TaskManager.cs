using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Lynicon.Collation;
using Lynicon.Membership;
using Lynicon.Models;

namespace Lynicon.Tasks.Models
{
    public class TaskManager
    {
        static readonly TaskManager instance = new TaskManager();
        public static TaskManager Instance { get { return instance; } }

        static TaskManager() { }

        public Dictionary<Type, TaskFlow> TaskFlows { get; set; }

        public TaskManager()
        {
            TaskFlows = new Dictionary<Type, TaskFlow>();
        }

        public void Initialise() {}

        public void RegisterTaskFlow(TaskFlow taskFlow)
        {
            foreach (Type contentType in ContentTypeHierarchy.AllContentTypes)
            {
                if (!TaskFlows.ContainsKey(contentType) && taskFlow.AppliesToType(contentType))
                    TaskFlows.Add(contentType, taskFlow);
            }
        }

        public List<ItemTask> AvailableTasks(Guid identity, Type type)
        {
            var activeTasks = Collator.Instance
                .Get<ItemTask, ItemTask>(iq => iq.Where(it => it.ItemIdentity == identity && it.CompletedDate == null))
                .ToList();
            var taskSpecs = TaskFlows[type].TaskSpecs;
            var roles = Roles.GetRolesForUser();
            var completableTasks = activeTasks
                .Where(it => taskSpecs[it.TaskName].RolesCanComplete.ToCharArray().Any(c => roles.Contains(c.ToString())))
                .ToList();
            var createableTasks = activeTasks
                .SelectMany(it => taskSpecs[it.TaskName]
                                    .EnablesTasks.Where(t => taskSpecs[t].RolesCanCreate.ToCharArray().Any(c => roles.Contains(c.ToString()))))
                .Distinct()
                .ToList();
            return null;
        }
    }
}
