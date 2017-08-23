namespace Spark.Graphics.Implementation
{
    using System;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Representation of a shader
    /// </summary>
    public sealed class OpenGLShader : OpenGLGraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShader"/> class
        /// </summary>
        /// <param name="shaderType">Shader type</param>
        /// <param name="source">Shader source code</param>
        public OpenGLShader(OGL.ShaderType shaderType, string source)
        {
            ResourceId = OGL.GL.CreateShader(shaderType);
            Compile(source);
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
                OGL.GL.DeleteShader(ResourceId);
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Compiles the shader
        /// </summary>
        /// <param name="source"></param>
        private void Compile(string source)
        {
            ThrowIfDisposed();
            
            OGL.GL.ShaderSource(ResourceId, source);
            OGL.GL.CompileShader(ResourceId);

            OGL.GL.GetShader(ResourceId, OGL.ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus == 0)
            {
                throw new Exception("Could not compile shader");
            }

            string infoLog = OGL.GL.GetShaderInfoLog(ResourceId);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception(infoLog);
            }
        }
    }
}
