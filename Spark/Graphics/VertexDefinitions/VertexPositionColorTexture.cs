namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;
    using Content;

    /// <summary>
    /// Represents a vertex with position, vertex color, and texture coordinate data.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTexture : IEquatable<VertexPositionColorTexture>, IVertexType, IPrimitiveValue
    {
        /// <summary>
        /// Companion vertex layout to use when dealing with this vertex type.
        /// </summary>
        public static VertexLayout VertexLayout => new VertexLayout(
            new VertexElement[] {
                new VertexElement(VertexSemantic.Position, 0, VertexFormat.Float3, 0),
                new VertexElement(VertexSemantic.Color, 0, VertexFormat.Color, 12),
                new VertexElement(VertexSemantic.TextureCoordinate, 0, VertexFormat.Float2, 16)
            });

        /// <summary>
        /// Size of the structure in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<VertexPositionColorTexture>();

        /// <summary>
        /// Vertex Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Vertex color.
        /// </summary>
        public Color VertexColor;

        /// <summary>
        /// Vertex Texture Coordinate
        /// </summary>
        public Vector2 TextureCoordinate;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionColorTexture"/> struct.
        /// </summary>
        /// <param name="position">The vertex position.</param>
        /// <param name="vertexColor">The vertex color.</param>
        /// <param name="textureCoordinate">The vertex texture coordinate.</param>
        public VertexPositionColorTexture(Vector3 position, Color vertexColor, Vector2 textureCoordinate)
        {
            Position = position;
            VertexColor = vertexColor;
            TextureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(VertexPositionColorTexture a, VertexPositionColorTexture b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two vertices.
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(VertexPositionColorTexture a, VertexPositionColorTexture b)
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
            if (obj is VertexPositionColorTexture)
            {
                return Equals((VertexPositionColorTexture)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPositionColorTexture other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(VertexPositionColorTexture other, float tolerance)
        {
            return Equals(ref other, tolerance);
        }

        /// <summary>
        /// Tests equality between two vertices.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPositionColorTexture other)
        {
            return Equals(ref other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between two vertices within tolerance.
        /// </summary>
        /// <param name="other">Other vertex to compare to.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(ref VertexPositionColorTexture other, float tolerance)
        {
            return Position.Equals(ref other.Position, tolerance) && 
                   VertexColor.Equals(ref other.VertexColor) && 
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
                return Position.GetHashCode() + VertexColor.GetHashCode() + TextureCoordinate.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Position:{0}, VertexColor:{1}, TextureCoordinate:{2}", new object[] { Position.ToString(), VertexColor.ToString(), TextureCoordinate.ToString() });
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            input.Read(out Position);
            input.Read(out VertexColor);
            input.Read(out TextureCoordinate);
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Position", ref Position);
            output.Write("VertexColor", ref VertexColor);
            output.Write("TextureCoordinate", ref TextureCoordinate);
        }
    }
}
