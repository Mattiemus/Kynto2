namespace Spark.Graphics
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Key representing a render state - the state type and the state's hash code.
    /// </summary>
    public struct RenderStateKey : IEquatable<RenderStateKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderStateKey"/> struct.
        /// </summary>
        /// <param name="stateType">Render state type.</param>
        /// <param name="hash">Render state hash code.</param>
        public RenderStateKey(RenderStateType stateType, int hash)
        {
            StateType = stateType;
            Hash = hash;
        }

        /// <summary>
        /// Gets the render state type.
        /// </summary>
        public RenderStateType StateType { get; }

        /// <summary>
        /// Gets the render state's hash code.
        /// </summary>
        public int Hash { get; }

        /// <summary>
        /// Tests equality between two render state keys.
        /// </summary>
        /// <param name="a">First render state key</param>
        /// <param name="b">Second render state key</param>
        /// <returns>True if the keys are equal, false otherwise.</returns>
        public static bool operator ==(RenderStateKey a, RenderStateKey b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two render state keys.
        /// </summary>
        /// <param name="a">First render state key</param>
        /// <param name="b">Second render state key</param>
        /// <returns>True if the keys are not equal, false otherwise.</returns>
        public static bool operator !=(RenderStateKey a, RenderStateKey b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RenderStateKey)
            {
                return Equals((RenderStateKey)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(RenderStateKey other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ref RenderStateKey other)
        {
            return (StateType == other.StateType) && (Hash == other.Hash);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return Hash;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "StateType: {0}, Hash: {1}", new object[] { StateType.ToString(), Hash.ToString() });
        }
    }
}
