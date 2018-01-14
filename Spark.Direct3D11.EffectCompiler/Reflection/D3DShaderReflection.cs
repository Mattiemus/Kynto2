namespace Spark.Direct3D11.Graphics
{
    using Spark.Graphics;

    using D3D = SharpDX.D3DCompiler;

    public sealed class D3DShaderReflection
    {
        private int _hashCode;

        public D3DShaderReflection(D3D.ShaderBytecode byteCode)
        {
            var reflection = new D3D.ShaderReflection(byteCode);

            Initialize(reflection);

            CompiledByteCode = byteCode.Data;
            InputSignatureByteCode = byteCode.GetPart(D3D.ShaderBytecodePart.InputSignatureBlob);
            OutputSignatureByteCode = byteCode.GetPart(D3D.ShaderBytecodePart.OutputSignatureBlob);

            _hashCode = MemoryHelper.ComputeFNVModifiedHashCode(byteCode.Data);
            InputSignatureByteCodeHash = MemoryHelper.ComputeFNVModifiedHashCode(InputSignatureByteCode);
            OutputSignatureByteCodeHash = MemoryHelper.ComputeFNVModifiedHashCode(OutputSignatureByteCode);

            reflection.Dispose();
        }

        public D3DShaderReflection(byte[] byteCode)
        {
            var shaderByteCode = new D3D.ShaderBytecode(byteCode);
            var reflection = new D3D.ShaderReflection(shaderByteCode);

            Initialize(reflection);

            CompiledByteCode = byteCode;
            InputSignatureByteCode = shaderByteCode.GetPart(D3D.ShaderBytecodePart.InputSignatureBlob);
            OutputSignatureByteCode = shaderByteCode.GetPart(D3D.ShaderBytecodePart.OutputSignatureBlob);

            _hashCode = MemoryHelper.ComputeFNVModifiedHashCode(byteCode);
            InputSignatureByteCodeHash = MemoryHelper.ComputeFNVModifiedHashCode(InputSignatureByteCode);
            OutputSignatureByteCodeHash = MemoryHelper.ComputeFNVModifiedHashCode(OutputSignatureByteCode);

            shaderByteCode.Dispose();
            reflection.Dispose();
        }

        public D3DConstantBuffer[] ConstantBuffers { get; private set; }

        public D3DResourceBinding[] BoundResources { get; private set; }

        public D3DShaderParameter[] InputSignature { get; private set; }

        public D3DShaderParameter[] OutputSignature { get; private set; }

        public D3DShaderParameter[] PatchConstantSignature { get; private set; }

        public int BitwiseInstructionCount { get; private set; }

        public int ConditionalMoveInstructionCount { get; private set; }

        public int ConversionInstructionCount { get; private set; }

        public D3DInputPrimitive GeometryShaderInputPrimitive { get; private set; }

        public bool IsSampleFrequencyShader { get; private set; }

        public int MoveInstructionCount { get; private set; }

        public int ShaderVersion { get; private set; }

        public int ArrayInstructionCount { get; private set; }

        public int BarrierInstructionCount { get; private set; }

        public int ControlPointCount { get; private set; }

        public string Creator { get; private set; }

        public int CutInstructionCount { get; private set; }

        public int DeclarationCount { get; private set; }

        public int DefineCount { get; private set; }

        public int DynamicFlowControlInstructionCount { get; private set; }

        public int EmitInstructionCount { get; private set; }

        public ShaderCompileFlags ShaderFlags { get; private set; }

        public int FloatInstructionCount { get; private set; }

        public int GeometryShaderInstanceCount { get; private set; }

        public D3DPrimitiveTopology GeometryShaderOutputTopology { get; private set; }

        public D3DTessellatorOutputPrimitive HullShaderOutputPrimitive { get; private set; }

        public D3DTessellatorPartitioning HullShaderPartitioning { get; private set; }

        public D3DInputPrimitive GeometryOrHullShaderInputPrimitive { get; private set; }

        public int InstructionCount { get; private set; }

        public int InterlockedInstructionCount { get; private set; }

        public int IntegerInstructionCount { get; private set; }

        public int MacroInstructionCount { get; private set; }

        public int MaxGeometryShaderOutputVertexCount { get; private set; }

        public int StaticFlowControlInstructionCount { get; private set; }

        public int TempArrayCount { get; private set; }

        public int TempRegisterCount { get; private set; }

        public D3DTessellatorDomain TessellatorDomain { get; private set; }

        public int TextureBiasInstructionCount { get; private set; }

        public int TextureCompareInstructionCount { get; private set; }

        public int TextureGradientInstructionCount { get; private set; }

        public int TextureLoadInstructionCount { get; private set; }

        public int TextureNormalInstructionCount { get; private set; }

        public int TextureStoreInstructionCount { get; private set; }

        public int UnsignedIntegerInstructionCount { get; private set; }

        public D3D11FeatureLevel MinFeatureLevel { get; private set; }

        public byte[] CompiledByteCode { get; private set; }

        public byte[] InputSignatureByteCode { get; private set; }

        public byte[] OutputSignatureByteCode { get; private set; }

        public int CompiledByteCodeHash { get; private set; }

        public int InputSignatureByteCodeHash { get; private set; }

        public int OutputSignatureByteCodeHash { get; private set; }

        public D3DConstantBuffer GetConstantBufferByName(string name)
        {
            foreach (D3DConstantBuffer cb in ConstantBuffers)
            {
                if (cb.Name.Equals(name))
                {
                    return cb;
                }
            }

            return null;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool AreSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            if (_hashCode != shader._hashCode)
            {
                reasonWhyNotSame = "Bytecode hash not the same";
                return false;
            }

            if (ShaderVersion != shader.ShaderVersion)
            {
                reasonWhyNotSame = "Shader version is not the same";
                return false;
            }

            if (BitwiseInstructionCount != shader.BitwiseInstructionCount ||
                ConditionalMoveInstructionCount != shader.ConditionalMoveInstructionCount ||
                ConversionInstructionCount != shader.ConversionInstructionCount ||
                GeometryShaderInputPrimitive != shader.GeometryShaderInputPrimitive ||
                IsSampleFrequencyShader != shader.IsSampleFrequencyShader ||
                MoveInstructionCount != shader.MoveInstructionCount ||
                ArrayInstructionCount != shader.ArrayInstructionCount ||
                BarrierInstructionCount != shader.BarrierInstructionCount ||
                DeclarationCount != shader.DeclarationCount ||
                DefineCount != shader.DefineCount ||
                DynamicFlowControlInstructionCount != shader.DynamicFlowControlInstructionCount ||
                EmitInstructionCount != shader.EmitInstructionCount ||
                ShaderFlags != shader.ShaderFlags ||
                FloatInstructionCount != shader.FloatInstructionCount ||
                GeometryShaderInstanceCount != shader.GeometryShaderInstanceCount ||
                GeometryShaderOutputTopology != shader.GeometryShaderOutputTopology ||
                HullShaderOutputPrimitive != shader.HullShaderOutputPrimitive ||
                HullShaderPartitioning != shader.HullShaderPartitioning ||
                GeometryOrHullShaderInputPrimitive != shader.GeometryOrHullShaderInputPrimitive ||
                InstructionCount != shader.InstructionCount ||
                InterlockedInstructionCount != shader.InterlockedInstructionCount ||
                IntegerInstructionCount != shader.IntegerInstructionCount ||
                MacroInstructionCount != shader.MacroInstructionCount ||
                MaxGeometryShaderOutputVertexCount != shader.MaxGeometryShaderOutputVertexCount ||
                StaticFlowControlInstructionCount != shader.StaticFlowControlInstructionCount ||
                TempArrayCount != shader.TempArrayCount ||
                TempRegisterCount != shader.TempRegisterCount ||
                TessellatorDomain != shader.TessellatorDomain ||
                TextureBiasInstructionCount != shader.TextureBiasInstructionCount ||
                TextureCompareInstructionCount != shader.TextureCompareInstructionCount ||
                TextureGradientInstructionCount != shader.TextureGradientInstructionCount ||
                TextureLoadInstructionCount != shader.TextureLoadInstructionCount ||
                TextureNormalInstructionCount != shader.TextureNormalInstructionCount ||
                TextureStoreInstructionCount != shader.TextureStoreInstructionCount ||
                UnsignedIntegerInstructionCount != shader.UnsignedIntegerInstructionCount ||
                MinFeatureLevel != shader.MinFeatureLevel ||
                !Creator.Equals(shader.Creator))
            {
                reasonWhyNotSame = "Description not the same";
                return false;
            }

            if (!AreInputSignatureSame(shader, out reasonWhyNotSame) || 
                !AreOutputSignatureSame(shader, out reasonWhyNotSame) || 
                !AreBoundResourcesSame(shader, out reasonWhyNotSame) || 
                !AreConstantBuffersSame(shader, out reasonWhyNotSame))
            {
                return false;
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private bool AreInputSignatureSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            D3DShaderParameter[] inputSig = shader.InputSignature;

            if (InputSignature.Length != inputSig.Length)
            {
                reasonWhyNotSame = "Input signature not the same count";
                return false;
            }

            for (int i = 0; i < InputSignature.Length; i++)
            {
                if (!InputSignature[i].AreSame(inputSig[i]))
                {
                    reasonWhyNotSame = string.Format("Input Parameter {0} not the same", i.ToString());
                    return false;
                }
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private bool AreOutputSignatureSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            D3DShaderParameter[] outputSig = shader.OutputSignature;

            if (OutputSignature.Length != outputSig.Length)
            {
                reasonWhyNotSame = "Output signature not the same count";
                return false;
            }

            for (int i = 0; i < OutputSignature.Length; i++)
            {
                if (!OutputSignature[i].AreSame(outputSig[i]))
                {
                    reasonWhyNotSame = string.Format("Output Parameter {0} not the same", i.ToString());
                    return false;
                }
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private bool ArePatchSignatureSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            D3DShaderParameter[] patchSig = shader.PatchConstantSignature;

            if (PatchConstantSignature.Length != patchSig.Length)
            {
                reasonWhyNotSame = "Patch Constant signature not the same count";
                return false;
            }

            for (int i = 0; i < PatchConstantSignature.Length; i++)
            {
                if (!PatchConstantSignature[i].AreSame(patchSig[i]))
                {
                    reasonWhyNotSame = string.Format("Patch Constant Parameter {0} not the same", i.ToString());
                    return false;
                }
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private bool AreBoundResourcesSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            D3DResourceBinding[] resources = shader.BoundResources;

            if (BoundResources.Length != resources.Length)
            {
                reasonWhyNotSame = "Bound resource count not the same";
                return false;
            }

            for (int i = 0; i < BoundResources.Length; i++)
            {
                if (!BoundResources[i].AreSame(resources[i]))
                {
                    reasonWhyNotSame = string.Format("Bound Resource {0} not the same", i.ToString());
                    return false;
                }
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private bool AreConstantBuffersSame(D3DShaderReflection shader, out string reasonWhyNotSame)
        {
            D3DConstantBuffer[] buffers = shader.ConstantBuffers;

            if (ConstantBuffers.Length != buffers.Length)
            {
                reasonWhyNotSame = "Constant buffer count not the same";
                return false;
            }

            for (int i = 0; i < ConstantBuffers.Length; i++)
            {
                if (!ConstantBuffers[i].AreSame(buffers[i], out reasonWhyNotSame))
                {
                    reasonWhyNotSame = string.Format("Constant Buffer {0} not the same:\n{1}", i.ToString(), reasonWhyNotSame);
                    return false;
                }
            }

            reasonWhyNotSame = string.Empty;
            return true;
        }

        private void Initialize(D3D.ShaderReflection reflection)
        {
            D3D.ShaderDescription desc = reflection.Description;

            BitwiseInstructionCount = reflection.BitwiseInstructionCount;
            ConditionalMoveInstructionCount = reflection.ConditionalMoveInstructionCount;
            ConversionInstructionCount = reflection.ConversionInstructionCount;
            GeometryShaderInputPrimitive = (D3DInputPrimitive)reflection.GeometryShaderSInputPrimitive;
            IsSampleFrequencyShader = reflection.IsSampleFrequencyShader;
            MoveInstructionCount = reflection.MoveInstructionCount;

            ArrayInstructionCount = desc.ArrayInstructionCount;
            BarrierInstructionCount = desc.BarrierInstructions;
            ControlPointCount = desc.ControlPoints;
            Creator = desc.Creator;
            CutInstructionCount = desc.CutInstructionCount;
            DeclarationCount = desc.DeclarationCount;
            DefineCount = desc.DefineCount;
            DynamicFlowControlInstructionCount = desc.DynamicFlowControlCount;
            EmitInstructionCount = desc.EmitInstructionCount;
            ShaderFlags = (ShaderCompileFlags)desc.Flags;
            FloatInstructionCount = desc.FloatInstructionCount;
            GeometryShaderInstanceCount = desc.GeometryShaderInstanceCount;
            GeometryShaderOutputTopology = (D3DPrimitiveTopology)desc.GeometryShaderOutputTopology;
            HullShaderOutputPrimitive = (D3DTessellatorOutputPrimitive)desc.HullShaderOutputPrimitive;
            HullShaderPartitioning = (D3DTessellatorPartitioning)desc.HullShaderPartitioning;
            GeometryOrHullShaderInputPrimitive = (D3DInputPrimitive)desc.InputPrimitive;
            InstructionCount = desc.InstructionCount;
            InterlockedInstructionCount = desc.InterlockedInstructions;
            IntegerInstructionCount = desc.IntInstructionCount;
            MacroInstructionCount = desc.MacroInstructionCount;
            MaxGeometryShaderOutputVertexCount = desc.GeometryShaderMaxOutputVertexCount;
            StaticFlowControlInstructionCount = desc.StaticFlowControlCount;
            TempArrayCount = desc.TempArrayCount;
            TempRegisterCount = desc.TempRegisterCount;
            TessellatorDomain = (D3DTessellatorDomain)desc.TessellatorDomain;
            TextureBiasInstructionCount = desc.TextureBiasInstructions;
            TextureCompareInstructionCount = desc.TextureCompInstructions;
            TextureGradientInstructionCount = desc.TextureGradientInstructions;
            TextureLoadInstructionCount = desc.TextureLoadInstructions;
            TextureNormalInstructionCount = desc.TextureNormalInstructions;
            TextureStoreInstructionCount = desc.TextureStoreInstructions;
            UnsignedIntegerInstructionCount = desc.UintInstructionCount;
            ShaderVersion = desc.Version;
            MinFeatureLevel = D3D11FeatureLevel.Level_11_0;

            ConstantBuffers = new D3DConstantBuffer[desc.ConstantBuffers];
            for (int i = 0; i < ConstantBuffers.Length; i++)
            {
                using (var cb = reflection.GetConstantBuffer(i))
                {
                    ConstantBuffers[i] = new D3DConstantBuffer(cb);
                }
            }

            BoundResources = new D3DResourceBinding[desc.BoundResources];
            for (int i = 0; i < BoundResources.Length; i++)
            {
                var resource = reflection.GetResourceBindingDescription(i);
                BoundResources[i] = new D3DResourceBinding(resource);
            }

            InputSignature = new D3DShaderParameter[desc.InputParameters];
            for (int i = 0; i < InputSignature.Length; i++)
            {
                var param = reflection.GetInputParameterDescription(i);
                InputSignature[i] = new D3DShaderParameter(param);
            }

            OutputSignature = new D3DShaderParameter[desc.OutputParameters];
            for (int i = 0; i < OutputSignature.Length; i++)
            {
                var param = reflection.GetOutputParameterDescription(i);
                OutputSignature[i] = new D3DShaderParameter(param);
            }

            PatchConstantSignature = new D3DShaderParameter[desc.PatchConstantParameters];
            for (int i = 0; i < PatchConstantSignature.Length; i++)
            {
                var param = reflection.GetPatchConstantParameterDescription(i);
                PatchConstantSignature[i] = new D3DShaderParameter(param);
            }
        }
    }
}
