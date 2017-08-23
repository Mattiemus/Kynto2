namespace Spark.Content
{
    /// <summary>
    /// Interface for a handler that resolves an external reference path to a full path.
    /// </summary>
    public interface IExternalReferenceResolver
    {
        /// <summary>
        /// Gets the content manager that can load the external resource.
        /// </summary>
        ContentManager ContentManager { get; }

        /// <summary>
        /// Gets the resource file this handler resolves references relative to.
        /// </summary>
        IResourceFile ParentResourceFile { get; }

        /// <summary>
        /// Resolves an external reference and loads the savable using the content manager.
        /// </summary>
        /// <typeparam name="T">Savable type</typeparam>
        /// <param name="externalReference">External reference</param>
        /// <returns>The read savable</returns>
        T ResolveSavable<T>(ExternalReference externalReference) where T : ISavable;
    }
}
