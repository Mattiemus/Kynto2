namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    using Spark.Graphics;
    using Spark.Utilities;

    using D3D11 = SharpDX.Direct3D11;

    public sealed class InputLayoutCache
    {
        private static readonly int SemanticCount = Enum.GetValues(typeof(VertexSemantic)).Length;

        private readonly D3D11.Device _device;
        private readonly HashedShaderSignature _shaderSig;
        private readonly ConcurrentDictionary<int, D3D11.InputLayout> _cachedLayouts;

        public InputLayoutCache(D3D11.Device device, HashedShaderSignature inputSig)
        {
            _device = device;
            _shaderSig = inputSig;
            _cachedLayouts = new ConcurrentDictionary<int, D3D11.InputLayout>();
        }

        /// <summary>
        /// Clears the cache and optionally disposes of contained input layouts. Generally a cache is used as a local cache paired with the <see cref="InputLayoutManager"/>
        /// that serves as a global cache. What's in the local cache will be in the global cache, therefore the global cache "owns" the layouts and should properly dispose of them.
        /// However, since the local cache can be used as stand alone, we need to expose a method to dispose
        /// </summary>
        /// <param name="disposeLayouts">True if layouts should be disposed, false otherwise</param>
        public void Clear(bool disposeLayouts)
        {
            if (disposeLayouts)
            {
                lock (_cachedLayouts)
                {
                    foreach (KeyValuePair<int, D3D11.InputLayout> kv in _cachedLayouts)
                    {
                        kv.Value.Dispose();
                    }

                    _cachedLayouts.Clear();
                }
            }
            else
            {
                _cachedLayouts.Clear();
            }
        }

        public D3D11.InputLayout GetOrCreate(int numBuffers, params VertexBufferBinding[] vertexBufferSlots)
        {
            return GetOrCreate(null, numBuffers, vertexBufferSlots);
        }

        public D3D11.InputLayout GetOrCreate(VertexBufferBinding vertexBuffer)
        {
            return GetOrCreate(null, vertexBuffer);
        }

        public unsafe D3D11.InputLayout GetOrCreate(InputLayoutManager globalCache, int numBuffers, params VertexBufferBinding[] vertexBufferSlots)
        {
            if (numBuffers <= 0 || vertexBufferSlots == null || vertexBufferSlots.Length == 0 || numBuffers > vertexBufferSlots.Length)
            {
                return null;
            }

            if (numBuffers == 1)
            {
                // Take care of trivial case
                return GetOrCreate(globalCache, vertexBufferSlots[0]);
            }

            int key = GenerateLayoutKey(vertexBufferSlots, numBuffers); // Also ensures [0, numBuffers) are valid
            D3D11.InputLayout layout;

            // Look up if the composite declaration is in the cache
            if (!_cachedLayouts.TryGetValue(key, out layout))
            {
                bool createdLayout = false;

                // Not found - look up in the global cache if it exists. Or if not, create a new layout to add to the cache.
                if (globalCache == null)
                {
                    try
                    {
                        // For keeping track of current semantic index
                        int* semanticIndices = stackalloc int[SemanticCount];

                        var elements = new D3D11.InputElement[CountVertexElements(vertexBufferSlots, numBuffers)];
                        for (int i = 0, index = 0; i < numBuffers; i++)
                        {
                            VertexBufferBinding binding = vertexBufferSlots[i];
                            VertexLayout decl = binding.VertexBuffer.VertexLayout;
                            int offset = 0; //For every stream, reset offset
                            int instanceFrequency = binding.InstanceFrequency;

                            for (int j = 0; j < decl.ElementCount; j++)
                            {
                                VertexElement vertexElement = decl[j];

                                elements[index++] = new D3D11.InputElement
                                (
                                    Direct3DHelper.ToD3DVertexSemantic(vertexElement.SemanticName),
                                    IncrementSemanticIndex(semanticIndices, vertexElement.SemanticName),
                                    Direct3DHelper.ToD3DVertexFormat(vertexElement.Format),
                                    offset,
                                    i, // Stream slot
                                    (instanceFrequency > 0) ? D3D11.InputClassification.PerInstanceData : D3D11.InputClassification.PerVertexData,
                                    instanceFrequency
                                );

                                offset += vertexElement.Format.SizeInBytes();
                            }
                        }

                        layout = new D3D11.InputLayout(_device, _shaderSig.ByteCode, elements);
                        createdLayout = true;
                    }
                    catch (Exception e)
                    {
                        throw new SparkGraphicsException("Invalid input layout", e);
                    }
                }
                else
                {
                    layout = globalCache.GetOrCreate(_shaderSig, numBuffers, vertexBufferSlots);
                }

                // Now we have our layout, either from the global cache or we just created it. At this point we attempt to add it, but it may be there from
                // another cache operation, so that will fail and we'll return what's already in the cache (and properly dispose of a layout we just created but didn't need).
                if (!_cachedLayouts.TryAdd(key, layout))
                {
                    if (createdLayout)
                    {
                        layout.Dispose();
                    }

                    _cachedLayouts.TryGetValue(key, out layout);
                }
            }

            return layout;
        }

        public D3D11.InputLayout GetOrCreate(InputLayoutManager globalCache, VertexBufferBinding vertexBuffer)
        {
            if (vertexBuffer == null)
            {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }

            VertexLayout vertexDecl = vertexBuffer.VertexBuffer.VertexLayout;
            int instanceFrequency = vertexBuffer.InstanceFrequency;
            int key = vertexDecl.GetHashCode();

            // Look up the declaration in the cache
            if (!_cachedLayouts.TryGetValue(key, out D3D11.InputLayout layout))
            {
                bool createdLayout = false;

                // Not found - look up in the global cache if it exists. Or if not, create a new layout to add to the cache.
                if (globalCache == null)
                {
                    try
                    {
                        var elements = new D3D11.InputElement[vertexDecl.ElementCount];
                        for (int i = 0; i < vertexDecl.ElementCount; i++)
                        {
                            VertexElement vertexElement = vertexDecl[i];

                            elements[i] = new D3D11.InputElement
                            (
                                Direct3DHelper.ToD3DVertexSemantic(vertexElement.SemanticName),
                                vertexElement.SemanticIndex,
                                Direct3DHelper.ToD3DVertexFormat(vertexElement.Format),
                                vertexElement.Offset,
                                0,
                                (instanceFrequency > 0) ? D3D11.InputClassification.PerInstanceData : D3D11.InputClassification.PerVertexData,
                                instanceFrequency
                            );
                        }

                        layout = new D3D11.InputLayout(_device, _shaderSig.ByteCode, elements);
                        createdLayout = true;
                    }
                    catch (Exception e)
                    {
                        throw new SparkGraphicsException("Invalid input layout", e);
                    }
                }
                else
                {
                    layout = globalCache.GetOrCreate(_shaderSig, vertexBuffer);
                }

                // Now we have our layout, either from the global cache or we just created it. At this point we attempt to add it, but it may be there from
                // another cache operation, so that will fail and we'll return what's already in the cache (and properly dispose of a layout we just created but didn't need).
                if (!_cachedLayouts.TryAdd(key, layout))
                {
                    if (createdLayout)
                    {
                        layout.Dispose();
                    }

                    _cachedLayouts.TryGetValue(key, out layout);
                }
            }

            return layout;
        }

        private static int GenerateLayoutKey(VertexBufferBinding[] vBuffers, int numBuffers)
        {
            unchecked
            {
                uint p = 16777619;
                uint hash = 2166136261;

                for (int i = 0; i < numBuffers; i++)
                {
                    VertexBufferBinding binding = vBuffers[i];
                    VertexBuffer vb = binding.VertexBuffer;

                    if (vb == null)
                    {
                        throw new SparkGraphicsException("Input layout must be contiguous");
                    }

                    hash = (hash ^ (uint)vb.VertexLayout.GetHashCode()) * p;
                    hash = (hash ^ (uint)binding.InstanceFrequency) * p; // Need to inclue step rate since thats part of the layout and may change
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                return (int)hash;
            }
        }

        private unsafe int IncrementSemanticIndex(int* semanticCounts, VertexSemantic semantic)
        {
            int index = (int)semantic;
            int currCount = semanticCounts[index];
            semanticCounts[index] = currCount + 1;

            return currCount;
        }

        private static int CountVertexElements(VertexBufferBinding[] vBuffers, int numBuffers)
        {
            int count = 0;
            for (int i = 0; i < numBuffers; i++)
            {
                count += vBuffers[i].VertexBuffer.VertexLayout.ElementCount;
            }

            return count;
        }
    }
}
