namespace Spark.Content
{
    using System;

    /// <summary>
    /// A content item that supports conversion to another content item.
    /// </summary>
    public interface IContentCastable
    {
        /// <summary>
        /// Attempts to cast the content item to another type.
        /// </summary>
        /// <param name="targetType">Type to cast to.</param>
        /// <param name="subresourceName">Optional subresource name.</param>
        /// <returns>Casted type or null if the type could not be converted.</returns>
        object CastTo(Type targetType, string subresourceName);
    }
}
