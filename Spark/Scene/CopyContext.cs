namespace Spark.Scene
{
    using System.Collections.Generic;

    /// <summary>
    /// Context for copying scenegraph information. It contains a collection of parameters, each represented by
    /// a key, instructing the Spatials of how to copy data.
    /// </summary>
    public class CopyContext
    {
        /// <summary>
        /// Do a deep copy of materials or share them.
        /// </summary>
        public static readonly string CopyMaterialKey = "CopyMaterial";

        /// <summary>
        /// Do a deep copy of mesh data or shares them.
        /// </summary>
        public static readonly string CopyMeshDataKey = "CopyMeshData";

        private readonly Dictionary<string, bool> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyContext"/> class.
        /// </summary>
        public CopyContext()
        {
            _properties = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Gets a context that instructs Visuals to clone material instances. Default is to share.
        /// </summary>
        public static CopyContext CopyMaterialContext
        {
            get
            {
                CopyContext c = new CopyContext();
                c[CopyMaterialKey] = true;

                return c;
            }
        }

        /// <summary>
        /// Gets a context that instructs Meshes to clone mesh data instances. Default is to share.
        /// </summary>
        public static CopyContext CopyMeshDataContext
        {
            get
            {
                CopyContext c = new CopyContext();
                c[CopyMeshDataKey] = true;

                return c;
            }
        }

        /// <summary>
        /// Gets a context that instructs Visuals and Meshes to clone materials and mesh data instances. Default is to share.
        /// </summary>
        public static CopyContext CopyMaterialMeshDataContext
        {
            get
            {
                CopyContext c = new CopyContext();
                c[CopyMaterialKey] = true;
                c[CopyMeshDataKey] = true;

                return c;
            }
        }

        /// <summary>
        /// Gets the copy parameter by its key.
        /// </summary>
        /// <param name="key">Key of the copy parameter.</param>
        /// <returns>Copy parameter, or false if the key was not found.</returns>
        public bool this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    return false;
                }

                if (_properties.TryGetValue(key, out bool value))
                {
                    return value;
                }

                return false;
            }
            set
            {
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Tries to get a copy parameter by its key.
        /// </summary>
        /// <param name="key">Key of the copy parameter.</param>
        /// <param name="value">Copy parameter, or false if it was not found.</param>
        /// <returns>True if the copy parameter was found, false otherwise.</returns>
        public bool TryGetValue(string key, out bool value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = false;
                return false;
            }

            if (_properties.TryGetValue(key, out value))
            {
                return true;
            }

            return false;
        }
    }
}
