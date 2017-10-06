namespace Spark.Graphics
{
    /// <summary>
    /// Defines a deferred render context which allows for GPU commands to be recorded on separate threads, which then are played back on the main render thread. This is
    /// used for multi-threaded rendering and has some limitations. Queries cannot be started and only updating of dynamic resources (initially with the discard write flag, then subsequently with
    /// no-overwrite) are allowed in a deferred context. Each deferred context instance is intended for usage on a single thread; contexts themselves are not threadsafe. The GPU commands do not actually get invoked until executed 
    /// by an immediate render context, and the resulting command list can be re-used as many times as desired.
    /// </summary>
    public interface IDeferredRenderContext : IRenderContext
    {
        /// <summary>
        /// Records the GPU commands that have been submitted to the deferred render context into a command list for playback by the immediate render context. The command
        /// list represents all the GPU commands submitted up to this point from the last time this method was called.
        /// </summary>
        /// <param name="restoreDeferredContextState">True if the context state should be preserved or not. If true, the state is saved and then restored afterwards. Typically this is set to false to
        /// prevent unnecessary state setting. This only affects the next command list, not the one produced by this call. If false, the context state returns to the default state (e.g. as if ClearState had been called).</param>
        /// <returns>The command list</returns>
        ICommandList FinishCommandList(bool restoreDeferredContextState);
    }
}
