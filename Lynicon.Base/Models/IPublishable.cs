using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Base.Models
{
    public interface IPublishable
    {
        Guid Id { get; set; }
        bool IsPubVersion { get; set; }
        DateTime? PubFrom { get; set; }
        DateTime? PubTo { get; set; }
    }
}
