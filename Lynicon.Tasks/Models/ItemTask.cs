using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tasks.Models
{
    public class ItemTask
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ItemIdentity { get; set; }
        public string TaskName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Guid? CompletedBy { get; set; }
        public DateTime? TargetDate { get; set; }
        public int? TargetPriority { get; set; }
        public string Note { get; set; }
    }
}
