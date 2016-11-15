using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lynicon.Attributes;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    public class BasicRepository : IRepository
    {
        public OrderedProcess<IQueryBuilder> ProcessQueryGet { get; private set; }
        public OrderedProcess<IQueryBuilder> ProcessQuerySet { get; private set; }
        public string TableName { get; set; }
        public string IdName { get; set; }

        public BasicRepository(string tableName)
        {
            ProcessQueryGet = new OrderedProcess<IQueryBuilder>();
            ProcessQuerySet = new OrderedProcess<IQueryBuilder>();
            TableName = null;
            IdName = null;
        }

        private string GetTableName(Type tableType)
        {
            if (TableName != null) return TableName;
            var attr = tableType.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (attr != null)
                return ((TableAttribute)attr).Name;
            return tableType.Name;
        }

        public string GetIdName(Type tableType)
        {
            if (IdName != null) return IdName;
            var keyProp = tableType.GetProperties().FirstOrDefault(pi => pi.GetCustomAttribute<KeyAttribute>() != null);
            if (keyProp == null)
                throw new Exception("Can't find Id Name of " + tableType.FullName);
            return keyProp.Name;
        }

        #region IRepository Members

        public PropertyStore Get(Type type, object id)
        {
            string idName = GetIdName(type);
            object idConv = ConvertForField(type, idName, id);
            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add(idName + " = @id");
            query.SqlParameters.Add("@id", idConv);

            query = ProcessQueryGet.Process(query);

            query.SelectModifier = "TOP 1";
            query.SqlTable = GetTableName(type);
            return query.RunSelect().FirstOrDefault();
        }

        private object ConvertForField(Type type, string propertyName, object val)
        {
            Type propertyType = type.GetProperty(propertyName).PropertyType;
            if (val.GetType() != propertyType)
                return ReflectionX.ChangeType(val, propertyType);
            else
                return val;
        }

        protected virtual List<string> FieldsForSummary(Type tableType, Type summaryType)
        {
            List<string> summaryProps = summaryType.GetProperties().Select(pi => pi.Name).ToList();
            var propertyMap = tableType.GetProperties()
                .Select(pi => new { pi.Name, SummaryAttribute = pi.GetCustomAttribute<SummaryAttribute>() })
                .Where(pia => pia.SummaryAttribute != null && summaryProps.Contains(pia.SummaryAttribute.SummaryPropertyName))
                .Select(pia => pia.Name)
                .ToList();
            propertyMap.Add(GetIdName(tableType));    // Add Id as needed to generate url
            return propertyMap;
        }

        public IEnumerable<PropertyStore> Get(Type type, Dictionary<string, object> clauses, List<string> fields, bool exceptFields)
        {
            // Build a list of multiple types - have to recurse using @Type argument
            if (clauses.ContainsKey("@Types"))
            {
                var newClauses = clauses.Where(kvp => kvp.Key != "@Types").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                newClauses.Add("@Type", null);
                foreach (Type subType in (List<Type>)clauses["@Types"])
                {
                    newClauses["@Type"] = subType;
                    var res = Get(type, newClauses, fields, exceptFields);
                    foreach (PropertyStore ps in res)
                        yield return ps;
                }
            }

            Type tableType = type;
            bool isSummaryType = false;
            if (typeof(Summary).IsAssignableFrom(type))
            {
                isSummaryType = true;
                if (clauses.ContainsKey("@Type"))
                    tableType = (Type)clauses["@Type"];
                else
                    throw new ArgumentException("Must specify a type clause if returning summary type " + type.FullName);
            }

            var query = QueryBuilderFactory.Instance.Create();
            if (isSummaryType)
            {
                query.SqlFields.AddRange(FieldsForSummary(tableType, type));
            }
            else
                query.SqlFields.AddRange(fields);

            PagingSpec paging = null;
            foreach (var kvp in clauses)
            {
                if (kvp.Key == "@Paging")
                {
                    paging = (PagingSpec)kvp.Value;
                }
                else if (!"@Type @Types".Contains(kvp.Key))
                {
                    query.SqlConditionals.Add(kvp.Key);
                    query.SqlParameters.Add("@" + kvp.Key.After("@").UpTo(" "), kvp.Value);
                }
            }

            query = EventHub.Instance.ProcessEvent<IQueryBuilder>("BeforeContentGet", this, query);
            query.SqlTable = GetTableName(tableType);
            if (paging == null)
                foreach (var ps in query.RunSelect())
                    yield return ps;
            else
            {
                int count = 0;
                string idName = GetIdName(tableType);
                List<PropertyStore> result = query.RunPagedSelect(paging.Skip, paging.Take,
                    idName, type.GetProperty(idName).PropertyType,
                    paging.Sort,
                    string.IsNullOrEmpty(paging.Sort) ? null : type.GetProperty(paging.Sort).PropertyType,
                    out count);
                paging.Total = count;
                result.Insert(0, PropertyStore.CreateFrom(paging));
                foreach (var ps in result)
                    yield return ps;
            }
        }

        public IEnumerable<PropertyStore> Get(Type type, IEnumerable<object> ids)
        {
            if (!ids.Any()) return new List<PropertyStore>();

            var query = QueryBuilderFactory.Instance.Create();
            List<string> idParamNames = Enumerable.Range(0, ids.Count()).Select(n => "@id" + n).ToList();
            string idName = GetIdName(type);
            query.SqlConditionals.Add(idName + " IN (" + idParamNames.Join(", ") + ")");
            ids.Select((id, n) => new KeyValuePair<string, string>(idParamNames[n], id as string))
                .Do(kvp => query.SqlParameters.Add(kvp.Key, kvp.Value));

            query = ProcessQueryGet.Process(query);

            query.SelectModifier = "TOP " + ids.Count().ToString();
            query.SqlTable = GetTableName(type);
            return query.RunSelect();
        }

        public int GetCount(Type type, Dictionary<string, object> clauses)
        {
            var query = QueryBuilderFactory.Instance.Create();
            foreach (var kvp in clauses)
            {
                query.SqlConditionals.Add(kvp.Key);
                query.SqlParameters.Add("@" + kvp.Key.After("@").UpTo(" "), kvp.Value);
            }

            query = ProcessQueryGet.Process(query);
            query.SqlTable = GetTableName(type);
            return query.RunCount();
        }

        public void Set(Type type, PropertyStore val, object id)
        {
            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add(GetIdName(type) + " = @id");
            query.SqlParameters.Add("@id", id);

            string idName = GetIdName(type);
            // Assumes int primary keys will be identities
            val.Where(kvp => kvp.Key != idName || !(kvp.Value is int))
                .Do(kvp => query.SqlSets.Add(kvp.Key, kvp.Value));

            query = ProcessQuerySet.Process(query);
            query.SqlTable = GetTableName(type);
            query.RunUpdateInsert();
        }

        public void Set(Type type, IEnumerable<PropertyStore> vals, IEnumerable<object> ids)
        {
            // slow and dirty
            List<PropertyStore> valsList = vals.ToList();
            List<object> idsList = ids.ToList();
            for (int i = 0; i < valsList.Count; i++)
            {
                Set(type, valsList[i], idsList[i]);
            }
        }

        public void Delete(Type type, PropertyStore val, object id)
        {
            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add(GetIdName(type) + " = @id");
            query.SqlParameters.Add("@id", id);

            // Assumes int primary keys will be identities
            val.Where(kvp => kvp.Key != GetIdName(type) || !(kvp.Value is int))
                .Do(kvp => query.SqlSets.Add(kvp.Key, kvp.Value));

            query = ProcessQuerySet.Process(query);
            query.SqlTable = GetTableName(type);
            query.RunDelete();
        }

        #endregion
    }
}
