using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsPixelReader
{
    public class PixelReader
    {
        public Pixel GetPixelAtCursor()
        {
            Point cursorLocation = GetCursorLocation();
            return GetPixelAtLocation(cursorLocation.X, cursorLocation.Y);
        }

        public Pixel GetPixelAtLocation(int x, int y)
        {
            return new Pixel(x, y, GetScreenAverageColor(x, y, 1, 1));
        }

        public void SaveScreen(int x, int y, int width, int height, string path)
        {
            DoActionWithBitmap(
                x,
                y,
                width,
                height,
                bitmap => bitmap.Save(path, GetImageFormatFromPath(path))
            );
        }
        
        public Color GetScreenAverageColor(int x, int y, int width, int height)
        {
            Color result = default;
            DoActionWithBitmap(
                x,
                y,
                width,
                height,
                bitmap => { result = GetAverageColor(bitmap); }
            );
            return result;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);

        public Rectangle GetWindowRect(IntPtr hWnd)
        {
            Rectangle r = new(0, 0, 0, 0);
            GetWindowRect(hWnd, ref r);
            return r;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        public Point GetCursorLocation()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        public Point GetClientToScreen(IntPtr hWnd, ref Point p)
        {
            ClientToScreen(hWnd, ref p);
            return p;
        }
        
        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref Point point);

        public Point GetScreenToClient(IntPtr hWnd, ref Point point)
        {
            ScreenToClient(hWnd, ref point);
            return point;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, ref Rectangle rect);

        public Rectangle GetClientRect(IntPtr hWnd)
        {
            Rectangle r = new(0, 0, 0, 0);
            GetClientRect(hWnd, ref r);
            return r;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        public void SaveClient(Rectangle clientRect, Point clientPoint, string path)
        {
            using Bitmap clientBitmap = new(clientRect.Width, clientRect.Height);
            using (Graphics g = Graphics.FromImage(clientBitmap))
            {
                g.CopyFromScreen(clientPoint, Point.Empty, new Size(clientBitmap.Width, clientBitmap.Height));
            }
            clientBitmap.Save(path, GetImageFormatFromPath(path));
        }

        private static void DoActionWithBitmap(int x, int y, int width, int height, Action<Bitmap> action)
        {
            using (Bitmap bitmap = new(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(x, y), Point.Empty, new Size(width, height));
                }
                action.Invoke(bitmap);
            }
        }

        private static Color GetAverageColor(Bitmap bm)
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
        
        private static ImageFormat GetImageFormatFromPath(string path)
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