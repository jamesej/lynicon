using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Files
{
    /// <summary>
    /// Simple facade for abstracting over file storage
    /// </summary>
    public interface IFileStore
    {
        /// <summary>
        /// Store a file at a path, given a stream and a mime type
        /// </summary>
        /// <param name="path">location of the file in the file system</param>
        /// <param name="readStream">stream from which to read the bytes of the file</param>
        /// <param name="mimeType">mime type describing type of file</param>
        /// <returns>a file store object</returns>
        FileStoreFile StoreFile(string path, Stream readStream, string mimeType);

        /// <summary>
        /// Delete the file at a path from the file system
        /// </summary>
        /// <param name="path">Path of file to delete</param>
        void DeleteFile(string path);

        /// <summary>
        /// Read a file from a file system
        /// </summary>
        /// <param name="path">location of the file in the file system</param>
        /// <returns>a stream from which to read the file</returns>
        Stream ReadFile(string path);
    }
}
