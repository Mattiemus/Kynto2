namespace Spark.Graphics
{
    using System;

    using Graphics.Implementation;

    /// <summary>
    /// Represents a buffer of vertices that exists on the GPU. Unlike a regular vertex buffer, this data is streamed out from the geometry shader stage (or vertex shader
    /// stage if the geometry shader stage is inactive). The streamed out vertex data then can be bound as input to the graphics pipeline, in a subsequent rendering pass.
    /// </summary>
    public class StreamOutputBuffer : VertexBuffer, IShaderResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamOutputBuffer"/> class.
        /// </summary>
        protected StreamOutputBuffer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamOutputBuffer"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="vertexLayout">Vertex layout that defines the vertex data this buffer will contain</param>
        /// <param name="vertexCount">Number of vertices the buffer will contain</param>
        public StreamOutputBuffer(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
        {
            CreateImplementation(renderSystem, vertexLayout, vertexCount);
        }

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public ShaderResourceType ResourceType => ShaderResourceType.Buffer;

        /// <summary>
        /// Gets or sets the stream output buffer implementation
        /// </summary>
        private IStreamOutputBufferImplementation StreamOutputBufferImplementation
        {
            get => Implementation as IStreamOutputBufferImplementation;
            set => BindImplementation(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderSystem"></param>
        /// <param name="vertexLayout"></param>
        /// <param name="vertexCount"></param>
        private void CreateImplementation(IRenderSystem renderSystem, VertexLayout vertexLayout, int vertexCount)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            ValidateCreationParameters(vertexLayout, vertexCount);
            
            if (!renderSystem.TryGetImplementationFactory(out IStreamOutputBufferImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                StreamOutputBufferImplementation = factory.CreateImplementation(vertexLayout, vertexCount);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }
    }
}
