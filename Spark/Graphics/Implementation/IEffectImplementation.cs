namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines an implementation for <see cref="Effect"/>.
    /// </summary>
    public interface IEffectImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets all effect parameters contained in this effect.
        /// </summary>
        EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the compiled effect byte code that represents this effect.
        /// </summary>
        byte[] EffectByteCode { get; }

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
