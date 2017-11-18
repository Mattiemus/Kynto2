namespace Spark.OpenGL.Graphics
{
    using System;
    using System.Collections.Generic;

    using Spark.Graphics;
    using Spark.Utilities;
    
    using Math;
    using Implementation;

    using OTK = OpenTK.Graphics;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Render context for the OpenGL renderer
    /// </summary>
    public sealed class OpenGLRenderContext : BaseDisposable, IRenderContext
    {
        private BlendState _blendState;
        private RasterizerState _rasterizerState;
        private DepthStencilState _depthStencilState;
        private Rectangle _scissorRectangle;
        private Color _blendFactor;
        private int _referenceStencil;
        private int _blendSampleMask;
        private Camera _camera;

        private readonly int _vao;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLRenderContext"/> class.
        /// </summary>
        /// <param name="renderSystem">Parent render context</param>
        public OpenGLRenderContext(OpenGLRenderSystem renderSystem)
        {
            OpenGLRenderSystem = renderSystem;
            OpenGLState = new OpenGLState();

            _vao = OGL.GL.GenVertexArray();
            OGL.GL.BindVertexArray(_vao);

            SetDefaultRenderStates();
        }

        /// <summary>
        /// Gets the parent render system
        /// </summary>
        internal OpenGLRenderSystem OpenGLRenderSystem { get; }

        /// <summary>
        /// Gets the OpenGL state manager for this context
        /// </summary>
        internal OpenGLState OpenGLState { get; }

        /// <summary>
        /// Event for when the render context is in the process of being disposed.
        /// </summary>
        public event TypedEventHandler<IRenderContext, EventArgs> Disposing;

        /// <summary>
        /// Gets if the render context is immediate. If false, then it is deferred.
        /// </summary>
        public bool IsImmediateContext => true;

        /// <summary>
        /// Gets the render system that this context belongs to.
        /// </summary>
        public IRenderSystem RenderSystem => OpenGLRenderSystem;

        /// <summary>
        /// Gets or sets the blend state. By default, this is <see cref="Spark.Graphics.BlendState.Opaque"/>.
        /// </summary>
        public BlendState BlendState
        {
            get => _blendState;
            set
            {
                // If enforced, filter out
                if (EnforcedRenderState.HasFlag(EnforcedRenderState.BlendState))
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = OpenGLRenderSystem.PredefinedBlendStates.Opaque;
                }

                // If a new state, apply it
                if (!value.IsSameState(_blendState))
                {
                    if (!value.IsBound)
                    {
                        value.BindRenderState();
                    }

                    _blendState = value;
                    _blendFactor = value.BlendFactor;
                    _blendSampleMask = value.MultiSampleMask;
                                        
                    OpenGLBlendStateImplementation nativeState = value.Implementation as OpenGLBlendStateImplementation;
                    nativeState.ApplyState(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the rasterizer state. By default, this is <see cref="Spark.Graphics.RasterizerState.CullBackClockwiseFront"/>.
        /// </summary>
        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                // If enforced, filter out
                if (EnforcedRenderState.HasFlag(EnforcedRenderState.RasterizerState))
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = OpenGLRenderSystem.PredefinedRasterizerStates.CullBackClockwiseFront;
                }

                // If a new state, apply it
                if (!value.IsSameState(_rasterizerState))
                {
                    if (!value.IsBound)
                    {
                        value.BindRenderState();
                    }

                    _rasterizerState = value;

                    OpenGLRasterizerStateImplementation nativeState = value.Implementation as OpenGLRasterizerStateImplementation;
                    nativeState.ApplyState(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the depth stencil state. By default, this is <see cref="Spark.Graphics.DepthStencilState.Default"/>.
        /// </summary>
        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                // If enforced, filter out
                if (EnforcedRenderState.HasFlag(EnforcedRenderState.DepthStencilState))
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = OpenGLRenderSystem.PredefinedDepthStencilStates.Default;
                }

                // If a new state, apply it
                if (!value.IsSameState(_depthStencilState))
                {
                    if (!value.IsBound)
                    {
                        value.BindRenderState();
                    }

                    _depthStencilState = value;
                    _referenceStencil = _depthStencilState.ReferenceStencil;

                    OpenGLDepthStencilStateImplementation nativeState = value.Implementation as OpenGLDepthStencilStateImplementation;
                    nativeState.ApplyState(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently enforced render state. If a state is enforced, then the currently active one is preserved and subsequent state setting
        /// is filtered.
        /// </summary>
        public EnforcedRenderState EnforcedRenderState { get; set; }

        /// <summary>
        /// Gets or sets the rectangle used for scissor testing, if it is enabled.
        /// </summary>
        public Rectangle ScissorRectangle
        {
            get => _scissorRectangle;
            set
            {
                if (value.Equals(ref _scissorRectangle))
                {
                    return;
                }

                _scissorRectangle = value;
                OGL.GL.Scissor(_scissorRectangle.X, _scissorRectangle.Y, _scissorRectangle.Width, _scissorRectangle.Height);
            }
        }

        /// <summary>
        /// Gets or sets the blend factor which is a constant color used for alpha blending. By default, this value is <see cref="Color.White"/>. This is
        /// a "high frequency" render state and setting a blend state to the context will also set this value.
        /// </summary>
        public Color BlendFactor
        {
            get => _blendFactor;
            set
            {
                if (value.Equals(ref _blendFactor))
                {
                    return;
                }

                _blendFactor = value;

                Vector4 blendColor = value.ToVector4();
                OGL.GL.BlendColor(blendColor.X, blendColor.Y, blendColor.Z, blendColor.W);
            }
        }

        /// <summary>
        /// Gets or sets the bitmask which defines which samples can be written during multisampling. By default, this value is -1 (0xffffffff). This is
        /// a "high frequency" render state and setting a blend state to the context will also set this value.
        /// </summary>
        public int BlendSampleMask
        {
            get => _blendSampleMask;
            set
            {
                if (value == _blendSampleMask)
                {
                    return;
                }

                _blendSampleMask = value;

                OGL.GL.SampleMask(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the reference value for stencil testing. By default, this value is 0. This is a "high frequency" render state and setting a 
        /// depth stencil state to the context will also set this value.
        /// </summary>
        public int ReferenceStencil
        {
            get => _referenceStencil;
            set
            {
                if (value == _referenceStencil)
                {
                    return;
                }

                _referenceStencil = value;

                OGL.GL.StencilMask(_referenceStencil);
            }
        }

        /// <summary>
        /// Gets or sets the currently active camera. The camera controls the viewport which identifies the portion of the 
        /// currently bound render target which is being rendered to.
        /// </summary>
        public Camera Camera
        {
            get => _camera;
            set
            {
                if (_camera == value)
                {
                    return;
                }

                StopCameraEvents();

                _camera = value;

                StartCameraEvents();
            }
        }

        /// <summary>
        /// Gets or sets the currently active backbuffer (swapchain). If render targets are set to null, the backbuffer is set as the current render target.
        /// </summary>
        public SwapChain BackBuffer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                // TODO
            }
        }

        /// <summary>
        /// Gets the shader stage corresponding to the enumeration type. If not supported, this will return null. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <param name="shaderStage">Shader stage type</param>
        /// <returns>The shader stage, if it exists. Otherwise null.</returns>
        public IShaderStage GetShaderStage(ShaderStage shaderStage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all supported shader stages. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <returns>All supported shader stages.</returns>
        public IEnumerable<IShaderStage> GetShaderStages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries if the specified shader stage type is supported or not. A shader stage manages
        /// resources that can be bound to that particular stage in the pipeline.
        /// </summary>
        /// <param name="shaderStage">Shader stage type</param>
        /// <returns>True if the shader stage is supported, false otherwise.</returns>
        public bool IsShaderStageSupported(ShaderStage shaderStage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a render extension of the specified type. Render extensions extend the functionality of a render context with platform-specific
        /// functionality which are not supported by the engine core.
        /// </summary>
        /// <typeparam name="T">Type of render extension.</typeparam>
        /// <returns>The render extension, if registered, otherwise null.</returns>
        public T GetExtension<T>() where T : IRenderContextExtension
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all supported render extensions. Render extensions extend the functionality of a render context with platform-specific
        /// functionality which are not supported by the engine core.
        /// </summary>
        /// <returns>All supported render extensions.</returns>
        public IEnumerable<IRenderContextExtension> GetExtensions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries if render extension of the specified type is supported or not. Render extensions extend the functionality of a render context with 
        /// platform-specific functionality which are not supported by the engine core.
        /// </summary>
        /// <typeparam name="T">Type of render extension.</typeparam>
        /// <returns>True if the extension is supported, false otherwise.</returns>
        public bool IsExtensionSupported<T>() where T : IRenderContextExtension
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command list by playbacking the recorded GPU commands contained in the list.
        /// </summary>
        /// <param name="commandList">Command list to execute</param>
        /// <param name="restoreImmediateContextState">True if the render context state should be preserved or not. If true, the context state is saved and then restored
        /// after playback. Typically this is set to false to prevent unnecessary state setting. If false, the context state returns to the defautl state (e.g. as if ClearState was called).</param>
        public void ExecuteCommandList(ICommandList commandList, bool restoreImmediateContextState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds the index buffer to the render context. A value of null will unbind the currently bound index buffer.
        /// </summary>
        /// <param name="indexBuffer">Index buffer to bind.</param>
        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            OpenGLIndexBufferImplementation oglIndexBuffer = indexBuffer.Implementation as OpenGLIndexBufferImplementation;

            OGL.GL.BindBuffer(OGL.BufferTarget.ElementArrayBuffer, oglIndexBuffer.OpenGLBufferId);
        }

        /// <summary>
        /// Binds the specified vertex buffer to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffer">Vertex buffer to bind.</param>
        public void SetVertexBuffer(VertexBufferBinding vertexBuffer)
        {
            OpenGLVertexBufferImplementation oglVertexBuffer = vertexBuffer.VertexBuffer.Implementation as OpenGLVertexBufferImplementation;

            OGL.GL.BindBuffer(OGL.BufferTarget.ArrayBuffer, oglVertexBuffer.OpenGLBufferId);

            // TODO: base this off the vertex buffer layout
            OGL.GL.EnableVertexAttribArray(0);
            OGL.GL.VertexAttribPointer(0, 3, OGL.VertexAttribPointerType.Float, false, 0, 0);
        }

        /// <summary>
        /// Binds the specified number of vertex buffers, starting at the first slot. Any remaining slots are set to null. A value of null
        /// will unbind all currently bound buffers.
        /// </summary>
        /// <param name="vertexBuffers">Vertexbuffers to bind.</param>
        public void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds the specified stream output buffer to the first slot and the remaining slots are set to null. A stream output buffer cannot be bound 
        /// as both input and output at the same time. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="streamOutputBuffer">Stream output buffer to bind.</param>
        public void SetStreamOutputTarget(StreamOutputBufferBinding streamOutputBuffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds the specified number of stream output buffers, starting at the first slot. Any remaining slots are set to null. A stream output buffer cannot
        /// be bound as both input and output at the same time. A value of null will unbind all currently bound buffers.
        /// </summary>
        /// <param name="streamOutputBuffers">Stream output buffers to bind.</param>
        public void SetStreamOutputTargets(params StreamOutputBufferBinding[] streamOutputBuffers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds the specified render target to the first slot and the remaining slots are set to null. A value of null will unbind all currently bound
        /// render targets.
        /// </summary>
        /// <param name="options">Options when setting the render target.</param>
        /// <param name="renderTarget">Render target to bind.</param>
        public void SetRenderTarget(SetTargetOptions options, IRenderTarget renderTarget)
        {
            // TODO
        }

        /// <summary>
        /// Binds the specified number of render targets, starting at the first slot. Any remaining slots are set to null. A render target cannot be bound
        /// as both input and output at the same time. A value of null will unbind all currently
        /// bound render targets.
        /// </summary>
        /// <param name="options">Options when setting the render targets.</param>
        /// <param name="renderTargets">Render targets to bind.</param>
        public void SetRenderTargets(SetTargetOptions options, params IRenderTarget[] renderTargets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the currently bound index buffer, or null if one is not bound
        /// </summary>
        /// <returns>Currently bound index buffer.</returns>
        public IndexBuffer GetIndexBuffer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the currently bound vertex buffers.
        /// </summary>
        /// <returns>Currently bound vertex buffers.</returns>
        public VertexBufferBinding[] GetVertexBuffers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the currently bound stream output buffers.
        /// </summary>
        /// <returns>Currently bound stream output buffers.</returns>
        public StreamOutputBufferBinding[] GetStreamOutputTargets()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the currently bound render targets.
        /// </summary>
        /// <returns>Currently bound render targets.</returns>
        public IRenderTarget[] GetRenderTargets()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the state of the context to the default. This includes all render states, bound resources and targets, and other properties.
        /// </summary>
        public void ClearState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all bounded render targets to the specified color.
        /// </summary>
        /// <param name="color">Color to clear to.</param>
        public void Clear(Color color)
        {
            Clear(ClearOptions.All, color, 1.0f, 0);
        }

        /// <summary>
        /// Clears all bounded render targets and depth buffer.
        /// </summary>
        /// <param name="options">Clear options specifying which buffer to clear.</param>
        /// <param name="color">Color to clear to</param>
        /// <param name="depth">Depth value to clear to</param>
        /// <param name="stencil">Stencil value to clear to</param>
        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            OGL.ClearBufferMask nativeClearMask = OpenGLHelper.ToNative(options);
            Vector4 clearColor = color.ToVector4();

            OGL.GL.ClearColor(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
            OGL.GL.ClearDepth(depth);
            OGL.GL.ClearStencil(stencil);
            OGL.GL.Clear(nativeClearMask);
        }

        /// <summary>
        /// Draws non-indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCount">Number of vertices to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        public void Draw(PrimitiveType primitiveType, int vertexCount, int startVertexIndex)
        {
            OGL.PrimitiveType nativePrimType = OpenGLHelper.ToNative(primitiveType);
            OGL.GL.DrawArrays(nativePrimType, vertexCount, startVertexIndex);
        }

        /// <summary>
        /// Draws indexed, non-instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="indexCount">Number of indices to draw.</param>
        /// <param name="startIndex">Starting index in the index buffer at which to read vertex indices from.</param>
        /// <param name="baseVertexOffset">Offset to add to each index before reading a vertex from a vertex buffer.</param>
        public void DrawIndexed(PrimitiveType primitiveType, int indexCount, int startIndex, int baseVertexOffset)
        {
            OGL.PrimitiveType nativePrimType = OpenGLHelper.ToNative(primitiveType);
            OGL.GL.DrawElementsBaseVertex(nativePrimType, indexCount, OGL.DrawElementsType.UnsignedShort, new IntPtr(startIndex), baseVertexOffset);
        }

        /// <summary>
        /// Draws indexed, instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="indexCountPerInstance">Number of indices per instance to draw.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startIndex">Starting index in the index buffer at which to read vertex indices from.</param>
        /// <param name="baseVertexOffset">Offset to add to each index before reading a vertex from a vertex buffer.</param>
        /// <param name="startInstanceOffset">Offset to add to each index before reading per-instance data from a vertex buffer.</param>
        public void DrawIndexedInstanced(PrimitiveType primitiveType, int indexCountPerInstance, int instanceCount, int startIndex, int baseVertexOffset, int startInstanceOffset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws non-indexed, instanced geometry.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        /// <param name="vertexCountPerInstance">Number of vertices per instance to draw.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startVertexIndex">Starting index in a vertex buffer at which to read vertices from.</param>
        /// <param name="startInstanceOffset">Offset to add to each index before reading per-instance data from a vertex buffer.</param>
        public void DrawInstanced(PrimitiveType primitiveType, int vertexCountPerInstance, int instanceCount, int startVertexIndex, int startInstanceOffset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws geometry of an unknown size that has been streamed out.
        /// </summary>
        /// <param name="primitiveType">Type of primitives to draw.</param>
        public void DrawAuto(PrimitiveType primitiveType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends queued up commands in the command buffer to the GPU.
        /// </summary>
        public void Flush()
        {
            OGL.GL.Flush();
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
                OGL.GL.DeleteVertexArray(_vao);
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Sets all render states back to defaults
        /// </summary>
        private void SetDefaultRenderStates()
        {
            EnforcedRenderState = EnforcedRenderState.None;
            _blendState = null;
            _rasterizerState = null;
            _depthStencilState = null;

            BlendState = OpenGLRenderSystem.PredefinedBlendStates.Opaque;
            RasterizerState = OpenGLRenderSystem.PredefinedRasterizerStates.CullBackClockwiseFront;
            DepthStencilState = OpenGLRenderSystem.PredefinedDepthStencilStates.Default;

            _scissorRectangle = new Rectangle(0, 0, 0, 0);
        }

        /// <summary>
        /// Starts listening for camera events
        /// </summary>
        private void StartCameraEvents()
        {
            if (_camera != null)
            {
                // Apply the viewport
                Viewport vp = _camera.Viewport;
                OGL.GL.Viewport(vp.X, vp.Y, vp.Width, vp.Height);
                OGL.GL.DepthRange(vp.MinDepth, vp.MaxDepth);

                _camera.ViewportChanged += CameraViewportChanged;
            }
        }

        /// <summary>
        /// Stops listening for camera events
        /// </summary>
        private void StopCameraEvents()
        {
            if (_camera != null)
            {
                _camera.ViewportChanged -= CameraViewportChanged;
            }
        }

        /// <summary>
        /// Invoked when the camera viewport changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraViewportChanged(Camera sender, EventArgs e)
        {
            Viewport vp = sender.Viewport;
            OGL.GL.Viewport(vp.X, vp.Y, vp.Width, vp.Height);
            OGL.GL.DepthRange(vp.MinDepth, vp.MaxDepth);
        }
    }
}
