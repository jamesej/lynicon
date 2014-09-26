using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Test.Models;

namespace Lynicon.Test.Controllers
{
    public class SingleController : Controller
    {
        //
        // GET: /Header/

        public ActionResult Index(SingleContent data)
        {
            var rd = RouteData;
            return View(data);
        }

    }
}
