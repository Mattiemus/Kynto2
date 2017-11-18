namespace Spark.Application
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// A read-only collection of windows.
    /// </summary>
    public sealed class WindowCollection : ReadOnlyList<IWindow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCollection"/> class.
        /// </summary>
        /// <param name="windowList">Collection of windows</param>
        public WindowCollection(IList<IWindow> windowList) 
            : base(windowList)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCollection"/> class.
        /// </summary>
        /// <param name="windows">Collection of windows</param>
        public WindowCollection(IEnumerable<IWindow> windows) 
            : base(windows)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCollection"/> class.
        /// </summary>
        /// <param name="windows">Collection of windows</param>
        public WindowCollection(params IWindow[] windows) 
            : base(windows)
        {
        }

        /// <summary>
        /// Gets an <see cref="IWindow"/> contained in this collection that corresponds to the specified window handle.
        /// </summary>
        /// <param name="handle">Window handle.</param>
        /// <returns>Corresponding window, or null if none match.</returns>
        public IWindow this[IntPtr handle]
        {
            get
            {
                foreach (IWindow window in this)
                {
                    if (window != null && window.Handle == handle)
                    {
                        return window;
                    }
                }

                return null;
            }
        }
    }
}
