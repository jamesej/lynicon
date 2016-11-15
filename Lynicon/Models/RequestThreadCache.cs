using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.Models
{
    /// <summary>
    /// The request thread cache maintains a cache of data scoped to a request, or if not in a request, the current
    /// thread
    /// </summary>
    public class RequestThreadCache
    {
        static readonly RequestThreadCache instance = new RequestThreadCache();

        /// <summary>
        /// The cache currently active - a Dictionary of string x object
        /// </summary>
        public static IDictionary Current
        {
            get
            {
                var reqCache = instance.GetRequestCache();
                if (reqCache != null)
                    return reqCache;
                else
                    return instance.GetThreadCache();
            }
        }

        ThreadLocal<Dictionary<string, object>> threadCache = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());

        public RequestThreadCache()
        { }

        IDictionary GetRequestCache()
        {
            IDictionary items = null;
            try
            {
                items = HttpContext.Current.Items;
            }
            catch { }
            if (items == null)
                return null;
            else
                return items;
        }

        IDictionary GetThreadCache()
        {
            return threadCache.Value;
        }

    }
}
