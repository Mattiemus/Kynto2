namespace Spark.Engine
{
    using Content;

    /// <summary>
    /// Base class for components
    /// </summary>
    public class Component : IComponent
    {
        private string _name;

        /// <summary>
        /// Initializes the <see cref="Component"/> class.
        /// </summary>
        public Component()
            : this("Component")
        {
        }

        /// <summary>
        /// Initializes the <see cref="Component"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public Component(string name)
        {
            TypeId = ComponentTypeId.GetTypeId(GetType());
            _name = name;
        }

        /// <summary>
        /// Gets the unique type id that represents this component.
        /// </summary>
        public ComponentTypeId TypeId { get; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnNameChanged();
            }
        }

        /// <summary>
        /// Gets the entity that this component is attached to. Each component can only be attached to one parent at a time.
        /// </summary>
        public Entity Parent { get; private set; }

        /// <summary>
        /// Gets the world that the parent entity belongs to.
        /// </summary>
        public World World => Parent?.World;

        /// <summary>
        /// Called when the component is added to the entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public virtual void OnAttach(Entity parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Called when the component is removed from an entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public virtual void OnRemove(Entity parent)
        {
            Parent = null;
        }

        /// <summary>
        /// Called when the entity has been added to the world.
        /// </summary>
        public virtual void OnEntitySpawned()
        {
            // No-op
        }

        /// <summary>
        /// Called when the entity has been removed from the world.
        /// </summary>
        public virtual void OnEntityDestroyed()
        {
            // No-op
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public virtual void Read(ISavableReader input)
        {
            // No-op
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public virtual void Write(ISavableWriter output)
        {
            // No-op
        }

        /// <summary>
        /// Invoked when the components name is changed
        /// </summary>
        protected virtual void OnNameChanged()
        {
            // No-op
        }
    }
}
