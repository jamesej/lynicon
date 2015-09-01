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
using Lynicon.Models;

namespace Lynicon.Workflow
{
    public class WorkflowModule : Module
    {
        public const string VersionKey = "Layer";

        public WorkflowModule(params string[] dependentOn)
            : base("Workflow", dependentOn)
        {
            Collator.RegisterExtensionType(typeof(WorkflowUser));
        }

        public override void RegisterRoutes(AreaRegistrationContext regContext)
        {
            regContext.MapRoute("lyniconworkflowmain",
                "Lynicon/Workflow/{action}",
                new { controller = "Workflow" });
        }

        public override bool Initialise()
        {
            if (!VerifyDbState("LyniconWorkflow 0.0"))
                return false;

            VersionManager.Instance.RegisterVersion(VersionKey, false, new WorkflowVersioner(VersionKey), null);

            EventHub.Instance.RegisterEventProcessor("Repository.Get", ProcessGet, "Workflow",
                new OrderConstraint("Workflow", "Caching"));
            EventHub.Instance.RegisterEventProcessor("Repository.Set", ProcessSet, "Workflow",
                new OrderConstraint("Workflow", ConstraintType.ItemsAfter, "Caching", "Auditing"));
            
            LyniconUi.Instance.RevealPanelViews.Add(
                new KeyValuePair<string, string>("Workflow", "~/Areas/Lynicon.Workflow/Views/Shared/WorkflowPanel.ascx"),
                "Core" );

            return true;
        }

        public void SetCurrentVersion(VersioningMode mode, ItemVersion version)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null || mode == VersioningMode.Public)
                version[VersionKey] = LayerManager.Instance.LiveLayer;
            else
                version[VersionKey] = wfUser.CurrentLevel;
        }
        public void GetItemVersion(object o, ItemVersion version)
        {
            if (o is ILayered)
                version[VersionKey] = (o as ILayered).Layer;
        }
        public void SetItemVersion(ItemVersion version, object o)
        {
            if (o is ILayered && version.ContainsKey(VersionKey))
                ((ILayered)o).Layer = (int)version[VersionKey];
        }

        public VersionDisplay DisplayItemVersion(ItemVersion version)
        {
            return new VersionDisplay
            {
                Text = "Layer " + version[VersionKey].ToString(),
                CssClass = "workflow-version-display",
                Title = "Layer of current item"
            };
        }
        public bool TestVersioningMode(object container, VersioningMode mode)
        {
            if (!(container is ILayered))
                return true;

            int testLevel = 0;
            switch (mode)
            {
                case VersioningMode.Public:
                    testLevel = LayerManager.Instance.LiveLayer;
                    break;
                case VersioningMode.Current:
                    if (VersionManager.Instance.CurrentVersion.ContainsKey(VersionKey))
                        testLevel = (int)VersionManager.Instance.CurrentVersion[VersionKey];
                    else
                        testLevel = LayerManager.Instance.LiveLayer;
                    break;
                case VersioningMode.Specific:
                    if (VersionManager.Instance.SpecificVersion.ContainsKey(VersionKey))
                        testLevel = (int)VersionManager.Instance.SpecificVersion[VersionKey];
                    else
                        testLevel = LayerManager.Instance.LiveLayer;
                    break;
                default:
                    return true;
            }

            // Will the ILayered be shown at the testLevel?
            var layered = ((ILayered)container);
            if (layered.Layer > testLevel) // too high
                return false;
            if (layered.Layer == testLevel) // must be as its on that level
                return true;
            var ivid = new ItemVersionedId(container);
            ivid.Version[VersionKey] = testLevel;

            // Find the current ILayered at testLevel
            var summ = Collator.Instance.Get<Summary>(new List<ItemId> { ivid })
                .FirstOrDefault();
            if (summ == null) // currently no ILayered below test level, so new one will become the only one and be shown
                return true;
            return ((int)summ.Version[VersionKey] <= layered.Layer); // is the tested ILayered equal or above the one found
        }

        public object ProcessGet(EventHubData ehd)
        {
            int currLayer;
            switch (ehd.EventName)
            {
                case "Repository.Get.Items":
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
                        if (!version.ContainsKey(VersionKey))
                            return d2;

                        currLayer = version[VersionKey] is long ? (int)(long) version[VersionKey] : (int)version[VersionKey];
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
            if (!version.ContainsKey(VersionKey))
                return d;

            var currLayer = (int)version[VersionKey];

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
