namespace Spark.Graphics
{
    using Math;
    using Content;

    /// <summary>
    /// Represents a light source that is infinitely away from the viewer, thus it only has a direction. For example, the Sun relative
    /// to the Earth "seems infinitely away". This is also known as a parallel light. Due to the nature of this type of light, it ignores attenuation
    /// properties from the base light class.
    /// </summary>
    public class DirectionalLight : Light
    {
        private Vector3 _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        public DirectionalLight() 
            : this("DirectionalLight", Vector3.Forward)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        public DirectionalLight(string name) 
            : this(name, Vector3.Forward)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="direction">Direction of the light.</param>
        public DirectionalLight(Vector3 direction) 
            : this("DirectionalLight", direction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        /// <param name="direction">Direction of the light.</param>
        public DirectionalLight(string name, Vector3 direction)
            : base(name)
        {
            _direction = direction;
            _direction.Normalize();
        }

        /// <summary>
        /// Gets or sets the direction of the light. The value is always normalized.
        /// 
        /// Default: (0, 0, -1.0f)
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
        /// Gets the light type for this light.
        /// </summary>
        public override LightType LightType => LightType.Directional;

        /// <summary>
        /// Clones the light.
        /// </summary>
        /// <returns>Copy of the light.</returns>
        public override Light Clone()
        {
            DirectionalLight dl = new DirectionalLight();
            dl.Set(this);

            return dl;
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
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            base.Read(input);
            input.Read(out _direction);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            base.Write(output);
            output.Write("Direction", ref _direction);
        }

        /// <summary>
        /// Copies light properties.
        /// </summary>
        /// <param name="l">Light to copy from.</param>
        protected override void OnSet(Light l)
        {
            DirectionalLight dl = l as DirectionalLight;
            if (dl == null)
            {
                return;
            }

            _direction = dl._direction;
        }
    }
}
