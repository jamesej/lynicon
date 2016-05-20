using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Models;
using Lynicon.Attributes;
using Lynicon.Base.Attributes;

namespace Lynicon.Test.Models
{
    public class SearchContent : BaseContent
    {
        [Summary]
        public string Title { get; set; }

        [Index(IndexAttribute.Mode.TextualAndAgglomerated)]
        public MinHtml Body { get; set; }

        [Index(IndexAttribute.Mode.NonTextual)]
        public DateTime Date { get; set; }

        public SearchContent()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}