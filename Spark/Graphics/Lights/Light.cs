namespace Spark.Graphics
{
    using System;
    
    using Math;
    using Content;

    /// <summary>
    /// Base representation for a light. Contains common color and attenuation properties shared by most types of lights.
    /// </summary>
    public abstract class Light : ISavable
    {
        private string _name;
        private Color _ambient;
        private Color _diffuse;
        private Color _specular;
        private float _range;
        private bool _attenuate;
        private bool _isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        protected Light() 
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        /// <param name="name">Name of the light</param>
        protected Light(string name)
        {
            _name = (string.IsNullOrEmpty(name)) ? LightType.ToString() + "Light" : name;
            _ambient = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            _diffuse = Color.White;
            _specular = Color.White;
            _range = float.MaxValue;
            _attenuate = false;
            _isEnabled = true;
            Id = -1;
        }

        /// <summary>
        /// Occurs when the light changes.
        /// </summary>
        public event TypedEventHandler<Light> LightChanged;

        /// <summary>
        /// Gets or sets the name of this light.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the (non-persistent) ID for this light. This is used by various light management systems to track the light source.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the ambient contribution of this light,
        /// which is the color that appears to be coming from everywhere. Default is
        /// Color(.4f, .4f, .4f, 1.0f).
        /// </summary>
        public Color Ambient
        {
            get => _ambient;
            set
            {
                _ambient = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the diffuse contribution of this light,
        /// which is the color directly from the light source. Default is <see cref="Color.White"/>.
        /// </summary>
        public Color Diffuse
        {
            get => _diffuse;
            set
            {
                _diffuse = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the specular contribution of this light,
        /// which is the color reflected by an object depending on
        /// how shiny it is. Default is <see cref="Color.White"/>;
        /// </summary>
        public Color Specular
        {
            get => _specular;
            set
            {
                _specular = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets the range of the light, used for attenuation. Default is <see cref="float.MaxValue"/>.
        /// </summary>
        public float Range
        {
            get => _range;
            set
            {
                _range = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets if this light should attenuate. Some lights may choose to
        /// ignore this (e.g. Directional lights should never attenuate). Default is false.
        /// </summary>
        public bool Attenuate
        {
            get => _attenuate;
            set
            {
                _attenuate = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets or sets if this light should be enabled or not. Default is true.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnLightChanged();
            }
        }

        /// <summary>
        /// Gets the light type for this light.
        /// </summary>
        public abstract LightType LightType { get; }

        /// <summary>
        /// Clones the light.
        /// </summary>
        /// <returns>Copy of the light.</returns>
        public abstract Light Clone();

        /// <summary>
        /// Copies properties of the specified light.
        /// </summary>
        /// <param name="l">Light to copy propreties from</param>
        public void Set(Light l)
        {
            if (l == null)
            {
                return;
            }

            _name = l._name;
            _ambient = l._ambient;
            _diffuse = l._diffuse;
            _specular = l._specular;
            _range = l._range;
            _attenuate = l._attenuate;
            _isEnabled = l._isEnabled;

            OnSet(l);
            OnLightChanged();
        }

        /// <summary>
        /// Sets all the color properties of the light.
        /// </summary>
        /// <param name="ambient">Ambient color.</param>
        /// <param name="diffuse">Diffuse color.</param>
        /// <param name="specular">Specular color.</param>
        public void SetColor(Color ambient, Color diffuse, Color specular)
        {
            _ambient = ambient;
            _diffuse = diffuse;
            _specular = specular;
            OnLightChanged();
        }

        /// <summary>
        /// Sets the attenuation properties of the light.
        /// </summary>
        /// <param name="range">Distance from the light source at which the light influences.</param>
        /// <param name="attenuate">True if the light should attenuate, false otherwise.</param>
        public void SetAttenuation(float range, bool attenuate = true)
        {
            _range = range;
            _attenuate = attenuate;
            OnLightChanged();
        }

        /// <summary>
        /// Sets the attenuation properties of the light (approximately) based on the classic light attenuation function, where
        /// Attenuation = 1.0 / (constant + linear*D + quadratic*D*D) where D is distance from the light source to the object.
        /// </summary>
        /// <param name="constant">Constant parameter.</param>
        /// <param name="linear">Linear parameter.</param>
        /// <param name="quadratic">Quadratic parameter.</param>
        /// <param name="attenuate">True if the light should attenuate, false otherwise.</param>
        public void SetAttenuation(float constant, float linear, float quadratic, bool attenuate = true)
        {
            if (MathHelper.SolveQuadratic(quadratic, linear, constant - 70.0f, out float result1, out float result2))
            {
                _range = Math.Max(Math.Abs(result1), Math.Abs(result2));
            }

            _attenuate = attenuate;
            OnLightChanged();
        }

        /// <summary>
        /// Computes an attenuation factor (as it would in the standard shader library). Attenuation is between 0 and 1,
        /// where 0 is no effect on the object and 1 is the maximum intensity applied to the lighting.
        /// </summary>
        /// <param name="pos">Position of the object.</param>
        /// <param name="lightPos">Position of the light source.</param>
        /// <param name="lightRange">Range of the light.</param>
        /// <returns>Attenuation factor between 0 and 1.</returns>
        public static float ComputeAttenuationFactor(Vector3 pos, Vector3 lightPos, float lightRange)
        {
            return ComputeAttenuationFactor(ref pos, ref lightPos, lightRange);
        }

        /// <summary>
        /// Computes an attenuation factor (as it would in the standard shader library). Attenuation is between 0 and 1,
        /// where 0 is no effect on the object and 1 is the maximum intensity applied to the lighting.
        /// </summary>
        /// <param name="pos">Position of the object.</param>
        /// <param name="lightPos">Position of the light source.</param>
        /// <param name="lightRange">Range of the light.</param>
        /// <returns>Attenuation factor between 0 and 1.</returns>
        public static float ComputeAttenuationFactor(ref Vector3 pos, ref Vector3 lightPos, float lightRange)
        {
            Vector3.DistanceSquared(ref lightPos, ref pos, out float sqrDist);

            float invRangeSquared = 0.0f;
            if (!MathHelper.IsApproxZero(lightRange))
            {
                invRangeSquared = 1.0f / (lightRange * lightRange);
            }

            float atten = MathHelper.Clamp(1.0f - sqrDist * invRangeSquared, 0, 1);
            return atten * atten;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public virtual void Read(ISavableReader input)
        {
            _name = input.ReadString();
            input.Read(out _ambient);
            input.Read(out _diffuse);
            input.Read(out _specular);
            _range = input.ReadSingle();
            _attenuate = input.ReadBoolean();
            _isEnabled = input.ReadBoolean();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public virtual void Write(ISavableWriter output)
        {
            output.Write("Name", _name);
            output.Write("Ambient", ref _ambient);
            output.Write("Diffuse", ref _diffuse);
            output.Write("Specular", ref _specular);
            output.Write("AttenuationRange", _range);
            output.Write("Attenuate", _attenuate);
            output.Write("IsEnabled", _isEnabled);
        }

        /// <summary>
        /// Notifies event handlers of light changes.
        /// </summary>
        protected virtual void OnLightChanged()
        {
            LightChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Copies light properties.
        /// </summary>
        /// <param name="l">Light to copy from.</param>
        protected virtual void OnSet(Light l)
        {
            // No-op
        }
    }
}
