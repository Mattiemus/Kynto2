namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IRasterizerStateImplementation"/>.
    /// </summary>
    public interface IRasterizerStateImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        IRasterizerStateImplementation CreateImplementation();
    }
}
