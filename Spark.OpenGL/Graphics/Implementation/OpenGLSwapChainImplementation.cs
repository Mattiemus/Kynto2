namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    using Core;
    using Math;
    
    /// <summary>
    /// OpenGL implementation for <see cref="SwapChain"/>
    /// </summary>
    public sealed class OpenGLSwapChainImplementation : OpenGLGraphicsResourceImplementation, ISwapChainImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLSwapChainImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="windowHandle">Window handle to bind to</param>
        /// <param name="presentParams">Presentation parameters</param>
        public OpenGLSwapChainImplementation(OpenGLRenderSystem renderSystem, IntPtr windowHandle, PresentationParameters presentParams)
            : base(renderSystem)
        {
        }

        /// <summary>
        /// Gets the presentation parameters the swap chain is initialized to.
        /// </summary>
        public PresentationParameters PresentationParameters { get; }

        /// <summary>
        /// Gets the current display mode of the swap chain.
        /// </summary>
        public DisplayMode CurrentDisplayMode { get; }

        /// <summary>
        /// Gets the handle of the window the swap chain presents to.
        /// </summary>
        public IntPtr WindowHandle { get; }

        /// <summary>
        /// Gets the handle to the monitor that contains the majority of the output.
        /// </summary>
        public IntPtr MonitorHandle { get; }

        /// <summary>
        /// Gets if the swap chain is in full screen or not. By default, swap chains are not in full screen mode.
        /// </summary>
        public bool IsFullScreen { get; }

        /// <summary>
        /// Gets if the current display mode is in wide screen or not.
        /// </summary>
        public bool IsWideScreen { get; }

        /// <summary>
        /// Clears the backbuffer and depthbuffer.
        /// </summary>
        /// <param name="renderContext">Render context used to clear.</param>
        /// <param name="clearOptions">Specifies which buffer to clear.</param>
        /// <param name="color">Color value to clear to.</param>
        /// <param name="depth">Depth value to clear to.</param>
        /// <param name="stencil">Stencil value to clear to.</param>
        public void Clear(IRenderContext renderContext, ClearOptions clearOptions, Color color, float depth, int stencil)
        {
            // TODO
        }

        /// <summary>
        /// Reads data from the backbuffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="subimage">The subimage region, in texels, of the backbuffer to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetBackBufferData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Presents the contents of the backbuffer to the screen and flips the front/back buffers.
        /// </summary>
        public void Present()
        {
            OpenTK.Graphics.GraphicsContext.CurrentContext.SwapBuffers();
        }

        /// <summary>
        /// Resets the swapchain with new presentation parameters.
        /// </summary>
        /// <param name="windowHandle">New window handle to present to.</param>
        /// <param name="presentParams">New presentation parameters.</param>
        public void Reset(IntPtr windowHandle, PresentationParameters presentParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resizes the backbuffer.
        /// </summary>
        /// <param name="width">Width of the backbuffer.</param>
        /// <param name="height">Height of the backbuffer.</param>
        public void Resize(int width, int height)
        {
            // TODO
        }

        /// <summary>
        /// Toggles the swapchain to full screen mode.
        /// </summary>
        /// <returns>True if the swapchain is in full screen mode, false if otherwise.</returns>
        public bool ToggleFullScreen()
        {
            throw new NotImplementedException();
        }
    }
}
