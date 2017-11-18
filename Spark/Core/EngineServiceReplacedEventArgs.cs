namespace Spark
{
    using System;

    /// <summary>
    /// Provides data for an event that is triggered when an engine service is replaced by a new service.
    /// </summary>
    public sealed class EngineServiceReplacedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineServiceReplacedEventArgs"/> class.
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="oldService">Old service instance</param>
        /// <param name="newService">New service instance</param>
        internal EngineServiceReplacedEventArgs(Type serviceType, IEngineService oldService, IEngineService newService)
        {
            ServiceType = serviceType;
            OldService = oldService;
            NewService = newService;
        }
        /// <summary>
        /// Gets the type that the service is registered to.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the old service that has been removed.
        /// </summary>
        public IEngineService OldService { get; }

        /// <summary>
        /// Gets the new service that has been added.
        /// </summary>
        public IEngineService NewService { get; }
    }
}
