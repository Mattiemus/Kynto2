namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Collections.Generic;

    using Spark.Graphics;
    using Spark.Math;
    using Spark.Utilities;

    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;
    using SDXM = SharpDX.Mathematics.Interop;

    public sealed class D3D11RenderContext : Disposable, IDeferredRenderContext
    {
        private D3D11.DeviceContext _deviceContext;
        private readonly IRenderSystem _renderSystem;
        private readonly Dictionary<Type, IRenderContextExtension> _extensions;
        private readonly D3D11ShaderStage[] _shaderStages;
        private readonly bool _isImmediateContext;
        private BlendState _blendState;
        private RasterizerState _rasterizerState;
        private DepthStencilState _depthStencilState;
        private Rectangle _scissorRectangle;
        private Color _blendFactor;
        private int _referenceStencil;
        private int _blendSampleMask;
        private Camera _camera;

        private IndexBuffer _indexBuffer;
        private readonly BufferHelper _bufferHelper;
        private readonly RenderTargetHelper _renderTargetHelper;
        private readonly VertexBufferBinding[] _singleVB;
        private readonly StreamOutputBufferBinding[] _singleSO;
        private readonly IRenderTarget[] _singleRT;
        private readonly InputLayoutManager _inputLayoutManager;
        private InputLayoutCache _localInputLayoutCache;
        private D3D11.InputLayout _previousInputLayout;

        internal D3D11RenderContext(D3D11RenderSystem renderSystem, D3D11GraphicsAdapter adapter, D3D11.DeviceContext deviceContext, bool isImmediate)
        {
            _renderSystem = renderSystem;
            _deviceContext = deviceContext;
            _isImmediateContext = isImmediate;
            Camera = new Camera(); // Create an uninitialized camera

            _extensions = new Dictionary<Type, IRenderContextExtension>();

            // Initialize shader stage wrappers
            int maxSamplerSlots = D3D11.CommonShaderStage.SamplerSlotCount;
            int maxResourceSlots = D3D11.CommonShaderStage.InputResourceSlotCount;
            int maxConstantBufferSlots = D3D11.CommonShaderStage.ConstantBufferApiSlotCount;

            _shaderStages = new D3D11ShaderStage[]
            {
                new D3D11ShaderStage(deviceContext.VertexShader, ShaderStage.VertexShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots),
                new D3D11ShaderStage(deviceContext.HullShader, ShaderStage.HullShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots),
                new D3D11ShaderStage(deviceContext.DomainShader, ShaderStage.DomainShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots),
                new D3D11ShaderStage(deviceContext.GeometryShader, ShaderStage.GeometryShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots),
                new D3D11ShaderStage(deviceContext.PixelShader, ShaderStage.PixelShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots),
                new D3D11ShaderStage(deviceContext.ComputeShader, ShaderStage.ComputeShader, maxSamplerSlots, maxResourceSlots, maxConstantBufferSlots)
            };

            _bufferHelper = new BufferHelper(_deviceContext, adapter);
            _renderTargetHelper = new RenderTargetHelper(this, adapter);
            _singleVB = new VertexBufferBinding[1];
            _singleSO = new StreamOutputBufferBinding[1];
            _singleRT = new IRenderTarget[1];
            _inputLayoutManager = renderSystem.GlobalInputLayoutManager;

            SetDefaultRenderStates();
        }

        public event TypedEventHandler<IRenderContext, EventArgs> Disposing;

        public IRenderSystem RenderSystem => _renderSystem;

        public bool IsImmediateContext => _isImmediateContext;

        public BlendState BlendState
        {
            get => _blendState;
            set
            {
                // If enforced, filter out
                if ((EnforcedRenderState & EnforcedRenderState.BlendState) == EnforcedRenderState.BlendState)
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = _renderSystem.PredefinedBlendStates.Opaque;
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
                    Direct3DHelper.ConvertColor(ref _blendFactor, out SDXM.RawColor4 sdxBf);

                    var nativeState = value.Implementation as ID3D11BlendState;
                    _deviceContext.OutputMerger.SetBlendState(nativeState.D3DBlendState, sdxBf, _blendSampleMask);
                }
            }
        }

        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                // If enforced, filter out
                if ((EnforcedRenderState & EnforcedRenderState.RasterizerState) == EnforcedRenderState.RasterizerState)
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = _renderSystem.PredefinedRasterizerStates.CullBackClockwiseFront;
                }

                // If a new state, apply it
                if (!value.IsSameState(_rasterizerState))
                {
                    if (!value.IsBound)
                    {
                        value.BindRenderState();
                    }

                    _rasterizerState = value;

                    var nativeState = value.Implementation as ID3D11RasterizerState;
                    _deviceContext.Rasterizer.State = nativeState.D3DRasterizerState;
                }
            }
        }

        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                // If enforced, filter out
                if ((EnforcedRenderState & EnforcedRenderState.DepthStencilState) == EnforcedRenderState.DepthStencilState)
                {
                    return;
                }

                // If null, set default state
                if (value == null)
                {
                    value = _renderSystem.PredefinedDepthStencilStates.Default;
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

                    var nativeState = value.Implementation as ID3D11DepthStencilState;
                    _deviceContext.OutputMerger.SetDepthStencilState(nativeState.D3DDepthStencilState, _referenceStencil);
                }
            }
        }

        public EnforcedRenderState EnforcedRenderState { get; set; }

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
                _deviceContext.Rasterizer.SetScissorRectangle(_scissorRectangle.Left, _scissorRectangle.Top, _scissorRectangle.Right, _scissorRectangle.Bottom);
            }
        }

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

                // Applying blend factor means re-applying the entire blend state
                Direct3DHelper.ConvertColor(ref _blendFactor, out SDXM.RawColor4 sdxBf);
                var nativeState = _blendState.Implementation as ID3D11BlendState;
                _deviceContext.OutputMerger.SetBlendState(nativeState.D3DBlendState, sdxBf, _blendSampleMask);
            }
        }

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

                // Applying the sample mask means re-applying the entire blend state
                Direct3DHelper.ConvertColor(ref _blendFactor, out SDXM.RawColor4 sdxBf);
                var nativeState = _blendState.Implementation as ID3D11BlendState;
                _deviceContext.OutputMerger.SetBlendState(nativeState.D3DBlendState, sdxBf, _blendSampleMask);
            }
        }

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

                // Applying the reference stencil means re-applying the entire depth stencil state
                var nativeState = _depthStencilState.Implementation as ID3D11DepthStencilState;
                _deviceContext.OutputMerger.SetDepthStencilState(nativeState.D3DDepthStencilState, _referenceStencil);
            }
        }

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

        public SwapChain BackBuffer
        {
            get => _renderTargetHelper.BackBuffer;
            set => _renderTargetHelper.BackBuffer = value;
        }

        public D3D11.Device D3DDevice => _deviceContext.Device;

        public D3D11.DeviceContext D3DDeviceContext => _deviceContext;

        public InputLayoutManager InputLayoutManager => _inputLayoutManager;

        public void SetCurrentInputLayoutCache(InputLayoutCache localCache)
        {
            _localInputLayoutCache = localCache;
        }

        public IShaderStage GetShaderStage(ShaderStage shaderStage)
        {
            return _shaderStages[(int)shaderStage];
        }

        public IEnumerable<IShaderStage> GetShaderStages()
        {
            return _shaderStages.Clone() as IShaderStage[];
        }

        public bool IsShaderStageSupported(ShaderStage shaderStage)
        {
            return true;
        }

        public T GetExtension<T>() where T : IRenderContextExtension
        {
            if (_extensions.TryGetValue(typeof(T), out IRenderContextExtension ext))
            {
                return (T)ext;
            }

            return default(T);
        }

        public IEnumerable<IRenderContextExtension> GetExtensions()
        {
            return _extensions.Values;
        }

        public bool IsExtensionSupported<T>() where T : IRenderContextExtension
        {
            return _extensions.ContainsKey(typeof(T));
        }

        public ICommandList FinishCommandList(bool restoreDeferredContextState)
        {
            if (_isImmediateContext)
            {
                throw new SparkGraphicsException("Command list should be finished on a deferred context");
            }

            D3D11.CommandList cmdList = _deviceContext.FinishCommandList(restoreDeferredContextState);

            // If not restoring state, then our cached state will be dirty
            if (!restoreDeferredContextState)
            {
                ClearOurState(true);
            }

            return new D3D11CommandListWrapper(cmdList);
        }

        public void ExecuteCommandList(ICommandList commandList, bool restoreImmediateContextState)
        {
            if (!_isImmediateContext)
            {
                throw new SparkGraphicsException("Command list must be executed on an immediate context");
            }

            var cmdList = commandList as ID3D11CommandList;
            if (cmdList == null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            // If not restoring state, then our cached state will be dirty
            if (!restoreImmediateContextState)
            {
                ClearOurState(true);
            }

            _deviceContext.ExecuteCommandList(cmdList.D3DCommandList, restoreImmediateContextState);
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            if (ReferenceEquals(_indexBuffer, indexBuffer))
            {
                return;
            }

            _indexBuffer = indexBuffer;

            D3D11.Buffer nativeBuffer = null;
            var format = DXGI.Format.R16_UInt;

            if (indexBuffer != null)
            {
                nativeBuffer = (indexBuffer.Implementation as ID3D11Buffer).D3DBuffer;
                format = Direct3DHelper.ToD3DIndexFormat(indexBuffer.IndexFormat);
            }

            _deviceContext.InputAssembler.SetIndexBuffer(nativeBuffer, format, 0);
        }

        public void SetVertexBuffer(VertexBufferBinding vertexBuffer)
        {
            if (vertexBuffer.VertexBuffer != null)
            {
                _singleVB[0] = vertexBuffer;
                _bufferHelper.SetVertexBuffers(_singleVB, 1);
                _singleVB[0] = new VertexBufferBinding(); // Don't hold onto the object
            }
            else
            {
                _bufferHelper.SetVertexBuffers(null, 0);
            }
        }

        public void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers)
        {
            int count = (vertexBuffers != null) ? vertexBuffers.Length : 0;
            _bufferHelper.SetVertexBuffers(vertexBuffers, count);
        }

        public void SetStreamOutputTarget(StreamOutputBufferBinding streamOutputBuffer)
        {
            if (streamOutputBuffer.StreamOutputBuffer != null)
            {
                _singleSO[0] = streamOutputBuffer;
                _bufferHelper.SetStreamOutputTargets(_singleSO, 1);
                _singleSO[0] = new StreamOutputBufferBinding(); // Don't hold onto the object
            }
            else
            {
                _bufferHelper.SetStreamOutputTargets(null, 0);
            }
        }

        public void SetStreamOutputTargets(params StreamOutputBufferBinding[] streamOutputBuffers)
        {
            int count = (streamOutputBuffers != null) ? streamOutputBuffers.Length : 0;
            _bufferHelper.SetStreamOutputTargets(streamOutputBuffers, count);
        }

        public void SetRenderTarget(SetTargetOptions options, IRenderTarget renderTarget)
        {
            if (renderTarget != null)
            {
                _singleRT[0] = renderTarget;
                _renderTargetHelper.SetRenderTargets(options, _singleRT, 1);
                _singleRT[0] = null; // Don't hold onto the object
            }
            else
            {
                _renderTargetHelper.SetRenderTargets(options, null, 0);
            }
        }

        public void SetRenderTargets(SetTargetOptions options, params IRenderTarget[] renderTargets)
        {
            int count = (renderTargets != null) ? renderTargets.Length : 0;
            _renderTargetHelper.SetRenderTargets(options, renderTargets, count);
        }

        public IndexBuffer GetIndexBuffer()
        {
            return _indexBuffer;
        }

        public VertexBufferBinding[] GetVertexBuffers()
        {
            return _bufferHelper.GetVertexBuffers();
        }

        public StreamOutputBufferBinding[] GetStreamOutputTargets()
        {
            return _bufferHelper.GetStreamOutputTargets();
        }

        public IRenderTarget[] GetRenderTargets()
        {
            return _renderTargetHelper.GetRenderTargets();
        }

        public void ClearState()
        {
            _deviceContext.ClearState();
            ClearOurState(true);
        }

        public void Clear(Color color)
        {
            _renderTargetHelper.Clear(ClearOptions.All, color, 1.0f, 0);
        }

        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            _renderTargetHelper.Clear(options, color, depth, stencil);
        }

        public void Draw(PrimitiveType primitiveType, int vertexCount, int startVertexIndex)
        {
            if (vertexCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount), "Vertex count must be greater than zero");
            }

            if (startVertexIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startVertexIndex), "Buffer offset must be zero or greater");
            }

            if (_localInputLayoutCache == null)
            {
                throw new SparkGraphicsException("Must apply pass before performing draw call");
            }

            SetCurrentInputLayout();
            _deviceContext.InputAssembler.PrimitiveTopology = Direct3DHelper.ToD3DPrimitiveTopology(primitiveType);
            _deviceContext.Draw(vertexCount, startVertexIndex);
        }

        public void DrawIndexed(PrimitiveType primitiveType, int indexCount, int startIndex, int baseVertexOffset)
        {
            if (indexCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount), "Index count must be greater than zero");
            }

            if (_indexBuffer == null)
            {
                throw new SparkGraphicsException("Index buffer is required");
            }

            if (baseVertexOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseVertexOffset), "Buffer offset must be zero or greater");
            }

            if (_localInputLayoutCache == null)
            {
                throw new SparkGraphicsException("Must apply pass before performing draw call");
            }

            SetCurrentInputLayout();
            _deviceContext.InputAssembler.PrimitiveTopology = Direct3DHelper.ToD3DPrimitiveTopology(primitiveType);
            _deviceContext.DrawIndexed(indexCount, startIndex, baseVertexOffset);
        }

        public void DrawIndexedInstanced(PrimitiveType primitiveType, int indexCountPerInstance, int instanceCount, int startIndex, int baseVertexOffset, int startInstanceOffset)
        {
            if (indexCountPerInstance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCountPerInstance), "Index count must be greater than zero");
            }

            if (instanceCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceCount), "Instance count must be greater than zero");
            }

            if (_indexBuffer == null)
            {
                throw new SparkGraphicsException("Index buffer is required");
            }

            if (baseVertexOffset < 0 || startIndex < 0 || startInstanceOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseVertexOffset), "Buffer offset must be zero or greater");
            }

            if (_localInputLayoutCache == null)
            {
                throw new SparkGraphicsException("Must apply pass before performing draw call");
            }

            SetCurrentInputLayout();
            _deviceContext.InputAssembler.PrimitiveTopology = Direct3DHelper.ToD3DPrimitiveTopology(primitiveType);
            _deviceContext.DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndex, baseVertexOffset, startInstanceOffset);
        }

        public void DrawInstanced(PrimitiveType primitiveType, int vertexCountPerInstance, int instanceCount, int startVertexIndex, int startInstanceOffset)
        {
            if (vertexCountPerInstance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCountPerInstance), "Vertex count must be greater than zero");
            }

            if (instanceCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceCount), "Instance count must be greater than zero");
            }

            if (startVertexIndex < 0 || startInstanceOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startVertexIndex), "Buffer offset must be zero or greater");
            }

            if (_localInputLayoutCache == null)
            {
                throw new SparkGraphicsException("Must apply pass before performing draw call");
            }

            SetCurrentInputLayout();
            _deviceContext.InputAssembler.PrimitiveTopology = Direct3DHelper.ToD3DPrimitiveTopology(primitiveType);
            _deviceContext.DrawInstanced(vertexCountPerInstance, instanceCount, startVertexIndex, startInstanceOffset);
        }

        public void DrawAuto(PrimitiveType primitiveType)
        {
            if (_localInputLayoutCache == null)
            {
                throw new SparkGraphicsException("Must apply pass before performing draw call");
            }

            SetCurrentInputLayout();
            _deviceContext.InputAssembler.PrimitiveTopology = Direct3DHelper.ToD3DPrimitiveTopology(primitiveType);
            _deviceContext.DrawAuto();
        }

        public void Flush()
        {
            _deviceContext.Flush();
        }

        public StreamOutputBufferBinding[] GetBoundSOWithoutCopy(out int numActive)
        {
            numActive = _bufferHelper.CurrentStreamOutputCount;
            return _bufferHelper.BoundStreamOutputTargets;
        }
        
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            Disposing?.Invoke(this, EventArgs.Empty);

            if (isDisposing)
            {
                // Null out references
                ClearOurState(false);

                StopCameraEvents();

                if (!_isImmediateContext && _deviceContext != null)
                {
                    _deviceContext.Dispose();
                    _deviceContext = null;
                }
            }

            base.Dispose(isDisposing);
        }

        private void ClearOurState(bool setDefaultRenderStates)
        {
            _bufferHelper.ClearState();
            _renderTargetHelper.ClearState();

            for (int i = 0; i < _shaderStages.Length; i++)
            {
                _shaderStages[i].ClearState();
            }

            _indexBuffer = null;
            _previousInputLayout = null;
            _camera = null;

            if (setDefaultRenderStates)
            {
                SetDefaultRenderStates();
            }
        }

        private void SetDefaultRenderStates()
        {
            EnforcedRenderState = EnforcedRenderState.None;
            _blendState = null;
            _rasterizerState = null;
            _depthStencilState = null;

            BlendState = _renderSystem.PredefinedBlendStates.Opaque;
            RasterizerState = _renderSystem.PredefinedRasterizerStates.CullBackClockwiseFront;
            DepthStencilState = _renderSystem.PredefinedDepthStencilStates.Default;

            _scissorRectangle = new Rectangle(0, 0, 0, 0);
        }

        private void SetCurrentInputLayout()
        {
            D3D11.InputLayout inputLayout = null;

            if (_localInputLayoutCache != null)
            {
                inputLayout = _localInputLayoutCache.GetOrCreate(_inputLayoutManager, _bufferHelper.CurrentVertexBufferCount, _bufferHelper.BoundVertexBuffers);
            }

            if (!ReferenceEquals(_previousInputLayout, inputLayout))
            {
                _deviceContext.InputAssembler.InputLayout = inputLayout;
                _previousInputLayout = inputLayout;
            }
        }

        private void StartCameraEvents()
        {
            if (_camera != null)
            {
                // Apply the viewport
                Viewport vp = _camera.Viewport;
                _deviceContext.Rasterizer.SetViewport(vp.X, vp.Y, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth);

                _camera.ViewportChanged += Camera_ViewportChanged;
            }
        }

        private void StopCameraEvents()
        {
            if (_camera != null)
            {
                _camera.ViewportChanged -= Camera_ViewportChanged;
            }
        }

        private void Camera_ViewportChanged(Camera sender, EventArgs args)
        {
            Viewport vp = sender.Viewport;
            _deviceContext.Rasterizer.SetViewport(vp.X, vp.Y, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth);
        }

        #region Buffer Helper

        internal sealed class BufferHelper
        {
            private int _currVbCount;
            private int _currSOCount;
            private readonly int _maxVbCount;
            private readonly int _maxSOCount;

            private readonly VertexBufferBinding[] _boundVertexBuffers;
            private readonly StreamOutputBufferBinding[] _boundStreamOutputTargets;

            private readonly D3D11.VertexBufferBinding[] _tempVBuffers;
            private readonly D3D11.StreamOutputBufferBinding[] _tempSOBuffers;

            private readonly D3D11.InputAssemblerStage _inputAssembler;
            private readonly D3D11.StreamOutputStage _streamOutput;

            public BufferHelper(D3D11.DeviceContext context, D3D11GraphicsAdapter adapter)
            {
                _currVbCount = 0;
                _currSOCount = 0;

                _maxVbCount = adapter.MaximumVertexStreams;
                _maxSOCount = adapter.MaximumStreamOutputTargets;

                _boundVertexBuffers = new VertexBufferBinding[_maxVbCount];
                _tempVBuffers = new D3D11.VertexBufferBinding[_maxVbCount];

                _boundStreamOutputTargets = new StreamOutputBufferBinding[_maxSOCount];
                _tempSOBuffers = new D3D11.StreamOutputBufferBinding[_maxSOCount];

                _inputAssembler = context.InputAssembler;
                _streamOutput = context.StreamOutput;
            }

            public VertexBufferBinding[] BoundVertexBuffers => _boundVertexBuffers;

            public int CurrentVertexBufferCount => _currVbCount;

            public StreamOutputBufferBinding[] BoundStreamOutputTargets => _boundStreamOutputTargets;

            public int CurrentStreamOutputCount => _currSOCount;

            #region Vertex Buffers

            public void SetVertexBuffers(VertexBufferBinding[] vertexBuffers, int vbCount)
            {
                if (vbCount > _maxVbCount)
                {
                    throw new SparkGraphicsException(nameof(vertexBuffers), "Too many vertex buffers");
                }

                int prevCount = _currVbCount;
                _currVbCount = vbCount;

                // If zero, setting null, want to clear all previously active buffers, set all to null, and return early
                if (vbCount == 0)
                {
                    if (prevCount != 0)
                    {
                        Array.Clear(_boundVertexBuffers, 0, prevCount);
                        _inputAssembler.SetVertexBuffers(0, _tempVBuffers);
                    }
                }
                // Else go through each buffer we want to set, if all are present in the correct slots then we don't update, but if
                // at least one needs updating we need to set them all
                else
                {
                    int applyCount = 0;

                    // Iterate through buffers, apply if not the same
                    for (int i = 0; i < vbCount; i++)
                    {
                        VertexBufferBinding binding = vertexBuffers[i];

                        // Set to temporary array
                        var vb = binding.VertexBuffer;
                        var nativeBinding = new D3D11.VertexBufferBinding();

                        if (vb != null)
                        {
                            nativeBinding.Buffer = (vb.Implementation as ID3D11Buffer).D3DBuffer;
                            nativeBinding.Stride = vb.VertexLayout.VertexStride;
                            nativeBinding.Offset = binding.VertexOffset;
                        }
                        else
                        {
                            nativeBinding.Buffer = null;
                            nativeBinding.Stride = 0;
                            nativeBinding.Offset = 0;
                        }

                        _tempVBuffers[i] = nativeBinding;

                        // Check if the VB is already set at the index, if so then we may -not- have to apply. But we need to keep checking.
                        // If at least one VB isnt set, we need to reset all. If all are already there, we can ignore setting (but still need to clear
                        // the temp array).
                        if (!binding.Equals(_boundVertexBuffers[i]))
                        {
                            _boundVertexBuffers[i] = binding;
                            applyCount++;
                        }
                    }

                    // Clear remaining slots to ensure we don't have any leftover cruft because we had a lot of buffers bound previously
                    if (vbCount < prevCount)
                    {
                        Array.Clear(_boundVertexBuffers, vbCount, _maxVbCount - vbCount);
                    }

                    // If at least one needs to be set, apply all the buffers in the temp array
                    if (applyCount > 0)
                    {
                        _inputAssembler.SetVertexBuffers(0, _tempVBuffers);
                    }

                    // Always clear the temp array before returning
                    Array.Clear(_tempVBuffers, 0, vbCount);
                }
            }

            public VertexBufferBinding[] GetVertexBuffers()
            {
                if (_currVbCount == 0)
                {
                    return new VertexBufferBinding[0];
                }

                var buffers = new VertexBufferBinding[_currVbCount];
                Array.Copy(_boundVertexBuffers, 0, buffers, 0, _currVbCount);

                return buffers;
            }

            #endregion

            #region Stream Output

            public void SetStreamOutputTargets(StreamOutputBufferBinding[] streamOutputBuffers, int soCount)
            {
                if (soCount > _boundStreamOutputTargets.Length)
                {
                    throw new SparkGraphicsException(nameof(streamOutputBuffers), "Too many stream output targets");
                }

                int prevCount = _currSOCount;
                _currSOCount = soCount;

                // If zero, setting null, want to clear all previously active buffers, set all to null, and return early
                if (soCount == 0 && prevCount != 0)
                {
                    Array.Clear(_boundStreamOutputTargets, 0, prevCount);
                    _streamOutput.SetTargets(_tempSOBuffers); // ALL or nothing
                }
                // Else go through each buffer we want to set, if all are present in the correct slots then we don't update,
                // but but if at least one needs updating we need to set them all
                else
                {

                    int applyCount = 0;

                    // Iterate through buffers, apply if not the same
                    for (int i = 0; i < soCount; i++)
                    {
                        var binding = streamOutputBuffers[i];

                        // Set to temporary array
                        var so = binding.StreamOutputBuffer;
                        var nativeBinding = new D3D11.StreamOutputBufferBinding();

                        if (so != null)
                        {
                            nativeBinding.Buffer = (so.Implementation as ID3D11Buffer).D3DBuffer;
                            nativeBinding.Offset = binding.VertexOffset;
                        }
                        else
                        {
                            nativeBinding.Buffer = null;
                            nativeBinding.Offset = 0;
                        }

                        _tempSOBuffers[i] = nativeBinding;

                        // Check if the SO is already set at the index, if so then we may -not- have to apply. But we need to keep checking.
                        // If at least one SO isn't set, we need to reset all. If all are already there we can ignore setting (but still need to clear
                        // the temp array).

                        if (!binding.Equals(_boundStreamOutputTargets[i]))
                        {
                            _boundStreamOutputTargets[i] = binding;
                            applyCount++;
                        }
                    }

                    // Clear remaining slots to ensure we don't have any leftover cruft because we had a lot of buffers bound previously
                    if (soCount < prevCount)
                    {
                        Array.Clear(_boundStreamOutputTargets, soCount, _maxSOCount - soCount);
                    }

                    // If at least one needs to be set, apply all the buffers in the temp array
                    if (applyCount > 0)
                    {
                        _streamOutput.SetTargets(_tempSOBuffers); // ALL or nothing
                    }

                    // Always clear the temp array before returning
                    Array.Clear(_tempSOBuffers, 0, soCount);
                }
            }

            public StreamOutputBufferBinding[] GetStreamOutputTargets()
            {
                if (_currSOCount == 0)
                {
                    return new StreamOutputBufferBinding[0];
                }

                var buffers = new StreamOutputBufferBinding[_currSOCount];
                Array.Copy(_boundStreamOutputTargets, 0, buffers, 0, _currSOCount);

                return buffers;
            }

            #endregion

            public void ClearState()
            {
                Array.Clear(_boundVertexBuffers, 0, _currVbCount);
                Array.Clear(_boundStreamOutputTargets, 0, _currSOCount);

                _currVbCount = 0;
                _currSOCount = 0;
            }
        }

        #endregion

        #region RenderTarget Helper

        internal class RenderTargetHelper
        {
            private SwapChain _backBuffer;
            private readonly D3D11RenderContext _renderContext;
            private readonly D3D11.DeviceContext _deviceContext;
            private readonly D3D11.OutputMergerStage _outputMerger;

            private bool _noUserRenderTargetsSet;
            private int _currRenderTargetCount;
            private readonly int _maxRenderTargetCount;
            private readonly IRenderTarget[] _boundRenderTargets;
            private readonly D3D11.RenderTargetView[] _tempRenderTargets;

            public RenderTargetHelper(D3D11RenderContext renderContext, D3D11GraphicsAdapter adapter)
            {
                _backBuffer = null;
                _renderContext = renderContext;
                _deviceContext = renderContext.D3DDeviceContext;
                _outputMerger = _deviceContext.OutputMerger;

                _noUserRenderTargetsSet = true;
                _currRenderTargetCount = 0;
                _maxRenderTargetCount = adapter.MaximumMultiRenderTargets;

                _boundRenderTargets = new IRenderTarget[_maxRenderTargetCount];
                _tempRenderTargets = new D3D11.RenderTargetView[_maxRenderTargetCount];
            }

            public SwapChain BackBuffer
            {
                get => _backBuffer;
                set
                {
                    if (!ReferenceEquals(_backBuffer, value))
                    {
                        // Setup events to handle when the swapchain is reset or resized.
                        StopSwapChainEvents();

                        _backBuffer = value;

                        StartSwapChainEvents();

                        // If the old swapchain was active OR no render targets set, set the new one's targets. 
                        // So if user targets are set, then we don't unseat them. When we set user targets to null, then the
                        // current active backbuffer will be set back to active.
                        if (_noUserRenderTargetsSet)
                        {
                            SetActiveBackBufferTargets();
                        }
                    }
                }
            }

            public bool NoUserRenderTargetsSet => _noUserRenderTargetsSet;

            public int CurrentRenderTargetCount => _currRenderTargetCount;

            public IRenderTarget[] BoundRenderTargets => _boundRenderTargets;

            public void Clear(ClearOptions options, Color color, float depth, int stencil)
            {
                if (_noUserRenderTargetsSet)
                {
                    _backBuffer.Clear(_renderContext, options, color, depth, stencil);
                }
                else
                {
                    var justTarget = ClearOptions.Target;
                    var clearDepth = (options & ClearOptions.Depth) == ClearOptions.Depth || (options & ClearOptions.Stencil) == ClearOptions.Stencil;

                    // Only clear the depth/stencil of the very first render target, otherwise just clear the target and not any depth buffer of the rest (they may also share depth buffers)
                    for (int i = 0; i < _currRenderTargetCount; i++)
                    {
                        var renderTarget = _boundRenderTargets[i];
                        var rtv = GetRenderTargetView(renderTarget);
                        rtv?.Clear(_deviceContext, justTarget, color, depth, stencil);

                        if (i == 0 && clearDepth)
                        {
                            var dsv = renderTarget.DepthStencilBuffer as ID3D11DepthStencilView;
                            dsv?.Clear(_deviceContext, options, depth, stencil);
                        }
                    }
                }
            }

            public void SetRenderTargets(SetTargetOptions options, IRenderTarget[] renderTargets, int numTargets)
            {
                // If not active and setting to null, set back to the active swap chain and resolve user targets
                if (renderTargets == null || numTargets == 0)
                {
                    SetActiveBackBufferTargets();
                    return;
                }

                // Otherwise back buffer is not active, and we want to set user targets
                _noUserRenderTargetsSet = false;

                if (numTargets > _maxRenderTargetCount)
                {
                    throw new SparkGraphicsException("Too many render targets");
                }

                // Resolve any outstanding targets
                ResolveTargets();

                int width = 0;
                int height = 0;
                int depth = 0;
                int sampleCount = 0;
                int arrayCount = 0;
                bool isCube = false;
                bool isArray = false;
                D3D11.DepthStencilView dsv = null;

                // Compare all targets to set with the first one, also extract the DSV from the first one
                for (int i = 0; i < numTargets; i++)
                {
                    IRenderTarget rt = renderTargets[i];
                    if (rt == null)
                    {
                        throw new ArgumentNullException($"Render target null at index {i}", nameof(renderTargets));
                    }

                    if (i == 0)
                    {
                        width = rt.Width;
                        height = rt.Height;
                        depth = rt.Depth;
                        sampleCount = rt.MultisampleDescription.Count;
                        arrayCount = rt.ArrayCount;
                        isCube = rt.IsCubeResource;
                        isArray = rt.IsArrayResource;

                        if (options != SetTargetOptions.NoDepthBuffer)
                        {
                            var depthBuffer = rt.DepthStencilBuffer as ID3D11DepthStencilView;
                            if (depthBuffer != null)
                            {
                                switch (options)
                                {
                                    case SetTargetOptions.ReadOnlyDepthBuffer:
                                        dsv = depthBuffer.D3DReadOnlyDepthStencilView;
                                        break;
                                    case SetTargetOptions.None:
                                        dsv = depthBuffer.D3DDepthStencilView;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (rt.Width != width ||
                            rt.Height != height ||
                            rt.Depth != depth ||
                            rt.MultisampleDescription.Count != sampleCount ||
                            rt.ArrayCount != arrayCount ||
                            rt.IsCubeResource != isCube || 
                            rt.IsArrayResource != isArray)
                        {
                            throw new SparkGraphicsException("Render target dimensions do not match");
                        }
                    }

                    _boundRenderTargets[i] = rt;
                    _tempRenderTargets[i] = GetRenderTargetView(rt).D3DRenderTargetView;
                    _currRenderTargetCount++;
                }

                // Clear remaining slots
                if (numTargets < _maxRenderTargetCount)
                {
                    int remainingCount = _maxRenderTargetCount - numTargets;
                    Array.Clear(_boundRenderTargets, numTargets, remainingCount);
                    Array.Clear(_tempRenderTargets, numTargets, remainingCount);
                }

                _outputMerger.SetTargets(dsv, _currRenderTargetCount, _tempRenderTargets);
                Array.Clear(_tempRenderTargets, 0, _currRenderTargetCount);
            }

            public IRenderTarget[] GetRenderTargets()
            {
                if (_currRenderTargetCount == 0)
                {
                    return new IRenderTarget[0];
                }

                var renderTargets = new IRenderTarget[_currRenderTargetCount];
                Array.Copy(_boundRenderTargets, 0, renderTargets, 0, _currRenderTargetCount);

                return renderTargets;
            }

            public void ClearState()
            {
                Array.Clear(_boundRenderTargets, 0, _currRenderTargetCount);

                _currRenderTargetCount = 0;
                _noUserRenderTargetsSet = false;
                _backBuffer = null;
                StopSwapChainEvents();
            }

            private void StartSwapChainEvents()
            {
                if (_backBuffer != null)
                {
                    var swp = _backBuffer.Implementation as ID3D11Backbuffer;
                    if (swp != null)
                    {
                        swp.OnResetResize += ReApplyActiveBackBufferTargets;
                    }
                }
            }

            private void StopSwapChainEvents()
            {
                if (_backBuffer != null)
                {
                    var swp = _backBuffer.Implementation as ID3D11Backbuffer;
                    if (swp != null)
                    {
                        swp.OnResetResize -= ReApplyActiveBackBufferTargets;
                    }
                }
            }

            private void ResolveTargets()
            {
                if (_currRenderTargetCount == 0)
                {
                    return;
                }

                for (int i = 0; i < _currRenderTargetCount; i++)
                {
                    ID3D11RenderTargetView rt = GetRenderTargetView(_boundRenderTargets[i]);
                    rt?.ResolveResource(_deviceContext);
                }
            }

            private void SetActiveBackBufferTargets()
            {
                // Resolve any outstanding targets
                ResolveTargets();

                // Ensure we unseat any targets beyond the first slot!
                if (_currRenderTargetCount > 0)
                {
                    Array.Clear(_boundRenderTargets, 0, _currRenderTargetCount);
                    _currRenderTargetCount = 0;
                }

                if (_backBuffer != null)
                {
                    var rtv = _backBuffer.Implementation as ID3D11RenderTargetView;
                    var dsv = _backBuffer.Implementation as ID3D11DepthStencilView;

                    D3D11.DepthStencilView depthStencilView = (dsv != null) ? dsv.D3DDepthStencilView : null;

                    if (rtv != null)
                    {
                        _tempRenderTargets[0] = rtv.D3DRenderTargetView;
                    }

                    _outputMerger.SetTargets(depthStencilView, 1, _tempRenderTargets);
                    _tempRenderTargets[0] = null;
                }
                // No active buffer, but also need to set state to null. Avoid setting for repeated null calls
                else if (!_noUserRenderTargetsSet)
                {
                    // No backbuffer to set to...
                    _outputMerger.SetTargets((D3D11.RenderTargetView)null);
                }

                _noUserRenderTargetsSet = true;
            }

            private static ID3D11RenderTargetView GetRenderTargetView(IRenderTarget rt)
            {
                if (rt is GraphicsResource)
                {
                    return (rt as GraphicsResource).Implementation as ID3D11RenderTargetView;
                }
                else
                {
                    return rt as ID3D11RenderTargetView;
                }
            }

            private void ReApplyActiveBackBufferTargets(ID3D11Backbuffer swapChain, EventArgs e)
            {
                if (_noUserRenderTargetsSet)
                {
                    SetActiveBackBufferTargets();
                }
            }
        }

        #endregion
    }
}
