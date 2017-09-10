namespace Spark.Effects.Importer.Parser
{
    /// <summary>
    /// Enumeration of possible effect token types
    /// </summary>
    internal enum EffectTokenType
    {
        /// <summary>
        /// Represents no token
        /// </summary>
        None,        
        
        /// <summary>
        /// End of file token
        /// </summary>
        EndOfFile,

        /// <summary>
        /// Block comment token
        /// </summary>
        BlockComment,

        /// <summary>
        /// Comment token
        /// </summary>
        Comment,

        /// <summary>
        /// Whitespace token
        /// </summary>
        Whitespace,

        /// <summary>
        /// Shader block token
        /// </summary>
        ShaderBlock,

        /// <summary>
        /// Vertex shader block token
        /// </summary>
        VertexShaderBlock,

        /// <summary>
        /// Pixel shader block token
        /// </summary>
        PixelShaderBlock,

        /// <summary>
        /// Identifier token
        /// </summary>
        Identifier,

        /// <summary>
        /// Start brace token
        /// </summary>
        StartBrace,

        /// <summary>
        /// End brace token
        /// </summary>
        EndBrace,

        /// <summary>
        /// Code section token
        /// </summary>
        Code
    }
}
