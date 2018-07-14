namespace Spark.UI.Controls
{
    using System;

    using Input;
    using Graphics;
    using Media;
    using Math;

    public class InterfaceHost : ContentControl, ITopLevelElement
    {
        private readonly DrawingContext _drawingContext;

        public InterfaceHost(IRenderSystem renderSystem, Size initialSize)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            PresentationSource = new PresentationSource();
            PresentationSource.RootVisual = this;
            //PresentationSource.Closed += (s, e) => OnClosed(EventArgs.Empty);
            //PresentationSource.Resized += (s, e) => ((ITopLevelWindow)this).DoLayoutPass();

            Mouse.PrimaryDevice.ActiveSource = PresentationSource;
            Keyboard.PrimaryDevice.ActiveSource = PresentationSource;
            
            _drawingContext = new DrawingContext(renderSystem);

            Background = new SolidColorBrush(Color.TransparentBlack);
            Width = initialSize.Width;
            Height = initialSize.Height;
        }

        static InterfaceHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InterfaceHost), new FrameworkPropertyMetadata(typeof(InterfaceHost)));
            WidthProperty.OverrideMetadata(typeof(InterfaceHost), new PropertyMetadata(WidthChanged));
            HeightProperty.OverrideMetadata(typeof(InterfaceHost), new PropertyMetadata(HeightChanged));
        }

        public event EventHandler Closed;
        
        public PresentationSource PresentationSource { get; }
        
        public void DoLayoutPass()
        {
            if (PresentationSource == null)
            {
                return;
            }

            Size clientSize = PresentationSource.ClientSize;
            Measure(clientSize);
            Arrange(new RectangleF(Vector2.Zero, clientSize));
        }

        public void Render(IRenderContext renderContext)
        {
            _drawingContext.Begin(renderContext);
            Render(_drawingContext, this);
            _drawingContext.End();
        }

        protected virtual void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        private static void WidthChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InterfaceHost interfaceHost = (InterfaceHost)sender;
            interfaceHost.PresentationSource.ClientSize = new Size((float)e.NewValue, interfaceHost.PresentationSource.ClientSize.Height);
        }

        private static void HeightChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InterfaceHost interfaceHost = (InterfaceHost)sender;
            interfaceHost.PresentationSource.ClientSize = new Size(interfaceHost.PresentationSource.ClientSize.Width, (float)e.NewValue);
        }

        private void Render(DrawingContext drawingContext, DependencyObject o)
        {
            Visual visual = o as Visual;
            UIElement uiElement = o as UIElement;
            int popCount = 0;

            if (uiElement != null)
            {
                drawingContext.PushTranslation(uiElement.VisualOffset);
                ++popCount;

                if (uiElement.Opacity != 1)
                {
                    drawingContext.PushOpacity(uiElement.Opacity);
                    ++popCount;
                }

                uiElement.OnRender(drawingContext);
            }

            if (visual != null)
            {
                for (int i = 0; i < visual.VisualChildrenCount; ++i)
                {
                    Render(drawingContext, visual.GetVisualChild(i));
                }
            }

            for (int i = 0; i < popCount; ++i)
            {
                drawingContext.Pop();
            }
        }
    }
}
