namespace Spark.Math
{
    using System;
    using System.Globalization;

    using Core;
    using Content;

    /// <summary>
    /// The Transform class represents a 3D SRT (scaling/rotation/translation) matrix. 
    /// Generally this class is used to represent a world matrix and has functions to set
    /// each transform part, compute the SRT matrix, and do parent-child transform combination.
    /// </summary>
    public sealed class Transform : IEquatable<Transform>, IFormattable, ISavable, IDeepCloneable
    {
        private Vector3 _scale;
        private Quaternion _rotation;
        private Vector3 _translation;
        private Matrix4x4 _cachedMatrix;
        private bool _cacheRefresh;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with unit scaling, no translation, and an identity rotation quaternion.
        /// </summary>
        public Transform()
        {
            _scale = Vector3.One;
            _rotation = Quaternion.Identity;
            _translation = Vector3.Zero;
            _cachedMatrix = Matrix4x4.Identity;
            _cacheRefresh = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class from the components of the supplied prototype.
        /// </summary>
        /// <param name="prototype">Transform to copy from</param>
        public Transform(Transform prototype)
        {
            _scale = prototype._scale;
            _rotation = prototype._rotation;
            _translation = prototype._translation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with the supplied components.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        public Transform(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            _scale = scale;
            _rotation = rotation;
            _translation = translation;
        }

        /// <summary>
        /// Gets or sets the scaling vector.
        /// </summary>
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets or sets the rotation quaternion.
        /// </summary>
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets or sets the translation vector.
        /// </summary>
        public Vector3 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets the computed SRT matrix.
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                if (_cacheRefresh)
                {
                    ComputeMatrix();
                }

                return _cachedMatrix;
            }
        }

        /// <summary>
        /// Copies the transform data into a new instance.
        /// </summary>
        /// <returns>Cloned transform.</returns>
        public Transform Clone()
        {
            return new Transform(this);
        }

        /// <summary>
        /// Get a copy of the object.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        IDeepCloneable IDeepCloneable.Clone()
        {
            return new Transform(this);
        }


        /// <summary>
        /// Sets the transform with the store from the supplied transform.
        /// </summary>
        /// <param name="transform">Transform to copy from</param>
        public void Set(Transform transform)
        {
            _scale = transform._scale;
            _rotation = transform._rotation;
            _translation = transform._translation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform from a (S)cale-(R)otation-(T)ranslation matrix.
        /// </summary>
        /// <param name="matrix">Matrix to decompose the scale/rotation/translation components from.</param>
        public void Set(Matrix4x4 matrix)
        {
            Set(ref matrix);
        }

        /// <summary>
        /// Sets the transform from a (S)cale-(R)otation-(T)ranslation matrix.
        /// </summary>
        /// <param name="matrix">Matrix to decompose the scale/rotation/translation components from.</param>
        public void Set(ref Matrix4x4 matrix)
        {
            matrix.Decompose(out _scale, out _rotation, out _translation);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform with the supplied components.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        public void Set(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            Set(ref scale, ref rotation, ref translation);
        }

        /// <summary>
        /// Sets the transform with the supplied components.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        public void Set(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            _scale = scale;
            _rotation = rotation;
            _translation = translation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's translation vector from the supplied coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public void SetTranslation(float x, float y, float z)
        {
            _translation.X = x;
            _translation.Y = y;
            _translation.Z = z;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's translation vector from the supplied coordinates.
        /// </summary>
        /// <param name="translation">Translation vector</param>
        public void SetTranslation(ref Vector3 translation)
        {
            _translation = translation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's scaling vector with the supplied scaling factors for each axis.
        /// </summary>
        /// <param name="x">Scaling on x axis</param>
        /// <param name="y">Scaling on y axis</param>
        /// <param name="z">Scaling on z axis</param>
        public void SetScale(float x, float y, float z)
        {
            _scale.X = x;
            _scale.Y = y;
            _scale.Z = z;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's scaling vector with a single uniform index.
        /// </summary>
        /// <param name="scale">Uniform scaling index</param>
        public void SetScale(float scale)
        {
            _scale.X = scale;
            _scale.Y = scale;
            _scale.Z = scale;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's scaling vector with the supplied scaling factors for each axis.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        public void SetScale(ref Vector3 scale)
        {
            _scale = scale;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the rotation quaternion from a rotation matrix. The matrix *must* represent a rotation!
        /// This method does NOT check if if the matrix is a valid rotation matrix.
        /// </summary>
        /// <param name="rotMatrix">Rotation matrix</param>
        public void SetRotation(Matrix4x4 rotMatrix)
        {
            SetRotation(ref rotMatrix);
        }

        /// <summary>
        /// Sets the rotation quaternion from a rotation matrix. The matrix *must* represent a rotation!
        /// This method does NOT check if if the matrix is a valid rotation matrix.
        /// </summary>
        /// <param name="rotMatrix">Rotation matrix</param>
        public void SetRotation(ref Matrix4x4 rotMatrix)
        {
            Quaternion.FromRotationMatrix(ref rotMatrix, out _rotation);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the rotation quaternion.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public void SetRotation(ref Quaternion rotation)
        {
            _rotation = rotation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the SRT components of the transform.
        /// </summary>
        /// <param name="scale">Scaling vector.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="translation">Translation vector.</param>
        public void SetComponents(ref Vector3 scale, ref Quaternion rotation, ref Vector3 translation)
        {
            _scale = scale;
            _rotation = rotation;
            _translation = translation;
        }

        /// <summary>
        /// Sets the transform to its identity state - scaling vector is all 1's, translation is all 0's, and
        /// the rotation quaternion is the identity.
        /// </summary>
        public void SetIdentity()
        {
            _scale = Vector3.One;
            _rotation = Quaternion.Identity;
            _translation = Vector3.Zero;
            _cachedMatrix = Matrix4x4.Identity;
            _cacheRefresh = false;
        }

        /// <summary>
        /// Returns the calculated row vector of the 3x3 rotation matrix (represented by the quaternion).
        /// Row 0 is Right, row 1 is Up, and row 2 is Forward.
        /// </summary>
        /// <param name="i">Row index, must be between 0 and 2</param>
        /// <returns>Row vector of the rotation matrix</returns>
        public Vector3 GetRotationVector(int i)
        {
            if (i > 2 || i < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Index must be between 0 and 2.");
            }
            
            Quaternion.GetRotationVector(ref _rotation, i, out Vector3 row);
            return row;
        }

        /// <summary>
        /// Gets the scaling vector.
        /// </summary>
        /// <param name="scale">Scaling vector.</param>
        public void GetScale(out Vector3 scale)
        {
            scale = _scale;
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public void GetRotation(out Quaternion rotation)
        {
            rotation = _rotation;
        }

        /// <summary>
        /// Gets the translation vector.
        /// </summary>
        /// <param name="translation">Translation vector.</param>
        public void GetTranslation(out Vector3 translation)
        {
            translation = _translation;
        }

        /// <summary>
        /// Gets the computed SRT matrix.
        /// </summary>
        /// <param name="result">SRT matrix.</param>
        public void GetMatrix(out Matrix4x4 result)
        {
            if (_cacheRefresh)
            {
                ComputeMatrix();
            }

            result = _cachedMatrix;
        }

        /// <summary>
        /// Gets the SRT components of the transform.
        /// </summary>
        /// <param name="scale">Scaling vector.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="translation">Translation vector.</param>
        public void GetComponents(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            scale = _scale;
            rotation = _rotation;
            translation = _translation;
        }

        /// <summary>
        /// Combines this transform with a transform that represents its "parent".
        /// </summary>
        /// <param name="parent">Parent transform</param>
        public void CombineWithParent(Transform parent)
        {
            // Multiply scaling
            Vector3.Multiply(ref parent._scale, ref _scale, out _scale);

            // Multiply rotation
            Quaternion.Multiply(ref parent._rotation, ref _rotation, out _rotation);

            // Combine translation
            Vector3.Multiply(ref _translation, ref parent._scale, out _translation);
            Vector3.Transform(ref _translation, ref parent._rotation, out _translation);
            Vector3.Add(ref _translation, ref parent._translation, out _translation);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Interpolates between two transforms, setting the result to this transform. Slerp is applied
        /// to the rotations and Lerp to the translation/scale.
        /// </summary>
        /// <param name="start">Starting transform</param>
        /// <param name="end">Ending transform</param>
        /// <param name="percent">Percent to interpolate between the two transforms, must be between 0 and 1</param>
        public void InterpolateTransforms(Transform start, Transform end, float percent)
        {
            Quaternion.Slerp(ref start._rotation, ref end._rotation, percent, out _rotation);
            Vector3.Lerp(ref start._scale, ref end._scale, percent, out _scale);
            Vector3.Lerp(ref start._translation, ref end._translation, percent, out _translation);
        }

        /// <summary>
        /// Transforms the supplied vector.
        /// </summary>
        /// <param name="v">Vector3 to be transformed</param>
        /// <returns>Transformed vector</returns>
        public Vector3 TransformVector(Vector3 v)
        {
            Vector3.Multiply(ref v, ref _scale, out Vector3 result);
            Vector3.Transform(ref result, ref _rotation, out result);
            Vector3.Add(ref result, ref _translation, out result);
            return result;
        }

        /// <summary>
        /// Transforms the supplied vector.
        /// </summary>
        /// <param name="v">Vector3 to be transformed</param>
        /// <param name="result">Existing Vector3 to hold result</param>
        public void TransformVector(ref Vector3 v, out Vector3 result)
        {
            Vector3.Multiply(ref v, ref _scale, out result);
            Vector3.Transform(ref result, ref _rotation, out result);
            Vector3.Add(ref result, ref _translation, out result);
        }

        /// <summary>
        /// Tests equality between two transforms.
        /// </summary>
        /// <param name="a">First transform</param>
        /// <param name="b">Second transform</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(Transform a, Transform b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Tests inequality between two transforms.
        /// </summary>
        /// <param name="a">First transform</param>
        /// <param name="b">Second transform</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(Transform a, Transform b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Tests equality between this transform and another.
        /// </summary>
        /// <param name="other">Other transform to compare against.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(Transform other)
        {
            return Equals(other, MathHelper.ZeroTolerance);
        }

        /// <summary>
        /// Tests equality between this transform and another.
        /// </summary>
        /// <param name="other">Other transform to compare against.</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(Transform other, float tolerance)
        {
            if (other == null)
            {
                return false;
            }

            return _scale.Equals(ref other._scale, tolerance) && _rotation.Equals(ref other._rotation, tolerance) && _translation.Equals(ref other._translation, tolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to the current <see cref="object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            Transform other = obj as Transform;
            if (other != null)
            {
                return _scale.Equals(ref other._scale) && _rotation.Equals(ref other._rotation) && _translation.Equals(ref other._translation);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.  </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return _scale.GetHashCode() + _rotation.GetHashCode() + _translation.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString("G", formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return ToString();
            }

            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(formatProvider, "Scale: {0}, Rotation: {1}, Translation: {2}", new object[] { _scale.ToString(format, formatProvider), _rotation.ToString(format, formatProvider), _translation.ToString(format, formatProvider) });
        }
    
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            _scale = input.Read<Vector3>();
            _rotation = input.Read<Quaternion>();
            _translation = input.Read<Vector3>();
            ComputeMatrix();
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Scale", _scale);
            output.Write("Rotation", _rotation);
            output.Write("Translation", _translation);
        }

        /// <summary>
        /// Computes the transformation matrix
        /// </summary>
        private void ComputeMatrix()
        {
            Matrix4x4.FromScale(ref _scale, out Matrix4x4 scaleM);
            Matrix4x4.FromQuaternion(ref _rotation, out Matrix4x4 rotationM);
            Matrix4x4.FromTranslation(ref _translation, out Matrix4x4 translationM);

            // ((Scale * Rotation) * Translation)
            Matrix4x4.Multiply(ref scaleM, ref rotationM, out _cachedMatrix);
            Matrix4x4.Multiply(ref _cachedMatrix, ref translationM, out _cachedMatrix);
            _cacheRefresh = false;
        }
    }
}
