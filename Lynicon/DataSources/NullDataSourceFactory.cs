using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.DataSources
{
    /// <summary>
    /// Factory for NullDataSources
    /// </summary>
    public class NullDataSourceFactory : IDataSourceFactory
    {
        /// <summary>
        /// Always the empty string
        /// </summary>
        public string DataSourceSpecifier
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Create a new NullDataSource
        /// </summary>
        /// <param name="forSummaries"></param>
        /// <returns></returns>
        public IDataSource Create(bool forSummaries)
        {
            return new NullDataSource();
        }
    }
}
