namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Common base class for a <see cref="DepthStencilState"/> implementation.
    /// </summary>
    public abstract class DepthStencilStateImplementation : GraphicsResourceImplementation, IDepthStencilStateImplementation
    {
        private bool _depthEnable;
        private bool _depthWriteEnable;
        private ComparisonFunction _depthFunction;
        private bool _stencilEnable;
        private int _referenceStencil;
        private int _stencilReadMask;
        private int _stencilWriteMask;
        private bool _twoSidedStencilEnable;
        private ComparisonFunction _stencilFunction;
        private StencilOperation _stencilDepthFail;
        private StencilOperation _stencilFail;
        private StencilOperation _stencilPass;
        private ComparisonFunction _backFaceStencilFunction;
        private StencilOperation _backFaceStencilDepthFail;
        private StencilOperation _backFaceStencilFail;
        private StencilOperation _backFaceStencilPass;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthStencilStateImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        protected DepthStencilStateImplementation(IRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
            IsBound = false;
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets or sets if the depth buffer should be enabled. By default, this value is true.
        /// </summary>
        public bool DepthEnable
        {
            get => _depthEnable;
            set
            {
                ThrowIfBound();
                _depthEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets if the depth buffer should be writable. By default, this value is true.
        /// </summary>
        public bool DepthWriteEnable
        {
            get => _depthWriteEnable;
            set
            {
                ThrowIfBound();
                _depthWriteEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets the depth comparison function for the depth test. By default, this value is <see cref="ComparisonFunction.LessEqual" />.
        /// </summary>
        public ComparisonFunction DepthFunction
        {
            get => _depthFunction;
            set
            {
                ThrowIfBound();
                _depthFunction = value;
            }
        }

        /// <summary>
        /// Gets or sets if the stencil buffer should be enabled. By default, this value is false.
        /// </summary>
        public bool StencilEnable
        {
            get => _stencilEnable;
            set
            {
                ThrowIfBound();
                _stencilEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets the reference stencil value used for stencil testing. By default, this value is zero.
        /// </summary>
        public int ReferenceStencil
        {
            get => _referenceStencil;
            set
            {
                ThrowIfBound();
                _referenceStencil = value;
            }
        }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for reading stencil data. By default, this value is <see cref="int.MaxValue" />.
        /// </summary>
        public int StencilReadMask
        {
            get => _stencilReadMask;
            set
            {
                ThrowIfBound();
                _stencilReadMask = value;
            }
        }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for writing stencil data. By default, this value is <see cref="int.MaxValue" />.
        /// </summary>
        public int StencilWriteMask
        {
            get => _stencilWriteMask;
            set
            {
                ThrowIfBound();
                _stencilWriteMask = value;
            }
        }

        /// <summary>
        /// Gets or sets if two sided stenciling is enabled, where if back face stencil testing/operations should be conducted in addition to the front face (as dictated by the winding order
        /// of the primitive). By default, this value is false.
        /// </summary>
        public bool TwoSidedStencilEnable
        {
            get => _twoSidedStencilEnable;
            set
            {
                ThrowIfBound();
                _twoSidedStencilEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets the comparison function used for testing a front facing triangle. By default, this value is <see cref="ComparisonFunction.Always" />.
        /// </summary>
        public ComparisonFunction StencilFunction
        {
            get => _stencilFunction;
            set
            {
                ThrowIfBound();
                _stencilFunction = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a front facing triangle. By default, this value is
        /// <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation StencilDepthFail
        {
            get => _stencilDepthFail;
            set
            {
                ThrowIfBound();
                _stencilDepthFail = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation StencilFail
        {
            get => _stencilFail;
            set
            {
                ThrowIfBound();
                _stencilFail = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation StencilPass
        {
            get => _stencilPass;
            set
            {
                ThrowIfBound();
                _stencilPass = value;
            }
        }

        /// <summary>
        /// Gets or sets the comparison function used for testing a back facing triangle. By default, this value is <see cref="ComparisonFunction.Always" />.
        /// </summary>
        public ComparisonFunction BackFaceStencilFunction
        {
            get => _backFaceStencilFunction;
            set
            {
                ThrowIfBound();
                _backFaceStencilFunction = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a back facing triangle. By default, this value is
        /// <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation BackFaceStencilDepthFail
        {
            get => _backFaceStencilDepthFail;
            set
            {
                ThrowIfBound();
                _backFaceStencilDepthFail = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation BackFaceStencilFail
        {
            get => _backFaceStencilFail;
            set
            {
                ThrowIfBound();
                _backFaceStencilFail = value;
            }
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep" />.
        /// </summary>
        public StencilOperation BackFaceStencilPass
        {
            get => _backFaceStencilPass;
            set
            {
                ThrowIfBound();
                _backFaceStencilPass = value;
            }
        }

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        public void BindDepthStencilState()
        {
            if (!IsBound)
            {
                CreateNativeState();
                IsBound = true;
            }
        }

        /// <summary>
        /// Called when the state is bound, signaling the implementation to create and bind the native state.
        /// </summary>
        protected abstract void CreateNativeState();

        /// <summary>
        /// Throws an exception if the state is already bound
        /// </summary>
        private void ThrowIfBound()
        {
            if (IsBound)
            {
                throw new InvalidOperationException("Render state has been bound and cannot be changed");
            }
        }
    }
}
