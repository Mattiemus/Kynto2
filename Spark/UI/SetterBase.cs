namespace Spark.UI
{
    using System;

    public abstract class SetterBase
    {
        public bool IsSealed { get; internal set; }

        internal abstract void Attach(FrameworkElement frameworkElement);

        internal abstract void Detach(FrameworkElement frameworkElement);

        protected void CheckSealed()
        {
            if (IsSealed)
            {
                throw new InvalidOperationException("Object is sealed.");
            }
        }
    }
}
