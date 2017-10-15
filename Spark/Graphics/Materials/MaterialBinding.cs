namespace Spark.Graphics.Materials
{
    using System;
    using System.Collections.Generic;

    using Core;
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
        /// Gets or sets the current game time
        /// </summary>
        public static IGameTime GameTime { get; set; }
        
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

        #region Base Binding Providers

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

        /// <summary>
        /// Abstract base class for a <see cref="Matrix4x4"/> parameter binding provider
        /// </summary>
        private abstract class BaseMatrix4x4ParameterBindingProvider : BaseParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BaseMatrix4x4ParameterBindingProvider"/> class.
            /// </summary>
            /// <param name="name">Named of the parameter</param>
            protected BaseMatrix4x4ParameterBindingProvider(string name)
                : base(name)
            {
            }

            /// <summary>
            /// Validates the specified effect parameter against the provider logic. If the effect parameter is not of the
            /// same format or type as the provider expects then it is not valid.
            /// </summary>
            /// <param name="parameter">Effect parameter instance to validate.</param>
            /// <returns>True if the effect parameter is valid, false otherwise.</returns>
            public override bool ValidateParameter(IEffectParameter parameter)
            {
                if (parameter == null)
                {
                    return false;
                }

                // TODO: sort this
                return true;

                return (parameter.ParameterClass == EffectParameterClass.MatrixColumns || parameter.ParameterClass == EffectParameterClass.MatrixRows) &&
                       parameter.RowCount == 4 &&
                       parameter.ColumnCount == 4 &&
                       parameter.SizeInBytes == Matrix4x4.SizeInBytes;
            }
        }

        #endregion

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
