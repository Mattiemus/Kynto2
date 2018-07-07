namespace Spark.UI.Controls
{
    public class StackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty 
            = DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override float MeasureHeight(float availableHeight)
        {
            var height = 0.0f;
            var maxHeight = 0.0f;

            foreach (UIElement child in Children)
            {
                float itemHeight = child.ActualHeight + child.Margin.Bottom + child.Margin.Top;

                if (Orientation == Orientation.Vertical)
                {
                    height += itemHeight;
                }
                else
                {
                    maxHeight = System.Math.Max(itemHeight, maxHeight);
                }
            }

            return (Orientation == Orientation.Vertical) ? height : maxHeight;
        }

        protected override float MeasureWidth(float availableWidth)
        {
            float width = 0.0f;
            float maxWidth = 0.0f;

            foreach (UIElement c in Children)
            {
                float itemWidth = c.ActualWidth + c.Margin.Right + c.Margin.Left;

                if (Orientation == Orientation.Horizontal)
                {
                    width += itemWidth;
                }
                else
                {
                    maxWidth = System.Math.Max(itemWidth, maxWidth);
                }
            }

            return (Orientation == Orientation.Horizontal) ? width : maxWidth;
        }

        protected override float GetAbsoluteLeft(UIElement child)
        {
            float left = child.Parent.GetAbsoluteLeft();
            left += Padding.Left;

            foreach (UIElement panelChild in Children)
            {
                if (panelChild == child)
                {
                    return left + panelChild.Margin.Left;
                }

                if (Orientation == Orientation.Horizontal)
                {
                    left += panelChild.ActualWidth + panelChild.Margin.Right + panelChild.Margin.Left;
                }
            }

            return base.GetAbsoluteLeft(child);
        }

        protected override float GetAbsoluteTop(UIElement child)
        {
            float top = child.Parent.GetAbsoluteTop();
            top += Padding.Top;

            foreach (UIElement panelChild in Children)
            {
                if (panelChild == child)
                {
                    return top + panelChild.Margin.Top;
                }

                if (Orientation == Orientation.Vertical)
                {
                    top += panelChild.ActualHeight + panelChild.Margin.Bottom + panelChild.Margin.Top;
                }
            }

            return base.GetAbsoluteTop(child);
        }
    }
}
