using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using Lynicon.Config;
using Lynicon.Utility;
using Lynicon.Attributes;
using Lynicon.Membership;

namespace Lynicon.Controllers
{
    /// <summary>
    /// Controller for managing lists of users
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        /// Shows the administrative list of users.  Note the code in the action
        /// method will never normally be run as if you are an editor as required by
        /// authorization, you will see the list editor
        /// </summary>
        /// <param name="data">The list of users</param>
        /// <returns>List of the usernames of the users</returns>
        [NoCache, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult List(List<User> data)
        {
            return Content(data.Select(u => u.UserName).Join(", "), "text/plain");
        }
    }
}
