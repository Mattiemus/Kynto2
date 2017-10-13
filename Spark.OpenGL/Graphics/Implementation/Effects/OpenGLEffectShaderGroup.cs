namespace Spark.OpenGL.Graphics.Implementation.Effects
{
    using System.Text;
    using System.Collections.Generic;

    using Spark.Utilities;
    using Spark.Graphics;
    using Spark.Graphics.Effects;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// OpenGL implementation of a shader group
    /// </summary>
    public sealed class OpenGLEffectShaderGroup : BaseDisposable, IEffectShaderGroup
    {
        private readonly OpenGLEffectImplementation _implementation;
        private readonly Dictionary<ShaderStage, int> _programShaders;
        private int _program;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectShaderGroup"/> class
        /// </summary>
        /// <param name="implementation">Parent effect implementation</param>
        /// <param name="effectData">Effect data</param>
        public OpenGLEffectShaderGroup(OpenGLEffectImplementation implementation, EffectData effectData)
        {
            _implementation = implementation;
            _programShaders = new Dictionary<ShaderStage, int>();
            
            Name = effectData.EffectName;
            ShaderGroupIndex = 0;

            Initialize(effectData);

            Parameters = new EffectParameterCollection(EnumerateShaderParameters());
            ConstantBuffers = new EffectConstantBufferCollection();
        }

        /// <summary>
        /// Gets the collection of parameters
        /// </summary>
        internal EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the collection of constant buffers
        /// </summary>
        internal EffectConstantBufferCollection ConstantBuffers { get; }

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
            OpenGLRenderContext oglContext = renderContext as OpenGLRenderContext;

            OGL.GL.UseProgram(_program);
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return _programShaders.ContainsKey(shaderStage);
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
                OGL.GL.DeleteProgram(_program);

                foreach (KeyValuePair<ShaderStage, int> shaderStage in _programShaders)
                {
                    OGL.GL.DeleteShader(shaderStage.Value);
                }
                _programShaders.Clear();
            }
                        
            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Initializes the shader
        /// </summary>
        /// <param name="effectData">Effect data to be compiled</param>
        private void Initialize(EffectData effectData)
        {
            _program = OGL.GL.CreateProgram();
            
            AttatchShader(ShaderStage.PixelShader, effectData.PixelShader);
            AttatchShader(ShaderStage.VertexShader, effectData.VertexShader);
            
            CompileProgram();
        }
        
        /// <summary>
        /// Creates, compiles, and attatches a shader
        /// </summary>
        /// <param name="shaderStage">Shader stage</param>
        /// <param name="shaderSource">Shader source</param>
        /// <returns>Shader resource id</returns>
        private void AttatchShader(ShaderStage shaderStage, string shaderSource)
        {
            int shaderId = OGL.GL.CreateShader(OpenGLHelper.ToNative(shaderStage));

            OGL.GL.ShaderSource(shaderId, shaderSource);
            OGL.GL.CompileShader(shaderId);

            OGL.GL.GetShader(shaderId, OGL.ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus == 0)
            {
                throw new SparkGraphicsException("Could not compile shader");
            }

            string infoLog = OGL.GL.GetShaderInfoLog(shaderId);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new SparkGraphicsException(infoLog);
            }

            OGL.GL.AttachShader(_program, shaderId);
            _programShaders.Add(shaderStage, shaderId);
        }

        /// <summary>
        /// Compiles the shader program
        /// </summary>
        private void CompileProgram()
        {
            OGL.GL.LinkProgram(_program);
            OGL.GL.GetProgram(_program, OGL.GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = OGL.GL.GetProgramInfoLog(_program);
                throw new SparkGraphicsException(infoLog);
            }

            OGL.GL.ValidateProgram(_program);
            OGL.GL.GetProgram(_program, OGL.GetProgramParameterName.ValidateStatus, out int validationSuccess);
            if (validationSuccess == 0)
            {
                throw new SparkGraphicsException("Could not validate shader program");
            }
        }

        /// <summary>
        /// Enumerates all shader parameters
        /// </summary>
        private IEnumerable<OpenGLEffectParameter> EnumerateShaderParameters()
        {
            OGL.GL.GetProgram(_program, OGL.GetProgramParameterName.ActiveUniforms, out int uniformCount);
            OGL.GL.GetProgram(_program, OGL.GetProgramParameterName.ActiveUniformMaxLength, out int maxUniformNameLength);

            for (int i = 0; i < uniformCount; i++)
            {
                StringBuilder uniformName = new StringBuilder(maxUniformNameLength);
                OGL.GL.GetActiveUniform(_program, i, uniformName.Capacity, out int nameLength, out int uniformSize, out OGL.ActiveUniformType uniformType, uniformName);

                yield return new OpenGLEffectParameter(_program, i, uniformSize, uniformType, uniformName.ToString());
            }            
        }
    }
}
