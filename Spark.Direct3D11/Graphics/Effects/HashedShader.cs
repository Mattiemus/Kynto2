namespace Spark.Direct3D11.Graphics
{
    using System;
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    public struct HashedShader : IEquatable<HashedShader>
    {

        public HashedShader(D3D11.DeviceChild shader, int hashCode, ShaderStage shaderType)
        {
            Shader = shader;
            HashCode = hashCode;
            ShaderType = shaderType;
        }

        public D3D11.DeviceChild Shader { get; set; }

        public int HashCode { get; set; }

        public ShaderStage ShaderType { get; set; }

        public static implicit operator HashedShader(D3D11.VertexShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.VertexShader);
        }

        public static implicit operator HashedShader(D3D11.PixelShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.PixelShader);
        }

        public static implicit operator HashedShader(D3D11.GeometryShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.GeometryShader);
        }

        public static implicit operator HashedShader(D3D11.HullShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.HullShader);
        }

        public static implicit operator HashedShader(D3D11.DomainShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.DomainShader);
        }

        public static implicit operator HashedShader(D3D11.ComputeShader shader)
        {
            return new HashedShader(shader, (shader != null) ? shader.GetHashCode() : 0, ShaderStage.ComputeShader);
        }

        public static bool operator ==(HashedShader a, HashedShader b)
        {
            return (a.ShaderType == b.ShaderType) && (a.HashCode == b.HashCode) && ReferenceEquals(a.Shader, b.Shader);
        }

        public static bool operator !=(HashedShader a, HashedShader b)
        {
            return (a.ShaderType != b.ShaderType) || (a.HashCode != b.HashCode) || !ReferenceEquals(a.Shader, b.Shader);
        }

        public static HashedShader NullShader(ShaderStage stageType)
        {
            return new HashedShader(null, 0, stageType);
        }

        public override bool Equals(object obj)
        {
            if (obj is HashedShader)
            {
                return Equals((HashedShader)obj);
            }

            return false;
        }

        public bool Equals(HashedShader other)
        {
            return (ShaderType == other.ShaderType) && (HashCode == other.HashCode) && ReferenceEquals(Shader, other.Shader);
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
