using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsScreenReading
{
    public class BitmapWorker
    {
        // Laptop screen
        public T ProcessBitmap<T>(
            int startX,
            int startY,
            int endX,
            int endY,
            Func<Bitmap, T> bitmapFn,
            string? clientName = null
        ) {
            using (Bitmap img = new(endX - startX, endY - startY))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    var screenPoint = new Point(startX, startY);
                    if (clientName != null)
                    {
                        User32.ClientToScreen(clientName, ref screenPoint);
                    }
                    g.CopyFromScreen(new Point(screenPoint.X, screenPoint.Y), Point.Empty, new Size(img.Width, img.Height));
                    // img.Save("tessTest.png", ImageFormat.Png);
                }
                return bitmapFn(img);
            }
        }
        
        public T ProcessBitmap<T>(int startX, int startY, Func<Bitmap, T> bitmapFn, string? clientName = null) =>
            ProcessBitmap(startX, startY, startX + 1, startY + 1, bitmapFn, clientName);
        
        public T ProcessBitmap<T>(string clientName, Func<Bitmap, T> bitmapFn)
        {
            var rect = User32.GetClientRect(clientName);
            return ProcessBitmap(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, bitmapFn, clientName);
        }

        public Color GetAverageColor(Bitmap bm)
        {
            BitmapData srcData = bm.LockBits(
                new Rectangle(0, 0, bm.Width, bm.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;
            int[] totals = {0, 0, 0};
            int width = bm.Width;
            int height = bm.Height;
            unsafe
            {
                byte* p = (byte*) (void*) Scan0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * 4 + color;
                            totals[color] += p[idx];
                        }
                    }
                }
            }

            int avgR = totals[2] / (width * height);
            int avgG = totals[1] / (width * height);
            int avgB = totals[0] / (width * height);
            return Color.FromArgb(avgR, avgG, avgB);
        }

        public static ImageFormat GetImageFormatFromPath(string path)
        {
            ImageFormat imageFormat = ImageFormat.Jpeg;
            string extension = Path.GetExtension(path).ToUpper();
            if (extension is ".JPG" or ".JPEG") imageFormat = ImageFormat.Jpeg;
            else if (extension == ".BMP") imageFormat = ImageFormat.Bmp;
            else if (extension == ".PNG") imageFormat = ImageFormat.Png;
            else throw new Exception("Could not determine image format");
            return imageFormat;
        }
    }
}