namespace Spark.Graphics
{
    using System;

    using Core;
    using Math;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a backbuffer and screenbuffer pair that operates as the on screen target which the renderer draws onto
    /// </summary>
    public class SwapChain : GraphicsResource
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChain"/> class.
        /// </summary>
        protected SwapChain()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChain"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="windowHandle">The window handle the swapchain will present to.</param>
        /// <param name="preferredPresentParams">The preferred presentation parameters, if non-format values are out of range, they will be adjusted accordingly.</param>
        public SwapChain(IRenderSystem renderSystem, IntPtr windowHandle, PresentationParameters preferredPresentParams)
        {
            CreateImplementation(renderSystem, windowHandle, ref preferredPresentParams);
        }

        /// <summary>
        /// Gets the presentation parameters the swap chain is initialized to.
        /// </summary>
        public PresentationParameters PresentationParameters => SwapChainImpl.PresentationParameters;

        /// <summary>
        /// Gets the current display mode of the swap chain.
        /// </summary>
        public DisplayMode CurrentDisplayMode => SwapChainImpl.CurrentDisplayMode;

        /// <summary>
        /// Gets the handle of the window the swap chain presents to.
        /// </summary>
        public IntPtr WindowHandle => SwapChainImpl.WindowHandle;

        /// <summary>
        /// Gets the handle to the monitor that contains the majority of the output.
        /// </summary>
        public IntPtr MonitorHandle => SwapChainImpl.MonitorHandle;

        /// <summary>
        /// Gets if the swap chain is in full screen or not. By default, swap chains are not in full screen mode.
        /// </summary>
        public bool IsFullScreen => SwapChainImpl.IsFullScreen;

        /// <summary>
        /// Gets if the current display mode is in wide screen or not.
        /// </summary>
        public bool IsWideScreen => SwapChainImpl.IsWideScreen;

        /// <summary>
        /// Gets or sets the swap chain implementation
        /// </summary>
        private ISwapChainImplementation SwapChainImpl
        {
            get => Implementation as ISwapChainImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// Clears the backbuffer and depthbuffer to default values (1.0 for depth, 0 for stencil).
        /// </summary>
        /// <param name="renderContext">Render context used to clear.</param>
        /// <param name="color">Color value to clear to.</param>
        public void Clear(IRenderContext renderContext, Color color)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            SwapChainImpl.Clear(renderContext, ClearOptions.All, color, 1.0f, 0);
        }

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
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            SwapChainImpl.Clear(renderContext, clearOptions, color, depth, stencil);
        }

        /// <summary>
        /// Clears the backbuffer and binds it to the render context as the currently active backbuffer. This means when the context's list of
        /// render targets are set to null, the context will switch to the targets that represent this backbuffer.
        /// </summary>
        /// <param name="renderContext">Render context used to clear.</param>
        /// <param name="color">Color value to clear to.</param>
        public void SetActiveAndClear(IRenderContext renderContext, Color color)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            renderContext.SetRenderTarget(null); // Resolve any render targets already set, we expect the backbuffer to always be set
            renderContext.BackBuffer = this;
            SwapChainImpl.Clear(renderContext, ClearOptions.All, color, 1.0f, 0);
        }

        /// <summary>
        /// Clears the backbuffer and binds it to the render context as the currently active backbuffer. This means when the context's list of
        /// render targets are set to null, the context will switch to the targets that represent this backbuffer.
        /// </summary>
        /// <param name="renderContext">Render context used to clear.</param>
        /// <param name="clearOptions">Specifies which buffer to clear.</param>
        /// <param name="color">Color value to clear to.</param>
        /// <param name="depth">Depth value to clear to.</param>
        /// <param name="stencil">Stencil value to clear to.</param>
        public void SetActiveAndClear(IRenderContext renderContext, ClearOptions clearOptions, Color color, float depth, int stencil)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext));
            }

            renderContext.SetRenderTarget(null); // Resolve any render targets already set, we expect the backbuffer to always be set
            renderContext.BackBuffer = this;
            SwapChainImpl.Clear(renderContext, clearOptions, color, depth, stencil);
        }

        /// <summary>
        /// Reads data from the backbuffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the backbuffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the backbuffer.</param>
        public void GetBackBufferData<T>(IDataBuffer<T> data) where T : struct
        {
            try
            {
                SwapChainImpl.GetBackBufferData(data, null, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the backbuffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the backbuffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the backbuffer.</param>
        /// <param name="subimage">The subimage region, in texels, of the backbuffer to read from, if null the whole image is read from.</param>
        public void GetBackBufferData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage) where T : struct
        {
            try
            {
                SwapChainImpl.GetBackBufferData(data, subimage, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the backbuffer into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the backbuffer.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the backbuffer.</param>
        /// <param name="subimage">The subimage region, in texels, of the backbuffer to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetBackBufferData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            try
            {
                SwapChainImpl.GetBackBufferData(data, subimage, startIndex);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Presents the contents of the backbuffer to the screen and flips the front/back buffers.
        /// </summary>
        /// <remarks>See implementors for details on specific exceptions that can be thrown.</remarks> 
        public void Present()
        {
            SwapChainImpl.Present();
        }

        /// <summary>
        /// Resets the swapchain with new presentation parameters.
        /// </summary>
        /// <param name="windowHandle">New window handle to present to.</param>
        /// <param name="preferredPresentParams">New preferred presentation parameters, if non-format values are out of range, they will be adjusted accordingly.</param>
        public void Reset(IntPtr windowHandle, PresentationParameters preferredPresentParams)
        {
            ValidateCreationParameters(Implementation.RenderSystem.Adapter, windowHandle, ref preferredPresentParams);

            try
            {
                SwapChainImpl.Reset(windowHandle, preferredPresentParams);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error resetting swap chain", e);
            }
        }

        /// <summary>
        /// Resizes the backbuffer.
        /// </summary>
        /// <param name="width">Width of the backbuffer.</param>
        /// <param name="height">Height of the backbuffer.</param>
        public void Resize(int width, int height)
        {
            if (width == 0 && height == 0)
            {
                return;
            }

            int maxSize = RenderSystem.Adapter.MaximumTexture2DSize;

            if (width < 0 || width > maxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Texture dimension out of range");
            }

            if (height < 0 || height > maxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Texture dimension out of range");
            }

            try
            {
                SwapChainImpl.Resize(width, height);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error resetting swap chain", e);
            }
        }

        /// <summary>
        /// Toggles the swapchain to full screen mode.
        /// </summary>
        /// <returns>True if the swapchain is in full screen mode, false if otherwise.</returns>
        public bool ToggleFullScreen()
        {
            try
            {
                return SwapChainImpl.ToggleFullScreen();
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error toggling full screen swap chain", e);
            }
        }
        
        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="windowHandle">The window handle the swapchain will present to.</param>
        /// <param name="preferredPresentParams">The preferred presentation parameters, if non-format values are out of range, they will be adjusted accordingly.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, IntPtr windowHandle, ref PresentationParameters preferredPresentParams)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Swap chain window handle not valid", nameof(windowHandle));
            }

            if (!PresentationParameters.CheckPresentationParameters(adapter, ref preferredPresentParams))
            {
                throw new SparkGraphicsException("Bad back buffer format");
            }
        }
        
        /// <summary>
        /// Creates the swap chain implementation
        /// </summary>
        /// <param name="renderSystem"></param>
        /// <param name="windowHandle"></param>
        /// <param name="preferredPresentParams"></param>
        private void CreateImplementation(IRenderSystem renderSystem, IntPtr windowHandle, ref PresentationParameters preferredPresentParams)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system is null");
            }

            ValidateCreationParameters(renderSystem.Adapter, windowHandle, ref preferredPresentParams);
            
            if (!renderSystem.TryGetImplementationFactory(out ISwapChainImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                SwapChainImpl = factory.CreateImplementation(windowHandle, preferredPresentParams);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
