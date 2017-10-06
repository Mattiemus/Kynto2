namespace Spark.Graphics
{
    using System;

    using Core;
    using Content;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a render state that controls how geometry is rasterized.
    /// </summary>
    public class RasterizerState : RenderState
    {
        private static IRenderSystem _renderSystem;

        private RenderStateKey _key;
        
        /// <summary>
        /// Wire up engine events to get prebuilt render states as render systems are added/changed/removed
        /// </summary>
        static RasterizerState()
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
        /// Initializes a new instance of the <see cref="RasterizerState"/> class.
        /// </summary>
        protected RasterizerState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RasterizerState"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the render system is null.</exception>
        /// <exception cref="TeslaGraphicsException">Thrown if creating the underlying implementation fails or is unsupported, see inner exception for a more detailed error.</exception>
        public RasterizerState(IRenderSystem renderSystem)
        {
            CreateImplementation(renderSystem);
            SetDefaults();
        }

        /// <summary>
        /// Gets a predefined state object where culling is disabled.
        /// </summary>
        public static RasterizerState CullNone
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedRasterizerStates.CullNone;
            }
        }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have
        /// a clockwise vertex winding. This is the default state.
        /// </summary>
        public static RasterizerState CullBackClockwiseFront
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedRasterizerStates.CullBackClockwiseFront;
            }
        }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have a counterclockwise
        /// vertex winding.
        /// </summary>
        public static RasterizerState CullBackCounterClockwiseFront
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedRasterizerStates.CullBackCounterClockwiseFront;
            }
        }

        /// <summary>
        /// Gets a predefined state object where culling is disabled and fillmode is wireframe.
        /// </summary>
        public static RasterizerState CullNoneWireframe
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedRasterizerStates.CullNoneWireframe;
            }
        }
        
        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public override bool IsBound => RasterizerStateImplementation.IsBound;

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public override RenderStateType StateType => RenderStateType.RasterizerState;
            
        /// <summary>
        /// Gets the key that identifies this render state type and configuration for comparing states.
        /// </summary>
        public override RenderStateKey RenderStateKey
        {
            get
            {
                if (!RasterizerStateImplementation.IsBound)
                {
                    return ComputeRenderStateKey();
                }

                return _key;
            }
        }
        
        /// <summary>
        /// Gets if the <see cref="AntialiasedLineEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public bool IsAntialiasedLineOptionSupported => RasterizerStateImplementation.IsAntialiasedLineOptionSupported;

        /// <summary>
        /// Gets if the <see cref="DepthClipEnable" /> property is supported. This can vary by implementation.
        /// </summary>
        public bool IsDepthClipOptionSupported => RasterizerStateImplementation.IsDepthClipOptionSupported;

        /// <summary>
        /// Gets or sets how primitives are to be culled. By default, this value is <see cref="CullMode.Back" />.
        /// </summary>
        public CullMode Cull
        {
            get => RasterizerStateImplementation.Cull;
            set => RasterizerStateImplementation.Cull = value;
        }

        /// <summary>
        /// Gets or sets the vertex winding of a primitive, specifying the front face of the triangle. By default, this value is <see cref="Graphics.VertexWinding.CounterClockwise" />.
        /// </summary>
        public VertexWinding VertexWinding
        {
            get => RasterizerStateImplementation.VertexWinding;
            set => RasterizerStateImplementation.VertexWinding = value;
        }

        /// <summary>
        /// Gets or sets the fill mode of a primitive. By default, this value is <see cref="FillMode.Solid" />.
        /// </summary>
        public FillMode Fill
        {
            get => RasterizerStateImplementation.Fill;
            set => RasterizerStateImplementation.Fill = value;
        }

        /// <summary>
        /// Gets or sets the depth bias, which is a value added to the depth value at a given pixel. By default, this value is zero.
        /// </summary>
        public int DepthBias
        {
            get => RasterizerStateImplementation.DepthBias;
            set => RasterizerStateImplementation.DepthBias = value;
        }

        /// <summary>
        /// Gets or sets the depth bias clamp (maximum value) of a pixel. By default, this value is zero.
        /// </summary>
        public float DepthBiasClamp
        {
            get => RasterizerStateImplementation.DepthBiasClamp;
            set => RasterizerStateImplementation.DepthBiasClamp = value;
        }

        /// <summary>
        /// Gets or sets the slope scaled depth bias, a scalar on a given pixel's slope. By default, this value is zero.
        /// </summary>
        public float SlopeScaledDepthBias
        {
            get => RasterizerStateImplementation.SlopeScaledDepthBias;
            set => RasterizerStateImplementation.SlopeScaledDepthBias = value;
        }

        /// <summary>
        /// Gets or sets if depth clipping is enabled. If false, the hardware skips z-clipping. By default, this value is true.
        /// </summary>
        public bool DepthClipEnable
        {
            get => RasterizerStateImplementation.DepthClipEnable;
            set => RasterizerStateImplementation.DepthClipEnable = value;
        }

        /// <summary>
        /// Gets or sets whether to use the quadrilateral or alpha line anti-aliasing algorithm on MSAA render targets. If set to true, the quadrilaterla line anti-aliasing algorithm is used.
        /// Otherwise the alpha line-anti-aliasing algorithm is used, if <see cref="MultiSampleEnable"/> is set to false and is supported. By default, this value is true.
        /// </summary>
        public bool MultiSampleEnable
        {
            get => RasterizerStateImplementation.MultiSampleEnable;
            set => RasterizerStateImplementation.MultiSampleEnable = value;
        }

        /// <summary>
        /// Gets or sets whether to enable line antialising. This only applies if doing line drawing and <see cref="MultiSampleEnable" /> is set to false. 
        /// </summary>
        public bool AntialiasedLineEnable
        {
            get => RasterizerStateImplementation.AntialiasedLineEnable;
            set => RasterizerStateImplementation.AntialiasedLineEnable = value;
        }

        /// <summary>
        /// Gets or sets if scissor rectangle culling should be enabled or not. All pixels outside an active scissor rectangle are culled. By default, this value is set to false.
        /// </summary>
        public bool ScissorTestEnable
        {
            get => RasterizerStateImplementation.ScissorTestEnable;
            set => RasterizerStateImplementation.ScissorTestEnable = value;
        }
        
        /// <summary>
        /// Gets or sets the rasterizer state implementation
        /// </summary>
        private IRasterizerStateImplementation RasterizerStateImplementation
        {
            get => Implementation as IRasterizerStateImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Binds the render state to the graphics pipeline. If not called after the state is created, it is automatically done the first time the render state
        /// is applied. Once bound, the render state becomes immutable.
        /// </summary>
        public override void BindRenderState()
        {
            if (!RasterizerStateImplementation.IsBound)
            {
                RasterizerStateImplementation.BindRasterizerState();
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
            IRasterizerStateImplementation impl = RasterizerStateImplementation;

            Name = input.ReadString();
            impl.Cull = input.ReadEnum<CullMode>();
            impl.VertexWinding = input.ReadEnum<VertexWinding>();
            impl.Fill = input.ReadEnum<FillMode>();
            impl.DepthBias = input.ReadInt32();
            impl.DepthBiasClamp = input.ReadSingle();
            impl.SlopeScaledDepthBias = input.ReadSingle();
            impl.DepthClipEnable = input.ReadBoolean();
            impl.MultiSampleEnable = input.ReadBoolean();
            impl.AntialiasedLineEnable = input.ReadBoolean();
            impl.ScissorTestEnable = input.ReadBoolean();

            impl.BindRasterizerState();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            IRasterizerStateImplementation impl = RasterizerStateImplementation;

            output.Write("Name", Name);
            output.WriteEnum("Cull", impl.Cull);
            output.WriteEnum("VertexWinding", impl.VertexWinding);
            output.WriteEnum("Fill", impl.Fill);
            output.Write("DepthBias", impl.DepthBias);
            output.Write("DepthBiasClamp", impl.DepthBiasClamp);
            output.Write("SlopeScaledDepthBias", impl.SlopeScaledDepthBias);
            output.Write("DepthClipEnable", impl.DepthClipEnable);
            output.Write("MultiSampleEnable", impl.MultiSampleEnable);
            output.Write("AntialiasedLineEnable", impl.AntialiasedLineEnable);
            output.Write("ScissorTestEnable", impl.ScissorTestEnable);
        }

        /// <summary>
        /// Create the rasterizer state implementation
        /// </summary>
        /// <param name="renderSystem">Render system to use when creating the implementation</param>
        private void CreateImplementation(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (!renderSystem.TryGetImplementationFactory(out IRasterizerStateImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                RasterizerStateImplementation = factory.CreateImplementation();
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
            IRasterizerStateImplementation impl = RasterizerStateImplementation;

            impl.Cull = CullMode.Back;
            impl.VertexWinding = VertexWinding.Clockwise;
            impl.Fill = FillMode.Solid;
            impl.DepthBias = 0;
            impl.DepthBiasClamp = 0.0f;
            impl.SlopeScaledDepthBias = 0.0f;
            impl.DepthClipEnable = true;
            impl.MultiSampleEnable = true;
            impl.AntialiasedLineEnable = false;
            impl.ScissorTestEnable = false;
        }

        /// <summary>
        /// Calculates the render state key value
        /// </summary>
        /// <returns>Render state key</returns>
        private RenderStateKey ComputeRenderStateKey()
        {
            unchecked
            {
                IRasterizerStateImplementation impl = RasterizerStateImplementation;

                int hash = 17;

                hash = (hash * 31) + StateType.GetHashCode();
                hash = (hash * 31) + impl.Cull.GetHashCode();
                hash = (hash * 31) + impl.VertexWinding.GetHashCode();
                hash = (hash * 31) + impl.Fill.GetHashCode();
                hash = (hash * 31) + impl.DepthBias;
                hash = (hash * 31) + impl.DepthBiasClamp.GetHashCode();
                hash = (hash * 31) + impl.SlopeScaledDepthBias.GetHashCode();
                hash = (hash * 31) + ((impl.DepthClipEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.MultiSampleEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.AntialiasedLineEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.ScissorTestEnable) ? 1 : 0);

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
