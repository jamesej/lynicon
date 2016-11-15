using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Lynicon.Utility;

namespace Lynicon.Editors
{
    /// <summary>
    /// Common functionality for deciding on editor redirection
    /// </summary>
    public class EditorRedirect : TypeRegistry<IEditorRedirect>, IEditorRedirect
    {
        static readonly EditorRedirect instance = new EditorRedirect();
        public static EditorRedirect Instance { get { return instance; } }

        static EditorRedirect()
        {
            instance.Register(typeof(List<>), new ListEditorRedirect());
        }

        public EditorRedirect()
        {
            this.DefaultHandler = new ContentEditorRedirect();
        }

        #region IEditorRedirect Members

        /// <summary>
        /// Decide whether to redirect
        /// </summary>
        /// <typeparam name="T">type of content data</typeparam>
        /// <param name="rd">route data of request</param>
        /// <param name="httpContext">http context data of request</param>
        /// <param name="data">original content data associated with request</param>
        /// <returns></returns>
        public bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T: class, new()
        {
            if (this.Registered(typeof(T)) == this.DefaultHandler && typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                return Registered(typeof(List<>)).Redirect<T>(rd, httpContext, data);
            else
                return Registered(typeof(T)).Redirect<T>(rd, httpContext, data);
        }

        #endregion
    }
}
