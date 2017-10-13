namespace Spark.Graphics.Implementation
{
    using System;

    using Math;

    /// <summary>
    /// Common base class for a <see cref="SamplerState"/> implementation.
    /// </summary>
    public abstract class SamplerStateImplementation : GraphicsResourceImplementation, ISamplerStateImplementation
    {
        private TextureAddressMode _addressU;
        private TextureAddressMode _addressV;
        private TextureAddressMode _addressW;
        private TextureFilter _filter;
        private int _maxAnisotropy;
        private float _mipMapLevelOfDetailBias;
        private int _minMipMapLevel;
        private int _maxMipMapLevel;
        private Color _borderColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplerStateImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceId">ID of the resource, supplied by the render system</param>
        protected SamplerStateImplementation(IRenderSystem renderSystem, int resourceId)
            : base(renderSystem, resourceId)
        {
            IsBound = false;
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets the number of anisotropy levels supported. This can vary by implementation.
        /// </summary>
        public abstract int SupportedAnisotropyLevels { get; }

        /// <summary>
        /// Gets or sets the addressing mode for the U coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressU
        {
            get => _addressU;
            set
            {
                ThrowIfBound();
                _addressU = value;
            }
        }

        /// <summary>
        /// Gets or sets the addressing mode for the V coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressV
        {
            get => _addressV;
            set
            {
                ThrowIfBound();
                _addressV = value;
            }
        }

        /// <summary>
        /// Gets or sets the addressing mode for the W coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressW
        {
            get => _addressW;
            set
            {
                ThrowIfBound();
                _addressW = value;
            }
        }

        /// <summary>
        /// Gets or sets the filtering used during texture sampling. By default, this value is <see cref="TextureFilter.Linear" />.
        /// </summary>
        public TextureFilter Filter
        {
            get => _filter;
            set
            {
                ThrowIfBound();
                _filter = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum anisotropy. This is used to clamp values when the filter is set to anisotropic. By default, this value is
        /// <see cref="SupportedAnisotropyLevels" /> as it can vary by implementation. The minimum value is always one. If a value is higher or lower than
        /// these limits, it is clamped. 
        /// </summary>
        public int MaxAnisotropy
        {
            get => _maxAnisotropy;
            set
            {
                ThrowIfBound();
                _maxAnisotropy = MathHelper.Clamp(value, 1, SupportedAnisotropyLevels);
            }
        }

        /// <summary>
        /// Gets or sets the mipmap LOD bias. This is the offset from the calculated mipmap level that is actually used (e.g. sampled at mipmap level 3 with offset 2, then the
        /// mipmap at level 5 is sampled). By default, this value is zero.
        /// </summary>
        public float MipMapLevelOfDetailBias
        {
            get => _mipMapLevelOfDetailBias;
            set
            {
                ThrowIfBound();
                _mipMapLevelOfDetailBias = value;
            }
        }

        /// <summary>
        /// Gets or sets the lower bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is zero.
        /// </summary>
        public int MinMipMapLevel
        {
            get => _minMipMapLevel;
            set
            {
                ThrowIfBound();
                _minMipMapLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the upper bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is <see cref="int.MaxValue" />.
        /// </summary>
        public int MaxMipMapLevel
        {
            get => _maxMipMapLevel;
            set
            {
                ThrowIfBound();
                _maxMipMapLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the border color if the texture addressing is set to border. By default, this value is <see cref="Color.TransparentBlack" />.
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                ThrowIfBound();
                _borderColor = value;
            }
        }

        /// <summary>
        /// Checks if the specified texture addressing mode is supported by the graphics platform.
        /// </summary>
        /// <param name="mode">Texture addressing mode</param>
        /// <returns>True if supported, false otherwise.</returns>
        public abstract bool IsAddressingModeSupported(TextureAddressMode mode);

        /// <summary>
        /// Binds the implementation, creating the underlying state. Once bound the state is read-only. If unbound, this will happen
        /// automatically when the state is first used during rendering. It is best practice to do this ahead of time.
        /// </summary>
        public void BindSamplerState()
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
