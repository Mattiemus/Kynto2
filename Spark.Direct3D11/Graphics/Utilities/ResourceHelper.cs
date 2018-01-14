namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark.Math;
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;
    using SDX = SharpDX;

    /// <summary>
    /// Helper class for reading/writing to Direct3D11 resources.
    /// </summary>
    public static class ResourceHelper
    {
        private static object _sync = new object();

        /// <summary>
        /// Createds an interleaved vertex buffer containing data from each attribute buffer.
        /// </summary>
        /// <param name="vertexLayout">The vertex layout defining the contents of the buffer.</param>
        /// <param name="data">Array of databuffers representing the vertex data.</param>
        /// <returns>Interleaved vertex buffer.</returns>
        public static IDataBuffer CreatedInterleavedVertexBuffer(VertexLayout vertexLayout, IReadOnlyDataBuffer[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            // Verify if the incoming data vertex streams match right with the supplied vertex declaration
            if (vertexLayout.ElementCount != data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex element count mismatch");
            }

            // Verify each data buffer matches each vertex element
            int totalSizeInBytes = 0;
            int vertexStride = 0;
            int vertexCount = 0;

            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];
                VertexElement elem = vertexLayout[i];

                int vSizeInBytes = db.ElementSizeInBytes;
                int vCount = db.SizeInBytes / vSizeInBytes;

                // Calculate vertex count from first attribute buffer
                if (i == 0)
                {
                    vertexCount = vCount;
                }
                else if (vCount != vertexCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex count mismatch");
                }

                if (vSizeInBytes != elem.Format.SizeInBytes())
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex element mismatch");
                }

                totalSizeInBytes += db.SizeInBytes;
                vertexStride += vSizeInBytes;
                db.Position = 0;
            }

            if (totalSizeInBytes != vertexLayout.VertexStride * vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex buffer size mismatch");
            }

            var interleavedData = new DataBuffer<byte>(totalSizeInBytes);
            IntPtr vertex = MemoryHelper.AllocateMemory(vertexStride);

            try
            {
                IntPtr[] vStreams = MapDataBuffers(data);
                MappedDataBuffer interleavedPtr = interleavedData.Map();

                //For each vertex, accumulate in our temp buffer
                for (int i = 0; i < vertexCount; i++)
                {
                    int vertexOffset = 0;
                    for (int j = 0; j < data.Length; j++)
                    {
                        IReadOnlyDataBuffer db = data[j];
                        IntPtr dbPtr = vStreams[j];
                        int elementSize = db.ElementSizeInBytes;

                        MemoryHelper.CopyMemory(vertex + vertexOffset, dbPtr + (i * elementSize), elementSize);

                        vertexOffset += elementSize;
                    }

                    MemoryHelper.CopyMemory(interleavedPtr.Pointer + (i * vertexStride), vertex, vertexStride);
                }
            }
            finally
            {
                UnmapDataBuffers(data);

                if (interleavedData.IsMapped)
                {
                    interleavedData.Unmap();
                }

                MemoryHelper.FreeMemory(vertex);
            }

            return interleavedData;
        }

        /// <summary>
        /// Writes interleaved vertex data to a D3D11 Buffer.
        /// </summary>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="vertexCount">The number of vertices in the buffer.</param>
        /// <param name="vertexLayout">The vertex layout defining the contents of the buffer.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">Array of databuffers representing the vertex data.</param>
        /// <param name="data">True if writing data, false if reading data.</param>
        public static void WriteVertexData(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int vertexCount, VertexLayout vertexLayout, ResourceUsage resourceUsage, IReadOnlyDataBuffer[] data)
        {
            Direct3DHelper.CheckIfImmutable(resourceUsage);

            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            // Verify if the incoming data vertex streams match right with the supplied vertex declaration
            if (vertexLayout.ElementCount != data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex layout element count mismatch");
            }

            // Verify each data buffer matches each vertex element
            int totalSizeInBytes = 0;
            int vertexStride = 0;

            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];
                VertexElement elem = vertexLayout[i];

                int vSizeInBytes = db.ElementSizeInBytes;
                int vCount = db.SizeInBytes / vSizeInBytes;

                if (vCount != vertexCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex count mismatch");
                }

                if (vSizeInBytes != elem.Format.SizeInBytes())
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex element mismatch");
                }

                totalSizeInBytes += db.SizeInBytes;
                vertexStride += vSizeInBytes;
                db.Position = 0;
            }

            if (totalSizeInBytes != vertexLayout.VertexStride * vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex buffer size mismatch");
            }

            IntPtr vertex = IntPtr.Zero;

            lock (_sync)
            {
                try
                {
                    vertex = MemoryHelper.AllocateMemory(vertexStride);
                    IntPtr[] vStreams = MapDataBuffers(data);

                    // If dynamic buffer, just map it, and always doing a write discard
                    if (resourceUsage == ResourceUsage.Dynamic)
                    {
                        try
                        {
                            SDX.DataBox dataBox = context.MapSubresource(nativeBuffer, 0, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None);

                            // For each vertex, accumulate in our temp buffer
                            for (int i = 0; i < vertexCount; i++)
                            {
                                int vertexOffset = 0;
                                for (int j = 0; j < data.Length; j++)
                                {
                                    IReadOnlyDataBuffer db = data[j];
                                    IntPtr dbPtr = vStreams[j];
                                    int elementSize = db.ElementSizeInBytes;

                                    MemoryHelper.CopyMemory(vertex + vertexOffset, dbPtr + (i * elementSize), elementSize);

                                    vertexOffset += elementSize;
                                }

                                MemoryHelper.CopyMemory(dataBox.DataPointer + (i * vertexStride), vertex, vertexStride);
                            }

                        }
                        finally
                        {
                            context.UnmapSubresource(nativeBuffer, 0);
                        }

                        // Otherwise need to create a staging resource, accumulate each vertex into it then copy to the buffer
                    }
                    else
                    {
                        using (D3D11.Resource staging = CreateStaging(context.Device, nativeBuffer))
                        {
                            try
                            {
                                SDX.DataBox dataBox = context.MapSubresource(staging, 0, D3D11.MapMode.Write, D3D11.MapFlags.None);

                                // For each vertex, accumulate in our temp buffer
                                for (int i = 0; i < vertexCount; i++)
                                {

                                    int vertexOffset = 0;

                                    for (int j = 0; j < data.Length; j++)
                                    {
                                        IReadOnlyDataBuffer db = data[j];
                                        IntPtr dbPtr = vStreams[j];
                                        int elementSize = db.ElementSizeInBytes;

                                        MemoryHelper.CopyMemory(vertex + vertexOffset, dbPtr + (i * elementSize), elementSize);

                                        vertexOffset += elementSize;
                                    }

                                    MemoryHelper.CopyMemory(dataBox.DataPointer + (i * vertexStride), vertex, vertexStride);
                                }
                            }
                            finally
                            {
                                context.UnmapSubresource(staging, 0);
                            }

                            // Copy entire staging resource to buffer
                            context.CopyResource(staging, nativeBuffer);
                        }
                    }
                }
                finally
                {
                    UnmapDataBuffers(data);

                    if (vertex != IntPtr.Zero)
                    {
                        MemoryHelper.FreeMemory(vertex);
                    }
                }
            }
        }

        /// <summary>
        /// Reads interleaved vertex data from a D3D11 Buffer.
        /// </summary>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="vertexCount">The number of vertices in the buffer.</param>
        /// <param name="vertexLayout">The vertex layout defining the contents of the buffer.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">Array of databuffers representing the vertex data.</param>
        /// <param name="data">True if writing data, false if reading data.</param>
        public static void ReadVertexData(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int vertexCount, VertexLayout vertexLayout, ResourceUsage resourceUsage, IDataBuffer[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException(nameof(data), "Data buffer cannot be null");
            }

            // Verify if the incoming data vertex streams match right with the supplied vertex declaration
            if (vertexLayout.ElementCount != data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex layout element count mismatch");
            }

            // Verify each data buffer matches each vertex element
            int totalSizeInBytes = 0;
            int vertexStride = 0;

            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];
                VertexElement elem = vertexLayout[i];

                int vSizeInBytes = db.ElementSizeInBytes;
                int vCount = db.SizeInBytes / vSizeInBytes;

                if (vCount != vertexCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex count mismatch");
                }

                if (vSizeInBytes != elem.Format.SizeInBytes())
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Vertex element mismatch");
                }

                totalSizeInBytes += db.SizeInBytes;
                vertexStride += vSizeInBytes;
                db.Position = 0;
            }

            if (totalSizeInBytes != vertexLayout.VertexStride * vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Vertex buffer size mismatch");
            }
            
            IntPtr vertex = IntPtr.Zero;

            lock (_sync)
            {
                try
                {
                    vertex = MemoryHelper.AllocateMemory(vertexStride);
                    IntPtr[] vStreams = MapDataBuffers(data);

                    // Reading - always need a staging buffer
                    using (D3D11.Resource staging = CreateStaging(context.Device, nativeBuffer))
                    {
                        // Copy entire buffer to the staging resource
                        context.CopyResource(nativeBuffer, staging);

                        try
                        {
                            SDX.DataBox dataBox = context.MapSubresource(staging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);

                            // For each vertex, accumulate in our temp buffer
                            for (int i = 0; i < vertexCount; i++)
                            {
                                MemoryHelper.CopyMemory(vertex, dataBox.DataPointer + (i * vertexStride), vertexStride);

                                int vertexOffset = 0;

                                for (int j = 0; j < data.Length; j++)
                                {
                                    IReadOnlyDataBuffer db = data[j];
                                    IntPtr dbPtr = vStreams[j];
                                    int elementSize = db.ElementSizeInBytes;

                                    MemoryHelper.CopyMemory(dbPtr + (i * elementSize), vertex + vertexOffset, elementSize);

                                    vertexOffset += elementSize;
                                }

                            }
                        }
                        finally
                        {
                            context.UnmapSubresource(staging, 0);
                        }
                    }
                }
                finally
                {
                    UnmapDataBuffers(data);

                    if (vertex != IntPtr.Zero)
                    {
                        MemoryHelper.FreeMemory(vertex);
                    }
                }
            }
        }

        /// <summary>
        /// Writes vertex data to a D3D11 Buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="vertexCount">The number of vertices in the buffer.</param>
        /// <param name="vertexLayout">The vertex layout defining the contents of the buffer.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        /// <param name="elementCount">The number of elements to copy.</param>
        /// <param name="offsetInBytes">Offset from the start of the native buffer at which to start copying from</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        /// <param name="writeOptions">Write options for writing.</param>
        public static void WriteVertexData<T>(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int vertexCount, VertexLayout vertexLayout, ResourceUsage resourceUsage,
            IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride, DataWriteOptions writeOptions) where T : struct
        {
            Direct3DHelper.CheckIfImmutable(resourceUsage);
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int bufferSizeInBytes = vertexCount * vertexLayout.VertexStride;
            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = elementCount * elemSizeInBytes;
            int vertexStep = vertexStride;

            if (vertexStride != 0)
            {
                vertexStep -= elemSizeInBytes;

                if (vertexStep < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(vertexStride), "Vertex stride too small");
                }

                // If we get this far, we need to make sure the actual bytes we're going to look at matches up since we can grab parts of a vertex
                // and not the whole thing
                if (elementCount > 1)
                {
                    dataSizeInBytes = ((elementCount - 1) * vertexStep) + dataSizeInBytes;
                }
            }

            // Prevent overflow out of range errors
            if (offsetInBytes < 0 || offsetInBytes > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes), "Byte offset out of range");
            }

            if ((offsetInBytes + dataSizeInBytes) > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Byte offset and count overflow");
            }

            var region = new D3D11.ResourceRegion(offsetInBytes, 0, 0, offsetInBytes + dataSizeInBytes, 1, 1);

            lock (_sync)
            {
                // Map directly to dynamic resources
                if (resourceUsage == ResourceUsage.Dynamic)
                {
                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(nativeBuffer, 0, Direct3DHelper.ToD3DMapMode(writeOptions), D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {

                            // If no step, then just do a straight copy
                            if (vertexStep == 0)
                            {
                                MemoryHelper.CopyMemory(dataBox.DataPointer + offsetInBytes, dbPtr + (startIndex * elemSizeInBytes), dataSizeInBytes);
                                // Otherwise need to go element by element, incrementing by the vertex step
                            }
                            else
                            {
                                IntPtr srcPtr = dbPtr.Pointer;
                                IntPtr dstPtr = dataBox.DataPointer + offsetInBytes;
                                for (int i = 0; i < elementCount; i++)
                                {
                                    MemoryHelper.CopyMemory(dstPtr, srcPtr + ((i + startIndex) * elemSizeInBytes), elemSizeInBytes);
                                    dstPtr += vertexStep;
                                }
                            }
                        }
                    }
                    finally
                    {
                        context.UnmapSubresource(nativeBuffer, 0);
                    }
                }
                else
                {
                    // If no vertex step, then do an update subresource as normal
                    if (vertexStep == 0)
                    {
                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            context.UpdateSubresource(new SDX.DataBox(dbPtr + (startIndex * elemSizeInBytes), bufferSizeInBytes, bufferSizeInBytes), nativeBuffer, 0, region);
                        }

                        // Otherwise need to create a staging resource, copy to it, write each element, then copy back
                    }
                    else
                    {
                        using (D3D11.Resource staging = CreateStaging(context.Device, nativeBuffer))
                        {
                            // Need to first copy the affected vertices, so we don't have any gaps when we copy back
                            context.CopySubresourceRegion(nativeBuffer, 0, region, staging, 0, offsetInBytes, 0, 0);

                            try
                            {
                                SDX.DataBox dataBox = context.MapSubresource(staging, 0, D3D11.MapMode.Write, D3D11.MapFlags.None);

                                using (MappedDataBuffer dbPtr = data.Map())
                                {
                                    // Need to go element by element
                                    IntPtr srcPtr = dbPtr.Pointer;
                                    IntPtr dstPtr = dataBox.DataPointer + offsetInBytes;
                                    for (int i = 0; i < elementCount; i++)
                                    {
                                        MemoryHelper.CopyMemory(dstPtr, srcPtr + ((i + startIndex) * elemSizeInBytes), elemSizeInBytes);
                                        dstPtr += vertexStep;
                                    }
                                }

                                // And copy back to the buffer at the end
                                context.CopySubresourceRegion(staging, 0, region, nativeBuffer, 0, offsetInBytes, 0, 0);
                            }
                            finally
                            {
                                context.UnmapSubresource(staging, 0);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read vertex data from a D3D11 Buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="vertexCount">The number of vertices in the buffer.</param>
        /// <param name="vertexLayout">The vertex layout defining the contents of the buffer.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        /// <param name="elementCount">The number of elements to copy.</param>
        /// <param name="offsetInBytes">Offset from the start of the native buffer at which to start copying from</param>
        /// <param name="vertexStride">Size of an element in bytes.</param>
        public static void ReadVertexData<T>(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int vertexCount, VertexLayout vertexLayout, ResourceUsage resourceUsage,
            IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, int vertexStride) where T : struct
        {
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int bufferSizeInBytes = vertexCount * vertexLayout.VertexStride;
            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = elementCount * elemSizeInBytes;
            int vertexStep = vertexStride;

            if (vertexStride != 0)
            {
                vertexStep -= elemSizeInBytes;

                if (vertexStep < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(vertexStride), "Vertex stride too small");
                }

                // If we get this far, we need to make sure the actual bytes we're going to look at matches up since we can grab parts of a vertex
                // and not the whole thing
                if (elementCount > 1)
                {
                    dataSizeInBytes = ((elementCount - 1) * vertexStep) + dataSizeInBytes;
                }
            }

            // Prevent overflow out of range errors
            if (offsetInBytes < 0 || offsetInBytes > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes), "Byte offset out of range");
            }

            if ((offsetInBytes + dataSizeInBytes) > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Byte offset and count overflow");
            }

            var region = new D3D11.ResourceRegion(offsetInBytes, 0, 0, offsetInBytes + dataSizeInBytes, 1, 1);

            lock (_sync)
            {
                // Reading data will always mean creating the staging resource
                using (D3D11.Resource staging = CreateStaging(context.Device, nativeBuffer))
                {
                    context.CopySubresourceRegion(nativeBuffer, 0, region, staging, 0, offsetInBytes, 0, 0);

                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(staging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            // If no vertex step, do a straight  copy as normal
                            if (vertexStep == 0)
                            {
                                MemoryHelper.CopyMemory(dbPtr + (startIndex * elemSizeInBytes), dataBox.DataPointer + offsetInBytes, dataSizeInBytes);
                                // Otherwise need to go element by element
                            }
                            else
                            {
                                IntPtr srcPtr = dataBox.DataPointer + offsetInBytes;
                                IntPtr dstPtr = dbPtr.Pointer;
                                for (int i = 0; i < elementCount; i++)
                                {
                                    MemoryHelper.CopyMemory(dstPtr + ((i + startIndex) * elemSizeInBytes), srcPtr, elemSizeInBytes);
                                    srcPtr += vertexStep;
                                }
                            }
                        }
                    }
                    finally
                    {
                        context.UnmapSubresource(staging, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Write data to a D3D11 Buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="bufferSizeInBytes">The buffer size in bytes.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        /// <param name="elementCount">The number of elements to copy.</param>
        /// <param name="offsetInBytes">Offset from the start of the native buffer at which to start copying from</param>
        /// <param name="writeOptions">Write options for writing.</param>
        public static void WriteBufferData<T>(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int bufferSizeInBytes, ResourceUsage resourceUsage,
            IReadOnlyDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes, DataWriteOptions writeOptions) where T : struct
        {
            Direct3DHelper.CheckIfImmutable(resourceUsage);
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = elementCount * elemSizeInBytes;

            // Prevent overflow out of range errors
            if (offsetInBytes < 0 || offsetInBytes > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes), "Byte offset out of range");
            }

            if ((offsetInBytes + dataSizeInBytes) > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Byte offset and count overflow");
            }

            lock (_sync)
            {
                // Writing means either mapping the dynamic resource or using update subresource.
                if (resourceUsage == ResourceUsage.Dynamic)
                {
                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(nativeBuffer, 0, Direct3DHelper.ToD3DMapMode(writeOptions), D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            MemoryHelper.CopyMemory(dataBox.DataPointer + offsetInBytes, dbPtr + (startIndex * elemSizeInBytes), dataSizeInBytes);
                        }

                    }
                    finally
                    {
                        context.UnmapSubresource(nativeBuffer, 0);
                    }
                }
                else
                {
                    var region = new D3D11.ResourceRegion(offsetInBytes, 0, 0, offsetInBytes + dataSizeInBytes, 1, 1);

                    using (MappedDataBuffer dbPtr = data.Map())
                    {
                        context.UpdateSubresource(new SDX.DataBox(dbPtr + (startIndex * elemSizeInBytes), bufferSizeInBytes, bufferSizeInBytes), nativeBuffer, 0, region);
                    }
                }
            }
        }

        /// <summary>
        /// Read data from a D3D11 Buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeBuffer">The native D3D11 Buffer.</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="bufferSizeInBytes">The buffer size in bytes.</param>
        /// <param name="resourceUsage">The resource usage of the buffer.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        /// <param name="elementCount">The number of elements to copy.</param>
        /// <param name="offsetInBytes">Offset from the start of the native buffer at which to start copying from</param>
        public static void ReadBufferData<T>(D3D11.Buffer nativeBuffer, D3D11.DeviceContext context, int bufferSizeInBytes, ResourceUsage resourceUsage,
            IDataBuffer<T> data, int startIndex, int elementCount, int offsetInBytes) where T : struct
        {
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = elementCount * elemSizeInBytes;

            // Prevent overflow out of range errors
            if (offsetInBytes < 0 || offsetInBytes > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetInBytes), "Byte offset out of range");
            }

            if ((offsetInBytes + dataSizeInBytes) > bufferSizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Byte offset and count overflow");
            }
            
            lock (_sync)
            {
                // Reading means creating a staging resource, copying to it, then mapping it.
                using (D3D11.Resource staging = CreateStaging(context.Device, nativeBuffer))
                {
                    var region = new D3D11.ResourceRegion(offsetInBytes, 0, 0, offsetInBytes + dataSizeInBytes, 1, 1);
                    context.CopySubresourceRegion(nativeBuffer, 0, region, staging, 0, offsetInBytes, 0, 0);

                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(staging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            MemoryHelper.CopyMemory(dbPtr + (startIndex * elemSizeInBytes), dataBox.DataPointer + offsetInBytes, dataSizeInBytes);
                        }

                    }
                    finally
                    {
                        context.UnmapSubresource(staging, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Writes data to a D3D11 Texture resource (1D, 2D, 3D, and array variants).
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeTexture">The D3D11 Texture resource</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="width">The width of the texture (first mip level).</param>
        /// <param name="height">The height of the texture (first mip level).</param>
        /// <param name="depth">The depth of the texture (first mip level).</param>
        /// <param name="arrayCount">The number of array slices.</param>
        /// <param name="mipCount">The number of mip levels.</param>
        /// <param name="format">The surface format of the texture.</param>
        /// <param name="resourceUsage">The resource usage of the texture.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="arraySlice">The array slice to access.</param>
        /// <param name="mipLevel">The mip level to access.</param>
        /// <param name="subimage">The subimage resource region.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        /// <param name="writeOptions">Write options for writing.</param>
        public static void WriteTextureData<T>(D3D11.Resource nativeTexture, D3D11.DeviceContext context, int width, int height, int depth, int arrayCount, int mipCount, SurfaceFormat format, ResourceUsage resourceUsage,
            IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ref ResourceRegion3D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            Direct3DHelper.CheckIfImmutable(resourceUsage);

            Direct3DHelper.CheckMipLevels(mipLevel, mipCount);
            Direct3DHelper.CheckArraySlice(arraySlice, arrayCount);

            // Calc subresource dimensions - Get the dimensions of the mip level to access
            Texture.CalculateMipLevelDimensions(mipLevel, ref width, ref height, ref depth);

            // Check subimage - Get the image region
            ResourceRegion3D subimageValue;
            if (subimage.HasValue)
            {
                subimageValue = subimage.Value;
                subimageValue.ValidateRegion(ref width, ref height, ref depth);
            }
            else
            {
                subimageValue = new ResourceRegion3D(0, width, 0, height, 0, depth);
            }

            // Mostly a null check
            int elementCount = (data != null) ? data.Length - startIndex : 0;
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int formatSizeInBytes = format.SizeInBytes();
            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = Texture.CalculateRegionSizeInBytes(format, ref width, ref height, depth, out formatSizeInBytes);

            // Check total size - Make sure the actual total size in bytes meets what we're expecting
            if (dataSizeInBytes != (elementCount * elemSizeInBytes))
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Invalid size of data to copy");
            }

            Direct3DHelper.CheckFormatSize(formatSizeInBytes, elemSizeInBytes);
            
            int subresourceIndex = Texture.CalculateSubResourceIndex(arraySlice, mipLevel, mipCount);
            int rowStride = width * formatSizeInBytes;
            int depthStride = rowStride * height;
            D3D11.ResourceRegion region = new D3D11.ResourceRegion(subimageValue.Left, subimageValue.Top, subimageValue.Front, subimageValue.Right, subimageValue.Bottom, subimageValue.Back);

            lock (_sync)
            {
                // Writing means either mapping the dynamic resource or using update subresource.
                if (resourceUsage == ResourceUsage.Dynamic)
                {
                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(nativeTexture, subresourceIndex, Direct3DHelper.ToD3DMapMode(writeOptions), D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            IntPtr srcPtr = dbPtr + (startIndex * elemSizeInBytes);
                            IntPtr dstPtr = dataBox.DataPointer + (region.Left * formatSizeInBytes) + (region.Top * dataBox.RowPitch) + (region.Front * dataBox.SlicePitch);

                            // If Texture1D, Texture2D, or TextureCube the depth stride is not needed
                            int boxDepthStride = (depth == 1) ? dataBox.SlicePitch : depthStride;

                            // If same stride, then there is no padding and we can just copy the amount of data in one fell swoop
                            if (dataBox.RowPitch == rowStride && MathHelper.IsApproxEquals(boxDepthStride, depthStride))
                            {
                                MemoryHelper.CopyMemory(dstPtr, srcPtr, dataSizeInBytes);
                                // Otherwise, go by volume slice and scanline by scanline
                            }
                            else
                            {
                                for (int slice = 0; slice < depth; slice++)
                                {
                                    for (int row = 0; row < height; row++)
                                    {
                                        MemoryHelper.CopyMemory(dstPtr, srcPtr, rowStride);

                                        srcPtr += rowStride;
                                        dstPtr += dataBox.RowPitch;
                                    }

                                    dstPtr += dataBox.SlicePitch;
                                }
                            }
                        }
                    }
                    finally
                    {
                        context.UnmapSubresource(nativeTexture, subresourceIndex);
                    }
                }
                else
                {
                    using (MappedDataBuffer dbPtr = data.Map())
                    {
                        context.UpdateSubresource(new SDX.DataBox(dbPtr + (startIndex * elemSizeInBytes), rowStride, depthStride), nativeTexture, subresourceIndex, region);
                    }
                }
            }
        }

        /// <summary>
        /// Reads data from a D3D11 Texture resource (1D, 2D, 3D, and array variants).
        /// </summary>
        /// <typeparam name="T">Type of data to copy</typeparam>
        /// <param name="nativeTexture">The D3D11 Texture resource</param>
        /// <param name="context">The D3D11 device context</param>
        /// <param name="width">The width of the texture (first mip level).</param>
        /// <param name="height">The height of the texture (first mip level).</param>
        /// <param name="depth">The depth of the texture (first mip level).</param>
        /// <param name="arrayCount">The number of array slices.</param>
        /// <param name="mipCount">The number of mip levels.</param>
        /// <param name="format">The surface format of the texture.</param>
        /// <param name="resourceUsage">The resource usage of the texture.</param>
        /// <param name="data">The data buffer to copy data to/from.</param>
        /// <param name="arraySlice">The array slice to access.</param>
        /// <param name="mipLevel">The mip level to access.</param>
        /// <param name="subimage">The subimage resource region.</param>
        /// <param name="startIndex">The start index to read/write to in the data buffer.</param>
        public static void ReadTextureData<T>(D3D11.Resource nativeTexture, D3D11.DeviceContext context, int width, int height, int depth, int arrayCount, int mipCount, SurfaceFormat format, ResourceUsage resourceUsage,
            IDataBuffer<T> data, int arraySlice, int mipLevel, ref ResourceRegion3D? subimage, int startIndex) where T : struct
        {
            Direct3DHelper.CheckMipLevels(mipLevel, mipCount);
            Direct3DHelper.CheckArraySlice(arraySlice, arrayCount);

            // Calc subresource dimensions - Get the dimensions of the mip level to access
            Texture.CalculateMipLevelDimensions(mipLevel, ref width, ref height, ref depth);

            // Check subimage - Get the image region
            ResourceRegion3D subimageValue;
            if (subimage.HasValue)
            {
                subimageValue = subimage.Value;
                subimageValue.ValidateRegion(ref width, ref height, ref depth);
            }
            else
            {
                subimageValue = new ResourceRegion3D(0, width, 0, height, 0, depth);
            }

            // Mostly a null check
            int elementCount = (data != null) ? data.Length - startIndex : 0;
            Direct3DHelper.CheckResourceBounds(data, startIndex, elementCount);

            int formatSizeInBytes = format.SizeInBytes();
            int elemSizeInBytes = data.ElementSizeInBytes;
            int dataSizeInBytes = Texture.CalculateRegionSizeInBytes(format, ref width, ref height, depth, out formatSizeInBytes);

            // Check total size - Make sure the actual total size in bytes meets what we're expecting
            if (dataSizeInBytes != (elementCount * elemSizeInBytes))
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Invalid size of data to copy");
            }

            Direct3DHelper.CheckFormatSize(formatSizeInBytes, elemSizeInBytes);
            
            int subresourceIndex = Texture.CalculateSubResourceIndex(arraySlice, mipLevel, mipCount);
            int rowStride = width * formatSizeInBytes;
            int depthStride = rowStride * height;
            var region = new D3D11.ResourceRegion(subimageValue.Left, subimageValue.Top, subimageValue.Front, subimageValue.Right, subimageValue.Bottom, subimageValue.Back);

            lock (_sync)
            {
                // Reading means creating a staging resource, copying to it, then mapping it.
                using (D3D11.Resource staging = CreateStaging(context.Device, nativeTexture))
                {
                    context.CopySubresourceRegion(nativeTexture, subresourceIndex, region, staging, subresourceIndex, region.Left, region.Top, region.Front);

                    try
                    {
                        SDX.DataBox dataBox = context.MapSubresource(staging, subresourceIndex, D3D11.MapMode.Read, D3D11.MapFlags.None);

                        using (MappedDataBuffer dbPtr = data.Map())
                        {
                            IntPtr srcPtr = dataBox.DataPointer + (region.Left * formatSizeInBytes) + (region.Top * dataBox.RowPitch) + (region.Front * dataBox.SlicePitch);
                            IntPtr dstPtr = dbPtr + (startIndex * elemSizeInBytes);

                            // If Texture1D, Texture2D, or TextureCube the depth stride is not needed
                            int boxDepthStride = (depth == 1) ? dataBox.SlicePitch : depthStride;

                            // If same stride, then there is no padding and we can just copy the amount of data in one fell swoop
                            if (dataBox.RowPitch == rowStride && MathHelper.IsApproxEquals(boxDepthStride, depthStride))
                            {
                                MemoryHelper.CopyMemory(dstPtr, srcPtr, dataSizeInBytes);
                                // Otherwise, go by volume slice and scanline by scanline
                            }
                            else
                            {
                                for (int slice = 0; slice < depth; slice++)
                                {
                                    for (int row = 0; row < height; row++)
                                    {
                                        MemoryHelper.CopyMemory(dstPtr, srcPtr, rowStride);

                                        srcPtr += dataBox.RowPitch;
                                        dstPtr += rowStride;
                                    }

                                    srcPtr += dataBox.SlicePitch;
                                }
                            }
                        }
                    }
                    finally
                    {
                        context.UnmapSubresource(staging, subresourceIndex);
                    }
                }
            }
        }

        public static D3D11.Resource CreateStaging(D3D11.Device device, D3D11.Resource resource)
        {
            switch (resource.Dimension)
            {
                case D3D11.ResourceDimension.Buffer:
                    {
                        var buffer = resource as D3D11.Buffer;
                        var desc = buffer.Description;
                        return CreateStaging(device, ref desc);
                    }
                case D3D11.ResourceDimension.Texture1D:
                    {
                        var texture = resource as D3D11.Texture1D;
                        var desc = texture.Description;
                        return CreateStaging(device, ref desc);
                    }
                case D3D11.ResourceDimension.Texture2D:
                    {
                        var texture = resource as D3D11.Texture2D;
                        var desc = texture.Description;
                        return CreateStaging(device, ref desc);
                    }
                case D3D11.ResourceDimension.Texture3D:
                    {
                        var texture = resource as D3D11.Texture3D;
                        var desc = texture.Description;
                        return CreateStaging(device, ref desc);
                    }
                default:
                    throw new SparkGraphicsException("Unknown resource for staging resource creation"); // Shouldn't happen
            }
        }

        public static D3D11.Buffer CreateStaging(D3D11.Device device, ref D3D11.BufferDescription bufferDesc)
        {
            var desc = new D3D11.BufferDescription
            {
                BindFlags = D3D11.BindFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.Read | D3D11.CpuAccessFlags.Write,
                OptionFlags = D3D11.ResourceOptionFlags.None,
                SizeInBytes = bufferDesc.SizeInBytes,
                Usage = D3D11.ResourceUsage.Staging
            };

            return new D3D11.Buffer(device, desc);
        }

        public static D3D11.Texture1D CreateStaging(D3D11.Device device, ref D3D11.Texture1DDescription texDesc)
        {
            var desc = new D3D11.Texture1DDescription
            {
                ArraySize = texDesc.ArraySize,
                MipLevels = texDesc.MipLevels,
                Width = texDesc.Width,
                Usage = D3D11.ResourceUsage.Staging,
                Format = texDesc.Format,
                BindFlags = D3D11.BindFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.Read | D3D11.CpuAccessFlags.Write,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            return new D3D11.Texture1D(device, desc);
        }

        public static D3D11.Texture2D CreateStaging(D3D11.Device device, ref D3D11.Texture2DDescription texDesc)
        {
            var desc = new D3D11.Texture2DDescription
            {
                ArraySize = texDesc.ArraySize,
                MipLevels = texDesc.MipLevels,
                Width = texDesc.Width,
                Height = texDesc.Height,
                Format = texDesc.Format,
                Usage = D3D11.ResourceUsage.Staging,
                BindFlags = D3D11.BindFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.Read | D3D11.CpuAccessFlags.Write,
                SampleDescription = new SDX.DXGI.SampleDescription(1, 0),
                OptionFlags = texDesc.OptionFlags
            };

            return new D3D11.Texture2D(device, desc);
        }

        public static D3D11.Texture3D CreateStaging(D3D11.Device device, ref D3D11.Texture3DDescription texDesc)
        {
            var desc = new D3D11.Texture3DDescription
            {
                MipLevels = texDesc.MipLevels,
                Width = texDesc.Width,
                Height = texDesc.Height,
                Depth = texDesc.Depth,
                Format = texDesc.Format,
                Usage = D3D11.ResourceUsage.Staging,
                BindFlags = D3D11.BindFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.Read | D3D11.CpuAccessFlags.Write,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            return new D3D11.Texture3D(device, desc);
        }

        public static IntPtr[] MapDataBuffers(params IReadOnlyDataBuffer[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var ptrs = new IntPtr[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];
                IntPtr ptr = IntPtr.Zero;

                if (db != null)
                {
                    ptr = db.Map().Pointer;
                }

                ptrs[i] = ptr;
            }

            return ptrs;
        }

        public static SDX.DataBox[] MapDataBuffers(SurfaceFormat format, int arrayCount, int mipCount, int width, int height, params IReadOnlyDataBuffer[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var ptrs = new SDX.DataBox[data.Length];

            for (int i = 0; i < arrayCount; i++)
            {
                for (int j = 0; j < mipCount; j++)
                {
                    int index = Texture.CalculateSubResourceIndex(i, j, mipCount);
                    IReadOnlyDataBuffer db = data[index];

                    if (db != null)
                    {
                        int formatSize;
                        int widthCount = width;
                        int heightCount = height;

                        Texture.CalculateMipLevelDimensions(j, ref widthCount, ref heightCount);
                        Texture.CalculateRegionSizeInBytes(format, ref widthCount, ref heightCount, out formatSize);

                        int rowPitch = widthCount * formatSize;
                        int slicePitch = rowPitch * heightCount;
                        ptrs[index] = new SDX.DataBox(db.Map(), rowPitch, slicePitch);
                    }
                }
            }

            return ptrs;
        }

        public static void UnmapDataBuffers(IReadOnlyDataBuffer[] data)
        {
            if (data == null)
            {
                return;
            }

            for (int i = 0; i < data.Length; i++)
            {
                IReadOnlyDataBuffer db = data[i];
                if (db != null && db.IsMapped)
                {
                    db.Unmap();
                }
            }
        }
    }
}
