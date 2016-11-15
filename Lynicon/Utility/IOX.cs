using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lynicon.Utility
{
    /// <summary>
    /// Extension methods for file io
    /// </summary>
    public static class IOX
    {
        /// <summary>
        /// Recursively delete files in a directory
        /// </summary>
        /// <param name="dir">The directory to delete recursively</param>
        public static void DeleteRecursive(this DirectoryInfo dir)
        {
            if (dir.FullName == "C:\\"
                || dir.FullName.StartsWith("C:\\Windows")
                || dir.FullName.StartsWith("C:\\Program Files"))
                throw new ArgumentException("Cannot recursive delete from system folders");

            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in files)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                subDir.DeleteRecursive();
            }

            dir.Delete(false);
        }


        public static byte[] ToBinary(object item)
        {
            var formatter = new BinaryFormatter();
            using (var memstream = new MemoryStream())
            {
                formatter.Serialize(memstream, item);
                memstream.Seek(0, SeekOrigin.Begin);
                return memstream.ToArray();
            }
        }
    }
}
