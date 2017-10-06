namespace Spark.Graphics
{
    using System;

    using Core;
    using Math;
    using Content;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a render state that controls how a texture is sampled.
    /// </summary>
    public class SamplerState : RenderState, IShaderResource
    {
        private static IRenderSystem _renderSystem;

        private RenderStateKey _key;

        /// <summary>
        /// Wire up engine events to get prebuilt render states as render systems are added/changed/removed
        /// </summary>
        static SamplerState()
        {
            Engine.Initialized += HandleEngineInitialized;
            Engine.Destroyed += HandleEngineDestroyed;

            Engine engine = Engine.Instance;

            if (engine != null)
            {
                HandleEngineInitialized(engine, EventArgs.Empty);

                _renderSystem = engine.Services.GetService<IRenderSystem>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplerState"/> class.
        /// </summary>
        protected SamplerState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplerState"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        public SamplerState(IRenderSystem renderSystem)
        {
            CreateImplementation(renderSystem);
            SetDefaults();
        }

        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates wrap.
        /// </summary>
        public static SamplerState PointWrap
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.PointWrap;
            }
        }

        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates are clamped in the range of [0, 1]. This
        /// is the default state.
        /// </summary>
        public static SamplerState PointClamp
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.PointClamp;
            }
        }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates wrap.
        /// </summary>
        public static SamplerState LinearWrap
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.LinearWrap;
            }
        }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates are clamped in the range of [0, 1].
        /// </summary>
        public static SamplerState LinearClamp
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.LinearClamp;
            }
        }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates wrap.
        /// </summary>
        public static SamplerState AnisotropicWrap
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.AnisotropicWrap;
            }
        }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates are
        /// clamped in the range of [0, 1].
        /// </summary>
        public static SamplerState AnisotropicClamp
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedSamplerStates.AnisotropicClamp;
            }
        }
        
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public override bool IsBound => SamplerStateImpl.IsBound;

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public override RenderStateType StateType => RenderStateType.SamplerState;

        /// <summary>
        /// Gets the key that identifies this render state type and configuration for comparing states.
        /// </summary>
        public override RenderStateKey RenderStateKey
        {
            get
            {
                if (!SamplerStateImpl.IsBound)
                {
                    return ComputeRenderStateKey();
                }

                return _key;
            }
        }
        
        /// <summary>
        /// Gets the number of anisotropy levels supported. This can vary by implementation.
        /// </summary>
        public int SupportedAnisotropyLevels => SamplerStateImpl.SupportedAnisotropyLevels;

        /// <summary>
        /// Gets or sets the addressing mode for the U coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressU
        {
            get => SamplerStateImpl.AddressU;
            set => SamplerStateImpl.AddressU = value;
        }

        /// <summary>
        /// Gets or sets the addressing mode for the V coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressV
        {
            get => SamplerStateImpl.AddressV;
            set => SamplerStateImpl.AddressV = value;
        }

        /// <summary>
        /// Gets or sets the addressing mode for the W coordinate. By default, this value is <see cref="TextureAddressMode.Clamp" />.
        /// </summary>
        public TextureAddressMode AddressW
        {
            get => SamplerStateImpl.AddressW;
            set => SamplerStateImpl.AddressW = value;
        }

        /// <summary>
        /// Gets or sets the filtering used during texture sampling. By default, this value is <see cref="TextureFilter.Linear" />.
        /// </summary>
        public TextureFilter Filter
        {
            get => SamplerStateImpl.Filter;
            set => SamplerStateImpl.Filter = value;
        }

        /// <summary>
        /// Gets or sets the maximum anisotropy. This is used to clamp values when the filter is set to anisotropic. By default, this value is set to 1 and can be set up
        /// to <see cref="SupportedAnisotropyLevels" />, which can vary by implementation. If a higher or lower value is set than supported, it is clamped.
        /// </summary>
        public int MaxAnisotropy
        {
            get => SamplerStateImpl.MaxAnisotropy;
            set => SamplerStateImpl.MaxAnisotropy = value;
        }

        /// <summary>
        /// Gets or sets the mipmap LOD bias. This is the offset from the calculated mipmap level that is actually used (e.g. sampled at mipmap level 3 with offset 2, then the
        /// mipmap at level 5 is sampled). By default, this value is zero.
        /// </summary>
        public float MipMapLevelOfDetailBias
        {
            get => SamplerStateImpl.MipMapLevelOfDetailBias;
            set => SamplerStateImpl.MipMapLevelOfDetailBias = value;
        }

        /// <summary>
        /// Gets or sets the lower bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is zero.
        /// </summary>
        public int MinMipMapLevel
        {
            get => SamplerStateImpl.MinMipMapLevel;
            set => SamplerStateImpl.MinMipMapLevel = value;
        }

        /// <summary>
        /// Gets or sets the upper bound of the mipmap range [0, n-1] to clamp access to, where zero is the largest and most detailed mipmap level. The level n-1 is the least detailed mipmap level.
        /// By default, this value is <see cref="int.MaxValue" />.
        /// </summary>
        public int MaxMipMapLevel
        {
            get => SamplerStateImpl.MaxMipMapLevel;
            set => SamplerStateImpl.MaxMipMapLevel = value;
        }

        /// <summary>
        /// Gets or sets the border color if the texture addressing is set to border. By default, this value is <see cref="Color.TransparentBlack" />.
        /// </summary>
        public Color BorderColor
        {
            get => SamplerStateImpl.BorderColor;
            set => SamplerStateImpl.BorderColor = value;
        }

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public ShaderResourceType ResourceType => ShaderResourceType.SamplerState;

        /// <summary>
        /// Gets or sets the sampler state implementation
        /// </summary>
        private ISamplerStateImplementation SamplerStateImpl
        {
            get => Implementation as ISamplerStateImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Binds the render state to the graphics pipeline. If not called after the state is created, it is automatically done the first time the render state
        /// is applied. Once bound, the render state becomes immutable.
        /// </summary>
        public override void BindRenderState()
        {
            if (!SamplerStateImpl.IsBound)
            {
                SamplerStateImpl.BindSamplerState();
                _key = ComputeRenderStateKey();
            }
        }
        
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);
            CreateImplementation(renderSystem);
            ISamplerStateImplementation impl = SamplerStateImpl;

            Name = input.ReadString();
            impl.AddressU = input.ReadEnum<TextureAddressMode>();
            impl.AddressV = input.ReadEnum<TextureAddressMode>();
            impl.AddressW = input.ReadEnum<TextureAddressMode>();
            impl.Filter = input.ReadEnum<TextureFilter>();
            impl.MipMapLevelOfDetailBias = input.ReadSingle();
            impl.MinMipMapLevel = input.ReadInt32();
            impl.MaxMipMapLevel = input.ReadInt32();
            impl.MaxAnisotropy = input.ReadInt32();
            impl.BorderColor = input.Read<Color>();

            impl.BindSamplerState();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            ISamplerStateImplementation impl = SamplerStateImpl;

            output.Write("Name", Name);
            output.WriteEnum("AddressU", impl.AddressU);
            output.WriteEnum("AddressV", impl.AddressV);
            output.WriteEnum("AddressW", impl.AddressW);
            output.WriteEnum("Filter", impl.Filter);
            output.Write("MipMapLevelOfDetailBias", impl.MipMapLevelOfDetailBias);
            output.Write("MinMipMapLevel", impl.MinMipMapLevel);
            output.Write("MaxMipMapLevel", impl.MaxMipMapLevel);
            output.Write("MaxAnisotropy", impl.MaxAnisotropy);
            output.Write("BorderColor", impl.BorderColor);
        }

        /// <summary>
        /// Create the sampler state implementation
        /// </summary>
        /// <param name="renderSystem">Render system to use when creating the implementation</param>
        private void CreateImplementation(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }
            
            if (!renderSystem.TryGetImplementationFactory(out ISamplerStateImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                SamplerStateImpl = factory.CreateImplementation();
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Sets all values to their defaults
        /// </summary>
        private void SetDefaults()
        {
            ISamplerStateImplementation impl = SamplerStateImpl;

            impl.AddressU = TextureAddressMode.Clamp;
            impl.AddressV = TextureAddressMode.Clamp;
            impl.AddressW = TextureAddressMode.Clamp;
            impl.Filter = TextureFilter.Linear;
            impl.MaxAnisotropy = 1;
            impl.MipMapLevelOfDetailBias = 0.0f;
            impl.MinMipMapLevel = 0;
            impl.MaxMipMapLevel = int.MaxValue;
            impl.BorderColor = Color.White;
        }

        /// <summary>
        /// Calculates the render state key value
        /// </summary>
        /// <returns>Render state key</returns>
        private RenderStateKey ComputeRenderStateKey()
        {
            unchecked
            {
                ISamplerStateImplementation impl = SamplerStateImpl;

                int hash = 17;

                hash = (hash * 31) + StateType.GetHashCode();
                hash = (hash * 31) + impl.AddressU.GetHashCode();
                hash = (hash * 31) + impl.AddressV.GetHashCode();
                hash = (hash * 31) + impl.AddressW.GetHashCode();
                hash = (hash * 31) + impl.Filter.GetHashCode();
                hash = (hash * 31) + impl.MaxAnisotropy;
                hash = (hash * 31) + impl.MinMipMapLevel;
                hash = (hash * 31) + (impl.MaxMipMapLevel % int.MaxValue);
                hash = (hash * 31) + impl.MipMapLevelOfDetailBias.GetHashCode();
                hash = (hash * 31) + impl.BorderColor.GetHashCode();

                return new RenderStateKey(StateType, hash);
            }
        }

        /// <summary>
        /// Invoked when the engine is initialized
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleEngineInitialized(Engine sender, EventArgs args)
        {
            sender.Services.ServiceAdded += HandleServiceAdded;
            sender.Services.ServiceReplaced += HandleServiceChanged;
            sender.Services.ServiceRemoved += HandleServiceRemoved;
        }

        /// <summary>
        /// Invoked when the engine is destroyed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleEngineDestroyed(Engine sender, EventArgs args)
        {
            sender.Services.ServiceAdded -= HandleServiceAdded;
            sender.Services.ServiceReplaced -= HandleServiceChanged;
            sender.Services.ServiceRemoved -= HandleServiceRemoved;
        }

        /// <summary>
        /// Invoked when an engine service is added
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceAdded(EngineServiceRegistry sender, EngineServiceEventArgs args)
        {
            if (args.ServiceType == typeof(IRenderSystem))
            {
                _renderSystem = args.Service as IRenderSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceChanged(EngineServiceRegistry sender, EngineServiceReplacedEventArgs args)
        {
            if (args.ServiceType == typeof(IRenderSystem))
            {
                _renderSystem = args.NewService as IRenderSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is removed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceRemoved(EngineServiceRegistry sender, EngineServiceEventArgs args)
        {
            if (args.ServiceType == typeof(IRenderSystem))
            {
                _renderSystem = null;
            }
        }
    }
}
