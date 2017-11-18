namespace Spark.Graphics
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Read only collection for display modes.
    /// </summary>
    public sealed class DisplayModeCollection : ReadOnlyList<DisplayMode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayModeCollection"/> class.
        /// </summary>
        /// <param name="displayModes">Display modes to initialize the collection with.</param>
        public DisplayModeCollection(IEnumerable<DisplayMode> displayModes) 
            : base(displayModes)
        {
        }

        /// <summary>
        /// Gets all the display modes associated with the specified format.
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>List of all the display modes that are associated with the specified format.</returns>
        public IEnumerable<DisplayMode> this[SurfaceFormat format]
        {
            get
            {
                List<DisplayMode> modes = new List<DisplayMode>();
                for (int i = 0; i < Count; i++)
                {
                    DisplayMode mode = this[i];
                    if (mode.SurfaceFormat == format)
                    {
                        modes.Add(mode);
                    }
                }

                return modes;
            }
        }
    }
}
