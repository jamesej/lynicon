using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Lynicon.Attributes;
using Lynicon.Utility;
using System.ComponentModel;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;
using Lynicon.Repositories;
using Lynicon.Models;
using System.Web;

namespace Lynicon.Collation
{
    /// <summary>
    /// Information extracted from a url which maps to one and only one content item
    /// </summary>
    public class Address : Dictionary<string, object>, IEquatable<Address>
    {
        List<string> matched = new List<string>();
        Dictionary<string, string> conversionFormats = new Dictionary<string, string>();
        Type conversionType = null;

        /// <summary>
        /// Returns the C# string formats specified for each address element
        /// </summary>
        public Dictionary<string, string> ConversionFormats
        {
            get
            {
                if (this.Type != null && conversionType != this.Type)
                {
                    conversionFormats = this.Type.GetProperties()
                        .Select(pi => pi.GetCustomAttribute<AddressComponentAttribute>())
                        .Where(aca => aca != null && aca.ConversionFormat != null)
                        .ToDictionary(aca => aca.RouteKey, aca => aca.ConversionFormat);
                    conversionType = this.Type;
                }
                return conversionFormats;
            }
        }

        private Type type = null;
        /// <summary>
        /// The type of the content item addressed
        /// </summary>
        public Type Type
        {
            get
            {
                return type;
            }
            set
            {
                if (!ContentTypeHierarchy.AllContentTypes.Contains(value.ContentType()))
                    throw new ArgumentException("Can't set address type to non-content type " + value.FullName);
                type = value.ContentType();
            }
        }

        /// <summary>
        /// Constructs an empty address
        /// </summary>
        public Address()
        {
        }
        /// <summary>
        /// Constructs an address from any string-object dictionary
        /// </summary>
        /// <param name="dict">a string-object dictionary</param>
        public Address(Type type, IDictionary<string, object> dict) : base(dict)
        {
            this.Type = type;
        }
        /// <summary>
        /// Constructs an address from the type of the content item and a content 'path', a string made up of
        /// a series of string indexes separated by ampersands
        /// </summary>
        /// <param name="type">Type of the addressed item</param>
        /// <param name="contentPath">Series of string indexes separated by ampersands</param>
        public Address(Type type, string contentPath) : this()
        {
            LoadPath(contentPath);
            this.Type = type;
        }
        /// <summary>
        /// Constructs an address from the type of a content item and the route data of the request for the item
        /// </summary>
        /// <param name="type">Type of the content item</param>
        /// <param name="rd">Route data of request for item</param>
        public Address(Type type, RouteData rd)
        {
            var addr = Collator.Instance.GetAddress(type, rd);
            addr.Do(kvp => this.Add(kvp.Key, kvp.Value is string ? ((string)kvp.Value).ToLower() : kvp.Value));
            this.Type = type;
        }
        /// <summary>
        /// Constructs an address from a serialized string form (created via .ToString())
        /// </summary>
        /// <param name="serialized">the serialized string</param>
        public Address(string serialized)
        {
            this.Type = ContentTypeHierarchy.GetContentType(serialized.UpTo(":"));
            foreach (string kvpStr in serialized.After(":").Split('&'))
            {
                this.Add(HttpUtility.UrlDecode(kvpStr.UpTo("=")),
                    HttpUtility.UrlDecode(kvpStr.After("=")));
            }
        }
        /// <summary>
        /// Constructs an address by extracting it from a container or a content item where AddressComponent attributes were
        /// used to map content fields to the address
        /// </summary>
        /// <param name="cont">The container or content item</param>
        public Address(object cont)
            : this()
        {
            var addressProps = cont.GetType().GetProperties()
                .Select(pi => new { pi, attr = pi.GetCustomAttribute<AddressComponentAttribute>() })
                .Where(pii => pii.attr != null)
                .ToList();
            if (addressProps.Any(ap => ap.attr.UsePath))
                LoadPath((string)addressProps.First(ap => ap.attr.UsePath).pi.GetValue(cont));
            else
            {
                addressProps.Do(pii => this.Add(
                    pii.attr.RouteKey ?? ("_" + pii.pi.Name),
                    pii.pi.GetValue(cont)));

                if (this.Count == 0)
                {
                    var keyPi = cont.GetType().GetProperties()
                        .FirstOrDefault(pi => pi.GetCustomAttribute<KeyAttribute>() != null);
                    if (keyPi != null)
                        this.Add("_id", keyPi.GetValue(cont).ToString());
                }
            }

            this.Type = Collator.GetContentType(cont);
        }

        private void LoadPath(string path)
        {
            var pathParts = path.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathParts.Length; i++)
                this.Add("_" + i.ToString(), pathParts[i]);
        }

        /// <summary>
        /// Gets the value of a given key in the address as a string
        /// </summary>
        /// <param name="key">The key to get the value for</param>
        /// <returns></returns>
        public virtual string GetAsString(string key)
        {

            if (ConversionFormats.ContainsKey(key))
                return string.Format("{0:" + ConversionFormats[key] + "}", this[key]);
            else
                return (this[key] ?? "").ToString();
        }

        /// <summary>
        /// Converts the address into a content path (the values in key order separated by '&'s)
        /// </summary>
        /// <returns>Content path</returns>
        public string GetAsContentPath()
        {
            return this.OrderBy(kvp => kvp.Key).Select(kvp => GetAsString(kvp.Key)).Join("&"); //.ToLower();
        }

        /// <summary>
        /// Get a query body which converts the address into a query which should return the item from that address
        /// when used as an argument to a Repository method.
        /// </summary>
        /// <typeparam name="T">Container type query is applied to, which the address addresses</typeparam>
        /// <returns>The query body</returns>
        public Func<IQueryable<T>, IQueryable<T>> GetAsQueryBody<T>()
        {
            Func<IQueryable<T>, IQueryable<T>> queryBody = null;

            if (this.ContainsKey("_id"))
            {
                var idProp = Collator.Instance.GetIdProperty(typeof(T));
                return iq => iq.Where(LinqX.GetPropertyTest<T>(idProp.Name, this["_id"]));
            }

            Dictionary<string, string> keyProps = typeof(T).GetProperties()
                .Select(pi => new { pi, attr = pi.GetCustomAttribute<AddressComponentAttribute>() })
                .Where(pii => pii.attr != null)
                .ToDictionary(pii => pii.attr.UsePath ? "{Path}" : (pii.attr.RouteKey ?? ("_" + pii.pi.Name)),
                    pii => pii.pi.Name);

            if (keyProps.ContainsKey("{Path}"))
            {
                queryBody = iq => iq.Where(LinqX.GetPropertyTest<T>(keyProps["{Path}"], GetAsContentPath()));
            }
            else
            {
                foreach (string key in this.Keys)
                {
                    string propName = keyProps[key];
                    if (queryBody == null)
                        queryBody = iq => iq.Where(LinqX.GetPropertyTest<T>(propName, this[key]));
                    else
                        queryBody = iq => queryBody(iq).Where(LinqX.GetPropertyTest<T>(propName, this[key]));
                }
                if (queryBody == null)
                    queryBody = iq => iq;
            }

            return queryBody;
        }

        /// <summary>
        /// Sets any fields on a content item which map to an address to have the values in this address
        /// </summary>
        /// <param name="item">Content item on which to set address fields</param>
        public void SetAddressFields(object item)
        {
            var propMap = item.GetType()
                .GetProperties()
                .Select(pi => new { Prop = pi, Attr = pi.GetCustomAttribute<AddressComponentAttribute>() })
                .Where(pii => pii.Attr != null)
                .ToDictionary(pii => pii.Attr.RouteKey ?? "|PATH|", pii => pii.Prop);

            foreach (var kvp in this)
            {
                PropertyInfo pi = null;
                if (propMap.ContainsKey(kvp.Key))
                    pi = propMap[kvp.Key];
                else
                    pi = item.GetType().GetProperty(kvp.Key);

                if (pi == null) continue;

                object val = kvp.Value;
                if (val is string && pi.PropertyType != typeof(string))
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(pi.PropertyType);
                    val = typeConverter.ConvertFromString(val as string);
                }

                pi.SetValue(item, val);
            }

            if (propMap.ContainsKey("|PATH|"))
            {
                propMap["|PATH|"].SetValue(item, this.GetAsContentPath());
            }
        }

        /// <summary>
        /// Set a key as having been matched in an algorithm which needs to match all
        /// the keys of the Address
        /// </summary>
        /// <param name="key">The address element key</param>
        public virtual void SetMatched(string key)
        {
            matched.Add(key);
        }

        /// <summary>
        /// Whether all the address element keys have been matched
        /// </summary>
        public bool FullyMatched
        {
            get
            {
                if (matched.Contains("_id")) return true;
                if (Keys.Where(k => k != "_id").All(k => matched.Contains(k))) return true;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Address);
        }

        public override int GetHashCode()
        {
            // Note the hash is independent of the order of the keys in the dictionary
            unchecked
            {
                int hash = 0;
                foreach (var kvp in this)
                {
                    hash += 31 * (kvp.Key == null ? 0 : kvp.Key.GetHashCode()) + (kvp.Value == null ? 0 : kvp.Value.GetHashCode());
                }
                hash = (hash << 5) + 3 + hash ^ (this.Type == null ? 0 : this.Type.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(Address lhs, Address rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                    return true;

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Address lhs, Address rhs)
        {
            return !(lhs == rhs);
        }

        #region IEquatable<Address> Members

        public bool Equals(Address other)
        {
            if (other == null || this.Type != other.Type || this.Count != other.Count)
                return false;

            foreach (var kvp in this)
            {
                if (!other.ContainsKey(kvp.Key))
                    return false;
                if (other[kvp.Key] == null && kvp.Value == null)
                    return true;
                if (other[kvp.Key] == null || !other[kvp.Key].Equals(kvp.Value))
                    return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Serialize the address.  Equality of serialized string is equivalent to equality of the address.
        /// </summary>
        /// <returns>Address serialized to string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.Type.FullName + ":");
            bool first = true;
            foreach (var kvp in this.OrderBy(kvp => kvp.Key))
            {
                if (first)
                    first = false;
                else
                    sb.Append("&");
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");
                string val = (kvp.Value ?? "").ToString();
                //if (kvp.Value is string)
                //    val = val.ToLower();
                sb.Append(HttpUtility.UrlEncode(val));
            }
            return sb.ToString();
        }
    }
}
