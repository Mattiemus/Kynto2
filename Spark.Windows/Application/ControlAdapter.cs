namespace Spark.Windows.Application
{
    using System;
    using System.Windows.Forms;

    using Math;

    using SD = System.Drawing;

    /// <summary>
    /// Adapter for any WinForms control that will be used to render graphics to.
    /// </summary>
    public sealed class ControlAdapter : IWinFormsControlAdapter
    {
        private Form _parentForm;
        private bool _enableUserResize;
        private bool _isMouseVisible;
        private bool _mouseIsHidden;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlAdapter"/> class.
        /// </summary>
        /// <param name="control">Control to adapt.</param>
        public ControlAdapter(Control control)
        {
            Control = control;
            _isMouseVisible = true;
            _mouseIsHidden = false;
            _enableUserResize = false;

            Control.CausesValidation = false;
            _parentForm = Control.FindForm();

            StartEvents();
        }

        ~ControlAdapter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Occurs when the control is activated (gains focus).
        /// </summary>
        public event EventHandler GotFocus;

        /// <summary>
        /// Occurs when the control is deactivated (loses focus).
        /// </summary>
        public event EventHandler LostFocus;

        /// <summary>
        /// Occurs when rendering should resume, after resizing of the control ends.
        /// </summary>
        public event EventHandler ResumeRendering;

        /// <summary>
        /// Occurs when rendering should suspend, when resizing of the control begins.
        /// </summary>
        public event EventHandler SuspendRendering;

        /// <summary>
        /// Occurs when the control is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the control is painted.
        /// </summary>
        public event EventHandler Render;

        /// <summary>
        /// Occurs when the control is resizing.
        /// </summary>
        public event EventHandler Resize;

        /// <summary>
        /// Occurs when the control is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        public event KeyEventHandler KeyUp;

        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        public event MouseEventHandler MouseClick;

        /// <summary>
        /// Occurs when the control is double clicked by the mouse.
        /// </summary>
        public event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is pressed.
        /// </summary>
        public event MouseEventHandler MouseDown;

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        public event MouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        public event EventHandler MouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer rests on the control.
        /// </summary>
        public event EventHandler MouseHover;

        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        public event EventHandler MouseLeave;

        /// <summary>
        /// Occurs when the mouse pointer is moved over the control.
        /// </summary>
        public event MouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        public event MouseEventHandler MouseWheel;

        /// <summary>
        /// Gets if the control has been disposed.
        /// </summary>
        public bool IsDisposed => Control == null || Control.IsDisposed;

        /// <summary>
        /// Gets or sets if the control can be resized by the user. This will affect the parent form that owns the user control.
        /// </summary>
        public bool EnableUserResizing
        {
            get => _enableUserResize;
            set
            {
                _enableUserResize = value;
                if (_parentForm == null)
                {
                    return;
                }

                _parentForm.FormBorderStyle = (value) ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }
        }

        /// <summary>
        /// Gets or sets if the control should forward keyboard and mouse input events.
        /// </summary>
        public bool EnableInputEvents
        {
            get => true;
            set
            {
                //Do nothing, will always receive input events
            }
        }

        /// <summary>
        /// Gets or sets if the control should forward paint events.
        /// </summary>
        public bool EnablePaintEvents
        {
            get => true;
            set
            {
                // Do nothing, will always receive paint events
            }
        }

        /// <summary>
        /// Gets if the control currently has input focus.
        /// </summary>
        public bool Focused => Control.Focused;

        /// <summary>
        /// Gets or sets the screen location of the control.
        /// </summary>
        Int2 IWinFormsControlAdapter.Location
        {
            get
            {
                if (_parentForm != null)
                {
                    SD.Point pt = _parentForm.Location;
                    return new Int2(pt.X, pt.Y);
                }

                SD.Point ptC = Control.Location;
                return new Int2(ptC.X, ptC.Y);
            }
            set
            {
                if (_parentForm != null)
                {
                    _parentForm.Location = new SD.Point(value.X, value.Y);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the control.
        /// </summary>
        Int2 IWinFormsControlAdapter.ClientSize
        {
            get
            {
                SD.Size clientSize = Control.ClientSize;
                return new Int2(clientSize.Width, clientSize.Height);
            }
            set => Control.ClientSize = new SD.Size(value.X, value.Y);
        }

        /// <summary>
        /// Gets the client bounds of the control.
        /// </summary>
        public Rectangle ClientBounds
        {
            get
            {
                SD.Point pt = Control.PointToScreen(SD.Point.Empty);
                SD.Size clientSize = Control.ClientSize;
                return new Rectangle(pt.X, pt.Y, clientSize.Width, clientSize.Height);
            }
        }

        /// <summary>
        /// Gets the bounds of the screen that contains the majority of the control.
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                SD.Rectangle screenBounds = Screen.GetBounds(Control);
                return new Rectangle(screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            }
        }

        /// <summary>
        /// Gets or sets if the mouse should be visible when it hovers over the control.
        /// </summary>
        public bool IsMouseVisible
        {
            get => _isMouseVisible;
            set
            {
                if (_isMouseVisible != value)
                {
                    _isMouseVisible = value;
                    ToggleCursorVisible();
                }
            }
        }

        /// <summary>
        /// Gets if the control is minimized.
        /// </summary>
        public bool IsMinimized => Control.ClientSize == SD.Size.Empty;

        /// <summary>
        /// Get the underlying WinForm control.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Disposed?.Invoke(this, EventArgs.Empty);

                if (Control != null && !Control.IsDisposed && !Control.Disposing)
                {
                    StopEvents();
                }
            }
        }

        private void ToggleCursorVisible()
        {
            if (_isMouseVisible && _mouseIsHidden)
            {
                Cursor.Show();
                _mouseIsHidden = false;
            }
            else if (!_isMouseVisible && !_mouseIsHidden)
            {
                Cursor.Hide();
                _mouseIsHidden = true;
            }
        }

        private void StartEvents()
        {
            if (Control == null)
            {
                return;
            }

            AddParentOnResizeEvents();

            Control.Disposed += Control_Disposed;
            Control.ParentChanged += Control_ParentChanged;
            Control.GotFocus += Control_GotFocus;
            Control.LostFocus += Control_LostFocus;
            Control.Paint += Control_Paint;
            Control.Resize += Control_Resize;
            Control.KeyDown += Control_KeyDown;
            Control.KeyPress += Control_KeyPress;
            Control.KeyUp += Control_KeyUp;
            Control.MouseClick += Control_MouseClick;
            Control.MouseDoubleClick += Control_MouseDoubleClick;
            Control.MouseDown += Control_MouseDown;
            Control.MouseUp += Control_MouseUp;
            Control.MouseEnter += Control_MouseEnter;
            Control.MouseHover += Control_MouseHover;
            Control.MouseLeave += Control_MouseLeave;
            Control.MouseMove += Control_MouseMove;
            Control.MouseWheel += Control_MouseWheel;
        }

        private void StopEvents()
        {
            if (Control == null)
            {
                return;
            }

            RemoveParentOnResizeEvents();

            Control.Disposed -= Control_Disposed;
            Control.ParentChanged -= Control_ParentChanged;
            Control.GotFocus -= Control_GotFocus;
            Control.LostFocus -= Control_LostFocus;
            Control.Paint -= Control_Paint;
            Control.Resize -= Control_Resize;
            Control.KeyDown -= Control_KeyDown;
            Control.KeyPress -= Control_KeyPress;
            Control.KeyUp -= Control_KeyUp;
            Control.MouseClick -= Control_MouseClick;
            Control.MouseDoubleClick -= Control_MouseDoubleClick;
            Control.MouseDown -= Control_MouseDown;
            Control.MouseUp -= Control_MouseUp;
            Control.MouseEnter -= Control_MouseEnter;
            Control.MouseHover -= Control_MouseHover;
            Control.MouseLeave -= Control_MouseLeave;
            Control.MouseMove -= Control_MouseMove;
            Control.MouseWheel -= Control_MouseWheel;
        }

        private void Control_Disposed(object sender, EventArgs e)
        {
            StopEvents();
        }

        private void Control_ParentChanged(object sender, EventArgs e)
        {
            RemoveParentOnResizeEvents();
            _parentForm = Control.FindForm();
            AddParentOnResizeEvents();
        }

        private void Control_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            Render?.Invoke(this, e);
        }

        private void Control_Resize(object sender, EventArgs e)
        {
            Resize?.Invoke(this, e);
        }

        private void Control_ResizeBegin(object sender, EventArgs e)
        {
            SuspendRendering?.Invoke(this, e);
        }

        private void Control_ResizeEnd(object sender, EventArgs e)
        {
            ResumeRendering?.Invoke(this, e);
        }

        private void Control_FormClosed(object sender, FormClosedEventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        private void Control_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
            ToggleCursorVisible();
        }

        private void Control_MouseHover(object sender, EventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
            ToggleCursorVisible();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }

        private void RemoveParentOnResizeEvents()
        {
            if (_parentForm != null)
            {
                _parentForm.ResizeBegin -= Control_ResizeBegin;
                _parentForm.ResizeEnd -= Control_ResizeEnd;
                _parentForm.FormClosed -= Control_FormClosed;
            }
        }

        private void AddParentOnResizeEvents()
        {
            if (_parentForm != null)
            {
                _parentForm.ResizeBegin += Control_ResizeBegin;
                _parentForm.ResizeEnd += Control_ResizeEnd;
                _parentForm.FormClosed += Control_FormClosed;
            }
        }
    }
}
