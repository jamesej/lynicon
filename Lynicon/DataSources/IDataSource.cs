using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.DataSources
{
    /// <summary>
    /// Adapter interface for a data persistence service which can provide an IQueryable for requesting data
    /// </summary>
    public interface IDataSource : IDisposable
    {
        /// <summary>
        /// Get an IQueryable for fetching items of the type given
        /// </summary>
        /// <param name="type">Type of items to fetch</param>
        /// <returns>IQueryable (could have any appropriate type)</returns>
        IQueryable GetSource(Type type);

        /// <summary>
        /// Update an entity in the persistence source
        /// </summary>
        /// <param name="o">The entity to update</param>
        void Update(object o);

        /// <summary>
        /// Create a new entity in the persistence source
        /// </summary>
        /// <param name="o">The entity to create</param>
        void Create(object o);

        /// <summary>
        /// Delete an entity from the persistence source
        /// </summary>
        /// <param name="o">The entity to delete</param>
        void Delete(object o);

        /// <summary>
        /// Save changes made to since last call to this method via Create, Update or Delete methods
        /// </summary>
        void SaveChanges();
    }
}
