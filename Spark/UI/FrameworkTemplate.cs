namespace Spark.UI
{
    using System;
    using System.Windows.Markup;

    using Controls;

    [ContentProperty(nameof(Template))]
    public class FrameworkTemplate : INameScope, IQueryAmbient
    {
        private readonly NameScope _nameScope = new NameScope();

        [Ambient]
        [XamlDeferLoad(typeof(TemplateContentLoader), typeof(TemplateContent))]
        public TemplateContent Template { get; set; }

        public FrameworkElementFactory VisualTree { get; set; }

        public void RegisterName(string name, object scopedElement)
        {
            _nameScope.RegisterName(name, scopedElement);
        }

        public void UnregisterName(string name)
        {
            _nameScope.UnregisterName(name);
        }

        public object FindName(string name)
        {
            return _nameScope.FindName(name);
        }

        public bool IsAmbientPropertyAvailable(string propertyName)
        {
            // TODO: this should be more complex but I can't understand the docs:
            // http://msdn.microsoft.com/en-us/library/system.windows.markup.iqueryambient.aspx
            return true;
        }

        internal virtual FrameworkElement CreateVisualTree(DependencyObject parent)
        {
            if (Template != null)
            {
                FrameworkElement result = Template.Load() as FrameworkElement;
                result.TemplatedParent = parent;
                return result;
            }

            if (VisualTree != null)
            {
                FrameworkElement result = VisualTree.Load() as FrameworkElement;
                result.TemplatedParent = parent;
                return result;
            }

            throw new InvalidOperationException("One of the Template or VisualTree properties must be set on a FrameworkTemplate.");
        }
    }
}
