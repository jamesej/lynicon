using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Models;
using Lynicon.Attributes;

namespace Lynicon.Test.Models
{
    [Serializable]
    public class TagContent : BaseContent
    {
        [Summary]
        public string Title { get; set; }

        public MinHtml Text { get; set; }

        public TagContent()
        {
            Text = new MinHtml();
        }
    }
}