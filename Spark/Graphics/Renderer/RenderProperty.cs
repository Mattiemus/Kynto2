namespace Spark.Graphics.Renderer
{
    using System;

    /// <summary>
    /// Base class for all render properties. These properties are holders for bits of information that may be used by interested parties
    /// (such as the Material computed parameters) when an object is rendered. Each property has a unique ID associated and is based on the property type,
    /// allowing for fast look up and organization.
    /// </summary>
    public abstract class RenderProperty : IEquatable<RenderProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderProperty"/> class.
        /// </summary>
        /// <param name="id">The unique ID value.</param>
        protected RenderProperty(RenderPropertyId id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the unique ID value for this property.
        /// </summary>
        public RenderPropertyId Id { get; protected set; }

        /// <summary>
        /// Gets if the property can be cloned.
        /// </summary>
        public virtual bool CanClone => false;

        /// <summary>
        /// Queries the unique ID value based on property type.
        /// </summary>
        /// <typeparam name="Property">Type of property.</typeparam>
        /// <returns>Associated render property of the type.</returns>
        public static RenderPropertyId GetPropertyId<Property>()
        {
            return RenderPropertyIdHolder<Property>.Id;
        }

        /// <summary>
        /// Checks if the render property and this render property are the same, that is, have the same ID.
        /// </summary>
        /// <param name="property">Other property to check if it is the same type.</param>
        /// <returns>True if both properties share the same ID, false otherwise.</returns>
        public bool IsSameProperty(RenderProperty property)
        {
            if (property == null)
            {
                return false;
            }

            return Id == property.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>True</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            RenderProperty other = obj as RenderProperty;
            if (other != null)
            {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(RenderProperty other)
        {
            return other.Id == Id;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        /// <summary>
        /// Get a copy of the render property. Some properties may not be cloned.
        /// </summary>
        /// <returns>Copy of the render property.</returns>
        public virtual RenderProperty Clone()
        {
            return null;
        }
    }

    /// <summary>
    /// Generic base class for render properties. Generally new render properties derive from this type. By default this property
    /// does not implement serialization, but derived classes can.
    /// </summary>
    /// <remarks>
    /// If serialization is required for the property, there are a number of ready to use subclasses for <see cref="IPrimitiveValue"/>,
    /// <see cref="ISavable"/> and .NET primitive types.
    /// </remarks>
    /// <typeparam name="T">Property value type.</typeparam>
    public abstract class RenderProperty<T> : RenderProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderProperty{T}"/> class.
        /// </summary>
        /// <param name="id">The unique ID value.</param>
        /// <param name="value">The property value.</param>
        protected RenderProperty(RenderPropertyId id, T value)
            : base(id)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("ID: {0}, Type: {1}", Id.ToString(), typeof(T).ToString());
        }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <param name="value">Value held by the property.</param>
        public void GetValue(out T value)
        {
            value = Value;
        }
    }
}
