namespace Spark.Graphics
{
    using System;
    using System.Globalization;

    using Content;


    /// <summary>
    /// Common base class for all render state objects. Render states configure different stages of the graphics pipeline. When first created, the state is mutable,
    /// until it is bound either by calling <see cref="BindRenderState"/> directly or the first time the state is applied to the pipeline. This makes the state immutable thereafter,
    /// and exceptions will be thrown if properties are attempted to be set. It is best practice to re-use state objects as often as possible and to bind them early and up front. 
    /// </summary>
    public abstract class RenderState : GraphicsResource, ISavable, IStandardLibraryContent, IEquatable<RenderState>
    {
        internal string _predefinedStateName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderState"/> class.
        /// </summary>
        protected RenderState()
        {
        }

        /// <summary>
        /// Gets if the render state has been bound to the pipeline, once bound the state becomes read-only.
        /// </summary>
        public abstract bool IsBound { get; }

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public abstract RenderStateType StateType { get; }

        /// <summary>
        /// Gets the key that identifies this render state type and configuration for comparing states.
        /// </summary>
        public abstract RenderStateKey RenderStateKey { get; }

        /// <summary>
        /// Gets the name of the content this instance represents. If <see cref="IsStandardContent" /> is false, then this returns an empty string.
        /// </summary>
        public string StandardContentName => _predefinedStateName;

        /// <summary>
        /// Gets if the instance represents a predefined state.
        /// </summary>
        public bool IsStandardContent => !string.IsNullOrEmpty(_predefinedStateName);

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to the current <see cref="object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RenderState)
            {
                return Equals((obj as RenderState));
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this render state and another, that is if they both represent the same state configuration.
        /// </summary>
        /// <param name="other">Other render state to compare against.</param>
        /// <returns>True if the two states are equal, false otherwise.</returns>
        public bool Equals(RenderState other)
        {
            if (other == null)
            {
                return false;
            }

            return RenderStateKey.Equals(other);
        }

        /// <summary>
        /// Tests if this state represents the same state as the other. This does not mean they are the same object, however, just that the two states
        /// represent the same underlying render state.
        /// </summary>
        /// <param name="other">Other render state to compare against.</param>
        /// <returns>True if the two states are the same, false otherwise.</returns>
        public bool IsSameState(RenderState other)
        {
            if (other == null)
            {
                return false;
            }

            return RenderStateKey.Equals(other.RenderStateKey);
        }

        /// <summary>
        /// Gets a consistent hash code that identifies the content item. If it is not standard content, each instance should have a unique hash, if the instance is
        /// standard content, each instance should have the same hash code. This might differ from .NET's hash code and is only used to identify two instances that
        /// represent the same data (there may be situations where we want to differentiate the two instances, so we don't rely on the .NET's get hash code function).
        /// </summary>
        /// <returns>32-bit hash code.</returns>
        public int GetContentHashCode()
        {
            return RenderStateKey.Hash; // Same as normal hash code
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return RenderStateKey.Hash;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (IsStandardContent)
            {
                return _predefinedStateName;
            }

            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "IsBound: {0}, {1}", new object[] { IsBound.ToString(), RenderStateKey.ToString() });
        }

        /// <summary>
        /// Binds the render state to the graphics pipeline. If not called after the state is created, it is automatically done the first time the render state
        /// is applied. Once bound, the render state becomes immutable.
        /// </summary>
        public abstract void BindRenderState();

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public abstract void Read(ISavableReader input);

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public abstract void Write(ISavableWriter output);
    }
}
