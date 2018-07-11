namespace Spark.Math
{
    using System;
    using System.ComponentModel;

    public class ThicknessConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            char[] separators = { ' ', ',' };
            string[] components = value.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (components.Length == 1)
            {
                return new Thickness(float.Parse(components[0]));
            }

            if (components.Length == 2)
            {
                return new Thickness(
                    float.Parse(components[0]),
                    float.Parse(components[1]),
                    float.Parse(components[0]),
                    float.Parse(components[1]));
            }

            if (components.Length == 4)
            {
                return new Thickness(
                    float.Parse(components[0]),
                    float.Parse(components[1]),
                    float.Parse(components[2]),
                    float.Parse(components[3]));
            }

            throw new NotSupportedException("Value is not valid: must contain one, two or four delineated lengths.");
        }
    }
}
