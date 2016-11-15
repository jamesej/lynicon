using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lynicon.Utility
{
    /// <summary>
    /// Base class for the two different kinds of file uploader
    /// </summary>
    public abstract class FileUploader
    {
        protected HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        /// <summary>
        /// The size of the file
        /// </summary>
        public abstract int Size { get; }
        /// <summary>
        /// The name of the file
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Method to save the file to a given path
        /// </summary>
        /// <param name="path">The filesystem path</param>
        /// <returns>Whether save was successful</returns>
        public abstract bool Save(string path);
    }
}
