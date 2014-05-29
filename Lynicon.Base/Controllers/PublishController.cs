using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Base.Models;
using Lynicon.Base.Modules;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Controllers
{
    public class PublishController : Controller
    {
        public ActionResult Index(string path, string typeName)
        {
            Type type = ContentTypeHierarchy.GetContentType(typeName);
            Type containerType = Repository.Instance.ContainerType(type);
            Address address = new Address(type, path);
            ReflectionX.InvokeGenericMethod(this, "Publish", false, m => true, new Type[] { containerType }, address);
            return Content("OK");
        }

        private void Publish<T>(Address address) where T: class
        {
            var unpublishedVersion = ItemVersion.Current;
            unpublishedVersion.Remove(Publishing.VersionKey);
            VersionManager.Instance.PushState(VersioningMode.Specific);
            try
            {

                VersionManager.Instance.SpecificVersion = unpublishedVersion;
                var items = Repository.Instance.Get<T>(address.Type, address.GetAsQueryBody<T>());
                if (items != null && items.Count() > 0)
                {
                    var iPubs = items.Cast<IPublishable>().ToList();
                    IPublishable publishing = iPubs.FirstOrDefault(ip => !ip.IsPubVersion);
                    if (iPubs.Any(ip => ip.IsPubVersion))
                        publishing.Id = iPubs.First(ip => ip.IsPubVersion).Id;
                    else
                        publishing.Id = Guid.Empty;
                    publishing.IsPubVersion = true;

                    VersionManager.Instance.SpecificVersion.Add(Publishing.VersionKey, true);
                    Repository.Instance.Set(publishing);
                }
            }
            finally
            {
                VersionManager.Instance.PopState();
            }

        }
    }
}
