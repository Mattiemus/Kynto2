namespace Spark.Entities
{
    using Content;

    /// <summary>
    /// Defines a component that can be added to an entity. A component is either data ("Attributes") or logic ("Behaviors").
    /// Each component has a unique ID associated with its runtime type and each entity can have only one type of component.
    /// </summary>
    public interface IComponent : INamable, ISavable
    {
        /// <summary>
        /// Gets the unique type id that represents this component.
        /// </summary>
        ComponentTypeId TypeId { get; }
        
        /// <summary>
        /// Gets the entity that this component is attached to. Each component can only be attached to one parent at a time.
        /// </summary>
        Entity Parent { get; }

        /// <summary>
        /// Gets the world that the parent entity belongs to.
        /// </summary>
        World World { get; }

        /// <summary>
        /// Called when the component is added to the entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        void OnAttach(Entity parent);

        /// <summary>
        /// Called when the component is removed from an entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        void OnRemove(Entity parent);

        /// <summary>
        /// Called when the entity has been added to the world.
        /// </summary>
        void OnEntitySpawned();

        /// <summary>
        /// Called when the entity has been removed from the world.
        /// </summary>
        void OnEntityDestroyed();
    }
}
