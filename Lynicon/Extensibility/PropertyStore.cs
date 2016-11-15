using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.Extensibility
{
    public class PropertyStore : Dictionary<string, object>
    {
        public static PropertyStore CreateFrom(object o)
        {
            PropertyStore ps = new PropertyStore();
            ps.Inject(o);
            return ps;
        }

        public PropertyStore() : base()
        { }
        public PropertyStore(IDataReader reader) : base()
        {
            Enumerable.Range(0, reader.FieldCount)
                .Do(i => this.Add(reader.GetName(i), reader.GetValue(i) == DBNull.Value ? null : reader.GetValue(i)));
        }

        public PropertyStore Inject(object o)
        {
            o.GetType().GetProperties()
                .Where(prop => !prop.PropertyType.IsAssignableFrom(typeof(PropertyStore)))
                .Do(prop => this[prop.Name] = prop.GetValue(o));
            return this;
        }

        public T Project<T>() where T : class, new()
        {
            T val = new T();
            typeof(T).GetProperties()
                .Where(prop => this.ContainsKey(prop.Name) && prop.CanWrite)
                .Do(prop =>
                {
                    if (prop.PropertyType.IsAssignableFrom(typeof(PropertyStore)))
                        prop.SetValue(val, this);
                    else
                        prop.SetValue(val, this[prop.Name]);
                        
                });
            return val;
        }
        public object Project(Type t)
        {
            object val = Activator.CreateInstance(t);
            t.GetProperties()
                .Where(prop => this.ContainsKey(prop.Name))
                .Do(prop =>
                {
                    if (prop.PropertyType.IsAssignableFrom(typeof(PropertyStore)))
                        prop.SetValue(val, this);
                    else
                        prop.SetValue(val, this[prop.Name]);

                });
            return val;
        }
        public object Project(Type t, Dictionary<string, string> propertyMap)
        {
            if (propertyMap == null)
                return Project(t);

            object val = Activator.CreateInstance(t);
            t.GetProperties()
                .Where(prop => propertyMap.ContainsKey(prop.Name) && this.ContainsKey(propertyMap[prop.Name]))
                .Do(prop =>
                {
                    if (prop.PropertyType.IsAssignableFrom(typeof(PropertyStore)))
                        prop.SetValue(val, this);
                    else
                        prop.SetValue(val, this[propertyMap[prop.Name]]);

                });
            return val;
        }
    }
}
