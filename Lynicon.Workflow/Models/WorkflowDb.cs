using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Configuration;

namespace Lynicon.Workflow.Models
{
    public class WorkflowDb : DbContext
    {
        static WorkflowDb()
        {
            Database.SetInitializer<WorkflowDb>(null);
        }

        public WorkflowDb()
            : base(ConfigurationManager.ConnectionStrings["LyniconContent"].ConnectionString)
        { }
        public WorkflowDb(string nameOrCs)
            : base(nameOrCs)
        { }

        public DbSet<Layer> Layers { get; set; }
        public DbSet<LayerTransaction> LayerTransactions { get; set; }
        public DbSet<WorkflowUser> Users { get; set; }
    }
}
