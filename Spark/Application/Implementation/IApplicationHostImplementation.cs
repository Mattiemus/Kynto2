namespace Spark.Application.Implementation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    
    using Graphics;
    
    /// <summary>
    /// Defines an implementation for <see cref="ApplicationHost"/>.
    /// </summary>
    public interface IApplicationHostImplementation : IDisposable, INamable
    {
        /// <summary>
        /// Gets if the host has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the application system that created the host.
        /// </summary>
        IApplicationSystem ApplicationSystem { get; }

        /// <summary>
        /// Gets the host that is bound to this implementation.
        /// </summary>
        ApplicationHost Parent { get; }

        /// <summary>
        /// Gets the thread the host is running on.
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// Gets the main UI window the host owns.
        /// </summary>
        IWindow MainWindow { get; }

        /// <summary>
        /// Gets the collection of UI windows the host owns.
        /// </summary>
        WindowCollection Windows { get; }

        /// <summary>
        /// Gets whether the host's message loop is running or not.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Runs the application host's message loop in headless mode. On idle (after message processing) the callback will be invoked. If
        /// a window is created after the application host is already running, then the first window created is assumed to be the main window and then
        /// the lifetime of the host is dictated by that window.
        /// </summary>
        /// <param name="runCallback">Loop callback</param>
        void Run(Action runCallback);

        /// <summary>
        /// Runs the application host's message loop using the specified window. On idle (after message processing) the callback will be invoked.
        /// The lifetime of the application host is dictated by the specified window, which becomes the main window.
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="runCallback">Loop callback</param>
        void Run(IWindow mainWindow, Action runCallback);

        /// <summary>
        /// Explicitly shuts down the application host. If in headless mode, then this is required to be called.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Invokes the action on the thread that the application host runs its message loop on (aka run on UI thread). This
        /// returns immediately with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task Invoke(Action action);

        /// <summary>
        /// Invokes the fucnction on the thread that the application host runs its message loop on (aka run on UI thread). This
        /// returns immediately with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <typeparam name="TResult">Type of result to return</typeparam>
        /// <param name="action">Action that returns a result</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task<TResult> Invoke<TResult>(Func<TResult> action);

        /// <summary>
        /// Creates a new UI window owned by the application host.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="presentParams">Presentation options for the window.</param>
        /// <param name="isTopLevelWindow">Notifies whether the window should be a top level container (e.g. a Form).</param>
        /// <returns>The newly created window.</returns>
        IWindow CreateWindow(IRenderSystem renderSystem, PresentationParameters presentParams, bool isTopLevelWindow);

        /// <summary>
        /// Creates a UI window adapter to an existing native window.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="nativeWindow">Native window to adapt.</param>
        /// <param name="templatePresentationParameters">Optional template presentation parameters. Some properties may be ignored.</param>
        /// <param name="resizeNativeWindow">True if the native window should be resized based on template presentation parameters, false if those
        /// properties should be ignored.</param>
        /// <returns>The UI window adapter</returns>
        IWindow FromNativeWindow(IRenderSystem renderSystem, object nativeWindow, PresentationParameters? templatePresentationParameters, bool resizeNativeWindow);
    }
}
