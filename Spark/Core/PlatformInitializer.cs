namespace Spark
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>
    /// Generic platform initializer that dynamically loads assemblies containing implementations of <see cref="IEngineService"/> and
    /// then registers them to the engine.
    /// </summary>
    public sealed class PlatformInitializer : IPlatformInitializer
    {
        private readonly ServiceDescriptor[] _descriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformInitializer"/> class.
        /// </summary>
        /// <param name="servicesToLoad">The services to dynamically load and register.</param>
        public PlatformInitializer(IEnumerable<ServiceDescriptor> servicesToLoad)
        {
            if (servicesToLoad == null)
            {
                throw new ArgumentNullException(nameof(servicesToLoad));
            }

            _descriptors = servicesToLoad.ToArray();
        }

        /// <summary>
        /// Initializes the platform's services.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(Engine engine)
        {
            foreach (ServiceDescriptor descriptor in _descriptors)
            {
                if (descriptor == null)
                {
                    continue;
                }
                
                Assembly assembly = Assembly.Load(descriptor.AssemblyName);

                Type type = assembly.GetType(descriptor.TypeName);
                if (type == null)
                {
                    ThrowIfBadDescriptor(descriptor);
                }

                Type serviceType = Type.GetType(descriptor.ServiceTypeName);
                if (serviceType == null)
                {
                    ThrowIfBadDescriptor(descriptor);
                }

                if (!typeof(IEngineService).IsAssignableFrom(serviceType))
                {
                    throw new SparkException("Type is not an engine service");
                }

                IEngineService service;
                if (descriptor.ConstructorParameters == null || descriptor.ConstructorParameters.Length == 0)
                {
                    service = Activator.CreateInstance(type) as IEngineService;
                }
                else
                {
                    service = Activator.CreateInstance(type, descriptor.ConstructorParameters) as IEngineService;
                }

                engine.Services.AddService(serviceType, service);
            }
        }

        /// <summary>
        /// Throws an exception if the service descriptor cannot be loaded
        /// </summary>
        /// <param name="descriptor">Descriptor which could not be loaded</param>
        private void ThrowIfBadDescriptor(ServiceDescriptor descriptor)
        {
            throw new SparkException($"Unable to load service '{descriptor.TypeName}' from assembly '{descriptor.AssemblyName}'");
        }
    }
}
