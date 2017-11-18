namespace Spark.Graphics.Renderer
{
    /// <summary>
    /// A renderable that can be instanced. Every instance is associated with a single instance definition that
    /// holds the logic to setup hardware instancing, the materials and geometry that will be drawn.
    /// </summary>
    public interface IInstancedRenderable : IRenderable
    {
        /// <summary>
        /// Gets the instance definition associated with this instance.
        /// </summary>
        InstanceDefinition InstanceDefinition { get; }

        /// <summary>
        /// Sets the instance definition and associate it with this object. This should not be called directly, instead
        /// call <see cref="InstanceDefinition.AddInstance(IInstancedRenderable)"/>.
        /// </summary>
        /// <param name="instanceDef">Instance definition to associate with.</param>
        /// <returns>True if the instance is now associated, false otherwise.</returns>
        bool SetInstanceDefinition(InstanceDefinition instanceDef);

        /// <summary>
        /// Removes the association between this object and the instance definition. This should not be called directly,
        /// instead call <see cref="InstanceDefinition.RemoveInstance(IInstancedRenderable)"/>.
        /// </summary>
        void RemoveInstanceDefinition();
    }
}
