using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lynicon.Collation;
using Lynicon.Models;
using Lynicon.Utility;
using Newtonsoft.Json;

namespace Lynicon.Test.Models
{
    public class HeaderContent : PageContent
    {
        public GeneralSummary Summary { get; set; }
        [UIHint("Html")]
        public string HeaderBody { get; set; }

        private List<GeneralSummary> childItems = null;
        [JsonIgnore]
        public List<GeneralSummary> ChildItems
        {
            get
            {
                if (childItems == null)
                {
                    childItems = Collator.Instance.Get(typeof(GeneralSummary),
                        new Dictionary<string, object> { { "Path LIKE '
                }
            }
        }

        public RouteData RouteData { set; private get; }

        public HeaderContent()
        {
            Summary = new GeneralSummary();
        }
    }
}