namespace Spark.UI
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Markup;

    using Media;
    using Math;

    [RuntimeNameProperty(nameof(Name))]
    public class FrameworkElement : UIElement, ISupportInitialize
    {
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
                nameof(DataContext),
                typeof(object),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty DefaultStyleKeyProperty =
            DependencyProperty.Register(
                nameof(DefaultStyleKey),
                typeof(object),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    float.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    HorizontalAlignment.Stretch,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(
                nameof(Margin),
                typeof(Thickness),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    float.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    float.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    0.0f,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    0.0f,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(
                nameof(Style),
                typeof(Style),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    StyleChanged));

        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalAlignment),
                typeof(VerticalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    VerticalAlignment.Stretch,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(float),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    float.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        internal static readonly DependencyProperty TemplatedParentProperty =
            DependencyProperty.Register(
                nameof(TemplatedParent),
                typeof(DependencyObject),
                typeof(FrameworkElement),
                new PropertyMetadata(TemplatedParentChanged));

        private bool isInitialized;

        public FrameworkElement()
        {
            Resources = new ResourceDictionary();
        }

        public event EventHandler Initialized;

        public float ActualWidth => RenderSize.Width;

        public float ActualHeight => RenderSize.Height;

        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        public float Height
        {
            get => (float)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }

        public bool IsInitialized
        {
            get => isInitialized;
            internal set
            {
                isInitialized = value;

                if (isInitialized)
                {
                    OnInitialized(EventArgs.Empty);
                }
            }
        }

        public Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }

        public float MaxHeight
        {
            get => (float)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        public float MaxWidth
        {
            get => (float)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        public float MinHeight
        {
            get => (float)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        public float MinWidth
        {
            get => (float)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        public string Name
        {
            get;
            set;
        }

        public DependencyObject Parent
        {
            get;
            private set;
        }

        [Ambient]
        public ResourceDictionary Resources
        {
            get;
            set;
        }

        public Style Style
        {
            get => (Style)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        public DependencyObject TemplatedParent
        {
            get { return (DependencyObject)GetValue(TemplatedParentProperty); }
            internal set => SetValue(TemplatedParentProperty, value);
        }

        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }

        public float Width
        {
            get => (float)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        protected internal object DefaultStyleKey
        {
            get => GetValue(DefaultStyleKeyProperty);
            set => SetValue(DefaultStyleKeyProperty, value);
        }

        protected internal virtual IEnumerator LogicalChildren => new object[0].GetEnumerator();

        public virtual bool ApplyTemplate()
        {
            // NOTE: this isn't virtual in WPF, but the Template property isn't defined until 
            // Control so I don't see how it is applied at this level. Making it virtual makes 
            // the most sense for now.
            return false;
        }

        public object FindName(string name)
        {
            INameScope nameScope = FindNameScope(this);
            return (nameScope != null) ? nameScope.FindName(name) : null;
        }

        public object FindResource(object resourceKey)
        {
            object resource = TryFindResource(resourceKey);

            if (resource != null)
            {
                return resource;
            }

            throw new ResourceReferenceKeyNotFoundException($"'{resourceKey}' resource not found",  resourceKey);
        }

        public virtual void OnApplyTemplate()
        {
        }

        public object TryFindResource(object resourceKey)
        {
            FrameworkElement element = this;
            object resource = null;

            while (resource == null && element != null)
            {
                resource = element.Resources[resourceKey];
                element = (FrameworkElement)VisualTreeHelper.GetParent(element);
            }

            if (resource == null)
            {
                resource = Themes.GenericTheme[resourceKey];
            }

            return resource;
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            IsInitialized = true;
        }

        protected internal void AddLogicalChild(object child)
        {
            FrameworkElement fe = child as FrameworkElement;
            if (fe == null)
            {
                return;
            }

            if (fe.Parent != null)
            {
                throw new InvalidOperationException("FrameworkElement already has a parent.");
            }

            fe.Parent = this;

            if (TemplatedParent != null)
            {
                PropagateTemplatedParent(fe, TemplatedParent);
            }

            InvalidateMeasure();
        }

        protected internal void RemoveLogicalChild(object child)
        {
            FrameworkElement fe = child as FrameworkElement;
            if (fe == null)
            { 
                return;
            }

            if (fe.Parent != this)
            {
                throw new InvalidOperationException("FrameworkElement is not a child of this object.");
            }

            fe.Parent = null;
            InvalidateMeasure();
        }

        protected internal virtual DependencyObject GetTemplateChild(string childName)
        {
            return null;
        }

        protected internal virtual void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            if (oldStyle != null)
            {
                oldStyle.Detach(this);
            }

            if (newStyle != null)
            {
                newStyle.Attach(this);
            }
        }

        protected internal override void OnVisualParentChanged(DependencyObject oldParent)
        {
            if (VisualParent != null)
            {
                IsInitialized = true;
            }
        }

        protected sealed override Size MeasureCore(Size availableSize)
        {
            ApplyTemplate();

            availableSize = new Size(
                Math.Max(0, float.IsNaN(Width) ? availableSize.Width : Width),
                Math.Max(0, float.IsNaN(Height) ? availableSize.Height : Height));

            Size size = MeasureOverride(availableSize);

            size = new Size(
                Math.Min(availableSize.Width, float.IsNaN(Width) ? size.Width + Margin.Left + Margin.Right : Width),
                Math.Min(availableSize.Height, float.IsNaN(Height) ? size.Height + Margin.Top + Margin.Bottom : Height));

            return size;
        }

        protected virtual Size MeasureOverride(Size constraint)
        {
            return new Size();
        }

        protected sealed override void ArrangeCore(RectangleF finalRect)
        {
            Vector2 origin = new Vector2(
                finalRect.Left + Margin.Left,
                finalRect.Top + Margin.Top);

            Size size = new Size(
                Math.Max(0, (float.IsNaN(Width) ? finalRect.Width : Width) - Margin.Left - Margin.Right),
                Math.Max(0, (float.IsNaN(Width) ? finalRect.Height : Height) - Margin.Top - Margin.Bottom));

            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                size = new Size(Math.Min(size.Width, DesiredSize.Width), size.Height);
            }

            if (VerticalAlignment != VerticalAlignment.Stretch)
            {
                size = new Size(size.Width, Math.Min(size.Height, DesiredSize.Height));
            }

            Size taken = ArrangeOverride(size);
            
            size = new Size(
                Math.Min(taken.Width, size.Width),
                Math.Min(taken.Height, size.Height));

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    origin.X += ((finalRect.Width - size.Width) / 2.0f) - Margin.Left;
                    break;
                case HorizontalAlignment.Right:
                    origin.X += (finalRect.Width - size.Width) - Margin.Left;
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    origin.Y += ((finalRect.Height - size.Height) / 2.0f) - Margin.Top;
                    break;
                case VerticalAlignment.Bottom:
                    origin.Y += (finalRect.Height - size.Height) - Margin.Top;
                    break;
            }
            
            base.ArrangeCore(new RectangleF(origin, size));
        }

        protected virtual Size ArrangeOverride(Size finalSize)
        {
            return new Size(
                float.IsNaN(Width) ? finalSize.Width : Width,
                float.IsNaN(Height) ? finalSize.Height : Height);
        }

        protected virtual void OnInitialized(EventArgs e)
        {
            Initialized?.Invoke(this, e);
        }

        private static void StyleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)sender).OnStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        private static void TemplatedParentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.PropagateTemplatedParent(element, element.TemplatedParent);
        }

        private INameScope FindNameScope(FrameworkElement e)
        {
            while (e != null)
            {
                INameScope nameScope = e as INameScope ?? NameScope.GetNameScope(e);

                if (nameScope != null)
                {
                    return nameScope;
                }

                e = LogicalTreeHelper.GetParent(e) as FrameworkElement;
            }

            return null;
        }

        private void PropagateTemplatedParent(FrameworkElement element, DependencyObject templatedParent)
        {
            element.TemplatedParent = templatedParent;

            foreach (FrameworkElement child in LogicalTreeHelper.GetChildren(element).OfType<FrameworkElement>())
            {
                child.TemplatedParent = templatedParent;
            }
        }
    }
}
