namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Spark.Math;
    using Spark.Graphics;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;
    using SDX = SharpDX;
    using SDXM = SharpDX.Mathematics.Interop;

    /// <summary>
    /// Implementation for a Direct3D11 graphics adapter.
    /// </summary>
    public class D3D11GraphicsAdapter : IGraphicsAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11GraphicsAdapter"/> class.
        /// </summary>
        /// <param name="adapter">The DXGI adapter.</param>
        /// <param name="index">The adapter index.</param>
        internal D3D11GraphicsAdapter(DXGI.Adapter adapter, int index)
        {
            AdapterIndex = index;

            DXGI.AdapterDescription desc = adapter.Description;
            DedicatedSystemMemory = desc.DedicatedSystemMemory;
            DedicatedVideoMemory = desc.DedicatedVideoMemory;
            SharedSystemMemory = desc.SharedSystemMemory;
            Description = desc.Description;
            DeviceId = desc.DeviceId;
            SubSystemId = desc.SubsystemId;
            VendorId = desc.VendorId;
            Revision = desc.Revision;

            var dxgiOutputs = adapter.Outputs;
            var outputs = new Output[dxgiOutputs.Length];
            var modes = new List<DisplayMode>();

            for (int i = 0; i < dxgiOutputs.Length; i++)
            {
                using (var dxgiOutput = dxgiOutputs[i])
                {
                    var dxgiOutputDesc = dxgiOutput.Description;

                    SDXM.RawRectangle dxgiDesktopBounds = dxgiOutputDesc.DesktopBounds;
                    Direct3DHelper.ConvertRectangle(ref dxgiDesktopBounds, out Rectangle desktopBounds);

                    outputs[i] = new Output(dxgiOutputDesc.MonitorHandle, dxgiOutputDesc.DeviceName, desktopBounds);
                    foreach (SurfaceFormat format in Enum.GetValues(typeof(SurfaceFormat)))
                    {
                        try
                        {
                            DXGI.ModeDescription[] modeList = dxgiOutput.GetDisplayModeList(Direct3DHelper.ToD3DSurfaceFormat(format), 0);
                            if (modeList == null)
                            {
                                continue;
                            }

                            foreach (DXGI.ModeDescription modeDesc in modeList)
                            {
                                var displayMode = new DisplayMode(modeDesc.Width, modeDesc.Height, (int)Math.Floor(modeDesc.RefreshRate.Numerator / (float)modeDesc.RefreshRate.Denominator), format);
                                if (!modes.Contains(displayMode))
                                {
                                    modes.Add(displayMode);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e);
                        }
                    }
                }
            }

            SupportedDisplayModes = new DisplayModeCollection(modes);
            Outputs = new OutputCollection(outputs);
        }

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
        public bool IsDefaultAdapter => AdapterIndex == 0;

        /// <summary>
        /// Gets the adapter index.
        /// </summary>
        public int AdapterIndex { get; }

        /// <summary>
        /// Gets the maximum (U) size of a Texture1D resource.
        /// </summary>
        public int MaximumTexture1DSize => GetMaximumTextureSize(TextureDimension.One);

        /// <summary>
        /// Gets the maximum number of array slices in a Texture1DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTexture1DArrayCount => GetMaximumTextureArrayCount(TextureDimension.One);

        /// <summary>
        /// Gets the maximum size (U,V) of a Texture2D resource.
        /// </summary>
        public int MaximumTexture2DSize => GetMaximumTextureSize(TextureDimension.Two);

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTexture2DArrayCount => GetMaximumTextureArrayCount(TextureDimension.Two);

        /// <summary>
        /// Gets the maximum size (U,V,W) of a Texture3D resource.
        /// </summary>
        public int MaximumTexture3DSize => GetMaximumTextureSize(TextureDimension.Three);

        /// <summary>
        /// Gets the maximum size of a TextureCube resource.
        /// </summary>
        public int MaximumTextureCubeSize => GetMaximumTextureSize(TextureDimension.Cube);

        /// <summary>
        /// Gets the maximum number of array slices in a Texture2DArray resource, if zero arrays are not supported.
        /// </summary>
        public int MaximumTextureCubeArrayCount => GetMaximumTextureArrayCount(TextureDimension.Cube);

        /// <summary>
        /// Gets the maximum size of any texture resource in bytes.
        /// </summary>
        public int MaximumTextureResourceSize => GetMaximumTextureResouceSize();

        /// <summary>
        /// Gets the maximum number of render targets that can be set to the
        /// device at once (MRT).
        /// </summary>
        public int MaximumMultiRenderTargets => GetMaximumMultiRenderTargets();

        /// <summary>
        /// Gets the maximum number of vertex buffers that can be set to the device at once.
        /// </summary>
        public int MaximumVertexStreams => GetMaximumVertexStreams();

        /// <summary>
        /// Gets the maximum number of stream output targets that can be set to the device at once.
        /// </summary>
        public int MaximumStreamOutputTargets => GetMaximumStreamOutputTargets();

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
        /// Gets the D3D11 Device. Only valid when the adapter is used to initialize a render system.
        /// </summary>
        public D3D11.Device D3D11Device { get; private set; }

        /// <summary>
        /// Checks if the specified surface format is valid for texture resources.
        /// </summary>
        /// <param name="surfaceFormat">Surface format</param>
        /// <param name="texType">Type of texture</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool CheckTextureFormat(SurfaceFormat surfaceFormat, TextureDimension texType)
        {
            if (D3D11Device == null)
            {
                return false;
            }

            D3D11.FormatSupport support = D3D11Device.CheckFormatSupport(Direct3DHelper.ToD3DSurfaceFormat(surfaceFormat));

            switch (texType)
            {
                case TextureDimension.One:
                    return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.Texture1D);
                case TextureDimension.Two:
                case TextureDimension.Cube:
                    return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.Texture2D);
                case TextureDimension.Three:
                    return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.Texture3D);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the specified formats and sample counts are valid for a render target.
        /// </summary>
        /// <param name="format">Surface format</param>
        /// <param name="depthFormat">Depth format</param>
        /// <param name="multiSampleCount">Sample count</param>
        /// <returns>
        /// True if a valid combination, false otherwise.
        /// </returns>
        public bool CheckRenderTargetFormat(SurfaceFormat format, DepthFormat depthFormat, int multiSampleCount)
        {
            return CheckSurfaceFormat(format, multiSampleCount) && CheckDepthFormat(depthFormat, multiSampleCount);
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
            return CheckSurfaceFormat(format, multiSampleCount) && CheckDepthFormat(depthFormat, multiSampleCount);
        }

        /// <summary>
        /// Checks if the render target format can be resolved if multisampled.
        /// </summary>
        /// <param name="format">Format to test</param>
        /// <returns>True if it is resolvable, false otherwise.</returns>
        public bool IsMultisampleResolvable(SurfaceFormat format)
        {
            if (D3D11Device == null)
            {
                return false;
            }

            D3D11.FormatSupport support = D3D11Device.CheckFormatSupport(Direct3DHelper.ToD3DSurfaceFormat(format));
            return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.MultisampleResolve);
        }

        /// <summary>
        /// Checks if the shader stage is supported or not.
        /// </summary>
        /// <param name="shaderStage">Shader stage</param>
        /// <returns>True if the shader stage is supported, false otherwise.</returns>
        public bool IsShaderStageSupported(ShaderStage shaderStage)
        {
            if (D3D11Device == null)
            {
                return false;
            }

            switch (D3D11Device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                    return true;
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    if (shaderStage == ShaderStage.ComputeShader)
                    {
                        return false;
                    }

                    return true;
                case D3D.FeatureLevel.Level_9_3:
                case D3D.FeatureLevel.Level_9_2:
                case D3D.FeatureLevel.Level_9_1:
                    if (shaderStage == ShaderStage.VertexShader || shaderStage == ShaderStage.PixelShader)
                    {
                        return true;
                    }

                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks for the number of multisample quality levels are supported for the sample count. A value of zero
        /// means the format/multisample count combination is not valid. A non-zero value determines the number of quality levels that can be
        /// set, so a value of one means quality level zero, a value of two means quality levels zero and one, etc. The meanings of each quality level is
        /// vendor-specific.
        /// </summary>
        /// <param name="format">Specified format</param>
        /// <param name="multiSamplecount">Sample count</param>
        /// <returns>Number of supported quality levels.</returns>
        public int CheckMultisampleQualityLevels(SurfaceFormat format, int multiSamplecount)
        {
            if (D3D11Device == null)
            {
                return 0;
            }

            return D3D11Device.CheckMultisampleQualityLevels(Direct3DHelper.ToD3DSurfaceFormat(format), multiSamplecount);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Description;
        }

        internal void SetDevice(D3D11.Device device)
        {
            D3D11Device = device;
        }

        private bool CheckDepthFormat(DepthFormat depthFormat, int sampleCount)
        {
            if (depthFormat == DepthFormat.None)
            {
                return true;
            }

            if (D3D11Device == null)
            {
                return false;
            }

            DXGI.Format format = Direct3DHelper.ToD3DDepthFormat(depthFormat);
            D3D11.FormatSupport support = D3D11Device.CheckFormatSupport(format);

            if (sampleCount > 0)
            {
                bool correctSamples = D3D11Device.CheckMultisampleQualityLevels(format, sampleCount) > 0;

                return correctSamples && 
                       Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.DepthStencil) &&
                       Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.MultisampleRenderTarget);
            }

            return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.DepthStencil);
        }

        private bool CheckSurfaceFormat(SurfaceFormat surfaceFormat, int sampleCount)
        {
            if (D3D11Device == null)
            {
                return false;
            }

            DXGI.Format format = Direct3DHelper.ToD3DSurfaceFormat(surfaceFormat);
            D3D11.FormatSupport support = D3D11Device.CheckFormatSupport(format);

            if (sampleCount > 0)
            {
                bool correctSamples = D3D11Device.CheckMultisampleQualityLevels(format, sampleCount) > 0;

                return correctSamples && 
                       Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.RenderTarget) &&
                       Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.MultisampleRenderTarget);
            }

            return Direct3DHelper.IsFlagSet((int)support, (int)D3D11.FormatSupport.RenderTarget);
        }

        private int GetMaximumTextureSize(TextureDimension texDim)
        {
            if (D3D11Device == null)
            {
                return 0;
            }

            switch (texDim)
            {
                case TextureDimension.One:
                case TextureDimension.Two:
                    switch (D3D11Device.FeatureLevel)
                    {
                        case D3D.FeatureLevel.Level_11_0:
                            return 16384;
                        case D3D.FeatureLevel.Level_10_1:
                        case D3D.FeatureLevel.Level_10_0:
                            return 8192;
                        case D3D.FeatureLevel.Level_9_3:
                            return 4096;
                        case D3D.FeatureLevel.Level_9_2:
                        case D3D.FeatureLevel.Level_9_1:
                            return 2048;
                        default:
                            return 0;
                    }
                case TextureDimension.Three:
                    switch (D3D11Device.FeatureLevel)
                    {
                        case D3D.FeatureLevel.Level_11_0:
                        case D3D.FeatureLevel.Level_10_1:
                        case D3D.FeatureLevel.Level_10_0:
                            return 2048;
                        case D3D.FeatureLevel.Level_9_3:
                        case D3D.FeatureLevel.Level_9_2:
                        case D3D.FeatureLevel.Level_9_1:
                            return 256;
                        default:
                            return 0;
                    }
                case TextureDimension.Cube:
                    switch (D3D11Device.FeatureLevel)
                    {
                        case D3D.FeatureLevel.Level_11_0:
                            return 16384;
                        case D3D.FeatureLevel.Level_10_1:
                        case D3D.FeatureLevel.Level_10_0:
                            return 8192;
                        case D3D.FeatureLevel.Level_9_3:
                            return 4096;
                        case D3D.FeatureLevel.Level_9_2:
                        case D3D.FeatureLevel.Level_9_1:
                            return 512;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        private int GetMaximumTextureArrayCount(TextureDimension texDim)
        {
            if (D3D11Device == null)
            {
                return 0;
            }

            switch (texDim)
            {
                case TextureDimension.One:
                case TextureDimension.Two:
                    switch (D3D11Device.FeatureLevel)
                    {
                        case D3D.FeatureLevel.Level_11_0:
                            return 2048;
                        case D3D.FeatureLevel.Level_10_1:
                        case D3D.FeatureLevel.Level_10_0:
                            return 512;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        private int GetMaximumTextureResouceSize()
        {
            if (D3D11Device == null)
            {
                return 0;
            }

            switch (D3D11Device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                    return (int)(Math.Min(Math.Max(128f, 0.5f * (DedicatedVideoMemory / 1048576f)), 2048f) * 1048576f);
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                case D3D.FeatureLevel.Level_9_3:
                case D3D.FeatureLevel.Level_9_2:
                case D3D.FeatureLevel.Level_9_1:
                    return 128;
                default:
                    return 0;
            }
        }

        private int GetMaximumVertexStreams()
        {
            if (D3D11Device == null)
            {
                return 1;
            }

            switch (D3D11Device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                    return 32;
                case D3D.FeatureLevel.Level_10_0:
                case D3D.FeatureLevel.Level_9_3:
                case D3D.FeatureLevel.Level_9_2:
                case D3D.FeatureLevel.Level_9_1:
                    return 16;
                default:
                    return 1;
            }
        }

        private int GetMaximumMultiRenderTargets()
        {
            if (D3D11Device == null)
            {
                return 1;
            }

            switch (D3D11Device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    return 8;
                case D3D.FeatureLevel.Level_9_3:
                    return 4;
                case D3D.FeatureLevel.Level_9_2:
                case D3D.FeatureLevel.Level_9_1:
                    return 1;
                default:
                    return 0;
            }
        }

        private int GetMaximumStreamOutputTargets()
        {
            if (D3D11Device == null)
            {
                return 0;
            }

            switch (D3D11Device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
