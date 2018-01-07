namespace Spark.Engine
{
    using System;

    using Math;
    using Scene;

    /// <summary>
    /// A scene component represents a component which has a transform and have support for attatchments
    /// </summary>
    public class SceneComponent : Component
    {
        private Spatial _sceneNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneComponent"/> class.
        /// </summary>
        public SceneComponent()
            : this("SceneComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public SceneComponent(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the parent scene component, or null if this is a top level component
        /// </summary>
        public SceneComponent ParentComponent { get; }
        
        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public Transform Transform
        {
            get => SceneNode.Transform;
            set => SceneNode.Transform = value;
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public Vector3 Scale
        {
            get => SceneNode.Scale;
            set => SceneNode.Scale = value;
        }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public Quaternion Rotation
        {
            get => SceneNode.Rotation;
            set => SceneNode.Rotation = value;
        }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public Vector3 Translation
        {
            get => SceneNode.Translation;
            set => SceneNode.Translation = value;
        }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public Transform WorldTransform => SceneNode.WorldTransform;

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public Vector3 WorldScale => SceneNode.WorldTransform.Scale;

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public Quaternion WorldRotation => SceneNode.WorldTransform.Rotation;

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public Vector3 WorldTranslation => SceneNode.WorldTransform.Translation;

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public Matrix4x4 WorldMatrix => SceneNode.WorldTransform.Matrix;

        /// <summary>
        /// Gets the world bounding volume of the entity
        /// </summary>
        public BoundingVolume WorldBounding => SceneNode.WorldBounding;

        /// <summary>
        /// Gets the scene node
        /// </summary>
        internal Spatial SceneNode
        {
            get
            {
                if (_sceneNode == null)
                {
                    _sceneNode = CreateSceneNode();
                }

                return _sceneNode;
            }
        }
        
        /// <summary>
        /// Determines if given component can be attached as a child
        /// </summary>
        /// <param name="component">Component to be attached</param>
        /// <returns>True if the component can be attached as a child, false otherwise</returns>
        public virtual bool CanAttachAsChild(SceneComponent component)
        {
            return true;
        }

        /// <summary>
        /// Attaches this component to another scene component
        /// </summary>
        /// <param name="parent">Parent scene component to attach to</param>
        public void AttachToComponent(SceneComponent parent)
        {
            if (!parent.CanAttachAsChild(this))
            {
                throw new SparkException("Cannot attatch to parent");
            }

            var parentNode = parent.SceneNode as Node;
            if (parentNode == null)
            {
                throw new ArgumentException("Parent component does not accept attatchments", nameof(parent));
            }

            parentNode.Children.Add(SceneNode);
        }

        /// <summary>
        /// Deatches this component from its parent
        /// </summary>
        public void DetachFromParent()
        {
            var parentNode = ParentComponent.SceneNode as Node;
            if (parentNode == null)
            {
                throw new InvalidOperationException("Parent component does not accept attatchments");
            }

            parentNode.Children.Remove(SceneNode);
        }
        
        /// <summary>
        /// Creates the scene node for the component
        /// </summary>
        /// <returns>Scene node representing the component</returns>
        protected virtual Spatial CreateSceneNode()
        {
            return new Node(Name);
        }

        /// <summary>
        /// Invoked when the components name is changed
        /// </summary>
        protected override void OnNameChanged()
        {
            SceneNode.Name = Name;
        }
    }
}
