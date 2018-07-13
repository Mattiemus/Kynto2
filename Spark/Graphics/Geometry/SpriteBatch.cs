namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;

    using Utilities;
    using Math;

    /// <summary>
    /// Batcher that specializes in drawing many sprites to the screen. The primary 
    /// function is to enable batching of sprites based on their texture to only
    /// use the minimum number of draw calls needed to render all the requested sprites
    /// to the screen.
    /// </summary>
    public class SpriteBatch : Disposable
    {
        private const int MaxBatchSize = 2048;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private DataBuffer<VertexPositionColorTexture> _vertices;
        private int _spriteVbPos;

        private Sprite[] _spriteQueue;
        private int[] _sortedSpriteIndices;
        private Sprite[] _sortedSprites;
        private int _spriteQueueCount;
        private readonly int _batchSize;
        
        private bool _inBeginEnd;
        private bool _applyCameraViewProjection;
        private SpriteSortMode _sortMode;
        private Matrix4x4 _worldMatrix;
        private IEffectShaderGroup _customShaderGroup;
        private Effect _spriteEffect;
        private IEffectParameter _matrixParam;
        private IEffectParameter _samplerParam;
        private readonly IEffectShaderGroup _pass;

        private IRenderContext _renderContext;
        private IShaderStage _pixelStage;
        private SamplerState _ss;

        private readonly TextureComparer _textureComparer;
        private readonly OrthoComparer _frontToBackComparer;
        private readonly OrthoComparer _backToFrontComparer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create resources.</param>
        public SpriteBatch(IRenderSystem renderSystem) 
            : this(renderSystem, MaxBatchSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create resources.</param>
        /// <param name="maxBatchSize">Maximum number of sprite batches that can be buffered.</param>
        public SpriteBatch(IRenderSystem renderSystem, int maxBatchSize)
        {
            if (maxBatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchSize), "Batch size must be positive");
            }

            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            _indexBuffer = new IndexBuffer(renderSystem, IndexFormat.SixteenBits, CreateIndexData(MaxBatchSize), ResourceUsage.Immutable);
            _vertexBuffer = new VertexBuffer(renderSystem, VertexPositionColorTexture.VertexLayout, maxBatchSize * 4, ResourceUsage.Dynamic);
            _vertices = new DataBuffer<VertexPositionColorTexture>(maxBatchSize * 4);
            _spriteVbPos = 0;

            _spriteQueue = new Sprite[maxBatchSize];
            _spriteQueueCount = 0;
            _batchSize = maxBatchSize;
            
            _inBeginEnd = false;
            _sortMode = SpriteSortMode.Deferred;
            _worldMatrix = Matrix4x4.Identity;
            _customShaderGroup = null;
            _spriteEffect = renderSystem.StandardEffects.CreateEffect("Sprite");
            _matrixParam = _spriteEffect.Parameters["SpriteTransform"];
            _samplerParam = _spriteEffect.Parameters["SpriteMapSampler"];
            _pass = _spriteEffect.ShaderGroups["SpriteTexture"];

            _textureComparer = new TextureComparer(this);
            _frontToBackComparer = new OrthoComparer(this, true);
            _backToFrontComparer = new OrthoComparer(this, false);
        }

        /// <summary>
        /// Creates a sprite projection matrix that sprite batch uses internally, useful if using a custom shader with spritebatch.
        /// </summary>
        /// <param name="viewport">The camera viewport.</param>
        /// <param name="spriteTransform">The sprite projection transform.</param>
        public static void ComputeSpriteProjection(ref Viewport viewport, out Matrix4x4 spriteTransform)
        {
            float invWidth = (viewport.Width > 0) ? (1.0f / viewport.Width) : 0.0f;
            float invHeight = (viewport.Height > 0) ? (-1.0f / viewport.Height) : 0.0f;

            spriteTransform.M11 = invWidth * 2f;
            spriteTransform.M12 = 0f;
            spriteTransform.M13 = 0f;
            spriteTransform.M14 = 0f;

            spriteTransform.M21 = 0f;
            spriteTransform.M22 = invHeight * 2f;
            spriteTransform.M23 = 0f;
            spriteTransform.M24 = 0f;

            spriteTransform.M31 = 0f;
            spriteTransform.M32 = 0f;
            spriteTransform.M33 = 1f;
            spriteTransform.M34 = 0f;

            spriteTransform.M41 = -1f;
            spriteTransform.M42 = 1f;
            spriteTransform.M43 = 0f;
            spriteTransform.M44 = 1f;
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        public void Begin(IRenderContext renderContext)
        {
            Begin(renderContext, SpriteSortMode.Deferred, null, null, null, null, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode)
        {
            Begin(renderContext, sortMode, null, null, null, null, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, sortMode, null, null, null, null, worldMatrix, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            Begin(renderContext, sortMode, null, null, null, null, worldMatrix, applyCameraViewProj);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState)
        {
            Begin(renderContext, sortMode, blendState, null, null, null, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, sortMode, blendState, null, null, null, worldMatrix, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            Begin(renderContext, sortMode, blendState, null, null, null, worldMatrix, applyCameraViewProj);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState)
        {
            Begin(renderContext, sortMode, blendState, rasterizerState, samplerState, depthStencilState, null, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="customShaderGroup">Custom effect to use in leui of the sprite effect, if null then the sprite effect is used.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, IEffectShaderGroup customShaderGroup)
        {
            Begin(renderContext, sortMode, blendState, rasterizerState, samplerState, depthStencilState, customShaderGroup, Matrix4x4.Identity, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix4x4 worldMatrix)
        {
            Begin(renderContext, sortMode, blendState, rasterizerState, samplerState, depthStencilState, null, worldMatrix, false);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            Begin(renderContext, sortMode, blendState, rasterizerState, samplerState, depthStencilState, null, worldMatrix, applyCameraViewProj);
        }

        /// <summary>
        /// Begins a sprite batch operation.
        /// </summary>
        /// <param name="renderContext">Render context</param>
        /// <param name="sortMode">Sort mode to use</param>
        /// <param name="blendState">Blend state to use, if null then <see cref="BlendState.AlphaBlendNonPremultiplied"/> is used.</param>
        /// <param name="rasterizerState">Rasterizer state to use, if null then <see cref="RasterizerState.CullBackClockwiseFront"/> is used.</param>
        /// <param name="samplerState">Sampler state to use, if null then <see cref="SamplerState.LinearClamp"/> is used.</param>
        /// <param name="depthStencilState">Depth stencil state to use, if null then <see cref="DepthStencilState.None"/> is used.</param>
        /// <param name="customShaderGroup">Custom effect shader group to use in leui of the sprite effect, if null then the sprite effect is used.</param>
        /// <param name="worldMatrix">Transformation matrix for scale, rotation, and translation of each sprite.</param>
        /// <param name="applyCameraViewProj">Applies the view-projection matrix from the camera, otherwise creates a projection to draw in screen space.</param>
        public void Begin(IRenderContext renderContext, SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, SamplerState samplerState, DepthStencilState depthStencilState, IEffectShaderGroup customShaderGroup, Matrix4x4 worldMatrix, bool applyCameraViewProj)
        {
            ThrowIfDisposed();

            if (_inBeginEnd)
            {
                throw new InvalidOperationException("Cannot nest begin calls");
            }

            if (renderContext == null)
            {
                throw new ArgumentNullException(nameof(renderContext), "Render context cannot be null");
            }

            _pixelStage = renderContext.GetShaderStage(ShaderStage.PixelShader);
            _renderContext = renderContext;
            _sortMode = sortMode;

            if (blendState == null)
            {
                blendState = BlendState.AlphaBlendNonPremultiplied;
            }

            if (rasterizerState == null)
            {
                rasterizerState = RasterizerState.CullBackClockwiseFront;
            }

            if (samplerState == null)
            {
                samplerState = SamplerState.LinearClamp;
            }

            if (depthStencilState == null)
            {
                depthStencilState = DepthStencilState.None;
            }

            renderContext.BlendState = blendState;
            renderContext.RasterizerState = rasterizerState;
            renderContext.DepthStencilState = depthStencilState;
            _ss = samplerState;

            _customShaderGroup = customShaderGroup;
            _worldMatrix = worldMatrix;
            _inBeginEnd = true;
            _applyCameraViewProjection = applyCameraViewProj;

            SetBuffers();

            if (customShaderGroup == null)
            {
                ApplyEffect();
            }
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void Draw(Texture2D texture, Vector2 position, Color tintColor)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            Rectangle srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            DrawInternal(texture, dstRect, srcRect, true, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void Draw(Texture2D texture, Vector2 position, Color tintColor, Angle angle)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            Rectangle srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            DrawInternal(texture, dstRect, srcRect, true, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void Draw(Texture2D texture, Vector2 position, Color tintColor, Angle angle, Vector2 origin)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            Rectangle srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            DrawInternal(texture, dstRect, srcRect, true, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void Draw(Texture2D texture, Vector2 position, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            Rectangle srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            DrawInternal(texture, dstRect, srcRect, true, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color tintColor)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, sourceRectangle, false, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color tintColor, Angle angle)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, sourceRectangle, false, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, sourceRectangle, false, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, sourceRectangle, false, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color tintColor)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, null, false, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void Draw(Texture2D texture, RectangleF destinationRectangle, Color tintColor)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, null, false, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color tintColor, Angle angle)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, null, false, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color tintColor, Angle angle, Vector2 origin)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, null, false, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, null, false, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color tintColor)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color tintColor, Angle angle)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, 1, 1);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="destinationRectangle">Rectangle that specifies (in screen coordinates) the destination, position and size, for drawing the sprite. If this rectangle is not the same size as the source,
        /// the sprite will be scaled to fit.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            Vector4 dstRect = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
            DrawInternal(texture, dstRect, sourceRectangle, false, tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void Draw(Texture2D texture, Vector2 position, Vector2 scale, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, scale.X, scale.Y);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a sprite to the batch for rendering using the specified parameters. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="texture">Sprite texture</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="sourceRectangle">Rectangle that specifies the source texels from a texture, if null then the entire texture is used.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void Draw(Texture2D texture, Vector2 position, float scale, Rectangle? sourceRectangle, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            Vector4 dstRect = new Vector4(position.X, position.Y, scale, scale);
            DrawInternal(texture, dstRect, sourceRectangle, true, tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, Color tintColor)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, Color tintColor, Angle angle)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, Color tintColor, Angle angle, Vector2 origin)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, float scale, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, new Vector2(scale, scale), tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void DrawString(SpriteFont font, String text, Vector2 position, Vector2 scale, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, scale, tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Color tintColor)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, Angle.Zero, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Color tintColor, Angle angle)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, Vector2.Zero, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Color tintColor, Angle angle, Vector2 origin)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, origin, SpriteFlipEffect.None, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, Vector2.One, tintColor, angle, origin, flipEffect, 0);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, float scale, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, new Vector2(scale, scale), tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Adds a string to the batch for rendering using the specified parameters. The text is rendered as a series of sprites, one for each character. Depending on the sort mode
        /// this may be an immediate draw operation or one queued for later processing.
        /// </summary>
        /// <param name="font">Font that represents the character data that is used to draw the text.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position to draw the sprite, in screen coordinates.</param>
        /// <param name="scale">Scaling factor.</param>
        /// <param name="tintColor">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Vector2 scale, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            StringReference str = new StringReference(text);
            font.DrawString(this, str, position, scale, tintColor, angle, origin, flipEffect, depth);
        }

        /// <summary>
        /// Draws a sprite
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="dstRect">Destination rectangle</param>
        /// <param name="srcRect">Source rectangle</param>
        /// <param name="scaleDstFromSrc">Scaling factor.</param>
        /// <param name="color">Color to tint the sprite. Use <see cref="Color.White"/> for no tinting.</param>
        /// <param name="angle">Rotation angle to rotate the sprite about its origin.</param>
        /// <param name="origin">Sprite origin, (0,0) is the default which represents the upper-left corner of the sprite.</param>
        /// <param name="flipEffect">Flip mirror effects to apply to the sprite.</param>
        /// <param name="depth">Depth of the sprite for use with sorting. Lower values indicate sprites that are in the front, higher indicate those in back.</param>
        internal void DrawInternal(Texture2D texture, Vector4 dstRect, Rectangle? srcRect, bool scaleDstFromSrc, Color color, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("Draw called before begin");
            }

            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture), "Sprite texture cannot be null");
            }

            if (_spriteQueueCount >= _spriteQueue.Length)
            {
                Array.Resize(ref _spriteQueue, _spriteQueue.Length * 2);
            }

            Sprite sprite;
            sprite.Texture = texture;
            sprite.Color = color;
            sprite.Angle = angle;
            sprite.Origin = origin;
            sprite.OrthoOrder = depth;
            sprite.FlipEffect = flipEffect;

            // Compute our cropping and screen offsets. Scale the screen offsets if user hasn't supplied us with their own dest rectangle
            float dWidth = dstRect.Z;
            float dHeight = dstRect.W;

            // Using only a subset of the texture
            if (srcRect.HasValue)
            {
                Rectangle rect = srcRect.Value;
                if (rect.Height == 0 || rect.Width == 0)
                {
                    throw new ArgumentOutOfRangeException("Sprite rectangle cannot be empty");
                }

                Vector4 uvOffsets;
                uvOffsets.X = rect.X;
                uvOffsets.Y = rect.Y;
                uvOffsets.Z = rect.Width;
                uvOffsets.W = rect.Height;
                sprite.TextureOffsets = uvOffsets;

                // If scaling dest rect based on src rect width/height
                Vector4 scOffsets;
                scOffsets.X = dstRect.X;
                scOffsets.Y = dstRect.Y;
                if (scaleDstFromSrc)
                {
                    scOffsets.Z = dstRect.Z * rect.Width;
                    scOffsets.W = dstRect.W * rect.Height;
                }
                else
                {
                    scOffsets.Z = dstRect.Z;
                    scOffsets.W = dstRect.W;
                }
                sprite.ScreenOffsets = scOffsets;
            }
            // Otherwise using the whole texture
            else
            {
                Vector4 uvOffsets;
                uvOffsets.X = 0f;
                uvOffsets.Y = 0f;
                uvOffsets.Z = texture.Width;
                uvOffsets.W = texture.Height;
                sprite.TextureOffsets = uvOffsets;

                // If scaling dest rect based on texture width/height
                Vector4 scOffsets;
                scOffsets.X = dstRect.X;
                scOffsets.Y = dstRect.Y;
                if (scaleDstFromSrc)
                {
                    scOffsets.Z = dstRect.Z * texture.Width;
                    scOffsets.W = dstRect.W * texture.Height;
                }
                else
                {
                    scOffsets.Z = dstRect.Z;
                    scOffsets.W = dstRect.W;
                }
                sprite.ScreenOffsets = scOffsets;
            }

            // Add sprite to the queue
            _spriteQueue[_spriteQueueCount] = sprite;
            if (_sortMode == SpriteSortMode.Immediate)
            {
                RenderBatch(texture, _spriteQueue, 0, 1);
            }
            else
            {
                _spriteQueueCount++;
            }
        }

        /// <summary>
        /// Flushes the sprite batch and performs any remaining draw operations, then sets
        /// the batcher to it's default state. This does not restore the state on the render context.
        /// </summary>
        public void End()
        {
            ThrowIfDisposed();

            if (!_inBeginEnd)
            {
                throw new InvalidOperationException("End called before begin");
            }

            if (_sortMode != SpriteSortMode.Immediate && _spriteQueueCount > 0)
            {
                ProcessRenderQueue();
            }

            // Make sure we don't hold onto references...
            if (_customShaderGroup == null)
            {
                _samplerParam.SetResource((SamplerState)null);
            }

            _customShaderGroup = null;
            _renderContext = null;
            _pixelStage = null;
            _applyCameraViewProjection = false;
            _inBeginEnd = false;
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
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Dispose();
                    _vertexBuffer = null;
                }

                if (_indexBuffer != null)
                {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }

                if (_spriteEffect != null)
                {
                    _spriteEffect.Dispose();
                    _spriteEffect = null;
                    _matrixParam = null;
                    _samplerParam = null;
                    _customShaderGroup = null;
                }

                if (_vertices != null)
                {
                    _vertices.Dispose();
                    _vertices = null;
                }

                _spriteQueue = null;
                _sortedSpriteIndices = null;
                _sortedSprites = null;
            }

            base.Dispose(isDisposing);
        }
        
        /// <summary>
        /// Renders an entire batch
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="sprites">Sprites to be drawn</param>
        /// <param name="index">Starting sprite ndex</param>
        /// <param name="numBatches">Number of batches to be drawn</param>
        private void RenderBatch(Texture2D texture, Sprite[] sprites, int index, int numBatches)
        {
            // Setup pass/texture/state setting
            if (_customShaderGroup != null)
            {
                // Apply the custom effect shader group, then AFTER set the sprite resources (ensures they won't get overwrriten)
                _customShaderGroup.Apply(_renderContext);
                _pixelStage.SetShaderResource(0, texture);
                _pixelStage.SetSampler(0, _ss);

                OnRenderBatch(texture, sprites, index, numBatches);
            }
            else
            {
                _pixelStage.SetShaderResource(0, texture);

                // Don't need to set samplerstate again, because we did that at the beginning
                OnRenderBatch(texture, sprites, index, numBatches);
            }
        }

        /// <summary>
        /// Renders a batch of sprites
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="sprites">Sprites to be drawn</param>
        /// <param name="index">Starting sprite index</param>
        /// <param name="numBatches">Number of batches to draw</param>
        private void OnRenderBatch(Texture2D texture, Sprite[] sprites, int index, int numBatches)
        {
            float scaleWidth = 1.0f / texture.Width;
            float scaleHeight = 1.0f / texture.Height;

            // Setup the vertices for all the batches. The vertexbuffer acts like a wrap around queue
            while (numBatches > 0)
            {
                DataWriteOptions writeOptions = DataWriteOptions.NoOverwrite;
                int actCount = numBatches;
                if (actCount > (_batchSize - _spriteVbPos))
                {
                    // Need to split up, so set actual count to the remaining space
                    actCount = _batchSize - _spriteVbPos;

                    // Need to check if actually we've reached the end of the VB,
                    // if so we need to wrap around and set discard
                    if (_spriteVbPos % _batchSize == 0)
                    {
                        writeOptions = DataWriteOptions.Discard;
                        _spriteVbPos = 0;

                        // Reset actual count to the maximum space available, either
                        // requested batch number, or max batch size - whichever is smallest
                        actCount = Math.Min(_batchSize, numBatches);
                    }
                }

                // Loop through sprite array, create the geometry
                for (int i = index, j = 0; i < index + actCount; i++)
                {
                    Sprite sprite = sprites[i];
                    float cos = 1.0f;
                    float sin = 0.0f;

                    Angle angle = sprite.Angle;
                    if (angle != Angle.Zero)
                    {
                        cos = angle.Cos;
                        sin = angle.Sin;
                    }

                    Vector4 scOffsets = sprite.ScreenOffsets;
                    float sx = scOffsets.X;
                    float sy = scOffsets.Y;
                    float sw = scOffsets.Z;
                    float sh = scOffsets.W;

                    Vector4 texOffsets = sprite.TextureOffsets;
                    float tu = texOffsets.X * scaleWidth;
                    float tv = texOffsets.Y * scaleHeight;
                    float tw = texOffsets.Z * scaleWidth;
                    float th = texOffsets.W * scaleHeight;

                    Vector2 origin = sprite.Origin;
                    float oriX = origin.X / texOffsets.Z;
                    float oriY = origin.Y / texOffsets.W;

                    bool flipH = (sprite.FlipEffect & SpriteFlipEffect.FlipHorizontally) == SpriteFlipEffect.FlipHorizontally;
                    bool flipV = (sprite.FlipEffect & SpriteFlipEffect.FlipVertically) == SpriteFlipEffect.FlipVertically;

                    float z = sprite.OrthoOrder;

                    // Apply change of origin to screen width/height for rotations
                    float sw1 = -oriX * sw;
                    float sw2 = (1 - oriX) * sw;
                    float sh1 = -oriY * sh;
                    float sh2 = (1 - oriY) * sh;

                    // Top-left
                    Vector3 v = new Vector3((sx + (sw1 * cos)) - (sh1 * sin), (sy + (sw1 * sin)) + (sh1 * cos), z);
                    Vector2 uv = new Vector2(tu, tv);

                    if (flipH)
                    {
                        uv.X = 1.0f - uv.X;
                    }

                    if (flipV)
                    {
                        uv.Y = 1.0f - uv.Y;
                    }

                    _vertices[j++] = new VertexPositionColorTexture(v, sprite.Color, uv);

                    // Lower-left
                    v = new Vector3((sx + (sw1 * cos)) - (sh2 * sin), (sy + (sw1 * sin)) + (sh2 * cos), z);
                    uv = new Vector2(tu, tv + th);

                    if (flipH)
                    {
                        uv.X = 1.0f - uv.X;
                    }

                    if (flipV)
                    {
                        uv.Y = 1.0f - uv.Y;
                    }

                    _vertices[j++] = new VertexPositionColorTexture(v, sprite.Color, uv);

                    // Lower-right
                    v = new Vector3((sx + (sw2 * cos)) - (sh2 * sin), (sy + (sw2 * sin)) + (sh2 * cos), z);
                    uv = new Vector2(tu + tw, tv + th);

                    if (flipH)
                    {
                        uv.X = 1.0f - uv.X;
                    }

                    if (flipV)
                    {
                        uv.Y = 1.0f - uv.Y;
                    }

                    _vertices[j++] = new VertexPositionColorTexture(v, sprite.Color, uv);

                    // Upper-right
                    v = new Vector3((sx + (sw2 * cos)) - (sh1 * sin), (sy + (sw2 * sin)) + (sh1 * cos), z);
                    uv = new Vector2(tu + tw, tv);

                    if (flipH)
                    {
                        uv.X = 1.0f - uv.X;
                    }

                    if (flipV)
                    {
                        uv.Y = 1.0f - uv.Y;
                    }

                    _vertices[j++] = new VertexPositionColorTexture(v, sprite.Color, uv);
                }

                // Write to VB
                int stride = VertexPositionColorTexture.SizeInBytes;
                int offset = _spriteVbPos * stride * 4;
                _vertexBuffer.SetData(_renderContext, _vertices, 0, actCount * 4, offset, stride, writeOptions);

                // Render
                _renderContext.DrawIndexed(PrimitiveType.TriangleList, actCount * 6, _spriteVbPos * 6, 0);

                index += actCount;
                _spriteVbPos += actCount;
                numBatches -= actCount;
            }
        }

        /// <summary>
        /// Sorts the render queue
        /// </summary>
        private void SortRenderQueue()
        {
            // Queue indices for sprite sorting, so we don't have to keep copying the Sprite struct
            // all over the place.
            if (_sortedSpriteIndices == null || _sortedSpriteIndices.Length < _spriteQueueCount)
            {
                _sortedSpriteIndices = new int[_spriteQueueCount];
                _sortedSprites = new Sprite[_spriteQueueCount];
            }

            IComparer<int> comparer = _textureComparer;
            if (_sortMode == SpriteSortMode.BackToFront)
            {
                comparer = _backToFrontComparer;
            }
            else if (_sortMode == SpriteSortMode.FrontToBack)
            {
                comparer = _frontToBackComparer;
            }

            // Set the indices
            for (int i = 0; i < _spriteQueueCount; i++)
            {
                _sortedSpriteIndices[i] = i;
            }

            // Sort
            Array.Sort(_sortedSpriteIndices, 0, _spriteQueueCount, comparer);
        }

        /// <summary>
        /// Processes the render queue, rendering all the sprites in batches
        /// </summary>
        private void ProcessRenderQueue()
        {
            Sprite[] sprites;
            if (_sortMode == SpriteSortMode.Deferred)
            {
                sprites = _spriteQueue;
            }
            else
            {
                SortRenderQueue();
                sprites = _sortedSprites; // Set this after sort, since we may be resizing the sorted array
            }

            int index = 0;
            Texture2D prevTex = null;

            // Loop through the queue and accumulate sprites with the same texture,
            // when we hit a new texture, we render the accumulated sprites.
            for (int i = 0; i < _spriteQueueCount; i++)
            {
                Texture2D currTex;
                if (_sortMode == SpriteSortMode.Deferred)
                {
                    currTex = sprites[i].Texture;
                }
                else
                {
                    // Using sorted array - grab the sprite from the queue and place it in the sorted array
                    Sprite sprite = _spriteQueue[_sortedSpriteIndices[i]];
                    currTex = sprite.Texture;
                    sprites[i] = sprite;
                }

                // If texture is not the same, we render any accumulated sprites that have
                // not been rendered since they all had the same texture
                if (prevTex == null || prevTex.ResourceId != currTex.ResourceId)
                {
                    // Render accumulated sprites with the previous texture
                    if (i > index)
                    {
                        RenderBatch(prevTex, sprites, index, i - index);
                    }

                    index = i;
                    prevTex = currTex;
                }
            }

            // Perform final rendering for any sprites not yet rendered
            RenderBatch(prevTex, sprites, index, _spriteQueueCount - index);

            // Clear sprite queue
            Array.Clear(_spriteQueue, 0, _spriteQueueCount);

            // And sorted sprite queue if that was used
            if (_sortMode != SpriteSortMode.Deferred)
            {
                Array.Clear(_sortedSprites, 0, _spriteQueueCount);
            }

            _spriteQueueCount = 0;
        }
        
        /// <summary>
        /// Sets the buffers to be used when rendering
        /// </summary>
        private void SetBuffers()
        {
            _renderContext.SetVertexBuffer(_vertexBuffer);
            _renderContext.SetIndexBuffer(_indexBuffer);
        }

        /// <summary>
        /// Applies the effect to being used when rendering
        /// </summary>
        private void ApplyEffect()
        {
            Matrix4x4 matrix = Matrix4x4.Identity;

            if (_applyCameraViewProjection && _renderContext.Camera != null)
            {
                matrix = _renderContext.Camera.ViewProjectionMatrix;
            }
            else
            {
                Viewport viewport = (_renderContext.Camera == null) ? new Viewport(0, 0, 800, 600) : _renderContext.Camera.Viewport;

                ComputeSpriteProjection(ref viewport, out matrix);
            }
            
            Matrix4x4.Multiply(ref _worldMatrix, ref matrix, out Matrix4x4 spriteTransform);
            _matrixParam.SetValue(ref spriteTransform);
            _samplerParam.SetResource(_ss);

            _pass.Apply(_renderContext);
        }

        /// <summary>
        /// Creates the index data for the given sized batch
        /// </summary>
        /// <param name="maxBatchSize">Maximum batch size</param>
        /// <returns>Index buffer</returns>
        private IDataBuffer<short> CreateIndexData(int maxBatchSize)
        {
            DataBuffer<short> indices = new DataBuffer<short>(maxBatchSize * 6);

            for (short i = 0; i < maxBatchSize; i++)
            {
                short startIndex = (short)(i * 6);
                short index = (short)(i * 4);
                indices[startIndex] = index;
                indices[startIndex + 1] = (short)(index + 2);
                indices[startIndex + 2] = (short)(index + 1);
                indices[startIndex + 3] = index;
                indices[startIndex + 4] = (short)(index + 3);
                indices[startIndex + 5] = (short)(index + 2);
            }

            return indices;
        }

        /// <summary>
        /// Sprite information structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Sprite
        {
            public Texture2D Texture;
            public Vector4 TextureOffsets;
            public Vector4 ScreenOffsets;
            public Vector2 Origin;
            public Angle Angle;
            public float OrthoOrder;
            public SpriteFlipEffect FlipEffect;
            public Color Color;
        }

        /// <summary>
        /// Texture comparer
        /// </summary>
        private class TextureComparer : IComparer<int>
        {
            private readonly SpriteBatch _batch;

            /// <summary>
            /// Initializes a new instance of the <see cref="TextureComparer"/> class.
            /// </summary>
            /// <param name="batch">Parent sprite batch</param>
            public TextureComparer(SpriteBatch batch)
            {
                _batch = batch;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
            public int Compare(int x, int y)
            {
                Texture2D xTex = _batch._spriteQueue[x].Texture;
                Texture2D yTex = _batch._spriteQueue[y].Texture;

                return xTex.ResourceId - yTex.ResourceId;
            }
        }
        
        /// <summary>
        /// Orthographic comparer
        /// </summary>
        private class OrthoComparer : IComparer<int>
        {
            private readonly SpriteBatch _batch;
            private readonly bool _frontToBack;

            /// <summary>
            /// Initializes a new instance of the <see cref="OrthoComparer"/> class.
            /// </summary>
            /// <param name="batch">Parent sprite batch</param>
            /// <param name="frontToBack">True if front to back ordering should be used, false otherwise</param>
            public OrthoComparer(SpriteBatch batch, bool frontToBack)
            {
                _batch = batch;
                _frontToBack = frontToBack;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
            public int Compare(int x, int y)
            {
                float xDepth = _batch._spriteQueue[x].OrthoOrder;
                float yDepth = _batch._spriteQueue[y].OrthoOrder;

                if (_frontToBack)
                {
                    if (xDepth > yDepth)
                    {
                        return -1;
                    }
                    else if (xDepth < yDepth)
                    {
                        return 1;
                    }
                }
                else
                {
                    if (xDepth < yDepth)
                    {
                        return -1;
                    }
                    else if (xDepth > yDepth)
                    {
                        return 1;
                    }
                }

                return 0;
            }
        }
    }
}
