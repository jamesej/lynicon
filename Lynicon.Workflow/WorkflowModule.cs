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

            VersionManager.Instance.RegisterVersionSetter("Workflow", SetVersion);

            EventHub.Instance.RegisterEventProcessor("Repository.Get", ProcessGet, "Workflow");
            EventHub.Instance.RegisterEventProcessor("Repository.Set", ProcessSet, "Workflow");
            
            LyniconUi.Instance.RevealPanelViews.Add(
                new KeyValuePair<string, string>("Workflow", "~/Areas/Lynicon.Workflow/Views/Shared/WorkflowPanel.ascx"),
                "Core" );

            context.MapRoute("lyniconworkflowmain",
                "Lynicon/Workflow/{action}",
                new { controller = "Workflow" });

            return true;
        }

        public void SetVersion(Dictionary<string, object> version)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null)
                version["Layer"] = LayerManager.Instance.LiveLayer;
            else
                version["Layer"] = wfUser.CurrentLevel;
        }

        public object ProcessGet(EventHubData ehd)
        {
            int currLayer;
            switch (ehd.EventName)
            {
                //case "Repository.Get.Item.Id":
                //    var d = ehd.Data as QueryEventData<ContentItem, ContentItem>;
                //    if (d != null)
                //    {
                //        d.Source = d.Source.AsFacade<WorkflowContentItem>()
                //            .Where(wci => wci.Layer <= currLayer)
                //            .OrderByDescending(wci => wci.Layer);
                //        return d;
                //    }
                //    break;
                case "Repository.Get.Items.Ids":
                case "Repository.Get.Count":
                    var d2 = ehd.Data as QueryEventData<ContentItem, IQueryable<ContentItem>>;
                    if (d2 != null)
                    {
                        currLayer = (int)VersionManager.Instance.CurrentVersion["Layer"];
                        d2.Source = d2.Source.AsFacade<WorkflowContentItem>()
                            .GroupBy(wci => wci.Identity)
                            .Select(wcig => wcig.Where(wci => wci.Layer <= currLayer)
                                .OrderByDescending(wci => wci.Layer)
                                .FirstOrDefault())
                            .Where(wci => wci != null && wci.Content != null)
                            .AsFacade<ContentItem>();
                        return d2;
                    }
                    break;
                case "Repository.Get.Items.Summaries":
                    var d3 = ehd.Data as QueryEventData<ContentItem, IQueryable>;
                    if (d3 != null)
                    {
                        currLayer = (int)VersionManager.Instance.CurrentVersion["Layer"];
                        d3.Source = d3.Source.AsFacade<WorkflowContentItem>()
                            .GroupBy(wci => wci.Identity)
                            .Select(wcig => wcig.Where(wci => wci.Layer <= currLayer)
                                .OrderByDescending(wci => wci.Layer)
                                .FirstOrDefault())
                            .Where(wci => wci != null && wci.Content != null)
                            .AsFacade<ContentItem>();
                        return d3;
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

            var currLayer = (int)VersionManager.Instance.CurrentVersion["Layer"];

            // if we're on a different layer from the one where the current record came from, we create a new record
            // on the new layer rather than updating the old one
            if (ehd.EventName == "Repository.Set.Update" && d.Layer != currLayer)
                d.Id = Guid.Empty;

            return d;
        }
    }
}
