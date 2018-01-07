namespace Spark.Engine
{
    using System;

    using Utilities;

    public abstract class ServiceBase : Disposable, IService
    {
        protected ServiceBase(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            Name = serviceName;
        }

        public string Name { get; }

        public bool IsInitialized { get; private set; }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsInitialized = false;

            base.Dispose(isDisposing);
        }

        protected void ThrowIfInitialized()
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Service has already been initialized");
            }
        }

        protected void ThrowIfNotInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Service has not been initialized");
            }
        }
    }
}
