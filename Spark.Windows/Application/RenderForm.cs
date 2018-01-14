namespace Spark.Windows.Application
{
    using System;
    using System.Windows.Forms;

    using Math;

    using SD = System.Drawing;

    /// <summary>
    /// Specialized <see cref="Form" /> for rendering graphics in a WinForms application.
    /// </summary>
    public class RenderForm : Form, IWinFormsControlAdapter
    {
        private bool _enableUserResize;
        private bool _enableInputEvents;
        private bool _enablePaintEvents;
        private bool _isMouseVisible;
        private bool _mouseIsHidden;

        private SD.Font _fontForDesignMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderForm"/> class.
        /// </summary>
        public RenderForm() 
            : this("Spark RenderForm", 300, 300)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderForm"/> class.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderForm(int width, int height) 
            : this("Spark RenderForm", width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderForm"/> class.
        /// </summary>
        /// <param name="title">Title text of the control.</param>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public RenderForm(string title, int width, int height)
        {
            _isMouseVisible = true;
            _mouseIsHidden = false;
            _enableInputEvents = false;
            _enablePaintEvents = false;

            SuspendLayout();

            EnableUserResizing = true;
            ClientSize = new SD.Size(width, height);
            StartPosition = FormStartPosition.CenterScreen;
            Text = title;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
            CausesValidation = false;
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SD.SizeF(6.0f, 13.0f);
            ResizeRedraw = true;

            ResumeLayout();
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
        /// Occurs when the control is painted.
        /// </summary>
        public event EventHandler Render;

        /// <summary>
        /// Gets or sets if the control can be resized by the user.
        /// </summary>
        public bool EnableUserResizing
        {
            get => _enableUserResize;
            set
            {
                _enableUserResize = value;
                FormBorderStyle = (value) ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
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
                SD.Point ptC = base.Location;
                return new Int2(ptC.X, ptC.Y);
            }
            set => base.Location = new SD.Point(value.X, value.Y);
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
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
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
            if (_enableInputEvents)
            {
                base.OnMouseLeave(e);
            }

            ToggleCursorVisible();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.ResizeBegin" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            OnSuspendRendering(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.ResizeEnd" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            OnResumeRendering(e);
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
        /// Called when the form needs to be repainted.
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
                var text = "Spark RenderForm";
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

                    // Mouse wheel
                    case 522:

                    // Mouse xbuttons
                    case 523:
                    case 524:
                    case 525:

                    // Mouse hover
                    case 673:
                        return;
                }
            }

            base.WndProc(ref m);
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
    }
}
