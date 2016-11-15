using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lynicon.Utility
{
    /// <summary>
    /// Lynicon functions to add to the UrlHelper
    /// </summary>
    public static class UrlHelperX
    {
        /// <summary>
        /// Set this to override how a canonical url is retrieved
        /// </summary>
        public static Func<string> CanonicalGetter { get; set; }

        static UrlHelperX()
        {
            CanonicalGetter = () =>
                {
                    var canonical = HttpContext.Current.Request.Url.AbsoluteUri;
                    if (canonical.EndsWith("/"))
                        canonical = canonical.Substring(0, canonical.Length - 1);
                    canonical = canonical.ToLower();
                    return canonical;
                };
        }

        /// <summary>
        /// Get the current canonical url
        /// </summary>
        /// <param name="url">Url helper</param>
        /// <returns>Current canonical url</returns>
        public static string Canonical(this UrlHelper url)
        {
            return CanonicalGetter();
        }
    }
}
