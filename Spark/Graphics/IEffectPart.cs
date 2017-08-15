namespace Spark.Graphics
{
    using Core;

    /// <summary>
    /// Represents a part of an <see cref="Effect"/>, all the individual components that comprise a complete effect implement this interface.
    /// </summary>
    public interface IEffectPart : INamed
    {
        /// <summary>
        /// Checks if this part belongs to the given effect.
        /// </summary>
        /// <param name="effect">Effect to check against</param>
        /// <returns>True if the effect is the parent of this part, false otherwise.</returns>
        bool IsPartOf(Effect effect);
    }
}
