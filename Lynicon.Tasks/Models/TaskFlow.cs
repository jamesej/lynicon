using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tasks.Models
{
    public class TaskFlow
    {
        public Func<Type, bool> AppliesToType { get; set; }
        public Dictionary<string, TaskSpec> TaskSpecs { get; set; }

        public TaskFlow()
        {
            TaskSpecs = new Dictionary<string, TaskSpec>();
        }
    }
}
