using System.Drawing;

namespace Common_ClassLibrary.Interfaces
{
    public interface INativeMethods
    {
        Color GetColorAtLocation(Point location);
        Point GetCursorLocation();
    }
}