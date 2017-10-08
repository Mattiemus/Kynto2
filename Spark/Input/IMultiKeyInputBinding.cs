namespace Spark.Input
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a multi input binding that can be a combination of key or mouse buttons or both.
    /// </summary>
    public interface IMultiKeyInputBinding
    {
        /// <summary>
        /// Gets the input binding combination.
        /// </summary>
        IReadOnlyList<KeyOrMouseButton> InputBindings { get; }

        /// <summary>
        /// Sets the input binding combination.
        /// </summary>
        /// <param name="bindings">Input bindings.</param>
        void SetInputBindings(params KeyOrMouseButton[] bindings);
    }
}
