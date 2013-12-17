using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Map;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Models
{
    public class UrlTreeNavManager<T> : INavManager where T : class
    {
        Dictionary<Guid, NavTreeSummary> summaryTree = null;
        Dictionary<string, NavTreeSummary> summaryUrls = null;

        public UrlTreeNavManager()
        {
            string dataType = typeof(T).FullName;
            var summaries = Collator.Instance.GetSummaries<Summary, ContentItem>(
                        new List<Type> { typeof(T) },
                        iq => iq.Where(ci => ci.DataType == dataType)
                    ).ToDictionary(s => s.Url, s => s);
            List<string> urls = summaries.Values.Select(s => s.Url).ToList();
            urls.Sort();
            Stack<NavTreeSummary> parents = new Stack<NavTreeSummary>();
            var root = new NavTreeSummary { Id = Guid.Empty, Title = "", Type = null, Url = "" };
            summaryTree = new Dictionary<Guid, NavTreeSummary>();
            summaryUrls = new Dictionary<string, NavTreeSummary>();
            summaryTree.Add(Guid.Empty, root);
            parents.Push(root);
            foreach (string url in urls)
            {
                var summary = summaries[url];
                var navSummary = new NavTreeSummary(summary);
                
                summaryTree.Add((Guid)navSummary.Id, navSummary);  // id will be a Guid as ContentItem
                summaryUrls.Add(url, navSummary);
                while (!(parents.Peek().Url == "" || url.StartsWith(parents.Peek().Url + "/")))
                    parents.Pop();

                parents.Peek().Children.Add(navSummary);
                navSummary.Parent = parents.Peek();
                parents.Push(navSummary);
            }
            EventHub.Instance.RegisterEventProcessor("Repository.Set",
                ehd => { this.ApplyChange(ehd.EventName, ehd.Data); return ehd.Data; },
                "Caching.Navigation.UrlTree", new List<string>());
        }

        private void ApplyChange(string eventName, object item)
        {
            Summary itemSumm = Collator.Instance.GetSummary<Summary>(item);
            if (itemSumm.Type != typeof(T))
                return;

            NavTreeSummary navSumm = new NavTreeSummary(itemSumm);
            if (eventName.EndsWith(".Delete") && summaryTree.ContainsKey((Guid)navSumm.Id))
            {
                var existing = summaryTree[(Guid)navSumm.Id];
                existing.Parent.Children.Remove(existing);
                foreach (var child in existing.Children)
                    existing.Parent.Children.Add(child);
                summaryTree.Remove((Guid)navSumm.Id);
                summaryUrls.Remove(navSumm.Url);
            }
            else
            {
                List<string> urls = summaryTree.Values.Select(s => s.Url).ToList();
                urls.Sort();
                var urlPos = urls.BinarySearch(itemSumm.Url);
                if (urlPos > 0)
                    return;
                else
                {
                    urlPos = ~urlPos;
                    string prevUrl = urls[urlPos];
                    var prevSummary = summaryUrls[prevUrl];
                    while (prevSummary.Parent != null && !navSumm.Url.StartsWith(prevSummary.Url + "/"))
                        prevSummary = prevSummary.Parent;
                    prevSummary.Children.Add(navSumm);
                    urlPos++;
                    // acquire as children any appropriate nav summaries
                    while (urls[urlPos].StartsWith(navSumm.Url + "/")) // loop over all tree under added nav summ
                    {
                        var childSumm = summaryUrls[urls[urlPos]];
                        if (!childSumm.Parent.Url.StartsWith(navSumm.Url + "/")) // don't put indirect children in children list, only direct
                        {
                            navSumm.Children.Add(childSumm);
                            if (childSumm.Parent != null)
                                childSumm.Parent.Children.Remove(childSumm);
                            childSumm.Parent = navSumm;
                        }
                        urlPos++;
                    }
                    navSumm.Parent = prevSummary;
                    summaryTree.Add((Guid)navSumm.Id, navSumm);
                    summaryUrls.Add(navSumm.Url, navSumm);
                }
            }
        }

        #region INavManager Members

        public List<NavTreeSummary> GetChildren(object id)
        {
            return summaryTree[(Guid)id].Children;
        }

        #endregion
    }
}
