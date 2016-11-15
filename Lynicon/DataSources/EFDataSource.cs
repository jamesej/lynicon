using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;
using System.Data.Entity;
using System.Web;

namespace Lynicon.DataSources
{
    /// <summary>
    /// A data source based on a specific Entity Framework DbContext
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext</typeparam>
    public class EFDataSource<TContext> : IDataSource
        where TContext : DbContext, new()
    {
        /// <summary>
        /// The connection string to use for this DbContext
        /// </summary>
        public string DataSourceSpecifier { get; set; }

        /// <summary>
        /// The number of seconds before a query times out for all future queries on this data source
        /// </summary>
        public int? QueryTimeoutSecs { get; set; }

        protected TContext Db { get; set; }

        Dictionary<Type, Func<TContext, IQueryable>> dbSetSelectors { get; set; }

        ContextLifetimeMode contextLifetimeMode = ContextLifetimeMode.PerCall;

        /// <summary>
        /// Create a new EFDataSource
        /// </summary>
        /// <param name="dbSetSelectors">A dictionary by type of functions which supply from the context an IQueryable in that type</param>
        /// <param name="contextLifetimeMode">The lifetime of the DbContext used, whether per call or per request</param>
        /// <param name="forSummaries">Whether the data source outputs summaries (or full records)</param>
        public EFDataSource(Dictionary<Type, Func<TContext, IQueryable>> dbSetSelectors, ContextLifetimeMode contextLifetimeMode, bool forSummaries)
        {
            this.contextLifetimeMode = contextLifetimeMode;
            this.dbSetSelectors = dbSetSelectors;
            Db = GetDb();
        }

        /// <summary>
        /// Get the context which the Repository uses for database persistence.  This is a new context
        /// unless ContextLifetimeMode is set to PerRequest in which case it is the instance associated with
        /// the current request
        /// </summary>
        /// <returns>the Repository's database context</returns>
        public virtual TContext GetDb()
        {
            var db = GetDbInner();

            if (QueryTimeoutSecs.HasValue)
                db.Database.CommandTimeout = QueryTimeoutSecs.Value;
            else if (Repository.Instance.QueryTimeoutSecs.HasValue)
                db.Database.CommandTimeout = Repository.Instance.QueryTimeoutSecs;

            return db;
        }

        protected virtual TContext GetDbInner()
        {
            // Uses the service container to get the context.  Generally this will be scoped by
            // request unless overridden to be Transient in config code
            return new TContext();
        }

        /// <summary>
        /// Create a new entity in the persistence source
        /// </summary>
        /// <param name="o">The entity to create</param>
        public void Create(object o)
        {
            Db.Entry(o).State = EntityState.Added;
        }

        /// <summary>
        /// Delete an entity from the persistence source
        /// </summary>
        /// <param name="o">The entity to delete</param>
        public void Delete(object o)
        {
            Db.Entry(o).State = EntityState.Deleted;
        }

        /// <summary>
        /// Get an IQueryable for fetching items of the type given
        /// </summary>
        /// <param name="type">Type of items to fetch</param>
        /// <returns>IQueryable (could have any appropriate type)</returns>
        public IQueryable GetSource(Type type)
        {
            return dbSetSelectors[type](Db);
        }

        /// <summary>
        /// Save changes made to since last call to this method via Create, Update or Delete methods
        /// </summary>
        public void SaveChanges()
        {
            Db.SaveChanges();
        }

        /// <summary>
        /// Update an entity in the persistence source
        /// </summary>
        /// <param name="o">The entity to update</param>
        public void Update(object o)
        {
            Db.SafeUpdate(o);
        }

        /// <summary>
        /// Ensure underlying DbContext is disposed of
        /// </summary>
        public void Dispose()
        {
            if (this.contextLifetimeMode == ContextLifetimeMode.PerCall || HttpContext.Current == null)
                Db.Dispose();
        }
    }
}
