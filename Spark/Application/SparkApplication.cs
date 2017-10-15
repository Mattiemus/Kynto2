namespace Spark.Application
{
    using System;
    using System.Threading;

    using Core;
    using Math;
    using Content;
    using Graphics;
    using Graphics.Materials;
    using Input;

    public abstract class SparkApplication
    {
        private readonly PresentationParameters _initPP;
        private readonly IResourceRepository _initRepo;
        private readonly IPlatformInitializer _initPlatform;

        private GameTimer _timer;
        private SwapChain _swapChain;
        private IRenderContext _renderContext;

        private GameTime _gameTime;
        private TimeSpan _totalGameTime;
        private TimeSpan _targetElapsedTime;
        private TimeSpan _inactiveSleepTime;
        private TimeSpan _maximumElapsedTime;
        private TimeSpan _lastFrameElapsedTime;
        private TimeSpan _accumulatedElapsedGameTime;
        private int[] _lastUpdateCount;
        private int _nextLastUpdateCountIndex;
        private float _updateCountAvgSlowLimit;
        private bool _forceElapsedTimeToZero;
        private bool _isExiting;
        private bool _isDrawRunningSlowly;
        
        protected SparkApplication(IPlatformInitializer platformInitializer)
        {
            _initPP = new PresentationParameters()
            {
                BackBufferFormat = SurfaceFormat.Color,
                BackBufferWidth = 1024,
                BackBufferHeight = 768,
                DepthStencilFormat = DepthFormat.Depth24Stencil8,
                DisplayOrientation = DisplayOrientation.Default,
                IsFullScreen = false,
                MultiSampleCount = 0,
                MultiSampleQuality = 0,
                PresentInterval = PresentInterval.Immediate,
                RenderTargetUsage = RenderTargetUsage.DiscardContents
            }; 
            _initRepo = null;
            _initPlatform = platformInitializer;
        }

        protected SparkApplication(PresentationParameters presentParams, IPlatformInitializer platformInitializer)
        {
            _initPP = presentParams;
            _initRepo = null;
            _initPlatform = platformInitializer;
        }

        protected SparkApplication(PresentationParameters presentParams, IResourceRepository resourceRepo, IPlatformInitializer platformInitializer)
        {
            _initPP = presentParams;
            _initRepo = resourceRepo;
            _initPlatform = platformInitializer;
        }

        public Engine Engine { get; private set; }

        public EngineServiceRegistry Services { get; private set; }

        public IWindow GameWindow { get; private set; }

        public ApplicationHost GameHost { get; private set; }

        public ContentManager Content { get; private set; }

        public IRenderSystem RenderSystem { get; private set; }

        public Color ClearColor { get; set; }

        public TimeSpan TargetElapsedTime
        {
            get => _targetElapsedTime;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Time span cannot be zero");
                }

                _targetElapsedTime = value;
            }
        }

        public TimeSpan InactiveSleepTime
        {
            get => _inactiveSleepTime;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Time span cannot be zero");
                }

                _inactiveSleepTime = value;
            }
        }

        public bool IsFixedTimeStep { get; set; }

        public bool IsActive { get; private set; }

        public bool IsHeadless { get; private set; }

        public bool IsRunning { get; private set; }

        public bool SuppressDrawOneFrame { get; set; }

        public void Run(ApplicationMode mode = ApplicationMode.Normal)
        {
            Initialize(_initPP, _initRepo, _initPlatform, mode);

            _timer.Start();

            if (mode == ApplicationMode.Headless)
            {
                GameHost.Run(GameLoopTick);
            }
            else
            {
                GameHost.Run(GameWindow, GameLoopTick);
            }
        }

        public void Exit()
        {
            _isExiting = true;

            DoExitingCleanup();

            GameHost.Shutdown();
        }

        public void ResetElapsedTime()
        {
            _forceElapsedTimeToZero = true;
            _isDrawRunningSlowly = false;

            Array.Clear(_lastUpdateCount, 0, _lastUpdateCount.Length);
            _nextLastUpdateCountIndex = 0;
        }

        protected virtual void OnInitialize(Engine engine)
        {
        }

        protected virtual void OnExiting()
        {
        }

        protected virtual void ConfigureContentManager(ContentManager content)
        {
        }

        protected virtual void LoadContent(ContentManager content)
        {
        }

        protected virtual void UnloadContent(ContentManager content)
        {
        }

        protected virtual void OnViewportResized(IWindow gameWindow)
        {
        }

        protected abstract void Update(IGameTime time);

        protected abstract void Render(IRenderContext context, IGameTime time);

        private void Initialize(PresentationParameters presentParams, IResourceRepository resourceRepo, IPlatformInitializer platformInitializer, ApplicationMode mode)
        {
            Engine = Engine.Initialize(platformInitializer);
            Services = Engine.Services;

            RenderSystem = Services.GetService<IRenderSystem>();
            if (RenderSystem != null)
            {
                _renderContext = RenderSystem.ImmediateContext;
            }

            GameHost = new ApplicationHost(Services.GetService<IApplicationSystem>());
            IsHeadless = (mode == ApplicationMode.Headless) || RenderSystem == null;

            if (!IsHeadless)
            {
                GameWindow = GameHost.CreateWindow(RenderSystem, presentParams, mode == ApplicationMode.Normal); 
                GameWindow.EnableInputEvents = false;
                GameWindow.EnablePaintEvents = false;
                GameWindow.EnableUserResizing = false;
                GameWindow.CenterToScreen();
                GameWindow.Closed += Closed;
                GameWindow.ResumeRendering += ResumeRendering;
                GameWindow.SuspendRendering += SuspendRendering;
                GameWindow.ClientSizeChanged += ClientSizeChanged;
                _swapChain = GameWindow.SwapChain;
            }

            ClearColor = Color.CornflowerBlue;

            InitializeClocks();

            // If we have a mouse input system, ensure that the mouse is set to this window handle
            if (Services.GetService<IMouseInputSystem>() != null && GameWindow != null)
            {
                Mouse.WindowHandle = GameWindow.Handle;
                Rectangle bounds = GameWindow.ClientBounds;
                Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);
            }

            if (resourceRepo == null)
            {
                Content = new ContentManager(Services);
            }
            else
            {
                Content = new ContentManager(Services, resourceRepo);
            }

            ConfigureContentManager(Content);
            OnInitialize(Engine);

            LoadContent(Content);

            IsRunning = true;
        }

        private void InitializeClocks()
        {
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            _timer = new GameTimer();
            _maximumElapsedTime = TimeSpan.FromMilliseconds(500.0f);
            IsFixedTimeStep = true;
            _isDrawRunningSlowly = false;
            SuppressDrawOneFrame = false;
            IsActive = true;
            _isExiting = false;
            _inactiveSleepTime = TimeSpan.FromMilliseconds(20.0f);
            _targetElapsedTime = TimeSpan.FromTicks(166667L);
            _totalGameTime = TimeSpan.Zero;
            _lastFrameElapsedTime = TimeSpan.Zero;
            _accumulatedElapsedGameTime = TimeSpan.Zero;

            _lastUpdateCount = new int[4];
            _nextLastUpdateCountIndex = 0;

            // Calculate the update count avg slow limit (assuming moving average is >=3)
            // E.g., moving average of 4:
            // Avg slow limit = (2 * 2 + (4 - 2)) / 4 = 1.5f
            const int badUpdateCountTime = 2; // Number of bad frames, which is a frame that has at  least two updates
            int maxLastCount = 2 * Math.Min(badUpdateCountTime, _lastUpdateCount.Length);
            _updateCountAvgSlowLimit = ((maxLastCount + (_lastUpdateCount.Length - maxLastCount)) / _lastUpdateCount.Length);
        }

        private void GameLoopTick()
        {
            if (_isExiting)
            {
                return;
            }

            if (!IsActive)
            {
                Thread.Sleep(_inactiveSleepTime);
            }

            _timer.Update();

            TimeSpan elapsedTime = _timer.GameTime.ElapsedGameTime;

            if (_forceElapsedTimeToZero)
            {
                elapsedTime = TimeSpan.Zero;
                _forceElapsedTimeToZero = false;
            }

            if (elapsedTime > _maximumElapsedTime)
            {
                elapsedTime = _maximumElapsedTime;
            }

            bool suppressNextDraw = true;
            int updateCount = 1;
            TimeSpan singleFrameElapsedTime = elapsedTime;

            if (IsFixedTimeStep)
            {
                if (Math.Abs(elapsedTime.Ticks - _targetElapsedTime.Ticks) < (_targetElapsedTime.Ticks >> 6))
                {
                    elapsedTime = _targetElapsedTime;
                }

                _accumulatedElapsedGameTime += elapsedTime;

                // Calculate number of updates to do
                updateCount = (int)(_accumulatedElapsedGameTime.Ticks / _targetElapsedTime.Ticks);

                // If there is no need for update, then exit
                if (updateCount == 0)
                {
                    // Check if we can sleep the thread to free CPU resources
                    TimeSpan sleepTime = _targetElapsedTime - _accumulatedElapsedGameTime;
                    if (sleepTime > TimeSpan.Zero)
                    {
                        Thread.Sleep(sleepTime);
                    }

                    return;
                }

                // Calculate a moving average on updatecount
                _lastUpdateCount[_nextLastUpdateCountIndex] = updateCount;
                float updateCountMean = 0;
                for (int i = 0; i < _lastUpdateCount.Length; i++)
                {
                    updateCountMean += _lastUpdateCount[i];
                }

                updateCountMean /= _lastUpdateCount.Length;
                _nextLastUpdateCountIndex = (_nextLastUpdateCountIndex + 1) % _lastUpdateCount.Length;

                // Test when we are running slowly
                _isDrawRunningSlowly = updateCountMean > _updateCountAvgSlowLimit;

                // We are going to call Update "updateCount" times so we can subtract this from accumulated elapsed game time
                _accumulatedElapsedGameTime = new TimeSpan(_accumulatedElapsedGameTime.Ticks - (updateCount * _targetElapsedTime.Ticks));
                singleFrameElapsedTime = _targetElapsedTime;
            }
            else
            {
                Array.Clear(_lastUpdateCount, 0, _lastUpdateCount.Length);
                _nextLastUpdateCountIndex = 0;
                _isDrawRunningSlowly = false;
            }

            // Reset the time of the next frame
            for (_lastFrameElapsedTime = TimeSpan.Zero; updateCount > 0 && !_isExiting; updateCount--)
            {
                _gameTime.Set(singleFrameElapsedTime, _totalGameTime, _isDrawRunningSlowly);

                try
                {
                    DoUpdate(_gameTime);

                    // If no exception, then we can draw the frame
                    suppressNextDraw &= SuppressDrawOneFrame;
                    SuppressDrawOneFrame = false;
                }
                finally
                {
                    _lastFrameElapsedTime += singleFrameElapsedTime;
                    _totalGameTime += singleFrameElapsedTime;
                }
            }

            bool isMinimized = (GameWindow != null) ? GameWindow.IsMinimized : false;

            if (!suppressNextDraw && !_isExiting && !isMinimized)
            {
                DoRender(_gameTime);
            }
        }

        private void DoUpdate(IGameTime gameTime)
        {
            // Setup time for computed parameters
            MaterialBinding.GameTime = gameTime;

            Engine.UpdateServices();
            Update(gameTime);
        }

        private void DoRender(IGameTime gameTime)
        {
            // Setup time for computed parameters
            MaterialBinding.GameTime = gameTime;

            // Do normal single window rendering if not in headless mode
            if (!IsHeadless)
            {
                _swapChain.SetActiveAndClear(_renderContext, ClearOptions.All, ClearColor, 1.0f, 0);

                Render(_renderContext, gameTime);

                _swapChain.Present();
            }
            // If in headless, and rendersystem is not null, then just call the render method for whatever the client wants to do
            else if (RenderSystem != null)
            {
                Render(_renderContext, gameTime);
            }
        }

        private void SuspendRendering(IWindow sender, EventArgs args)
        {
            _timer.Pause();
            IsActive = false;
        }

        private void ResumeRendering(IWindow sender, EventArgs args)
        {
            _timer.Resume();
            IsActive = true;
        }

        private void Closed(IWindow sender, EventArgs args)
        {
            // If already exiting, and responding to a close event...don't do anything!
            if (_isExiting)
            {
                return;
            }

            _isExiting = true;

            DoExitingCleanup();

            if (GameHost.IsRunning)
            {
                GameHost.Shutdown();
            }
        }

        private void DoExitingCleanup()
        {
            OnExiting();

            Content.Unload();
            UnloadContent(Content);

            Engine.Destroy();
        }

        private void ClientSizeChanged(IWindow sender, EventArgs args)
        {
            if (!IsRunning)
            {
                return;
            }

            OnViewportResized(GameWindow);
        }
    }
}
