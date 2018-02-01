namespace Spark.Engine
{
    using Math;
    using Scene;

    /// <summary>
    /// A scene component represents a component which has a transform and have support for attatchments
    /// </summary>
    public abstract class SceneComponent : Component, ITransformed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneComponent"/> class.
        /// </summary>
        protected SceneComponent()
            : this("SceneComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        protected SceneComponent(string name)
            : base(name)
        {
        }
                
        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public abstract Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public abstract Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public abstract Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public abstract Vector3 Translation { get; set; }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public abstract Transform WorldTransform { get; }

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public abstract Vector3 WorldScale { get; }

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public abstract Quaternion WorldRotation { get; }

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public abstract Vector3 WorldTranslation { get; }

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public abstract Matrix4x4 WorldMatrix { get; }

        /// <summary>
        /// Gets the world bounding volume of the entity
        /// </summary>
        public abstract BoundingVolume WorldBounding { get; }
    }
}
