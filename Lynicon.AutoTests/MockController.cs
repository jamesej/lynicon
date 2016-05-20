using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.AutoTests
{
    public class MockController : Controller
    {
        public ActionResult Mock()
        {
            return Content("OK");
        }
    }
}
