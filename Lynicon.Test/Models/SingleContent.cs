using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lynicon.Test.Models
{
    public class SingleContent
    {
        public GeneralSummary Summary { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }

        public SingleContent()
        {
            Summary = new GeneralSummary();
        }
    }
}