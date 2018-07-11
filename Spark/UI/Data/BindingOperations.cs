namespace Spark.UI.Data
{
    using System;

    public static class BindingOperations
    {
        public static BindingExpressionBase SetBinding(
            DependencyObject target,
            DependencyProperty dp,
            BindingBase binding)
        {
            Binding b = binding as Binding;
            if (b == null)
            {
                throw new NotSupportedException("Unsupported binding type.");
            }

            return target.SetBinding(dp, b);
        }
    }
}
