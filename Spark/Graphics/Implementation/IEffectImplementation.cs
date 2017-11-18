namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a callback when an effect shader group is applied to execute any pre-apply logic.
    /// </summary>
    /// <param name="renderContext">Render context</param>
    /// <param name="shaderGroup">Shader group to be applied to the context</param>
    public delegate void OnApplyDelegate(IRenderContext renderContext, IEffectShaderGroup shaderGroup);

    /// <summary>
    /// Defines an implementation for <see cref="Effect"/>.
    /// </summary>
    public interface IEffectImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Occurs when an effect shader group that is contained by this effect is about to be applied to a render context.
        /// </summary>
        event OnApplyDelegate OnShaderGroupApply;

        /// <summary>
        /// Gets the effect data
        /// </summary>
        EffectData EffectData { get; }

        /// <summary>
        /// Gets the effect sort key, used to compare effects as a first step in sorting objects to render. The sort key is the same
        /// for cloned effects. Further sorting can then be done using the indices of the contained techniques, and the indices of their passes.
        /// </summary>
        int SortKey { get; }

        /// <summary>
        /// Gets or sets the currently active shader group.
        /// </summary>
        IEffectShaderGroup CurrentShaderGroup { get; set; }

        /// <summary>
        /// Gets the shader groups contained in this effect.
        /// </summary>
        EffectShaderGroupCollection ShaderGroups { get; }

        /// <summary>
        /// Gets all effect parameters used by all shader groups.
        /// </summary>
        EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets all constant buffers that contain all value type parameters used by all shader groups.
        /// </summary>
        EffectConstantBufferCollection ConstantBuffers { get; }
        
        /// <summary>
        /// Clones the effect, and possibly sharing relevant underlying resources. Cloned instances are guaranteed to be
        /// completely separate from the source effect in terms of parameters, changing one will not change the other. But unlike
        /// creating a new effect from the same compiled byte code, native resources can still be shared more effectively.
        /// </summary>
        /// <returns>Cloned effect implementation.</returns>
        IEffectImplementation Clone();
    }
}
