namespace Spark.Windows.Application
{
    using System;

    using Spark.Application;
    using Spark.Application.Implementation;
    using Spark.Windows.Application.Implementation;
    using Spark.Utilities;

    public sealed class WinFormsApplicationSystem : Disposable, IApplicationSystem
    {
        public event TypedEventHandler<IApplicationSystem> Disposing;

        public string Platform => "WindowsForms";

        public string Name => "WinForms_Application";

        public void Initialize(SparkEngine engine)
        {
            // No-op
        }
        
        public IApplicationHostImplementation CreateApplicationHostImplementation(ApplicationHost host)
        {
            return new WinFormsApplicationHostImplementation(this, host);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                OnDisposing();
            }

            base.Dispose(isDisposing);
        }

        private void OnDisposing()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
