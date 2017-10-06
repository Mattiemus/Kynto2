namespace Spark.Graphics
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Math;

    /// <summary>
    /// Describes multisample anti-aliasing (MSAA) options for creating resources.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSAADescription : IEquatable<MSAADescription>
    {
        /// <summary>
        /// The multisample count.
        /// </summary>
        public int Count;

        /// <summary>
        /// The vendor-specific multisample quality level.
        /// </summary>
        public int QualityLevel;

        /// <summary>
        /// If the MSAA resource should be resolved to non-MSAA resource for shader input or not.
        /// </summary>
        public bool ResolveShaderResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSAADescription"/> struct. The resource is set to not be
        /// resolved and quality level of zero.
        /// </summary>
        /// <param name="count">The multisample count.</param>
        public MSAADescription(int count)
        {
            Count = Math.Max(1, count);
            QualityLevel = 0;
            ResolveShaderResource = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSAADescription"/> struct. The resource is set to not be resolved.
        /// </summary>
        /// <param name="count">The multisample count.</param>
        /// <param name="qualityLevel">The vendor-specific quality level.</param>
        public MSAADescription(int count, int qualityLevel)
        {
            Count = Math.Max(1, count);
            QualityLevel = Math.Max(0, qualityLevel);
            ResolveShaderResource = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSAADescription"/> struct.
        /// </summary>
        /// <param name="count">The multisample count.</param>
        /// <param name="qualityLevel">The vendor-specific quality level.</param>
        /// <param name="resolveShaderResource">True if the MSAA resource should be resolved to a non-MSAA resource for shader input or not.</param>
        public MSAADescription(int count, int qualityLevel, bool resolveShaderResource)
        {
            Count = Math.Max(1, count);
            QualityLevel = Math.Max(0, qualityLevel);
            ResolveShaderResource = resolveShaderResource;
        }

        /// <summary>
        /// Gets the default MSAA description, which means no multisampling.
        /// </summary>
        public static MSAADescription Default => new MSAADescription(1, 0, false);

        /// <summary>
        /// Gets if the MSAA Description will enable multisampling or not
        /// </summary>
        public bool IsMultisampled => Count > 1;

        /// <summary>
        /// Checks the preferred configuration. If any values are not supported by the graphics adapter, values that are approximate and valid
        /// are set.
        /// </summary>
        /// <param name="adapter">The graphics adapter</param>
        /// <param name="format">The format of the resource.</param>
        /// <param name="preferredMSAA">The preferred MSAA settings.</param>
        /// <returns>Return true if the MSAA settings have been configured or not.</returns>
        public static bool CheckMSAAConfiguration(IGraphicsAdapter adapter, SurfaceFormat format, ref MSAADescription preferredMSAA)
        {
            if (adapter == null)
            {
                return false;
            }

            if (preferredMSAA.ResolveShaderResource && !adapter.IsMultisampleResolvable(format))
            {
                preferredMSAA.ResolveShaderResource = false;
            }

            int maxQualityLevels = adapter.CheckMultisampleQualityLevels(format, preferredMSAA.Count);
            if (maxQualityLevels == 0)
            {
                int count = Math.Max(1, preferredMSAA.Count);
                int quality = preferredMSAA.QualityLevel;
                preferredMSAA.Count = 1;
                preferredMSAA.QualityLevel = 0;

                while (count > 1)
                {
                    count--;
                    maxQualityLevels = adapter.CheckMultisampleQualityLevels(format, count);

                    // If found a valid quality level for count, set it and clamp our preferred quality level
                    if (quality > 0)
                    {
                        preferredMSAA.Count = count;
                        preferredMSAA.QualityLevel = MathHelper.Clamp(quality, 0, maxQualityLevels - 1);
                        break;
                    }
                }
            }
            else
            {
                // MSAA count is ok, just ensure we clamp quality level
                preferredMSAA.QualityLevel = MathHelper.Clamp(preferredMSAA.QualityLevel, 0, maxQualityLevels - 1);
            }

            return true;
        }

        /// <summary>
        /// Tests equality between two MSAA descriptions.
        /// </summary>
        /// <param name="a">First MSAA description</param>
        /// <param name="b">Second MSAA description</param>
        /// <returns>True if both are equal, false otherwise</returns>
        public static bool operator ==(MSAADescription a, MSAADescription b)
        {
            return a.Equals(ref b);
        }

        /// <summary>
        /// Tests inequality between two MSAA descriptions.
        /// </summary>
        /// <param name="a">First MSAA description</param>
        /// <param name="b">Second MSAA description</param>
        /// <returns>True if both are not equal, false otherwise</returns>
        public static bool operator !=(MSAADescription a, MSAADescription b)
        {
            return !a.Equals(ref b);
        }

        /// <summary>
        /// Tests equality between this instance and another.
        /// </summary>
        /// <param name="other">Other MSAA description to compare to</param>
        /// <returns>True if the MSAA descriptions are equal, false otherwise</returns>
        public bool Equals(MSAADescription other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Tests equality between this instance and another.
        /// </summary>
        /// <param name="other">Other MSAA description to compare to</param>
        /// <returns>True if the MSAA descriptions are equal, false otherwise</returns>
        public bool Equals(ref MSAADescription other)
        {
            return (Count == other.Count) && (QualityLevel == other.QualityLevel) && (ResolveShaderResource == other.ResolveShaderResource);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is MSAADescription)
            {
                MSAADescription other = (MSAADescription)obj;
                return Equals(ref other);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Count.GetHashCode() + QualityLevel.GetHashCode() + ResolveShaderResource.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return string.Format(info, "Count: {0}, QualityLevel: {1}, ResolveShaderResource: {2}", Count.ToString(), QualityLevel.ToString(), ResolveShaderResource.ToString());
        }
    }
}
