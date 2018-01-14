namespace Spark.Direct3D11.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// A factory that creates Direct3D11 implementations of type <see cref="IEffectImplementation"/>.
    /// </summary>
    public sealed class D3D11EffectImplementationFactory : D3D11GraphicsResourceImplementationFactory, IEffectImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11EffectImplementationFactory"/> class.
        /// </summary>
        /// <param name="renderSystem">The D3D11 render system.</param>
        public D3D11EffectImplementationFactory(D3D11RenderSystem renderSystem)
            : base(renderSystem, typeof(Effect))
        {
        }

        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="effectData">Effect data</param>
        /// <returns>The effect implementation.</returns>
        public IEffectImplementation CreateImplementation(EffectData effectData)
        {
            return new D3D11EffectImplementation(D3DRenderSystem, GetNextUniqueResourceId(), effectData);
        }
    }
}
