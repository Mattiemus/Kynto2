namespace Spark.Direct3D11.Graphics
{
    using D3D = SharpDX.D3DCompiler;

    public sealed class D3DShaderVariableMember
    {
        internal D3DShaderVariableMember(string typeName, D3D.ShaderReflectionType memberType)
        {
            D3D.ShaderTypeDescription desc = memberType.Description;

            Name = typeName;
            Class = (D3DShaderVariableClass)desc.Class;
            ColumnCount = desc.ColumnCount;
            RowCount = desc.RowCount;
            OffsetFromParent = desc.Offset;
            VariableType = (D3DShaderVariableType)desc.Type;
            ElementCount = desc.ElementCount;

            ComputeSizeInBytes();

            Members = new D3DShaderVariableMember[desc.MemberCount];

            for (int i = 0; i < Members.Length; i++)
            {
                D3D.ShaderReflectionType memberMemberType = memberType.GetMemberType(i);
                string memberMemberName = memberType.GetMemberTypeName(i);
                Members[i] = new D3DShaderVariableMember(memberMemberName, memberMemberType);
                memberMemberType.Dispose();
            }
        }

        public string Name { get; }

        public D3DShaderVariableClass Class { get; }

        public int ColumnCount { get; }

        public int RowCount { get; }

        public int OffsetFromParent { get; }

        public int SizeInBytes { get; private set; }

        public int AlignedElementSizeInBytes { get; private set; }

        public D3DShaderVariableType VariableType { get; }

        public int ElementCount { get; }

        public D3DShaderVariableMember[] Members { get; }

        internal bool AreSame(D3DShaderVariableMember member, out string reasonWhyNotSame)
        {
            if (!Name.Equals(member.Name))
            {
                reasonWhyNotSame = "Type name not the same";
                return false;
            }

            if (Class != member.Class || 
                ColumnCount != member.ColumnCount || 
                RowCount != member.RowCount || 
                OffsetFromParent != member.OffsetFromParent || 
                SizeInBytes != member.SizeInBytes || 
                VariableType != member.VariableType || 
                ElementCount != member.ElementCount)
            {
                reasonWhyNotSame = "Type not the same";
                return false;
            }

            D3DShaderVariableMember[] members = member.Members;

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

        private void ComputeSizeInBytes()
        {
            SizeInBytes = (ColumnCount * RowCount) * GetSizeOfType();
            if (ElementCount > 0)
            {
                SizeInBytes *= ElementCount;
            }

            AlignedElementSizeInBytes = D3DShaderVariable.CalculateAlignedElementSize(SizeInBytes, ElementCount);
        }

        private int GetSizeOfType()
        {
            if (Class == D3DShaderVariableClass.Object ||
                Class == D3DShaderVariableClass.InterfaceClass ||
                Class == D3DShaderVariableClass.InterfacePointer)
            {
                return 0;
            }

            if (Class == D3DShaderVariableClass.Struct)
            {
                return 4;
            }

            switch (VariableType)
            {
                case D3DShaderVariableType.Bool:
                case D3DShaderVariableType.Int:
                case D3DShaderVariableType.Float:
                case D3DShaderVariableType.UInt:
                    return 4;

                case D3DShaderVariableType.UInt8:
                    return 2;

                case D3DShaderVariableType.Double:
                    return 8;
            }

            return 0;
        }
    }
}
