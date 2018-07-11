namespace Spark.UI.Media
{
    using System.ComponentModel;

    using Graphics;
    using Animation;

    [TypeConverter(typeof(BrushConverter))]
    public abstract class Brush : Animatable
    {
        private Texture2D _brushTexture;

        internal Texture2D GetTexture(IRenderSystem renderSystem)
        {
            if (_brushTexture == null)
            {
                _brushTexture = CreateTexture(renderSystem);
            }

            return _brushTexture;
        }

        protected abstract Texture2D CreateTexture(IRenderSystem renderSystem);

        protected void InvalidateTexture()
        {
            _brushTexture?.Dispose();
            _brushTexture = null;
        }
    }
}
