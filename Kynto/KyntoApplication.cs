namespace Kynto
{
    using System.Linq;

    using Spark;
    using Spark.Application;
    using Spark.Content;
    using Spark.Content.Importers;
    using Spark.Graphics;
    using Spark.Math;
    using Spark.Engine;
    using Spark.Direct3D11.Graphics;
    using Spark.Toolkit.Input;
    using Spark.UI.Controls;
        
    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private InterfaceHost _host;
        private ForwardRenderer _forwardRenderer;
        private World _world;
        private Texture2D _pixel;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoApplication"/> class.
        /// </summary>
        public KyntoApplication()
            : base(Platforms.Windows)
        {
        }

        protected override void OnInitialize(SparkEngine engine)
        {
            _forwardRenderer = new ForwardRenderer(RenderSystem.ImmediateContext);

            GameWindow.Title = "Kynto";
            
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
            content.ResourceImporters.Add(new Effects11ResourceImporter());
            content.ResourceImporters.Add(new BitmapTextureImporter());
            content.ResourceImporters.Add(new BitmapFontImporter());

            _spriteBatch = new SpriteBatch(RenderSystem);

            _pixel = Content.Load<Texture2D>("Content/pixel.png");
            _font = Content.Load<SpriteFont>("Content/font.fnt");

            _world = new World();

            var floorEntity = new Entity();
            floorEntity.AddComponent(new BoxComponent
            {
                MaterialDefinition = CreateMaterial(Color.Green),
                Scale = new Vector3(1000, 10, 1000)
            });
            _world.Add(floorEntity);
            
            var charEntity = new Entity();
            charEntity.AddComponent(new BoxComponent
            {
                MaterialDefinition = CreateMaterial(Color.Orange),
                Scale = new Vector3(50, 100, 50),
                Translation = new Vector3(0, 110, 0)
            });
            _world.Add(charEntity);
            


            




            
            var camEntity = new Entity();
            camEntity.AddComponent(new CameraComponent
            {
                Translation = new Vector3(2000, 2000, 2000)
            });
            
            RenderSystem.ImmediateContext.Camera = camEntity.GetComponent<CameraComponent>().Camera;
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000000);
            
            camEntity.AddComponent(new OrbitCameraController(RenderSystem.ImmediateContext.Camera, Vector3.Zero));
            _world.Add(camEntity);

            _host = new InterfaceHost();

            base.LoadContent(content);
        }

        protected override void Update(IGameTime time)
        {
            _host.Update(time);
            _world.Update(time);
        }

        protected override void Render(IRenderContext context, IGameTime time)
        {
            context.Clear(Color.Black);
            _world.ProcessVisibleSet(_forwardRenderer);
            _forwardRenderer.Render();
            _host.ProcessVisibleSet(context);

            _spriteBatch.Begin(context);
            _spriteBatch.DrawString(_font, "Hello, world!", Vector2.Zero, Color.Green);
            _spriteBatch.End();
        }

        private MaterialDefinition CreateMaterial(Color color)
        {
            var effect = Content.Load<Effect>("Content/BasicEffect.fx");
            effect = effect.Clone();

            var material = new Material(effect);
            material.Passes.Add("Pass0", effect.ShaderGroups.First());
            material.SetParameterBinding("WVP", MaterialBinding.WorldViewProjectionMatrix);
            material.SetParameter("MatDiffuse", color.ToVector3());
            material.SetParameter("DiffuseMap", (IShaderResource)_pixel);
            
            return new MaterialDefinition
            {
                { RenderBucketId.Opaque, material }
            };
        }
    }
}
