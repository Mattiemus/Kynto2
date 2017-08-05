namespace Kynto
{
    using System;

    using Spark.Core;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    using Spark.Math;

    using OTK = OpenTK;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoWindow : OTK.GameWindow
    {
        private IRenderSystem renderer;
        private OpenGLShaderProgram program;
        private VertexBuffer vertexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoWindow"/> class.
        /// </summary>
        public KyntoWindow()
            : base(1024, 768, OTK.Graphics.GraphicsMode.Default, "Kynto", OTK.GameWindowFlags.Default, OTK.DisplayDevice.Default, 4, 5, OTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            Engine.Initialize(Platforms.WindowsOpenGLPlatform);
            
            renderer = Engine.Instance.Services.GetService<IRenderSystem>();
        }

        /// <summary>
        /// Called when the windows is loaded and our GL context is created
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            program = new OpenGLShaderProgram(new [] 
            {
                new OpenGLShader(OGL.ShaderType.VertexShader,
                    @"
                        #version 330 core
                        layout(location = 0) in vec3 vertexPosition_modelspace;
                        void main() {
                            gl_Position.xyz = vertexPosition_modelspace;
                            gl_Position.w = 1.0;
                        }
                    "),
                new OpenGLShader(OGL.ShaderType.FragmentShader,
                    @"
                        #version 330 core
                        out vec3 color;
                        void main() {
                          color = vec3(1, 0, 0);
                        }
                    ")
            });

            vertexBuffer = new VertexBuffer(
                new VertexLayout(new VertexElement[] {
                    new VertexElement(VertexFormat.Float3, 0)
                }), 
                new DataBuffer<float>(new[]
                {
                    -1.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f,
                    -1.0f,  1.0f, 0.0f
                }));

            base.OnLoad(e);
        }

        /// <summary>
        /// Called when a frame is rendered
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnRenderFrame(OTK.FrameEventArgs e)
        {
            renderer.Clear(LinearColor.Blue);

            OGL.GL.UseProgram(program.ResourceId);

            renderer.SetVertexBuffer(vertexBuffer);
            renderer.Draw(PrimitiveType.Triangles, 0, 3);
                        
            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}
