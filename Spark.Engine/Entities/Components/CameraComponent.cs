namespace Spark.Engine
{
    using System;

    using Math;
    using Graphics;

    public class CameraComponent : SceneComponent, IBehavior
    {
        private Camera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraComponent"/> class.
        /// </summary>
        public CameraComponent()
            : this("CameraComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        public CameraComponent(string name)
            : base(name)
        {
            _camera = CreateCamera();
        }

        /// <summary>
        /// Gets or sets the camera contained by the component
        /// </summary>
        public Camera Camera
        {
            get => _camera;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _camera = value;
            }
        }

        /// <summary>
        /// Gets or sets the update priority. Smaller values represent a higher priority.
        /// </summary>
        public int UpdatePriority { get; set; }
        
        /// <summary>
        /// Gets or sets the local transform
        /// </summary>
        public override Transform Transform
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public override Vector3 Scale
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
    }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public override Quaternion Rotation
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the local translation
        /// </summary>
        public override Vector3 Translation
        {
            get => Camera.Position;
            set => Camera.Position = value;
        }

        /// <summary>
        /// Gets the world transform
        /// </summary>
        public override Transform WorldTransform => throw new NotImplementedException();

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public override Vector3 WorldScale => throw new NotImplementedException();

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public override Quaternion WorldRotation => throw new NotImplementedException();

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public override Vector3 WorldTranslation => throw new NotImplementedException();

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public override Matrix4x4 WorldMatrix => throw new NotImplementedException();

        /// <summary>
        /// Gets the world bounding volume of the entity
        /// </summary>
        public override BoundingVolume WorldBounding => throw new NotImplementedException();

        /// <summary>
        /// Performs the logic update.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last update.</param>
        public void Update(IGameTime gameTime)
        {
            Camera.Update();
        }

        /// <summary>
        /// Creates the default camera instance
        /// </summary>
        /// <returns>Camera instance</returns>
        private Camera CreateCamera()
        {
            var cam = new Camera();
            cam.Viewport = new Viewport(0, 0, 1, 1);
            cam.SetProjection(45, 1, 10000000);
            cam.Position = Vector3.Backward;
            cam.LookAt(Vector3.Zero, Vector3.Up);
            
            return cam;
        }
    }
}
