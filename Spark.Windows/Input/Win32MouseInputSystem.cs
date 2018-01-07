namespace Spark.Windows.Input
{
    using System;
    
    using Spark.Input;
    
    using Utilities;

    using SWF = System.Windows.Forms;

    /// <summary>
    /// Mouse input system that acquires state data from the Win32 API.
    /// </summary>
    public sealed class Win32MouseInputSystem : Disposable, IMouseInputSystem, IDisposableEngineService
    {
        private IntPtr _handle;
        private readonly MouseWheelListener _wheelListener;

        private POINT _rawPos;
        private POINT _currPos;
        private ButtonState _lButton;
        private ButtonState _rButton;
        private ButtonState _mButton;
        private ButtonState _xButton1;
        private ButtonState _xButton2;
        private int _wheelValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32MouseInputSystem"/> class.
        /// </summary>
        public Win32MouseInputSystem() 
            : this(IntPtr.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32MouseInputSystem"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public Win32MouseInputSystem(IntPtr handle)
        {
            _wheelListener = new MouseWheelListener();
            WindowHandle = handle;
        }
        
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name => "Win32MouseInputSystem";

        /// <summary>
        /// Gets or sets the window handle the mouse will receive its coordinates from. Mouse screen coordinates
        /// are relative to the top-left corner of this window.
        /// </summary>
        public IntPtr WindowHandle
        {
            get => _handle;
            set
            {
                if (value != _handle)
                {
                    _handle = value;

                    _wheelListener.ReleaseHandle();
                    if (_handle != IntPtr.Zero)
                    {
                        _wheelListener.AssignHandle(_handle);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(SparkEngine engine)
        {
            // No-op
        }

        /// <summary>
        /// Notifies the service to update its internal state.
        /// </summary>
        public void Update()
        {
            NativeMethods.GetCursorPos(out _rawPos);
            _currPos = _rawPos;

            if (_handle != IntPtr.Zero)
            {
                NativeMethods.ScreenToClient(_handle, ref _currPos);
            }

            _lButton = (ButtonState)(((ushort)NativeMethods.GetAsyncKeyState(1)) >> 15);
            _rButton = (ButtonState)(((ushort)NativeMethods.GetAsyncKeyState(2)) >> 15);
            _mButton = (ButtonState)(((ushort)NativeMethods.GetAsyncKeyState(4)) >> 15);
            _xButton1 = (ButtonState)(((ushort)NativeMethods.GetAsyncKeyState(5)) >> 15);
            _xButton2 = (ButtonState)(((ushort)NativeMethods.GetAsyncKeyState(6)) >> 15);
            _wheelValue = _wheelListener.CurrentWheelValue;
        }

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <returns>The current mouse state.</returns>
        public MouseState GetMouseState()
        {
            return new MouseState(_currPos.X, _currPos.Y, _wheelValue, _lButton, _rButton, _mButton, _xButton1, _xButton2);
        }

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <returns>The current mouse state.</returns>
        public MouseState GetMouseState(IntPtr windowHandle)
        {
            POINT pos = _rawPos;
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.ScreenToClient(windowHandle, ref pos);
            }

            return new MouseState(pos.X, pos.Y, _wheelValue, _lButton, _rButton, _mButton, _xButton1, _xButton2);
        }

        /// <summary>
        /// Queries the current state of the mouse button and screen position.
        /// </summary>
        /// <param name="state">The current mouse state.</param>
        public void GetMouseState(out MouseState state)
        {
            state = new MouseState(_currPos.X, _currPos.Y, _wheelValue, _lButton, _rButton, _mButton, _xButton1, _xButton2);
        }

        /// <summary>
        /// Queries the current state of the mouse buttons and screen position, using the specified window handle.
        /// </summary>
        /// <param name="windowHandle">Window handle to get the screen coordinates in</param>
        /// <param name="state">The current mouse state.</param>
        public void GetMouseState(IntPtr windowHandle, out MouseState state)
        {
            POINT pos = _rawPos;
            if (windowHandle != IntPtr.Zero)
            {
                NativeMethods.ScreenToClient(windowHandle, ref pos);
            }

            state = new MouseState(pos.X, pos.Y, _wheelValue, _lButton, _rButton, _mButton, _xButton1, _xButton2);
        }

        /// <summary>
        /// Sets the position of the mouse relative to the top-left corner of the window that the mouse is currently bound to.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public void SetPosition(int x, int y)
        {
            _currPos = _rawPos = new POINT(x, y);

            if (_handle != IntPtr.Zero)
            {
                NativeMethods.ClientToScreen(_handle, ref _rawPos);
            }

            NativeMethods.SetCursorPos(_rawPos.X, _rawPos.Y);
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
                _wheelListener.ReleaseHandle();
            }

            base.Dispose(isDisposing);
        }
        
        /// <summary>
        /// Hidden window to listen for mouse wheel events
        /// </summary>
        private sealed class MouseWheelListener : SWF.NativeWindow
        {
            /// <summary>
            /// Current mouse wheel value
            /// </summary>
            public int CurrentWheelValue { get; private set; }

            /// <summary>
            /// Window proceedure
            /// </summary>
            /// <param name="m">Message</param>
            protected override void WndProc(ref SWF.Message m)
            {
                if (m.Msg == 522)
                {
                    int delta = SignedHIWORD(m.WParam);
                    CurrentWheelValue += delta;
                }

                base.WndProc(ref m);
            }

            /// <summary>
            /// Gets the signed word from the high portion of the input
            /// </summary>
            /// <param name="n">Input integer</param>
            /// <returns>High bits of the input as a signed integer</returns>
            private static int SignedHIWORD(IntPtr n)
            {
                return SignedHIWORD((int)((long)n));
            }

            /// <summary>
            /// Gets the signed word from the high portion of the input
            /// </summary>
            /// <param name="n">Input integer</param>
            /// <returns>High bits of the input as a signed integer</returns>
            private static int SignedHIWORD(int n)
            {
                return (short)((n >> 16) & 65535);
            }
        }
    }
}
