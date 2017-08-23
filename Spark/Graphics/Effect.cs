namespace Spark.Graphics
{
    using System;

    using Core;
    using Content;
    using Implementation;

    /// <summary>
    /// Represents an effect that is used to render an object. This completely describes how to configure the graphics pipeline - what render states, resources,
    /// and shaders are bound to the GPU. An effect is necessary to render any object.
    /// </summary>
    public sealed class Effect : GraphicsResource, ISavable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        private Effect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="effectByteCode">Compiled effect code that represents this effect.</param>
        public Effect(IRenderSystem renderSystem, byte[] effectByteCode)
        {
            CreateImplementation(renderSystem, effectByteCode);
        }

        /// <summary>
        /// Gets all effect parameters contained in this effect.
        /// </summary>
        public EffectParameterCollection Parameters => EffectImplementation.Parameters; 

        /// <summary>
        /// Gets the compiled byte code that represents this effect.
        /// </summary>
        public byte[] EffectByteCode => EffectImplementation.EffectByteCode;

        /// <summary>
        /// Gets the effect implementation
        /// </summary>
        private IEffectImplementation EffectImplementation
        {
            get
            {
                return Implementation as IEffectImplementation;
            }
            set
            {
                BindImplementation(value);
            }
        }

        /// <summary>
        /// Binds the set of shaders defined in the group and the resources used by them to the context.
        /// </summary>
        /// <param name="renderContext">Render context to apply to.</param>
        public void Apply(IRenderContext renderContext)
        {
            EffectImplementation.Apply(renderContext);
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return EffectImplementation.ContainsShader(shaderStage);
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelpers.GetRenderSystem(Engine.Instance.Services);

            string name = input.ReadString();
            byte[] effectByteCode = input.ReadByteArray();

            CreateImplementation(renderSystem, effectByteCode);
            Name = name;
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Name", EffectImplementation.Name);
            output.Write("EffectByteCode", EffectImplementation.EffectByteCode);
        }
        
        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem"></param>
        /// <param name="effectByteCode"></param>
        private void CreateImplementation(IRenderSystem renderSystem, byte[] effectByteCode)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (effectByteCode == null || effectByteCode.Length == 0)
            {
                throw new ArgumentNullException(nameof(effectByteCode));
            }
            
            if (!renderSystem.TryGetImplementationFactory(out IEffectImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                EffectImplementation = factory.CreateImplementation(effectByteCode);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
