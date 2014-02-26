using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Repositories;

namespace Lynicon.Workflow.Models
{
    public class WorkflowContentItem : ContentItem, ILayered
    {
        public int Layer { get; set; }
        public bool IsLive { get; set; }
        public bool IsDeletion { get; set; }
    }
}
