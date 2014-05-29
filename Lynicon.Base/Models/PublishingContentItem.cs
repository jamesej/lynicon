using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Repositories;

namespace Lynicon.Base.Models
{
    public class PublishingContentItem : ContentItem, IPublishable
    {
        public bool IsPubVersion { get; set; }
        public DateTime? PubFrom { get; set; }
        public DateTime? PubTo { get; set; }
    }
}
