namespace Spark.Windows.Input
{
    using Spark.Core;
    using Spark.Input;
    using Spark.Input.Utilities;

    using Core;

    /// <summary>
    /// Keyboard input system that acquires state data through the Win32 API.
    /// </summary>
    public sealed class Win32KeyboardInputSystem : IKeyboardInputSystem
    {
        private readonly KeyboardStateBuilder _stateBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32KeyboardInputSystem"/> class.
        /// </summary>
        public Win32KeyboardInputSystem()
        {
            _stateBuilder = new KeyboardStateBuilder();
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name => "Win32KeyboardInputSystem";

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(Engine engine)
        {
            // No-op
        }

        /// <summary>
        /// Notifies the service to update its internal state.
        /// </summary>
        public void Update()
        {
            _stateBuilder.Clear();

            unsafe
            {
                KeyByteArray keys;
                if (!NativeMethods.GetKeyboardState((byte*)&keys))
                {
                    return;
                }

                for (int i = 0; i < 256; i++)
                {
                    byte key = keys.Keys[i];
                    if ((key & 0x80) != 0)
                    {
                        // Make sure we filter out any mouse buttons!
                        switch (i)
                        {
                            case 0x01: // Left
                            case 0x02: // Right
                            case 0x04: // Middle
                            case 0x05: // XButton1
                            case 0x06: // XButton2
                                break;
                            default:
                                _stateBuilder.AddPressedKey((Keys)i);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Queries the current state of the keyboard - e.g. which keys are pressed.
        /// </summary>
        /// <returns>The current keyboard state.</returns>
        public KeyboardState GetKeyboardState()
        {
            _stateBuilder.ConstructState(out KeyboardState state);
            return state;
        }

        /// <summary>
        /// Queries the current state of the ke yboard - e.g. which keys are pressed.
        /// </summary>
        /// <param name="state">The current keyboard state.</param>
        public void GetKeyboardState(out KeyboardState state)
        {
            _stateBuilder.ConstructState(out state);
        }
    }
}
