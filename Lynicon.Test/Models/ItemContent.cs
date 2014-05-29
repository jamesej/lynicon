using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Models;

namespace Lynicon.Test.Models
{
    public class ItemContent
    {
        public GeneralSummary Summary { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public List<Multiline> Things { get; set; }

        public ItemContent()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}