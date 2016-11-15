using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lynicon.Utility
{
    /// <summary>
    /// Uploader for a file sent via a form element
    /// </summary>
    public class FormFileUploader : FileUploader
    {
        public override int Size
        {
            get { return Request.Files["qqfile"].ContentLength; }
        }

        public override string Name
        {
            get { return Request.Files["qqfile"].FileName; }
        }

        public override bool Save(string path)
        {
            try
            {
                Request.Files["qqfile"].SaveAs(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
