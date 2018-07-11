namespace Spark.UI
{
    using System;

    public sealed class RoutedEvent
    {
        internal RoutedEvent(
            string name,
            RoutingStrategy routingStrategy,
            Type handlerType,
            Type ownerType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (!typeof(Delegate).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException("Must be a delegate type.", nameof(handlerType));
            }

            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            Name = name;
            RoutingStrategy = routingStrategy;
            HandlerType = handlerType;
            OwnerType = ownerType;
        }

        public Type HandlerType { get; }

        public string Name { get; }

        public Type OwnerType { get; }

        public RoutingStrategy RoutingStrategy { get; }

        public RoutedEvent AddOwner(Type type)
        {
            // TODO: Register owner somewhere.
            return this;
        }
    }
}
