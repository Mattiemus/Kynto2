namespace Spark.Windows.Application
{
    using System;
    using System.Windows.Forms;

    using Spark.Math;
    using Spark.Application;
    using Spark.Graphics;
    using Spark.Input;
    using Spark.Input.Utilities;

    using SD = System.Drawing;

    public sealed class WinFormsWindow : IWindow
    {
        private readonly ApplicationHost _host;
        private readonly SwapChain _swapChain;
        private readonly IWinFormsControlAdapter _controlAdapter;
        private readonly KeyboardStateBuilder _keystateBuilder;
        private readonly MouseStateBuilder _mousestateBuilder;
        private readonly bool _isTopLevelWindow;
        private object _tag;
        private PresentationParameters _originalPP;
        private bool _inSuspended;
        private bool _isDisposed;
        private bool _enableResizeRedraw;

        internal WinFormsWindow(IRenderSystem renderSystem, ApplicationHost host, PresentationParameters pp, bool isTopLevelWindow)
        {
            _host = host;
            _originalPP = pp;
            _isTopLevelWindow = isTopLevelWindow;
            _keystateBuilder = new KeyboardStateBuilder();
            _mousestateBuilder = new MouseStateBuilder();
            _isDisposed = false;
            _inSuspended = false;
            _enableResizeRedraw = false;

            if (_isTopLevelWindow)
            {
                _controlAdapter = new RenderForm(pp.BackBufferWidth, pp.BackBufferHeight);
            }
            else
            {
                _controlAdapter = new RenderControl(pp.BackBufferWidth, pp.BackBufferHeight);
            }

            Control c = _controlAdapter.Control;
            c.Show();

            StartEvents();

            _swapChain = new SwapChain(renderSystem, Handle, pp);
        }

        internal WinFormsWindow(IRenderSystem renderSystem, ApplicationHost host, Control existingControl, PresentationParameters? pp, bool sizeNativeWindow)
            : this(renderSystem, host, CreateWinFormsAdapter(existingControl), pp, sizeNativeWindow)
        {
        }

        internal WinFormsWindow(IRenderSystem renderSystem, ApplicationHost host, IWinFormsControlAdapter existingControl, PresentationParameters? pp, bool resizeNativeWindow)
        {
            _host = host;
            _keystateBuilder = new KeyboardStateBuilder();
            _mousestateBuilder = new MouseStateBuilder();
            _isDisposed = false;
            _inSuspended = false;
            _enableResizeRedraw = false;
            _controlAdapter = existingControl;

            _isTopLevelWindow = existingControl is Form;
            _originalPP = (pp.HasValue) ? pp.Value : PresentParamsFromControl(existingControl);

            if (resizeNativeWindow)
            {
                existingControl.ClientSize = new Int2(_originalPP.BackBufferWidth, _originalPP.BackBufferHeight);

                Int2 size = existingControl.ClientSize;

                // Ensure resize can stick
                _originalPP.BackBufferWidth = Math.Max(size.X, 1);
                _originalPP.BackBufferHeight = Math.Max(size.Y, 1);
                _originalPP.IsFullScreen = (_isTopLevelWindow) ? _originalPP.IsFullScreen : false;
            }
            else
            {
                Int2 size = existingControl.ClientSize;

                _originalPP.BackBufferWidth = Math.Max(size.X, 1);
                _originalPP.BackBufferHeight = Math.Max(size.Y, 1);
                _originalPP.IsFullScreen = false;
            }

            existingControl.Control?.Show();

            StartEvents();

            _swapChain = new SwapChain(renderSystem, Handle, _originalPP);
        }

        public event TypedEventHandler<IWindow> ClientSizeChanged;
        public event TypedEventHandler<IWindow> OrientationChanged;
        public event TypedEventHandler<IWindow> GotFocus;
        public event TypedEventHandler<IWindow> LostFocus;
        public event TypedEventHandler<IWindow> ResumeRendering;
        public event TypedEventHandler<IWindow> SuspendRendering;
        public event TypedEventHandler<IWindow> Closed;
        public event TypedEventHandler<IWindow> Paint;
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

        public bool EnableUserResizing
        {
            get => _controlAdapter.EnableUserResizing;
            set => _controlAdapter.EnableUserResizing = value;
        }

        public bool EnableInputEvents
        {
            get => _controlAdapter.EnableInputEvents;
            set => _controlAdapter.EnableInputEvents = value;
        }

        public bool EnablePaintEvents
        {
            get => _controlAdapter.EnablePaintEvents;
            set => _controlAdapter.EnablePaintEvents = value;
        }

        public bool EnableResizeRedraw
        {
            get => _enableResizeRedraw;
            set => _enableResizeRedraw = value;
        }

        public bool IsMinimized => _controlAdapter.IsMinimized;

        public bool IsMouseVisible
        {
            get => _controlAdapter.IsMouseVisible;
            set => _controlAdapter.IsMouseVisible = value;
        }

        public bool IsVisible
        {
            get
            {
                Control c = _controlAdapter.Control;
                if (c == null)
                {
                    return false;
                }

                return c.Visible;
            }
            set
            {
                Control c = _controlAdapter.Control;
                if (c != null && c.Visible != value)
                {
                    c.Visible = value;
                }
            }
        }

        public bool IsFullScreen
        {
            get => _swapChain.IsFullScreen;
            set
            {
                if (!_swapChain.IsFullScreen && value)
                {
                    _swapChain.ToggleFullScreen();
                }
                else if (_swapChain.IsFullScreen && !value)
                {
                    // Make sure we're visible
                    IsVisible = true;
                    _swapChain.ToggleFullScreen();
                }
            }
        }

        public bool Focused => _controlAdapter.Focused;

        public bool IsTopLevelWindow => _isTopLevelWindow;

        public bool IsDisposed => _isDisposed;

        public Int2 Location
        {
            get => _controlAdapter.Location;
            set => _controlAdapter.Location = value;
        }

        public Rectangle ClientBounds => _controlAdapter.ClientBounds;

        public Rectangle ScreenBounds => _controlAdapter.ScreenBounds;

        public IntPtr Handle
        {
            get
            {
                Control c = _controlAdapter.Control;
                if (c == null)
                {
                    return IntPtr.Zero;
                }

                return c.Handle;
            }
        }

        public SwapChain SwapChain => _swapChain;

        public string Title
        {
            get
            {
                Control c = _controlAdapter.Control;
                if (c == null)
                {
                    return string.Empty;
                }

                return c.Text;
            }
            set
            {
                Control c = _controlAdapter.Control;
                if (c != null)
                {
                    c.Text = value;
                }
            }
        }

        public object NativeWindow => _controlAdapter.Control;

        public object Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public ApplicationHost Host => _host;

        private bool InvokeRequired
        {
            get
            {
                Control c = _controlAdapter.Control;
                if (c != null)
                {
                    int currThreadId = NativeMethods.GetCurrentThreadId();
                    int windowThreadId;
                    NativeMethods.GetWindowThreadProcessId(c.Handle, out windowThreadId);

                    return windowThreadId != currThreadId;
                }

                return false;
            }
        }
        
        public void Repaint()
        {
            Control c = _controlAdapter.Control;
            if (c != null)
            {
                if (InvokeRequired)
                {
                    c.Invoke(new Action(() => c.Invalidate()));
                }
                else
                {
                    c.Invalidate();
                }
            }
        }

        public void Focus()
        {
            Control c = _controlAdapter.Control;
            if (c != null)
            {
                if (InvokeRequired)
                {
                    c.Invoke(new Action(() => c.Focus()));
                }
                else
                {
                    c.Focus();
                }
            }
        }

        public void Resize(int width, int height)
        {
            if (_swapChain == null)
            {
                return;
            }

            if (_swapChain.IsFullScreen)
            {
                if (width == _originalPP.BackBufferWidth && height == _originalPP.BackBufferHeight)
                {
                    return;
                }

                _originalPP.BackBufferWidth = width;
                _originalPP.BackBufferHeight = height;

                PresentationParameters pp = _originalPP;
                pp.IsFullScreen = true;
                _swapChain.Reset(Handle, pp);
                OnResize(this, EventArgs.Empty);
            }
            else
            {
                Control c = _controlAdapter.Control;
                if (c != null)
                {
                    SD.Size clientSize = c.ClientSize;
                    if (clientSize.Width == width && clientSize.Height == height)
                    {
                        return;
                    }

                    if (c is Form)
                    {
                        Form f = c as Form;
                        if (f.WindowState == FormWindowState.Maximized)
                        {
                            f.WindowState = FormWindowState.Normal;
                        }
                    }

                    c.ClientSize = new SD.Size(width, height); ;
                }
            }
        }

        public void Reset(PresentationParameters pp)
        {
            if (_swapChain == null)
            {
                return;
            }

            _originalPP = pp;

            Control c = _controlAdapter.Control;
            if (c != null)
            {
                c.ClientSize = new SD.Size(pp.BackBufferWidth, pp.BackBufferHeight);
            }

            _swapChain.Reset(Handle, pp);
        }

        public void Close()
        {
            Control c = _controlAdapter.Control;
            if (c != null)
            {
                Form f = c.FindForm();
                f?.Close();
            }
        }

        public void CenterToScreen()
        {
            Control c = _controlAdapter.Control;
            if (c != null)
            {
                Form f = c.FindForm();
                if (f != null)
                {
                    SD.Rectangle screenRect = Screen.GetBounds(f);
                    SD.Size windSize = f.Size;
                    SD.Point topLeftLoc = f.Location;
                    SD.Point windCenter = new SD.Point(topLeftLoc.X + (windSize.Width / 2), topLeftLoc.Y + (windSize.Height / 2));
                    SD.Point screenCenter = new SD.Point(screenRect.X + (screenRect.Width / 2), screenRect.Y + (screenRect.Height / 2));
                    f.Location = new SD.Point((screenCenter.X - windCenter.X) + topLeftLoc.X, (screenCenter.Y - windCenter.Y) + topLeftLoc.Y);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true, false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing, bool disposeExternallyCalled)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (disposing)
                {
                    if (_controlAdapter != null)
                    {
                        StopEvents();

                        if (!disposeExternallyCalled)
                        {
                            _controlAdapter.Dispose();
                        }
                    }

                    _swapChain?.Dispose();

                    OnDisposed();
                }
            }
        }

        private void StartEvents()
        {
            _controlAdapter.GotFocus += OnGotFocus;
            _controlAdapter.LostFocus += OnLostFocus;
            _controlAdapter.ResumeRendering += OnResumeRendering;
            _controlAdapter.SuspendRendering += OnSuspendRendering;
            _controlAdapter.Render += OnPaint;
            _controlAdapter.Resize += OnResize;
            _controlAdapter.Closed += OnClosed;
            _controlAdapter.Disposed += OnDisposed;
            _controlAdapter.KeyDown += OnKeyDown;
            _controlAdapter.KeyPress += OnKeyPress;
            _controlAdapter.KeyUp += OnKeyUp;
            _controlAdapter.MouseClick += OnMouseClick;
            _controlAdapter.MouseDoubleClick += OnMouseDoubleClick;
            _controlAdapter.MouseDown += OnMouseDown;
            _controlAdapter.MouseUp += OnMouseUp;
            _controlAdapter.MouseEnter += OnMouseEnter;
            _controlAdapter.MouseHover += OnMouseHover;
            _controlAdapter.MouseLeave += OnMouseLeave;
            _controlAdapter.MouseMove += OnMouseMove;
            _controlAdapter.MouseWheel += OnMouseWheel;
        }

        private void StopEvents()
        {
            _controlAdapter.GotFocus -= OnGotFocus;
            _controlAdapter.LostFocus -= OnLostFocus;
            _controlAdapter.ResumeRendering -= OnResumeRendering;
            _controlAdapter.SuspendRendering -= OnSuspendRendering;
            _controlAdapter.Render -= OnPaint;
            _controlAdapter.Resize -= OnResize;
            _controlAdapter.Closed -= OnClosed;
            _controlAdapter.Disposed -= OnDisposed;
            _controlAdapter.KeyDown -= OnKeyDown;
            _controlAdapter.KeyPress -= OnKeyPress;
            _controlAdapter.KeyUp -= OnKeyUp;
            _controlAdapter.MouseClick -= OnMouseClick;
            _controlAdapter.MouseDoubleClick -= OnMouseDoubleClick;
            _controlAdapter.MouseDown -= OnMouseDown;
            _controlAdapter.MouseUp -= OnMouseUp;
            _controlAdapter.MouseEnter -= OnMouseEnter;
            _controlAdapter.MouseHover -= OnMouseHover;
            _controlAdapter.MouseLeave -= OnMouseLeave;
            _controlAdapter.MouseMove -= OnMouseMove;
            _controlAdapter.MouseWheel -= OnMouseWheel;
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(this, EventArgs.Empty);
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(this, EventArgs.Empty);
        }

        private void OnResumeRendering(object sender, EventArgs e)
        {
            _inSuspended = false;

            // If suspending resize until it ends, then we need to make one call to OnResize, if allowing swapChain resizing,
            // then this will be an extra one
            if (!_enableResizeRedraw)
            {
                OnResize(this, e);
            }

            ResumeRendering?.Invoke(this, EventArgs.Empty);
        }

        private void OnSuspendRendering(object sender, EventArgs e)
        {
            _inSuspended = true;
            SuspendRendering?.Invoke(this, EventArgs.Empty);
        }

        private void OnPaint(object sender, EventArgs e)
        {
            Paint?.Invoke(this, EventArgs.Empty);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void OnResize(object sender, EventArgs e)
        {
            bool isNotSuspended = _enableResizeRedraw || !_inSuspended;
            if (!IsMinimized && isNotSuspended)
            {
                if (_swapChain != null)
                {
                    if (_swapChain.IsFullScreen)
                    {
                        _swapChain.Resize(_originalPP.BackBufferWidth, _originalPP.BackBufferHeight);
                    }
                    else
                    {
                        Int2 size = _controlAdapter.ClientSize;
                        _swapChain.Resize(size.X, size.Y);
                    }
                }

                ClientSizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            // If diposing by calling this Window, we don't want to call Dispose() again. If externally, then yes we do.
            if (!_isDisposed)
            {
                Dispose(true, true);
                GC.SuppressFinalize(this);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            ApplyState(_keystateBuilder, e, true);
            KeyDown?.Invoke(this, CreateKeyboardStateEventArgs(_keystateBuilder));
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            ApplyState(_keystateBuilder, e);
            KeyPress?.Invoke(this, CreateKeyboardStateEventArgs(_keystateBuilder));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ApplyState(_keystateBuilder, e, false);
            KeyUp?.Invoke(this, CreateKeyboardStateEventArgs(_keystateBuilder));
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, true);
            MouseClick?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, true);
            MouseDoubleClick?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, true);
            MouseDown?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, false);
            MouseUp?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            MouseEnter?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            MouseHover?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, true);
            MouseMove?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            ApplyState(_mousestateBuilder, e, true);
            MouseWheel?.Invoke(this, CreateMouseStateEventArgs(_mousestateBuilder));
        }

        private void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);

            // Also remove this window from the application host implementation's collection (it should safely remove in all cases, if we're iterating/disposing then it won't remove)
            (_host.Implementation as Spark.Windows.Application.Implementation.WinFormsApplicationHostImplementation).RemoveWindowOnDispose(this);
        }

        private void OnOrientationChanged()
        {
            OrientationChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void ApplyState(MouseStateBuilder builder, MouseEventArgs args, bool pressed)
        {
            builder.WheelValue += args.Delta;
            builder.CursorPosition = new Int2(args.X, args.Y);

            Spark.Input.ButtonState state = (pressed) ? Spark.Input.ButtonState.Pressed : Spark.Input.ButtonState.Released;

            if ((args.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                builder.SetButtonState(MouseButton.Left, state);
            }

            if ((args.Button & MouseButtons.Middle) == MouseButtons.Middle)
            {
                builder.SetButtonState(MouseButton.Middle, state);
            }

            if ((args.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                builder.SetButtonState(MouseButton.Right, state);
            }

            if ((args.Button & MouseButtons.XButton1) == MouseButtons.XButton1)
            {
                builder.SetButtonState(MouseButton.XButton1, state);
            }

            if ((args.Button & MouseButtons.XButton2) == MouseButtons.XButton2)
            {
                builder.SetButtonState(MouseButton.XButton2, state);
            }
        }

        private static void ApplyState(KeyboardStateBuilder builder, KeyEventArgs args, bool pressed)
        {
            Spark.Input.KeyState state = (pressed) ? Spark.Input.KeyState.Down : Spark.Input.KeyState.Up;
            builder.SetKeyState((Spark.Input.Keys)args.KeyValue, state);
        }

        private static void ApplyState(KeyboardStateBuilder builder, KeyPressEventArgs args)
        {
            builder.AddPressedKey((Spark.Input.Keys)char.ToUpperInvariant(args.KeyChar));
        }

        private static MouseStateEventArgs CreateMouseStateEventArgs(MouseStateBuilder builder)
        {
            builder.ConstructState(out MouseState state);
            return new MouseStateEventArgs(state);
        }

        private static KeyboardStateEventArgs CreateKeyboardStateEventArgs(KeyboardStateBuilder builder)
        {
            builder.ConstructState(out KeyboardState state);
            return new KeyboardStateEventArgs(state);
        }

        private static PresentationParameters PresentParamsFromControl(IWinFormsControlAdapter c)
        {
            var pp = new PresentationParameters();
            pp.BackBufferFormat = SurfaceFormat.Color;

            var size = c.ClientSize;
            pp.BackBufferWidth = size.X;
            pp.BackBufferHeight = size.Y;

            pp.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.DisplayOrientation = DisplayOrientation.Default;
            pp.IsFullScreen = false;
            pp.PresentInterval = PresentInterval.Immediate;
            pp.RenderTargetUsage = RenderTargetUsage.PlatformDefault;
            pp.MultiSampleQuality = 0;
            pp.MultiSampleCount = 0;

            return pp;
        }

        private static IWinFormsControlAdapter CreateWinFormsAdapter(Control existingControl)
        {
            if (existingControl is IWinFormsControlAdapter)
            {
                return existingControl as IWinFormsControlAdapter;
            }
            else
            {
                return new ControlAdapter(existingControl);
            }
        }
    }
}
