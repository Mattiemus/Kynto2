namespace Spark.Graphics
{
    using Math;
    using Content;

    /// <summary>
    /// Represents a light source that has a position in space but the influence of the light is restricted to a cone (opposed to a <see cref="PointLight"/> that is
    /// omni-directional). The cone is controlled by a direction and falloff angles to soften the edges.
    /// </summary>
    public class SpotLight : Light
    {
        private Vector3 _position;
        private Vector3 _direction;
        private Angle _innerAngle;
        private Angle _outerAngle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class.
        /// </summary>
        public SpotLight() 
            : this("SpotLight", Vector3.Zero, Vector3.Forward, Angle.PiOverFour, Angle.PiOverTwo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        public SpotLight(string name) 
            : this(name, Vector3.Zero, Vector3.Forward, Angle.PiOverFour, Angle.PiOverTwo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class.
        /// </summary>
        /// <param name="position">Position of the light.</param>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="innerAngle">Cone's innter angle.</param>
        /// <param name="outerAngle">Cone's outer angle.</param>
        public SpotLight(Vector3 position, Vector3 direction, Angle innerAngle, Angle outerAngle)
            : this("SpotLight", position, direction, innerAngle, outerAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        /// <param name="position">Position of the light.</param>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="innerAngle">Cone's innter angle.</param>
        /// <param name="outerAngle">Cone's outer angle.</param>
        public SpotLight(string name, Vector3 position, Vector3 direction, Angle innerAngle, Angle outerAngle)
            : base(name)
        {
            _position = position;
            Direction = direction;
            InnerAngle = innerAngle;
            OuterAngle = outerAngle;
        }

        /// <summary>
        /// Gets or sets the position of the light in space.
        /// 
        /// Default: (0, 0, 0)
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the direction of the light. The value 
        /// is always normalized.
        /// 
        /// Default: (0, 0, -1)
        /// </summary>
        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _direction.Normalize();
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the innter cutoff angle for this light.
        /// This is the extent of the concentration of the light
        /// along the direction (the cone). This should always be smaller than
        /// the outer angle.
        /// 
        /// Default: 45 degrees
        /// </summary>
        public Angle InnerAngle
        {
            get => _innerAngle;
            set
            {
                _innerAngle = Angle.Clamp(value, -Angle.Pi, Angle.Pi);
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gest or sets the outer cutoff angle for this light. This
        /// is used to allow a soft blending from the main concentration
        /// of the light to the area outside of the spotlight's cone.
        /// 
        /// Default: 90 degrees
        /// </summary>
        public Angle OuterAngle
        {
            get => _outerAngle;
            set
            {
                _outerAngle = Angle.Clamp(value, -Angle.Pi, Angle.Pi);
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets the light type for this light.
        /// </summary>
        public override LightType LightType => LightType.Spot;

        /// <summary>
        /// Clones the light.
        /// </summary>
        /// <returns>Copy of the light.</returns>
        public override Light Clone()
        {
            SpotLight sl = new SpotLight();
            sl.Set(this);

            return sl;
        }

        /// <summary>
        /// Sets the position of the light.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public void SetPosition(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
            OnLightChanged();
        }

        /// <summary>
        /// Sets the direction of the light.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        /// <param name="z">Z component.</param>
        public void SetDirection(float x, float y, float z)
        {
            _direction = new Vector3(x, y, z);
            _direction.Normalize();
            OnLightChanged();
        }

        /// <summary>
        /// Set the angle properties of the light.
        /// </summary>
        /// <param name="innerAngle">Inner angle.</param>
        /// <param name="outerAngle">Outer angle.</param>
        public void SetAngles(Angle innerAngle, Angle outerAngle)
        {
            _innerAngle = Angle.Clamp(innerAngle, -Angle.Pi, Angle.Pi);
            _outerAngle = Angle.Clamp(outerAngle, -Angle.Pi, Angle.Pi);
            OnLightChanged();
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            base.Read(input);
            input.Read(out _position);
            input.Read(out _direction);
            input.Read(out _innerAngle);
            input.Read(out _outerAngle);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            base.Write(output);
            output.Write("Position", ref _position);
            output.Write("Direction", ref _direction);
            output.Write("InnerAngle", ref _innerAngle);
            output.Write("OuterAngle", ref _outerAngle);
        }

        /// <summary>
        /// Copies light properties.
        /// </summary>
        /// <param name="l">Light to copy from.</param>
        protected override void OnSet(Light l)
        {
            SpotLight sl = l as SpotLight;
            if (sl == null)
            {
                return;
            }

            _position = sl._position;
            _direction = sl._direction;
            _innerAngle = sl._innerAngle;
            _outerAngle = sl._outerAngle;
        }
    }
}
