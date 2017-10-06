namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;
    using Content;
    using Core.Interop;

    /// <summary>
    /// Defines a 2D region that a 3D rendering is projected onto where the positive X axis is right and positive Y axis is down.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Viewport : IEquatable<Viewport>, IPrimitiveValue
    {
        /// <summary>
        /// Top left X coordinate of the viewport.
        /// </summary>
        public int X;

        /// <summary>
        /// Top left Y coordinate of the viewport.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of the viewport.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the viewport.
        /// </summary>
        public int Height;

        /// <summary>
        /// Minimum Z depth.
        /// </summary>
        public float MinDepth;

        /// <summary>
        /// Maximum Z depth.
        /// </summary>
        public float MaxDepth;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x">Top left X coordinate</param>
        /// <param name="y">Top left Y coordinate</param>
        /// <param name="width">Width of the viewport</param>
        /// <param name="height">Height of the viewport</param>
        public Viewport(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = 0.0f;
            MaxDepth = 1.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x">Top left X coordinate</param>
        /// <param name="y">Top left Y coordinate</param>
        /// <param name="width">Width of the viewport</param>
        /// <param name="height">Height of the viewport</param>
        /// <param name="minDepth">Minimum depth</param>
        /// <param name="maxDepth">Maximum depth</param>
        public Viewport(int x, int y, int width, int height, float minDepth, float maxDepth)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct. Min depth is set to 0.0 and Max depth to 1.0.
        /// </summary>
        /// <param name="bounds">The rectangle that represents the bounds of the viewport.</param>
        public Viewport(Rectangle bounds)
        {
            X = bounds.X;
            Y = bounds.Y;
            Width = bounds.Width;
            Height = bounds.Height;
            MinDepth = 0.0f;
            MaxDepth = 1.0f;
        }

        /// <summary>
        /// Gets the size of the viewport type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Viewport>();

        /// <summary>
        /// Gets or sets the bounds of the viewport as a rectangle.
        /// </summary>
        public Rectangle Bounds
        {
            get => new Rectangle(X, Y, Width, Height);
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets the aspect ratio of the viewport, which is width divided by height.
        /// </summary>
        public float AspectRatio
        {
            get
            {
                if (Height != 0 && Width != 0)
                {
                    return Width / (float)Height;
                }

                return 0.0f;
            }
        }

        /// <summary>
        /// Tests equality between two viewports.
        /// </summary>
        /// <param name="a">First viewport</param>
        /// <param name="b">Second viewport</param>
        /// <returns>True if the two viewports are equal, false otherwise.</returns>
        public static bool operator ==(Viewport a, Viewport b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two viewports.
        /// </summary>
        /// <param name="a">First viewport</param>
        /// <param name="b">Second viewport</param>
        /// <returns>True if the two viewports are not equal, false otherwise.</returns>
        public static bool operator !=(Viewport a, Viewport b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Projects a 3D vector from object space to screen space.
        /// </summary>
        /// <param name="source">Vector to project</param>
        /// <param name="worldMatrix">World matrix</param>
        /// <param name="viewMatrix">View matrix</param>
        /// <param name="projMatrix">Projection matrix</param>
        /// <returns>The projected vector in screen space.</returns>
        public Vector3 Project(Vector3 source, Matrix4x4 worldMatrix, Matrix4x4 viewMatrix, Matrix4x4 projMatrix)
        {
            Matrix4x4.Multiply(ref worldMatrix, ref viewMatrix, out Matrix4x4 wv);
            Matrix4x4.Multiply(ref wv, ref projMatrix, out Matrix4x4 wvp);
            
            Project(ref source, ref wvp, out Vector3 result);

            return result;
        }

        /// <summary>
        /// Projects a 3D vector from object space to screen space.
        /// </summary>
        /// <param name="source">Vector to project</param>
        /// <param name="worldMatrix">World matrix</param>
        /// <param name="viewMatrix">View matrix</param>
        /// <param name="projMatrix">Projection matrix</param>
        /// <param name="result">The projected vector in screen space.</param>
        public void Project(ref Vector3 source, ref Matrix4x4 worldMatrix, ref Matrix4x4 viewMatrix, ref Matrix4x4 projMatrix, out Vector3 result)
        {
            Matrix4x4.Multiply(ref worldMatrix, ref viewMatrix, out Matrix4x4 wv);
            Matrix4x4.Multiply(ref wv, ref projMatrix, out Matrix4x4 wvp);

            Project(ref source, ref wvp, out result);
        }

        /// <summary>
        /// Projects a 3D vector from object space to screen space.
        /// </summary>
        /// <param name="source">Vector to project</param>
        /// <param name="worldViewProjectionMatrix">The World-View-Projection matrix</param>
        /// <returns>The projected vector in screen space.</returns>
        public Vector3 Project(Vector3 source, Matrix4x4 worldViewProjectionMatrix)
        {
            Project(ref source, ref worldViewProjectionMatrix, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Projects a 3D vector from object space to screen space.
        /// </summary>
        /// <param name="source">Vector to project</param>
        /// <param name="worldViewProjectionMatrix">The World-View-Projection matrix</param>
        /// <param name="result">The projected vector in screen space.</param>
        public void Project(ref Vector3 source, ref Matrix4x4 worldViewProjectionMatrix, out Vector3 result)
        {
            Vector3.Transform(ref source, ref worldViewProjectionMatrix, out result);

            float w = (source.X * worldViewProjectionMatrix.M14) + (source.Y * worldViewProjectionMatrix.M24) + (source.Z * worldViewProjectionMatrix.M34) + worldViewProjectionMatrix.M44;
            if (!MathHelper.IsApproxEquals(w, 1.0f))
            {
                Vector3.Divide(ref result, w, out result);
            }

            result.X = (((result.X + 1.0f) * 0.5f) * Width) + X;
            result.Y = (((-result.Y + 1.0f) * 0.5f) * Height) + Y;
            result.Z = (result.Z * (MaxDepth - MinDepth)) + MinDepth;
        }

        /// <summary>
        /// Converts a point in screen space to a point in object space.
        /// </summary>
        /// <param name="source">Vector to un-project (in screen coordinates)</param>
        /// <param name="worldMatrix">World matrix</param>
        /// <param name="viewMatrix">View matrix</param>
        /// <param name="projMatrix">Projection matrix</param>
        /// <returns>The un-projected vector in object space.</returns>
        public Vector3 UnProject(Vector3 source, Matrix4x4 worldMatrix, Matrix4x4 viewMatrix, Matrix4x4 projMatrix)
        {
            Matrix4x4.Multiply(ref worldMatrix, ref viewMatrix, out Matrix4x4 wv);
            Matrix4x4.Multiply(ref wv, ref projMatrix, out Matrix4x4 wvp);
            
            UnProject(ref source, ref wvp, out Vector3 result);

            return result;
        }

        /// <summary>
        /// Converts a point in screen space to a point in object space.
        /// </summary>
        /// <param name="source">Vector to un-project (in screen coordinates)</param>
        /// <param name="worldMatrix">World matrix</param>
        /// <param name="viewMatrix">View matrix</param>
        /// <param name="projMatrix">Projection matrix</param>
        /// <param name="result">The un-projected vector in object space.</param>
        public void UnProject(ref Vector3 source, ref Matrix4x4 worldMatrix, ref Matrix4x4 viewMatrix, ref Matrix4x4 projMatrix, out Vector3 result)
        {
            Matrix4x4.Multiply(ref worldMatrix, ref viewMatrix, out Matrix4x4 wv);
            Matrix4x4.Multiply(ref wv, ref projMatrix, out Matrix4x4 wvp);

            UnProject(ref source, ref wvp, out result);
        }

        /// <summary>
        /// Converts a point in screen space to a point in object space.
        /// </summary>
        /// <param name="source">Vector to un-project (in screen coordinates)</param>
        /// <param name="worldViewProjectionMatrix">The World-View-Projection matrix</param>
        /// <returns>The un-projected vector in object space.</returns>
        public Vector3 UnProject(Vector3 source, Matrix4x4 worldViewProjectionMatrix)
        {
            Matrix4x4.Invert(ref worldViewProjectionMatrix, out Matrix4x4 invWvp);

            Vector3 temp;
            temp.X = (((source.X - X) / Width) * 2.0f) - 1.0f;
            temp.Y = -((((source.Y - Y) / Height) * 2.0f) - 1.0f);
            temp.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
            
            Vector3.Transform(ref temp, ref invWvp, out Vector3 result);

            float w = (temp.X * invWvp.M14) + (temp.Y * invWvp.M24) + (temp.Z * invWvp.M34) + invWvp.M44;
            if (!MathHelper.IsApproxEquals(w, 1.0f))
            {
                Vector3.Divide(ref result, w, out result);
            }

            return result;
        }

        /// <summary>
        /// Converts a point in screen space to a point in object space.
        /// </summary>
        /// <param name="source">Vector to un-project (in screen coordinates)</param>
        /// <param name="worldViewProjectionMatrix">The World-View-Projection matrix</param>
        /// <param name="result">The un-projected vector in object space.</param>
        public void UnProject(ref Vector3 source, ref Matrix4x4 worldViewProjectionMatrix, out Vector3 result)
        {
            Matrix4x4.Invert(ref worldViewProjectionMatrix, out Matrix4x4 invWvp);

            Vector3 temp;
            temp.X = (((source.X - X) / Width) * 2.0f) - 1.0f;
            temp.Y = -((((source.Y - Y) / Height) * 2.0f) - 1.0f);
            temp.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);

            Vector3.Transform(ref temp, ref invWvp, out result);

            float w = (temp.X * invWvp.M14) + (temp.Y * invWvp.M24) + (temp.Z * invWvp.M34) + invWvp.M44;
            if (!MathHelper.IsApproxEquals(w, 1.0f))
            {
                Vector3.Divide(ref result, w, out result);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Viewport)
            {
                return Equals((Viewport)obj);
            }

            return false;
        }

        /// <summary>
        /// Tests equality between the viewport and another viewport.
        /// </summary>
        /// <param name="other">Viewport to test</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(Viewport other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between the viewport and another viewport.
        /// </summary>
        /// <param name="other">Viewport to test</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(ref Viewport other)
        {
            return (X == other.X) && (Y == other.Y) && 
                   (Width == other.Width) && (Height == other.Height) && 
                   MathHelper.IsApproxEquals(MinDepth, other.MinDepth) && MathHelper.IsApproxEquals(MaxDepth, other.MaxDepth);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() + Y.GetHashCode() + Width.GetHashCode() + Height.GetHashCode() + MaxDepth.GetHashCode() + MinDepth.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> containing a fully qualified type name.</returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "X:{0} Y:{1} Width:{2} Height:{3} MinDepth:{4} MaxDepth:{5}", new object[] { X.ToString(info), Y.ToString(info), Width.ToString(info), Height.ToString(info), MinDepth.ToString(info), MaxDepth.ToString(info) });
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("X", X);
            output.Write("Y", Y);
            output.Write("Width", Width);
            output.Write("Height", Height);
            output.Write("MinDepth", MinDepth);
            output.Write("MaxDepth", MaxDepth);
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            X = input.ReadInt32();
            Y = input.ReadInt32();
            Width = input.ReadInt32();
            Height = input.ReadInt32();
            MinDepth = input.ReadSingle();
            MaxDepth = input.ReadSingle();
        }
    }
}
