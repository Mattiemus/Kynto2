namespace Spark.Graphics
{
    using System;

    using Math;
    using Content;
    using Implementation;

    /// <summary>
    /// Represents a render state that controls how pixels are blended during rendering. Blending is specified on a per-render target basis.
    /// </summary>
    public class BlendState : RenderState
    {
        private static IRenderSystem _renderSystem;

        private RenderStateKey _key;

        /// <summary>
        /// Wire up engine events to get prebuilt render states as render systems are added/changed/removed
        /// </summary>
        static BlendState()
        {
            SparkEngine.Initialized += HandleEngineInitialized;
            SparkEngine.Destroyed += HandleEngineDestroyed;

            SparkEngine engine = SparkEngine.Instance;

            if (engine != null)
            {
                HandleEngineInitialized(engine, EventArgs.Empty);

                _renderSystem = engine.Services.GetService<IRenderSystem>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendState"/> class.
        /// </summary>
        protected BlendState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendState"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        public BlendState(IRenderSystem renderSystem)
        {
            CreateImplementation(renderSystem);
            SetDefaults();
        }

        /// <summary>
        /// Gets a predefined state object for opaque blending, where no blending occurs and destination color overwrites source color. This is 
        /// the default state.
        /// </summary>
        public static BlendState Opaque
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedBlendStates.Opaque;
            }
        }

        /// <summary>
        /// Gets a predefined state object for premultiplied alpha blending, where source and destination colors are blended by using
        /// alpha, and where the color contains alpha pre-multiplied.
        /// </summary>
        public static BlendState AlphaBlendPremultiplied
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedBlendStates.AlphaBlendPremultiplied;
            }
        }

        /// <summary>
        /// Gets a predefined state object for additive blending, where source and destination color are blended without using alpha.
        /// </summary>
        public static BlendState AdditiveBlend
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedBlendStates.AdditiveBlend;
            }
        }

        /// <summary>
        ///Gets a predefined state object for non-premultiplied alpha blending, where the source and destination color are blended by using alpha,
        ///and where the color does not contain the alpha pre-multiplied.
        /// </summary>
        public static BlendState AlphaBlendNonPremultiplied
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedBlendStates.AlphaBlendNonPremultiplied;
            }
        }
        
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public override bool IsBound => BlendStateImplementation.IsBound;

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public override RenderStateType StateType => RenderStateType.BlendState;

        /// <summary>
        /// Gets the key that identifies this render state type and configuration for comparing states.
        /// </summary>
        public override RenderStateKey RenderStateKey
        {
            get
            {
                if (!BlendStateImplementation.IsBound)
                {
                    return ComputeRenderStateKey();
                }

                return _key;
            }
        }
        
        /// <summary>
        /// Gets the number of render targets that allow for independent blending. This can vary by implementation, at least one is always guaranteed.
        /// </summary>
        public int RenderTargetBlendCount => BlendStateImplementation.RenderTargetBlendCount;

        /// <summary>
        /// Checks if alpha-to-coverage is supported. This can vary by implementation.
        /// </summary>
        public bool IsAlphaToCoverageSupported => BlendStateImplementation.IsAlphaToCoverageSupported;

        /// <summary>
        /// Checks if independent blending of multiple render targets (MRT) is supported. This can vary by implementation. If not supported, then the blending options
        /// specified for the first render target index are used for all other bound render targets, if those targets blend are enabled.
        /// </summary>
        public bool IsIndependentBlendSupported => BlendStateImplementation.IsIndependentBlendSupported;

        /// <summary>
        /// Gets or sets whether alpha-to-coverage should be used as a multisampling technique when writing a pixel to a render target. Support for this may vary by implementation. By
        /// default, this value is false.
        /// </summary>
        public bool AlphaToCoverageEnable
        {
            get => BlendStateImplementation.AlphaToCoverageEnable;
            set => BlendStateImplementation.AlphaToCoverageEnable = value;
        }

        /// <summary>
        /// Gets or sets whether independent blending is enabled for multiple render targets (MRT). If this is false, the blending options specified for the first render target index
        /// is used for all render targets currently bound. Support for this may vary by implementation. By default, this value is false.
        /// </summary>
        public bool IndependentBlendEnable
        {
            get => BlendStateImplementation.IndependentBlendEnable;
            set => BlendStateImplementation.IndependentBlendEnable = value;
        }

        /// <summary>
        /// Gets or sets the blend factor color. By default, this value is <see cref="Color.White" />.
        /// </summary>
        public Color BlendFactor
        {
            get => BlendStateImplementation.BlendFactor;
            set => BlendStateImplementation.BlendFactor = value;
        }

        /// <summary>
        /// Gets or sets the multisample mask. By default, this value is 0xffffffff.
        /// </summary>
        public int MultiSampleMask
        {
            get => BlendStateImplementation.MultiSampleMask;
            set => BlendStateImplementation.MultiSampleMask = value;
        }
        
        /// <summary>
        /// Gets or sets if blending is enabled for the first render target. By default, this value is false.
        /// </summary>
        public bool BlendEnable
        {
            get => GetBlendEnable(0);
            set => SetBlendEnable(0, value);
        }

        /// <summary>
        /// Gets or sets the alpha blend function for the first render target. By default, this value is <see cref="BlendFunction.Add"/>.
        /// </summary>
        public BlendFunction AlphaBlendFunction
        {
            get => GetAlphaBlendFunction(0);
            set => SetAlphaBlendFunction(0, value);
        }

        /// <summary>
        /// Gets or sets the alpha source blend for the first render target. By default, this value is <see cref="Blend.One"/>.
        /// </summary>
        public Blend AlphaSourceBlend
        {
            get => GetAlphaSourceBlend(0);
            set => SetAlphaSourceBlend(0, value);
        }

        /// <summary>
        /// Gets or sets the alpha destination blend for the first render target. By default, this value is <see cref="Blend.Zero"/>.
        /// </summary>
        public Blend AlphaDestinationBlend
        {
            get => GetAlphaDestinationBlend(0);
            set => SetAlphaDestinationBlend(0, value);
        }

        /// <summary>
        /// Gets or sets the color blend function for the first render target. By default, this value is <see cref="BlendFunction.Add"/>.
        /// </summary>
        public BlendFunction ColorBlendFunction
        {
            get => GetColorBlendFunction(0);
            set => SetColorBlendFunction(0, value);
        }

        /// <summary>
        /// Gets or sets the color source blend for the first render target. By default, this value is <see cref="Blend.One"/>.
        /// </summary>
        public Blend ColorSourceBlend
        {
            get => GetColorSourceBlend(0);
            set => SetColorSourceBlend(0, value);
        }

        /// <summary>
        /// Gets or sets the color destination blend for the first render target. By default, this value is <see cref="Blend.Zero"/>.
        /// </summary>
        public Blend ColorDestinationBlend
        {
            get => GetColorDestinationBlend(0);
            set => SetColorDestinationBlend(0, value);
        }

        /// <summary>
        /// Gets or sets the color write channels for the first render target. By default, this value is <see cref="ColorWriteChannels.All"/>.
        /// </summary>
        public ColorWriteChannels WriteChannels
        {
            get => GetWriteChannels(0);
            set => SetWriteChannels(0, value);
        }

        /// <summary>
        /// Gets or sets the blend state implementation
        /// </summary>
        private IBlendStateImplementation BlendStateImplementation
        {
            get => Implementation as IBlendStateImplementation;
            set => BindImplementation(value);
        }
        
        /// <summary>
        /// Gets if blending is enabled for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>True if blending is enabled, false otherwise.</returns>
        public bool GetBlendEnable(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.BlendEnable;
        }

        /// <summary>
        /// Sets if blending should be enabled for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendEnable">True if blending should be enabled, false otherwise.</param>
        public void SetBlendEnable(int renderTargetIndex, bool blendEnable)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.BlendEnable = blendEnable;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the alpha blend function for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The alpha blend function</returns>
        public BlendFunction GetAlphaBlendFunction(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.AlphaBlendFunction;
        }

        /// <summary>
        /// Sets the alpha blend function for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="alphaBlendFunction">Alpha blend function</param>
        public void SetAlphaBlendFunction(int renderTargetIndex, BlendFunction alphaBlendFunction)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.AlphaBlendFunction = alphaBlendFunction;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the alpha source blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The alpha source blend</returns>
        public Blend GetAlphaSourceBlend(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.AlphaSourceBlend;
        }

        /// <summary>
        /// Sets the alpha source blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="alphaSourceBlend">Alpha source blend to set</param>
        public void SetAlphaSourceBlend(int renderTargetIndex, Blend alphaSourceBlend)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.AlphaSourceBlend = alphaSourceBlend;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the alpha destination blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The alpha destination blend</returns>
        public Blend GetAlphaDestinationBlend(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.AlphaDestinationBlend;
        }

        /// <summary>
        /// Sets the alpha destination blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="alphaDestinationBlend">Alpha destination blend to set</param>
        public void SetAlphaDestinationBlend(int renderTargetIndex, Blend alphaDestinationBlend)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.AlphaDestinationBlend = alphaDestinationBlend;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the color blend function for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The color blend function</returns>
        public BlendFunction GetColorBlendFunction(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.ColorBlendFunction;
        }

        /// <summary>
        /// Sets the color blend function for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="colorBlendFunction">Color blend function</param>
        public void SetColorBlendFunction(int renderTargetIndex, BlendFunction colorBlendFunction)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.ColorBlendFunction = colorBlendFunction;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the color source blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The color source blend</returns>
        public Blend GetColorSourceBlend(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.ColorSourceBlend;
        }

        /// <summary>
        /// Sets the color source blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="colorSourceBlend">Color source blend to set</param>
        public void SetColorSourceBlend(int renderTargetIndex, Blend colorSourceBlend)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.ColorSourceBlend = colorSourceBlend;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the color destination blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The color destination blend</returns>
        public Blend GetColorDestinationBlend(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.ColorDestinationBlend;
        }

        /// <summary>
        /// Sets the color destination blend for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="colorDestinationBlend">Color destination blend to set</param>
        public void SetColorDestinationBlend(int renderTargetIndex, Blend colorDestinationBlend)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.ColorDestinationBlend = colorDestinationBlend;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the color write channels for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <returns>The color write channel mask</returns>
        public ColorWriteChannels GetWriteChannels(int renderTargetIndex)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);
            return blendDesc.WriteChannels;
        }

        /// <summary>
        /// Sets the color write channels for the render target at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="writeChannels">Color write channel mask to set</param>
        public void SetWriteChannels(int renderTargetIndex, ColorWriteChannels writeChannels)
        {
            IBlendStateImplementation impl = BlendStateImplementation;
            impl.GetRenderTargetBlendDescription(renderTargetIndex, out RenderTargetBlendDescription blendDesc);

            blendDesc.WriteChannels = writeChannels;
            impl.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Gets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        public void GetRenderTargetBlendDescription(int renderTargetIndex, out RenderTargetBlendDescription blendDesc)
        {
            BlendStateImplementation.GetRenderTargetBlendDescription(renderTargetIndex, out blendDesc);
        }

        /// <summary>
        /// Sets the complete blend description for a render target bound at the specified index.
        /// </summary>
        /// <param name="renderTargetIndex">Zero-based index of the render target that is bound to the pipeline.</param>
        /// <param name="blendDesc">The blend description that holds the blending options for the render target.</param>
        public void SetRenderTargetBlendDescription(int renderTargetIndex, ref RenderTargetBlendDescription blendDesc)
        {
            BlendStateImplementation.SetRenderTargetBlendDescription(renderTargetIndex, ref blendDesc);
        }

        /// <summary>
        /// Binds the render state to the graphics pipeline. If not called after the state is created, it is automatically done the first time the render state
        /// is applied. Once bound, the render state becomes immutable.
        /// </summary>
        public override void BindRenderState()
        {
            if (!BlendStateImplementation.IsBound)
            {
                BlendStateImplementation.BindBlendState();
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
            IBlendStateImplementation impl = BlendStateImplementation;

            Name = input.ReadString();
            impl.AlphaToCoverageEnable = input.ReadBoolean();
            impl.IndependentBlendEnable = input.ReadBoolean();
            impl.BlendFactor = input.Read<Color>();
            impl.MultiSampleMask = input.ReadInt32();

            int rtBlendCount = input.ReadInt32();
            int supportedCount = impl.RenderTargetBlendCount;
            for (int i = 0; i < rtBlendCount; i++)
            {
                RenderTargetBlendDescription desc = input.Read<RenderTargetBlendDescription>();
                if (i < supportedCount)
                {
                    impl.SetRenderTargetBlendDescription(i, ref desc);
                }
            }

            impl.BindBlendState();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            IBlendStateImplementation impl = BlendStateImplementation;

            output.Write("Name", Name);
            output.Write("AlphaToCoverageEnable", impl.AlphaToCoverageEnable);
            output.Write("IndependentBlendEnable", impl.IndependentBlendEnable);
            output.Write("BlendFactor", impl.BlendFactor);
            output.Write("MultiSampleMask", impl.MultiSampleMask);

            output.Write("RenderTargetBlendCount", impl.RenderTargetBlendCount);

            for (int i = 0; i < impl.RenderTargetBlendCount; i++)
            {
                impl.GetRenderTargetBlendDescription(i, out RenderTargetBlendDescription desc);
                output.Write($"RenderTargetBlendDescription{desc}", desc);
            }
        }

        /// <summary>
        /// Create the blend state implementation
        /// </summary>
        /// <param name="renderSystem">Render system to use when creating the implementation</param>
        private void CreateImplementation(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }
            
            if (!renderSystem.TryGetImplementationFactory(out IBlendStateImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                BlendStateImplementation = factory.CreateImplementation();
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
            IBlendStateImplementation impl = BlendStateImplementation;

            impl.AlphaToCoverageEnable = false;
            impl.IndependentBlendEnable = false;
            impl.BlendFactor = Color.White;
            impl.MultiSampleMask = int.MaxValue;

            for (int i = 0; i < impl.RenderTargetBlendCount; i++)
            {
                RenderTargetBlendDescription desc = RenderTargetBlendDescription.Default;

                if (i == 0)
                {
                    desc.BlendEnable = true; // If creating a blend state, most likely want the first one enabled
                }

                impl.SetRenderTargetBlendDescription(i, ref desc);
            }
        }

        /// <summary>
        /// Calculates the render state key value
        /// </summary>
        /// <returns>Render state key</returns>
        private RenderStateKey ComputeRenderStateKey()
        {
            unchecked
            {
                IBlendStateImplementation impl = BlendStateImplementation;

                int hash = 17;

                hash = (hash * 31) + StateType.GetHashCode();
                hash = (hash * 31) + ((impl.AlphaToCoverageEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.IndependentBlendEnable) ? 1 : 0);
                hash = (hash * 31) + impl.BlendFactor.GetHashCode();
                hash = (hash * 31) + (impl.MultiSampleMask % int.MaxValue);

                for (int i = 0; i < impl.RenderTargetBlendCount; i++)
                {
                    impl.GetRenderTargetBlendDescription(i, out RenderTargetBlendDescription desc);
                    if (!desc.IsDefault)
                    {
                        hash = (hash * 31) + i;
                        hash = (hash * 31) + desc.GetHashCode();
                    }
                }

                return new RenderStateKey(StateType, hash);
            }
        }

        /// <summary>
        /// Invoked when the engine is initialized
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleEngineInitialized(SparkEngine sender, EventArgs args)
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
        private static void HandleEngineDestroyed(SparkEngine sender, EventArgs args)
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
