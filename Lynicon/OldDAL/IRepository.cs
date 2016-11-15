using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;

namespace Lynicon.Repositories
{
    public interface IRepository
    {
        PropertyStore Get(Type type, object id);
        IEnumerable<PropertyStore> Get(Type type, IEnumerable<object> ids);
        IEnumerable<PropertyStore> Get(Type type, Dictionary<string, object> clauses, List<string> fields, bool excludeFields);
        int GetCount(Type type, Dictionary<string, object> clauses);
        void Set(Type type, PropertyStore val, object id);
        void Set(Type type, IEnumerable<PropertyStore> vals, IEnumerable<object> ids);
        void Delete(Type type, PropertyStore val, object id);
    }
}
