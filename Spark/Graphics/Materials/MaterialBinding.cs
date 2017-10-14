namespace Spark.Graphics.Materials
{
    using System;
    using System.Collections.Generic;

    using Math;
    using Graphics.Renderer;
    
    /// <summary>
    /// Definitions for built in bindable material parameters
    /// </summary>
    public static class MaterialBinding
    {
        private static Dictionary<String, IParameterBindingProvider> _providers;

        /// <summary>
        /// Static initializer for the <see cref="MaterialBinding"/> class.
        /// </summary>
        static MaterialBinding()
        {
            _providers = new Dictionary<String, IParameterBindingProvider>();

            ViewProjectionMatrix = new ViewProjectionMatrixParameterBindingProvider();
            Register(ViewProjectionMatrix);
        }
        
        /// <summary>
        /// Gets the camera view projection matrix parameter provider
        /// </summary>
        public static IParameterBindingProvider ViewProjectionMatrix { get; private set; }
        
        /// <summary>
        /// Registers a parameter binding provider
        /// </summary>
        /// <param name="provider">Binding provider</param>
        /// <returns></returns>
        public static bool Register(IParameterBindingProvider provider)
        {
            if (provider == null)
            {
                return false;
            }

            if (_providers.ContainsKey(provider.ParameterName))
            {
                return false;
            }

            _providers.Add(provider.ParameterName, provider);
            return true;
        }

        /// <summary>
        /// Gets a material parameter by its name
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter by its given name</returns>
        public static IParameterBindingProvider GetProvider(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return null;
            }

            if (_providers.TryGetValue(parameterName, out IParameterBindingProvider provider))
            {
                return provider;
            }

            return null;
        }

        /// <summary>
        /// Parameter provider of the camera view projection matrix
        /// </summary>
        private sealed class ViewProjectionMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
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
}
