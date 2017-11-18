namespace Spark.Graphics
{
    using Math;

    /// <summary>
    /// Render property for a <see cref="Transform"/> that represents an object's world transform.
    /// </summary>
    public sealed class WorldTransformProperty : RenderPropertyAccessor<Transform>
    {
        private readonly Transform _value;

        /// <summary>
        /// Unique ID for this render property.
        /// </summary>
        public static readonly RenderPropertyId PropertyId = GetPropertyId<WorldTransformProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldTransformProperty"/> class.
        /// </summary>
        /// <param name="accessor">Accessor function.</param>
        public WorldTransformProperty(GetPropertyValue<Transform> accessor) 
            : base(PropertyId, accessor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldTransformProperty"/> class.
        /// </summary>
        public WorldTransformProperty() 
            : this(new Transform())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldTransformProperty"/> class.
        /// </summary>
        /// <param name="transform">World transform.</param>
        public WorldTransformProperty(Transform transform)
            : base(PropertyId)
        {
            _value = transform ?? new Transform();
            Accessor = GetValue;
        }

        /// <summary>
        /// Gets the current world transform value
        /// </summary>
        /// <returns>Current world transform value</returns>
        private Transform GetValue()
        {
            return _value;
        }
    }
}
