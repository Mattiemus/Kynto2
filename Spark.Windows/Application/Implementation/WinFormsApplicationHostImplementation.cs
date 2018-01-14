namespace Spark.Windows.Application.Implementation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    
    using Spark.Application;
    using Spark.Application.Implementation;
    using Spark.Graphics;

    public sealed class WinFormsApplicationHostImplementation : IApplicationHostImplementation
    {
        private readonly IApplicationSystem _appSystem;
        private ApplicationHost _parent;
        private bool _isDisposed;
        private string _name;
        private readonly Thread _thread;
        private IWindow _mainWindow;
        private readonly List<IWindow> _windowCollection;
        private readonly WindowCollection _readOnlyWindowCollection;
        private readonly ConcurrentQueue<InvokeOperation> _invokeQueue;
        private bool _isRunning;
        private bool _disposingWindows;

        internal WinFormsApplicationHostImplementation(IApplicationSystem appSystem, ApplicationHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host), "Cannot bind null implementation");
            }

            _appSystem = appSystem;
            _parent = host;
            _windowCollection = new List<IWindow>();
            _readOnlyWindowCollection = new WindowCollection(_windowCollection);
            _isDisposed = false;
            _isRunning = false;
            _thread = Thread.CurrentThread;
            _name = string.Empty;
            _mainWindow = null;
            _invokeQueue = new ConcurrentQueue<InvokeOperation>();
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool IsDisposed => _isDisposed;

        public IApplicationSystem ApplicationSystem => _appSystem;

        public ApplicationHost Parent => _parent;

        public Thread Thread => _thread;

        public IWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    if (_windowCollection.Count > 0)
                    {
                        _mainWindow = _windowCollection[0];
                    }

                    if (_mainWindow != null)
                    {
                        StartDisposeEvent();
                    }
                }

                return _mainWindow;
            }
        }

        public WindowCollection Windows => _readOnlyWindowCollection;

        public bool IsRunning => _isRunning;
        
        public void Run(Action runCallback)
        {
            Run(null, runCallback);
        }

        public void Run(IWindow mainWindow, Action runCallback)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("Application host implementation not bound");
            }

            if (runCallback == null)
            {
                throw new ArgumentNullException(nameof(runCallback), "Application loop must not be null");
            }

            _mainWindow = mainWindow;

            if (_mainWindow == null && _windowCollection.Count > 0)
            {
                _mainWindow = _windowCollection[0];
            }

            StartDisposeEvent();

            _isRunning = true;

            while (_isRunning)
            {
                Spark.Windows.Message msg;
                while (NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0) != 0)
                {
                    if (NativeMethods.GetMessage(out msg, IntPtr.Zero, 0, 0) == -1)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Error occured while processing window messages: {0}", Marshal.GetLastWin32Error()));
                    }

                    //NCDESTROY event
                    if (msg.msg == 130)
                    {
                        _isRunning = false;
                    }

                    System.Windows.Forms.Message swfMsg = new System.Windows.Forms.Message()
                    {
                        HWnd = msg.hWnd,
                        LParam = msg.lParam,
                        Msg = (int)msg.msg,
                        WParam = msg.wParam
                    };

                    if (!System.Windows.Forms.Application.FilterMessage(ref swfMsg))
                    {
                        NativeMethods.TranslateMessage(ref msg);
                        NativeMethods.DispatchMessage(ref msg);
                    }
                }

                if (_isRunning)
                {
                    InvokeCallbackQueue();
                    runCallback();
                }
            }

            StopDisposeEvent();
        }

        public void Shutdown()
        {
            _isRunning = false;
            StopDisposeEvent();

            _disposingWindows = true;
            foreach (IWindow window in _windowCollection)
            {
                if (!window.IsDisposed)
                {
                    window.Close();
                    window.Dispose();
                }
            }
            _disposingWindows = false;

            _windowCollection.Clear();
        }

        public Task Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var operation = new InvokeOperationAction(action);

            if (Thread.CurrentThread == _thread)
            {
                operation.Invoke();
            }
            else
            {
                _invokeQueue.Enqueue(operation);
            }

            return operation.GetTask();
        }

        public Task<TResult> Invoke<TResult>(Func<TResult> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var operation = new InvokeOperationFunc<TResult>(action);

            if (Thread.CurrentThread == _thread)
            {
                operation.Invoke();
            }
            else
            {
                _invokeQueue.Enqueue(operation);
            }

            return operation.GetTask() as Task<TResult>;
        }

        public IWindow CreateWindow(IRenderSystem renderSystem, PresentationParameters presentParams, bool isTopLevelWindow)
        {
            var swfWindow = new WinFormsWindow(renderSystem, _parent, presentParams, isTopLevelWindow);
            _windowCollection.Add(swfWindow);
            return swfWindow;
        }

        public IWindow FromNativeWindow(IRenderSystem renderSystem, object nativeWindow, PresentationParameters? presentParams, bool resizeNativeWindow)
        {
            WinFormsWindow swfWindow = null;

            if (nativeWindow is Control)
            {
                swfWindow = new WinFormsWindow(renderSystem, _parent, nativeWindow as Control, presentParams, resizeNativeWindow);
            }
            else if (nativeWindow is IWinFormsControlAdapter)
            {
                swfWindow = new WinFormsWindow(renderSystem, _parent, nativeWindow as IWinFormsControlAdapter, presentParams, resizeNativeWindow);
            }

            if (swfWindow != null)
            {
                _windowCollection.Add(swfWindow);
            }

            return swfWindow;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void RemoveWindowOnDispose(IWindow window)
        {
            if (_disposingWindows)
            {
                return;
            }

            _windowCollection.Remove(window);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _disposingWindows = true;
                    for (int i = 0; i < _windowCollection.Count; i++)
                    {
                        IWindow window = _windowCollection[i];
                        window.Dispose();
                    }
                    _disposingWindows = false;

                    _windowCollection.Clear();
                }

                _isDisposed = true;
            }
        }

        private void StartDisposeEvent()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Disposed += WindowDisposed;
            }
        }

        private void StopDisposeEvent()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Disposed -= WindowDisposed;
            }
        }

        private void WindowDisposed(IWindow window, EventArgs e)
        {
            _isRunning = false;
        }

        private void InvokeCallbackQueue()
        {
            while (_invokeQueue.TryDequeue(out InvokeOperation operation))
            {
                operation.Invoke();
            }
        }

        #region InvokeOperation

        private abstract class InvokeOperation
        {
            public abstract Task GetTask();
            public abstract void Invoke();
        }

        private sealed class InvokeOperationAction : InvokeOperation
        {
            private readonly Action _action;
            private readonly TaskCompletionSource<object> _taskSource;

            public InvokeOperationAction(Action action)
            {
                _action = action;
                _taskSource = new TaskCompletionSource<object>();
            }

            public override Task GetTask()
            {
                return _taskSource.Task;
            }

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

        private sealed class InvokeOperationFunc<TResult> : InvokeOperation
        {
            private readonly Func<TResult> _action;
            private readonly TaskCompletionSource<TResult> _taskSource;

            public InvokeOperationFunc(Func<TResult> func)
            {
                _action = func;
                _taskSource = new TaskCompletionSource<TResult>();
            }

            public override Task GetTask()
            {
                return _taskSource.Task;
            }

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
