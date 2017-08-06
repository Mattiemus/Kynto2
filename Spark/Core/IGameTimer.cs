namespace Spark.Core
{
    /// <summary>
    /// Interface for a timer used for interpolation during simulation (the "game"). When the timer is updated, it returns a time snapshot - the elapsed
    /// time from the last update call, and the total time the simulation has been running. This does not necessarily reflect real time (wall clock), since
    /// the timer can be paused. 
    /// </summary>
    public interface IGameTimer
    {
        /// <summary>
        /// Gets the game time snapshot.
        /// </summary>
        IGameTime GameTime { get; }

        /// <summary>
        /// Gets the resolution of the timer (inverse of frequency).
        /// </summary>
        double Resolution { get; }

        /// <summary>
        /// Gets the frequency of the timer as ticks per second.
        /// </summary>
        long Frequency { get; }

        /// <summary>
        /// Gets if the timer is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        void Resume();

        /// <summary>
        /// Resets the timer - the timer will be invalid until it is started again.
        /// </summary>
        void Reset();

        /// <summary>
        /// Advance and update the timer.
        /// </summary>
        /// <returns>Updated game time snapshot.</returns>
        IGameTime Update();
    }
}
