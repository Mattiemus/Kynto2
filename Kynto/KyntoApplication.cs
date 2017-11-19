namespace Kynto
{
    using System.Linq;

    using Spark;
    using Spark.Application;
    using Spark.Content;
    using Spark.Content.Importers;
    using Spark.Graphics;
    using Spark.Effects.Importer;
    using Spark.Math;
    using Spark.Input;
    using Spark.Scene;

    using Input;
        
    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private OrbitCameraController _orbitCamera;
        private ForwardRenderer _forwardRenderer;
        private Node _sceneRoot;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoApplication"/> class.
        /// </summary>
        public KyntoApplication()
            : base(Platforms.WindowsOpenGLBasicInputNoSound)
        {
        }

        protected override void OnInitialize(Engine engine)
        {
            _forwardRenderer = new ForwardRenderer(RenderSystem.ImmediateContext);

            RenderSystem.ImmediateContext.Camera = new Camera();
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000000);
            RenderSystem.ImmediateContext.Camera.Position = new Vector3(0, 0, 1000);
            RenderSystem.ImmediateContext.Camera.LookAt(Vector3.Zero, Vector3.Up);

            _orbitCamera = new OrbitCameraController(RenderSystem.ImmediateContext.Camera, Vector3.Zero);
            _orbitCamera.MapControls(null);

            Mouse.WindowHandle = GameWindow.Handle;

            base.OnInitialize(engine);
        }

        protected override void OnViewportResized(IWindow gameWindow)
        {
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 1000000);

            base.OnViewportResized(gameWindow);
        }

        protected override void LoadContent(ContentManager content)
        {
            content.ResourceImporters.Add(new EffectImporter());
            content.ResourceImporters.Add(new BitmapTextureImporter());
            
            _sceneRoot = new Node("Root");

            int dimension = 4;
            for (int x = -dimension; x <= dimension; x++)
            {
                for (int y = -dimension; y <= dimension; y++)
                {
                    for (int z = -dimension; z <= dimension; z++)
                    {
                        var mesh = CreateBoxMesh();
                        mesh.Translation = new Vector3(x * 500.0f, y * 500.0f, z * 500.0f);
                        
                        _sceneRoot.Children.Add(mesh);
                    }
                }
            }
            
            base.LoadContent(content);
        }

        protected override void Update(IGameTime time)
        {
            _orbitCamera.Update(time, true);
        }

        protected override void Render(IRenderContext context, IGameTime time)
        {
            context.Clear(Color.Indigo);

            context.Camera.Update();
            _sceneRoot.Update(time);

            _sceneRoot.ProcessVisibleSet(_forwardRenderer);

            _forwardRenderer.Render();
        }

        private Mesh CreateBoxMesh()
        {
            var effect = Content.Load<Effect>("Content/BasicEffect.effect");

            var material = new Material(effect);
            material.Passes.Add("Pass0", effect.ShaderGroups.First());
            material.SetParameterBinding("mvp", MaterialBinding.WorldViewProjectionMatrix);

            var meshData = new MeshData();
            var boxGen = new BoxGenerator(new Vector3(100, 100, 100));
            boxGen.BuildMeshData(meshData, GenerateOptions.Positions);
            meshData.Compile();

            var mesh = new Mesh("Mesh_{x}_{y}_{z}", meshData);
            mesh.MakeNonInstanced(new MaterialDefinition
            {
                { RenderBucketId.Opaque, material }
            }, false);

            return mesh;
        }
    }
}
