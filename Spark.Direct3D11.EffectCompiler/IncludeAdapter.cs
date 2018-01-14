namespace Spark.Direct3D11.Graphics
{
    using System;
    using System.IO;

    using D3DC = SharpDX.D3DCompiler;
    using SDX = SharpDX;

    public sealed class IncludeAdapter : SDX.CallbackBase, D3DC.Include
    {
        public IncludeAdapter(IIncludeHandler handler)
        {
            IncludeHandler = handler;
        }
        public IIncludeHandler IncludeHandler { get; }


        public void Close(Stream stream)
        {
            IncludeHandler?.Close(stream);
        }

        public Stream Open(D3DC.IncludeType type, String fileName, Stream parentStream)
        {
            if (IncludeHandler != null)
            {
                IncludeType incType = (type == D3DC.IncludeType.Local) ? IncludeType.Local : IncludeType.System;
                return IncludeHandler.Open(incType, fileName, parentStream);
            }

            return null;
        }
    }
}
