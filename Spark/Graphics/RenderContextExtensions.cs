namespace Spark.Graphics
{
    /// <summary>
    /// Extensions methods to the <see cref="IRenderContext"/> interface.
    /// </summary>
    public static class RenderContextExtensions
    {
        /// <summary>
        /// Binds the specified render target to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound
        /// render targets.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="renderTarget">Render target to bind.</param>
        public static void SetRenderTarget(this IRenderContext context, IRenderTarget renderTarget)
        {
            context.SetRenderTarget(SetTargetOptions.None, renderTarget);
        }

        /// <summary>
        /// Binds the specified number of render targets, starting at the first slot. Any remaining slots are set to null. A render target cannot be bound
        /// as both input and output at the same time. A value of null will unbind all currently
        /// bound render targets.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="renderTargets">Render targets to bind.</param>
        public static void SetRenderTargets(this IRenderContext context, params IRenderTarget[] renderTargets)
        {
            context.SetRenderTargets(SetTargetOptions.None, renderTargets);
        }
    }
}
