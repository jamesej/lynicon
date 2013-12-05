using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Workflow.Models
{
    public class LayerManager
    {
        static readonly LayerManager instance = new LayerManager();
        public static LayerManager Instance { get { return instance; } }

        static LayerManager() { }

        public const int LayerSpacing = 10;

        public int LiveLayer { get; private set; }

        public Dictionary<int, string> LayerNames { get; set; }

        public LayerManager()
        {
            var db = new WorkflowDb();
            LiveLayer = db.Layers.FirstOrDefault(l => l.IsLive).Level;
            LayerNames = new Dictionary<int, string>();
        }

        public string GetLayerName(int layer)
        {
            if (!LayerNames.ContainsKey(layer))
            {
                var db = new WorkflowDb();
                var layerRec = db.Layers.FirstOrDefault(l => l.Level == layer);
                if (layerRec == null)
                    return null;
                else
                    LayerNames.Add(layer, layerRec.Name);
            }
            return LayerNames[layer];
        }

        public Layer GetNewUserLayer(WorkflowUser wfUser, string name)
        {
            using (var db = new WorkflowDb())
            {
                int layerMin = (wfUser.NewLayerMinOffset ?? 0) + LiveLayer;
                int layerMax = wfUser.NewLayerMaxOffset.HasValue ? wfUser.NewLayerMaxOffset.Value + LiveLayer : int.MaxValue;
                var maxLayer = db.Layers
                    .Where(l => l.Level >= layerMin && l.Level <= layerMax)
                    .Max(l => (int?)l.Level);
                if (maxLayer == null) maxLayer = layerMin;
                var newLayer = new Layer {
                    IsLive = false, Level = maxLayer.Value + LayerSpacing, Name = name
                };
                db.Layers.Add(newLayer);
                var newLayerTrans = new LayerTransaction {
                    Layer = newLayer, Date = DateTime.Now, Type = LayerTransaction.Create, User = wfUser
                };
                db.LayerTransactions.Add(newLayerTrans);
                db.SaveChanges();

                return newLayer;
            }
        }

        public List<Layer> GetUserLayers(Guid userId)
        {
            using (var db = new WorkflowDb())
            {
                var layers = db.Users
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Layers.Where(l => l.Level > LiveLayer))
                    .ToList();
                return layers;
            }
        }
    }
}
