using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Repositories;

namespace Lynicon.Workflow.Models
{
    public interface ILayered
    {
        Guid Id { get; set; }
        Guid Identity { get; set; }
        int Layer { get; set; }
        bool IsLive { get; set; }
    }
}
