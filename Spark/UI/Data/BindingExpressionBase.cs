namespace Spark.UI.Data
{
    using System;

    public abstract class BindingExpressionBase : Expression, IWeakEventListener
    {
        private bool _evaluated;
        private object _value;

        protected BindingExpressionBase(DependencyObject target, DependencyProperty dp)
        {
            Target = target;
            TargetProperty = dp;
        }

        public DependencyObject Target { get; }

        public DependencyProperty TargetProperty { get; }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public object GetValue()
        {
            if (!_evaluated)
            {
                _value = Evaluate();
                _evaluated = true;
            }

            return _value;
        }

        protected void Invalidate()
        {
            _evaluated = false;
            _value = null;
            Target.InvalidateProperty(TargetProperty);
        }

        protected abstract object Evaluate();
    }
}
