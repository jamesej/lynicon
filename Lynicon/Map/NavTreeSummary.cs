using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Models;

namespace Lynicon.Map
{
    public class NavTreeSummary : Summary
    {
        public List<NavTreeSummary> Children { get; set; }
        public NavTreeSummary Parent { get; set; }
        public NavTreeSummary()
            : base()
        {
            Children = new List<NavTreeSummary>();
            Parent = null;
        }
        public NavTreeSummary(Summary summary)
            : this()
        {
            Guid id = GetGuidId(summary);

            Id = id;
            Title = summary.Title;
            Type = summary.Type;
            Url = summary.Url;
        }

        private Guid GetGuidId(Summary summary)
        {
            if (summary.Id is string)
                return new Guid((string)summary.Id);
            else if (summary.Id is Guid)
                return (Guid)summary.Id;
            else
                throw new Exception("Building navigation: summary has wrong id type: " + summary.Id.GetType().FullName);
        }
    }
}
