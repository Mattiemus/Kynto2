namespace Spark.Direct3D11.Graphics
{
    using System;

    public struct HashedShaderSignature : IEquatable<HashedShaderSignature>
    {
        public HashedShaderSignature(byte[] byteCode, int hashCode)
        {
            ByteCode = byteCode;
            HashCode = hashCode;
        }

        public byte[] ByteCode { get; set; }
        public int HashCode { get; set; }

        public static bool operator ==(HashedShaderSignature a, HashedShaderSignature b)
        {
            return (a.HashCode == b.HashCode) && MemoryHelper.Compare(a.ByteCode, b.ByteCode);
        }

        public static bool operator !=(HashedShaderSignature a, HashedShaderSignature b)
        {
            return (a.HashCode != b.HashCode) || !MemoryHelper.Compare(a.ByteCode, b.ByteCode);
        }

        public override bool Equals(object obj)
        {
            if (obj is HashedShaderSignature)
            {
                return Equals((HashedShaderSignature)obj);
            }

            return false;
        }

        public bool Equals(HashedShaderSignature other)
        {
            return (HashCode == other.HashCode) && MemoryHelper.Compare(ByteCode, other.ByteCode);
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
