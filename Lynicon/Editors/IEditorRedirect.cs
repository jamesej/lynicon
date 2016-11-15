using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Lynicon.Editors
{
    /// <summary>
    /// Interface for editor redirects
    /// </summary>
    public interface IEditorRedirect
    {
        /// <summary>
        /// Decide whether to redirect
        /// </summary>
        /// <typeparam name="T">type of content data</typeparam>
        /// <param name="rd">route data of request</param>
        /// <param name="httpContext">http context data of request</param>
        /// <param name="data">original content data associated with request</param>
        /// <returns>Whether to redirect</returns>
        bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T: class, new();
    }
}
