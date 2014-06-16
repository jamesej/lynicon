using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tasks.Models
{
    public class TaskSpec
    {
        public string Name { get; set; }
        public List<string> DependsOn { get; set; }
        public List<string> RestrictFields { get; set; }
        public List<string> CompleteFields { get; set; }
        public bool Repeatable { get; set; }
        public string RolesCanCreate { get; set; }
        public string RolesCanComplete { get; set; }
        public List<string> StartsTasks { get; set; }
        public List<string> EnablesTasks { get; set; }
        public List<string> BlocksTasks { get; set; }
        public bool IsCreateTask { get; set; }
        public bool IsPublishableIfActive { get; set; }

        public TaskSpec()
        {
            DependsOn = new List<string>();
            RestrictFields = new List<string>();
            CompleteFields = new List<string>();
            Repeatable = false;
            RolesCanCreate = "EA";
            RolesCanComplete = "EA";
            StartsTasks = new List<string>();
            EnablesTasks = new List<string>();
            BlocksTasks = new List<string>();
            IsCreateTask = false;
            IsPublishableIfActive = false;
        }
    }
}
