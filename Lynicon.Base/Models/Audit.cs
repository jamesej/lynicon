using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Base.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string ItemId { get; set; }
        public string DataType { get; set; }
        public string Version { get; set; }
        public string Change { get; set; }
        public string ChangeType { get; set; }
    }
}
