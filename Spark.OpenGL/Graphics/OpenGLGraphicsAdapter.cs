namespace Spark.OpenGL.Graphics
{
    using System;

    using Spark.Graphics;

    using OGL = OpenTK.Graphics.OpenGL;

    public sealed class OpenGLGraphicsAdapter : IGraphicsAdapter
    {
        /// <summary>
        /// Gets the collection of supported display modes.
        /// </summary>
        public DisplayModeCollection SupportedDisplayModes { get; }

        /// <summary>
        /// Gets the collection of outputs (e.g. monitors).
        /// </summary>
        public OutputCollection Outputs { get; }

        /// <summary>
        /// Gets the description of the device.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the device ID which identifies the particular chipset.
        /// </summary>
        public int DeviceId { get; }

        /// <summary>
        /// Gets the adapter's revision number for the particular chipset its associated with.
        /// </summary>
        public int Revision { get; }

        /// <summary>
        /// Gets the value that identifies the adapter's subsystem.
        /// </summary>
        public int SubSystemId { get; }

        /// <summary>
        /// Gets the value that identifies that chipset's manufacturer.
        /// </summary>
        public int VendorId { get; }

        /// <summary>
        /// Gets if this is the default adapter, always the first adapter.
        /// </summary>
        public bool IsDefaultAdapter { get; }

        /// <summary>
        /// Gets the adapter index.
        /// </summary>
        public int AdapterIndex { get; }

        /// <summary>
        /// Gets the maximum (U) size of a Texture1D resource.
        /// </summary>
        public int MaximumTexture1DSize { get; }

        /// <summary>
        /// Gets the maximum number of array slices in a Texture1DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTexture1DArrayCount { get; }

        /// <summary>
        /// Gets the maximum size (U,V) of a Texture2D resource.
        /// </summary>
        public int MaximumTexture2DSize => OGL.GL.GetInteger(OGL.GetPName.MaxTextureSize);

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTexture2DArrayCount { get; }

        /// <summary>
        /// Gets the maximum size (U,V,W) of a Texture3D resource.
        /// </summary>
        public int MaximumTexture3DSize { get; }

        /// <summary>
        /// Gets the maximum size of a TextureCube resource.
        /// </summary>
        public int MaximumTextureCubeSize { get; }

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTextureCubeArrayCount { get; }

        /// <summary>
        /// Gets the maximum size of any texture resource in bytes.
        /// </summary>
        public int MaximumTextureResourceSize { get; }

        /// <summary>
        /// Gets the maximum number of render targets that can be set to the
        /// device at once (MRT).
        /// </summary>
        public int MaximumMultiRenderTargets { get; }

        /// <summary>
        /// Gets the maximum number of vertex buffers that can be set to the device at once.
        /// </summary>
        public int MaximumVertexStreams { get; }

        /// <summary>
        /// Gets the maximum number of stream output targets that can be set to the device at once.
        /// </summary>
        public int MaximumStreamOutputTargets { get; }

        /// <summary>
        /// Gets the number of bytes of system memory not shared with the CPU.
        /// </summary>
        public long DedicatedSystemMemory { get; }

        /// <summary>
        /// Gets the number of bytes of video memory not shared with the CPU.
        /// </summary>
        public long DedicatedVideoMemory { get; }

        /// <summary>
        /// Gets the number of bytes of system memory shared with the CPU.
        /// </summary>
        public long SharedSystemMemory { get; }

        /// <summary>
        /// Checks if the specified surface format is valid for texture resources.
        /// </summary>
        /// <param name="surfaceFormat">Surface format</param>
        /// <param name="texType">Type of texture</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool CheckTextureFormat(SurfaceFormat surfaceFormat, TextureDimension texType)
        {
            // TODO:
            return true;
        }

        /// <summary>
        /// Checks if the specified formats and sample counts are valid for a render target.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="multiSampleCount">Sample count</param>
        /// <returns>True if a valid combination, false otherwise.</returns>
        public bool CheckRenderTargetFormat(SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount)
        {
            // TODO:
            return true;
        }

        /// <summary>
        /// Checks if the specified formats and sample counts are valid for a back buffer.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="multiSampleCount">Sample count</param>
        /// <returns>True if a valid combination, false otherwise.</returns>
        public bool CheckBackBufferFormat(SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount)
        {
            // TODO:
            return true;
        }

        /// <summary>
        /// Checks if the render target format can be resolved if multisampled.
        /// </summary>
        /// <param name="format">Format to test</param>
        /// <returns>True if it is resolvable, false otherwise.</returns>
        public bool IsMultisampleResolvable(SurfaceFormat format)
        {
            // TODO:
            return true;
        }

        /// <summary>
        /// Checks if the shader stage is supported or not.
        /// </summary>
        /// <param name="shaderStage">Shader stage</param>
        /// <returns>True if the shader stage is supported, false otherwise.</returns>
        public bool IsShaderStageSupported(ShaderStage shaderStage)
        {
            // TODO:
            return true;
        }

        /// <summary>
        /// Checks for the number of multisample quality levels are supported for the sample count. A value of zero
        /// means the format/multisample count combination is not valid. A non-zero value determines the number of quality levels that can be
        /// set, so a value of one means quality level zero, a value of two means quality levels zero and one, etc. The meanings of each quality level is 
        /// vendor-specific.
        /// </summary>
        /// <param name="format">Specified format</param>
        /// <param name="multiSamplecount">Sample count</param>
        /// <returns>Number of supported quality levels</returns>
        public int CheckMultisampleQualityLevels(SurfaceFormat format, int multiSamplecount)
        {
            // TODO:
            return 0;
        }
    }
}
