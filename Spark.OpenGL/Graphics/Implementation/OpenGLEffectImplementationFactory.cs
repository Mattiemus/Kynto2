namespace Spark.OpenGL.Graphics.Implementation
{
    using Spark.Graphics;
    using Spark.Graphics.Implementation;

    /// <summary>
    /// Factory for creating <see cref="OpenGLEffectImplementation"/> instances
    /// </summary>
    public sealed class OpenGLEffectImplementationFactory : OpenGLGraphicsResourceImplementationFactory, IEffectImplementationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectImplementationFactory"/> class
        /// </summary>
        /// <param name="renderSystem">Parent render system</param>
        public OpenGLEffectImplementationFactory(OpenGLRenderSystem renderSystem)
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
            return new OpenGLEffectImplementation(OpenGLRenderSystem, effectData);
        }

        /// <summary>
        /// Initializes for the current render system, and registers the factory to the render system.
        /// </summary>
        public override void Initialize()
        {
            OpenGLRenderSystem.AddImplementationFactory<IEffectImplementationFactory>(this);
        }
    }
}
