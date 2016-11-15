using Lynicon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.Utility
{
    /// <summary>
    /// Utility to time processes in code
    /// </summary>
    public static class CodeTimer
    {
        public const string CacheKey = "lyn_code_timer";

        /// <summary>
        /// Start the timer
        /// </summary>
        public static void Start()
        {
            RequestThreadCache.Current[CacheKey] = DateTime.Now;
            Debug.WriteLine("Timer start 00:00");
        }

        /// <summary>
        /// Generate debug message with elapsed time since start
        /// </summary>
        /// <param name="message">The message</param>
        public static void MarkTime(string message)
        {
            DateTime? start = (DateTime?)RequestThreadCache.Current[CacheKey];
            if (start == null)
            {
                //Debug.WriteLine(message + "(initiating timer)");
                Start();
            }
            else
            {
                string url = null;
                if (HttpContext.Current != null && HttpContext.Current.Handler != null && HttpContext.Current.Request != null)
                    url = HttpContext.Current.Request.Url.AbsolutePath;
                //Debug.WriteLine("Time " + (DateTime.Now - start.Value).ToString(@"ss\:fff") + (url == null ? "" : "(" + url + ")") + " " + message);
            }
        }
    }
}
