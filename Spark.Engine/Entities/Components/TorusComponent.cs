namespace Spark.Engine
{
    using Math;
    using Graphics;
    using Scene;

    /// <summary>
    /// Component representing a torus
    /// </summary>
    public sealed class TorusComponent : ShapeComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorusComponent"/> class.
        /// </summary>
        public TorusComponent()
            : this("TorusComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TorusComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public TorusComponent(string name)
            : base(name)
        {
            var torusGenerator = new TorusGenerator(Vector3.Zero, 10.0f, 0.5f, 24);
            Mesh.MeshData = new MeshData();
            torusGenerator.BuildMeshData(Mesh.MeshData, GenerateOptions.Positions | GenerateOptions.TextureCoordinates);
            Mesh.MeshData.Compile();
        }
    }
}
