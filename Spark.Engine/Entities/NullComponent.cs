namespace Spark.Entities
{
    using Content;

    /// <summary>
    /// Represents the null component.
    /// </summary>
    public sealed class NullComponent : IComponent
    {
        /// <summary>
        /// The null component value.
        /// </summary>
        public static readonly NullComponent Value = new NullComponent();
        
        /// <summary>
        /// Gets the unique type id that represents this component.
        /// </summary>
        public ComponentTypeId TypeId => ComponentTypeId.NullTypeId;

        /// <summary>
        /// Gets the entity that this component is attached to. Each component can only be attached to one parent at a time.
        /// </summary>
        public Entity Parent => null;

        /// <summary>
        /// Gets the world that the parent entity belongs to.
        /// </summary>
        public World World => null;

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name
        {
            get => "NullComponent";
            set { }
        }

        /// <summary>
        /// Called when the component is added to the entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public void OnAttach(Entity parent)
        {
            // No-op
        }

        /// <summary>
        /// Called when the component is removed from an entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public void OnRemove(Entity parent)
        {
            // No-op
        }

        /// <summary>
        /// Called when the entity has been added to the world.
        /// </summary>
        public void OnEntitySpawned()
        {
            // No-op
        }

        /// <summary>
        /// Called when the entity has been removed from the world.
        /// </summary>
        public void OnEntityDestroyed()
        {
            // No-op
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            // No-op
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            // No-op
        }
    }
}
