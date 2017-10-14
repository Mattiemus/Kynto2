namespace Spark.Graphics.Renderer
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a comparer for render bucket entries.
    /// </summary>
    public interface IRenderBucketEntryComparer : IComparer<RenderBucketEntry>
    {
        /// <summary>
        /// Sets a camera to be used by the comparer during sorting.
        /// </summary>
        /// <param name="cam">Camera to use during sorting.</param>
        void SetCamera(Camera cam);
    }
}
