namespace Spark.Engine
{
    using Graphics;
    using Scene;

    /// <summary>
    /// Component representing a box
    /// </summary>
    public sealed class BoxComponent : ShapeComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxComponent"/> class.
        /// </summary>
        public BoxComponent()
            : this("BoxComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public BoxComponent(string name)
            : base(name)
        {
            BoxGenerator boxGenerator = new BoxGenerator();

            Mesh.MeshData = new MeshData();
            boxGenerator.BuildMeshData(Mesh.MeshData, GenerateOptions.Positions | GenerateOptions.TextureCoordinates);
            Mesh.MeshData.Compile();
        }
    }
}
