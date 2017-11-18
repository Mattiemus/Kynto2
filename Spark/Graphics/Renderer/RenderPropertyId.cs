namespace Spark.Graphics
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// Represents a unique value for identifying render properties to allow for fast lookup.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct RenderPropertyId : IEquatable<RenderPropertyId>, IComparable<RenderPropertyId>
    {
        private static int _globalCurrentId = -1;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPropertyId"/> struct.
        /// </summary>
        /// <param name="idValue">The integer ID value.</param>
        public RenderPropertyId(int idValue)
        {
            if (idValue < 0)
            {
                idValue = -1;
            }

            Value = idValue;
        }

        /// <summary>
        /// Gets the invalid render property ID value.
        /// </summary>
        public static RenderPropertyId Invalid => new RenderPropertyId(-1);

        /// <summary>
        /// Gets the integer value of the ID.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets if the ID is valid (greater than or equal to zero).
        /// </summary>
        public bool IsValid => Value >= 0;

        /// <summary>
        /// Generates a new unique ID. This is only unique for the current session and is thread safe.
        /// </summary>
        /// <returns>The new render property ID.</returns>
        public static RenderPropertyId GenerateNewUniqueId()
        {
            return new RenderPropertyId(Interlocked.Increment(ref _globalCurrentId));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Value;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(RenderPropertyId other)
        {
            if (Value < other.Value)
            {
                return -1;
            }

            if (Value > other.Value)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>True</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RenderPropertyId)
            {
                return Equals((RenderPropertyId)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(RenderPropertyId other)
        {
            return other.Value == Value;
        }

        /// <summary>
        /// Implicitly converts the render property ID to an integer value.
        /// </summary>
        /// <param name="id">Render property ID.</param>
        /// <returns>Integer value.</returns>
        public static implicit operator int(RenderPropertyId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly converts the integer value to a render property ID.
        /// </summary>
        /// <param name="idValue">Integer value.</param>
        /// <returns>Render property ID.</returns>
        public static implicit operator RenderPropertyId(int idValue)
        {
            return new RenderPropertyId(idValue);
        }

        /// <summary>
        /// Checks equality between two render property IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are the same, false otherwise.</returns>
        public static bool operator ==(RenderPropertyId a, RenderPropertyId b)
        {
            return a.Value == b.Value;
        }

        /// <summary>
        /// Checks inequality between two render property IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are not the same, false otherwise.</returns>
        public static bool operator !=(RenderPropertyId a, RenderPropertyId b)
        {
            return a.Value != b.Value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            return Value.ToString(formatProvider);
        }
    }
}
