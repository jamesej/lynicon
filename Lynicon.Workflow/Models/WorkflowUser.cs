using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Attributes;
using Lynicon.Membership;
using Lynicon.Repositories;

namespace Lynicon.Workflow.Models
{
    public class WorkflowUser : User, IWorkflowUser
    {
        public int? CurrentLevel { get; set; }
        public int? NewLayerMinOffset { get; set; }
        public int? NewLayerMaxOffset { get; set; }
        [NonComposite]
        public virtual ICollection<Layer> Layers { get; set; }

        public WorkflowUser()
            : base()
        {
        }
        public WorkflowUser(IWorkflowUser iWfUser)
        {
            base.Created = iWfUser.Created;
            base.Email = iWfUser.Email;
            base.Id = iWfUser.Id;
            base.Modified = iWfUser.Modified;
            base.Password = iWfUser.Password;
            base.Roles = iWfUser.Roles;
            base.UserName = iWfUser.UserName;
            this.CurrentLevel = iWfUser.CurrentLevel;
            this.NewLayerMaxOffset = iWfUser.NewLayerMaxOffset;
            this.NewLayerMinOffset = iWfUser.NewLayerMinOffset;
        }
    }
}
