namespace Kynto
{
    using System.Linq;
    using System.Collections.Generic;

    using Spark.Core;
    using Spark.Application;
    using Spark.Content;
    using Spark.Graphics;
    using Spark.Graphics.Materials;
    using Spark.Graphics.Geometry;
    using Spark.Graphics.Renderer;
    using Spark.Graphics.Renderer.Forward;
    using Spark.Effects.Importer;
    using Spark.Math;
    using Spark.Input;

    using Input;
        
    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private OrbitCameraController _orbitCamera;
        private ForwardRenderer _forwardRenderer;
        private List<BoxMesh> _meshes;

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
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 100000);
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
            RenderSystem.ImmediateContext.Camera.SetProjection(45, 1, 10000);

            base.OnViewportResized(gameWindow);
        }

        protected override void LoadContent(ContentManager content)
        {
            content.ResourceImporters.Add(new EffectImporter());

            Effect effect = Content.Load<Effect>("Content/BasicEffect.effect");

            Material material = new Material(effect);
            material.Passes.Add("Pass0", effect.ShaderGroups.First());
            material.SetParameterBinding("mvp", MaterialBinding.WorldViewProjectionMatrix);

            MeshData meshData = new MeshData();

            BoxGenerator boxGen = new BoxGenerator(new Vector3(100, 100, 100));
            boxGen.BuildMeshData(meshData, GenerateOptions.Positions);

            meshData.Compile();

            _meshes = new List<BoxMesh>();

            int dimension = 8;
            for (int x = -dimension; x <= dimension; x++)
            {
                for (int y = -dimension; y <= dimension; y++)
                {
                    for (int z = -dimension; z <= dimension; z++)
                    {
                        BoxMesh mesh = new BoxMesh(meshData, material);
                        mesh.WorldTransform.SetTranslation(x * 500.0f, y * 500.0f, z * 500.0f);

                        _meshes.Add(mesh);
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

            foreach (BoxMesh mesh in _meshes)
            {
                _forwardRenderer.Process(mesh);
            }

            _forwardRenderer.Render();
        }
    }

    public sealed class BoxMesh : IRenderable
    {
        private readonly MeshData _meshData;

        public BoxMesh(MeshData mesh, Material material)
        {
            MaterialDefinition = new MaterialDefinition();
            WorldTransform = new Transform();
            RenderProperties = new RenderPropertyCollection();
            
            _meshData = mesh;

            MaterialDefinition.Add(RenderBucketId.Opaque, material);

            RenderProperties.Add(new WorldTransformProperty(WorldTransform));
            RenderProperties.Add(new OrthoOrderProperty(0));
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
