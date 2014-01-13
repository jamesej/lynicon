using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Membership;

namespace Lynicon.Workflow.Models
{
    public interface IWorkflowUser : IUser
    {
        int? CurrentLevel { get; set; }
        int? NewLayerMinOffset { get; set; }
        int? NewLayerMaxOffset { get; set; }
        //ICollection<Layer> Layers { get; set; }
    }
}
