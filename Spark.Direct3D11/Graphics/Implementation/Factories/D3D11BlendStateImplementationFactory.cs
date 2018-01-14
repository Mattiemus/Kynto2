namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    
    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IBlendStateImplementation"/>.
    /// </summary>
    public sealed class D3D11BlendStateImplementationFactory : D3D11GraphicsResourceImplementationFactory, IBlendStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11BlendStateImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11BlendStateImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(BlendState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IBlendStateImplementation CreateImplementation()
        {
            return new D3D11BlendStateImplementation(D3DRenderSystem, GetNextUniqueResourceId());
        }
    }
}
