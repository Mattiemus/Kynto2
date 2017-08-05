namespace Spark.Core
{
    /// <summary>
    /// Describes an <see cref="IEngineService"/> so that it can be dynamically loaded.
    /// </summary>
    public sealed class ServiceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly the type is located in.</param>
        /// <param name="serviceTypeName">Name of the interface type the service should be registered under.</param>
        /// <param name="typeName">Name of the concrete type to instantiate.</param>
        public ServiceDescriptor(string assemblyName, string serviceTypeName, string typeName)
        {
            AssemblyName = assemblyName;
            ServiceTypeName = serviceTypeName;
            TypeName = typeName;
            ConstructorParameters = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly the type is located in.</param>
        /// <param name="serviceTypeName">Name of the interface type the service should be registered under.</param>
        /// <param name="typeName">Name of the concrete type to instantiate.</param>
        /// <param name="parameters">Constructor parameters required by the service.</param>
        public ServiceDescriptor(string assemblyName, string serviceTypeName, string typeName, params object[] parameters)
        {
            AssemblyName = assemblyName;
            ServiceTypeName = serviceTypeName;
            TypeName = typeName;

            if (parameters == null)
            {
                ConstructorParameters = new object[0];
            }
            else
            {
                ConstructorParameters = parameters.Clone() as object[];
            }
        }

        /// <summary>
        /// Gets the assembly name (including extension).
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// Gets the interface type name the service should be registered under.
        /// </summary>
        public string ServiceTypeName { get; }

        /// <summary>
        /// Gets the concrete type name of the service to be instantiated.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets any constructor parameters the service requires.
        /// </summary>
        public object[] ConstructorParameters { get; }        
    }
}
