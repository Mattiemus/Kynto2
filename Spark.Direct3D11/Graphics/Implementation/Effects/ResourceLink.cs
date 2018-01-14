namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Math;

    internal struct ResourceLink
    {
        public Range Range;
        public bool IsConstantBuffer;

        public ResourceLink(Range range, bool isConstantBuffer)
        {
            Range = range;
            IsConstantBuffer = isConstantBuffer;
        }
    }
}
