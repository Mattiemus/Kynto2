namespace Spark.Core.Collections
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A key for two pieces of data that are linked together in some manner.
    /// </summary>
    /// <typeparam name="T1">First piece of data</typeparam>
    /// <typeparam name="T2">Second piece of data</typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TupleKey<T1, T2> : IEquatable<TupleKey<T1, T2>>, IComparable<TupleKey<T1, T2>>
    {
        private int _firstHash;
        private int _secondHash;
        private int _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleKey{T1, T2}"/> struct.
        /// </summary>
        /// <param name="first">The first piece of data.</param>
        /// <param name="second">The second piece of data.</param>
        public TupleKey(T1 first, T2 second)
        {
            First = first;
            Second = second;

            _firstHash = (First != null) ? First.GetHashCode() : 0;
            _secondHash = (Second != null) ? Second.GetHashCode() : 0;

            _hash = _firstHash;
            _hash = (_hash * 31) + _secondHash;
        }

        /// <summary>
        /// Gets the first piece of data the key represents.
        /// </summary>
        public T1 First { get; }

        /// <summary>
        /// Gets the second piece of data the key represents.
        /// </summary>
        public T2 Second { get; }

        /// <summary>
        /// Tests equality between two keys.
        /// </summary>
        /// <param name="a">The first key</param>
        /// <param name="b">The second key</param>
        /// <returns>True if both keys are equal, false otherwise.</returns>
        public static bool operator ==(TupleKey<T1, T2> a, TupleKey<T1, T2> b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two keys.
        /// </summary>
        /// <param name="a">The first key</param>
        /// <param name="b">The second key</param>
        /// <returns>True if both keys are not equal, false otherwise.</returns>
        public static bool operator !=(TupleKey<T1, T2> a, TupleKey<T1, T2> b)
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
            if (obj is TupleKey<T1, T2>)
            {
                return Equals((TupleKey<T1, T2>)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between this key and another key.
        /// </summary>
        /// <param name="other">The other key to compare to</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(TupleKey<T1, T2> other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between this key and another key.
        /// </summary>
        /// <param name="other">The other key to compare to</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref TupleKey<T1, T2> other)
        {
            return (_firstHash == other._firstHash) && (_secondHash == other._secondHash);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.  </returns>
        public override int GetHashCode()
        {
            return _hash;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns> A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "First: {0}, Second: {1}", new object[] { (First != null) ? First.ToString() : "null", (Second != null) ? Second.ToString() : "null" });
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        /// Less than zero: This object is less than the other. Zero: This object is equal to the other. Greater than Zero: This object is greater than the other.
        /// </returns>
        public int CompareTo(TupleKey<T1, T2> other)
        {
            int otherFirst = other._firstHash;
            int otherSecond = other._secondHash;

            int first = _firstHash;
            int second = _secondHash;

            // First comes...first, if this is less than it should always come before
            if (first < otherFirst)
            {
                return -1;
            }
            else if (first == otherFirst)
            {
                // First equal, second determines order
                if (second < otherSecond)
                {
                    // Second is less, this should always come before
                    return -1;
                }
                else if (second == otherSecond)
                {
                    // All equal, return 0
                    return 0;
                }
                else
                {
                    // Greater than, this should always come after
                    return 1;
                }
            }
            else
            {
                // Otherwise should come after
                return 1;
            }
        }
    }
}
