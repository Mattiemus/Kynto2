namespace Spark.Entities
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
        /// Performs the logic update.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last update.</param>
        public void Update(IGameTime gameTime)
        {
            Camera.Position = Translation;
            Camera.Update();
        }

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
