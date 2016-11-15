using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.DataSources
{
    public class CoreDataSourceFactory : IDataSourceFactory
    {
        /// <summary>
        /// The connection string for the database used (if not given, uses value from web.config)
        /// </summary>
        public string DataSourceSpecifier { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for all future queries on the created data sources
        /// </summary>
        public int? QueryTimeoutSecs { get; set; }

        /// <summary>
        /// Create an an IDataSource of the appropriate type with appropriate lifetime, with option for this to
        /// for returning summaries instead of fully hydrated records
        /// </summary>
        /// <param name="forSummaries">Whether the data source should return summaries instead of fully hydrated records</param>
        /// <returns>The IDataSource persistence service instance</returns>
        public IDataSource Create(bool forSummaries)
        {
            var ds = new CoreDataSource(ContextLifetimeMode, forSummaries);
            if (!string.IsNullOrEmpty(DataSourceSpecifier))
                ds.DataSourceSpecifier = this.DataSourceSpecifier;
            if (QueryTimeoutSecs.HasValue)
                ds.QueryTimeoutSecs = this.QueryTimeoutSecs;
            return ds;
        }

        ContextLifetimeMode contextLifetimeMode = ContextLifetimeMode.PerCall;
        /// <summary>
        /// Set how long the context persists for.  Can be per call to the repository or per request
        /// </summary>
        public ContextLifetimeMode ContextLifetimeMode
        {
            get
            {
                if (HttpContext.Current == null)    // nowhere to store context if not in a request thread
                    return ContextLifetimeMode.PerCall;
                else
                    return contextLifetimeMode;
            }
            set
            {
                contextLifetimeMode = value;
            }
        }
    }
}
