using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Common_ClassLibrary.Interfaces;

namespace LeagueGui
{
    public class WindowsNativeMethods : INativeMethods
    {
        
        private readonly Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(
            IntPtr hDC,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hSrcDC,
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
            IntPtr hSrcDC = gsrc.GetHdc();
            IntPtr hDC = gdest.GetHdc();
            int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int) CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();
            return screenPixel.GetPixel(0, 0);
        }

        public Point GetCursorPosition()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }
    }
}