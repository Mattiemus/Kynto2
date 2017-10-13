namespace Spark.Graphics.Implementation
{
    using System;

    using Math;

    /// <summary>
    /// Common base class for a <see cref="BlendState"/> implementation.
    /// </summary>
    public abstract class BlendStateImplementation : GraphicsResourceImplementation, IBlendStateImplementation
    {
        private bool _alphaToCoverageEnable;
        private bool _independentBlendEnable;
        private Color _blendFactor;
        private int _multiSampleMask;

        private readonly RenderTargetBlendDescription[] _renderTargetBlends;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendStateImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        protected BlendStateImplementation(IRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
            _renderTargetBlends = new RenderTargetBlendDescription[RenderTargetBlendCount];
            IsBound = false;
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes immutable.
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets the number of render targets that allow for independent blending. This can vary by implementation, at least one is always guaranteed.
        /// </summary>
        public abstract int RenderTargetBlendCount { get; }

        /// <summary>
        /// Checks if alpha-to-coverage is supported. This can vary by implementation.
        /// </summary>
        public abstract bool IsAlphaToCoverageSupported { get; }

        /// <summary>
        /// Checks if independent blending of multiple render targets (MRT) is supported. This can vary by implementation. If not supported, then the blending options
        /// specified for the first render target index are used for all other bound render targets, if those targets blend are enabled.
        /// </summary>
        public abstract bool IsIndependentBlendSupported { get; }

        /// <summary>
        /// Gets or sets whether alpha-to-coverage should be used as a multisampling technique when writing a pixel to a render target. Support for this may vary by implementation. By
        /// default, this value is false.
        /// </summary>
        public bool AlphaToCoverageEnable
        {
            get => _alphaToCoverageEnable;
            set
            {
                ThrowIfBound();
                _alphaToCoverageEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets whether independent blending is enabled for multiple render targets (MRT). If this is false, the blending options specified for the first render target index
        /// is used for all render targets currently bound. Support for this may vary by implementation. By default, this value is false.
        /// </summary>
        public bool IndependentBlendEnable
        {
            get => _independentBlendEnable;
            set
            {
                ThrowIfBound();
                _independentBlendEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets the blend factor color. By default, this value is <see cref="Color.White" />.
        /// </summary>
        public Color BlendFactor
        {
            get => _blendFactor;
            set
            {
                ThrowIfBound();
                _blendFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the multisample mask. By default, this value is 0xffffffff.
        /// </summary>
        public int MultiSampleMask
        {
            get => _multiSampleMask;
            set
            {
                ThrowIfBound();
                _multiSampleMask = value;
            }
        }

        /// <summary>
        /// Gets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        public void GetRenderTargetBlendDescription(int renderTargetIndex, out RenderTargetBlendDescription blendDesc)
        {
            ThrowIfIndexOutOfRange(renderTargetIndex);
            blendDesc = _renderTargetBlends[renderTargetIndex];
        }

        /// <summary>
        /// Sets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        public void SetRenderTargetBlendDescription(int renderTargetIndex, ref RenderTargetBlendDescription blendDesc)
        {
            ThrowIfBound();
            ThrowIfIndexOutOfRange(renderTargetIndex);
            _renderTargetBlends[renderTargetIndex] = blendDesc;
        }

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        public void BindBlendState()
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

        /// <summary>
        /// Throws an exception if the given render target index is outside the supported range
        /// </summary>
        /// <param name="renderTargetIndex">Render target index</param>
        private void ThrowIfIndexOutOfRange(int renderTargetIndex)
        {
            if (renderTargetIndex < 0 || renderTargetIndex >= _renderTargetBlends.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(renderTargetIndex), "Index is out of range");
            }
        }
    }
}
