using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowsScreenReading;

public static class User32
{
    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);
    public static void RestoreWindow(string processName)
    {
        var handle = GetProcessHandle(processName);
        ShowWindowAsync(handle, 1);
        SetForegroundWindow(handle);
    }

    
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
    public static Rectangle GetWindowRect(IntPtr hWnd)
    {
        Rectangle r = new();
        GetWindowRect(hWnd, ref r);
        return r;
    }
        
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool GetClientRect(IntPtr hWnd, ref Rectangle rect);
    public static Rectangle GetClientRect(string processName)
    {
        Rectangle r = new();
        GetClientRect(GetProcessHandle(processName), ref r);
        return r;
    }
        
    [DllImport("user32.dll")]
    public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(ref Point lpPoint);
    public static Point GetCursorPos()
    {
        Point cursor = new();
        GetCursorPos(ref cursor);
        return cursor;
    }
        
    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);
    public static Point ClientToScreen(string processName, ref Point p)
    {
        ClientToScreen(GetProcessHandle(processName), ref p);
        return p;
    }

        
    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref Point point);
    public static Point ScreenToClient(string processName, ref Point p)
    {
        ScreenToClient(GetProcessHandle(processName), ref p);
        return p;
    }
        
    private static nint GetProcessHandle(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        Process process = processes.First();
        nint handle = process.MainWindowHandle;
        return handle;
    }
}