using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// Abstraction to ease management of ASP.Net cookies
    /// </summary>
    public class CookieObject
    {
        public string Name { get; private set; }

        bool persists = false;
        /// <summary>
        /// Whether the cookie persists for a long time
        /// </summary>
        public bool Persists
        {
            get { return persists; }
            set
            {
                var ctx = EnsureResponseCookie();
                ctx.Response.Cookies[Name].Expires = value ? DateTime.Now.AddYears(1) : DateTime.MinValue;
            }
        }

        private bool httpOnly = false;
        /// <summary>
        /// Whether the cookie is HttpOnly (i.e. cannot be accessed on the client side)
        /// </summary>
        public bool HttpOnly
        {
            get { return httpOnly; }
            set
            {
                var ctx = EnsureResponseCookie();
                ctx.Response.Cookies[Name].HttpOnly = value;
            }
        }

        /// <summary>
        /// Create a cookie object with its name. Any cookie with this name present in the
        /// request can now be managed via the CookieObject.  If none is present, writing to
        /// the CookieObject will create one.
        /// </summary>
        /// <param name="name">Name of the cookie object</param>
        public CookieObject(string name)
        {
            this.Name = name;
        }

        private HttpContext GetHttpContext()
        {
            if (HttpContext.Current == null)
                return null;
            try
            {
                var req = HttpContext.Current.Request;
                return HttpContext.Current;
            }
            catch { }

            return null;
        }

        private HttpContext EnsureResponseCookie()
        {
            var ctx = GetHttpContext();
            if (ctx == null)
                throw new Exception("Attempt to set CookieObject outside of a request context");
            if (ctx.Response.Cookies[Name] == null)
            {
                var cookie = ctx.Request.Cookies[Name];
                if (cookie == null)
                {
                    cookie = new HttpCookie(Name);
                    cookie.HttpOnly = this.HttpOnly;
                    cookie.Expires = this.Persists ? DateTime.Now.AddYears(1) : DateTime.MinValue;
                }
                ctx.Response.Cookies.Add(cookie);
            }

            return ctx;
        }

        /// <summary>
        /// Get or set a value from the cookie by key
        /// </summary>
        /// <param name="key">Key of the value</param>
        /// <returns>Value from cookie</returns>
        public string this[string key]
        {
            get
            {
                var ctx = GetHttpContext();
                if (ctx == null)
                    return null;
                HttpCookie cookie = null;
                if (ctx.Response.Cookies.AllKeys.Contains(Name))
                    cookie = ctx.Response.Cookies[Name];
                else if (ctx.Request.Cookies.AllKeys.Contains(Name))
                    cookie = ctx.Request.Cookies[Name];
                if (cookie == null)
                    return null;
                return HttpUtility.UrlDecode(cookie[key]);
            }

            set
            {
                var ctx = EnsureResponseCookie();
                ctx.Response.Cookies[Name][key] = HttpUtility.UrlEncode(value);
            }
        }
    }
}
