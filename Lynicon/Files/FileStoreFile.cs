using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Files
{
    /// <summary>
    /// Class for a file in simple facade for abstracting file storage
    /// </summary>
    public class FileStoreFile
    {
        /// <summary>
        /// Description of location of file in the file system
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size { get; set; }
    }
}
