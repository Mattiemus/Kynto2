using Spark.Input;

namespace Spark.UI.Input
{
    using System.Collections.Generic;
    using System.Linq;

    using Media;
    using Math;

    using SparkMouse = Spark.Input.Mouse;
    
    public sealed class MouseDevice : InputDevice
    {
        private readonly List<UIElement> _mouseOvers;
        private MouseState _mouseState;

        public MouseDevice()
        {
            _mouseOvers = new List<UIElement>();
            _mouseState = SparkMouse.GetMouseState();
        }
        
        public IInputElement Captured { get; private set; }

        public IInputElement DirectlyOver => Target;

        public override IInputElement Target
        {
            get
            {
                if (Captured != null)
                {
                    return Captured;
                }

                Vector2 p = GetClientPosition();
                UIElement ui = ActiveSource.RootVisual as UIElement;
                return ui.InputHitTest(p);
            }
        }

        public void Capture(IInputElement element)
        {
            Captured = element;
            UpdateUIElementMouseOvers();
        }
        
        public Vector2 GetPosition(IInputElement relativeTo)
        {
            Vector2 p = GetClientPosition();
            Visual v = (Visual)relativeTo;

            if (v != null)
            {
                p -= v.VisualOffset;

                foreach (Visual ancestor in VisualTreeHelper.GetAncestors(v).OfType<Visual>())
                {
                    p -= ancestor.VisualOffset;
                }
            }

            return p;
        }

        public void Update()
        {
            MouseState oldState = _mouseState;
            MouseState newState = SparkMouse.GetMouseState();

            if (newState.Position != oldState.Position)
            {
                UpdateUIElementMouseOvers();

                InputManager.Current.ProcessInput(new MouseEventArgs(this)
                {
                    RoutedEvent = UIElement.MouseMoveEvent
                });
            }

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                InputManager.Current.ProcessInput(new MouseButtonEventArgs(this)
                {
                    RoutedEvent = UIElement.MouseLeftButtonDownEvent
                });
            }

            if (newState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed)
            {
                InputManager.Current.ProcessInput(new MouseButtonEventArgs(this)
                {
                    RoutedEvent = UIElement.MouseLeftButtonUpEvent
                });
            }
            
            _mouseState = newState;
        }

        private Vector2 GetClientPosition()
        {
            return _mouseState.Position;
        }

        private Vector2 GetScreenPosition()
        {
            return _mouseState.Position;
        }
        
        private void UpdateUIElementMouseOvers()
        {
            IEnumerable<UIElement> current = ElementAndAncestors(Target);

            foreach (UIElement ui in current.Except(_mouseOvers))
            {
                ui.SetValue(UIElement.IsMouseOverProperty, true);
                _mouseOvers.Add(ui);

                MouseEventArgs e = new MouseEventArgs(this);
                e.RoutedEvent = UIElement.MouseEnterEvent;
                ui.RaiseEvent(e);
            }

            foreach (UIElement ui in _mouseOvers.Except(current).ToArray())
            {
                ui.SetValue(UIElement.IsMouseOverProperty, false);
                _mouseOvers.Remove(ui);

                MouseEventArgs e = new MouseEventArgs(this);
                e.RoutedEvent = UIElement.MouseLeaveEvent;
                ui.RaiseEvent(e);
            }
        }

        private IEnumerable<UIElement> ElementAndAncestors(IInputElement mouseOver)
        {
            UIElement mo = (UIElement)mouseOver;

            if (mo != null)
            {
                yield return mo;

                foreach (UIElement e in VisualTreeHelper.GetAncestors(mo).OfType<UIElement>())
                {
                    yield return e;
                }
            }
        }
    }
}
