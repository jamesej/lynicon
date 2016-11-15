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
using System.Web.Mvc;

namespace Lynicon.Routing
{
    /// <summary>
    /// Interface for data route
    /// </summary>
    public interface IDataRoute
    {
        RouteData GetRouteDataAssumingExists(HttpContextBase httpContext);

        RouteData PostProcessRoute(RouteData rd, HttpContextBase httpContext);
    }

    /// <summary>
    /// Untyped DataRoute base
    /// </summary>
    public class DataRoute : Route
    {
        public static Func<Type, bool> TypeCheckExistenceBySummary = (t => false);

        public DataRoute(string url, IRouteHandler rh)
            : base(url, rh) { }
        public DataRoute(string url, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh) { }
        public DataRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh) { }
        public DataRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh) { }
    }

    /// <summary>
    /// Data route, a route which automatically fetches an associated content item from the data system and passes it to the controller,
    /// and fails to match if there is no associated content item stored
    /// </summary>
    /// <typeparam name="T">The type of the content item fetched</typeparam>
    public class DataRoute<T> : DataRoute, IDataRoute where T: class, new()
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// If not null, this editor redirect is used instead of the usual one for this route only
        /// </summary>
        public IEditorRedirect RedirectOverride { get; set; }

        /// <summary>
        /// Create a data route with a url pattern and a route handler
        /// </summary>
        /// <param name="url">Url pattern</param>
        /// <param name="rh">Route handler</param>
        public DataRoute(string url, IRouteHandler rh)
            : base(url, rh) { }
        /// <summary>
        /// Create a data route with a url pattern, default route values, and a route handler
        /// </summary>
        /// <param name="url">Url pattern</param>
        /// <param name="defaults">Default route values</param>
        /// <param name="rh">Route handler</param>
        public DataRoute(string url, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh) { }
        /// <summary>
        /// Create a data route with a url pattern, default route values, constraints, and a route handler
        /// </summary>
        /// <param name="url">Url pattern</param>
        /// <param name="defaults">Default route values</param>
        /// <param name="constraints">Constraints for use of this route</param>
        /// <param name="rh">Route handler</param>
        public DataRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh) { }
        /// <summary>
        /// Create a data route with a url pattern, default route values, constraints, preset data tokens and a route handler
        /// </summary>
        /// <param name="url">Url pattern</param>
        /// <param name="defaults">Default route values</param>
        /// <param name="constraints">Constraints for use of this route</param>
        /// <param name="dataTokens">Preset data tokens</param>
        /// <param name="rh">Route handler</param>
        public DataRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh) { }

        /// <summary>
        /// Whether the route sends a Lazy<T> to the controller so that data can be fetched in the controller rather than the route
        /// </summary>
        public bool LazyData = false;

        /// <summary>
        /// Get route data assuming the content at this url exists
        /// </summary>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>Route data for match or null if underlying route doesn't match request</returns>
        public RouteData GetRouteDataAssumingExists(HttpContextBase httpContext)
        {
            RouteData rd = base.GetRouteData(httpContext);

            if (rd == null) return null;

            Dictionary<string, string> qsParams = httpContext.Request.QueryString
                .Cast<string>()
                .Where(key => key != null)
                .ToDictionary(key => key, key => httpContext.Request.QueryString[key]);

            // Deal with type restrictor query parameter
            if (qsParams.ContainsKey("$type"))
            {
                if (qsParams["$type"].Contains("."))
                {
                    if (typeof(T).FullName.ToLower() != qsParams["$type"].ToLower())
                        return null;
                }
                else
                {
                    if (typeof(T).Name.ToLower() != qsParams["$type"].ToLower())
                        return null;
                }
            }

            return rd;
        }

        /// <summary>
        /// Determines the last matching route for a request on the basis of url form, but not
        /// the presence of data.  We need to know when we have failed to find data on the
        /// last matching route so we can put up a blank editor screen if an editor is logged in.
        /// </summary>
        /// <param name="httpContext">Current request specification</param>
        /// <returns>The last route matching this spec in conventional terms but not in terms of content existence</returns>
        public IDataRoute LastMatchingRoute(HttpContextBase httpContext)
        {
            IDataRoute dr = null;
            if (httpContext.Items.Contains("lyniconLastMatchingRouteName"))
                dr = (IDataRoute)httpContext.Items["lyniconLastMatchingRouteName"];
            else
            {
                dr = RouteTable.Routes.OfType<IDataRoute>()
                    .SkipWhile(r => r != this)
                    .Skip(1)
                    .Where(r => r.GetRouteDataAssumingExists(httpContext) != null)
                    .LastOrDefault();
                if (dr == null)
                    dr = this;
                httpContext.Items["lyniconLastMatchingRouteName"] = dr;
            }
            return dr;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            try
            {
                RouteData rd = GetRouteDataAssumingExists(httpContext);
                if (rd == null)
                    return null;

                return PostProcessRoute(rd, httpContext);
            }
            catch (Exception ex)
            {
                string url = "<not known>";
                if (httpContext != null && httpContext.Request != null && httpContext.Request.Url != null)
                    url = httpContext.Request.Url.OriginalString;
                log.Error("Failed in DataRoute url pattern '" + this.Url + "', req '" + url + "' :", ex);
            }

            return null;
        }

        protected virtual object GetData(RouteData rd)
        {
            if (DataRoute.TypeCheckExistenceBySummary(typeof(T)))
            {
                bool isList = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>);
                if (!isList)
                {
                    Summary summ = Collator.Instance.Get<Summary>(typeof(T), rd);
                    if (summ == null)
                        return null;
                }
            }
            if (LazyData)
                return new Lazy<T>(() => Collator.Instance.Get<T>(rd));
            else
                return Collator.Instance.Get<T>(rd);
        }

        /// <summary>
        /// Given the request matches the underlying route, attempt to get the associated content item(s)
        /// and return null if these do not exist.  If the request should be internally redirected according
        /// to the appropriate tests (e.g. to an editor), return a redirected RouteData.  Otherwise
        /// return the normal RouteData
        /// </summary>
        /// <param name="rd">The RouteData for the match to the underlying route</param>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>Modified route data</returns>
        public RouteData PostProcessRoute(RouteData rd, HttpContextBase httpContext)
        {
            Dictionary<string, string> qsParams = new Dictionary<string, string>();
            if (httpContext != null)
            {
                httpContext.Items[RouteX.CurrentRouteDataKey] = rd;

                qsParams = httpContext.Request.QueryString
                    .Cast<string>()
                    .Where(key => key != null)
                    .ToDictionary(key => key, key => httpContext.Request.QueryString[key]);
                var specialQueryParams = new List<string> { "$filter" };
                specialQueryParams.AddRange(PagingSpec.PagingKeys);
                qsParams
                    .Where(kvp => specialQueryParams.Contains(kvp.Key))
                    .Do(kvp => rd.DataTokens.Add(kvp.Key, kvp.Value));
            }

            var data = GetData(rd);

            var ied = (DataRouteInterceptEventData)EventHub.Instance.ProcessEvent("DataRoute.Intercept", this, new DataRouteInterceptEventData
            {
                QueryStringParams = qsParams,
                Data = data,
                ContentType = typeof(T),
                RouteData = rd,
                WasHandled = false
            }).Data;

            if (ied.WasHandled)
                return ied.RouteData;

            IEditorRedirect redirect = this.RedirectOverride;
            if (redirect == null)
                redirect = EditorRedirect.Instance.Registered<T>();
            if (redirect.Redirect<T>(rd, httpContext, data))
            {
                if (data == null)
                {
                    if (httpContext == null || this == LastMatchingRoute(httpContext))
                        data = Collator.Instance.GetNew<T>(rd); // if last possible matching route, but no data, show new page
                    else
                        return null;
                }
                else if (data is Lazy<T>)
                {
                    data = ((Lazy<T>)data).Value;
                }
                rd.Values.Add("data", data);
                if (httpContext.PreviousHandler == null || !(httpContext.PreviousHandler is MvcHandler))
                    rd.DataTokens.Add("pageData", data);
                return rd;
            }
            else if (data == null)
                return null;
            else
            {
                CodeTimer.MarkTime("Route chosen");

                rd.Values.Add("data", data);
                if (httpContext.PreviousHandler == null || !(httpContext.PreviousHandler is MvcHandler))
                    rd.DataTokens.Add("pageData", data);
                return rd;
            }
        }

        /// <summary>
        /// Convert a RouteValueDictionary to a string
        /// </summary>
        /// <param name="values">RouteValueDictionary</param>
        /// <returns>String serialized from RouteValueDictionary</returns>
        public string SerialiseRVD(RouteValueDictionary values)
        {
            return values
                .Where(kvp => kvp.Value is string)
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Key + "=" + kvp.Value)
                .Join("&");
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData vpd = base.GetVirtualPath(requestContext, values);
            if (vpd != null)
            {
                // List-typed data routes always exist
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                    return vpd;

                RouteValueDictionary rvdMerged = new RouteValueDictionary();
                requestContext.RouteData.Values
                    .Where(kvp => !values.ContainsKey(kvp.Key))
                    .Do(kvp => rvdMerged[kvp.Key] = kvp.Value);
                values
                    .Do(kvp => rvdMerged[kvp.Key] = kvp.Value);
                string fullRvdKey = SerialiseRVD(rvdMerged);

                // We know this has previously been found on this request
                if (requestContext.RouteData.DataTokens["lyniconFound" + fullRvdKey] == this)
                    return vpd;

                // We need to ensure where the matched route is a dataroute with a different
                // type, we don't pick up address fields from the request into our search
                Address dontInheritFields = new Address();
                Type routeType = requestContext.RouteData.Route.GetType();
                if (routeType.IsGenericType && routeType.GetGenericTypeDefinition() == typeof(DataRoute<>))
                {
                    Type requestType = routeType.GetGenericArguments()[0];
                    if (requestType != typeof(T))
                        dontInheritFields = new Address(requestType, requestContext.RouteData);
                }

                // override route vals in requestContext with provided values
                RouteData rd = new RouteData();
                requestContext.RouteData.Values
                    .Where(kvp => !(dontInheritFields.ContainsKey(kvp.Key) || values.ContainsKey(kvp.Key)))
                    .Do(kvp => rd.Values[kvp.Key] = kvp.Value);
                values
                    .Do(kvp => rd.Values[kvp.Key] = kvp.Value);

                var data = GetData(rd);
                if (data == null)
                    return null;
                else
                {
                    requestContext.RouteData.DataTokens["lyniconVpData"] = data;
                    requestContext.RouteData.DataTokens["lyniconFound" + fullRvdKey] = this;
                }
            }
            return vpd;
        }
    }
}
