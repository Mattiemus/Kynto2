namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    
    /// <summary>
    /// A Direct3D11 implementation for <see cref="BlendState"/>.
    /// </summary>
    public sealed class D3D11BlendStateImplementation : BlendStateImplementation, ID3D11BlendState
    {
        private D3D11.BlendState _nativeBlendState;
        private readonly bool _alphaToCoverageSupported;
        private readonly bool _independentBlendSupported;

        internal D3D11BlendStateImplementation(D3D11RenderSystem renderSystem, int resourceID)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;
            CheckSupport(D3DDevice, out _alphaToCoverageSupported, out _independentBlendSupported);

            Name = string.Empty;
        }

        /// <summary>
        /// Gets the number of render targets that allow for independent blending. This can vary by implementation, at least one is always guaranteed.
        /// </summary>
        public override int RenderTargetBlendCount => (RenderSystem as D3D11RenderSystem).Adapter.MaximumMultiRenderTargets;

        /// <summary>
        /// Checks if alpha-to-coverage is supported. This can vary by implementation.
        /// </summary>
        public override bool IsAlphaToCoverageSupported => _alphaToCoverageSupported;

        /// <summary>
        /// Checks if independent blending of multiple render targets (MRT) is supported. This can vary by implementation. If not supported, then the blending options
        /// specified for the first render target index are used for all other bound render targets, if those targets blend are enabled.
        /// </summary>
        public override bool IsIndependentBlendSupported => _independentBlendSupported;

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 blend state.
        /// </summary>
        public D3D11.BlendState D3DBlendState => _nativeBlendState;

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected override void CreateNativeState()
        {
            var desc = new D3D11.BlendStateDescription();
            desc.AlphaToCoverageEnable = (IsAlphaToCoverageSupported) && AlphaToCoverageEnable;
            desc.IndependentBlendEnable = (IsIndependentBlendSupported) && IndependentBlendEnable;

            var rtDescs = desc.RenderTarget;
            for (int i = 0; i < rtDescs.Length; i++)
            {
                GetRenderTargetBlendDescription(i, out RenderTargetBlendDescription blendDesc);                
                Direct3DHelper.ConvertRenderTargetBlendDescription(ref blendDesc, out D3D11.RenderTargetBlendDescription d3dBlendDesc);

                rtDescs[i] = d3dBlendDesc;
            }

            _nativeBlendState = new D3D11.BlendState(D3DDevice, desc);
            _nativeBlendState.DebugName = Name;
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeBlendState != null)
            {
                _nativeBlendState.DebugName = name;
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

            if (isDisposing && _nativeBlendState != null)
            {
                _nativeBlendState.Dispose();
                _nativeBlendState = null;
            }

            base.Dispose(isDisposing);
        }

        private void CheckSupport(D3D11.Device device, out bool alphaToCoverageSupport, out bool independentBlendSupport)
        {
            alphaToCoverageSupport = false;
            independentBlendSupport = false;

            switch (device.FeatureLevel)
            {
                case D3D.FeatureLevel.Level_11_0:
                case D3D.FeatureLevel.Level_10_1:
                case D3D.FeatureLevel.Level_10_0:
                    alphaToCoverageSupport = true;
                    independentBlendSupport = true;
                    break;
            }
        }
    }
}
