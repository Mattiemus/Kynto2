namespace Spark.Content.Importers.Texture
{
    using System;
    using System.IO;

    using Spark.Core;
    using Spark.Core.Interop;

    using Math;
    using Core;
    using Content;
    using Graphics;
    
    using SD = System.Drawing;

    /// <summary>
    /// 
    /// </summary>
    public class BitmapTextureImporter : ResourceImporter<Texture2D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapTextureImporter"/> class.
        /// </summary>
        public BitmapTextureImporter() 
            : base(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".tif")
        {
        }

        /// <summary>
        /// Loads content from the specified resource as the target runtime type.
        /// </summary>
        /// <param name="resourceFile">Resource file to read from</param>
        /// <param name="contentManager">Calling content manager</param>
        /// <param name="parameters">Optional loading parameters</param>
        /// <returns>The loaded object or null if it could not be loaded</returns>
        public override Texture2D Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            using (Stream str = resourceFile.OpenRead())
            {
                return LoadInternal(str, contentManager, parameters, resourceFile.Name);
            }
        }

        /// <summary>
        /// Loads content from the specified stream as the target runtime type.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="contentManager">Calling content manager.</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <returns>The loaded object or null if it could not be loaded.</returns>
        public override Texture2D Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            return LoadInternal(input, contentManager, parameters, "Texture");
        }

        /// <summary>
        /// Performs loading of a texture
        /// </summary>
        /// <param name="input">Stream to read from</param>
        /// <param name="contentManager">Calling content manger</param>
        /// <param name="parameters">Optional loading parameters.</param>
        /// <param name="texName">Name of the texture being loaded</param>
        /// <returns>The loaded texture or null if it could not be loaded</returns>
        private Texture2D LoadInternal(Stream input, ContentManager contentManager, ImporterParameters parameters, String texName)
        {
            using (SD.Bitmap bitmap = new SD.Bitmap(input))
            {
                TextureImporterParameters textureParams = parameters as TextureImporterParameters;

                if(QueryFlip(textureParams))
                {
                    bitmap.RotateFlip(SD.RotateFlipType.RotateNoneFlipY);
                }

                byte[] data = GetData(bitmap);
                SD.Imaging.PixelFormat pixelFormat = bitmap.PixelFormat;
                Color[] colors = null;

                switch (pixelFormat)
                {
                    case SD.Imaging.PixelFormat.Format32bppArgb:
                        colors = GetRGBA(bitmap, data);
                        break;
                    case SD.Imaging.PixelFormat.Format24bppRgb:
                        colors = GetRGB(bitmap, data);
                        break;
                    case SD.Imaging.PixelFormat.Format8bppIndexed:
                        colors = GetIndexed(bitmap, data);
                        break;
                    default:
                        throw new InvalidOperationException("Cannot read from that format encoding");
                }
                
                IRenderSystem renderSystem = GraphicsHelper.GetRenderSystem(contentManager.ServiceProvider);

                Texture2D tex = null;
                if (QueryGenerateMipMaps(textureParams))
                {
                    tex = new Texture2D(renderSystem, bitmap.Width, bitmap.Height, true, SurfaceFormat.Color, ImageHelper.GenerateMipChain(new DataBuffer<Color>(colors), bitmap.Width, bitmap.Height));
                }
                else
                {
                    tex = new Texture2D(renderSystem, bitmap.Width, bitmap.Height, false, SurfaceFormat.Color, new DataBuffer<Color>(colors));
                }

                tex.Name = texName;

                return tex;
            }
        }

        /// <summary>
        /// Determines if the image being loaded should be flipped
        /// </summary>
        /// <param name="importParams">Importer parameters</param>
        /// <returns>True if the image should be flipped, false otherwise</returns>
        private bool QueryFlip(TextureImporterParameters importParams)
        {
            if (importParams == null)
            {
                return false;
            }

            return importParams.FlipImage;
        }

        /// <summary>
        /// Determines if mipmaps should be generated for the image being loaded
        /// </summary>
        /// <param name="importParams">Importer parameters</param>
        /// <returns>True if mipmaps should be generated, false otherwise</returns>
        private bool QueryGenerateMipMaps(TextureImporterParameters importParams)
        {
            if (importParams == null)
            {
                return false;
            }

            return importParams.GenerateMipMaps;
        }

        /// <summary>
        /// Gets a bitmap as a sequence of binary data
        /// </summary>
        /// <param name="bitmap">Bitmap to get the data from</param>
        /// <returns>Raw binary data representing the bitmap</returns>
        private static byte[] GetData(SD.Bitmap bitmap)
        {
            int formatSize = SD.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int stride = bitmap.Width * formatSize;
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] bytes = new byte[stride * height];

            IntPtr ptr = MemoryHelper.PinObject(bytes);
            SD.Imaging.BitmapData data = new SD.Imaging.BitmapData
            {
                Width = width,
                Height = height,
                Stride = stride,
                Scan0 = ptr
            };

            bitmap.LockBits(new SD.Rectangle(0, 0, width, height), SD.Imaging.ImageLockMode.ReadOnly | SD.Imaging.ImageLockMode.UserInputBuffer, bitmap.PixelFormat, data);
            bitmap.UnlockBits(data);
            MemoryHelper.UnpinObject(bytes);

            return bytes;
        }

        /// <summary>
        /// Gets a bitmap as an array of colors where the source bitmap is a RGB image
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="data">Source data</param>
        /// <returns>Bitmap as a flat array of colors</returns>
        private static Color[] GetRGB(SD.Bitmap bitmap, byte[] data)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int totalSize = width * height;
            Color[] colors = new Color[totalSize];

            for (int i = 0, index = 0; i < totalSize; i++, index += 3)
            {
                byte b = data[index];
                byte g = data[index + 1];
                byte r = data[index + 2];
                colors[i] = new Color(r, g, b);
            }

            return colors;
        }

        /// <summary>
        /// Gets a bitmap as an array of colors where the source bitmap is a RGBA image
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="data">Source data</param>
        /// <returns>Bitmap as a flat array of colors</returns>
        private static Color[] GetRGBA(SD.Bitmap bitmap, byte[] data)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int totalSize = width * height;
            Color[] colors = new Color[totalSize];

            for (int i = 0, index = 0; i < totalSize; i++, index += 4)
            {
                byte b = data[index];
                byte g = data[index + 1];
                byte r = data[index + 2];
                byte a = data[index + 3];
                colors[i] = new Color(r, g, b, a);
            }

            return colors;
        }

        /// <summary>
        /// Gets a bitmap as an array of colors where the source bitmap is a palettized bitmap
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="data">Source data</param>
        /// <returns>Bitmap as a flat array of colors</returns>
        private static Color[] GetIndexed(SD.Bitmap bitmap, byte[] data)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int totalSize = width * height;
            SD.Imaging.ColorPalette palette = bitmap.Palette;
            Color[] colors = new Color[totalSize];

            for (int i = 0; i < totalSize; i++)
            {
                int index = data[i];
                SD.Color c = palette.Entries[index];
                colors[i] = new Color(c.R, c.G, c.B, c.A);
            }

            return colors;
        }
    }
}
