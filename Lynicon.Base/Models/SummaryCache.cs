using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Models
{
    public class SummaryCache : Module
    {
        public Dictionary<ItemVersionedId, object> Cache { get; private set; }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            // We can suppress version management as we will be single threaded as we are in global.asax Application_Start
            // We have to suppress it otherwise
            VersionManager.Instance.Suppressed = true;
            var summaries = Repository.Instance
                    .GetSummaries<object, object>(typeof(Summary), ContentTypeHierarchy.AllContentTypes, iq => iq)
                    .ToList();
            VersionManager.Instance.Suppressed = false;

            // Now we want version management back to build the ItemVersionedId for the summaries
            Cache = summaries.ToDictionary(sc => new ItemVersionedId(sc), sc => sc,
                        sc => { throw new Exception("Duplicate versioned id of database item: " + new ItemVersionedId(sc).ToString()); });
            

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
                case "Repository.Get.Summaries":
                    var d = ehd.Data as IQueryEventData<IQueryable>;
                    if (d != null)
                    {
                        Type itemTypeRequired = d.Source.ElementType;
                        // d.Source starts off containing an IQueryable<T> where T is the container type from the repository
                        // (itemTypeRequired).  We have to build another IQueryable<T> from the IEnumerable<object>
                        // which is the cache because AsFacade (applied later) relies on getting the original element type from the
                        // base IQueryable
                        d.Source = LinqX.OfTypeRuntime(Cache.Values, itemTypeRequired).AsQueryable();
                        return d;
                    }
                    break;
            }

            return ehd.Data;
        }

        public object ProcessSet(EventHubData ehd)
        {
            var ivid = new ItemVersionedId(ehd.Data);
            if (ehd.EventName == "Repository.Set.Delete")
                Cache.Remove(ivid);
            else
                Cache[ivid] = Collator.Instance.Summarise(ehd.Data);

            return ehd.Data;
        }
    }
}
