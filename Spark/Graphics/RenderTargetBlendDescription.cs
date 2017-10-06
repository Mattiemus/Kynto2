namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    
    using Content;

    /// <summary>
    /// Describes a blending states for a render target. This is used to group common data together for BlendState only and changing values here
    /// will not affect platform-device states.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTargetBlendDescription : IEquatable<RenderTargetBlendDescription>, IPrimitiveValue
    {
        /// <summary>
        /// If blending is enabled for the render target. The default is false.
        /// </summary>
        public bool BlendEnable;

        /// <summary>
        /// The alpha blend function used. The default is Add.
        /// </summary>
        public BlendFunction AlphaBlendFunction;

        /// <summary>
        /// The alpha source blend used. The default is One.
        /// </summary>
        public Blend AlphaSourceBlend;

        /// <summary>
        /// The alpha destination blend used. The default is Zero.
        /// </summary>
        public Blend AlphaDestinationBlend;

        /// <summary>
        /// The color blend function used. The default is Add.
        /// </summary>
        public BlendFunction ColorBlendFunction;

        /// <summary>
        /// The color source blend used. The default is One.
        /// </summary>
        public Blend ColorSourceBlend;

        /// <summary>
        /// The color destination blend used. The default is Zero.
        /// </summary>
        public Blend ColorDestinationBlend;

        /// <summary>
        /// The render target color channels that should be written to during blending.
        /// </summary>
        public ColorWriteChannels WriteChannels;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetBlendDescription"/> struct.
        /// </summary>
        /// <param name="blendEnable">True if blending is enabled for the render target, false otherwise.</param>
        /// <param name="alphaBlendFunction">The alpha blend function used.</param>
        /// <param name="alphaSourceBlend">The alpha source blend used.</param>
        /// <param name="alphaDestinationBlend">The alpha destination blend used.</param>
        /// <param name="colorBlendFunction">The color blend function used.</param>
        /// <param name="colorSourceBlend">The color source blend used.</param>
        /// <param name="colorDestinationBlend">The color destination blend used.</param>
        /// <param name="writeChannels">The render target color channels that should be written to during blending.</param>
        public RenderTargetBlendDescription(bool blendEnable, BlendFunction alphaBlendFunction, Blend alphaSourceBlend, Blend alphaDestinationBlend, 
            BlendFunction colorBlendFunction, Blend colorSourceBlend, Blend colorDestinationBlend, ColorWriteChannels writeChannels)
        {
            BlendEnable = blendEnable;
            AlphaBlendFunction = alphaBlendFunction;
            AlphaSourceBlend = alphaSourceBlend;
            AlphaDestinationBlend = alphaDestinationBlend;
            ColorBlendFunction = colorBlendFunction;
            ColorSourceBlend = colorSourceBlend;
            ColorDestinationBlend = colorDestinationBlend;
            WriteChannels = writeChannels;
        }

        /// <summary>
        /// Gets a default render target blend description.
        /// </summary>
        public static RenderTargetBlendDescription Default => new RenderTargetBlendDescription(false, 
                                                                                               BlendFunction.Add, Blend.One, Blend.Zero, 
                                                                                               BlendFunction.Add, Blend.One, Blend.Zero, 
                                                                                               ColorWriteChannels.All);

        /// <summary>
        /// Gets if the this description matches the default description settings.
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return !BlendEnable && 
                       AlphaBlendFunction == BlendFunction.Add && 
                       AlphaSourceBlend == Blend.One && 
                       AlphaDestinationBlend == Blend.Zero && 
                       ColorBlendFunction == BlendFunction.Add && 
                       ColorSourceBlend == Blend.One && 
                       ColorDestinationBlend == Blend.Zero && 
                       WriteChannels == ColorWriteChannels.All;
            }
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("BlendEnable", BlendEnable);
            output.WriteEnum("AlphaBlendFunction", AlphaBlendFunction);
            output.WriteEnum("AlphaSourceBlend", AlphaSourceBlend);
            output.WriteEnum("AlphaDestinationBlend", AlphaDestinationBlend);

            output.WriteEnum("ColorBlendFunction", ColorBlendFunction);
            output.WriteEnum("ColorSourceBlend", ColorSourceBlend);
            output.WriteEnum("ColorDestinationBlend", ColorDestinationBlend);

            output.WriteEnum("ColorWriteChannels", WriteChannels);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            BlendEnable = input.ReadBoolean();
            AlphaBlendFunction = input.ReadEnum<BlendFunction>();
            AlphaSourceBlend = input.ReadEnum<Blend>();
            AlphaDestinationBlend = input.ReadEnum<Blend>();

            ColorBlendFunction = input.ReadEnum<BlendFunction>();
            ColorSourceBlend = input.ReadEnum<Blend>();
            ColorDestinationBlend = input.ReadEnum<Blend>();

            WriteChannels = input.ReadEnum<ColorWriteChannels>();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 31) + ((BlendEnable) ? 1 : 0);

                hash = (hash * 31) + AlphaBlendFunction.GetHashCode();
                hash = (hash * 31) + AlphaSourceBlend.GetHashCode();
                hash = (hash * 31) + AlphaDestinationBlend.GetHashCode();

                hash = (hash * 31) + ColorBlendFunction.GetHashCode();
                hash = (hash * 31) + ColorSourceBlend.GetHashCode();
                hash = (hash * 31) + ColorDestinationBlend.GetHashCode();

                hash = (hash * 31) + WriteChannels.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is RenderTargetBlendDescription)
            {
                return Equals((RenderTargetBlendDescription)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(RenderTargetBlendDescription other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ref RenderTargetBlendDescription other)
        {
            return BlendEnable == other.BlendEnable &&
                   AlphaBlendFunction == other.AlphaBlendFunction &&
                   AlphaSourceBlend == other.AlphaSourceBlend &&
                   AlphaDestinationBlend == other.AlphaDestinationBlend &&
                   ColorBlendFunction == other.ColorBlendFunction &&
                   ColorSourceBlend == other.ColorSourceBlend &&
                   ColorDestinationBlend == other.ColorDestinationBlend &&
                   WriteChannels == other.WriteChannels;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "BlendEnable: {0}, AlphaBlendFunction: {1}, AlphaSourceBlend: {2}, AlphaDestinationBlend: {3}, BlendFunction: {4}, ColorSourceBlend: {5}, ColorDestinationBlend: {6}, ColorWriteChannels: {7}", new object[] { BlendEnable.ToString(), AlphaBlendFunction.ToString(), AlphaSourceBlend.ToString(), AlphaDestinationBlend.ToString(), ColorBlendFunction.ToString(), ColorSourceBlend.ToString(), ColorDestinationBlend.ToString(), WriteChannels.ToString() });
        }
    }
}
