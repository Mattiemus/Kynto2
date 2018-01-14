namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark;

    using D3D = SharpDX.D3DCompiler;

    public class D3DShaderVariable
    {
        internal D3DShaderVariable(D3D.ShaderReflectionVariable variable)
        {
            D3D.ShaderVariableDescription desc = variable.Description;
            D3D.ShaderReflectionType type = variable.GetVariableType();
            D3D.ShaderTypeDescription typeDesc = type.Description;

            Name = desc.Name;
            SizeInBytes = desc.Size;
            StartOffset = desc.StartOffset;
            Flags = (D3DShaderVariableFlags)desc.Flags;
            StartSampler = desc.StartSampler;
            SamplerSize = desc.SamplerSize;
            StartTexture = desc.StartTexture;
            TextureSize = desc.TextureSize;

            DefaultValue = new byte[SizeInBytes];

            if (desc.DefaultValue != IntPtr.Zero)
            {
                MemoryHelper.Read(desc.DefaultValue, DefaultValue, 0, DefaultValue.Length);
            }

            VariableClass = (D3DShaderVariableClass)typeDesc.Class;
            ColumnCount = typeDesc.ColumnCount;
            RowCount = typeDesc.RowCount;
            VariableType = (D3DShaderVariableType)typeDesc.Type;
            ElementCount = typeDesc.ElementCount;

            AlignedElementSizeInBytes = CalculateAlignedElementSize(SizeInBytes, ElementCount);

            Members = new D3DShaderVariableMember[typeDesc.MemberCount];

            for (int i = 0; i < Members.Length; i++)
            {
                using (D3D.ShaderReflectionType memberType = type.GetMemberType(i))
                {
                    string memberName = type.GetMemberTypeName(i);
                    Members[i] = new D3DShaderVariableMember(memberName, memberType);
                }
            }

            type.Dispose();
        }

        public string Name { get; }

        public int SizeInBytes { get; }

        public int AlignedElementSizeInBytes { get; }

        public int StartOffset { get; }

        public D3DShaderVariableFlags Flags { get; }

        public int StartSampler { get; }

        public int SamplerSize { get; }

        public int StartTexture { get; }

        public int TextureSize { get; }

        public byte[] DefaultValue { get; }

        public D3DShaderVariableClass VariableClass { get; }

        public int ColumnCount { get; }

        public int RowCount { get; }

        public D3DShaderVariableType VariableType { get; }

        public int ElementCount { get; }

        public D3DShaderVariableMember[] Members { get; }

        internal bool AreSame(D3DShaderVariable variable, out string reasonWhyNotSame)
        {
            if (!Name.Equals(variable) || 
                SizeInBytes != variable.SizeInBytes || 
                StartOffset != variable.StartOffset || 
                Flags != variable.Flags || 
                StartSampler != variable.StartSampler || 
                SamplerSize != variable.SamplerSize || 
                StartTexture != variable.StartTexture || 
                TextureSize != variable.TextureSize)
            {
                reasonWhyNotSame = "Description not the same";
                return false;
            }

            if (VariableClass != variable.VariableClass || 
                ColumnCount != variable.ColumnCount ||
                RowCount != variable.RowCount ||
                ElementCount != variable.ElementCount)
            {
                reasonWhyNotSame = "Type not the same";
                return false;
            }

            D3DShaderVariableMember[] members = variable.Members;

            if (Members.Length != members.Length)
            {
                reasonWhyNotSame = "Member count not the same";
                return false;
            }

            for (int i = 0; i < Members.Length; i++)
            {
                if (Members[i].AreSame(members[i], out reasonWhyNotSame))
                {
                    reasonWhyNotSame = string.Format("Member {0} not the same:\n{1}", i.ToString(), reasonWhyNotSame);
                    return false;
                }
            }

            reasonWhyNotSame = "";
            return true;
        }

        internal static int CalculateAlignedElementSize(int sizeInBytes, int elementCount)
        {
            int sizePerElement = (elementCount > 0) ? sizeInBytes / elementCount : sizeInBytes;

            bool needsAlignment = (sizePerElement & 0xF) != 0;
            if (needsAlignment)
            {
                sizePerElement = ((sizePerElement >> 4) + 1) << 4;
            }

            return sizePerElement;
        }
    }
}
