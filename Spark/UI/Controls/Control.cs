namespace Spark.UI.Controls
{
    using Math;

    public abstract class Control : FrameworkElement
    {
        public static readonly DependencyProperty BorderBrushProperty 
            = DependencyProperty.Register(
                nameof(BorderBrush),
                typeof(Brush),
                typeof(Control),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundProperty 
            = DependencyProperty.Register(
                nameof(Background),
                typeof(Brush),
                typeof(Control),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderThicknessProperty 
            = DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(Control),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public override void Draw(DrawingContext drawingContext)
        {
            if (!IsVisible)
            {
                return;
            }

            Control parentControl = Parent as Control;
            if (parentControl != null)
            {
                var parentHeight = Parent.ActualHeight;
                var parentWidth = Parent.ActualWidth;
                var parentLeft = Parent.GetAbsoluteLeft();
                var parentTop = Parent.GetAbsoluteTop();

                var parentBounds 
                    = new Rectangle(
                        (int)parentLeft, 
                        (int)parentTop, 
                        (int)parentWidth,
                        (int)parentHeight);

                var bounds 
                    = new Rectangle(
                        (int)GetAbsoluteLeft(),
                        (int)GetAbsoluteTop(),
                        (int)ActualWidth,
                        (int)ActualHeight);

                var newLeft = bounds.Left;
                if (bounds.Left < parentBounds.Left)
                {
                    newLeft = parentBounds.Left;
                }

                var newTop = bounds.Top;
                if (bounds.Top < parentBounds.Top)
                {
                    newTop = parentBounds.Top;
                }

                var newWidth = bounds.Width;
                if ((bounds.Left + bounds.Width) > (parentBounds.Left + parentBounds.Width))
                {
                    newWidth = (int)Parent.ActualWidth;
                }

                var newHeight = bounds.Height;
                if ((bounds.Top + bounds.Height) > (parentBounds.Top + parentBounds.Height))
                {
                    newHeight = (int)Parent.ActualHeight;
                }

                if (newLeft < 0)
                {
                    newLeft = 0;
                }

                if (newTop < 0)
                {
                    newTop = 0;
                }

                if (newWidth < 0)
                {
                    newWidth = 0;
                }

                if (newHeight < 0)
                {
                    newHeight = 0;
                }

                var newBounds =
                    new Rectangle(
                        newLeft,
                        newTop,
                        newWidth,
                        newHeight);

                // TODO: apply clip as scissor rectangle
            }

            drawingContext.DrawRectangle(Background, new Pen(BorderBrush, BorderThickness.Left), Bounds);
        }
    }
}
