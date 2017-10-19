namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Math;
    using Extensions;

    /// <summary>
    /// Defines a render context which is responsible for generating draw commands for the GPU. A render context contains all the functionality to configure
    /// the programmable pipeline, by setting resources, render states, and targets.
    /// </summary>
    public interface IRenderContext : IDisposable
    {
        /// <summary>
        /// Event for when the render context is in the process of being disposed.
        /// </summary>
        event TypedEventHandler<IRenderContext, EventArgs> Disposing;

        /// <summary>
        /// Gets if the context has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets if the render context is immediate. If false, then it is deferred.
        /// </summary>
        bool IsImmediateContext { get; }

        /// <summary>
        /// Gets the render system that this context belongs to.
        /// </summary>
        IRenderSystem RenderSystem { get; }

        /// <summary>
        /// Gets or sets the blend state. By default, this is <see cref="Spark.Graphics.BlendState.Opaque"/>.
        /// </summary>
        BlendState BlendState { get; set; }

        /// <summary>
        /// Gets or sets the rasterizer state. By default, this is <see cref="Spark.Graphics.RasterizerState.CullBackClockwiseFront"/>.
        /// </summary>
        RasterizerState RasterizerState { get; set; }

        /// <summary>
        /// Gets or sets the depth stencil state. By default, this is <see cref="Spark.Graphics.DepthStencilState.Default"/>.
        /// </summary>
        DepthStencilState DepthStencilState { get; set; }

        /// <summary>
        /// Gets or sets the currently enforced render state. If a state is enforced, then the currently active one is preserved and subsequent state setting
        /// is filtered.
        /// </summary>
        EnforcedRenderState EnforcedRenderState { get; set; }

        /// <summary>
        /// Gets or sets the rectangle used for scissor testing, if it is enabled.
        /// </summary>
        Rectangle ScissorRectangle { get; set; }

        /// <summary>
        /// Gets or sets the blend factor which is a constant color used for alpha blending. By default, this value is <see cref="Color.White"/>. This is
        /// a "high frequency" render state and setting a blend state to the context will also set this value.
        /// </summary>
        Color BlendFactor { get; set; }

        /// <summary>
        /// Gets or sets the bitmask which defines which samples can be written during multisampling. By default, this value is -1 (0xffffffff). This is
        /// a "high frequency" render state and setting a blend state to the context will also set this value.
        /// </summary>
        int BlendSampleMask { get; set; }

        /// <summary>
        /// Gets or sets the reference value for stencil testing. By default, this value is 0. This is a "high frequency" render state and setting a 
        /// depth stencil state to the context will also set this value.
        /// </summary>
        int ReferenceStencil { get; set; }

        /// <summary>
        /// Gets or sets the currently active camera. The camera controls the viewport which identifies the portion of the 
        /// currently bound render target which is being rendered to.
        /// </summary>
        Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the currently active backbuffer (swapchain). If render targets are set to null, the backbuffer is set as the current render target.
        /// </summary>
        SwapChain BackBuffer { get; set; }

        /// <summary>
        /// Gets the shader stage corresponding to the enumeration type. If not supported, this will return null. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <param name="shaderStage">Shader stage type</param>
        /// <returns>The shader stage, if it exists. Otherwise null.</returns>
        IShaderStage GetShaderStage(ShaderStage shaderStage);

        /// <summary>
        /// Gets all supported shader stages. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <returns>All supported shader stages.</returns>
        IEnumerable<IShaderStage> GetShaderStages();

        /// <summary>
        /// Queries if the specified shader stage type is supported or not. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <param name="shaderStage">Shader stage type</param>
        /// <returns>True if the shader stage is supported, false otherwise.</returns>
        bool IsShaderStageSupported(ShaderStage shaderStage);

        /// <summary>
        /// Gets a render extension of the specified type. Render extensions extend the functionality of a render context with platform-specific
        /// functionality which are not supported by the engine core.
        /// </summary>
        /// <typeparam name="T">Type of render extension.</typeparam>
        /// <returns>The render extension, if registered, otherwise null.</returns>
        T GetExtension<T>() where T : IRenderContextExtension;

        /// <summary>
        /// Gets all supported render extensions. Render extensions extend the functionality of a render context with platform-specific
        /// functionality which are not supported by the engine core.
        /// </summary>
        /// <returns>All supported render extensions.</returns>
        IEnumerable<IRenderContextExtension> GetExtensions();

        /// <summary>
        /// Queries if render extension of the specified type is supported or not. Render extensions extend the functionality of a render context with 
        /// platform-specific functionality which are not supported by the engine core.
        /// </summary>
        /// <typeparam name="T">Type of render extension.</typeparam>
        /// <returns>True if the extension is supported, false otherwise.</returns>
        bool IsExtensionSupported<T>() where T : IRenderContextExtension;

        /// <summary>
        /// Executes the command list by playbacking the recorded GPU commands contained in the list.
        /// </summary>
        /// <param name="commandList">Command list to execute</param>
        /// <param name="restoreImmediateContextState">True if the render context state should be preserved or not. If true, the context state is saved and then restored
        /// after playback. Typically this is set to false to prevent unnecessary state setting. If false, the context state returns to the defautl state (e.g. as if ClearState was called).</param>
        void ExecuteCommandList(ICommandList commandList, bool restoreImmediateContextState);

        /// <summary>
        /// Binds the index buffer to the render context. A value of null will unbind the currently bound index buffer.
        /// </summary>
        /// <param name="indexBuffer">Index buffer to bind.</param>
        void SetIndexBuffer(IndexBuffer indexBuffer);

        /// <summary>
        /// Binds the specified vertex buffer to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffer">Vertex buffer to bind.</param>
        void SetVertexBuffer(VertexBufferBinding vertexBuffer);

        /// <summary>
        /// Binds the specified number of vertex buffers, starting at the first slot. Any remaining slots are set to null. A value of null
        /// will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffers">Vertexbuffers to bind.</param>
        void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers);

        /// <summary>
        /// Binds the specified stream output buffer to the first slot and the remaining slots are set to null. A stream output buffer cannot be bound 
        /// as both input and output at the same time. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="streamOutputBuffer">Stream output buffer to bind.</param>
        void SetStreamOutputTarget(StreamOutputBufferBinding streamOutputBuffer);

        /// <summary>
        /// Binds the specified number of stream output buffers, starting at the first slot. Any remaining slots are set to null. A stream output buffer cannot
        /// be bound as both input and output at the same time. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="streamOutputBuffers">Stream output buffers to bind.</param>
        void SetStreamOutputTargets(params StreamOutputBufferBinding[] streamOutputBuffers);

        /// <summary>
        /// Binds the specified render target to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound
        /// render targets.
        /// </summary>
        /// <param name="options">Options when setting the render target.</param>
        /// <param name="renderTarget">Render target to bind.</param>
        void SetRenderTarget(SetTargetOptions options, IRenderTarget renderTarget);

        /// <summary>
        /// Binds the specified number of render targets, starting at the first slot. Any remaining slots are set to null. A render target cannot be bound
        /// as both input and output at the same time. A value of null will unbind all currently
        /// bound render targets.
        /// </summary>
        /// <param name="options">Options when setting the render targets.</param>
        /// <param name="renderTargets">Render targets to bind.</param>
        void SetRenderTargets(SetTargetOptions options, params IRenderTarget[] renderTargets);

        /// <summary>
        /// Gets the currently bound index buffer, or null if one is not bound
        /// </summary>
        /// <returns>Currently bound index buffer.</returns>
        IndexBuffer GetIndexBuffer();

        /// <summary>
        /// Gets the currently bound vertex buffers.
        /// </summary>
        /// <returns>Currently bound vertex buffers.</returns>
        VertexBufferBinding[] GetVertexBuffers();

        /// <summary>
        /// Gets the currently bound stream output buffers.
        /// </summary>
        /// <returns>Currently bound stream output buffers.</returns>
        StreamOutputBufferBinding[] GetStreamOutputTargets();

        /// <summary>
        /// Gets the currently bound render targets.
        /// </summary>
        /// <returns>Currently bound render targets.</returns>
        IRenderTarget[] GetRenderTargets();

        /// <summary>
        /// Clears the state of the context to the default. This includes all render states, bound resources and targets, and other properties.
        /// </summary>
        void ClearState();

        /// <summary>
        /// Clears all bounded render targets to the specified color.
        /// </summary>
        /// <param name="color">Color to clear to.</param>
        void Clear(Color color);

        /// <summary>
        /// Clears all bounded render targets and depth buffer.
        /// </summary>
        /// <param name="options">Clear options specifying which buffer to clear.</param>
        /// <param name="color">Color to clear to</param>
        /// <param name="depth">Depth value to clear to</param>
        /// <param name="stencil">Stencil value to clear to</param>
        void Clear(ClearOptions options, Color color, float depth, int stencil);

        /// <summary>
        /// Draws non-indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCount">Number of vertices to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        void Draw(PrimitiveType primitiveType, int vertexCount, int startVertexIndex);

        /// <summary>
        /// Draws indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="indexCount">Number of indices to draw.</param>
        /// <param name="startIndex">Starting index in the index buffer at which to read vertex indices from.</param>
        /// <param name="baseVertexOffset">Offset to add to each index before reading a vertex from a vertex buffer.</param>
        void DrawIndexed(PrimitiveType primitiveType, int indexCount, int startIndex, int baseVertexOffset);

        /// <summary>
        /// Draws indexed, instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="indexCountPerInstance">Number of indices per instance to draw.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startIndex">Starting index in the index buffer at which to read vertex indices from.</param>
        /// <param name="baseVertexOffset">Offset to add to each index before reading a vertex from a vertex buffer.</param>
        /// <param name="startInstanceOffset">Offset to add to each index before reading per-instance data from a vertex buffer.</param>
        void DrawIndexedInstanced(PrimitiveType primitiveType, int indexCountPerInstance, int instanceCount, int startIndex, int baseVertexOffset, int startInstanceOffset);

        /// <summary>
        /// Draws non-indexed, instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCountPerInstance">Number of vertices per instance to draw.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        /// <param name="startInstanceOffset">Offset to add to each index before reading per-instance data from a vertex buffer.</param>
        void DrawInstanced(PrimitiveType primitiveType, int vertexCountPerInstance, int instanceCount, int startVertexIndex, int startInstanceOffset);

        /// <summary>
        /// Draws geometry of an unknown size that has been streamed out.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        void DrawAuto(PrimitiveType primitiveType);

        /// <summary>
        /// Sends queued up commands in the command buffer to the GPU.
        /// </summary>
        void Flush();
    }

    /// <summary>
    /// Extensions methods to the <see cref="IRenderContext"/> interface.
    /// </summary>
    public static class RenderContextExtensions
    {
        /// <summary>
        /// Binds the specified render target to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound
        /// render targets.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="renderTarget">Render target to bind.</param>
        public static void SetRenderTarget(this IRenderContext context, IRenderTarget renderTarget)
        {
            context.SetRenderTarget(SetTargetOptions.None, renderTarget);
        }

        /// <summary>
        /// Binds the specified number of render targets, starting at the first slot. Any remaining slots are set to null. A render target cannot be bound
        /// as both input and output at the same time. A value of null will unbind all currently
        /// bound render targets.
        /// </summary>
        /// <param name="context">Render context.</param>
        /// <param name="renderTargets">Render targets to bind.</param>
        public static void SetRenderTargets(this IRenderContext context, params IRenderTarget[] renderTargets)
        {
            context.SetRenderTargets(SetTargetOptions.None, renderTargets);
        }
    }
}
