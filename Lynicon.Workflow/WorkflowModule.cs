using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Extensibility;
using Lynicon.Routing;
using Lynicon.Membership;
using Lynicon.Collation;
using Lynicon.Repositories;
using Lynicon.Editors;
using Lynicon.Linq;
using Lynicon.Workflow.Models;
using Lynicon.Utility;

namespace Lynicon
{
    public class WorkflowModule : Module
    {
        public WorkflowModule(AreaRegistrationContext context, params string[] dependentOn)
            : base("Workflow", context, dependentOn)
        {
        }

        public override bool Initialise(AreaRegistrationContext context)
        {
            if (!VerifyDbState("LyniconWorkflow 0.0"))
                return false;

            VersionManager.Instance.RegisterVersion("Workflow", SetCurrentVersion, GetItemVersion, SetItemVersion);

            EventHub.Instance.RegisterEventProcessor("Repository.Get", ProcessGet, "Workflow",
                new OrderConstraint("Workflow", "Caching"));
            EventHub.Instance.RegisterEventProcessor("Repository.Set", ProcessSet, "Workflow",
                new OrderConstraint("Workflow", ConstraintType.ItemsAfter, "Caching"));
            
            LyniconUi.Instance.RevealPanelViews.Add(
                new KeyValuePair<string, string>("Workflow", "~/Areas/Lynicon.Workflow/Views/Shared/WorkflowPanel.ascx"),
                "Core" );

            context.MapRoute("lyniconworkflowmain",
                "Lynicon/Workflow/{action}",
                new { controller = "Workflow" });

            return true;
        }

        public void SetCurrentVersion(ItemVersion version)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null)
                version["Layer"] = LayerManager.Instance.LiveLayer;
            else
                version["Layer"] = wfUser.CurrentLevel;
        }
        public void GetItemVersion(object o, ItemVersion version)
        {
            if (o is ILayered)
                version["Layer"] = (o as ILayered).Layer;
        }
        public void SetItemVersion(ItemVersion version, object o)
        {
            if (o is ILayered && version.ContainsKey("Layer"))
                ((ILayered)o).Layer = (int)version["Layer"];
        }

        public object ProcessGet(EventHubData ehd)
        {
            int currLayer;
            switch (ehd.EventName)
            {
                case "Repository.Get.Items.Ids":
                case "Repository.Get.Items.Paths":
                case "Repository.Get.Summaries.Ids":
                case "Repository.Get.Summaries":
                case "Repository.Get.Count":
                    var d2 = ehd.Data as IQueryEventData<IQueryable>;
                    if (d2 != null && typeof(ILayered).IsAssignableFrom(d2.Source.ElementType))
                    {
                        var version = VersionManager.Instance.CurrentVersion;

                        // No layer version registered, or versioning suppressed
                        if (!version.ContainsKey("Layer"))
                            return d2;

                        currLayer = (int)version["Layer"];
                        d2.Source = d2.Source.AsFacade<ILayered>()
                            .GroupBy(wci => wci.Identity)
                            .Select(wcig => wcig.Where(wci => wci.Layer <= currLayer)
                                .OrderByDescending(wci => wci.Layer)
                                .FirstOrDefault())
                            .Where(wci => wci != null && !wci.IsDeletion);
                        return d2;
                    }
                    break;
            }

            return ehd.Data;
        }

        public object ProcessSet(EventHubData ehd)
        {
            var d = ehd.Data as ILayered;
            if (d == null)
                return ehd.Data;

            var version = VersionManager.Instance.CurrentVersion;

            // No layer version registered, or versioning suppressed
            if (!version.ContainsKey("Layer"))
                return d;

            var currLayer = (int)version["Layer"];

            // if we're on a different layer from the one where the current record came from, we create a new record
            // on the new layer rather than updating the old one
            if (ehd.EventName == "Repository.Set.Update" && d.Layer != currLayer)
            {
                d = (ILayered)ReflectionX.CopyEntity(d);
                d.Id = Guid.Empty;
                d.IsUpdate = true;
                ehd.EventName = "Repository.Set.Add";
            }
            if (ehd.EventName == "Repository.Set.Delete")
            {
                d = (ILayered)ReflectionX.CopyEntity(d);
                d.Id = Guid.Empty;
                d.IsDeletion = true;
                ehd.EventName = "Repository.Set.Add";
            }
            if (d.Layer != currLayer)
                d.Layer = currLayer;


            return d;
        }
    }
}
