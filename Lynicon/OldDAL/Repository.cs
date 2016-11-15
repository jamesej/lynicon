using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    public class Repository : TypeRegistry<IRepository, ContentRepository>, IRepository
    {
        static readonly Repository instance = new Repository();
        public static Repository Instance { get { return instance; } }

        static Repository() { }

        public PropertyStore Get(Type type, object id)
        {
            return Registered(type).Get(type, id);
        }
        public IEnumerable<PropertyStore> Get(Type type, IEnumerable<object> ids)
        {
            return Registered(type).Get(type, ids);
        }
        public IEnumerable<PropertyStore> Get(Type type, Dictionary<string, object> clauses)
        {
            return Get(type, clauses, new List<string>(), false);
        }
        public IEnumerable<PropertyStore> Get(Type type, Dictionary<string, object> clauses, List<string> fields, bool excludeFields)
        {
            if (typeof(Summary).IsAssignableFrom(type))
            {
                if (clauses.ContainsKey("@Type"))
                    return Registered((Type)clauses["@Type"]).Get(type, clauses, fields, excludeFields);

                List<Type> types;
                if (clauses.ContainsKey("@Types"))
                    types = (List<Type>)clauses["@Types"];
                else
                    types = ContentTypeHierarchy.GetSummaryContainers(type);

                var typeGroups = types
                    .GroupBy(ct => Registered(ct));
                List<PropertyStore> resAll = new List<PropertyStore>();
                foreach (var typeGroup in typeGroups)
                {
                    clauses["@Types"] = typeGroup.ToList();
                    var res = typeGroup.Key.Get(type, clauses, fields, excludeFields);
                    resAll.AddRange(res);
                }
                return resAll;
            }
            return Registered(type).Get(type, clauses, fields, excludeFields);
        }

        public int GetCount(Type type, Dictionary<string, object> clauses)
        {
            return Registered(type).GetCount(type, clauses);
        }

        public void Set(Type type, PropertyStore val, object id)
        {
            Registered(type).Set(type, val, id);
        }
        public void Set(Type type, IEnumerable<PropertyStore> vals, IEnumerable<object> ids)
        {
            if (!vals.Any()) return;
            Registered(type).Set(type, vals, ids);
        }

        public void Delete(Type type, PropertyStore val, object id)
        {
            Registered(type).Delete(type, val, id);
        }
    }
}
