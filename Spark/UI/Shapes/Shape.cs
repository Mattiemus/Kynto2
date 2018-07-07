namespace Spark.UI.Shapes
{
    public abstract class Shape : FrameworkElement
    {
        public static readonly DependencyProperty FillProperty 
            = DependencyProperty.Register(
                nameof(Fill),
                typeof(Brush),
                typeof(Shape));

        public static readonly DependencyProperty StrokeProperty 
            = DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(Shape));

        public static readonly DependencyProperty StrokeThicknessProperty 
            = DependencyProperty.Register(
                nameof(StrokeThickness),
                typeof(float),
                typeof(Shape));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public float StrokeThickness
        {
            get { return (float)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}
