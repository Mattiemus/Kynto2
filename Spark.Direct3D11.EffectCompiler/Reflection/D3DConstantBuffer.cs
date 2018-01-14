namespace Spark.Direct3D11.Graphics
{
    using System;

    using D3D = SharpDX.D3DCompiler;

    public class D3DConstantBuffer
    {
        public D3DConstantBuffer(D3D.ConstantBuffer constantBuffer)
        {
            D3D.ConstantBufferDescription desc = constantBuffer.Description;

            Name = desc.Name;
            BufferType = (D3DConstantBufferType)desc.Type;
            Flags = (D3DShaderVariableFlags)desc.Flags;
            SizeInBytes = desc.Size;

            Variables = new D3DShaderVariable[desc.VariableCount];

            for (int i = 0; i < Variables.Length; i++)
            {
                using (D3D.ShaderReflectionVariable variable = constantBuffer.GetVariable(i))
                {
                    D3DShaderVariable myVariable = new D3DShaderVariable(variable);
                    Variables[i] = myVariable;
                }
            }
        }

        public string Name { get; }

        public int SizeInBytes { get; }

        public D3DShaderVariableFlags Flags { get; }

        public D3DConstantBufferType BufferType { get; }

        public D3DShaderVariable[] Variables { get; }

        public bool AreSame(D3DConstantBuffer constantBuffer, out string reasonWhyNotSame)
        {
            if (GetHashCode() != constantBuffer.GetHashCode())
            {
                reasonWhyNotSame = "Hashcodes not the same";
                return false;
            }

            if (!Name.Equals(constantBuffer.Name) ||
                BufferType != constantBuffer.BufferType || 
                Flags != constantBuffer.Flags || 
                SizeInBytes != constantBuffer.SizeInBytes)
            {
                reasonWhyNotSame = "Description not same";
                return false;
            }

            D3DShaderVariable[] variables = constantBuffer.Variables;

            if (Variables.Length != variables.Length)
            {
                reasonWhyNotSame = "Variable count not the same";
                return false;
            }

            for (int i = 0; i < Variables.Length; i++)
            {
                if (Variables[i].AreSame(variables[i], out reasonWhyNotSame))
                {
                    reasonWhyNotSame = String.Format("Variable {0} not the same:\n{1}", i.ToString(), reasonWhyNotSame);
                    return false;
                }
            }

            reasonWhyNotSame = String.Empty;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 31) + Name.GetHashCode();
                hash = (hash * 31) + SizeInBytes;
                hash = (hash * 31) + Variables.Length;
                hash = (hash * 31) + BufferType.GetHashCode();
                hash = (hash * 31) + Flags.GetHashCode();

                return hash;
            }
        }
    }
}
