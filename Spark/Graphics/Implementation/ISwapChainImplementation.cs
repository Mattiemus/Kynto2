namespace Spark.Graphics.Implementation
{
    using System;

    using Math;

    /// <summary>
    /// Defines an implementation for <see cref="SwapChain"/>.
    /// </summary>
    public interface ISwapChainImplementation : IGraphicsResourceImplementation
    {
        /// <summary>
        /// Gets the presentation parameters the swap chain is initialized to.
        /// </summary>
        PresentationParameters PresentationParameters { get; }

        /// <summary>
        /// Gets the current display mode of the swap chain.
        /// </summary>
        DisplayMode CurrentDisplayMode { get; }

        /// <summary>
        /// Gets the handle of the window the swap chain presents to.
        /// </summary>
        IntPtr WindowHandle { get; }

        /// <summary>
        /// Gets the handle to the monitor that contains the majority of the output.
        /// </summary>
        IntPtr MonitorHandle { get; }

        /// <summary>
        /// Gets if the swap chain is in full screen or not. By default, swap chains are not in full screen mode.
        /// </summary>
        bool IsFullScreen { get; }

        /// <summary>
        /// Gets if the current display mode is in wide screen or not.
        /// </summary>
        bool IsWideScreen { get; }

        /// <summary>
        /// Clears the backbuffer and depthbuffer.
        /// </summary>
        /// <param name="renderContext">Render context used to clear.</param>
        /// <param name="clearOptions">Specifies which buffer to clear.</param>
        /// <param name="color">Color value to clear to.</param>
        /// <param name="depth">Depth value to clear to.</param>
        /// <param name="stencil">Stencil value to clear to.</param>
        void Clear(IRenderContext renderContext, ClearOptions clearOptions, Color color, float depth, int stencil);

        /// <summary>
        /// Reads data from the backbuffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="subimage">The subimage region, in texels, of the backbuffer to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        void GetBackBufferData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage, int startIndex) where T : struct;

        /// <summary>
        /// Presents the contents of the backbuffer to the screen and flips the front/back buffers.
        /// </summary>
        void Present();

        /// <summary>
        /// Resets the swapchain with new presentation parameters.
        /// </summary>
        /// <param name="windowHandle">New window handle to present to.</param>
        /// <param name="presentParams">New presentation parameters.</param>
        void Reset(IntPtr windowHandle, PresentationParameters presentParams);

        /// <summary>
        /// Resizes the backbuffer.
        /// </summary>
        /// <param name="width">Width of the backbuffer.</param>
        /// <param name="height">Height of the backbuffer.</param>
        void Resize(int width, int height);

        /// <summary>
        /// Toggles the swapchain to full screen mode.
        /// </summary>
        /// <returns>True if the swapchain is in full screen mode, false if otherwise.</returns>
        bool ToggleFullScreen();
    }
}
