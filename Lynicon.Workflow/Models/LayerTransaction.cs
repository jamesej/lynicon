using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Workflow.Models
{
    public class LayerTransaction
    {
        public const string Create = "CREATE";
        public const string Permit = "PERMIT";
        public const string Deny = "DENY";
        public const string Complete = "COMPLETE";

        public int Id { get; set; }
        public int Level { get; set; }
        [ForeignKey("Level")]
        public Layer Layer { get; set; }
        public DateTime Date { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public WorkflowUser User { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
}
