using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Base.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Modules
{
    public class Auditing : Module
    {
        public Auditing(AreaRegistrationContext context, params string[] dependentOn)
            : base("Auditing.Basic", context, dependentOn)
        {
            Collator.Instance.SetupType(typeof(Audit), new BasicCollator(), new BasicRepository());
        }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            EventHub.Instance.RegisterEventProcessor("Repository.Set",
                ProcessSet, "Auditing.Basic");

            return true;
        }

        public void Deactivate()
        {
            EventHub.Instance.DeregisterEventProcessor("Repository.Set", "Auditing.Basic");
        }

        public object ProcessSet(EventHubData ehd)
        {
            var ivid = new ItemVersionedId(ehd.Data);
            if (ivid.Type == typeof(Audit)) // avoid recursion
                return ehd.Data;

            var audit = Repository.Instance.New<Audit>();
            audit.Date = DateTime.Now;
            audit.DataType = ivid.Type.FullName;
            audit.ItemId = ivid.Id.ToString();
            audit.UserId = LyniconSecurityManager.Current.User == null ? Guid.Empty : LyniconSecurityManager.Current.User.Id;
            audit.Version = ivid.Version.ToString();

            audit.ChangeType = "";
            switch (ehd.EventName)
            {
                case "Repository.Set.Update":
                    audit.ChangeType = "U";
                    break;
                case "Repository.Set.Delete":
                    audit.ChangeType = "D";
                    break;
                case "Repository.Set.Add":
                    audit.ChangeType = "A";
                    break;
            }

            Collator.Instance.Set(audit);

            return ehd.Data;
        }
    }
}
