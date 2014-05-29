using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Linq;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Modules
{
    public class SummaryCache : Module
    {
        public Dictionary<Type, ConcurrentDictionary<ItemVersionedId, object>> Cache { get; private set; }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            // We can suppress version management as we will be single threaded as we are in global.asax Application_Start
            // We have to suppress it otherwise
            VersionManager.Instance.Mode = VersioningMode.All;
            var summaries = Repository.Instance
                    .Get<object>(typeof(Summary), ContentTypeHierarchy.AllContentTypes, iq => iq)
                    .ToList();
            VersionManager.Instance.Mode = VersioningMode.Current;

            // Now we want version management back to build the ItemVersionedId for the summaries
            Cache = new Dictionary<Type, ConcurrentDictionary<ItemVersionedId, object>>();
            summaries.Do(sc => Cache[sc.GetType().UnproxiedType()].TryAdd(new ItemVersionedId(sc), sc));
            

            EventHub.Instance.RegisterEventProcessor("Repository.Get",
                ProcessGet, "Caching.Summaries.Full");

            EventHub.Instance.RegisterEventProcessor("Repository.Set",
                ProcessSet, "Caching.Summaries.Full");

            return true;
        }

        public SummaryCache(AreaRegistrationContext context, params string[] dependentOn)
            : base("Caching.Summaries.Full", context, dependentOn)
        {

        }

        public void Deactivate()
        {
            EventHub.Instance.DeregisterEventProcessor("Repository.Get", "Caching.Summaries.Full");
            EventHub.Instance.DeregisterEventProcessor("Repository.Set", "Caching.Summaries.Full");
        }

        public object ProcessGet(EventHubData ehd)
        {
            switch (ehd.EventName)
            {
                // the cache can only be used for queries if we can ensure that all the fields referred to in the
                    // query exist in the summarised container
                case "Repository.Get.Summaries":
                case "Repository.Get.Summaries.Ids":
                case "Repository.Get.Count":
                    var d = ehd.Data as IQueryEventData<IQueryable>;
                    if (d != null)
                    {
                        Type itemTypeRequired = d.Source.ElementType;
                        // d.Source starts off containing an IQueryable<T> where T is the container type from the repository
                        // (itemTypeRequired).  We have to build another IQueryable<T> from the IEnumerable<object>
                        // which is the cache because AsFacade (applied later) relies on getting the original element type from the
                        // base IQueryable
                        var newSource = Cache[itemTypeRequired.UnproxiedType()].AsQueryable();

                        if (!ehd.EventName.EndsWith(".Ids"))
                        {
                            // where we have a query in the request, we have to ensure the query doesn't access any
                            // fields which are not stored in the container objects in the cache on account of their
                            // only retaining the necessary data to create a summary
                            var fieldNames = d.QueryBody(newSource).ExtractFields();
                            var summaryFieldNames = Collator.Instance.ContainerSummaryFields(itemTypeRequired);
                            if (!fieldNames.All(fn => summaryFieldNames.Contains(fn)))
                                return d;
                        }

                        d.Source = newSource;
                        return d;
                    }
                    break;
            }

            return ehd.Data;
        }

        public object ProcessSet(EventHubData ehd)
        {
            var ivid = new ItemVersionedId(ehd.Data);
            Type containerType = ehd.Data.GetType().UnproxiedType();
            if (ehd.EventName == "Repository.Set.Delete")
            {
                object removed;
                Cache[containerType].TryRemove(ivid, out removed);
            }
            else
                Cache[containerType][ivid] = Collator.Instance.Summarise(ehd.Data);

            return ehd.Data;
        }
    }
}
