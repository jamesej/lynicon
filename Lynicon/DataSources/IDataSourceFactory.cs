using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.DataSources
{
    /// <summary>
    /// Factory interface for creating persistence services with appropriate lifetime
    /// </summary>
    public interface IDataSourceFactory
    {
        /// <summary>
        /// String which specifies how to find or connect to the specific instance of the underlying persistence mechanism
        /// </summary>
        string DataSourceSpecifier { get; }

        /// <summary>
        /// Create an an IDataSource of the appropriate type with appropriate lifetime, with option for this to
        /// for returning summaries instead of fully hydrated records
        /// </summary>
        /// <param name="forSummaries">Whether the data source should return summaries instead of fully hydrated records</param>
        /// <returns>The IDataSource persistence service instance</returns>
        IDataSource Create(bool forSummaries);
    }
}
