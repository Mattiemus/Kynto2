namespace Spark.Effects.Importer
{
    using System.Linq;
    using System.IO;
    
    using Graphics;
    using Parser;
    using Utilities.Parsing;

    /// <summary>
    /// Effect file compiler
    /// </summary>
    internal sealed class EffectCompiler
    {
        /// <summary>
        /// Gets the handler to use when dealing with include directives
        /// </summary>
        /// <param name="includeHandler"></param>
        public void SetIncludeHandler(DefaultIncludeHandler includeHandler)
        {
            // No-op
        }

        /// <summary>
        /// Compiles a effect file
        /// </summary>
        /// <param name="fullName">Name of the file</param>
        /// <param name="flags">Compilation flags</param>
        /// <returns>Effect compiler result</returns>
        public EffectCompilerResult CompileFromFile(string fullName, CompileFlags flags)
        {
            return CompileFromFile(fullName, flags, null);
        }

        /// <summary>
        /// Compiles a effect file
        /// </summary>
        /// <param name="fullName">Name of the file</param>
        /// <param name="flags">Compilation flags</param>
        /// <param name="macros">Effect macros</param>
        /// <returns>Effect compiler result</returns>
        public EffectCompilerResult CompileFromFile(string fullName, CompileFlags flags, ShaderMacro[] macros)
        {
            if (!File.Exists(fullName))
            {
                return new EffectCompilerResult(null, new [] 
                {
                    $"File not found: {fullName}"
                });
            }

            string effectCode = File.ReadAllText(fullName);
            return Compile(effectCode, flags, fullName, macros);
        }

        /// <summary>
        /// Compiles a effect
        /// </summary>
        /// <param name="readToEnd">File contents</param>
        /// <param name="flags">Compilation flags</param>
        /// <param name="macros">Effect macros</param>
        /// <returns>Effect compiler result</returns>
        public EffectCompilerResult Compile(string readToEnd, CompileFlags flags, ShaderMacro[] shaderMacro)
        {
            return Compile(readToEnd, flags, "Unknown", shaderMacro);
        }

        /// <summary>
        /// Compiles a effect
        /// </summary>
        /// <param name="readToEnd">File contents</param>
        /// <param name="flags">Compilation flags</param>
        /// <param name="sourceFileName">Name of the effect file</param>
        /// <param name="shaderMacro">Effect macros</param>
        /// <returns>Effect compiler result</returns>
        public EffectCompilerResult Compile(string readToEnd, CompileFlags flags, string sourceFileName, ShaderMacro[] shaderMacro)
        {
            try
            {
                var scanner = new EffectScanner();
                var parser = new EffectParser(scanner);
                ParseTree<EffectTokenType, EffectNodeType> parseTree = parser.Parse(readToEnd);

                var effectData = new EffectData();
                effectData.EffectName = parseTree.Children[1].Text;
                effectData.VertexShader = parseTree.Children.Last(c => c.NodeType == EffectNodeType.VertexShader).Children[2].Text;
                effectData.PixelShader = parseTree.Children.Last(c => c.NodeType == EffectNodeType.PixelShader).Children[2].Text;

                return new EffectCompilerResult(effectData);
            }
            catch (SparkParseException ex)
            {
                return new EffectCompilerResult(null, new[] { ex.Message });
            }
        }
    }
}
