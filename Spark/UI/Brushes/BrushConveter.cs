namespace Spark.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using Math;

    public sealed class BrushConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            if (sourceType == typeof(Color))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string source = value as string;
            if (source != null)
            {
                if (source.StartsWith("#", StringComparison.InvariantCulture))
                {
                    return new SolidColorBrush(Color.ParseHexColor(source));
                }

                return Brushes.GetBrush(Color.GetNamedColor(source));
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
