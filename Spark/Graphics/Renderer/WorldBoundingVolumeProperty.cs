namespace Spark.Graphics
{
    using Math;

    /// <summary>
    /// Render property for a <see cref="BoundingVolume"/> that represents an object's world bounding volume.
    /// </summary>
    public sealed class WorldBoundingVolumeProperty : RenderPropertyAccessor<BoundingVolume>
    {
        private readonly BoundingVolume _value;

        /// <summary>
        /// Unique ID for this render property.
        /// </summary>
        public static readonly RenderPropertyId PropertyId = GetPropertyId<WorldBoundingVolumeProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldBoundingVolumeProperty"/> class.
        /// </summary>
        /// <param name="accessor">Accessor function.</param>
        public WorldBoundingVolumeProperty(GetPropertyValue<BoundingVolume> accessor) 
            : base(PropertyId, accessor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldBoundingVolumeProperty"/> class.
        /// </summary>
        public WorldBoundingVolumeProperty() 
            : this(new BoundingSphere())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldTransformProperty"/> class.
        /// </summary>
        /// <param name="volume">World transform.</param>
        public WorldBoundingVolumeProperty(BoundingVolume volume)
            : base(PropertyId)
        {
            _value = volume ?? new BoundingSphere();
            Accessor = GetValue;
        }

        /// <summary>
        /// Gets the current world bounding volume value
        /// </summary>
        /// <returns>Current world bounding volume value</returns>
        private BoundingVolume GetValue()
        {
            return _value;
        }
    }
}
