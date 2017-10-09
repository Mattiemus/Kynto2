namespace Kynto
{
    using System;
    using System.Linq;

    using Spark.Core;
    using Spark.Content;
    using Spark.Graphics;
    using Spark.Graphics.Geometry;
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
        private double _totalTime;
        private Camera _cam;
        private Effect _shader;
        private MeshData _meshData;

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
            _cam = new Camera();
            _cam.Viewport = new Viewport(0, 0, Width, Height);
            _cam.SetProjection(45, 1, 10000);            
            _cam.Position = new Vector3(0, 15, 15);
            _cam.LookAt(Vector3.Zero, Vector3.Up);

            _shader = _contentManager.Load<Effect>("Content/BasicEffect.effect");
            _shader.CurrentShaderGroup = _shader.ShaderGroups.First();

            _meshData = new MeshData();
            BoxGenerator boxGen = new BoxGenerator(Vector3.One);
            boxGen.BuildMeshData(_meshData, GenerateOptions.Positions);
            _meshData.Compile();

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

            _cam.Update();
            _shader.Parameters["mvp"].SetValue(Matrix4x4.FromRotationY((Angle)_totalTime) * _cam.ViewProjectionMatrix);

            context.SetIndexBuffer(_meshData.IndexBuffer);
            context.SetVertexBuffer(_meshData.VertexBuffer);
            
            context.DrawIndexed(PrimitiveType.TriangleList, _meshData.IndexCount, 0, 0);
                        
            SwapBuffers();

            _totalTime += e.Time;

            base.OnRenderFrame(e);
        }
    }
}
