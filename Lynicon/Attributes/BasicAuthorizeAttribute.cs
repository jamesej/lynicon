using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Mark an action method as requiring Basic Authorization.
    /// </summary>
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        bool _RequireSsl = true;
        public bool RequireSsl
        {
            get { return _RequireSsl; }
            set { _RequireSsl = value; }
        }

        protected string Username { get; set; }
        protected string Password { get; set; }

        /// <summary>
        /// Do Basic Authorization using configuration file settings
        /// </summary>
        public BasicAuthorizeAttribute()
        {
            this.Username = ConfigurationManager.AppSettings["BaUsername"];
            this.Password = ConfigurationManager.AppSettings["BaPassword"];
        }
        /// <summary>
        /// Do Basic Authorization with the given username and password
        /// </summary>
        /// <param name="username">Basic Auth username</param>
        /// <param name="password">Basic Auth password</param>
        public BasicAuthorizeAttribute(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");

            if (!Authenticate(filterContext.HttpContext))
            {
                // HttpCustomBasicUnauthorizedResult inherits from HttpUnauthorizedResult and does the
                // work of displaying the basic authentication prompt to the client
                filterContext.Result = new HttpBasicUnauthorizedResult();
            }
            //else
            //{
            //    if (AuthorizeCore(filterContext.HttpContext))
            //    {
            //        HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            //        cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            //        cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            //    }
            //    else
            //    {
            //        // auth failed, display login

            //        // HttpCustomBasicUnauthorizedResult inherits from HttpUnauthorizedResult and does the
            //        // work of displaying the basic authentication prompt to the client
            //        filterContext.Result = new HttpBasicUnauthorizedResult();
            //    }
            //}
        }

        // from here on are private methods to do the grunt work of parsing/verifying the credentials

        private bool Authenticate(HttpContextBase context)
        {
            if (_RequireSsl && !context.Request.IsSecureConnection && !context.Request.IsLocal) return false;

            if (!context.Request.Headers.AllKeys.Contains("Authorization")) return false;

            string authHeader = context.Request.Headers["Authorization"];

            var creds = ParseAuthHeader(authHeader);

            var user = new { Name = creds[0], Pass = creds[1] };
            if (user.Name == Username && user.Pass == Password) return true;

            return false;
        }

        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic")) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

            // Okay this is the credentials
            return credentials;
        }
    }

    public class HttpBasicUnauthorizedResult : HttpUnauthorizedResult
    {
        // the base class already assigns the 401.
        // we bring these constructors with us to allow setting status text
        public HttpBasicUnauthorizedResult() : base() { }
        public HttpBasicUnauthorizedResult(string statusDescription) : base(statusDescription) { }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            // this is really the key to bringing up the basic authentication login prompt.
            // this header is what tells the client we need basic authentication
            context.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic");
            base.ExecuteResult(context);
        }
    }
}
