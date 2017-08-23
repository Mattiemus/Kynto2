namespace Spark.Content
{
    using System;

    /// <summary>
    /// A handler that returns a "default" piece of content to serve as a placeholder for content that could not be located or loaded.
    /// </summary>
    public interface IMissingContentHandler
    {
        /// <summary>
        /// Gets the content target type this handler serves.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Gets a place holder piece of content. The type must be the same as the target type or
        /// can be casted to it.
        /// </summary>
        /// <typeparam name="Content">Content type</typeparam>
        /// <returns>The placeholder content</returns>
        Content GetPlaceHolderContent<Content>() where Content : class;
    }
}
