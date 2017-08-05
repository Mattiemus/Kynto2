namespace Spark.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Registry that manages core engine services. An engine service typically is registered by an interface that it implements, e.g. a render system
    /// implementation to an <code>IRenderSystem</code> interface.
    /// </summary>
    public sealed class EngineServiceRegistry : IServiceProvider, IEnumerable<IEngineService>
    {
        private readonly Dictionary<Type, IEngineService> _services;
        private readonly List<IUpdatableEngineService> _updateServices;
        private readonly Engine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineServiceRegistry"/> class.
        /// </summary>
        /// <param name="engine"></param>
        internal EngineServiceRegistry(Engine engine)
        {
            _services = new Dictionary<Type, IEngineService>();
            _updateServices = new List<IUpdatableEngineService>();
            _engine = engine;
        }

        /// <summary>
        /// Occurs when an engine service has been added to the registry.
        /// </summary>
        public event TypedEventHandler<EngineServiceRegistry, EngineServiceEventArgs> ServiceAdded;

        /// <summary>
        /// Occurs when an engine service has been removed from the registry. This occurs before the service is potentially disposed.
        /// </summary>
        public event TypedEventHandler<EngineServiceRegistry, EngineServiceEventArgs> ServiceRemoved;

        /// <summary>
        /// Occurs when an existing engine service has been replaced with a new one. This occurs before the old service is potentially disposed.
        /// </summary>
        public event TypedEventHandler<EngineServiceRegistry, EngineServiceReplacedEventArgs> ServiceReplaced;

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                return null;
            }

            IEngineService service;
            if (_services.TryGetValue(serviceType, out service))
            {
                return service;
            }

            return GetNextBestService(serviceType);
        }

        /// <summary>
        /// Adds a service to the registry.
        /// </summary>
        /// <typeparam name="T">Service type - commonly an interface.</typeparam>
        /// <param name="service">The service to add</param>
        public void AddService<T>(IEngineService service) where T : class, IEngineService
        {
            AddService(typeof(T), service);
        }

        /// <summary>
        /// Adds a service to the registry.
        /// </summary>
        /// <param name="type">Type of service to add - commonly an interface.</param>
        /// <param name="service">The service to add</param>
        public void AddService(Type type, IEngineService service)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Service cannot be null");
            }

            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Service is not assignable to the given type", nameof(service));
            }

            if (_services.ContainsKey(type))
            {
                throw new SparkException("Engine service is already registered");
            }

            _services.Add(type, service);

            if (service is IUpdatableEngineService)
            {
                _updateServices.Add(service as IUpdatableEngineService);
            }

            service.Initialize(_engine);
            OnServiceAdded(type, service);
        }

        /// <summary>
        /// Gets a service from the registry.
        /// </summary>
        /// <typeparam name="T">Service type - commonly an interface.</typeparam>
        /// <returns>The service, or null if it does not exist or the service type is not assignable from the actual object type</returns>
        public T GetService<T>() where T : class, IEngineService
        {
            Type type = typeof(T);

            IEngineService service;
            if (_services.TryGetValue(type, out service))
            {
                return service as T;
            }

            return GetNextBestService(type) as T;
        }

        /// <summary>
        /// Replaces a service in the registry. If an already registered service is not found, the new one is simply added. Otherwise,
        /// the old one is replaced. This is mainly used to "hot swap" currently running implementations.
        /// </summary>
        /// <typeparam name="T">Service type - commonly an interface.</typeparam>
        /// <param name="service">The service</param>
        /// <param name="disposeOldService">True if the old service should be disposed, false otherwise. By default this is true.</param>
        /// <returns>The old engine service, if there was one.</returns>
        public IEngineService ReplaceService<T>(IEngineService service, bool disposeOldService = true) where T : class, IEngineService
        {
            return ReplaceService(typeof(T), service, disposeOldService);
        }

        /// <summary>
        /// Replaces a service in the registry. If an already registered service is not found, the new one is simply added. Otherwise,
        /// the old one is replaced. This is mainly used to "hot swap" currently running implementations. The order of services that can be
        /// updated is preserved.
        /// </summary>
        /// <param name="type">Type of service to replace - commonly an interface.</param>
        /// <param name="service">The service</param>
        /// <param name="disposeOldService">True if the old service should be disposed, false otherwise. By default this is true.</param>
        /// <returns>The old engine service, if there was one.</returns>
        public IEngineService ReplaceService(Type type, IEngineService service, bool disposeOldService = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Service cannot be null");
            }

            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Service is not assignable to the given type", nameof(service));
            }

            IEngineService oldService;
            if (!_services.TryGetValue(type, out oldService))
            {
                _services.Add(type, service);

                if (service is IUpdatableEngineService)
                {
                    _updateServices.Add(service as IUpdatableEngineService);
                }

                service.Initialize(_engine);
                OnServiceAdded(type, service);
            }
            else
            {
                _services[type] = service;
                
                bool oldServiceUpdateable = oldService is IUpdatableEngineService;
                bool newServiceUpdateable = service is IUpdatableEngineService;
                if (oldServiceUpdateable && newServiceUpdateable)
                {
                    int index = _updateServices.IndexOf(oldService as IUpdatableEngineService);
                    _updateServices[index] = service as IUpdatableEngineService;
                }
                else if (oldServiceUpdateable)
                {
                    _updateServices.Remove(oldService as IUpdatableEngineService);
                }

                service.Initialize(_engine);
                OnServiceReplaced(type, oldService, service);

                if (disposeOldService)
                {
                    DisposeService(oldService);
                }
            }

            return oldService;
        }

        /// <summary>
        /// Removes a service from the registry.
        /// </summary>
        /// <typeparam name="T">Service type - commonly an interface.</typeparam>
        /// <param name="dispose">True if the service should be disposed, false otherwise. By default this is true.</param>
        /// <returns>The old engine service, if there was one.</returns>
        public IEngineService RemoveService<T>(bool dispose = true) where T : class, IEngineService
        {
            return RemoveService(typeof(T), dispose);
        }

        /// <summary>
        /// Removes a service from the registry.
        /// </summary>
        /// <param name="type">Service type - commonly an interface.</param>
        /// <param name="dispose">True if the service should be disposed, false otherwise. By default this is true.</param>
        /// <returns>The old engine service, if there was one.</returns>
        public IEngineService RemoveService(Type type, bool dispose = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            IEngineService oldService;
            if (_services.TryGetValue(type, out oldService))
            {
                _services.Remove(type);

                if (oldService is IUpdatableEngineService)
                {
                    _updateServices.Remove(oldService as IUpdatableEngineService);
                }

                OnServiceRemoved(type, oldService);

                if (dispose)
                {
                    DisposeService(oldService);
                }
            }

            return oldService;
        }

        /// <summary>
        /// Updates all engine services (in the order they were registered) in the registry that implement the <see cref="IUpdatableEngineService"/> interface.
        /// </summary>
        public void UpdateServices()
        {
            for (int i = 0; i < _updateServices.Count; i++)
            {
                _updateServices[i].Update();
            }
        }

        /// <summary>
        /// Called when the engine is destroyed to clear the registry but also dispose of all resources.
        /// </summary>
        internal void RemoveAllAndDispose()
        {
            foreach (KeyValuePair<Type, IEngineService> kv in _services)
            {
                DisposeService(kv.Value);
            }

            _services.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IEngineService> GetEnumerator()
        {
            return _services.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _services.Values.GetEnumerator();
        }

        /// <summary>
        /// Disposes a service instance
        /// </summary>
        /// <param name="service">Service to dispose</param>
        private void DisposeService(IEngineService service)
        {
            IDisposableEngineService disposable = service as IDisposableEngineService;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Gets the next best serviice instance for the given type
        /// </summary>
        /// <param name="type">Type of service to get</param>
        /// <returns>Next best possible instance for the given service type</returns>
        private IEngineService GetNextBestService(Type type)
        {
            foreach (KeyValuePair<Type, IEngineService> kv in _services)
            {
                if (kv.Key.IsAssignableFrom(type))
                {
                    return kv.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Invokes the service added event
        /// </summary>
        /// <param name="type">Service type</param>
        /// <param name="service">Service instance</param>
        private void OnServiceAdded(Type type, IEngineService service)
        {
            ServiceAdded?.Invoke(this, new EngineServiceEventArgs(type, service, ServiceChangeEventType.Added));
        }

        /// <summary>
        /// Invokes the service removed event
        /// </summary>
        /// <param name="type">Service type</param>
        /// <param name="service">Service instance</param>
        private void OnServiceRemoved(Type type, IEngineService service)
        {
            ServiceRemoved?.Invoke(this, new EngineServiceEventArgs(type, service, ServiceChangeEventType.Removed));
        }

        /// <summary>
        /// Invokes the service replaced event
        /// </summary>
        /// <param name="type">Service type</param>
        /// <param name="oldService">Old service instance</param>
        /// <param name="newService">New service instance</param>
        private void OnServiceReplaced(Type type, IEngineService oldService, IEngineService newService)
        {
            ServiceReplaced?.Invoke(this, new EngineServiceReplacedEventArgs(type, oldService, newService));
        }
    }
}
