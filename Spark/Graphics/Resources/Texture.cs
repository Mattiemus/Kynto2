namespace Spark.Graphics
{
    using System;

    using Content;

    /// <summary>
    /// Common base class for all texture resources.
    /// </summary>
    public abstract class Texture : GraphicsResource, ISavable, IShaderResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        protected Texture()
        {
        }

        /// <summary>
        /// Gets the format of the texture resource.
        /// </summary>
        public abstract SurfaceFormat Format { get; }

        /// <summary>
        /// Gets the number of mip map levels in the texture resource. Mip levels may be indexed in the range of [0, MipCount).
        /// </summary>
        public abstract int MipCount { get; }

        /// <summary>
        /// Gets the texture dimension, identifying the shape of the texture resoure.
        /// </summary>
        public abstract TextureDimension Dimension { get; }

        /// <summary>
        /// Gets the resource usage of the texture.
        /// </summary>
        public abstract ResourceUsage ResourceUsage { get; }

        /// <summary>
        /// Gets the shader resource type.
        /// </summary>
        public abstract ShaderResourceType ResourceType { get; }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public abstract void Read(ISavableReader input);

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public abstract void Write(ISavableWriter output);

        /// <summary>
        /// Checks if the number is a power of two.
        /// </summary>
        /// <param name="num">Integer value</param>
        /// <returns>True if the number is a power of two, false otherwise.</returns>
        public static bool IsPowerOfTwo(int num)
        {
            return (num != 0) && ((num & (num - 1)) == 0);
        }

        /// <summary>
        /// Calculates the number of mip map levels for the given 1D width.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <returns>The number of mip map levels.</returns>
        public static int CalculateMipMapCount(int width)
        {
            return CalculateMipMapCount(width, 1, 1);
        }

        /// <summary>
        /// Calculates the number of mip map levels for the given 2D width/height.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <returns>The number of mip map levels.</returns>
        public static int CalculateMipMapCount(int width, int height)
        {
            return CalculateMipMapCount(width, height, 1);
        }

        /// <summary>
        /// Calculates the number of mip map levels for the given 3D width/height/depth.
        /// </summary>
        /// <param name="width">Width of the texture, in texels.</param>
        /// <param name="height">Height of the texture, in texels.</param>
        /// <param name="depth">Depth of the texture, in texels.</param>
        /// <returns>The number of mip map levels.</returns>
        public static int CalculateMipMapCount(int width, int height, int depth)
        {
            int max = Math.Max(Math.Max(width, height), depth);

            if (max == 0)
            {
                return 0;
            }

            return 1 + (int)Math.Floor((Math.Log(max) / Math.Log(2)));
        }

        /// <summary>
        /// Calculates the uncompressed width of the mip map level, given the mip level index and the width of the texture's
        /// first mip level.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level. This will hold the resulting width at the specified mip level.</param>
        public static void CalculateMipLevelDimensions(int mipLevel, ref int width)
        {
            int height = 1;
            int depth = 1;
            CalculateMipLevelDimensions(mipLevel, ref width, ref height, ref depth);
        }

        /// <summary>
        /// Calculates the uncompressed width/height of the mip map level, given the mip level index and the width/height of the texture's
        /// first mip level.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level. This will hold the resulting width at the specified mip level.</param>
        /// <param name="height">Height of the texture at the first mip level. This will hold the resulting height at the specified mip level.</param>
        public static void CalculateMipLevelDimensions(int mipLevel, ref int width, ref int height)
        {
            int depth = 1;
            CalculateMipLevelDimensions(mipLevel, ref width, ref height, ref depth);
        }

        /// <summary>
        /// Calculates the uncompressed width/height/depth of the mip map level, given the mip level index and the width/height/depth of the texture's
        /// first mip level.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level. This will hold the resulting width at the specified mip level.</param>
        /// <param name="height">Height of the texture at the first mip level. This will hold the resulting height at the specified mip level.</param>
        /// <param name="depth">Depth of the texture at the first mip level. This will hold the resulting depth at the specified mip level.</param>
        public static void CalculateMipLevelDimensions(int mipLevel, ref int width, ref int height, ref int depth)
        {
            if (mipLevel < 0)
            {
                return;
            }

            width = Math.Max(1, width >> mipLevel);
            height = Math.Max(1, height >> mipLevel);
            depth = Math.Max(1, depth >> mipLevel);
        }

        /// <summary>
        /// Calculates the size of the specified mip level in bytes.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level.</param>
        /// <param name="format">Format of the texture. 1D textures cannot be block compressed, so those are invalid formats.</param>
        /// <returns>The size of the mip level in bytes, if the calculation was not valid, zero is returned</returns>
        public static int CalculateMipLevelSizeInBytes(int mipLevel, int width, SurfaceFormat format)
        {
            if (mipLevel < 0 || format.IsCompressedFormat())
            {
                return 0;
            }

            CalculateMipLevelDimensions(mipLevel, ref width);
            return CalculateRegionSizeInBytes(format, ref width, out int formatSize);
        }

        /// <summary>
        /// Calculates the size of the specified mip level in bytes.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level. </param>
        /// <param name="height">Height of the texture at the first mip level. </param>
        /// <param name="format">Format of the texture.</param>
        /// <returns>The size of the mip level in bytes, if the calculation was not valid, zero is returned</returns>
        public static int CalculateMipLevelSizeInBytes(int mipLevel, int width, int height, SurfaceFormat format)
        {
            return CalculateMipLevelSizeInBytes(mipLevel, width, height, 1, format);
        }

        /// <summary>
        /// Calculates the size of the specified mip level in bytes.
        /// </summary>
        /// <param name="mipLevel">Zero-based index of the mip level.</param>
        /// <param name="width">Width of the texture at the first mip level.</param>
        /// <param name="height">Height of the texture at the first mip level.</param>
        /// <param name="depth">Depth of the texture at the first mip level.</param>
        /// <param name="format">Format of the texture.</param>
        /// <returns>The size of the mip level in bytes, if the calculation was not valid, zero is returned</returns>
        public static int CalculateMipLevelSizeInBytes(int mipLevel, int width, int height, int depth, SurfaceFormat format)
        {
            if (mipLevel < 0)
            {
                return 0;
            }

            CalculateMipLevelDimensions(mipLevel, ref width, ref height, ref depth);
            
            return CalculateRegionSizeInBytes(format, ref width, ref height, depth, out int formatSize);
        }

        /// <summary>
        /// Calculates the data size in bytes, given a subimage region.
        /// </summary>
        /// <param name="format">Format of the texture. 1D textures cannot be block compressed, so those are invalid formats.</param>
        /// <param name="width">Width of the subimage region.</param>
        /// <param name="formatSize">Holds the format size upon method return. If the surface format is compressed, this will be zero.</param>
        /// <returns>The size of the subimage region for the given format, in bytes. If the surface format is compressed, this will be zero.</returns>
        public static int CalculateRegionSizeInBytes(SurfaceFormat format, ref int width, out int formatSize)
        {
            if (format.IsCompressedFormat())
            {
                formatSize = 0;
                return 0;
            }

            formatSize = format.SizeInBytes();
            return width * formatSize;
        }

        /// <summary>
        /// Calculates the data size in bytes, given a subimage region. For compressed surface formats, the region is adjusted appropiately.
        /// </summary>
        /// <param name="format">Format of the texture.</param>
        /// <param name="width">Width of the subimage region. This will be modified if the format is compressed.</param>
        /// <param name="height">Height of the subimage region. This will be modified if the format is compressed.</param>
        /// <param name="formatSize">Holds the format size upon method return.</param>
        /// <returns>The size of the subimage region for the given format, in bytes.</returns>
        public static int CalculateRegionSizeInBytes(SurfaceFormat format, ref int width, ref int height, out int formatSize)
        {
            if (format.IsCompressedFormat())
            {
                return CalculateCompressedDimensions(format, ref width, ref height, 1, out formatSize);
            }
            else
            {
                formatSize = format.SizeInBytes();
                return width * height * format.SizeInBytes();
            }
        }

        /// <summary>
        /// Calculates the data size in bytes, given a subimage region. For compressed surface formats, the region is adjusted appropiately.
        /// </summary>
        /// <param name="format">Format of the texture.</param>
        /// <param name="width">Width of the subimage region. This will be modified if the format is compressed.</param>
        /// <param name="height">Height of the subimage region. This will be modified if the format is compressed.</param>
        /// <param name="depth">Depth of the subimage region.</param>
        /// <param name="formatSize">Holds the format size upon method return.</param>
        /// <returns>The size of the subimage region for the given format, in bytes.</returns>
        public static int CalculateRegionSizeInBytes(SurfaceFormat format, ref int width, ref int height, int depth, out int formatSize)
        {
            if (format.IsCompressedFormat())
            {
                return CalculateCompressedDimensions(format, ref width, ref height, depth, out formatSize);
            }
            else
            {
                formatSize = format.SizeInBytes();
                return width * height * depth * format.SizeInBytes();
            }
        }

        /// <summary>
        /// Calculates the compressed width/height of a texture given the format and uncompressed width/height.
        /// </summary>
        /// <param name="format">Compression format.</param>
        /// <param name="width">Uncompressed width of the texture. This will hold the resulting compressed width.</param>
        /// <param name="height">Uncompressed height of the texture. This will hold the resulting compressed height.</param>
        /// <param name="formatSize">The format size of the compression format, in bytes.</param>
        /// <returns>If the format was invalid, a value of zero is returned. Otherwise, the total compressed texture size, in bytes, is returned.</returns>
        public static int CalculateCompressedDimensions(SurfaceFormat format, ref int width, ref int height, out int formatSize)
        {
            return CalculateCompressedDimensions(format, ref width, ref height, 1, out formatSize);
        }

        /// <summary>
        /// Calculates the compressed width/height/depth of a texture given the format and uncompressed width/height.
        /// </summary>
        /// <param name="format">Compression format.</param>
        /// <param name="width">Uncompressed width of the texture. This will hold the resulting compressed width.</param>
        /// <param name="height">Uncompressed height of the texture. This will hold the resulting compressed height.</param>
        /// <param name="depth">Depth of the texture.</param>
        /// <param name="formatSize">The format size of the compression format, in bytes.</param>
        /// <returns>If the format was invalid, a value of zero is returned. Otherwise, the total compressed texture size, in bytes, is returned.</returns>
        public static int CalculateCompressedDimensions(SurfaceFormat format, ref int width, ref int height, int depth, out int formatSize)
        {
            formatSize = 0;

            if (format == SurfaceFormat.DXT1 || format == SurfaceFormat.DXT3 || format == SurfaceFormat.DXT5)
            {
                width = Math.Max(1, (width + 3) / 4);
                height = Math.Max(1, (height + 3) / 4);

                formatSize = (format == SurfaceFormat.DXT1) ? 8 : 16;

                return width * height * depth * formatSize;
            }

            return 0;
        }

        /// <summary>
        /// Calculates the sub resource index of a resource at the specified array and mip index, given the number of mip levels
        /// the resource contains.
        /// </summary>
        /// <param name="arraySlice">Zero-based array index.</param>
        /// <param name="mipSlice">Zero-based mip level index.</param>
        /// <param name="numMipLevels">Number of mip levels.</param>
        /// <returns>The calculated subresource index.</returns>
        public static int CalculateSubResourceIndex(int arraySlice, int mipSlice, int numMipLevels)
        {
            return (numMipLevels * arraySlice) + mipSlice;
        }

        /// <summary>
        /// Generate mipmaps for the texture.
        /// </summary>
        /// <param name="texture">Texture to generate mipmaps for.</param>
        /// <returns>True if mipmaps were generated, false otherwise.</returns>
        public static bool GenerateMipMaps(Texture texture)
        {
            if (texture == null)
            {
                return false;
            }

            IRenderSystem renderSystem = texture.RenderSystem;
            IGenerateMipMapsExtension genMipMaps = renderSystem.ImmediateContext.GetExtension<IGenerateMipMapsExtension>();

            if (genMipMaps == null)
            {
                return false;
            }

            return genMipMaps.GenerateMipMaps(texture);
        }
    }
}
