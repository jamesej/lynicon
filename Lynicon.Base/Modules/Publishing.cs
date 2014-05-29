using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Lynicon.Base.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Linq;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Modules
{
    public class Publishing : Module
    {
        public const string VersionKey = "Published";

        public Publishing(AreaRegistrationContext context, params string[] dependentOn)
            : base("Versioning.Publishing", context, dependentOn)
        {
            this.IncompatibleWith.Add("Versioning.Workflow");

            Collator.RegisterExtensionType(typeof(PublishingContentItem));

            VersionManager.Instance.RegisterVersion(VersionKey, false, SetCurrentVersion, GetItemVersion, SetItemVersion, TestVersioningMode);
        }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            if (!VerifyDbState("LyniconPublishing 0.0"))
                return false;

            EventHub.Instance.RegisterEventProcessor("Repository.Get", ProcessGet, "Versioning.Publishing",
                new OrderConstraint("Versioning.Publishing", "Caching"));
            EventHub.Instance.RegisterEventProcessor("Repository.Set", ProcessSet, "Versioning.Publishing",
                new OrderConstraint("Versioning.Publishing", ConstraintType.ItemsAfter, "Caching", "Auditing"));

            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbPublish",
                Float = "right",
                Caption = "Publish",
                RequiredRoles = "A",
                ClientClickScript = @"
                    $.post(""/lynicon/publish"",
                    { path: '$$Path$$', typeName: '$$Type$$' }, function (res) {
                        if (res == 'OK') alert('Published successfully');
                    });",
                ShouldShow = this.CanPublish 
            });

            RouteTable.Routes.MapRoute("publish", "Lynicon/Publish", new { controller = "publish", action = "index" });

            return true;
        }

        public bool CanPublish(object model)
        {
            return true;
        }

        public void SetCurrentVersion(VersioningMode mode, ItemVersion version)
        {
            var isEditor = Roles.IsUserInRole("E");
            if (!isEditor || mode == VersioningMode.Public)
                version[VersionKey] = true;
            else
                version[VersionKey] = false;
        }
        public void GetItemVersion(object o, ItemVersion version)
        {
            if (o is IPublishable)
                version[VersionKey] = (o as IPublishable).IsPubVersion;
        }
        public void SetItemVersion(ItemVersion version, object o)
        {
            if (o is IPublishable && version.ContainsKey(VersionKey))
                ((IPublishable)o).IsPubVersion = (bool)version[VersionKey];
        }
        public bool TestVersioningMode(object container, VersioningMode mode)
        {
            if (!(container is IPublishable))
                return true;

            var iPub = container as IPublishable;
            switch (mode)
            {
                case VersioningMode.Public:
                    return iPub.IsPubVersion;
                case VersioningMode.Current:
                    if (VersionManager.Instance.CurrentVersion.ContainsKey(VersionKey))
                        return iPub.IsPubVersion == (bool)VersionManager.Instance.CurrentVersion[VersionKey];
                    else
                        return iPub.IsPubVersion;
                case VersioningMode.Specific:
                    if (VersionManager.Instance.SpecificVersion.ContainsKey(VersionKey))
                        return iPub.IsPubVersion == (bool)VersionManager.Instance.SpecificVersion[VersionKey];
                    else
                        return iPub.IsPubVersion;
                default:
                    return true;
            }
        }

        public object ProcessGet(EventHubData ehd)
        {
            switch (ehd.EventName)
            {
                case "Repository.Get.Items":
                case "Repository.Get.Items.Ids":
                case "Repository.Get.Items.Paths":
                case "Repository.Get.Summaries.Ids":
                case "Repository.Get.Summaries":
                case "Repository.Get.Count":
                    var d2 = ehd.Data as IQueryEventData<IQueryable>;
                    if (d2 != null && typeof(IPublishable).IsAssignableFrom(d2.Source.ElementType))
                    {
                        var version = VersionManager.Instance.CurrentVersion;

                        // No layer version registered, or versioning suppressed
                        if (!version.ContainsKey(VersionKey))
                            return d2;

                        bool showPublished = (bool)version[VersionKey];

                        DateTime now = DateTime.UtcNow;
                        d2.Source = d2.Source.AsFacade<IPublishable>()
                            .Where(ip => ip.IsPubVersion == showPublished
                                && ((ip.PubFrom == null || ip.PubFrom <= now)
                                    && (ip.PubTo == null || ip.PubTo >= now)));

                        return d2;
                    }
                    break;
            }

            return ehd.Data;
        }
        public object ProcessSet(EventHubData ehd)
        {
            var d = ehd.Data as IPublishable;
            if (d == null)
                return ehd.Data;

            var version = VersionManager.Instance.CurrentVersion;

            // No layer version registered, or versioning suppressed
            if (!version.ContainsKey(VersionKey))
                return d;

            bool showPublished = (bool)version[VersionKey];

            return d;
        }
    }
}
