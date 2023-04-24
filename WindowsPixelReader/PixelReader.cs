using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsPixelReader
{
    public class PixelReader
    {
        private readonly Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);
        public Pixel GetPixelAtCursor()
        {
            return GetPixelAtLocation(GetCursorLocation());
        }
        
        public Pixel GetPixelAtLocation(int x, int y)
        {
            return GetPixelAtLocation(new Point(x, y));
        }
        
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(
            IntPtr hDc,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hSrcDc,
            int xSrc,
            int ySrc,
            int dwRop
        );
        
        public Pixel GetPixelAtLocation(Point location)
        {
            using Graphics gdest = Graphics.FromImage(screenPixel);
            using Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hSrcDc = gsrc.GetHdc();
            IntPtr hDc = gdest.GetHdc();
            int retval = BitBlt(hDc, 0, 0, 1, 1, hSrcDc, location.X, location.Y, (int) CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();
            return new Pixel(location.X, location.Y, screenPixel.GetPixel(0, 0));
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Point lpPoint);
        
        private static Point GetCursorLocation()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }
    }
}