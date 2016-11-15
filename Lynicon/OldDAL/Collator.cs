using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Routing;
using Lynicon.Utility;

namespace Lynicon.Collation
{
    public class Collator : TypeRegistry<ICollator, ContentCollator>, ICollator
    {
        static readonly Collator instance = new Collator();
        public static Collator Instance { get { return instance; } }

        static Collator() { }

        public object Get(Type type, RouteData rd)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elType = type.GetGenericArguments()[0];
                ICollator coll = Registered(elType);
                IList list = (IList)Activator.CreateInstance(type);
                List<object> oList = coll.GetList(elType, rd);
                if (oList != null)
                    foreach (object o in oList)
                        list.Add(o);
                return list;
            }
            else
                return Registered(type).Get(type, rd);
        }

        public List<T> GetList<T>()
        {
            return GetList(typeof(T), new Dictionary<string, object>()).Cast<T>().ToList();
        }
        public List<T> GetList<T>(Dictionary<string, object> clauses)
        {
            return GetList(typeof(T), clauses).Cast<T>().ToList();
        }
        public List<object> GetList(Type type, Dictionary<string, object> clauses)
        {
            if (typeof(Summary).IsAssignableFrom(type))
            {
                List<Type> types;
                if (clauses.ContainsKey("@Types"))
                    types = (List<Type>)clauses["@Types"];
                else
                    types = ContentTypeHierarchy.GetSummaryContainers(type);
                var typeGroups = types
                    .GroupBy(ct => Registered(ct));
                List<object> res = new List<object>();

                foreach (var typeGroup in typeGroups)
                {
                    clauses["@Types"] = typeGroup.ToList();
                    res.AddRange(typeGroup.Key.GetList(type, clauses));
                }
                return res;
            }
            return Registered(type).GetList(type, clauses);
        }
        public List<T> GetList<T>(RouteData rd)
        {
            return GetList(typeof(T), rd).Cast<T>().ToList();
        }
        public List<object> GetList(Type type, RouteData rd)
        {
            return Registered(type).GetList(type, rd);
        }

        public T GetNew<T>(RouteData rd)
        {
            return (T)GetNew(typeof(T), rd);
        }
        public object GetNew(Type type, RouteData rd)
        {
            return Registered(type).GetNew(type, rd);
        }

        public void Set(object data)
        {
            Set(null, data);
        }
        public void Set(RouteValueDictionary values, object data)
        {
            Registered(data.GetType()).Set(values, data);
        }

        public void Delete(object data)
        {
            Delete(null, data);
        }
        public void Delete(RouteValueDictionary values, object data)
        {
            Registered(data.GetType()).Delete(values, data);
        }

        //public IEnumerable<string> GetUrls(object item)
        //{
        //    Type type = item.GetType();
        //    if (type == typeof(ContentItem))
        //        type = ContentTypeHierarchy.GetContentType((item as ContentItem).DataType);
        //    var routes = RouteTable.Routes
        //        .OfType<Route>()
        //        .Where(r => r.GetType().IsGenericType
        //            && r.GetType().GetGenericTypeDefinition() == typeof(DataRoute<>)
        //            && r.GetType().GetGenericArguments()[0] == type);
        //    foreach (var route in routes)
        //    {
        //        var urls = GetUrls(route, item);
        //        foreach (string url in urls)
        //            yield return url;
        //    }
        //}
        //public List<string> GetUrls(Route route, object item)
        //{
        //    return Registered(route.GetType().GetGenericArguments()[0]).GetUrls(route, item);
        //}

        public Dictionary<string, string> GetAddress(object item)
        {
            Type type = item.GetType();
            if (item is ContentItem)
                type = ContentTypeHierarchy.GetContentType(((ContentItem)item).DataType);
            return Registered(type).GetAddress(item);
        }
    }
}
