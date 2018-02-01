namespace Spark.Toolkit.Input
{
    using System;
    using System.Collections.Generic;

    using Spark.Input;
    using Spark.Engine;

    using Graphics;
    using Math;
        
    /// <summary>
    /// A controller that orbits a camera around a target point, at a certain distance away from that point. This is defined by three values, yaw/pitch angles and a zoom distance. The pitch angle
    /// is always limited between (-90, 90) degrees, not inclusive, while the yaw angle can rotate a full 360 degrees. The controller defines a number of methods that take in delta values (e.g. mouse positions) 
    /// to update the controller state.
    /// </summary>
    public class OrbitCameraController : Component, IBehavior
    {
        /// <summary>
        /// Default yaw rotate speed.
        /// </summary>
        public static readonly Angle DefaultYawRotateSpeed = Angle.FromDegrees(20.0f);

        /// <summary>
        /// Default pitch rotate speed.
        /// </summary>
        public static readonly Angle DefaultPitchRotateSpeed = Angle.FromDegrees(20.0f);

        /// <summary>
        /// Default pan speed.
        /// </summary>
        public static readonly float DefaultPanSpeed = 30;

        /// <summary>
        /// Default zoom speed.
        /// </summary>
        public static readonly float DefaultZoomSpeed = 500.0f;

        /// <summary>
        /// Binding name for the Rotate action.
        /// </summary>
        public static readonly string RotateBindingName = "Rotate";

        /// <summary>
        /// Binding name for the Zoom action.
        /// </summary>
        public static readonly string ZoomBindingName = "Zoom";

        /// <summary>
        /// Binding name for the Pan action.
        /// </summary>
        public static readonly string PanBindingName = "Pan";

        /// <summary>
        /// Default input binding names.
        /// </summary>
        public static readonly IReadOnlyList<string> InputBindingNames = new string[] { RotateBindingName, ZoomBindingName, PanBindingName };

        private static readonly Angle MinPitch = Angle.FromDegrees(-89.9f);
        private static readonly Angle MaxPitch = Angle.FromDegrees(89.9f);
        private static readonly float MinZoom = 0.3f;

        private Camera _camera;
        private Vector3 _camPos;
        private Vector3 _targetPoint;
        private float _distance;
        private Angle _yaw;
        private Angle _pitch;

        private Angle _yawRotateSpeed;
        private Angle _pitchRotateSpeed;
        private float _zoomSpeed;
        private float _panSpeed;
        private bool _isDirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitCameraController"/> class.
        /// </summary>
        /// <param name="camera">The camera to control</param>
        public OrbitCameraController(Camera camera)
            : this(camera, null, DefaultYawRotateSpeed, DefaultPitchRotateSpeed, DefaultZoomSpeed, DefaultPanSpeed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitCameraController"/> class.
        /// </summary>
        /// <param name="camera">The camera to control</param>
        /// <param name="lookAtPoint">The look at point to orbit around. Distance is based on the camera's current position and the look at point.</param>
        public OrbitCameraController(Camera camera, Vector3 lookAtPoint) 
            : this(camera, lookAtPoint, DefaultYawRotateSpeed, DefaultPitchRotateSpeed, DefaultZoomSpeed, DefaultPanSpeed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitCameraController"/> class.
        /// </summary>
        /// <param name="camera">The camera to control</param>
        /// <param name="lookAtPoint">Optional look at point to orbit around. Distance is based on the camera's current position and the look at point.</param>
        /// <param name="yawRotateSpeed">The yaw rotate speed.</param>
        /// <param name="pitchRotateSpeed">The pitch rotate speed.</param>
        /// <param name="zoomSpeed">The zoom speed.</param>
        /// <param name="panSpeed">The pan speed.</param>
        public OrbitCameraController(Camera camera, Vector3? lookAtPoint, Angle yawRotateSpeed, Angle pitchRotateSpeed, float zoomSpeed, float panSpeed)
        {
            _camera = camera;
            _camPos = (camera != null) ? camera.Position : Vector3.Zero;
            IsEnabled = true;
            _isDirty = true;

            _distance = MinZoom;
            if (camera != null && lookAtPoint.HasValue)
            {
                _targetPoint = lookAtPoint.Value;
                _camera.LookAt(lookAtPoint.Value, Vector3.Up);
                _camera.Update();

                Vector3.Distance(ref _camPos, ref _targetPoint, out _distance);
            }

            UpdateFromCamera(_distance);

            _yawRotateSpeed = yawRotateSpeed;
            _pitchRotateSpeed = pitchRotateSpeed;
            _panSpeed = panSpeed;
            _zoomSpeed = zoomSpeed;

            InputGroup = new InputGroup(GetType().Name);

            MapControls(null);
        }

        /// <summary>
        /// Gets or sets the camera that the controller manipulates.
        /// </summary>
        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                UpdateFromCamera();
            }
        }

        /// <summary>
        /// Gets the input group the controller creates triggers for.
        /// </summary>
        public InputGroup InputGroup { get; }

        /// <summary>
        /// Gets or sets the look at point that the camera orbits around.
        /// </summary>
        public Vector3 TargetLookAtPoint
        {
            get => _targetPoint;
            set
            {
                _targetPoint = value;
                Distance = Vector3.Distance(_targetPoint, _camera.Position); // Sets is dirty
            }
        }

        /// <summary>
        /// Gets or sets the distance from the look at point to the camera position.
        /// </summary>
        public float Distance
        {
            get => _distance;
            set
            {
                _distance = MathHelper.Clamp(value, MinZoom, float.MaxValue);
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the current yaw angle.
        /// </summary>
        public Angle Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the current pitch angle.
        /// </summary>
        public Angle Pitch
        {
            get => _pitch;
            set
            {

                _pitch = Angle.Clamp(value, MinPitch, MaxPitch);
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the yaw rotate speed.
        /// </summary>
        public Angle YawRotateSpeed
        {
            get => _yawRotateSpeed;
            set => _yawRotateSpeed = Angle.Max(value, Angle.Zero);
        }

        /// <summary>
        /// Gets or sets the pitch rotate speed.
        /// </summary>
        public Angle PitchRotateSpeed
        {
            get => _pitchRotateSpeed;
            set => _pitchRotateSpeed = Angle.Max(value, Angle.Zero);
        }

        /// <summary>
        /// Gets or sets the pan speed.
        /// </summary>
        public float PanSpeed
        {
            get => _panSpeed;
            set => _panSpeed = Math.Max(value, 0.0f);
        }

        /// <summary>
        /// Gets or sets the zoom speed.
        /// </summary>
        public float ZoomSpeed
        {
            get => _zoomSpeed;
            set => _zoomSpeed = Math.Max(value, 0.0f);
        }

        /// <summary>
        /// Gets or sets if the controller is active.
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets the update priority. Smaller values represent a higher priority.
        /// </summary>
        public int UpdatePriority { get; set; }

        /// <summary>
        /// Updates the state of the controller, based on the current camera.
        /// </summary>
        public void UpdateFromCamera()
        {
            UpdateFromCamera(0.0f);
        }

        /// <summary>
        /// Updates the state of the controller, based on the current camera.
        /// </summary>
        /// <param name="lookAtDistance">Distance from the look at point.</param>
        public void UpdateFromCamera(float lookAtDistance)
        {
            if (_camera == null)
            {
                return;
            }

            _camPos = _camera.Position;

            // If look at distance is almost zero, take a look at the current target point. Otherwise create a new target point based on what the camera is positioned
            // and looking towards
            if (MathHelper.IsApproxZero(lookAtDistance))
            {
                Vector3.Distance(ref _camPos, ref _targetPoint, out _distance);
            }
            else
            {
                Vector3 dir = _camera.Direction;
                Vector3.Multiply(ref dir, lookAtDistance, out dir);
                Vector3.Add(ref dir, ref _camPos, out _targetPoint);
                _distance = lookAtDistance;
            }
            
            _camera.ViewMatrix.ToEulerAngles(out _yaw, out _pitch, out Angle roll);

            _isDirty = true;
        }

        /// <summary>
        /// Checks if input has happened and updates the controller state, then updates the camera if anything has changed.
        /// </summary>
        /// <param name="time">The time between updates.</param>
        public void Update(IGameTime time)
        {
            if (_camera == null || !IsEnabled)
            {
                return;
            }
            
            InputGroup.CheckAndPerformTriggers(time);

            if (_isDirty)
            {
                Quaternion.FromEulerAngles(_yaw, _pitch, Angle.Zero, out Quaternion rot);
                Quaternion.GetRotationVector(ref rot, 2, out Vector3 pos);

                Vector3.Multiply(ref pos, _distance, out pos);
                Vector3.Add(ref pos, ref _targetPoint, out _camPos);

                _camera.Position = _camPos;
                _camera.LookAt(_targetPoint, Vector3.Up);
                _camera.Update();

                _isDirty = false;
            }
        }

        /// <summary>
        /// Rotate the camera about both the camera Y (yaw) and X (pitch) axes.
        /// </summary>
        /// <param name="delta">Mouse position delta along the X, Y screen axis (origin upper left). The delta values scales the rotation speed. -X rotates right, +X rotates left (yaw) and +Y rotates up, -Y rotates down (pitch)</param>
        /// <param name="time">Elapsed time.</param>
        public void Rotate(Int2 delta, IGameTime time)
        {
            RotateYaw(delta.X, time);
            RotatePitch(delta.Y, time);
        }

        /// <summary>
        /// Rotate the camera about the camera X axis.
        /// </summary>
        /// <param name="deltaX">Mouse position delta along the X screen axis (origin upper left). The delta values scales the rotation speed. Negative rotates right, positive rotates left.</param>
        /// <param name="time">Elapsed time.</param>
        public void RotateYaw(int deltaX, IGameTime time)
        {
            if (deltaX == 0)
            {
                return;
            }

            _yaw += deltaX * -_yawRotateSpeed * time.ElapsedTimeInSeconds;

            if (_yaw.Radians > MathHelper.TwoPi || _yaw.Radians <= -MathHelper.TwoPi)
            {
                _yaw = Angle.Zero;
            }

            _isDirty = true;
        }

        /// <summary>
        /// Rotate the camera about the camera Y axis.
        /// </summary>
        /// <param name="deltaY">Mouse position delta along the Y screen axis (origin upper left). The delta values scales the rotation speed. Positive rotates up, negative rotates down.<param>
        /// <param name="time">Elapsed time.</param>
        public void RotatePitch(int deltaY, IGameTime time)
        {
            if (deltaY == 0)
            {
                return;
            }

            _pitch += deltaY * -_pitchRotateSpeed * time.ElapsedTimeInSeconds;

            _pitch = Angle.Clamp(_pitch, MinPitch, MaxPitch);

            _isDirty = true;
        }

        /// <summary>
        /// Zooms the camera in/out along the camera's direction axis.
        /// </summary>
        /// <param name="delta">Mouse wheel delta (increments of 120). The delta values scales the zoom speed. Positive zooms in, negative zooms out.</param>
        /// <param name="time">Elapsed time.</param>
        public void Zoom(int delta, IGameTime time)
        {
            if (delta == 0)
            {
                return;
            }

            Distance = _distance * (float)Math.Pow(1.00105f, -delta);
            
            Vector3.Subtract(ref _camPos, ref _targetPoint, out Vector3 dir);
            dir.Normalize();

            Vector3.Multiply(ref dir, _distance, out dir);
            Vector3.Add(ref _targetPoint, ref dir, out _camPos);

            _isDirty = true;
        }

        /// <summary>
        /// Pans the camera by a certain amount along the Right/Up axes. This translates both the camera position and target point.
        /// </summary>
        /// <param name="delta">Mouse position delta along the X, Y screen axis (origin upper left). The delta values scales the pan speed. -X moves right, +X moves left and -Y moves down, +Y moves up</param>
        /// <param name="time">Elapsed time.</param>
        public void Pan(Int2 delta, IGameTime time)
        {
            if (delta == Int2.Zero)
            {
                return;
            }

            Vector3 right = _camera.Right;
            Vector3.Multiply(ref right, -delta.X * _panSpeed * time.ElapsedTimeInSeconds, out right);

            Vector3 up = _camera.Up;
            Vector3.Multiply(ref up, delta.Y * _panSpeed * time.ElapsedTimeInSeconds, out up);
            
            Vector3.Add(ref right, ref up, out Vector3 translation);

            Vector3.Add(ref _camPos, ref translation, out _camPos);
            Vector3.Add(ref _targetPoint, ref translation, out _targetPoint);

            _isDirty = true;
        }

        /// <summary>
        /// Populates the controller's input grouping with triggers to manipulate the camera. If the keybindings is null or a key is not present, then the default binding is used (see implementors).
        /// </summary>
        /// <param name="keyBindings">The key bindings.</param>
        public void MapControls(IReadOnlyDictionary<string, KeyOrMouseButton[]> keyBindings)
        {
            InputGroup.Clear();

            InputGroup.Add(new InputTrigger(
                RotateBindingName, 
                KeyOrMouseButton.CreateInputCondition(keyBindings, RotateBindingName, MouseButton.Left, false, true), 
                new RotateInputAction(Rotate)));

            InputGroup.Add(new InputTrigger(
                PanBindingName, 
                KeyOrMouseButton.CreateInputCondition(keyBindings, PanBindingName, MouseButton.Right, false, true), 
                new PanInputAction(Pan)));

            InputGroup.Add(CreateZoomTrigger(keyBindings));
        }

        /// <summary>
        /// Creates the zoom input trigger
        /// </summary>
        /// <param name="keyBindings">Key binding buttons</param>
        /// <returns>Input trigger for zooming</returns>
        private InputTrigger CreateZoomTrigger(IReadOnlyDictionary<string, KeyOrMouseButton[]> keyBindings)
        {
            if (keyBindings == null || !keyBindings.TryGetValue(ZoomBindingName, out KeyOrMouseButton[] buttons))
            {
                return new ZoomInputTrigger(ZoomBindingName, Zoom);
            }

            var compositeCondition = new CompositeInputCondition(
                new MouseWheelScrollCondition(), 
                KeyOrMouseButton.CreateInputCondition(false, true, buttons));

            return new InputTrigger(ZoomBindingName, compositeCondition, new ZoomInputAction(Zoom));
        }
    }
}
