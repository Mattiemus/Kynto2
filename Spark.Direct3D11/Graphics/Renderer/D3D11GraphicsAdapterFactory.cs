namespace Spark.Direct3D11.Graphics.Renderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using DXGI = SharpDX.DXGI;

    /// <summary>
    /// Factory for creating <see cref="D3D11GraphicsAdapter" /> instances.
    /// </summary>
    public class D3D11GraphicsAdapterFactory : IEnumerable<D3D11GraphicsAdapter>
    {
        private readonly List<D3D11GraphicsAdapter> _adapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11GraphicsAdapterFactory"/> class.
        /// </summary>
        public D3D11GraphicsAdapterFactory()
        {
            var factory = new DXGI.Factory1();
            var adapters = factory.Adapters;

            _adapters = new List<D3D11GraphicsAdapter>();
            foreach (DXGI.Adapter adapter in adapters)
            {
                _adapters.Add(new D3D11GraphicsAdapter(adapter, _adapters.Count));
            }
        }

        /// <summary>
        /// Gets the default adapter.
        /// </summary>
        public D3D11GraphicsAdapter DefaultAdapter => _adapters[0];

        /// <summary>
        /// Gets the number of adapters.
        /// </summary>
        public int Count => _adapters.Count;

        /// <summary>
        /// Gets the adapter at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the adapter.</param>
        /// <returns>The adapter.</returns>
        public D3D11GraphicsAdapter this[int index]
        {
            get
            {
                if (index < 0 || index >= _adapters.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _adapters[index];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<D3D11GraphicsAdapter> GetEnumerator()
        {
            return _adapters.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _adapters.GetEnumerator();
        }
    }
}
