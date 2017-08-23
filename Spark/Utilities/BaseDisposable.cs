namespace Spark.Utilities
{
    using System;

    public abstract class BaseDisposable : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="BaseDisposable"/> class.
        /// </summary>
        ~BaseDisposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indcating whether the instance has been disposed
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs the dispose action
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected abstract void DisposeInternal(bool isDisposing);

        /// <summary>
        /// Disposes the graphics resource
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            DisposeInternal(isDisposing);

            IsDisposed = true;
        }

        /// <summary>
        /// Throws an exception if the graphics resource is disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
    }
}
