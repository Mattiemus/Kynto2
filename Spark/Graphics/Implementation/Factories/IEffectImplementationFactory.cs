namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IEffectImplementationFactory"/>.
    /// </summary>
    public interface IEffectImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <param name="effectData">Effect data</param>
        /// <returns>The effect implementation.</returns>
        IEffectImplementation CreateImplementation(EffectData effectData);
    }
}
