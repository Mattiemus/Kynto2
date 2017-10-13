namespace Spark.Graphics.Materials
{
    using Renderer;

    /// <summary>
    /// Abstract base class for a parameter binding provider
    /// </summary>
    public abstract class BaseParameterBindingProvider : IParameterBindingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseParameterBindingProvider"/> class.
        /// </summary>
        /// <param name="name">Name of the parameter binding</param>
        protected BaseParameterBindingProvider(string name)
        {
            ParameterName = name;
        }

        /// <summary>
        /// Gets the name of the parameter. This is used to bind effect parameter instances
        /// to this provider in material scripts.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Validates the specified effect parameter against the provider logic. If the effect parameter is not of the
        /// same format or type as the provider expects then it is not valid.
        /// </summary>
        /// <param name="parameter">Effect parameter instance to validate.</param>
        /// <returns>True if the effect parameter is valid, false otherwise.</returns>
        public abstract bool ValidateParameter(IEffectParameter parameter);

        /// <summary>
        /// Updates the specified effect parameter with the computed value from the provider.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="properties">Render properties.</param>
        /// <param name="material">Calling material.</param>
        /// <param name="parameter">Effect parameter instance.</param>
        /// <param name="localState">Optional local state object, may be null and may be populated during the update call.</param>
        public abstract void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref object localState);
    }
}
