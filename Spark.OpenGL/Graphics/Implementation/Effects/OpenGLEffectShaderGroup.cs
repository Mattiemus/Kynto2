namespace Spark.OpenGL.Graphics.Implementation.Effects
{
    using Spark.Utilities;
    using Spark.Graphics;
    using Spark.Graphics.Effects;
    
    /// <summary>
    /// OpenGL implementation of a shader group
    /// </summary>
    public sealed class OpenGLEffectShaderGroup : BaseDisposable, IEffectShaderGroup
    {
        private readonly OpenGLShaderProgram _program;
        private readonly OpenGLEffectImplementation _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectShaderGroup"/> class
        /// </summary>
        /// <param name="implementation">Parent effect implementation</param>
        /// <param name="effectData">Effect data</param>
        public OpenGLEffectShaderGroup(OpenGLEffectImplementation implementation, EffectData effectData)
        {
            _implementation = implementation;

            Name = effectData.EffectName;
            ShaderGroupIndex = 0;

            _program = new OpenGLShaderProgram();
            _program.Attatch(new OpenGLShader(ShaderStage.PixelShader, effectData.PixelShader));
            _program.Attatch(new OpenGLShader(ShaderStage.VertexShader, effectData.VertexShader));
            _program.Compile();
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the index of this shader group in the collection it is contained in.
        /// </summary>
        public int ShaderGroupIndex { get; }

        /// <summary>
        /// Checks if this part belongs to the given effect.
        /// </summary>
        /// <param name="effect">Effect to check against</param>
        /// <returns>True if the effect is the parent of this part, false otherwise.</returns>
        public bool IsPartOf(Effect effect)
        {
            if (effect == null)
            {
                return false;
            }

            return ReferenceEquals(effect.Implementation, _implementation);
        }

        /// <summary>
        /// Binds the set of shaders defined in the group and the resources used by them to the context.
        /// </summary>
        /// <param name="renderContext">Render context to apply to.</param>
        public void Apply(IRenderContext renderContext)
        {
            OpenGLRenderContext oglContext = (OpenGLRenderContext)renderContext;
            oglContext.OpenGLState.UseProgram(_program);
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return _program.ContainsShader(shaderStage);
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            _program.Dispose();

            base.Dispose(isDisposing);
        }
    }
}
