namespace Spark.Graphics.Renderer
{
    using Math;
    using Graphics.Materials;
    
    /// <summary>
    /// Defines an object that can be rendered and thus has a set of properties that describe how it should
    /// be rendered.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Gets the material definition that contains the materials used to render the object.
        /// </summary>
        MaterialDefinition MaterialDefinition { get; }

        /// <summary>
        /// Gets the world transform of the renderable. At the bare minimum, this render property should be present in the render properties collection.
        /// </summary>
        Transform WorldTransform { get; }

        /// <summary>
        /// Gets the collection of render properties of the object.
        /// </summary>
        RenderPropertyCollection RenderProperties { get; }

        /// <summary>
        /// Gets if the renderable is valid for drawing.
        /// </summary>
        bool IsValidForDraw { get; }

        /// <summary>
        /// Performs the necessary draw calls to render the object.
        /// </summary>
        /// <param name="renderContext">Render context.</param>
        /// <param name="currentBucketId">The current bucket being drawn, may be invalid.</param>
        /// <param name="currentPass">The current pass that is drawing the renderable, may be null.</param>
        void SetupDrawCall(IRenderContext renderContext, RenderBucketId currentBucketId, MaterialPass currentPass);
    }
}
