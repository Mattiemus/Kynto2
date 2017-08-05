namespace Kynto
{
    using System;

    using Spark.Core;
    using Spark.Graphics;
    using Spark.Graphics.Implementation;
    using Spark.Graphics.Renderer;
    using Spark.Math;

    using OTK = OpenTK;
    using OGL = OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoWindow : OTK.GameWindow
    {
        private OpenGLRenderSystem renderer;
        private OpenGLShaderProgram program;
        private OpenGLVertexBuffer vertexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoWindow"/> class.
        /// </summary>
        public KyntoWindow()
            : base(1024, 768, OTK.Graphics.GraphicsMode.Default, "Kynto", OTK.GameWindowFlags.Default, OTK.DisplayDevice.Default, 4, 5, OTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            Engine.Initialize();
            
            renderer = new OpenGLRenderSystem();
            Engine.Instance.Services.AddService<IRenderSystem>(renderer);
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

            VertexBuffer vb = new VertexBuffer(new VertexLayout(new VertexElement[] {
                new VertexElement(VertexFormat.Float3, 0)
            }));
            
            vertexBuffer = new OpenGLVertexBuffer(new VertexLayout(new VertexElement[] {
                new VertexElement(VertexFormat.Float3, 0)
            }));
            
            vertexBuffer.SetData(new DataBuffer<float>(new[]
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
            
            OGL.GL.EnableVertexAttribArray(0);

            OGL.GL.BindBuffer(vertexBuffer.BufferType, vertexBuffer.ResourceId);

            OGL.GL.VertexAttribPointer(0, 3, OGL.VertexAttribPointerType.Float, false, 0, 0);

            renderer.Draw(PrimitiveType.Triangles, 0, 3);
                        
            OGL.GL.DisableVertexAttribArray(0);

            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}
