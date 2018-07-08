namespace Spark.Math
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Reflection;

    using Content;

    /// <summary>
    /// Defines a packed 32-bit color using red, green, blue, and alpha components (in RGBA order).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct Color : IEquatable<Color>, IFormattable, IPrimitiveValue
    {
        private static Dictionary<string, Color> NamedColors;

        /// <summary>
        /// Red component.
        /// </summary>
        public byte R;

        /// <summary>
        /// Green component.
        /// </summary>
        public byte G;

        /// <summary>
        /// Blue component.
        /// </summary>
        public byte B;

        /// <summary>
        /// Alpha component.
        /// </summary>
        public byte A;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="rgba">Packed int containing RGBA values.</param>
        public Color(uint rgba)
        {
            UnpackColor(rgba, out R, out G, out B, out A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGBA byte values with range 0-255. If
        /// the component values are greater than 255 or less than 0, they will be clamped
        /// to the range 0-255.
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        public Color(int red, int green, int blue)
        {
            R = (byte)MathHelper.ClampToByte(red);
            G = (byte)MathHelper.ClampToByte(green);
            B = (byte)MathHelper.ClampToByte(blue);
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGBA byte values with range 0-255. If
        /// the component values are greater than 255 or less than 0, they will be clamped to the range 0-255.
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        /// <param name="alpha">Alpha component</param>
        public Color(int red, int green, int blue, int alpha)
        {
            R = (byte)MathHelper.ClampToByte(red);
            G = (byte)MathHelper.ClampToByte(green);
            B = (byte)MathHelper.ClampToByte(blue);
            A = (byte)MathHelper.ClampToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGB float values with range 0 to 1.0f. Alpha is set to 1.0f.
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        public Color(float red, float green, float blue)
        {
            R = (byte)MathHelper.ClampAndRound(red * 255.0f, 0.0f, 255.0f);
            G = (byte)MathHelper.ClampAndRound(green * 255.0f, 0.0f, 255.0f);
            B = (byte)MathHelper.ClampAndRound(blue * 255.0f, 0.0f, 255.0f);
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGBA float values with range 0 to 1.0f.
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        /// <param name="alpha">Alpha component</param>
        public Color(float red, float green, float blue, float alpha)
        {
            R = (byte)MathHelper.ClampAndRound(red * 255.0f, 0.0f, 255.0f);
            G = (byte)MathHelper.ClampAndRound(green * 255.0f, 0.0f, 255.0f);
            B = (byte)MathHelper.ClampAndRound(blue * 255.0f, 0.0f, 255.0f);
            A = (byte)MathHelper.ClampAndRound(alpha * 255.0f, 0.0f, 255.0f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGB float values with range 0 to 1.0f.
        /// </summary>
        /// <param name="rgb">Vector3 containing RGB components as XYZ</param>
        public Color(Vector3 rgb)
        {
            R = (byte)MathHelper.ClampAndRound(rgb.X * 255.0f, 0.0f, 255.0f);
            G = (byte)MathHelper.ClampAndRound(rgb.Y * 255.0f, 0.0f, 255.0f);
            B = (byte)MathHelper.ClampAndRound(rgb.Z * 255.0f, 0.0f, 255.0f);
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGBA float values with range 0 to 1.0f.
        /// </summary>
        /// <param name="rgb">Vector3 containing RGB components as XYZ</param>
        /// <param name="alpha">Alpha component</param>
        public Color(Vector3 rgb, float alpha)
        {
            R = (byte)MathHelper.ClampAndRound(rgb.X * 255.0f, 0.0f, 255.0f);
            G = (byte)MathHelper.ClampAndRound(rgb.Y * 255.0f, 0.0f, 255.0f);
            B = (byte)MathHelper.ClampAndRound(rgb.Z * 255.0f, 0.0f, 255.0f);
            A = (byte)MathHelper.ClampAndRound(alpha * 255.0f, 0.0f, 255.0f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from RGBA float values with range 0 to 1.0f.
        /// </summary>
        /// <param name="rgba">Vector4 containing RGBA components as XYZW</param>
        public Color(Vector4 rgba)
        {
            R = (byte)MathHelper.ClampAndRound(rgba.X * 255.0f, 0.0f, 255.0f);
            G = (byte)MathHelper.ClampAndRound(rgba.Y * 255.0f, 0.0f, 255.0f);
            B = (byte)MathHelper.ClampAndRound(rgba.Z * 255.0f, 0.0f, 255.0f);
            A = (byte)MathHelper.ClampAndRound(rgba.W * 255.0f, 0.0f, 255.0f);
        }

        /// <summary>
        /// Static constructor for the <see cref="Color"/> struct.
        /// </summary>
        static Color()
        {
            NamedColors = new Dictionary<string, Color>();

            PropertyInfo[] props = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo p in props)
            {
                if (p.PropertyType == typeof(Color))
                {
                    NamedColors.Add(
                        p.Name.ToLower(),
                        (Color)p.GetValue(null, null));
                }
            }
        }

        /// <summary>
        /// Gets the size of <see cref="Color"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Color>();

        /// <summary>
        /// Gets the hue of the color. First component of the Hue-Saturation-Brightness (HSB) representation of the color.
        /// </summary>
        public float Hue
        {
            get
            {
                if (R == G && G == B)
                {
                    return 0.0f;
                }

                float r = R / 255.0f;
                float g = G / 255.0f;
                float b = B / 255.0f;

                float max = r;
                float min = r;
                float delta;
                float hue = 0.0f;

                if (g > max)
                {
                    max = g;
                }

                if (b > max)
                {
                    max = b;
                }

                if (g < min)
                {
                    min = g;
                }

                if (b < min)
                {
                    min = b;
                }

                delta = max - min;

                if (r == max)
                {
                    hue = (g - b) / delta;
                }
                else if (g == max)
                {
                    hue = 2.0f + (b - r) / delta;
                }
                else if (b == max)
                {
                    hue = 4.0f + (r - g) / delta;
                }

                hue *= 60.0f;

                if (hue < 0.0f)
                {
                    hue += 360.0f;
                }

                return hue;
            }
        }

        /// <summary>
        /// Gets the saturation of the color. Second component of the Hue-Saturation-Brightness (HSB) representation of the color.
        /// </summary>
        public float Saturation
        {
            get
            {
                float r = R / 255.0f;
                float g = G / 255.0f;
                float b = B / 255.0f;

                float max = r;
                float min = r;
                float saturation = 0.0f;

                if (g > max)
                {
                    max = g;
                }

                if (b > max)
                {
                    max = b;
                }

                if (g < min)
                {
                    min = g;
                }

                if (b < min)
                {
                    min = b;
                }

                // If max == min, then there is no color and the saturation is zero
                if (max != min)
                {
                    float l = (max + min) / 2.0f;
                    if (l <= 0.5f)
                    {
                        saturation = (max - min) / (max + min);
                    }
                    else
                    {
                        saturation = (max - min) / (2.0f - max - min);
                    }
                }

                return saturation;
            }
        }

        /// <summary>
        /// Gets the brightness of the color. Third component of the Hue-Saturation-Brightness (HSB) representation of the color.
        /// </summary>
        public float Brightness
        {
            get
            {
                float r = R / 255.0f;
                float g = G / 255.0f;
                float b = B / 255.0f;

                float max = r;
                float min = r;

                if (g > max)
                {
                    max = g;
                }

                if (b > max)
                {
                    max = b;
                }

                if (g < min)
                {
                    min = g;
                }

                if (b < min)
                {
                    min = b;
                }

                return (max + min) / 2.0f;
            }
        }

        /// <summary>
        /// Gets the color as a packed RGBA value.
        /// </summary>
        public uint PackedValue => (uint)(R | (G << 8) | (B << 16) | (A << 24));

        /// <summary>
        /// Gets or sets individual components of the color in the order that the components are declared (RGBA).
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>The value of the specified component.</returns>
        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return R;
                    case 1:
                        return G;
                    case 2:
                        return B;
                    case 3:
                        return A;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        R = value;
                        break;
                    case 1:
                        G = value;
                        break;
                    case 2:
                        B = value;
                        break;
                    case 3:
                        A = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
                }
            }
        }

        /// <summary>
        /// Converts a color from a packed BGRA format.
        /// </summary>
        /// <param name="bgra">Packed format, 4 bytes, one for each component in BGRA order.</param>
        /// <returns>Color</returns>
        public static Color FromBGRA(uint bgra)
        {
            FromBGRA(bgra, out Color result);
            return result;
        }

        /// <summary>
        /// Converts a color from a packed BGRA format.
        /// </summary>
        /// <param name="bgra">Packed format, 4 bytes, one for each component in BGRA order.</param>
        /// <param name="result">Color</param>
        public static void FromBGRA(uint bgra, out Color result)
        {
            UnpackColor(bgra, out byte r, out byte g, out byte b, out byte a);
            result.R = b;
            result.G = g;
            result.B = r;
            result.A = a;
        }
        
        /// <summary>
        /// Adjusts the contrast of the color. If the contrast is 0, the color is 50% gray
        /// and if its 1 the original color is returned.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="contrast">Contrast amount</param>
        /// <returns>Adjusted color</returns>
        public static Color AdjustContrast(Color value, float contrast)
        {
            AdjustContrast(ref value, contrast, out Color result);
            return result;
        }

        /// <summary>
        /// Adjusts the contrast of the color. If the contrast is 0, the color is 50% gray
        /// and if its 1 the original color is returned.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="contrast">Contrast amount</param>
        /// <param name="result">Adjusted color</param>
        public static void AdjustContrast(ref Color value, float contrast, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte((int)(((value.R - 128) * contrast) + 128));
            result.G = (byte)MathHelper.ClampToByte((int)(((value.G - 128) * contrast) + 128));
            result.B = (byte)MathHelper.ClampToByte((int)(((value.B - 128) * contrast) + 128));
            result.A = value.A;
        }

        /// <summary>
        /// Adjusts the saturation of the color. If the saturation is 0, then the grayscale
        /// color is chosen and if its 1, then the original color is returned.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="saturation">Saturation amount</param>
        /// <returns>Adjusted color</returns>
        public static Color AdjustSaturation(Color value, float saturation)
        {
            AdjustSaturation(ref value, saturation, out Color result);
            return result;
        }

        /// <summary>
        /// Adjusts the saturation of the color. If the saturation is 0, then the grayscale
        /// color is chosen and if its 1, then the original color is returned.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="saturation">Saturation amount</param>
        /// <param name="result">Adjusted color</param>
        public static void AdjustSaturation(ref Color value, float saturation, out Color result)
        {
            uint r = value.R;
            uint g = value.G;
            uint b = value.B;

            uint grey = (uint)MathHelper.Clamp(((r * .2125f) + (g * .7154f) + (b * .0721f)), 0.0f, 255.0f);

            result.R = (byte)MathHelper.ClampToByte((int)(((r - grey) * saturation) + grey));
            result.G = (byte)MathHelper.ClampToByte((int)(((g - grey) * saturation) + grey));
            result.B = (byte)MathHelper.ClampToByte((int)(((b - grey) * saturation) + grey));
            result.A = value.A;
        }

        /// <summary>
        /// Clamps the color within range of the min and max values.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped color</returns>
        public static Color Clamp(Color value, Color min, Color max)
        {
            Clamp(ref value, ref min, ref max, out Color result);
            return result;
        }

        /// <summary>
        /// Clamps the color within range of the min and max values.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="result">Clamped color</param>
        public static void Clamp(ref Color value, ref Color min, ref Color max, out Color result)
        {
            result.R = (byte)MathHelper.Clamp(value.R, min.R, max.R);
            result.G = (byte)MathHelper.Clamp(value.G, min.G, max.G);
            result.B = (byte)MathHelper.Clamp(value.B, min.B, max.B);
            result.A = (byte)MathHelper.Clamp(value.A, min.A, max.A);
        }

        /// <summary>
        /// Gets the color that contains the maximum value from each of the components of the 
        /// two supplied colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Maximum color</returns>
        public static Color Max(Color a, Color b)
        {
            Max(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Gets the color that contains the maximum value from each of the components of the 
        /// two supplied colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="result">Maximum color</param>
        public static void Max(ref Color a, ref Color b, out Color result)
        {
            result.R = (a.R > b.R) ? a.R : b.R;
            result.G = (a.G > b.G) ? a.G : b.G;
            result.B = (a.B > b.B) ? a.B : b.B;
            result.A = (a.A > b.A) ? a.A : b.A;
        }

        /// <summary>
        /// Gets the color that contains the mininum value from each of the components of the 
        /// two supplied colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Minimum color</returns>
        public static Color Min(Color a, Color b)
        {
            Min(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Gets the color that contains the mininum value from each of the components of the 
        /// two supplied colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="result">Minimum color</param>
        public static void Min(ref Color a, ref Color b, out Color result)
        {
            result.R = (a.R < b.R) ? a.R : b.R;
            result.G = (a.G < b.G) ? a.G : b.G;
            result.B = (a.B < b.B) ? a.B : b.B;
            result.A = (a.A < b.A) ? a.A : b.A;
        }

        /// <summary>
        /// Adds two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Sum of the two colors</returns>
        public static Color Add(Color a, Color b)
        {
            Add(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Adds two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="result">Sum of the two colors</param>
        public static void Add(ref Color a, ref Color b, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte(a.R + b.R);
            result.G = (byte)MathHelper.ClampToByte(a.G + b.G);
            result.B = (byte)MathHelper.ClampToByte(a.B + b.B);
            result.A = (byte)MathHelper.ClampToByte(a.A + b.A);
        }

        /// <summary>
        /// Subtracts a color from another.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Difference of the two colors</returns>
        public static Color Subtract(Color a, Color b)
        {
            Subtract(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Subtracts a color from another.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="result">Difference of the two colors</param>
        public static void Subtract(ref Color a, ref Color b, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte(a.R - b.R);
            result.G = (byte)MathHelper.ClampToByte(a.G - b.G);
            result.B = (byte)MathHelper.ClampToByte(a.B - b.B);
            result.A = (byte)MathHelper.ClampToByte(a.A - b.A);
        }

        /// <summary>
        /// Modulates two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Modulated color</returns>
        public static Color Modulate(Color a, Color b)
        {
            Modulate(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Modulates two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="result">Modulated color</param>
        public static void Modulate(ref Color a, ref Color b, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte(a.R * b.R);
            result.G = (byte)MathHelper.ClampToByte(a.G * b.G);
            result.B = (byte)MathHelper.ClampToByte(a.B * b.B);
            result.A = (byte)MathHelper.ClampToByte(a.A * b.A);
        }

        /// <summary>
        /// Premultiplies the RGB component of a color by its alpha.
        /// </summary>
        /// <param name="color">Non-premultiplied color</param>
        /// <returns>Premultiplied color</returns>
        public static Color PremultiplyAlpha(Color color)
        {
            PremultiplyAlpha(ref color, out Color result);
            return result;
        }

        /// <summary>
        /// Premultiplies the RGB component of a color by its alpha.
        /// </summary>
        /// <param name="color">Color to premultiply</param>
        /// <param name="result">Premultiplied color</param>
        public static void PremultiplyAlpha(ref Color color, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte(color.R * color.A);
            result.G = (byte)MathHelper.ClampToByte(color.G * color.A);
            result.B = (byte)MathHelper.ClampToByte(color.B * color.A);
            result.A = color.A;
        }

        /// <summary>
        /// Scales a color by a scaling factor.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="scale">Amount to multiply</param>
        /// <returns>Scaled color</returns>
        public static Color Scale(Color value, float scale)
        {
            Scale(ref value, scale, out Color result);
            return result;
        }

        /// <summary>
        /// Scales a color by a scaling factor.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="scale">Amount to multiply</param>
        /// <param name="result">Scaled color</param>
        public static void Scale(ref Color value, float scale, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte((int)(value.R * scale));
            result.G = (byte)MathHelper.ClampToByte((int)(value.G * scale));
            result.B = (byte)MathHelper.ClampToByte((int)(value.B * scale));
            result.A = (byte)MathHelper.ClampToByte((int)(value.A * scale));
        }

        /// <summary>
        /// Negates the specified color by subtracting each of its components from 1.0f.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <returns>Negated color</returns>
        public static Color Negate(Color value)
        {
            Negate(ref value, out Color result);
            return result;
        }

        /// <summary>
        /// Negates the specified color by subtracting each of its components from 1.0f.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="result">Negated color</param>
        public static void Negate(ref Color value, out Color result)
        {
            result.R = (byte)(255 - value.R);
            result.G = (byte)(255 - value.G);
            result.B = (byte)(255 - value.B);
            result.A = (byte)(255 - value.A);
        }

        /// <summary>
        /// Linearly interpolates between two colors.
        /// </summary>
        /// <param name="a">Starting color</param>
        /// <param name="b">Ending color</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <returns>Interpolated color</returns>
        public static Color Lerp(Color a, Color b, float percent)
        {
            Lerp(ref a, ref b, percent, out Color result);
            return result;
        }

        /// <summary>
        /// Linearly interpolates between two colors.
        /// </summary>
        /// <param name="a">Starting color</param>
        /// <param name="b">Ending color</param>
        /// <param name="percent">Amount to interpolate by</param>
        /// <param name="result">Interpolated color</param>
        public static void Lerp(ref Color a, ref Color b, float percent, out Color result)
        {
            result.R = (byte)MathHelper.ClampToByte(a.R + (int)((b.R - a.R) * percent));
            result.G = (byte)MathHelper.ClampToByte(a.G + (int)((b.G - a.G) * percent));
            result.B = (byte)MathHelper.ClampToByte(a.B + (int)((b.B - a.B) * percent));
            result.A = (byte)MathHelper.ClampToByte(a.A + (int)((b.A - a.A) * percent));
        }

        /// <summary>
        /// Compute a cubic interpolation between two colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <returns>Cubic interpolated color</returns>
        public static Color SmoothStep(Color a, Color b, float wf)
        {
            SmoothStep(ref a, ref b, wf, out Color result);
            return result;
        }

        /// <summary>
        /// Compute a cubic interpolation between two colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <param name="wf">Weighting factor (between 0 and 1.0)</param>
        /// <param name="result">Cubic interpolated color</param>
        public static void SmoothStep(ref Color a, ref Color b, float wf, out Color result)
        {
            float amt = MathHelper.Clamp(wf, 0.0f, 1.0f);
            amt = (amt * amt) * (3.0f - (2.0f * amt));

            result.R = (byte)MathHelper.ClampToByte(a.R + (int)((b.R - a.R) * amt));
            result.G = (byte)MathHelper.ClampToByte(a.G + (int)((b.G - a.G) * amt));
            result.B = (byte)MathHelper.ClampToByte(a.B + (int)((b.B - a.B) * amt));
            result.A = (byte)MathHelper.ClampToByte(a.A + (int)((b.A - a.A) * amt));
        }

        /// <summary>
        /// Gets a color by its name
        /// </summary>
        /// <param name="name">Color name</param>
        /// <returns>Color that goes by the given name</returns>
        public static Color GetNamedColor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (NamedColors.TryGetValue(name.ToLower(), out Color color))
            {
                return color;
            }

            throw new SparkException($"Unknown named color '{name}'");
        }

        /// <summary>
        /// Parses a hexadecimal color
        /// </summary>
        /// <param name="hexValue">Hex color value</param>
        /// <returns>Color defined by the hex value</returns>
        public static Color ParseHexColor(string hexValue)
        {
            int a = 255;
            int r;
            int g;
            int b;

            Func<char, int> ParseHexChar = c =>
            {
                const int zeroChar = '0';
                const int aLower = 'a';
                const int aUpper = 'A';
                
                int intChar = c;
                if ((intChar >= zeroChar) && (intChar <= (zeroChar + 9)))
                {
                    return (intChar - zeroChar);
                }

                if ((intChar >= aLower) && (intChar <= (aLower + 5)))
                {
                    return (intChar - aLower + 10);
                }

                if ((intChar >= aUpper) && (intChar <= (aUpper + 5)))
                {
                    return (intChar - aUpper + 10);
                }

                throw new ArgumentException("Character is not a valid hexadecimal value", nameof(c));
            };

            if (hexValue.Length > 7)
            {
                a = ParseHexChar(hexValue[1]) * 16 + ParseHexChar(hexValue[2]);
                r = ParseHexChar(hexValue[3]) * 16 + ParseHexChar(hexValue[4]);
                g = ParseHexChar(hexValue[5]) * 16 + ParseHexChar(hexValue[6]);
                b = ParseHexChar(hexValue[7]) * 16 + ParseHexChar(hexValue[8]);
            }
            else if (hexValue.Length > 5)
            {
                r = ParseHexChar(hexValue[1]) * 16 + ParseHexChar(hexValue[2]);
                g = ParseHexChar(hexValue[3]) * 16 + ParseHexChar(hexValue[4]);
                b = ParseHexChar(hexValue[5]) * 16 + ParseHexChar(hexValue[6]);
            }
            else if (hexValue.Length > 4)
            {
                a = ParseHexChar(hexValue[1]);
                a = a + a * 16;
                r = ParseHexChar(hexValue[2]);
                r = r + r * 16;
                g = ParseHexChar(hexValue[3]);
                g = g + g * 16;
                b = ParseHexChar(hexValue[4]);
                b = b + b * 16;
            }
            else
            {
                r = ParseHexChar(hexValue[1]);
                r = r + r * 16;
                g = ParseHexChar(hexValue[2]);
                g = g + g * 16;
                b = ParseHexChar(hexValue[3]);
                b = b + b * 16;
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Tests equality between two colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>True if components are equal</returns>
        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two colors.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>True if components are not equal</returns>
        public static bool operator !=(Color a, Color b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Adds the two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Sum of the two colors</returns>
        public static Color operator +(Color a, Color b)
        {
            Add(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Negates the color (subtracts value from 1.0f) of each color component.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <returns>Difference of the two colors</returns>
        public static Color operator -(Color value)
        {
            Negate(ref value, out Color result);
            return result;
        }

        /// <summary>
        /// Subtracts a color from another.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Difference of the two colors</returns>
        public static Color operator -(Color a, Color b)
        {
            Subtract(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Modulates two colors together.
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns>Modulated color</returns>
        public static Color operator *(Color a, Color b)
        {
            Modulate(ref a, ref b, out Color result);
            return result;
        }

        /// <summary>
        /// Scales a color by a scaling factor.
        /// </summary>
        /// <param name="value">Source color</param>
        /// <param name="scale">Amount to multiply</param>
        /// <returns>Scaled color</returns>
        public static Color operator *(Color value, float scale)
        {
            Scale(ref value, scale, out Color result);
            return result;
        }

        /// <summary>
        /// Scales a color by a scaling factor.
        /// </summary>
        /// <param name="scale">Amount to multiply</param>
        /// <param name="value">Source color</param>
        /// <returns>Scaled color</returns>
        public static Color operator *(float scale, Color value)
        {
            Scale(ref value, scale, out Color result);
            return result;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color"/> to <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">Color value.</param>
        /// <returns>RGB as a vector</returns>
        public static explicit operator Vector3(Color value)
        {
            Vector3 result;
            result.X = value.R / 255.0f;
            result.Y = value.G / 255.0f;
            result.Z = value.B / 255.0f;

            return result;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color"/> to <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">Color value</param>
        /// <returns>RGBA as a vector</returns>
        public static explicit operator Vector4(Color value)
        {
            Vector4 result;
            result.X = value.R / 255.0f;
            result.Y = value.G / 255.0f;
            result.Z = value.B / 255.0f;
            result.W = value.A / 255.0f;

            return result;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">RGB as a vector</param>
        /// <returns>Converted color</returns>
        public static explicit operator Color(Vector3 value)
        {
            return new Color(value);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector4"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">RGBA as a vector</param>
        /// <returns>Converted color</returns>
        public static explicit operator Color(Vector4 value)
        {
            return new Color(value);
        }

        /// <summary>
        /// Tests equality between this color and another color.
        /// </summary>
        /// <param name="other">Color to test against</param>
        /// <returns>True if the colors are equal</returns>
        public bool Equals(Color other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between this color and another color.
        /// </summary>
        /// <param name="other">Color to test against</param>
        /// <returns>True if the colors are equal</returns>
        public bool Equals(ref Color other)
        {
            return (R == other.R) && 
                   (G == other.G) && 
                   (B == other.B) && 
                   (A == other.A);
        }

        /// <summary>
        /// Tests equality between this color and the supplied object.
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if object is a color and components are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                return Equals((Color)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns this Color as as 3-component float vector. RGB corresponds to XYZ.
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 ToVector3()
        {
            Vector3 result;
            result.X = R / 255.0f;
            result.Y = G / 255.0f;
            result.Z = B / 255.0f;

            return result;
        }

        /// <summary>
        /// Returns this Color as 4-component float vector. RGBA corresponds to XYZW.
        /// </summary>
        /// <returns>Color</returns>
        public Vector4 ToVector4()
        {
            Vector4 result;
            result.X = R / 255.0f;
            result.Y = G / 255.0f;
            result.Z = B / 255.0f;
            result.W = A / 255.0f;

            return result;
        }

        /// <summary>
        /// Negates this color by subtracting each of its components from 1.0f.
        /// </summary>
        public void Negate()
        {
            R = (byte)(255 - R);
            G = (byte)(255 - G);
            B = (byte)(255 - B);
            A = (byte)(255 - A);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return R.GetHashCode() + G.GetHashCode() + B.GetHashCode() + A.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString("G", formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider, "R: {0} G: {1} B: {2} A: {3}", new object[] { R.ToString(format, formatProvider), G.ToString(format, formatProvider), B.ToString(format, formatProvider), A.ToString(format, formatProvider) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("R", R);
            output.Write("G", G);
            output.Write("B", B);
            output.Write("A", A);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            R = input.ReadByte();
            G = input.ReadByte();
            B = input.ReadByte();
            A = input.ReadByte();
        }

        /// <summary>
        /// Create a packed unsigned integer from a set of components
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        /// <returns>Packed unsigned integer representing the color</returns>
        private static uint PackColor(float r, float g, float b, float a)
        {
            uint n = (uint)MathHelper.ClampAndRound(r * 255.0f, 0.0f, 255.0f);
            uint n1 = ((uint)MathHelper.ClampAndRound(g * 255.0f, 0.0f, 255.0f)) << 8;
            uint n2 = ((uint)MathHelper.ClampAndRound(b * 255.0f, 0.0f, 255.0f)) << 16;
            uint n3 = ((uint)MathHelper.ClampAndRound(a * 255.0f, 0.0f, 255.0f)) << 24;

            return n | n1 | n2 | n3;
        }

        /// <summary>
        /// Unpacks the components from a packed color value
        /// </summary>
        /// <param name="packedValue">Value to be unpacked</param>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        private static void UnpackColor(uint packedValue, out byte r, out byte g, out byte b, out byte a)
        {
            r = (byte)packedValue;
            g = (byte)(packedValue >> 8);
            b = (byte)(packedValue >> 16);
            a = (byte)(packedValue >> 24);
        }

        #region Predefined Colors

        /// <summary>
        /// Gets a color with the value R:0 G: 0 B: 0 A: 0.
        /// </summary>
        public static Color TransparentBlack => new Color(0, 0, 0, 0);

        /// <summary>
        /// Gets a color with the value R:240 G:248 B:255 A:255.
        /// </summary>
        public static Color AliceBlue => new Color(240, 248, 255, 255);

        /// <summary>
        /// Gets a color with the value R:250 G:235 B:215 A:255.
        /// </summary>
        public static Color AntiqueWhite => new Color(250, 235, 215, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:255 B:255 A:255.
        /// </summary>
        public static Color Aqua => new Color(0, 255, 255, 255);

        /// <summary>
        /// Gets a color with the value R:127 G:255 B:212 A:255.
        /// </summary>
        public static Color Aquamarine => new Color(127, 255, 212, 255);

        /// <summary>
        /// Gets a color with the value R:240 G:255 B:255 A:255.
        /// </summary>
        public static Color Azure => new Color(240, 255, 255, 255);

        /// <summary>
        /// Gets a color with the value R:245 G:245 B:220 A:255.
        /// </summary>
        public static Color Beige => new Color(245, 245, 220, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:228 B:196 A:255.
        /// </summary>
        public static Color Bisque => new Color(255, 228, 196, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:0 B:0 A:255.
        /// </summary>
        public static Color Black => new Color(0, 0, 0, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:235 B:205 A:255.
        /// </summary>
        public static Color BlanchedAlmond => new Color(255, 235, 205, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:0 B:255 A:255.
        /// </summary>
        public static Color Blue => new Color(0, 0, 255, 255);

        /// <summary>
        /// Gets a color with the value R:138 G:43 B:226 A:255.
        /// </summary>
        public static Color BlueViolet => new Color(138, 43, 226, 255);

        /// <summary>
        /// Gets a color with the value R:165 G:42 B:42 A:255.
        /// </summary>
        public static Color Brown => new Color(165, 42, 42, 255);

        /// <summary>
        /// Gets a color with the value R:222 G:184 B:135 A:255.
        /// </summary>
        public static Color BurlyWood => new Color(222, 184, 135, 255);

        /// <summary>
        /// Gets a color with the value R:95 G:158 B:160 A:255.
        /// </summary>
        public static Color CadetBlue => new Color(95, 158, 160, 255);

        /// <summary>
        /// Gets a color with the value R:127 G:255 B:0 A:255.
        /// </summary>
        public static Color Chartreuse => new Color(127, 255, 0, 255);

        /// <summary>
        /// Gets a color with the value R:210 G:105 B:30 A:255.
        /// </summary>
        public static Color Chocolate => new Color(210, 105, 30, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:127 B:80 A:255.
        /// </summary>
        public static Color Coral => new Color(255, 127, 80, 255);

        /// <summary>
        /// Gets a color with the value R:100 G:149 B:237 A:255.
        /// </summary>
        public static Color CornflowerBlue => new Color(100, 149, 237, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:248 B:220 A:255.
        /// </summary>
        public static Color Cornsilk => new Color(255, 248, 220, 255);

        /// <summary>
        /// Gets a color with the value R:220 G:20 B:60 A:255.
        /// </summary>
        public static Color Crimson => new Color(220, 20, 60, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:255 B:255 A:255.
        /// </summary>
        public static Color Cyan => new Color(0, 255, 255, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:0 B:139 A:255.
        /// </summary>
        public static Color DarkBlue => new Color(0, 0, 139, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:139 B:139 A:255.
        /// </summary>
        public static Color DarkCyan => new Color(0, 139, 139, 255);

        /// <summary>
        /// Gets a color with the value R:184 G:134 B:11 A:255.
        /// </summary>
        public static Color DarkGoldenrod => new Color(184, 134, 11, 255);

        /// <summary>
        /// Gets a color with the value R:169 G:169 B:169 A:255.
        /// </summary>
        public static Color DarkGray => new Color(169, 169, 169, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:100 B:0 A:255.
        /// </summary>
        public static Color DarkGreen => new Color(40, 100, 0, 255);

        /// <summary>
        /// Gets a color with the value R:189 G:183 B:107 A:255.
        /// </summary>
        public static Color DarkKhaki => new Color(189, 183, 107, 255);

        /// <summary>
        /// Gets a color with the value R:139 G:0 B:139 A:255.
        /// </summary>
        public static Color DarkMagenta => new Color(139, 0, 139, 255);

        /// <summary>
        /// Gets a color with the value R:85 G:107 B:47 A:255.
        /// </summary>
        public static Color DarkOliveGreen => new Color(85, 107, 47, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:140 B:0 A:255.
        /// </summary>
        public static Color DarkOrange => new Color(255, 140, 0, 255);

        /// <summary>
        /// Gets a color with the value R:153 G:50 B:204 A:255.
        /// </summary>
        public static Color DarkOrchid => new Color(153, 50, 204, 255);

        /// <summary>
        /// Gets a color with the value R:139 G:0 B:0 A:255.
        /// </summary>
        public static Color DarkRed => new Color(139, 0, 0, 255);

        /// <summary>
        /// Gets a color with the value R:233 G:150 B:122 A:255.
        /// </summary>
        public static Color DarkSalmon => new Color(233, 150, 122, 255);

        /// <summary>
        /// Gets a color with the value R:143 G:188 B:139 A:255.
        /// </summary>
        public static Color DarkSeaGreen => new Color(143, 188, 139, 255);

        /// <summary>
        /// Gets a color with the value R:72 G:61 B:139 A:255.
        /// </summary>
        public static Color DarkSlateBlue => new Color(72, 61, 139, 255);

        /// <summary>
        /// Gets a color with the value R:47 G:79 B:79 A:255.
        /// </summary>
        public static Color DarkSlateGray => new Color(47, 79, 79, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:206 B:209 A:255.
        /// </summary>
        public static Color DarkTurquoise => new Color(0, 206, 209, 255);

        /// <summary>
        /// Gets a color with the value R:148 G:0 B:211 A:255.
        /// </summary>
        public static Color DarkViolet => new Color(148, 0, 211, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:20 B:147 A:255.
        /// </summary>
        public static Color DeepPink => new Color(255, 20, 147, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:191 B:255 A:255.
        /// </summary>
        public static Color DeepSkyBlue => new Color(0, 191, 255, 255);

        /// <summary>
        /// Gets a color with the value R:105 G:105 B:105 A:255.
        /// </summary>
        public static Color DimGray => new Color(105, 105, 105, 255);

        /// <summary>
        /// Gets a color with the value R:30 G:144 B:255 A:255.
        /// </summary>
        public static Color DodgerBlue => new Color(30, 144, 255, 255);

        /// <summary>
        /// Gets a color with the value R:178 G:34 B:34 A:255.
        /// </summary>
        public static Color Firebrick => new Color(178, 34, 34, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:250 B:240 A:255.
        /// </summary>
        public static Color FloralWhite => new Color(255, 250, 240, 255);

        /// <summary>
        /// Gets a color with the value R:34 G:139 B:34 A:255.
        /// </summary>
        public static Color ForestGreen => new Color(34, 139, 34, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:0 B:255 A:255.
        /// </summary>
        public static Color Fuchsia => new Color(255, 0, 255, 255);

        /// <summary>
        /// Gets a color with the value R:220 G:220 B:220 A:255.
        /// </summary>
        public static Color Gainsboro => new Color(220, 220, 220, 255);

        /// <summary>
        /// Gets a color with the value R:248 G:248 B:255 A:255.
        /// </summary>
        public static Color GhostWhite => new Color(248, 248, 255, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:215 B:0 A:255.
        /// </summary>
        public static Color Gold => new Color(255, 215, 0, 255);

        /// <summary>
        /// Gets a color with the value R:218 G:165 B:32 A:255.
        /// </summary>
        public static Color Goldenrod => new Color(218, 165, 32, 255);

        /// <summary>
        /// Gets a color with the value R:128 G:128 B:128 A:255.
        /// </summary>
        public static Color Gray => new Color(128, 128, 128, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:128 B:0 A:255.
        /// </summary>
        public static Color Green => new Color(0, 128, 0, 255);

        /// <summary>
        /// Gets a color with the value R:173 G:255 B:47 A:255.
        /// </summary>
        public static Color GreenYellow => new Color(173, 255, 47, 255);

        /// <summary>
        /// Gets a color with the value R:240 G:255 B:240 A:255.
        /// </summary>
        public static Color Honeydew => new Color(240, 255, 240, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:105 B:180 A:255.
        /// </summary>
        public static Color HotPink => new Color(255, 105, 180, 255);

        /// <summary>
        /// Gets a color with the value R:205 G:92 B:92 A:255.
        /// </summary>
        public static Color IndianRed => new Color(205, 92, 92, 255);

        /// <summary>
        /// Gets a color with the value R:75 G:0 B:130 A:255.
        /// </summary>
        public static Color Indigo => new Color(75, 0, 130, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:255 B:240 A:255.
        /// </summary>
        public static Color Ivory => new Color(255, 255, 240, 255);

        /// <summary>
        /// Gets a color with the value R:240 G:230 B:140 A:255.
        /// </summary>
        public static Color Khaki => new Color(240, 230, 140, 255);

        /// <summary>
        /// Gets a color with the value R:230 G:230 B:250 A:255.
        /// </summary>
        public static Color Lavender => new Color(230, 230, 250, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:240 B:245 A:255.
        /// </summary>
        public static Color LavenderBlush => new Color(255, 240, 245, 255);

        /// <summary>
        /// Gets a color with the value R:124 G:252 B:0 A:255.
        /// </summary>
        public static Color LawnGreen => new Color(124, 252, 0, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:250 B:205 A:255.
        /// </summary>
        public static Color LemonChiffon => new Color(255, 250, 205, 255);

        /// <summary>
        /// Gets a color with the value R:173 G:216 B:230 A:255.
        /// </summary>
        public static Color LightBlue => new Color(173, 216, 230, 255);

        /// <summary>
        /// Gets a color with the value R:240 G:128 B:128 A:255.
        /// </summary>
        public static Color LightCoral => new Color(240, 128, 128, 255);

        /// <summary>
        /// Gets a color with the value R:224 G:255 B:255 A:255.
        /// </summary>
        public static Color LightCyan => new Color(224, 255, 255, 255);

        /// <summary>
        /// Gets a color with the value R:250 G:250 B:210 A:255.
        /// </summary>
        public static Color LightGoldenrodYellow => new Color(250, 250, 210, 255);

        /// <summary>
        /// Gets a color with the value R:144 G:238 B:144 A:255.
        /// </summary>
        public static Color LightGreen => new Color(144, 238, 144, 255);

        /// <summary>
        /// Gets a color with the value R:211 G:211 B:211 A:255.
        /// </summary>
        public static Color LightGray => new Color(211, 211, 211, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:182 B:193 A:255.
        /// </summary>
        public static Color LightPink => new Color(255, 182, 193, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:160 B:122 A:255.
        /// </summary>
        public static Color LightSalmon => new Color(255, 160, 122, 255);

        /// <summary>
        /// Gets a color with the value R:32 G:178 B:170 A:255.
        /// </summary>
        public static Color LightSeaGreen => new Color(32, 178, 170, 255);

        /// <summary>
        /// Gets a color with the value R:135 G:206 B:250 A:255.
        /// </summary>
        public static Color LightSkyBlue => new Color(135, 206, 250, 255);

        /// <summary>
        /// Gets a color with the value R:119 G:136 B:153 A:255.
        /// </summary>
        public static Color LightSlateGray => new Color(119, 136, 153, 255);

        /// <summary>
        /// Gets a color with the value R:176 G:196 B:222 A:255.
        /// </summary>
        public static Color LightSteelBlue => new Color(176, 196, 222, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:255 B:224 A:255.
        /// </summary>
        public static Color LightYellow => new Color(255, 255, 224, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:255 B:0 A:255.
        /// </summary>
        public static Color Lime => new Color(0, 255, 0, 255);

        /// <summary>
        /// Gets a color with the value R:50 G:205 B:50 A:255.
        /// </summary>
        public static Color LimeGreen => new Color(50, 205, 50, 255);

        /// <summary>
        /// Gets a color with the value R:250 G:240 B:230 A:255.
        /// </summary>
        public static Color Linen => new Color(250, 240, 230, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:0 B:255 A:255.
        /// </summary>
        public static Color Magenta => new Color(255, 0, 255, 255);

        /// <summary>
        /// Gets a color with the value R:128 G:0 B:0 A:255.
        /// </summary>
        public static Color Maroon => new Color(128, 0, 0, 255);

        /// <summary>
        /// Gets a color with the value R:102 G:205 B:170 A:255.
        /// </summary>
        public static Color MediumAquamarine => new Color(102, 205, 170, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:0 B:205 A:255.
        /// </summary>
        public static Color MediumBlue => new Color(0, 0, 205, 255);

        /// <summary>
        /// Gets a color with the value R:186 G:85 B:211 A:255.
        /// </summary>
        public static Color MediumOrchid => new Color(186, 85, 211, 255);

        /// <summary>
        /// Gets a color with the value R:147 G:112 B:219 A:255.
        /// </summary>
        public static Color MediumPurple => new Color(147, 112, 219, 255);

        /// <summary>
        /// Gets a color with the value R:60 G:179 B:113 A:255.
        /// </summary>
        public static Color MediumSeaGreen => new Color(60, 179, 113, 255);

        /// <summary>
        /// Gets a color with the value R:123 G:104 B:238 A:255.
        /// </summary>
        public static Color MediumSlateBlue => new Color(123, 104, 238, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:250 B:154 A:255.
        /// </summary>
        public static Color MediumSpringGreen => new Color(0, 250, 154, 255);

        /// <summary>
        /// Gets a color with the value R:72 G:209 B:204 A:255.
        /// </summary>
        public static Color MediumTurquoise => new Color(72, 209, 204, 255);

        /// <summary>
        /// Gets a color with the value R:199 G:21 B:133 A:255.
        /// </summary>
        public static Color MediumVioletRed => new Color(199, 21, 133, 255);

        /// <summary>
        /// Gets a color with the value R:25 G:25 B:112 A:255.
        /// </summary>
        public static Color MidnightBlue => new Color(25, 25, 112, 255);

        /// <summary>
        /// Gets a color with the value R:245 G:255 B:250 A:255.
        /// </summary>
        public static Color MintCream => new Color(245, 255, 250, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:228 B:225 A:255.
        /// </summary>
        public static Color MistyRose => new Color(255, 228, 225, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:228 B:181 A:255.
        /// </summary>
        public static Color Moccasin => new Color(255, 228, 181, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:222 B:173 A:255.
        /// </summary>
        public static Color NavajoWhite => new Color(255, 222, 173, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:0 B:128 A:255.
        /// </summary>
        public static Color Navy => new Color(0, 0, 128, 255);

        /// <summary>
        /// Gets a color with the value R:253 G:245 B:230 A:255.
        /// </summary>
        public static Color OldLace => new Color(253, 245, 230, 255);

        /// <summary>
        /// Gets a color with the value R:128 G:128 B:0 A:255.
        /// </summary>
        public static Color Olive => new Color(128, 128, 0, 255);

        /// <summary>
        /// Gets a color with the value R:107 G:142 B:35 A:255.
        /// </summary>
        public static Color OliveDrab => new Color(107, 142, 35, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:165 B:0 A:255.
        /// </summary>
        public static Color Orange => new Color(255, 165, 0, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:69 B:0 A:255.
        /// </summary>
        public static Color OrangeRed => new Color(255, 69, 0, 255);

        /// <summary>
        /// Gets a color with the value R:218 G:112 B:214 A:255.
        /// </summary>
        public static Color Orchid => new Color(218, 112, 214, 255);

        /// <summary>
        /// Gets a color with the value R:238 G:232 B:170 A:255.
        /// </summary>
        public static Color PaleGoldenrod => new Color(238, 232, 170, 255);

        /// <summary>
        /// Gets a color with the value R:152 G:251 B:152 A:255.
        /// </summary>
        public static Color PaleGreen => new Color(152, 251, 152, 255);

        /// <summary>
        /// Gets a color with the value R:175 G:238 B:238 A:255.
        /// </summary>
        public static Color PaleTurquoise => new Color(175, 238, 238, 255);

        /// <summary>
        /// Gets a color with the value R:219 G:112 B:147 A:255.
        /// </summary>
        public static Color PaleVioletRed => new Color(219, 112, 147, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:239 B:213 A:255.
        /// </summary>
        public static Color PapayaWhip => new Color(255, 239, 213, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:218 B:185 A:255.
        /// </summary>
        public static Color PeachPuff => new Color(255, 218, 185, 255);

        /// <summary>
        /// Gets a color with the value R:205 G:133 B:63 A:255.
        /// </summary>
        public static Color Peru => new Color(205, 133, 63, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:192 B:203 A:255.
        /// </summary>
        public static Color Pink => new Color(255, 192, 203, 255);

        /// <summary>
        /// Gets a color with the value R:221 G:160 B:221 A:255.
        /// </summary>
        public static Color Plum => new Color(221, 160, 221, 255);

        /// <summary>
        /// Gets a color with the value R:176 G:224 B:230 A:255.
        /// </summary>
        public static Color PowderBlue => new Color(176, 224, 230, 255);

        /// <summary>
        /// Gets a color with the value R:128 G:0 B:128 A:255.
        /// </summary>
        public static Color Purple => new Color(128, 0, 128, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:0 B:0 A:255.
        /// </summary>
        public static Color Red => new Color(255, 0, 0, 255);

        /// <summary>
        /// Gets a color with the value R:188 G:143 B:143 A:255.
        /// </summary>
        public static Color RosyBrown => new Color(188, 143, 143, 255);

        /// <summary>
        /// Gets a color with the value R:65 G:105 B:225 A:255.
        /// </summary>
        public static Color RoyalBlue => new Color(65, 105, 225, 255);

        /// <summary>
        /// Gets a color with the value R:139 G:69 B:19 A:255.
        /// </summary>
        public static Color SaddleBrown => new Color(139, 69, 19, 255);

        /// <summary>
        /// Gets a color with the value R:250 G:128 B:114 A:255.
        /// </summary>
        public static Color Salmon => new Color(250, 128, 114, 255);

        /// <summary>
        /// Gets a color with the value R:244 G:164 B:96 A:255.
        /// </summary>
        public static Color SandyBrown => new Color(244, 164, 96, 255);

        /// <summary>
        /// Gets a color with the value R:46 G:139 B:87 A:255.
        /// </summary>
        public static Color SeaGreen => new Color(46, 139, 87, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:245 B:238 A:255.
        /// </summary>
        public static Color SeaShell => new Color(255, 245, 238, 255);

        /// <summary>
        /// Gets a color with the value R:160 G:82 B:45 A:255.
        /// </summary>
        public static Color Sienna => new Color(160, 82, 45, 255);

        /// <summary>
        /// Gets a color with the value R:192 G:192 B:192 A:255.
        /// </summary>
        public static Color Silver => new Color(192, 192, 192, 255);

        /// <summary>
        /// Gets a color with the value R:135 G:206 B:235 A:255.
        /// </summary>
        public static Color SkyBlue => new Color(135, 206, 235, 255);

        /// <summary>
        /// Gets a color with the value R:106 G:90 B:205 A:255.
        /// </summary>
        public static Color SlateBlue => new Color(106, 90, 205, 255);

        /// <summary>
        /// Gets a color with the value R:112 G:128 B:144 A:255.
        /// </summary>
        public static Color SlateGray => new Color(112, 128, 144, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:250 B:250 A:255.
        /// </summary>
        public static Color Snow => new Color(255, 250, 250, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:255 B:127 A:255.
        /// </summary>
        public static Color SpringGreen => new Color(0, 255, 127, 255);

        /// <summary>
        /// Gets a color with the value R:70 G:130 B:180 A:255.
        /// </summary>
        public static Color SteelBlue => new Color(70, 130, 180, 255);

        /// <summary>
        /// Gets a color with the value R:210 G:180 B:140 A:255.
        /// </summary>
        public static Color Tan => new Color(210, 180, 140, 255);

        /// <summary>
        /// Gets a color with the value R:0 G:128 B:128 A:255.
        /// </summary>
        public static Color Teal => new Color(0, 128, 128, 255);

        /// <summary>
        /// Gets a color with the value R:216 G:191 B:216 A:255.
        /// </summary>
        public static Color Thistle => new Color(216, 191, 216, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:99 B:71 A:255.
        /// </summary>
        public static Color Tomato => new Color(255, 99, 71, 255);

        /// <summary>
        /// Gets a color with the value R:64 G:224 B:208 A:255.
        /// </summary>
        public static Color Turquoise => new Color(64, 224, 208, 255);

        /// <summary>
        /// Gets a color with the value R:238 G:130 B:238 A:255.
        /// </summary>
        public static Color Violet => new Color(238, 130, 238, 255);

        /// <summary>
        /// Gets a color with the value R:245 G:222 B:179 A:255.
        /// </summary>
        public static Color Wheat => new Color(245, 222, 179, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:255 B:255 A:255.
        /// </summary>
        public static Color White => new Color(255, 255, 255, 255);

        /// <summary>
        /// Gets a color with the value R:245 G:245 B:245 A:255.
        /// </summary>
        public static Color WhiteSmoke => new Color(245, 245, 245, 255);

        /// <summary>
        /// Gets a color with the value R:255 G:255 B:0 A:255.
        /// </summary>
        public static Color Yellow => new Color(255, 255, 0, 255);

        /// <summary>
        /// Gets a color with the value R:154 G:205 B:50 A:255.
        /// </summary>
        public static Color YellowGreen => new Color(154, 205, 50, 255);

        #endregion
    }
}
