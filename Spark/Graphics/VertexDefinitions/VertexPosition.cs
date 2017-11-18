namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;
    using Content;

    /// <summary>
    /// Represents a vertex that only has position data.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct VertexPosition : IEquatable<VertexPosition>, IVertexType, IPrimitiveValue
    {
        /// <summary>
        /// Companion vertex layout to use when dealing with this vertex type.
        /// </summary>
        public static VertexLayout VertexLayout => new VertexLayout(
            new VertexElement[] {
                new VertexElement(VertexSemantic.Position, 0, VertexFormat.Float3, 0)
            });

        /// <summary>
        /// Size of the structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<VertexPosition>();

        /// <summary>
        /// Vertex Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPosition"/> struct.
        /// </summary>
        /// <param name="position">The vertex position.</param>
        public VertexPosition(Vector3 position)
        {
            Position = position;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(VertexPosition a, VertexPosition b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator !=(VertexPosition a, VertexPosition b)
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
            if (obj is VertexPosition)
            {
                return Equals((VertexPosition)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPosition other)
        {
            return Equals(ref other);
        }
        
        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPosition other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPosition other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPosition other, float tolerance)
        {
            return Position.Equals(ref other.Position, tolerance);
        }

        /// <summary>
        /// Gets the layout of the vertex.
        /// </summary>
        /// <returns>Vertex layout</returns>
        public VertexLayout GetVertexLayout()
        {
            return VertexLayout;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Position.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Position:{0}", new object[] { Position.ToString() });
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            input.Read(out Position);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Position", ref Position);
        }
    }
}
