namespace Spark.Input
{
    using System;

    using Core;

    /// <summary>
    /// Provides convienent access to a registered <see cref="IKeyboardInputSystem"/> service.
    /// </summary>
    public static class Keyboard
    {
        private static IKeyboardInputSystem _keyboardInputSystem;

        /// <summary>
        /// Static initializer for the <see cref="Keyboard"/> class.
        /// </summary>
        static Keyboard()
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
        /// Gets if the keyboard is available.
        /// </summary>
        public static bool IsAvailable => _keyboardInputSystem != null;

        /// <summary>
        /// Queries the current state of the keyboard - e.g. which keys are pressed.
        /// </summary>
        /// <returns>The current keyboard state.</returns>
        public static KeyboardState GetKeyboardState()
        {
            if (!IsAvailable)
            {
                return new KeyboardState();
            }
            
            _keyboardInputSystem.GetKeyboardState(out KeyboardState state);
            return state;
        }

        /// <summary>
        /// Queries the current state of the keyboard - e.g. which keys are pressed.
        /// </summary>
        /// <param name="state">The current keyboard state.</param>
        public static void GetKeyboardState(out KeyboardState state)
        {
            if (!IsAvailable)
            {
                state = new KeyboardState();
                return;
            }

            _keyboardInputSystem.GetKeyboardState(out state);
        }

        /// <summary>
        /// Invoked when the engine is initialized
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleEngineInitialized(Engine sender, EventArgs args)
        {
            _keyboardInputSystem = sender.Services.GetService<IKeyboardInputSystem>();

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
            _keyboardInputSystem = null;

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
            if (args.ServiceType == typeof(IKeyboardInputSystem))
            {
                _keyboardInputSystem = args.Service as IKeyboardInputSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceChanged(EngineServiceRegistry sender, EngineServiceReplacedEventArgs args)
        {
            if (args.ServiceType == typeof(IKeyboardInputSystem))
            {
                _keyboardInputSystem = args.NewService as IKeyboardInputSystem;
            }
        }

        /// <summary>
        /// Invoked when an engine service is removed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        private static void HandleServiceRemoved(EngineServiceRegistry sender, EngineServiceEventArgs args)
        {
            if (args.ServiceType == typeof(IKeyboardInputSystem))
            {
                _keyboardInputSystem = null;
            }
        }
    }
}
