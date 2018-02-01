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
            Mesh = new Mesh(name);
        }
        
        /// <summary>
        /// Gets or sets the material definition
        /// </summary>
        public MaterialDefinition MaterialDefinition
        {
            get => Mesh.MaterialDefinition;
            set => Mesh.MaterialDefinition = value;
        }

        /// <summary>
        /// Gets or sets the meshes data
        /// </summary>
        public MeshData MeshData
        {
            get => Mesh.MeshData;
            set => Mesh.MeshData = value;
        }

        /// <summary>
        /// Gets the mesh data
        /// </summary>
        public Mesh Mesh { get; }

        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public override Transform Transform
        {
            get => Mesh.Transform;
            set => Mesh.Transform = value;
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public override Vector3 Scale
        {
            get => Mesh.Scale;
            set => Mesh.Scale = value;
        }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public override Quaternion Rotation
        {
            get => Mesh.Rotation;
            set => Mesh.Rotation = value;
        }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public override Vector3 Translation
        {
            get => Mesh.Translation;
            set => Mesh.Translation = value;
        }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public override Transform WorldTransform => Mesh.WorldTransform;

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public override Vector3 WorldScale => Mesh.WorldScale;

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public override Quaternion WorldRotation => Mesh.WorldRotation;

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public override Vector3 WorldTranslation => Mesh.WorldTranslation;

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public override Matrix4x4 WorldMatrix => Mesh.WorldMatrix;

        /// <summary>
        /// Gets the world bounding volume of the entity
        /// </summary>
        public override BoundingVolume WorldBounding => Mesh.WorldBounding;

        /// <summary>
        /// Called when the component is added to the entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public override void OnAttach(Entity parent)
        {
            base.OnAttach(parent);
            parent.Scene.Children.Add(Mesh);
        }

        /// <summary>
        /// Called when the component is removed from an entity.
        /// </summary>
        /// <param name="parent">Entity</param>
        public override void OnRemove(Entity parent)
        {
            base.OnRemove(parent);
            parent.Scene.Children.Remove(Mesh);
        }

        /// <summary>
        /// Invoked when the components name is changed
        /// </summary>
        protected override void OnNameChanged()
        {
            Mesh.Name = Name;
            base.OnNameChanged();
        }
    }
}
