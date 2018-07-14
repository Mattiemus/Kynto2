namespace Spark.UI.Media
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class GeometryConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return Geometry.Parse((string)value);
        }
    }
}
