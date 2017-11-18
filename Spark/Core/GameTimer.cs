namespace Spark
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A timer used for interpolation during simulation (the "game"). When the timer is updated, it returns a time snapshot - the elapsed
    /// time from the last update call, and the total time the simulation has been running. This does not necessarily reflect real time (wall clock), since
    /// the timer can be paused. 
    /// </summary>
    public sealed class GameTimer : IGameTimer
    {
        private readonly GameTime _gameTime;
        private long _prevTime;
        private bool _wasPaused;
        private long _pauseStart;
        private long _timePaused;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GameTimer"/> class.
        /// </summary>
        public GameTimer()
        {
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            Reset();
        }

        /// <summary>
        /// Gets the game time snapshot.
        /// </summary>
        public IGameTime GameTime => _gameTime;

        /// <summary>
        /// Gets the resolution of the timer (inverse of frequency).
        /// </summary>
        public double Resolution => 1.0 / Frequency;

        /// <summary>
        /// Gets the frequency of the timer as ticks per second.
        /// </summary>
        public long Frequency => Stopwatch.Frequency;

        /// <summary>
        /// Gets if the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                Reset();
                _prevTime = Stopwatch.GetTimestamp();
                IsRunning = true;
            }
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _wasPaused = false;

                _pauseStart = Stopwatch.GetTimestamp();
            }
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _wasPaused = true;

                _timePaused += Stopwatch.GetTimestamp() - _pauseStart;
                _pauseStart = 0L;
            }
        }

        /// <summary>
        /// Resets the timer - the timer will be invalid until it is started again.
        /// </summary>
        public void Reset()
        {
            IsRunning = _wasPaused = false;
            _prevTime = _pauseStart = _timePaused = 0L;
        }

        /// <summary>
        /// Advance and update the timer.
        /// </summary>
        /// <returns>Updated game time snapshot.</returns>
        public IGameTime Update()
        {
            long currTime = Stopwatch.GetTimestamp();

            if (!IsRunning)
            {
                _gameTime.ElapsedGameTime = TimeSpan.Zero;
                return _gameTime;
            }
            else if (_wasPaused)
            {
                TimeSpan elapsedTime = CreateTimeSpan(currTime - (_prevTime + _timePaused));
                _timePaused = 0L;
                _gameTime.ElapsedGameTime = elapsedTime;
                _gameTime.TotalGameTime += elapsedTime;
                _prevTime = currTime;
                _wasPaused = false;
                return _gameTime;
            }
            else
            {
                TimeSpan elapsedTime = CreateTimeSpan(currTime - _prevTime);
                _gameTime.ElapsedGameTime = elapsedTime;
                _gameTime.TotalGameTime += elapsedTime;

                _prevTime = currTime;
                return _gameTime;
            }
        }

        /// <summary>
        /// Creates a timestamp from the given tick count
        /// </summary>
        /// <param name="ticks">Number of clock ticks</param>
        /// <returns>Time span representing the time taken to perform the given number of clock ticks</returns>
        private TimeSpan CreateTimeSpan(long ticks)
        {
            long time = (ticks * 10000000L) / Frequency;
            return TimeSpan.FromTicks(time);
        }
    }
}
