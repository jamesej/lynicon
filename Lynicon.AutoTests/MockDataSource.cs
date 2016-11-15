using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.DataSources;
using Lynicon.Utility;

namespace Lynicon.AutoTests
{
    public class MockDataSource : IDataSource
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<ItemId, object>> Data = new ConcurrentDictionary<Type, ConcurrentDictionary<ItemId, object>>();

        #region IDataSource Members

        public IQueryable GetSource(Type type)
        {

            if (!Data.ContainsKey(type))
                return Array.CreateInstance(type, 0).AsQueryable();
            else
                return Data[type].Values.OfTypeRuntime(type).AsQueryable();
        }

        public void Update(object o)
        {
            if (o == null)
                return;

            Data[o.GetType()][new ItemId(o)] = o;
        }

        public void Create(object o)
        {
            if (o == null)
                return;

            if (!Data.ContainsKey(o.GetType()))
                Data.TryAdd(o.GetType(), new ConcurrentDictionary<ItemId,object>());

            Data[o.GetType()].TryAdd(new ItemId(o), o);
        }

        public void Delete(object o)
        {
            if (o == null)
                return;

            if (!Data.ContainsKey(o.GetType()))
                return;

            object remd;
            Data[o.GetType()].TryRemove(new ItemId(o), out remd);
        }

        public void SaveChanges()
        { }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
