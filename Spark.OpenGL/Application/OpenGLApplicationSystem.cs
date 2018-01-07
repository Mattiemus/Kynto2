namespace Spark.OpenGL.Application
{
    using System;

    using Spark.Application;
    using Spark.Application.Implementation;
    
    using Utilities;
    using Implementation;

    /// <summary>
    /// OpenGL application system implementation
    /// </summary>
    public sealed class OpenGLApplicationSystem : Disposable, IApplicationSystem
    {
        /// <summary>
        /// Event fired when the system is disposing
        /// </summary>
        public event TypedEventHandler<IApplicationSystem> Disposing;

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name => "OpenGL Application System";

        /// <summary>
        /// Gets the name of the application platform
        /// </summary>
        public string Platform => "OpenGL 4.5";

        /// <summary>
        /// Initializes the service. This is called by the engine when a service is newly registered.
        /// </summary>
        /// <param name="engine">Engine instance</param>
        public void Initialize(SparkEngine engine)
        {
            // No-op
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="implementor">Application host implementor</param>
        /// <returns>The application host implementation</returns>
        public IApplicationHostImplementation CreateApplicationHostImplementation(ApplicationHost implementor)
        {
            return new OpenGLApplicationHostImplementation(this, implementor);
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                Disposing?.Invoke(this, EventArgs.Empty);
            }

            base.Dispose(isDisposing);
        }
    }
}
