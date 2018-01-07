namespace Spark.Engine
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Utilities;

    public sealed class EventDispatcher : ServiceBase, IEventDispatcher
    {
        private readonly Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;
        private readonly Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;

        public EventDispatcher()
            : base("EventDispatcher")
        {
            _recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
            _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
        }

        public override void Initialize()
        {
            ThrowIfDisposed();
            ThrowIfInitialized();

            base.Initialize();
        }

        public void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Register(recipient, null, false, action);
        }

        public void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Register(recipient, token, false, action);
        }

        public void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            ThrowIfDisposed();
            ThrowIfNotInitialized();

            Guard.Against.NullArgument(recipient, nameof(recipient));
            Guard.Against.NullArgument(action, nameof(action));

            var messageType = typeof(TMessage);
            var recipients = receiveDerivedMessagesToo ? _recipientsOfSubclassesAction : _recipientsStrictAction;

            if (!recipients.TryGetValue(messageType, out List<WeakActionAndToken> list))
            {
                list = new List<WeakActionAndToken>();
                recipients.Add(messageType, list);
            }

            var weakAction = new WeakAction<TMessage>(recipient, action);
            var item = new WeakActionAndToken(weakAction, token);
            list.Add(item);

            Cleanup();
        }

        public void Unregister(object recipient)
        {
            ThrowIfDisposed();
            ThrowIfNotInitialized();

            Guard.Against.NullArgument(recipient, nameof(recipient));

            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
        }

        public void Unregister<TMessage>(object recipient)
        {
            Unregister<TMessage>(recipient, null);
        }

        public void Unregister<TMessage>(object recipient, object token)
        {
            Unregister<TMessage>(recipient, token, null);
        }

        public void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            Unregister(recipient, null, action);
        }

        public void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            ThrowIfDisposed();
            ThrowIfNotInitialized();

            Guard.Against.NullArgument(recipient, nameof(recipient));

            UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
            Cleanup();
        }

        public void Send<TMessage>(TMessage message, object token = null)
        {
            ThrowIfDisposed();
            ThrowIfNotInitialized();

            Guard.Against.NullArgument(message, nameof(message));

            SendToTargetOrType(message, null, token);
        }

        public void Cleanup()
        {
            ThrowIfDisposed();
            ThrowIfNotInitialized();

            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing && IsInitialized)
            {
                CleanupList(_recipientsOfSubclassesAction);
                CleanupList(_recipientsStrictAction);
            }

            base.Dispose(isDisposing);
        }

        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            var listsToRemove = new List<Type>();
            foreach (var list in lists)
            {
                var recipientsToRemove = new List<WeakActionAndToken>();
                foreach (var item in list.Value)
                {
                    if (item.Action == null || !item.Action.IsAlive)
                    {
                        recipientsToRemove.Add(item);
                    }
                }

                foreach (var recipient in recipientsToRemove)
                {
                    list.Value.Remove(recipient);
                }

                if (list.Value.Count == 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (var key in listsToRemove)
            {
                lists.Remove(key);
            }
        }

        private static bool Implements(Type instanceType, Type interfaceType)
        {
            if (interfaceType == null || instanceType == null)
            {
                return false;
            }

            var interfaces = instanceType.GetInterfaces();
            foreach (var currentInterface in interfaces)
            {
                if (currentInterface == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SendToList<TMessage>(TMessage message, IEnumerable<WeakActionAndToken> list, Type messageTargetType, object token)
        {
            var listClone = list.Take(list.Count()).ToList();
            foreach (var item in listClone)
            {
                var executeAction = item.Action as IExecuteWithObject;
                if (executeAction != null &&
                    item.Action.IsAlive &&
                    item.Action.Target != null &&
                    (messageTargetType == null || item.Action.Target.GetType() == messageTargetType || Implements(item.Action.Target.GetType(), messageTargetType)) &&
                    ((item.Token == null && token == null) || item.Token != null && item.Token.Equals(token)))
                {
                    executeAction.ExecuteWithObject(message);
                }
            }
        }

        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient == null || lists.Count == 0)
            {
                return;
            }

            lock (lists)
            {
                foreach (var messageType in lists.Keys)
                {
                    foreach (var item in lists[messageType])
                    {
                        var weakAction = item.Action;
                        if (weakAction != null && recipient == weakAction.Target)
                        {
                            weakAction.MarkForDeletion();
                        }
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            Type messageType = typeof(TMessage);
            if (recipient == null || lists.Count == 0 || !lists.ContainsKey(messageType))
            {
                return;
            }

            lock (lists)
            {
                foreach (WeakActionAndToken item in lists[messageType])
                {
                    var weakActionCasted = item.Action as WeakAction<TMessage>;

                    if (weakActionCasted != null &&
                        recipient == weakActionCasted.Target &&
                        (action == null || action == weakActionCasted.Action) &&
                        (token == null || token.Equals(item.Token)))
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }

        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            var messageType = typeof(TMessage);

            foreach (var type in _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList())
            {
                List<WeakActionAndToken> list = null;
                if (messageType == type || messageType.IsSubclassOf(type) || Implements(messageType, type))
                {
                    list = _recipientsOfSubclassesAction[type];
                }

                SendToList(message, list, messageTargetType, token);
            }

            if (_recipientsStrictAction.ContainsKey(messageType))
            {
                var list = _recipientsStrictAction[messageType];
                SendToList(message, list, messageTargetType, token);
            }

            Cleanup();
        }

        private struct WeakActionAndToken
        {
            public WeakActionAndToken(WeakAction action, object token)
            {
                Action = action;
                Token = token;
            }

            public WeakAction Action { get; }

            public object Token { get; }
        }
    }
}
