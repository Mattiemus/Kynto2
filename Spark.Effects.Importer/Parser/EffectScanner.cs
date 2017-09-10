namespace Spark.Effects.Importer.Parser
{
    using Spark.Utilities.Parsing;

    /// <summary>
    /// Scanner for parsing effect files
    /// </summary>
    internal sealed class EffectScanner : Scanner<EffectTokenType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectScanner"/> class
        /// </summary>
        public EffectScanner()
        {
            // Tokens we want to skip
            RegisterSkipToken(EffectTokenType.BlockComment);
            RegisterSkipToken(EffectTokenType.Comment);
            RegisterSkipToken(EffectTokenType.Whitespace);

            // Tokens we are interested in
            RegisterToken(EffectTokenType.EndOfFile, @"^$");
            RegisterToken(EffectTokenType.BlockComment, @"/\*([^*]|\*[^/])*\*/");
            RegisterToken(EffectTokenType.Comment, @"//[^\n\r]*");
            RegisterToken(EffectTokenType.Whitespace, @"[ \t\n\r]+");
            RegisterToken(EffectTokenType.ShaderBlock, @"Shader");
            RegisterToken(EffectTokenType.VertexShaderBlock, @"VertexShader");
            RegisterToken(EffectTokenType.PixelShaderBlock, @"PixelShader");
            RegisterToken(EffectTokenType.Identifier, @"[A-Za-z_][A-Za-z0-9_]*");
            RegisterToken(EffectTokenType.StartBrace, @"{");
            RegisterToken(EffectTokenType.EndBrace, @"}");
            RegisterToken(EffectTokenType.Code, @"[\S]");
        }

        /// <summary>
        /// Gets the default token
        /// </summary>
        public override EffectTokenType DefaultToken => EffectTokenType.None;

        /// <summary>
        /// Gets the token that represents end of file
        /// </summary>
        public override EffectTokenType EndOfFileToken => EffectTokenType.EndOfFile;
    }
}
