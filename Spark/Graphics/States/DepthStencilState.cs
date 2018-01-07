namespace Spark.Graphics
{
    using Content;
    using Graphics.Implementation;
    using System;

    /// <summary>
    /// Represents a render state that configures the behavior of the depth-stencil buffer.
    /// </summary>
    public class DepthStencilState : RenderState
    {
        private static IRenderSystem _renderSystem;

        private RenderStateKey _key;

        /// <summary>
        /// Wire up engine events to get prebuilt render states as render systems are added/changed/removed
        /// </summary>
        static DepthStencilState()
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
        /// Initializes a new instance of the <see cref="DepthStencilState"/> class.
        /// </summary>
        protected DepthStencilState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthStencilState"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        public DepthStencilState(IRenderSystem renderSystem)
        {
            CreateImplementation(renderSystem);
            SetDefaults();
        }

        /// <summary>
        /// Binds the render state to the graphics pipeline. If not called after the state is created, it is automatically done the first time the render state
        /// is applied. Once bound, the render state becomes immutable.
        /// </summary>
        public override void BindRenderState()
        {
            if (!DepthStencilStateImplementation.IsBound)
            {
                DepthStencilStateImplementation.BindDepthStencilState();
                _key = ComputeRenderStateKey();
            }
        }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is disabled and writing to it is disabled.
        /// </summary>
        public static DepthStencilState None
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedDepthStencilStates.None;
            }
        }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabled and writing to it is disabled.
        /// </summary>
        public static DepthStencilState DepthWriteOff
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedDepthStencilStates.DepthWriteOff;
            }
        }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabld and writing to it is enabled.
        /// This is the default state.
        /// </summary>
        public static DepthStencilState Default
        {
            get
            {
                if (_renderSystem == null)
                {
                    return null;
                }

                return _renderSystem.PredefinedDepthStencilStates.Default;
            }
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public override bool IsBound => DepthStencilStateImplementation.IsBound;

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public override RenderStateType StateType => RenderStateType.DepthStencilState;

        /// <summary>
        /// Gets the key that identifies this render state type and configuration for comparing states.
        /// </summary>
        public override RenderStateKey RenderStateKey
        {
            get
            {
                if (!DepthStencilStateImplementation.IsBound)
                {
                    return ComputeRenderStateKey();
                }

                return _key;
            }
        }
        
        /// <summary>
        /// Gets or sets if the depth buffer should be enabled. By default, this value is true.
        /// </summary>
        public bool DepthEnable
        {
            get => DepthStencilStateImplementation.DepthEnable;
            set => DepthStencilStateImplementation.DepthEnable = value;
        }

        /// <summary>
        /// Gets or sets if the depth buffer should be writable. By default, this value is true.
        /// </summary>
        public bool DepthWriteEnable
        {
            get => DepthStencilStateImplementation.DepthWriteEnable;
            set => DepthStencilStateImplementation.DepthWriteEnable = value;
        }

        /// <summary>
        /// Gets or sets the depth comparison function for the depth test. By default, this value is <see cref="ComparisonFunction.LessEqual"/>.
        /// </summary>
        public ComparisonFunction DepthFunction
        {
            get => DepthStencilStateImplementation.DepthFunction;
            set => DepthStencilStateImplementation.DepthFunction = value;
        }

        /// <summary>
        /// Gets or sets if the stencil buffer should be enabled. By default, this value is false.
        /// </summary>
        public bool StencilEnable
        {
            get => DepthStencilStateImplementation.StencilEnable;
            set => DepthStencilStateImplementation.StencilEnable = value;
        }

        /// <summary>
        /// Gets or sets the reference stencil value used for stencil testing. By default, this value is zero.
        /// </summary>
        public int ReferenceStencil
        {
            get => DepthStencilStateImplementation.ReferenceStencil;
            set => DepthStencilStateImplementation.ReferenceStencil = value;
        }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for reading stencil data. By default, this value is <see cref="int.MaxValue"/>.
        /// </summary>
        public int StencilReadMask
        {
            get => DepthStencilStateImplementation.StencilReadMask;
            set => DepthStencilStateImplementation.StencilReadMask = value;
        }

        /// <summary>
        /// Gets or sets the value that identifies a portion of the depth-stencil buffer for writing stencil data. By default, this value is <see cref="int.MaxValue"/>.
        /// </summary>
        public int StencilWriteMask
        {
            get => DepthStencilStateImplementation.StencilWriteMask;
            set => DepthStencilStateImplementation.StencilWriteMask = value;
        }

        /// <summary>
        /// Gets or sets if two sided stenciling is enabled, where if back face stencil testing/operations should be conducted in addition to the front face (as dictated by the winding order
        /// of the primitive). By default, this value is false.
        /// </summary>
        public bool TwoSidedStencilEnable
        {
            get => DepthStencilStateImplementation.TwoSidedStencilEnable;
            set => DepthStencilStateImplementation.TwoSidedStencilEnable = value;
        }

        /// <summary>
        /// Gets or sets the comparison function used for testing a front facing triangle. By default, this value is <see cref="ComparisonFunction.Always"/>.
        /// </summary>
        public ComparisonFunction StencilFunction
        {
            get => DepthStencilStateImplementation.StencilFunction;
            set => DepthStencilStateImplementation.StencilFunction = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a front facing triangle. By default, this value is
        /// <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation StencilDepthFail
        {
            get => DepthStencilStateImplementation.StencilDepthFail;
            set => DepthStencilStateImplementation.StencilDepthFail = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation StencilFail
        {
            get => DepthStencilStateImplementation.StencilFail;
            set => DepthStencilStateImplementation.StencilFail = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a front facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation StencilPass
        {
            get => DepthStencilStateImplementation.StencilPass;
            set => DepthStencilStateImplementation.StencilPass = value;
        }

        /// <summary>
        /// Gets or sets the comparison function used for testing a back facing triangle. By default, this value is <see cref="ComparisonFunction.Always"/>.
        /// </summary>
        public ComparisonFunction BackFaceStencilFunction
        {
            get => DepthStencilStateImplementation.BackFaceStencilFunction;
            set => DepthStencilStateImplementation.BackFaceStencilFunction = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes, but the depth test fails for a back facing triangle. By default, this value is 
        /// <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation BackFaceStencilDepthFail
        {
            get => DepthStencilStateImplementation.BackFaceStencilDepthFail;
            set => DepthStencilStateImplementation.BackFaceStencilDepthFail = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test fails for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation BackFaceStencilFail
        {
            get => DepthStencilStateImplementation.BackFaceStencilFail;
            set => DepthStencilStateImplementation.BackFaceStencilFail = value;
        }

        /// <summary>
        /// Gets or sets the stencil operation done when the stencil test passes for a back facing triangle. By default, this value is <see cref="StencilOperation.Keep"/>.
        /// </summary>
        public StencilOperation BackFaceStencilPass
        {
            get => DepthStencilStateImplementation.BackFaceStencilPass;
            set => DepthStencilStateImplementation.BackFaceStencilPass = value;
        }

        /// <summary>
        /// Gets or sets the depth stencil state implementation
        /// </summary>
        private IDepthStencilStateImplementation DepthStencilStateImplementation
        {
            get => Implementation as IDepthStencilStateImplementation;
            set => BindImplementation(value);
        }
        
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);
            CreateImplementation(renderSystem);
            IDepthStencilStateImplementation impl = DepthStencilStateImplementation;

            Name = input.ReadString();
            impl.DepthEnable = input.ReadBoolean();
            impl.DepthWriteEnable = input.ReadBoolean();
            impl.DepthFunction = input.ReadEnum<ComparisonFunction>();
            impl.StencilEnable = input.ReadBoolean();
            impl.ReferenceStencil = input.ReadInt32();
            impl.StencilReadMask = input.ReadInt32();
            impl.StencilWriteMask = input.ReadInt32();
            impl.TwoSidedStencilEnable = input.ReadBoolean();
            impl.StencilFunction = input.ReadEnum<ComparisonFunction>();
            impl.StencilDepthFail = input.ReadEnum<StencilOperation>();
            impl.StencilFail = input.ReadEnum<StencilOperation>();
            impl.StencilPass = input.ReadEnum<StencilOperation>();
            impl.BackFaceStencilFunction = input.ReadEnum<ComparisonFunction>();
            impl.BackFaceStencilDepthFail = input.ReadEnum<StencilOperation>();
            impl.BackFaceStencilFail = input.ReadEnum<StencilOperation>();
            impl.BackFaceStencilPass = input.ReadEnum<StencilOperation>();

            impl.BindDepthStencilState();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            IDepthStencilStateImplementation impl = DepthStencilStateImplementation;

            output.Write("Name", Name);
            output.Write("DepthEnable", impl.DepthEnable);
            output.Write("DepthWriteEnable", impl.DepthWriteEnable);
            output.WriteEnum("DepthFunction", impl.DepthFunction);
            output.Write("StencilEnable", impl.StencilEnable);
            output.Write("ReferenceStencil", impl.ReferenceStencil);
            output.Write("StencilReadMask", impl.StencilReadMask);
            output.Write("StencilWriteMask", impl.StencilWriteMask);
            output.Write("TwoSidedStencilEnable", impl.TwoSidedStencilEnable);
            output.WriteEnum("StencilFunction", impl.StencilFunction);
            output.WriteEnum("StencilDepthFail", impl.StencilDepthFail);
            output.WriteEnum("StencilFail", impl.StencilFail);
            output.WriteEnum("StencilPass", impl.StencilPass);
            output.WriteEnum("BackFaceStencilFunction", impl.BackFaceStencilFunction);
            output.WriteEnum("BackFaceStencilDepthFail", impl.BackFaceStencilDepthFail);
            output.WriteEnum("BackFaceStencilFail", impl.BackFaceStencilFail);
            output.WriteEnum("BackFaceStencilPass", impl.BackFaceStencilPass);
        }
        
        private void CreateImplementation(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }
            
            if (!renderSystem.TryGetImplementationFactory(out IDepthStencilStateImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                DepthStencilStateImplementation = factory.CreateImplementation();
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
            IDepthStencilStateImplementation impl = DepthStencilStateImplementation;

            impl.DepthEnable = true;
            impl.DepthWriteEnable = true;
            impl.DepthFunction = ComparisonFunction.LessEqual;
            impl.StencilEnable = false;
            impl.ReferenceStencil = 0;
            impl.BackFaceStencilFunction = ComparisonFunction.Always;
            impl.BackFaceStencilDepthFail = StencilOperation.Keep;
            impl.BackFaceStencilFail = StencilOperation.Keep;
            impl.BackFaceStencilPass = StencilOperation.Keep;
            impl.StencilFunction = ComparisonFunction.Always;
            impl.StencilDepthFail = StencilOperation.Keep;
            impl.StencilFail = StencilOperation.Keep;
            impl.StencilPass = StencilOperation.Keep;
            impl.TwoSidedStencilEnable = false;
            impl.StencilReadMask = int.MaxValue;
            impl.StencilWriteMask = int.MaxValue;
        }

        /// <summary>
        /// Calculates the render state key value
        /// </summary>
        /// <returns>Render state key</returns>
        private RenderStateKey ComputeRenderStateKey()
        {
            unchecked
            {
                IDepthStencilStateImplementation impl = DepthStencilStateImplementation;

                int hash = 17;

                hash = (hash * 31) + StateType.GetHashCode();
                hash = (hash * 31) + ((impl.DepthEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.DepthWriteEnable) ? 1 : 0);
                hash = (hash * 31) + impl.DepthFunction.GetHashCode();
                hash = (hash * 31) + ((impl.StencilEnable) ? 1 : 0);
                hash = (hash * 31) + ((impl.TwoSidedStencilEnable) ? 1 : 0);
                hash = (hash * 31) + impl.ReferenceStencil;
                hash = (hash * 31) + impl.StencilFunction.GetHashCode();
                hash = (hash * 31) + impl.StencilDepthFail.GetHashCode();
                hash = (hash * 31) + impl.StencilFail.GetHashCode();
                hash = (hash * 31) + impl.StencilPass.GetHashCode();
                hash = (hash * 31) + impl.BackFaceStencilFunction.GetHashCode();
                hash = (hash * 31) + impl.BackFaceStencilDepthFail.GetHashCode();
                hash = (hash * 31) + impl.BackFaceStencilFail.GetHashCode();
                hash = (hash * 31) + impl.BackFaceStencilPass.GetHashCode();
                hash = (hash * 31) + (impl.StencilWriteMask % int.MaxValue);
                hash = (hash * 31) + (impl.StencilReadMask % int.MaxValue);

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
