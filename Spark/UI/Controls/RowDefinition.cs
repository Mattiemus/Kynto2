namespace Spark.UI.Controls
{
    public class RowDefinition : DefinitionBase
    {
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(float),
                typeof(ColumnDefinition),
                new PropertyMetadata(float.PositiveInfinity));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight),
                typeof(float),
                typeof(ColumnDefinition),
                new PropertyMetadata(0.0f));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(GridLength),
                typeof(ColumnDefinition),
                new PropertyMetadata(new GridLength(1.0f, GridUnitType.Star)));

        public float ActualHeight { get; internal set; }

        public float MaxHeight
        {
            get => (float)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        public float MinHeight
        {
            get => (float)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        public GridLength Height
        {
            get => (GridLength)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }
    }
}
