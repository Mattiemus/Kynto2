namespace Spark.Effects.Importer.Parser
{
    /// <summary>
    /// Enumeration of possible effect node types
    /// </summary>
    internal enum EffectNodeType
    {
        /// <summary>
        /// Shader block
        /// </summary>
        ShaderBlock,

        /// <summary>
        /// Vertex shader
        /// </summary>
        VertexShader,

        /// <summary>
        /// Pixel shader
        /// </summary>
        PixelShader,

        /// <summary>
        /// Block of code
        /// </summary>
        CodeBlock
    }
}
