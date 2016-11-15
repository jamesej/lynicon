using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynicon.Utility
{
    /// <summary>
    /// Extension methods for DateTime
    /// </summary>
    public static class DateTimeX
    {
        /// <summary>
        /// Zero time of Unix timestamps
        /// </summary>
        public static DateTime UnixEra = new DateTime(1970, 1, 1);

        /// <summary>
        /// Get a DateTime from a unix timestamp
        /// </summary>
        /// <param name="unixTimestamp">Unix timestamp</param>
        /// <returns>timestamp as a DateTime</returns>
        public static DateTime? FromUnix(string unixTimestamp)
        {
            if (unixTimestamp == "NaN" || unixTimestamp == "0" || string.IsNullOrEmpty(unixTimestamp))
                return null;
            else
                return UnixEra.AddSeconds(double.Parse(unixTimestamp));
        }

        /// <summary>
        /// Get a Unix timestamp from a DateTime
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <returns>The equivalent Unix timestamp</returns>
        public static string ToUnix(this DateTime dt)
        {
            return (dt - UnixEra).TotalSeconds.ToString();
        }
    }
}
