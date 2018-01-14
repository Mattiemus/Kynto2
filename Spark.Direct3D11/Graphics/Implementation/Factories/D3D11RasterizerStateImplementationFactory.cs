namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IRasterizerStateImplementation"/>.
    /// </summary>
    public sealed class D3D11RasterizerStateImplementationFactory : D3D11GraphicsResourceImplementationFactory, IRasterizerStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11RasterizerStateImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11RasterizerStateImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(RasterizerState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public IRasterizerStateImplementation CreateImplementation()
        {
            return new D3D11RasterizerStateImplementation(D3DRenderSystem, GetNextUniqueResourceId());
        }
    }
}
