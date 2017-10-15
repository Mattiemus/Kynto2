namespace Spark.Application
{
    using System;

    using Input;

    /// <summary>
    /// Event arguments for a mouse event
    /// </summary>
    public class MouseStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseStateEventArgs"/> class.
        /// </summary>
        /// <param name="state">Current mouse state</param>
        public MouseStateEventArgs(MouseState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the current mouse state
        /// </summary>
        public MouseState State { get; }
    }
}
