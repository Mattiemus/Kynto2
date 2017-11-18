namespace Spark.Graphics
{
    using System;
    
    /// <summary>
    /// Defines a command list, which is a list of GPU commands that can be played back by an immediate render context.
    /// </summary>
    public interface ICommandList : INamable, IDisposable
    {
        /// <summary>
        /// Gets if the command list has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }
    }
}
