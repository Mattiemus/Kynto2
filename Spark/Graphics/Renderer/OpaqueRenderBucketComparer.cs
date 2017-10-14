namespace Spark.Graphics.Renderer
{
    using System.Diagnostics;
    
    /// <summary>
    /// Compares renderables primarily by their material to minimize state changes in the graphics pipeline. If two objects
    /// share a similar material, then they are sorted front-to-back to minimize overdraw.
    /// </summary>
    public sealed class OpaqueRenderBucketComparer : IRenderBucketEntryComparer
    {
        private Camera _cam;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpaqueRenderBucketComparer"/> class.
        /// </summary>
        public OpaqueRenderBucketComparer()
        {
        }

        /// <summary>
        /// Sets a camera to be used by the comparer during sorting.
        /// </summary>
        /// <param name="cam">Camera to use during sorting.</param>
        public void SetCamera(Camera cam)
        {
            _cam = cam;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(RenderBucketEntry x, RenderBucketEntry y)
        {
            // Compare materials if applicable
            if (x.Material != null && y.Material != null)
            {
                int matCompareResult = x.Material.CompareTo(y.Material);
                if (matCompareResult != 0)
                {
                    return matCompareResult;
                }
            }
            
            Debug.Assert(_cam != null);

            // Otherwise, do front-back ordering
            float distX = RenderHelper.DistanceToCamera(x.Renderable, _cam);
            float distY = RenderHelper.DistanceToCamera(y.Renderable, _cam);

            if (distX < distY)
            {
                return -1;
            }

            if (distX > distY)
            {
                return 1;
            }

            return 0;
        }
    }
}
