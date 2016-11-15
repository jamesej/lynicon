using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// Collects CookieObjects as a service for management of http cookies
    /// </summary>
    public class CookieManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly CookieManager instance = new CookieManager();
        public static CookieManager Instance { get { return instance; } }

        static CookieManager() { }

        public const string LynCookieId = "lyn_control";

        private Dictionary<string, CookieObject> cookies = new Dictionary<string, CookieObject>();

        /// <summary>
        /// Add a new cookie with the given name to the cookie manager
        /// </summary>
        /// <param name="name">Name of the cookie</param>
        public void Add(string name)
        {
            cookies.Add(name, new CookieObject(name));
        }

        /// <summary>
        /// Test whether cookie manager has a given named cookie
        /// </summary>
        /// <param name="name">Name of the cookie</param>
        /// <returns>True if manager has cookie</returns>
        public bool Contains(string name)
        {
            return cookies.Keys.Contains(name);
        }

        /// <summary>
        /// List of all the CookieObjects in the manager
        /// </summary>
        public List<CookieObject> Cookies
        {
            get { return cookies.Values.ToList();  }
        }

        /// <summary>
        /// Get a CookieObject by name
        /// </summary>
        /// <param name="name">name of a cookie</param>
        /// <returns>the CookieObject</returns>
        public CookieObject this[string name]
        {
            get { return cookies[name];  }
        }

        /// <summary>
        /// Get the general purpose cookie for Lynicon
        /// </summary>
        public CookieObject LyniconCookie
        {
            get
            {
                if (!Contains(LynCookieId))
                    Add(LynCookieId);
                return this[LynCookieId];
            }
        }
    }
}
