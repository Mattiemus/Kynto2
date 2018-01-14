namespace Spark.Windows.Application
{
    using System;
    using System.Windows.Forms;

    using Math;

    /// <summary>
    /// Defines an adapter for treating Windows Forms controls generically.
    /// </summary>
    public interface IWinFormsControlAdapter : IDisposable
    {
        /// <summary>
        /// Occurs when the control is activated (gains focus).
        /// </summary>
        event EventHandler GotFocus;

        /// <summary>
        /// Occurs when the control is deactivated (loses focus).
        /// </summary>
        event EventHandler LostFocus;

        /// <summary>
        /// Occurs when rendering should resume, after resizing of the control ends.
        /// </summary>
        event EventHandler ResumeRendering;

        /// <summary>
        /// Occurs when rendering should suspend, when resizing of the control begins.
        /// </summary>
        event EventHandler SuspendRendering;

        /// <summary>
        /// Occurs when the control is painted.
        /// </summary>
        event EventHandler Render;

        /// <summary>
        /// Occurs when the control is resizing.
        /// </summary>
        event EventHandler Resize;

        /// <summary>
        /// Occurs when the control is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occurs when the control is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        event KeyEventHandler KeyUp;

        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        event MouseEventHandler MouseClick;

        /// <summary>
        /// Occurs when the control is double clicked by the mouse.
        /// </summary>
        event MouseEventHandler MouseDoubleClick;

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is pressed.
        /// </summary>
        event MouseEventHandler MouseDown;

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        event MouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        event EventHandler MouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer rests on the control.
        /// </summary>
        event EventHandler MouseHover;

        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        event EventHandler MouseLeave;

        /// <summary>
        /// Occurs when the mouse pointer is moved over the control.
        /// </summary>
        event MouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        event MouseEventHandler MouseWheel;

        /// <summary>
        /// Gets if the control has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets or sets if the control can be resized by the user.
        /// </summary>
        bool EnableUserResizing { get; set; }

        /// <summary>
        /// Gets or sets if the control will invoke the resize event at the very end of resizing.
        /// If false, then resize will be invoked repeatedly as the control resizes. Suspend/Resume events will
        /// still be called.
        /// </summary>
        //bool EnableResizeEventOnEnd { get; set; }

        /// <summary>
        /// Gets or sets if the control should forward keyboard and mouse input events.
        /// </summary>
        bool EnableInputEvents { get; set; }

        /// <summary>
        /// Gets or sets if the control should forward paint events.
        /// </summary>
        bool EnablePaintEvents { get; set; }

        /// <summary>
        /// Gets if the control currently has input focus.
        /// </summary>
        bool Focused { get; }

        /// <summary>
        /// Gets or sets the screen location of the control.
        /// </summary>
        Int2 Location { get; set; }

        /// <summary>
        /// Gets or sets the size of the control.
        /// </summary>
        Int2 ClientSize { get; set; }

        /// <summary>
        /// Gets the client bounds of the control.
        /// </summary>
        Rectangle ClientBounds { get; }

        /// <summary>
        /// Gets the bounds of the screen that contains the majority of the control.
        /// </summary>
        Rectangle ScreenBounds { get; }

        /// <summary>
        /// Gets or sets if the mouse should be visible when it hovers over the control.
        /// </summary>
        bool IsMouseVisible { get; set; }

        /// <summary>
        /// Gets if the control is minimized.
        /// </summary>
        bool IsMinimized { get; }

        /// <summary>
        /// Get the underlying WinForm control.
        /// </summary>
        Control Control { get; }
    }
}
