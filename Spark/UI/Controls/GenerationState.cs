namespace Spark.UI.Controls
{
    using Primitives;
    using Utilities;

    internal class GenerationState : Disposable
    {
        public bool AllowStartAtRealizedItem { get; set; }

        public GeneratorDirection Direction { get; set; }

        public ItemContainerGenerator Generator { get; set; }

        public GeneratorPosition Position { get; set; }

        public int Step => Direction == GeneratorDirection.Forward ? 1 : -1;

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }
            
            Generator.GenerationState = null;

            base.Dispose(isDisposing);
        }
    }
}
