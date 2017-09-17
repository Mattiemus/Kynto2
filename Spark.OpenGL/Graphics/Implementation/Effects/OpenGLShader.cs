namespace Spark.OpenGL.Graphics.Implementation.Effects
{
    using Spark.Graphics;

    using Utilities;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Representation of a shader
    /// </summary>
    public sealed class OpenGLShader : BaseDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShader"/> class
        /// </summary>
        /// <param name="stage">Shader stage</param>
        /// <param name="source">Shader source code</param>
        public OpenGLShader(ShaderStage stage, string source)
        {
            Stage = stage;
            Source = source;

            OGL.ShaderType shaderType = OpenGLHelper.ToNative(stage);
            ShaderId = OGL.GL.CreateShader(shaderType);

            Compile();
        }

        /// <summary>
        /// Gets the shader stage
        /// </summary>
        public ShaderStage Stage { get; }

        /// <summary>
        /// Gets the shader source
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the shader object id
        /// </summary>
        public int ShaderId { get; }

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
                OGL.GL.DeleteShader(ShaderId);
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Compiles the shader
        /// </summary>
        private void Compile()
        {
            OGL.GL.ShaderSource(ShaderId, Source);
            OGL.GL.CompileShader(ShaderId);

            OGL.GL.GetShader(ShaderId, OGL.ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus == 0)
            {
                throw new SparkGraphicsException("Could not compile shader");
            }

            string infoLog = OGL.GL.GetShaderInfoLog(ShaderId);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new SparkGraphicsException(infoLog);
            }
        }
    }
}
