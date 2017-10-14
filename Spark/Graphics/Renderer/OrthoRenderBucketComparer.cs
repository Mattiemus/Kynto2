namespace Spark.Graphics.Renderer
{
    /// <summary>
    /// Compares renderables based on their <see cref="OrthoOrderProperty"/>.
    /// </summary>
    public sealed class OrthoRenderBucketComparer : IRenderBucketEntryComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthoRenderBucketComparer"/> class.
        /// </summary>
        public OrthoRenderBucketComparer()
        {
            // No-op
        }

        /// <summary>
        /// Sets a camera to be used by the comparer during sorting.
        /// </summary>
        /// <param name="cam">Camera to use during sorting.</param>
        public void SetCamera(Camera cam)
        {
            // No-op
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(RenderBucketEntry x, RenderBucketEntry y)
        {
            if (x.Renderable.RenderProperties.TryGet(out OrthoOrderProperty orthoX) && y.Renderable.RenderProperties.TryGet(out OrthoOrderProperty orthoY))
            {
                int oX = orthoX.Value;
                int oY = orthoY.Value;

                if (oX < oY)
                {
                    return -1;
                }

                if (oX > oY)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
