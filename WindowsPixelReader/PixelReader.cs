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