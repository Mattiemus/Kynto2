namespace Spark.Content
{
    using Math;

    /// <summary>
    /// Defines a set of common parameters for texture importers.
    /// </summary>
    public class TextureImporterParameters : ImporterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureImporterParameters"/> class.
        /// </summary>
        public TextureImporterParameters()
            : this(false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureImporterParameters"/> class.
        /// </summary>
        /// <param name="flipImage">True if the image should be flipped about its V axis, false otherwise.</param>
        /// <param name="genMipMaps">True if the image should have mipmaps generated if none are present, false otherwise.</param>
        /// <param name="resizePowerTwo">True if the image should be resized to a power of two, false otherwise.</param>
        public TextureImporterParameters(bool flipImage, bool genMipMaps, bool resizePowerTwo)
        {
            FlipImage = flipImage;
            GenerateMipMaps = genMipMaps;
            ResizePowerOfTwo = resizePowerTwo;
            TextureFormat = TextureConversionFormat.NoChange;
            ColorKey = new Color(255, 0, 255, 255);
            ColorKeyEnabled = false;
            PreMultiplyAlpha = false;
            IsNormalMap = false;
        }

        /// <summary>
        /// Gets or sets if images should be flipped along the V axis when they are
        /// imported. Default is false.
        /// </summary>
        public bool FlipImage { get; set; }

        /// <summary>
        /// Gets or sets if mip maps should be generated for images that do not have them.
        /// Existing mip maps are not changed. Default is false.
        /// </summary>
        public bool GenerateMipMaps { get; set; }

        /// <summary>
        /// Gets or sets if the image should be resized to the nearest power of two
        /// dimension for compatibility with older GPUs and performance. Default is false.
        /// </summary>
        public bool ResizePowerOfTwo { get; set; }

        /// <summary>
        /// Gets or sets what format the image should be converted to upon import. If
        /// <see cref="TextureConversionFormat.NoChange"/> (the default) then the image's existing format is used
        /// if possible. If not possible, then the standard color format is used.
        /// </summary>
        public TextureConversionFormat TextureFormat { get; set; }

        /// <summary>
        /// Gets or sets the color key. Any pixel that matches this color will get replaced
        /// by transparent black.  Default color is Magenta (255, 0, 255, 255).
        /// </summary>
        public Color ColorKey { get; set; }

        /// <summary>
        /// Gets or sets if the color key should be enabled. Default is false.
        /// </summary>
        public bool ColorKeyEnabled { get; set; }

        /// <summary>
        /// Gets or sets if the image's alpha should be pre-multiplied with the pixel color values. Default is false.
        /// </summary>
        public bool PreMultiplyAlpha { get; set; }

        /// <summary>
        /// Gets or sets if the image is to be used as a normal map. Default is false.
        /// </summary>
        public bool IsNormalMap { get; set; }

        /// <summary>
        /// Copies the importer parameters from the specified instance.
        /// </summary>
        /// <param name="parameters">Importer parameter instance to copy from.</param>
        public override void Set(ImporterParameters parameters)
        {
            base.Set(parameters);

            TextureImporterParameters imageParams = parameters as TextureImporterParameters;

            if (imageParams == null)
            {
                return;
            }

            FlipImage = imageParams.FlipImage;
            GenerateMipMaps = imageParams.GenerateMipMaps;
            ResizePowerOfTwo = imageParams.ResizePowerOfTwo;
            TextureFormat = imageParams.TextureFormat;
            ColorKey = imageParams.ColorKey;
            ColorKeyEnabled = imageParams.ColorKeyEnabled;
            PreMultiplyAlpha = imageParams.PreMultiplyAlpha;
            IsNormalMap = imageParams.IsNormalMap;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            base.Write(output);

            output.Write("FlipImage", FlipImage);
            output.Write("GenerateMipMaps", GenerateMipMaps);
            output.Write("ResizePowerOfTwo", ResizePowerOfTwo);
            output.WriteEnum("TextureFormat", TextureFormat);
            output.Write("ColorKey", ColorKey);
            output.Write("ColorKeyEnabled", ColorKeyEnabled);
            output.Write("PreMultiplyAlpha", PreMultiplyAlpha);
            output.Write("IsNormalMap", IsNormalMap);
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            base.Read(input);

            FlipImage = input.ReadBoolean();
            GenerateMipMaps = input.ReadBoolean();
            ResizePowerOfTwo = input.ReadBoolean();
            TextureFormat = input.ReadEnum<TextureConversionFormat>();
            ColorKey = input.Read<Color>();
            ColorKeyEnabled = input.ReadBoolean();
            PreMultiplyAlpha = input.ReadBoolean();
            IsNormalMap = input.ReadBoolean();
        }
    }
}
