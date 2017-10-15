namespace Kynto
{
    using System.Linq;

    using Spark.Core;
    using Spark.Application;
    using Spark.Content;
    using Spark.Graphics;
    using Spark.Graphics.Materials;
    using Spark.Graphics.Geometry;
    using Spark.Graphics.Renderer;
    using Spark.Graphics.Renderer.Forward;
    using Spark.Math;

    using Spark.Effects.Importer;
        
    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private ForwardRenderer _forwardRenderer;
        private BoxMesh _mesh;

        /// <summary>
        /// Initializes a new instance of the <see cref="KyntoApplication"/> class.
        /// </summary>
        public KyntoApplication()
            : base(Platforms.OpenGLPlatformInitializer)
        {
        }

        protected override void OnInitialize(Engine engine)
        {
            _forwardRenderer = new ForwardRenderer(RenderSystem.ImmediateContext);

            RenderSystem.ImmediateContext.Camera = new Camera();
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000);
            RenderSystem.ImmediateContext.Camera.Position = new Vector3(0, 10, 15);
            RenderSystem.ImmediateContext.Camera.LookAt(Vector3.Zero, Vector3.Up);

            base.OnInitialize(engine);
        }

        protected override void OnViewportResized(IWindow gameWindow)
        {
            RenderSystem.ImmediateContext.Camera.Viewport = new Viewport(0, 0, GameWindow.ClientBounds.Width, GameWindow.ClientBounds.Height);
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000);

            base.OnViewportResized(gameWindow);
        }

        protected override void LoadContent(ContentManager content)
        {
            content.ResourceImporters.Add(new EffectImporter());

            _mesh = new BoxMesh(content);

            base.LoadContent(content);
        }

        protected override void Update(IGameTime time)
        {
        }

        protected override void Render(IRenderContext context, IGameTime time)
        {
            context.Clear(Color.Indigo);

            context.Camera.Update();

            _forwardRenderer.Process(_mesh);
            _forwardRenderer.Render();
        }
    }

    public sealed class BoxMesh : IRenderable
    {
        private readonly MeshData _meshData;

        public BoxMesh(ContentManager contentManager)
        {
            MaterialDefinition = new MaterialDefinition();
            WorldTransform = new Transform();
            RenderProperties = new RenderPropertyCollection();

            Effect effect = contentManager.Load<Effect>("Content/BasicEffect.effect");

            Material material = new Material(effect);
            material.Passes.Add("Pass0", effect.ShaderGroups.First());
            material.SetParameterBinding("mvp", MaterialBinding.ViewProjectionMatrix);

            MaterialDefinition.Add(RenderBucketId.Ortho, material);

            _meshData = new MeshData();

            BoxGenerator boxGen = new BoxGenerator(Vector3.One);
            boxGen.BuildMeshData(_meshData, GenerateOptions.Positions);

            _meshData.Compile();
        }

        public MaterialDefinition MaterialDefinition { get; }

        public Transform WorldTransform { get; }

        public RenderPropertyCollection RenderProperties { get; }

        public bool IsValidForDraw => true;

        public void SetupDrawCall(IRenderContext renderContext, RenderBucketId currentBucketId, MaterialPass currentPass)
        {
            renderContext.SetVertexBuffer(_meshData.VertexBuffer);

            if (_meshData.UseIndexedPrimitives)
            {
                renderContext.SetIndexBuffer(_meshData.IndexBuffer);
                renderContext.DrawIndexed(_meshData.PrimitiveType, _meshData.IndexCount, 0, 0);
            }
            else
            {
                renderContext.Draw(_meshData.PrimitiveType, _meshData.VertexCount, 0);
            }
        }
    }
}
