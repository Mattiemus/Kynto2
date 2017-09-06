namespace Spark.Effects.Importer
{
    using System;
    using System.IO;
    
    using Graphics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EffectCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeHandler"></param>
        public void SetIncludeHandler(DefaultIncludeHandler includeHandler)
        {
            // No-op
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public EffectCompilerResult CompileFromFile(string fullName, CompileFlags flags)
        {
            return CompileFromFile(fullName, flags, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="flags"></param>
        /// <param name="macros"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="readToEnd"></param>
        /// <param name="flags"></param>
        /// <param name="shaderMacro"></param>
        /// <returns></returns>
        public EffectCompilerResult Compile(string readToEnd, CompileFlags flags, ShaderMacro[] shaderMacro)
        {
            return Compile(readToEnd, flags, "Unknown", shaderMacro);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readToEnd"></param>
        /// <param name="flags"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="shaderMacro"></param>
        /// <returns></returns>
        public EffectCompilerResult Compile(string readToEnd, CompileFlags flags, string sourceFileName, ShaderMacro[] shaderMacro)
        {
            Regex codeblock = new Regex(@"\s*\{[^\}]*\}([^};][^}]*\})*", RegexOptions.Compiled);

            var s = readToEnd.Substring(readToEnd.IndexOf('{'));
            var m = codeblock.Match(s);

            throw new NotImplementedException();
        }
    }
}
