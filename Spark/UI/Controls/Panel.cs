namespace Spark.UI.Controls
{
    public abstract class Panel : Control
    {
        protected Panel()
        {
            Children = new UIElementCollection(this);
        }

        public UIElementCollection Children { get; }

        public override void Initialize()
        {
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Initialize();
                }
            }

            base.Initialize();
        }

        public override void Draw(DrawingContext drawingContext)
        {
            base.Draw(drawingContext);

            if (IsVisible)
            {
                foreach (UIElement child in Children)
                {
                    child.Draw(drawingContext);
                }
            }
        }
    }
}
