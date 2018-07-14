namespace Kynto
{
    using System;
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
    using Spark.UI.Media;
    using Spark.UI.Controls;
    using Spark.UI.Shapes;

    /// <summary>
    /// Main application window
    /// </summary>
    public sealed class KyntoApplication : SparkApplication
    {
        private ForwardRenderer _forwardRenderer;
        private World _world;
        private Texture2D _pixel;

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
                        
            _pixel = Content.Load<Texture2D>("Content/pixel.png");

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

            var stack = new StackPanel();
            stack.Children.Add(new Button
            {
                Width = 128,
                Height = 128,
                Opacity = 1.0f,
                Margin = new Thickness(1),
                Content = new Path
                {
                    Stretch = Stretch.Fill,
                    Data = Geometry.Parse("M 0 5 L 5 5 L 5 0 L 10 0 L 10 5 L 15 5 L 15 10 L 10 10 L 10 15 L 5 15 L 5 10 L 0 10 Z"),
                    Fill = new SolidColorBrush(Color.Blue)
                }
            });
            stack.Children.Add(new Button
            {
                Width = 128,
                Height = 128,
                Opacity = 0.7f,
                Margin = new Thickness(1),
                Content = new Path
                {
                    Stretch = Stretch.Fill,
                    Data = Geometry.Parse("M 0 5 L 5 5 L 5 0 L 10 0 L 10 5 L 15 5 L 15 10 L 10 10 L 10 15 L 5 15 L 5 10 L 0 10 Z"),
                    Fill = new SolidColorBrush(Color.Blue)
                }
            });
            stack.Children.Add(new Button
            {
                Width = 128,
                Height = 128,
                Opacity = 0.6f,
                Margin = new Thickness(1),
                Content = new Path
                {
                    Stretch = Stretch.Fill,
                    Data = Geometry.Parse("M 0 5 L 5 5 L 5 0 L 10 0 L 10 5 L 15 5 L 15 10 L 10 10 L 10 15 L 5 15 L 5 10 L 0 10 Z"),
                    Fill = new SolidColorBrush(Color.Blue)
                }
            });



            StreamGeometry sg = new StreamGeometry();
            using (var ctx = sg.Open())
            {
                Vector2 center = new Vector2(0.5f, 0.5f);

                float rU = 0.5f;
                float z = (float)Math.PI / 10.0f;

                float pt1X = center.X + rU;
                float pt1Y = center.Y;

                ctx.BeginFigure(new Vector2(pt1X, pt1Y), true, false);

                int j = 0;
                for (double i = z; i <= Math.PI * 2.0; i += z)
                {
                    float ptX = (float)(((pt1X - center.X) * Math.Cos(i)) + ((pt1Y - center.Y) * -Math.Sin(i)) + center.X);
                    float ptY = (float)(((pt1X - center.X) * Math.Sin(i)) + ((pt1Y - center.Y) * Math.Cos(i)) + center.Y);

                    ctx.LineTo(new Vector2(ptX, ptY), true, false);

                    j += 1;
                }
            }

            stack.Children.Add(new Button
            {
                Width = 128,
                Height = 128,
                Margin = new Thickness(1),
                Content = new Path
                {
                    Stretch = Stretch.Fill,
                    Data = sg,
                    Fill = new SolidColorBrush(Color.Blue)
                }
            });

            InterfaceHost.Content = stack;

            InterfaceHost.DoLayoutPass();

            base.LoadContent(content);
        }
                
        protected override void Update(IGameTime time)
        {
            _world.Update(time);
        }

        protected override void Render(IRenderContext context, IGameTime time)
        {
            context.Clear(Color.Black);
            _world.ProcessVisibleSet(_forwardRenderer);
            _forwardRenderer.Render();
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
