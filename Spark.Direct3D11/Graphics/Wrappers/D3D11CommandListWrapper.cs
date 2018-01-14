namespace Spark.Direct3D11.Graphics
{
    using Spark.Graphics;
    using Spark.Utilities;

    using D3D11 = SharpDX.Direct3D11;

    public sealed class D3D11CommandListWrapper : Disposable, ICommandList, ID3D11CommandList
    {
        private string _name;

        internal D3D11CommandListWrapper(D3D11.CommandList commandList)
        {
            D3DCommandList = commandList;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (D3DCommandList != null)
                {
                    D3DCommandList.DebugName = _name;
                }
            }
        }

        public D3D11.CommandList D3DCommandList { get; }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                D3DCommandList.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
