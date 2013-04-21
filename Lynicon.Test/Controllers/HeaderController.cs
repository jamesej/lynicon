using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Test.Models;

namespace Lynicon.Test.Controllers
{
    public class HeaderController : Controller
    {
        //
        // GET: /Header/

        public ActionResult Index(HeaderContent data)
        {
            return View(data);
        }

    }
}
