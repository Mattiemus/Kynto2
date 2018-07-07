namespace Spark.UI.Shapes
{
    using Math;

    public class RectangleShape : Shape
    {
        public static readonly DependencyProperty RadiusXProperty 
            = DependencyProperty.Register(
                nameof(RadiusX),
                typeof(float),
                typeof(RectangleShape));

        public static readonly DependencyProperty RadiusYProperty 
            = DependencyProperty.Register(
                nameof(RadiusY),
                typeof(float),
                typeof(RectangleShape));

        public float RadiusX
        {
            get { return (float)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public float RadiusY
        {
            get { return (float)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public override void Draw(DrawingContext drawingContext)
        {
            if (MathHelper.IsApproxZero(RadiusX) && MathHelper.IsApproxZero(RadiusY))
            {
                drawingContext.DrawRectangle(
                    Fill, 
                    new Pen(Stroke, StrokeThickness), 
                    Bounds);
            }
            else
            {
                drawingContext.DrawRoundedRectangle(
                    Fill, 
                    new Pen(Stroke, StrokeThickness), 
                    Bounds, 
                    RadiusX,
                    RadiusY);
            }
        }
    }
}
