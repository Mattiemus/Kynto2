namespace Spark.UI.Media
{
    using Math;
    using Graphics;

    public abstract class Visual : DependencyObject
    {
        protected virtual void Draw(IRenderContext renderContext, SpriteBatch spriteBatch, float alpha, Matrix4x4 transform)
        {
            // No-op
        }
    }
}
