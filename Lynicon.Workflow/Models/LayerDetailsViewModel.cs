using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Models;

namespace Lynicon.Workflow.Models
{
    public class LayerDetailsViewModel
    {
        public List<WorkflowUser> Users { get; set; }
        public List<Summary> ChangedItems { get; set; }
        public int Level { get; set; }

        public LayerDetailsViewModel(int level)
        {
            Level = level;
            var lm = LayerManager.Instance;
            Users = lm.GetLayerUsers(level);
            ChangedItems = 
                lm.LayerChanges.ContainsKey(level)
                ? lm.LayerChanges[level].OrderBy(l => l.Type.Name + l.DisplayTitle()).ToList()
                : new List<Summary>();
        }
    }
}
