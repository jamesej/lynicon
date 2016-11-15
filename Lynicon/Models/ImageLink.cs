using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace Lynicon.Models
{
    /// <summary>
    /// Content subtype for an image with a link around it
    /// </summary>
    [Serializable]
    public class ImageLink : Image
    {
        public string LinkUrl { get; set; }
    }
}
