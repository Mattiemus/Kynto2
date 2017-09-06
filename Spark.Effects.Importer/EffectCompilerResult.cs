namespace Spark.Effects.Importer
{
    using Graphics.Effects;

    /// <summary>
    /// Represents the result of compiling an effect source file.
    /// </summary>
    public sealed class EffectCompilerResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectCompilerResult"/> class.
        /// </summary>
        /// <param name="effectData">Compiled effect data.</param>
        /// <param name="errorMessages">Compile error messages.</param>
        public EffectCompilerResult(EffectData effectData, string[] errorMessages)
        {
            EffectData = effectData;
            CompileErrors = errorMessages;
        }

        /// <summary>
        /// Gets the compiled effect (if successful).
        /// </summary>
        public EffectData EffectData { get; }

        /// <summary>
        /// Gets if the result was not successful and there are compile errors.
        /// </summary>
        public bool HasCompileErrors => CompileErrors != null && CompileErrors.Length > 0;

        /// <summary>
        /// Gets compile errors, if any.
        /// </summary>
        public string[] CompileErrors { get; }
    }
}
