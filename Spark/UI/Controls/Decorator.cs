namespace Spark.UI.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows.Markup;

    using Media;
    using Math;

    [ContentProperty("Child")]
    public class Decorator : FrameworkElement
    {
        private UIElement _child;

        public virtual UIElement Child
        {
            get => _child;
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                    RemoveLogicalChild(_child);
                }

                _child = value;

                if (_child != null)
                {
                    AddVisualChild(_child);
                    AddLogicalChild(_child);
                }

                InvalidateMeasure();
            }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (_child != null)
                {
                    return Enumerable.Repeat<object>(_child, 1).GetEnumerator();
                }

                return Enumerable.Empty<object>().GetEnumerator();
            }
        }

        protected internal override int VisualChildrenCount => (Child != null) ? 1 : 0;

        protected internal override Visual GetVisualChild(int index)
        {
            if (index > 0 || Child == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_child != null)
            {
                _child.Measure(constraint);
                return _child.DesiredSize;
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child != null)
            {
                _child.Arrange(new RectangleF(Vector2.Zero, finalSize));
                return finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
