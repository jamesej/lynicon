using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.Relations
{
    public class ReferenceProperty
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SetReferenceProperties(object o, List<ReferenceProperty> refProps)
        {
            object curr = o;
            string currLeafParent = "";

            foreach (var refProp in refProps)
            {
                string path = refProp.PropertyPath;
                string leafParent = path.Contains(".") ? path.UpToLast(".") : "";
                string leafProperty = path.Contains(".") ? path.LastAfter(".") : path;
                PropertyInfo pi = null;

                if (currLeafParent != leafParent)
                {
                    curr = ReflectionX.GetPropertyValueByPath(o, leafParent);
                }

                if (curr != null)
                    pi = curr.GetType().GetProperty(leafProperty.UpTo("["));

                if (pi == null)
                {
                    log.Debug("Tried to assign reference value to property " + path + " on " + o.GetType().FullName);
                    continue;
                }

                if (typeof(Reference).IsAssignableFrom(pi.PropertyType))
                {
                    if (pi.PropertyType.IsGenericType)
                        pi.SetValue(curr, refProp.ToReference(pi.PropertyType.GetGenericArguments()[0]));
                    else
                        pi.SetValue(curr, refProp.ToReference());
                }
                else
                {
                    bool found = false;
                    string indexStr = leafProperty.After("[").UpTo("]");
                    int idx = -1;
                    int.TryParse(indexStr, out idx);

                    // Now look for the property to be an IList of References
                    if (idx > -1)
                    {
                        foreach (Type interf in pi.PropertyType.GetInterfaces())
                            if (interf.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                found = true;
                                Type elType = interf.GetGenericArguments()[0];
                                if (!typeof(Reference).IsAssignableFrom(elType))
                                    throw new Exception("Cannot insert references into list at " + path + " of element type " + elType.FullName);
                                Reference refForList = null;
                                if (elType.IsGenericType)
                                    refForList = refProp.ToReference(elType.GetGenericArguments()[0]);
                                else
                                    refForList = refProp.ToReference();

                                object refList = pi.GetValue(curr);
                                if (refList == null)
                                {
                                    refList = Activator.CreateInstance(pi.PropertyType);
                                    pi.SetValue(curr, refList);
                                }
                                int count = (int)refList.GetType().GetProperty("Count").GetValue(refList);
                                if (count < idx)
                                    throw new Exception("Reference index " + idx + " at path " + path + " longer than maximum " + (count - 1));
                                if (count == idx)
                                    refList.GetType().GetMethod("Add").Invoke(refList, new object[] { refForList });
                                else
                                    refList.GetType().GetProperty("Item").SetValue(refList, refForList, new object[] { idx });

                                break;
                            }
                    }
                    
                    if (!found)
                        throw new Exception("Trying to SetReferenceProperties on a non reference property " + path);
                }
            }
        }

        public string Id { get; set; }
        public string DataType { get; set; }
        public string PropertyPath { get; set; }

        public Reference ToReference()
        {
            return new Reference { Id = Id, DataType = DataType };
        }

        public Reference ToReference(Type type)
        {
            Type refType = typeof(Reference<>).MakeGenericType(type);
            Reference r = (Reference)Activator.CreateInstance(refType);
            r.DataType = null;
            r.Id = this.Id;
            return r;
        }
    }
}
