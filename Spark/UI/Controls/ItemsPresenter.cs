namespace Spark.UI.Controls
{
    using System;

    using Media;
    using Math;

    public class ItemsPresenter : FrameworkElement
    {
        private Panel _child;

        public Panel Child
        {
            get => _child;
            set
            {
                if (_child != value)
                {
                    if (_child != null)
                    {
                        RemoveVisualChild(_child);
                    }

                    _child = value;

                    if (_child != null)
                    {
                        AddVisualChild(_child);
                    }
                }
            }
        }

        protected internal override int VisualChildrenCount => (_child != null) ? 1 : 0;

        protected internal override Visual GetVisualChild(int index)
        {
            if (index > 0 || _child == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (VisualChildrenCount > 0)
            {
                UIElement ui = GetVisualChild(0) as UIElement;

                if (ui != null)
                {
                    ui.Measure(constraint);
                    return ui.DesiredSize;
                }
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (VisualChildrenCount > 0)
            {
                UIElement ui = GetVisualChild(0) as UIElement;

                if (ui != null)
                {
                    ui.Arrange(new RectangleF(Vector2.Zero, finalSize));
                    return finalSize;
                }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
