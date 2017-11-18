namespace Spark.Graphics
{
    /// <summary>
    /// Represents a group of shaders. 
    /// </summary>
    public interface IEffectShaderGroup : IEffectPart
    {
        /// <summary>
        /// Gets the index of this shader group in the collection it is contained in.
        /// </summary>
        int ShaderGroupIndex { get; }

        /// <summary>
        /// Binds the set of shaders defined in the group and the resources used by them to the context.
        /// </summary>
        /// <param name="renderContext">Render context to apply to.</param>
        void Apply(IRenderContext renderContext);

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        bool ContainsShader(ShaderStage shaderStage);
    }
}
