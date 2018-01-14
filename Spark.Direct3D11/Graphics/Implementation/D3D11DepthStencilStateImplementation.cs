namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics.Implementation;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A Direct3D11 implementation for <see cref="DepthStencilState"/>.
    /// </summary>
    public sealed class D3D11DepthStencilStateImplementation : DepthStencilStateImplementation, ID3D11DepthStencilState
    {
        private D3D11.DepthStencilState _nativeDepthStencilState;

        internal D3D11DepthStencilStateImplementation(D3D11RenderSystem renderSystem, int resourceID)
            : base(renderSystem, resourceID)
        {
            D3DDevice = renderSystem.D3DDevice;

            Name = string.Empty;
        }

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 depth stencil state.
        /// </summary>
        public D3D11.DepthStencilState D3DDepthStencilState => _nativeDepthStencilState;

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected override void CreateNativeState()
        {
            var desc = new D3D11.DepthStencilStateDescription();
            desc.IsDepthEnabled = DepthEnable;
            desc.IsStencilEnabled = StencilEnable;
            desc.DepthWriteMask = (DepthWriteEnable) ? D3D11.DepthWriteMask.All : D3D11.DepthWriteMask.Zero;
            desc.DepthComparison = Direct3DHelper.ToD3DComparison(DepthFunction);
            desc.StencilWriteMask = (byte)StencilWriteMask;
            desc.StencilReadMask = (byte)StencilReadMask;

            // Set front facing stencil op
            var frontOpDesc = new D3D11.DepthStencilOperationDescription();
            frontOpDesc.Comparison = Direct3DHelper.ToD3DComparison(StencilFunction);
            frontOpDesc.DepthFailOperation = Direct3DHelper.ToD3DStencilOperation(StencilDepthFail);
            frontOpDesc.FailOperation = Direct3DHelper.ToD3DStencilOperation(StencilFail);
            frontOpDesc.PassOperation = Direct3DHelper.ToD3DStencilOperation(StencilPass);

            // Setup back facing stencil op
            var backOpDesc = new D3D11.DepthStencilOperationDescription();

            if (TwoSidedStencilEnable)
            {
                backOpDesc.Comparison = Direct3DHelper.ToD3DComparison(BackFaceStencilFunction);
                backOpDesc.DepthFailOperation = Direct3DHelper.ToD3DStencilOperation(BackFaceStencilDepthFail);
                backOpDesc.FailOperation = Direct3DHelper.ToD3DStencilOperation(BackFaceStencilFail);
                backOpDesc.PassOperation = Direct3DHelper.ToD3DStencilOperation(BackFaceStencilPass);
            }
            else
            {
                // Set defaults
                backOpDesc.Comparison = D3D11.Comparison.Always;
                backOpDesc.DepthFailOperation = D3D11.StencilOperation.Keep;
                backOpDesc.FailOperation = D3D11.StencilOperation.Keep;
                backOpDesc.PassOperation = D3D11.StencilOperation.Keep;
            }

            desc.FrontFace = frontOpDesc;
            desc.BackFace = backOpDesc;

            _nativeDepthStencilState = new D3D11.DepthStencilState(D3DDevice, desc);
            _nativeDepthStencilState.DebugName = Name;
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected override void OnNameChange(string name)
        {
            if (_nativeDepthStencilState != null)
            {
                _nativeDepthStencilState.DebugName = name;
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

            if (isDisposing && _nativeDepthStencilState != null)
            {
                _nativeDepthStencilState.Dispose();
                _nativeDepthStencilState = null;
            }

            base.Dispose(isDisposing);            
        }
    }
}
