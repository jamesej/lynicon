using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.Routing
{
    /// <summary>
    /// An HttpHandler which redirects the request
    /// </summary>
    public class RedirectHttpHandler : IHttpHandler
    {
        string url = null;
        bool permanent = true;

        /// <summary>
        /// Create a redirecting HttpHandler which redirects to a specific url with a permanent or temporary HTTP redirect
        /// </summary>
        /// <param name="url"></param>
        /// <param name="permanent"></param>
        public RedirectHttpHandler(string url, bool permanent)
        {
            this.url = url;
            this.permanent = permanent;
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (permanent)
            {
                context.Response.Status = "301 Moved Permanently";
                context.Response.StatusCode = 301;
                context.Response.AddHeader("Location", url);
            }
            else
            {
                context.Response.Status = "302 Found";
                context.Response.StatusCode = 302;
                context.Response.AddHeader("Location", url);
            }
        }

        #endregion
    }
}
