using System.Diagnostics;
using System.Drawing;

namespace WindowsScreenReading
{
    public class BitmapWorker
    {
        // Laptop screen
        public T ProcessBitmap<T>(
            string processName,
            int clientStartX,
            int clientStartY,
            int clientEndX,
            int clientEndY,
            Func<Bitmap, T> bitmapFn
        ) {
            nint handle = GetProcessHandle(processName);
            // Rectangle rect = User32.GetClientRect(handle);
            // using (Bitmap image = new(clientRect.Width, clientRect.Height))
            using (Bitmap img = new(clientEndX - clientStartX, clientEndY - clientStartY))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    var screenPoint = new Point(clientStartX, clientStartY);
                    User32.ClientToScreen(handle, ref screenPoint);
                    g.CopyFromScreen(new Point(screenPoint.X, screenPoint.Y), Point.Empty, new Size(img.Width, img.Height));
                    // img.Save("tessTest.png", ImageFormat.Png);
                }
                return bitmapFn(img);
            }
        }
        
        private static nint GetProcessHandle(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            Process process = processes.First();
            nint handle = process.MainWindowHandle;
            return handle;
        }

    }
}