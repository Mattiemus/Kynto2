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
    using Spark.Engine;

    using Spark.Toolkit.Input;
        
    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private OrbitCameraController _orbitCamera;
        private ForwardRenderer _forwardRenderer;
        private World _world;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoApplication"/> class.
        /// </summary>
        public KyntoApplication()
            : base(Platforms.WindowsOpenGLBasicInputNoSound)
        {
        }

        protected override void OnInitialize(SparkEngine engine)
        {
            _forwardRenderer = new ForwardRenderer(RenderSystem.ImmediateContext);
            
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

            _world = new World();

            var boxEntity = new Entity();
            boxEntity.AddComponent(new BoxComponent
            {
                MaterialDefinition = CreateMaterial(),
                Scale = new Vector3(100, 100, 100)
            });
            _world.Add(boxEntity);

            var camEntity = new Entity();
            camEntity.AddComponent(new CameraComponent
            {
                Translation = new Vector3(0, 0, 1000)
            });
            _world.Add(camEntity);

            RenderSystem.ImmediateContext.Camera = camEntity.GetComponent<CameraComponent>().Camera;
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000000);

            _orbitCamera = new OrbitCameraController(RenderSystem.ImmediateContext.Camera, Vector3.Zero);
            _orbitCamera.MapControls(null);

            base.LoadContent(content);
        }

        protected override void Update(IGameTime time)
        {
            _orbitCamera.Update(time, true);
            _world.Update(time);
        }

        protected override void Render(IRenderContext context, IGameTime time)
        {
            context.Clear(Color.Indigo);
            _world.ProcessVisibleSet(_forwardRenderer);
            _forwardRenderer.Render();
        }

        private MaterialDefinition CreateMaterial()
        {
            var effect = Content.Load<Effect>("Content/BasicEffect.effect");

            var material = new Material(effect);
            material.Passes.Add("Pass0", effect.ShaderGroups.First());
            material.SetParameterBinding("mvp", MaterialBinding.WorldViewProjectionMatrix);

            return new MaterialDefinition
            {
                { RenderBucketId.Opaque, material }
            };
        }
    }
}
