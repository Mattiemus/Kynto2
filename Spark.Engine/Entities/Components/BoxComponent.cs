namespace Spark.Entities
{
    using Graphics;
    using Scene;

    /// <summary>
    /// Component representing a box
    /// </summary>
    public sealed class BoxComponent : ShapeComponent
    {
        private readonly BoxGenerator _boxGenerator;

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
            _boxGenerator = new BoxGenerator();
        }
        
        protected override void CreateGeometry(Mesh mesh)
        {
            base.CreateGeometry(mesh);
            
            mesh.MeshData = new MeshData();
            _boxGenerator.BuildMeshData(mesh.MeshData, GenerateOptions.Positions);
            mesh.MeshData.Compile();
        }
    }
}
