using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lynicon.Attributes;
using Lynicon.Collation;
using Lynicon.Models;
using Lynicon.Utility;
using Newtonsoft.Json;

namespace Lynicon.Test.Models
{
    [Serializable]
    public class SubTest
    {
        public string A {get; set;}

        [Index(IndexAttribute.Mode.TextualAndAgglomerated)]
        public string B {get; set;}
    }
    [Serializable, RedirectPropertySource("Common")]
    public class HeaderContent : PageContent
    {
        public GeneralSummary Summary { get; set; }
        [UIHint("MinHtml"), Index(IndexAttribute.Mode.Agglomerate)]
        public string HeaderBody { get; set; }

        public string Common { get; set; }

        public List<SubTest> SubTests { get; set; }

        private List<GeneralSummary> childItems = null;
        [JsonIgnore, ScaffoldColumn(false)]
        public List<GeneralSummary> ChildItems
        {
            get
            {
                if (childItems == null)
                    childItems = GetPathChildren<GeneralSummary>().ToList();
                return childItems;
            }
        }

        public HeaderContent()
        {
            Summary = new GeneralSummary();
            this.AlternateUrls = new AlternateUrlList();
            this.SubTests = new List<SubTest>();
        }
    }
}