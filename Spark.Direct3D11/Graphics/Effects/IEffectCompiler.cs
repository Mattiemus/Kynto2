namespace Spark.Direct3D11.Graphics
{
    using Spark.Graphics;

    /// <summary>
    /// Interface for an effect compiler that takes in TEFX source code (.fx or HLSL fragments) and outputs effect data
    /// that can be consumed by the runtime.
    /// </summary>
    public interface IEffectCompiler
    {
        /// <summary>
        /// Compiles an effect (.fx) from a file.
        /// </summary>
        /// <param name="effectFileName">File name of the effect.</param>
        /// <param name="flags">Compile flags.</param>
        /// <param name="macros">Macros to be used during compiling.</param>
        /// <returns>Compile result</returns>
        EffectCompilerResult CompileFromFile(string effectFileName, ShaderCompileFlags flags, ShaderMacro[] macros);

        /// <summary>
        /// Compiles an effect (.fx).
        /// </summary>
        /// <param name="effectCode">Effect code to compile.</param>
        /// <param name="flags">Compile flags.</param>
        /// <param name="sourceFileName">Original source name of file.</param>
        /// <param name="macros">Macros to be used during compiling.</param>
        /// <returns>Compile result</returns>
        EffectCompilerResult Compile(string effectCode, ShaderCompileFlags flags, string sourceFileName, ShaderMacro[] macros);

        /// <summary>
        /// Compiles an effect.
        /// </summary>
        /// <param name="effectContent">Effect content that defines HLSL source code, entry points, etc that will be assembled into a complete effect.</param>
        /// <param name="flags">Compile flags.</param>
        /// <param name="macros">Macros to be used during compiling.</param>
        /// <returns>Compile result</returns>
        EffectCompilerResult Compile(EffectContent effectContent, ShaderCompileFlags flags, ShaderMacro[] macros);

        /// <summary>
        /// Sets the include handler to locate include files.
        /// </summary>
        /// <param name="includeHandler">Handler to locate and open include files.</param>
        void SetIncludeHandler(IIncludeHandler includeHandler);
    }
}
