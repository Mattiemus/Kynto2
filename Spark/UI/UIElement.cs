namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Media;
    using Input;
    using Math;

    public class UIElement : Visual, IInputElement
    {
        public static readonly DependencyProperty FocusableProperty =
            DependencyProperty.Register(
                nameof(Focusable),
                typeof(bool),
                typeof(UIElement));

        public static readonly DependencyProperty IsKeyboardFocusedProperty =
            DependencyProperty.Register(
                nameof(IsKeyboardFocused),
                typeof(bool),
                typeof(UIElement));

        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register(
                nameof(IsMouseOver),
                typeof(bool),
                typeof(UIElement));

        public static readonly DependencyProperty IsMouseCapturedProperty =
            DependencyProperty.Register(
                nameof(IsMouseCaptured),
                typeof(bool),
                typeof(UIElement));

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity),
                typeof(float),
                typeof(UIElement),
                new PropertyMetadata(1.0f));

        public static readonly DependencyProperty SnapsToDevicePixelsProperty =
            DependencyProperty.Register(
                nameof(SnapsToDevicePixels),
                typeof(bool),
                typeof(UIElement));

        public static readonly RoutedEvent GotKeyboardFocusEvent =
            Keyboard.GotKeyboardFocusEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent LostKeyboardFocusEvent =
            Keyboard.LostKeyboardFocusEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent KeyDownEvent =
            Keyboard.KeyDownEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent MouseEnterEvent =
            Mouse.MouseEnterEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent MouseLeaveEvent =
            Mouse.MouseLeaveEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent MouseLeftButtonDownEvent =
            Mouse.MouseLeftButtonDownEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent MouseLeftButtonUpEvent =
            Mouse.MouseLeftButtonUpEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent MouseMoveEvent =
            Mouse.MouseMoveEvent.AddOwner(typeof(UIElement));

        public static readonly RoutedEvent TextInputEvent =
            TextCompositionManager.TextInputEvent.AddOwner(typeof(UIElement));

        private bool _measureCalled;
        private Size _previousMeasureSize;
        private readonly Dictionary<RoutedEvent, List<Delegate>> _eventHandlers;

        public UIElement()
        {
            _eventHandlers = new Dictionary<RoutedEvent, List<Delegate>>();

            IsMeasureValid = true;
            IsArrangeValid = true;

            AddHandler(GotKeyboardFocusEvent, (KeyboardFocusChangedEventHandler)((s, e) => OnGotKeyboardFocus(e)));
            AddHandler(LostKeyboardFocusEvent, (KeyboardFocusChangedEventHandler)((s, e) => OnLostKeyboardFocus(e)));
            AddHandler(MouseEnterEvent, (MouseEventHandler)((s, e) => OnMouseEnter(e)));
            AddHandler(MouseLeaveEvent, (MouseEventHandler)((s, e) => OnMouseLeave(e)));
            AddHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler)((s, e) => OnMouseLeftButtonDown(e)));
            AddHandler(MouseLeftButtonUpEvent, (MouseButtonEventHandler)((s, e) => OnMouseLeftButtonUp(e)));
            AddHandler(MouseMoveEvent, (MouseEventHandler)((s, e) => OnMouseMove(e)));
            AddHandler(TextInputEvent, (TextCompositionEventHandler)((s, e) => OnTextInput(e)));
        }

        public event KeyEventHandler KeyDown
        {
            add { AddHandler(KeyDownEvent, value); }
            remove { RemoveHandler(KeyDownEvent, value); }
        }

        public event MouseEventHandler MouseEnter
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        public event MouseEventHandler MouseLeave
        {
            add { AddHandler(MouseLeaveEvent, value); }
            remove { RemoveHandler(MouseLeaveEvent, value); }
        }

        public event MouseButtonEventHandler MouseLeftButtonDown
        {
            add { AddHandler(MouseLeftButtonDownEvent, value); }
            remove { RemoveHandler(MouseLeftButtonDownEvent, value); }
        }

        public event MouseButtonEventHandler MouseLeftButtonUp
        {
            add { AddHandler(MouseLeftButtonUpEvent, value); }
            remove { RemoveHandler(MouseLeftButtonUpEvent, value); }
        }

        public event MouseEventHandler MouseMove
        {
            add { AddHandler(MouseMoveEvent, value); }
            remove { RemoveHandler(MouseMoveEvent, value); }
        }

        public Size DesiredSize { get; set; }

        public bool IsMeasureValid { get; private set; }

        public bool IsArrangeValid { get; private set; }

        public bool IsVisible
        {
            get
            {
                // TODO: Implement visibility.
                return true;
            }
        }

        public Size RenderSize { get; private set; }

        public bool Focusable
        {
            get => (bool)GetValue(FocusableProperty);
            set => SetValue(FocusableProperty, value);
        }

        public bool IsKeyboardFocused => (bool)GetValue(IsKeyboardFocusedProperty);

        public bool IsMouseOver => (bool)GetValue(IsMouseOverProperty);

        public bool IsMouseCaptured
        {
            get => (bool)GetValue(IsMouseCapturedProperty);
            private set => SetValue(IsMouseCapturedProperty, value);
        }

        public float Opacity
        {
            get => (float)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        // TODO: Actually implement 
        public bool SnapsToDevicePixels
        {
            get => (bool)GetValue(SnapsToDevicePixelsProperty);
            set => SetValue(SnapsToDevicePixelsProperty, value);
        }

        public void CaptureMouse()
        {
            Mouse.Capture(this);
            IsMouseCaptured = true;
        }

        public void ReleaseMouseCapture()
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
                IsMouseCaptured = false;
            }
        }

        public void Measure(Size availableSize)
        {
            _measureCalled = true;
            _previousMeasureSize = availableSize;
            DesiredSize = MeasureCore(availableSize);
            IsMeasureValid = true;
            IsArrangeValid = false;
        }

        public void Arrange(RectangleF finalRect)
        {
            if (!_measureCalled || !IsMeasureValid)
            {
                Measure(_measureCalled ? _previousMeasureSize : finalRect.Size);
            }

            ArrangeCore(finalRect);
            IsArrangeValid = true;
        }

        public void InvalidateMeasure()
        {
            IsMeasureValid = false;
            LayoutManager.Instance.QueueMeasure(this);
        }

        public void InvalidateArrange()
        {
            IsArrangeValid = false;
            LayoutManager.Instance.QueueArrange(this);
        }

        public void InvalidateVisual()
        {
            InvalidateArrange();
        }

        public void UpdateLayout()
        {
            Size size = RenderSize;
            Measure(size);
            Arrange(new RectangleF(Vector2.Zero, size));
        }

        public IInputElement InputHitTest(Vector2 point)
        {
            RectangleF bounds = new RectangleF(Vector2.Zero, RenderSize);
            if (bounds.Contains(point))
            {
                foreach (UIElement child in VisualTreeHelper.GetChildren(this).OfType<UIElement>())
                {
                    Vector2 offsetPoint = point - child.VisualOffset;
                    IInputElement hit = child.InputHitTest(offsetPoint);

                    if (hit != null)
                    {
                        return hit;
                    }
                }

                return this;
            }

            return null;
        }

        public void AddHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            
            if (!_eventHandlers.TryGetValue(routedEvent, out List<Delegate> delegates))
            {
                delegates = new List<Delegate>();
                _eventHandlers.Add(routedEvent, delegates);
            }

            delegates.Add(handler);
        }

        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException(nameof(routedEvent));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            
            if (_eventHandlers.TryGetValue(routedEvent, out List<Delegate> delegates))
            {
                delegates.Remove(handler);
            }
        }

        public void RaiseEvent(RoutedEventArgs e)
        {
            if (e.RoutedEvent != null)
            {
                switch (e.RoutedEvent.RoutingStrategy)
                {
                    case RoutingStrategy.Bubble:
                        BubbleEvent(e);
                        break;
                    case RoutingStrategy.Direct:
                        RaiseEventImpl(e);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        internal override RectangleF GetHitTestBounds()
        {
            return new RectangleF(VisualOffset, RenderSize);
        }

        protected internal virtual void OnRender(DrawingContext drawingContext)
        {
        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            return new Size();
        }

        protected virtual void ArrangeCore(RectangleF finalRect)
        {
            VisualOffset = finalRect.TopLeftPoint;
            RenderSize = finalRect.Size;
        }

        protected virtual void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
        }

        protected virtual void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
        }

        protected virtual void OnMouseEnter(MouseEventArgs e)
        {
        }

        protected virtual void OnMouseLeave(MouseEventArgs e)
        {
        }

        protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
        }

        protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
        }

        protected virtual void OnTextInput(TextCompositionEventArgs e)
        {
        }

        private void BubbleEvent(RoutedEventArgs e)
        {
            UIElement target = this;
            while (target != null)
            {
                target.RaiseEventImpl(e);
                target = VisualTreeHelper.GetAncestor<UIElement>(target);
            }
        }

        private void RaiseEventImpl(RoutedEventArgs e)
        {
            if (_eventHandlers.TryGetValue(e.RoutedEvent, out List<Delegate> delegates))
            {
                foreach (Delegate handler in delegates)
                {
                    // TODO: Implement the Handled stuff.
                    handler.DynamicInvoke(this, e);
                }
            }
        }
    }
}
