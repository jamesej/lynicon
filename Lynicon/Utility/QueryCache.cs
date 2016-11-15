using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Lynicon.Utility
{
    /// <summary>
    /// Global QueryCache provider
    /// </summary>
    public static class QueryCache
    {
        private static IQueryCacheProvider instance;
        private static object locker = new object();

        public static IQueryCacheProvider Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new MemoryQueryCacheProvider();
                    }
                }
                return instance;
            }
            set
            {
                lock (locker)
                {
                    instance = value;
                }
            }
        }

        /// <summary>
        /// Append this operator to an IQueryable to compute it or get it from cache if available
        /// </summary>
        /// <typeparam name="T">Element type of the IQueryable</typeparam>
        /// <param name="query">The source IQueryable</param>
        /// <returns>The computed/cached IEnumerable</returns>
        public static IEnumerable<T> AsCacheable<T>(this IQueryable<T> query)
        {
            return Instance.GetOrCreateCache<T>(query);
        }
        /// <summary>
        /// Append this operator to an IQueryable to compute it or get it from cache if available
        /// </summary>
        /// <typeparam name="T">Element type of the IQueryable</typeparam>
        /// <param name="query">The source IQueryable</param>
        /// <param name="cacheDuration">The duration of caching</param>
        /// <returns>The computed/cached IEnumerable</returns>
        public static IEnumerable<T> AsCacheable<T>(this IQueryable<T> query, TimeSpan cacheDuration)
        {
            return Instance.GetOrCreateCache<T>(query, cacheDuration);
        }
    }

}
