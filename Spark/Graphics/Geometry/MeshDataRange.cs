namespace Spark.Graphics.Geometry
{
    public struct MeshDataRange
    {
        public MeshDataRange(MeshData meshData)
        {
            MeshData = meshData;
            StartIndex = 0;
            Count = meshData.VertexCount;
        }

        public MeshDataRange(MeshData meshData, int startIndex, int count)
        {
            MeshData = meshData;
            StartIndex = startIndex;
            Count = count;
        }

        public MeshData MeshData { get; set; }

        public int StartIndex { get; set; }

        public int Count { get; set; }
        
        public void Draw(IRenderContext renderContext)
        {
            if (MeshData == null || renderContext == null || Count <= 0)
            {
                return;
            }

            renderContext.SetVertexBuffer(MeshData.VertexBuffer);

            if (MeshData.UseIndexedPrimitives)
            {
                renderContext.SetIndexBuffer(MeshData.IndexBuffer);
                renderContext.DrawIndexed(MeshData.PrimitiveType, Count, StartIndex, 0);
            }
            else
            {
                renderContext.Draw(MeshData.PrimitiveType, Count, StartIndex);
            }
        }
    }
}
