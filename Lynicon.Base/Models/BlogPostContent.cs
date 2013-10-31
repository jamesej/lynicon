using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Attributes;

namespace Lynicon.Base.Models
{
    public class BlogPostContent
    {
        [AddressComponent("_3")]
        public string UrlTitle { get; set; }
        [AddressComponent("_0", ConversionFormat="0000")]
        public int Year { get; set; }
        [AddressComponent("_1", ConversionFormat="00")]
        public int Month { get; set; }
        [AddressComponent("_2", ConversionFormat="00")]
        public int Day { get; set; }
        public string AuthorName { get; set; }
        [UIHint("Html")]
        public string Body { get; set; }
    }
}
