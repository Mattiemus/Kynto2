namespace Spark
{
    using System;

    /// <summary>
    /// Interface for a snapshot of time during simulation. It is managed by a game timer.
    /// </summary>
    public interface IGameTime
    {
        /// <summary>
        /// Gets the elapsed time span since the last snapshot.
        /// </summary>
        TimeSpan ElapsedGameTime { get; }

        /// <summary>
        /// Gets the total time the simulation has been running.
        /// </summary>
        TimeSpan TotalGameTime { get; }

        /// <summary>
        /// Gets the elapsed time since the last snapshot in seconds.
        /// </summary>
        float ElapsedTimeInSeconds { get; }

        /// <summary>
        /// Gets if the game update is running slowly, that is the actual
        /// elapsed time is greater than the target elapsed time.
        /// </summary>
        bool IsRunningSlowly { get; }
    }
}
