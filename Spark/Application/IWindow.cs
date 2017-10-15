namespace Spark.Application
{
    using System;

    using Core;
    using Math;
    using Graphics;

    public interface IWindow : IDisposable
    {
        event TypedEventHandler<IWindow> ClientSizeChanged;
        event TypedEventHandler<IWindow> OrientationChanged;
        event TypedEventHandler<IWindow> GotFocus;
        event TypedEventHandler<IWindow> LostFocus;
        event TypedEventHandler<IWindow> ResumeRendering;
        event TypedEventHandler<IWindow> SuspendRendering;
        event TypedEventHandler<IWindow> Paint;
        event TypedEventHandler<IWindow> Closed;
        event TypedEventHandler<IWindow> Disposed;

        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseClick;
        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseDoubleClick;
        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseUp;
        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseDown;
        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseMove;
        event TypedEventHandler<IWindow, MouseStateEventArgs> MouseWheel;

        event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyDown;
        event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyPress;
        event TypedEventHandler<IWindow, KeyboardStateEventArgs> KeyUp;

        event TypedEventHandler<IWindow> MouseEnter;
        event TypedEventHandler<IWindow> MouseLeave;
        event TypedEventHandler<IWindow> MouseHover;

        bool EnableUserResizing { get; set; }
        bool EnableInputEvents { get; set; }
        bool EnablePaintEvents { get; set; }
        bool EnableResizeRedraw { get; set; }
        bool IsMinimized { get; }
        bool IsMouseVisible { get; set; }
        bool IsFullScreen { get; set; }
        bool IsVisible { get; set; }
        bool IsTopLevelWindow { get; }
        bool IsDisposed { get; }
        bool Focused { get; }

        Int2 Location { get; set; }
        Rectangle ClientBounds { get; }
        Rectangle ScreenBounds { get; }
        IntPtr Handle { get; }

        SwapChain SwapChain { get; }
        string Title { get; set; }
        object NativeWindow { get; }
        object Tag { get; }
        ApplicationHost Host { get; }

        void Focus();
        void CenterToScreen();
        void Close();
        void Repaint();
        void Reset(PresentationParameters pp);
        void Resize(int width, int height);
    }
}
