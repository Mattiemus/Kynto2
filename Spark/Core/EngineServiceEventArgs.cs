namespace Spark.Core
{
    using System;

    /// <summary>
    /// Provides data for an event that is triggered when an engine service is added or removed.
    /// </summary>
    public sealed class EngineServiceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineServiceEventArgs"/> class.
        /// </summary>
        /// <param name="serviceType">Event service type</param>
        /// <param name="service">Service instance</param>
        /// <param name="eventType">Service changed event type</param>
        internal EngineServiceEventArgs(Type serviceType, IEngineService service, ServiceChangeEventType eventType)
        {
            ServiceType = serviceType;
            Service = service;
            ChangedEventType = eventType;
        }

        /// <summary>
        /// Gets the type that the service is registered to.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the service that was modified.
        /// </summary>
        public IEngineService Service { get; }

        /// <summary>
        /// Gets the change event type.
        /// </summary>
        public ServiceChangeEventType ChangedEventType { get; }
    }
}
