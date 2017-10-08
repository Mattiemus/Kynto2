namespace Spark.Input
{
    /// <summary>
    /// Defines an input binding that is either a key or mouse button.
    /// </summary>
    public interface IKeyInputBinding
    {
        /// <summary>
        /// Gets or sets the input binding.
        /// </summary>
        KeyOrMouseButton InputBinding { get; set; }
    }
}
