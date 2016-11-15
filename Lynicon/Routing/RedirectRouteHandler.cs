using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Lynicon.Routing
{
    /// <summary>
    /// A route handler to wrap the RedirectHttpHandler
    /// </summary>
    public class RedirectRouteHandler : IRouteHandler
    {
        bool permanent = true;
        public string Url { get; set; }

        /// <summary>
        /// Create a redirect route handler to redirect to a url
        /// </summary>
        /// <param name="url">The url to redirect to</param>
        /// <param name="permanent">Whether the redirect is permanent (otherwise temporary)</param>
        public RedirectRouteHandler(string url, bool permanent)
        {
            this.Url = url;
            this.permanent = permanent;
        }

        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new RedirectHttpHandler(Url, permanent);
        }

        #endregion
    }
}
