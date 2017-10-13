namespace Spark.Graphics.Renderer
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares two <see cref="RenderPropertyId" /> objects for equality
    /// </summary>
    internal sealed class RenderPropertyIdEqualityComparer : IEqualityComparer<RenderPropertyId>
    {
        /// <summary>
        /// Compares two render property ids for equality
        /// </summary>
        /// <param name="x">First render property id</param>
        /// <param name="y">Second render property id</param>
        /// <returns>True if the two ids are equal, false otherwise</returns>
        public bool Equals(RenderPropertyId x, RenderPropertyId y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets the hash code for a render property id
        /// </summary>
        /// <param name="obj">Render property instance</param>
        /// <returns>Hash code for the render property id</returns>
        public int GetHashCode(RenderPropertyId obj)
        {
            return obj.Value;
        }
    }
}
