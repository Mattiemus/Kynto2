namespace Spark.Scene
{
    using Math;

    /// <summary>
    /// Defines an object which has a transformation, with a local part and world part
    /// </summary>
    public interface ITransformed
    {
        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        Vector3 Translation { get; set; }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        Transform WorldTransform { get; }

        /// <summary>
        /// Gets the world scale
        /// </summary>
        Vector3 WorldScale { get; }

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        Quaternion WorldRotation { get; }

        /// <summary>
        /// Gets the world translation
        /// </summary>
        Vector3 WorldTranslation { get; }

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        Matrix4x4 WorldMatrix { get; }
    }
}
