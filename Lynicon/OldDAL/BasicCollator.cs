using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Lynicon.Utility;
using Lynicon.Repositories;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Routing;
using System.Web.Mvc;
using System.Web;
using Lynicon.Attributes;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Lynicon.Map;

namespace Lynicon.Collation
{
    public class BasicCollator : ICollator
    {
        public object Get(Type type, RouteData rd)
        {
            if (rd.Values.ContainsKey("@id"))
            {
                PropertyStore ps = Repository.Instance.Get(type, rd.Values["@id"]);
                return ps == null ? null : ps.Project(type);
            }
            else
                throw new NotSupportedException();
        }

        public object GetNew(Type type, RouteData rd)
        {
            object newObj = Activator.CreateInstance(type);
            if (rd != null && rd.Values.ContainsKey("@id"))
            {
                string idName = (Repository.Instance.Registered(type) as BasicRepository).GetIdName(type);
                PropertyInfo prop = type.GetProperty(idName);
                prop.SetValue(newObj, ReflectionX.ChangeType(rd.Values["@id"], prop.PropertyType));
            }
            return newObj;
        }

        //public void AddFilters(Type type, Dictionary<string, object> clauses, RouteData rd)
        //{
        //    List<string> specialQueryParams = type.GetProperties().Select(pi => pi.Name).ToList();
        //    var qb = QueryBuilderFactory.Instance.Create();
        //    rd.DataTokens
        //        .Where(dt => specialQueryParams.Any(sqp => sqp == dt.Key || dt.Key.StartsWith(sqp + ".")))
        //        .Select(dt => new KeyValuePair<string, object>(
        //}

        public List<object> GetList(Type type, RouteData rd)
        {
            var clauses = new Dictionary<string, object>();
            PagingSpec pagingSpec = PagingSpec.Create(rd);
            if (pagingSpec != null)
                clauses.Add("@Paging", pagingSpec);
            FilterSpec filterSpec = FilterSpec.Create(type, rd);
            if (filterSpec != null)
                filterSpec.AddFilterClauses(clauses);
            List<object> res = GetList(type, clauses);
            if (res != null && res.Count > 0 && res[0].GetType() == typeof(PagingSpec))
            {
                rd.DataTokens.Add("@Paging", res[0]);
                res.RemoveAt(0);
            }
            return res;
        }
        public List<object> GetList(Type type, Dictionary<string, object> clauses)
        {
            bool isSummaryType = typeof(Summary).IsAssignableFrom(type);

            List<PropertyStore> pss;
            Dictionary<string, string> propertyMap = null;
            Type itemType = null;
            if (isSummaryType)
            {
                if (clauses.Keys.Contains("@Types"))
                {
                    var newClauses = clauses.Where(kvp => kvp.Key != "@Types").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    newClauses.Add("@Type", null);
                    List<object> resAll = new List<object>();
                    foreach (Type subType in (List<Type>)clauses["@Types"])
                    {
                        newClauses["@Type"] = subType;
                        var subTypeRes = GetList(type, newClauses);
                        resAll.AddRange(subTypeRes);
                    }
                    return resAll;
                }

                if (clauses.Keys.Contains("@Type"))
                {
                    itemType = (Type)clauses["@Type"];
                    propertyMap = itemType.GetProperties()
                            .Select(pi => new { pi.Name, SummaryAttribute = pi.GetCustomAttribute<SummaryAttribute>() })
                            .Where(pia => pia.SummaryAttribute != null)
                            .ToDictionary(pia => pia.SummaryAttribute.SummaryPropertyName, pia => pia.Name);
                    propertyMap.Add("Id", (Repository.Instance.Registered(itemType) as BasicRepository).GetIdName(itemType));
                }
                else
                    throw new Exception("Getting list of summaries without @Types clause");
            }
    
            pss = Repository.Instance.Get(type, clauses).ToList();

            if (pss == null || pss.Count == 0)
                return new List<object>();

            List<object> res;
            if (clauses.Keys.Contains("@Paging"))
            {
                var paging = pss[0].Project<PagingSpec>();
                res = new List<object> { paging };
                res.AddRange(pss.Skip(1).Select(ps => ps.Project(type, propertyMap)));
            }
            else
                res = pss.Select(ps => ps.Project(type, propertyMap)).ToList();

            if (isSummaryType)
            {
                int idx = 0;
                string idName = (Repository.Instance.Registered(itemType) as BasicRepository).GetIdName(itemType);
                foreach (object summObj in res)
                {
                    Summary summary = (Summary)summObj;
                    PropertyStore ps = pss[idx++];
                    summary.Type = itemType;
                    summary.Url = ContentMap.Instance.GetUrls(ps.Project(itemType)).FirstOrDefault();
                    summary.Id = ps[idName].ToString();
                }
            }

            return res;
        }

        public void Set(RouteValueDictionary values, object data)
        {
            object id = GetId(values, data);
            PropertyStore ps = PropertyStore.CreateFrom(data);
            Repository.Instance.Set(data.GetType(), ps, id);
        }

        public void Delete(RouteValueDictionary values, object data)
        {
            object id = GetId(values, data);
            PropertyStore ps = PropertyStore.CreateFrom(data);
            Repository.Instance.Delete(data.GetType(), ps, id);
        }

        public List<string> GetUrls(Route route, object item)
        {
            string url = route.Url;
            var keyOutputs = route.KeyOutputs();
            foreach (var kvp in keyOutputs)
            {
                if (kvp.Key == "@id") // We're looking at the id variable, the only one matchable by our item
                {
                    PropertyInfo keyProp = item.GetType().GetProperties().FirstOrDefault(p => p.CustomAttributes.Any(cad => cad.AttributeType == typeof(KeyAttribute)));
                    if (keyProp != null)
                    {
                        string id = keyProp.GetValue(item).ToString();
                        url.Replace("{@id}", id);
                    }
                    else
                        url.Replace("{@id}", "?");
                }
                else if (kvp.Value == "?" || kvp.Value == "??")
                    url.Replace("{" + kvp.Key + "}", kvp.Value);
                else if (kvp.Value.StartsWith("?"))
                {
                    
                    url.Replace("{" + kvp.Key + "}", "?");
                }
            }
            while (url.EndsWith("/??"))
                url = url.UpToLast("/??");
            return new List<string> { url };
        }

        public virtual Dictionary<string, string> GetAddress(object item)
        {
            return new Dictionary<string, string>
            {
                { "@id", GetId(null, item).ToString() }
            };
        }

        private object GetId(RouteValueDictionary values, object data)
        {
            object id = null;

            if (values == null) // attempt to get id from data, we don't have a url
            {
                PropertyInfo keyProp = data.GetType().GetProperties().FirstOrDefault(p => p.CustomAttributes.Any(cad => cad.AttributeType == typeof(KeyAttribute)));
                if (keyProp != null)
                    id = keyProp.GetValue(data);
            }
            else if (values.ContainsKey("@id"))
                id = values["@id"];

            if (id == null)
                throw new ArgumentException("Setting basic content type with no key " + data.GetType().FullName);

            return id;
        }
    }
}
