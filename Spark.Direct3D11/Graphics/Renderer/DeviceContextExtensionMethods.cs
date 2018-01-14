namespace Spark.Direct3D11.Graphics.Renderer
{
    using System;

    using D3D11 = SharpDX.Direct3D11;

    internal static class DeviceContextExtensionMethods
    {
        public unsafe static void SetVertexBuffers(this D3D11.InputAssemblerStage inputAssembler, int startSlot, int numBuffers, params D3D11.VertexBufferBinding[] vertexBufferBindings)
        {
            IntPtr* vertexBuffers = stackalloc IntPtr[numBuffers];
            int* stridesPtr = stackalloc int[numBuffers];
            int* offsetsPtr = stackalloc int[numBuffers];

            for (int i = 0; i < numBuffers; i++)
            {
                D3D11.VertexBufferBinding binding = vertexBufferBindings[i];

                vertexBuffers[i] = (binding.Buffer == null) ? IntPtr.Zero : binding.Buffer.NativePointer;
                stridesPtr[i] = binding.Stride;
                offsetsPtr[i] = binding.Offset;
            }

            inputAssembler.SetVertexBuffers(startSlot, numBuffers, new IntPtr(vertexBuffers), new IntPtr(stridesPtr), new IntPtr(offsetsPtr));
        }
    }
}
