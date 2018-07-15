namespace Spark.UI.Controls
{
    public class ColumnDefinition : DefinitionBase
    {
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(float),
                typeof(ColumnDefinition),
                new PropertyMetadata(float.PositiveInfinity));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth),
                typeof(float),
                typeof(ColumnDefinition),
                new PropertyMetadata(0.0f));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(GridLength),
                typeof(ColumnDefinition),
                new PropertyMetadata(new GridLength(1.0f, GridUnitType.Star)));

        public float ActualWidth { get; internal set; }

        public float MaxWidth
        {
            get => (float)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        public float MinWidth
        {
            get => (float)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        public GridLength Width
        {
            get => (GridLength)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }
    }
}
