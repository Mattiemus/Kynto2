namespace Spark.OpenGL.Application.Implementation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    using Spark.Application;
    using Spark.Application.Implementation;
    using Spark.Graphics;
    
    using Utilities;

    /// <summary>
    /// OpenGL implementation for <see cref="ApplicationHost"/>
    /// </summary>
    public sealed class OpenGLApplicationHostImplementation : BaseDisposable, IApplicationHostImplementation
    {
        private readonly OpenGLApplicationSystem _applicationSystem;
        private OpenGLWindow _mainWindow;
        private readonly List<IWindow> _windowList;
        private ConcurrentQueue<InvokeOperation> _invokeQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLApplicationHostImplementation"/> class.
        /// </summary>
        /// <param name="applicationSystem">Parent application system</param>
        /// <param name="implementor">Application host implementor</param>
        public OpenGLApplicationHostImplementation(OpenGLApplicationSystem applicationSystem, ApplicationHost implementor)
        {
            if (implementor == null)
            {
                throw new ArgumentNullException(nameof(implementor), "Application host cannot be null");
            }

            Name = string.Empty;
            _applicationSystem = applicationSystem;
            Parent = implementor;
            Thread = Thread.CurrentThread;
            _windowList = new List<IWindow>();
            Windows = new WindowCollection(_windowList);
            _invokeQueue = new ConcurrentQueue<InvokeOperation>();
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the application system that created the host.
        /// </summary>
        public IApplicationSystem ApplicationSystem => _applicationSystem;

        /// <summary>
        /// Gets the host that is bound to this implementation.
        /// </summary>
        public ApplicationHost Parent { get; }

        /// <summary>
        /// Gets the thread the host is running on.
        /// </summary>
        public Thread Thread { get; }

        /// <summary>
        /// Gets the main UI window the host owns.
        /// </summary>
        public IWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    if (_windowList.Count > 0)
                    {
                        _mainWindow = (OpenGLWindow)_windowList[0];
                    }

                    if (_mainWindow != null)
                    {
                        StartDisposeEvent();
                    }
                }

                return _mainWindow;
            }
        }

        /// <summary>
        /// Gets the collection of UI windows the host owns.
        /// </summary>
        public WindowCollection Windows { get; }

        /// <summary>
        /// Gets whether the host's message loop is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Runs the application host's message loop in headless mode. On idle (after message processing) the callback will be invoked. If
        /// a window is created after the application host is already running, then the first window created is assumed to be the main window and then
        /// the lifetime of the host is dictated by that window.
        /// </summary>
        /// <param name="runCallback">Loop callback</param>
        public void Run(Action runCallback)
        {
            Run(null, runCallback);
        }

        /// <summary>
        /// Runs the application host's message loop using the specified window. On idle (after message processing) the callback will be invoked.
        /// The lifetime of the application host is dictated by the specified window, which becomes the main window.
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="runCallback">Loop callback</param>
        public void Run(IWindow mainWindow, Action runCallback)
        {
            if (runCallback == null)
            {
                throw new ArgumentNullException(nameof(runCallback), "Application loop cannot be null");
            }

            _mainWindow = (OpenGLWindow)mainWindow;

            if (_mainWindow == null && _windowList.Count > 0)
            {
                _mainWindow = (OpenGLWindow)_windowList[0];
            }

            StartDisposeEvent();

            IsRunning = true;

            while (IsRunning)
            {
                _mainWindow.OpenGLNativeWindow.ProcessEvents();

                if (IsRunning)
                {
                    InvokeCallbackQueue();
                    runCallback();
                }
            }

            StopDisposeEvent();
        }

        /// <summary>
        /// Explicitly shuts down the application host. If in headless mode, then this is required to be called.
        /// </summary>
        public void Shutdown()
        {
            IsRunning = false;
            StopDisposeEvent();
            
            foreach (IWindow window in _windowList)
            {
                if (!window.IsDisposed)
                {
                    window.Close();
                    window.Dispose();
                }
            }

            _windowList.Clear();
        }

        /// <summary>
        /// Invokes the action on the thread that the application host runs its message loop on (aka run on UI thread). This
        /// returns immediately with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public Task Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            InvokeOperationAction operation = new InvokeOperationAction(action);

            if (Thread.CurrentThread == Thread)
            {
                operation.Invoke();
            }
            else
            {
                _invokeQueue.Enqueue(operation);
            }

            return operation.GetTask();
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
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            InvokeOperationFunc<TResult> operation = new InvokeOperationFunc<TResult>(action);

            if (Thread.CurrentThread == Thread)
            {
                operation.Invoke();
            }
            else
            {
                _invokeQueue.Enqueue(operation);
            }

            return operation.GetTask() as Task<TResult>;
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
            if (isTopLevelWindow)
            {
                OpenGLWindow window = new OpenGLWindow(renderSystem, presentParams);
                _windowList.Add(window);
                return window;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a UI window adapter to an existing native window.
        /// </summary>
        /// <param name="renderSystem">Render system to create the window for.</param>
        /// <param name="nativeWindow">Native window to adapt.</param>
        /// <param name="templatePresentationParameters">Optional template presentation parameters. Some properties may be ignored.</param>
        /// <param name="resizeNativeWindow">True if the native window should be resized based on template presentation parameters, false if those
        /// properties should be ignored.</param>
        /// <returns>The UI window adapter</returns>
        public IWindow FromNativeWindow(IRenderSystem renderSystem, object nativeWindow, PresentationParameters? templatePresentationParameters, bool resizeNativeWindow)
        {
            throw new NotImplementedException();
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
                for (int i = 0; i < _windowList.Count; i++)
                {
                    IWindow window = _windowList[i];
                    window.Dispose();
                }

                _windowList.Clear();
            }
                
            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Starts listening for the disposed event on the window window
        /// </summary>
        private void StartDisposeEvent()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Disposed += WindowDisposed;
            }
        }

        /// <summary>
        /// Stops listening for the disposed event on the window window
        /// </summary>
        private void StopDisposeEvent()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Disposed -= WindowDisposed;
            }
        }

        /// <summary>
        /// Invoked when a window is disposed
        /// </summary>
        /// <param name="window">Window that was disposed</param>
        /// <param name="e">Event arguments</param>
        private void WindowDisposed(IWindow window, EventArgs e)
        {
            IsRunning = false;
        }

        /// <summary>
        /// Invokes all the pending operations in the invoke queue
        /// </summary>
        private void InvokeCallbackQueue()
        {
            while (_invokeQueue.TryDequeue(out InvokeOperation operation))
            {
                operation.Invoke();
            }
        }

        #region InvokeOperation

        /// <summary>
        /// Base container for an invokable action
        /// </summary>
        private abstract class InvokeOperation
        {
            /// <summary>
            /// Gets the task for the action
            /// </summary>
            /// <returns>Task representing the action</returns>
            public abstract Task GetTask();

            /// <summary>
            /// Invokes the action
            /// </summary>
            public abstract void Invoke();
        }

        /// <summary>
        /// Invokable action sourced from a <see cref="Action"/>
        /// </summary>
        private sealed class InvokeOperationAction : InvokeOperation
        {
            private readonly Action _action;
            private readonly TaskCompletionSource<Object> _taskSource;

            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeOperationAction"/> class.
            /// </summary>
            /// <param name="action">Action to perform</param>
            public InvokeOperationAction(Action action)
            {
                _action = action;
                _taskSource = new TaskCompletionSource<object>();
            }

            /// <summary>
            /// Gets the task for the action
            /// </summary>
            /// <returns>Task representing the action</returns>
            public override Task GetTask()
            {
                return _taskSource.Task;
            }

            /// <summary>
            /// Invokes the action
            /// </summary>
            public override void Invoke()
            {
                try
                {
                    _action();
                    _taskSource.SetResult(null);
                }
                catch (Exception e)
                {
                    _taskSource.SetException(e);
                }
            }
        }

        /// <summary>
        /// Invokable action sourced from a <see cref="Func{TResult}"/>
        /// </summary>
        private sealed class InvokeOperationFunc<TResult> : InvokeOperation
        {
            private readonly Func<TResult> _action;
            private readonly TaskCompletionSource<TResult> _taskSource;

            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeOperationFunc{TResult}"/> class.
            /// </summary>
            /// <param name="func">Action to perform</param>
            public InvokeOperationFunc(Func<TResult> func)
            {
                _action = func;
                _taskSource = new TaskCompletionSource<TResult>();
            }

            /// <summary>
            /// Gets the task for the action
            /// </summary>
            /// <returns>Task representing the action</returns>
            public override Task GetTask()
            {
                return _taskSource.Task;
            }

            /// <summary>
            /// Invokes the action
            /// </summary>
            public override void Invoke()
            {
                try
                {
                    TResult result = _action();
                    _taskSource.SetResult(result);
                }
                catch (Exception e)
                {
                    _taskSource.SetException(e);
                }
            }
        }

        #endregion
    }
}
