namespace Spark.Input
{
    using System;
    
    /// <summary>
    /// Provides convienent access to a registered <see cref="IMouseInputSystem"/> service.
    /// </summary>
    public static class Mouse
    {
        private static IMouseInputSystem _mouseInputSystem;

        /// <summary>
        /// Static initializer for the <see cref="Mouse"/> class
        /// </summary>
        static Mouse()
        {
            Engine.Initialized += HandleEngineInitialized;
            Engine.Destroyed += HandleEngineDestroyed;

            Engine engine = Engine.Instance;

            if (engine != null)
            {
                HandleEngineInitialized(engine, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets if the mouse is available.
        /// </summary>
        public static bool IsAvailable => _mouseInputSystem != null;

        /// <summary>
        /// Gets or sets the window handle the mouse will receive its coordinates from. Mouse screen coordinates
        /// are relative to the top-left corner of this window.
        /// </summary>
        public static IntPtr WindowHandle
        {
            get
            {
                if (!IsAvailable)
                {
                    return IntPtr.Zero;
                }

                return _mouseInputSystem.WindowHandle;
            }
            set
            {
                if (IsAvailable)
                {
                    _mouseInputSystem.WindowHandle = value;
                }
            }
        }

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <returns>The current mouse state.</returns>
        public static MouseState GetMouseState()
        {
            if (!IsAvailable)
            {
                return new MouseState();
            }

            MouseState state;
            _mouseInputSystem.GetMouseState(out state);

            return state;
        }

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <returns>The current mouse state.</returns>
        public static MouseState GetMouseState(IntPtr windowHandle)
        {
            if (!IsAvailable)
            {
                return new MouseState();
            }

            return _mouseInputSystem.GetMouseState(windowHandle);
        }

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <param name="state">The current mouse state.</param>
        public static void GetMouseState(out MouseState state)
        {
            if (!IsAvailable)
            {
                state = new MouseState();
                return;
            }

            _mouseInputSystem.GetMouseState(out state);
        }

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <param name="state">The current mouse state.</param>
        public static void GetMouseState(IntPtr windowHandle, out MouseState state)
        {
            if (!IsAvailable)
            {
                state = new MouseState();
                return;
            }

            _mouseInputSystem.GetMouseState(windowHandle, out state);
        }

        /// <summary>
        /// Sets the position of the mouse relative to the top-left corner of the window that the mouse is currently bound to.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public static void SetPosition(int x, int y)
        {
            if (!IsAvailable)
            {
                return;
            }

            _mouseInputSystem.SetPosition(x, y);
        }

        /// <summary>
        /// Invoked when the engine is initialized
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleEngineInitialized(Engine sender, EventArgs args)
        {
            _mouseInputSystem = sender.Services.GetService<IMouseInputSystem>();

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
            _mouseInputSystem = null;

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
            if (args.ServiceType == typeof(IMouseInputSystem))
            {
                _mouseInputSystem = args.Service as IMouseInputSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceChanged(EngineServiceRegistry sender, EngineServiceReplacedEventArgs args)
        {
            if (args.ServiceType == typeof(IMouseInputSystem))
            {
                _mouseInputSystem = args.NewService as IMouseInputSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is removed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceRemoved(EngineServiceRegistry sender, EngineServiceEventArgs args)
        {
            if (args.ServiceType == typeof(IMouseInputSystem))
            {
                _mouseInputSystem = null;
            }
        }
    }
}
