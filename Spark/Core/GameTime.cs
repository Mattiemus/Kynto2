namespace Spark.Core
{
    using System;

    /// <summary>
    /// GameTime is a snapshot of time during the simulation. It is managed by the GameTimer class for
    /// its values to be set.
    /// </summary>
    public sealed class GameTime : IGameTime
    {
        private TimeSpan _elapsedGameTime;

        /// <summary>
        /// Constructs a new instance of the <see cref="GameTime"/> class.
        /// </summary>
        public GameTime() 
            : this(TimeSpan.Zero, TimeSpan.Zero, false)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="GameTime"/> class.
        /// </summary>
        /// <param name="elapsedGameTime">The elapsed game time.</param>
        /// <param name="totalGameTime">The total game time.</param>
        public GameTime(TimeSpan elapsedGameTime, TimeSpan totalGameTime) 
            : this(elapsedGameTime, totalGameTime, false)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="GameTime"/> class.
        /// </summary>
        /// <param name="elapsedGameTime">The elapsed game time.</param>
        /// <param name="totalGameTime">The total game time.</param>
        /// <param name="isRunningSlowly">If the update is running slowly, that is the actual elapsed time is more than the target elapsed time.</param>
        public GameTime(TimeSpan elapsedGameTime, TimeSpan totalGameTime, bool isRunningSlowly)
        {
            _elapsedGameTime = elapsedGameTime;
            TotalGameTime = totalGameTime;
            ElapsedTimeInSeconds = (float)elapsedGameTime.TotalSeconds;
            IsRunningSlowly = isRunningSlowly;
        }

        /// <summary>
        /// Gets a game time that has zero elapsed time, representing a "null" value.
        /// </summary>
        public static IGameTime ZeroTime => new GameTime(TimeSpan.Zero, TimeSpan.Zero);

        /// <summary>
        /// Gets or sets the elapsed time span since the last snapshot.
        /// </summary>
        public TimeSpan ElapsedGameTime
        {
            get
            {
                return _elapsedGameTime;
            }
            set
            {
                _elapsedGameTime = value;
                ElapsedTimeInSeconds = (float)value.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the total time the simulation has been running.
        /// </summary>
        public TimeSpan TotalGameTime { get; set; }

        /// <summary>
        /// Gets the elapsed time since the last snapshot in seconds, in single precision.
        /// </summary>
        public float ElapsedTimeInSeconds { get; private set; }

        /// <summary>
        /// Gets if the game update is running slowly, that is the actual
        /// elapsed time is greater than the target elapsed time.
        /// </summary>
        public bool IsRunningSlowly { get; private set; }

        /// <summary>
        /// Populates the game time with the specified values.
        /// </summary>
        /// <param name="elapsedGameTime">The elapsed game time.</param>
        /// <param name="totalGameTime">The total game time.</param>
        /// <param name="isRunningSlowly">If the update is running slowly, that is the actual elapsed time is more than the target elapsed time.</param>
        public void Set(TimeSpan elapsedGameTime, TimeSpan totalGameTime, bool isRunningSlowly)
        {
            _elapsedGameTime = elapsedGameTime;
            TotalGameTime = totalGameTime;
            ElapsedTimeInSeconds = (float)elapsedGameTime.TotalSeconds;
            IsRunningSlowly = isRunningSlowly;
        }

        /// <summary>
        /// Populates the game time from another instance.
        /// </summary>
        /// <param name="time">Other game time instance.</param>
        public void Set(IGameTime time)
        {
            if (time == null)
            {
                return;
            }

            _elapsedGameTime = time.ElapsedGameTime;
            TotalGameTime = time.TotalGameTime;
            ElapsedTimeInSeconds = (float)_elapsedGameTime.TotalSeconds;
            IsRunningSlowly = time.IsRunningSlowly;
        }
    }
}
