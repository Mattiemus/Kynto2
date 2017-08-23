namespace Spark.Core.Collections
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a two-dimensional key that has a "Major" and a "Minor" component.
    /// </summary>
    /// <typeparam name="TMajorKey">Major key type.</typeparam>
    /// <typeparam name="TMinorKey">Minor key type.</typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct MultiKey<TMajorKey, TMinorKey>
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="MultiKey{TMajorKey, TMinorKey}"/> struct.
        /// </summary>
        /// <param name="majorKey">Major key component.</param>
        /// <param name="minorKey">Minor key component.</param>
        public MultiKey(TMajorKey majorKey, TMinorKey minorKey)
        {
            Major = majorKey;
            Minor = minorKey;
        }

        /// <summary>
        /// Gets the major key component.
        /// </summary>
        public TMajorKey Major { get; }

        /// <summary>
        /// Gets the minor key component.
        /// </summary>
        public TMinorKey Minor { get; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int majorHash = (Major != null) ? Major.GetHashCode() : 0;
                int minorHash = (Minor != null) ? Minor.GetHashCode() : 0;

                return ((majorHash << 5) + majorHash) ^ minorHash;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is MultiKey<TMajorKey, TMinorKey> other)
            {
                return Equals(Major, other.Major) && Equals(Minor, other.Minor);
            }

            return false;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            string majorString = (Major != null) ? Major.ToString() : "NULL";
            string minorString = (Minor != null) ? Minor.ToString() : "NULL";

            return string.Format("[{0},{1}]", majorString, minorString);
        }
    }
}
