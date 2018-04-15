namespace Spark.Engine
{
    using System.Collections.Generic;
    
    using Content;
    using Scene;

    /// <summary>
    /// Defines a entity that contains a collection of components. Each entity has a unique ID associated with its runtime type.
    /// </summary>
    public interface IEntity : INamable, ITransformed, ISavable
    {
        /// <summary>
        /// Gets the unique type id that represents this entity.
        /// </summary>
        EntityTypeId TypeId { get; }

        /// <summary>
        /// Gets the entities id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the world the entity resides within
        /// </summary>
        World World { get; }

        /// <summary>
        /// Gets the scene for the entity
        /// </summary>
        Scene Scene { get; }

        /// <summary>
        /// Adds a component to the entity
        /// </summary>
        /// <param name="component">Component to remove</param>
        /// <returns>True if the component was added, false otherwise</returns>
        bool AddComponent(IComponent component);

        /// <summary>
        /// Removes a specific component from the entity
        /// </summary>
        /// <param name="component">Component to remove</param>
        /// <returns>True if the component was removed, false otherwise</returns>
        bool RemoveComponent(IComponent component);

        /// <summary>
        /// Removes all components of a given type
        /// </summary>
        /// <typeparam name="T">Component types</typeparam>
        /// <returns>True if components were removed, false otherwise</returns>
        bool RemoveAllComponents<T>() where T : class, IComponent;

        /// <summary>
        /// Removes all components of a given type
        /// </summary>
        /// <param name="typeId">Component types</param>
        /// <returns>True if components were removed, false otherwise</returns>
        bool RemoveAllComponents(ComponentTypeId typeId);

        /// <summary>
        /// Gets a component of a given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Component of type</returns>
        T GetComponent<T>() where T : class, IComponent;

        /// <summary>
        /// Gets a component of a given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <returns>Component of type</returns>
        IComponent GetComponent(ComponentTypeId typeId);

        /// <summary>
        /// Gets all components of a specific type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>Enumeration of all components</returns>
        IEnumerable<T> GetComponents<T>() where T : class, IComponent;

        /// <summary>
        /// Gets all components of a specific type
        /// </summary>
        /// <param name="typeId">Type of component</param>
        /// <returns>Enumeration of all components</returns>
        IEnumerable<IComponent> GetComponents(ComponentTypeId typeId);

        /// <summary>
        /// Determines if the entity contains a component of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>True if a component of the given type is contained within the entity, false otherwise</returns>
        bool ContainsComponents<T>() where T : class, IComponent;

        /// <summary>
        /// Determines if the entity contains a component of the given type
        /// </summary>
        /// <v name="typeId">Component type</v>
        /// <returns>True if a component of the given type is contained within the entity, false otherwise</returns>
        bool ContainsComponents(ComponentTypeId typeId);

        /// <summary>
        /// Determines if the given component is contained within the entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="component">Component to search for</param>
        /// <returns>True if the component is contained within the entity</returns>
        bool ContainsComponent<T>(T component) where T : class, IComponent;

        /// <summary>
        /// Determines if the given component is contained within the entity
        /// </summary>
        /// <param name="component">Component to search for</param>
        /// <returns>True if the component is contained within the entity</returns>
        bool ContainsComponent(IComponent component);

        /// <summary>
        /// Attempts to get a component of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="component">Component value</param>
        /// <returns>True if a component was found, false otherwise</returns>
        bool TryGetComponent<T>(out T component) where T : class, IComponent;

        /// <summary>
        /// Attempts to get a component of the given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <param name="component">Component value</param>
        /// <returns>True if a component was found, false otherwise</returns>
        bool TryGetComponent(ComponentTypeId typeId, out IComponent component);

        /// <summary>
        /// Attempts to get all components of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="components">Enumeration of components</param>
        /// <returns>True if components were found, false otherwise</returns>
        bool TryGetComponents<T>(out IEnumerable<T> components) where T : class, IComponent;

        /// <summary>
        /// Attempts to get all components of the given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <param name="components">Enumeration of components</param>
        /// <returns>True if components were found, false otherwise</returns>
        bool TryGetComponents(ComponentTypeId typeId, out IEnumerable<IComponent> components);

        /// <summary>
        /// Called when the entity has been added to the world.
        /// </summary>
        void OnSpawned();

        /// <summary>
        /// Called when the entity has been removed from the world.
        /// </summary>
        void OnDestroyed();

        /// <summary>
        /// Updates all behaviors within the entity
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        void Update(IGameTime time);

        /// <summary>
        /// Sorts behaviors according to their <see cref="IBehavior.UpdatePriority"/>.
        /// </summary>
        void SortBehaviors();

        /// <summary>
        /// Sets the world information
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="world">Parent world</param>
        void SetWorldInfo(int id, World world);
    }
}
