using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Map;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

namespace Lynicon.Base.Models
{
    public class UrlTreeNavManager<T> : INavManager where T : class
    {
        Dictionary<Guid, NavTreeSummary> summaryTree = null;

        public UrlTreeNavManager(string rootUrl)
        {
            var summaries = Collator.Instance.GetList<Summary, ContentItem>(
                        iq => iq.Where(ci => ci.DataType == typeof(T).FullName)
                    ).ToDictionary(s => s.Url, s => s);
            List<string> urls = summaries.Values.Select(s => s.Url).ToList();
            urls.Sort();
            List<NavTreeSummary> parents = new List<NavTreeSummary>();
            var root = new NavTreeSummary { Id = Guid.Empty, Title = "", Type = null, Url = "" };
            summaryTree = new Dictionary<Guid,NavTreeSummary>();
            summaryTree.Add(Guid.Empty, root);
            parents.Add(root);
            foreach (string url in urls)
            {
                var summary = summaries[url];
                var navSummary = new NavTreeSummary { Id = summary.Id, Title = summary.Title, Type = summary.Type, Url = summary.Url };
                summaryTree.Add(summary.Id, navSummary);
                foreach (NavTreeSummary parent in parents)
                    if (url.StartsWith(parent.Url + "/"))
                    {
                        parent.Children.Add(navSummary);

                        break;
                    }
            }

        }

        #region INavManager Members

        public List<Summary> GetChildren(object id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
