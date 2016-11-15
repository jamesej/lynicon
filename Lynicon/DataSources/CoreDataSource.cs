using Lynicon.Extensibility;
using Lynicon.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lynicon.DataSources
{
    /// <summary>
    /// Data source which provides access to Lynicon's main DbContext
    /// </summary>
    public class CoreDataSource : IDataSource
    {
        /// <summary>
        /// Connection string for database used (if not given uses value from web.config)
        /// </summary>
        public string DataSourceSpecifier { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for all future queries on this data source
        /// </summary>
        public int? QueryTimeoutSecs { get; set; }

        protected DbContext Db { get; set; }

        ContextLifetimeMode contextLifetimeMode = ContextLifetimeMode.PerCall;

        /// <summary>
        /// Create a new CoreDataSource, specifying what its lifetime should be and whether it should return summaries
        /// </summary>
        /// <param name="contextLifetimeMode">Whether the lifetime is per call, or per web request</param>
        /// <param name="forSummaries">Whether the data source returns summaries</param>
        public CoreDataSource(ContextLifetimeMode contextLifetimeMode, bool forSummaries)
        {
            this.contextLifetimeMode = contextLifetimeMode;
            Db = GetDb(forSummaries);
        }

        protected DbContext GetDb(bool forSummaries)
        {
            if (forSummaries)
                return new SummaryDb();

            if (this.contextLifetimeMode == ContextLifetimeMode.PerRequest
                && HttpContext.Current.Items != null
                && HttpContext.Current.Items.Contains("_lynicon_request_context"))
                return (CoreDb)HttpContext.Current.Items["_lynicon_request_context"];

            CoreDb db;
            if (DataSourceSpecifier == null)
                db = new CoreDb();
            else
                db = new CoreDb(DataSourceSpecifier);

            if (this.contextLifetimeMode == ContextLifetimeMode.PerRequest
                && HttpContext.Current.Items != null)
                HttpContext.Current.Items.Add("_lynicon_request_context", db);

            if (QueryTimeoutSecs.HasValue)
                db.Database.CommandTimeout = QueryTimeoutSecs.Value;
            else if (Repository.Instance.QueryTimeoutSecs.HasValue)
                db.Database.CommandTimeout = Repository.Instance.QueryTimeoutSecs;

            return db;
        }


        /// <summary>
        /// Create a new entity in the persistence source
        /// </summary>
        /// <param name="o">The entity to create</param>
        public void Create(object o)
        {
            var item = CompositeTypeManager.Instance.ConvertToComposite(o);
            Db.Entry(item).State = EntityState.Added;
        }

        /// <summary>
        /// Delete an entity from the persistence source
        /// </summary>
        /// <param name="o">The entity to delete</param>
        public void Delete(object o)
        {
            var item = CompositeTypeManager.Instance.ConvertToComposite(o);
            Db.Entry(item).State = EntityState.Deleted;
        }

        /// <summary>
        /// Get an IQueryable for fetching items of the type given
        /// </summary>
        /// <param name="type">Type of items to fetch</param>
        /// <returns>IQueryable (could have any appropriate type)</returns>
        public IQueryable GetSource(Type type)
        {
            if (Db is SummaryDb)
                return ((SummaryDb)Db).SummarisedSet(type);
            else
                return ((CoreDb)Db).CompositeSet(type);
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
            var item = CompositeTypeManager.Instance.ConvertToComposite(o);
            Db.Entry(item).State = EntityState.Modified;
        }

        /// <summary>
        /// Ensure underlying DbContext is disposed of
        /// </summary>
        public void Dispose()
        {
            if (this.contextLifetimeMode == ContextLifetimeMode.PerCall || HttpContext.Current.Items == null)
                Db.Dispose();
        }
    }
}
