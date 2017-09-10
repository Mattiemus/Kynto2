namespace Spark.Effects.Importer
{
    using System.IO;
    using System.Text;

    using Content;
    using Graphics;
    using Graphics.Effects;

    /// <summary>
    /// A resource importer that can load <see cref="Effect"/> objects from a Spark effect file
    /// </summary>
    public sealed class EffectImporter : ResourceImporter<Effect>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectImporter"/> class.
        /// </summary>
        public EffectImporter()
            : base(".effect")
        {
        }

        /// <summary>
        /// Loads content from the specified resource as the target runtime type.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        public override Effect Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(resourceFile, contentManager, ref parameters);

            EffectImporterParameters fxParams = parameters as EffectImporterParameters;
            
            ShaderMacro[] shaderMacros = (fxParams == null) ? null : fxParams.ShaderMacros;

            DefaultIncludeHandler includeHandler = new DefaultIncludeHandler();
            includeHandler.AddIncludeDirectory(Path.GetDirectoryName(resourceFile.FullName));
            if (fxParams != null && fxParams.IncludeDirectories != null)
            {
                foreach (string includeDir in fxParams.IncludeDirectories)
                {
                    includeHandler.AddIncludeDirectory(includeDir);
                }
            }

            EffectCompiler compiler = new EffectCompiler();
            compiler.SetIncludeHandler(includeHandler);
            EffectCompilerResult result = compiler.CompileFromFile(resourceFile.FullName, CompileFlags.None, shaderMacros);

            if (result.HasCompileErrors)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (string text in result.CompileErrors)
                {
                    errorString.AppendLine(text);
                }

                throw new SparkContentException(errorString.ToString());
            }

            return new Effect(GraphicsHelpers.GetRenderSystem(contentManager.ServiceProvider), result.EffectData);
        }

        /// <summary>
        /// Loads content from the specified stream as the target runtime type.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        public override Effect Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            ValidateParameters(input, contentManager, ref parameters);

            EffectImporterParameters fxParams = parameters as EffectImporterParameters;
            
            ShaderMacro[] shaderMacros = (fxParams == null) ? null : fxParams.ShaderMacros;

            using (StreamReader reader = new StreamReader(input, Encoding.UTF8, true, 1024, true))
            {
                EffectCompiler compiler = new EffectCompiler();
                EffectCompilerResult result = compiler.Compile(reader.ReadToEnd(), CompileFlags.None, shaderMacros);

                if (result.HasCompileErrors)
                {
                    StringBuilder errorString = new StringBuilder();
                    foreach (string text in result.CompileErrors)
                    {
                        errorString.AppendLine(text);
                    }

                    throw new SparkContentException(errorString.ToString());
                }

                return new Effect(GraphicsHelpers.GetRenderSystem(contentManager.ServiceProvider), result.EffectData);
            }
        }
    }
}
