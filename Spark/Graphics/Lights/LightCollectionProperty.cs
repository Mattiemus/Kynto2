namespace Spark.Graphics.Lights
{
    using Renderer;

    /// <summary>
    /// Render property for a <see cref="LightCollection"/>.
    /// </summary>
    public sealed class LightCollectionProperty : RenderPropertyAccessor<LightCollection>
    {
        private readonly LightCollection _value;

        /// <summary>
        /// Unique ID for this render property.
        /// </summary>
        public static readonly RenderPropertyId PropertyId = GetPropertyId<LightCollectionProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollectionProperty"/> class.
        /// </summary>
        /// <param name="accessor">Accessor function.</param>
        public LightCollectionProperty(GetPropertyValue<LightCollection> accessor) 
            : base(PropertyId, accessor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollectionProperty"/> class.
        /// </summary>
        public LightCollectionProperty() 
            : this(new LightCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightCollectionProperty"/> class.
        /// </summary>
        /// <param name="lights">Collection of lights.</param>
        public LightCollectionProperty(LightCollection lights)
            : base(PropertyId)
        {
            _value = lights ?? new LightCollection();
            Accessor = GetValue;
        }

        /// <summary>
        /// Gets the current light collection value
        /// </summary>
        /// <returns>Current light collection value</returns>
        private LightCollection GetValue()
        {
            return _value;
        }
    }
}
