namespace Spark.UI.Controls
{
    using System;

    using Math;

    public class StackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(StackPanel),
                new FrameworkPropertyMetadata(
                    Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size childAvailable = new Size(float.PositiveInfinity, float.PositiveInfinity);
            Size measured = new Size(0, 0);

            if (Orientation == Orientation.Vertical)
            {
                childAvailable.Width = constraint.Width;

                if (!float.IsNaN(Width))
                {
                    childAvailable.Width = Width;
                }

                childAvailable.Width = Math.Min(childAvailable.Width, MaxWidth);
                childAvailable.Width = Math.Max(childAvailable.Width, MinWidth);
            }
            else
            {
                childAvailable.Height = constraint.Height;

                if (!float.IsNaN(Height))
                {
                    childAvailable.Height = Height;
                }

                childAvailable.Height = Math.Min(childAvailable.Height, MaxHeight);
                childAvailable.Height = Math.Max(childAvailable.Height, MinHeight);
            }

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(childAvailable);
                Size size = child.DesiredSize;

                if (Orientation == Orientation.Vertical)
                {
                    measured.Height += size.Height;
                    measured.Width = Math.Max(measured.Width, size.Width);
                }
                else
                {
                    measured.Width += size.Width;
                    measured.Height = Math.Max(measured.Height, size.Height);
                }
            }

            return measured;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size arranged = finalSize;

            if (Orientation == Orientation.Vertical)
            {
                arranged.Height = 0;
            }
            else
            {
                arranged.Width = 0;
            }

            foreach (UIElement child in InternalChildren)
            {
                Size size = child.DesiredSize;

                if (Orientation == Orientation.Vertical)
                {
                    size.Width = finalSize.Width;

                    RectangleF childFinal = new RectangleF(0, arranged.Height, size.Width, size.Height);

                    if (childFinal.IsEmpty)
                    {
                        child.Arrange(new RectangleF());
                    }
                    else
                    {
                        child.Arrange(childFinal);
                    }

                    arranged.Width = Math.Max(arranged.Width, size.Width);
                    arranged.Height += size.Height;
                }
                else
                {
                    size.Height = finalSize.Height;

                    RectangleF childFinal = new RectangleF(arranged.Width, 0, size.Width, size.Height);

                    if (childFinal.IsEmpty)
                    {
                        child.Arrange(new RectangleF());
                    }
                    else
                    {
                        child.Arrange(childFinal);
                    }

                    arranged.Width += size.Width;
                    arranged.Height = Math.Max(arranged.Height, size.Height);
                }
            }

            if (Orientation == Orientation.Vertical)
            {
                arranged.Height = Math.Max(arranged.Height, finalSize.Height);
            }
            else
            {
                arranged.Width = Math.Max(arranged.Width, finalSize.Width);
            }

            return arranged;
        }
    }
}
