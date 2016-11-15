using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Models;

namespace Lynicon.Map
{
    public class NavManager : INavManager
    {
        static readonly NavManager instance = new NavManager();
        public static NavManager Instance { get { return instance; } }

        static NavManager()
        {
        }

        public List<INavManager> SubNavManagers { get; set; }

        public NavManager()
        {
            SubNavManagers = new List<INavManager>();
        }

        public void RegisterNavManager(INavManager navManager)
        {
            SubNavManagers.Add(navManager);
        }

        #region INavManager Members

        public List<NavTreeSummary> GetChildren(object id)
        {
            List<NavTreeSummary> summaries = null;
            foreach (var navManager in SubNavManagers)
            {
                summaries = navManager.GetChildren(id);
                if (summaries != null)
                    return summaries;
            }
            return null;
        }

        #endregion
    }
}
