namespace Spark.Graphics
{
    using System;
    
    using Content;
    using Graphics.Implementation;

    /// <summary>
    /// Represents a Cube-Dimensional texture (6 faces) where each face is a square 2D texture with width/height.
    /// </summary>
    public class TextureCube : Texture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        protected TextureCube()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        public TextureCube(IRenderSystem renderSystem, int size)
        {
            CreateImplementation(renderSystem, size, false, SurfaceFormat.Color, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public TextureCube(IRenderSystem renderSystem, int size, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, size, false, SurfaceFormat.Color, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        public TextureCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public TextureCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of faces (6) * number of mip map levels (in this case, each mip count is one). Data is assumed to be in the 
        /// order of the faces, where the entire mip map chain of the first face comes first, then the next face's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public TextureCube(IRenderSystem renderSystem, int size, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, size, false, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of faces (6) * number of mip map levels (in this case, each mip count is one). Data is assumed to be in the 
        /// order of the faces, where the entire mip map chain of the first face comes first, then the next face's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public TextureCube(IRenderSystem renderSystem, int size, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, size, false, format, resourceUsage, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of faces (6) * number of mip map levels. Data is assumed to be in the order of the faces, 
        /// where the entire mip map chain of the first face comes first, then the next face's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public TextureCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCube"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="size">Size (width/height) of the texture, in texels.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of faces (6) * number of mip map levels. Data is assumed to be in the order of the faces, 
        /// where the entire mip map chain of the first face comes first, then the next face's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public TextureCube(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, size, genMipMaps, format, resourceUsage, data);
        }

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public override SurfaceFormat Format => TextureCubeImpl.Format;

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public override int MipCount => TextureCubeImpl.MipCount;

        /// <summary>
        /// Gets the texture dimension, identifying the shape of the texture resoure.
        /// </summary>
        public override TextureDimension Dimension => TextureDimension.Cube;

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public override ResourceUsage ResourceUsage => TextureCubeImpl.ResourceUsage;

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public override ShaderResourceType ResourceType => ShaderResourceType.TextureCube;

        /// <summary>
        /// Gets the texture size, in texels.
        /// </summary>
        public int Size => TextureCubeImpl.Size;

        /// <summary>
        /// Gets the texture cube implementation
        /// </summary>
        private ITextureCubeImplementation TextureCubeImpl
        {
            get => Implementation as ITextureCubeImplementation;
            set => BindImplementation(value);
        }

        #region Public Methods

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="face">Face index to access.</param>
        public void GetData<T>(IDataBuffer<T> data, CubeMapFace face) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                TextureCubeImpl.GetData(data, face, 0, null, 0);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        public void GetData<T>(IDataBuffer<T> data, CubeMapFace face, ResourceRegion2D? subimage) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                TextureCubeImpl.GetData(data, face, 0, subimage, 0);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, CubeMapFace face, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                TextureCubeImpl.GetData(data, face, mipLevel, subimage, startIndex);
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
        /// <param name="face">Face index to access.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, 0, null, 0, DataWriteOptions.Discard);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, 0, null, 0, writeOptions);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face, ResourceRegion2D? subimage) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, 0, subimage, 0, DataWriteOptions.Discard);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face, ResourceRegion2D? subimage, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, 0, subimage, 0, writeOptions);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face, int mipLevel, ResourceRegion2D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, mipLevel, subimage, startIndex, DataWriteOptions.Discard);
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
        /// <param name="face">Face index to access.</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 2D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, CubeMapFace face, int mipLevel, ResourceRegion2D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                TextureCubeImpl.SetData(renderContext, data, face, mipLevel, subimage, startIndex, writeOptions);
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
            int size = input.ReadInt32();

            IDataBuffer<byte>[] dataArray = new IDataBuffer<byte>[6 * mipCount];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < mipCount; j++)
                {
                    dataArray[CalculateSubResourceIndex(i, j, mipCount)] = input.ReadArrayData<byte>();
                }
            }

            CreateImplementation(renderSystem, size, mipCount > 1, format, resourceUsage, dataArray);
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public override void Write(ISavableWriter output)
        {
            ITextureCubeImplementation impl = TextureCubeImpl;
            int mipCount = impl.MipCount;
            int size = impl.Size;
            SurfaceFormat format = impl.Format;

            output.Write("Name", Name);
            output.WriteEnum("Format", format);
            output.Write("MipCount", mipCount);
            output.WriteEnum("ResourceUsage", impl.ResourceUsage);
            output.Write("Size", size);
            
            for (int i = 0; i < 6; i++)
            {
                CubeMapFace face = (CubeMapFace)i;
                for (int j = 0; j < mipCount; j++)
                {
                    IDataBuffer<byte> byteBuffer = DataBuffer<byte>.Create(CalculateMipLevelSizeInBytes(j, size, size, format));

                    impl.GetData(byteBuffer, face, j, null, 0);
                    byteBuffer.Position = 0;

                    output.Write<byte>(string.Format("{0}, MipLevel{1}", face.ToString(), j.ToString()), byteBuffer);
                }
            }
        }

        #endregion

        #region Creation Parameter Validation

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="size">Size of the texture (width/height), in texels.</param>
        /// <param name="format">Format of the texture.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int size, SurfaceFormat format)
        {
            if (size <= 0 || size > adapter.MaximumTextureCubeSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Texture dimension is out of range");
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.Cube))
            {
                throw new SparkGraphicsException("Bad texture format");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="size">Size of the texture (width/height), in texels.</param>
        /// <param name="mipCount">Number of mip levels in the texture.</param>
        /// <param name="format">Format of the texture.</param>
        /// <param name="data">Initial data for mip levels.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int size, int mipCount, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            ValidateCreationParameters(adapter, size, format);

            if (data == null || data.Length == 0)
            {
                return;
            }

            if (data.Length != (6 * mipCount))
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Data array length not equal to subresource count");
            }

            for (int i = 0; i < 6; i++)
            {
                for (int mipLevel = 0; mipLevel < mipCount; mipLevel++)
                {
                    int subresourceIndex = CalculateSubResourceIndex(i, mipLevel, mipCount);
                    int mipSizeInBytes = CalculateMipLevelSizeInBytes(mipLevel, size, size, format); // Accounts for compressed texture size

                    IReadOnlyDataBuffer db = data[subresourceIndex];

                    if (db != null && (db.SizeInBytes > mipSizeInBytes))
                    {
                        throw new ArgumentOutOfRangeException(nameof(data), "Data size larger than mip level size");
                    }
                }
            }
        }

        #endregion

        #region Implementation Creation

        /// <summary>
        /// Creates the underlying implementation
        /// </summary>
        /// <param name="renderSystem">Render system</param>
        /// <param name="size">Size of a side of the texture cube</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (resourceUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Must supply data for immutable buffer");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(size, size) : 1;

            ValidateCreationParameters(renderSystem.Adapter, size, format);
            
            if (!renderSystem.TryGetImplementationFactory(out ITextureCubeImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                TextureCubeImpl = factory.CreateImplementation(size, mipLevels, format, resourceUsage);
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
        /// <param name="size">Size of a side of the texture cube</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        /// <param name="data">Initial data for the resource</param>
        private void CreateImplementation(IRenderSystem renderSystem, int size, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(size, size) : 1;

            ValidateCreationParameters(renderSystem.Adapter, size, mipLevels, format, data);
            
            if (!renderSystem.TryGetImplementationFactory(out ITextureCubeImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                TextureCubeImpl = factory.CreateImplementation(size, mipLevels, format, resourceUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        #endregion
    }
}
