namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.IO;
    using System.Text;

    using Content;
    using Spark.Graphics;

    /// <summary>
    /// A resource importer that can load <see cref="Effect"/> objects from the Microsoft FX (Effects11) format.
    /// </summary>
    public sealed class Effects11ResourceImporter : ResourceImporter<Effect>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Effects11ResourceImporter"/> class.
        /// </summary>
        public Effects11ResourceImporter() 
            : base(".fx")
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

            var fxParams = parameters as EffectImporterParameters;

            // For now, ShaderCompileFlags == CompileFlags
            var compileFlags = (fxParams == null) ? ShaderCompileFlags.None : fxParams.CompileFlags;
            var shaderMacros = (fxParams == null) ? null : fxParams.ShaderMacros;

            var includeHandler = new DefaultIncludeHandler();
            includeHandler.AddIncludeDirectory(Path.GetDirectoryName(resourceFile.FullName));
            if (fxParams != null && fxParams.IncludeDirectories != null)
            {
                foreach (string includeDir in fxParams.IncludeDirectories)
                {
                    includeHandler.AddIncludeDirectory(includeDir);
                }
            }

            var compiler = new EffectCompiler();
            compiler.SetIncludeHandler(includeHandler);

            var result = compiler.CompileFromFile(resourceFile.FullName, compileFlags, shaderMacros);

            if (result.HasCompileErrors)
            {
                var errorString = new StringBuilder();
                foreach (var text in result.CompileErrors)
                {
                    errorString.AppendLine(text);
                }

                throw new SparkContentException(errorString.ToString());
            }

            return new Effect(GraphicsHelper.GetRenderSystem(contentManager.ServiceProvider), result.EffectData);
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

            var fxParams = parameters as EffectImporterParameters;

            // For now, ShaderCompileFlags == CompileFlags
            var compileFlags = (fxParams == null) ? ShaderCompileFlags.None : fxParams.CompileFlags;
            var shaderMacros = (fxParams == null) ? null : fxParams.ShaderMacros;

            using (var reader = new StreamReader(input, Encoding.UTF8, true, 1024, true))
            {
                var compiler = new EffectCompiler();
                var result = compiler.Compile(reader.ReadToEnd(), compileFlags, shaderMacros);

                if (result.HasCompileErrors)
                {
                    var errorString = new StringBuilder();
                    foreach (var text in result.CompileErrors)
                    {
                        errorString.AppendLine(text);
                    }

                    throw new SparkContentException(errorString.ToString());
                }

                return new Effect(GraphicsHelper.GetRenderSystem(contentManager.ServiceProvider), result.EffectData);
            }
        }
    }
}
