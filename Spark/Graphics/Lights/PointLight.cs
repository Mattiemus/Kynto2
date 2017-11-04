namespace Spark.Graphics.Lights
{
    using Math;
    using Content;

    /// <summary>
    /// Represents an omni-directional light source that has a position in space.
    /// </summary>
    public class PointLight : Light
    {
        private Vector3 _position;

        /// <summary>
        /// Initialize a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        public PointLight() 
            : this("PointLight", Vector3.Zero)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        public PointLight(string name) 
            : this(name, Vector3.Zero)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="position">Position of the light.</param>
        public PointLight(Vector3 position) 
            : this("PointLight", position)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="name">Name of the light.</param>
        /// <param name="position">Position of the light.</param>
        public PointLight(string name, Vector3 position)
            : base(name)
        {
            _position = position;
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
        /// Gets the light type for this light.
        /// </summary>
        public override LightType LightType => LightType.Point;

        /// <summary>
        /// Clones the light.
        /// </summary>
        /// <returns>Copy of the light.</returns>
        public override Light Clone()
        {
            PointLight pl = new PointLight();
            pl.Set(this);

            return pl;
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
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            base.Read(input);
            input.Read(out _position);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            base.Write(output);
            output.Write("Position", ref _position);
        }

        /// <summary>
        /// Copies light properties.
        /// </summary>
        /// <param name="l">Light to copy from.</param>
        protected override void OnSet(Light l)
        {
            PointLight pl = l as PointLight;
            if (pl == null)
            {
                return;
            }

            _position = pl._position;
        }
    }
}
