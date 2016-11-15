using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    public class ContentRepository : IRepository
    {
        public OrderedProcess<IQueryBuilder> ProcessQueryGet { get; private set; }
        public OrderedProcess<IQueryBuilder> ProcessQuerySet { get; private set; }
        public OrderedProcess<IQueryBuilder> ProcessQueryDelete { get; private set; }

        public ContentRepository()
        {
            ProcessQueryGet = new OrderedProcess<IQueryBuilder>();
            ProcessQuerySet = new OrderedProcess<IQueryBuilder>();
            ProcessQueryDelete = new OrderedProcess<IQueryBuilder>();
        }

        #region IRepository Members

        public PropertyStore Get(Type type, object id)
        {
            if (id != null && !(id is string))
                throw new ArgumentException("Invalid id type " + id.GetType().FullName);

            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add("DataType = @dataType");
            query.SqlParameters.Add("@dataType", type.FullName);

            if (id != null)
            {
                query.SqlConditionals.Add("Path = @path");
                query.SqlParameters.Add("@path", id);
            }
            else
                query.SqlConditionals.Add("Path IS NULL");

            query = ProcessQueryGet.Process(query);

            query.SelectModifier = "TOP 1";
            query.SqlTable = "ContentItems";
            return query.RunSelect().FirstOrDefault();
        }

        public IEnumerable<PropertyStore> Get(Type type, IEnumerable<object> ids)
        {
            if (!ids.Any()) return new List<PropertyStore>();

            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add("DataType = @dataType");
            query.SqlParameters.Add("@dataType", type.FullName);
            List<string> idParamNames = Enumerable.Range(0, ids.Count()).Select(n => "@id" + n).ToList();
            query.SqlConditionals.Add("Path IN (" + idParamNames.Join(", ") + ")");
            ids.Select((id, n) => new KeyValuePair<string, string>(idParamNames[n], id as string))
                .Do(kvp => query.SqlParameters.Add(kvp.Key, kvp.Value));

            query = ProcessQueryGet.Process(query);

            query.SelectModifier = "TOP " + ids.Count().ToString();
            query.SqlTable = "ContentItems";
            return query.RunSelect();
        }

        public IEnumerable<PropertyStore> Get(Type type, Dictionary<string, object> clauses, List<string> fields, bool excludeFields)
        {
            var query = QueryBuilderFactory.Instance.Create();

            bool isSummary = typeof(Summary).IsAssignableFrom(type);
            if (clauses.ContainsKey("@Types"))
            {
                query.SqlConditionals.Add("DataType IN ("
                    + ((List<Type>)clauses["@Types"]).Select(sc => query.QuoteString(sc.FullName)).Join(",")
                    + ")");
            }
            else
            {
                query.SqlConditionals.Add("DataType = @dataType");
                if (clauses.ContainsKey("@Type"))
                    query.SqlParameters.Add("@dataType", ((Type)clauses["@Type"]).FullName);
                else
                    query.SqlParameters.Add("@dataType", type.FullName);
            }

            foreach (var kvp in clauses)
            {
                if ("@Types @Type".Contains(kvp.Key))
                    continue;

                query.SqlConditionals.Add(kvp.Key);
                query.SqlParameters.Add("@" + kvp.Key.After("@").UpTo(" "), kvp.Value);
            }

            query = ProcessQueryGet.Process(query);

            if (excludeFields)
                query.SqlFields.AddRange(GetFieldList().Except(fields));
            else
                query.SqlFields.AddRange(fields);

            // Don't get content field for summaries
            if (isSummary && query.SqlFields.Contains("Content"))
                query.SqlFields.Remove("Content");

            query.SqlTable = "ContentItems";
            return query.RunSelect();
        }


        public int GetCount(Type type, Dictionary<string, object> clauses)
        {
            var query = QueryBuilderFactory.Instance.Create();

            query.SqlConditionals.Add("DataType = @dataType");
            query.SqlParameters.Add("@dataType", type.FullName);

            foreach (var kvp in clauses)
            {
                query.SqlConditionals.Add(kvp.Key);
                query.SqlParameters.Add("@" + kvp.Key.After("@").UpTo(" "), kvp.Value);
            }

            query = ProcessQueryGet.Process(query);

            query.SqlTable = "ContentItems";
            return query.RunCount();
        }

        protected virtual List<string> GetFieldList()
        {
            List<string> fieldList = HttpContext.Current.Cache.Get("_LYN_ContentFieldList") as List<string>;
            if (fieldList == null)
            {
                var qb = QueryBuilderFactory.Instance.Create();
                fieldList = qb.GetFieldNames("ContentItems");
                HttpContext.Current.Cache.Add("_LYN_ContentFieldList", fieldList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(2), CacheItemPriority.Normal, null);
            }
            return fieldList;
        }

        public void Set(Type type, PropertyStore val, object id)
        {
            var query = GetQuery(type, val, id, ProcessQuerySet);
            query.ShouldInsert = false; //
            int affected = query.RunUpdateInsert();

            if (affected == 0) // insert
                ContentTypeHierarchy.EnsureContainsType(type);  // this might be the first content item of this type in the db
        }

        public void Set(Type type, IEnumerable<PropertyStore> vals, IEnumerable<object> ids)
        {
            List<PropertyStore> valsList = vals.ToList();
            List<object> idsList = ids.ToList();
            List<IQueryBuilder> updates = new List<IQueryBuilder>();
            for (int i = 0; i < valsList.Count; i++)
            {
                bool shouldInsert = ((Guid)valsList[i]["Id"] == Guid.Empty);
                var uiQuery = GetQuery(type, valsList[i], idsList[i], ProcessQuerySet);
                uiQuery.ShouldInsert = shouldInsert;
                updates.Add(uiQuery);
            }
            var multiQuery = QueryBuilderFactory.Instance.Create();
            multiQuery.RunMultipleUpdate(updates);
        }

        protected virtual IQueryBuilder GetQuery(Type type, PropertyStore val, object id, OrderedProcess<IQueryBuilder> processQuery)
        {
            if (id != null && !(id is string))
                throw new ArgumentException("Invalid id type " + id.GetType().FullName);

            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add("DataType = @dataType");
            query.SqlParameters.Add("@dataType", type.FullName);

            if (id != null)
            {
                query.SqlConditionals.Add("Path = @path");
                query.SqlParameters.Add("@path", id);
            }
            else
                query.SqlConditionals.Add("Path IS NULL");

            if ((Guid)val["Id"] == Guid.Empty)
            {
                // Initialise new ContentItem
                val["Path"] = id;
                val["DataType"] = type.FullName;
                val["Id"] = Guid.NewGuid();
                if ((Guid)val["Identity"] == Guid.Empty)
                    val["Identity"] = Guid.NewGuid();
            }

            val.Do(kvp => query.SqlSets.Add(kvp.Key, kvp.Value));

            query.PreservedFields.Add("Id");
            query.PreservedFields.Add("Identity");

            query = processQuery.Process(query);
            query.SqlTable = "ContentItems";

            return query;
        }

        public void Delete(Type type, PropertyStore val, object id)
        {
            var query = GetQuery(type, val, id, ProcessQueryDelete);
            query.RunDelete();
        }

        #endregion
    }
}
