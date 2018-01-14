namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="ISamplerStateImplementation"/>.
    /// </summary>
    public sealed class D3D11SamplerStateImplementationFactory : D3D11GraphicsResourceImplementationFactory, ISamplerStateImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11SamplerStateImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11SamplerStateImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(SamplerState))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        public ISamplerStateImplementation CreateImplementation()
        {
            return new D3D11SamplerStateImplementation(D3DRenderSystem, GetNextUniqueResourceId());
        }
    }
}
