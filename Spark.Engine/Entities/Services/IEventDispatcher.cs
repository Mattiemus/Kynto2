namespace Spark.Engine
{
    using System;

    public interface IEventDispatcher : IService
    {
        void Register<TMessage>(object recipient, Action<TMessage> action);

        void Register<TMessage>(object recipient, object token, Action<TMessage> action);

        void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action);

        void Unregister<TMessage>(object recipient);

        void Unregister<TMessage>(object recipient, object token);

        void Unregister<TMessage>(object recipient, Action<TMessage> action);

        void Unregister<TMessage>(object recipient, object token, Action<TMessage> action);

        void Send<TMessage>(TMessage message, object token = null);

        void Cleanup();
    }
}
