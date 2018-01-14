namespace Spark.Windows.Application
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;

    using Math;

    using SWF = System.Windows.Forms;

    /// <summary>
    /// Specialized <see cref="HwndHost"/> for rendering graphics in a Windows Presentation Foundation (WPF) application by way of a
    /// hosted <see cref="RenderControl"/>.
    /// </summary>
    public sealed class RenderHwndHost : HwndHost, IWinFormsControlAdapter
    {
        private const int WS_CAPTION = 12582912;
        private const int WS_THICKFRAME = 262144;
        private const int WS_CHILD = 1073741824;

        private readonly IWinFormsControlAdapter _swfControl;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderHwndHost"/> class.
        /// </summary>
        public RenderHwndHost() 
            : this("Spark RenderElement", 200, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderHwndHost"/> class.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderHwndHost(int width, int height) 
            : this("Spark RenderElement", width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderHwndHost"/> class.
        /// </summary>
        /// <param name="title">Title text of the control.</param>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderHwndHost(String title, int width, int height)
        {
            _swfControl = CreateRenderControl(title, width, height);

            Focusable = true;
        }

        /// <summary>
        /// Occurs when the control is activated (gains focus).
        /// </summary>
        event EventHandler IWinFormsControlAdapter.GotFocus
        {
            add => _swfControl.GotFocus += value;
            remove => _swfControl.GotFocus -= value;
        }

        /// <summary>
        /// Occurs when the control is deactivated (loses focus).
        /// </summary>
        event EventHandler IWinFormsControlAdapter.LostFocus
        {
            add => _swfControl.LostFocus += value;
            remove => _swfControl.LostFocus -= value;
        }

        /// <summary>
        /// Occurs when rendering should resume, after resizing of the control ends.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.ResumeRendering
        {
            add => _swfControl.ResumeRendering += value;
            remove => _swfControl.ResumeRendering -= value;
        }

        /// <summary>
        /// Occurs when rendering should suspend, when resizing of the control begins.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.SuspendRendering
        {
            add => _swfControl.SuspendRendering += value;
            remove => _swfControl.SuspendRendering -= value;
        }

        /// <summary>
        /// Occurs when the control is painted.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.Render
        {
            add => _swfControl.Render += value;
            remove => _swfControl.Render -= value;
        }

        /// <summary>
        /// Occurs when the control is resizing.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.Resize
        {
            add => _swfControl.Resize += value;
            remove => _swfControl.Resize -= value;
        }

        /// <summary>
        /// Occurs when the control is closed.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.Closed
        {
            add => _swfControl.Closed += value;
            remove => _swfControl.Closed -= value;
        }

        /// <summary>
        /// Occurs when the control is disposed.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.Disposed
        {
            add => _swfControl.Disposed += value;
            remove => _swfControl.Disposed -= value;
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        event SWF.KeyEventHandler IWinFormsControlAdapter.KeyDown
        {
            add => _swfControl.KeyDown += value;
            remove => _swfControl.KeyDown -= value;
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        event SWF.KeyPressEventHandler IWinFormsControlAdapter.KeyPress
        {
            add => _swfControl.KeyPress += value;
            remove => _swfControl.KeyPress -= value;
        }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        event SWF.KeyEventHandler IWinFormsControlAdapter.KeyUp
        {
            add => _swfControl.KeyUp += value;
            remove => _swfControl.KeyUp -= value;
        }

        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseClick
        {
            add => _swfControl.MouseClick += value;
            remove => _swfControl.MouseClick -= value;
        }

        /// <summary>
        /// Occurs when the control is double clicked by the mouse.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseDoubleClick
        {
            add => _swfControl.MouseDoubleClick += value;
            remove => _swfControl.MouseDoubleClick -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is pressed.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseDown
        {
            add => _swfControl.MouseDown += value;
            remove => _swfControl.MouseDown -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseUp
        {
            add => _swfControl.MouseUp += value;
            remove => _swfControl.MouseUp -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.MouseEnter
        {
            add => _swfControl.MouseEnter += value;
            remove => _swfControl.MouseEnter -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer rests on the control.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.MouseHover
        {
            add => _swfControl.MouseHover += value;
            remove => _swfControl.MouseHover -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        event EventHandler IWinFormsControlAdapter.MouseLeave
        {
            add => _swfControl.MouseLeave += value;
            remove => _swfControl.MouseLeave -= value;
        }

        /// <summary>
        /// Occurs when the mouse pointer is moved over the control.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseMove
        {
            add => _swfControl.MouseMove += value;
            remove => _swfControl.MouseMove -= value;
        }

        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        event SWF.MouseEventHandler IWinFormsControlAdapter.MouseWheel
        {
            add => _swfControl.MouseWheel += value;
            remove => _swfControl.MouseWheel -= value;
        }

        /// <summary>
        /// Gets if the control has been disposed.
        /// </summary>
        bool IWinFormsControlAdapter.IsDisposed => _isDisposed;

        /// <summary>
        /// Gets or sets if the control can be resized by the user.
        /// </summary>
        bool IWinFormsControlAdapter.EnableUserResizing
        {
            get => _swfControl.EnableUserResizing;
            set => _swfControl.EnableUserResizing = value;
        }

        /// <summary>
        /// Gets or sets if the control should forward keyboard and mouse input events.
        /// </summary>
        bool IWinFormsControlAdapter.EnableInputEvents
        {
            get => _swfControl.EnableInputEvents;
            set => _swfControl.EnableInputEvents = value;
        }

        /// <summary>
        /// Gets or sets if the control should forward paint events.
        /// </summary>
        bool IWinFormsControlAdapter.EnablePaintEvents
        {
            get => _swfControl.EnablePaintEvents;
            set => _swfControl.EnablePaintEvents = value;
        }

        /// <summary>
        /// Gets if the control currently has input focus.
        /// </summary>
        bool IWinFormsControlAdapter.Focused => _swfControl.Focused;

        /// <summary>
        /// Gets or sets the screen location of the control.
        /// </summary>
        Int2 IWinFormsControlAdapter.Location
        {
            get => _swfControl.Location;
            set => _swfControl.Location = value;
        }

        /// <summary>
        /// Gets or sets the size of the control.
        /// </summary>
        Int2 IWinFormsControlAdapter.ClientSize
        {
            get => _swfControl.ClientSize;
            set => _swfControl.ClientSize = value;
        }

        /// <summary>
        /// Gets the client bounds of the control.
        /// </summary>
        Rectangle IWinFormsControlAdapter.ClientBounds => _swfControl.ClientBounds;

        /// <summary>
        /// Gets the bounds of the screen that contains the majority of the control.
        /// </summary>
        Rectangle IWinFormsControlAdapter.ScreenBounds => _swfControl.ScreenBounds;

        /// <summary>
        /// Gets or sets if the mouse should be visible when it hovers over the control.
        /// </summary>
        bool IWinFormsControlAdapter.IsMouseVisible
        {
            get => _swfControl.IsMouseVisible;
            set => _swfControl.IsMouseVisible = value;
        }

        /// <summary>
        /// Gets if the control is minimized.
        /// </summary>
        bool IWinFormsControlAdapter.IsMinimized => _swfControl.IsMinimized;

        /// <summary>
        /// Get the underlying WinForm control.
        /// </summary>
        SWF.Control IWinFormsControlAdapter.Control => _swfControl.Control;

        /// <summary>
        /// When overridden in a derived class, creates the window to be hosted.
        /// </summary>
        /// <param name="hwndParent">The window handle of the parent window.</param>
        /// <returns>The handle to the child Win32 window to create.</returns>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var childRef = new HandleRef(this, _swfControl.Control.Handle);

            int style = NativeMethods.GetWindowLong(childRef, WindowLongType.Style).ToInt32();
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            style |= WS_CHILD;

            NativeMethods.SetWindowLong(childRef, WindowLongType.Style, new IntPtr(style));
            NativeMethods.SetParent(childRef, hwndParent.Handle);
            NativeMethods.ShowWindow(childRef, false);

            return childRef;
        }

        /// <summary>
        /// When overridden in a derived class, destroys the hosted window.
        /// </summary>
        /// <param name="hwnd">A structure that contains the window handle.</param>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.SetParent(hwnd, IntPtr.Zero);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Integration.WindowsFormsHost" />, and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_isDisposed && disposing)
            {
                _swfControl.Dispose();
            }

            _isDisposed = true;
        }

        private IWinFormsControlAdapter CreateRenderControl(String title, int width, int height)
        {
            RenderControl rc = new RenderControl(title, width, height);
            rc.EnableInputEvents = true;
            rc.EnablePaintEvents = true;
            rc.EnableUserResizing = true;
            rc.Dock = SWF.DockStyle.Fill;

            return rc;
        }
    }
}
