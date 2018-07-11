namespace Spark.UI.Media
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    using Math;

    public class BrushConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = (string)value;

            if (s.StartsWith("#", StringComparison.InvariantCulture))
            {
                s = s.Substring(1);

                if (s.Length == 6)
                {
                    s = "ff" + s;
                }

                if (s.Length != 8)
                {
                    throw new NotSupportedException("Invalid color string.");
                }

                return new SolidColorBrush(new Color(uint.Parse(s, NumberStyles.HexNumber)));
            }

            PropertyInfo p = typeof(Color).GetProperty(s, BindingFlags.Public | BindingFlags.Static);
            if (p != null && p.PropertyType == typeof(Color))
            {
                return new SolidColorBrush((Color)p.GetValue(null));
            }

            throw new NotSupportedException();
        }
    }
}
