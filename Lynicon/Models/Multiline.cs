using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lynicon.Models
{
    /// <summary>
    /// Content subtype for a multiline (i.e. textarea element) with BbText content
    /// </summary>
    [JsonConverter(typeof(MultilineJsonConverter)), TypeConverter(typeof(MultilineConverter)), Serializable]
    public class Multiline : BbText
    {
        public Multiline()
            : base()
        { }
        public Multiline(string value)
            : base(value)
        { }
    }

    /// <summary>
    /// A converter that serializes a Multiline to/from its text as JSON
    /// </summary>
    public class MultilineJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as Multiline).Text);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Multiline).IsAssignableFrom(objectType);
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
                return (Multiline)null;
            Multiline bbt = Activator.CreateInstance(objectType, reader.Value as string) as Multiline;
            return bbt;
        }
    }

    /// <summary>
    /// A converter that converts a string of text to and from a Multiline type
    /// </summary>
    public class MultilineConverter : TypeConverter
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
                return new Multiline(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as BbText).Text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
