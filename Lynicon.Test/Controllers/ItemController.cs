using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Test.Models;

namespace Lynicon.Test.Controllers
{
    public class ItemController : Controller
    {
        //
        // GET: /Header/

        public ActionResult Index(ItemContent data)
        {
            var a = ControllerContext.HttpContext;
            return View(data);
        }

    }
}
