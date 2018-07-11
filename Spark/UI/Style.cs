namespace Spark.UI
{
    using System;
    using System.Windows.Markup;

    [Ambient]
    [ContentProperty(nameof(Setters))]
    [DictionaryKeyProperty(nameof(TargetType))]
    public class Style : INameScope
    {
        private readonly NameScope _nameScope;

        public Style()
        {
            _nameScope = new NameScope();

            Setters = new SetterBaseCollection();
        }

        public Style(Type targetType)
            : this()
        {
            TargetType = targetType;
        }

        public Style(Type targetType, Style basedOn)
            : this()
        {
            TargetType = targetType;
            BasedOn = basedOn;
        }

        [Ambient]
        public Style BasedOn { get; set; }

        [Ambient]
        public ResourceDictionary Resources { get; set; }

        public SetterBaseCollection Setters { get; private set; }

        [Ambient]
        public Type TargetType { get; set; }

        public object FindName(string name)
        {
            return _nameScope.FindName(name);
        }

        public void RegisterName(string name, object scopedElement)
        {
            _nameScope.RegisterName(name, scopedElement);
        }

        public void UnregisterName(string name)
        {
            _nameScope.UnregisterName(name);
        }

        internal void Attach(FrameworkElement frameworkElement)
        {
            foreach (SetterBase setter in Setters)
            {
                setter.Attach(frameworkElement);
            }
        }

        internal void Detach(FrameworkElement frameworkElement)
        {
            foreach (SetterBase setter in Setters)
            {
                setter.Detach(frameworkElement);
            }
        }
    }
}
