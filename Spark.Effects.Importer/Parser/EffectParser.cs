namespace Spark.Effects.Importer.Parser
{
    using Spark.Utilities.Parsing;

    /// <summary>
    /// Parser for effect files
    /// </summary>
    internal sealed class EffectParser : Parser<EffectTokenType, EffectNodeType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectParser"/> class
        /// </summary>
        /// <param name="scanner">Effect scanner</param>
        public EffectParser(EffectScanner scanner)
            : base(scanner)
        {
        }

        /// <summary>
        /// Gets the node type of the start node
        /// </summary>
        public override EffectNodeType StartNode => EffectNodeType.ShaderBlock;

        /// <summary>
        /// Entrypoint for the parser
        /// </summary>
        /// <param name="node">Parent node</param>
        protected override void ParseStart(ParseNode<EffectTokenType, EffectNodeType> node)
        {
            ExpectToken(node, EffectTokenType.ShaderBlock);
            ExpectToken(node, EffectTokenType.Identifier);
            ExpectToken(node, EffectTokenType.StartBrace);

            Token<EffectTokenType> tok = Scanner.LookAhead(EffectTokenType.VertexShaderBlock, EffectTokenType.PixelShaderBlock);
            while (tok.TokenType == EffectTokenType.VertexShaderBlock || tok.TokenType == EffectTokenType.PixelShaderBlock)
            {
                switch (tok.TokenType)
                {
                    case EffectTokenType.VertexShaderBlock:
                        ParseWith(node, EffectNodeType.VertexShader, ParseVertexShaderBlock);
                        break;
                    case EffectTokenType.PixelShaderBlock:
                        ParseWith(node, EffectNodeType.PixelShader, ParsePixelShaderBlock);
                        break;
                    default:
                        throw new SparkParseException($"Unexpected token '{tok.Text.Replace("\n", "")}' found.");
                }
                tok = Scanner.LookAhead(EffectTokenType.VertexShaderBlock, EffectTokenType.PixelShaderBlock);
            }

            ExpectToken(node, EffectTokenType.EndBrace);
            ExpectToken(node, EffectTokenType.EndOfFile);
        }

        /// <summary>
        /// Parses a vertex shader block
        /// </summary>
        /// <param name="node">Parent node</param>
        private void ParseVertexShaderBlock(ParseNode<EffectTokenType, EffectNodeType> node)
        {
            ExpectToken(node, EffectTokenType.VertexShaderBlock);
            ExpectToken(node, EffectTokenType.StartBrace);
            ParseWith(node, EffectNodeType.CodeBlock, ParseCodeBlock);
            ExpectToken(node, EffectTokenType.EndBrace);
        }

        /// <summary>
        /// Parses a pixel shader block
        /// </summary>
        /// <param name="node">Parent node</param>
        private void ParsePixelShaderBlock(ParseNode<EffectTokenType, EffectNodeType> node)
        {
            ExpectToken(node, EffectTokenType.PixelShaderBlock);
            ExpectToken(node, EffectTokenType.StartBrace);
            ParseWith(node, EffectNodeType.CodeBlock, ParseCodeBlock);
            ExpectToken(node, EffectTokenType.EndBrace);
        }

        /// <summary>
        /// Parses a block of code
        /// </summary>
        /// <param name="node">Parent node</param>
        private void ParseCodeBlock(ParseNode<EffectTokenType, EffectNodeType> node)
        {
            int braceCount = 0;
            while(true)
            {
                Token<EffectTokenType> token = Scanner.LookAhead();
                if (token.TokenType == EffectTokenType.EndBrace && braceCount == 0)
                {
                    break;
                }
                
                Scanner.Scan();
                node.CreateNode(token);

                if (token.TokenType == EffectTokenType.StartBrace)
                {
                    braceCount++;
                }

                if (token.TokenType == EffectTokenType.EndBrace)
                {
                    braceCount--;
                }
            }
        }
    }
}
