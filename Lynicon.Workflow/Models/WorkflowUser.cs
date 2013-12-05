using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Attributes;
using Lynicon.Membership;
using Lynicon.Repositories;

namespace Lynicon.Workflow.Models
{
    public class WorkflowUser : User
    {
        public int? CurrentLevel { get; set; }
        public int? NewLayerMinOffset { get; set; }
        public int? NewLayerMaxOffset { get; set; }
        [NonComposite]
        public virtual ICollection<Layer> Layers { get; set; }
    }
}
