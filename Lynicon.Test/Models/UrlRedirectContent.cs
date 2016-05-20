using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Models;
using Lynicon.Attributes;
using Lynicon.Base.Models;

namespace Lynicon.Test.Models
{
    public class UrlRedirectContent : BaseContent, IHasAlternateUrls
    {
        [Summary]
        public string Title { get; set; }

        public MinHtml Body { get; set; }

        #region IHasAlternateUrls Members

        public object GetId()
        {
            return this.OriginalRecord.Id;
        }

        public AlternateUrlList AlternateUrls { get; set; }

        #endregion

        public UrlRedirectContent()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}