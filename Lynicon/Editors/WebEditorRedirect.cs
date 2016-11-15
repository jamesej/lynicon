using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Lynicon.Routing;

namespace Lynicon.Editors
{
    /// <summary>
    /// Create a redirect for HTTP webservice editing
    /// </summary>
    public class WebEditorRedirect : IEditorRedirect
    {
        /// <summary>
        /// Any role a user is required to have to read the data
        /// </summary>
        public string ReadRole { get; private set; }
        /// <summary>
        /// Any role a user is required to have to write the data
        /// </summary>
        public string WriteRole { get; private set; }
        /// <summary>
        /// Any role a user is required to have to delete the data
        /// </summary>
        public string DeleteRole { get; private set; }

        /// <summary>
        /// Preprocessing function to change data before it is sent via JSON
        /// </summary>
        public Action<object> AmendDataBeforeGet { get; private set; }

        /// <summary>
        /// Preprocessing function to change data before it is written to the data API
        /// </summary>
        public Func<object, bool> AmendDataBeforeSet { get; private set; }  

        /// <summary>
        /// Confirm permission to write based on the data itself
        /// </summary>
        public Func<ControllerContext, object, FormCollection, bool> VerifySetPermissions { get; private set; }

        /// <summary>
        /// Create a default web editor redirect which requires admin permission for all operations
        /// </summary>
        public WebEditorRedirect()
        {
            ReadRole = WriteRole = DeleteRole = Membership.User.AdminRole;
        }
        /// <summary>
        /// Create a web editor redirect specifying the role required for all operations
        /// </summary>
        /// <param name="readRole">Role required to read</param>
        /// <param name="writeRole">Role required to write</param>
        /// <param name="deleteRole">Role required to delete</param>
        public WebEditorRedirect(string readRole, string writeRole, string deleteRole)
        {
            ReadRole = readRole;
            WriteRole = writeRole;
            DeleteRole = deleteRole;
        }
        /// <summary>
        /// Create a web editor redirect specifying the roles required for all operations plus preprocessing functions
        /// </summary>
        /// <param name="readRole">Role required to read</param>
        /// <param name="writeRole">Role required to write</param>
        /// <param name="deleteRole">Role required to delete</param>
        /// <param name="amendDataBeforeGet">Preprocessing function to change data before it is sent via JSON</param>
        /// <param name="amendDataBeforeSet">Preprocessing function to change data before it is written to the data API</param>
        public WebEditorRedirect(string readRole, string writeRole, string deleteRole, Action<object> amendDataBeforeGet, Func<object, bool> amendDataBeforeSet)
            : this(readRole, writeRole, deleteRole)
        {
            AmendDataBeforeGet = amendDataBeforeGet;
            AmendDataBeforeSet = amendDataBeforeSet;
        }
        /// <summary>
        /// Create a web editor redirect specifying the roles required for all operations plus preprocessing functions and verification of write permission from the item itself
        /// </summary>
        /// <param name="readRole">Role required to read</param>
        /// <param name="writeRole">Role required to write</param>
        /// <param name="deleteRole">Role required to delete</param>
        /// <param name="amendDataBeforeGet">Preprocessing function to change data before it is sent via JSON</param>
        /// <param name="amendDataBeforeSet">Preprocessing function to change data before it is written to the data API</param>
        /// <param name="verifySetPermissions">Confirm permission to write based on the data itself</param>
        public WebEditorRedirect(string readRole, string writeRole, string deleteRole,
            Action<object> amendDataBeforeGet,
            Func<ControllerContext, object, FormCollection, bool> verifySetPermissions,
            Func<object, bool> amendDataBeforeSend)
            : this(readRole, writeRole, deleteRole, amendDataBeforeGet, amendDataBeforeSend)
        {
            VerifySetPermissions = verifySetPermissions;
        }

        #region IEditorRedirect Members


        public bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T: class, new()
        {
            string modeFlag = httpContext == null ? "" : (httpContext.Request.QueryString["$mode"] ?? "").ToLower();
            bool redirect = true;
            if (redirect)
            {
                string action = "Index";
                switch (modeFlag)
                {
                    case "delete":
                        action = "Delete";
                        break;
                }

                rd.RedirectAction("Lynicon", "WebEditor", action);
                rd.RouteHandler = new MvcRouteHandler(); // we need to use standard mvc to show the editor
                rd.DataTokens.Add("Redirect", this);

                if (data == null)
                {
                    data = new T();
                    rd.DataTokens.Add("LynNewItem", true);
                }
                else if (httpContext != null && httpContext.Request.HttpMethod == "GET" && AmendDataBeforeGet != null)
                {
                    AmendDataBeforeGet(data);
                }
            }
            return redirect;
        }

        #endregion
    }
}
