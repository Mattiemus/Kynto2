namespace Spark.Graphics.Implementation
{
    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Effect underlying implementation
    /// </summary>
    public sealed class OpenGLEffectImplementation : GraphicsResourceImplementation, IEffectImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectImplementation"/> class
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="shaderByteCode">Compiled shader byte code.</param>
        public OpenGLEffectImplementation(OpenGLRenderSystem renderSystem, byte[] shaderByteCode)
            : base(renderSystem, OGL.GL.CreateProgram())
        {
            EffectByteCode = shaderByteCode;
        }

        /// <summary>
        /// Gets all effect parameters contained in this effect.
        /// </summary>
        public EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the compiled effect byte code that represents this effect.
        /// </summary>
        public byte[] EffectByteCode { get; }

        /// <summary>
        /// Binds the set of shaders defined in the group and the resources used by them to the context.
        /// </summary>
        /// <param name="renderContext">Render context to apply to.</param>
        public void Apply(IRenderContext renderContext)
        {
            // TODO
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return false;
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

            if (OTK.GraphicsContext.CurrentContext != null && !OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                OGL.GL.DeleteProgram(ResourceId);
            }

            base.Dispose(isDisposing);
        }
    }
}
