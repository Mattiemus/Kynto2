namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IDepthStencilStateImplementation"/>.
    /// </summary>
    public sealed class D3D11DepthStencilStateImplementationFactory : D3D11GraphicsResourceImplementationFactory, IDepthStencilStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11DepthStencilStateImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11DepthStencilStateImplementationFactory(D3D11RenderSystem renderSystem) 
            : base(renderSystem, typeof(DepthStencilState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IDepthStencilStateImplementation CreateImplementation()
        {
            return new D3D11DepthStencilStateImplementation(D3DRenderSystem, GetNextUniqueResourceId());
        }
    }
}
