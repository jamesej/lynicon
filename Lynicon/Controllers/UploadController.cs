using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lynicon.Utility;

namespace Lynicon.Controllers
{
    public class UploadController : Controller
    {
        /// <summary>
        /// Do a file upload to the FileManager
        /// </summary>
        /// <param name="dir">Folder into which to load the file</param>
        /// <returns>Status of operation</returns>
        public ActionResult Index(string dir)
        {
            FileUploader uploader = null;
            string status = "";
            
            try
            {
                if (Request.QueryString["qqfile"] != null)
                    uploader = new XhrFileUploader();
                else if (Request.Files["qqfile"] != null)
                    uploader = new FormFileUploader();

                string uploadBasePath = Server.MapPath(dir);

                if (uploader != null)
                {
                    uploader.Save(uploadBasePath + "\\" + uploader.Name);
                    status = "OK";
                }
                else
                    status = "Couldn't find file in request";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }

            return Content("{ 'success' : \"" + status.Replace("\"", "'") + "\" }");
        }

    }
}
