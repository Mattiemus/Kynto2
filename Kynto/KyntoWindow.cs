namespace Kynto
{
    using System;
    using System.Linq;

    using Spark.Core;
    using Spark.Content;
    using Spark.Graphics;
    using Spark.Math;

    using Spark.Effects.Importer;

    using OTK = OpenTK;

    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoWindow : OTK.GameWindow
    {
        private readonly IRenderSystem _renderer;
        private readonly ContentManager _contentManager;
        private Effect _shader;
        private VertexBuffer _vertexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoWindow"/> class.
        /// </summary>
        public KyntoWindow()
            : base(1024, 768, OTK.Graphics.GraphicsMode.Default, "Kynto", OTK.GameWindowFlags.Default, OTK.DisplayDevice.Default, 4, 5, OTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            Engine.Initialize(Platforms.GeneralCrossPlatformInitializer);
            
            _renderer = Engine.Instance.Services.GetService<IRenderSystem>();

            _contentManager = new ContentManager(Engine.Instance.Services);
            _contentManager.ResourceImporters.Add(new EffectImporter());
        }

        /// <summary>
        /// Called when the windows is loaded and our GL context is created
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            _shader = _contentManager.Load<Effect>("Content/BasicEffect.effect");
            _shader.CurrentShaderGroup = _shader.ShaderGroups.First();
            
            _vertexBuffer = new VertexBuffer(
                _renderer,
                new VertexLayout(new [] {
                    new VertexElement(VertexSemantic.Position, 0, VertexFormat.Float3, 0)
                }), 
                new DataBuffer<Vector3>(new []
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f)
                }));

            base.OnLoad(e);
        }

        /// <summary>
        /// Called when a frame is rendered
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnRenderFrame(OTK.FrameEventArgs e)
        {
            IRenderContext context = _renderer.ImmediateContext;

            context.Clear(Color.Indigo);

            _shader.CurrentShaderGroup.Apply(context);

            Matrix4x4.FromScale(0.5f, out Matrix4x4 scaleMatrix);
            _shader.Parameters["mvp"].SetValue(scaleMatrix);
            
            context.SetVertexBuffer(_vertexBuffer);
            context.Draw(PrimitiveType.TriangleList, 0, 3);
                        
            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}
