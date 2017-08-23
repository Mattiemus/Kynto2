namespace Spark.Graphics
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Math;
    using Implementation;

    using OGL = OpenTK.Graphics.OpenGL;
    
    public sealed class OpenGLState
    {
        private OGL.FrontFaceDirection _frontFace;
        private OGL.CullFaceMode? _cullFace;
        private OpenGLShaderProgram _currentProgram;
        private readonly bool[] _enabledAttributes;
        private readonly Dictionary<OGL.EnableCap, bool> _capabilities = new Dictionary<OGL.EnableCap, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLState"/> class
        /// </summary>
        public OpenGLState()
        {
            // Enabled statuses
            foreach (OGL.EnableCap capability in Enum.GetValues(typeof(OGL.EnableCap)).Cast<OGL.EnableCap>().Distinct())
            {
                _capabilities.Add(capability, OGL.GL.IsEnabled(capability));
            }

            //  Create the vertex attributes list
            int maxVertexAttribs = OGL.GL.GetInteger(OGL.GetPName.MaxVertexAttribs);
            _enabledAttributes = new bool[maxVertexAttribs];

            // Other properties
            _frontFace = (OGL.FrontFaceDirection)OGL.GL.GetInteger(OGL.GetPName.FrontFace);
            _cullFace = (OGL.CullFaceMode)OGL.GL.GetInteger(OGL.GetPName.CullFaceMode);

            // State management objects
            ColorBuffer = new ColorBufferState(this);
            DepthBuffer = new DepthBufferState(this);
            StencilBuffer = new StencilBufferState(this);

            // Initialize
            Initialize();
        }

        /// <summary>
        /// Gets the color buffer state manager
        /// </summary>
        public ColorBufferState ColorBuffer { get; }

        /// <summary>
        /// Gets the depth buffer state manager
        /// </summary>
        public DepthBufferState DepthBuffer { get; }

        /// <summary>
        /// Gets the stencil buffer state manager
        /// </summary>
        public StencilBufferState StencilBuffer { get; }

        /// <summary>
        /// Gets or sets the current front face value
        /// </summary>
        public OGL.FrontFaceDirection FrontFace
        {
            get
            {
                return _frontFace;
            }
            set
            {
                if (_frontFace == value)
                {
                    return;
                }

                OGL.GL.FrontFace(value);
                _frontFace = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the current culling direction
        /// </summary>
        public OGL.CullFaceMode? CullFace
        {
            get
            {
                if (IsEnabled(OGL.EnableCap.CullFace))
                {
                    return _cullFace;
                }

                return null;
            }
            set
            {
                if (_cullFace == value)
                {
                    return;
                }

                if (value.HasValue)
                {
                    Enable(OGL.EnableCap.CullFace);
                    OGL.GL.CullFace(value.Value);
                }
                else
                {
                    Disable(OGL.EnableCap.CullFace);
                }

                _cullFace = value;
            }
        }

        /// <summary>
        /// Sets the specified program as the current shader program
        /// </summary>
        /// <param name="program">Program to be used</param>
        /// <returns>True if the value needed to be set, false if it was already set</returns>
        public bool UseProgram(OpenGLShaderProgram program)
        {
            if (!ReferenceEquals(program, _currentProgram))
            {
                OGL.GL.UseProgram(program.ResourceId);
                _currentProgram = program;
                return true;
            }

            return false;
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
        /// Enables an OpenGL capability
        /// </summary>
        /// <param name="cap"></param>
        /// <returns>True if the value needed to be set, false if it was already set</returns>
        public bool Enable(OGL.EnableCap cap)
        {
            if (!_capabilities[cap])
            {
                OGL.GL.Enable(cap);
                _capabilities[cap] = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disables an OpenGL capability
        /// </summary>
        /// <param name="cap"></param>
        /// <returns>True if the value needed to be set, false if it was already set</returns>
        public bool Disable(OGL.EnableCap cap)
        {
            if (!_capabilities[cap])
            {
                OGL.GL.Disable(cap);
                _capabilities[cap] = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes the default values
        /// </summary>
        private void Initialize()
        {
            // Other defaults
            ColorBuffer.ClearValue = LinearColor.Black;
            DepthBuffer.ClearValue = 1.0f;
            StencilBuffer.ClearValue = 0;

            // TODO: remove
            return;

            // Depth testing
            Enable(OGL.EnableCap.DepthTest);
            //depthBuffer.setFunc(LessEqualDepth);
            
            // Backface culling
            FrontFace = OGL.FrontFaceDirection.Ccw;
            CullFace = OGL.CullFaceMode.Back;
            Enable(OGL.EnableCap.CullFace);

            // Enable blending
            Enable(OGL.EnableCap.Blend);
            //setBlending(NormalBlending);
        }

        /// <summary>
        /// Color buffer state manager
        /// </summary>
        public sealed class ColorBufferState
        {
            private LinearColor _clearValue;
            private readonly OpenGLState _state;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="ColorBufferState"/> class
            /// </summary>
            /// <param name="state">Parent state manager</param>
            public ColorBufferState(OpenGLState state)
            {
                _state = state;

                float[] values = new float[4];
                OGL.GL.GetFloat(OGL.GetPName.ColorClearValue, values);
                _clearValue = new LinearColor(values[0], values[1], values[2], values[3]);
            }

            /// <summary>
            /// Gets or sets the clear color value
            /// </summary>
            public LinearColor ClearValue
            {
                get
                {
                    return _clearValue;
                }
                set
                {
                    if (value == _clearValue)
                    {
                        return;
                    }

                    OGL.GL.ClearColor(value.R, value.G, value.B, value.A);
                    _clearValue = value;
                }
            }
        }

        /// <summary>
        /// Depth buffer state manager
        /// </summary>
        public sealed class DepthBufferState
        {
            private float _clearValue;
            private readonly OpenGLState _state;

            /// <summary>
            /// Initializes a new instance of the <see cref="DepthBufferState"/> class
            /// </summary>
            /// <param name="state">Parent state manager</param>
            public DepthBufferState(OpenGLState state)
            {
                _state = state;

                _clearValue = OGL.GL.GetFloat(OGL.GetPName.DepthClearValue);
            }

            /// <summary>
            /// Gets or sets the depth buffer value
            /// </summary>
            public float ClearValue
            {
                get
                {
                    return _clearValue;
                }
                set
                {
                    if (_clearValue == value)
                    {
                        return;
                    }

                    OGL.GL.ClearDepth(value);
                    _clearValue = value;
                }
            }
        }

        /// <summary>
        /// Stencil buffer state manager
        /// </summary>
        public sealed class StencilBufferState
        {
            private int _clearValue;
            private readonly OpenGLState _state;

            /// <summary>
            /// Initializes a new instance of the <see cref="StencilBufferState"/> class
            /// </summary>
            /// <param name="state">Parent state manager</param>
            public StencilBufferState(OpenGLState state)
            {
                _state = state;

                _clearValue = OGL.GL.GetInteger(OGL.GetPName.StencilClearValue);
            }

            /// <summary>
            /// Gets or sets the stencil buffer value
            /// </summary>
            public int ClearValue
            {
                get
                {
                    return _clearValue;
                }
                set
                {
                    if (_clearValue == value)
                    {
                        return;
                    }

                    OGL.GL.ClearStencil(value);
                    _clearValue = value;
                }
            }
        }
    }
}
