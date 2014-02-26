using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Repositories;

namespace Lynicon.Workflow.Models
{
    public interface IWorkflowContentItem : IContentItem
    {
        int Layer { get; set; }
        bool IsLive { get; set; }
        bool IsDeletion { get; set; }
    }
}
