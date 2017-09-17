namespace Spark.OpenGL.Graphics.Implementation.Effects
{
    using System.Collections.Generic;

    using Spark.Graphics;

    using Utilities;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// A shader program is a linked set of OpenGL shaders
    /// </summary>
    public sealed class OpenGLShaderProgram : BaseDisposable
    {
        private readonly Dictionary<ShaderStage, OpenGLShader> _shaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShaderProgram"/> class
        /// </summary>
        public OpenGLShaderProgram()
        {
            _shaders = new Dictionary<ShaderStage, OpenGLShader>();

            ProgramId = OGL.GL.CreateProgram();
        }

        /// <summary>
        /// Gets the shader program object id
        /// </summary>
        public int ProgramId { get; }

        /// <summary>
        /// Queries the program if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the program contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return _shaders.ContainsKey(shaderStage);
        }

        /// <summary>
        /// Attatches a shader to the program
        /// </summary>
        /// <param name="shader">Shader to attatch</param>
        public void Attatch(OpenGLShader shader)
        {
            _shaders.Add(shader.Stage, shader);
            OGL.GL.AttachShader(ProgramId, shader.ShaderId);
        }

        /// <summary>
        /// Compiles the shader program
        /// </summary>
        public void Compile()
        {
            OGL.GL.LinkProgram(ProgramId);
            OGL.GL.GetProgram(ProgramId, OGL.GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = OGL.GL.GetProgramInfoLog(ProgramId);
                throw new SparkGraphicsException(infoLog);
            }

            OGL.GL.ValidateProgram(ProgramId);
            OGL.GL.GetProgram(ProgramId, OGL.GetProgramParameterName.ValidateStatus, out int validationSuccess);
            if (validationSuccess == 0)
            {
                throw new SparkGraphicsException("Could not validate shader program");
            }
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
                OGL.GL.DeleteProgram(ProgramId);
            }

            base.Dispose(isDisposing);
        }
    }
}
