namespace Spark.Graphics
{
    using System;
    
    using Content;
    using Graphics.Implementation;

    /// <summary>
    /// Represents an array of One-Dimensional texture resources, each with a width.
    /// </summary>
    public class Texture1DArray : Texture1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        protected Texture1DArray()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount)
        {
            CreateImplementation(renderSystem, width, arrayCount, false, SurfaceFormat.Color, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, width, arrayCount, false, SurfaceFormat.Color, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format)
        {
            CreateImplementation(renderSystem, width, arrayCount, genMipMaps, format, ResourceUsage.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            CreateImplementation(renderSystem, width, arrayCount, genMipMaps, format, resourceUsage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of array slices * number of mip map levels (in this case, each mip count is one). Data is assumed to be in the 
        /// order of the array slices, where the entire mip map chain of the first array slice comes first, then the next slice's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, arrayCount, false, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of array slices * number of mip map levels (in this case, each mip count is one). Data is assumed to be in the 
        /// order of the array slices, where the entire mip map chain of the first array slice comes first, then the next slice's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, arrayCount, false, format, resourceUsage, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of array slices * number of mip map levels. Data is assumed to be in the order of the array slices, 
        /// where the entire mip map chain of the first array slice comes first, then the next slice's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, arrayCount, genMipMaps, format, ResourceUsage.Static, data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DArray"/> class.
        /// </summary>
        /// <param name="renderSystem">Render system used to create the underlying implementation.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices, must be greater than zero.</param>
        /// <param name="genMipMaps">True to generate the entire mip map chain of the texture, false otherwise (only the first mip level is created).</param>
        /// <param name="format">Surface format of the texture.</param>
        /// <param name="resourceUsage">Resource usage specifying the type of memory the texture should use.</param>
        /// <param name="data">Data to initialize the texture with, the array length must not exceed the number of array slices * number of mip map levels. Data is assumed to be in the order of the array slices, 
        /// where the entire mip map chain of the first array slice comes first, then the next slice's mip map chain, and so on. Each data buffer must not exceed the size of the mip level, and is permitted to be null.</param>
        public Texture1DArray(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            CreateImplementation(renderSystem, width, arrayCount, genMipMaps, format, resourceUsage, data);
        }

        /// <summary>
        /// Gets the number of array slices in the texture. Slices may be indexed in the range [0, ArrayCount).
        /// </summary>
        public int ArrayCount => Texture1DArrayImplementation.ArrayCount;

        /// <summary>
        /// Gets the texture 1D array implementation
        /// </summary>
        private ITexture1DArrayImplementation Texture1DArrayImplementation
        {
            get => Implementation as ITexture1DArrayImplementation;
            set => BindImplementation(value);
        }

        #region Public Methods

        /// <summary>
        /// Reads data from the texture into the specified data buffer.
        /// </summary>
        /// <typeparam name="T">Type of data to read from the texture.</typeparam>
        /// <param name="data">Data buffer to hold contents copied from the texture.</param>
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        public void GetData<T>(IDataBuffer<T> data, int arraySlice) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture1DArrayImplementation.GetData(data, arraySlice, 0, null, 0);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to read from, if null the whole image is read from.</param>
        public void GetData<T>(IDataBuffer<T> data, int arraySlice, ResourceRegion1D? subimage) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture1DArrayImplementation.GetData(data, arraySlice, 0, subimage, 0);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to read from, if null the whole image is read from.</param>
        /// <param name="startIndex">Starting index in the data buffer to start writing to.</param>
        public void GetData<T>(IDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();

            try
            {
                Texture1DArrayImplementation.GetData(data, arraySlice, mipLevel, subimage, startIndex);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, 0, null, 0, DataWriteOptions.Discard);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, 0, null, 0, writeOptions);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, ResourceRegion1D? subimage) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, 0, subimage, 0, DataWriteOptions.Discard);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, ResourceRegion1D? subimage, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, 0, subimage, 0, writeOptions);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, mipLevel, subimage, startIndex, DataWriteOptions.Discard);
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
        /// <param name="arraySlice">Zero-based index specifying the array slice to access</param>
        /// <param name="mipLevel">Zero-based index specifying the mip map level to access.</param>
        /// <param name="subimage">The subimage region, in texels, of the 1D texture to write to, if null the whole image is written to.</param>
        /// <param name="startIndex">Starting index in the data buffer to start reading from.</param>
        /// <param name="writeOptions">Writing options, valid only for dynamic textures.</param>
        public void SetData<T>(IRenderContext renderContext, IReadOnlyDataBuffer<T> data, int arraySlice, int mipLevel, ResourceRegion1D? subimage, int startIndex, DataWriteOptions writeOptions) where T : struct
        {
            ThrowIfDisposed();
            ThrowIfDefferedSetDataIsNotPermitted(renderContext, ResourceUsage);

            try
            {
                Texture1DArrayImplementation.SetData(renderContext, data, arraySlice, mipLevel, subimage, startIndex, writeOptions);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error writing to resource", e);
            }
        }

        /// <summary>
        /// Gets a sub texture at the specified array index.
        /// </summary>
        /// <param name="arraySlice">Zero-based index of the sub texture.</param>
        /// <returns>The sub texture.</returns>
        public IShaderResource GetSubTexture(int arraySlice)
        {
            try
            {
                return Texture1DArrayImplementation.GetSubTexture(arraySlice);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Failed to retrieve sub texture from texture array");
            }
        }

        #endregion

        #region ISavable Methods

        /// <summary>
        /// Reads the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        public override void Read(ISavableReader input)
        {
            IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(input.ServiceProvider);

            string name = input.ReadString();
            SurfaceFormat format = input.ReadEnum<SurfaceFormat>();
            int mipCount = input.ReadInt32();
            ResourceUsage resourceUsage = input.ReadEnum<ResourceUsage>();
            int width = input.ReadInt32();
            int arrayCount = input.ReadInt32();

            IDataBuffer<byte>[] dataArray = new IDataBuffer<byte>[arrayCount * mipCount];
            for (int i = 0; i < arrayCount; i++)
            {
                for (int j = 0; j < mipCount; j++)
                {
                    dataArray[CalculateSubResourceIndex(i, j, mipCount)] = input.ReadArrayData<byte>();
                }
            }

            CreateImplementation(renderSystem, width, arrayCount, mipCount > 1, format, resourceUsage, dataArray);
        }

        /// <summary>
        /// Writes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        public override void Write(ISavableWriter output)
        {
            ITexture1DArrayImplementation impl = Texture1DArrayImplementation;
            int arrayCount = impl.ArrayCount;
            int mipCount = impl.MipCount;
            int width = impl.Width;
            SurfaceFormat format = impl.Format;

            output.Write("Name", Name);
            output.WriteEnum("Format", format);
            output.Write("MipCount", mipCount);
            output.WriteEnum("ResourceUsage", impl.ResourceUsage);
            output.Write("Width", width);
            output.Write("ArrayCount", arrayCount);
            
            for (int i = 0; i < arrayCount; i++)
            {
                for (int j = 0; j < mipCount; j++)
                {
                    IDataBuffer<byte> byteBuffer = DataBuffer<byte>.Create(CalculateMipLevelSizeInBytes(j, width, format));

                    impl.GetData(byteBuffer, i, j, null, 0);
                    byteBuffer.Position = 0;

                    output.Write<byte>(string.Format("Slice{0}, MipLevel{1}", i.ToString(), j.ToString()), byteBuffer);
                }
            }
        }

        #endregion

        #region Creation Parameter Validation

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices in the texture array.</param>
        /// <param name="format">Format of the texture.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int width, int arrayCount, SurfaceFormat format)
        {
            if (width <= 0 || width > adapter.MaximumTexture1DSize)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Texture dimension is out of range");
            }

            if (arrayCount <= 0 || arrayCount > adapter.MaximumTexture2DArrayCount)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayCount), "Texture count out of range");
            }

            if (!adapter.CheckTextureFormat(format, TextureDimension.One))
            {
                throw new SparkGraphicsException("Bad texture format");
            }
        }

        /// <summary>
        /// Validates creation parameters.
        /// </summary>
        /// <param name="adapter">Graphics adapter from the render system.</param>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="arrayCount">Number of array slices in the texture array.</param>
        /// <param name="mipCount">Number of mip levels in the texture.</param>
        /// <param name="format">Format of the texture.</param>
        /// <param name="data">Initial data for array slices and mip levels.</param>
        protected void ValidateCreationParameters(IGraphicsAdapter adapter, int width, int arrayCount, int mipCount, SurfaceFormat format, params IReadOnlyDataBuffer[] data)
        {
            ValidateCreationParameters(adapter, width, arrayCount, format);

            if (data == null || data.Length == 0)
            {
                return;
            }

            if (data.Length != (mipCount * arrayCount))
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Data array length not equal to subresource count");
            }

            for (int i = 0; i < arrayCount; i++)
            {
                for (int mipLevel = 0; mipLevel < mipCount; mipLevel++)
                {
                    int subresourceIndex = CalculateSubResourceIndex(i, mipLevel, mipCount);
                    int mipSizeInBytes = CalculateMipLevelSizeInBytes(mipLevel, width, format);

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
        /// <param name="width">Texture width</param>
        /// <param name="arrayCount">Array element count</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        private void CreateImplementation(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            if (resourceUsage == ResourceUsage.Immutable)
            {
                throw new SparkGraphicsException("Must supply data for immutable buffer");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(width) : 1;

            ValidateCreationParameters(renderSystem.Adapter, width, arrayCount, format);

            if (!renderSystem.TryGetImplementationFactory(out ITexture1DArrayImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                Texture1DArrayImplementation = factory.CreateImplementation(width, arrayCount, mipLevels, format, resourceUsage);
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
        /// <param name="arrayCount">Array element count</param>
        /// <param name="genMipMaps">Should mip maps be generated</param>
        /// <param name="format">Texture format</param>
        /// <param name="resourceUsage">Resource usage</param>
        /// <param name="data">Initial data for the resource</param>
        private void CreateImplementation(IRenderSystem renderSystem, int width, int arrayCount, bool genMipMaps, SurfaceFormat format, ResourceUsage resourceUsage, params IReadOnlyDataBuffer[] data)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem), "Render system cannot be null");
            }

            int mipLevels = (genMipMaps) ? CalculateMipMapCount(width) : 1;

            ValidateCreationParameters(renderSystem.Adapter, width, arrayCount, mipLevels, format, data);

            if (!renderSystem.TryGetImplementationFactory(out ITexture1DArrayImplementationFactory factory))
            {
                throw new SparkGraphicsException("Feature is not supported");
            }

            try
            {
                Texture1DArrayImplementation = factory.CreateImplementation(width, arrayCount, mipLevels, format, resourceUsage, data);
            }
            catch (Exception e)
            {
                throw new SparkGraphicsException("Error while creating implementation", e);
            }
        }

        #endregion
    }
}
