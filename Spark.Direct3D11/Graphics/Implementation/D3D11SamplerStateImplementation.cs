namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Math;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="SamplerState"/>.
    /// </summary>
    public sealed class D3D11SamplerStateImplementation : SamplerStateImplementation, ID3D11SamplerState
    {
        private D3D11.SamplerState _nativeSamplerState;
        private readonly int _supportedAnisotropyLevels;

        internal D3D11SamplerStateImplementation(D3D11RenderSystem renderSystem, int resourceID)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            _supportedAnisotropyLevels = CheckSupport(D3DDevice);

            Name = string.Empty;
        }

        /// <summary>
        /// Gets the number of anisotropy levels supported. This can vary by implementation.
        /// </summary>
        public override int SupportedAnisotropyLevels => _supportedAnisotropyLevels;

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 sampler state.
        /// </summary>
        public D3D11.SamplerState D3DSamplerState => _nativeSamplerState;

        /// <summary>
        /// Checks if the specified texture addressing mode is supported by the graphics platform.
        /// </summary>
        /// <param name="mode">Texture addressing mode</param>
        /// <returns>True if supported, false otherwise.</returns>
        public override bool IsAddressingModeSupported(TextureAddressMode mode)
        {
            switch (mode)
            {
                case TextureAddressMode.Border:
                case TextureAddressMode.Clamp:
                case TextureAddressMode.Mirror:
                case TextureAddressMode.MirrorOnce:
                case TextureAddressMode.Wrap:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected override void CreateNativeState()
        {
            var desc = new D3D11.SamplerStateDescription();
            desc.AddressU = Direct3DHelper.ToD3DTextureAddressMode(AddressU);
            desc.AddressV = Direct3DHelper.ToD3DTextureAddressMode(AddressV);
            desc.AddressW = Direct3DHelper.ToD3DTextureAddressMode(AddressW);
            desc.Filter = Direct3DHelper.ToD3DFilter(Filter);

            desc.MaximumAnisotropy = MaxAnisotropy;
            desc.MipLodBias = MipMapLevelOfDetailBias;
            desc.MinimumLod = MinMipMapLevel;
            desc.MaximumLod = (MaxMipMapLevel == int.MaxValue) ? float.MaxValue : MaxMipMapLevel;

            Color borderColor = BorderColor;
            Direct3DHelper.ConvertColor(ref borderColor, out desc.BorderColor);
            desc.ComparisonFunction = D3D11.Comparison.Never;

            _nativeSamplerState = new D3D11.SamplerState(D3DDevice, desc);
            _nativeSamplerState.DebugName = Name;
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeSamplerState != null)
            {
                _nativeSamplerState.DebugName = name;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing && _nativeSamplerState != null)
            {
                _nativeSamplerState.Dispose();
                _nativeSamplerState = null;
            }

            base.Dispose(isDisposing);
        }

        private int CheckSupport(D3D11.Device device)
        {
            switch (device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    return 16;
                default:
                    return 4;
            }
        }
    }
}
