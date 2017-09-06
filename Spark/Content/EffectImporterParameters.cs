namespace Spark.Content
{
    using Graphics;

    /// <summary>
    /// Defines a set of common parameters that configures how shader files are compiled.
    /// </summary>
    public class EffectImporterParameters : ImporterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectImporterParameters"/> class.
        /// </summary>
        public EffectImporterParameters()
        {
            CompressionMode = EffectCompressionMode.GZip;
            ShaderMacros = null;
            IncludeDirectories = null;
        }

        /// <summary>
        /// Gets or sets the compression mode used when importing the effect. Default is <see cref="EffectCompressionMode.GZip"/>
        /// </summary>
        public EffectCompressionMode CompressionMode { get; set; }

        /// <summary>
        /// Gets or sets shader macros. A null array is valid.
        /// </summary>
        public ShaderMacro[] ShaderMacros { get; set; }

        /// <summary>
        /// Gets or sets additional include directories. A null array is valid.
        /// </summary>
        public string[] IncludeDirectories { get; set; }

        /// <summary>
        /// Copies the importer parameters from the specified instance.
        /// </summary>
        /// <param name="parameters">Importer parameter instance to copy from.</param>
        public override void Set(ImporterParameters parameters)
        {
            base.Set(parameters);

            EffectImporterParameters effectParams = parameters as EffectImporterParameters;

            if (effectParams == null)
            {
                return;
            }

            CompressionMode = effectParams.CompressionMode;
            ShaderMacros = (effectParams.ShaderMacros == null) ? null : effectParams.ShaderMacros.Clone() as ShaderMacro[];
            IncludeDirectories = (effectParams.IncludeDirectories == null) ? null : effectParams.IncludeDirectories.Clone() as string[];
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            base.Write(output);

            output.WriteEnum("CompressionMode", CompressionMode);
            output.WriteSavable("ShaderMacros", ShaderMacros);
            output.Write("IncludeDirectories", IncludeDirectories);
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            base.Read(input);

            CompressionMode = input.ReadEnum<EffectCompressionMode>();
            ShaderMacros = input.ReadSavableArray<ShaderMacro>();
            IncludeDirectories = input.ReadStringArray();
        }
    }
}
