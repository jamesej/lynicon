using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Attributes;
using Lynicon.Models;
using Lynicon.Relations;

namespace Lynicon.Base.Models
{
    public class WikiContent : PageContent
    {
        protected override string PathSep
        {
            get
            {
                return "/";
            }
        }

        public HierarchySummary Summary { get; set; }

        [UIHint("Html")]
        public string Body { get; set; }

        public WikiContent()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}
