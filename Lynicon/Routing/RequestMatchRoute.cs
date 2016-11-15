using System;
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

namespace Lynicon.Routing
{
    /// <summary>
    /// A route which contains an additional check in code as to whether the route matches
    /// </summary>
    public class RequestMatchRoute : Route
    {
        public Func<HttpContextBase, bool> Check { get; set; }
        public Func<string, string> ConformUrl { get; set; }

        public RequestMatchRoute(string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl, IRouteHandler rh)
            : base(url, rh) { Check = check; ConformUrl = conformUrl; }
        public RequestMatchRoute(string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh) { Check = check; ConformUrl = conformUrl; }
        public RequestMatchRoute(string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh) { Check = check; ConformUrl = conformUrl; }
        public RequestMatchRoute(string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh) { Check = check; ConformUrl = conformUrl; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData rd = base.GetRouteData(httpContext);
            if (rd == null || !Check(httpContext))
                return null;

            return rd;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData vpd = base.GetVirtualPath(requestContext, values);
            if (vpd != null && ConformUrl != null)
            {
                vpd.VirtualPath = ConformUrl(vpd.VirtualPath);
            }
            return vpd;
        }
    }
}
