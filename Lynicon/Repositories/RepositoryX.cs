using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    public static class RepositoryX
    {
        public static T Get<T>(this IRepository repo, object id)
            where T: class, new()
        {
            PropertyStore ps = repo.Get(typeof(T), id);
            return ps.Project<T>();
        }

        public static IEnumerable<T> Get<T>(this IRepository repo, IEnumerable<object> ids)
            where T : class, new()
        {
            IEnumerable<PropertyStore> pses = repo.Get(typeof(T), ids);
            return pses.Select(ps => ps.Project<T>());
        }

        public static void Set(this IRepository repo, object val, object id)
        {
            PropertyStore ps = new PropertyStore().Inject(val);
            repo.Set(val.GetType(), ps, id);
        }

        public static void Set(this IRepository repo, IEnumerable<object> vals, IEnumerable<object> ids)
        {
            if (vals.Count() == 0)
                throw new ArgumentException("Can't Repository.Set empty list of values");

            IEnumerable<PropertyStore> pses = vals.Select(val => new PropertyStore().Inject(val));
            repo.Set(vals.First().GetType(), pses, ids);
        }
    }
}
