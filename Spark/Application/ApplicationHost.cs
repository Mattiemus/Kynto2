namespace Spark.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Core;
    using Implementation;
    using Graphics;

    /// <summary>
    /// Represents a mechanism to host an application, supplying a UI message loop implementation. The message loop serves as a means to
    /// create a "game loop" where a supplied callback is invoked after processing window messages ("On Idle"). The host is multipurpose
    /// where it can create and run a simple top-level window (e.g. Form) that runs a game application, or serve as an adapter to interface
    /// with an existing native UI application allowing you to separate drawing code from application code to promote re-usability.
    /// </summary>
    public class ApplicationHost : IDisposable, INamable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationHost"/> class.
        /// </summary>
        /// <param name="appSystem">The application system used to create the underlying implementation.</param>
        public ApplicationHost(IApplicationSystem appSystem)
        {
            CreateImplementation(appSystem);
        }

        /// <summary>
        /// Occurs when the host is in the process of being disposed.
        /// </summary>
        public event TypedEventHandler<ApplicationHost> Disposing;

        /// <summary>
        /// Gets if the host has been disposed or not.
        /// </summary>
        public bool IsDisposed => Implementation.IsDisposed;

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        public string Name
        {
            get => Implementation.Name;
            set => Implementation.Name = value;
        }

        /// <summary>
        /// Gets or sets custom data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the application system that created the host.
        /// </summary>
        public IApplicationSystem ApplicationSystem => Implementation.ApplicationSystem;

        /// <summary>
        /// Gets the underlying platform-specific implementation of the host.
        /// </summary>
        public IApplicationHostImplementation Implementation { get; private set; }

        /// <summary>
        /// Gets the main UI window the host owns.
        /// </summary>
        public IWindow MainWindow => Implementation.MainWindow;

        /// <summary>
        /// Gets the collection of UI windows the host owns.
        /// </summary>
        public WindowCollection Windows => Implementation.Windows;

        /// <summary>
        /// Gets the thread the host is running on.
        /// </summary>
        public Thread Thread => Implementation.Thread;

        /// <summary>
        /// Gets whether the host's message loop is running or not.
        /// </summary>
        public bool IsRunning => Implementation.IsRunning;

        /// <summary>
        /// Gets of the host is in headless mode or not. Headless mode means a UI isn't running, just the "game loop".
        /// </summary>
        public bool IsHeadless => Implementation.MainWindow == null;

        #region Public Methods

        /// <summary>
        /// Runs the application host's message loop in headless mode. On idle (after message processing) the callback will be invoked. If
        /// a window is created after the application host is already running, then the first window created is assumed to be the main window and then
        /// the lifetime of the host is dictated by that window.
        /// </summary>
        /// <param name="runCallback">Loop callback</param>
        public void Run(Action runCallback)
        {
            ThrowIfDisposed();

            if (IsRunning)
            {
                throw new ArgumentException("Application host cannot be run multiple times");
            }

            try
            {
                Implementation.Run(runCallback);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Error while running application", e);
            }
        }

        /// <summary>
        /// Runs the application host's message loop using the specified window. On idle (after message processing) the callback will be invoked.
        /// The lifetime of the application host is dictated by the specified window, which becomes the main window.
        /// </summary>
        /// <param name="mainWindow">Main window to run the message loop on</param>
        /// <param name="runCallback">Loop callback</param>
        public void Run(IWindow mainWindow, Action runCallback)
        {
            ThrowIfDisposed();

            if (IsRunning)
            {
                throw new ArgumentException("Application host cannot be run multiple times");
            }

            try
            {
                Implementation.Run(mainWindow, runCallback);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Error while running application", e);
            }
        }

        /// <summary>
        /// Explicitly shuts down the application host. If in headless mode, then this is required to be called.
        /// </summary>
        public void Shutdown()
        {
            ThrowIfDisposed();

            try
            {
                Implementation.Shutdown();
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Error while shutting down application", e);
            }
        }

        /// <summary>
        /// Creates a new UI window owned by the application host.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="presentParams">Presentation options for the window.</param>
        /// <param name="isTopLevelWindow">Notifies whether the window should be a top level container (e.g. a Form).</param>
        /// <returns>The newly created window.</returns>
        public IWindow CreateWindow(IRenderSystem renderSystem, PresentationParameters presentParams, bool isTopLevelWindow)
        {
            ThrowIfDisposed();

            try
            {
                return Implementation.CreateWindow(renderSystem, presentParams, isTopLevelWindow);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Failed to create window", e);
            }
        }

        /// <summary>
        /// Creates a UI window adapter to an existing native window.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="nativeWindow">Native window to adapt.</param>
        /// <returns>The UI window adapter</returns>
        public IWindow FromNativeWindow(IRenderSystem renderSystem, object nativeWindow)
        {
            return FromNativeWindow(renderSystem, nativeWindow, null, false);
        }

        /// <summary>
        /// Creates a UI window adapter to an existing native window.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="nativeWindow">Native window to adapt.</param>
        /// <param name="presentParams">Optional template presentation parameters. Some properties may be ignored.</param>
        /// <param name="resizeNativeWindow">True if the native window should be resized based on template presentation parameters, false if those
        /// properties should be ignored.</param>
        /// <returns>The UI window adapter</returns>
        public IWindow FromNativeWindow(IRenderSystem renderSystem, object nativeWindow, PresentationParameters? presentParams, bool resizeNativeWindow)
        {
            ThrowIfDisposed();

            if (nativeWindow == null)
            {
                throw new ArgumentNullException(nameof(nativeWindow));
            }

            try
            {
                return Implementation.FromNativeWindow(renderSystem, nativeWindow, presentParams, resizeNativeWindow);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Failed to create window", e);
            }
        }

        /// <summary>
        /// Invokes the action on the thread that the application host runs its message loop on (aka run on UI thread). This
        /// returns immediately with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public Task Invoke(Action action)
        {
            ThrowIfDisposed();

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                return Implementation.Invoke(action);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("An error occured while invoking an action", e);
            }
        }

        /// <summary>
        /// Invokes the fucnction on the thread that the application host runs its message loop on (aka run on UI thread). This
        /// returns immediately with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <typeparam name="TResult">Type of result to return</typeparam>
        /// <param name="action">Action that returns a result</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public Task<TResult> Invoke<TResult>(Func<TResult> action)
        {
            ThrowIfDisposed();

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                return Implementation.Invoke(action);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("An error occured while invoking an action", e);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            
            if (disposing)
            {
                OnDispose();
                Implementation?.Dispose();
            }
        }

        /// <summary>
        /// Called right before the host is disposed of.
        /// </summary>
        protected virtual void OnDispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Throws an exception if the graphics resource is disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        #endregion

        #region Implementation Creation

        /// <summary>
        /// Creates the application host implementation
        /// </summary>
        /// <param name="appSystem">Application system</param>
        private void CreateImplementation(IApplicationSystem appSystem)
        {
            if (appSystem == null)
            {
                throw new ArgumentNullException(nameof(appSystem));
            }

            try
            {
                Implementation = appSystem.CreateApplicationHostImplementation(this);
            }
            catch (Exception e)
            {
                throw new SparkApplicationException("Failed to create application host", e);
            }
        }

        #endregion
    }
}
