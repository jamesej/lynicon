using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Workflow.Models
{
    public class Layer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Level { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
        public virtual ICollection<WorkflowUser> Users { get; set; }

        public Layer()
        {
            Users = new List<WorkflowUser>();
        }
    }
}
