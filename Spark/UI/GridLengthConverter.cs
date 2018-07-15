namespace Spark.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class GridLengthConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value.ToString().Trim();
            GridUnitType type = GridUnitType.Pixel;
            float val = 1;
            float scale = 1;
            bool canBeEmptyString = false;

            if (s == "Auto")
            {
                type = GridUnitType.Auto;
            }
            else if (s.EndsWith("*", StringComparison.InvariantCulture))
            {
                type = GridUnitType.Star;
                s = s.Substring(0, s.Length - 1);
                canBeEmptyString = true;
            }
            else if (s.EndsWith("px", StringComparison.InvariantCulture))
            {
                s = s.Substring(0, s.Length - 2);
            }
            else if (s.EndsWith("in", StringComparison.InvariantCulture))
            {
                s = s.Substring(0, s.Length - 2);
                scale = 96.0f;
            }
            else if (s.EndsWith("cm", StringComparison.InvariantCulture))
            {
                s = s.Substring(0, s.Length - 2);
                scale = 96.0f / 2.54f;
            }
            else if (s.EndsWith("pt", StringComparison.InvariantCulture))
            {
                s = s.Substring(0, s.Length - 2);
                scale = 96.0f / 72.0f;
            }

            if (type != GridUnitType.Auto)
            {
                s = s.Trim();

                if (s.Length > 0)
                {
                    val = float.Parse(s);
                }
                else if (!canBeEmptyString)
                {
                    throw new NotSupportedException("Could not convert grid length value.");
                }
            }

            return new GridLength(val * scale, type);
        }
    }
}
