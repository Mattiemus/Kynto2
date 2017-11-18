namespace Spark.Graphics
{
    using System;

    using Geometry;
    
    /// <summary>
    /// General graphics helper
    /// </summary>
    public static class GraphicsHelper
    {
        /// <summary>
        /// Gets a render system from the service provider.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <returns>Render system in the service provider.</returns>
        public static IRenderSystem GetRenderSystem(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            IRenderSystem renderSystem = serviceProvider.GetService(typeof(IRenderSystem)) as IRenderSystem;
            if (renderSystem == null)
            {
                throw new SparkGraphicsException("No render system has been registered with the engine");
            }

            return renderSystem;
        }

        /// <summary>
        /// Resolves draw parameters for a submesh. This does not validate parameters and is agnostic for indexed and non-indexed drawing (the latter
        /// can ignore the base vertex offset).
        /// </summary>
        /// <param name="meshData">Mesh data.</param>
        /// <param name="range">Range in the mesh geometry buffers.</param>
        /// <param name="offset">Offset in the vertex / index buffer</param>
        /// <param name="count">Number of vertices.</param>
        /// <param name="baseVertexOffset">Offset to add to an index buffer value to read a vertex buffer value.</param>
        public static void GetMeshDrawParameters(MeshData meshData, ref SubMeshRange? range, out int offset, out int count, out int baseVertexOffset)
        {
            offset = 0;
            count = 0;
            baseVertexOffset = 0;

            if (range.HasValue)
            {
                SubMeshRange meshRange = range.Value;
                offset = meshRange.Offset;
                count = meshRange.Count;
                baseVertexOffset = meshRange.BaseVertexOffset;
            }
            else if (meshData != null)
            {
                count = (meshData.UseIndexedPrimitives) ? meshData.IndexCount : meshData.VertexCount;
            }
        }
    }
}
