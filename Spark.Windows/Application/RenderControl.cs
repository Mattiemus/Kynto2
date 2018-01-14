namespace Spark.Windows.Application
{
    using System;
    using System.Windows.Forms;

    using Math;

    using SD = System.Drawing;

    /// <summary>
    /// Specialized <see cref="UserControl"/> for rendering graphics in a WinForms application.
    /// </summary>
    public class RenderControl : UserControl, IWinFormsControlAdapter
    {
        private Form _parentForm;
        private bool _enableUserResize;
        private bool _enableInputEvents;
        private bool _enablePaintEvents;
        private bool _isMouseVisible;
        private bool _mouseIsHidden;
        private bool _mouseEntered;

        private SD.Font _fontForDesignMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControl"/> class.
        /// </summary>
        public RenderControl() 
            : this("Spark RenderControl", 200, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControl"/> class.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderControl(int width, int height) 
            : this("Spark RenderControl", width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControl"/> class.
        /// </summary>
        /// <param name="title">Title text of the control.</param>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderControl(string title, int width, int height)
        {
            _isMouseVisible = true;
            _mouseIsHidden = false;
            _enableInputEvents = false;
            _enablePaintEvents = false;

            SuspendLayout();

            SetStyle(ControlStyles.ContainerControl, false);
            SetStyle(ControlStyles.Selectable, true);

            EnableUserResizing = true;
            ClientSize = new SD.Size(width, height);
            Text = title;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
            CausesValidation = false;
            ResizeRedraw = true;
        }

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
            get => _enableInputEvents;
            set => _enableInputEvents = value;
        }

        /// <summary>
        /// Gets or sets if the control should forward paint events.
        /// </summary>
        public bool EnablePaintEvents
        {
            get => _enablePaintEvents;
            set => _enablePaintEvents = value;
        }

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

                SD.Point ptC = base.Location;
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
                SD.Size clientSize = ClientSize;
                return new Int2(clientSize.Width, clientSize.Height);
            }
            set => base.ClientSize = new SD.Size(value.X, value.Y);
        }

        /// <summary>
        /// Gets the client bounds of the control.
        /// </summary>
        public Rectangle ClientBounds
        {
            get
            {
                SD.Point pt = PointToScreen(SD.Point.Empty);
                SD.Size clientSize = ClientSize;
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
                SD.Rectangle screenBounds = Screen.GetBounds(this);
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
        public bool IsMinimized => ClientSize == SD.Size.Empty;

        /// <summary>
        /// Get the underlying WinForm control.
        /// </summary>
        public Control Control => this;

        /// <summary>
        /// Invoked when the parent window changes
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            RemoveParentOnResizeEvents();
            _parentForm = FindForm();
            AddParentOnResizeEvents();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseEntered = true;

            if (_enableInputEvents)
            {
                base.OnMouseEnter(e);
            }

            ToggleCursorVisible();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseEntered = false;

            if (_enableInputEvents)
            {
                base.OnMouseLeave(e);
            }

            ToggleCursorVisible();
        }

        /// <summary>
        /// Raises the <see cref="E:ResumeRendering" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnResumeRendering(EventArgs e)
        {
            ResumeRendering?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SuspendRendering" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSuspendRendering(EventArgs e)
        {
            SuspendRendering?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Render" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRender(EventArgs e)
        {
            Render?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Closed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
            {
                if (_fontForDesignMode == null)
                {
                    _fontForDesignMode = new SD.Font("Calibri", 24f, SD.FontStyle.Regular);
                }

                e.Graphics.Clear(SD.Color.CornflowerBlue);
                var text = "Spark RenderControl";
                var size = e.Graphics.MeasureString(text, _fontForDesignMode);
                var pt = new SD.PointF();

                pt.X = (Width - size.Width) / 2.0f;
                pt.Y = (Height - size.Height) / 2.0f;
                e.Graphics.DrawString(text, _fontForDesignMode, new SD.SolidBrush(SD.Color.Black), pt.X, pt.Y);
            }
            else if (_enablePaintEvents)
            {
                OnRender(e);
            }
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaintBackground(e);
            }
        }

        /// <summary>
        /// Windows proc handler.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            // Regardless if we're handling input, if mouse entered and any button was clicked, make sure this gives us focus
            switch (m.Msg)
            {
                // Left mouse button
                case 513:
                case 514:
                case 515:

                // Right mouse button
                case 516:
                case 517:
                case 518:

                // Middle mouse button
                case 519:
                case 520:
                case 521:

                // Mouse xbuttons
                case 523:
                case 524:
                case 525:
                    {
                        if (_mouseEntered && !Focused)
                        {
                            Focus();
                        }

                        if (!_enableInputEvents)
                            return;
                    }
                    break;
            }

            if (!_enableInputEvents)
            {
                switch (m.Msg)
                {
                    // Keys
                    case 256:
                    case 257:
                    case 258:
                    case 260:
                    case 261:

                    // Mouse move
                    case 512:

                    // Mouse wheel
                    case 522:

                    // Mouse hover
                    case 673:
                        return;
                }
            }

            // Try and handle certain WM messages if we don't have a parent form (e.g. the control is a child of a native Win32 window that doesn't have
            // a .NET equivalent
            if (_parentForm == null)
            {
                switch (m.Msg)
                {
                    // WM_SIZE
                    case 5:
                        SetClientSizeFromParent();
                        OnResize(EventArgs.Empty);
                        break;
                    // WM_ENTERSIZEMOVE
                    case 561:
                        OnSuspendRendering(EventArgs.Empty);
                        break;
                    // WM_EXITSIZEMOVE
                    case 562:
                        OnResumeRendering(EventArgs.Empty);
                        break;
                    // WM_DESTROY
                    case 2:
                        OnClosed(EventArgs.Empty);
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void SetClientSizeFromParent()
        {
            IntPtr parentHwnd = NativeMethods.GetParent(Handle);
            if (parentHwnd == IntPtr.Zero)
            {
                return;
            }

            RECT clientRect;
            NativeMethods.GetClientRect(parentHwnd, out clientRect);
            ClientSize = new SD.Size(clientRect.Right, clientRect.Bottom);
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

        private void RemoveParentOnResizeEvents()
        {
            if (_parentForm != null)
            {
                _parentForm.ResizeBegin -= RenderControl_ResizeBegin;
                _parentForm.ResizeEnd -= RenderControl_ResizeEnd;
                _parentForm.FormClosed -= RenderControl_FormClosed;
            }
        }

        private void AddParentOnResizeEvents()
        {
            if (_parentForm != null)
            {
                _parentForm.ResizeBegin += RenderControl_ResizeBegin;
                _parentForm.ResizeEnd += RenderControl_ResizeEnd;
                _parentForm.FormClosed += RenderControl_FormClosed;
            }
        }

        private void RenderControl_ResizeBegin(object sender, EventArgs e)
        {
            OnSuspendRendering(e);
        }

        private void RenderControl_ResizeEnd(object sender, EventArgs e)
        {
            OnResumeRendering(e);
        }

        private void RenderControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnClosed(e);
        }
    }
}
