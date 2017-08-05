namespace Spark.Graphics.Implementation
{
    using System;

    using Utilities;

    /// <summary>
    /// Base class for all platform-specific graphics resource implementations that are created by a render system and bound to a graphics resource.
    /// </summary>
    public abstract class GraphicsResourceImplementation : BaseDisposable, IGraphicsResourceImplementation
    {
        private string _name;

        /// <summary>
        /// Constructs a new instance of the <see cref="GraphicsResourceImplementation"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system that manages this graphics implementation</param>
        /// <param name="resourceID">ID of the resource, supplied by the render system</param>
        protected GraphicsResourceImplementation(IRenderSystem renderSystem, int resourceId)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }
            
            ResourceId = resourceId;
            RenderSystem = renderSystem;
            _name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnNameChange(value);
            }
        }

        /// <summary>
        /// Gets the ID of this resource.
        /// </summary>
        public int ResourceId { get; }

        /// <summary>
        /// Gets the render system that created and manages this resource.
        /// </summary>
        public IRenderSystem RenderSystem { get; }

        /// <summary>
        /// Gets the graphics resource parent that this implementation is bound to.
        /// </summary>
        public GraphicsResource Parent { get; private set; }

        /// <summary>
        /// Called by the parent graphics resource when the implementation is first created. This only should be called once, during the creation of
        /// a graphics resource.
        /// </summary>
        /// <param name="resource">Graphics resource that is to be bound to this implementation</param>
        public void BindImplementation(GraphicsResource resource)
        {
            if (Parent != null)
            {
                throw new InvalidOperationException("Graphics resource implementation is already bound");
            }

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            Parent = resource;
        }

        /// <summary>
        /// Called when the name of the graphics resource is changed, useful if the implementation wants to set the name to
        /// be used as a debug name.
        /// </summary>
        /// <param name="name">New name of the resource</param>
        protected virtual void OnNameChange(String name)
        {
        }
    }
}
