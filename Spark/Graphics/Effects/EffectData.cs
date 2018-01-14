namespace Spark.Graphics
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    using Math;
    using Content;

    /// <summary>
    /// Effect data object
    /// </summary>
    public sealed class EffectData
    {
        public const int RuntimeId = 1;
        public const int Version = 2;
        
        public ConstantBuffer[] ConstantBuffers { get; set; }

        public ResourceVariable[] ResourceVariables { get; set; }

        public ShaderGroup[] ShaderGroups { get; set; }

        public Shader[] Shaders { get; set; }

        public static EffectData Read(byte[] effectByteCode)
        {
            if (effectByteCode == null || effectByteCode.Length < EffectHeader.SizeInBytes)
            {
                return null;
            }

            using (var stream = new MemoryStream(effectByteCode))
            {
                return Read(stream);
            }
        }

        public static EffectData Read(Stream effectByteCode)
        {
            if (effectByteCode == null || !effectByteCode.CanRead)
            {
                return null;
            }

            EffectHeader? hasHeader = EffectHeader.ReadHeader(effectByteCode);

            if (hasHeader == null)
            {
                return null;
            }

            EffectHeader header = hasHeader.Value;

            if (header.RuntimeID != RuntimeId || header.DataSizeInBytes == 0)
            {
                return null;
            }

            EffectData effectData = null;
            using (var input = new BinaryReader(effectByteCode, Encoding.Default, true))
            {
                effectData = new EffectData();
                if (header.CompressionMode == EffectCompressionMode.GZip)
                {
                    using (var decompStream = new GZipStream(new MemoryStream(input.ReadBytes((int)header.DataSizeInBytes)), CompressionMode.Decompress))
                    {
                        using (var byteDataStream = new MemoryStream())
                        {
                            decompStream.CopyTo(byteDataStream);
                            byteDataStream.Position = 0;

                            using (var decompressedInput = new BinaryReader(byteDataStream))
                            {
                                effectData.Read(decompressedInput, header.Version);
                            }
                        }
                    }
                }
                else
                {
                    effectData.Read(input, header.Version);
                }
            }

            return effectData;
        }

        public static byte[] Write(EffectData effectData, EffectCompressionMode compressionMode = EffectCompressionMode.None)
        {
            if (effectData == null)
            {
                return null;
            }

            using (var memStream = new MemoryStream())
            {
                Write(effectData, memStream, compressionMode);
                return memStream.ToArray();
            }
        }

        public static bool Write(EffectData effectData, Stream output, EffectCompressionMode compressionMode = EffectCompressionMode.None)
        {
            if (effectData == null || output == null || !output.CanWrite)
            {
                return false;
            }

            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream, Encoding.Default, true))
                {
                    effectData.Write(writer);
                }

                // Write header
                var header = new EffectHeader(RuntimeId, Version, compressionMode, (uint)dataStream.Length);
                EffectHeader.WriteHeader(header, output);

                // Write data
                if (compressionMode == EffectCompressionMode.GZip)
                {
                    using (var compressedData = new MemoryStream())
                    {
                        using (var compressionStream = new GZipStream(compressedData, CompressionMode.Compress, true))
                        {
                            dataStream.WriteTo(compressionStream);
                        }

                        compressedData.WriteTo(output);
                    }
                }
                else
                {
                    dataStream.WriteTo(output);
                }
            }

            return true;
        }
        
        private void Read(BinaryReader input, int versionNum)
        {
            int cbCount = input.ReadInt32();
            ConstantBuffers = new ConstantBuffer[cbCount];

            for (int i = 0; i < cbCount; i++)
            {
                ConstantBuffers[i] = ConstantBuffer.Read(input, versionNum);
            }

            int resourceVariableCount = input.ReadInt32();
            ResourceVariables = new ResourceVariable[resourceVariableCount];

            for (int i = 0; i < resourceVariableCount; i++)
            {
                ResourceVariables[i] = ResourceVariable.Read(input, versionNum);
            }

            int shaderCount = input.ReadInt32();
            Shaders = new Shader[shaderCount];

            for (int i = 0; i < shaderCount; i++)
            {
                Shaders[i] = Shader.Read(input, versionNum);
            }

            int techniqueCount = input.ReadInt32();
            ShaderGroups = new ShaderGroup[techniqueCount];
            for (int i = 0; i < techniqueCount; i++)
            {
                ShaderGroups[i] = ShaderGroup.Read(input, versionNum);
            }
        }

        private void Write(BinaryWriter output)
        {
            if (ConstantBuffers == null)
            {
                output.Write(0);
            }
            else
            {
                output.Write(ConstantBuffers.Length);
                for (int i = 0; i < ConstantBuffers.Length; i++)
                {
                    ConstantBuffer.Write(ConstantBuffers[i], output);
                }
            }

            if (ResourceVariables == null)
            {
                output.Write(0);
            }
            else
            {
                output.Write(ResourceVariables.Length);
                for (int i = 0; i < ResourceVariables.Length; i++)
                {
                    ResourceVariable.Write(ResourceVariables[i], output);
                }
            }

            if (Shaders == null)
            {
                output.Write(0);
            }
            else
            {
                output.Write(Shaders.Length);
                for (int i = 0; i < Shaders.Length; i++)
                {
                    Shader.Write(Shaders[i], output);
                }
            }

            if (ShaderGroups == null)
            {
                output.Write(0);
            }
            else
            {
                output.Write(ShaderGroups.Length);
                for (int i = 0; i < ShaderGroups.Length; i++)
                {
                    ShaderGroup.Write(ShaderGroups[i], output);
                }
            }
        }

        public sealed class ShaderGroup
        {
            public string Name { get; set; }

            public int[] ShaderIndices { get; set; }

            public static ShaderGroup Read(BinaryReader input, int versionNum)
            {
                var shaderGroup = new ShaderGroup
                {
                    Name = input.ReadString()
                };

                int shaderIndicesCount = input.ReadInt32();
                shaderGroup.ShaderIndices = new int[shaderIndicesCount];

                for (int i = 0; i < shaderIndicesCount; i++)
                {
                    shaderGroup.ShaderIndices[i] = input.ReadInt32();
                }

                return shaderGroup;
            }

            public static void Write(ShaderGroup shaderGroup, BinaryWriter output)
            {
                output.Write(shaderGroup.Name ?? "ShaderGroup");

                if (shaderGroup.ShaderIndices == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(shaderGroup.ShaderIndices.Length);
                    for (int i = 0; i < shaderGroup.ShaderIndices.Length; i++)
                    {
                        output.Write(shaderGroup.ShaderIndices[i]);
                    }
                }
            }
        }

        public sealed class ConstantBuffer
        {
            public string Name { get; set; }

            public int SizeInBytes { get; set; }

            public D3DShaderVariableFlags Flags { get; set; }

            public D3DConstantBufferType BufferType { get; set; }

            public ValueVariable[] Variables { get; set; }

            public static ConstantBuffer Read(BinaryReader input, int versionNum)
            {
                var cb = new ConstantBuffer
                {
                    Name = input.ReadString(),
                    SizeInBytes = input.ReadInt32(),
                    Flags = (D3DShaderVariableFlags)input.ReadInt32(),
                    BufferType = (D3DConstantBufferType)input.ReadInt32()
                };

                int variableCount = input.ReadInt32();
                cb.Variables = new ValueVariable[variableCount];

                for (int i = 0; i < variableCount; i++)
                {
                    cb.Variables[i] = ValueVariable.Read(input, versionNum);
                }

                return cb;
            }

            public static void Write(ConstantBuffer buffer, BinaryWriter output)
            {
                output.Write(buffer.Name);
                output.Write(buffer.SizeInBytes);
                output.Write((int)buffer.Flags);
                output.Write((int)buffer.BufferType);

                if (buffer.Variables == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(buffer.Variables.Length);
                    for (int i = 0; i < buffer.Variables.Length; i++)
                    {
                        ValueVariable.Write(buffer.Variables[i], output);
                    }
                }
            }
        }

        public sealed class ResourceVariable
        {
            public string Name { get; set; }

            public D3DShaderInputType ResourceType { get; set; }

            public D3DResourceDimension ResourceDimension { get; set; }

            public D3DResourceReturnType ReturnType { get; set; }

            public D3DShaderInputFlags InputFlags { get; set; }

            public int SampleCount { get; set; }

            public int ElementCount { get; set; }

            public SamplerStateData SamplerData { get; set; }

            private bool IsSampler => ResourceType == D3DShaderInputType.Sampler;

            private bool HasSamplerData => SamplerData != null;

            public static ResourceVariable Read(BinaryReader input, int versionNum)
            {
                var variable = new ResourceVariable
                {
                    Name = input.ReadString(),
                    ResourceType = (D3DShaderInputType)input.ReadInt32(),
                    ResourceDimension = (D3DResourceDimension)input.ReadInt32(),
                    ReturnType = (D3DResourceReturnType)input.ReadInt32(),
                    InputFlags = (D3DShaderInputFlags)input.ReadInt32(),
                    SampleCount = input.ReadInt32(),
                    ElementCount = input.ReadInt32(),
                    SamplerData = null
                };

                if (variable.IsSampler && input.ReadBoolean())
                {
                    variable.SamplerData = SamplerStateData.Read(input);
                }

                return variable;
            }

            public static void Write(ResourceVariable variable, BinaryWriter output)
            {
                output.Write(variable.Name);
                output.Write((int)variable.ResourceType);
                output.Write((int)variable.ResourceDimension);
                output.Write((int)variable.ReturnType);
                output.Write((int)variable.InputFlags);
                output.Write(variable.SampleCount);
                output.Write(variable.ElementCount);

                if (variable.IsSampler)
                {
                    bool hasData = variable.HasSamplerData;
                    output.Write(hasData);

                    if (hasData)
                    {
                        SamplerStateData.Write(variable.SamplerData, output);
                    }
                }
            }
        }

        public sealed class ValueVariableMember
        {
            public string Name { get; set; }

            public D3DShaderVariableClass VariableClass { get; set; }

            public D3DShaderVariableType VariableType { get; set; }

            public int ColumnCount { get; set; }

            public int RowCount { get; set; }

            public int ElementCount { get; set; }

            public int OffsetFromParentStructure { get; set; }

            public int SizeInBytes { get; set; }

            public int AlignedElementSizeInBytes { get; set; }

            public ValueVariableMember[] Members { get; set; }

            public static ValueVariableMember Read(BinaryReader input, int versionNum)
            {
                var variable = new ValueVariableMember
                {
                    Name = input.ReadString(),
                    VariableClass = (D3DShaderVariableClass)input.ReadInt32(),
                    VariableType = (D3DShaderVariableType)input.ReadInt32(),
                    ColumnCount = input.ReadInt32(),
                    RowCount = input.ReadInt32(),
                    ElementCount = input.ReadInt32(),
                    OffsetFromParentStructure = input.ReadInt32(),
                    SizeInBytes = input.ReadInt32(),
                    AlignedElementSizeInBytes = input.ReadInt32()
                };

                int memberCount = input.ReadInt32();
                variable.Members = new ValueVariableMember[memberCount];

                for (int i = 0; i < memberCount; i++)
                {
                    variable.Members[i] = Read(input, versionNum);
                }

                return variable;
            }

            public static void Write(ValueVariableMember variable, BinaryWriter output)
            {
                output.Write(variable.Name);
                output.Write((int)variable.VariableClass);
                output.Write((int)variable.VariableType);
                output.Write(variable.ColumnCount);
                output.Write(variable.RowCount);
                output.Write(variable.ElementCount);
                output.Write(variable.OffsetFromParentStructure);
                output.Write(variable.SizeInBytes);
                output.Write(variable.AlignedElementSizeInBytes);

                if (variable.Members == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(variable.Members.Length);
                    for (int i = 0; i < variable.Members.Length; i++)
                    {
                        Write(variable.Members[i], output);
                    }
                }
            }
        }

        public sealed class ValueVariable
        {
            public string Name { get; set; }

            public int SizeInBytes { get; set; }

            public int AlignedElementSizeInBytes { get; set; }

            public int StartOffset { get; set; }

            public D3DShaderVariableClass VariableClass { get; set; }

            public D3DShaderVariableType VariableType { get; set; }

            public int ColumnCount { get; set; }

            public int RowCount { get; set; }

            public D3DShaderVariableFlags Flags { get; set; }

            public int StartSampler { get; set; }

            public int SamplerSize { get; set; }

            public int StartTexture { get; set; }

            public int TextureSize { get; set; }

            public byte[] DefaultValue { get; set; }

            public int ElementCount { get; set; }

            public ValueVariableMember[] Members { get; set; }

            public static ValueVariable Read(BinaryReader input, int versionNum)
            {
                var variable = new ValueVariable
                {
                    Name = input.ReadString(),
                    SizeInBytes = input.ReadInt32(),
                    AlignedElementSizeInBytes = input.ReadInt32(),
                    StartOffset = input.ReadInt32(),
                    VariableClass = (D3DShaderVariableClass)input.ReadInt32(),
                    VariableType = (D3DShaderVariableType)input.ReadInt32(),
                    ColumnCount = input.ReadInt32(),
                    RowCount = input.ReadInt32(),
                    Flags = (D3DShaderVariableFlags)input.ReadInt32(),
                    StartSampler = input.ReadInt32(),
                    SamplerSize = input.ReadInt32(),
                    StartTexture = input.ReadInt32(),
                    TextureSize = input.ReadInt32(),
                    DefaultValue = input.ReadBytes(input.ReadInt32()),
                    ElementCount = input.ReadInt32()
                };

                int memberCount = input.ReadInt32();
                variable.Members = new ValueVariableMember[memberCount];

                for (int i = 0; i < memberCount; i++)
                {
                    variable.Members[i] = ValueVariableMember.Read(input, versionNum);
                }

                return variable;
            }

            public static void Write(ValueVariable variable, BinaryWriter output)
            {
                output.Write(variable.Name);
                output.Write(variable.SizeInBytes);
                output.Write(variable.AlignedElementSizeInBytes);
                output.Write(variable.StartOffset);
                output.Write((int)variable.VariableClass);
                output.Write((int)variable.VariableType);
                output.Write(variable.ColumnCount);
                output.Write(variable.RowCount);
                output.Write((int)variable.Flags);
                output.Write(variable.StartSampler);
                output.Write(variable.SamplerSize);
                output.Write(variable.StartTexture);
                output.Write(variable.TextureSize);
                output.Write(variable.DefaultValue.Length);
                output.Write(variable.DefaultValue, 0, variable.DefaultValue.Length);
                output.Write(variable.ElementCount);

                if (variable.Members == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(variable.Members.Length);
                    for (int i = 0; i < variable.Members.Length; i++)
                    {
                        ValueVariableMember.Write(variable.Members[i], output);
                    }
                }
            }
        }

        public sealed class Shader
        {
            public ShaderStage ShaderType { get; set; }

            public String ShaderProfile { get; set; }

            public byte[] ShaderByteCode { get; set; }

            public int HashCode { get; set; }

            public D3DInputPrimitive GeometryShaderInputPrimitive { get; set; }

            public D3DPrimitiveTopology GeometryShaderOutputTopology { get; set; }

            public int GeometryShaderInstanceCount { get; set; }

            public int MaxGeometryShaderOutputVertexCount { get; set; }

            public D3DInputPrimitive GeometryOrHullShaderInputPrimitive { get; set; }

            public D3DTessellatorOutputPrimitive HullShaderOutputPrimitive { get; set; }

            public D3DTessellatorPartitioning HullShaderPartitioning { get; set; }

            public D3DTessellatorDomain TessellatorDomain { get; set; }

            public bool IsSampleFrequencyShader { get; set; }

            public ShaderCompileFlags ShaderFlags { get; set; }

            public Signature InputSignature { get; set; }

            public Signature OutputSignature { get; set; }

            public BoundResource[] BoundResources { get; set; }

            public static Shader Read(BinaryReader input, int versionNum)
            {
                var shader = new Shader
                {
                    ShaderType = (ShaderStage)input.ReadInt32(),
                    ShaderProfile = input.ReadString(),
                    ShaderByteCode = input.ReadBytes(input.ReadInt32()),
                    HashCode = input.ReadInt32(),
                    GeometryShaderInputPrimitive = (D3DInputPrimitive)input.ReadInt32(),
                    GeometryShaderOutputTopology = (D3DPrimitiveTopology)input.ReadInt32(),
                    GeometryShaderInstanceCount = input.ReadInt32(),
                    MaxGeometryShaderOutputVertexCount = input.ReadInt32(),
                    GeometryOrHullShaderInputPrimitive = (D3DInputPrimitive)input.ReadInt32(),
                    HullShaderOutputPrimitive = (D3DTessellatorOutputPrimitive)input.ReadInt32(),
                    HullShaderPartitioning = (D3DTessellatorPartitioning)input.ReadInt32(),
                    TessellatorDomain = (D3DTessellatorDomain)input.ReadInt32(),
                    IsSampleFrequencyShader = input.ReadBoolean(),
                    ShaderFlags = (ShaderCompileFlags)input.ReadInt32(),

                    InputSignature = Signature.Read(input, versionNum),
                    OutputSignature = Signature.Read(input, versionNum)
                };

                int boundResourceCount = input.ReadInt32();
                shader.BoundResources = new BoundResource[boundResourceCount];
                for (int i = 0; i < boundResourceCount; i++)
                {
                    shader.BoundResources[i] = BoundResource.Read(input);
                }

                return shader;
            }

            public static void Write(Shader shader, BinaryWriter output)
            {
                output.Write((int)shader.ShaderType);
                output.Write(shader.ShaderProfile);
                output.Write(shader.ShaderByteCode.Length);
                output.Write(shader.ShaderByteCode, 0, shader.ShaderByteCode.Length);
                output.Write(shader.HashCode);
                output.Write((int)shader.GeometryShaderInputPrimitive);
                output.Write((int)shader.GeometryShaderOutputTopology);
                output.Write(shader.GeometryShaderInstanceCount);
                output.Write(shader.MaxGeometryShaderOutputVertexCount);
                output.Write((int)shader.GeometryOrHullShaderInputPrimitive);
                output.Write((int)shader.HullShaderOutputPrimitive);
                output.Write((int)shader.HullShaderPartitioning);
                output.Write((int)shader.TessellatorDomain);
                output.Write(shader.IsSampleFrequencyShader);
                output.Write((int)shader.ShaderFlags);

                Signature.Write((shader.InputSignature == null) ? new Signature() : shader.InputSignature, output);
                Signature.Write((shader.OutputSignature == null) ? new Signature() : shader.OutputSignature, output);

                if (shader.BoundResources == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(shader.BoundResources.Length);
                    for (int i = 0; i < shader.BoundResources.Length; i++)
                    {
                        BoundResource.Write(shader.BoundResources[i], output);
                    }
                }
            }

            public override int GetHashCode()
            {
                return HashCode;
            }
        }

        public sealed class Signature
        {
            public Signature()
            {
                ByteCode = new byte[0];
                HashCode = 0;
                Parameters = new SignatureParameter[0];
            }

            public byte[] ByteCode { get; set; }

            public int HashCode { get; set; }

            public SignatureParameter[] Parameters { get; set; }

            public static Signature Read(BinaryReader input, int versionNum)
            {
                var signature = new Signature
                {
                    ByteCode = input.ReadBytes(input.ReadInt32()),
                    HashCode = input.ReadInt32()
                };

                int paramCount = input.ReadInt32();
                signature.Parameters = new SignatureParameter[paramCount];

                for (int i = 0; i < paramCount; i++)
                {
                    signature.Parameters[i] = SignatureParameter.Read(input);
                }

                return signature;
            }

            public static void Write(Signature signature, BinaryWriter output)
            {
                output.Write(signature.ByteCode.Length);
                output.Write(signature.ByteCode);
                output.Write(signature.HashCode);

                if (signature.Parameters == null)
                {
                    output.Write(0);
                }
                else
                {
                    output.Write(signature.Parameters.Length);
                    for (int i = 0; i < signature.Parameters.Length; i++)
                    {
                        SignatureParameter.Write(signature.Parameters[i], output);
                    }
                }
            }

            public override int GetHashCode()
            {
                return HashCode;
            }
        }

        public sealed class BoundResource
        {
            public D3DShaderInputType ResourceType { get; set; }

            public int ResourceIndex { get; set; }

            public int BindPoint { get; set; }

            public int BindCount { get; set; }

            public static BoundResource Read(BinaryReader input)
            {
                return new BoundResource
                {
                    ResourceType = (D3DShaderInputType)input.ReadInt32(),
                    ResourceIndex = input.ReadInt32(),
                    BindPoint = input.ReadInt32(),
                    BindCount = input.ReadInt32()
                };
            }

            public static void Write(BoundResource resource, BinaryWriter output)
            {
                output.Write((int)resource.ResourceType);
                output.Write(resource.ResourceIndex);
                output.Write(resource.BindPoint);
                output.Write(resource.BindCount);
            }
        }

        public sealed class SignatureParameter
        {
            public string SemanticName { get; set; }

            public int SemanticIndex { get; set; }

            public int StreamIndex { get; set; }

            public int Register { get; set; }

            public D3DComponentMaskFlags UsageMask { get; set; }

            public D3DComponentMaskFlags ReadWriteMask { get; set; }

            public D3DComponentType ComponentType { get; set; }

            public D3DSystemValueType SystemType { get; set; }

            public static SignatureParameter Read(BinaryReader input)
            {
                return new SignatureParameter
                {
                    SemanticName = input.ReadString(),
                    SemanticIndex = input.ReadInt32(),
                    StreamIndex = input.ReadInt32(),
                    Register = input.ReadInt32(),
                    UsageMask = (D3DComponentMaskFlags)input.ReadInt32(),
                    ReadWriteMask = (D3DComponentMaskFlags)input.ReadInt32(),
                    ComponentType = (D3DComponentType)input.ReadInt32(),
                    SystemType = (D3DSystemValueType)input.ReadInt32()
                };
            }

            public static void Write(SignatureParameter parameter, BinaryWriter output)
            {
                output.Write(parameter.SemanticName);
                output.Write(parameter.SemanticIndex);
                output.Write(parameter.StreamIndex);
                output.Write(parameter.Register);
                output.Write((int)parameter.UsageMask);
                output.Write((int)parameter.ReadWriteMask);
                output.Write((int)parameter.ComponentType);
                output.Write((int)parameter.SystemType);
            }
        }

        public sealed class SamplerStateData
        {
            public TextureAddressMode AddressU { get; set; }

            public TextureAddressMode AddressV { get; set; }

            public TextureAddressMode AddressW { get; set; }

            public ComparisonFunction ComparisonFunction { get; set; }

            public TextureFilter Filter { get; set; }

            public int MaxAnisotropy { get; set; }

            public float MipMapLevelOfDetailBias { get; set; }

            public int MinMipMapLevel { get; set; }

            public int MaxMipMapLevel { get; set; }

            public Color BorderColor { get; set; }

            public static SamplerStateData Read(BinaryReader input)
            {
                return new SamplerStateData
                {
                    AddressU = (TextureAddressMode)input.ReadByte(),
                    AddressV = (TextureAddressMode)input.ReadByte(),
                    AddressW = (TextureAddressMode)input.ReadByte(),
                    ComparisonFunction = (ComparisonFunction)input.ReadByte(),
                    Filter = (TextureFilter)input.ReadByte(),
                    MaxAnisotropy = input.ReadByte(),
                    MipMapLevelOfDetailBias = input.ReadSingle(),
                    MinMipMapLevel = input.ReadInt32(),
                    MaxMipMapLevel = input.ReadInt32(),
                    BorderColor = new Color(input.ReadUInt32())
                };
            }

            public static void Write(SamplerStateData ssData, BinaryWriter output)
            {
                output.Write((byte)ssData.AddressU);
                output.Write((byte)ssData.AddressV);
                output.Write((byte)ssData.AddressW);
                output.Write((byte)ssData.ComparisonFunction);
                output.Write((byte)ssData.Filter);
                output.Write((byte)ssData.MaxAnisotropy);
                output.Write(ssData.MipMapLevelOfDetailBias);
                output.Write(ssData.MinMipMapLevel);
                output.Write(ssData.MaxMipMapLevel);
                output.Write(ssData.BorderColor.PackedValue);
            }
        }
    }
















    public enum D3DShaderVariableClass
    {
        Scalar = 0,
        Vector = 1,
        MatrixRows = 2,
        MatrixColumns = 3,
        Object = 4,
        Struct = 5,
        InterfaceClass = 6,
        InterfacePointer = 7
    }

    public enum D3DShaderVariableType
    {
        AppendStructuredBuffer = 50,
        Blend = 24,
        Bool = 1,
        Buffer = 25,
        ByteaddressBuffer = 46,
        Computeshader = 38,
        Constantbuffer = 26,
        ConsumeStructuredBuffer = 51,
        Depthstencil = 23,
        Depthstencilview = 31,
        Domainshader = 36,
        Double = 39,
        Float = 3,
        Geometryshader = 21,
        Hullshader = 35,
        Int = 2,
        InterfacePointer = 37,
        Pixelfragment = 17,
        Pixelshade = 15,
        Rasterizer = 22,
        Rendertargetview = 30,
        RWBuffer = 45,
        RWByteAddressBuffer = 47,
        RwstructuredBuffer = 49,
        RWTexture1D = 40,
        RWTexture1DArray = 41,
        RWTexture2D = 42,
        RWTexture2DArray = 43,
        RWTexture3D = 44,
        Sampler = 10,
        Sampler1d = 11,
        Sampler2d = 12,
        Sampler3d = 13,
        Samplercube = 14,
        String = 4,
        StructuredBuffer = 48,
        Texture = 5,
        Texture1D = 6,
        Texture1DArray = 28,
        Texture2D = 7,
        Texture2DArray = 29,
        Texture2DMultisampled = 32,
        Texture2DMultisampledArray = 33,
        Texture3D = 8,
        TextureBuffer = 27,
        TextureCube = 9,
        TextureCubeArray = 34,
        UInt = 19,
        UInt8 = 20,
        Vertexfragment = 18,
        Vertexshader = 16,
        Void = 0
    }

    public enum D3DConstantBufferType
    {
        ConstantBuffer = 0,
        TextureBuffer = 1,
        InterfacePointers = 2,
        ResourceBindInformation = 3
    }

    [Flags]
    public enum D3DComponentMaskFlags
    {
        None = 0,
        ComponentX = 1,
        ComponentY = 2,
        ComponentZ = 4,
        ComponentW = 8,
        All = 15
    }

    public enum D3DComponentType
    {
        Unknown = 0,
        UInt32 = 1,
        SInt32 = 2,
        Float32 = 3
    }

    public enum D3DSystemValueType
    {
        ClipDistance = 2,
        Coverage = 66,
        CullDistance = 3,
        Depth = 65,
        DepthGreaterEqual = 67,
        DepthLessEqual = 68,
        FinalLineDensityTessFactor = 16,
        FinalLineDetailTessFactor = 15,
        FinalQuadEdgeTessFactor = 11,
        FinalQuadInsideTessFactor = 12,
        FinalTriangleEdgeTessFactor = 13,
        FinalTriangleInsideTessFactor = 14,
        InstanceId = 8,
        IsFrontFace = 9,
        Position = 1,
        PrimitiveId = 7,
        RenderTargetArrayIndex = 4,
        SampleIndex = 10,
        Target = 64,
        Undefined = 0,
        VertexId = 6,
        ViewportArrayIndex = 5,
    }

    public enum D3DResourceReturnType
    {
        UNorm = 1,
        SNorm = 2,
        SInt = 3,
        UInt = 4,
        Float = 5,
        Mixed = 6,
        Double = 7,
        Continued = 8
    }

    public enum D3DResourceDimension
    {
        Buffer = 1,
        Texture1D = 2,
        Texture1DArray = 3,
        Texture2D = 4,
        Texture2DArray = 5,
        Texture2DMultisampled = 6,
        Texture2DMultiSampledArray = 7,
        Texture3D = 8,
        TextureCube = 9,
        TextureCubeArray = 10,
        ExtendedBuffer = 11
    }

    [Flags]
    public enum D3DShaderInputFlags
    {
        None = 0,
        UserPacked = 1,
        ComparisonSampler = 2,
        TextureComponent0 = 4,
        TextureComponent1 = 8,
        Texturecomponents = 12
    }

    public enum D3DShaderInputType
    {
        ConstantBuffer = 0,
        TextureBuffer = 1,
        Texture = 2,
        Sampler = 3,
        RWTyped = 4,
        Structured = 5,
        RWStructured = 6,
        ByteAddress = 7,
        RWByteAddress = 8,
        AppendStructured = 9,
        ConsumeStructured = 10,
        RWStructuredWithCounter = 11
    }

    [Flags]
    public enum D3DShaderVariableFlags
    {
        None = 0,
        UserPacked = 1,
        Used = 2,
        InterfacePointer = 4,
        InterfaceParameter = 8
    }

    public enum D3DTessellatorDomain
    {
        Undefined = 0,
        Isoline = 1,
        Triangle = 2,
        Quad = 3
    }

    public enum D3DPrimitiveTopology
    {
        Undefined = 0,
        PointList = 1,
        LineList = 2,
        LineStrip = 3,
        TriangleList = 4,
        TriangleStrip = 5,
        LineListWithAdjacency = 10,
        LineStripWithAdjacency = 11,
        TriangleListWithAdjacency = 12,
        TriangleStripWithAdjacency = 13,
        PatchListWith1ControlPoint = 33,
        PatchListWith2ControlPoints = 34,
        PatchListWith3ControlPoints = 35,
        PatchListWith4ControlPoints = 36,
        PatchListWith5ControlPoints = 37,
        PatchListWith6ControlPoints = 38,
        PatchListWith7ControlPoints = 39,
        PatchListWith8ControlPoints = 40,
        PatchListWith9ControlPoints = 41,
        PatchListWith10ControlPoints = 42,
        PatchListWith11ControlPoints = 43,
        PatchListWith12ControlPoints = 44,
        PatchListWith13ControlPoints = 45,
        PatchListWith14ControlPoints = 46,
        PatchListWith15ControlPoints = 47,
        PatchListWith16ControlPoints = 48,
        PatchListWith17ControlPoints = 49,
        PatchListWith18ControlPoints = 50,
        PatchListWith19ControlPoints = 51,
        PatchListWith20ControlPoints = 52,
        PatchListWith21ControlPoints = 53,
        PatchListWith22ControlPoints = 54,
        PatchListWith23ControlPoints = 55,
        PatchListWith24ControlPoints = 56,
        PatchListWith25ControlPoints = 57,
        PatchListWith26ControlPoints = 58,
        PatchListWith27ControlPoints = 59,
        PatchListWith28ControlPoints = 60,
        PatchListWith29ControlPoints = 61,
        PatchListWith30ControlPoints = 62,
        PatchListWith31ControlPoints = 63,
        PatchListWith32ControlPoints = 64
    }

    public enum D3DInputPrimitive
    {
        Undefined = 0,
        Point = 1,
        Line = 2,
        Triangle = 3,
        LineWithAdjacency = 6,
        TriangleWithAdjacency = 7,
        PatchWith1ControlPoint = 8,
        PatchWith2ControlPoints = 9,
        PatchWith3ControlPoints = 10,
        PatchWith4ControlPoints = 11,
        PatchWith5ControlPoints = 12,
        PatchWith6ControlPoints = 13,
        PatchWith7ControlPoints = 14,
        PatchWith8ControlPoints = 15,
        PatchWith9ControlPoints = 16,
        PatchWith10ControlPoints = 17,
        PatchWith11ControlPoints = 18,
        PatchWith12ControlPoints = 19,
        PatchWith13ControlPoints = 20,
        PatchWith14ControlPoints = 21,
        PatchWith15ControlPoints = 22,
        PatchWith16ControlPoints = 23,
        PatchWith17ControlPoints = 24,
        PatchWith18ControlPoints = 25,
        PatchWith19ControlPoints = 26,
        PatchWith20ControlPoints = 28,
        PatchWith21ControlPoints = 29,
        PatchWith22ControlPoints = 30,
        PatchWith23ControlPoints = 31,
        PatchWith24ControlPoints = 32,
        PatchWith25ControlPoints = 33,
        PatchWith26ControlPoints = 34,
        PatchWith27ControlPoints = 35,
        PatchWith28ControlPoints = 36,
        PatchWith29ControlPoints = 37,
        PatchWith30ControlPoints = 38,
        PatchWith31ControlPoints = 39,
        PatchWith32ControlPoints = 40,
    }

    public enum D3DTessellatorOutputPrimitive
    {
        Undefined = 0,
        Point = 1,
        Line = 2,
        TriangleClockwise = 3,
        TriangleCounterClockwise = 4
    }

    public enum D3DTessellatorPartitioning
    {
        Undefined = 0,
        Integer = 1,
        PowerOfTwo = 2,
        FractionalOdd = 3,
        FractionalEven = 4
    }

    public enum D3DShaderVersion
    {
        PixelShader = 0,
        VertexShader = 1,
        GeometryShader = 2,
        HullShader = 3,
        DomainShader = 4,
        ComputeShader = 5
    }










}
