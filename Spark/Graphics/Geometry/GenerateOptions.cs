namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Options for vertex attributes during geometry generation.
    /// </summary>
    [Flags]
    public enum GenerateOptions
    {
        /// <summary>
        /// Generate no vertex attributes.
        /// </summary>
        None = 0,

        /// <summary>
        /// Generate vertex positions.
        /// </summary>
        Positions = 1,

        /// <summary>
        /// Generate vertex normals.
        /// </summary>
        Normals = 2,

        /// <summary>
        /// Generate vertex texture coordinates.
        /// </summary>
        TextureCoordinates = 4,

        /// <summary>
        /// Generate all vertex attributes.
        /// </summary>
        All = Positions | Normals | TextureCoordinates
    }
}
