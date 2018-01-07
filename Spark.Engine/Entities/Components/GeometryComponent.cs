namespace Spark.Engine
{
    using Math;
    using Scene;
    using Graphics;

    public class GeometryComponent : SceneComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryComponent"/> class.
        /// </summary>
        public GeometryComponent()
            : this("GeometryComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public GeometryComponent(string name)
            : base(name)
        {            
        }
        
        public MaterialDefinition MaterialDefinition
        {
            get => Mesh.MaterialDefinition;
            set => Mesh.MaterialDefinition = value;
        }

        public MeshData MeshData
        {
            get => Mesh.MeshData;
            set => Mesh.MeshData = value;
        }

        protected Mesh Mesh => SceneNode as Mesh;

        /// <summary>
        /// Determines if given component can be attached as a child
        /// </summary>
        /// <param name="component">Component to be attached</param>
        /// <returns>True if the component can be attached as a child, false otherwise</returns>
        public override bool CanAttachAsChild(SceneComponent component)
        {
            return false;
        }

        /// <summary>
        /// Creates the scene node for the component
        /// </summary>
        /// <returns>Scene node representing the component</returns>
        protected override Spatial CreateSceneNode()
        {
            var mesh = new Mesh(Name);
            mesh.SetModelBounding(new BoundingBox(), true);
            CreateGeometry(mesh);
            return mesh;
        }

        protected virtual void CreateGeometry(Mesh mesh)
        {
            // No-op
        }
    }
}
