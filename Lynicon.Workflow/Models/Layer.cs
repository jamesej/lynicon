using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Workflow.Models
{
    public class Layer
    {
        [Key]
        public int Level { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
        public virtual ICollection<WorkflowUser> Users { get; set; }
    }
}
