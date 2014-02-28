using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;

namespace Lynicon.Workflow.Models
{
    public class ViewLayersViewModel
    {
        public class ViewLayersLayer
        {
            public Layer Layer { get; set; }
            public int ChangeCount { get; set; }
        }

        public List<ViewLayersLayer> Layers { get; set; }

        public ViewLayersViewModel()
        {
            using (var db = new WorkflowDb())
            {
                Layer liveLayer = db.Layers.FirstOrDefault(l => l.IsLive);
                int liveLevel = liveLayer.Level;
                var changes = LayerManager.Instance.LayerChanges;
                Layers = db.Layers.Where(l => l.Level >= liveLevel)
                    .OrderByDescending(l => l.Level)
                    .AsEnumerable()
                    .Select(l => new ViewLayersLayer
                    {
                        Layer = l,
                        ChangeCount = changes.ContainsKey(l.Level) ? changes[l.Level].Count : 0
                    })
                    .ToList();
            }
        }
    }
}
