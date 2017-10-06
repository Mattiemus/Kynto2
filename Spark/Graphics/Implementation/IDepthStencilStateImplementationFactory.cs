namespace Spark.Graphics.Implementation
{
    /// <summary>
    /// Defines a factory that creates platform-specific implementations of type <see cref="IDepthStencilStateImplementation"/>.
    /// </summary>
    public interface IDepthStencilStateImplementationFactory : IGraphicsResourceImplementationFactory
    {
        /// <summary>
        /// Creates a new implementation object instance.
        /// </summary>
        /// <returns>The render state implementation.</returns>
        IDepthStencilStateImplementation CreateImplementation();
    }
}
