using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Membership;
using Lynicon.Repositories;
using Lynicon.Utility;
using System.Linq.Dynamic;
using Lynicon.Models;
using System.Web;
using Lynicon.Extensibility;
using Lynicon.Modules;
using Lynicon.Map;

namespace Lynicon.Routing
{
    /// <summary>
    /// A route which will redirect a url to the original url transformed via a pattern.  The map url has the same
    /// syntax as the url match pattern and url elements matching the url match pattern are inserted into the equivalent
    /// {xyz} wildcards in the output pattern
    /// </summary>
    public class RedirectRoute : Route
    {
        string mapUrl = null;

        /// <summary>
        /// Whether the redirect is permanent (301) or temporary (302)
        /// </summary>
        public bool IsPermanent { get; set; }

        /// <summary>
        /// Create a new RedirectRoute with a url pattern, a url pattern it maps to, and a route handler
        /// </summary>
        /// <param name="url">The url pattern to divert</param>
        /// <param name="mapUrl">The url pattern to which to divert</param>
        /// <param name="rh">The route handler</param>
        public RedirectRoute(string url, string mapUrl, IRouteHandler rh)
            : base(url, rh)
        {
            this.mapUrl = mapUrl;
            IsPermanent = true;
        }
        /// <summary>
        /// Create a RedirectRoute with a url pattern, a url pattern it maps to, default values for url matching and a route handler
        /// </summary>
        /// <param name="url">The url pattern to match the request</param>
        /// <param name="mapUrl">A url pattern to generate the redirect url</param>
        /// <param name="defaults">Defaults for unmatched url elements</param>
        /// <param name="rh">The route handler</param>
        public RedirectRoute(string url, string mapUrl, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh)
        {
            this.mapUrl = mapUrl;
            IsPermanent = true;
        }
        /// <summary>
        /// Create a RedirectRoute with a url pattern, a url pattern it maps to, default values for url matching, constraints for when the route is matched, and a route handler
        /// </summary>
        /// <param name="url">The url pattern to match the request</param>
        /// <param name="mapUrl">A url pattern to generate the redirect url</param>
        /// <param name="defaults">Defaults for unmatched url elements</param>
        /// <param name="constraints">Constraints for when the request should match</param>
        /// <param name="rh">The route handler</param>
        public RedirectRoute(string url, string mapUrl, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh)
        {
            this.mapUrl = mapUrl;
            IsPermanent = true;
        }
        /// <summary>
        /// Create a RedirectRoute with a url pattern, a url pattern it maps to, default values for url matching, constraints for when the route is matched, data tokens to add, and a route handler
        /// </summary>
        /// <param name="url">The url pattern to match the request</param>
        /// <param name="mapUrl">A url pattern to generate the redirect url</param>
        /// <param name="defaults">Defaults for unmatched url elements</param>
        /// <param name="constraints">Constraints for when the request should match</param>
        /// <param name="dataTokens">Initial data tokens to set</param>
        /// <param name="rh">The route handler</param>
        public RedirectRoute(string url, string mapUrl, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh)
        {
            this.mapUrl = mapUrl;
            IsPermanent = true;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData rd = base.GetRouteData(httpContext);

            if (rd == null) return null;

            var newUrl = mapUrl;
            foreach (var v in rd.Values)
            {
                newUrl = newUrl.Replace("{" + v.Key + "}", v.Value.ToString());
            }
            if (!newUrl.StartsWith("/") && !newUrl.StartsWith("http"))
                newUrl = "/" + newUrl;

            RouteData redirect = new RouteData();
            redirect.RouteHandler = new RedirectRouteHandler(newUrl, IsPermanent);
            return redirect;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData vpd = null;
            return vpd;
        }
    }
}
