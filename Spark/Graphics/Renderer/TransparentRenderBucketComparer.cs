namespace Spark.Graphics.Renderer
{
    using System.Diagnostics;

    /// <summary>
    /// Compares renderables based on distance to the camera in order to ensure a back-to-front ordering for correct transparent rendering.
    /// </summary>
    public sealed class TransparentRenderBucketComparer : IRenderBucketEntryComparer
    {
        private Camera _cam;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransparentRenderBucketComparer"/> class.
        /// </summary>
        public TransparentRenderBucketComparer()
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
            Debug.Assert(_cam != null);

            // Sort back-to-front
            float distX = RenderHelper.DistanceToCamera(x.Renderable, _cam);
            float distY = RenderHelper.DistanceToCamera(y.Renderable, _cam);

            if (distX < distY)
            {
                return 1;
            }

            if (distX > distY)
            {
                return -1;
            }

            return 0;
        }
    }
}
