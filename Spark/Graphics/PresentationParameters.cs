namespace Spark.Graphics
{
    using System;
    using System.Globalization;

    using Content;
    using Math;

    /// <summary>
    /// Represents a collection of values used for setting up a graphics presentation, e.g. window and swap chain.
    /// </summary>
    public struct PresentationParameters : IEquatable<PresentationParameters>, IPrimitiveValue
    {
        /// <summary>
        /// Back buffer format.
        /// </summary>
        public SurfaceFormat BackBufferFormat;

        /// <summary>
        /// Depth-Stencil buffer format.
        /// </summary>
        public DepthFormat DepthStencilFormat;

        /// <summary>
        /// Width of the back buffer.
        /// </summary>
        public int BackBufferWidth;

        /// <summary>
        /// Height of the back buffer.
        /// </summary>
        public int BackBufferHeight;

        /// <summary>
        /// Number of samples to use in multi-sampling. Default is zero.
        /// </summary>
        public int MultiSampleCount;

        /// <summary>
        /// Number of qualtiy levels to use in multi-sampling. Default is zero.
        /// </summary>
        public int MultiSampleQuality;

        /// <summary>
        /// Is the presentation in full screen mode.
        /// </summary>
        public bool IsFullScreen;

        /// <summary>
        /// VSync enumeration.
        /// </summary>
        public PresentInterval PresentInterval;

        /// <summary>
        /// Backbuffer's render target usage.
        /// </summary>
        public RenderTargetUsage RenderTargetUsage;

        /// <summary>
        /// Display orientation of the presentation.
        /// </summary>
        public DisplayOrientation DisplayOrientation;

        /// <summary>
        /// Gets the bounds of the back buffer.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, BackBufferWidth, BackBufferHeight);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PresentationParameters"/>.
        /// </summary>
        /// <param name="format">The back buffer format..</param>
        /// <param name="depthFormat">The depth-stencil buffer format.</param>
        /// <param name="width">The back buffer width.</param>
        /// <param name="height">The back buffer height.</param>
        public PresentationParameters(SurfaceFormat format, DepthFormat depthFormat, int width, int height)
        {
            BackBufferFormat = format;
            DepthStencilFormat = depthFormat;
            BackBufferWidth = width;
            BackBufferHeight = height;
            MultiSampleCount = 0;
            MultiSampleQuality = 0;
            IsFullScreen = false;
            PresentInterval = PresentInterval.One;
            RenderTargetUsage = RenderTargetUsage.PlatformDefault;
            DisplayOrientation = DisplayOrientation.Default;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PresentationParameters"/>.
        /// </summary>
        /// <param name="format">The back buffer format..</param>
        /// <param name="depthFormat">The depth-stencil buffer format.</param>
        /// <param name="width">The back buffer width.</param>
        /// <param name="height">The back buffer height.</param>
        /// <param name="isFullScreen">If the presentation should be full screen.</param>
        public PresentationParameters(SurfaceFormat format, DepthFormat depthFormat, int width, int height, bool isFullScreen)
        {
            BackBufferFormat = format;
            DepthStencilFormat = depthFormat;
            BackBufferWidth = width;
            BackBufferHeight = height;
            MultiSampleCount = 0;
            MultiSampleQuality = 0;
            IsFullScreen = isFullScreen;
            PresentInterval = PresentInterval.One;
            RenderTargetUsage = RenderTargetUsage.PlatformDefault;
            DisplayOrientation = DisplayOrientation.Default;
        }

        /// <summary>
        /// Checks the preferred presentation parameter set. If any non-format value combinations are not supported by the graphics adapter, values that are approximate
        /// and valid are set. If the backbuffer surface format/depth format are not valid, this will return false.
        /// </summary>
        /// <param name="adapter">The graphics adapter</param>
        /// <param name="preferredParameters">Presentation parameters to check and potentially modify.</param>
        /// <returns>True if the presentation parameters are valid, false otherwise.</returns>
        public static bool CheckPresentationParameters(IGraphicsAdapter adapter, ref PresentationParameters preferredParameters)
        {
            if (adapter == null)
            {
                return false;
            }

            int maxQualityLevels = adapter.CheckMultisampleQualityLevels(preferredParameters.BackBufferFormat, preferredParameters.MultiSampleCount);
            if (maxQualityLevels == 0)
            {
                int count = Math.Max(1, preferredParameters.MultiSampleCount);
                int quality = preferredParameters.MultiSampleQuality;

                preferredParameters.MultiSampleCount = 1;
                preferredParameters.MultiSampleQuality = 0;

                while (count > 1)
                {
                    count--;
                    maxQualityLevels = adapter.CheckMultisampleQualityLevels(preferredParameters.BackBufferFormat, count);

                    // If found a valid quality level for count, set it and clamp our preferred quality level
                    if (quality > 0)
                    {
                        preferredParameters.MultiSampleCount = count;
                        preferredParameters.MultiSampleQuality = MathHelper.Clamp(quality, 0, maxQualityLevels - 1);
                        break;
                    }
                }

            }
            else
            {
                // MSAA count is ok, just ensure we clamp quality level
                preferredParameters.MultiSampleQuality = MathHelper.Clamp(preferredParameters.MultiSampleQuality, 0, maxQualityLevels - 1);
            }

            // Now clamp backbuffer size to be valid, and do a final check if all the formats and MSAA count is ok
            int maxSize = adapter.MaximumTexture2DSize;

            preferredParameters.BackBufferWidth = MathHelper.Clamp(preferredParameters.BackBufferWidth, 0, maxSize);
            preferredParameters.BackBufferHeight = MathHelper.Clamp(preferredParameters.BackBufferHeight, 0, maxSize);

            return adapter.CheckBackBufferFormat(preferredParameters.BackBufferFormat, preferredParameters.DepthStencilFormat, preferredParameters.MultiSampleCount);
        }

        /// <summary>
        /// Tests equality between two sets of presentation parameters.
        /// </summary>
        /// <param name="a">First set of presentation parameters</param>
        /// <param name="b">Second set of presentation parameters</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(PresentationParameters a, PresentationParameters b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two sets of presentation parameters.
        /// </summary>
        /// <param name="a">First set of presentation parameters</param>
        /// <param name="b">Second set of presentation parameters</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(PresentationParameters a, PresentationParameters b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            BackBufferFormat = input.ReadEnum<SurfaceFormat>();
            DepthStencilFormat = input.ReadEnum<DepthFormat>();
            BackBufferWidth = input.ReadInt32();
            BackBufferHeight = input.ReadInt32();
            MultiSampleCount = input.ReadInt32();
            MultiSampleQuality = input.ReadInt32();
            IsFullScreen = input.ReadBoolean();
            PresentInterval = input.ReadEnum<PresentInterval>();
            RenderTargetUsage = input.ReadEnum<RenderTargetUsage>();
            DisplayOrientation = input.ReadEnum<DisplayOrientation>();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.WriteEnum("BackBufferFormat", BackBufferFormat);
            output.WriteEnum("DepthStencilFormat", DepthStencilFormat);
            output.Write("BackBufferWidth", BackBufferWidth);
            output.Write("BackBufferHeight", BackBufferHeight);
            output.Write("MultiSampleCount", MultiSampleCount);
            output.Write("MultiSampleQuality", MultiSampleQuality);
            output.Write("IsFullScreen", IsFullScreen);
            output.WriteEnum("PresentInterval", PresentInterval);
            output.WriteEnum("RenderTargetUsage", RenderTargetUsage);
            output.WriteEnum("DisplayOrientation", DisplayOrientation);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is PresentationParameters)
            {
                PresentationParameters other = (PresentationParameters)obj;
                return (BackBufferFormat == other.BackBufferFormat) && (DepthStencilFormat == other.DepthStencilFormat) && (BackBufferWidth == other.BackBufferWidth) &&
                       (BackBufferHeight == other.BackBufferHeight) && (MultiSampleCount == other.MultiSampleCount) && (MultiSampleQuality == other.MultiSampleQuality) &&
                       (PresentInterval == other.PresentInterval) && (RenderTargetUsage == other.RenderTargetUsage) && (DisplayOrientation == other.DisplayOrientation);
            }

            return false;
        }

        /// <summary>
        /// Tests equality betwen thi presentation parameters instance and another.
        /// </summary>
        /// <param name="other">Other set of parameters to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(PresentationParameters other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality betwen thi presentation parameters instance and another.
        /// </summary>
        /// <param name="other">Other set of parameters to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref PresentationParameters other)
        {
            return (BackBufferFormat == other.BackBufferFormat) && (DepthStencilFormat == other.DepthStencilFormat) && (BackBufferWidth == other.BackBufferWidth) &&
                   (BackBufferHeight == other.BackBufferHeight) && (MultiSampleCount == other.MultiSampleCount) && (MultiSampleQuality == other.MultiSampleQuality) &&
                   (PresentInterval == other.PresentInterval) && (RenderTargetUsage == other.RenderTargetUsage) && (DisplayOrientation == other.DisplayOrientation);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return BackBufferFormat.GetHashCode() + 
                       DepthStencilFormat.GetHashCode() + 
                       BackBufferWidth.GetHashCode() + 
                       BackBufferHeight.GetHashCode() + 
                       MultiSampleCount.GetHashCode() + 
                       MultiSampleQuality.GetHashCode() +
                       IsFullScreen.GetHashCode() + 
                       PresentInterval.GetHashCode() + 
                       RenderTargetUsage.GetHashCode() + 
                       DisplayOrientation.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "BackBufferFormat: {0}, DepthStencilFormat: {1}, BackBufferWidth: {2}, BackBufferHeight: {3}, MultiSampleCount: {4}, MultiSampleQuality: {5}, IsFullScreen: {6}, PresentInterval: {7}, RenderTargetUsage: {8}, DisplayOrientation: {9}", new object[] { BackBufferFormat.ToString(), DepthStencilFormat.ToString(), BackBufferWidth.ToString(), BackBufferHeight.ToString(), MultiSampleCount.ToString(), MultiSampleQuality.ToString(), IsFullScreen.ToString(), PresentInterval.ToString(), RenderTargetUsage.ToString(), DisplayOrientation.ToString() });
        }
    }
}
