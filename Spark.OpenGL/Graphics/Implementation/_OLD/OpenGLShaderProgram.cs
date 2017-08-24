namespace Spark.Graphics.Implementation
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// A shader program is a group of shaders
    /// </summary>
    public sealed class OpenGLShaderProgram : OpenGLGraphicsResource
    {
        private readonly List<OpenGLShader> _shaders = new List<OpenGLShader>();
        private readonly Dictionary<string, OpenGLShaderUniform> _uniforms = new Dictionary<string, OpenGLShaderUniform>();
        private readonly Dictionary<string, OpenGLShaderAttribute> _attribtes = new Dictionary<string, OpenGLShaderAttribute>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLShaderProgram"/> class.
        /// </summary>
        /// <param name="shaders">List of shaders that make up the program</param>
        public OpenGLShaderProgram(params OpenGLShader[] shaders)
        {
            ResourceId = OGL.GL.CreateProgram();
            
            foreach (OpenGLShader shader in shaders)
            {
                AttatchShader(shader);
            }

            Link();
            Validate();

            CreateAttributes();
            CreateUniforms();
        }

        /// <summary>
        /// Gets the list of shaders that make up the program
        /// </summary>
        public IReadOnlyList<OpenGLShader> Shaders => _shaders;

        /// <summary>
        /// Gets the dictionary of shader uniforms
        /// </summary>
        public IReadOnlyDictionary<string, OpenGLShaderUniform> Uniforms => _uniforms;

        /// <summary>
        /// Gets the dictionary of shader attributes
        /// </summary>
        public IReadOnlyDictionary<string, OpenGLShaderAttribute> Attributes => _attribtes;

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

        /// <summary>
        /// Attatch a shader to the program
        /// </summary>
        /// <param name="shader">Shader to attatch</param>
        private void AttatchShader(OpenGLShader shader)
        {
            ThrowIfDisposed();

            _shaders.Add(shader);
            OGL.GL.AttachShader(ResourceId, shader.ResourceId);
        }

        /// <summary>
        /// Links the shader program
        /// </summary>
        private void Link()
        {
            ThrowIfDisposed();
            
            OGL.GL.LinkProgram(ResourceId);
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = OGL.GL.GetProgramInfoLog(ResourceId);
                throw new SparkGraphicsException(infoLog);
            }
        }

        /// <summary>
        /// Validates the shader program
        /// </summary>
        private void Validate()
        {
            ThrowIfDisposed();
            
            OGL.GL.ValidateProgram(ResourceId);
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.ValidateStatus, out int validationSuccess);
            if (validationSuccess == 0)
            {
                throw new SparkGraphicsException("Could not validate shader program");
            }
        }

        /// <summary>
        /// Reads the shader attributes
        /// </summary>
        private void CreateAttributes()
        {
            ThrowIfDisposed();
            
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.ActiveAttributeMaxLength, out int maxLength);
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.ActiveAttributes, out int attributeCount);
            for (int i = 0; i < attributeCount; i++)
            {
                StringBuilder name = new StringBuilder(maxLength);
                OGL.GL.GetActiveAttrib(ResourceId, i, name.Capacity, out int length, out int size, out OGL.ActiveAttribType attribType, name);

                int location = OGL.GL.GetAttribLocation(ResourceId, name.ToString());

                OpenGLShaderAttribute attribute = new OpenGLShaderAttribute(location, name.ToString(), attribType, size);
                _attribtes.Add(attribute.Name, attribute);
            }
        }

        /// <summary>
        /// Reads the shader uniforms
        /// </summary>
        private void CreateUniforms()
        {
            ThrowIfDisposed();
            
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.ActiveUniformMaxLength, out int maxLength);
            OGL.GL.GetProgram(ResourceId, OGL.GetProgramParameterName.ActiveUniforms, out int uniformCount);
            for (int i = 0; i < uniformCount; i++)
            {
                StringBuilder name = new StringBuilder(maxLength);
                OGL.GL.GetActiveUniform(ResourceId, i, name.Capacity, out int length, out int size, out OGL.ActiveUniformType uniformType, name);

                int location = OGL.GL.GetUniformLocation(ResourceId, name.ToString());

                OpenGLShaderUniform uniform = new OpenGLShaderUniform(location, name.ToString(), uniformType, size);
                _uniforms.Add(uniform.Name, uniform);
            }
        }
    }
}
