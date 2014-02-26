using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Workflow.Models
{
    public class ViewLayersViewModel
    {
        public List<Layer> Layers { get; set; }

        public ViewLayersViewModel()
        {
            using (var db = new WorkflowDb())
            {
                Layer liveLayer = db.Layers.FirstOrDefault(l => l.IsLive);
                int liveLevel = liveLayer.Level;
                Layers = db.Layers.Where(l => l.Level >= liveLevel).ToList();
            }
        }
    }
}
