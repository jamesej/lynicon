using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Lynicon.DataSources
{
    /// <summary>
    /// Generate data sources based on a given DbContext type
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to use</typeparam>
    public class EFDataSourceFactory<TContext> : IDataSourceFactory
        where TContext : DbContext, new()
    {
        /// <summary>
        /// The connection string to use for the DbContext
        /// </summary>
        public string DataSourceSpecifier { get; set; }

        /// <summary>
        /// The number of seconds before a query times out for all future data sources created
        /// </summary>
        public int? QueryTimeoutSecs { get; set; }

        /// <summary>
        /// A dictionary by type of functions which supply from the context an IQueryable in that type
        /// </summary>
        public Dictionary<Type, Func<TContext, IQueryable>> DbSetSelectors { get; set; }

        /// <summary>
        /// Create a new EFDataSouceFactory
        /// </summary>
        public EFDataSourceFactory()
        {
            DbSetSelectors = typeof(TContext).GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(
                    pi => pi.PropertyType.GenericTypeArguments[0],
                    pi =>
                    {
                        var x = Expression.Parameter(typeof(TContext));
                        var getDbSet = Expression.MakeMemberAccess(x, pi);
                        var castDbSet = Expression.TypeAs(getDbSet, typeof(IQueryable));
                        var selector = Expression.Lambda<Func<TContext, IQueryable>>(castDbSet, x);
                        return selector.Compile();
                    });
        }

        /// <summary>
        /// Create an an IDataSource of the appropriate type with appropriate lifetime, with option for this to
        /// for returning summaries instead of fully hydrated records
        /// </summary>
        /// <param name="forSummaries">Whether the data source should return summaries instead of fully hydrated records</param>
        /// <returns>The IDataSource persistence service instance</returns>
        public IDataSource Create(bool forSummaries)
        {
            return new EFDataSource<TContext>(DbSetSelectors, ContextLifetimeMode, forSummaries);
        }

        ContextLifetimeMode contextLifetimeMode = ContextLifetimeMode.PerCall;
        /// <summary>
        /// Set how long the context persists for.  Can be per call to the repository or per request
        /// </summary>
        public ContextLifetimeMode ContextLifetimeMode
        {
            get
            {
                return ContextLifetimeMode.PerRequest;
            }
            set
            { }
        }
    }
}
