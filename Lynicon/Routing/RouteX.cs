using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Editors;
using Lynicon.Models;
using Lynicon.Utility;
using Lynicon.Collation;

namespace Lynicon.Routing
{
    /// <summary>
    /// Extension methods to do routing
    /// </summary>
    public static class RouteX
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string CurrentRouteDataKey = "LynCurrRD";

        /// <summary>
        /// Get the current route data from the request cache for places where the route data is not otherwise
        /// available
        /// </summary>
        /// <returns></returns>
        static public RouteData CurrentRouteData()
        {
            if (HttpContext.Current == null
                || HttpContext.Current.Items[RouteX.CurrentRouteDataKey] == null)
                return null;
            var rd = (RouteData)HttpContext.Current.Items[RouteX.CurrentRouteDataKey];
            return rd;
        }

        /// <summary>
        /// Redirect the indicated area, controller and action in a RouteData elsewhere
        /// </summary>
        /// <param name="rd">The RouteData to redirect</param>
        /// <param name="area">The area to redirect to</param>
        /// <param name="controller">The controller to redirect to</param>
        /// <param name="action">The action to redirect to</param>
        static public void RedirectAction(this RouteData rd, string area, string controller, string action)
        {
            rd.DataTokens["originalArea"] = rd.DataTokens["area"] ?? "";
            rd.DataTokens["area"] = area;
            rd.RedirectAction(controller, action);
        }
        /// <summary>
        /// Redirect the indicated controller and action in a RouteData elsewhere
        /// </summary>
        /// <param name="rd">The RouteData to redirect</param>
        /// <param name="controller">The controller to redirect to</param>
        /// <param name="action">The action to redirect to</param>
        static public void RedirectAction(this RouteData rd, string controller, string action)
        {
            rd.Values["originalController"] = rd.Values["controller"];
            rd.Values["controller"] = controller;
            rd.RedirectAction(action);
        }
        /// <summary>
        /// Redirect the indicated action in a RouteData elsewhere
        /// </summary>
        /// <param name="rd">The RouteData to redirect</param>
        /// <param name="action">The action to redirect to</param>
        static public void RedirectAction(this RouteData rd, string action)
        {
            rd.Values["originalAction"] = rd.Values["action"];
            rd.Values["action"] = action;
        }

        /// <summary>
        /// Create a copy of RouteData
        /// </summary>
        /// <param name="rd">The RouteData to copy</param>
        /// <returns>Copy of the RouteData</returns>
        static public RouteData Copy(this RouteData rd)
        {
            var newRd = new RouteData();
            newRd.Route = rd.Route;
            newRd.RouteHandler = rd.RouteHandler;
            rd.Values.Do(kvp => newRd.Values.Add(kvp.Key, kvp.Value));
            rd.DataTokens.Do(kvp => newRd.DataTokens.Add(kvp.Key, kvp.Value));

            return newRd;
        }

        /// <summary>
        /// Test whether two RouteDatas have the same set of keys and values in their Values property
        /// </summary>
        /// <param name="rd0">The first RouteData</param>
        /// <param name="rd1">The second RouteData</param>
        /// <returns>True if match</returns>
        static public bool Match(this RouteData rd0, RouteData rd1)
        {
            foreach (var key in rd1.Values.Keys)
                if (!rd0.Values.ContainsKey(key) || !rd0.Values[key].Equals(rd1.Values[key]))
                    return false;

            foreach (var key in rd0.Values.Keys)
                if (!rd1.Values.ContainsKey(key))
                    return false;

            return true;
        }

        /// <summary>
        /// Get the undiverted version from a RouteData that was redirected using RedirectAction
        /// </summary>
        /// <param name="rd">A redirected RouteData</param>
        /// <returns>The original unredirected RouteData (a new object)</returns>
        static public RouteData GetOriginal(this RouteData rd)
        {
            var origRd = rd.Copy();
            origRd.Values["controller"] = rd.Values["originalController"];
            origRd.Values["action"] = rd.Values["originalAction"];
            origRd.DataTokens["area"] = rd.DataTokens["originalArea"];

            return origRd;
        }

        /// <summary>
        /// Add a DataRoute to an RouteCollection with route name, url pattern and default values
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="routes">A RouteCollection to add the route to</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this RouteCollection routes, string name, string url, object defaults)
            where TData: class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(defaults), new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a DataRoute to an RouteCollection with route name, url pattern, default values and constraints
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="routes">A RouteCollection to add the route to</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <param name="redirectOverride">An editor redirect to use with this route</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this RouteCollection routes, string name, string url, object defaults, IEditorRedirect redirectOverride)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(defaults), new MvcRouteHandler());
            route.RedirectOverride = redirectOverride;
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a DataRoute to an RouteCollection with route name, url pattern, default values and constraints
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="routes">A RouteCollection to add the route to</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <param name="constraints">Constraints for when the route should match</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this RouteCollection routes, string name, string url, object defaults, object constraints)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a DataRoute to an RouteCollection with route name, url pattern, default values and constraints
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="routes">A RouteCollection to add the route to</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <param name="constraints">Constraints for when the route should match</param>
        /// <param name="dataTokens">Data tokens to add to the route</param>
        /// <param name="redirectOverride">An editor redirect to use with this route</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this RouteCollection routes, string name, string url, object defaults, object constraints, object dataTokens, IEditorRedirect redirectOverride)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens), new MvcRouteHandler());
            if (redirectOverride != null)
                route.RedirectOverride = redirectOverride;
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a DataRoute to an AreaRegistractionContext with route name, url pattern, default values
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="areaReg">An AreaRegistrationContext</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this AreaRegistrationContext areaReg, string name, string url, object defaults)
            where TData: class, new()
        {
            return areaReg.AddDataRoute<TData>(name, url, defaults, null);
        }
        /// <summary>
        /// Add a DataRoute to an AreaRegistractionContext with route name, url pattern, default values and specific EditorRedirect to use
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="areaReg">An AreaRegistrationContext</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <param name="redirectOverride">Override for normal EditorRedirect for this route only</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this AreaRegistrationContext areaReg, string name, string url, object defaults, IEditorRedirect redirectOverride)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            DataRoute<TData> route = areaReg.Routes.AddDataRoute<TData>(name, url, defaults);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens["area"] = areaReg.AreaName;
            route.RedirectOverride = redirectOverride;

            // disabling the namespace lookup fallback mechanism keeps this areas from accidentally picking up
            // controllers belonging to other areas
            //bool useNamespaceFallback = (namespaces == null || namespaces.Length == 0);
            //route.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;
            return route;
        }
        /// <summary>
        /// Add a DataRoute to an AreaRegistractionContext with route name, url pattern, default values and specific EditorRedirect to use
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="areaReg">An AreaRegistrationContext</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <param name="constraints">Constraints for when the route should match</param>
        /// <param name="dataTokens">Data tokens to add to the route</param>
        /// <param name="redirectOverride">An editor redirect to use with this route</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddDataRoute<TData>(this AreaRegistrationContext areaReg, string name, string url, object defaults, object constraints, object dataTokens, IEditorRedirect redirectOverride)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            DataRoute<TData> route = areaReg.Routes.AddDataRoute<TData>(name, url, defaults, constraints, dataTokens, redirectOverride);
            route.DataTokens["area"] = areaReg.AreaName;

            // disabling the namespace lookup fallback mechanism keeps this areas from accidentally picking up
            // controllers belonging to other areas
            //bool useNamespaceFallback = (namespaces == null || namespaces.Length == 0);
            //route.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;
            return route;
        }

        /// <summary>
        /// Add a data route which instead of passing content data to the controller, passes a lazy evaluator which when executed fetches the content
        /// data so that the controller can decide whether or not to this fetch is done.
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the DataRoute</typeparam>
        /// <param name="routes">A RouteCollection to add the route to</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="defaults">Default values for unmatched pattern elements</param>
        /// <returns>The DataRoute that was created and registered</returns>
        static public DataRoute<TData> AddLazyDataRoute<TData>(this RouteCollection routes, string name, string url, object defaults)
            where TData : class, new()
        {
            ValidateRouteSpec(name, typeof(TData), url, defaults);
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(defaults), new MvcRouteHandler());
            route.LazyData = true;
            routes.Add(name, route);
            return route;
        }

        /// <summary>
        /// Adds a PageRoute with DataRoute functionality for providing data to routes in a classic ASP.Net application
        /// </summary>
        /// <typeparam name="TData">The type of the content data attached by the PageDataRoute</typeparam>
        /// <param name="routes">The route collection</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="pageUrl">The url of the .aspx page template</param>
        /// <returns>The PageDataRoute that was created and registered</returns>
        static public DataRoute<TData> AddPageDataRoute<TData>(this RouteCollection routes, string name, string url, string pageUrl)
            where TData : class, new()
        {
            ContentTypeHierarchy.RegisterType(typeof(TData));
            var route = new DataRoute<TData>(url, new RouteValueDictionary(), new PageRouteHandler(pageUrl));
            routes.Add(name, route);
            return route;
        }

        /// <summary>
        /// Add a route which generates an http redirect to another url
        /// </summary>
        /// <param name="routes">The route collection</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="mapUrl">The url to redirect to including matched elements from the url matching pattern</param>
        /// <returns>The RedirectRoute that was created and registered</returns>
        static public RedirectRoute Redirect(this RouteCollection routes, string name, string url, string mapUrl)
        {
            var route = new RedirectRoute(url, mapUrl, new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a route which generates an http redirect to another url
        /// </summary>
        /// <param name="routes">The route collection</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="mapUrl">The url to redirect to including matched elements from the url matching pattern</param>
        /// <param name="defaults">Default values for matched elements</param>
        /// <returns>The RedirectRoute that was created and registered</returns>
        static public RedirectRoute Redirect(this RouteCollection routes, string name, string url, string mapUrl, object defaults)
        {
            var route = new RedirectRoute(url, mapUrl, new RouteValueDictionary(defaults), new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
        /// <summary>
        /// Add a route which generates an http redirect to another url
        /// </summary>
        /// <param name="routes">The route collection</param>
        /// <param name="name">Name of the route table entry</param>
        /// <param name="url">The url matching pattern</param>
        /// <param name="mapUrl">The url to redirect to including matched elements from the url matching pattern</param>
        /// <param name="defaults">Default values for matched elements</param>
        /// <param name="constraints">Constraints for when the route should match</param>
        /// <returns>The RedirectRoute that was created and registered</returns>
        static public RedirectRoute Redirect(this RouteCollection routes, string name, string url, string mapUrl, object defaults, object constraints)
        {
            var route = new RedirectRoute(url, mapUrl, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }


        static public RequestMatchRoute AddRequestMatchRoute(this RouteCollection routes, string name, string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl)
        {
            var route = new RequestMatchRoute(url, check, conformUrl, new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
        static public RequestMatchRoute AddRequestMatchRoute(this RouteCollection routes, string name, string url, Func<HttpContextBase, bool> check, Func<string, string> conformUrl, object defaults)
        {
            var route = new RequestMatchRoute(url, check, conformUrl, new RouteValueDictionary(defaults), new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }

        static private void ValidateRouteSpec(string name, Type contentType, string url, object defaults)
        {
            bool hasAction = false;
            if (url.Contains("{action}"))
                hasAction = true;
            if (!hasAction)
            {
                var actPi = defaults.GetType().GetProperty("action");
                hasAction = (actPi != null);
            }
            bool hasController = false;
            if (url.Contains("{controller}"))
                hasController = true;
            if (!hasController)
            {
                var contrPi = defaults.GetType().GetProperty("controller");
                hasController = (contrPi != null);
            }
            string error = null;

            if (!(hasAction && hasController))
                error = "Route " + name + " cannot define both action and controller";


            if (typeof(Summary).IsAssignableFrom(contentType))
                error = "Data route " + name + " cannot be declared in terms of a summary type";
            
            if (error != null)
            {
                log.Fatal(error);
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// Analyzes a route to find the url variables in the url pattern and stores them in a dictionary, with the variable name as the
        /// key and the value as ? for a normal variable and ?? for a catchall
        /// </summary>
        /// <param name="route">the route to analyze</param>
        /// <returns>dictionary of variables</returns>
        static public Dictionary<string,string> KeyOutputs(this Route route)
        {
            var vars = route.Url.Split('{')
                .Where(s => s.Contains("}"))
                .Select(s => s.UpTo("}"))
                .ToDictionary(s => s.StartsWith("*") ? s.Substring(1).ToLower() : s.ToLower(), s => s.StartsWith("*") ? "??" : "?");
            foreach (var d in route.Defaults)
            {
                if (vars.ContainsKey(d.Key)) // variable with default
                {
                    if (d.Value == UrlParameter.Optional)
                        vars[d.Key.ToLower()] = "??";
                    else
                        vars[d.Key.ToLower()] = "?" + d.Value.ToString();
                }
                else
                    vars[d.Key.ToLower()] = d.Value.ToString().ToLower();
            }
            return vars;
        }

        /// <summary>
        /// Generates a string defining how to generate a UI for filling in the variables in a url pattern
        /// </summary>
        /// <param name="route">The route whose url pattern is used</param>
        /// <param name="rd">The route data providing the initial values for the url variables</param>
        /// <returns>a string defining how to generate a UI for filling in the variables in a url pattern</returns>
        public static string GetUrlPattern(this Route route, RouteData rd)
        {
            var patt = route.Url;
            var keyOutputs = route.KeyOutputs();

            if (route.RouteHandler is MvcRouteHandler)
            {
                if (!(keyOutputs.ContainsKey("controller") && keyOutputs.ContainsKey("action")))
                    throw new Exception("Route " + route.Url + " fails to define controller and action");

                if (keyOutputs["controller"].StartsWith("?"))
                    throw new Exception("Route " + route.Url + " is a data route which lacks but must have a specified controller");

                if (!ContentTypeHierarchy.ControllerActions.ContainsKey(keyOutputs["controller"]))
                    return null;
            }

            var sbPatt = new StringBuilder();
            foreach (string pattEl in patt.Split('{'))
            {
                if (!pattEl.Contains("}"))
                {
                    sbPatt.Append(pattEl);
                    continue;
                }

                string key = pattEl.UpTo("}");
                string remaining = pattEl.After("}");
                bool isCatchAll = key.StartsWith("*");
                if (isCatchAll)
                    key = key.Substring(1);
                if (key == "action")
                {

                    var actions = ContentTypeHierarchy.ControllerActions[keyOutputs["controller"]];
                    if (!actions.Contains(keyOutputs["action"].ToLower())) // if keyOutputs["action"] starts with "?" it has a default value and can be omitted, so the default action is assumed
                        continue;
                    sbPatt.Append("{");
                    sbPatt.Append(actions.Join("|"));
                    sbPatt.Append("}");
                }
                else
                {
                    string keyVal = "";
                    if (rd != null)
                        keyVal = rd.Values[key].ToString();
                    if (key.StartsWith("_"))
                    {
                        if (isCatchAll)
                            sbPatt.Append("{/" + keyVal + "}"); // catchall, can be empty and contain /
                        else if (keyOutputs[key] == "?")
                            sbPatt.Append("{?" + keyVal + "}"); // mandatory, can't be empty, no /s
                        else
                            sbPatt.Append("{*" + keyVal + "}"); // optional, can be empty at RH end of url, no /s
                            
                    }
                    else
                        sbPatt.Append(keyVal);
                }
                sbPatt.Append(remaining);
            }

            string ps = Regex.Replace(sbPatt.ToString(), "/+", "/");
            ps = Regex.Replace(ps, "/$", "");
            return ps;
        }

        /// <summary>
        /// Process a url through the route table to turn it into a RouteData
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The route data generated if the url was in a request to the site</returns>
        public static RouteData GetRouteDataByUrl(string url)
        {
            if (!(url.StartsWith("/") || url.StartsWith("~/")))
                url = "/" + url;
            return RouteTable.Routes.GetRouteData(new MockHttpContextBase(url));

            //string queryString = url.After("?");
            //var req = new HttpRequest(null,
            //                "http://www.example.com" + url, queryString);
            //var resp = new HttpResponse(new System.IO.StringWriter());
            //var ctx = new HttpContext(req, resp);
            //RouteData rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(ctx));
            //return rd;
        }

        /// <summary>
        /// Process a url through the route table to find what content address it would request content from
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The address from which content would be requested if that url was in a request to the site</returns>
        public static Address GetAddressByUrl(string url)
        {
            RouteData urlRd = RouteX.GetRouteDataByUrl(url);
            if (urlRd.Route is IDataRoute)
            {
                Type routeType = urlRd.Route.GetType().GetGenericArguments()[0];
                return new Address(routeType, urlRd);
            }

            return null;
        }


        public static string CreateUrlFromRouteValues(RouteValueDictionary routeValues)
        {
            var routeData = new RouteData();
            routeData.Route = new Route("", new MvcRouteHandler());
            var requestContext = new RequestContext(new MockHttpContextBase(""), routeData);
            var virtualPathData = RouteTable.Routes.GetVirtualPath(requestContext, routeValues);
            if (virtualPathData == null)
            {
                return null;
            }
            return virtualPathData.VirtualPath;
        }
    }
}
