using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Common_ClassLibrary.Interfaces;

namespace ViGEm_Gui
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
        
        // [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        // private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        //
        // [DllImport("user32.dll")]
        // private static extern IntPtr GetDC(IntPtr hwnd);
        //
        // [DllImport("user32.dll")]
        // private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        //
        // [DllImport("gdi32.dll")]
        // private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        
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
        
        // public IntPtr FindWindowByCaption(string caption)
        // {
        //     return FindWindowByCaption(IntPtr.Zero, caption);
        // }
        
        // public Color GetColorAtWindowLocation(IntPtr hwnd, int x, int y)
        // {
        //     // IntPtr hdc = GetDC(hwnd);
        //     // uint pixel = GetPixel(hdc, x, y);
        //     // ReleaseDC(hwnd, hdc);
        //     // Color color = Color.FromArgb((int)(pixel & 0x000000FF),
        //     //     (int)(pixel & 0x0000FF00) >> 8,
        //     //     (int)(pixel & 0x00FF0000) >> 16);
        //     // return color;
        //     
        //     // IntPtr hdc = GetDC(hwnd);
        //     // uint pixel = GetPixel(hdc, x, y);
        //     // ReleaseDC(IntPtr.Zero,hdc);
        //     // Color color = Color.FromArgb((int)pixel);
        //     // return color;
        //     
        //     using Graphics gdest = Graphics.FromImage(screenPixel);
        //     using Graphics gsrc = Graphics.FromHwnd(hwnd);
        //     IntPtr hSrcDC = gsrc.GetHdc();
        //     IntPtr hDC = gdest.GetHdc();
        //     int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, x, y, (int) CopyPixelOperation.SourceCopy);
        //     gdest.ReleaseHdc();
        //     gsrc.ReleaseHdc();
        //     return screenPixel.GetPixel(0, 0);
        // }

        public Point GetCursorPosition()
        {
            Point cursor = new();
            GetCursorPos(ref cursor);
            return cursor;
        }
    }
}