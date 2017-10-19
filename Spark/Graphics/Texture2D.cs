namespace Spark.Graphics
{
    using System;

    using Core;
    using Content;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a Two-Dimensional texture resource that has width and height.
    /// </summary>
    public class Texture2D : Texture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        protected Texture2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height)
        {
            CreateImplementation(renderSystem, width, height, false, SurfaceFormat.Color, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, width, height, false, SurfaceFormat.Color, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format)
        {
            CreateImplementation(renderSystem, width, height, genMipMaps, format, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, width, height, genMipMaps, format, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture's first mip level with, the data buffer must not exceed the size of the mip level and is permitted to be null.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, SurfaceFormat format, IReadOnlyDataBuffer data)
        {
            CreateImplementation(renderSystem, width, height, false, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture's first mip level with, the data buffer must not exceed the size of the mip level and is permitted to be null.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, SurfaceFormat format, ResourceUsage resourceUsage, IReadOnlyDataBuffer data)
        {
            CreateImplementation(renderSystem, width, height, false, format, resourceUsage, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, height, genMipMaps, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2D"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of mip levels and each data buffer must not exceed the size of the corresponding mip level, and is
        /// permitted to be null.</param>
        public Texture2D(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, height, genMipMaps, format, resourceUsage, data);
        }

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public override SurfaceFormat Format => Texture2DImplementation.Format;

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public override int MipCount => Texture2DImplementation.MipCount;

        /// <summary>
        /// Gets the texture dimension, identifying the shape of the texture resoure.
        /// </summary>
        public override TextureDimension Dimension => TextureDimension.Two;

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public override ResourceUsage ResourceUsage => Texture2DImplementation.ResourceUsage;

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public override ShaderResourceType ResourceType => ShaderResourceType.Texture2D;

        /// <summary>
        /// Gets the texture width, in texels.
        /// </summary>
        public int Width => Texture2DImplementation.Width;

        /// <summary>
        /// Gets the texture height, in texels.
        /// </summary>
        public int Height => Texture2DImplementation.Height;

        /// <summary>
        /// Gets the texture 2D implementation
        /// </summary>
        private ITexture2DImplementation Texture2DImplementation
        {
            get => Implementation as ITexture2DImplementation;
            set => BindImplementation(value);
        }

        #region Public Methods

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        public void GetData<T>(IDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture2DImplementation.GetData(data, 0, null, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        public void GetData<T>(IDataBuffer<T> data, ResourceRegion2D? subimage) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture2DImplementation.GetData(data, 0, subimage, 0);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture2DImplementation.GetData(data, mipLevel, subimage, startIndex);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error reading from resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, 0, null, 0, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, 0, null, 0, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, ResourceRegion2D? subimage) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, 0, subimage, 0, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, ResourceRegion2D? subimage, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, 0, subimage, 0, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param> 
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, mipLevel, subimage, startIndex, DataWriteOptions.Discard);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Writes data to the texture from the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to write to the texture.</typeparam>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="data">Data buffer that holds the contents that are to be copied to the texture.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int mipLevel, ResourceRegion2D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture2DImplementation.SetData(renderContext, data, mipLevel, subimage, startIndex, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        #endregion

        #region ISavable Methods

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public override void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);

            string name = input.ReadString();
            SurfaceFormat format = input.ReadEnum<SurfaceFormat>();
            int mipCount = input.ReadInt32();
            ResourceUsage resourceUsage = input.ReadEnum<ResourceUsage>();
            int width = input.ReadInt32();
            int height = input.ReadInt32();

            IDataBuffer<byte>[] dataArray = new IDataBuffer<byte>[mipCount];
            for (int i = 0; i < mipCount; i++)
            {
                dataArray[i] = input.ReadArrayData<byte>();
            }

            CreateImplementation(renderSystem, width, height, mipCount > 1, format, resourceUsage, dataArray);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            ITexture2DImplementation impl = Texture2DImplementation;
            int mipCount = impl.MipCount;
            int width = impl.Width;
            int height = impl.Height;
            SurfaceFormat format = impl.Format;
            int formatSize = format.SizeInBytes();

            output.Write("Name", Name);
            output.WriteEnum("Format", format);
            output.Write("MipCount", mipCount);
            output.WriteEnum("ResourceUsage", impl.ResourceUsage);
            output.Write("Width", width);
            output.Write("Height", height);
            
            for (int i = 0; i < mipCount; i++)
            {
                IDataBuffer<byte> byteBuffer = DataBuffer<byte>.Create(CalculateMipLevelSizeInBytes(i, width, height, format));

                impl.GetData(byteBuffer, i, null, 0);
                byteBuffer.Position = 0;

                output.Write<byte>(string.Format("MipLevel{0}", i.ToString()), byteBuffer);
            }
        }

        #endregion

        #region Creation Parameter Validation

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="format">Format of the texture.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int width, int height, SurfaceFormat format)
        {
            int maxSize = adapter.MaximumTexture2DSize;

            if (width <= 0 || width > maxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Texture dimension is out of range");
            }

            if (height <= 0 || height > maxSize)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Texture dimension is out of range");
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Two))
            {
                throw new SparkGraphicsException("Bad texture format");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="mipCount">Number of mip levels in the texture.</param>
        /// <param name="format">Format of the texture.</param>
        /// <param name="data">Initial data for mip levels.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int width, int height, int mipCount, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            ValidateCreationParameters(adapter, width, height, format);

            if (data == null || data.Length == 0)
            {
                return;
            }

            if (data.Length != mipCount)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Data array length not equal to subresource count");
            }

            for (int mipLevel = 0; mipLevel < data.Length; mipLevel++)
            {
                int mipSizeInBytes = CalculateMipLevelSizeInBytes(mipLevel, width, height, format); // Accounts for compressed texture size

                IReadOnlyDataBuffer db = data[mipLevel];

                if (db != null && (db.SizeInBytes > mipSizeInBytes))
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Data size larger than mip level size");
                }
            }
        }

        #endregion

        #region Implementation Creation

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (resourceUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Must supply data for immutable buffer");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(width, height) : 1;

            ValidateCreationParameters(renderSystem.Adapter, width, height, format);
            
            if (!renderSystem.TryGetImplementationFactory(out ITexture2DImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                Texture2DImplementation = factory.CreateImplementation(width, height, mipLevels, format, resourceUsage);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        /// <param name="data">Initial data for the resource</param>
        private void CreateImplementation(IRenderSystem renderSystem, int width, int height, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(width, height) : 1;

            ValidateCreationParameters(renderSystem.Adapter, width, height, mipLevels, format, data);
            
            if (!renderSystem.TryGetImplementationFactory(out ITexture2DImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                Texture2DImplementation = factory.CreateImplementation(width, height, mipLevels, format, resourceUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        #endregion
    }
}
