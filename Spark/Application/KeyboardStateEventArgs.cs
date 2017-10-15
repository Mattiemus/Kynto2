namespace Spark.Application
{
    using System;

    using Input;

    /// <summary>
    /// Event arguments for a keyboard event
    /// </summary>
    public class KeyboardStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardStateEventArgs"/> class.
        /// </summary>
        /// <param name="state">Current keyboard state</param>
        public KeyboardStateEventArgs(KeyboardState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the current keyboard state
        /// </summary>
        public KeyboardState State { get; }
    }
}
