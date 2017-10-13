namespace Spark.OpenGL.Graphics
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    using OGL = OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// Wrapper to optimise updating the OpenGL state. This will attempt to reduce the number of calls to OpenGL (and therefor improve performance)
    /// by holding a local cached copy of the state, and only pushing updates if the new state is different from the current state.
    /// </summary>
    public sealed class OpenGLState
    {
        private readonly Dictionary<OGL.EnableCap, bool> _capabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLState"/> class
        /// </summary>
        public OpenGLState()
        {
            _capabilities = Enum
                .GetValues(typeof(OGL.EnableCap))
                .Cast<OGL.EnableCap>()
                .Distinct()
                .ToDictionary(cap => cap, OGL.GL.IsEnabled);
        }
        
        /// <summary>
        /// Gets a value indicating whether the given capability is enabled
        /// </summary>
        /// <param name="cap">Capability to test</param>
        /// <returns>True if the capability is enabled/returns>
        public bool IsEnabled(OGL.EnableCap cap)
        {
            return _capabilities[cap];
        }
        
        /// <summary>
        /// Sets a state to either enabled or disabled
        /// </summary>
        /// <param name="cap">Capability to set</param>
        /// <param name="isEnabled">True if the state should be enabled, false otherwise</param>
        /// <returns>True if the state was updated, false if the state was already set to the desired value</returns>
        public bool SetEnabled(OGL.EnableCap cap, bool isEnabled)
        {
            if (isEnabled && !_capabilities[cap])
            {
                OGL.GL.Enable(cap);
                _capabilities[cap] = true;
                return true;
            }
            else if (!isEnabled && _capabilities[cap])
            {
                OGL.GL.Disable(cap);
                _capabilities[cap] = false;
                return true;
            }

            return false;
        }
    }
}
