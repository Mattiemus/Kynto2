namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    using Spark.Graphics;
    using Spark.Utilities;

    using D3D11 = SharpDX.Direct3D11;

    public sealed class InputLayoutManager : Disposable
    {
        private readonly D3D11.Device _device;
        private readonly ConcurrentDictionary<HashedShaderSignature, InputLayoutCache> _layoutCaches;
        private readonly Func<HashedShaderSignature, InputLayoutCache> _cacheCreatorFactory;

        public InputLayoutManager(D3D11.Device device)
        {
            _device = device;
            _layoutCaches = new ConcurrentDictionary<HashedShaderSignature, InputLayoutCache>();
            _cacheCreatorFactory = CacheCreatorFactory;
        }

        public D3D11.InputLayout GetOrCreate(HashedShaderSignature inputSig, int numBuffers, VertexBufferBinding[] vertexBufferSlots)
        {
            ThrowIfDisposed();

            if (inputSig.ByteCode == null || numBuffers <= 0 || vertexBufferSlots == null || numBuffers > vertexBufferSlots.Length)
            {
                return null;
            }

            var cache = _layoutCaches.GetOrAdd(inputSig, _cacheCreatorFactory);
            if (numBuffers == 1)
            {
                // Take care of trivial case
                return cache.GetOrCreate(null, vertexBufferSlots[0]);
            }

            return cache.GetOrCreate(null, numBuffers, vertexBufferSlots);
        }

        public D3D11.InputLayout GetOrCreate(HashedShaderSignature inputSig, VertexBufferBinding firstSlotVertexBuffer)
        {
            ThrowIfDisposed();

            if (inputSig.ByteCode == null)
            {
                return null;
            }

            var cache = _layoutCaches.GetOrAdd(inputSig, _cacheCreatorFactory);
            return cache.GetOrCreate(null, firstSlotVertexBuffer);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                lock (_layoutCaches)
                {
                    foreach (var kv in _layoutCaches)
                    {
                        kv.Value.Clear(true);
                    }
                }
            }

            base.Dispose(isDisposing);
        }

        private InputLayoutCache CacheCreatorFactory(HashedShaderSignature inputSig)
        {
            return new InputLayoutCache(_device, inputSig);
        }
    }
}
