namespace Spark.Graphics
{
    using System;

    using Implementation;
    
    /// <summary>
    /// Base class for all graphics resources that are created and managed by a render system.
    /// </summary>
    public abstract class GraphicsResource : IDisposable, INamable
    {
        private IGraphicsResourceImplementation _impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsResource"/> class.
        /// </summary>
        protected GraphicsResource()
        {
        }

        /// <summary>
        /// Occurs when the graphics resource is disposed.
        /// </summary>
        public event TypedEventHandler<GraphicsResource> Disposing;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name
        {
            get
            {
                if (_impl != null)
                {
                    return _impl.Name;
                }

                return string.Empty;
            }
            set
            {
                if (_impl != null)
                {
                    _impl.Name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets custom data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets if this resource has been disposed or not.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                if (_impl == null)
                {
                    return true;
                }

                return _impl.IsDisposed;
            }
        }

        /// <summary>
        /// Gets the ID of this resource.
        /// </summary>
        public int ResourceId
        {
            get
            {
                if (_impl == null)
                {
                    return -1;
                }

                return _impl.ResourceId;
            }
        }

        /// <summary>
        /// Gets the render system that created and manages this resource.
        /// </summary>
        public IRenderSystem RenderSystem
        {
            get
            {
                if (_impl == null)
                {
                    return null;
                }

                return _impl.RenderSystem;
            }
        }

        /// <summary>
        /// Gets the underlying platform-specific implementation of this resource.
        /// </summary>
        public IGraphicsResourceImplementation Implementation => _impl;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return _impl.ResourceId;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_impl == null || _impl.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                OnDispose();
                _impl.Dispose();
            }
        }

        /// <summary>
        /// Binds an implementation to the current graphics resource. An implementation can only be bound to a single graphics resource.
        /// </summary>
        /// <param name="implementor">Graphics resource implementation</param>
        protected void BindImplementation(IGraphicsResourceImplementation implementor)
        {
            if (implementor == null)
            {
                throw new ArgumentNullException(nameof(implementor), "Cannot bind to a null implementation");
            }

            _impl = implementor;
            _impl.BindImplementation(this);
        }

        /// <summary>
        /// Checks if writing to a resource is permitted with a deferred render context. Writing is valid if its the immediate
        /// context, or if deferred and the resource is a dynamic resource.
        /// </summary>
        /// <param name="renderContext">The current render context</param>
        /// <param name="usage">Usage of the resource</param>
        protected void ThrowIfDefferedSetDataIsNotPermitted(IRenderContext renderContext, ResourceUsage usage)
        {
            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext), "Render context cannot be  null");
            }

            if (!renderContext.IsImmediateContext && usage != ResourceUsage.Dynamic)
            {
                throw new SparkGraphicsException("Deferred set data must be dynamic");
            }
        }

        /// <summary>
        /// Checks if the resource was disposed and if so, throws an ObjectDisposedException.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
        
        /// <summary>
        /// Called right before the implementation is disposed.
        /// </summary>
        protected virtual void OnDispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
