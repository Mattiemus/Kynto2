namespace Spark.Graphics.Materials
{
    using Math;
    using Renderer;

    /// <summary>
    /// Parameter provider of the camera view projection matrix
    /// </summary>
    public sealed class ViewProjectionMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProjectionMatrixParameterBindingProvider"/> class.
        /// </summary>
        public ViewProjectionMatrixParameterBindingProvider() 
            : base("ViewProjectionMatrix")
        {
        }

        /// <summary>
        /// Updates the specified effect parameter with the computed value from the provider.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="properties">Render properties.</param>
        /// <param name="material">Calling material.</param>
        /// <param name="parameter">Effect parameter instance.</param>
        /// <param name="localState">Optional local state object, may be null and may be populated during the update call.</param>
        public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref object localState)
        {
            Matrix4x4 viewProj = context.Camera.ViewProjectionMatrix;

            parameter.SetValue(ref viewProj);
        }
    }
}
