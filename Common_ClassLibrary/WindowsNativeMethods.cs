using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Common_ClassLibrary.Interfaces;

namespace Common_ClassLibrary
{
    public class WindowsNativeMethods : INativeMethods
    {
        private readonly Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);

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

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Point lpPoint);

        public Color GetColorAtLocation(Point location)
        {
            using Graphics gdest = Graphics.FromImage(screenPixel);
            using Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hSrcDc = gsrc.GetHdc();
            IntPtr hDc = gdest.GetHdc();
            int retval = BitBlt(hDc, 0, 0, 1, 1, hSrcDc, location.X, location.Y, (int) CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();
            return screenPixel.GetPixel(0, 0);
        }

        public Point GetCursorLocation()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }
    }
}