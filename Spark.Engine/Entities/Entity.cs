namespace Spark.Engine
{
    using System.Linq;
    using System.Collections.Generic;

    using Math;
    using Content;
    using Scene;
    
    /// <summary>
    /// Represents an entity within the world
    /// </summary>
    public class Entity : INamable, ITransformed, ISavable
    {
        private readonly Dictionary<ComponentTypeId, List<IComponent>> _components;
        private readonly List<IBehavior> _behaviors;
        private readonly BehaviorComparer _sorter;
        private bool _behaviorsNeedSorting;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
            : this("Entity")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="name">Name of the entity</param>
        public Entity(string name)
        {
            _components = new Dictionary<ComponentTypeId, List<IComponent>>();
            _behaviors = new List<IBehavior>();
            _sorter = new BehaviorComparer();
            _behaviorsNeedSorting = true;

            Scene = new Scene(name);
        }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        public string Name
        {
            get => Scene.Name;
            set => Scene.Name = value;
        }

        /// <summary>
        /// Gets the entities id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the world the entity resides within
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Gets the scene for the entity
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public Transform Transform
        {
            get => Scene.Transform;
            set => Scene.Transform = value;
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public Vector3 Scale
        {
            get => Scene.Scale;
            set => Scene.Scale = value;
        }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public Quaternion Rotation
        {
            get => Scene.Rotation;
            set => Scene.Rotation = value;
        }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public Vector3 Translation
        {
            get => Scene.Translation;
            set => Scene.Translation = value;
        }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public Transform WorldTransform => Scene.WorldTransform;

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public Vector3 WorldScale => Scene.WorldTransform.Scale;

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public Quaternion WorldRotation => Scene.WorldTransform.Rotation;

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public Vector3 WorldTranslation => Scene.WorldTransform.Translation;

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public Matrix4x4 WorldMatrix => Scene.WorldTransform.Matrix;

        /// <summary>
        /// Gets the world bounding volume of the entity
        /// </summary>
        public BoundingVolume WorldBounding => Scene.WorldBounding;

        /// <summary>
        /// Adds a component to the entity
        /// </summary>
        /// <param name="component">Component to remove</param>
        /// <returns>True if the component was added, false otherwise</returns>
        public bool AddComponent(IComponent component)
        {
            if (component == null)
            {
                return false;
            }

            if (component.TypeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (!_components.TryGetValue(component.TypeId, out List<IComponent> components))
            {
                components = new List<IComponent>();
                _components.Add(component.TypeId, components);
            }

            if (components.Contains(component))
            {
                return false;
            }

            components.Add(component);
            component.OnAttach(this);

            var behavior = component as IBehavior;
            if (behavior != null)
            {
                _behaviors.Add(behavior);
            }

            return true;
        }

        /// <summary>
        /// Removes a specific component from the entity
        /// </summary>
        /// <param name="component">Component to remove</param>
        /// <returns>True if the component was removed, false otherwise</returns>
        public bool RemoveComponent(IComponent component)
        {
            if (component == null)
            {
                return false;
            }

            if (component.TypeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(component.TypeId, out List<IComponent> components))
            {
                bool removed = components.Remove(component);
                if (removed)
                {
                    if (!components.Any())
                    {
                        _components.Remove(component.TypeId);
                    }
                    
                    var behavior = component as IBehavior;
                    if (behavior != null)
                    {
                        _behaviors.Remove(behavior);
                    }

                    component.OnRemove(this);
                }

                return removed;
            }

            return false;
        }

        /// <summary>
        /// Removes all components of a given type
        /// </summary>
        /// <typeparam name="T">Component types</typeparam>
        /// <returns>True if components were removed, false otherwise</returns>
        public bool RemoveAllComponents<T>() where T : class, IComponent
        {
            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();
            return RemoveAllComponents(typeId);
        }

        /// <summary>
        /// Removes all components of a given type
        /// </summary>
        /// <param name="typeId">Component types</param>
        /// <returns>True if components were removed, false otherwise</returns>
        public bool RemoveAllComponents(ComponentTypeId typeId)
        {
            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }
            
            return _components.Remove(typeId);
        }

        /// <summary>
        /// Gets a component of a given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Component of type</returns>
        public T GetComponent<T>() where T : class, IComponent
        {
            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();
            return GetComponent(typeId) as T;
        }

        /// <summary>
        /// Gets a component of a given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <returns>Component of type</returns>
        public IComponent GetComponent(ComponentTypeId typeId)
        {
            if (typeId == ComponentTypeId.NullTypeId)
            {
                return null;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> components))
            {
                return components.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Gets all components of a specific type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>Enumeration of all components</returns>
        public IEnumerable<T> GetComponents<T>() where T : class, IComponent
        {
            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();
            return GetComponents(typeId)?.OfType<T>();
        }

        /// <summary>
        /// Gets all components of a specific type
        /// </summary>
        /// <param name="typeId">Type of component</param>
        /// <returns>Enumeration of all components</returns>
        public IEnumerable<IComponent> GetComponents(ComponentTypeId typeId)
        {
            if (typeId == ComponentTypeId.NullTypeId)
            {
                return null;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> components))
            {
                return components.ToList();
            }

            return null;
        }

        /// <summary>
        /// Determines if the entity contains a component of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>True if a component of the given type is contained within the entity, false otherwise</returns>
        public bool ContainsComponents<T>() where T : class, IComponent
        {
            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();
            return ContainsComponents(typeId);
        }
        
        /// <summary>
        /// Determines if the entity contains a component of the given type
        /// </summary>
        /// <v name="typeId">Component type</v>
        /// <returns>True if a component of the given type is contained within the entity, false otherwise</returns>
        public bool ContainsComponents(ComponentTypeId typeId)
        {
            return _components.ContainsKey(typeId);
        }
        
        /// <summary>
        /// Determines if the given component is contained within the entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="component">Component to search for</param>
        /// <returns>True if the component is contained within the entity</returns>
        public bool ContainsComponent<T>(T component) where T : class, IComponent
        {
            return ContainsComponent((IComponent)component);
        }

        /// <summary>
        /// Determines if the given component is contained within the entity
        /// </summary>
        /// <param name="component">Component to search for</param>
        /// <returns>True if the component is contained within the entity</returns>
        public bool ContainsComponent(IComponent component)
        {
            if (component == null)
            {
                return false;
            }

            ComponentTypeId typeId = component.TypeId;

            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> components))
            {
                return components.Contains(component);
            }

            return false;
        }
        
        /// <summary>
        /// Attempts to get a component of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="component">Component value</param>
        /// <returns>True if a component was found, false otherwise</returns>
        public bool TryGetComponent<T>(out T component) where T : class, IComponent
        {
            component = null;

            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();

            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> components))
            {
                component = components.FirstOrDefault() as T;
                return component != null;
            }

            return false;
        }

        /// <summary>
        /// Attempts to get a component of the given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <param name="component">Component value</param>
        /// <returns>True if a component was found, false otherwise</returns>
        public bool TryGetComponent(ComponentTypeId typeId, out IComponent component)
        {
            component = null;
            
            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> components))
            {
                component = components.FirstOrDefault();
                return component != null;
            }

            return false;
        }

        /// <summary>
        /// Attempts to get all components of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="components">Enumeration of components</param>
        /// <returns>True if components were found, false otherwise</returns>
        public bool TryGetComponents<T>(out IEnumerable<T> components) where T : class, IComponent
        {
            components = null;

            ComponentTypeId typeId = ComponentTypeId.GetTypeId<T>();

            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> componentsList))
            {
                components = componentsList.OfType<T>().ToList();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to get all components of the given type
        /// </summary>
        /// <param name="typeId">Component type</param>
        /// <param name="components">Enumeration of components</param>
        /// <returns>True if components were found, false otherwise</returns>
        public bool TryGetComponents(ComponentTypeId typeId, out IEnumerable<IComponent> components)
        {
            components = null;

            if (typeId == ComponentTypeId.NullTypeId)
            {
                return false;
            }

            if (_components.TryGetValue(typeId, out List<IComponent> componentsList))
            {
                components = componentsList.ToList();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the entity has been added to the world.
        /// </summary>
        public virtual void OnSpawned()
        {
            foreach (KeyValuePair<ComponentTypeId, List<IComponent>> kv in _components)
            {
                foreach (IComponent component in kv.Value)
                {
                    component.OnEntitySpawned();
                }
            }
        }

        /// <summary>
        /// Called when the entity has been removed from the world.
        /// </summary>
        public virtual void OnDestroyed()
        {
            foreach (KeyValuePair<ComponentTypeId, List<IComponent>> kv in _components)
            {
                foreach (IComponent component in kv.Value)
                {
                    component.OnEntityDestroyed();
                }
            }
        }

        /// <summary>
        /// Updates all behaviors within the entity
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public void Update(IGameTime time)
        {
            for (int i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].Update(time);
            }
        }
                
        /// <summary>
        /// Sorts behaviors according to their <see cref="IBehavior.UpdatePriority"/>.
        /// </summary>
        public void SortBehaviors()
        {
            if (_behaviorsNeedSorting)
            {
                _behaviors.Sort(_sorter);
            }

            _behaviorsNeedSorting = false;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            Name = input.ReadString();
            int count = input.BeginReadGroup();

            for (int i = 0; i < count; i++)
            {
                IComponent component = input.ReadSavable<IComponent>();
                AddComponent(component);
            }

            input.EndReadGroup();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Name", Name);
            output.BeginWriteGroup("Components", _components.Count);

            foreach (KeyValuePair<ComponentTypeId, List<IComponent>> kv in _components)
            {
                foreach (IComponent component in kv.Value)
                {
                    output.WriteSavable(kv.Value.GetType().Name, component);
                }
            }

            output.EndWriteGroup();
        }

        /// <summary>
        /// Sets the world information
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="world">Parent world</param>
        internal void SetWorldInfo(int id, World world)
        {
            World?.Scene.Children.Remove(Scene);

            Id = id;
            World = world;
            //m_dispatcher.SetParent((world != null) ? world.EventDispatcher : null);

            SortBehaviors();

            bool isBorn = world != null;
            if (isBorn)
            {
                World.Scene.Children.Add(Scene);
                OnSpawned();
            }
            else
            {
                OnDestroyed();
                World.Scene.Children.Remove(Scene);
            }
        }
        
        /// <summary>
        /// Compares two behaviors
        /// </summary>
        private sealed class BehaviorComparer : IComparer<IBehavior>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
            public int Compare(IBehavior x, IBehavior y)
            {
                // Smaller numbers denote higher priority
                int xOrder = x.UpdatePriority;
                int yOrder = y.UpdatePriority;

                if (xOrder < yOrder)
                {
                    return -1;
                }
                else if (xOrder > yOrder)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
