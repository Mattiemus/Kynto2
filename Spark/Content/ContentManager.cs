namespace Spark.Content
{
    using Utilities;

    public sealed class ContentManager : BaseDisposable
    {
        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }
            
            base.Dispose(isDisposing);
        }
    }
}
