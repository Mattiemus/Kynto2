namespace Spark
{
    using System;

    /// <summary>
    /// Engine root class that manages core engine services such as rendering, windowing, input, etc. The class itself
    /// is a singleton, therefore individual services do not have to be implemented in such a manner.
    /// </summary>
    public sealed class SparkEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkEngine"/> class.
        /// </summary>
        private SparkEngine()
        {
            Services = new EngineServiceRegistry(this);
        }

        /// <summary>
        /// Occurs when the engine has been initialized.
        /// </summary>
        public static event TypedEventHandler<SparkEngine, EventArgs> Initialized;

        /// <summary>
        /// Occurs when the engine has been destroyed, disposing of all resources.
        /// </summary>
        public static event TypedEventHandler<SparkEngine, EventArgs> Destroyed;

        /// <summary>
        /// Gets the current engine instance.
        /// </summary>
        public static SparkEngine Instance { get; private set; }

        /// <summary>
        /// Gets if the engine has been initialized or not.
        /// </summary>
        public static bool IsInitialized => Instance != null;

        /// <summary>
        /// Gets the engine service registry. The registry manages engine services, which are registered/queried by a type (usually an interface).
        /// </summary>
        public EngineServiceRegistry Services { get; }

        /// <summary>
        /// Initializes the engine. This should be called on the main thread.
        /// </summary>
        /// <returns>The initialized engine instance.</returns>
        public static SparkEngine Initialize()
        {
            if (IsInitialized)
            {
                throw new SparkException("Engine is already initialized");
            }

            Instance = new SparkEngine();

            OnInitialized(Instance);

            return Instance;
        }

        /// <summary>
        /// Initializes the engine. This takes in a platform plugin that initializes the engine with any number of engine service implementations.
        /// This should be called on the main thread.
        /// </summary>
        /// <param name="platform">Platform plugin</param>
        /// <returns>The initialized engine instance</returns>
        public static SparkEngine Initialize(IPlatformInitializer platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform), "Platform initializer cannot be null");
            }

            // Initialize the engine singleton, this will cause any initialize handlers to be called before we start adding services.
            Initialize();

            // After engine initialized, populate with platform specific services
            platform.Initialize(Instance);
            
            return Instance;
        }

        /// <summary>
        /// Destroys the engine and disposes of all resources. Services that are disposable are disposed here. This should be
        /// called on the main thread.
        /// </summary>
        public static void Destroy()
        {
            if (!IsInitialized)
            {
                throw new SparkException("Engine is not initialized");
            }

            Instance.Services.RemoveAllAndDispose();

            OnDestroyed(Instance);

            Instance = null;
        }

        /// <summary>
        /// Updates all services that are updatable where they can change their state or poll devices. This should
        /// be called on the main thread.
        /// </summary>
        public void UpdateServices()
        {
            Services.UpdateServices();
        }

        /// <summary>
        /// Invoked the engine initialized event
        /// </summary>
        /// <param name="engine">Engine instance</param>
        private static void OnInitialized(SparkEngine engine)
        {
            Initialized?.Invoke(engine, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked the engine destroyed event
        /// </summary>
        /// <param name="engine">Engine instance</param>
        private static void OnDestroyed(SparkEngine engine)
        {
            Destroyed?.Invoke(engine, EventArgs.Empty);
        }
    }
}
