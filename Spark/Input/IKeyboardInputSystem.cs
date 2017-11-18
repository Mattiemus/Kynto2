namespace Spark.Input
{
    /// <summary>
    /// Defines a service that polls a keyboard device and maintains the current device's state.
    /// </summary>
    public interface IKeyboardInputSystem : IUpdatableEngineService
    {
        /// <summary>
        /// Queries the current state of the keyboard - e.g. which keys are pressed.
        /// </summary>
        /// <returns>The current keyboard state.</returns>
        KeyboardState GetKeyboardState();

        /// <summary>
        /// Queries the current state of the ke yboard - e.g. which keys are pressed.
        /// </summary>
        /// <param name="state">The current keyboard state.</param>
        void GetKeyboardState(out KeyboardState state);
    }
}
