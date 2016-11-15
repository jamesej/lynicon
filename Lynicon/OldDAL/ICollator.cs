using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Lynicon.Collation
{
    public interface ICollator
    {
        object Get(Type type, RouteData rd);
        List<object> GetList(Type type, RouteData rd);
        List<object> GetList(Type type, Dictionary<string, object> fieldValues);
        object GetNew(Type type, RouteData rd);
        void Set(RouteValueDictionary values, object data);
        void Delete(RouteValueDictionary values, object data);
        Dictionary<string, string> GetAddress(object data);
    }
}
