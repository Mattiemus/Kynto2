namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="ISamplerStateImplementation"/>.
    /// </summary>
    public interface ISamplerStateImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        ISamplerStateImplementation CreateImplementation();
    }
}
