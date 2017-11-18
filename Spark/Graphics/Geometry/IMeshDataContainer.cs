namespace Spark.Graphics
{
    /// <summary>
    /// Defines an object that contains a <see cref="MeshData"/>.
    /// </summary>
    public interface IMeshDataContainer
    {
        /// <summary>
        /// Gets the mesh data.
        /// </summary>
        MeshData MeshData { get; }

        /// <summary>
        /// Gets submesh information, if it exists. This is the range in the geometry buffers of <see cref="MeshData"/> that
        /// make up the mesh, as <see cref="MeshData"/> may contain more than one mesh. If null, then the entire range of <see cref="MeshData"/> 
        /// is a single mesh.
        /// </summary>
        SubMeshRange? MeshRange { get; }
    }
}
