using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Models;
using Lynicon.Relations;

namespace Lynicon.Base.Models
{
    public class HierarchySummary : ListingSummary
    {
        public Reference Parent { get; set; }
    }
}
