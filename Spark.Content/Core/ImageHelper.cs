using System.Drawing;

namespace Spark.Content
{
    using System.Drawing.Imaging;
    
    using Math;
    using Graphics;

    using SD = System.Drawing;
    using SD2D = System.Drawing.Drawing2D;

    /// <summary>
    /// Helper methods for loading images
    /// </summary>
    internal static class ImageHelper
    {
        /// <summary>
        /// Creates a mip chain for the input image
        /// </summary>
        /// <param name="mip0">Mip level 0 image data</param>
        /// <param name="mip0Width">Mip level 0 image width in pixels</param>
        /// <param name="mip0Height">Mip level 0 image height in pixels</param>
        /// <returns></returns>
        public static IDataBuffer<Color>[] GenerateMipChain(IDataBuffer<Color> mip0, int mip0Width, int mip0Height)
        {
            if (mip0 == null || mip0Width <= 0 || mip0Height <= 0)
            {
                return null;
            }

            int numMips = Texture.CalculateMipMapCount(mip0Width, mip0Height);
            var mipChain = new IDataBuffer<Color>[numMips];
            mipChain[0] = mip0;

            Bitmap mip0Image = ToBitmap(mip0, mip0Width, mip0Height);

            for (int i = 1; i < numMips; i++)
            {
                int newWidth = mip0Width;
                int newHeight = mip0Height;

                Texture.CalculateMipLevelDimensions(i, ref newWidth, ref newHeight);
                var image = new Bitmap(newWidth, newHeight);
                using (SD.Graphics g = SD.Graphics.FromImage(image))
                {
                    g.SmoothingMode = SD2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = SD2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = SD2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(mip0Image, new SD.Rectangle(0, 0, newWidth, newHeight));

                    mipChain[i] = FromBitmap(image);
                }
            }

            return mipChain;
        }
        
        /// <summary>
        /// Converts a data buffer to a bitmap
        /// </summary>
        /// <param name="data">Raw bitmap data</param>
        /// <param name="width">Bitmap width in pixels</param>
        /// <param name="height">Bitmap height in pixels</param>
        /// <returns>Bitmap initialized from the source color data</returns>
        private unsafe static Bitmap ToBitmap(IDataBuffer<Color> data, int width, int height)
        {
            if (data == null || width <= 0 || height <= 0)
            {
                return null;
            }

            var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData lockedData = image.LockBits(new SD.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int offset = (lockedData.Stride - (lockedData.Width * 4));

            var paddingOffset = (byte*)lockedData.Scan0.ToPointer();

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = data[index];
                    paddingOffset[2] = c.R;
                    paddingOffset[1] = c.G;
                    paddingOffset[0] = c.B;
                    paddingOffset[3] = c.A;

                    index++;
                    paddingOffset += 4;
                }

                paddingOffset += offset;
            }

            image.UnlockBits(lockedData);

            return image;
        }

        /// <summary>
        /// Gets a data buffer from a bitmap image
        /// </summary>
        /// <param name="data">Source bitmap</param>
        /// <returns>Buffered bitmap data</returns>
        private unsafe static IDataBuffer<Color> FromBitmap(Bitmap data)
        {
            if (data == null)
            {
                return null;
            }

            int width = data.Width;
            int height = data.Height;
            BitmapData lockedData = data.LockBits(new SD.Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var db = new DataBuffer<Color>(width * height);

            int paddingOffset = (lockedData.Stride - (lockedData.Width * 4));

            var pData = (byte*)lockedData.Scan0.ToPointer();

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = new Color(pData[2], pData[1], pData[0], pData[3]);
                    db[index] = c;

                    index++;
                    pData += 4;
                }

                pData += paddingOffset;
            }

            data.UnlockBits(lockedData);

            return db;
        }
    }
}
