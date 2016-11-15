using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Utility
{
    public class CacheItem
    {
        public DateTime AdditionTime { get; set; }
        public object Item { get; set; } 
    }

    /// <summary>
    /// A Query Cache Provider which stores IQueryables with their outputs in memory
    /// </summary>
    public class MemoryQueryCacheProvider : IQueryCacheProvider
    {
        private ConcurrentDictionary<string, CacheItem> dictionary = new ConcurrentDictionary<string, CacheItem>();

        public IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query, TimeSpan cacheDuration)
        {
            string key = GetKey<T>(query);

            CacheItem item = dictionary.GetOrAdd(
                key,
                (keyToFind) =>
                {
                    return new CacheItem() { Item = query.ToList(), AdditionTime = DateTime.Now };
                });

            if (DateTime.Now.Subtract(item.AdditionTime) > cacheDuration)
            {
                item = dictionary.AddOrUpdate(
                    key,
                    new CacheItem() { Item = item.Item, AdditionTime = DateTime.Now },
                    (keyToFind, oldItem) =>
                    {
                        return new CacheItem() { Item = query.ToList(), AdditionTime = DateTime.Now };
                    });
            }
            foreach (var oneItem in ((List<T>)item.Item))
            {
                yield return oneItem;
            }
        }

        public IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query)
        {
            string key = GetKey<T>(query);

            CacheItem item = dictionary.GetOrAdd(
                key,
                (keyToFind) =>
                {
                    return new CacheItem() { Item = query.ToList(), AdditionTime = DateTime.Now };
                });

            foreach (var oneItem in ((List<T>)item.Item))
            {
                yield return oneItem;
            }
        }

        public bool RemoveFromCache<T>(IQueryable<T> query)
        {
            string key = GetKey<T>(query);
            CacheItem item = null;
            return dictionary.TryRemove(key, out item);
        }

        private static string GetKey<T>(IQueryable<T> query)
        {
            string key = string.Concat(query.ToString(), "\n\r",
                typeof(T).AssemblyQualifiedName);
            return key;
        }
    }
}
