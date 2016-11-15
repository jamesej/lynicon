using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lynicon.Controllers
{
    /// <summary>
    /// Serve the FileManager
    /// </summary>
    public class FileManagerController : Controller
    {
        /// <summary>
        /// Get the FileManager
        /// </summary>
        /// <returns>HTML page of FileManager</returns>
        public ActionResult Index()
        {
            return View("LyniconFileManager");
        }
    }
}
