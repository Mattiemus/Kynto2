namespace Spark.Engine
{
    using Graphics;
    using Scene;

    /// <summary>
    /// Component representing a cylinder
    /// </summary>
    public sealed class CylinderComponent : ShapeComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CylinderComponent"/> class.
        /// </summary>
        public CylinderComponent()
            : this("CylinderComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CylinderComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public CylinderComponent(string name)
            : base(name)
        {
            var cylinderGenerator = new CylinderGenerator();

            cylinderGenerator.TopRadius = 0.0f;

            Mesh.MeshData = new MeshData();
            cylinderGenerator.BuildMeshData(Mesh.MeshData, GenerateOptions.Positions | GenerateOptions.TextureCoordinates);
            Mesh.MeshData.Compile();
        }
    }
}
