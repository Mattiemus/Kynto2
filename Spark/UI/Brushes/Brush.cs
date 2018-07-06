namespace Spark.UI
{
    using System;

    using Graphics;
    using Math;

    public abstract class Brush
    {
        protected Brush()
            : this(SparkEngine.Instance.Services.GetService<IRenderSystem>())
        {
        }

        protected Brush(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            RenderSystem = renderSystem;

            Opacity = 1.0f;
        }

        public float Opacity { get; set; }

        protected IRenderSystem RenderSystem { get; }

        public abstract void Draw(IRenderContext context, SpriteBatch batch, Rectangle bounds, Matrix4x4 transform, float alpha);
    }
}
