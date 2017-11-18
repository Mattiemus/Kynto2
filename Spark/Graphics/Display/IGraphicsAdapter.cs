namespace Spark.Graphics
{
    /// <summary>
    /// Defines an adapter that represents the physical GPU and enumerates useful information about features and resource limits supported by the hardware.
    /// </summary>
    public interface IGraphicsAdapter
    {
        /// <summary>
        /// Gets the collection of supported display modes.
        /// </summary>
        DisplayModeCollection SupportedDisplayModes { get; }

        /// <summary>
        /// Gets the collection of outputs (e.g. monitors).
        /// </summary>
        OutputCollection Outputs { get; }

        /// <summary>
        /// Gets the description of the device.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the device ID which identifies the particular chipset.
        /// </summary>
        int DeviceId { get; }

        /// <summary>
        /// Gets the adapter's revision number for the particular chipset its associated with.
        /// </summary>
        int Revision { get; }

        /// <summary>
        /// Gets the value that identifies the adapter's subsystem.
        /// </summary>
        int SubSystemId { get; }

        /// <summary>
        /// Gets the value that identifies that chipset's manufacturer.
        /// </summary>
        int VendorId { get; }

        /// <summary>
        /// Gets if this is the default adapter, always the first adapter.
        /// </summary>
        bool IsDefaultAdapter { get; }

        /// <summary>
        /// Gets the adapter index.
        /// </summary>
        int AdapterIndex { get; }

        /// <summary>
        /// Gets the maximum (U) size of a Texture1D resource.
        /// </summary>
        int MaximumTexture1DSize { get; }

        /// <summary>
        /// Gets the maximum number of array slices in a Texture1DArray resource, if zero arrays are not supported.
        /// </summary>
        int MaximumTexture1DArrayCount { get; }

        /// <summary>
        /// Gets the maximum size (U,V) of a Texture2D resource.
        /// </summary>
        int MaximumTexture2DSize { get; }

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        int MaximumTexture2DArrayCount { get; }

        /// <summary>
        /// Gets the maximum size (U,V,W) of a Texture3D resource.
        /// </summary>
        int MaximumTexture3DSize { get; }

        /// <summary>
        /// Gets the maximum size of a TextureCube resource.
        /// </summary>
        int MaximumTextureCubeSize { get; }

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        int MaximumTextureCubeArrayCount { get; }

        /// <summary>
        /// Gets the maximum size of any texture resource in bytes.
        /// </summary>
        int MaximumTextureResourceSize { get; }

        /// <summary>
        /// Gets the maximum number of render targets that can be set to the
        /// device at once (MRT).
        /// </summary>
        int MaximumMultiRenderTargets { get; }

        /// <summary>
        /// Gets the maximum number of vertex buffers that can be set to the device at once.
        /// </summary>
        int MaximumVertexStreams { get; }

        /// <summary>
        /// Gets the maximum number of stream output targets that can be set to the device at once.
        /// </summary>
        int MaximumStreamOutputTargets { get; }

        /// <summary>
        /// Gets the number of bytes of system memory not shared with the CPU.
        /// </summary>
        long DedicatedSystemMemory { get; }

        /// <summary>
        /// Gets the number of bytes of video memory not shared with the CPU.
        /// </summary>
        long DedicatedVideoMemory { get; }

        /// <summary>
        /// Gets the number of bytes of system memory shared with the CPU.
        /// </summary>
        long SharedSystemMemory { get; }

        /// <summary>
        /// Checks if the specified surface format is valid for texture resources.
        /// </summary>
        /// <param name="surfaceFormat">Surface format</param>
        /// <param name="texType">Type of texture</param>
        /// <returns>True if valid, false otherwise</returns>
        bool CheckTextureFormat(SurfaceFormat surfaceFormat, TextureDimension texType);

        /// <summary>
        /// Checks if the specified formats and sample counts are valid for a render target.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="multiSampleCount">Sample count</param>
        /// <returns>True if a valid combination, false otherwise.</returns>
        bool CheckRenderTargetFormat(SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount);

        /// <summary>
        /// Checks if the specified formats and sample counts are valid for a back buffer.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="multiSampleCount">Sample count</param>
        /// <returns>True if a valid combination, false otherwise.</returns>
        bool CheckBackBufferFormat(SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount);

        /// <summary>
        /// Checks if the render target format can be resolved if multisampled.
        /// </summary>
        /// <param name="format">Format to test</param>
        /// <returns>True if it is resolvable, false otherwise.</returns>
        bool IsMultisampleResolvable(SurfaceFormat format);

        /// <summary>
        /// Checks if the shader stage is supported or not.
        /// </summary>
        /// <param name="shaderStage">Shader stage</param>
        /// <returns>True if the shader stage is supported, false otherwise.</returns>
        bool IsShaderStageSupported(ShaderStage shaderStage);

        /// <summary>
        /// Checks for the number of multisample quality levels are supported for the sample count. A value of zero
        /// means the format/multisample count combination is not valid. A non-zero value determines the number of quality levels that can be
        /// set, so a value of one means quality level zero, a value of two means quality levels zero and one, etc. The meanings of each quality level is 
        /// vendor-specific.
        /// </summary>
        /// <param name="format">Specified format</param>
        /// <param name="multiSamplecount">Sample count</param>
        /// <returns>Number of supported quality levels</returns>
        int CheckMultisampleQualityLevels(SurfaceFormat format, int multiSamplecount);
    }
}
