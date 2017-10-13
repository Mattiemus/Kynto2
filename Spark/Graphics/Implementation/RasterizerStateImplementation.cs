namespace Spark.Graphics.Implementation
{
    using System;

    /// <summary>
    /// Common base class for a <see cref="RasterizerState"/> implementation.
    /// </summary>
    public abstract class RasterizerStateImplementation : GraphicsResourceImplementation, IRasterizerStateImplementation
    {
        private CullMode _cull;
        private VertexWinding _vertexWinding;
        private FillMode _fill;
        private int _depthBias;
        private float _depthBiasClamp;
        private float _slopeScaledDepthBias;
        private bool _depthClipEnable;
        private bool _multiSampleEnable;
        private bool _antialiasedLineEnable;
        private bool _scissorTestEnable;

        /// <summary>
        /// Initializes a new instance of the <see cref="RasterizerStateImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        protected RasterizerStateImplementation(IRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
            IsBound = false;
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets if the <see cref="AntialiasedLineEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public abstract bool IsAntialiasedLineOptionSupported { get; }

        /// <summary>
        /// Gets if the <see cref="DepthClipEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public abstract bool IsDepthClipOptionSupported { get; }

        /// <summary>
        /// Gets or sets how primitives are to be culled. By default, this value is <see cref="CullMode.Back" />.
        /// </summary>
        public CullMode Cull
        {
            get => _cull;
            set
            {
                ThrowIfBound();
                _cull = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertex winding of a primitive, specifying the front face of the triangle. By default, this value is <see cref="Graphics.VertexWinding.CounterClockwise" />.
        /// </summary>
        public VertexWinding VertexWinding
        {
            get => _vertexWinding;
            set
            {
                ThrowIfBound();
                _vertexWinding = value;
            }
        }

        /// <summary>
        /// Gets or sets the fill mode of a primitive. By default, this value is <see cref="FillMode.Solid" />.
        /// </summary>
        public FillMode Fill
        {
            get => _fill;
            set
            {
                ThrowIfBound();
                _fill = value;
            }
        }

        /// <summary>
        /// Gets or sets the depth bias, which is a value added to the depth value at a given pixel. By default, this value is zero.
        /// </summary> 
        public int DepthBias
        {
            get => _depthBias;
            set
            {
                ThrowIfBound();
                _depthBias = value;
            }
        }

        /// <summary>
        /// Gets or sets the depth bias clamp (maximum value) of a pixel. By default, this value is zero.
        /// </summary>
        public float DepthBiasClamp
        {
            get => _depthBiasClamp;
            set
            {
                ThrowIfBound();
                _depthBiasClamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the slope scaled depth bias, a scalar on a given pixel's slope. By default, this value is zero.
        /// </summary>
        public float SlopeScaledDepthBias
        {
            get => _slopeScaledDepthBias;
            set
            {
                ThrowIfBound();
                _slopeScaledDepthBias = value;
            }
        }

        /// <summary>
        /// Gets or sets if depth clipping is enabled. If false, the hardware skips z-clipping. By default, this value is true.
        /// </summary>
        public bool DepthClipEnable
        {
            get => _depthClipEnable;
            set
            {
                ThrowIfBound();
                _depthClipEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to use the quadrilateral or alpha line anti-aliasing algorithm on MSAA render targets. If set to true, the quadrilaterla line anti-aliasing algorithm is used.
        /// Otherwise the alpha line-anti-aliasing algorithm is used, if <see cref="MultiSampleEnable"/> is set to false and is supported. By default, this value is true.
        /// </summary>
        public bool MultiSampleEnable
        {
            get => _multiSampleEnable;
            set
            {
                ThrowIfBound();
                _multiSampleEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to enable line antialising. This only applies if doing line drawing and <see cref="MultiSampleEnable" /> is set to false.
        /// </summary>
        public bool AntialiasedLineEnable
        {
            get => _antialiasedLineEnable;
            set
            {
                ThrowIfBound();
                _antialiasedLineEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets if scissor rectangle culling should be enabled or not. All pixels outside an active scissor rectangle are culled. By default, this value is set to false.
        /// </summary>
        public bool ScissorTestEnable
        {
            get => _scissorTestEnable;
            set
            {
                ThrowIfBound();
                _scissorTestEnable = value;
            }
        }

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        public void BindRasterizerState()
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
