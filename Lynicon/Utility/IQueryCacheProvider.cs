using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Utility
{
    /// <summary>
    /// A cache provider for storing IQueryables with their results
    /// </summary>
    public interface IQueryCacheProvider
    {
        /// <summary>
        /// Run an IQueryable or get its result from the cache
        /// </summary>
        /// <typeparam name="T">Type of the IQueryable</typeparam>
        /// <param name="query">The IQueryable</param>
        /// <returns>Results of the IQueryable, from cache if possible</returns>
        IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query);
        /// <summary>
        /// Run an IQueryable or get its result from the cache
        /// </summary>
        /// <typeparam name="T">Type of the IQueryable</typeparam>
        /// <param name="query">The IQueryable</param>
        /// <param name="cacheDuration">How long the result will be held in cache</param>
        /// <returns>Results of the IQueryable, from cache if possible</returns>
        IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query, TimeSpan cacheDuration);

        /// <summary>
        /// Remove an IQueryable from the cache
        /// </summary>
        /// <typeparam name="T">Type of the IQueryable</typeparam>
        /// <param name="query">The IQueryable</param>
        /// <returns>Whether the item was removed</returns>
        bool RemoveFromCache<T>(IQueryable<T> query);
    }
}
