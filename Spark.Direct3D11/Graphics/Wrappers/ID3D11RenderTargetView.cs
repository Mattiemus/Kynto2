namespace Spark.Direct3D11.Graphics
{
    using Math;
    using Spark.Graphics;

    using D3D11 = SharpDX.Direct3D11;

    /// <summary>
    /// Defines an object that holds a native D3D11 render target view and which can resolve the resource.
    /// </summary>
    public interface ID3D11RenderTargetView
    {
        /// <summary>
        /// Gets the native D3D11 render target view.
        /// </summary>
        D3D11.RenderTargetView D3DRenderTargetView { get; }

        /// <summary>
        /// Called on the first render target in the group before the group is bound to the context.
        /// </summary>
        void NotifyOnFirstBind();

        /// <summary>
        /// Resolves the resource if its multisampled and does any mip map generation.
        /// </summary>
        /// <param name="deviceContext">Device context</param>
        void ResolveResource(D3D11.DeviceContext deviceContext);

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="deviceContext">Device context</param>
        /// <param name="options">Clear options</param>
        /// <param name="color">Color to clear to.</param>
        /// <param name="depth">Depth to clear to.</param>
        /// <param name="stencil">Stencil to clear to</param>
        void Clear(D3D11.DeviceContext deviceContext, ClearOptions options, Color color, float depth, int stencil);
    }
}
