namespace Spark.UI.Data
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public class BindingExpression : BindingExpressionBase
    {
        private readonly IPropertyPathParser _pathParser;
        private PropertyPathToken[] _chain;

        public BindingExpression(
            IPropertyPathParser pathParser,
            DependencyObject target,
            DependencyProperty dp,
            Binding binding)
            : base(target, dp)
        {
            _pathParser = pathParser;
            ParentBinding = binding;
        }

        public object DataItem => ParentBinding.Source;

        public Binding ParentBinding { get; }

        public object ResolvedSource { get; }

        public string ResolvedSourcePropertyName { get; }

        protected override object Evaluate()
        {
            Tuple<object, string> o = RewritePath(ParentBinding.Path);
            _chain = _pathParser.Parse(o.Item1, o.Item2).ToArray();

            if (_chain != null)
            {
                AttachListeners();

                PropertyPathToken last = _chain.Last();

                if (last.Type == PropertyPathTokenType.FinalValue)
                {
                    return last.Object;
                }

                return DependencyProperty.UnsetValue;
            }

            return null;
        }
        
        private Tuple<object, string> RewritePath(PropertyPath path)
        {
            string pathString = ((path != null) ? path.Path : null) ?? string.Empty;

            if (DataItem != null)
            {
                return Tuple.Create(DataItem, pathString);
            }

            FrameworkElement fe = Target as FrameworkElement;
            string prefix = null;

            if (ParentBinding.RelativeSource != null)
            {
                switch (ParentBinding.RelativeSource.Mode)
                {
                    case RelativeSourceMode.TemplatedParent:
                        if (fe != null)
                        {
                            prefix = "TemplatedParent";
                        }

                        break;
                }
            }
            else
            {
                prefix = "DataContext";
            }

            if (prefix != null)
            {
                string result = prefix;
                if (!string.IsNullOrWhiteSpace(pathString))
                {
                    result += "." + pathString;
                }

                return Tuple.Create((object)fe, result);
            }

            throw new NotSupportedException("Don't know how to get binding source!");
        }

        private void AttachListeners()
        {
            foreach (PropertyPathToken link in _chain.Take(_chain.Length - 1))
            {
                AttachListener(link);
            }
        }

        private void AttachListener(PropertyPathToken link)
        {
            IObservableDependencyObject dependencyObject = link.Object as IObservableDependencyObject;
            INotifyPropertyChanged inpc = link.Object as INotifyPropertyChanged;

            if (dependencyObject != null)
            {
                dependencyObject.AttachPropertyChangedHandler(link.PropertyName, DependencyPropertyChanged);
            }
            else if (inpc != null)
            {
                inpc.PropertyChanged += PropertyChanged;
            }
        }

        private void DetachListeners()
        {
            foreach (PropertyPathToken link in _chain.Take(_chain.Length - 1))
            {
                DetachListener(link);
            }
        }

        private void DetachListener(PropertyPathToken link)
        {
            IObservableDependencyObject dependencyObject = link.Object as IObservableDependencyObject;
            INotifyPropertyChanged inpc = link.Object as INotifyPropertyChanged;

            if (dependencyObject != null)
            {
                dependencyObject.RemovePropertyChangedHandler(link.PropertyName, DependencyPropertyChanged);
            }
            else if (inpc != null)
            {
                inpc.PropertyChanged -= PropertyChanged;
            }
        }

        private PropertyPathToken FindLinkInChain(object o)
        {
            foreach (PropertyPathToken t in _chain)
            {
                if (t.Object == o)
                {
                    return t;
                }
            }

            throw new InvalidOperationException("Internal error: Could not find bound object.");
        }

        private void DependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DetachListeners();
            Invalidate();
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyPathToken link = FindLinkInChain(sender);

            if (e.PropertyName == link.PropertyName)
            {
                DetachListeners();
                Invalidate();
            }
        }
    }
}
