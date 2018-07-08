namespace Spark.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public class Binding
    {
        private readonly List<Tuple<FrameworkElement, DependencyProperty>> _targetProperties;
        private PropertyInfo _sourceProperty;
        private object _source;

        public Binding()
        {
            _targetProperties = new List<Tuple<FrameworkElement, DependencyProperty>>();
            Mode = BindingMode.TwoWay;
        }

        public Binding(string path) : this()
        {
            Path = new PropertyPath(path);
        }

        public event EventHandler SourceUpdated;
        public event EventHandler TargetUpdated;

        internal object DataContext { get; set; }

        public string ElementName { get; set; }

        public BindingMode Mode { get; set; }

        public PropertyPath Path { get; set; }

        public object Source
        {
            get => _source;
            set
            {
                if (_targetProperties.Count > 0)
                {
                    throw new InvalidOperationException("Source cannot be changed after binding");
                }

                if (value != _source)
                {
                    _sourceProperty = null;
                    if (_source != null)
                    {
                        if (_source is INotifyPropertyChanged notifyPropertyChanged)
                        {
                            notifyPropertyChanged.PropertyChanged -= OnSourcePropertyChanged;
                        }
                        else
                        {
                            if (_source is DependencyObject dependencyObject)
                            {
                                dependencyObject.PropertyChanged -= OnSourceDependencyPropertyChanged;
                            }
                        }
                    }

                    _source = value;
                    if (_source != null)
                    {
                        if (_source is INotifyPropertyChanged notifyPropertyChanged)
                        {
                            notifyPropertyChanged.PropertyChanged += OnSourcePropertyChanged;
                        }
                        else
                        {
                            if (_source is DependencyObject dependencyObject)
                            {
                                dependencyObject.PropertyChanged += OnSourceDependencyPropertyChanged;
                            }
                        }
                    }

                    if (value != null)
                    {
                        DataContext = null;
                    }
                }
            }
        }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        internal void OnSourceUpdated()
        {
            SourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal void OnTargetUpdated()
        {
            TargetUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal void AddTargetProperty(FrameworkElement element, DependencyProperty dp)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (dp == null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            _targetProperties.Add(Tuple.Create(element, dp));
        }

        internal IEnumerable<DependencyProperty> GetTargetProperties(FrameworkElement element)
        {
            foreach (Tuple<FrameworkElement, DependencyProperty> prop in _targetProperties)
            {
                if (prop.Item1 == element)
                {
                    yield return prop.Item2;
                }
            }
        }

        internal bool HasTargetProperty(DependencyProperty dp)
        {
            return _targetProperties.Any(t => t.Item2 == dp);
        }

        internal void OnTargetPropertyUpdated(FrameworkElement element, DependencyProperty dp)
        {
            var prop = element.GetType().GetProperty(dp.Name);
            var value = prop.GetValue(element, null);

            SetSourceValue(value);
            OnTargetUpdated();
        }

        internal void SetSourceValue(object value)
        {
            if (_sourceProperty == null)
            {
                _sourceProperty = (Source ?? DataContext).GetType().GetProperty(Path.Path);
            }

            _sourceProperty.SetValue(Source ?? DataContext, value, null);
        }

        internal object GetSourceValue()
        {
            if (_sourceProperty == null)
            {
                _sourceProperty = (Source ?? DataContext).GetType().GetProperty(Path.Path);
            }

            return _sourceProperty.GetValue(Source ?? DataContext, null);
        }

        internal bool IsBound()
        {
            return ((Source != null || DataContext != null) && _targetProperties.Count > 0);
        }

        private void OnSourceDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == Path.Path)
            {
                OnSourceUpdated();
            }
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Path.Path)
            {
                OnSourceUpdated();
            }
        }
    }
}
