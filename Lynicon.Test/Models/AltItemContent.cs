using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Models;

namespace Lynicon.Test.Models
{
    [Serializable]
    public class AltItemContent
    {
        public GeneralSummary Summary { get; set; }
        public string LineA { get; set; }
        public string LineB { get; set; }
        public Link Link { get; set; }

        public AltItemContent()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}