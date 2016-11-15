using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lynicon.Utility
{
    /// <summary>
    /// File upload via XHr
    /// </summary>
    public class XhrFileUploader : FileUploader
    {
        /// <summary>
        /// Size of the file
        /// </summary>
        public override int Size
        {
            get { return int.Parse(Request.Headers["Content-Length"]); }
        }

        /// <summary>
        /// Name of the file
        /// </summary>
        public override string Name
        {
            get { return Request.QueryString["qqfile"]; }
        }

        /// <summary>
        /// Save the file to a given filesystem path
        /// </summary>
        /// <param name="path">The filesystem path</param>
        /// <returns>True (or exception)</returns>
        public override bool Save(string path)
        {
            Stream input = null;
            FileStream output = null;
            try
            {
                input = Request.InputStream;
                output = File.OpenWrite(path);
                input.CopyTo(output);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (input != null) input.Close();
                if (output != null) output.Close();
            }
            return true;
        }
    }   
}
