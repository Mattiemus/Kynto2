namespace Spark.Direct3D11.Graphics.Implementation
{
    using System;
    
    using Spark.Graphics;
    using Spark.Utilities;

    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;
    using SDX = SharpDX;

    /// <summary>
    /// A Direct3D11 implementation of <see cref="IEffectConstantBuffer"/>.
    /// </summary>
    public sealed class D3D11EffectConstantBuffer : Disposable, IEffectConstantBuffer, ID3D11ShaderResourceView, ID3D11Buffer, IShaderResource
    {
        private RawBuffer _buffer;        
        private D3D11.ShaderResourceView _textureBufferView;
        private readonly D3D11EffectImplementation _impl;
        private readonly EffectData.ConstantBuffer _constantBufferData;

        internal D3D11EffectConstantBuffer(D3D11.Device device, D3D11EffectImplementation impl, EffectData.ConstantBuffer effectData) 
            : this(device, impl, effectData, null)
        {
        }

        internal D3D11EffectConstantBuffer(D3D11.Device device, D3D11EffectImplementation impl, EffectData.ConstantBuffer effectData, D3D11EffectConstantBuffer bufferToCloneFrom)
        {
            _impl = impl;
            _constantBufferData = effectData;
            Name = effectData.Name;
            SizeInBytes = effectData.SizeInBytes;
            IsTextureBuffer = false;

            if (effectData.BufferType == D3DConstantBufferType.TextureBuffer)
            {
                IsTextureBuffer = true;
            }

            _buffer = new RawBuffer(SizeInBytes);

            // If cloning, copy buffer memory, otherwise we'll set the default value (potentially) when we create the parameters
            if (bufferToCloneFrom != null)
            {
                MemoryHelper.CopyMemory(_buffer.BufferPointer, bufferToCloneFrom._buffer.BufferPointer, SizeInBytes);
            }

            if (effectData.Variables != null && effectData.Variables.Length > 0)
            {
                var variables = new D3D11EffectParameter[effectData.Variables.Length];
                bool isCloning = bufferToCloneFrom != null;
                for (int i = 0; i < variables.Length; i++)
                {
                    variables[i] = D3D11EffectParameter.CreateValueVariable(impl, this, effectData.Variables[i], isCloning);
                }

                Parameters = new EffectParameterCollection(variables);
            }
            else
            {
                Parameters = EffectParameterCollection.EmptyCollection;
            }

            // Set is dirty false here since we may have set it true when creating the parameters
            IsDirty = false;

            var desc = new D3D11.BufferDescription();
            desc.CpuAccessFlags = D3D11.CpuAccessFlags.Write;
            desc.OptionFlags = D3D11.ResourceOptionFlags.None;
            desc.SizeInBytes = SizeInBytes;
            desc.StructureByteStride = 0;
            desc.Usage = D3D11.ResourceUsage.Dynamic;
            desc.BindFlags = (IsTextureBuffer) ? D3D11.BindFlags.ShaderResource : D3D11.BindFlags.ConstantBuffer;

            D3DBuffer = new D3D11.Buffer(device, _buffer.BufferPointer, desc);

            if (IsTextureBuffer)
            {
                var srvDesc = new D3D11.ShaderResourceViewDescription();
                srvDesc.Format = SDX.DXGI.Format.R32G32B32A32_Float;
                srvDesc.Dimension = D3D.ShaderResourceViewDimension.Buffer;
                srvDesc.Buffer.ElementOffset = 0;
                srvDesc.Buffer.ElementWidth = SizeInBytes / (4 * sizeof(float)); //Divided by register size

                _textureBufferView = new D3D11.ShaderResourceView(device, D3DBuffer, srvDesc);
            }
        }

        public string Name { get; }

        public int SizeInBytes { get; }

        public EffectParameterCollection Parameters { get; }

        public bool IsTextureBuffer { get; }

        public D3D11.Buffer D3DBuffer { get; }

        public D3D11.ShaderResourceView D3DShaderResourceView => _textureBufferView;

        public ShaderResourceType ResourceType => ShaderResourceType.Buffer;

        string INamable.Name
        {
            get => Name;
            set
            {
                // No-op
            }
        }

        internal IntPtr RawBufferPointer => _buffer.BufferPointer;

        internal bool IsDirty { get; set; }

        public bool IsPartOf(Effect effect)
        {
            if (effect == null)
            {
                return false;
            }

            return ReferenceEquals(effect.Implementation, _impl);
        }

        public T Get<T>() where T : struct
        {
            return _buffer.Read<T>(0);
        }

        public T Get<T>(int offsetInBytes) where T : struct
        {
            return _buffer.Read<T>(offsetInBytes);
        }

        public T[] GetRange<T>(int count) where T : struct
        {
            return _buffer.Read<T>(0, count);
        }

        public T[] GetRange<T>(int offsetInBytes, int count) where T : struct
        {
            return _buffer.Read<T>(offsetInBytes, count);
        }

        public void Set<T>(T value) where T : struct
        {
            _buffer.Write(0, ref value);
            IsDirty = true;
        }

        public void Set<T>(ref T value) where T : struct
        {
            _buffer.Write(0, ref value);
            IsDirty = true;
        }

        public void Set<T>(int offsetInBytes, T value) where T : struct
        {
            _buffer.Write(offsetInBytes, ref value);
            IsDirty = true;
        }

        public void Set<T>(int offsetInBytes, ref T value) where T : struct
        {
            _buffer.Write(offsetInBytes, ref value);
            IsDirty = true;
        }

        public void Set<T>(params T[] values) where T : struct
        {
            _buffer.Write(0, values);
            IsDirty = true;
        }

        public void Set<T>(int offsetInBytes, params T[] values) where T : struct
        {
            _buffer.Write(offsetInBytes, values);
            IsDirty = true;
        }

        public void Update(D3D11.DeviceContext context)
        {
            if (IsDirty)
            {
                SDX.DataBox dataBox = context.MapSubresource(D3DBuffer, 0, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None);
                MemoryHelper.CopyMemory(dataBox.DataPointer, _buffer.BufferPointer, SizeInBytes);
                context.UnmapSubresource(D3DBuffer, 0);

                IsDirty = false;
            }
        }

        internal void SetDebugName(string name)
        {
            if (D3DBuffer != null)
            {
                D3DBuffer.DebugName = name;
            }

            if (_textureBufferView != null)
            {
                _textureBufferView.DebugName = name + "_SRV";
            }
        }
        
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }

                if (_textureBufferView != null)
                {
                    _textureBufferView.Dispose();
                    _textureBufferView = null;
                }
            }

            base.Dispose(isDisposing);
        }
    }
}
