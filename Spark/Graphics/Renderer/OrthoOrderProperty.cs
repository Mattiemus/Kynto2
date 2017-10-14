namespace Spark.Graphics.Renderer
{
    /// <summary>
    /// Render property to determine the order of renderables when rendered in orthographic.
    /// </summary>
    public sealed class OrthoOrderProperty : RenderProperty.Int
    {
        /// <summary>
        /// Unique ID for this render property.
        /// </summary>
        public static readonly RenderPropertyId PropertyId = GetPropertyId<OrthoOrderProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthoOrderProperty"/> class.
        /// </summary>
        public OrthoOrderProperty() 
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthoOrderProperty"/> class.
        /// </summary>
        /// <param name="orthoOrder">Ortho order.</param>
        public OrthoOrderProperty(int orthoOrder) 
            : base(PropertyId, orthoOrder)
        {
        }
    }
}
