namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics.Implementation;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="RasterizerState"/>.
    /// </summary>
    public sealed class D3D11RasterizerStateImplementation : RasterizerStateImplementation, ID3D11RasterizerState
    {
        private D3D11.RasterizerState _nativeRasterizerState;
        private readonly bool _antialiasedLineOptionSupported;
        private readonly bool _depthClipOptionSupported;

        internal D3D11RasterizerStateImplementation(D3D11RenderSystem renderSystem, int resourceID)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            CheckSupport(D3DDevice, out _antialiasedLineOptionSupported, out _depthClipOptionSupported);

            Name = string.Empty;
        }

        /// <summary>
        /// Gets if the <see cref="AntialiasedLineEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public override bool IsAntialiasedLineOptionSupported => _antialiasedLineOptionSupported;

        /// <summary>
        /// Gets if the <see cref="DepthClipEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public override bool IsDepthClipOptionSupported => _depthClipOptionSupported;

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 rasterizer state.
        /// </summary>
        public D3D11.RasterizerState D3DRasterizerState => _nativeRasterizerState;

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected override void CreateNativeState()
        {
            var desc = new D3D11.RasterizerStateDescription();
            desc.IsAntialiasedLineEnabled = (IsAntialiasedLineOptionSupported) && AntialiasedLineEnable;
            desc.IsDepthClipEnabled = (IsDepthClipOptionSupported) && DepthClipEnable;
            desc.IsMultisampleEnabled = MultiSampleEnable;
            desc.IsScissorEnabled = ScissorTestEnable;
            desc.IsFrontCounterClockwise = (VertexWinding == Spark.Graphics.VertexWinding.CounterClockwise);

            desc.DepthBias = DepthBias;
            desc.DepthBiasClamp = DepthBiasClamp;
            desc.SlopeScaledDepthBias = SlopeScaledDepthBias;
            desc.CullMode = Direct3DHelper.ToD3DCullMode(Cull);
            desc.FillMode = Direct3DHelper.ToD3DFillMode(Fill);

            _nativeRasterizerState = new D3D11.RasterizerState(D3DDevice, desc);
            _nativeRasterizerState.DebugName = Name;
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeRasterizerState != null)
            {
                _nativeRasterizerState.DebugName = name;
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

            if (isDisposing && _nativeRasterizerState != null)
            {
                _nativeRasterizerState.Dispose();
                _nativeRasterizerState = null;
            }

            base.Dispose(isDisposing);
        }

        private void CheckSupport(D3D11.Device device, out bool antialiasedLineOptionSupport, out bool depthClipOptionSupport)
        {
            antialiasedLineOptionSupport = false;
            depthClipOptionSupport = false;

            switch (device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    antialiasedLineOptionSupport = true;
                    depthClipOptionSupport = true;
                    break;
            }
        }
    }
}
