using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyotek.Web.BbCodeFormatter;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using Lynicon.Utility;

namespace Lynicon.Models
{
    /// <summary>
    /// A converter that serializes a MinHtml to/from its html as JSON
    /// </summary>
    public class MinHtmlJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as MinHtml).Html);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(MinHtml).IsAssignableFrom(objectType);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return (MinHtml)null;
            // we assume the MinHtml is always clean when serialized
            MinHtml mHtml = Activator.CreateInstance(objectType, reader.Value as string, true) as MinHtml;
            return mHtml;
        }
    }

    /// <summary>
    /// A converter that converts a string with HTML to and from a MinHtml type
    /// </summary>
    public class MinHtmlConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new MinHtml(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as MinHtml).Html;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Type which contains a very reduced subset of html consisting of basic headings, font formatting and links
    /// </summary>
    [JsonConverter(typeof(MinHtmlJsonConverter)), TypeConverter(typeof(MinHtmlConverter)), Serializable]
    public class MinHtml : IHtmlString
    {
        public static implicit operator MinHtml(string s)
        {
            if (s == null)
                return (MinHtml)null;
            return new MinHtml(s);
        }
        public static implicit operator string(MinHtml t)
        {
            if (t == null) return (string)null;
            return t.ToString();
        }

        /// <summary>
        /// Constant for an empty MinHtml
        /// </summary>
        public static MinHtml Empty
        {
            get
            {
                return new MinHtml("");
            }
        }

        /// <summary>
        /// The HTML of the MinHtml
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Create an empty MinHtml
        /// </summary>
        public MinHtml()
        { }
        /// <summary>
        /// Create a MinHtml from an HTML string (which is cleaned of illegal tags)
        /// </summary>
        /// <param name="anyHtml">HTML string</param>
        public MinHtml(string anyHtml)
        {
            Html = Clean(anyHtml);
        }
        /// <summary>
        /// Create a MinHtml from an HTML string with optional cleaning
        /// </summary>
        /// <param name="anyHtml">HTML string</param>
        /// <param name="isClean">If true, don't clean</param>
        public MinHtml(string anyHtml, bool isClean)
        {
            Html = isClean ? anyHtml : Clean(anyHtml);
        }

        public virtual string Clean(string html)
        {
            return HtmlX.MinimalHtml(html, true);
        }

        /// <summary>
        /// Return the HTML
        /// </summary>
        /// <returns>HTML string</returns>
        public override string ToString()
        {
            return Html ?? "";
        }

        #region IHtmlString Members

        /// <summary>
        /// Return the HTML string
        /// </summary>
        /// <returns>HTML string</returns>
        public string ToHtmlString()
        {
            return Html;
        }

        #endregion
    }
    
    /// <summary>
    /// Subtype of min HTML not allowing links
    /// </summary>
    public class MinHtmlNoLinks : MinHtml
    {
        public override string Clean(string html)
        {
            return HtmlX.MinimalHtml(html, false);
        }
    }
}
