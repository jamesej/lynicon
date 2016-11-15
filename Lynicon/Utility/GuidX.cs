using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Utility
{
    /// <summary>
    /// Some utility functions for Guids
    /// </summary>
    public static class GuidX
    {
        /// <summary>
        /// Convert a Guid to a Base64 string which is only 22 characters
        /// </summary>
        /// <param name="guid">The guid</param>
        /// <returns>Guid converted to Base64 string</returns>
        public static string ToBase64String(this Guid guid)
        {
            if (guid == null)
                return null;
            return Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace("/", "_")
                .Replace("+", "-");
        }

        /// <summary>
        /// Convert a Base64 string created by GuidX.ToBase64String back to a guid
        /// </summary>
        /// <param name="enc">The Base64 encoded guid</param>
        /// <returns>The original guid</returns>
        public static Guid FromBase64String(string enc)
        {
            if (enc == null)
                throw new ArgumentNullException("Base64 string");
            if (enc.Length != 22)
                throw new ArgumentException("Encoded guid must be 22 characters long");

            return new Guid(Convert.FromBase64String(enc.Replace("_", "/").Replace("-", "+") + "=="));
        }

        /// <summary>
        /// Add an offset value to a guid to get a new one
        /// </summary>
        /// <param name="guid">Original guid</param>
        /// <param name="offset">Offset number</param>
        /// <returns>Offset guid</returns>
        public static Guid OffsetBy(this Guid guid, byte offset)
        {
            var arry = guid.ToByteArray();
            arry[0] = (byte)(arry[0] + offset);
            return new Guid(arry);
        }
    }
}
