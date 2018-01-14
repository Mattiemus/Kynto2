namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Math;
    using Spark.Graphics;
    using Spark.Utilities;
    using Spark.Utilities.Parsing;

    using D3D = SharpDX.Direct3D;
    using D3DC = SharpDX.D3DCompiler;

    public sealed class EffectCompiler : IEffectCompiler
    {
        private IncludeAdapter _include;
        
        public void SetIncludeHandler(IIncludeHandler includeHandler)
        {
            if (includeHandler == null)
            {
                _include = null;
            }
            else
            {
                _include = new IncludeAdapter(includeHandler);
            }
        }

        #region Compilation

        public EffectCompilerResult CompileFromFile(string effectFileName, ShaderCompileFlags flags)
        {
            return CompileFromFile(effectFileName, flags, null);
        }

        public EffectCompilerResult Compile(string effectCode, ShaderCompileFlags flags)
        {
            return Compile(effectCode, flags, "<unknown>", null);
        }

        public EffectCompilerResult Compile(string effectCode, ShaderCompileFlags flags, ShaderMacro[] macros)
        {
            return Compile(effectCode, flags, "<unknown>", macros);
        }

        public EffectCompilerResult Compile(EffectContent effectContent, ShaderCompileFlags flags)
        {
            return Compile(effectContent, flags, null);
        }

        public EffectCompilerResult CompileFromFile(string effectFileName, ShaderCompileFlags flags, ShaderMacro[] macros)
        {
            if (!File.Exists(effectFileName))
            {
                return new EffectCompilerResult(null, new string[] { $"File could not be found: {effectFileName}" });
            }

            string effectCode = File.ReadAllText(effectFileName);
            return Compile(effectCode, flags, effectFileName, macros);
        }

        public EffectCompilerResult Compile(string effectCode, ShaderCompileFlags flags, string sourceFileName, ShaderMacro[] macros)
        {
            if (string.IsNullOrEmpty(effectCode))
            {
                return new EffectCompilerResult(null, new string[] { "Effect code is empty" });
            }

            // Parse FX file
            var effectContent = Parse(effectCode, sourceFileName, out string parseErrors);
            if (effectContent == null || !String.IsNullOrEmpty(parseErrors))
            {
                return new EffectCompilerResult(null, new string[] { parseErrors });
            }

            return Compile(effectContent, flags, macros);
        }

        public EffectCompilerResult Compile(EffectContent effectContent, ShaderCompileFlags flags, ShaderMacro[] macros)
        {
            if (effectContent == null)
            {
                return new EffectCompilerResult(null, new string[] { "Effect content is null" });
            }

            if (!HasAtLeastOneValidShaderGroup(effectContent))
            {
                return new EffectCompilerResult(null, new string[] { "No shaders to compile" });
            }

            var d3dShaderCompileFlags = (D3DC.ShaderFlags)flags;
            var d3dMacros = ConvertShaderMacros(macros);

            // Now we actually evoke the D3D HLSL effect compiler here, then reflect the compiled shaders
            var metaData = new ShaderMetaData((_include != null) ? _include.IncludeHandler : null);

            // Ensure each shader group name is unique
            var uniqueGroupNameCheck = new Dictionary<String, String>();

            foreach (EffectContent.ShaderGroupContent grp in effectContent)
            {
                if (uniqueGroupNameCheck.ContainsKey(grp.Name))
                {
                    return new EffectCompilerResult(null, new string[] { $"Shader group '{grp.Name}' already declared in effect" });
                }

                foreach (EffectContent.ShaderContent shader in grp)
                {
                    var errors = CompileShader(shader, effectContent.FileName, d3dShaderCompileFlags, d3dMacros, metaData);
                    if (errors != null)
                    {
                        return new EffectCompilerResult(null, errors.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }

            // Now we have a map of every unique compiled shader and reflected data, so create the effect data
            EffectData data = CreateEffectData(effectContent, metaData);
            return new EffectCompilerResult(data, null);
        }

        private string CompileShader(EffectContent.ShaderContent shaderContent, string sourceFileName, D3DC.ShaderFlags compileFlags, D3D.ShaderMacro[] macros, ShaderMetaData metaData)
        {
            if (shaderContent == null)
            {
                return "Shader content is null";
            }

            // Have we seen this content already? Don't try to compile if so
            if (metaData.ContainsShader(shaderContent))
            {
                return null;
            }

            // Compile HLSL
            D3DC.CompilationResult compResult;
            try
            {
                compResult = D3DC.ShaderBytecode.Compile(shaderContent.SourceCode, shaderContent.EntryPoint, shaderContent.ShaderProfile, compileFlags, D3DC.EffectFlags.None, macros, _include, sourceFileName);
                if (compResult.HasErrors || compResult.Bytecode == null)
                {
                    return compResult.Message;
                }
            }
            catch (SharpDX.CompilationException e)
            {
                return e.Message;
            }

            // Reflect compiled bytecode
            CompiledShader compiledShader;
            using (var byteCode = compResult.Bytecode)
            {
                compiledShader = new CompiledShader(byteCode);
            }

            metaData.AddCompiledShader(shaderContent, compiledShader);

            return null;
        }

        private D3D.ShaderMacro[] ConvertShaderMacros(ShaderMacro[] macros)
        {
            if (macros == null || macros.Length == 0)
            {
                return null;
            }

            var d3dMacros = new D3D.ShaderMacro[macros.Length];
            for (int i = 0; i < macros.Length; i++)
            {
                ShaderMacro macro = macros[i];
                if (macro != null)
                {
                    d3dMacros[i] = new D3D.ShaderMacro(macro.Name, macro.Definition);
                }
            }

            return d3dMacros;
        }

        private bool HasAtLeastOneValidShaderGroup(EffectContent effectContent)
        {
            int validPasses = 0;

            foreach (EffectContent.ShaderGroupContent group in effectContent)
            {
                validPasses += group.ShaderCount;
            }

            return validPasses > 0;
        }

        private void AddDefaultIncludeDirectory(string sourceFileName)
        {
            if (File.Exists(sourceFileName))
            {
                string dir = Path.GetDirectoryName(sourceFileName);
                if (!string.IsNullOrEmpty(dir) && _include == null && _include.IncludeHandler != null)
                {
                    _include.IncludeHandler.AddIncludeDirectory(dir);
                }
            }
        }

        #endregion

        #region EffectData structure setup

        private EffectData CreateEffectData(EffectContent effectContent, ShaderMetaData metaData)
        {
            var effectData = new EffectData();
            effectData.ShaderGroups = new EffectData.ShaderGroup[effectContent.ShaderGroupCount];

            // Process shader groups
            for (int i = 0; i < effectData.ShaderGroups.Length; i++)
            {
                effectData.ShaderGroups[i] = CreateShaderGroup(effectContent[i], metaData);
            }

            effectData.Shaders = metaData.Shaders.ToArray();
            effectData.ConstantBuffers = metaData.ConstantBuffers.ToArray();
            effectData.ResourceVariables = metaData.ResourceVariables.ToArray();

            return effectData;
        }

        private EffectData.ShaderGroup CreateShaderGroup(EffectContent.ShaderGroupContent shaderGroupContent, ShaderMetaData metaData)
        {
            var group = new EffectData.ShaderGroup();
            group.Name = shaderGroupContent.Name;
            group.ShaderIndices = new int[shaderGroupContent.ShaderCount];

            // Not directly setting shaders, instead look up based on the shader content and find the index
            int index = 0;
            foreach (EffectContent.ShaderContent shaderContent in shaderGroupContent)
            {
                group.ShaderIndices[index] = metaData.GetShaderIndex(shaderContent);
                index++;
            }

            return group;
        }

        #endregion

        #region SPFX Content Parsing

        public EffectContent Parse(string effectCode)
        {
            return Parse(effectCode, "<unknown>", out string errors);
        }

        public EffectContent Parse(string effectCode, out string parseErrors)
        {
            return Parse(effectCode, "<unknown>", out parseErrors);
        }

        public EffectContent Parse(string effectCode, string sourceFileName)
        {
            return Parse(effectCode, sourceFileName, out string errors);
        }

        public EffectContent Parse(string effectCode, string sourceFileName, out string parseErrors)
        {
            parseErrors = null;

            if (string.IsNullOrEmpty(effectCode))
            {
                return null;
            }

            var tinyParser = new Parser(new Scanner());
            var tree = tinyParser.Parse(effectCode);

            if (tree.Errors.Count > 0)
            {
                parseErrors = GetParseErrors(effectCode, tree);
                return null;
            }

            var effect = tree.Eval() as EffectContent;
            if (effect != null)
            {
                effect.FileName = String.IsNullOrEmpty(sourceFileName) ? String.Empty : Path.GetFileName(sourceFileName);

                SetSourceCode(effectCode, effect);
            }
            else
            {
                parseErrors = "An unknown error occured";
            }

            return effect;
        }

        private void SetSourceCode(string sourceCode, EffectContent effect)
        {
            foreach (var group in effect)
            {
                foreach (var shader in group)
                {
                    shader.SourceCode = sourceCode;
                }
            }
        }

        private string GetParseErrors(string effectCode, ParseTree tree)
        {
            var errors = new StringBuilder();
            foreach (ParseError error in tree.Errors)
            {
                FindLineAndColumn(effectCode, error.Position, out int line, out int col);
                errors.AppendLine($"{line}:{col}: {error.Message}");
            }

            return errors.ToString();
        }

        private void FindLineAndColumn(string src, int pos, out int line, out int col)
        {
            line = 1;
            col = 1;

            for (int i = 0; i < pos; i++)
            {
                if (src[i] == '\n')
                {
                    line++;
                    col = 1;
                }
                else
                {
                    col++;
                }
            }
        }

        #endregion

        #region Internal Structures

        private struct CompiledShader
        {
            public byte[] CompiledByteCode;
            public D3DShaderReflection MetaData;
            public EffectContent.ShaderContent ShaderContent;
            public int Index;

            public CompiledShader(byte[] compiledByteCode, D3DShaderReflection metaData)
            {
                CompiledByteCode = compiledByteCode;
                MetaData = metaData;
                ShaderContent = null;
                Index = -1;
            }

            public CompiledShader(D3DC.ShaderBytecode byteCode)
            {
                CompiledByteCode = byteCode.Data.Clone() as byte[];
                MetaData = new D3DShaderReflection(byteCode);
                ShaderContent = null;
                Index = -1;
            }
        }

        private class ShaderMetaData
        {
            private readonly Dictionary<EffectContent.ShaderContent, CompiledShader> _uniqueCompiledShadersMap;
            private readonly Dictionary<EffectContent.ShaderContent, CompiledShader> _completeCompiledShadersMap;
            private readonly Dictionary<String, Dictionary<String, EffectData.SamplerStateData>> _parsedSamplerStates;
            private readonly List<D3DConstantBuffer> _reflectedConstantBuffers;
            private SamplerDataParser _samplerParser;
            private readonly IIncludeHandler _includeHandler;

            public List<EffectData.Shader> Shaders;
            public List<EffectData.ConstantBuffer> ConstantBuffers;
            public List<EffectData.ResourceVariable> ResourceVariables;
            
            public ShaderMetaData(IIncludeHandler includeHandler)
            {
                _includeHandler = includeHandler;
                _uniqueCompiledShadersMap = new Dictionary<EffectContent.ShaderContent, CompiledShader>(new ReferenceEqualityComparer<EffectContent.ShaderContent>());
                _completeCompiledShadersMap = new Dictionary<EffectContent.ShaderContent, CompiledShader>(new ReferenceEqualityComparer<EffectContent.ShaderContent>());
                _parsedSamplerStates = new Dictionary<String, Dictionary<String, EffectData.SamplerStateData>>();
                _reflectedConstantBuffers = new List<D3DConstantBuffer>();

                Shaders = new List<EffectData.Shader>();
                ConstantBuffers = new List<EffectData.ConstantBuffer>();
                ResourceVariables = new List<EffectData.ResourceVariable>();
            }

            public int GetShaderIndex(EffectContent.ShaderContent shaderContent)
            {
                if (_completeCompiledShadersMap.TryGetValue(shaderContent, out CompiledShader shader))
                {
                    return shader.Index;
                }

                return -1;
            }

            public bool ContainsShader(EffectContent.ShaderContent shaderContent)
            {
                return _completeCompiledShadersMap.TryGetValue(shaderContent, out CompiledShader shader);
            }

            public bool AddCompiledShader(EffectContent.ShaderContent shaderContent, CompiledShader compiledShader)
            {
                if (ContainsShader(shaderContent))
                {
                    return false;
                }

                bool addToMap = true;
                bool addDuplicate = false;
                string reasonWhyNotSame;
                CompiledShader shaderContained = compiledShader;
                foreach (KeyValuePair<EffectContent.ShaderContent, CompiledShader> kv in _uniqueCompiledShadersMap)
                {
                    if (kv.Value.MetaData.AreSame(compiledShader.MetaData, out reasonWhyNotSame))
                    {
                        addToMap = false;
                        addDuplicate = true;
                        shaderContained = kv.Value;
                        break;
                    }
                }

                if (addToMap)
                {
                    compiledShader.Index = Shaders.Count;
                    compiledShader.ShaderContent = shaderContent;
                    Shaders.Add(CreateShader(compiledShader));
                    _uniqueCompiledShadersMap.Add(shaderContent, compiledShader);
                    _completeCompiledShadersMap[shaderContent] = compiledShader;
                }
                else if (addDuplicate)
                {
                    _completeCompiledShadersMap[shaderContent] = shaderContained;
                }

                return addToMap;
            }

            public int GetConstantBufferIndex(D3DConstantBuffer constantBuffer)
            {
                for (int i = 0; i < _reflectedConstantBuffers.Count; i++)
                {
                    if (_reflectedConstantBuffers[i].AreSame(constantBuffer, out string reasonWhyNotSame))
                    {
                        return i;
                    }
                }

                _reflectedConstantBuffers.Add(constantBuffer);
                ConstantBuffers.Add(CreateConstantBuffer(constantBuffer));
                return _reflectedConstantBuffers.Count - 1;
            }

            public int GetResourceBindingIndex(D3DResourceBinding resourceBinding, D3DShaderReflection parent, CompiledShader shader)
            {
                if (resourceBinding.IsConstantBuffer)
                {
                    return GetConstantBufferIndex(parent.GetConstantBufferByName(resourceBinding.Name));
                }
                else
                {
                    for (int i = 0; i < ResourceVariables.Count; i++)
                    {
                        EffectData.ResourceVariable resource = ResourceVariables[i];
                        if (resourceBinding.InputFlags == (D3DShaderInputFlags)resource.InputFlags && 
                            resourceBinding.Name == resource.Name && 
                            resourceBinding.ResourceDimension == (D3DResourceDimension)resource.ResourceDimension &&
                            resourceBinding.ResourceType == (D3DShaderInputType)resource.ResourceType && 
                            resourceBinding.ReturnType == (D3DResourceReturnType)resource.ReturnType && 
                            resourceBinding.SampleCount == resource.SampleCount)
                        {
                            // Check element count, noticed that the BindCount will be up to the maximum index used by the shader. So a Texture2D[3], and only first index is used
                            // bind count will be 1 - probably because the HLSL compiler unrolls this into three Texture2Ds. So be sure to take the maximum bindcount as the elementcount
                            resource.ElementCount = Math.Max(resource.ElementCount, (resourceBinding.BindCount > 1) ? resourceBinding.BindCount : 0);

                            return i;
                        }
                    }

                    ResourceVariables.Add(CreateResourceVariable(resourceBinding, shader));
                    return ResourceVariables.Count - 1;
                }
            }

            #region EffectData creation

            private EffectData.ResourceVariable CreateResourceVariable(D3DResourceBinding resourceBinding, CompiledShader shader)
            {
                var variable = new EffectData.ResourceVariable();
                variable.InputFlags = (Spark.Graphics.D3DShaderInputFlags)resourceBinding.InputFlags;
                variable.Name = resourceBinding.Name;
                variable.ResourceDimension = (Spark.Graphics.D3DResourceDimension)resourceBinding.ResourceDimension;
                variable.ResourceType = (Spark.Graphics.D3DShaderInputType)resourceBinding.ResourceType;
                variable.ReturnType = (Spark.Graphics.D3DResourceReturnType)resourceBinding.ReturnType;
                variable.SampleCount = resourceBinding.SampleCount;
                variable.ElementCount = (resourceBinding.BindCount > 1) ? resourceBinding.BindCount : 0;
                variable.SamplerData = null;

                // Sampler arrays not supported, for now. Not sure how much used they are
                if ((D3DShaderInputType)variable.ResourceType == D3DShaderInputType.Sampler && variable.ElementCount == 0)
                {
                    variable.SamplerData = GetSamplerStateData(shader.ShaderContent, variable.Name);
                }

                return variable;
            }

            private EffectData.SamplerStateData GetSamplerStateData(EffectContent.ShaderContent shader, string variableName)
            {
                if (_samplerParser == null)
                {
                    _samplerParser = new SamplerDataParser();
                }

                if (!_parsedSamplerStates.TryGetValue(shader.SourceCode, out Dictionary<String, EffectData.SamplerStateData> samplerStates))
                {
                    samplerStates = new Dictionary<String, EffectData.SamplerStateData>();
                    _samplerParser.ParseAllSamplerStates(shader.SourceCode, _includeHandler, samplerStates);
                    _parsedSamplerStates.Add(shader.SourceCode, samplerStates);
                }
                
                if (samplerStates.TryGetValue(variableName, out EffectData.SamplerStateData sampler))
                {
                    return sampler;
                }

                return null;
            }

            private EffectData.Shader CreateShader(CompiledShader compiledShader)
            {
                var reflect = compiledShader.MetaData;

                var shader = new EffectData.Shader();
                shader.ShaderType = compiledShader.ShaderContent.ShaderType;
                shader.ShaderProfile = compiledShader.ShaderContent.ShaderProfile;
                shader.ShaderFlags = reflect.ShaderFlags;
                shader.ShaderByteCode = compiledShader.CompiledByteCode;
                shader.HashCode = MemoryHelper.ComputeFNVModifiedHashCode(shader.ShaderByteCode);
                shader.GeometryShaderInputPrimitive = (Spark.Graphics.D3DInputPrimitive)reflect.GeometryShaderInputPrimitive;
                shader.GeometryShaderOutputTopology = (Spark.Graphics.D3DPrimitiveTopology)reflect.GeometryShaderOutputTopology;
                shader.GeometryShaderInstanceCount = reflect.GeometryShaderInstanceCount;
                shader.MaxGeometryShaderOutputVertexCount = reflect.MaxGeometryShaderOutputVertexCount;
                shader.GeometryOrHullShaderInputPrimitive = (Spark.Graphics.D3DInputPrimitive)reflect.GeometryOrHullShaderInputPrimitive;
                shader.HullShaderOutputPrimitive = (Spark.Graphics.D3DTessellatorOutputPrimitive)reflect.HullShaderOutputPrimitive;
                shader.HullShaderPartitioning = (Spark.Graphics.D3DTessellatorPartitioning)reflect.HullShaderPartitioning;
                shader.TessellatorDomain = (Spark.Graphics.D3DTessellatorDomain)reflect.TessellatorDomain;
                shader.IsSampleFrequencyShader = reflect.IsSampleFrequencyShader;

                // Bound resources
                shader.BoundResources = new EffectData.BoundResource[reflect.BoundResources.Length];
                for (int i = 0; i < reflect.BoundResources.Length; i++)
                {
                    var binding = reflect.BoundResources[i];

                    var bResource = new EffectData.BoundResource();
                    bResource.ResourceIndex = GetResourceBindingIndex(binding, reflect, compiledShader);
                    bResource.ResourceType = (Spark.Graphics.D3DShaderInputType)binding.ResourceType;
                    bResource.BindPoint = binding.BindPoint;
                    bResource.BindCount = binding.BindCount;

                    shader.BoundResources[i] = bResource;
                }

                // Add signatures
                shader.InputSignature = CreateSignature(reflect.InputSignatureByteCode, reflect.InputSignatureByteCodeHash, reflect.InputSignature);
                shader.OutputSignature = CreateSignature(reflect.OutputSignatureByteCode, reflect.OutputSignatureByteCodeHash, reflect.OutputSignature);

                return shader;
            }

            private EffectData.Signature CreateSignature(byte[] sigByteCode, int sigHash, D3DShaderParameter[] d3dSig)
            {
                var sig = new EffectData.Signature();
                sig.ByteCode = sigByteCode;
                sig.HashCode = sigHash;
                sig.Parameters = new EffectData.SignatureParameter[d3dSig.Length];
                for (int i = 0; i < d3dSig.Length; i++)
                {
                    var d3dSigParam = d3dSig[i];

                    var sigParam = new EffectData.SignatureParameter();
                    sigParam.ComponentType = (Spark.Graphics.D3DComponentType)d3dSigParam.ComponentType;
                    sigParam.ReadWriteMask = (Spark.Graphics.D3DComponentMaskFlags)d3dSigParam.ReadWriteMask;
                    sigParam.Register = d3dSigParam.Register;
                    sigParam.SemanticIndex = d3dSigParam.SemanticIndex;
                    sigParam.SemanticName = d3dSigParam.SemanticName;
                    sigParam.StreamIndex = d3dSigParam.StreamIndex;
                    sigParam.SystemType = (Spark.Graphics.D3DSystemValueType)d3dSigParam.SystemType;
                    sigParam.UsageMask = (Spark.Graphics.D3DComponentMaskFlags)d3dSigParam.UsageMask;

                    sig.Parameters[i] = sigParam;
                }

                return sig;
            }

            private EffectData.ConstantBuffer CreateConstantBuffer(D3DConstantBuffer reflectedBuffer)
            {
                var cb = new EffectData.ConstantBuffer();
                cb.BufferType = (Spark.Graphics.D3DConstantBufferType)reflectedBuffer.BufferType;
                cb.Flags = (Spark.Graphics.D3DShaderVariableFlags)reflectedBuffer.Flags;
                cb.Name = reflectedBuffer.Name;
                cb.SizeInBytes = reflectedBuffer.SizeInBytes;

                cb.Variables = new EffectData.ValueVariable[reflectedBuffer.Variables.Length];
                for (int i = 0; i < cb.Variables.Length; i++)
                {
                    cb.Variables[i] = CreateValueVariable(reflectedBuffer.Variables[i]);
                }

                return cb;
            }

            private EffectData.ValueVariable CreateValueVariable(D3DShaderVariable reflectedVariable)
            {
                var variable = new EffectData.ValueVariable();
                variable.Name = reflectedVariable.Name;
                variable.SizeInBytes = reflectedVariable.SizeInBytes;
                variable.AlignedElementSizeInBytes = reflectedVariable.AlignedElementSizeInBytes;
                variable.StartOffset = reflectedVariable.StartOffset;
                variable.VariableClass = (Spark.Graphics.D3DShaderVariableClass)reflectedVariable.VariableClass;
                variable.VariableType = (Spark.Graphics.D3DShaderVariableType)reflectedVariable.VariableType;
                variable.ColumnCount = reflectedVariable.ColumnCount;
                variable.RowCount = reflectedVariable.RowCount;
                variable.Flags = (Spark.Graphics.D3DShaderVariableFlags)reflectedVariable.Flags;
                variable.StartSampler = reflectedVariable.StartSampler;
                variable.SamplerSize = reflectedVariable.SamplerSize;
                variable.StartTexture = reflectedVariable.StartTexture;
                variable.TextureSize = reflectedVariable.TextureSize;
                variable.DefaultValue = reflectedVariable.DefaultValue;
                variable.ElementCount = reflectedVariable.ElementCount;

                variable.Members = new EffectData.ValueVariableMember[reflectedVariable.Members.Length];
                for (int i = 0; i < variable.Members.Length; i++)
                {
                    variable.Members[i] = CreateValueVariableMember(reflectedVariable.Members[i]);
                }

                return variable;
            }

            private EffectData.ValueVariableMember CreateValueVariableMember(D3DShaderVariableMember reflectedMember)
            {
                var member = new EffectData.ValueVariableMember();
                member.Name = reflectedMember.Name;
                member.VariableType = (Spark.Graphics.D3DShaderVariableType)reflectedMember.VariableType;
                member.VariableClass = (Spark.Graphics.D3DShaderVariableClass)reflectedMember.Class;
                member.SizeInBytes = reflectedMember.SizeInBytes;
                member.AlignedElementSizeInBytes = reflectedMember.AlignedElementSizeInBytes;
                member.RowCount = reflectedMember.RowCount;
                member.ColumnCount = reflectedMember.ColumnCount;
                member.ElementCount = reflectedMember.ElementCount;
                member.OffsetFromParentStructure = reflectedMember.OffsetFromParent;

                member.Members = new EffectData.ValueVariableMember[reflectedMember.Members.Length];
                for (int i = 0; i < member.Members.Length; i++)
                {
                    member.Members[i] = CreateValueVariableMember(reflectedMember.Members[i]);
                }

                return member;
            }

            #endregion

            #region Sampler Parsing

            private class SamplerDataParser
            {
                private StringTokenizer _tokenizer;
                private readonly char[] _delims;
                
                public SamplerDataParser()
                {
                    _tokenizer = new StringTokenizer();
                    _delims = new char[] { 'f', ';' };
                }

                public void ParseAllSamplerStates(string shaderSourceCode, IIncludeHandler includeHandler, Dictionary<String, EffectData.SamplerStateData> samplerStates)
                {
                    _tokenizer.Initialize(shaderSourceCode);

                    while (_tokenizer.HasNext())
                    {
                        string nextToken = _tokenizer.NextToken();
                        if (StringCompare(nextToken, "#include"))
                        {
                            string filePath = SanitizeIncludePath(_tokenizer.NextToken());
                            ParseIncludeHeader(filePath, includeHandler, samplerStates);
                            continue;
                        }

                        if (StringCompare(nextToken, "SamplerState") ||
                            StringCompare(nextToken, "sampler1D") ||
                            StringCompare(nextToken, "sampler2D") ||
                            StringCompare(nextToken, "sampler3D") ||
                            StringCompare(nextToken, "samplerCUBE") ||
                            StringCompare(nextToken, "sampler") ||
                            StringCompare(nextToken, "sampler_state"))
                        {
                            string samplerName = _tokenizer.NextToken();
                            if (samplerStates.ContainsKey(samplerName))
                            {
                                continue;
                            }

                            nextToken = _tokenizer.NextToken();

                            // Sometimes the sampler is set to a register...skip over it
                            if (StringCompare(nextToken, ":"))
                            {
                                _tokenizer.NextToken(); // Register
                                nextToken = _tokenizer.NextToken(); // Should be opening brace
                            }

                            if (StringCompare(nextToken, "{"))
                            {
                                // Create a sampler only when we really have a bunch of properties to set. If just declared, then we should be using the default sampler.
                                var data = new EffectData.SamplerStateData();

                                // Default values...
                                data.AddressU = TextureAddressMode.Clamp;
                                data.AddressV = TextureAddressMode.Clamp;
                                data.AddressW = TextureAddressMode.Clamp;
                                data.BorderColor = Color.White;
                                data.ComparisonFunction = ComparisonFunction.Never;
                                data.Filter = TextureFilter.Linear;
                                data.MaxAnisotropy = 1;
                                data.MipMapLevelOfDetailBias = 0.0f;
                                data.MinMipMapLevel = 0;
                                data.MaxMipMapLevel = int.MaxValue;

                                nextToken = _tokenizer.NextToken();

                                while (!StringCompare(nextToken, "}") || !nextToken.EndsWith("}"))
                                {
                                    if (StringCompare(nextToken, "AddressU"))
                                    {
                                        data.AddressU = ParseAddressMode();
                                    }
                                    else if (StringCompare(nextToken, "AddressV"))
                                    {
                                        data.AddressV = ParseAddressMode();
                                    }
                                    else if (StringCompare(nextToken, "AddressW"))
                                    {
                                        data.AddressW = ParseAddressMode();
                                    }
                                    else if (StringCompare(nextToken, "BorderColor"))
                                    {
                                        data.BorderColor = ParseBorderColor();
                                    }
                                    else if (StringCompare(nextToken, "Filter"))
                                    {
                                        data.Filter = ParseFilter();
                                    }
                                    else if (StringCompare(nextToken, "MaxAnisotropy"))
                                    {
                                        data.MaxAnisotropy = ParseInt(1);
                                    }
                                    else if (StringCompare(nextToken, "MaxLOD"))
                                    {
                                        float val = ParseFloat(float.MaxValue);
                                        if (MathHelper.IsApproxEquals(val, float.MaxValue))
                                        {
                                            data.MaxMipMapLevel = int.MaxValue;
                                        }
                                        else
                                        {
                                            data.MaxMipMapLevel = (int)val;
                                        }
                                    }
                                    else if (StringCompare(nextToken, "MinLOD"))
                                    {
                                        data.MinMipMapLevel = (int)ParseFloat(0.0f);
                                    }
                                    else if (StringCompare(nextToken, "MipLODBias"))
                                    {
                                        data.MipMapLevelOfDetailBias = ParseFloat(0.0f);
                                    }
                                    else if (StringCompare(nextToken, "ComparisonFunc"))
                                    {
                                        data.ComparisonFunction = ParseComparisonFunction();
                                    }

                                    // Unexpected end of file
                                    if (!_tokenizer.HasNext())
                                    {
                                        return;
                                    }

                                    nextToken = _tokenizer.NextToken();
                                }

                                samplerStates.Add(samplerName, data);
                            }
                        }
                    }
                }

                private void ParseIncludeHeader(string includePath, IIncludeHandler includeHandler, Dictionary<String, EffectData.SamplerStateData> samplerStates)
                {
                    if (String.IsNullOrEmpty(includePath) || includeHandler == null)
                    {
                        return;
                    }

                    Stream stream = includeHandler.Open(IncludeType.Local, includePath, null);
                    if (stream == null)
                    {
                        return;
                    }

                    string headerCode = null;
                    var reader = new StreamReader(stream);
                    try
                    {
                        headerCode = reader.ReadToEnd();
                    }
                    finally
                    {
                        reader.Dispose();
                        includeHandler.Close(stream);
                    }

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        return;
                    }

                    StringTokenizer oldTokenizer = _tokenizer;
                    _tokenizer = new StringTokenizer();

                    ParseAllSamplerStates(headerCode, includeHandler, samplerStates);

                    _tokenizer = oldTokenizer;
                }

                private int ParseInt(int defaultVal)
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        return _tokenizer.NextInt(); //Handles the ";" at the end.
                    }

                    return defaultVal;
                }

                private float ParseFloat(float defaultVal)
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        return _tokenizer.NextSingle(); //handles the ";" at the end.
                    }

                    return defaultVal;
                }

                private TextureAddressMode ParseAddressMode()
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        string addressMode = _tokenizer.NextToken();
                        if (StringCompare(addressMode, "Wrap"))
                        {
                            return TextureAddressMode.Wrap;
                        }
                        else if (StringCompare(addressMode, "Mirror"))
                        {
                            return TextureAddressMode.Border;
                        }
                        else if (StringCompare(addressMode, "Clamp"))
                        {
                            return TextureAddressMode.Clamp;
                        }
                        else if (StringCompare(addressMode, "Border"))
                        {
                            return TextureAddressMode.Mirror;
                        }
                        else if (StringCompare(addressMode, "Mirror_Once"))
                        {
                            return TextureAddressMode.MirrorOnce;
                        }
                    }

                    return TextureAddressMode.Clamp;
                }

                private ComparisonFunction ParseComparisonFunction()
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        string compFunc = _tokenizer.NextToken();
                        if (StringCompare(compFunc, "NEVER"))
                        {
                            return ComparisonFunction.Never;
                        }
                        else if (StringCompare(compFunc, "LESS"))
                        {
                            return ComparisonFunction.Less;
                        }
                        else if (StringCompare(compFunc, "EQUAL"))
                        {
                            return ComparisonFunction.Equal;
                        }
                        else if (StringCompare(compFunc, "LESS_EQUAL"))
                        {
                            return ComparisonFunction.LessEqual;
                        }
                        else if (StringCompare(compFunc, "GREATER"))
                        {
                            return ComparisonFunction.Greater;
                        }
                        else if (StringCompare(compFunc, "NOT_EQUAL"))
                        {
                            return ComparisonFunction.NotEqual;
                        }
                        else if (StringCompare(compFunc, "GREATER_EQUAL"))
                        {
                            return ComparisonFunction.GreaterEqual;
                        }
                        else if (StringCompare(compFunc, "ALWAYS"))
                        {
                            return ComparisonFunction.Always;
                        }
                    }

                    return ComparisonFunction.Never;
                }

                private TextureFilter ParseFilter()
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        string filter = _tokenizer.NextToken();
                        if (StringCompare(filter, "MIN_MAG_MIP_POINT"))
                        {
                            return TextureFilter.Point;
                        }
                        else if (StringCompare(filter, "MIN_MAG_POINT_MIP_LINEAR"))
                        {
                            return TextureFilter.PointMipLinear;
                        }
                        else if (StringCompare(filter, "MIN_POINT_MAG_LINEAR_MIP_POINT"))
                        {
                            return TextureFilter.MinPointMagLinearMipPoint;
                        }
                        else if (StringCompare(filter, "MIN_POINT_MAG_MIP_LINEAR"))
                        {
                            return TextureFilter.MinPointMagLinearMipLinear;
                        }
                        else if (StringCompare(filter, "MIN_LINEAR_MAG_MIP_POINT"))
                        {
                            return TextureFilter.MinLinearMagPointMipPoint;
                        }
                        else if (StringCompare(filter, "MIN_LINEAR_MAG_POINT_MIP_LINEAR"))
                        {
                            return TextureFilter.MinLinearMagPointMipLinear;
                        }
                        else if (StringCompare(filter, "MIN_MAG_LINEAR_MIP_POINT"))
                        {
                            return TextureFilter.LinearMipPoint;
                        }
                        else if (StringCompare(filter, "MIN_MAG_MIP_LINEAR"))
                        {
                            return TextureFilter.Linear;
                        }
                        else if (StringCompare(filter, "ANISOTROPIC"))
                        {
                            return TextureFilter.Anisotropic;
                        }
                    }

                    return TextureFilter.Linear;
                }

                private Color ParseBorderColor()
                {
                    string nextToken = _tokenizer.NextToken();
                    if (StringCompare(nextToken, "="))
                    {
                        var builder = new StringBuilder();
                        nextToken = _tokenizer.NextToken();
                        builder.Append(nextToken);
                        nextToken = _tokenizer.NextToken();

                        while (!nextToken.EndsWith(")"))
                        {
                            builder.Append(nextToken);

                            // Unexpected end of file
                            if (!_tokenizer.HasNext())
                            {
                                return Color.White;
                            }

                            nextToken = _tokenizer.NextToken();
                        }

                        // Append last token since it's still part of float4(..)
                        builder.Append(nextToken);

                        // Builder now contains "float4(x, y, z, w)"
                        GetFloatValues(builder.ToString(), out float x, out float y, out float z, out float w);
                        return new Color(x, y, z, w);
                    }

                    return Color.White;
                }

                private void GetFloatValues(string text, out float x, out float y, out float z, out float w)
                {
                    // Sometime later clean this up to use Regex.
                    if (text.StartsWith("float4("))
                    {
                        text = text.Substring(7);
                    }

                    text = text.TrimEnd(')');

                    string[] splits = text.Split(',');

                    x = 0.0f;
                    y = 0.0f;
                    z = 0.0f;
                    w = 0.0f;

                    if (splits.Length > 0)
                    {
                        x = CleanFloat(splits[0]);
                    }

                    if (splits.Length > 1)
                    {
                        y = CleanFloat(splits[1]);
                    }

                    if (splits.Length > 2)
                    {
                        z = CleanFloat(splits[2]);
                    }

                    if (splits.Length > 3)
                    {
                        w = CleanFloat(splits[3]);
                    }
                }

                private float CleanFloat(string value)
                {
                    if (float.TryParse(value.TrimEnd(_delims), out float floatVal))
                    {
                        return floatVal;
                    }

                    return 0.0f;
                }

                private static bool StringCompare(string left, string right)
                {
                    return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
                }

                private static string SanitizeIncludePath(string value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }

                    value = value.Replace(@"""", String.Empty);
                    value = value.Replace("<", String.Empty);
                    value = value.Replace(">", String.Empty);

                    return value;
                }
            }

            #endregion
        }

        #endregion
    }
}
