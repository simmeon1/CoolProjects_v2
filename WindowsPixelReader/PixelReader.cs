using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsPixelReader
{
    public class PixelReader
    {
        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public Pixel GetPixelAtCursor()
        {
            return GetPixelAtLocation(GetCursorLocation());
        }
        
        public Pixel GetPixelAtLocation(int x, int y)
        {
            return GetPixelAtLocation(new Point(x, y));
        }

        public void SaveWindowRect(int x, int y, int width, int height, string path)
        {
            using (Bitmap bitmap = new(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(x,y), Point.Empty, new Size(width, height));
                }
                bitmap.Save(path, ImageFormat.Jpeg);
            }
        }
        
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        public Point GetClientToScreen(IntPtr hWnd)
        {
            Point p = new(0, 0);
            ClientToScreen(hWnd, ref p);
            return p;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
        
        public Rectangle GetWindowRect(IntPtr hWnd)
        {
            Rectangle r = new(0, 0 ,0, 0);
            GetWindowRect(hWnd, ref r);
            return r;
        }
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, ref Rectangle rect);
        public Rectangle GetClientRect(IntPtr hWnd)
        {
            Rectangle r = new(0, 0 ,0, 0);
            GetClientRect(hWnd, ref r);
            return r;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        
        public Pixel GetPixelAtLocation(Point location)
        {
            using Graphics gdest = Graphics.FromImage(screenPixel);
            using Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hSrcDC = gsrc.GetHdc();
            IntPtr hDC = gdest.GetHdc();
            int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();

            return new Pixel(location.X, location.Y, screenPixel.GetPixel(0, 0));
        }
        
        private static Point GetCursorLocation()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }
    }
}