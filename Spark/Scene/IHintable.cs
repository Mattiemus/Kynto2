namespace Spark.Scene
{
    /// <summary>
    /// Represents an object that provides hints, local and absolute, that provide context to how to update/render a scene. Each hint
    /// can be set to inherit, deferring to whatever its parent does (or the default).
    /// </summary>
    public interface IHintable
    {
        /// <summary>
        /// Gets the parent of this hintable.
        /// </summary>
        IHintable ParentHintable { get; }

        /// <summary>
        /// Gets the scene hints bound to this hintable.
        /// </summary>
        SceneHints SceneHints { get; }
    }
}
