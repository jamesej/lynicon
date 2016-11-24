using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.DataSources
{
    /// <summary>
    /// An empty data source which persists nothing. Useful for setting up data persistence in memory by
    /// unbacked cache.
    /// </summary>
    public class NullDataSource : IDataSource
    {
        public string DataSourceSpecifier { get; set; }

        /// <summary>
        /// No operation
        /// </summary>
        /// <param name="o"></param>
        public void Create(object o)
        { }

        /// <summary>
        /// No operation
        /// </summary>
        /// <param name="o"></param>
        public void Delete(object o)
        { }

        /// <summary>
        /// Returns a Queryable with no items
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IQueryable GetSource(Type type)
        {
            return Array.CreateInstance(type, 0).AsQueryable();
        }

        /// <summary>
        /// No operation
        /// </summary>
        public void SaveChanges()
        { }

        /// <summary>
        /// No operation
        /// </summary>
        /// <param name="o"></param>
        public void Update(object o)
        { }

        /// <summary>
        /// No operation
        /// </summary>
        public void Dispose()
        { }
    }
}
