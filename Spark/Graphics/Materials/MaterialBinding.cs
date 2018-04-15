namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    
    using Math;
    
    /// <summary>
    /// Definitions for built in bindable material parameters
    /// </summary>
    public static class MaterialBinding
    {
        private static Dictionary<string, IParameterBindingProvider> _providers;

        /// <summary>
        /// Static initializer for the <see cref="MaterialBinding"/> class.
        /// </summary>
        static MaterialBinding()
        {
            _providers = new Dictionary<string, IParameterBindingProvider>();

            WorldMatrix = new WorldMatrixParameterBindingProvider();
            Register(WorldMatrix);

            ViewMatrix = new ViewMatrixParameterBindingProvider();
            Register(ViewMatrix);

            ProjectionMatrix = new ProjectionMatrixParameterBindingProvider();
            Register(ProjectionMatrix);

            WorldViewMatrix = new WorldViewMatrixParameterBindingProvider();
            Register(WorldViewMatrix);

            ViewProjectionMatrix = new ViewProjectionMatrixParameterBindingProvider();
            Register(ViewProjectionMatrix);

            WorldViewProjectionMatrix = new WorldViewProjectionParameterBindingProvider();
            Register(WorldViewProjectionMatrix);

            WorldMatrixInverse = new WorldMatrixInverseParameterBindingProvider();
            Register(WorldMatrixInverse);

            ViewMatrixInverse = new ViewMatrixInverseParameterBindingProvider();
            Register(ViewMatrixInverse);

            ProjectionMatrixInverse = new ProjectionMatrixInverseParameterBindingProvider();
            Register(ProjectionMatrixInverse);

            WorldMatrixInverseTranspose = new WorldMatrixInverseTransposeParameterBindingProvider();
            Register(WorldMatrixInverseTranspose);
        }

        /// <summary>
        /// Gets or sets the current game time
        /// </summary>
        public static IGameTime GameTime { get; set; }

        /// <summary>
        /// Gets the world matrix parameter provider
        /// </summary>
        public static IParameterBindingProvider WorldMatrix { get; private set; }

        /// <summary>
        /// Gets the view matrix parameter provider
        /// </summary>
        public static IParameterBindingProvider ViewMatrix { get; private set; }

        /// <summary>
        /// Gets the projection matrix parameter provider
        /// </summary>
        public static IParameterBindingProvider ProjectionMatrix { get; private set; }

        /// <summary>
        /// Gets the  world view matrix
        /// </summary>
        public static IParameterBindingProvider WorldViewMatrix { get; private set; }

        /// <summary>
        /// Gets the camera view projection matrix parameter provider
        /// </summary>
        public static IParameterBindingProvider ViewProjectionMatrix { get; private set; }

        /// <summary>
        /// Gets the world view projection matrix
        /// </summary>
        public static IParameterBindingProvider WorldViewProjectionMatrix { get; private set; }

        /// <summary>
        /// Gets the world matrix inverse
        /// </summary>
        public static IParameterBindingProvider WorldMatrixInverse { get; private set; }
        
        /// <summary>
        /// Gets the view matrix inverse
        /// </summary>
        public static IParameterBindingProvider ViewMatrixInverse { get; private set; }
        
        /// <summary>
        /// Gets the projection matrix inverse
        /// </summary>
        public static IParameterBindingProvider ProjectionMatrixInverse { get; private set; }
        
        /// <summary>
        /// Gets the world matrix inverse transpose
        /// </summary>
        public static IParameterBindingProvider WorldMatrixInverseTranspose { get; private set; }

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
                
                return (parameter.ParameterClass == EffectParameterClass.MatrixColumns || parameter.ParameterClass == EffectParameterClass.MatrixRows) &&
                       parameter.RowCount == 4 &&
                       parameter.ColumnCount == 4 &&
                       parameter.SizeInBytes == Matrix4x4.SizeInBytes;
            }
        }

        #endregion

        #region Matrix4x4 Binding Providers

        /// <summary>
        /// Parameter provider of the world matrix
        /// </summary>
        private sealed class WorldMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldMatrixParameterBindingProvider"/> class.
            /// </summary>
            public WorldMatrixParameterBindingProvider() 
                : base("WorldMatrix")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 world = Matrix4x4.Identity;
                if (properties.TryGet(out WorldTransformProperty prop))
                {
                    prop.Value.GetMatrix(out world);
                }

                parameter.SetValue(ref world);
            }
        }

        /// <summary>
        /// Parameter provider of the camera view matrix
        /// </summary>
        private sealed class ViewMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ViewMatrixParameterBindingProvider"/> class.
            /// </summary>
            public ViewMatrixParameterBindingProvider() 
                : base("ViewMatrix")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 view = context.Camera.ViewMatrix;
                parameter.SetValue(ref view);
            }
        }

        /// <summary>
        /// Parameter provider of the camera projection matrix
        /// </summary>
        private sealed class ProjectionMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProjectionMatrixParameterBindingProvider"/> class.
            /// </summary>
            public ProjectionMatrixParameterBindingProvider() 
                : base("ProjectionMatrix")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 projection = context.Camera.ProjectionMatrix;
                parameter.SetValue(ref projection);
            }
        }

        /// <summary>
        /// Parameter provider of the world view matrix
        /// </summary>
        private sealed class WorldViewMatrixParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldViewMatrixParameterBindingProvider"/> class.
            /// </summary>
            public WorldViewMatrixParameterBindingProvider() 
                : base("WorldViewMatrix")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 world = Matrix4x4.Identity;
                if (properties.TryGet(out WorldTransformProperty prop))
                {
                    prop.Value.GetMatrix(out world);
                }

                Matrix4x4 view = context.Camera.ViewMatrix;
                Matrix4x4.Multiply(ref world, ref view, out Matrix4x4 wv);

                parameter.SetValue(ref wv);
            }
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

        /// <summary>
        /// Parameter provider of the world view projection matrix
        /// </summary>
        private sealed class WorldViewProjectionParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldViewProjectionParameterBindingProvider"/> class.
            /// </summary>
            public WorldViewProjectionParameterBindingProvider() 
                : base("WorldViewProjectionMatrix")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 world = Matrix4x4.Identity;
                if (properties.TryGet(out WorldTransformProperty prop))
                {
                    prop.Value.GetMatrix(out world);
                }

                Matrix4x4 viewProj = context.Camera.ViewProjectionMatrix;
                Matrix4x4.Multiply(ref world, ref viewProj, out Matrix4x4 wvp);

                parameter.SetValue(ref wvp);
            }
        }

        /// <summary>
        /// Parameter provider of the world matrix inverse
        /// </summary>
        private sealed class WorldMatrixInverseParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldMatrixInverseParameterBindingProvider"/> class.
            /// </summary>
            public WorldMatrixInverseParameterBindingProvider() 
                : base("WorldMatrixInverse")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 world = Matrix4x4.Identity;
                if (properties.TryGet(out WorldTransformProperty prop))
                {
                    prop.Value.GetMatrix(out world);
                    world.Invert();
                }

                parameter.SetValue(ref world);
            }
        }

        /// <summary>
        /// Parameter provider of the view matrix inverse
        /// </summary>
        private sealed class ViewMatrixInverseParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldMatrixInverseParameterBindingProvider"/> class.
            /// </summary>
            public ViewMatrixInverseParameterBindingProvider() 
                : base("ViewMatrixInverse")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 view = context.Camera.ViewMatrix;
                view.Invert();

                parameter.SetValue(ref view);
            }
        }

        /// <summary>
        /// Parameter provider of the projection matrix inverse
        /// </summary>
        private sealed class ProjectionMatrixInverseParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProjectionMatrixInverseParameterBindingProvider"/> class.
            /// </summary>
            public ProjectionMatrixInverseParameterBindingProvider() 
                : base("ProjectionMatrixInverse")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 proj = context.Camera.ProjectionMatrix;
                proj.Invert();

                parameter.SetValue(ref proj);
            }
        }

        /// <summary>
        /// Parameter provider of the world matrix inverse transpose
        /// </summary>
        private sealed class WorldMatrixInverseTransposeParameterBindingProvider : BaseMatrix4x4ParameterBindingProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorldMatrixInverseTransposeParameterBindingProvider"/> class.
            /// </summary>
            public WorldMatrixInverseTransposeParameterBindingProvider() 
                : base("WorldMatrixInverseTranspose")
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
            public override void UpdateParameter(IRenderContext context, RenderPropertyCollection properties, Material material, IEffectParameter parameter, ref Object localState)
            {
                Matrix4x4 world = Matrix4x4.Identity;
                if (properties.TryGet(out WorldTransformProperty prop))
                {
                    prop.Value.GetMatrix(out world);
                    world.Invert();
                    world.Transpose();
                }

                parameter.SetValue(ref world);
            }
        }

        #endregion
    }
}
