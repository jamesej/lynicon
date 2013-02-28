using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Test.Models;

namespace Lynicon.Test.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index(TestContent data)
        {
            return View(data);
        }

    }
}
