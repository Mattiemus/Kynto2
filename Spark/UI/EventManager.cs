namespace Spark.UI
{
    using System;

    public static class EventManager
    {
        public static RoutedEvent RegisterRoutedEvent(
            string name,
            RoutingStrategy routingStrategy,
            Type handlerType,
            Type ownerType)
        {
            return new RoutedEvent(name, routingStrategy, handlerType, ownerType);
        }
    }
}
