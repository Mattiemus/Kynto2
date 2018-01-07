namespace Spark.OpenGL.Application
{
    using System;
    
    using Spark.Application;
    using Spark.Graphics;
    
    using Math;
    using Utilities;
    using Graphics;

    using OTK = OpenTK;
    using OTKG = OpenTK.Graphics;

    public sealed class OpenGLWindow : Disposable, IWindow
    {
        private readonly PresentationParameters _initPresentParams;
        private bool _inSuspended;

        public OpenGLWindow(IRenderSystem renderSystem, PresentationParameters presentParams)
        {
            _initPresentParams = presentParams;

            //OpenGLNativeWindow = new OTK.NativeWindow(presentParams.BackBufferWidth, presentParams.BackBufferHeight, "Spark Window", OTK.GameWindowFlags.FixedWindow, OTKG.GraphicsMode.Default, OTK.DisplayDevice.Default);
            OpenGLNativeWindow = OpenGLRenderSystem.window;
            OpenGLNativeWindow.Visible = true;
            
            StartEvents();

            SwapChain = new SwapChain(renderSystem, Handle, presentParams);
        } 

        public event TypedEventHandler<IWindow> ClientSizeChanged;
        public event TypedEventHandler<IWindow> OrientationChanged;
        public event TypedEventHandler<IWindow> GotFocus;
        public event TypedEventHandler<IWindow> LostFocus;
        public event TypedEventHandler<IWindow> ResumeRendering;
        public event TypedEventHandler<IWindow> SuspendRendering;
        public event TypedEventHandler<IWindow> Paint;
        public event TypedEventHandler<IWindow> Closed;
        public event TypedEventHandler<IWindow> Disposed;

        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseClick;
        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseDoubleClick;
        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseUp;
        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseDown;
        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseMove;
        public event TypedEventHandler<IWindow, MouseStateEventArgs> MouseWheel;

        public event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyDown;
        public event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyPress;
        public event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyUp;

        public event TypedEventHandler<IWindow> MouseEnter;
        public event TypedEventHandler<IWindow> MouseLeave;
        public event TypedEventHandler<IWindow> MouseHover;

        internal OTK.NativeWindow OpenGLNativeWindow { get; }

        public bool EnableUserResizing { get; set; }

        public bool EnableInputEvents { get; set; }

        public bool EnablePaintEvents { get; set; }

        public bool EnableResizeRedraw { get; set; }

        public bool IsMinimized { get; }

        public bool IsMouseVisible { get; set; }

        public bool IsFullScreen { get; set; }

        public bool IsVisible
        {
            get => OpenGLNativeWindow.Visible;
            set => OpenGLNativeWindow.Visible = value;
        }

        public bool IsTopLevelWindow { get; }

        public bool Focused { get; }

        public Int2 Location { get; set; }

        public Rectangle ClientBounds => new Rectangle(OpenGLNativeWindow.ClientRectangle.X, OpenGLNativeWindow.ClientRectangle.Y, OpenGLNativeWindow.ClientRectangle.Width, OpenGLNativeWindow.ClientRectangle.Height);

        public Rectangle ScreenBounds { get; }

        public IntPtr Handle => OpenGLNativeWindow.WindowInfo.Handle;

        public SwapChain SwapChain { get; }

        public string Title
        {
            get => OpenGLNativeWindow.Title;
            set => OpenGLNativeWindow.Title = value;
        }

        public object NativeWindow => OpenGLNativeWindow;

        public object Tag { get; }

        public ApplicationHost Host { get; }

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public void CenterToScreen()
        {
            // throw new NotImplementedException();
        }

        public void Close()
        {
            OpenGLNativeWindow.Close();
        }

        public void Repaint()
        {
            throw new NotImplementedException();
        }

        public void Reset(PresentationParameters pp)
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                StopEvents();
                
                OpenGLNativeWindow.Dispose();

                if (SwapChain != null)
                {
                    SwapChain.Dispose();
                }
            }

            base.Dispose(isDisposing);
        }

        private void StartEvents()
        {
            //OpenGLNativeWindow.GotFocus += OnGotFocus;
            //OpenGLNativeWindow.LostFocus += OnLostFocus;
            //OpenGLNativeWindow.ResumeRendering += OnResumeRendering;
            //OpenGLNativeWindow.SuspendRendering += OnSuspendRendering;
            //OpenGLNativeWindow.Render += OnPaint;
            OpenGLNativeWindow.Resize += OnResize;
            OpenGLNativeWindow.Closed += OnClosed;
            OpenGLNativeWindow.Disposed += OnDisposed;
            //OpenGLNativeWindow.KeyDown += OnKeyDown;
            //OpenGLNativeWindow.KeyPress += OnKeyPress;
            //OpenGLNativeWindow.KeyUp += OnKeyUp;
            //OpenGLNativeWindow.MouseClick += OnMouseClick;
            //OpenGLNativeWindow.MouseDoubleClick += OnMouseDoubleClick;
            //OpenGLNativeWindow.MouseDown += OnMouseDown;
            //OpenGLNativeWindow.MouseUp += OnMouseUp;
            //OpenGLNativeWindow.MouseEnter += OnMouseEnter;
            //OpenGLNativeWindow.MouseHover += OnMouseHover;
            //OpenGLNativeWindow.MouseLeave += OnMouseLeave;
            //OpenGLNativeWindow.MouseMove += OnMouseMove;
            //OpenGLNativeWindow.MouseWheel += OnMouseWheel;
        }

        private void StopEvents()
        {
            //OpenGLNativeWindow.GotFocus -= OnGotFocus;
            //OpenGLNativeWindow.LostFocus -= OnLostFocus;
            //OpenGLNativeWindow.ResumeRendering -= OnResumeRendering;
            //OpenGLNativeWindow.SuspendRendering -= OnSuspendRendering;
            //OpenGLNativeWindow.Render -= OnPaint;
            OpenGLNativeWindow.Resize -= OnResize;
            OpenGLNativeWindow.Closed -= OnClosed;
            OpenGLNativeWindow.Disposed -= OnDisposed;
            //OpenGLNativeWindow.KeyDown -= OnKeyDown;
            //OpenGLNativeWindow.KeyPress -= OnKeyPress;
            //OpenGLNativeWindow.KeyUp -= OnKeyUp;
            //OpenGLNativeWindow.MouseClick -= OnMouseClick;
            //OpenGLNativeWindow.MouseDoubleClick -= OnMouseDoubleClick;
            //OpenGLNativeWindow.MouseDown -= OnMouseDown;
            //OpenGLNativeWindow.MouseUp -= OnMouseUp;
            //OpenGLNativeWindow.MouseEnter -= OnMouseEnter;
            //OpenGLNativeWindow.MouseHover -= OnMouseHover;
            //OpenGLNativeWindow.MouseLeave -= OnMouseLeave;
            //OpenGLNativeWindow.MouseMove -= OnMouseMove;
            //OpenGLNativeWindow.MouseWheel -= OnMouseWheel;
        }

        /// <summary>
        /// Invoked when the native window is resized
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnResize(object sender, EventArgs e)
        {
            bool isNotSuspended = EnableResizeRedraw || !_inSuspended;
            if (!IsMinimized && isNotSuspended)
            {
                if (SwapChain != null)
                {
                    if (SwapChain.IsFullScreen)
                    {
                        SwapChain.Resize(_initPresentParams.BackBufferWidth, _initPresentParams.BackBufferHeight);
                    }
                    else
                    { 
                        SwapChain.Resize(OpenGLNativeWindow.ClientSize.Width, OpenGLNativeWindow.ClientSize.Height);
                    }
                }

                ClientSizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Invoked when the native window is closed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the native window is disposed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        }
    }
}
