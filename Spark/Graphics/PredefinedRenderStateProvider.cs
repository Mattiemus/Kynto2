namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;

    using Utilities;

    /// <summary>
    /// Standard implementation for creating all the engine's prebuilt render states.
    /// </summary>
    public sealed class PredefinedRenderStateProvider : BaseDisposable, IPredefinedBlendStateProvider, IPredefinedDepthStencilStateProvider, IPredefinedRasterizerStateProvider, IPredefinedSamplerStateProvider, IDisposable
    {
        private readonly Dictionary<string, RenderState> _namesToStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredefinedRenderStateProvider"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system used to create the renderstates.</param>
        public PredefinedRenderStateProvider(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            RenderSystem = renderSystem;
            _namesToStates = new Dictionary<string, RenderState>();

            CreateBlendStates();
            CreateDepthStencilStates();
            CreateRasterizerStates();
            CreateSamplerStates();
        }

        /// <summary>
        /// Gets the rendersystem the prebuilt renderstates are associated with.
        /// </summary>
        public IRenderSystem RenderSystem { get; }

        #region Blend States

        /// <summary>
        /// Gets a predefined state object for opaque blending, where no blending occurs and destination color overwrites source color. This is
        /// the default state.
        /// </summary>
        public BlendState Opaque { get; private set; }

        /// <summary>
        /// Gets a predefined state object for premultiplied alpha blending, where source and destination colors are blended by using
        /// alpha, and where the color contains alpha pre-multiplied.
        /// </summary>
        public BlendState AlphaBlendPremultiplied { get; private set; }

        /// <summary>
        /// Gets a predefined state object for additive blending, where source and destination color are blended without using alpha.
        /// </summary>
        public BlendState AdditiveBlend { get; private set; }

        /// <summary>
        /// Gets a predefined state object for non-premultiplied alpha blending, where the source and destination color are blended by using alpha,
        /// and where the color does not contain the alpha pre-multiplied.
        /// </summary>
        public BlendState AlphaBlendNonPremultiplied { get; private set; }

        #endregion

        #region DepthStencil States

        /// <summary>
        /// Gets a predefined state object where the depth buffer is disabled and writing to it is disabled.
        /// </summary>
        public DepthStencilState None { get; private set; }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabled and writing to it is disabled.
        /// </summary>
        public DepthStencilState DepthWriteOff { get; private set; }

        /// <summary>
        /// Gets a predefined state object where the depth buffer is enabld and writing to it is enabled.
        /// This is the default state.
        /// </summary>
        public DepthStencilState Default { get; private set; }

        #endregion

        #region Rasterizer States

        /// <summary>
        /// Gets a predefined state object where culling is disabled.
        /// </summary>
        public RasterizerState CullNone { get; private set; }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have
        /// a clockwise vertex winding. This is the default state.
        /// </summary>
        public RasterizerState CullBackClockwiseFront { get; private set; }

        /// <summary>
        /// Gets a predefined state object where back faces are culled and front faces have a counterclockwise
        /// vertex winding.
        /// </summary>
        public RasterizerState CullBackCounterClockwiseFront { get; private set; }

        /// <summary>
        /// Gets a predefined state object where culling is disabled and fillmode is wireframe.
        /// </summary>
        public RasterizerState CullNoneWireframe { get; private set; }

        #endregion

        #region Sampler States

        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates wrap.
        /// </summary>
        public SamplerState PointWrap { get; private set; }

        /// <summary>
        /// Gets the predefined state object where point filtering is used and UVW coordinates are clamped in the range of [0, 1]. This
        /// is the default state.
        /// </summary>
        public SamplerState PointClamp { get; private set; }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates wrap.
        /// </summary>
        public SamplerState LinearWrap { get; private set; }

        /// <summary>
        /// Gets the predefined state object where linear filtering is used and UVW coordinates are clamped in the range of [0, 1].
        /// </summary>
        public SamplerState LinearClamp { get; private set; }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates wrap.
        /// </summary>
        public SamplerState AnisotropicWrap { get; private set; }

        /// <summary>
        /// Gets the predefined state object where anisotropic filtering is used and UVW coordinates are
        /// clamped in the range of [0, 1].
        /// </summary>
        public SamplerState AnisotropicClamp { get; private set; }

        #endregion

        /// <summary>
        /// Queries a predefined render state by its name.
        /// </summary>
        /// <typeparam name="T">Render state type</typeparam>
        /// <param name="name">Name of the render state</param>
        /// <returns>The render state, or null if it was not found.</returns>
        public T GetRenderStateByName<T>(string name) where T : RenderState
        {
            if (_namesToStates.TryGetValue(name, out RenderState rs))
            {
                return rs as T;
            }

            return null;
        }

        /// <summary>
        /// Queries a predefined blend state by name.
        /// </summary>
        /// <param name="name">Name of the blend state.</param>
        /// <returns>Blend state, or null if it does not exist.</returns>
        public BlendState GetBlendStateByName(string name)
        {
            return GetRenderStateByName<BlendState>(name);
        }

        /// <summary>
        /// Queries a predefined depth stencil state by name.
        /// </summary>
        /// <param name="name">Name of the depth stencil state.</param>
        /// <returns>DepthStencil state, or null if it does not exist.</returns>
        public DepthStencilState GetDepthStencilStateByName(string name)
        {
            return GetRenderStateByName<DepthStencilState>(name);
        }

        /// <summary>
        /// Queries a predefined rasterizer state by name.
        /// </summary>
        /// <param name="name">Name of the rasterizer state.</param>
        /// <returns>Rasterizer state, or null if it does not exist.</returns>
        public RasterizerState GetRasterizerStateByName(string name)
        {
            return GetRenderStateByName<RasterizerState>(name);
        }

        /// <summary>
        /// Queries a predefined sampler state by name.
        /// </summary>
        /// <param name="name">Name of the sampler state.</param>
        /// <returns>Sampler state, or null if it does not exist.</returns>
        public SamplerState GetSamplerStateByName(string name)
        {
            return GetRenderStateByName<SamplerState>(name);
        }

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
            
            if (isDisposing)
            {
                _namesToStates.Clear();

                DestroyBlendStates();
                DestroyDepthStencilStates();
                DestroyRasterizerStates();
                DestroySamplerStates();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// Creates the default blend states
        /// </summary>
        private void CreateBlendStates()
        {
            // Opaque
            Opaque = new BlendState(RenderSystem)
            {
                BlendEnable = false
            };
            Opaque.BindRenderState();
            AddRenderState("Opaque", Opaque);

            // Alpha blend premultiplied
            AlphaBlendPremultiplied = new BlendState(RenderSystem)
            {
                BlendEnable = true,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };
            AlphaBlendPremultiplied.BindRenderState();
            AddRenderState("AlphaBlendPremultiplied", AlphaBlendPremultiplied);

            // Alpha blend non premultiplied
            AlphaBlendNonPremultiplied = new BlendState(RenderSystem)
            {
                BlendEnable = true,
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };
            AlphaBlendNonPremultiplied.BindRenderState();
            AddRenderState("AlphaBlendNonPremultiplied", AlphaBlendNonPremultiplied);

            // Additive blend
            AdditiveBlend = new BlendState(RenderSystem)
            {
                BlendEnable = true,
                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One
            };
            AdditiveBlend.BindRenderState();
            AddRenderState("AdditiveBlend", AdditiveBlend);
        }

        /// <summary>
        /// Creates the default depth stencil states
        /// </summary>
        private void CreateDepthStencilStates()
        {
            // None
            None = new DepthStencilState(RenderSystem)
            {
                DepthEnable = false,
                DepthWriteEnable = false
            };
            None.BindRenderState();
            AddRenderState("None", None);

            // Depth write off
            DepthWriteOff = new DepthStencilState(RenderSystem)
            {
                DepthEnable = true,
                DepthWriteEnable = false
            };
            DepthWriteOff.BindRenderState();
            AddRenderState("DepthWriteOff", DepthWriteOff);

            // Default
            Default = new DepthStencilState(RenderSystem)
            {
                DepthEnable = true,
                DepthWriteEnable = true
            };
            Default.BindRenderState();
            AddRenderState("Default", Default);
        }

        /// <summary>
        /// Creates the rasterizer states
        /// </summary>
        private void CreateRasterizerStates()
        {
            // Cull none
            CullNone = new RasterizerState(RenderSystem)
            {
                Cull = CullMode.None
            };
            CullNone.BindRenderState();
            AddRenderState("CullNone", CullNone);

            // Cull back clockwise front
            CullBackClockwiseFront = new RasterizerState(RenderSystem)
            {
                Cull = CullMode.Back,
                VertexWinding = VertexWinding.Clockwise
            };
            CullBackClockwiseFront.BindRenderState();
            AddRenderState("CullBackClockwiseFront", CullBackClockwiseFront);

            // Cull back counter clockwise front
            CullBackCounterClockwiseFront = new RasterizerState(RenderSystem)
            {
                Cull = CullMode.Back,
                VertexWinding = VertexWinding.CounterClockwise
            };
            CullBackCounterClockwiseFront.BindRenderState();
            AddRenderState("CullBackCounterClockwiseFront", CullBackCounterClockwiseFront);

            // Cull none wireframe
            CullNoneWireframe = new RasterizerState(RenderSystem)
            {
                Cull = CullMode.None,
                Fill = FillMode.WireFrame
            };
            CullNoneWireframe.BindRenderState();
            AddRenderState("CullNoneWireframe", CullNoneWireframe);
        }

        /// <summary>
        /// Creates the sampler sttates
        /// </summary>
        private void CreateSamplerStates()
        {
            // Point wrap
            PointWrap = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Point,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };
            PointWrap.BindRenderState();
            AddRenderState("PointWrap", PointWrap);

            // Point clamp
            PointClamp = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Point,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            };
            PointClamp.BindRenderState();
            AddRenderState("PointClamp", PointClamp);

            // Linear wrap
            LinearWrap = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Linear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };
            LinearWrap.BindRenderState();
            AddRenderState("LinearWrap", LinearWrap);

            // Linear clamp
            LinearClamp = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Linear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            };
            LinearClamp.BindRenderState();
            AddRenderState("LinearClamp", LinearClamp);

            // Anisotropic wrap
            AnisotropicWrap = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Anisotropic,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };
            AnisotropicWrap.BindRenderState();
            AddRenderState("AnisotropicWrap", AnisotropicWrap);

            // Anisotropic clamp
            AnisotropicClamp = new SamplerState(RenderSystem)
            {
                Filter = TextureFilter.Anisotropic,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            };
            AnisotropicClamp.BindRenderState();
            AddRenderState("AnisotropicClamp", AnisotropicClamp);
        }

        /// <summary>
        /// Adds the render state
        /// </summary>
        /// <param name="name">Render state name</param>
        /// <param name="rs">Render state instance</param>
        private void AddRenderState(string name, RenderState rs)
        {
            rs._predefinedStateName = name;
            rs.Name = name;

            _namesToStates.Add(name, rs);
        }

        /// <summary>
        /// Destroys the blend states
        /// </summary>
        private void DestroyBlendStates()
        {
            if (Opaque != null)
            {
                Opaque.Dispose();
                Opaque = null;
            }

            if (AlphaBlendPremultiplied != null)
            {
                AlphaBlendPremultiplied.Dispose();
                AlphaBlendPremultiplied = null;
            }

            if (AlphaBlendNonPremultiplied != null)
            {
                AlphaBlendNonPremultiplied.Dispose();
                AlphaBlendNonPremultiplied = null;
            }

            if (AdditiveBlend != null)
            {
                AdditiveBlend.Dispose();
                AdditiveBlend = null;
            }
        }

        /// <summary>
        /// Destroys the depth stencil states
        /// </summary>
        private void DestroyDepthStencilStates()
        {
            if (None != null)
            {
                None.Dispose();
                None = null;
            }

            if (DepthWriteOff != null)
            {
                DepthWriteOff.Dispose();
                DepthWriteOff = null;
            }

            if (Default != null)
            {
                Default.Dispose();
                Default = null;
            }
        }
        
        /// <summary>
        /// Destroys the rasterizer states
        /// </summary>
        private void DestroyRasterizerStates()
        {
            if (CullNone != null)
            {
                CullNone.Dispose();
                CullNone = null;
            }

            if (CullBackClockwiseFront != null)
            {
                CullBackClockwiseFront.Dispose();
                CullBackClockwiseFront = null;
            }

            if (CullBackCounterClockwiseFront != null)
            {
                CullBackCounterClockwiseFront.Dispose();
                CullBackCounterClockwiseFront = null;
            }

            if (CullNoneWireframe != null)
            {
                CullNoneWireframe.Dispose();
                CullNoneWireframe = null;
            }
        }

        /// <summary>
        /// Destroys the sampler states
        /// </summary>
        private void DestroySamplerStates()
        {
            if (PointWrap != null)
            {
                PointWrap.Dispose();
                PointWrap = null;
            }

            if (PointClamp != null)
            {
                PointClamp.Dispose();
                PointClamp = null;
            }

            if (LinearWrap != null)
            {
                LinearWrap.Dispose();
                LinearWrap = null;
            }

            if (LinearClamp != null)
            {
                LinearClamp.Dispose();
                LinearClamp = null;
            }

            if (AnisotropicWrap != null)
            {
                AnisotropicWrap.Dispose();
                AnisotropicWrap = null;
            }

            if (AnisotropicClamp != null)
            {
                AnisotropicClamp.Dispose();
                AnisotropicClamp = null;
            }
        }
    }
}
