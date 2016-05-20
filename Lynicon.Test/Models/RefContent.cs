using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lynicon.Attributes;
using Lynicon.Base.Attributes;
using Lynicon.Base.Models;
using Lynicon.Collation;
using Lynicon.Models;
using Lynicon.Relations;
using Lynicon.Utility;
using Newtonsoft.Json;

namespace Lynicon.Test.Models
{
    public class RefTargetContent : BaseContent
    {
        [Summary]
        public string Title { get; set; }

        public string RTString { get; set; }
    }

    [Serializable]
    public class RefSummary : Summary
    {
        public Reference<RefTargetContent> RefTarget { get; set; }

        public RefSummary()
        {
            RefTarget = new Reference<RefTargetContent>();
        }
    }

    [Serializable, SummaryType(typeof(RefSummary))]
    public class RefContent : PageWithUrlsContent
    {
        [Summary]
        public string Title { get; set; }

        [Summary]
        public Reference<RefTargetContent> RefTarget { get; set; }

        [UIHint("MinHtml"), Index(IndexAttribute.Mode.Agglomerate)]
        public string Body { get; set; }

        public RefContent()
        {
            this.RefTarget = new Reference<RefTargetContent>();
        }
    }
}