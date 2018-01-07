namespace Spark.Engine
{
    using System;

    /// <summary>
    /// Event arguments for an event involving an entity
    /// </summary>
    public class EntityEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEventArgs"/> class.
        /// /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="world">World the entity resides within</param>
        public EntityEventArgs(Entity entity, World world)
        {
            Entity = entity;
            World = world;
        }

        /// <summary>
        /// Gets the entity
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        /// Gets the world the entity resides within
        /// </summary>
        public World World { get; }
    }
}
