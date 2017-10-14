namespace Spark.Graphics.Renderer
{
    using System.Collections.Generic;

    internal sealed class RenderBucketIdEqualityComparer : IEqualityComparer<RenderBucketId>
    {
        public bool Equals(RenderBucketId x, RenderBucketId y)
        {
            return x == y;
        }

        public int GetHashCode(RenderBucketId obj)
        {
            return obj.Value;
        }
    }
}
