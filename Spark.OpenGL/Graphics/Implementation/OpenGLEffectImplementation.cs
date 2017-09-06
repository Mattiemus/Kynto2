namespace Spark.OpenGL.Graphics.Implementation
{
    using System;

    using Spark.Graphics;
    using Spark.Graphics.Implementation;

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
        /// Occurs when an effect shader group that is contained by this effect is about to be applied to a render context.
        /// </summary>
        public event OnApplyDelegate OnShaderGroupApply;

        /// <summary>
        /// Gets the effect sort key, used to compare effects as a first step in sorting objects to render. The sort key is the same
        /// for cloned effects. Further sorting can then be done using the indices of the contained techniques, and the indices of their passes.
        /// </summary>
        public int SortKey { get; }

        /// <summary>
        /// Gets or sets the currently active shader group.
        /// </summary>
        public IEffectShaderGroup CurrentShaderGroup { get; set; }

        /// <summary>
        /// Gets the shader groups contained in this effect.
        /// </summary>
        public EffectShaderGroupCollection ShaderGroups { get; }

        /// <summary>
        /// Gets all effect parameters used by all shader groups.
        /// </summary>
        public EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets all constant buffers that contain all value type parameters used by all shader groups.
        /// </summary>
        public EffectConstantBufferCollection ConstantBuffers { get; }

        /// <summary>
        /// Gets the compiled effect byte code that represents this effect.
        /// </summary>
        public byte[] EffectByteCode { get; }

        /// <summary>
        /// Clones the effect, and possibly sharing relevant underlying resources. Cloned instances are guaranteed to be
        /// completely separate from the source effect in terms of parameters, changing one will not change the other. But unlike
        /// creating a new effect from the same compiled byte code, native resources can still be shared more effectively.
        /// </summary>
        /// <returns>Cloned effect implementation.</returns>
        public IEffectImplementation Clone()
        {
            throw new NotImplementedException();
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
