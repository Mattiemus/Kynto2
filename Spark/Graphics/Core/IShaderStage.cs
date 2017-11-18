namespace Spark.Graphics
{
    /// <summary>
    /// Defines a stage in the programmable graphics pipeline.
    /// </summary>
    public interface IShaderStage
    {
        /// <summary>
        /// Gets the maximum number of sampler state slots that are supported.
        /// </summary>
        int MaximumSamplerSlots { get; }

        /// <summary>
        /// Gets the maximum number of shader resource slots that are supported.
        /// </summary>
        int MaximumResourceSlots { get; }

        /// <summary>
        /// Gets the type of shader stage.
        /// </summary>
        ShaderStage StageType { get; }

        /// <summary>
        /// Sets a sampler state to the first slot. 
        /// </summary>
        /// <param name="sampler">Sampler state set. Null represents the default state.</param>
        void SetSampler(SamplerState sampler);

        /// <summary>
        /// Sets a sampler state at the specified slot index.
        /// </summary>
        /// <param name="slotIndex">Zero-based slot index, range is [0, MaxSamplerSlots - 1].</param>
        /// <param name="sampler">Sampler state to set. Null represents the default state.</param>
        void SetSampler(int slotIndex, SamplerState sampler);

        /// <summary>
        /// Sets an array of sampler states, starting at the first slot index.
        /// </summary>
        /// <param name="samplers">Array of sampler states. Null values represent the default state.</param>
        void SetSamplers(params SamplerState[] samplers);

        /// <summary>
        /// Sets an array of sampler states, starting at the specified slot index.
        /// </summary>
        /// <param name="startSlotIndex">Zero-based starting slot index, range is [0, MaxSamplerSlots - 1].</param>
        /// <param name="samplers">Array of sampler states to set. Null values represent the default state.</param>
        void SetSamplers(int startSlotIndex, params SamplerState[] samplers);

        /// <summary>
        /// Sets a shader resource to the first slot.
        /// </summary>
        /// <param name="resource">Shader resource to set. Null represents the default state.</param>
        void SetShaderResource(IShaderResource resource);

        /// <summary>
        /// Sets a shader resource at the specified slot index.
        /// </summary>
        /// <param name="slotIndex">Zero-based slot index, range is [0, MaxResourceSlots - 1].</param>
        /// <param name="resource">Shader resource to set. Null represents the default state.</param>
        void SetShaderResource(int slotIndex, IShaderResource resource);

        /// <summary>
        /// Sets an array of shader resources, starting at the first slot index.
        /// </summary>
        /// <param name="resources">Array of shader resources to set. Null values represent the default state.</param>
        void SetShaderResources(params IShaderResource[] resources);

        /// <summary>
        /// Sets an array of shader resources, starting at the specified slot index.
        /// </summary>
        /// <param name="startSlotIndex">Zero-based starting slot index, range is [0, MaxResourceSlots - 1].</param>
        /// <param name="resources">Array of shader resources to set. Null values represent the default state.</param>
        void SetShaderResources(int startSlotIndex, params IShaderResource[] resources);

        /// <summary>
        /// Gets all bound sampler states.
        /// </summary>
        /// <param name="startSlotIndex">Zero-based starting slot index, range is [0, MaxSamplerSlots - 1].</param>
        /// <param name="count">Number of resources to retrieve.</param>
        /// <returns>Array of sampler states that are currently bound to the stage.</returns>
        SamplerState[] GetSamplers(int startSlotIndex, int count);

        /// <summary>
        /// Gets all bound shader resources.
        /// </summary>
        /// <param name="startSlotIndex">Zero-based starting slot index, range is [0, MaxResourceSlots - 1].</param>
        /// <param name="count">Number of resources to retrieve.</param>
        /// <returns>Array of shader resources that are currently bound to the stage.</returns>
        IShaderResource[] GetShaderResources(int startSlotIndex, int count);
    }
}
