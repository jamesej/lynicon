using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;

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

        public List<Type> LayerContentTypes { get; set; }

        private Dictionary<int, List<Summary>> layerChanges = null;
        public Dictionary<int, List<Summary>> LayerChanges
        {
            get
            {
                if (layerChanges == null)
                {
                    // Get all the versions, not just the current ones
                    VersionManager.Instance.Suppressed = true;
                    layerChanges = new Dictionary<int, List<Summary>>();
                    Collator.Instance.GetSummaries<Summary, ILayered>(LayerContentTypes, iq => iq.Where(l => l.Layer > LiveLayer))
                        .Do(s =>
                            {
                                int layerN = (int)s.Version["Layer"];
                                if (layerChanges.ContainsKey(layerN))
                                    layerChanges[layerN].Add(s);
                                else
                                    layerChanges.Add(layerN, new List<Summary> { s });
                            });
                    VersionManager.Instance.Suppressed = false;
                }
                return layerChanges;
            }
        }

        public LayerManager()
        {
            var db = new WorkflowDb();
            LiveLayer = db.Layers.FirstOrDefault(l => l.IsLive).Level;
            LayerNames = new Dictionary<int, string>();

            LayerContentTypes = ContentTypeHierarchy.AllContentTypes
                .Where(t => typeof(ILayered).IsAssignableFrom(Repository.Instance.OutputType(t)))
                .ToList();
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

        public Layer GetNewUserLayer(IWorkflowUser wfUser, string name)
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

                SetUserLayer(wfUser, newLayer.Level);

                var wfUserDummy = new WorkflowUser { Id = wfUser.Id };
                db.Users.Attach(wfUserDummy);
                newLayer.Users.Add(wfUserDummy);
                
                db.Layers.Add(newLayer);
                var newLayerTrans = new LayerTransaction {
                    Layer = newLayer, Date = DateTime.Now, Type = LayerTransaction.Create, UserId = wfUser.Id
                };
                db.LayerTransactions.Add(newLayerTrans);
                db.SaveChanges();

                return newLayer;
            }
        }

        public void SetUserLayer(IWorkflowUser wfUser, int level)
        {
            wfUser.CurrentLevel = level;
            Collator.Instance.Set(wfUser);
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

        public List<WorkflowUser> GetLayerUsers(int level)
        {
            using (var db = new WorkflowDb())
            {
                var users = db.Layers
                    .Where(l => l.Level == level)
                    .SelectMany(l => l.Users)
                    .ToList();
                return users;
            }
        }

    }
}
