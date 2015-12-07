using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Workflow.Models;

namespace Lynicon.Workflow
{
    public class WorkflowVersioner : Versioner
    {
        public override object PublicVersionValue
        {
            get
            {
                return null;
            }
        }
        public WorkflowVersioner(string versionKey) : base(versionKey)
        { }
        public WorkflowVersioner(string versionKey, Func<Type, bool> isVersionable) : base(versionKey, isVersionable)
        { }

        public override bool Versionable(Type containerType)
        {
            return typeof(ILayered).IsAssignableFrom(containerType);
        }

        public override void SetCurrentVersion(VersioningMode mode, ItemVersion version)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null || mode == VersioningMode.Public)
                version[VersionKey] = LayerManager.Instance.LiveLayer;
            else
                version[VersionKey] = wfUser.CurrentLevel;
        }

        public override void GetItemVersion(object container, ItemVersion version)
        {
            ((ILayered)container).Layer = (int)version[VersionKey];
        }

        public override void SetItemVersion(ItemVersion version, object container)
        {
            ((ILayered)container).Layer = (int)version[VersionKey];
        }

        public override VersionDisplay DisplayItemVersion(ItemVersion version)
        {
            return new VersionDisplay
            {
                Text = "Layer " + version[VersionKey].ToString(),
                CssClass = "workflow-version-display",
                Title = "Layer of current item"
            };
        }

        public override bool TestVersioningMode(object container, VersioningMode mode)
        {
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

        public override List<object> GetAllowedVersions(IUser u)
        {
            throw new NotImplementedException();
        }
    }
}
