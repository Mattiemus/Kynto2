namespace Spark.Graphics
{
    using System;

    using Math;

    /// <summary>
    /// Defines a render context which is responsible for generating draw commands for the GPU. A render context contains all the functionality to configure
    /// the programmable pipeline, by setting resources, render states, and targets.
    /// </summary>
    public interface IRenderContext : IDisposable
    {
        /// <summary>
        /// Binds the specified vertex buffer to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffer">Vertex buffer to bind.</param>
        void SetVertexBuffer(VertexBufferBinding vertexBuffer);

        /// <summary>
        /// Clears all bounded render targets to the specified color
        /// </summary>
        /// <param name="color">Color to clear to</param>
        void Clear(LinearColor color);

        /// <summary>
        /// Clears all bounded render targets and depth buffer.
        /// </summary>
        /// <param name="options">Clear options specifying which buffer to clear.</param>
        /// <param name="color">Color to clear to</param>
        /// <param name="depth">Depth value to clear to</param>
        /// <param name="stencil">Stencil value to clear to</param>
        void Clear(ClearOptions options, LinearColor color, float depth, int stencil);

        /// <summary>
        /// Draws non-indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCount">Number of vertices to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        void Draw(PrimitiveType primitiveType, int vertexCount, int startVertexIndex);
    }
}
