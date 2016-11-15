using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Lynicon.Routing
{
    /// <summary>
    /// A custom route constraint which checks that the domain of the request matches a regular expression
    /// </summary>
    public class DomainConstraint : IRouteConstraint
    {
        /// <summary>
        /// The regular expression the domain must match
        /// </summary>
        public Regex DomainMatch { get; set; }

        /// <summary>
        /// Create a DomainConstraint from the regular expression the domain must match
        /// </summary>
        /// <param name="regex">The regular expression the domain must match</param>
        public DomainConstraint(string regex)
        {
            DomainMatch = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return DomainMatch.IsMatch(httpContext.Request.Url.Host);
        }
    }
}
