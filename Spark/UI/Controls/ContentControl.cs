namespace Spark.UI.Controls
{
    public class ContentControl : Control
    {
        public static readonly DependencyProperty ContentProperty 
            = DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentControl),
                new PropertyMetadata(
                    (s, e) => 
                    {
                        ContentControl cc = s as ContentControl;
                        if (cc == null)
                        {
                            return;
                        }

                        UIElement newValue = e.NewValue as UIElement;
                        if (newValue != null)
                        {
                            newValue.Parent = cc;
                        }

                        FrameworkElement ctrl = e.NewValue as FrameworkElement;
                        if (ctrl != null)
                        {
                            cc._contentControl = ctrl;
                        }
                        else
                        {
                            cc._contentControl = null;
                        } 
                        
                        cc.OnContentChanged(e.OldValue, e.NewValue);
                    }));

        public static readonly DependencyProperty HorizontalContentAlignmentProperty 
            = DependencyProperty.Register(
                nameof(HorizontalContentAlignment),
                typeof(HorizontalAlignment),
                typeof(ContentControl),
                new PropertyMetadata(HorizontalAlignment.Center));

        public static readonly DependencyProperty VerticalContentAlignmentProperty 
            = DependencyProperty.Register(
                nameof(VerticalContentAlignment),
                typeof(VerticalAlignment),
                typeof(ContentControl),
                new PropertyMetadata(VerticalAlignment.Center));
        
        private FrameworkElement _contentControl;

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public bool HasContent
        {
            get
            {
                return Content != null;
            }
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public override void Initialize()
        {
            if (_contentControl != null && !_contentControl.IsInitialized)
            {
                _contentControl.Initialize();
            }

            base.Initialize();
        }

        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
        }

        protected override float GetAbsoluteLeft(UIElement child)
        {
            if (child == Content)
            {
                float absLeft = GetAbsoluteLeft();
                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Left:
                        return absLeft + Padding.Left - Padding.Right;
                    case HorizontalAlignment.Center:
                        return absLeft + ActualWidth / 2.0f - child.ActualWidth / 2.0f;
                    case HorizontalAlignment.Right:
                        return absLeft + ActualWidth - child.ActualWidth - Padding.Right + Padding.Left;
                }
            }

            return base.GetAbsoluteLeft(child);
        }

        protected override float GetAbsoluteTop(UIElement child)
        {
            if (child == Content)
            {
                float absTop = GetAbsoluteTop();
                switch (VerticalContentAlignment)
                {
                    case VerticalAlignment.Top:
                        return absTop + Padding.Top - Padding.Bottom;
                    case VerticalAlignment.Center:
                        return absTop + ActualHeight / 2.0f - child.ActualHeight / 2.0f;
                    case VerticalAlignment.Bottom:
                        return absTop + ActualHeight - child.ActualHeight + Padding.Top - Padding.Bottom;
                }
            }

            return base.GetAbsoluteTop(child);
        }

        public override void Draw(DrawingContext drawingContext)
        {
            base.Draw(drawingContext);

            if (Content == null || !IsVisible)
            {
                return;
            }

            if (_contentControl != null)
            {
                _contentControl.Draw(drawingContext);
            }

            // TODO: Draw content as string as fallback
        }
    }
}
