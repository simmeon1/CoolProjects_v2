// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Runtime.InteropServices;
// namespace WindowsScreenReading
// {
//     public class PixelReader
//     {
//         private readonly BitmapWorker bitmapWorker;
//
//         public PixelReader()
//         {
//             bitmapWorker = new BitmapWorker();
//         }
//         
//         public Pixel GetPixelAtCursor(string? clientName = null)
//         {
//             Point cursorLocation = User32.GetCursorPos();
//             return GetPixelAtLocation(cursorLocation.X, cursorLocation.Y, clientName);
//         }
//         
//         public Pixel GetPixelAtLocation(int x, int y, string? clientName = null)
//         {
//             return new Pixel(x, y, GetScreenAverageColor(x, y, 1, 1, clientName));
//         }
//         
//         public void SaveScreen(int x, int y, int width, int height, string path)
//         {
//             bitmapWorker.ProcessBitmap(
//                 null,
//                 x,
//                 y,
//                 x + width,
//                 y + height,
//                 bitmap =>
//                 {
//                     bitmap.Save(path, GetImageFormatFromPath(path));
//                     return true;
//                 }
//             );
//         }
//         
//         public Color GetScreenAverageColor(int x, int y, int width, int height, string? clientName = null)
//         {
//             return bitmapWorker.ProcessBitmap(
//                 clientName,
//                 x,
//                 y,
//                 x + width,
//                 y + height,
//                 GetAverageColor
//             );
//         }
//         
//         private static Color GetAverageColor(Bitmap bm)
//         {
//             BitmapData srcData = bm.LockBits(
//                 new Rectangle(0, 0, bm.Width, bm.Height),
//                 ImageLockMode.ReadOnly,
//                 PixelFormat.Format32bppArgb
//             );
//             int stride = srcData.Stride;
//             IntPtr Scan0 = srcData.Scan0;
//             int[] totals = {0, 0, 0};
//             int width = bm.Width;
//             int height = bm.Height;
//             unsafe
//             {
//                 byte* p = (byte*) (void*) Scan0;
//                 for (int y = 0; y < height; y++)
//                 {
//                     for (int x = 0; x < width; x++)
//                     {
//                         for (int color = 0; color < 3; color++)
//                         {
//                             int idx = (y * stride) + x * 4 + color;
//                             totals[color] += p[idx];
//                         }
//                     }
//                 }
//             }
//             int avgR = totals[2] / (width * height);
//             int avgG = totals[1] / (width * height);
//             int avgB = totals[0] / (width * height);
//             return Color.FromArgb(avgR, avgG, avgB);
//         }
//         
//         private static ImageFormat GetImageFormatFromPath(string path)
//         {
//             ImageFormat imageFormat = ImageFormat.Jpeg;
//             string extension = Path.GetExtension(path).ToUpper();
//             if (extension is ".JPG" or ".JPEG") imageFormat = ImageFormat.Jpeg;
//             else if (extension == ".BMP") imageFormat = ImageFormat.Bmp;
//             else if (extension == ".PNG") imageFormat = ImageFormat.Png;
//             else throw new Exception("Could not determine image format");
//             return imageFormat;
//         }
//     }
// }