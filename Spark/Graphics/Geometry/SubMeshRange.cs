namespace Spark.Graphics
{
    using Content;

    /// <summary>
    /// Defines a range of vertices or indexed vertices that make up a mesh. 
    /// </summary>
    public struct SubMeshRange : IPrimitiveValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubMeshRange"/> struct.
        /// </summary>
        /// <param name="offset">Offset in the buffer to start reading at.</param>
        /// <param name="count">Number of elements to read.</param>
        public SubMeshRange(int offset, int count)
        {
            Offset = offset;
            Count = count;
            BaseVertexOffset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubMeshRange"/> struct.
        /// </summary>
        /// <param name="offset">Offset in the buffer to start reading at.</param>
        /// <param name="count">Number of elements to read.</param>
        /// <param name="baseVertexOffset">Base vertex offset to add to each index (Only for indexed meshes).</param>
        public SubMeshRange(int offset, int count, int baseVertexOffset)
        {
            Offset = offset;
            Count = count;
            BaseVertexOffset = baseVertexOffset;
        }

        /// <summary>
        /// Gets the offset in the vertex/index buffer to start reading at.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets the number of vertices/indices to read.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the base vertex offset at which to start reading vertices from (indexed meshes only). This is added
        /// to the vertex index in the index buffer, allowing multiple meshes to be contained in a single buffer.
        /// </summary>
        public int BaseVertexOffset { get; private set; }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            Offset = input.ReadInt32();
            Count = input.ReadInt32();
            BaseVertexOffset = input.ReadInt32();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Offset", Offset);
            output.Write("Count", Count);
            output.Write("BaseVertexOffset", BaseVertexOffset);
        }
    }
}
