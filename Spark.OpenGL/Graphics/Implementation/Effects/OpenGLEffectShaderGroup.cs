namespace Spark.OpenGL.Graphics.Implementation.Effects
{
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
        private readonly int _programId;
        private readonly Dictionary<ShaderStage, int> _shaders;
        private readonly OpenGLEffectImplementation _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectShaderGroup"/> class
        /// </summary>
        /// <param name="implementation">Parent effect implementation</param>
        /// <param name="effectData">Effect data</param>
        public OpenGLEffectShaderGroup(OpenGLEffectImplementation implementation, EffectData effectData)
        {
            _implementation = implementation;
            _shaders = new Dictionary<ShaderStage, int>();

            Name = effectData.EffectName;
            ShaderGroupIndex = 0;

            _programId = OGL.GL.CreateProgram();

            int pixelShaderId = CreateShader(ShaderStage.PixelShader, effectData.PixelShader);
            _shaders.Add(ShaderStage.PixelShader, pixelShaderId);
            OGL.GL.AttachShader(_programId, pixelShaderId);

            int vertexShaderId = CreateShader(ShaderStage.VertexShader, effectData.VertexShader);
            _shaders.Add(ShaderStage.VertexShader, vertexShaderId);
            OGL.GL.AttachShader(_programId, vertexShaderId);

            CompileProgram();
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
            OGL.GL.UseProgram(_programId);
        }

        /// <summary>
        /// Queries the shader group if it contains a shader used by the specified shader stage.
        /// </summary>
        /// <param name="shaderStage">Shader stage to query.</param>
        /// <returns>True if the group contains a shader that will be bound to the shader stage, false otherwise.</returns>
        public bool ContainsShader(ShaderStage shaderStage)
        {
            return _shaders.ContainsKey(shaderStage);
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

            // TODO: do we need to delete the shaders?

            if (OTK.GraphicsContext.CurrentContext != null && !OTK.GraphicsContext.CurrentContext.IsDisposed)
            {
                OGL.GL.DeleteProgram(_programId);
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderStage"></param>
        /// <param name="shaderSource"></param>
        /// <returns></returns>
        private int CreateShader(ShaderStage shaderStage, string shaderSource)
        {
            OGL.ShaderType shaderType = shaderStage == ShaderStage.PixelShader ? OGL.ShaderType.FragmentShader : OGL.ShaderType.VertexShader;
            int shaderId = OGL.GL.CreateShader(shaderType);

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

            return shaderId;
        }

        /// <summary>
        /// Compiles the shader program
        /// </summary>
        private void CompileProgram()
        {
            OGL.GL.LinkProgram(_programId);
            OGL.GL.GetProgram(_programId, OGL.GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = OGL.GL.GetProgramInfoLog(_programId);
                throw new SparkGraphicsException(infoLog);
            }

            OGL.GL.ValidateProgram(_programId);
            OGL.GL.GetProgram(_programId, OGL.GetProgramParameterName.ValidateStatus, out int validationSuccess);
            if (validationSuccess == 0)
            {
                throw new SparkGraphicsException("Could not validate shader program");
            }
        }
    }
}
