namespace Spark.Graphics.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public sealed class ImplementationFactoryCollection : IEnumerable<IGraphicsResourceImplementationFactory>
    {
        private readonly Dictionary<Type, IGraphicsResourceImplementationFactory> _graphicResourceTypeToFactory;
        private readonly Dictionary<Type, IGraphicsResourceImplementationFactory> _factoryTypeToFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationFactoryCollection"/> class.
        /// </summary>
        public ImplementationFactoryCollection()
        {
            _graphicResourceTypeToFactory = new Dictionary<Type, IGraphicsResourceImplementationFactory>();
            _factoryTypeToFactory = new Dictionary<Type, IGraphicsResourceImplementationFactory>();
        }

        /// <summary>
        /// Adds an implementation factory of the specified type
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implFactory">Implementation factory to add</param>
        /// <returns>True if the factory was added, false if it was not</returns>
        public bool AddImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            if (implFactory == null || _graphicResourceTypeToFactory.ContainsKey(implFactory.GraphicsResourceType))
            {
                return false;
            }

            _graphicResourceTypeToFactory.Add(implFactory.GraphicsResourceType, implFactory);
            _factoryTypeToFactory.Add(typeof(T), implFactory);

            return true;
        }

        /// <summary>
        /// Removes an implementation factory of the specified type
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implFactory">Implementation factory to remove</param>
        /// <returns>True if the factory was removed, false if it was not</returns>
        public bool RemoveImplementationFactory<T>(T implFactory) where T : IGraphicsResourceImplementationFactory
        {
            if (implFactory == null || !_graphicResourceTypeToFactory.ContainsKey(implFactory.GraphicsResourceType))
            {
                return false;
            }

            return _graphicResourceTypeToFactory.Remove(implFactory.GraphicsResourceType) && _factoryTypeToFactory.Remove(typeof(T));
        }

        /// <summary>
        /// Gets the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <returns>The registered implementation factory, if it exists. Otherwise, null is returned.</returns>
        public T GetImplementationFactory<T>() where T : IGraphicsResourceImplementationFactory
        {
            IGraphicsResourceImplementationFactory factory;
            if (_factoryTypeToFactory.TryGetValue(typeof(T), out factory) && factory is T)
            {
                return (T)factory;
            }

            return default(T);
        }

        /// <summary>
        /// Tries to get the implementation factory of the specified type.
        /// </summary>
        /// <typeparam name="T">Implementation factory type</typeparam>
        /// <param name="implementationFactory">The registered implementation factory, if it exists.</param>
        /// <returns>True if the factory was registered and found, false otherwise.</returns>
        public bool TryGetImplementationFactory<T>(out T implementationFactory) where T : IGraphicsResourceImplementationFactory
        {
            implementationFactory = default(T);

            IGraphicsResourceImplementationFactory factory;
            if (_factoryTypeToFactory.TryGetValue(typeof(T), out factory) && factory is T)
            {
                implementationFactory = (T)factory;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Queries if the graphics resource type (e.g. VertexBuffer) is supported by any of the registered implementation factories.
        /// </summary>
        /// <typeparam name="T">Graphics resource type</typeparam>
        /// <returns>True if the type is supported by an implementation factory, false otherwise.</returns>
        public bool IsSupported<T>() where T : GraphicsResource
        {
            return _graphicResourceTypeToFactory.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IGraphicsResourceImplementationFactory> GetEnumerator()
        {
            return _factoryTypeToFactory.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _factoryTypeToFactory.Values.GetEnumerator();
        }
    }
}
