namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Identifies a render bucket in the render queue.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct RenderBucketId : IEquatable<RenderBucketId>, IComparable<RenderBucketId>, INamed
    {
        private const string INVALID_NAME = "Invalid";

        private static Dictionary<string, RenderBucketId> _registeredIDs;
        private static int _globalCurrentId = -1;

        /// <summary>
        /// Static initializer for the <see cref="RenderBucketId"/> struct.
        /// </summary>
        static RenderBucketId()
        {
            _registeredIDs = new Dictionary<string, RenderBucketId>();

            Invalid = new RenderBucketId(INVALID_NAME, -1);
            PreOpaqueBucket = RegisterId("PreOpaque");
            Opaque = RegisterId("Opaque");
            Transparent = RegisterId("Transparent");
            Ortho = RegisterId("Ortho");
            PostOpaqueBucket = RegisterId("PostOpaque");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderBucketId"/> struct.
        /// </summary>
        /// <param name="name">Name of the render bucket.</param>
        /// <param name="idValue">The integer ID value.</param>
        public RenderBucketId(string name, int idValue)
        {
            if (idValue < 0)
            {
                Name = INVALID_NAME;
                Value = -1;
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }

                Name = name;
                Value = idValue;
            }
        }

        /// <summary>
        /// Gets the invalid bucket ID.
        /// </summary>
        public static RenderBucketId Invalid { get; }

        /// <summary>
        /// Gets a predefined bucket ID: PreOpaqueBucket - Standard non-transparent geometry guaranteed to be rendered first, sorted by material and/or front-to-back.
        /// </summary>
        public static RenderBucketId PreOpaqueBucket { get; }

        /// <summary>
        /// Gets a predefined bucket ID: Opaque - Standard non-transparent geometry rendering, sorted by material and/or front-to-back.
        /// </summary>
        public static RenderBucketId Opaque { get; }

        /// <summary>
        /// Gets a predefined bucket ID: Transparent - Transparent geometry rendering, sorted back-to-front.
        /// </summary>
        public static RenderBucketId Transparent { get; }

        /// <summary>
        /// Gets a predefined bucket ID: Ortho - Geometry that should be rendered with an orthographic projection (or sprites), sorted by
        /// an ortho order defined by <see cref="OrthoOrderProperty"/>.
        /// </summary>
        public static RenderBucketId Ortho { get; }

        /// <summary>
        /// Gets a predefined bucket ID: PostOpaqueBucket - Standard non-transparent geometry guaranted to be rendered last, sorted by material and/or front-to-back.
        /// </summary>
        public static RenderBucketId PostOpaqueBucket { get; }

        /// <summary>
        /// Gets the name of the render bucket.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the integer value of the ID.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets if the ID is valid (greater than or equal to zero).
        /// </summary>
        public bool IsValid => Value >= 0;

        /// <summary>
        /// Registers a new render bucket. This is not thread safe, all render buckets should be registered up front at the start of the application. Buckets
        /// are referenced by a unique string name.
        /// </summary>
        /// <param name="bucketName">Unique render bucket name.</param>
        /// <returns>New render bucket ID.</returns>
        public static RenderBucketId RegisterId(string bucketName)
        {
            if (_registeredIDs.ContainsKey(bucketName))
            {
                throw new ArgumentException("Render bucket id already registered", nameof(bucketName));
            }

            _globalCurrentId++;
            RenderBucketId newId = new RenderBucketId(bucketName, _globalCurrentId);
            _registeredIDs.Add(bucketName, newId);

            return newId;
        }

        /// <summary>
        /// Queries a render bucket ID from the global map by its name.
        /// </summary>
        /// <param name="name">Name of the render bucket ID.</param>
        /// <returns>The corrosponding render bucket ID, or the <see cref="Invalid"/> ID if it could not be found.</returns>
        public static RenderBucketId QueryId(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Invalid;
            }

            if (_registeredIDs.TryGetValue(name, out RenderBucketId id))
            {
                return id;
            }

            return Invalid;
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
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>True</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is RenderBucketId)
            {
                return Equals((RenderBucketId)obj);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(RenderBucketId other)
        {
            return other.Value == Value;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(RenderBucketId other)
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
        /// Checks inequality between two render bucket IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are not the same, false otherwise.</returns>
        public static bool operator !=(RenderBucketId a, RenderBucketId b)
        {
            return a.Value != b.Value;
        }

        /// <summary>
        /// Checks equality between two render bucket IDs.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the two values are the same, false otherwise.</returns>
        public static bool operator ==(RenderBucketId a, RenderBucketId b)
        {
            return a.Value == b.Value;
        }

        /// <summary>
        /// Checks if the first render bucket ID is greater than the second.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the first value is greater than the second, false otherwise.</returns>
        public static bool operator >(RenderBucketId a, RenderBucketId b)
        {
            return a.Value > b.Value;
        }

        /// <summary>
        /// Checks if the first render bucket ID is less than the second.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the first value is less than the second, false otherwise.</returns>
        public static bool operator <(RenderBucketId a, RenderBucketId b)
        {
            return a.Value < b.Value;
        }

        /// <summary>
        /// Checks if the first render bucket ID is greater than or equal to the second.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the first value is greater than or equal to the second, false otherwise.</returns>
        public static bool operator >=(RenderBucketId a, RenderBucketId b)
        {
            return a.Value >= b.Value;
        }

        /// <summary>
        /// Checks if the first render bucket ID is less than or equal to the second.
        /// </summary>
        /// <param name="a">First ID.</param>
        /// <param name="b">Second ID.</param>
        /// <returns>True if the first value is less than or equal to the second, false otherwise.</returns>
        public static bool operator <=(RenderBucketId a, RenderBucketId b)
        {
            return a.Value <= b.Value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
