namespace Spark.Direct3D11.Graphics
{
    using System;

    using Spark.Graphics;
    using Spark.Utilities;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// A D3D11 Shader resource that specifically holds shader resource views.
    /// </summary>
    public sealed class D3D11ShaderResourceWrapper : Disposable, IShaderResource, ID3D11ShaderResourceView
    {
        private string _name;
        private readonly bool _owned;

        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11ShaderResourceWrapper"/> class.
        /// </summary>
        /// <param name="shaderResourceView">The shader resource view to manage.</param>
        public D3D11ShaderResourceWrapper(D3D11.ShaderResourceView shaderResourceView) 
            : this(shaderResourceView, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11ShaderResourceWrapper"/> class.
        /// </summary>
        /// <param name="shaderResourceView">The shader resource view to manage.</param>
        /// <param name="isOwned">True if this holder owns the resource, otherwise false. If true then when dispose is called, it will clean the resource up.</param>
        public D3D11ShaderResourceWrapper(D3D11.ShaderResourceView shaderResourceView, bool isOwned)
        {
            if (shaderResourceView == null)
            {
                throw new ArgumentNullException(nameof(shaderResourceView));
            }

            D3DDevice = shaderResourceView.Device;
            ResourceType = Direct3DHelper.FromD3DShaderResourceViewDimension(shaderResourceView.Description.Dimension);
            _owned = isOwned;
        }
        
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (D3DShaderResourceView != null)
                {
                    D3DShaderResourceView.DebugName = (string.IsNullOrEmpty(value)) ? value : value + "_SRV";
                }
            }
        }

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public ShaderResourceType ResourceType { get; }

        /// <summary>
        /// Gets the native D3D11 device.
        /// </summary>
        public D3D11.Device D3DDevice { get; }

        /// <summary>
        /// Gets the native D3D11 shader resource view.
        /// </summary>
        public D3D11.ShaderResourceView D3DShaderResourceView { get; private set; }

        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing && _owned)
            {
                D3DShaderResourceView.Dispose();
                D3DShaderResourceView = null;
            }

            base.Dispose(isDisposing);
        }
    }
}
