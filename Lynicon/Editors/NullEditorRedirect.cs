using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Lynicon.Routing;

namespace Lynicon.Editors
{
    /// <summary>
    /// An editor redirect which never shows the editor
    /// </summary>
    public class NullEditorRedirect : IEditorRedirect
    {
        #region IEditorRedirect Members

        public bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T: class, new()
        {
            return false;
        }

        #endregion
    }
}
