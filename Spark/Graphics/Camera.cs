namespace Spark.Graphics
{
    using System;

    using Core;
    using Math;
    using Content;

    /// <summary>
    /// A camera represents how a 3D scene is viewed. It handles updating the view and projection matrices and has a 2D viewport which the 3D scene is
    /// mapped to during rendering. 
    /// </summary>
    public class Camera : ISavable, INamable
    {
        /// <summary>
        /// Occurs when the camera's viewport changes.
        /// </summary>
        public event TypedEventHandler<Camera> ViewportChanged;

        /// <summary>
        /// Occurs when the camera is updated.
        /// </summary>
        public event TypedEventHandler<Camera> Updated;
        
        private Matrix4x4 _view;
        private Matrix4x4 _proj;
        private Matrix4x4 _viewProj;
        private Viewport _viewport;
        private Vector3 _position;
        private Vector3 _direction;
        private Vector3 _up;
        private Vector3 _right;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera()
        {
            Name = "Camera";
            _viewport = new Viewport(0, 0, 1, 1);
            _position = new Vector3(0, 0, 0);
            _right = Vector3.Right;
            _up = Vector3.Up;
            _direction = Vector3.Forward;
            ProjectionMode = ProjectionMode.Perspective;
            Frustum = new BoundingFrustum();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="viewport">Camera viewport</param>
        public Camera(Viewport viewport)
        {
            Name = "Camera";
            _viewport = viewport;
            _position = new Vector3(0, 0, 0);
            _right = Vector3.Right;
            _up = Vector3.Up;
            _direction = Vector3.Forward;
            ProjectionMode = ProjectionMode.Perspective;
            Frustum = new BoundingFrustum();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="name">Name of the camera</param>
        public Camera(string name)
        {
            Name = name;
            _viewport = new Viewport(0, 0, 1, 1);
            _position = new Vector3(0, 0, 0);
            _right = Vector3.Right;
            _up = Vector3.Up;
            _direction = Vector3.Forward;
            ProjectionMode = ProjectionMode.Perspective;
            Frustum = new BoundingFrustum();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="name">Name of the camera</param>
        /// <param name="viewport">Camera viewport</param>
        public Camera(string name, Viewport viewport)
        {
            Name = name;
            _viewport = viewport;
            _position = new Vector3(0, 0, 0);
            _right = Vector3.Right;
            _up = Vector3.Up;
            _direction = Vector3.Forward;
            ProjectionMode = ProjectionMode.Perspective;
            Frustum = new BoundingFrustum();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="camToClone">Camera to copy from</param>
        public Camera(Camera camToClone)
        {
            Frustum = new BoundingFrustum();
            Set(camToClone);
        }

        /// <summary>
        /// Gets or sets the name of the camera.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix4x4 ViewMatrix => _view;

        /// <summary>
        /// Gets the projection matrix of the camera.
        /// </summary>
        public Matrix4x4 ProjectionMatrix => _proj;

        /// <summary>
        /// Gets the View x Projection matrix.
        /// </summary>
        public Matrix4x4 ViewProjectionMatrix => _viewProj;

        /// <summary>
        /// Gets or sets the viewport associated with the camera.
        /// </summary>
        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                _viewport = value;
                OnViewportChanged();
            }
        }

        /// <summary>
        /// Gets or sets the position of the camera in world space.
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Gets or sets the direction the camera is facing in (forward vector of the camera's frame). This will be normalized automatically.
        /// </summary>
        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _direction.Normalize();
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the camera's frame. This will be normalized automatically.
        /// </summary>
        public Vector3 Right
        {
            get => _right;
            set
            {
                _right = value;
                _right.Normalize();
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the camera's frame. This will be normalized automatically.
        /// </summary>
        public Vector3 Up
        {
            get => _up;
            set
            {
                _up = value;
                _up.Normalize();
            }
        }

        /// <summary>
        /// Gets the projection mode of the camera. This can be set in the SetProjection methods.
        /// </summary>
        public ProjectionMode ProjectionMode { get; private set; }

        /// <summary>
        /// Gets the bounding frustum of the camera. This represents the visible space of the camera and can be used
        /// for a number of containment and intersection queries.
        /// </summary>
        public BoundingFrustum Frustum { get; }

        /// <summary>
        /// Sets this camera to match the values of the source camera.
        /// </summary>
        /// <param name="source">Source camera to copy from</param>
        public virtual void Set(Camera source)
        {
            Name = source.Name;
            _viewport = source.Viewport;
            _position = source.Position;
            _right = source.Right;
            _up = source.Up;
            _direction = source.Direction;
            ProjectionMode = source.ProjectionMode;
            _proj = source.ProjectionMatrix;
            _view = source.ViewMatrix;

            Update();
        }

        /// <summary>
        /// Set the camera's projection, either an orthographic camera or a perspective camera.
        /// </summary>
        /// <param name="mode">Projection mode, perspective or orthographic</param>
        /// <param name="width">Width of the view volume at the near view plane</param>
        /// <param name="height">Height of the view volume at the near view plane</param>
        /// <param name="near">Distance to the near view plane (or min-Z value if ortho)</param>
        /// <param name="far">Distance to the far view plane (or max-Z value if ortho)</param>
        public void SetProjection(ProjectionMode mode, float width, float height, float near, float far)
        {
            if (mode == ProjectionMode.Perspective)
            {
                _proj = Matrix4x4.CreatePerspectiveMatrix(width, height, near, far);
            }
            else
            {
                _proj = Matrix4x4.CreateOrthographicMatrix(width, height, near, far);
            }

            ProjectionMode = mode;
        }

        /// <summary>
        /// Sets the camera's projection. This will always create a perspective camera.
        /// </summary>
        /// <param name="fieldOfView">Field of view, in degrees</param>
        /// <param name="near">Distance to the near view plane</param>
        /// <param name="far">Distance to the far view plane</param>
        public void SetProjection(float fieldOfView, float near, float far)
        {
            _proj = Matrix4x4.CreatePerspectiveFOVMatrix(Angle.FromDegrees(fieldOfView), _viewport.AspectRatio, near, far);
            ProjectionMode = ProjectionMode.Perspective;
        }

        /// <summary>
        /// Sets the camera's projection, either an orthographic camera or a perspective camera.
        /// </summary>
        /// <param name="mode">Projection mode, perspective or orthographic</param>
        /// <param name="left">Minimum x value of the view volume at the near view plane</param>
        /// <param name="right">Maximum x value of the view volume at the near view plane</param>
        /// <param name="bottom">Minimum y value of the view volume at the near view plane</param>
        /// <param name="top">Maximum y value of the view volume at the near view plane</param>
        /// <param name="near">Distance to the near view plane (or min-Z value if ortho)</param>
        /// <param name="far">Distance to the far view plane (or max-Z value if ortho)</param>
        public void SetProjection(ProjectionMode mode, float left, float right, float bottom, float top, float near, float far)
        {
            if (mode == ProjectionMode.Perspective)
            {
                _proj = Matrix4x4.CreatePerspectiveMatrix(left, right, bottom, top, near, far);
            }
            else
            {
                _proj = Matrix4x4.CreateOrthographicMatrix(left, right, bottom, top, near, far);
            }

            ProjectionMode = mode;
        }

        /// <summary>
        /// Sets the axes of the camera's frame. This will normalize the axes.
        /// </summary>
        /// <param name="right">Right vector</param>
        /// <param name="up">Up vector</param>
        /// <param name="direction">Forward vector</param>
        public void SetAxes(Vector3 right, Vector3 up, Vector3 direction)
        {
            _right = right;
            _right.Normalize();

            _up = up;
            _up.Normalize();

            _direction = direction;
            _direction.Normalize();
        }

        /// <summary>
        /// Sets the axes of the camera's frame from a quaternion rotation. This will normalize the axes.
        /// </summary>
        /// <param name="axes">Quaternion rotation</param>
        public void SetAxes(Quaternion axes)
        {
            Matrix4x4.FromQuaternion(ref axes, out Matrix4x4 temp);

            _right = temp.Right;
            _right.Normalize();

            _up = temp.Up;
            _up.Normalize();

            _direction = temp.Forward;
            _direction.Normalize();
        }

        /// <summary>
        /// Sets the axes of the camera's frame from a matrix rotation. This will normalize the axes.
        /// </summary>
        /// <param name="axes">Matrix rotation</param>
        public void SetAxes(Matrix4x4 axes)
        {
            _right = axes.Right;
            _right.Normalize();

            _up = axes.Up;
            _up.Normalize();

            _direction = axes.Forward;
            _direction.Normalize();
        }

        /// <summary>
        /// Set's the camera's frame. This will normalize the axes.
        /// </summary>
        /// <param name="position">Camera position</param>
        /// <param name="right">Right vector</param>
        /// <param name="up">Up vector</param>
        /// <param name="direction">Forward vector</param>
        public void SetFrame(Vector3 position, Vector3 right, Vector3 up, Vector3 direction)
        {
            _position = position;

            _right = right;
            _right.Normalize();

            _up = up;
            _up.Normalize();

            _direction = direction;
            _direction.Normalize();
        }

        /// <summary>
        /// Sets the camera's frame from the Quaternion rotation. This will normalize the axes.
        /// </summary>
        /// <param name="position">Camera position</param>
        /// <param name="axes">Quaterion rotation to get the axes from</param>
        public void SetFrame(Vector3 position, Quaternion axes)
        {
            _position = position;
            
            Matrix4x4.FromQuaternion(ref axes, out Matrix4x4 temp);

            _right = temp.Right;
            _right.Normalize();

            _up = temp.Up;
            _up.Normalize();

            _direction = temp.Forward;
            _direction.Normalize();
        }

        /// <summary>
        /// Set's the camera's frame from the Matrix rotation. This will normalize the axes.
        /// </summary>
        /// <param name="position">Camera position</param>
        /// <param name="axes">Matrix rotation to get the axes from</param>
        public void SetFrame(Vector3 position, Matrix4x4 axes)
        {
            _position = position;

            _right = axes.Right;
            _right.Normalize();

            _up = axes.Up;
            _up.Normalize();

            _direction = axes.Forward;
            _direction.Normalize();
        }

        /// <summary>
        /// Updates the camera's view matrix and bounding frustum. This should be called whenever the projection matrix is changed or 
        /// the camera's frame is changed.
        /// </summary>
        public virtual void Update()
        {
            // Temp _position + _direction
            Vector3.Add(ref _position, ref _direction, out Vector3 posd);

            // Create the new view matrix
            Matrix4x4.CreateViewMatrix(ref _position, ref posd, ref _up, out _view);

            // Create the View x Projection matrix
            Matrix4x4.Multiply(ref _view, ref _proj, out _viewProj);

            // Update the frustum
            Frustum.Set(ref _viewProj);

            OnUpdated();
        }

        /// <summary>
        /// Makes the camera look at the target.
        /// </summary>
        /// <param name="target">Target vector</param>
        /// <param name="worldUp">The up vector in the world</param>
        public void LookAt(Vector3 target, Vector3 worldUp)
        {
            // Ensure world up is normalized + valid
            worldUp.Normalize();
            if (worldUp.Equals(Vector3.Zero))
            {
                worldUp = Vector3.Up;
            }

            // Find the direction
            Vector3.Subtract(ref _position, ref target, out Vector3 backward);
            backward.Normalize();
            Vector3.Negate(ref backward, out _direction);

            // Find right axis
            Vector3.Cross(ref worldUp, ref backward, out _right);
            _right.Normalize();

            // Find local up axis
            Vector3.Cross(ref backward, ref _right, out _up);

            // Update the view matrix
            Update();
        }

        /// <summary>
        /// Gets the world position based on the x,y screen coordinates.
        /// </summary>
        /// <param name="screenPosition">Screen position (x,y) and z being the depth (0 - near, 1 - far planes)</param>
        /// <returns>Constructed world position</returns>
        public Vector3 GetWorldCoordinates(Vector3 screenPosition)
        {
            GetWorldCoordinates(ref screenPosition, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets the world position based on the x,y screen position.
        /// </summary>
        /// <param name="screenPosition">Screen position (x,y) and z being the depth (0 - near, 1 - far planes)</param>
        /// <param name="result">Result to store constructed world position</param>
        public void GetWorldCoordinates(ref Vector3 screenPosition, out Vector3 result)
        {
            Matrix4x4 world = Matrix4x4.Identity;

            _viewport.UnProject(ref screenPosition, ref world, ref _view, ref _proj, out result);
        }

        /// <summary>
        /// Gets the screen position based on the world position.
        /// </summary>
        /// <param name="worldPosition">World position</param>
        /// <returns>Screen position</returns>
        public Vector2 GetScreenCoordinates(Vector3 worldPosition)
        {
            GetScreenCoordinates(ref worldPosition, out Vector2 result);
            return result;
        }

        /// <summary>
        /// Gets the screen position based on the world position.
        /// </summary>
        /// <param name="worldPosition">World position</param>
        /// <param name="result">Result to store screen position in</param>
        public void GetScreenCoordinates(ref Vector3 worldPosition, out Vector2 result)
        {
            // Treat the position as an absolute world position
            Matrix4x4 world = Matrix4x4.Identity;

            // Unproject it to get screen coordinates
            _viewport.Project(ref worldPosition, ref world, ref _view, ref _proj, out Vector3 temp);

            result.X = temp.X;
            result.Y = temp.Y;
        }

        /// <summary>
        /// Creates a ray to do picking tests, where the origin is on the near plane. This is the same as using
        /// GetWorldCoordinates for (x,y,0) and (x,y,1) and using the normalized difference as the ray's direction.
        /// </summary>
        /// <param name="screenPos">Screen position (x,y) and z being the depth (0 - near, 1 - far planes)</param>
        /// <returns>Resulting ray</returns>
        public Ray CreatePickRay(Vector2 screenPos)
        {
            CreatePickRay(ref screenPos, out Ray result);
            return result;
        }

        /// <summary>
        /// Creates a ray to do picking tests, where the origin is on the near plane. This is the same as using
        /// GetWorldCoordinates for (x,y,0) and (x,y,1) and using the normalized difference as the ray's direction.
        /// </summary>
        /// <param name="screenPos">Screen position (x,y) and z being the depth (0 - near, 1 - far planes)</param>
        /// <param name="result">Ray containing the result</param>
        public void CreatePickRay(ref Vector2 screenPos, out Ray result)
        {
            Vector3 pos1 = new Vector3(screenPos.X, screenPos.Y, 0);
            Vector3 pos2 = new Vector3(screenPos.X, screenPos.Y, 1);

            Matrix4x4 world = Matrix4x4.Identity;
            
            _viewport.UnProject(ref pos1, ref world, ref _view, ref _proj, out Vector3 posNear);
            _viewport.UnProject(ref pos2, ref world, ref _view, ref _proj, out Vector3 posFar);
            
            Vector3.Subtract(ref posFar, ref posNear, out Vector3 dir);
            dir.Normalize();

            result.Direction = dir;
            result.Origin = posNear;
        }

        /// <summary>
        /// Serializes the object and writes it to the output.
        /// </summary>
        /// <param name="output">Savable Output</param>
        public virtual void Write(ISavableWriter output)
        {
            output.Write("Name", Name);
            output.Write("Viewport", new Vector4(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height));
            output.Write("ViewportDepth", new Vector2(_viewport.MinDepth, _viewport.MaxDepth));
            output.Write("Position", _position);
            output.Write("Up", _up);
            output.Write("Direction", _direction);
            output.WriteEnum("ProjectionMode", ProjectionMode);
            output.Write("ProjectionMatrix", _proj);
        }

        /// <summary>
        /// Deserializes the object and populates it from the input.
        /// </summary>
        /// <param name="input">Savable input</param>
        public virtual void Read(ISavableReader input)
        {
            Name = input.ReadString();
            Vector4 vp = input.Read<Vector4>();
            Vector2 depth = input.Read<Vector2>();
            _viewport = new Viewport((int)vp.X, (int)vp.Y, (int)vp.Z, (int)vp.W);
            _viewport.MinDepth = depth.X;
            _viewport.MaxDepth = depth.Y;

            _position = input.Read<Vector3>();
            _up = input.Read<Vector3>();
            _direction = input.Read<Vector3>();
            ProjectionMode = input.ReadEnum<ProjectionMode>();
            _proj = input.Read<Matrix4x4>();

            Update();
        }

        /// <summary>
        /// Invokes the viewport changed event
        /// </summary>
        private void OnViewportChanged()
        {
            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the updated event
        /// </summary>
        private void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
