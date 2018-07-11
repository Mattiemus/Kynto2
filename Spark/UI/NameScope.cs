namespace Spark.UI
{
    using System.Collections.Generic;
    using System.Windows.Markup;

    public class NameScope : INameScope
    {
        public static readonly DependencyProperty NameScopeProperty =
            DependencyProperty.RegisterAttached(
                nameof(NameScope),
                typeof(INameScope),
                typeof(NameScope));

        private readonly Dictionary<string, object> _scope;

        public NameScope()
        {
            _scope = new Dictionary<string, object>();
        }

        public static INameScope GetNameScope(DependencyObject dependencyObject)
        {
            return (INameScope)dependencyObject.GetValue(NameScopeProperty);
        }

        public static void SetNameScope(DependencyObject dependencyObject, INameScope value)
        {
            dependencyObject.SetValue(NameScopeProperty, value);
        }

        public object FindName(string name)
        {
            _scope.TryGetValue(name, out object result);
            return result;
        }

        public void RegisterName(string name, object scopedElement)
        {
            _scope.Add(name, scopedElement);
        }

        public void UnregisterName(string name)
        {
            _scope.Remove(name);
        }
    }
}
