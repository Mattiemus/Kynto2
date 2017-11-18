namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;
    using Content;

    /// <summary>
    /// Represents a vertex with position, normal, and texture coordinate data.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture : IEquatable<VertexPositionNormalTexture>, IVertexType, IPrimitiveValue
    {
        /// <summary>
        /// Companion vertex layout to use when dealing with this vertex type.
        /// </summary>
        public static VertexLayout VertexLayout => new VertexLayout(
            new VertexElement[] {
                new VertexElement(VertexSemantic.Position, 0, VertexFormat.Float3, 0),
                new VertexElement(VertexSemantic.Normal, 0, VertexFormat.Float3, 12),
                new VertexElement(VertexSemantic.TextureCoordinate, 0, VertexFormat.Float2, 24)
            });

        /// <summary>
        /// Size of the structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<VertexPositionNormalTexture>();

        /// <summary>
        /// Vertex Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Vertex Normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Vertex Texture Coordinate
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionNormalTexture"/> struct.
        /// </summary>
        /// <param name="position">The vertex position.</param>
        /// <param name="normal">The vertex normal.</param>
        /// <param name="textureCoordinate">The vertex texture coordinate.</param>
        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(VertexPositionNormalTexture a, VertexPositionNormalTexture b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator !=(VertexPositionNormalTexture a, VertexPositionNormalTexture b)
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
            if (obj is VertexPositionNormalTexture)
            {
                return Equals((VertexPositionNormalTexture)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPositionNormalTexture other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPositionNormalTexture other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPositionNormalTexture other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPositionNormalTexture other, float tolerance)
        {
            return Position.Equals(ref other.Position, tolerance) && 
                   Normal.Equals(ref other.Normal, tolerance) && 
                   TextureCoordinate.Equals(ref other.TextureCoordinate, tolerance);
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
                return Position.GetHashCode() + Normal.GetHashCode() + TextureCoordinate.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name. </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Position:{0}, Normal:{1}, TextureCoordinate:{2}", new object[] { Position.ToString(), Normal.ToString(), TextureCoordinate.ToString() });
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            input.Read(out Position);
            input.Read(out Normal);
            input.Read(out TextureCoordinate);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Position", ref Position);
            output.Write("Normal", ref Normal);
            output.Write("TextureCoordinate", ref TextureCoordinate);
        }
    }
}
