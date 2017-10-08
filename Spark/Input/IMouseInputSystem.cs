namespace Spark.Input
{
    using System;

    using Core;

    /// <summary>
    /// Defines a service that polls a mouse device and maintains the current device's state.
    /// </summary>
    public interface IMouseInputSystem : IUpdatableEngineService
    {
        /// <summary>
        /// Gets or sets the window handle the mouse will receive its coordinates from. Mouse screen coordinates
        /// are relative to the top-left corner of this window.
        /// </summary>
        IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <returns>The current mouse state.</returns>
        MouseState GetMouseState();

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <returns>The current mouse state.</returns>
        MouseState GetMouseState(IntPtr windowHandle);

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <param name="state">The current mouse state.</param>
        void GetMouseState(out MouseState state);

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <param name="state">The current mouse state.</param>
        void GetMouseState(IntPtr windowHandle, out MouseState state);

        /// <summary>
        /// Sets the position of the mouse relative to the top-left corner of the window that the mouse is currently bound to.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        void SetPosition(int x, int y);
    }
}
