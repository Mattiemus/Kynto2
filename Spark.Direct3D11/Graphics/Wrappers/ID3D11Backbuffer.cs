namespace Spark.Direct3D11.Graphics
{
    using System;

    /// <summary>
    /// Defines an object that manages a backbuffer that is set to the render context. When it is the active backbuffer and is resized,
    /// the render context automatically knows to re-apply the resource views.
    /// </summary>
    public interface ID3D11Backbuffer : ID3D11RenderTargetView, ID3D11DepthStencilView
    {
        /// <summary>
        /// Occurs when the backbuffer views are destroyed during a resize or reset event.
        /// </summary>
        event TypedEventHandler<ID3D11Backbuffer, EventArgs> OnResetResize;
    }
}
