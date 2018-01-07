namespace Spark.Engine
{
    /// <summary>
    /// Defines a component that contains logic.
    /// </summary>
    public interface IBehavior : IComponent
    {
        /// <summary>
        /// Gets or sets the update priority. Smaller values represent a higher priority.
        /// </summary>
        int UpdatePriority { get; set; }

        /// <summary>
        /// Performs the logic update.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last update.</param>
        void Update(IGameTime gameTime);
    }
}
