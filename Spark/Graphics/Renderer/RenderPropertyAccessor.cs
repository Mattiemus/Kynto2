namespace Spark.Graphics
{
    /// <summary>
    /// Accessor delegate for getting the underlying value of a render property.
    /// </summary>
    /// <typeparam name="T">Generic value type.</typeparam>
    /// <returns>The value.</returns>
    public delegate T GetPropertyValue<T>();

    /// <summary>
    /// Generic base class for render properties whose value comes from a different source. This acts as a facade to query
    /// those values (generally value types).
    /// </summary>
    /// <typeparam name="T">Property value type.</typeparam>
    public abstract class RenderPropertyAccessor<T> : RenderProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyAccessor{T}"/> class.
        /// </summary>
        /// <param name="id">The unique ID value.</param>
        protected RenderPropertyAccessor(RenderPropertyId id) : base(id) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyAccessor{T}"/> class.
        /// </summary>
        /// <param name="id">The unique ID value.</param>
        /// <param name="accessor">Getter delegate for the property value.</param>
        protected RenderPropertyAccessor(RenderPropertyId id, GetPropertyValue<T> accessor)
            : base(id)
        {
            Accessor = accessor;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public T Value => Accessor();

        /// <summary>
        /// Gets or sets the backing store getter.
        /// </summary>
        protected GetPropertyValue<T> Accessor { get; set; }

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
            value = Accessor();
        }
    }
}
