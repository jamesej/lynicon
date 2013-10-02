using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Base.Models;

namespace Lynicon.Base.Controllers
{
    public class BlogPostController : Controller
    {
        public ActionResult Index(BlogPostContent data)
        {
            RouteData.DataTokens.Add("area", "Lynicon.Base");
            return View(data);
        }
    }
}
